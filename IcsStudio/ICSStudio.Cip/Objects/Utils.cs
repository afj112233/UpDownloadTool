using ICSStudio.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSStudio.Cip.Objects
{
    internal partial class Utils
    {

        public static List<byte> DeSerializeListByte(byte[] data)
        {
            int size = BitConverter.ToUInt16(data, 0);
            if (size > data.Length - 1)
            {
                throw new SerializeException("size mismatch");
            }

            List<byte> res = new List<byte>();
            for (int i = 0; i < size; ++i)
                res.Add(data[i + 2]);

            return res;
        }

        public static byte[] SerializeString(string str)
        {
            List<byte> res = new List<byte>();
            byte[] ser = System.Text.Encoding.ASCII.GetBytes(str);
            res.Add((byte)ser.Length);
            res.AddRange(ser);
            return res.ToArray();
        }

        public static string DeSerializeString(byte[] data)
        {
            int size = data[0];
            if (size > data.Length - 1)
            {
                throw new SerializeException("size mismatch");
            }

            return System.Text.Encoding.ASCII.GetString(data, 1, size);
        }

        public static byte[] SerializeListByte(List<byte> data)
        {
            var res = new List<byte> { };
            res.Add(0);
            res.Add(0);
            foreach (var d in data)
            {
                res.Add((byte)d);
            }

            var tmp = BitConverter.GetBytes((Int16)(res.Count - 2));
            res[0] = tmp[0];
            res[1] = tmp[1];
            return res.ToArray();
        }

        public static List<Int32> DeSerializeListInt32(byte[] data)
        {
            int size = BitConverter.ToUInt16(data, 0);
            if (size * 4 > data.Length - 1)
            {
                throw new SerializeException("size mismatch");
            }

            List<Int32> res = new List<Int32>();
            for (int i = 0; i < size; ++i)
                res.Add(BitConverter.ToInt32(data, 4 * i + 2));
            return res;
        }

        public static byte ToInt8(byte[] data, int offset)
        {
            return data[offset];
        }

        public static long ToInt64(byte[] data, int offset)
        {
            return BitConverter.ToInt64(data,offset);
        }
    }
}
