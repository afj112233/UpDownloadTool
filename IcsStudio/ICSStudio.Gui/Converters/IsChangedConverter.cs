using System;
using System.Globalization;
using System.Windows.Data;

namespace ICSStudio.Gui.Converters
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class IsChangedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return string.Empty;

            var isChanged = (bool) value;
            return isChanged ? "*" : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
