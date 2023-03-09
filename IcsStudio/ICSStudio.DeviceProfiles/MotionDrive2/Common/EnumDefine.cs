using System.Collections.Generic;

namespace ICSStudio.DeviceProfiles.MotionDrive2.Common
{
    public class EnumDefine
    {
        public string ID { get; set; }
        public string StringsID { get; set; }
        public List<EnumValue> Values { get; set; }
    }

    public class EnumValue
    {
        public int Value { get; set; }
        public int StringID { get; set; }
    }
}
