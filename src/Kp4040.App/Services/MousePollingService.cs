using System.Runtime.InteropServices;
using System.Threading;

namespace Kp4040.App.Services;

/// <summary>
/// Atualiza o mouse sem usar o DispatcherTimer da janela WPF.
/// Nao modifica o driver: le XInput e envia eventos de mouse do Windows.
/// </summary>
public sealed class MousePollingService : IDisposable
{
    private readonly XInputReader _reader = new();
    private readonly AnalogMouseController _mouse = new();
    private readonly Thread _thread;
    private volatile bool _enabled;
    private volatile bool _stopping;

    // Aumenta a resolucao do Sleep para o processo enquanto o mouse analogico esta ligado.
    // O Windows ainda pode variar o tempo real de agendamento.
    [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
    private static extern uint TimeBeginPeriod(uint period);

    [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
    private static extern uint TimeEndPeriod(uint period);

    public MousePollingService()
    {
        _thread = new Thread(Poll)
        {
            Name = "KP4040-Mouse",
            IsBackground = true
        };
        _thread.Start();
    }

    public void SetEnabled(bool enabled) => _enabled = enabled;

    private void Poll()
    {
        bool wasActive = false;
        bool highResolutionEnabled = false;
        uint? previousIndex = null;

        try
        {
            while (!_stopping)
            {
                if (!_enabled)
                {
                    if (wasActive) _mouse.Reset();
                    wasActive = false;
                    previousIndex = null;
                    if (highResolutionEnabled)
                    {
                        TimeEndPeriod(1);
                        highResolutionEnabled = false;
                    }
                    Thread.Sleep(35);
                    continue;
                }

                // Solicitamos melhor precisao para Thread.Sleep(8) somente durante o uso.
                if (!highResolutionEnabled)
                    highResolutionEnabled = TimeBeginPeriod(1) == 0;

                try
                {
                    var snapshot = _reader.ReadFirstConnected();
                    if (snapshot is null)
                    {
                        if (wasActive) _mouse.Reset();
                        wasActive = false;
                        previousIndex = null;
                    }
                    else
                    {
                        if (!wasActive || previousIndex != snapshot.UserIndex)
                            _mouse.Reset();

                        if (_enabled && !_stopping)
                            _mouse.Update(snapshot);

                        previousIndex = snapshot.UserIndex;
                        wasActive = true;
                    }
                }
                catch (DllNotFoundException)
                {
                    _enabled = false;
                }
                catch (EntryPointNotFoundException)
                {
                    _enabled = false;
                }

                // Alvo de 8ms (~125 Hz), nao promessa de frequencia exata.
                Thread.Sleep(8);
            }
        }
        finally
        {
            _mouse.Reset();
            if (highResolutionEnabled) TimeEndPeriod(1);
        }
    }

    public void Dispose()
    {
        _enabled = false;
        _stopping = true;
        if (Thread.CurrentThread != _thread)
            _thread.Join(TimeSpan.FromSeconds(2));
    }
}
