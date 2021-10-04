using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chess.GUI.ViewModels;

namespace Chess.GUI.Views
{
    /// <summary>
    /// Class for storing AISettingElement data.
    /// </summary>
    public class AISettingElement : UserControl
    {
        // All values shown here use data binding so that it can update using triggers.
        // Each property has a getter and setter.
        // Each property also has a StyledProperty which allows the registering of these properties to Avalonia.
        // This means these properties can be set through xaml.
        public int MaxValue
        {
            get => GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        public static readonly StyledProperty<int> MaxValueProperty =
            AvaloniaProperty.Register<AISettingElement, int>(nameof(MaxValue));
        
        public int MinValue
        {
            get => GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        public TickPlacement TickPlacement
        {
            get => GetValue(TickPlacementProperty);
            set => SetValue(TickPlacementProperty, value);
        }

        public static readonly StyledProperty<TickPlacement> TickPlacementProperty =
            AvaloniaProperty.Register<AISettingElement, TickPlacement>(nameof(TickPlacement));
        
        public static readonly StyledProperty<int> MinValueProperty =
            AvaloniaProperty.Register<AISettingElement, int>(nameof(MinValue));
        public int TickFrequency
        {
            get => GetValue(TickFrequencyProperty);
            set => SetValue(TickFrequencyProperty, value);
        }
        
        public static readonly StyledProperty<int> TickFrequencyProperty =
            AvaloniaProperty.Register<AISettingElement, int>(nameof(TickFrequency));
        public bool SnapToTick
        {
            get => GetValue(SnapToTickProperty);
            set => SetValue(SnapToTickProperty, value);
        }
        
        public static readonly StyledProperty<bool> SnapToTickProperty =
            AvaloniaProperty.Register<AISettingElement, bool>(nameof(SnapToTick));

        public bool IsTextBoxReadonly
        {
            get => GetValue(IsTextBoxReadonlyProperty);
            set => SetValue(IsTextBoxReadonlyProperty, value);
        }
        
        public static readonly StyledProperty<bool> IsTextBoxReadonlyProperty =
            AvaloniaProperty.Register<AISettingElement, bool>(nameof(IsTextBoxReadonly));

        public int SliderValue
        {
            get => GetValue(SliderValueProperty);
            set => SetValue(SliderValueProperty, value);
        }
        
        public static readonly StyledProperty<int> SliderValueProperty =
            AvaloniaProperty.Register<AISettingElement, int>(nameof(SliderValue));

        public AISettingElement()
        {
            InitializeComponent();
            
            // Set the data context of the xaml to this. Data bindings will pull data from this class.
            DataContext = this;
        }

        /// <summary>
        /// Initialises GUI components
        /// </summary>
        private void InitializeComponent()
        {
            // Load xaml
            AvaloniaXamlLoader.Load(this);
        }
    }
}