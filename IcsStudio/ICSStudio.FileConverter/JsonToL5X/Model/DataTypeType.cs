using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace ICSStudio.FileConverter.JsonToL5X.Model
{
    [GeneratedCode("xsd", "4.8.3928.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class DataTypeType
    {
        [XmlAttribute] public string Name { get; set; }

        [XmlAttribute] public DTFamilyTypeEnum Family { get; set; }

        [XmlAttribute] public DTClassTypeEnum Class { get; set; }

        public DescriptionType Description { get; set; }

        [XmlArrayItem("Member", IsNullable = false)]
        public DataTypeMemberType[] Members { get; set; }
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum DTFamilyTypeEnum
    {
        NoFamily,
        StringFamily,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum DTClassTypeEnum
    {
        User,
        ProductDefined,
        IO,
    }
}
