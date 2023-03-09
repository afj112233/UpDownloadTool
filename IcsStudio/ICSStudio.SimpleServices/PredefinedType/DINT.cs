using System;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace ICSStudio.SimpleServices.PredefinedType
{
    public sealed class DINT : DataType.DataType
    {
        private DINT()
        {
            Name = nameof(DINT);
        }

        public static readonly DINT Inst = new DINT();

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
            if (token == null) return new Int32Field(0);
            return new Int32Field(token);
        }
    }
}
