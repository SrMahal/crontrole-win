using System.Windows;
using System.Windows.Controls;
using Kp4040.App.Models;
using Kp4040.App.Services;

namespace Kp4040.App;

// Editor textual: nao depende de capturar atalhos que o Windows intercepta (ex.: Alt+Tab).
public sealed class ShortcutEditorWindow : Window
{
    private readonly TextBox _input;
    private readonly TextBlock _message;

    public KeyboardShortcut? Result { get; private set; }

    public ShortcutEditorWindow(KeyboardShortcut? current)
    {
        Title = "Crontrole · Editar combinação";
        Width = 470;
        Height = 255;
        MinWidth = 400;
        MinHeight = 230;
        ResizeMode = ResizeMode.NoResize;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Background = System.Windows.Media.Brushes.White;
        FontFamily = new System.Windows.Media.FontFamily("Segoe UI");

        var stack = new StackPanel { Margin = new Thickness(22) };
        stack.Children.Add(new TextBlock
        {
            Text = "Digite a combinação de teclas:",
            FontWeight = FontWeights.SemiBold,
            FontSize = 15
        });
        _input = new TextBox
        {
            Text = current?.Label ?? "Alt+Tab",
            Height = 34,
            FontSize = 15,
            Margin = new Thickness(0, 12, 0, 8),
            Padding = new Thickness(7, 4, 7, 4)
        };
        stack.Children.Add(_input);
        stack.Children.Add(new TextBlock
        {
            Text = "Exemplos: Alt+Tab, Ctrl+Shift+S, Win+E, F5, Ctrl+Alt+T",
            Foreground = System.Windows.Media.Brushes.DimGray,
            TextWrapping = TextWrapping.Wrap
        });
        _message = new TextBlock
        {
            Foreground = System.Windows.Media.Brushes.DarkRed,
            Margin = new Thickness(0, 9, 0, 2),
            TextWrapping = TextWrapping.Wrap
        };
        stack.Children.Add(_message);

        var buttons = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 7, 0, 0)
        };
        var cancel = new Button { Content = "Cancelar", Width = 100, Height = 30, Margin = new Thickness(0, 0, 8, 0), IsCancel = true };
        var save = new Button { Content = "Salvar combinação", MinWidth = 135, Height = 30, IsDefault = true };
        cancel.Click += (_, _) => { DialogResult = false; };
        save.Click += (_, _) =>
        {
            if (!ShortcutTextParser.TryParse(_input.Text, out var parsed, out string error))
            {
                _message.Text = error;
                return;
            }
            Result = parsed;
            DialogResult = true;
        };
        buttons.Children.Add(cancel);
        buttons.Children.Add(save);
        stack.Children.Add(buttons);
        Content = stack;
        Loaded += (_, _) => { _input.Focus(); _input.SelectAll(); };
    }
}
