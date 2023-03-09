using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace ICSStudio.SimpleServices.PredefinedType
{
    public sealed class LREAL : DataType.DataType
    {
        private LREAL()
        {
            Name = "LREAL";
        }

        public static readonly LREAL Inst = new LREAL();
        public override int BitSize => 8 * 8;
        public override int ByteSize => 8;
        public override int AlignSize => 8;
        public override bool SupportsOneDimensionalArray => true;
        public override bool SupportsMultiDimensionalArrays => true;
        public override bool IsPredefinedType => true;
        public override bool IsReal => true;
        public override bool IsNumber => true;
        public override bool IsAtomic => true;
        public override IField Create(JToken token)
        {
            //FIXE use LRealField?
            if (token == null) return new RealField(0.0);
            return new RealField(token);
        }
    }


}
