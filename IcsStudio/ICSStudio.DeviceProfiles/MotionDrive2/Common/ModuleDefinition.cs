using System.Collections.Generic;

namespace ICSStudio.DeviceProfiles.MotionDrive2.Common
{
    public class ModuleDefinition
    {
        public string ID { get; set; }
        public string StringsID { get; set; }
        public ModuleConnection Connection { get; set; }
    }

    public class ModuleConnection
    {
        public int StringID { get; set; }
        public List<ModuleConnectionChoice> Choices { get; set; }
    }

    public class ModuleConnectionChoice
    {
        public string ID { get; set; }
        public int StringID { get; set; }

        public uint ConfigID { get; set; }
    }
}
