using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;

namespace ICSStudio.SimpleServices.PredefinedType
{
    public sealed class LINT : DataType.DataType
    {
        private LINT() 
        {
            Name = nameof(LINT);
        }

        public static readonly LINT Inst = new LINT();
        public override int BitSize => 8 * 8;
        public override int ByteSize => 8;
        public override int AlignSize => 8;
        public override bool IsInteger => true;
        public override bool IsLINT => true;
        public override bool IsNumber => true;
        public override bool IsAtomic => true;
        public override bool SupportsOneDimensionalArray => true;
        public override bool SupportsMultiDimensionalArrays => true;
        public override bool IsPredefinedType => true;

        public override IField Create(JToken token)
        {
            if (token == null) return new Int64Field(0);
            return new Int64Field(token);
        }
    }


}
