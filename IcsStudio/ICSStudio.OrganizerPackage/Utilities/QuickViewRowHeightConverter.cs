using System;
using System.Globalization;
using System.Windows.Data;

namespace ICSStudio.OrganizerPackage.Utilities
{
    [ValueConversion(typeof(double), typeof(string))]
    internal class QuickViewRowHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double))
                return "Hide Quick View";

            double height = (double) value;
            return height == 0 ? "Show Quick View" : "Hide Quick View";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
