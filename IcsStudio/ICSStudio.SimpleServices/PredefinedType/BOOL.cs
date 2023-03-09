using System.Diagnostics.CodeAnalysis;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace ICSStudio.SimpleServices.PredefinedType
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public sealed class BOOL : DataType.DataType
    {
        private BOOL(DataType.DataType refDataType = null)
        {
            Name = refDataType == null ? nameof(BOOL) : $"{nameof(BOOL)}:{refDataType.Name}";
            RefDataType = refDataType == null ? SINT.Inst:refDataType;
        }

        public static readonly BOOL Inst = new BOOL();
        public static readonly BOOL SInst = new BOOL(SINT.Inst);
        public static readonly BOOL IInst = new BOOL(INT.Inst);
        public static readonly BOOL DInst = new BOOL(DINT.Inst);
        public static readonly BOOL LInst = new BOOL(LINT.Inst);
        public override int BitSize => 8;
        public override int ByteSize => 1;
        public override int AlignSize => 1;
        public override bool IsBool => true;
        public override bool IsAtomic => true;
        public override bool SupportsOneDimensionalArray => true;
        public override bool SupportsMultiDimensionalArrays => false;
        public override bool IsPredefinedType => true;
        public DataType.DataType RefDataType { get; }

        public static BOOL GetInst(IDataType reftype)
        {
            if (reftype is SINT)
            {
                return SInst;
            } else if (reftype is INT)
            {
                return IInst;
            } else if (reftype is DINT)
            {
                return DInst;
            } else if (reftype is LINT)
            {
                return LInst;
            }
            else
            {
                throw new BOOLException();
                //Debug.Assert(false);
                //return null;
            }
        }

        public override IField Create(JToken token)
        {
            if (token == null) return new BoolField(0);
            return new BoolField(token);
        }

        public override string ToString()
        {
            return $"BOOL:{RefDataType}";
        }
    }
}
