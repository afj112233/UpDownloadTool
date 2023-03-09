using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace ICSStudio.FileConverter.JsonToL5X.Model
{
    [GeneratedCode("xsd", "4.6.81.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class ModuleType
    {
        [XmlAttribute] public string Name { get; set; }

        [XmlAttribute] public string CatalogNumber { get; set; }

        [XmlAttribute] public ushort Vendor { get; set; }

        [XmlAttribute] public ushort ProductType { get; set; }

        [XmlAttribute] public ushort ProductCode { get; set; }

        [XmlAttribute] public byte Major { get; set; }

        [XmlAttribute] public byte Minor { get; set; }

        [XmlAttribute] public string ParentModule { get; set; }

        [XmlAttribute] public ushort ParentModPortId { get; set; }

        [XmlAttribute] public BoolEnum Inhibited { get; set; }

        [XmlAttribute] public BoolEnum MajorFault { get; set; }

        public ModuleEKeyType EKey { get; set; }

        [XmlArrayItem("Port", IsNullable = false)]
        public PortType[] Ports { get; set; }

        public CommunicationsType Communications { get; set; }

        public XmlElement ExtendedProperties { get; set; }
    }
}