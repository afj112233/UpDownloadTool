using System.Collections.Generic;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.L5XToJson.Objects
{
    public class STRoutine
    {
        public int Type { get; set; } = (int)RoutineType.ST;
        public string Name { get; set; } = string.Empty;
        public List<string> CodeText { get; set; } = new List<string>();
    }
}
