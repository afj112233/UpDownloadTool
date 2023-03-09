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
    public class OutputTagType
    {
        [XmlAttribute] public ExternalAccessEnum ExternalAccess { get; set; }

        [XmlIgnore] public bool ExternalAccessSpecified { get; set; }

        [XmlElement("Comments", typeof(CommentCollection))]
        [XmlElement("Data", typeof(Data))]
        [XmlElement("Description", typeof(DescriptionType))]
        [XmlElement("ForceData", typeof(ForceData))]
        public object[] Items { get; set; }
    }
}
