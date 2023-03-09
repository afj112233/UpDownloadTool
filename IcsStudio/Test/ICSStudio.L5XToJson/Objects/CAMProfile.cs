using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSStudio.L5XToJson.Objects
{
    public class CAMProfile
    {
        private static double ToDouble(int left, int right)
        {
            var arr = new List<byte>();
            arr.AddRange(BitConverter.GetBytes(left).Reverse());
            arr.AddRange(BitConverter.GetBytes(right).Reverse());
            arr.Reverse();
            return BitConverter.ToDouble(arr.ToArray(), 0);
        }

        public static JArray ToJObject(string data)
        {
            data = data.Trim();
            Debug.Assert(data.StartsWith("[") && data.EndsWith("]"), data);
            data = data.TrimStart('[');
            data = data.TrimEnd(']');
            var array = data.Split(',');
            Debug.Assert(array.Length == 14, array.Length.ToString());

            JArray res = new JArray();
            //把类型字段提到前面来。方便对齐
            res.Add(int.Parse(array[0]));
            res.Add(int.Parse(array[5]));
            res.Add(ToDouble(int.Parse(array[1]), int.Parse(array[2])));
            res.Add(ToDouble(int.Parse(array[3]), int.Parse(array[4])));
            res.Add(ToDouble(int.Parse(array[6]), int.Parse(array[7])));
            res.Add(ToDouble(int.Parse(array[8]), int.Parse(array[9])));
            res.Add(ToDouble(int.Parse(array[10]), int.Parse(array[11])));
            res.Add(ToDouble(int.Parse(array[12]), int.Parse(array[13])));
            return res;
        }

    }
}
