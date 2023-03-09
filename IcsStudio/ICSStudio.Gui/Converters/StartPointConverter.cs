using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ICSStudio.Gui.Converters
{
    public class StartPointConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var actualWidth = (double) values[0];
            var actualHeight = (double) values[1];

            if (Math.Abs(actualHeight) < double.Epsilon)
                return null;

            if (Math.Abs(actualWidth) < double.Epsilon)
                return null;

            var thickness = (Thickness) values[2];
            var up = bool.Parse(values[3].ToString());

            return GetStartPoint(actualHeight - thickness.Top - thickness.Bottom, up);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static Point GetStartPoint(double bottom, bool up)
        {
            if (up)
                return new Point(5.0, bottom - 3.0);

            return new Point(5.0, 3.0);
        }
    }
}