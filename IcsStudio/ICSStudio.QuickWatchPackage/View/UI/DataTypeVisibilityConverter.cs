using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using ICSStudio.QuickWatchPackage.View.Models;

namespace ICSStudio.QuickWatchPackage.View.UI
{
    [ValueConversion(typeof(TagItem), typeof(Visibility))]
    public class DataTypeVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TagItem tagItem = value as TagItem;

            if (tagItem?.Tag == null)
                return Visibility.Collapsed;

            if (tagItem.Tag.DataTypeInfo.DataType.IsAtomic)
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
