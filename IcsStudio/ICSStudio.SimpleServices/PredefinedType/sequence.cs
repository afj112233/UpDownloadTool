using System;
using System.Diagnostics;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using Newtonsoft.Json.Linq;
namespace ICSStudio.SimpleServices.PredefinedType
{
    public sealed class SEQUENCE : NativeType
    {
        private SEQUENCE()
        {
            Name = "SEQUENCE";
            {
                var member = new DataTypeMember
                {
                    Name = "State",
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
                    Name = "Running",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 0,
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
                    Name = "Holding",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 1,
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
                    Name = "Restarting",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 2,
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
                    Name = "Stopping",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 3,
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
                    Name = "Aborting",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 4,
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
                    Name = "Resetting",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 5,
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
                    Name = "Idle",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 6,
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
                    Name = "Held",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 7,
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
                    Name = "Complete",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 8,
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
                    Name = "Stopped",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 9,
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
                    Name = "Aborted",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 10,
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
                    Name = "Unknown",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 11,
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
                    Name = "Mode",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 4,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
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
                    Name = "Control",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
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
                    Name = "PauseControl",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 12,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
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
                    Name = "PauseEnabled",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 12,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.DInst,
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
                    Name = "Paused",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 12,
                    BitOffset = 1,
                    IsBit = true,
                    DataType = BOOL.DInst,
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
                    Name = "AutoPauseEnabled",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 12,
                    BitOffset = 2,
                    IsBit = true,
                    DataType = BOOL.DInst,
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
                    Name = "Failure",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 16,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
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
                    Name = "UnitID",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 20,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
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
                    Name = "Owner",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 24,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
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
                    Name = "InternalFailure",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 28,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
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
                    Name = "ExecutionIdHI",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 32,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = LINT.Inst,
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
                    Name = "ExecutionIdLO",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 40,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = LINT.Inst,
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
                    Name = "ValidCommands",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 48,
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
                    Name = "StartValid",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 52,
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
                    Name = "HoldValid",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 53,
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
                    Name = "StopValid",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 54,
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
                    Name = "RestartValid",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 55,
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
            {
                var member = new DataTypeMember
                {
                    Name = "AbortValid",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 56,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "ResetValid",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 57,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "AutoPauseValid",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 58,
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
                    Name = "PauseValid",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 59,
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
                    Name = "ResumeValid",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 60,
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
                    Name = "ClearFailureValid",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 61,
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
                    Name = "AutoModeValid",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 62,
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
                    Name = "ManualModeValid",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 63,
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
                    Name = "InitializeParamTagValid",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 64,
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
                    Name = "DefineUSSIValid",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 65,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 24,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
        }
        public static readonly SEQUENCE Inst = new SEQUENCE();
        public override IField Create(JToken token)
        {
            var res = new SEQUENCEField(token);
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
        public override int BitSize => 576;
        public override int ByteSize => 72;
        public const int size = 72;
    }
    public sealed class SEQUENCEField : PreDefinedField
    {
        public SEQUENCEField(JToken token)
        {
            Debug.Assert(token == null || token is JArray);
            var array = token as JArray;
            fields.Add(Tuple.Create(new Int32Field(array?[0]) as IField, 0));
            fields.Add(Tuple.Create(new Int32Field(array?[1]) as IField, 4));
            fields.Add(Tuple.Create(new Int32Field(array?[2]) as IField, 8));
            fields.Add(Tuple.Create(new Int32Field(array?[3]) as IField, 12));
            fields.Add(Tuple.Create(new Int32Field(array?[4]) as IField, 16));
            fields.Add(Tuple.Create(new Int32Field(array?[5]) as IField, 20));
            fields.Add(Tuple.Create(new Int32Field(array?[6]) as IField, 24));
            fields.Add(Tuple.Create(new Int32Field(array?[7]) as IField, 28));
            fields.Add(Tuple.Create(new Int64Field(array?[8]) as IField, 32));
            fields.Add(Tuple.Create(new Int64Field(array?[9]) as IField, 40));
            fields.Add(Tuple.Create(new Int32Field(array?[10]) as IField, 48));
            fields.Add(Tuple.Create(new Int8Field(array?[11]) as IField, 52));
            fields.Add(Tuple.Create(new Int8Field(array?[12]) as IField, 53));
            fields.Add(Tuple.Create(new Int8Field(array?[13]) as IField, 54));
            fields.Add(Tuple.Create(new Int8Field(array?[14]) as IField, 55));
            fields.Add(Tuple.Create(new Int8Field(array?[15]) as IField, 56));
            fields.Add(Tuple.Create(new Int8Field(array?[16]) as IField, 57));
            fields.Add(Tuple.Create(new Int8Field(array?[17]) as IField, 58));
            fields.Add(Tuple.Create(new Int8Field(array?[18]) as IField, 59));
            fields.Add(Tuple.Create(new Int8Field(array?[19]) as IField, 60));
            fields.Add(Tuple.Create(new Int8Field(array?[20]) as IField, 61));
            fields.Add(Tuple.Create(new Int8Field(array?[21]) as IField, 62));
            fields.Add(Tuple.Create(new Int8Field(array?[22]) as IField, 63));
            fields.Add(Tuple.Create(new Int8Field(array?[23]) as IField, 64));
            fields.Add(Tuple.Create(new Int8Field(array?[24]) as IField, 65));
        }
    }
}