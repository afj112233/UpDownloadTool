using System.Collections.Generic;

namespace ICSStudio.DeviceProfiles.DIOEnetAdapter.Common
{
    public class Connection
    {
        public string StringID { get; set; }
        public List<ConnectionChoice> Choices { get; set; }
    }

    public class ConnectionChoice
    {
        public string ID { get; set; }
        public int StringID { get; set; }
        public uint CommMethod { get; set; }
        public uint ConfigID { get; set; }
        public bool Default { get; set; }
    }
}
