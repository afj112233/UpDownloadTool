using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSStudio.DeviceProfiles.MotionDrive2.Common
{
    public class ModuleConnectionDefinition
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public uint? MinRPI { get; set; }
        public uint? MaxRPI { get; set; }
        public uint? RPI { get; set; }
        public uint? InputCxnPoint { get; set; }
        public uint? OutputCxnPoint { get; set; }

        public ModuleTag InputTag { get; set; }
        public ModuleTag OutputTag { get; set; }
    }

    public class ModuleTag
    {
        public string DataType { get; set; }
    }

}
