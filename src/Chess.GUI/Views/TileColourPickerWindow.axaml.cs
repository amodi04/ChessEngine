using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Chess.GUI.ViewModels;

namespace Chess.GUI.Views
{
    public class TileColourPickerWindow : Window
    {
        public TileColourPickerWindow()
        {
            // Initialise GUI components
            InitializeComponent();
            
            // Create and set the data context
            DataContext = new TileColourPickerWindowViewModel();

#if DEBUG
            this.AttachDevTools();
#endif
        }

        /// <summary>
        /// Initialises graphical components.
        /// </summary>
        private void InitializeComponent()
        {
            // Loads and links the xaml front end
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Called when the close set colour button is clicked.
        /// </summary>
        /// <param name="sender">The object that owns the event.</param>
        /// <param name="e">The event.</param>
        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            // Get the view model
            var viewModel = (TileColourPickerWindowViewModel) DataContext!;
            
            // Close the window, passing out the selected colour
            Close(viewModel.SelectedColor);
        }
    }
}