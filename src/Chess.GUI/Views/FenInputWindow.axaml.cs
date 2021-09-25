using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Chess.GUI.Views
{
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

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Event handler called when create board button is clicked.
        /// </summary>
        /// <param name="sender">The object that owns the event.</param>
        /// <param name="e">The event.</param>
        private void CreateBoard_OnClick(object? sender, RoutedEventArgs e)
        {
            // Get the string from the input box
            _fen = this.Find<TextBox>("FenInputLabel").Text;
            
            // Close the window, passing the string out to the main window
            Close(_fen);
        }
    }
}