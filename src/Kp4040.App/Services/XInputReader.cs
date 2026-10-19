using System.Runtime.InteropServices;
using Kp4040.App.Models;

namespace Kp4040.App.Services;

// Somente leitura: nao registra nem substitui nenhum driver do Windows.
public sealed class XInputReader
{
    [StructLayout(LayoutKind.Sequential)]
    private struct XInputGamepad
    {
        public ushort Buttons;
        public byte LeftTrigger;
        public byte RightTrigger;
        public short LX;
        public short LY;
        public short RX;
        public short RY;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct XInputState
    {
        public uint PacketNumber;
        public XInputGamepad Gamepad;
    }

    [DllImport("xinput1_4.dll", EntryPoint = "XInputGetState")]
    private static extern uint XInputGetState(uint userIndex, out XInputState state);

    public ControllerSnapshot? ReadFirstConnected()
    {
        for (uint index = 0; index < 4; index++)
        {
            if (XInputGetState(index, out var state) != 0) continue;
            var gamepad = state.Gamepad;
            return new ControllerSnapshot(index, gamepad.Buttons, gamepad.LeftTrigger,
                gamepad.RightTrigger, gamepad.LX, gamepad.LY, gamepad.RX, gamepad.RY);
        }
        return null;
    }
}
