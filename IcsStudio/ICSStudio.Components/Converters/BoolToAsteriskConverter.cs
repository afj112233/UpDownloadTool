using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ICSStudio.Components.Converters
{
    public class BoolToAsteriskConverter : IValueConverter
    {
        private static readonly string CleanString = string.Empty;
        private const string DirtyString = "*";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !(targetType == null))
            {
                if (targetType.IsAssignableFrom(typeof(string)))
                {
                    try
                    {
                        return (bool) value ? DirtyString : BoolToAsteriskConverter.CleanString as object;
                    }
                    catch (InvalidCastException)
                    {
                        return DependencyProperty.UnsetValue;
                    }
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Converting from a string to a boolean value is not supported.");
        }
    }
}
