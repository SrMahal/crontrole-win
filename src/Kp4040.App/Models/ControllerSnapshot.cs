namespace Kp4040.App.Models;

public sealed record ControllerSnapshot(
    uint UserIndex,
    ushort Buttons,
    byte LeftTrigger,
    byte RightTrigger,
    short LX,
    short LY,
    short RX,
    short RY);

public sealed record ButtonDescriptor(string Key, string Label, ushort Mask = 0, bool IsTrigger = false);

public sealed record ButtonEvent(string Key, string Label, bool IsPressed);

public static class ControllerButtons
{
    // Os nomes abaixo correspondem ao layout XInput, nao a uma gravacao fisica especifica.
    public static IReadOnlyList<ButtonDescriptor> All { get; } =
    [
        new("A", "A", 0x1000),
        new("B", "B", 0x2000),
        new("X", "X", 0x4000),
        new("Y", "Y", 0x8000),
        new("DPadUp", "Direcional ↑", 0x0001),
        new("DPadDown", "Direcional ↓", 0x0002),
        new("DPadLeft", "Direcional ←", 0x0004),
        new("DPadRight", "Direcional →", 0x0008),
        new("LB", "L1 / LB", 0x0100),
        new("RB", "R1 / RB", 0x0200),
        new("Back", "Select / Back", 0x0020),
        new("Start", "Start", 0x0010),
        new("L3", "Clique analógico esquerdo", 0x0040),
        new("R3", "Clique analógico direito", 0x0080),
        new("LT", "L2 / LT", IsTrigger: true),
        new("RT", "R2 / RT", IsTrigger: true)
    ];

    public static bool IsPressed(ButtonDescriptor button, ControllerSnapshot snapshot, byte threshold) =>
        button.IsTrigger
            ? (button.Key == "LT" ? snapshot.LeftTrigger : snapshot.RightTrigger) >= threshold
            : (snapshot.Buttons & button.Mask) != 0;
}
