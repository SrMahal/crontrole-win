using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Threading;
using Kp4040.App.Models;

namespace Kp4040.App.Services;

// Captura pressionamentos fora da thread WPF para nao perder toques entre frames da interface.
public sealed class ControllerPollingService : IDisposable
{
    public sealed record TimedEvent(ButtonEvent Event, DateTime CapturedUtc);

    private readonly XInputReader _reader = new();
    private readonly InputMonitor _monitor = new();
    private readonly ConcurrentQueue<TimedEvent> _queue = new();
    private readonly object _gate = new();
    private readonly Thread _thread;
    private ControllerSnapshot? _latest;
    private string? _error;
    private volatile bool _stopping;
    private int _triggerThreshold = 180;

    [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
    private static extern uint TimeBeginPeriod(uint period);

    [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
    private static extern uint TimeEndPeriod(uint period);

    public ControllerPollingService()
    {
        _thread = new Thread(Poll)
        {
            IsBackground = true,
            Name = "Crontrole-Buttons"
        };
        _thread.Start();
    }

    public ControllerSnapshot? LatestSnapshot
    {
        get { lock (_gate) return _latest; }
    }

    public string? Error
    {
        get { lock (_gate) return _error; }
    }

    // A variavel permanece na thread worker: a UI apenas atualiza a configuracao.
    public void SetTriggerThreshold(byte threshold)
    {
        lock (_gate)
        {
            _triggerThreshold = threshold;
            _monitor.AlignTo(_latest, threshold);
            ClearQueue();
        }
    }

    public void ResetEvents()
    {
        lock (_gate)
        {
            ClearQueue();
            _monitor.AlignTo(_latest, (byte)_triggerThreshold);
        }
    }

    public IReadOnlyList<TimedEvent> DrainEvents()
    {
        var items = new List<TimedEvent>();
        while (_queue.TryDequeue(out var item)) items.Add(item);
        return items;
    }

    private void ClearQueue()
    {
        while (_queue.TryDequeue(out _)) { }
    }

    private void Poll()
    {
        // Solicita maior precisao de agendamento enquanto o monitor funciona.
        // A frequencia real depende do Windows e do hardware.
        bool highRes = TimeBeginPeriod(1) == 0;
        try
        {
            while (!_stopping)
            {
                ControllerSnapshot? snapshot;
                try
                {
                    snapshot = _reader.ReadFirstConnected();
                }
                catch (DllNotFoundException)
                {
                    lock (_gate) _error = "XInput 1.4 indisponível";
                    break;
                }
                catch (EntryPointNotFoundException)
                {
                    lock (_gate) _error = "XInputGetState não encontrado";
                    break;
                }

                lock (_gate)
                {
                    byte threshold = (byte)_triggerThreshold;
                    if (snapshot is null)
                    {
                        _latest = null;
                        _monitor.AlignTo(null, threshold);
                        ClearQueue(); // eventos antigos nao devem disparar apos desconexao
                    }
                    else
                    {
                        if (_latest is null || _latest.UserIndex != snapshot.UserIndex)
                        {
                            _monitor.AlignTo(snapshot, threshold);
                            ClearQueue();
                        }

                        foreach (var item in _monitor.Process(snapshot, threshold))
                        {
                            while (_queue.Count >= 128) _queue.TryDequeue(out _);
                            _queue.Enqueue(new TimedEvent(item, DateTime.UtcNow));
                        }
                        _latest = snapshot;
                    }
                }
                Thread.Sleep(8);
            }
        }
        finally
        {
            if (highRes) TimeEndPeriod(1);
        }
    }

    public void Dispose()
    {
        _stopping = true;
        if (Thread.CurrentThread != _thread)
            _thread.Join(TimeSpan.FromSeconds(2));
    }
}
