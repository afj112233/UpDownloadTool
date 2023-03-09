using System.Collections.Generic;

namespace ICSStudio.DeviceProfiles.MotionDrive2.Common
{
    public class Axis
    {
        public int Number { get; set; }
        public SupportedAxisConfigurations SupportedAxisConfigurations { get; set; }

        public AllowableFeedbackPorts AllowableFeedbackPorts { get; set; }
    }

    public class SupportedAxisConfigurations
    {
        public List<string> Values { get; set; }
        public string Default { get; set; }
    }

    public class AllowableFeedbackPorts
    {
        //TODO(gjc): need check for MinMajorRev
        public List<PortNumber> MotorMaster { get; set; }
        public List<PortNumber> Load { get; set; }
    }

    public class PortNumber
    {
        public int Number { get; set; }
        public bool Default { get; set; }
    }
}
