using System;
using System.Globalization;
using System.Windows.Data;

namespace ICSStudio.Gui.Converters
{
    [ValueConversion(typeof(UInt16), typeof(int))]
    public class UInt16ToIntegerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is UInt16))
                return null;

            return System.Convert.ChangeType(value, typeof(int));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ChangeType(value, typeof(UInt16));
        }
    }
}
