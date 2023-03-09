using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace ICSStudio.SimpleServices.PredefinedType
{
    public sealed class REAL : DataType.DataType
    {
        private REAL()
        {
            Name = nameof(REAL);
        }

        public static readonly REAL Inst = new REAL();
        public override int BitSize => 8 * 4;
        public override int ByteSize => 4;
        public override int AlignSize => 4;
        public override bool SupportsOneDimensionalArray => true;
        public override bool SupportsMultiDimensionalArrays => true;
        public override bool IsPredefinedType => true;
        public override bool IsReal => true;
        public override bool IsNumber => true;
        public override bool IsAtomic => true;
        public override IField Create(JToken token)
        {
            if (token == null) return new RealField(0.0);
            return new RealField(token);
        }
    }


}
