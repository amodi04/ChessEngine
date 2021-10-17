using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Chess.GUI.Libraries.ColorPicker.Converters
{
    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Format((string) parameter, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Do nothing
            return AvaloniaProperty.UnsetValue;
        }
    }
}