using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace ICSStudio.TagToL5XTest
{
    [GeneratedCode("xsd", "4.6.81.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlRoot("RSLogix5000Content", IsNullable = false, Namespace = "")]
    [Serializable]
    public class RsLogix5000ContentType
    {
        public ControllerType Controller { get; set; }
    }
}
