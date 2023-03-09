using System;
using System.Globalization;
using System.Windows.Data;

namespace ICSStudio.DeviceProperties.Controls
{
    public class IsCheckedConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null || parameter == null)
                return false;

            int index = (int) value;
            int targetIndex = int.Parse(parameter.ToString());

            if (index == targetIndex)
                return true;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
