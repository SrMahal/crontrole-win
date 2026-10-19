using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Kp4040.App.Models;

namespace Kp4040.App.Services;

public sealed class ActionExecutor
{
    private const uint InputKeyboard = 1;
    private const uint KeyUp = 0x0002;
    private const ushort VkControl = 0x11;
    private const ushort VkC = 0x43;
    private const ushort VkV = 0x56;
    private const ushort VkA = 0x41;
    private const ushort VkZ = 0x5A;
    private const ushort VkEnter = 0x0D;

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

    public string Execute(ActionKind action, bool appHasFocus)
    {
        if (action == ActionKind.None) return "Nenhuma acao configurada";
        if (action == ActionKind.OpenChatGpt)
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://chatgpt.com/") { UseShellExecute = true });
                return "ChatGPT aberto no navegador";
            }
            catch (Exception ex)
            {
                return $"Nao foi possivel abrir o navegador: {ex.Message}";
            }
        }

        // Protege a propria janela do configurador contra teclas sinteticas.
        if (appHasFocus)
            return "Atalho ignorado: deixe outra janela em foco";

        return action switch
        {
            ActionKind.Copy => SendChord(VkControl, VkC, "Ctrl+C"),
            ActionKind.Paste => SendChord(VkControl, VkV, "Ctrl+V"),
            ActionKind.Enter => SendKey(VkEnter, "Enter"),
            ActionKind.SelectAll => SendChord(VkControl, VkA, "Ctrl+A"),
            ActionKind.Undo => SendChord(VkControl, VkZ, "Ctrl+Z"),
            _ => "Acao desconhecida"
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

    private static string SendKey(ushort key, string label) =>
        SendKeys([Keyboard(key), Keyboard(key, true)], label);

    private static string SendChord(ushort modifier, ushort key, string label) =>
        SendKeys([Keyboard(modifier), Keyboard(key), Keyboard(key, true), Keyboard(modifier, true)], label);

    private static string SendKeys(INPUT[] keys, string label)
    {
        var sent = SendInput((uint)keys.Length, keys, Marshal.SizeOf<INPUT>());
        if (sent != keys.Length)
        {
            var error = Marshal.GetLastWin32Error();
            return $"Falha ao enviar {label} (Win32: {error}). Verifique o foco/permissoes.";
        }
        return $"Enviado: {label}";
    }
}
