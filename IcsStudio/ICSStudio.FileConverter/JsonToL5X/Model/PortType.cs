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
    public class PortType
    {
        [XmlAttribute] public ushort Id { get; set; }
        [XmlAttribute] public string Address { get; set; }
        [XmlAttribute] public string Type { get; set; }
        [XmlAttribute] public BoolEnum Upstream { get; set; }

        public BusType Bus { get; set; }
    }
}
