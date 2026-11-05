using System.Globalization;
using Kp4040.App.Models;

namespace Kp4040.App.Services;

// Interpretador de uma combinacao simultanea de teclado: Alt+Tab, Ctrl+Shift+S etc.
// Nao e um gravador de macros ou de sequencias temporizadas.
public static class ShortcutTextParser
{
    private static readonly Dictionary<string, int> NamedKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Tab"] = 0x09, ["Alt"] = 0x12, ["Enter"] = 0x0D, ["Return"] = 0x0D,
        ["Esc"] = 0x1B, ["Escape"] = 0x1B,
        ["Space"] = 0x20, ["Espaco"] = 0x20, ["Espaço"] = 0x20,
        ["Backspace"] = 0x08, ["Delete"] = 0x2E, ["Del"] = 0x2E,
        ["Insert"] = 0x2D, ["Ins"] = 0x2D,
        ["Home"] = 0x24, ["End"] = 0x23,
        ["PageUp"] = 0x21, ["PgUp"] = 0x21,
        ["PageDown"] = 0x22, ["PgDn"] = 0x22,
        ["Up"] = 0x26, ["Down"] = 0x28, ["Left"] = 0x25, ["Right"] = 0x27,
        ["ArrowUp"] = 0x26, ["ArrowDown"] = 0x28,
        ["ArrowLeft"] = 0x25, ["ArrowRight"] = 0x27,
        ["PrintScreen"] = 0x2C, ["Pause"] = 0x13
    };

    public static bool TryParse(string? text, out KeyboardShortcut? shortcut, out string error)
    {
        shortcut = null;
        error = "";
        var parts = (text ?? "").Split('+', StringSplitOptions.TrimEntries);
        if (parts.Length == 0 || parts.Length > 5 || parts.Any(string.IsNullOrWhiteSpace))
        {
            error = "Digite uma combinação como Alt+Tab ou Ctrl+Shift+S.";
            return false;
        }

        bool ctrl = false, shift = false, alt = false, win = false;
        var keyText = parts[^1];
        foreach (var part in parts.Take(parts.Length - 1))
        {
            switch (part.ToUpperInvariant())
            {
                case "CTRL": case "CONTROL":
                    if (ctrl) return Fail("Ctrl repetido.", out error);
                    ctrl = true; break;
                case "SHIFT":
                    if (shift) return Fail("Shift repetido.", out error);
                    shift = true; break;
                case "ALT":
                    if (alt) return Fail("Alt repetido.", out error);
                    alt = true; break;
                case "WIN": case "WINDOWS":
                    if (win) return Fail("Win repetido.", out error);
                    win = true; break;
                default:
                    return Fail($"Modificador não suportado: {part}.", out error);
            }
        }

        if (new[] { "CTRL", "CONTROL", "SHIFT", "ALT", "WIN", "WINDOWS" }
            .Contains(keyText.ToUpperInvariant()) &&
            !(parts.Length == 1 && keyText.Equals("Alt", StringComparison.OrdinalIgnoreCase)))
            return Fail("Escolha uma tecla final, como Tab, S ou F5.", out error);

        int virtualKey;
        string label;
        if (keyText.Length == 1 && char.IsLetterOrDigit(keyText[0]) && keyText[0] < 128)
        {
            virtualKey = char.ToUpperInvariant(keyText[0]);
            label = keyText.ToUpperInvariant();
        }
        else if (NamedKeys.TryGetValue(keyText, out virtualKey))
        {
            label = keyText.Equals("Escape", StringComparison.OrdinalIgnoreCase) ? "Esc" : keyText;
        }
        else if (keyText.Length is >= 2 and <= 3 && keyText[0] is 'F' or 'f'
                 && int.TryParse(keyText.AsSpan(1), NumberStyles.None,
                     CultureInfo.InvariantCulture, out int n) && n is >= 1 and <= 24)
        {
            virtualKey = 0x6F + n;
            label = $"F{n}";
        }
        else if (keyText.StartsWith("VK_", StringComparison.OrdinalIgnoreCase)
                 && int.TryParse(keyText.AsSpan(3), NumberStyles.HexNumber,
                     CultureInfo.InvariantCulture, out virtualKey)
                 && virtualKey is >= 1 and <= 255)
        {
            label = keyText.ToUpperInvariant();
        }
        else
        {
            return Fail($"Tecla não reconhecida: {keyText}. Use Tab, F1, A-Z, 0-9 ou VK_XX.", out error);
        }

        if (ctrl && alt && virtualKey == 0x2E)
            return Fail("Ctrl+Alt+Del é reservado pelo Windows.", out error);
        if (win && virtualKey == 0x4C)
            return Fail("Win+L bloqueia o computador e não é permitido neste editor.", out error);

        shortcut = new KeyboardShortcut
        {
            VirtualKey = virtualKey,
            Control = ctrl,
            Alt = alt,
            Shift = shift,
            Windows = win,
            KeyLabel = label
        };
        return true;
    }

    private static bool Fail(string message, out string error)
    {
        error = message;
        return false;
    }
}
