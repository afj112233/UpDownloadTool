using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace ICSStudio.FileConverter.JsonToL5X.Model
{
    [GeneratedCode("xsd", "4.8.3928.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class ConfigTagType
    {
        [XmlElement("Comments", typeof(CommentCollection))]
        [XmlElement("Data", typeof(Data))]
        [XmlElement("Description", typeof(DescriptionType))]
        public object[] Items { get; set; }

        [XmlAttribute] public ulong ConfigSize { get; set; }

        [XmlIgnore] public bool ConfigSizeSpecified { get; set; }

        [XmlAttribute] public ExternalAccessEnum ExternalAccess { get; set; }

        [XmlIgnore] public bool ExternalAccessSpecified { get; set; }

    }
}
