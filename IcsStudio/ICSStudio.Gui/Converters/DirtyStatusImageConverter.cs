using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ICSStudio.Gui.Converters
{
    public class DirtyStatusImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return (bool) value ? Brushes.Red : Brushes.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
