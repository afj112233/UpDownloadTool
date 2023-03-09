using System.Collections.Generic;

namespace ICSStudio.DeviceProfiles.DIOModule.Common
{
    public class IOConnectionConfigDefinition
    {
        public uint ConfigID { get; set; }
        public IOConfigTag ConfigTag { get; set; }
        public List<string> Connections { get; set; }
    }

    public class IOConfigTag
    {
        public string ValueID { get; set; }
        public int Instance { get; set; }
        public string DataType { get; set; }
    }
}
