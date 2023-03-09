using System;

namespace ICSStudio.Gui.Utils
{
    public class FlagsEnumHelper
    {
        public static bool ContainFlag<T>(byte bits, T flag)
        {
            byte flagValue = Convert.ToByte(flag);

            return (bits & flagValue) != 0;
        }

        public static bool ContainFlag<T>(ushort bits, T flag)
        {
            ushort flagValue = Convert.ToUInt16(flag);

            return (bits & flagValue) != 0;
        }

        public static bool ContainFlag<T>(uint bits, T flag)
        {
            uint flagValue = Convert.ToUInt32(flag);

            return (bits & flagValue) != 0;
        }

        public static void SelectFlag<T>(T flag, bool selected, ref byte bits)
        {
            byte flagValue = Convert.ToByte(flag);

            if (selected)
            {
                bits |= flagValue;
            }
            else
            {
                flagValue = (byte) (~flagValue);
                bits &= flagValue;
            }

        }

        public static void SelectFlag<T>(T flag, bool selected, ref ushort bits)
        {
            ushort flagValue = Convert.ToUInt16(flag);

            if (selected)
            {
                bits |= flagValue;
            }
            else
            {
                flagValue = (ushort) (~flagValue);
                bits &= flagValue;
            }

        }

        public static void SelectFlag<T>(T flag, bool selected, ref uint bits)
        {
            uint flagValue = Convert.ToUInt32(flag);

            if (selected)
            {
                bits |= flagValue;
            }
            else
            {
                flagValue = (ushort) (~flagValue);
                bits &= flagValue;
            }

        }
    }
}
