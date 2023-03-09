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
    public class RedundancyInfoType
    {
        [XmlAttribute] public BoolEnum Enabled { get; set; }

        [XmlAttribute] public BoolEnum KeepTestEditsOnSwitchOver { get; set; }

        [XmlAttribute] public ushort IOMemoryPadPercentage { get; set; }

        [XmlIgnore] public bool IOMemoryPadPercentageSpecified { get; set; }

        [XmlAttribute] public ushort DataTablePadPercentage { get; set; }

        [XmlIgnore] public bool DataTablePadPercentageSpecified { get; set; }
    }
}
