using System.Collections.Generic;

namespace ICSStudio.DeviceProfiles.MotionDrive2.Common
{
    public class SupportedExceptions
    {
        public List<SupportedException> CIPAxisExceptionAction { get; set; }
        public List<SupportedException> CIPAxisExceptionActionRA { get; set; }
    }

    public class SupportedException
    {
        public string Exception { get; set; }
        public int MinMajorRev { get; set; }
        public List<SupportedValue<string>> Action { get; set; }
    }
}
