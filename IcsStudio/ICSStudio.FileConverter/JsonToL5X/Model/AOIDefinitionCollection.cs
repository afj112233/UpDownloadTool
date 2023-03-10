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
    public class AOIDefinitionCollection
    {
        [XmlElement("AddOnInstructionDefinition", typeof(AOIDefinitionType))]
        [XmlElement("EncodedData", typeof(EncodedAOIDefinitionType))]
        public object[] Items { get; set; }
    }
}
