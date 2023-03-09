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
    public class ProgramType
    {
        [XmlAttribute] public string Name { get; set; }
        [XmlAttribute] public BoolEnum TestEdits { get; set; }
        [XmlAttribute] public string MainRoutineName { get; set; }

        [XmlAttribute] public BoolEnum Disabled { get; set; }
        [XmlAttribute] public BoolEnum UseAsFolder { get; set; }

        public TagCollection Tags { get; set; }

        public RoutineCollection Routines
        {
            get;
            set;
        }
        }
}
