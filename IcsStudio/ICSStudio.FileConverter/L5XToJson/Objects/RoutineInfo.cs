using System;
using System.Collections.Generic;
using ICSStudio.Interfaces.Common;
using Newtonsoft.Json;

namespace ICSStudio.FileConverter.L5XToJson.Objects
{
    public class RoutineInfo
    {
        public int Type { get; set; } = (int) RoutineType.RLL;
        public string Name { get; set; } = string.Empty;
        public string Use { set; get; } = "Context";

        public string Description { set; get; }

        public List<string> CodeText { get; } = new List<string>();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> PendingCodeText { set;get; } 

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> TestCodeText { set;get; }

        [Obsolete]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<KeyValuePair<int, string>> RungComments { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<RungInfo> Rungs { get; set; }
    }

    public class RungInfo
    {
        public uint Number { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Comment { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Use { get; set; }
    }
}
