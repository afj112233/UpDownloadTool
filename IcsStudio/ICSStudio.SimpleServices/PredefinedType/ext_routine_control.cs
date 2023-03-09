using System;
using System.Diagnostics;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using Newtonsoft.Json.Linq;
namespace ICSStudio.SimpleServices.PredefinedType
{
    public sealed class EXT_ROUTINE_CONTROL : NativeType
    {
        private EXT_ROUTINE_CONTROL()
        {
            Name = "EXT_ROUTINE_CONTROL";
            {
                var member = new DataTypeMember
                {
                    Name = "ErrorCode",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = SINT.Inst,
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
                    Name = "NumParams",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 1,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = SINT.Inst,
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
                    Name = "ParameterDefs",
                    DisplayStyle = DisplayStyle.NullStyle,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = EXT_ROUTINE_PARAMETERS.Inst,
                    Dim1 = 10,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ReturnParamDef",
                    DisplayStyle = DisplayStyle.NullStyle,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 168,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = EXT_ROUTINE_PARAMETERS.Inst,
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
                    Name = "EN",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 184,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "ReturnsValue",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 185,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "DN",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 186,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "ER",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 187,
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
                    Name = "FirstScan",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 188,
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
                    Name = "EnableOut",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 189,
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
                    Name = "EnableIn",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 190,
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
                    Name = "User1",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 191,
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
                    Name = "User0",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 192,
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
                    Name = "ScanType1",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 193,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "ScanType0",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 194,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 14,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
        }
        public static readonly EXT_ROUTINE_CONTROL Inst = new EXT_ROUTINE_CONTROL();
        public override IField Create(JToken token)
        {
            var res = new EXT_ROUTINE_CONTROLField(token);
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
        public override int BitSize => 1600;
        public override int ByteSize => 200;
        public const int size = 200;
    }
    public sealed class EXT_ROUTINE_CONTROLField : PreDefinedField
    {
        public EXT_ROUTINE_CONTROLField(JToken token)
        {
            Debug.Assert(token == null || token is JArray);
            var array = token as JArray;
            fields.Add(Tuple.Create(new Int8Field(array?[0]) as IField, 0));
            fields.Add(Tuple.Create(new Int8Field(array?[1]) as IField, 1));
            {
                var tmp = array?[2] as JArray;
                var arr = new ArrayField(10, 0, 0);
                arr.Add(Tuple.Create(new EXT_ROUTINE_PARAMETERSField(tmp?[0]) as IField, 0));
                arr.Add(Tuple.Create(new EXT_ROUTINE_PARAMETERSField(tmp?[1]) as IField, 16));
                arr.Add(Tuple.Create(new EXT_ROUTINE_PARAMETERSField(tmp?[2]) as IField, 32));
                arr.Add(Tuple.Create(new EXT_ROUTINE_PARAMETERSField(tmp?[3]) as IField, 48));
                arr.Add(Tuple.Create(new EXT_ROUTINE_PARAMETERSField(tmp?[4]) as IField, 64));
                arr.Add(Tuple.Create(new EXT_ROUTINE_PARAMETERSField(tmp?[5]) as IField, 80));
                arr.Add(Tuple.Create(new EXT_ROUTINE_PARAMETERSField(tmp?[6]) as IField, 96));
                arr.Add(Tuple.Create(new EXT_ROUTINE_PARAMETERSField(tmp?[7]) as IField, 112));
                arr.Add(Tuple.Create(new EXT_ROUTINE_PARAMETERSField(tmp?[8]) as IField, 128));
                arr.Add(Tuple.Create(new EXT_ROUTINE_PARAMETERSField(tmp?[9]) as IField, 144));
                Debug.Assert(tmp == null || arr.Size() == tmp.Count);
                fields.Add(Tuple.Create(arr as IField, 8));
            }
            fields.Add(Tuple.Create(new EXT_ROUTINE_PARAMETERSField(array?[3]) as IField, 168));
            fields.Add(Tuple.Create(new Int8Field(array?[4]) as IField, 184));
            fields.Add(Tuple.Create(new Int8Field(array?[5]) as IField, 185));
            fields.Add(Tuple.Create(new Int8Field(array?[6]) as IField, 186));
            fields.Add(Tuple.Create(new Int8Field(array?[7]) as IField, 187));
            fields.Add(Tuple.Create(new Int8Field(array?[8]) as IField, 188));
            fields.Add(Tuple.Create(new Int8Field(array?[9]) as IField, 189));
            fields.Add(Tuple.Create(new Int8Field(array?[10]) as IField, 190));
            fields.Add(Tuple.Create(new Int8Field(array?[11]) as IField, 191));
            fields.Add(Tuple.Create(new Int8Field(array?[12]) as IField, 192));
            fields.Add(Tuple.Create(new Int8Field(array?[13]) as IField, 193));
            fields.Add(Tuple.Create(new Int8Field(array?[14]) as IField, 194));
        }
    }
}