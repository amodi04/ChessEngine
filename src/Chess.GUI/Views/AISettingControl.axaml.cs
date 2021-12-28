using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Chess.GUI.Views;

/// <summary>
///     Class for storing AISettingElement data.
/// </summary>
public class AISettingControl : UserControl
{
    public static readonly StyledProperty<int> MaxValueProperty =
        AvaloniaProperty.Register<AISettingControl, int>(nameof(MaxValue));

    public static readonly StyledProperty<TickPlacement> TickPlacementProperty =
        AvaloniaProperty.Register<AISettingControl, TickPlacement>(nameof(TickPlacement));

    public static readonly StyledProperty<int> MinValueProperty =
        AvaloniaProperty.Register<AISettingControl, int>(nameof(MinValue));

    public static readonly StyledProperty<int> TickFrequencyProperty =
        AvaloniaProperty.Register<AISettingControl, int>(nameof(TickFrequency));

    public static readonly StyledProperty<bool> SnapToTickProperty =
        AvaloniaProperty.Register<AISettingControl, bool>(nameof(SnapToTick));

    public static readonly StyledProperty<bool> IsTextBoxReadonlyProperty =
        AvaloniaProperty.Register<AISettingControl, bool>(nameof(IsTextBoxReadonly));

    public static readonly StyledProperty<int> SliderValueProperty =
        AvaloniaProperty.Register<AISettingControl, int>(nameof(SliderValue));

    public AISettingControl()
    {
        InitializeComponent();
        DataContext = this;
    }

    // All values shown here use data binding so that it can update using triggers.
    // Each property has a StyledProperty which allows the registering of these properties to Avalonia.
    // This means these properties can be set through xaml.
    public int MaxValue
    {
        get => GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

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

    public int TickFrequency
    {
        get => GetValue(TickFrequencyProperty);
        set => SetValue(TickFrequencyProperty, value);
    }

    public bool SnapToTick
    {
        get => GetValue(SnapToTickProperty);
        set => SetValue(SnapToTickProperty, value);
    }

    public bool IsTextBoxReadonly
    {
        get => GetValue(IsTextBoxReadonlyProperty);
        set => SetValue(IsTextBoxReadonlyProperty, value);
    }

    public int SliderValue
    {
        get => GetValue(SliderValueProperty);
        set => SetValue(SliderValueProperty, value);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}