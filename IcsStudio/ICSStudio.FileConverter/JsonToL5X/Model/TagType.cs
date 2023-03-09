using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace ICSStudio.FileConverter.JsonToL5X.Model
{
    [GeneratedCode("xsd", "4.6.81.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class TagType
    {
        [XmlElement("BaseDescription", typeof(string))]
        [XmlElement("Comments", typeof(CommentCollection))]
        [XmlElement("ConsumeInfo", typeof(ConsumeTagInfoType))]
        [XmlElement("Data", typeof(Data))]
        [XmlElement("Description", typeof(DescriptionType))]
        [XmlElement("ForceData", typeof(ForceData))]
        [XmlElement("ProduceInfo", typeof(ProduceTagInfoType))]
        public object[] Items { get; set; }

        [XmlAttribute] public string Name { get; set; }

        [XmlAttribute("TagType")] public TagTypeEnum TagType1 { get; set; }

        [XmlAttribute] public string DataType { get; set; }

        [XmlAttribute] public string Dimensions { get; set; }

        [XmlAttribute] public TagUsageEnum Usage { get; set; }
        [XmlIgnore] public bool UsageSpecified { get; set; }

        [XmlAttribute] public RadixEnum Radix { get; set; }
        [XmlIgnore] public bool RadixSpecified { get; set; }

        [XmlAttribute] public BoolEnum Constant { get; set; }

        [XmlIgnore] public bool ConstantSpecified { get; set; }

        [XmlAttribute] public ExternalAccessEnum ExternalAccess { get; set; }
    }

    [GeneratedCode("xsd", "4.6.81.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class ConsumeTagInfoType
    {
    }

    [GeneratedCode("xsd", "4.6.81.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class ForceData
    {
    }

    [GeneratedCode("xsd", "4.6.81.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class ProduceTagInfoType
    {
    }

    [GeneratedCode("xsd", "4.6.81.0")]
    [Serializable]
    public enum RadixEnum
    {
        NullType,
        General,
        Binary,
        Octal,
        Decimal,
        Hex,
        Exponential,
        Float,
        Ascii,
        Unicode,
        [XmlEnum("Date/Time")] DateTime,
        [XmlEnum("Date/Time (ns)")] DateTimens,
        UseTypeStyle,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum ExternalAccessEnum
    {
        [XmlEnum("Read/Write")] ReadWrite,
        [XmlEnum("Read Only")] ReadOnly,
        None,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum TagUsageEnum
    {
        Normal,
        Local,
        Input,
        Output,
        InOut,
        Static,
        NULL,
        Public
    }
}
