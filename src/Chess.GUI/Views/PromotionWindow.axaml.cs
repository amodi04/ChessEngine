using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Engine.Pieces;

namespace Chess.GUI.Views;

/// <summary>
///     Responsible for allowing user to choose pawn promotions.
/// </summary>
public class PromotionWindow : Window
{
    public PromotionWindow()
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
    ///     Method called on click event from all four buttons in the front end
    /// </summary>
    /// <param name="sender">The object that owns the event</param>
    /// <param name="e">The event</param>
    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        var buttonText = (string)((Button)sender!).Content!;
        switch (buttonText)
        {
            case "Queen":
                Close(PieceType.Queen);
                break;
            case "Rook":
                Close(PieceType.Rook);
                break;
            case "Bishop":
                Close(PieceType.Bishop);
                break;
            case "Knight":
                Close(PieceType.Knight);
                break;
        }
    }
}