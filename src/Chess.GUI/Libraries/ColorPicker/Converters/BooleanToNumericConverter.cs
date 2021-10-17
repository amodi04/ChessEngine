using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Chess.GUI.Libraries.ColorPicker.Converters
{
    public class BooleanToNumericConverter : IValueConverter
    {
        public double TrueValue { get; set; }
        public double FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cond = (bool) value;
            return cond ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // TODO - Probably not a good idea to compare doubles
            var val = (double) value;

            if (val == TrueValue)
                return true;
            if (val == FalseValue)
                return false;

            return AvaloniaProperty.UnsetValue;
        }
    }
}