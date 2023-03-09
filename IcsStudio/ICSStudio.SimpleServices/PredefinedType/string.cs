using System;
using System.Diagnostics;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using Newtonsoft.Json.Linq;
namespace ICSStudio.SimpleServices.PredefinedType
{
    public sealed class STRING : NativeType
    {
        private STRING()
        {
            Name = "STRING";
            {
                var member = new DataTypeMember
                {
                    Name = "LEN",
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
                    Name = "DATA",
                    DisplayStyle = DisplayStyle.Ascii,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 4,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = SINT.Inst,
                    Dim1 = 82,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 1,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
        }
        public static readonly STRING Inst = new STRING();
        public override IField Create(JToken token)
        {
            var res = new STRINGField(token);
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
        public override int BitSize => 704;
        public override int ByteSize => 88;
        public const int size = 88;
        public readonly int SIZE = 82;
        public override bool IsStringType => true;
        public override FamilyType FamilyType => FamilyType.StringFamily;
    }
    public sealed class STRINGField : PreDefinedField
    {
        public STRINGField(JToken token)
        {
            Debug.Assert(token == null || token is JArray);
            var array = token as JArray;
            fields.Add(Tuple.Create(new Int32Field(array?[0]) as IField, 0));
            {
                var tmp = array?[1] as JArray;
                var arr = new ArrayField(82, 0, 0);
                arr.Add(Tuple.Create(new Int8Field(tmp?[0]) as IField, 0));
                arr.Add(Tuple.Create(new Int8Field(tmp?[1]) as IField, 1));
                arr.Add(Tuple.Create(new Int8Field(tmp?[2]) as IField, 2));
                arr.Add(Tuple.Create(new Int8Field(tmp?[3]) as IField, 3));
                arr.Add(Tuple.Create(new Int8Field(tmp?[4]) as IField, 4));
                arr.Add(Tuple.Create(new Int8Field(tmp?[5]) as IField, 5));
                arr.Add(Tuple.Create(new Int8Field(tmp?[6]) as IField, 6));
                arr.Add(Tuple.Create(new Int8Field(tmp?[7]) as IField, 7));
                arr.Add(Tuple.Create(new Int8Field(tmp?[8]) as IField, 8));
                arr.Add(Tuple.Create(new Int8Field(tmp?[9]) as IField, 9));
                arr.Add(Tuple.Create(new Int8Field(tmp?[10]) as IField, 10));
                arr.Add(Tuple.Create(new Int8Field(tmp?[11]) as IField, 11));
                arr.Add(Tuple.Create(new Int8Field(tmp?[12]) as IField, 12));
                arr.Add(Tuple.Create(new Int8Field(tmp?[13]) as IField, 13));
                arr.Add(Tuple.Create(new Int8Field(tmp?[14]) as IField, 14));
                arr.Add(Tuple.Create(new Int8Field(tmp?[15]) as IField, 15));
                arr.Add(Tuple.Create(new Int8Field(tmp?[16]) as IField, 16));
                arr.Add(Tuple.Create(new Int8Field(tmp?[17]) as IField, 17));
                arr.Add(Tuple.Create(new Int8Field(tmp?[18]) as IField, 18));
                arr.Add(Tuple.Create(new Int8Field(tmp?[19]) as IField, 19));
                arr.Add(Tuple.Create(new Int8Field(tmp?[20]) as IField, 20));
                arr.Add(Tuple.Create(new Int8Field(tmp?[21]) as IField, 21));
                arr.Add(Tuple.Create(new Int8Field(tmp?[22]) as IField, 22));
                arr.Add(Tuple.Create(new Int8Field(tmp?[23]) as IField, 23));
                arr.Add(Tuple.Create(new Int8Field(tmp?[24]) as IField, 24));
                arr.Add(Tuple.Create(new Int8Field(tmp?[25]) as IField, 25));
                arr.Add(Tuple.Create(new Int8Field(tmp?[26]) as IField, 26));
                arr.Add(Tuple.Create(new Int8Field(tmp?[27]) as IField, 27));
                arr.Add(Tuple.Create(new Int8Field(tmp?[28]) as IField, 28));
                arr.Add(Tuple.Create(new Int8Field(tmp?[29]) as IField, 29));
                arr.Add(Tuple.Create(new Int8Field(tmp?[30]) as IField, 30));
                arr.Add(Tuple.Create(new Int8Field(tmp?[31]) as IField, 31));
                arr.Add(Tuple.Create(new Int8Field(tmp?[32]) as IField, 32));
                arr.Add(Tuple.Create(new Int8Field(tmp?[33]) as IField, 33));
                arr.Add(Tuple.Create(new Int8Field(tmp?[34]) as IField, 34));
                arr.Add(Tuple.Create(new Int8Field(tmp?[35]) as IField, 35));
                arr.Add(Tuple.Create(new Int8Field(tmp?[36]) as IField, 36));
                arr.Add(Tuple.Create(new Int8Field(tmp?[37]) as IField, 37));
                arr.Add(Tuple.Create(new Int8Field(tmp?[38]) as IField, 38));
                arr.Add(Tuple.Create(new Int8Field(tmp?[39]) as IField, 39));
                arr.Add(Tuple.Create(new Int8Field(tmp?[40]) as IField, 40));
                arr.Add(Tuple.Create(new Int8Field(tmp?[41]) as IField, 41));
                arr.Add(Tuple.Create(new Int8Field(tmp?[42]) as IField, 42));
                arr.Add(Tuple.Create(new Int8Field(tmp?[43]) as IField, 43));
                arr.Add(Tuple.Create(new Int8Field(tmp?[44]) as IField, 44));
                arr.Add(Tuple.Create(new Int8Field(tmp?[45]) as IField, 45));
                arr.Add(Tuple.Create(new Int8Field(tmp?[46]) as IField, 46));
                arr.Add(Tuple.Create(new Int8Field(tmp?[47]) as IField, 47));
                arr.Add(Tuple.Create(new Int8Field(tmp?[48]) as IField, 48));
                arr.Add(Tuple.Create(new Int8Field(tmp?[49]) as IField, 49));
                arr.Add(Tuple.Create(new Int8Field(tmp?[50]) as IField, 50));
                arr.Add(Tuple.Create(new Int8Field(tmp?[51]) as IField, 51));
                arr.Add(Tuple.Create(new Int8Field(tmp?[52]) as IField, 52));
                arr.Add(Tuple.Create(new Int8Field(tmp?[53]) as IField, 53));
                arr.Add(Tuple.Create(new Int8Field(tmp?[54]) as IField, 54));
                arr.Add(Tuple.Create(new Int8Field(tmp?[55]) as IField, 55));
                arr.Add(Tuple.Create(new Int8Field(tmp?[56]) as IField, 56));
                arr.Add(Tuple.Create(new Int8Field(tmp?[57]) as IField, 57));
                arr.Add(Tuple.Create(new Int8Field(tmp?[58]) as IField, 58));
                arr.Add(Tuple.Create(new Int8Field(tmp?[59]) as IField, 59));
                arr.Add(Tuple.Create(new Int8Field(tmp?[60]) as IField, 60));
                arr.Add(Tuple.Create(new Int8Field(tmp?[61]) as IField, 61));
                arr.Add(Tuple.Create(new Int8Field(tmp?[62]) as IField, 62));
                arr.Add(Tuple.Create(new Int8Field(tmp?[63]) as IField, 63));
                arr.Add(Tuple.Create(new Int8Field(tmp?[64]) as IField, 64));
                arr.Add(Tuple.Create(new Int8Field(tmp?[65]) as IField, 65));
                arr.Add(Tuple.Create(new Int8Field(tmp?[66]) as IField, 66));
                arr.Add(Tuple.Create(new Int8Field(tmp?[67]) as IField, 67));
                arr.Add(Tuple.Create(new Int8Field(tmp?[68]) as IField, 68));
                arr.Add(Tuple.Create(new Int8Field(tmp?[69]) as IField, 69));
                arr.Add(Tuple.Create(new Int8Field(tmp?[70]) as IField, 70));
                arr.Add(Tuple.Create(new Int8Field(tmp?[71]) as IField, 71));
                arr.Add(Tuple.Create(new Int8Field(tmp?[72]) as IField, 72));
                arr.Add(Tuple.Create(new Int8Field(tmp?[73]) as IField, 73));
                arr.Add(Tuple.Create(new Int8Field(tmp?[74]) as IField, 74));
                arr.Add(Tuple.Create(new Int8Field(tmp?[75]) as IField, 75));
                arr.Add(Tuple.Create(new Int8Field(tmp?[76]) as IField, 76));
                arr.Add(Tuple.Create(new Int8Field(tmp?[77]) as IField, 77));
                arr.Add(Tuple.Create(new Int8Field(tmp?[78]) as IField, 78));
                arr.Add(Tuple.Create(new Int8Field(tmp?[79]) as IField, 79));
                arr.Add(Tuple.Create(new Int8Field(tmp?[80]) as IField, 80));
                arr.Add(Tuple.Create(new Int8Field(tmp?[81]) as IField, 81));
                Debug.Assert(tmp == null || arr.Size() == tmp.Count);
                fields.Add(Tuple.Create(arr as IField, 4));
            }
        }
    }
}