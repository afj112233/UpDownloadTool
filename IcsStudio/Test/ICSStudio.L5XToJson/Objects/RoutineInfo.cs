using System.Collections.Generic;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.L5XToJson.Objects
{
    public class RoutineInfo
    {
        public int Type { get; set; } = (int)RoutineType.RLL;
        public string Name { get; set; } = string.Empty;
        public List<string> CodeText { get; set; } = new List<string>();

    }
}
