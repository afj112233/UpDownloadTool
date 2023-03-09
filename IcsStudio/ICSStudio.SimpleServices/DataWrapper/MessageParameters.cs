namespace ICSStudio.SimpleServices.DataWrapper
{
    public class MessageParameters
    {
        public byte MessageType { get; set; }
        public string RemoteElement { get; set; }
        public ushort RequestedLength { get; set; } //REQ_LEN
        public bool RequestedLengthSpecified { get; set; }
        public byte ConnectedFlag { get; set; } //0,1
        public bool ConnectedFlagSpecified { get; set; }
        public string ConnectionPath { get; set; }
        public byte CommTypeCode { get; set; }
        public bool CommTypeCodeSpecified { get; set; }
        public ushort ServiceCode { get; set; }
        public ushort ObjectType { get; set; }
        public ulong TargetObject { get; set; }
        public bool TargetObjectSpecified { get; set; }
        public ushort AttributeNumber { get; set; }
        public byte Channel { get; set; }
        public bool ChannelSpecified { get; set; }
        public ushort DHPlusSourceLink { get; set; }
        public bool DHPlusSourceLinkSpecified { get; set; }
        public string DHPlusDestinationLink { get; set; }
        public string DHPlusDestinationNode { get; set; }
        public byte Rack { get; set; }
        public bool RackSpecified { get; set; }
        public byte Group { get; set; }
        public bool GroupSpecified { get; set; }
        public byte Slot { get; set; }
        public bool SlotSpecified { get; set; }
        public ulong LocalIndex { get; set; }
        public bool LocalIndexSpecified { get; set; }
        public ulong RemoteIndex { get; set; }
        public bool RemoteIndexSpecified { get; set; }
        public string LocalElement { get; set; }
        public string DestinationTag { get; set; }
        public bool CacheConnections { get; set; } //EN_CC
        public bool LargePacketUsage { get; set; }
        public bool LargePacketUsageSpecified { get; set; }

        //
        public bool TimedOut { get; set; } //TO
        public ulong UnconnectedTimeout { get; set; }
        public ulong ConnectionRate { get; set; }
        public byte TimeoutMultiplier { get; set; }
    }
}
