using System;
using System.Diagnostics;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using Newtonsoft.Json.Linq;
namespace ICSStudio.SimpleServices.PredefinedType
{
    public sealed class SELECT_ENHANCED : NativeType
    {
        private SELECT_ENHANCED()
        {
            Name = "SELECT_ENHANCED";
            {
                var member = new DataTypeMember
                {
                    Name = "EnableIn",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "In1",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 4,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "In2",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "In3",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 12,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 3,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "In4",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 16,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 4,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "In5",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 20,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 5,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "In6",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 24,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 6,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "In1Fault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 28,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 7,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "In2Fault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 29,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 8,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "In3Fault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 30,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 9,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "In4Fault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 31,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 10,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "In5Fault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 32,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 11,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "In6Fault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 33,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 12,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "InsUsed",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 36,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 13,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "SelectorMode",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 40,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 14,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ProgSelector",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 44,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 15,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "OperSelector",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 48,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 16,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ProgProgReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 52,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 17,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ProgOperReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 53,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 18,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ProgOverrideReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 54,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 19,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "OperProgReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 55,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 20,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "OperOperReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 56,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 21,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ProgValueReset",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 57,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 22,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "EnableOut",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 58,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 23,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "Out",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 60,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 24,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "SelectedIn",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 64,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 25,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ProgOper",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 68,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 26,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "Override",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 69,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 27,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "Status",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 72,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 28,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "InstructFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 72,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 28,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "InsFaulted",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 72,
                    BitOffset = 1,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 28,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "InsUsedInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 72,
                    BitOffset = 2,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 28,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "SelectorModeInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 72,
                    BitOffset = 3,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 28,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ProgSelectorInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 72,
                    BitOffset = 4,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 28,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "OperSelectorInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 72,
                    BitOffset = 5,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 28,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "FirstRun",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 76,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 29,
                    IsHidden = true,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
        }
        public static readonly SELECT_ENHANCED Inst = new SELECT_ENHANCED();
        public override IField Create(JToken token)
        {
            var res = new SELECT_ENHANCEDField(token);
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
        public override int BitSize => 640;
        public override int ByteSize => 80;
        public const int size = 80;
    }
    public sealed class SELECT_ENHANCEDField : PreDefinedField
    {
        public SELECT_ENHANCEDField(JToken token)
        {
            Debug.Assert(token == null || token is JArray);
            var array = token as JArray;
            fields.Add(Tuple.Create(new Int8Field(array?[0]) as IField, 0));
            fields.Add(Tuple.Create(new RealField(array?[1]) as IField, 4));
            fields.Add(Tuple.Create(new RealField(array?[2]) as IField, 8));
            fields.Add(Tuple.Create(new RealField(array?[3]) as IField, 12));
            fields.Add(Tuple.Create(new RealField(array?[4]) as IField, 16));
            fields.Add(Tuple.Create(new RealField(array?[5]) as IField, 20));
            fields.Add(Tuple.Create(new RealField(array?[6]) as IField, 24));
            fields.Add(Tuple.Create(new Int8Field(array?[7]) as IField, 28));
            fields.Add(Tuple.Create(new Int8Field(array?[8]) as IField, 29));
            fields.Add(Tuple.Create(new Int8Field(array?[9]) as IField, 30));
            fields.Add(Tuple.Create(new Int8Field(array?[10]) as IField, 31));
            fields.Add(Tuple.Create(new Int8Field(array?[11]) as IField, 32));
            fields.Add(Tuple.Create(new Int8Field(array?[12]) as IField, 33));
            fields.Add(Tuple.Create(new Int32Field(array?[13]) as IField, 36));
            fields.Add(Tuple.Create(new Int32Field(array?[14]) as IField, 40));
            fields.Add(Tuple.Create(new Int32Field(array?[15]) as IField, 44));
            fields.Add(Tuple.Create(new Int32Field(array?[16]) as IField, 48));
            fields.Add(Tuple.Create(new Int8Field(array?[17]) as IField, 52));
            fields.Add(Tuple.Create(new Int8Field(array?[18]) as IField, 53));
            fields.Add(Tuple.Create(new Int8Field(array?[19]) as IField, 54));
            fields.Add(Tuple.Create(new Int8Field(array?[20]) as IField, 55));
            fields.Add(Tuple.Create(new Int8Field(array?[21]) as IField, 56));
            fields.Add(Tuple.Create(new Int8Field(array?[22]) as IField, 57));
            fields.Add(Tuple.Create(new Int8Field(array?[23]) as IField, 58));
            fields.Add(Tuple.Create(new RealField(array?[24]) as IField, 60));
            fields.Add(Tuple.Create(new Int32Field(array?[25]) as IField, 64));
            fields.Add(Tuple.Create(new Int8Field(array?[26]) as IField, 68));
            fields.Add(Tuple.Create(new Int8Field(array?[27]) as IField, 69));
            fields.Add(Tuple.Create(new Int32Field(array?[28]) as IField, 72));
            fields.Add(Tuple.Create(new Int8Field(array?[29]) as IField, 76));
        }
    }
}