namespace ICSStudio.Interfaces.Common
{
    public class ComponentCoreInfo
    {
        public int Uid { get; set; }

        public string Name { get; set; }

        public int DataTypeUid { get; set; }

        public uint Status { get; set; }

        public uint StatusEx { get; set; }

        public uint ComponentType { get; set; }

        public uint ComponentSubtype { get; set; }

        public TagDisplayInfo TagDisplayInfo { get; set; }

        public bool IsStatusBitSet(ComponentDataStatus bitmask)
        {
            return ((ComponentDataStatus)Status & bitmask) > 0;
        }

        public bool IsStatusExBitSet(ComponentDataStatus bitmask)
        {
            return ((ComponentDataStatus)StatusEx & bitmask) > 0;
        }
    }
}
