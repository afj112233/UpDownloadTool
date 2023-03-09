using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;

namespace ICSStudio.SimpleServices.PredefinedType
{
    public sealed class INT : DataType.DataType
    {
        private INT()
        {
            Name = nameof(INT);
        }

        public static readonly INT Inst = new INT();

        public override int BitSize => 8 * 2;
        public override int ByteSize => 2;
        public override int AlignSize => 2;
        public override bool IsInteger => true;
        public override bool IsNumber => true;
        public override bool IsAtomic => true;
        public override bool SupportsOneDimensionalArray => true;
        public override bool SupportsMultiDimensionalArrays => true;
        public override bool IsPredefinedType => true;

        public override IField Create(JToken token)
        {
            if (token == null) return new Int16Field(0);
            return new Int16Field(token);
        }
    }
}
