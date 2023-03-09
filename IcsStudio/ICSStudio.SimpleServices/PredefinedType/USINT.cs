using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.PredefinedType
{
    public sealed class USINT : DataType.DataType
    {
        private USINT()
        {
            Name = nameof(USINT);
        }

        public static readonly USINT Inst = new USINT();

        public override int BitSize => 8;
        public override int ByteSize => 1;
        public override int AlignSize => 1;
        public override bool IsInteger => true;
        public override bool IsNumber => true;
        public override bool IsAtomic => true;
        public override bool SupportsOneDimensionalArray => true;
        public override bool SupportsMultiDimensionalArrays => true;
        public override bool IsPredefinedType => true;

        public override IField Create(JToken token)
        {
            if (token == null) return new UInt8Field(0);
            return new UInt8Field(token);
        }
    }
}
