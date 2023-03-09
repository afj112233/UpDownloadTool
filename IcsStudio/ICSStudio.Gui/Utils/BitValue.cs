using System;

namespace ICSStudio.Gui.Utils
{
    public class BitValue
    {
        public static bool Get<T>(T value, int index)
        {
            var intType = value.GetType();
            int bitLength;

            if (intType == typeof(byte))
                bitLength = 8;
            else if (intType == typeof(sbyte))
                bitLength = 8;
            else if (intType == typeof(short))
                bitLength = 16;
            else if (intType == typeof(int))
                bitLength = 32;
            else if (intType == typeof(long))
                bitLength = 64;
            else
                throw new ArgumentException("must integer!");


            if (index > bitLength - 1 || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            object intObject = value;

            if (intType == typeof(long))
            {
                var tempBit = 1L << index;
                return ((long) intObject & tempBit) == tempBit;
            }

            if (intType == typeof(int))
            {
                var tempBit = 1 << index;
                return ((int) intObject & tempBit) == tempBit;
            }

            if (intType == typeof(short))
            {
                var tempBit = 1 << index;
                return ((short) intObject & tempBit) == tempBit;
            }

            if (intType == typeof(byte))
            {
                var tempBit = 1 << index;
                return ((byte) intObject & tempBit) == tempBit;
            }

            if (intType == typeof(sbyte))
            {
                var tempBit = 1 << index;
                return ((sbyte)intObject & tempBit) == tempBit;
            }

            throw new ArgumentException("not run here!");
        }

        public static T Set<T>(T value, int index, bool boolValue)
        {
            var intType = value.GetType();
            int bitLength;

            if (intType == typeof(byte))
                bitLength = 8;
            else if (intType == typeof(sbyte))
                bitLength = 8;
            else if (intType == typeof(short))
                bitLength = 16;
            else if (intType == typeof(int))
                bitLength = 32;
            else if (intType == typeof(long))
                bitLength = 64;
            else
                throw new ArgumentException("must integer!");


            if (index > bitLength - 1 || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            object intObject = value;
            if (intType == typeof(long))
            {
                var tempBit = 1L << index;

                if (boolValue) return (T) (object) ((long) intObject | tempBit);

                return (T) (object) ((long) intObject & ~tempBit);
            }

            if (intType == typeof(int))
            {
                var tempBit = 1 << index;
                if (boolValue) return (T) (object) ((int) intObject | tempBit);

                return (T) (object) ((int) intObject & ~tempBit);
            }

            if (intType == typeof(short))
            {
                var tempBit = (short) (1 << index);
                short result;
                if (boolValue)
                    result = (short) ((short) intObject | tempBit);
                else
                    result = (short) ((short) intObject & ~tempBit);

                return (T) (object) result;
            }

            if (intType == typeof(byte))
            {
                var tempBit = (byte) (1 << index);
                byte result;
                if (boolValue)
                    result = (byte) ((byte) intObject | tempBit);
                else
                    result = (byte) ((byte) intObject & ~tempBit);

                return (T) (object) result;
            }

            if (intType == typeof(sbyte))
            {
                var tempBit = (byte)(1 << index);
                sbyte result;
                if (boolValue)
                    result = (sbyte)(byte)((byte)(sbyte)intObject | tempBit);
                else
                    result = (sbyte)(byte)((byte)(sbyte)intObject & ~tempBit);

                return (T)(object)result;
            }

            throw new ArgumentException("not run here!");
        }
    }
}
