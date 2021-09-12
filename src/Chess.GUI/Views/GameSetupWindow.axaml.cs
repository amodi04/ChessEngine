using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Engine.Enums;

namespace Chess.GUI.Views
{
    /// <summary>
    /// Code behind for GameSetupWindow
    /// </summary>
    public class GameSetupWindow : Window
    {
        // Member fields
        private PlayerType _whitePlayerType;
        private PlayerType _blackPlayerType;
        
        /// <summary>
        /// Creates a GameSetupWindow
        /// </summary>
        public GameSetupWindow()
        {
            // Initialise components
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        /// <summary>
        /// Initialises components.
        /// </summary>
        private void InitializeComponent()
        {
            // Load front end xaml file
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Called when a radio button in front end is clicked.
        /// </summary>
        /// <param name="sender">The object that owns the event.</param>
        /// <param name="e">Arguments passed into the event.</param>
        private void RadioButton_Checked(object? sender, RoutedEventArgs e)
        {
            // If the sender is not a radio button, return
            if (sender is not RadioButton radioButton) return;
            
            // Switch based on radio button groups
            switch (radioButton.GroupName)
            {
                // If white
                case "White":
                    // Set the white player type to the value of the radio button group
                    _whitePlayerType = (string) radioButton.Content == "Human" ? PlayerType.Human : PlayerType.Computer;
                    break;
                // If black
                case "Black":
                    // Set the black player type to the value of the radio button group
                    _blackPlayerType = (string) radioButton.Content == "Human" ? PlayerType.Human : PlayerType.Computer;
                    break;
            }
        }

        /// <summary>
        /// Called when the close buttons are clicked.
        /// </summary>
        /// <param name="sender">The object that owns the event.</param>
        /// <param name="e">Arguments passed into the event.</param>
        private void CloseButton_OnClick(object? sender, RoutedEventArgs e)
        {
            // Get the content from the button
            string? buttonText = (string) ((Button) sender!).Content!;

            switch (buttonText)
            {
                // If "New Game"
                case "New Game":
                    // Close the dialog, passing a tuple result of the player types
                    Close(Tuple.Create(_whitePlayerType, _blackPlayerType));
                    break;
                // If "Cancel"
                case "Cancel":
                    // Close with no result
                    Close();
                    break;
            }
        }
    }
}