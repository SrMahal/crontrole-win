using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using Kp4040.App.Models;
using Kp4040.App.Services;

namespace Kp4040.App;

public partial class MainWindow : Window
{
    private readonly XInputReader _reader = new();
    private readonly InputMonitor _monitor = new();
    private readonly ActionExecutor _executor = new();
    private readonly ProfileStore _store = new();
    private readonly DispatcherTimer _timer;
    private ControllerProfile _profile;
    private ControllerSnapshot? _lastSnapshot;
    private bool _canAutosave = true;

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
            var row = new ButtonMappingRow(button, _profile.Mappings[button.Key]);
            row.MappingChanged += (_, _) =>
            {
                _profile.Mappings[row.Descriptor.Key] = row.SelectedAction;
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

        _timer = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = TimeSpan.FromMilliseconds(45)
        };
        _timer.Tick += OnTick;
        _timer.Start();
    }

    private void ResetArmingBaseline()
    {
        // Evita disparo inesperado se o usuario ativa acoes segurando algum botao.
        _monitor.AlignTo(_lastSnapshot, _profile.TriggerThreshold);
        LastActionText.Text = ActionsEnabledCheck.IsChecked == true
            ? "Ações habilitadas. Use outra janela para atalhos."
            : "Ações desativadas";
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

        // Ao conectar, assume o estado atual como base para evitar acoes inesperadas.
        if (_lastSnapshot is null)
            _monitor.AlignTo(snapshot, _profile.TriggerThreshold);

        // Trocas de indice nao devem misturar estados de controles diferentes.
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

            if (!inputEvent.IsPressed || ActionsEnabledCheck.IsChecked != true) continue;
            if (!_profile.Mappings.TryGetValue(inputEvent.Key, out var action) || action == ActionKind.None)
                continue;

            LastActionText.Text = $"{inputEvent.Label}: {_executor.Execute(action, IsActive)}";
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

    protected override void OnClosed(EventArgs e)
    {
        _timer.Stop();
        base.OnClosed(e);
    }
}
