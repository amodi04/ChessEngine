using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Engine.Enums;
using Engine.Types.Pieces;

namespace Chess.GUI
{
    /// <summary>
    /// Code behind for the promotion window
    /// </summary>
    public class PromotionWindow : Window
    {
        /// <summary>
        /// Initialises the object
        /// </summary>
        public PromotionWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        /// <summary>
        /// Load the xaml
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Method called on click event from all four buttons in the front end
        /// </summary>
        /// <param name="sender">The object that owns the event</param>
        /// <param name="e">The event</param>
        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            // Get the button text from the button pressed
            string? buttonText = (string) ((Button) sender!).Content!;

            // Switch depending on the text
            // The window is closed and the piece type is returned as a dialog result
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
}