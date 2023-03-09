using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.PredefinedType
{
    public sealed class UDINT : DataType.DataType
    {
        private UDINT()
        {
            Name = nameof(UDINT);
        }

        public static readonly UDINT Inst = new UDINT();

        public override int BitSize => 8 * 4;
        public override int ByteSize => 4;
        public override int AlignSize => 4;
        public override bool IsInteger => true;
        public override bool IsNumber => true;
        public override bool IsAtomic => true;
        public override bool SupportsOneDimensionalArray => true;
        public override bool SupportsMultiDimensionalArrays => true;
        public override bool IsPredefinedType => true;

        public override IField Create(JToken token)
        {
            if (token == null) return new UInt32Field(0);
            return new UInt32Field(token);
        }
    }
}
