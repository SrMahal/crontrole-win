using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Kp4040.App.Models;

namespace Kp4040.App.Services;

public sealed class ActionExecutor
{
    private const uint InputMouse = 0;
    private const uint InputKeyboard = 1;
    private const uint KeyUp = 0x0002;
    private const uint MouseLeftDown = 0x0002;
    private const uint MouseLeftUp = 0x0004;
    private const uint MouseRightDown = 0x0008;
    private const uint MouseRightUp = 0x0010;
    private const uint MouseMiddleDown = 0x0020;
    private const uint MouseMiddleUp = 0x0040;
    private const uint MouseXDown = 0x0080;
    private const uint MouseXUp = 0x0100;
    private const uint MouseWheel = 0x0800;
    private const uint MouseHWheel = 0x1000;
    private const uint WheelDelta = 120;

    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
        public uint Type;
        public InputUnion Data;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct InputUnion
    {
        [FieldOffset(0)] public KEYBDINPUT Keyboard;
        [FieldOffset(0)] public MOUSEINPUT Mouse;
        [FieldOffset(0)] public HARDWAREINPUT Hardware;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KEYBDINPUT
    {
        public ushort VirtualKey;
        public ushort ScanCode;
        public uint Flags;
        public uint Time;
        public UIntPtr ExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MOUSEINPUT
    {
        public int Dx;
        public int Dy;
        public uint MouseData;
        public uint Flags;
        public uint Time;
        public UIntPtr ExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct HARDWAREINPUT
    {
        public uint Message;
        public ushort ParamLow;
        public ushort ParamHigh;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint count, INPUT[] inputs, int size);

    public string Execute(ActionKind action, bool appHasFocus, KeyboardShortcut? shortcut = null)
    {
        if (action == ActionKind.None) return "Nenhuma ação";
        if (action == ActionKind.OpenChatGpt)
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://chatgpt.com/") { UseShellExecute = true });
                return "ChatGPT aberto no navegador";
            }
            catch (Exception ex)
            {
                return $"Falha ao abrir o navegador: {ex.Message}";
            }
        }

        // Evita clique/teclado sintetico sobre o proprio configurador.
        if (appHasFocus) return "Saída ignorada: deixe outro aplicativo em foco";

        return action switch
        {
            ActionKind.Copy => KeyChord(0x11, 0x43, "Ctrl+C"),
            ActionKind.Paste => KeyChord(0x11, 0x56, "Ctrl+V"),
            ActionKind.SelectAll => KeyChord(0x11, 0x41, "Ctrl+A"),
            ActionKind.Undo => KeyChord(0x11, 0x5A, "Ctrl+Z"),
            ActionKind.Enter => SingleKey(0x0D, "Enter"),
            ActionKind.Tab => SingleKey(0x09, "Tab"),
            ActionKind.Escape => SingleKey(0x1B, "Esc"),
            ActionKind.Space => SingleKey(0x20, "Espaço"),
            ActionKind.Backspace => SingleKey(0x08, "Backspace"),
            ActionKind.Delete => SingleKey(0x2E, "Delete"),
            ActionKind.ArrowUp => SingleKey(0x26, "↑"),
            ActionKind.ArrowDown => SingleKey(0x28, "↓"),
            ActionKind.ArrowLeft => SingleKey(0x25, "←"),
            ActionKind.ArrowRight => SingleKey(0x27, "→"),
            ActionKind.Home => SingleKey(0x24, "Home"),
            ActionKind.End => SingleKey(0x23, "End"),
            ActionKind.PageUp => SingleKey(0x21, "PageUp"),
            ActionKind.PageDown => SingleKey(0x22, "PageDown"),
            ActionKind.MediaPlayPause => SingleKey(0xB3, "Play/Pause"),
            ActionKind.MediaNext => SingleKey(0xB0, "Próxima faixa"),
            ActionKind.MediaPrevious => SingleKey(0xB1, "Faixa anterior"),
            ActionKind.VolumeUp => SingleKey(0xAF, "Volume +"),
            ActionKind.VolumeDown => SingleKey(0xAE, "Volume -"),
            ActionKind.VolumeMute => SingleKey(0xAD, "Mudo"),
            ActionKind.CustomKeyboard => CustomShortcut(shortcut),
            ActionKind.MouseLeft => MouseClick(MouseLeftDown, MouseLeftUp, 0, "Clique esquerdo"),
            ActionKind.MouseRight => MouseClick(MouseRightDown, MouseRightUp, 0, "Clique direito"),
            ActionKind.MouseMiddle => MouseClick(MouseMiddleDown, MouseMiddleUp, 0, "Clique do meio"),
            ActionKind.MouseBack => MouseClick(MouseXDown, MouseXUp, 1, "Mouse voltar"),
            ActionKind.MouseForward => MouseClick(MouseXDown, MouseXUp, 2, "Mouse avançar"),
            ActionKind.MouseDoubleLeft => MouseDoubleClick(),
            ActionKind.WheelUp => MouseScroll(MouseWheel, (int)WheelDelta, "Scroll cima"),
            ActionKind.WheelDown => MouseScroll(MouseWheel, -(int)WheelDelta, "Scroll baixo"),
            ActionKind.WheelLeft => MouseScroll(MouseHWheel, -(int)WheelDelta, "Scroll esquerda"),
            ActionKind.WheelRight => MouseScroll(MouseHWheel, (int)WheelDelta, "Scroll direita"),
            _ => "Ação não reconhecida"
        };
    }

    private static INPUT Keyboard(ushort key, bool release = false) => new()
    {
        Type = InputKeyboard,
        Data = new InputUnion
        {
            Keyboard = new KEYBDINPUT { VirtualKey = key, Flags = release ? KeyUp : 0 }
        }
    };

    private static INPUT Mouse(uint flag, uint data = 0) => new()
    {
        Type = InputMouse,
        Data = new InputUnion { Mouse = new MOUSEINPUT { Flags = flag, MouseData = data } }
    };

    private static string SingleKey(ushort key, string label) =>
        Send([Keyboard(key), Keyboard(key, true)], label);

    private static string KeyChord(ushort modifier, ushort key, string label) =>
        Send([Keyboard(modifier), Keyboard(key), Keyboard(key, true), Keyboard(modifier, true)], label);

    private static string CustomShortcut(KeyboardShortcut? shortcut)
    {
        if (shortcut is null || shortcut.VirtualKey is < 1 or > 255)
            return "Configure um atalho antes de executar";

        // Ordem fixa de pressões e liberações evita modificadores presos.
        var modifiers = new List<ushort>();
        if (shortcut.Control) modifiers.Add(0x11);
        if (shortcut.Shift) modifiers.Add(0x10);
        if (shortcut.Alt) modifiers.Add(0x12);
        if (shortcut.Windows) modifiers.Add(0x5B);
        var sequence = new List<INPUT>();
        foreach (ushort modifier in modifiers) sequence.Add(Keyboard(modifier));
        ushort key = (ushort)shortcut.VirtualKey;
        sequence.Add(Keyboard(key));
        sequence.Add(Keyboard(key, true));
        for (int i = modifiers.Count - 1; i >= 0; i--)
            sequence.Add(Keyboard(modifiers[i], true));
        return Send(sequence.ToArray(), shortcut.Label);
    }

    private static string MouseClick(uint down, uint up, uint data, string label) =>
        Send([Mouse(down, data), Mouse(up, data)], label);

    private static string MouseDoubleClick() => Send([
        Mouse(MouseLeftDown), Mouse(MouseLeftUp),
        Mouse(MouseLeftDown), Mouse(MouseLeftUp)
    ], "Clique duplo");

    private static string MouseScroll(uint flag, int steps, string label) =>
        Send([Mouse(flag, unchecked((uint)steps))], label);

    private static string Send(INPUT[] inputs, string label)
    {
        uint sent = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf<INPUT>());
        return sent == (uint)inputs.Length
            ? $"Enviado: {label}"
            : $"Falha ao enviar {label} (Win32: {Marshal.GetLastWin32Error()}).";
    }
}
