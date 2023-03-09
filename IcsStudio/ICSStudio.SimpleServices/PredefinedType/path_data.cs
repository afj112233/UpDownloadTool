using System;
using System.Diagnostics;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using Newtonsoft.Json.Linq;
namespace ICSStudio.SimpleServices.PredefinedType
{
    public sealed class PATH_DATA : NativeType
    {
        private PATH_DATA()
        {
            Name = "PATH_DATA";
            {
                var member = new DataTypeMember
                {
                    Name = "InterpolationType",
                    DisplayStyle = DisplayStyle.Decimal,
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
                    Name = "Position",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 4,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 9,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 1,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "RobotConfiguration",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 40,
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
                    Name = "TurnsCounters",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 44,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = INT.Inst,
                    Dim1 = 4,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 3,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "MoveType",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 52,
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
                    Name = "TerminationType",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 56,
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
                    Name = "CommandToleranceLinear",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 60,
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
        }
        public static readonly PATH_DATA Inst = new PATH_DATA();
        public override IField Create(JToken token)
        {
            var res = new PATH_DATAField(token);
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
        public override int BitSize => 512;
        public override int ByteSize => 64;
        public const int size = 64;
    }
    public sealed class PATH_DATAField : PreDefinedField
    {
        public PATH_DATAField(JToken token)
        {
            Debug.Assert(token == null || token is JArray);
            var array = token as JArray;
            fields.Add(Tuple.Create(new Int32Field(array?[0]) as IField, 0));
            {
                var tmp = array?[1] as JArray;
                var arr = new ArrayField(9, 0, 0);
                arr.Add(Tuple.Create(new RealField(tmp?[0]) as IField, 0));
                arr.Add(Tuple.Create(new RealField(tmp?[1]) as IField, 4));
                arr.Add(Tuple.Create(new RealField(tmp?[2]) as IField, 8));
                arr.Add(Tuple.Create(new RealField(tmp?[3]) as IField, 12));
                arr.Add(Tuple.Create(new RealField(tmp?[4]) as IField, 16));
                arr.Add(Tuple.Create(new RealField(tmp?[5]) as IField, 20));
                arr.Add(Tuple.Create(new RealField(tmp?[6]) as IField, 24));
                arr.Add(Tuple.Create(new RealField(tmp?[7]) as IField, 28));
                arr.Add(Tuple.Create(new RealField(tmp?[8]) as IField, 32));
                Debug.Assert(tmp == null || arr.Size() == tmp.Count);
                fields.Add(Tuple.Create(arr as IField, 4));
            }
            fields.Add(Tuple.Create(new Int32Field(array?[2]) as IField, 40));
            {
                var tmp = array?[3] as JArray;
                var arr = new ArrayField(4, 0, 0);
                arr.Add(Tuple.Create(new Int16Field(tmp?[0]) as IField, 0));
                arr.Add(Tuple.Create(new Int16Field(tmp?[1]) as IField, 2));
                arr.Add(Tuple.Create(new Int16Field(tmp?[2]) as IField, 4));
                arr.Add(Tuple.Create(new Int16Field(tmp?[3]) as IField, 6));
                Debug.Assert(tmp == null || arr.Size() == tmp.Count);
                fields.Add(Tuple.Create(arr as IField, 44));
            }
            fields.Add(Tuple.Create(new Int32Field(array?[4]) as IField, 52));
            fields.Add(Tuple.Create(new Int32Field(array?[5]) as IField, 56));
            fields.Add(Tuple.Create(new RealField(array?[6]) as IField, 60));
        }
    }
}