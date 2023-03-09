using System;
using System.Diagnostics;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using Newtonsoft.Json.Linq;
namespace ICSStudio.SimpleServices.PredefinedType
{
    public sealed class SEQ_STRING : NativeType
    {
        private SEQ_STRING()
        {
            Name = "SEQ_STRING";
            {
                var member = new DataTypeMember
                {
                    Name = "Value",
                    DisplayStyle = DisplayStyle.NullStyle,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = STRING.Inst,
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
                    Name = "InitialValue",
                    DisplayStyle = DisplayStyle.NullStyle,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 88,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = STRING.Inst,
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
                    Name = "Valid",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 176,
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
        }
        public static readonly SEQ_STRING Inst = new SEQ_STRING();
        public override IField Create(JToken token)
        {
            var res = new SEQ_STRINGField(token);
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
        public override int BitSize => 1472;
        public override int ByteSize => 184;
        public const int size = 184;
    }
    public sealed class SEQ_STRINGField : PreDefinedField
    {
        public SEQ_STRINGField(JToken token)
        {
            Debug.Assert(token == null || token is JArray);
            var array = token as JArray;
            fields.Add(Tuple.Create(new STRINGField(array?[0]) as IField, 0));
            fields.Add(Tuple.Create(new STRINGField(array?[1]) as IField, 88));
            fields.Add(Tuple.Create(new Int8Field(array?[2]) as IField, 176));
        }
    }
}