using System.ComponentModel;
using System.Runtime.Serialization;
using ICSStudio.Gui.Converters;

namespace ICSStudio.Cip
{
    /// <summary>
    /// vol1 Table 6-7.1 Device Profiles Sorted by Name
    /// </summary>
    [TypeConverter(typeof(EnumMemberValueConverter))]
    public enum CipDeviceType
    {
        Generic = 0x00,
        ACDrive = 0x02,
        MotorOverload = 0x03,
        LimitSwitch = 0x04,
        InductiveProximitySwitch = 0x05,
        PhotoelectricSensor = 0x06,

        [EnumMember(Value = "General Purpose Discrete I/O")]
        GeneralPurposeDiscreteIO = 0x07,
        Resolver = 0x09,

        [EnumMember(Value = "Communications Adapter")]
        CommunicationsAdapter = 0x0C,
        ProgrammableLogicController = 0x0E,
        PositionController = 0x10,
        DCDrive = 0x13,
        Contactor = 0x15,
        MotorStarter = 0x16,
        SoftstartStater = 0x17,
        HumanMachineInterface = 0x18,
        MassFlowController = 0x1A,
        PneumaticValue = 0x1B,
        VacuumPressureGauge = 0x1C,
        ProcessControlVale = 0x1D,
        ResidualGasAnalyzer = 0x1E,
        DCPowerGenerator = 0x1F,
        RFPowerGenerator = 0x20,
        TurboMolecularVacuumPump = 0x21,
        Encoder = 0x22,
        SafetyDiscreteIO = 0x23,
        FluidFlowController = 0x24,
        CIPMotionDrive = 0x25,
        CompoNetRepeater = 0x26,
        MassFlowControllerEnhanced = 0x27,
        CIPModbus = 0x28,
        CIPModbusTranslator = 0x29,
        SafetyAnalogIO = 0x2A,
        GenericKeyable = 0x2B,
        ManagedEthernetSwitch = 0x2C,
        CIPMotionSafetyDrive = 0x2D,
        SafetyDrive = 0x2E,
        CIPMotionEncoder = 0x2F,
        CIPMotionConverter = 0x30,
        CIPMotionIO = 0x31,
        ControlNetPhysicalLayerComponent = 0x32,
        CircuitBreaker = 0x33,
        Hart = 0x34,
        CIPHartTranslator = 0x35,

        //FIXME 
        GeneralPurposeAnalogIO = 0x73,
        EmbeddedComponent = 0xC8

    }

    public enum CipDeviceCode
    {
        ICDIR4 = 0x32,
        ICDIF4 = 0xD1,
        ICDOF4 = 0xD3,
        B1734OB8 = 0xE8,
        ICDOV16 = 0xE9,
        B1734OB4 = 0xE7,
        B1734IB8 = 0xD8,
        ICDIQ16 = 0xD9,
        B1734IB4 = 0x82,
        B1734IE4C = 0xD1,
        B1734OE4C = 0xD3,
        EmbeddedIO = 0x0474,
    }
}
