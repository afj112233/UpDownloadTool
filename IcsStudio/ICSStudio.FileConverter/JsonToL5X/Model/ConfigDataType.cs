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
    public class ConfigDataType
    {
        [XmlAttribute] public ulong ConfigSize { get; set; }

        [XmlIgnore] public bool ConfigSizeSpecified { get; set; }

        [XmlElement("Data")] public Data[] Data { get; set; }
    }
}
