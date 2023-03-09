using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace ICSStudio.Gui.Converters
{
    public class BinaryConverter : IValueConverter
    {
        private int _value;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            _value = (int) value;

            return new BitArray(new[] {_value})[(short) parameter] ? "1" : "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            BitArray bitArray = new BitArray(new[] {_value});
            int index = (short) parameter;

            bool flag = value.ToString() == "1";

            bitArray[index] = flag;

            int[] numArray = new int[1];
            bitArray.CopyTo(numArray, 0);
            return numArray[0];
        }
    }
}
