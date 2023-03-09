using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ICSStudio.Interfaces.Common
{
    public class Function
    {
        public int LocalsSize { get; }
        public int ArgsSize { get; }
        public List<byte> Codes { get; }
        public List<int> SafePoints { get; }

        public Function(List<byte> codes, List<int> safepoints, int localsSize, int argsSize = 0)
        {
            Codes = codes;
            SafePoints = safepoints;
            LocalsSize = localsSize;
            ArgsSize = argsSize;
        }

        public JObject ToJson()
        {
            return new JObject
            {
                {"LocalsSize", LocalsSize},
                {"Codes", JArray.FromObject(Codes)},
                {"SafePoints", JArray.FromObject(SafePoints)}
            };
        }
    }
}