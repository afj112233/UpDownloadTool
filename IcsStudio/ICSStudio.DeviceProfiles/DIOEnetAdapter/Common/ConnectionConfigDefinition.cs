using System.Collections.Generic;

namespace ICSStudio.DeviceProfiles.DIOEnetAdapter.Common
{
    public class ConnectionConfigDefinition
    {
        public int ConfigID { get; set; }
        public Tag ConfigTag { get; set; }
        public List<string> Connections { get; set; }
    }
    
}
