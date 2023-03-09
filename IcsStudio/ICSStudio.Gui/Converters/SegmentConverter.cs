using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ICSStudio.Gui.Converters
{
    public class SegmentConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            double actualWidth = (double) values[0];
            double actualHeight = (double) values[1];

            if (Math.Abs(actualHeight) < double.Epsilon)
                return null;

            if (Math.Abs(actualWidth) < double.Epsilon)
                return null;

            Thickness thickness = (Thickness) values[2];
            bool up = bool.Parse(values[3].ToString());
            double bottom = actualHeight - thickness.Top - thickness.Bottom;
            return GetSegment(actualWidth - thickness.Left - thickness.Right, bottom, up);

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static PathSegmentCollection GetSegment(double right, double bottom, bool up)
        {
            PathSegmentCollection segmentCollection = new PathSegmentCollection();

            if (up)
            {
                segmentCollection.Add((PathSegment) new LineSegment(new Point(right / 2.0, 3.0), true));
                segmentCollection.Add((PathSegment) new LineSegment(new Point(right - 5.0, bottom - 3.0), true));
            }
            else
            {
                segmentCollection.Add((PathSegment) new LineSegment(new Point(right / 2.0, bottom - 3.0), true));
                segmentCollection.Add((PathSegment) new LineSegment(new Point(right - 5.0, 3.0), true));
            }

            return segmentCollection;
        }
    }
}
