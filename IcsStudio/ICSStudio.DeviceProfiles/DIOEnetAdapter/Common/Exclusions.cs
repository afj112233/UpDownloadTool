namespace ICSStudio.DeviceProfiles.DIOEnetAdapter.Common
{
    public class Exclusions
    {
        public ExcludeAddressRange ExcludeAddressRange { get; set; }
    }

    public class ExcludeAddressRange
    {
        public int MinAddress { get; set; }
        public int MaxAddress { get; set; }
    }
}
