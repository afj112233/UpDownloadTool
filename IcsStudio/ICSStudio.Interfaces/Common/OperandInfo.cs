using ICSStudio.Interfaces.Tags;

namespace ICSStudio.Interfaces.Common
{
    public class OperandInfo
    {
        public DataTypeInfo DataTypeInfo { get; set; }

        public ITag BaseTag { get; set; }

        public uint BitOffset { get; set; }

        public uint BitLength { get; set; }
    }
}
