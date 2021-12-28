using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Chess.GUI.ViewModels;

namespace Chess.GUI.Views;

/// <summary>
///     Endgame Window UI class
/// </summary>
public class EndgameWindow : Window
{
    public EndgameWindow()
    {
        InitializeComponent();
        ViewModel = new EndgameWindowViewModel();
        DataContext = ViewModel;
#if DEBUG
        this.AttachDevTools();
#endif
    }

    public EndgameWindowViewModel ViewModel { get; }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    ///     Called when the close button is clicked.
    /// </summary>
    /// <param name="sender">The object that owns the event.</param>
    /// <param name="e">The event.</param>
    private void CloseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}