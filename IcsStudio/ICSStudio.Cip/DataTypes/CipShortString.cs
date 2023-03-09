using System;
using System.Collections.Generic;
using System.Text;
using ICSStudio.Cip.Objects;

namespace ICSStudio.Cip.DataTypes
{
    public class CipShortString : ICipDataType,IComparable
    {
        private readonly string _value;

        public CipShortString(string v)
        {
            _value = v;
        }

        public string GetString()
        {
            return _value;
        }

        public byte[] GetBytes()
        {
            if (string.IsNullOrEmpty(_value))
                return null;

            // TODO(gjc): ASCII is right?
            var byteValue = Encoding.ASCII.GetBytes(_value);

            var byteList = new List<byte>
            {
                (byte) byteValue.Length
            };
            
            byteList.AddRange(byteValue);

            return byteList.ToArray();
        }

        public static implicit operator CipShortString(string v)
        {
            return new CipShortString(v);
        }

        public static CipShortString Parse(byte[] data, ref int startIndex)
        {
            if (data.Length < sizeof(byte) + startIndex)
                throw new OverflowException("Overflow CipShortString");

            byte length = data[startIndex];
            startIndex++;

            if (data.Length < length + startIndex)
                throw new OverflowException("Overflow CipShortString");
            
            string name = Encoding.ASCII.GetString(data, startIndex, length);
            startIndex += length;

            return new CipShortString(name);
        }

        public static bool TryParse(byte[] data, ref int startIndex, out CipShortString result)
        {
            result = new CipShortString(string.Empty);

            if (data.Length < sizeof(byte) + startIndex)
                return false;

            byte length = data[startIndex];
            startIndex++;

            if (data.Length < length + startIndex)
                return false;

            string name = Encoding.ASCII.GetString(data, startIndex, length);
            startIndex += length;

            result = name;

            return true;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is CipShortString))
                throw new ArgumentException("Arg must be CipShortString");

            CipShortString cipShortString = (CipShortString)obj;

            byte[] bytes1 = GetBytes();
            byte[] bytes2 = cipShortString.GetBytes();

            return CipAttributeHelper.BytesCompareTo(bytes1, bytes2);
        }
    }
}