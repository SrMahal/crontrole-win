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
    private readonly XInputReader _reader = new();
    private readonly InputMonitor _monitor = new();
    private readonly ActionExecutor _executor = new();
    private readonly MousePollingService _mouseWorker = new();
    private readonly ProfileStore _store = new();
    private readonly DispatcherTimer _timer;
    private ControllerProfile _profile;
    private ControllerSnapshot? _lastSnapshot;
    private bool _canAutosave = true;
    private bool _identifyNextPress;
    private ButtonMappingRow? _captureShortcutRow;

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
                _profile.Mappings[row.Descriptor.Key] = row.SelectedAction;
                if (row.Shortcut is not null)
                    _profile.Shortcuts[row.Descriptor.Key] = row.Shortcut;
                else
                    _profile.Shortcuts.Remove(row.Descriptor.Key);
                if (_canAutosave) SaveProfile();
            };
            Bindings.Add(row);
        }

        DataContext = this;
        ProfileStatusText.Text = _canAutosave
            ? $"Perfil: {_store.ProfilePath}"
            : "Perfil em memoria: corrija o arquivo ou salve manualmente";

        ActionsEnabledCheck.Checked += (_, _) => ResetArmingBaseline();
        ActionsEnabledCheck.Unchecked += (_, _) => ResetArmingBaseline();
        AnalogMouseEnabledCheck.Checked += (_, _) => SyncMouseState();
        AnalogMouseEnabledCheck.Unchecked += (_, _) => SyncMouseState();

        PreviewKeyDown += OnPreviewKeyDown;
        _timer = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = TimeSpan.FromMilliseconds(45)
        };
        _timer.Tick += OnTick;
        _timer.Start();
    }

    private void ResetArmingBaseline()
    {
        _monitor.AlignTo(_lastSnapshot, _profile.TriggerThreshold);
        SyncMouseState();
        LastActionText.Text = ActionsEnabledCheck.IsChecked == true
            ? "Ações habilitadas. Use outra janela para atalhos."
            : "Ações desativadas";
    }

    private void IdentifyButton_Click(object sender, RoutedEventArgs e)
    {
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
        IdentifyStatusText.Text = "Aguardando: pressione um botão do KP-4040 (não mova os sticks).";
        _monitor.AlignTo(_lastSnapshot, _profile.TriggerThreshold);
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
        _monitor.AlignTo(_lastSnapshot, _profile.TriggerThreshold);
        SyncMouseState();
    }

    private void CaptureShortcut_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { DataContext: ButtonMappingRow row }) return;
        _identifyNextPress = false;
        IdentifyButton.Content = "Identificar botão";
        _captureShortcutRow = row;
        foreach (var item in Bindings) item.IsIdentified = item == row;
        MappingList.SelectedItem = row;
        MappingList.ScrollIntoView(row);
        IdentifyStatusText.Text = $"Gravando para {row.Label}: pressione a tecla/combinação no teclado (Esc cancela).";
        SyncMouseState();
        Focus();
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

    private void OnTick(object? sender, EventArgs e)
    {
        ControllerSnapshot? snapshot;
        try
        {
            snapshot = _reader.ReadFirstConnected();
        }
        catch (DllNotFoundException)
        {
            _timer.Stop();
            _mouseWorker.SetEnabled(false);
            ConnectionText.Text = "XInput 1.4 indisponível";
            ConnectionDetailText.Text = "Esta versão exige Windows 8+ / Windows 11.";
            return;
        }

        if (snapshot is null)
        {
            ConnectionText.Text = "Nenhum controle XInput conectado";
            ConnectionDetailText.Text = "Conecte o KP-4040 via USB.";
            if (_lastSnapshot is not null)
                LastEventText.Text = "Controle desconectado";
            _lastSnapshot = null;
            _monitor.AlignTo(null, _profile.TriggerThreshold);
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

        if (_lastSnapshot is null)
            _monitor.AlignTo(snapshot, _profile.TriggerThreshold);
        if (_lastSnapshot is not null && _lastSnapshot.UserIndex != snapshot.UserIndex)
        {
            _monitor.AlignTo(snapshot, _profile.TriggerThreshold);
            LastEventText.Text = $"Dispositivo alterado: índice {snapshot.UserIndex}";
        }

        var events = _monitor.Process(snapshot, _profile.TriggerThreshold);
        _lastSnapshot = snapshot;

        foreach (var row in Bindings)
            row.IsPressed = ControllerButtons.IsPressed(row.Descriptor, snapshot, _profile.TriggerThreshold);

        foreach (var inputEvent in events)
        {
            LastEventText.Text = $"{inputEvent.Label}: " +
                                 (inputEvent.IsPressed ? "pressionado" : "solto");
            if (!inputEvent.IsPressed) continue;

            if (_identifyNextPress)
            {
                Identify(inputEvent);
                return; // nunca executa ação durante a identificação
            }
            if (_captureShortcutRow is not null || ActionsEnabledCheck.IsChecked != true)
                continue;
            if (!_profile.Mappings.TryGetValue(inputEvent.Key, out var action) || action == ActionKind.None)
                continue;
            _profile.Shortcuts.TryGetValue(inputEvent.Key, out var shortcut);
            LastActionText.Text = $"{inputEvent.Label}: {_executor.Execute(action, IsActive, shortcut)}";
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
        !_identifyNextPress && _captureShortcutRow is null);

    protected override void OnClosed(EventArgs e)
    {
        _timer.Stop();
        _mouseWorker.Dispose();
        base.OnClosed(e);
    }
}
