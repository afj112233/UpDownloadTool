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
    public class AOIParameterType
    {
        [XmlAttribute] public string Name { get; set; }

        [XmlAttribute] public TagTypeEnum TagType { get; set; }

        [XmlAttribute] public string DataType { get; set; }

        [XmlAttribute] public string Dimensions { get; set; }

        [XmlAttribute] public TagUsageEnum Usage { get; set; }

        [XmlAttribute] public RadixEnum Radix { get; set; }

        [XmlIgnore] public bool RadixSpecified { get; set; }

        [XmlAttribute] public BoolEnum Required { get; set; }

        [XmlAttribute] public BoolEnum Visible { get; set; }

        [XmlAttribute] public BoolEnum Constant { get; set; }
        [XmlIgnore] public bool ConstantSpecified { get; set; }

        [XmlAttribute] public ExternalAccessEnum ExternalAccess { get; set; }

        [XmlIgnore] public bool ExternalAccessSpecified { get; set; }

        [XmlElement("Comments", typeof(CommentCollection))]
        [XmlElement("DefaultData", typeof(Data))]
        [XmlElement("Description", typeof(DescriptionType))]
        public object[] Items { get; set; }
    }

    [GeneratedCode("xsd", "4.6.81.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class CommentCollection
    {
    }

    [GeneratedCode("xsd", "4.6.81.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class AlmaType
    {
    }

    [GeneratedCode("xsd", "4.6.81.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class AlarmConfigType
    {
    }

    [GeneratedCode("xsd", "4.6.81.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class AlmdType
    {
    }

    [GeneratedCode("xsd", "4.6.81.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class DataArrayElement
    {
        [XmlElement("Structure")] public DataStructure[] Structure { get; set; }

        [XmlAttribute] public string Index { get; set; }

        [XmlAttribute] public string Value { get; set; }
    }

    [GeneratedCode("xsd", "4.6.81.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class CoordinateSystemType
    {
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

        [XmlAttribute] public string Name { get; set; }

        [XmlAttribute] public string DataType { get; set; }
    }

    [GeneratedCode("xsd", "4.6.81.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class LocalizedDescriptionType
    {
    }
}
