using System.Collections.Generic;

namespace ICSStudio.DeviceProfiles.DIOEnetAdapter.Common
{
    public class CIPObject
    {
        public string ID { get; set; }
        public int TimeOut { get; set; }
        public List<CIPService> Services { get; set; }
    }

    public class CIPService
    {
        public string Name { get; set; }
        public MessageStream MessageStream { get; set; }
    }

    public class MessageStream
    {
        public string ByteData { get; set; }
        public List<string> Insert { get; set; }
    }
}
