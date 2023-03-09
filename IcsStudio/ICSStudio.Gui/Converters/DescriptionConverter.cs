using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ICSStudio.Gui.Converters
{
    public class DescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null || !targetType.IsAssignableFrom(typeof(string))
                ? DependencyProperty.UnsetValue
                : value.ToString().Replace(Environment.NewLine, " ").Replace("\t", " ");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
