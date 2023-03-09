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
    public class AOILocalTagType
    {

        [XmlAttribute] public string Name { get; set; }

        [XmlAttribute] public string DataType { get; set; }

        [XmlAttribute] public string Dimensions { get; set; }

        [XmlAttribute] public RadixEnum Radix { get; set; }
        [XmlIgnore] public bool RadixSpecified { get; set; }

        [XmlAttribute] public ExternalAccessEnum ExternalAccess { get; set; }

        [XmlElement("Comments", typeof(CommentCollection))]
        [XmlElement("DefaultData", typeof(Data))]
        [XmlElement("Description", typeof(DescriptionType))]
        public object[] Items { get; set; }
    }
}
