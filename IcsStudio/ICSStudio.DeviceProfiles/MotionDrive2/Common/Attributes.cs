using System.Collections.Generic;

namespace ICSStudio.DeviceProfiles.MotionDrive2.Common
{
    public class Attributes
    {
        public SupportedAttributes SupportedAttributes { get; set; }

        public List<SupportedEnum> SupportedEnums { get; set; }

        public List<SupportedEnumPerAxisConfiguration> SupportedEnumsPerAxisConfiguration { get; set; }

        public List<SupportedEnumPerAxisConfigurationAndFeedbackConfiguration>
            SupportedEnumsPerAxisConfigurationAndFeedbackConfiguration { get; set; }

        public CurrentLoopBandwidthScalingFactor CurrentLoopBandwidthScalingFactor { get; set; }

        public List<SupportedMotorTest> SupportedMotorTests { get; set; }

        public List<AttributeDefault> ProductSpecificAttributeDefaults { get; set; }

        public SupportedExceptions SupportedExceptions { get; set; }
        public SupportedExceptions FeedbackOnlySupportedExceptions { get; set; }

    }

    public class SupportedAttributes
    {
        public List<SupportedValue<string>> PositionLoop { get; set; }
        public List<SupportedValue<string>> VelocityLoop { get; set; }
        public List<SupportedValue<string>> TorqueLoop { get; set; }
        public List<SupportedValue<string>> FrequencyControl { get; set; }
        public List<SupportedValue<string>> FeedbackOnly { get; set; }
    }

    public class CurrentLoopBandwidthScalingFactor
    {
        public float RotaryPermanentMagnet { get; set; }
        public float LinearPermanentMagnet { get; set; }
        public float RotaryInduction { get; set; }
    }

    public class SupportedMotorTest
    {
        public string Name { get; set; }
        public List<SupportedValue<string>> Methods { get; set; }
    }

    public class AttributeDefault
    {
        public string Attribute { get; set; }
        public string Default { get; set; }
        public int MinMajorRev { get; set; }

        public bool ShouldSerializeMinMajorRev()
        {
            return MinMajorRev != 0;
        }
    }
}
