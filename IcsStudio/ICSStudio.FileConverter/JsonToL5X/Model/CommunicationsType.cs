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
    public class CommunicationsType
    {
        [XmlElement("ConfigData", typeof(ConfigDataType))]
        [XmlElement("ConfigScript", typeof(ConfigScriptType))]
        [XmlElement("ConfigTag", typeof(ConfigTagType))]
        [XmlElement("Connections", typeof(ConnectionCollection))]
        [XmlElement("SafetyScript", typeof(SafetyScriptType))]
        public object[] Items { get; set; }

        [XmlAttribute] public ulong CommMethod { get; set; }
        [XmlIgnore] public bool CommMethodSpecified { get; set; }

        [XmlAttribute] public ushort PrimCxnInputSize { get; set; }
        [XmlIgnore] public bool PrimCxnInputSizeSpecified { get; set; }

        [XmlAttribute] public ushort PrimCxnOutputSize { get; set; }
        [XmlIgnore] public bool PrimCxnOutputSizeSpecified { get; set; }
    }
}
