using System;
using System.Runtime.InteropServices;

namespace ICSStudio.Cip.DataTypes
{
    /// <summary>
    /// UInt8
    /// </summary>
    [Serializable]
    [ComVisible(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct CipUsint : IComparable, IConvertible, ICipDataType
    {
        private readonly byte m_value;
        public static readonly CipUsint MaxValue = byte.MaxValue;
        public static readonly CipUsint MinValue = byte.MinValue;

        public CipUsint(byte v)
        {
            m_value = v;
        }

        public static implicit operator CipUsint(byte v)
        {
            return new CipUsint(v);
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            if (!(obj is CipUsint))
                throw new ArgumentException("Arg must be CipUsint");

            return m_value - ((CipUsint) obj).m_value;
        }

        public byte[] GetBytes()
        {
            return new[] {m_value};
        }

        public static CipUsint Parse(byte[] data, ref int startIndex)
        {
            if (data.Length < sizeof(byte) + startIndex)
                throw new OverflowException("Overflow CipUsint");

            var b = data[startIndex];

            CipUsint result = b;
            startIndex++;

            return result;
        }

        public static bool TryParse(byte[] data, ref int startIndex, out CipUsint result)
        {
            result = new CipUsint(0);

            if (data.Length < sizeof(byte) + startIndex)
                return false;

            var b = data[startIndex];

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

        public static explicit operator CipUsint(string v)
        {
            byte value;
            if (byte.TryParse(v, out value))
                return new CipUsint(value);

            return new CipUsint(0);
        }
    }
}
