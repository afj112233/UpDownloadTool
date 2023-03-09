using System;
using System.Collections.Generic;
using System.Text;
using ICSStudio.Cip.Objects;

namespace ICSStudio.Cip.DataTypes
{
    public class CipString: ICipDataType,IComparable
    {
        private readonly string _value;
        public CipString(string v)
        {
            _value = v;
        }

        public string GetString()
        {
            return _value;
        }

        public static implicit operator CipString(string v)
        {
            return new CipString(v);
        }

        public byte[] GetBytes()
        {
            if (string.IsNullOrEmpty(_value))
                return null;
            
            var byteValue = Encoding.ASCII.GetBytes(_value);

            var byteList = new List<byte>();
            
            byteList.AddRange(byteValue);

            return byteList.ToArray();
        }
        // startIndex to endIndex
        public static CipString Parse(byte[] data, ref int startIndex)
        {
            if (data == null)
            {
                return new CipString("");
            }
            if (data.Length <= startIndex)
            {
                throw new OverflowException($"{data.Length}:{startIndex} Overflow CipString");
            }

            int length = data.Length - startIndex;
            
            string name = Encoding.ASCII.GetString(data, startIndex, length);
            startIndex += length;

            return new CipString(name);
        }
        public static bool TryParse(byte[] data, ref int startIndex, out CipString result)
        {
            result = new CipString(string.Empty);

            if (data.Length <=  startIndex)
                return false;

            int length = data.Length - startIndex;
            
            string name = Encoding.ASCII.GetString(data, startIndex, length);
            startIndex += length;

            result = name;

            return true;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is CipString))
                throw new ArgumentException("Arg must be CipString");

            CipString cipString = (CipString)obj;

            byte[] bytes1 = GetBytes();
            byte[] bytes2 = cipString.GetBytes();

            return CipAttributeHelper.BytesCompareTo(bytes1, bytes2);
        }

        public override string ToString()
        {
            return _value;
        }
    }
}
