using System;
using System.Collections.Generic;
using ICSStudio.Cip.Objects;

namespace ICSStudio.Cip.DataTypes
{
    public class CipRealArray:ICipDataType,IComparable
    {
        private readonly float[] _array;

        public CipRealArray(int count)
        {
            if (count > 0)
                _array = new float[count];
        }
        
        public void SetValue(int index, float newValue)
        {
            if (_array != null && _array.Length > index)
            {
                _array[index] = newValue;
            }
        }

        public float GetValue(int index)
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

            var count = (ushort) _array.Length;
            byteList.AddRange(BitConverter.GetBytes(count));

            foreach (var item in _array)
            {
                byteList.AddRange(BitConverter.GetBytes(item));
            }

            return byteList.ToArray();
        }

        public static CipRealArray Parse(byte[] data, ref int startIndex)
        {

            if (data.Length < sizeof(ushort) + startIndex)
                throw new OverflowException("Overflow CipRealArray");

            ushort count = BitConverter.ToUInt16(data, startIndex);
            startIndex += sizeof(ushort);
            
            if (data.Length < sizeof(float)*count + startIndex)
                throw new OverflowException("Overflow CipRealArray");

            CipRealArray result = new CipRealArray(count);

            for (int i = 0; i < count; i++)
            {
                result.SetValue(i, BitConverter.ToSingle(data, startIndex));
                startIndex += sizeof(float);
            }

            return result;
        }

        public static bool TryParse(byte[] data, ref int startIndex, out CipRealArray result)
        {
            result = null;

            if (data.Length < sizeof(ushort) + startIndex)
                return false;

            ushort count = BitConverter.ToUInt16(data, startIndex);
            startIndex += sizeof(ushort);

            if (data.Length < sizeof(float)*count + startIndex)
                return false;
            
            result = new CipRealArray(count);

            for (int i = 0; i < count; i++)
            {
                result.SetValue(i, BitConverter.ToSingle(data, startIndex));
                startIndex += sizeof(float);
            }

            return true;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is CipRealArray))
                throw new ArgumentException("Arg must be CipRealArray");

            CipRealArray cipRealArray = (CipRealArray) obj;

            byte[] bytes1 = GetBytes();
            byte[] bytes2 = cipRealArray.GetBytes();

            return CipAttributeHelper.BytesCompareTo(bytes1, bytes2);
        }
    }
}
