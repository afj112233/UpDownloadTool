using System;
using System.Runtime.InteropServices;

namespace ICSStudio.Cip.DataTypes
{
    [Serializable]
    [ComVisible(true)]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CipRevision : ICipDataType, IComparable
    {
        public byte Major;
        public byte Minor;

        public byte[] GetBytes()
        {
            return new[] {Major, Minor};
        }

        public override string ToString()
        {
            return $"{Major}.{Minor:D3}";
        }

        public static CipRevision Parse(byte[] data, ref int startIndex)
        {
            if (data.Length < 2 + startIndex)
                throw new OverflowException("Overflow CipRevision");

            CipRevision result;

            result.Major = data[startIndex];
            result.Minor = data[startIndex + 1];

            startIndex += 2;

            return result;
        }

        public static bool TryParse(byte[] data, ref int startIndex, out CipRevision result)
        {
            result = new CipRevision();

            if (data.Length < 2 + startIndex)
                return false;

            result.Major = data[startIndex];
            result.Minor = data[startIndex + 1];

            startIndex += 2;

            return true;
        }

        public int CompareTo(object value)
        {
            if (!(value is CipRevision))
                throw new ArgumentException("Arg must be CipRevision");

            CipRevision cipRevision = (CipRevision) value;
            if (cipRevision.Major == Major && cipRevision.Minor == Minor)
                return 0;
            if (Major > cipRevision.Major)
                return 1;
            if (Major < cipRevision.Major)
                return -1;
            if (Minor > cipRevision.Minor)
                return 1;
            return -1;
        }
    }
}