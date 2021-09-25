using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Chess.GUI.Views
{
    public class FenOutputWindow : Window
    {
        public FenOutputWindow(string fen) : this()
        {
            this.Find<TextBox>("FenOutputLabel").Text = fen;
        }
        
        public FenOutputWindow()
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
        /// Event handler called when close board button is clicked.
        /// </summary>
        /// <param name="sender">The object that owns the event.</param>
        /// <param name="e">The event.</param>
        private void CloseButton_OnClick(object? sender, RoutedEventArgs e)
        {
            // Close the window
            Close();
        }
    }
}