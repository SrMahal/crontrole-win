using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Kp4040.App.Models;
using Kp4040.App.Services;

namespace Kp4040.App;

public partial class MainWindow : Window
{
    private readonly ControllerPollingService _poller = new();
    private readonly ActionExecutor _executor = new();
    private readonly MousePollingService _mouseWorker = new();
    private readonly ProfileStore _store = new();
    private readonly DispatcherTimer _timer;
    private ControllerProfile _profile;
    private ControllerSnapshot? _lastSnapshot;
    private bool _canAutosave = true;
    private bool _identifyNextPress;
    private ButtonMappingRow? _captureShortcutRow;
    private bool _pendingAltOnly;
    private bool _editingShortcut;
    private readonly HashSet<string> _heldAltButtons = new();
    private readonly HashSet<string> _heldLeftMouseButtons = new();

    public ObservableCollection<ButtonMappingRow> Bindings { get; } = new();

    public MainWindow()
    {
        InitializeComponent();

        try
        {
            _profile = _store.Load();
        }
        catch (Exception ex)
        {
            _canAutosave = false;
            _profile = ControllerProfile.CreateDefault();
            MessageBox.Show(
                "Nao foi possivel abrir o perfil local. Seu arquivo existente NAO sera sobrescrito automaticamente.\n\n" +
                ex.Message + "\n\nPara substituir o arquivo conscientemente, clique em 'Salvar perfil'.",
                "Problema com perfil", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        foreach (var button in ControllerButtons.All)
        {
            _profile.Shortcuts.TryGetValue(button.Key, out var shortcut);
            var row = new ButtonMappingRow(button, _profile.Mappings[button.Key], shortcut);
            row.MappingChanged += (_, _) =>
            {
                ReleaseHeldAlt(row.Descriptor.Key);
                ReleaseHeldLeftMouse(row.Descriptor.Key);
                _profile.Mappings[row.Descriptor.Key] = row.SelectedAction;
                if (row.Shortcut is not null)
                    _profile.Shortcuts[row.Descriptor.Key] = row.Shortcut;
                else
                    _profile.Shortcuts.Remove(row.Descriptor.Key);
                if (_canAutosave) SaveProfile();
            };
            Bindings.Add(row);
        }

        _poller.SetTriggerThreshold(_profile.TriggerThreshold);

        DataContext = this;
        ProfileStatusText.Text = _canAutosave
            ? $"Perfil: {_store.ProfilePath}"
            : "Perfil em memoria: corrija o arquivo ou salve manualmente";

        ActionsEnabledCheck.Checked += (_, _) => ResetArmingBaseline();
        ActionsEnabledCheck.Unchecked += (_, _) => ResetArmingBaseline();
        AnalogMouseEnabledCheck.Checked += (_, _) => SyncMouseState();
        AnalogMouseEnabledCheck.Unchecked += (_, _) => SyncMouseState();
        ResetArmingBaseline();

        PreviewKeyDown += OnPreviewKeyDown;
        PreviewKeyUp += OnPreviewKeyUp;
        _timer = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = TimeSpan.FromMilliseconds(45)
        };
        _timer.Tick += OnTick;
        _timer.Start();
    }

    private void ResetArmingBaseline()
    {
        ReleaseAllHeldInputs();
        _poller.ResetEvents();
        SyncMouseState();
        LastActionText.Text = ActionsEnabledCheck.IsChecked == true
            ? "Ações habilitadas. Use outra janela para atalhos."
            : "Ações desativadas";
    }

    private void IdentifyButton_Click(object sender, RoutedEventArgs e)
    {
        ReleaseAllHeldInputs();
        if (_identifyNextPress)
        {
            _identifyNextPress = false;
            IdentifyButton.Content = "Identificar botão";
            IdentifyStatusText.Text = "Identificação cancelada.";
            SyncMouseState();
            return;
        }

        _captureShortcutRow = null;
        _identifyNextPress = true;
        IdentifyButton.Content = "Cancelar identificação";
        IdentifyStatusText.Text = "Aguardando: pressione um botão do controle (não mova os sticks).";
        _poller.ResetEvents();
        SyncMouseState();
    }

    private void Identify(ButtonEvent inputEvent)
    {
        _identifyNextPress = false;
        IdentifyButton.Content = "Identificar botão";
        var row = Bindings.FirstOrDefault(r => r.Descriptor.Key == inputEvent.Key);
        if (row is null) return;
        foreach (var item in Bindings) item.IsIdentified = false;
        row.IsIdentified = true;
        MappingList.SelectedItem = row;
        MappingList.ScrollIntoView(row);
        IdentifyStatusText.Text = $"Identificado: {inputEvent.Label}. Agora escolha a ação na linha destacada.";
        _poller.ResetEvents();
        SyncMouseState();
    }

    private void CaptureShortcut_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { DataContext: ButtonMappingRow row }) return;
        ReleaseAllHeldInputs();
        _identifyNextPress = false;
        IdentifyButton.Content = "Identificar botão";
        _captureShortcutRow = row;
        _pendingAltOnly = false;
        foreach (var item in Bindings) item.IsIdentified = item == row;
        MappingList.SelectedItem = row;
        MappingList.ScrollIntoView(row);
        IdentifyStatusText.Text = $"Gravando para {row.Label}: pressione uma tecla/combinação ou pressione e solte Alt (Esc cancela).";
        SyncMouseState();
        Focus();
    }

    private void EditShortcut_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { DataContext: ButtonMappingRow row }) return;
        ReleaseAllHeldInputs();
        _captureShortcutRow = null;
        _pendingAltOnly = false;
        _identifyNextPress = false;
        IdentifyButton.Content = "Identificar botão";
        _poller.ResetEvents();
        _editingShortcut = true;
        SyncMouseState();
        try
        {
            var editor = new ShortcutEditorWindow(row.Shortcut) { Owner = this };
            if (editor.ShowDialog() == true && editor.Result is not null)
            {
                row.Shortcut = editor.Result;
                row.SelectedAction = ActionKind.CustomKeyboard;
                IdentifyStatusText.Text = $"{row.Label}: {row.Shortcut.Label}. Use outro aplicativo para testar.";
            }
        }
        finally
        {
            _editingShortcut = false;
            _poller.ResetEvents();
            SyncMouseState();
        }
    }

    private void RemoveShortcut_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { DataContext: ButtonMappingRow row }) return;
        ReleaseAllHeldInputs();
        _captureShortcutRow = null;
        _pendingAltOnly = false;
        _poller.ResetEvents();
        row.RemoveShortcut();
        IdentifyStatusText.Text = $"Atalho de {row.Label} removido.";
        SyncMouseState();
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (_captureShortcutRow is null) return;
        e.Handled = true;
        Key key = e.Key == Key.System ? e.SystemKey : e.Key;
        var mods = Keyboard.Modifiers;
        bool ctrl = mods.HasFlag(ModifierKeys.Control);
        bool alt = mods.HasFlag(ModifierKeys.Alt);
        bool shift = mods.HasFlag(ModifierKeys.Shift);
        bool win = Keyboard.IsKeyDown(Key.LWin) || Keyboard.IsKeyDown(Key.RWin);

        if (key is Key.LeftAlt or Key.RightAlt)
        {
            _pendingAltOnly = !ctrl && !shift && !win;
            IdentifyStatusText.Text = "Solte Alt para gravá-lo sozinho ou pressione outra tecla para uma combinação.";
            return;
        }
        _pendingAltOnly = false;

        if (key == Key.Escape && !ctrl && !alt && !shift && !win)
        {
            _captureShortcutRow = null;
            IdentifyStatusText.Text = "Gravação cancelada. Para mapear Esc, use o preset Teclado · Esc.";
            SyncMouseState();
            return;
        }

        if (key is Key.LeftCtrl or Key.RightCtrl or Key.LeftShift or Key.RightShift
            or Key.LeftAlt or Key.RightAlt or Key.LWin or Key.RWin or Key.System)
        {
            IdentifyStatusText.Text = "Continue: pressione uma tecla junto ao modificador.";
            return;
        }

        int virtualKey = KeyInterop.VirtualKeyFromKey(key);
        if (virtualKey is < 1 or > 255)
        {
            IdentifyStatusText.Text = "Tecla não reconhecida. Tente outra combinação.";
            return;
        }

        // Alguns atalhos Win são reservados pelo sistema e não chegam ao app.
        var shortcut = new KeyboardShortcut
        {
            VirtualKey = virtualKey,
            Control = ctrl,
            Alt = alt,
            Shift = shift,
            Windows = win,
            KeyLabel = key.ToString()
        };
        var row = _captureShortcutRow;
        _captureShortcutRow = null;
        SyncMouseState();
        row.Shortcut = shortcut;
        IdentifyStatusText.Text = $"{row.Label} configurado: {shortcut.Label}. Clique fora da janela para testar.";
    }

    private void OnPreviewKeyUp(object sender, KeyEventArgs e)
    {
        if (_captureShortcutRow is null) return;
        e.Handled = true;
        Key key = e.Key == Key.System ? e.SystemKey : e.Key;
        if (key is not (Key.LeftAlt or Key.RightAlt) || !_pendingAltOnly) return;

        var row = _captureShortcutRow;
        _captureShortcutRow = null;
        _pendingAltOnly = false;
        row.Shortcut = new KeyboardShortcut { VirtualKey = 0x12, KeyLabel = "Alt" };
        IdentifyStatusText.Text = $"{row.Label} configurado: Alt. Clique fora da janela para testar.";
        SyncMouseState();
    }

    private void OnTick(object? sender, EventArgs e)
    {
        var snapshot = _poller.LatestSnapshot;
        if (_poller.Error is string error)
        {
            ReleaseAllHeldInputs();
            _timer.Stop();
            _mouseWorker.SetEnabled(false);
            ConnectionText.Text = error;
            ConnectionDetailText.Text = "Verifique o XInput 1.4 no Windows.";
            return;
        }

        if (snapshot is null)
        {
            ReleaseAllHeldInputs();
            ConnectionText.Text = "Nenhum controle XInput conectado";
            ConnectionDetailText.Text = "Conecte um controle compatível com XInput.";
            if (_lastSnapshot is not null)
                LastEventText.Text = "Controle desconectado";
            _lastSnapshot = null;
            foreach (var row in Bindings) row.IsPressed = false;
            TriggersText.Text = "LT: — | RT: —";
            LeftStickText.Text = "— / —";
            RightStickText.Text = "— / —";
            return;
        }

        ConnectionText.Text = "Controle conectado";
        ConnectionDetailText.Text = $"XInput 1.4 · índice {snapshot.UserIndex}";
        TriggersText.Text = $"LT: {snapshot.LeftTrigger} | RT: {snapshot.RightTrigger}";
        LeftStickText.Text = $"{snapshot.LX} / {snapshot.LY}";
        RightStickText.Text = $"{snapshot.RX} / {snapshot.RY}";

        if (_lastSnapshot is not null && _lastSnapshot.UserIndex != snapshot.UserIndex)
        {
            ReleaseAllHeldInputs();
            _poller.ResetEvents();
            LastEventText.Text = $"Dispositivo alterado: índice {snapshot.UserIndex}";
        }

        // Libera Alt mesmo se o evento de soltura tiver envelhecido na fila.
        foreach (var key in _heldAltButtons.ToArray())
        {
            var button = ControllerButtons.All.First(b => b.Key == key);
            if (!ControllerButtons.IsPressed(button, snapshot, _profile.TriggerThreshold))
                ReleaseHeldAlt(key);
        }
        foreach (var key in _heldLeftMouseButtons.ToArray())
        {
            var button = ControllerButtons.All.First(b => b.Key == key);
            if (!ControllerButtons.IsPressed(button, snapshot, _profile.TriggerThreshold))
                ReleaseHeldLeftMouse(key);
        }

        var events = _poller.DrainEvents();
        _lastSnapshot = snapshot;

        foreach (var row in Bindings)
            row.IsPressed = ControllerButtons.IsPressed(row.Descriptor, snapshot, _profile.TriggerThreshold);

        foreach (var captured in events)
        {
            // Nao executar comandos antigos depois de um congelamento da interface.
            if (DateTime.UtcNow - captured.CapturedUtc > TimeSpan.FromMilliseconds(300))
                continue;
            var inputEvent = captured.Event;
            LastEventText.Text = $"{inputEvent.Label}: " +
                                 (inputEvent.IsPressed ? "pressionado" : "solto");
            if (!inputEvent.IsPressed)
            {
                ReleaseHeldAlt(inputEvent.Key);
                ReleaseHeldLeftMouse(inputEvent.Key);
                continue;
            }

            if (_identifyNextPress)
            {
                Identify(inputEvent);
                return; // nunca executa ação durante a identificação
            }
            if (_captureShortcutRow is not null || _editingShortcut)
                continue;
            if (ActionsEnabledCheck.IsChecked != true)
            {
                LastActionText.Text = $"{inputEvent.Label}: ações desligadas";
                continue;
            }
            if (!_profile.Mappings.TryGetValue(inputEvent.Key, out var action) || action == ActionKind.None)
            {
                LastActionText.Text = $"{inputEvent.Label}: nenhuma ação configurada";
                continue;
            }
            _profile.Shortcuts.TryGetValue(inputEvent.Key, out var shortcut);
            if (IsAltHoldMapping(action, shortcut))
            {
                if (_heldAltButtons.Count > 0 || _executor.SetAltHeld(true))
                {
                    _heldAltButtons.Add(inputEvent.Key);
                    LastActionText.Text = $"{inputEvent.Label}: Alt pressionado até soltar o botão";
                }
                else
                    LastActionText.Text = $"{inputEvent.Label}: falha ao pressionar Alt";
                continue;
            }
            if (action == ActionKind.MouseLeft)
            {
                if (_heldLeftMouseButtons.Count > 0 || _executor.SetLeftMouseHeld(true))
                {
                    _heldLeftMouseButtons.Add(inputEvent.Key);
                    LastActionText.Text = $"{inputEvent.Label}: clique pressionado até soltar o botão";
                }
                else
                    LastActionText.Text = $"{inputEvent.Label}: falha ao pressionar clique esquerdo";
                continue;
            }

            // Permite Tab com Alt segurado mesmo quando o configurador tem foco.
            bool appHasFocus = IsActive && !(action == ActionKind.Tab && _heldAltButtons.Count > 0);
            LastActionText.Text = $"{inputEvent.Label}: {_executor.Execute(action, appHasFocus, shortcut)}";
        }
    }

    private static bool IsAltHoldMapping(ActionKind action, KeyboardShortcut? shortcut) =>
        action == ActionKind.Alt ||
        action == ActionKind.CustomKeyboard && shortcut is
            { VirtualKey: 0x12, Alt: false, Control: false, Shift: false, Windows: false };

    private void ReleaseHeldAlt(string key)
    {
        if (!_heldAltButtons.Remove(key) || _heldAltButtons.Count > 0) return;
        _executor.SetAltHeld(false);
    }

    private void ReleaseHeldLeftMouse(string key)
    {
        if (!_heldLeftMouseButtons.Remove(key) || _heldLeftMouseButtons.Count > 0) return;
        _executor.SetLeftMouseHeld(false);
    }

    private void ReleaseAllHeldInputs()
    {
        if (_heldAltButtons.Count > 0)
        {
            _heldAltButtons.Clear();
            _executor.SetAltHeld(false);
        }
        if (_heldLeftMouseButtons.Count > 0)
        {
            _heldLeftMouseButtons.Clear();
            _executor.SetLeftMouseHeld(false);
        }
    }

    private void SaveProfileButton_Click(object sender, RoutedEventArgs e) => SaveProfile();

    private void SaveProfile()
    {
        try
        {
            _store.Save(_profile);
            _canAutosave = true;
            ProfileStatusText.Text = $"Perfil salvo: {_store.ProfilePath}";
        }
        catch (Exception ex)
        {
            ProfileStatusText.Text = $"Erro ao salvar: {ex.Message}";
        }
    }

    // O cursor roda em uma thread separada; a UI fica responsavel apenas pela configuracao.
    private void SyncMouseState() => _mouseWorker.SetEnabled(
        ActionsEnabledCheck.IsChecked == true &&
        AnalogMouseEnabledCheck.IsChecked == true &&
        !_identifyNextPress && _captureShortcutRow is null && !_editingShortcut);

    protected override void OnClosed(EventArgs e)
    {
        ReleaseAllHeldInputs();
        _timer.Stop();
        _mouseWorker.Dispose();
        _poller.Dispose();
        base.OnClosed(e);
    }
}
