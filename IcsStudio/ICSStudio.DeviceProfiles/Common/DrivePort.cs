namespace ICSStudio.DeviceProfiles.Common
{
    public class DrivePort
    {
        public int Number { get; set; }
        public string Type { get; set; }
        public string Label { get; set; }
        public int Width { get; set; }
        public int ConnectorOffset { get; set; }

        public DrivePortExtendedProperties ExtendedProperties { get; set; }
    }

    public class DrivePortExtendedProperties
    {
        public bool DownstreamOnly { get; set; }
        public int MaxRPIRestriction { get; set; }
        public int MinRPIRestriction { get; set; }
        public int RPIResolutionRestriction { get; set; }
    }
}
