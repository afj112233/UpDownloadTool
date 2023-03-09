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
    public class ModuleEKeyType
    {
        [XmlAttribute] public EKeyingStateEnum State { get; set; }
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum EKeyingStateEnum
    {
        ExactMatch,
        CompatibleModule,
        Disabled,
        Custom,
    }
}
