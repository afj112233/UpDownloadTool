using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ICSStudio.Gui.Controls;

namespace ICSStudio.Gui.Converters
{
    public class DataTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            DataType dataType = (DataType) value;
            short rowIndex = (short) parameter; // 1,2,3,4

            switch (dataType)
            {
                case DataType.SINT:
                {
                    if (rowIndex == 1)
                        return Visibility.Visible;
                    return Visibility.Collapsed;
                }

                case DataType.INT:
                {
                    if (rowIndex == 1 || rowIndex == 2)
                        return Visibility.Visible;
                    return Visibility.Collapsed;
                }

                case DataType.DINT:
                {
                    if (rowIndex == 1 || rowIndex == 2 || rowIndex == 3 || rowIndex == 4)
                        return Visibility.Visible;
                    return Visibility.Collapsed;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
