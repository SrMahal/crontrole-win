namespace Kp4040.App.Models;

// Valores existentes mantidos por nome para compatibilidade com perfis JSON da Alpha 0.4.
public enum ActionKind
{
    None, Copy, Paste, Enter, SelectAll, Undo, OpenChatGpt,
    CustomKeyboard,
    Tab, Escape, Space, Backspace, Delete, ArrowUp, ArrowDown, ArrowLeft,
    ArrowRight, Home, End, PageUp, PageDown,
    MouseLeft, MouseRight, MouseMiddle, MouseDoubleLeft,
    MouseBack, MouseForward, WheelUp, WheelDown, WheelLeft, WheelRight,
    MediaPlayPause, MediaNext, MediaPrevious, VolumeUp, VolumeDown, VolumeMute
}

public sealed record ActionChoice(ActionKind Kind, string Label);

public static class ActionCatalog
{
    public static IReadOnlyList<ActionChoice> All { get; } =
    [
        new(ActionKind.None, "Nenhuma"),
        new(ActionKind.CustomKeyboard, "⌨ Teclado · Gravar tecla/atalho..."),
        new(ActionKind.Copy, "⌨ Copiar · Ctrl+C"),
        new(ActionKind.Paste, "⌨ Colar · Ctrl+V"),
        new(ActionKind.Enter, "⌨ Enter"),
        new(ActionKind.SelectAll, "⌨ Selecionar tudo · Ctrl+A"),
        new(ActionKind.Undo, "⌨ Desfazer · Ctrl+Z"),
        new(ActionKind.Tab, "⌨ Tab"),
        new(ActionKind.Escape, "⌨ Esc"),
        new(ActionKind.Space, "⌨ Espaço"),
        new(ActionKind.Backspace, "⌨ Backspace"),
        new(ActionKind.Delete, "⌨ Delete"),
        new(ActionKind.ArrowUp, "⌨ Seta para cima"),
        new(ActionKind.ArrowDown, "⌨ Seta para baixo"),
        new(ActionKind.ArrowLeft, "⌨ Seta para esquerda"),
        new(ActionKind.ArrowRight, "⌨ Seta para direita"),
        new(ActionKind.Home, "⌨ Home"),
        new(ActionKind.End, "⌨ End"),
        new(ActionKind.PageUp, "⌨ Page Up"),
        new(ActionKind.PageDown, "⌨ Page Down"),
        new(ActionKind.OpenChatGpt, "🌐 Abrir ChatGPT"),
        new(ActionKind.MouseLeft, "🖱 Mouse · Clique esquerdo"),
        new(ActionKind.MouseRight, "🖱 Mouse · Clique direito"),
        new(ActionKind.MouseMiddle, "🖱 Mouse · Clique do meio"),
        new(ActionKind.MouseDoubleLeft, "🖱 Mouse · Clique duplo"),
        new(ActionKind.MouseBack, "🖱 Mouse · Voltar (X1)"),
        new(ActionKind.MouseForward, "🖱 Mouse · Avançar (X2)"),
        new(ActionKind.WheelUp, "🖱 Scroll · Para cima"),
        new(ActionKind.WheelDown, "🖱 Scroll · Para baixo"),
        new(ActionKind.WheelLeft, "🖱 Scroll · Para esquerda"),
        new(ActionKind.WheelRight, "🖱 Scroll · Para direita"),
        new(ActionKind.MediaPlayPause, "⌨ Mídia · Play/Pause"),
        new(ActionKind.MediaNext, "⌨ Mídia · Próxima faixa"),
        new(ActionKind.MediaPrevious, "⌨ Mídia · Faixa anterior"),
        new(ActionKind.VolumeUp, "⌨ Mídia · Volume +"),
        new(ActionKind.VolumeDown, "⌨ Mídia · Volume -"),
        new(ActionKind.VolumeMute, "⌨ Mídia · Mudo")
    ];
}
