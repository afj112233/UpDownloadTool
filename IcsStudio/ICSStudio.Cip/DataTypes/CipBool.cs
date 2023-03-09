using System;
using System.Runtime.InteropServices;

namespace ICSStudio.Cip.DataTypes
{
    [Serializable]
    [ComVisible(true)]
    [StructLayout(LayoutKind.Sequential)]
    // IComparable<CipBool>,  IEquatable<CipBool>
    public struct CipBool : IComparable, IConvertible, ICipDataType
    {
        private readonly byte m_value;
        public static readonly CipBool MaxValue = 1;
        public static readonly CipBool MinValue = 0;

        public CipBool(byte v)
        {
            m_value = v;
        }

        public static implicit operator CipBool(byte v)
        {
            return new CipBool(v);
        }

        public int CompareTo(object value)
        {
            if (value == null)
                return 1;
            if (!(value is CipBool))
                throw new ArgumentException("Arg must be CipBool");

            return m_value - ((CipBool) value).m_value;
        }

        public byte[] GetBytes()
        {
            return new[] {m_value};
        }

        public static CipBool Parse(byte[] data, ref int startIndex)
        {
            if (data.Length < sizeof(byte) + startIndex)
                throw new OverflowException("Overflow CipBool");

            var b = data[startIndex];
            if (b > MaxValue.m_value)
                throw new OverflowException("Overflow CipBool");

            CipBool result = b;
            startIndex++;

            return result;
        }

        public static bool TryParse(byte[] data, ref int startIndex, out CipBool result)
        {
            result = new CipBool(0);

            if (data.Length < sizeof(byte) + startIndex)
                return false;

            var b = data[startIndex];
            if (b > MaxValue.m_value)
                return false;

            result = b;
            startIndex++;

            return true;
        }

        public TypeCode GetTypeCode()
        {
           return TypeCode.Byte;
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
            return m_value;
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
    }
}