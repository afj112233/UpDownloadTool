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
    public class RoutineType
    {
        [XmlElement("Description")] public DescriptionType[] Description { get; set; }

        [XmlElement("RLLContent")] public RLLContentType[] RLLContent { get; set; }

        [XmlElement("STContent")] public STContentType[] STContent { get; set; }

        [XmlAttribute] public string Name { get; set; }

        [XmlAttribute] public RoutineTypeEnum Type { get; set; }

    }
}
