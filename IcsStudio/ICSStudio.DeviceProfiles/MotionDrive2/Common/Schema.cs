using System.Collections.Generic;


namespace ICSStudio.DeviceProfiles.MotionDrive2.Common
{
    public class Schema
    {
        public OptionalAttributeRevision OptionalAttributeRevision { get; set; }
        public string AxisType { get; set; }
        public TimeSync TimeSync { get; set; }
        public bool UpdateableFlash { get; set; }
        public Safety Safety { get; set; }

        public List<SupportedAttribute> SupportedAttributes { get; set; }
        public List<PowerStructure> PowerStructures { get; set; }

        public Feedback Feedback { get; set; }
        public List<Axis> Axes { get; set; }
        public Attributes Attributes { get; set; }
    }

    public class OptionalAttributeRevision
    {
        public int Revision { get; set; }
        public int MinMajorRev { get; set; }
    }

    public class TimeSync
    {
        public bool HighQuality { get; set; }
    }

    public class Safety
    {
        public bool EnhancedSafeTorqueOff { get; set; }
    }
}
