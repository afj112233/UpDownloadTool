using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace ICSStudio.FileConverter.JsonToL5X.Model
{
    [GeneratedCode("xsd", "4.8.3928.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class DataValue
    {
        [XmlAttribute] public string Name { get; set; }

        [XmlAttribute] public string DataType { get; set; }

        [XmlAttribute] public RadixEnum Radix { get; set; }

        [XmlIgnore] public bool RadixSpecified { get; set; }

        [XmlAttribute] public string Value { get; set; }


        [XmlText] public XmlNode[] Text { get; set; }
    }
}
