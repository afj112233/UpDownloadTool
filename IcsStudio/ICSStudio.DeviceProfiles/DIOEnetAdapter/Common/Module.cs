namespace ICSStudio.DeviceProfiles.DIOEnetAdapter.Common
{
    public class Module
    {
        public string ID { get; set; }
        public bool SupportsReset { get; set; }
        public int DriverType { get; set; }
        public string BusID { get; set; }
        public string CIPObjectDefinesID { get; set; }
        public bool Relaxed { get; set; }
    }
}
