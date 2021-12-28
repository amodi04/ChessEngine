using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Chess.GUI.Libraries.ColorPicker.Converters;

/// <summary>
///     See http://stackoverflow.com/questions/397556/how-to-bind-radiobuttons-to-an-enum
/// </summary>
public class EnumToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value.Equals(parameter);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value.Equals(true) ? parameter : AvaloniaProperty.UnsetValue;
        ;
    }
}