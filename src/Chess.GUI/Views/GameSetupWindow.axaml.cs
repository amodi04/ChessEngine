using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Engine.Enums;

namespace Chess.GUI.Views
{
    public class GameSetupWindow : Window
    {
        private PlayerType WhitePlayerType { get; set; }
        private PlayerType BlackPlayerType { get; set; }
        
        public GameSetupWindow()
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

        private void RadioButton_Checked(object? sender, RoutedEventArgs e)
        {
            if (sender is not RadioButton radioButton) return;
            switch (radioButton.GroupName)
            {
                case "White":
                    WhitePlayerType = (string) radioButton.Content == "Human" ? PlayerType.Human : PlayerType.Computer;
                    break;
                case "Black":
                    BlackPlayerType = (string) radioButton.Content == "Human" ? PlayerType.Human : PlayerType.Computer;
                    break;
            }
        }

        private void CloseButton_OnClick(object? sender, RoutedEventArgs e)
        {
            string? buttonText = (string) ((Button) sender!).Content!;

            switch (buttonText)
            {
                case "New Game":
                    Close(Tuple.Create(WhitePlayerType, BlackPlayerType));
                    break;
                case "Cancel":
                    Close();
                    break;
            }
        }
    }
}