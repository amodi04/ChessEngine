using Chess.GUI.Libraries.ColorPicker.Structures;
using ReactiveUI;

namespace Chess.GUI.ViewModels
{
    public class TileColourPickerWindowViewModel : ViewModelBase
    {
        // Backing fields
        private RGBColor _selectedColor;

        public RGBColor SelectedColor
        {
            // Return selected colour for getter
            get => _selectedColor;
            
            // Set the value and raise the set event so that binding can occur
            set => this.RaiseAndSetIfChanged(ref _selectedColor, value);
        }

        // Constructor
        public TileColourPickerWindowViewModel()
        {
            // Create new RGBColor struct to store colour data
            SelectedColor = new RGBColor();
        }
    }
}