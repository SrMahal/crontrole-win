using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Kp4040.App.Models;

public sealed class ButtonMappingRow : INotifyPropertyChanged
{
    private ActionKind _selectedAction;
    private KeyboardShortcut? _shortcut;
    private bool _isPressed;
    private bool _isIdentified;

    public ButtonMappingRow(ButtonDescriptor descriptor, ActionKind selectedAction, KeyboardShortcut? shortcut = null)
    {
        Descriptor = descriptor;
        _selectedAction = selectedAction;
        _shortcut = shortcut;
    }

    public ButtonDescriptor Descriptor { get; }
    public string Label => Descriptor.Label;
    public IReadOnlyList<ActionChoice> Choices => ActionCatalog.All;
    public bool NeedsShortcut => SelectedAction == ActionKind.CustomKeyboard;
    public string ShortcutLabel => Shortcut?.Label ?? "Gravar atalho...";

    public ActionKind SelectedAction
    {
        get => _selectedAction;
        set
        {
            if (_selectedAction == value) return;
            _selectedAction = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(NeedsShortcut));
            MappingChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public KeyboardShortcut? Shortcut
    {
        get => _shortcut;
        set
        {
            _shortcut = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ShortcutLabel));
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

    public bool IsIdentified
    {
        get => _isIdentified;
        set
        {
            if (_isIdentified == value) return;
            _isIdentified = value;
            OnPropertyChanged();
        }
    }

    public event EventHandler? MappingChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
