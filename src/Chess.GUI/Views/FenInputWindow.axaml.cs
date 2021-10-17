using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Chess.GUI.Views
{
    /// <summary>
    /// Class responsible for handling user input of FEN strings
    /// </summary>
    public class FenInputWindow : Window
    {
        private string _fen = "";

        public FenInputWindow()
        {
            InitializeComponent();
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
        ///     Event handler called when create board button is clicked.
        /// </summary>
        /// <param name="sender">The object that owns the event.</param>
        /// <param name="e">The event.</param>
        private void CreateBoard_OnClick(object? sender, RoutedEventArgs e)
        {
            _fen = this.Find<TextBox>("FenInputLabel").Text;
            Close(_fen);
        }
    }
}