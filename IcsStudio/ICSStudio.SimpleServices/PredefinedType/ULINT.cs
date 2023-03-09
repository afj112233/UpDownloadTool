using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.PredefinedType
{
    public sealed class ULINT : DataType.DataType
    {
        private ULINT()
        {
            Name = nameof(ULINT);
        }

        public static readonly ULINT Inst = new ULINT();
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
            if (token == null) return new UInt64Field(0);
            return new UInt64Field(token);
        }
    }

}
