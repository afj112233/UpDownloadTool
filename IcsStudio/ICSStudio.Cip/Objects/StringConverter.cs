using System;
using System.Text;

namespace ICSStudio.Cip.Objects
{
    public class StringConverter
    {
        public static string ToString(byte[] buffer, ref int offset)
        {
            ushort length = BitConverter.ToUInt16(buffer, offset);
            offset += 2;

            byte[] stringBytes = new byte[length];
            Array.Copy(buffer, offset, stringBytes, 0, length);
            offset += length;

            return Encoding.ASCII.GetString(stringBytes);
        }

        public static string ToShortString(byte[] buffer, ref int offset)
        {
            byte length = buffer[offset];
            offset += 1;

            byte[] stringBytes = new byte[length];
            Array.Copy(buffer, offset, stringBytes, 0, length);
            offset += length;

            return Encoding.ASCII.GetString(stringBytes);
        }
        
    }
}
