namespace Kp4040.App.Models;

public enum ActionKind
{
    None,
    Copy,
    Paste,
    Enter,
    SelectAll,
    Undo,
    OpenChatGpt
}

public sealed record ActionChoice(ActionKind Kind, string Label);

public static class ActionCatalog
{
    public static IReadOnlyList<ActionChoice> All { get; } =
    [
        new(ActionKind.None, "Nenhuma"),
        new(ActionKind.Copy, "Copiar · Ctrl+C"),
        new(ActionKind.Paste, "Colar · Ctrl+V"),
        new(ActionKind.Enter, "Enter"),
        new(ActionKind.SelectAll, "Selecionar tudo · Ctrl+A"),
        new(ActionKind.Undo, "Desfazer · Ctrl+Z"),
        new(ActionKind.OpenChatGpt, "Abrir ChatGPT")
    ];
}
