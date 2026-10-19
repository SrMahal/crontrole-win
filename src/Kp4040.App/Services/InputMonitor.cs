using Kp4040.App.Models;

namespace Kp4040.App.Services;

// Gera eventos apenas nas bordas (pressionar e soltar), nunca em cada frame.
public sealed class InputMonitor
{
    private ControllerSnapshot? _previous;
    private byte _previousThreshold;

    public IReadOnlyList<ButtonEvent> Process(ControllerSnapshot current, byte triggerThreshold)
    {
        var events = new List<ButtonEvent>();
        foreach (var button in ControllerButtons.All)
        {
            var now = ControllerButtons.IsPressed(button, current, triggerThreshold);
            var before = _previous is not null &&
                ControllerButtons.IsPressed(button, _previous, _previousThreshold);
            if (now != before)
                events.Add(new ButtonEvent(button.Key, button.Label, now));
        }
        AlignTo(current, triggerThreshold);
        return events;
    }

    public void AlignTo(ControllerSnapshot? snapshot, byte threshold)
    {
        _previous = snapshot;
        _previousThreshold = threshold;
    }
}
