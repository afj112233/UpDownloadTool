using System.Collections.Generic;

namespace ICSStudio.DeviceProfiles.MotionDrive2.Common
{
    public class SupportedValue<T>
    {
        public T Value { get; set; }
        public int MinMajorRev { get; set; }

        public bool ShouldSerializeMinMajorRev()
        {
            return MinMajorRev != 0;
        }
    }

    public class SupportedEnum
    {
        public string Name { get; set; }
        public int MinMajorRev { get; set; }
        public List<SupportedValue<string>> Values { get; set; }

        public bool ShouldSerializeMinMajorRev()
        {
            return MinMajorRev != 0;
        }
    }

    public class SupportedEnumPerAxisConfiguration
    {
        public string Name { get; set; }

        public List<SupportedValue<string>> PositionLoop { get; set; }
        public List<SupportedValue<string>> VelocityLoop { get; set; }
        public List<SupportedValue<string>> TorqueLoop { get; set; }
        public List<SupportedValue<string>> FrequencyControl { get; set; }
        public List<SupportedValue<string>> FeedbackOnly { get; set; }
    }

    public class SupportedEnumPerAxisConfigurationAndFeedbackConfiguration
    {
        public string Name { get; set; }

        public SupportedValuePerFeedback FrequencyControl { get; set; }
        public SupportedValuePerFeedback PositionLoop { get; set; }
        public SupportedValuePerFeedback VelocityLoop { get; set; }
        public SupportedValuePerFeedback TorqueLoop { get; set; }
    }

    public class SupportedValuePerFeedback
    {
        public List<SupportedValue<string>> Feedback { get; set; }
        public List<SupportedValue<string>> NoFeedback { get; set; }
    }
}
