using System;
using System.Runtime.InteropServices;

namespace ICSStudio.Cip.DataTypes
{
    [Serializable]
    [ComVisible(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct CipInt : IComparable, IConvertible, ICipDataType
    {
        private readonly short m_value;

        private CipInt(short v)
        {
            m_value = v;
        }

        public static implicit operator CipInt(short v)
        {
            return new CipInt(v);
        }

        public int CompareTo(object value)
        {
            if (value == null)
            {
                return 1;
            }
            if (value is CipInt)
            {
                return m_value - ((CipInt)value).m_value;
            }
            throw new ArgumentException("Arg must be CipInt");
        }

        public TypeCode GetTypeCode()
        {
            return TypeCode.Int16;
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
            return (ushort) m_value;
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

        public override string ToString()
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
        public static CipInt Parse(byte[] data, ref int startIndex)
        {
            if (data.Length < sizeof(short) + startIndex)
                throw new OverflowException("Overflow CipInt");

            var b = BitConverter.ToInt16(data, startIndex);

            CipInt result = b;
            startIndex += sizeof(short);

            return result;
        }

        public static bool TryParse(byte[] data, ref int startIndex, out CipInt result)
        {
            result = new CipInt(0);

            if (data.Length < sizeof(short) + startIndex)
                return false;

            var b = BitConverter.ToInt16(data, startIndex);

            result = b;
            startIndex += sizeof(short);

            return true;
        }
    }
}
