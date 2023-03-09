using System.ComponentModel;
using ICSStudio.Interfaces.Tags;
using System.Runtime.Serialization;
using ICSStudio.Gui.Converters;

namespace ICSStudio.Interfaces.DataType
{
    [TypeConverter(typeof(EnumMemberValueConverter))]
    public enum DisplayStyle
    {
        [EnumMember(Value = "NullType")]
        NullStyle,
        General,
        Binary,
        Octal,
        Decimal,
        Hex,
        Exponential,
        Float,
        [EnumMember(Value = "ASCII")] Ascii,
        Unicode,
        DateTime,
        UseTypeStyle,
        DateTimeNS,
    }

    [TypeConverter(typeof(EnumMemberValueConverter))]
    public enum ExternalAccess
    {
        [Browsable(false)] NullExternalAccess = -1,
        [EnumMember(Value = "Read/Write")] ReadWrite = 0,
        [Browsable(false)] Undefined = 1,
        [EnumMember(Value = "Read Only")] ReadOnly = 2,
        [EnumMember(Value = "None")] None = 3,
    }

    public interface IDataTypeMember: INotifyPropertyChanged
    {
        IDataType ParentDataType { get; }

        string Name { get; }

        string Description { get; }

        int DataTypeUid { get; }

        DataTypeInfo DataTypeInfo { get;}

        DisplayStyle DisplayStyle { get; }

        ExternalAccess ExternalAccess { get; }

        bool IsHidden { get; }

        bool IsBit { get; }

        int ByteOffset { get; }

        int BitOffset { get; }
    }
}
