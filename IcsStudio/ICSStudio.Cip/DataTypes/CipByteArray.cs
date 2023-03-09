using System;
using System.Collections.Generic;
using ICSStudio.Cip.Objects;

namespace ICSStudio.Cip.DataTypes
{
    public class CipByteArray : ICipDataType, IComparable
    {
        private readonly byte[] _array;

        public CipByteArray(int count)
        {
            if (count > 0)
                _array = new byte[count];
        }

        public void SetValue(int index, byte newValue)
        {
            if (_array != null && _array.Length > index)
            {
                _array[index] = newValue;
            }
        }

        public byte GetValue(int index)
        {
            if (_array != null && _array.Length > index)
            {
                return _array[index];
            }

            return 0;
        }

        public int GetCount()
        {
            if (_array != null)
                return _array.Length;
            return 0;
        }


        public byte[] GetBytes()
        {
            if (_array == null)
                return null;

            var byteList = new List<byte>();

            // fix count
            //var count = (ushort)_array.Length;
            //byteList.AddRange(BitConverter.GetBytes(count));

            foreach (var item in _array)
            {
                byteList.Add(item);
            }

            return byteList.ToArray();
        }

        public static CipByteArray Parse(int count, byte[] data, ref int startIndex)
        {

            if (data.Length < sizeof(ushort) + startIndex)
                throw new OverflowException("Overflow CipByteArray");

            //ushort count = BitConverter.ToUInt16(data, startIndex);
            //startIndex += sizeof(ushort);

            //if (data.Length < sizeof(byte) * count + startIndex)
            //    throw new OverflowException("Overflow CipByteArray");

            CipByteArray result = new CipByteArray(count);

            for (int i = 0; i < count; i++)
            {
                result.SetValue(i, data[startIndex]);
                startIndex += sizeof(byte);
            }

            return result;
        }

        public static bool TryParse(int count, byte[] data, ref int startIndex, out CipByteArray result)
        {
            result = null;

            if (data.Length < sizeof(ushort) + startIndex)
                return false;

            //ushort count = BitConverter.ToUInt16(data, startIndex);
            //startIndex += sizeof(ushort);

            //if (data.Length < sizeof(byte) * count + startIndex)
            //    return false;

            result = new CipByteArray(count);

            for (int i = 0; i < count; i++)
            {
                result.SetValue(i, data[startIndex]);
                startIndex += sizeof(byte);
            }

            return true;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is CipByteArray))
                throw new ArgumentException("Arg must be CipByteArray");

            CipByteArray cipByteArray = (CipByteArray)obj;

            byte[] bytes1 = GetBytes();
            byte[] bytes2 = cipByteArray.GetBytes();

            return CipAttributeHelper.BytesCompareTo(bytes1, bytes2);
        }

        public override string ToString()
        {
            if (_array == null)
                return string.Empty;

            if (_array.Length == 0)
                return "[]";

            return $"[{string.Join(",", _array)}]";
        }
    }
}
