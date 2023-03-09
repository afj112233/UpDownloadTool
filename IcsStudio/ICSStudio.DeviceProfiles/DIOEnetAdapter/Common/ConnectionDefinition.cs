namespace ICSStudio.DeviceProfiles.DIOEnetAdapter.Common
{
    public class ConnectionDefinition
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public uint MinRPI { get; set; }
        public uint MaxRPI { get; set; }
        public uint RPI { get; set; }
        public uint InputCxnPoint { get; set; }
        public uint OutputCxnPoint { get; set; }
        public Tag InputTag { get; set; }
        public Tag OutputTag { get; set; }
    }
}
