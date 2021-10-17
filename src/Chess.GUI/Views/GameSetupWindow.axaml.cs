using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Engine.Player;

namespace Chess.GUI.Views
{
    /// <summary>
    ///     Class responsible for setting up new chess games.
    /// </summary>
    public class GameSetupWindow : Window
    {
        private PlayerType _blackPlayerType;
        private PlayerType _whitePlayerType;
        
        public GameSetupWindow()
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
        ///     Called when a radio button in front end is clicked.
        /// </summary>
        /// <param name="sender">The object that owns the event.</param>
        /// <param name="e">Arguments passed into the event.</param>
        private void RadioButton_Checked(object? sender, RoutedEventArgs e)
        {
            if (sender is not RadioButton radioButton) return;
            
            switch (radioButton.GroupName)
            {
                case "White":
                    _whitePlayerType = (string) radioButton.Content == "Human" ? PlayerType.Human : PlayerType.Computer;
                    break;
                case "Black":
                    _blackPlayerType = (string) radioButton.Content == "Human" ? PlayerType.Human : PlayerType.Computer;
                    break;
            }
        }

        /// <summary>
        ///     Called when the close buttons are clicked.
        /// </summary>
        /// <param name="sender">The object that owns the event.</param>
        /// <param name="e">Arguments passed into the event.</param>
        private void CloseButton_OnClick(object? sender, RoutedEventArgs e)
        {
            var buttonText = (string) ((Button) sender!).Content!;

            switch (buttonText)
            {
                case "New Game":
                    Close(Tuple.Create(_whitePlayerType, _blackPlayerType));
                    break;
                case "Cancel":
                    // Close with no result
                    Close();
                    break;
            }
        }
    }
}