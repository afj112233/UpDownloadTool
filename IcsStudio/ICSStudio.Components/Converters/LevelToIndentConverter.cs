using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ICSStudio.Components.Converters
{
    public class LevelToIndentConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            Contract.Assert(value != null);
            Contract.Assert(parameter != null);

            return new Thickness((int) value * (int) parameter, 0.0, 0.0, 0.0);
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
