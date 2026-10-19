namespace Kp4040.App.Models;

public sealed class ControllerProfile
{
    public int SchemaVersion { get; set; } = 1;
    public string Name { get; set; } = "Padrao";
    public byte TriggerThreshold { get; set; } = 180;
    public Dictionary<string, ActionKind> Mappings { get; set; } = new();

    public static ControllerProfile CreateDefault()
    {
        var profile = new ControllerProfile();
        foreach (var button in ControllerButtons.All)
            profile.Mappings[button.Key] = ActionKind.None;

        // Apenas sugestoes visuais: executar acoes permanece DESLIGADO ao abrir o programa.
        profile.Mappings["A"] = ActionKind.Copy;
        profile.Mappings["B"] = ActionKind.Paste;
        profile.Mappings["X"] = ActionKind.Enter;
        profile.Mappings["Y"] = ActionKind.OpenChatGpt;
        return profile;
    }
}
