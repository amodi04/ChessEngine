using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace Chess.GUI.Views;

/// <summary>
///     Class responsible for handling user input of FEN strings
/// </summary>
public class FenInputWindow : Window
{
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
    ///     Event handler called when create board button is clicked.
    /// </summary>
    /// <param name="sender">The object that owns the event.</param>
    /// <param name="e">The event.</param>
    private void CreateBoard_OnClick(object? sender, RoutedEventArgs e)
    {
        // Validate against regex for fen strings
        Regex re = new(
            @"^\s*([rnbqkpRNBQKP1-8]+\/){7}([rnbqkpRNBQKP1-8]+)\s[bw-]\s(([a-hkqA-HKQ]{1,4})|(-))\s(([a-h][45])|(-))\s\d+\s\d+\s*$");
        var fenToCheck = this.Find<TextBox>("FenInputLabel").Text;
        if (string.IsNullOrWhiteSpace(fenToCheck) || !re.IsMatch(fenToCheck))
        {
            var textBox = this.Find<TextBox>("FenInputLabel");
            textBox.Background = Brushes.Red;
        }
        else
        {
            Close(fenToCheck);
        }
    }
}