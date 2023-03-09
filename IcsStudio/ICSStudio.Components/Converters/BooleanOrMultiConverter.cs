using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ICSStudio.Components.Converters
{
    public class BooleanOrMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == null)
                return false;
            if (!targetType.IsAssignableFrom(typeof(bool)))
                return DependencyProperty.UnsetValue;
            return values
                .Select(v =>
                    System.Convert.ToBoolean(v, CultureInfo.InvariantCulture)).ToList()
                .Any(val => val);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
