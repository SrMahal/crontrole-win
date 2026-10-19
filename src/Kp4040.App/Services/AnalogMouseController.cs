using System.Diagnostics;
using System.Runtime.InteropServices;
using Kp4040.App.Models;

namespace Kp4040.App.Services;

// Transforma os analogicos em movimentos de mouse no Windows.
// Nao altera os sinais originais do controle e nao instala drivers.
public sealed class AnalogMouseController
{
    private const double DeadZone = 0.20;
    private const double MaxMouseSpeed = 1000.0;  // pixels por segundo
    private const double MaxScrollSpeed = 7.0;    // passos por segundo
    private const int WheelDelta = 120;

    private const uint InputMouse = 0;
    private const uint MouseMove = 0x0001;
    private const uint MouseWheel = 0x0800;
    private const uint MouseHWheel = 0x1000;

    private readonly Stopwatch _watch = Stopwatch.StartNew();
    private double _lastSeconds;
    private double _pixelRemainderX;
    private double _pixelRemainderY;
    private double _scrollRemainderV;
    private double _scrollRemainderH;

    [StructLayout(LayoutKind.Sequential)]
    private struct MouseInput
    {
        public int Dx;
        public int Dy;
        public uint MouseData;
        public uint Flags;
        public uint Time;
        public UIntPtr ExtraInfo;
    }

    // Para INPUT do tipo mouse, a union nativa tem o layout de MOUSEINPUT.
    [StructLayout(LayoutKind.Sequential)]
    private struct Input
    {
        public uint Type;
        public MouseInput Mouse;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint count, Input[] inputs, int size);

    public void Reset()
    {
        _lastSeconds = _watch.Elapsed.TotalSeconds;
        _pixelRemainderX = _pixelRemainderY = 0;
        _scrollRemainderV = _scrollRemainderH = 0;
    }

    public void Update(ControllerSnapshot state)
    {
        double now = _watch.Elapsed.TotalSeconds;
        // Evita saltos apos pausas no sistema ou na interface.
        double elapsed = Math.Clamp(now - _lastSeconds, 0, 0.10);
        _lastSeconds = now;
        if (elapsed <= 0) return;

        // XInput: Y positivo = cima. Windows: delta Y negativo = cima.
        double x = Normalize(state.LX);
        double y = -Normalize(state.LY);
        Move(x, y, elapsed);

        // Stick direito: cima/baixo faz scroll vertical, lados fazem horizontal.
        Scroll(ref _scrollRemainderV, Normalize(state.RY), elapsed, MouseWheel);
        Scroll(ref _scrollRemainderH, Normalize(state.RX), elapsed, MouseHWheel);
    }

    private static double Normalize(short value)
    {
        double normalized = value < 0 ? value / 32768.0 : value / 32767.0;
        double magnitude = Math.Abs(normalized);
        if (magnitude <= DeadZone) return 0;

        double adjusted = (magnitude - DeadZone) / (1.0 - DeadZone);
        return Math.Sign(normalized) * adjusted * adjusted;
    }

    private void Move(double x, double y, double elapsed)
    {
        if (x == 0) _pixelRemainderX = 0;
        else _pixelRemainderX += x * MaxMouseSpeed * elapsed;

        if (y == 0) _pixelRemainderY = 0;
        else _pixelRemainderY += y * MaxMouseSpeed * elapsed;

        int dx = (int)Math.Truncate(_pixelRemainderX);
        int dy = (int)Math.Truncate(_pixelRemainderY);
        if (dx == 0 && dy == 0) return;

        if (SendMouse(dx, dy, 0, MouseMove))
        {
            _pixelRemainderX -= dx;
            _pixelRemainderY -= dy;
        }
        else
        {
            _pixelRemainderX = _pixelRemainderY = 0;
        }
    }

    private static void Scroll(ref double remainder, double direction, double elapsed, uint flag)
    {
        if (direction == 0)
        {
            remainder = 0;
            return;
        }

        remainder += direction * MaxScrollSpeed * elapsed;
        int steps = (int)Math.Truncate(remainder);
        if (steps == 0) return;

        if (SendMouse(0, 0, unchecked((uint)(steps * WheelDelta)), flag))
            remainder -= steps;
        else
            remainder = 0;
    }

    private static bool SendMouse(int dx, int dy, uint data, uint flags)
    {
        var inputs = new[]
        {
            new Input
            {
                Type = InputMouse,
                Mouse = new MouseInput { Dx = dx, Dy = dy, MouseData = data, Flags = flags }
            }
        };
        return SendInput(1, inputs, Marshal.SizeOf<Input>()) == 1;
    }
}
