using System;
using System.Runtime.InteropServices;

namespace ICSStudio.Cip.DataTypes
{
    [Serializable]
    [ComVisible(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct CipReal : IComparable, IConvertible, ICipDataType
    {
        private readonly float m_value;

        public CipReal(float v)
        {
            m_value = v;
        }

        public static unsafe bool IsNaN(float f)
        {
            return (*(int*) &f & 0x7FFFFFFF) > 0x7F800000;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            if (obj is CipReal)
            {
                var f = ((CipReal) obj).m_value;
                if (m_value < f) return -1;
                if (m_value > f) return 1;
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (m_value == f) return 0;

                // At least one of the values is NaN.
                if (IsNaN(m_value))
                    return IsNaN(f) ? 0 : -1;

                return 1;
            }

            throw new ArgumentException("Arg must be CipReal");
        }

        public TypeCode GetTypeCode()
        {
            return TypeCode.Single;
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
            return m_value;
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
            return m_value.ToString(provider);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(m_value, conversionType, provider);
        }

        public byte[] GetBytes()
        {
            return BitConverter.GetBytes(m_value);
        }


        public static CipReal Parse(byte[] data, ref int startIndex)
        {
            if (data.Length < sizeof(float) + startIndex)
                throw new OverflowException("Overflow CipReal");

            var b = BitConverter.ToSingle(data, startIndex);

            CipReal result = b;
            startIndex += sizeof(float);

            return result;
        }

        public static bool TryParse(byte[] data, ref int startIndex, out CipReal result)
        {
            result = new CipReal(0);

            if (data.Length < sizeof(float) + startIndex)
                return false;

            var b = BitConverter.ToSingle(data, startIndex);

            result = b;
            startIndex += sizeof(float);

            return true;
        }

        public static implicit operator CipReal(float v)
        {
            return new CipReal(v);
        }

        public override string ToString()
        {
            return m_value.ToString("g9");
        }

        public static explicit operator CipReal(string v)
        {
            float value;
            if (float.TryParse(v, out value))
                return new CipReal(value);

            return new CipReal(0);
        }
    }
}