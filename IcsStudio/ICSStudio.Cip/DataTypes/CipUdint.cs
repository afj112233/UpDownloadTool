using System;
using System.Runtime.InteropServices;

namespace ICSStudio.Cip.DataTypes
{

    /// <summary>
    /// UInt32,DWORD
    /// </summary>
    [Serializable]
    [ComVisible(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct CipUdint : IComparable, IConvertible, ICipDataType
    {
        private readonly uint m_value;

        public CipUdint(uint v)
        {
            m_value = v;
        }

        public static implicit operator CipUdint(uint v)
        {
            return new CipUdint(v);
        }

        public int CompareTo(object value)
        {
            if (value == null)
            {
                return 1;
            }

            if (value is CipUdint)
            {
                // Need to use compare because subtraction will wrap
                // to positive for very large neg numbers, etc.
                CipUdint i = (CipUdint) value;
                if (m_value < i.m_value) return -1;
                if (m_value > i.m_value) return 1;
                return 0;
            }

            throw new ArgumentException("Arg must be CipUdint");
        }

        public TypeCode GetTypeCode()
        {
            return TypeCode.UInt32;
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

        public static CipUdint Parse(byte[] data, ref int startIndex)
        {
            if (data.Length < sizeof(uint) + startIndex)
                throw new OverflowException("Overflow CipUdint");

            var b = BitConverter.ToUInt32(data, startIndex);

            CipUdint result = b;
            startIndex += sizeof(uint);

            return result;

        }

        public static bool TryParse(byte[] data, ref int startIndex, out CipUdint result)
        {
            result = new CipUdint(0);

            if (data.Length < sizeof(uint) + startIndex)
                return false;

            var b = BitConverter.ToUInt32(data, startIndex);

            result = b;
            startIndex += sizeof(uint);

            return true;
        }

        public static explicit operator CipUdint(string v)
        {
            uint value;
            if (uint.TryParse(v, out value))
                return new CipUdint(value);

            return new CipUdint(0);
        }
    }
}
