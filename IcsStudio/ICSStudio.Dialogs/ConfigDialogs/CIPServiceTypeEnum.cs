using System.Runtime.Serialization;

namespace ICSStudio.Dialogs.ConfigDialogs
{
    public enum CIPServiceTypeEnum
    {
        AcceptConnection,

        [EnumMember(Value = "Apply Attributes")]
        ApplyAttributes,

        [EnumMember(Value = "Audit Value Get")]
        AuditValueGet,

        [EnumMember(Value = "Changes To Detect Get")]
        ChangesToDetectGet,

        [EnumMember(Value = "Changes To Detect Set")]
        ChangesToDetectSet,

        [EnumMember(Value = "Controller Log Add Entry")]
        ControllerLogAddEntry,

        [EnumMember(Value = "Controller Log Automatic Write Get")]
        ControllerLogAutomaticWriteGet,

        [EnumMember(Value = "Controller Log Automatic Write Set")]
        ControllerLogAutomaticWriteSet,

        [EnumMember(Value = "Controller Log Config Execution Get")]
        ControllerLogConfigExecutionGet,

        [EnumMember(Value = "Controller Log Config Execution Set")]
        ControllerLogConfigExecutionSet,

        [EnumMember(Value = "Controller Log Write To Media")]
        ControllerLogWriteToMedia,
        Custom,
        DeleteSocket,
        [EnumMember(Value = "Device Reset")] DeviceReset,
        [EnumMember(Value = "Device WHO")] DeviceWHO,

        [EnumMember(Value = "Get Attribute Single")]
        GetAttributeSingle,
        OpenConnection,
        [EnumMember(Value = "Parameter Read")] ParameterRead,

        [EnumMember(Value = "Parameter Write")]
        ParameterWrite,

        [EnumMember(Value = "PLS Axis Configuration")]
        PLSAxisConfiguration,

        [EnumMember(Value = "PLS Input Registration")]
        PLSInputRegistration,
        [EnumMember(Value = "PLS Offsets")] PLSOffsets,

        [EnumMember(Value = "PLS Switch Configuration")]
        PLSSwitchConfiguration,
        [EnumMember(Value = "Pulse Test")] PulseTest,
        ReadSocket,

        [EnumMember(Value = "Reset Electronic Fuse")]
        ResetElectronicFuse,

        [EnumMember(Value = "Reset Latched Diagnostics (I)")]
        ResetLatchedDiagnosticsI,

        [EnumMember(Value = "Reset Latched Diagnostics (O)")]
        ResetLatchedDiagnosticsO,

        [EnumMember(Value = "Retrieve CST information")]
        RetrieveCSTInformation,

        [EnumMember(Value = "Set Attribute Single")]
        SetAttributeSingle,
        [EnumMember(Value = "Socket Create")] SocketCreate,

        [EnumMember(Value = "Tracked State Value Get")]
        TrackedStateValueGet,

        [EnumMember(Value = "Unlatch All Alarms (I)")]
        UnlatchAllAlarmsI,

        [EnumMember(Value = "Unlatch All Alarms (O)")]
        UnlatchAllAlarmsO,

        [EnumMember(Value = "Unlatch Analog High Alarm (I)")]
        UnlatchAnalogHighAlarmI,

        [EnumMember(Value = "Unlatch Analog High High Alarm (I)")]
        UnlatchAnalogHighHighAlarmI,

        [EnumMember(Value = "Unlatch Analog Low Alarm (I)")]
        UnlatchAnalogLowAlarmI,

        [EnumMember(Value = "Unlatch Analog Low Low Alarm (I)")]
        UnlatchAnalogLowLowAlarmI,

        [EnumMember(Value = "Unlatch High Alarm (O)")]
        UnlatchHighAlarmO,

        [EnumMember(Value = "Unlatch Low Alarm (O)")]
        UnlatchLowAlarmO,

        [EnumMember(Value = "Unlatch Ramp Alarm (O)")]
        UnlatchRampAlarmO,

        [EnumMember(Value = "Unlatch Rate Alarm (I)")]
        UnlatchRateAlarmI,
        WriteSocket,
    }
}
