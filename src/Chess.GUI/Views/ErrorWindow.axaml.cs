using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Chess.GUI.Views;

/// <summary>
///     Handles GUI errors
/// </summary>
public class ErrorWindow : Window
{
    public ErrorWindow(string error) : this()
    {
        this.Find<TextBox>("ErrorLabel").Text = error;
    }

    public ErrorWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    ///     Event handler called when close board button is clicked.
    /// </summary>
    /// <param name="sender">The object that owns the event.</param>
    /// <param name="e">The event.</param>
    private void CloseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}