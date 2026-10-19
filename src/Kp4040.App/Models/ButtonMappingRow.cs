using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Kp4040.App.Models;

public sealed class ButtonMappingRow : INotifyPropertyChanged
{
    private ActionKind _selectedAction;
    private bool _isPressed;

    public ButtonMappingRow(ButtonDescriptor descriptor, ActionKind selectedAction)
    {
        Descriptor = descriptor;
        _selectedAction = selectedAction;
    }

    public ButtonDescriptor Descriptor { get; }
    public string Label => Descriptor.Label;
    public IReadOnlyList<ActionChoice> Choices => ActionCatalog.All;

    public ActionKind SelectedAction
    {
        get => _selectedAction;
        set
        {
            if (_selectedAction == value) return;
            _selectedAction = value;
            OnPropertyChanged();
            MappingChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsPressed
    {
        get => _isPressed;
        set
        {
            if (_isPressed == value) return;
            _isPressed = value;
            OnPropertyChanged();
        }
    }

    public event EventHandler? MappingChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
