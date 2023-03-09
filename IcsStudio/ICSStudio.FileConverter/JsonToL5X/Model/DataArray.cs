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
    public class DataArray
    {
        [XmlElement("Element")] public DataArrayElement[] Element { get; set; }

        [XmlAttribute] public string Name { get; set; }

        [XmlAttribute] public string DataType { get; set; }

        [XmlAttribute] public string Dimensions { get; set; }

        [XmlAttribute] public RadixEnum Radix { get; set; }

        [XmlIgnore] public bool RadixSpecified { get; set; }
    }
}
