using System.Text.Json.Serialization;

namespace Kp4040.App.Models;

// Entrada independente de hardware. Sem tecla pressionada = sem atalho salvo.
public sealed class KeyboardShortcut
{
    public int VirtualKey { get; set; }
    public bool Control { get; set; }
    public bool Shift { get; set; }
    public bool Alt { get; set; }
    public bool Windows { get; set; }
    public string KeyLabel { get; set; } = "";

    [JsonIgnore]
    public string Label => string.Join("+", new []
    {
        Control ? "Ctrl" : null,
        Shift ? "Shift" : null,
        Alt ? "Alt" : null,
        Windows ? "Win" : null,
        string.IsNullOrWhiteSpace(KeyLabel) ? $"VK_{VirtualKey:X2}" : KeyLabel
    }.Where(s => s is not null));
}
