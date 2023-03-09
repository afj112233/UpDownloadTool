using System;
using System.Runtime.InteropServices;

namespace ICSStudio.Cip.DataTypes
{
    [Serializable]
    [ComVisible(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct CipSint : IComparable, IConvertible,ICipDataType
    {
        private readonly sbyte m_value;
        public static readonly CipSint MaxValue = sbyte.MaxValue;
        public static readonly CipSint MinValue = sbyte.MinValue;

        public CipSint(sbyte v)
        {
            m_value = v;
        }

        public static implicit operator CipSint(sbyte v)
        {
            return new CipSint(v);
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            if (!(obj is CipSint))
                throw new ArgumentException("Arg must be CipSint");

            return m_value - ((CipSint) obj).m_value;
        }

        public byte[] GetBytes()
        {
            return new[] {(byte) m_value};
        }

        public static CipSint Parse(byte[] data, ref int startIndex)
        {
            if (data.Length < sizeof(sbyte) + startIndex)
                throw new OverflowException("Overflow CipSint");

            var b = (sbyte) data[startIndex];

            CipSint result = b;
            startIndex++;

            return result;
        }

        public static bool TryParse(byte[] data, ref int startIndex, out CipSint result)
        {
            result = new CipSint(0);

            if (data.Length < sizeof(sbyte) + startIndex)
                return false;

            var b = (sbyte) data[startIndex];

            result = b;
            startIndex++;

            return true;
        }

        public TypeCode GetTypeCode()
        {
            return TypeCode.SByte;
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
            return m_value;
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

        public override string ToString()
        {
            return Convert.ToString(m_value);
        }
    }
}