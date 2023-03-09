using System;
using System.Diagnostics;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using Newtonsoft.Json.Linq;
namespace ICSStudio.SimpleServices.PredefinedType
{
    public sealed class CB_SINGLE_STROKE_MODE : NativeType
    {
        private CB_SINGLE_STROKE_MODE()
        {
            Name = "CB_SINGLE_STROKE_MODE";
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
                    Name = "AckType",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 1,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "TakeoverMode",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 2,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "Enable",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 3,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "SafetyEnable",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 4,
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
                    Name = "StandardEnable",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 5,
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
                    Name = "Start",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 6,
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
                    Name = "PressInMotion",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 7,
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
                    Name = "MotionMonitorFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
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
                    Name = "SafetyEnableAck",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 9,
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
                    Name = "SlideZone",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 12,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
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
                    Name = "EnableOut",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 16,
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
                    Name = "O1",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 17,
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
                    Name = "DiagnosticCode",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 20,
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
        }
        public static readonly CB_SINGLE_STROKE_MODE Inst = new CB_SINGLE_STROKE_MODE();
        public override IField Create(JToken token)
        {
            var res = new CB_SINGLE_STROKE_MODEField(token);
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
        public override int BitSize => 192;
        public override int ByteSize => 24;
        public const int size = 24;
    }
    public sealed class CB_SINGLE_STROKE_MODEField : PreDefinedField
    {
        public CB_SINGLE_STROKE_MODEField(JToken token)
        {
            Debug.Assert(token == null || token is JArray);
            var array = token as JArray;
            fields.Add(Tuple.Create(new Int8Field(array?[0]) as IField, 0));
            fields.Add(Tuple.Create(new Int8Field(array?[1]) as IField, 1));
            fields.Add(Tuple.Create(new Int8Field(array?[2]) as IField, 2));
            fields.Add(Tuple.Create(new Int8Field(array?[3]) as IField, 3));
            fields.Add(Tuple.Create(new Int8Field(array?[4]) as IField, 4));
            fields.Add(Tuple.Create(new Int8Field(array?[5]) as IField, 5));
            fields.Add(Tuple.Create(new Int8Field(array?[6]) as IField, 6));
            fields.Add(Tuple.Create(new Int8Field(array?[7]) as IField, 7));
            fields.Add(Tuple.Create(new Int8Field(array?[8]) as IField, 8));
            fields.Add(Tuple.Create(new Int8Field(array?[9]) as IField, 9));
            fields.Add(Tuple.Create(new Int32Field(array?[10]) as IField, 12));
            fields.Add(Tuple.Create(new Int8Field(array?[11]) as IField, 16));
            fields.Add(Tuple.Create(new Int8Field(array?[12]) as IField, 17));
            fields.Add(Tuple.Create(new Int32Field(array?[13]) as IField, 20));
        }
    }
}