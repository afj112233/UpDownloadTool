using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.PredefinedType
{
    public sealed class SINT : DataType.DataType
    {
        private SINT()
        {
            Name = nameof(SINT);
        }

        public static readonly SINT Inst = new SINT();

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
            if (token == null) return new Int8Field(0);
            return new Int8Field(token);
        }
    }
}
