namespace ICSStudio.DeviceProfiles.MotionDrive2.Common
{
    public class Module
    {
        public string ID { get; set; }
        public bool SupportsReset { get; set; }
        public int NumberOfInputs { get; set; }
        public int NumberOfOutputs { get; set; }
        public int DriverType { get; set; }
        public string CIPObjectDefinesID { get; set; }
    }
}
