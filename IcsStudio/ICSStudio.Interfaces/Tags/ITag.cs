using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using ICSStudio.Gui.Converters;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;

namespace ICSStudio.Interfaces.Tags
{
    [TypeConverter(typeof(EnumMemberValueConverter))]
    public enum TagType
    {
        Base,
        Alias,
        Produced,
        Consumed,
        Undefined,
    }

    [TypeConverter(typeof(EnumMemberValueConverter))]
    public enum Usage
    {
        [EnumMember(Value = "Static")]
        Static,
        [EnumMember(Value = "Input")]
        Input,
        [EnumMember(Value = "Output")]
        Output,
        [EnumMember(Value = "InOut")]
        InOut,
        [EnumMember(Value = "Local")]
        Local,
        [EnumMember(Value = "Public")]
        SharedData,
        [EnumMember(Value = "InputArg")]
        InputArg,
        [EnumMember(Value = "OutputArg")]
        OutputArg,
        [EnumMember(Value = "NullParameterType")]
        NullParameterType,
    }

    public class ExtendUsage
    {
        public static string ToString(Usage usage)
        {
            var attribute = Attribute.GetCustomAttribute(usage.GetType().GetField(usage.ToString()),
                typeof(EnumMemberAttribute)) as EnumMemberAttribute;
            return attribute?.Value;
        }
    }

    public interface ITag : IBaseComponent, ITrackedComponent
    {
        ITagCollection ParentCollection { get; }

        DataTypeInfo DataTypeInfo { get; set; }

        int TargetUid { get; }

        string AliasSpecifier { get; set; }

        bool IsAlias { get; }

        bool IsConsuming { get; }
        bool IsProducing { get; }
        bool IsProgramTag { get; }
        bool IsUDITag { get; }

        string AliasBaseSpecifier { get; }

        ITag BaseTag { get; }

        string OldName { get; }

        DisplayStyle DisplayStyle { get; set; }

        Usage Usage { get; set; }

        ExternalAccess ExternalAccess { get; set; }

        bool IsConstant { get; set; }

        bool IsSequencing { get; set; }

        bool IsHidden { get; }

        bool IsReadOnly { get; }

        bool IsStorage { get; }

        bool IsProgramParameter { get; }

        bool IsControllerScoped { get; }

        bool IsRequired { get; }

        bool IsVisible { get; }

        string FullName { get; }

        byte[] GetByteValue(int byteOffset, int byteLength);

        void SetByteValue(int byteOffset, int byteLength, byte[] buffer);

        void SetStringValue(string specifier, string value);

        bool GetBitValue(int bitOffset);

        void SetBitValue(int bitOffset, bool value);

        string GetMemberValue(string member, bool allowPrivateMemberReferences);

        bool IsDataTruncated(int uid, int dim1, int dim2, int dim3);

        string GetQualifierDescription(string qualifier, ref bool passThrough,
            bool allowPrivateMemberReferences = false);

        void SetQualifierDescription(string qualifier, string description);

        string GetMetadataValueString(
            MetadataDefinitionID metadataID,
            string qualifier,
            ref bool passThrough,
            bool allowPrivateMemberReferences = false);

        void SetMetadataValueString(
            MetadataDefinitionID metadataID,
            string qualifier,
            string description);
    }
}
