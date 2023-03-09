using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ICSStudio.Gui.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class IsVisibleConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return Visibility.Hidden;

            var isVisible = (bool)value;
            return isVisible ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
