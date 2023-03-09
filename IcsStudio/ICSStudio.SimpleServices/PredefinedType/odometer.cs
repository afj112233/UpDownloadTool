using System;
using System.Diagnostics;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using Newtonsoft.Json.Linq;
namespace ICSStudio.SimpleServices.PredefinedType
{
    public sealed class ODOMETER : NativeType
    {
        private ODOMETER()
        {
            Name = "ODOMETER";
            {
                var member = new DataTypeMember
                {
                    Name = "Data",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = INT.Inst,
                    Dim1 = 5,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 0,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
        }
        public static readonly ODOMETER Inst = new ODOMETER();
        public override IField Create(JToken token)
        {
            var res = new ODOMETERField(token);
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
        public override int BitSize => 128;
        public override int ByteSize => 16;
        public const int size = 16;
    }
    public sealed class ODOMETERField : PreDefinedField
    {
        public ODOMETERField(JToken token)
        {
            Debug.Assert(token == null || token is JArray);
            var array = token as JArray;
            {
                var tmp = array?[0] as JArray;
                var arr = new ArrayField(5, 0, 0);
                arr.Add(Tuple.Create(new Int16Field(tmp?[0]) as IField, 0));
                arr.Add(Tuple.Create(new Int16Field(tmp?[1]) as IField, 2));
                arr.Add(Tuple.Create(new Int16Field(tmp?[2]) as IField, 4));
                arr.Add(Tuple.Create(new Int16Field(tmp?[3]) as IField, 6));
                arr.Add(Tuple.Create(new Int16Field(tmp?[4]) as IField, 8));
                Debug.Assert(tmp == null || arr.Size() == tmp.Count);
                fields.Add(Tuple.Create(arr as IField, 0));
            }
        }
    }
}