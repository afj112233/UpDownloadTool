using System.Collections.Generic;

namespace ICSStudio.DeviceProfiles.DIOModule.Common
{
    public class IOModuleDefinition
    {
        public string ID { get; set; }
        public string StringsID { get; set; }
        public IOConnection Connection { get; set; }
        public IODataFormat DataFormat { get; set; }
    }

    public class IOConnection
    {
        public int StringID { get; set; }
        public List<IOConnectionChoice> Choices { get; set; }
    }

    public class IODataFormat
    {
        public int StringID { get; set; }
        public List<IODataFormatChoice> Choices { get; set; }
    }

    public class IOConnectionChoice
    {
        public string ID { get; set; }
        public int StringID { get; set; }

        public string DataFormat { get; set; }
        public uint ConfigID { get; set; }
        public uint? CommMethod { get; set; }
        public uint? FilterMask { get; set; }

    }

    public class IODataFormatChoice
    {
        public string ID { get; set; }
        public int StringID { get; set; }
    }
}
