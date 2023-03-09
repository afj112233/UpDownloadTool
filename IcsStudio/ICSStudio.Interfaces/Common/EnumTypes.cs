using System;

namespace ICSStudio.Interfaces.Common
{
    public enum ComponentType
    {
        Offline = -1,
        NullType = 0,
        DeviceIdentity = 1,
        TimeSynchronize = 67, // 0x00000043
        DeviceLevelRing = 71, // 0x00000047
        ExtDevice = 100, // 0x00000064
        ICP = 102, // 0x00000066
        PCCC = 103, // 0x00000067
        Program = 104, // 0x00000068
        Device = 105, // 0x00000069
        DataTable = 106, // 0x0000006A
        Tag = 107, // 0x0000006B
        DataType = 108, // 0x0000006C
        Routine = 109, // 0x0000006D
        ERRD = 110, // 0x0000006E
        SerialPort = 111, // 0x0000006F
        Task = 112, // 0x00000070
        DataLog = 113, // 0x00000071
        Memory = 114, // 0x00000072
        FaultLog = 115, // 0x00000073
        CST = 119, // 0x00000077
        Connection = 126, // 0x0000007E
        BIF = 127, // 0x0000007F
        WCT = 139, // 0x0000008B
        Label = 140, // 0x0000008C
        Message = 141, // 0x0000008D
        Controller = 142, // 0x0000008E
        DF1Driver = 162, // 0x000000A2
        ASCIIDriver = 163, // 0x000000A3
        ChangeLog = 172, // 0x000000AC
        AxisGroup = 176, // 0x000000B0
        MotionGroup = 176, // 0x000000B0
        Axis = 177, // 0x000000B1
        Trend = 178, // 0x000000B2
        Redundancy = 192, // 0x000000C0
        TCPIP = 245, // 0x000000F5
        EthernetLink = 246, // 0x000000F6
        VirtualBackplane = 768, // 0x00000300
        Transition = 790, // 0x00000316
        Chart = 791, // 0x00000317
        Leg = 791, // 0x00000317
        Step = 792, // 0x00000318
        Action = 793, // 0x00000319
        FileManager = 794, // 0x0000031A
        CoordinateSystem = 811, // 0x0000032B
        EquipmentPhase = 813, // 0x0000032D
        ALMD = 817, // 0x00000331
        ALMA = 818, // 0x00000332
        SafetyController = 820, // 0x00000334
        AlarmBuffer = 823, // 0x00000337
        UDIDefinition = 824, // 0x00000338
        MetadataDefinition = 841, // 0x00000349
        SABO = 874, // 0x0000036A
        HMIBC = 878, // 0x0000036E
        LibraryObj = 893, // 0x0000037D
        LastType = 894, // 0x0000037E
        ConfiguredAlarm = 934, // 0x000003A6
        AlarmGroup = 935, // 0x000003A7
        AlarmConditionDefinition = 936, // 0x000003A8
        TypeMember = 1278, // 0x000004FE
        Rung = 32767, // 0x00007FFF
    }

    public enum ControllerOperationMode
    {
        OperationModeNull,
        OperationModeRun,
        OperationModeProgram,
        OperationModeFaulted,
        OperationModeDebug,
    }

    public enum ControllerKeySwitch
    {
        NullKeySwitch,
        RunKeySwitch,
        ProgramKeySwitch,
        RemoteKeySwitch,
    }

    public enum ControllerLockState
    {
        NullLockState,
        NotLocked,
        LockedByOther,
        LockedByMe,
    }

    public enum AccessType
    {
        CopyFrom,
        CreateSequence,
        DeleteSequence,
        ManualSequenceControl,
        EditSequenceProperty,
        CreateTag,
        DeleteTag,
        ModifyConstantProperty,
        ModifyConstantValue,
        ModifyTagProperties,
        ModifyTagValue,
        CreateRoutine,
        DeleteRoutine,
        ModifyRoutineProperties,
        ModifyRoutineLogic,
        CreateTask,
        DeleteTask,
        EditTaskProperties,
        EditProgramProperties,
        EditPhaseProperties,
        ModifyAlarmUse,
        AlarmDirectCommands,
        CreateAlarm,
        DeleteAlarm,
        EditAlarm,
        CreateAlarmDefinition,
        DeleteAlarmDefinition,
        EditAlarmDefinition,
        ModifyAlarmDefinitionRequired,
        ModifyAOI,
    }

    public enum ControllerFeature
    {
        AddIOOnline,
        AddDataTypeOnline,
        OnlineTransactions,
        EventTasks,
        PhaseManager,
        MotionTransform,
        DebugInstructions,
        DataLogging,
        HMIButtonControl,
        SystemOverheadTimeSlice,
        RedundancyPadPercentage,
        AlarmsDigital,
        AlarmsAnalog,
        Redundancy,
        LicenseSourceProtection,
        SameRPIMulticastConsumers,
        EquipmentSequence,
        SequenceEvents,
        PrecompileEncrypt,
        AddProduceTagOnline,
        AddConsumeTagOnline,
        ConfiguredAlarm,
        AlarmSet,
        AlarmConditionDefinition,
        StringLiteral,
        EthernetAndOrTimeSyncOptionalRestore,
        TrackComponentState,
        EnhancedKinematics,
        CompositeInstructionDevelopment,
        DriveSafetyInstructions,
        BaseEnergyObject,
        ElectricalEnergyObject,
        LocalScopeMessage,
    }

    //public enum ControllerTemporaryFeature
    //{
    //    IncludeConfigAlarmInTagBrowser,
    //}

    [Flags]
    public enum ComponentDataStatus : uint
    {
        ALIAS_FLAG = 1,
        MOTIONGROUP_TYPE_FLAG = 2,
        AXIS_TYPE_FLAG = 4,
        MESSAGE_TYPE_FLAG = 8,
        DATATABLE_TYPE_FLAG = 16, // 0x00000010
        IO_TAG_FLAG = 32, // 0x00000020
        CONSUMED_TAG = 64, // 0x00000040
        PHASE_BACKING_TAG_FLAG = 128, // 0x00000080
        STORAGE_USE_TAG = 256, // 0x00000100
        PublishToAC = 512, // 0x00000200
        PublishToPLC2 = 1024, // 0x00000400
        PublishToPLC5 = 2048, // 0x00000800
        CIRCULAR_ALIAS_FLAG = 4096, // 0x00001000
        TAG_FORCIBLE = 8192, // 0x00002000
        CACHING_PC_DATA = 16384, // 0x00004000
        COORDINATESYSTEM_TYPE_FLAG = 32768, // 0x00008000
        SAFETY_DATA_FLAG = 65536, // 0x00010000
        UDI_CLONE = 131072, // 0x00020000
        UDI_OFFSET_TAG = 262144, // 0x00040000
        HIDDEN_TAG_PREFIX = 524288, // 0x00080000
        ALMD_TYPE_FLAG = 1048576, // 0x00100000
        ALMA_TYPE_FLAG = 2097152, // 0x00200000
        HMIBC_TYPE_FLAG = 4194304, // 0x00400000
        INOUTPROGPARAM_FLAG = 8388608, // 0x00800000
        RxStatusFlag_Verified = 16777216, // 0x01000000
        RxStatusFlag_TypeLess = 33554432, // 0x02000000
        RxStatusFlag_ReadOnly = 67108864, // 0x04000000
        RxStatusFlag_Compiled = 134217728, // 0x08000000
        TYPEMEMBER_HIDDEN_FLAG = 2147483648, // 0x80000000
    }

    public enum OnlineEditType
    {
        Original,
        Pending,
        Test,
        TypeLast,
    }

}
