using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Chess.GUI.Libraries.ColorPicker.Structures;

namespace Chess.GUI.Libraries.ColorPicker.Converters;

public class RGBColorToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is RGBColor color)
            return new SolidColorBrush(color);
        return AvaloniaProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is SolidColorBrush brush)
            return new RGBColor(brush.Color.R, brush.Color.G, brush.Color.B);
        return AvaloniaProperty.UnsetValue;
    }
}