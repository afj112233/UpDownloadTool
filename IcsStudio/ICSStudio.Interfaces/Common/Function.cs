using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using ICSStudio.Interfaces.DataType;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICSStudio.Interfaces.Common
{
    public class ArgInfo
    {
        public IDataType Type;
        public bool IsRef;
    }



    public class Function
    {
        public int LocalsSize { get; }
        public int ArgsSize { get; }
        public List<byte> Codes { get; }
        public List<int> SafePoints { get; }
        [JsonIgnore] public List<ArgInfo> ArgInfos { get; }


        public static string EncodeByteArray(List<byte> data)
        {
            return System.Convert.ToBase64String(data.ToArray());
        }

        public static byte[] DecodeToByteArray(string data)
        {
            return System.Convert.FromBase64String(data);
        }


        public Function(List<byte> codes, List<int> safepoints, int localsSize, List<ArgInfo> argInfos = null)
        {
            Codes = codes;
            SafePoints = safepoints;
            LocalsSize = localsSize;
            if (argInfos == null)
            {
                ArgsSize = 0;
                ArgInfos = new List<ArgInfo>();
            }
            else
            {
                ArgsSize = argInfos.Count;
                ArgInfos = argInfos;
            }
        }

        public JObject ToJson()
        {
            return new JObject
            {
                {"LocalsSize", LocalsSize},
                {"ArgsSize", ArgsSize},
                {"Codes", Function.EncodeByteArray(Codes)},
                {"SafePoints", JArray.FromObject(SafePoints)}
            };
        }
    }
}