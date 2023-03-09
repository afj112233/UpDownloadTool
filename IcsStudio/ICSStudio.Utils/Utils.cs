using System;
using System.Linq;
using System.Collections.Generic;
using Crc32C;
using Newtonsoft.Json.Linq;

namespace ICSStudio.Utils
{
    public class SerializeException : ICSStudioException
    {
        public SerializeException(string str) : base(str)
        {
        }
    }

    public class Utils
    {
        public static JObject SortJObject(JObject obj)
        {
            var res = new JObject();
            var pairs = new List<KeyValuePair<string, JToken>>();
            foreach (var pair in obj)
            {
                pairs.Add(pair);
            }

            foreach (var pair in pairs.OrderBy(p => p.Key))
            {
                res[pair.Key] = pair.Value;
            }

            return res;
        }

        public static string BytesToHex(byte[] buffer)
        {
            return string.Join(":", buffer.Select(v => v.ToString("X02")));
        }

        public static string HashByCrc32C(string data)
        {
            var bytes = System.Text.Encoding.ASCII.GetBytes(data);
            uint crc = Crc32CAlgorithm.Compute(bytes);

            return crc.ToString("X8");
        }

        public static string GetNotDuplicateName(string name, List<string> names, int index = 0)
        {
            var n = name;
            if (index > 0)
            {
                n = $"{name}{index}";
            }

            foreach (var name1 in names)
            {
                if (name1.Equals(n, StringComparison.OrdinalIgnoreCase))
                {
                    return GetNotDuplicateName(name, names, ++index);
                }
            }

            return n;
        }

        public static int SequenceCompare(long value0, long value1)
        {
            long result = value0 - value1;

            if (result == 0)
                return 0;

            if (result > 0)
                return 1;

            return -1;

        }
    }

    public class BinaryReader
    {
        public BinaryReader(byte[] data)
        {
            this.data = data;
            offset = 0;
        }

        public byte ReadByte()
        {
            var res = data[offset];
            offset += 1;
            return res;
        }

        public Int16 ReadInt16()
        {
            var res = BitConverter.ToInt16(data, offset);
            offset += 2;
            return res;
        }

        public Int32 ReadInt32()
        {
            var res = BitConverter.ToInt32(data, offset);
            offset += 4;
            return res;
        }

        public string ReadString()
        {
            int size = data[offset];
            if (size > data.Length - 1)
            {
                throw new SerializeException("size mismatch");
            }

            offset += 1;

            var res = System.Text.Encoding.ASCII.GetString(data, offset, size);
            offset += size;
            return res;
        }


        public List<byte> ReadBytes()
        {
            int size = BitConverter.ToUInt16(data, offset);
            if (size > data.Length - 1)
            {
                throw new SerializeException("size mismatch");
            }

            offset += 2;
            List<byte> res = new List<byte>();
            for (int i = 0; i < size; ++i)
            {
                res.Add(data[offset]);
                offset += 1;
            }

            return res;
        }

        public List<Int32> DeSerializeListInt32(byte[] data)
        {
            int size = BitConverter.ToUInt16(data, offset);
            if (size * 4 > data.Length - 1)
            {
                throw new SerializeException("size mismatch");
            }

            offset += 2;

            List<Int32> res = new List<Int32>();
            for (int i = 0; i < size; ++i)
            {
                res.Add(BitConverter.ToInt32(data, offset));
                offset += 4;
            }

            return res;
        }

        private byte[] data;
        private int offset = 0;

    }


}
