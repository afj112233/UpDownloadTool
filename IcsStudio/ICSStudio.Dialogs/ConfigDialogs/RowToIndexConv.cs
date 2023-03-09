using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
namespace ICSStudio.Dialogs.ConfigDialogs
{
    public class RowToIndexConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DataGridRow row = value as DataGridRow;
            Debug.Assert(row != null, nameof(row) + " != null");
            if (row.Item == CollectionView.NewItemPlaceholder)
                return "*";
            return row.GetIndex();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
