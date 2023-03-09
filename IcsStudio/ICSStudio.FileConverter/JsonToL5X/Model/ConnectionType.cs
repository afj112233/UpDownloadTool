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
    public class ConnectionType
    {
        [XmlAttribute] public string Name { get; set; }

        [XmlAttribute] public ulong RPI { get; set; }
        [XmlAttribute] public ConnTypeEnum Type { get; set; }

        [XmlAttribute] public ushort InputCxnPoint { get; set; }

        [XmlIgnore] public bool InputCxnPointSpecified { get; set; }

        [XmlAttribute] public ushort OutputCxnPoint { get; set; }

        [XmlIgnore] public bool OutputCxnPointSpecified { get; set; }

        [XmlAttribute] public ushort InputSize { get; set; }

        [XmlIgnore] public bool InputSizeSpecified { get; set; }

        [XmlAttribute] public ushort OutputSize { get; set; }

        [XmlIgnore] public bool OutputSizeSpecified { get; set; }


        [XmlAttribute] public ulong EventID { get; set; }
        [XmlAttribute] public BoolEnum ProgrammaticallySendEventTrigger { get; set; }

        [XmlAttribute] public BoolEnum Unicast { get; set; }
        [XmlIgnore] public bool UnicastSpecified { get; set; }

        public InputTagType InputTag { get; set; }

        public OutputTagType OutputTag { get; set; }

    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum ConnTypeEnum
    {
        Unknown,
        Input,
        Output,
        DiagnosticInput,
        C2C_RC,
        C2C_SC,
        MotionSync,
        MotionAsync,
        MotionEvent,
        SafetyInput,
        SafetyOutput,
        Safety_C2C_RC,
        Safety_C2C_SC,
        Safety_C2C_SCHB,
        StandardDataDriven,
        SafetyInputDataDriven,
        SafetyOutputDataDriven,
    }
}
