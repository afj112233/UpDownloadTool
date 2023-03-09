using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ICSStudio.FileConverter.JsonToL5X.Model
{
    [GeneratedCode("xsd", "4.8.3928.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class TimeSynchronizeType
    {
        [XmlAttribute] public byte Priority1 { get; set; }
        [XmlAttribute] public byte Priority2 { get; set; }
        [XmlAttribute] public BoolEnum PTPEnable { get; set; }
    }
}
