using System;
using System.Runtime.InteropServices;

namespace ICSStudio.Cip.Objects
{
    
    [AttributeUsage(AttributeTargets.Property)]
    public class CipDetailInfoAttribute : Attribute
    {
        public CipDetailInfoAttribute(
            ushort id, 
            string displayName,
            string unit="",
            int arraySize = 1)
        {
            Id = id;
            DisplayName = displayName;
            Unit = unit;
            ArraySize = arraySize;
        }

        public ushort Id { get; }
        public string DisplayName { get; }
        // "$Units"
        public string Unit { get; set; }

        public int ArraySize { get; set; }

        public static byte[] ConvertTo(object value)
        {
            // TODO(gjc):add handle code 

            return null;
        }

        public static object ConvertFrom(byte[] data, Type type, ref int useLength)
        {
            // TODO(gjc):add string convert

            if (data.Length < Marshal.SizeOf(type))
                return null;
            var num = Marshal.AllocHGlobal(data.Length);
            try
            {
                Marshal.Copy(data, 0, num, data.Length);
                useLength = Marshal.SizeOf(type);
                return Marshal.PtrToStructure(num, type);
            }
            finally
            {
                Marshal.FreeHGlobal(num);
            }
        }
    }
}