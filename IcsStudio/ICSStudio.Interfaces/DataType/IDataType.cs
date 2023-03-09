using System.Collections.Generic;
using ICSStudio.Interfaces.Common;
using Newtonsoft.Json.Linq;

namespace ICSStudio.Interfaces.DataType
{
    public enum FamilyType
    {
        NoFamily,
        StringFamily,
    }

    public enum DataTypeClass
    {
        Predefined,
        VendorDefined,
        User,
        IO
    }

    public interface IField
    {
        JToken ToJToken();
        IField DeepCopy();
        void ToMsgPack(List<byte> data);
    }

    public interface IDataType : IBaseComponent
    {

        int BitSize { get; }
        int ByteSize { get; }
        int BaseDataSize { get; }
        int AlignSize { get; }

        FamilyType FamilyType { get; }

        bool IsStringType { get; }

        bool IsIOType { get; }

        bool IsVendorDefinedType { get; set; }

        bool IsPredefinedType { get; }

        bool IsUDIDefinedType { get; set; }

        bool IsSupported { get; }

        bool IsAtomic { get; }

        bool IsInteger { get; }

        bool IsNumber { get; }

        bool IsBool { get; }

        bool IsLINT { get; }
        bool IsReal { get; }

        bool IsStruct { get; }

        bool IsGlobalScopeOnly { get; }

        bool IsAtomicOnly { get; }

        bool IsSequenceInteractionType { get; }

        bool SupportsOneDimensionalArray { get; }

        bool SupportsMultiDimensionalArrays { get; }

        bool IsAxisType { get; }

        bool IsMotionGroupType { get; }

        bool IsCoordinateSystemType { get; }

        bool IsMessageType { get; }

        bool IsAlarmDigitalType { get; }

        bool IsAlarmAnalogType { get; }
        
        DisplayStyle DefaultDisplayStyle { get; }

        DisplayStyle[] GetValidDisplayStyles();

        bool IsValidString(string value, ref uint size);

        IField Create(JToken token);

        IField FixDataField(JToken broken);
    }
}
