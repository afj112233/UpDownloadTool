using System;
using ICSStudio.EditorPackage.MonitorEditTags.Models;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ICSStudio.EditorPackage.MonitorEditTags.UI
{
    [ValueConversion(typeof(TagItem), typeof(Visibility))]
    public class DescriptionVisibilityConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TagItem tagItem = value as TagItem;
            if (tagItem == null)
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
