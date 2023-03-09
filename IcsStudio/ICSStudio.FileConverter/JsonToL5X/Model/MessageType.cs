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
    public class MessageType
    {
        [XmlAttribute("MessageType")] public MessageTypeEnum MessageType1 { get; set; }

        [XmlAttribute] public ushort RequestedLength { get; set; }

        [XmlIgnore] public bool RequestedLengthSpecified { get; set; }

        [XmlAttribute] public ConnectedEnum ConnectedFlag { get; set; }

        [XmlIgnore] public bool ConnectedFlagSpecified { get; set; }

        [XmlAttribute] public string ConnectionPath { get; set; }

        [XmlAttribute] public MsgDF1FlagEnum CommTypeCode { get; set; }

        [XmlIgnore] public bool CommTypeCodeSpecified { get; set; }

        [XmlAttribute] public string ServiceCode { get; set; }

        [XmlAttribute] public string ObjectType { get; set; }

        [XmlAttribute] public ulong TargetObject { get; set; }

        [XmlIgnore] public bool TargetObjectSpecified { get; set; }

        [XmlAttribute] public string AttributeNumber { get; set; }


        [XmlAttribute] public ulong LocalIndex { get; set; }
        [XmlIgnore] public bool LocalIndexSpecified { get; set; }

        [XmlAttribute] public string LocalElement { get; set; }

        [XmlAttribute] public string DestinationTag { get; set; }

        [XmlAttribute] public BoolEnum LargePacketUsage { get; set; }

        [XmlIgnore] public bool LargePacketUsageSpecified { get; set; }
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum MessageTypeEnum
    {
        Unconfigured,
        [XmlEnum("CIP Data Table Read")] CIPDataTableRead,
        [XmlEnum("CIP Data Table Write")] CIPDataTableWrite,
        [XmlEnum("CIP Generic")] CIPGeneric,
        [XmlEnum("PLC2 Unprotected Read")] PLC2UnprotectedRead,
        [XmlEnum("PLC2 Unprotected Write")] PLC2UnprotectedWrite,
        [XmlEnum("PLC3 Word Range Read")] PLC3WordRangeRead,
        [XmlEnum("PLC3 Word Range Write")] PLC3WordRangeWrite,
        [XmlEnum("PLC3 Typed Read")] PLC3TypedRead,
        [XmlEnum("PLC3 Typed Write")] PLC3TypedWrite,
        [XmlEnum("PLC5 Word Range Read")] PLC5WordRangeRead,
        [XmlEnum("PLC5 Word Range Write")] PLC5WordRangeWrite,
        [XmlEnum("PLC5 Typed Read")] PLC5TypedRead,
        [XmlEnum("PLC5 Typed Write")] PLC5TypedWrite,
        [XmlEnum("SLC Typed Read")] SLCTypedRead,
        [XmlEnum("SLC Typed Write")] SLCTypedWrite,
        [XmlEnum("Block Transfer Read")] BlockTransferRead,
        [XmlEnum("Block Transfer Write")] BlockTransferWrite,
        [XmlEnum("Module Reconfigure")] ModuleReconfigure,
        [XmlEnum("SERCOS IDN Read")] SERCOSIDNRead,
        [XmlEnum("SERCOS IDN Write")] SERCOSIDNWrite,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum ConnectedEnum
    {
        [XmlEnum("1")] Item1,
        [XmlEnum("2")] Item2,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum MsgDF1FlagEnum
    {
        [XmlEnum("0")] Item0,
        [XmlEnum("1")] Item1,
        [XmlEnum("2")] Item2,
        [XmlEnum("3")] Item3,
        [XmlEnum("4")] Item4,
        [XmlEnum("5")] Item5,
        [XmlEnum("6")] Item6,
    }
}
