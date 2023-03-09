using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ICSStudio.Cip.DataTypes
{
    [Serializable]
    [ComVisible(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct CipLint : IComparable, IConvertible, ICipDataType
    {
        private readonly long m_value;

        public CipLint(long v)
        {
            m_value = v;
        }

        public static implicit operator CipLint(long v)
        {
            return new CipLint(v);
        }

        public int CompareTo(object value)
        {
            if (value == null)
            {
                return 1;
            }

            if (value is CipLint)
            {
                // Need to use compare because subtraction will wrap
                // to positive for very large neg numbers, etc.
                CipLint i = (CipLint)value;
                if (m_value < i.m_value) return -1;
                if (m_value > i.m_value) return 1;
                return 0;
            }

            throw new ArgumentException("Arg must be CipLint");
        }

        public TypeCode GetTypeCode()
        {
            return TypeCode.Int64;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(m_value);
        }

        public char ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(m_value);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(m_value);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(m_value);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(m_value);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(m_value);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(m_value);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(m_value);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(m_value);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(m_value);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(m_value);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(m_value);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(m_value);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(m_value);
        }

        public string ToString(IFormatProvider provider)
        {
            return Convert.ToString(m_value);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(m_value, conversionType, provider);
        }

        public byte[] GetBytes()
        {
            return BitConverter.GetBytes(m_value);
        }

        public static CipLint Parse(byte[] data, ref int startIndex)
        {
            if (data.Length < sizeof(ulong) + startIndex)
                throw new OverflowException("Overflow CipLint");

            var b = BitConverter.ToInt64(data, startIndex);

            CipLint result = b;
            startIndex += sizeof(long);

            return result;

        }

        public static bool TryParse(byte[] data, ref int startIndex, out CipLint result)
        {
            result = new CipLint(0);

            if (data.Length < sizeof(ulong) + startIndex)
                return false;

            var b = BitConverter.ToInt64(data, startIndex);

            result = b;
            startIndex += sizeof(ulong);

            return true;
        }

        public static explicit operator CipLint(string v)
        {
            long value;
            if (long.TryParse(v, out value))
                return new CipLint(value);

            return new CipLint(0);
        }
    }
}
