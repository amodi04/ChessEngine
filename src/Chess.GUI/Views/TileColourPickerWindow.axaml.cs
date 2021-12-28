using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Chess.GUI.ViewModels;

namespace Chess.GUI.Views;

/// <summary>
///     Responsible for setting tile colours from user choice
/// </summary>
public class TileColourPickerWindow : Window
{
    public TileColourPickerWindow()
    {
        InitializeComponent();
        DataContext = new TileColourPickerWindowViewModel();

#if DEBUG
        this.AttachDevTools();
#endif
    }

    /// <summary>
    ///     Initialises GUI components
    /// </summary>
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    ///     Called when the close set colour button is clicked.
    /// </summary>
    /// <param name="sender">The object that owns the event.</param>
    /// <param name="e">The event.</param>
    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        var viewModel = (TileColourPickerWindowViewModel)DataContext!;
        Close(viewModel.SelectedColor);
    }
}