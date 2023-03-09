namespace ICSStudio.Cip.Objects
{
    public class AxisExceptionLogItem
    {
        public short Type { get; set; } // 0x100,alarm; 0x00,fault
        public byte Code { get; set; }
        public short SubCode { get; set; }
        public byte StopAction { get; set; } // action
        public long Timestamp { get; set; }
        public byte StateChange { get; set; } // state
    }
}
