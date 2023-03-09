namespace ICSStudio.DeviceProfiles.DIOModule.Common
{
    public class IOConnectionDefinition
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public uint? MinRPI { get; set; }
        public uint? MaxRPI { get; set; }
        public uint? RPI { get; set; }
        public uint? InputCxnPoint { get; set; }
        public uint? OutputCxnPoint { get; set; }

        public IOTag InputTag { get; set; }
        public IOTag OutputTag { get; set; }
        public IOAliasTag InAliasTag { get; set; }
        public IOAliasTag OutAliasTag { get; set; }
    }

    public class IOTag
    {
        public string DataType { get; set; }
    }

    public class IOAliasTag
    {
        public string Suffix { get; set; }
    }
}
