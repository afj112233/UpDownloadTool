using System;
using ICSStudio.QuickWatchPackage.View.Models;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ICSStudio.QuickWatchPackage.View.UI
{
    [ValueConversion(typeof(TagItem), typeof(Visibility))]
    public class CrossReferenceVisibilityConverter : IValueConverter
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
