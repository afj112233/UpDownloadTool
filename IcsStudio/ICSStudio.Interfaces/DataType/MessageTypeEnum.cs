using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;


namespace ICSStudio.Interfaces.DataType
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum MessageTypeEnum : byte
    {
        Unconfigured = 0,

        [EnumMember(Value = "CIP Data Table Read")]
        CIPDataTableRead,

        [EnumMember(Value = "CIP Data Table Write")]
        CIPDataTableWrite,

        [EnumMember(Value = "CIP Generic")] CIPGeneric,

        [EnumMember(Value = "PLC2 Unprotected Read")]
        PLC2UnprotectedRead,

        [EnumMember(Value = "PLC2 Unprotected Write")]
        PLC2UnprotectedWrite,

        [EnumMember(Value = "PLC3 Word Range Read")]
        PLC3WordRangeRead,

        [EnumMember(Value = "PLC3 Word Range Write")]
        PLC3WordRangeWrite,

        [EnumMember(Value = "PLC3 Typed Read")]
        PLC3TypedRead,

        [EnumMember(Value = "PLC3 Typed Write")]
        PLC3TypedWrite,

        [EnumMember(Value = "PLC5 Word Range Read")]
        PLC5WordRangeRead,

        [EnumMember(Value = "PLC5 Word Range Write")]
        PLC5WordRangeWrite,

        [EnumMember(Value = "PLC5 Typed Read")]
        PLC5TypedRead,

        [EnumMember(Value = "PLC5 Typed Write")]
        PLC5TypedWrite,

        [EnumMember(Value = "SLC Typed Read")] SLCTypedRead,

        [EnumMember(Value = "SLC Typed Write")]
        SLCTypedWrite,

        [EnumMember(Value = "Block Transfer Read")]
        BlockTransferRead,

        [EnumMember(Value = "Block Transfer Write")]
        BlockTransferWrite,

        [EnumMember(Value = "Module Reconfigure")]
        ModuleReconfigure,

        [EnumMember(Value = "SERCOS IDN Read")]
        SERCOSIDNRead,

        [EnumMember(Value = "SERCOS IDN Write")]
        SERCOSIDNWrite,
    }
}
