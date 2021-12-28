using Chess.GUI.Libraries.ColorPicker.Structures;
using ReactiveUI;

namespace Chess.GUI.ViewModels;

/// <summary>
///     TileColourPickerWindow View Model class. Responsible for passing data in and out of window.
/// </summary>
public class TileColourPickerWindowViewModel : ViewModelBase
{
    private RGBColor _selectedColor;

    public TileColourPickerWindowViewModel()
    {
        SelectedColor = new RGBColor();
    }

    public RGBColor SelectedColor
    {
        get => _selectedColor;
        set => this.RaiseAndSetIfChanged(ref _selectedColor, value);
    }
}