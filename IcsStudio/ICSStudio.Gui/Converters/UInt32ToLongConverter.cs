using System;
using System.Globalization;
using System.Windows.Data;

namespace ICSStudio.Gui.Converters
{
    [ValueConversion(typeof(UInt32), typeof(long))]
    public class UInt32ToLongConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is UInt32))
                return null;

            return System.Convert.ChangeType(value, typeof(long));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            return System.Convert.ChangeType(value, typeof(UInt32));
        }
    }
}
