using System;
using System.Diagnostics;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using Newtonsoft.Json.Linq;
namespace ICSStudio.SimpleServices.PredefinedType
{
    public sealed class PHASE_INSTRUCTION : NativeType
    {
        private PHASE_INSTRUCTION()
        {
            Name = "PHASE_INSTRUCTION";
            {
                var member = new DataTypeMember
                {
                    Name = "Status",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 0,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "EN",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 31,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 0,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ER",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 28,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 0,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PC",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 27,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 0,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "IP",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 26,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 0,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "WA",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 25,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 0,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ABORT",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 24,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 0,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ERR",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 4,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = INT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 1,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "EXERR",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 6,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = INT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
        }
        public static readonly PHASE_INSTRUCTION Inst = new PHASE_INSTRUCTION();
        public override IField Create(JToken token)
        {
            var res = new PHASE_INSTRUCTIONField(token);
            FixUp(token, res);
            return res;
        }
        public override bool SupportsOneDimensionalArray => true;
        public override bool SupportsMultiDimensionalArrays => true;
        public override bool IsPredefinedType => true;
        public override bool IsStruct => true;
        public override bool IsAxisType => false;
        public override bool IsMotionGroupType => false;
        public override bool IsCoordinateSystemType => false;
        public override int BitSize => 64;
        public override int ByteSize => 8;
        public const int size = 8;
    }
    public sealed class PHASE_INSTRUCTIONField : PreDefinedField
    {
        public PHASE_INSTRUCTIONField(JToken token)
        {
            Debug.Assert(token == null || token is JArray);
            var array = token as JArray;
            fields.Add(Tuple.Create(new Int32Field(array?[0]) as IField, 0));
            fields.Add(Tuple.Create(new Int16Field(array?[1]) as IField, 4));
            fields.Add(Tuple.Create(new Int16Field(array?[2]) as IField, 6));
        }
    }
}