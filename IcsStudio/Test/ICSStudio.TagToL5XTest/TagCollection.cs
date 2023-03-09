using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace ICSStudio.TagToL5XTest
{
    [GeneratedCode("xsd", "4.6.81.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class TagCollection
    {
        [XmlElement("Tag")] public TagType[] Tag { get; set; }
    }

    [GeneratedCode("xsd", "4.6.81.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class TagType
    {
        [XmlElement("Data", typeof(Data))] public object[] Items { get; set; }

        [XmlAttribute] public string Name { get; set; }


        [XmlAttribute] public string DataType { get; set; }

        [XmlAttribute] public RadixEnum Radix { get; set; }
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

    [GeneratedCode("xsd", "4.6.81.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class Data
    {

        [XmlElement("Array", typeof(DataArray))]
        [XmlElement("DataValue", typeof(DataValue))]
        [XmlElement("Structure", typeof(DataStructure))]
        public object[] Items { get; set; }

        [XmlAttribute] public string Format { get; set; }
    }

    [GeneratedCode("xsd", "4.6.81.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class DataArray
    {
        [XmlElement("Element")] public DataArrayElement[] Element { get; set; }

        [XmlAttribute] public string DataType { get; set; }
        [XmlAttribute] public string Dimensions { get; set; }
    }

    [GeneratedCode("xsd", "4.6.81.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class DataArrayElement
    {
        [XmlAttribute] public string Index { get; set; }
        [XmlElement("Structure")] public DataStructure[] Structure { get; set; }
    }

    [GeneratedCode("xsd", "4.6.81.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class DataStructure
    {
        [XmlElement("ArrayMember", typeof(DataArray))]
        [XmlElement("DataValueMember", typeof(DataValue))]
        [XmlElement("StructureMember", typeof(DataStructure))]
        public object[] Items { get; set; }

        [XmlAttribute] public string DataType { get; set; }
        [XmlAttribute] public string Name { get; set; }
    }

    [GeneratedCode("xsd", "4.6.81.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class DataValue
    {
        [XmlAttribute] public string Name { get; set; }

        [XmlAttribute] public string DataType { get; set; }
        [XmlAttribute] public string Value { get; set; }
        [XmlAttribute] public RadixEnum Radix { get; set; }
    }
}
