using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Chess.GUI.ViewModels;

namespace Chess.GUI.Views
{
    public class EndgameWindow : Window
    {
        // Member fields
        public EndgameWindowViewModel ViewModel { get; }
        public EndgameWindow()
        {
            InitializeComponent();
            
            // Create and set view model
            ViewModel = new EndgameWindowViewModel();
            DataContext = ViewModel;
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Called when the close button is clicked.
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