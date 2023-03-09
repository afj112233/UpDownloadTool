using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSStudio.DeviceProfiles.MotionDrive2.Common
{
    public class ModuleConnectionConfigDefinition
    {
        public uint ConfigID { get; set; }
        public ModuleConfigTag ConfigTag { get; set; }
        public List<string> Connections { get; set; }
    }

    public class ModuleConfigTag
    {
        public string ValueID { get; set; }
        public int Instance { get; set; }
        public string DataType { get; set; }
        public List<string> Enums { get; set; }
    }
}
