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
    public class MotionGroupType
    {
        [XmlAttribute] public ulong CoarseUpdatePeriod { get; set; }

        [XmlIgnore] public bool CoarseUpdatePeriodSpecified { get; set; }

        [XmlAttribute] public ulong PhaseShift { get; set; }

        [XmlIgnore] public bool PhaseShiftSpecified { get; set; }

        [XmlAttribute] public MGGeneralFaultEnum GeneralFaultType { get; set; }

        [XmlIgnore] public bool GeneralFaultTypeSpecified { get; set; }

        [XmlAttribute] public EnabledEnum AutoTagUpdate { get; set; }

        [XmlAttribute] public int Alternate1UpdateMultiplier { get; set; }

        [XmlAttribute] public int Alternate2UpdateMultiplier { get; set; }
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum MGGeneralFaultEnum
    {
        [XmlEnum("Non Major Fault")] NonMajorFault,
        [XmlEnum("Major Fault")] MajorFault,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum EnabledEnum
    {
        Disabled,
        Enabled,
        [XmlEnum("0")] Item0,
        [XmlEnum("1")] Item1,
    }
}
