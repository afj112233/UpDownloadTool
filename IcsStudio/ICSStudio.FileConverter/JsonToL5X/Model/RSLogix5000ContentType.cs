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
    [XmlRoot("RSLogix5000Content", IsNullable = false, Namespace = "")]
    [Serializable]
    public class RSLogix5000ContentType
    {
        public ControllerType Controller { get; set; }

        [XmlAttribute] public string SchemaRevision { get; set; }

        [XmlAttribute] public string SoftwareRevision { get; set; }

        [XmlAttribute] public string TargetName { get; set; }

        [XmlAttribute] public string TargetType { get; set; }

        [XmlAttribute] public BoolEnum ContainsContext { get; set; }

        [XmlAttribute] public string ExportDate { get; set; }

        [XmlAttribute] public string ExportOptions { get; set; }
    }
}
