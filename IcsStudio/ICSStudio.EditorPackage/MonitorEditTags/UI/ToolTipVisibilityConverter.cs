using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ICSStudio.EditorPackage.MonitorEditTags.UI
{
    [ValueConversion(typeof(string), typeof(Visibility))]
    public class ToolTipVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = value as string;
            if (string.IsNullOrEmpty(text))
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
