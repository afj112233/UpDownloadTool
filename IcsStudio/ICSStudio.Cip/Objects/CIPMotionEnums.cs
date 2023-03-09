using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using ICSStudio.Gui.Converters;

namespace ICSStudio.Cip.Objects
{
    public enum CipAxisServiceCode : byte
    {
        // vol9, page 228
        SetCyclicWriteList = 0x4E,
        SetCyclicReadList = 0x4F,
        GetLogs = 0x50,
        ClearLogs = 0x51
    }

    public enum MotionDirectCommand : uint
    {
        NullCommand,

        // Motion State
        MSO = 0x05,
        MSF = 0x06,
        MASD = 0x17,
        MASR = 0x18,
        MDO = 0x03,
        MDF = 0x04,
        MDS = 0x2A,
        MAFR = 0x16,

        // Motion Move
        MAS = 0x11,
        MAH = 0x07,
        MAJ = 0x0D,
        MAM = 0x10,
        MAG = 0x0F,
        MCD = 0x0E,
        MRP = 0x08,

        // Motion Event
        MAW = 0x09,
        MDW = 0x0A,
        MAR = 0x0B,
        MDR = 0x0C,

        // Motion Group
        MGS = 0x1A,
        MGSD = 0x1B,
        MGSR = 0x1C,
        MGSP = 0x19,

        // other
        AutoTune = 0x12,
        HookupTest = 0x14
    }

    public enum MotionGeneratorCommand : uint
    {
        NullCommand,

        // Motion State
        MSO = 0x05,
        MSF = 0x06,
        MAH = 0x07,
        MAJ = 0x0D,
        MAM = 0x10,
        MAS = 0x11,
        MDS = 0x2A,
        MAFR = 0x16,
    }

    public enum AxisAttributeId : ushort
    {
        #region Todo region

        AxisConfiguration = 30,
        FeedbackConfiguration = 31,
        ApplicationType = 201,
        LoopResponse = 202,

        AxisUpdateSchedule = 124,

        AxisID = 106,
        AxisState = 13,
        MotionFaultBits = 24,
        AxisStatusBits = 33,
        AxisFaultBits = 34,

        ModuleFaultBits = 163,
        AttributeErrorCode = 164,

        // ReSharper disable once InconsistentNaming
        AttributeErrorID = 165,

        CIPAxisState = 650,
        CIPAxisStatus = 651,
        CIPAxisIOStatus = 653,
        CIPAxisFaults = 657,
        CIPInitializationFaults = 674,
        CIPStartInhibits = 676,
        CIPAPRFaults = 756,
        CIPAxisFaultsRA = 903,
        CIPAPRFaultsRA = 905,
        CIPAxisIOStatusRA = 901,

        GuardFaults = 981,
        GuardStatus = 980,

        ConverterACInputPhasing = 890,
        ConverterACInputVoltage = 891,

        MotorElectricalAngle = 523,

        // Motor
        MotorDataSource = 1313,
        MotorCatalogNumber = 1310,
        MotorType = 1315,
        MotorUnit = 1316,
        MotorDeviceCode = 1314,
        MotorIntegralThermalSwitch = 1323,
        MotorMaxWindingTemperature = 1324,
        MotorWindingToAmbientCapacitance = 1325,
        MotorWindingToAmbientResistance = 1326,

        MotorTestResistance = 170,
        MotorTestInductance = 171,

        MotorRatedOutputPower = 1321,
        MotorRatedVoltage = 1318,
        MotorRatedContinuousCurrent = 1319,
        MotorRatedPeakCurrent = 1320,
        RotaryMotorRatedSpeed = 1331,
        RotaryMotorMaxSpeed = 1332,
        PMMotorRatedTorque = 1339,
        RotaryMotorPoles = 1329,
        MotorOverloadLimit = 1322,

        LinearMotorRatedSpeed = 1335,
        PMMotorRatedForce = 1342,
        LinearMotorPolePitch = 1334,
        LinearMotorMaxSpeed = 1337,

        InductionMotorRatedFrequency = 1345,


        // Model
        PMMotorTorqueConstant = 1340,
        PMMotorRotaryVoltageConstant = 1341,
        PMMotorResistance = 1327,
        PMMotorInductance = 1328,
        PMMotorFluxSaturation = 2310,

        PMMotorForceConstant = 1343,
        PMMotorLinearVoltageConstant = 1344,

        InductionMotorFluxCurrent = 1346,
        InductionMotorRatedSlipSpeed = 1352,
        InductionMotorStatorLeakageReactance = 1348,
        InductionMotorRotorLeakageReactance = 1351,
        InductionMotorStatorResistance = 1347,

        // Feedback1
        Feedback1Type = 1413,
        Feedback1Unit = 1411,
        Feedback1CycleInterpolation = 1417,
        Feedback1CycleResolution = 1416,
        Feedback1StartupMethod = 1415,
        Feedback1Turns = 1418, // Rev
        Feedback1Length = 1419, // Meter
        Feedback1AccelFilterBandwidth = 1435,
        Feedback1AccelFilterTaps = 2404,
        Feedback1VelocityFilterBandwidth = 1434,
        Feedback1VelocityFilterTaps = 2403,
        Feedback1Polarity = 1414,

        // Feedback2
        FeedbackUnitRatio = 44,
        Feedback2Type = 1463,
        Feedback2Unit = 1461,
        Feedback2CycleInterpolation = 1467,
        Feedback2CycleResolution = 1466,
        Feedback2StartupMethod = 1465,
        Feedback2Turns = 1468, // Rev
        Feedback2Length = 1469, // Meter
        Feedback2AccelFilterBandwidth = 1485,
        Feedback2AccelFilterTaps = 2454,
        Feedback2VelocityFilterBandwidth = 1484,
        Feedback2VelocityFilterTaps = 2453,
        Feedback2Polarity = 1464,

        FeedbackCommutationAligned = 250,
        CommutationOffset = 561,
        CommutationPolarity = 563,

        // Hookup Tests
        HookupTestDistance = 109,
        HookupTestTime = 110,
        HookupTestFeedbackChannel = 111,


        // Scaling
        LoadType = 1370,
        TransmissionRatioInput = 1371,
        TransmissionRatioOutput = 1372,
        ActuatorType = 1373,
        ActuatorLead = 1374,
        ActuatorLeadUnit = 1375,
        ActuatorDiameter = 1376,
        ActuatorDiameterUnit = 1377,
        PositionUnits = 80,
        PositionScalingNumerator = 72,
        PositionScalingDenominator = 73,
        MotionUnit = 77,
        TravelMode = 71,
        TravelRange = 76,
        PositionUnwind = 84,
        PositionUnwindNumerator = 74,
        PositionUnwindDenominator = 75,

        SoftTravelLimitChecking = 92,
        SoftTravelLimitPositive = 93,
        SoftTravelLimitNegative = 94,

        ScalingSource = 70,

        // Polarity
        MotionPolarity = 79,
        MotorPolarity = 1317,

        // Autotune
        LoadCoupling = 203,
        GainTuningConfigurationBits = 189,
        TuningSelect = 190,
        TuningTravelLimit = 193,
        TuningSpeed = 194,
        TuningTorque = 195,
        TuningDirection = 191,

        TuneFriction = 187,
        TuneInertiaMass = 186,
        TuneLoadOffset = 188,

        TuneAcceleration = 181,
        TuneAccelerationTime = 179,
        TuneDeceleration = 182,
        TuneDecelerationTime = 180,

        // Load
        LoadRatio = 205,
        RotaryMotorInertia = 1330,
        LinearMotorMass = 1336,
        TotalInertia = 206,
        TotalMass = 207,
        SystemInertia = 496,
        TorqueOffset = 232,
        LoadInertiaRatio = 352,

        // Backlash
        BacklashReversalOffset = 423,
        BacklashCompensationWindow = 825,

        // Compliance
        TorqueLowPassFilterBandwidth = 502,
        TorqueNotchFilterFrequency = 503,
        TorqueLeadLagFilterGain = 828,
        TorqueLeadLagFilterBandwidth = 827,

        AdaptiveTuningConfiguration = 836,
        TorqueNotchFilterHighFrequencyLimit = 837,
        TorqueNotchFilterLowFrequencyLimit = 838,
        TorqueNotchFilterTuningThreshold = 839,

        TorqueNotchFilterFrequencyEstimate = 841,
        TorqueNotchFilterMagnitudeEstimate = 842,
        TorqueLowPassFilterBandwidthEstimate = 843,
        AdaptiveTuningGainScalingFactor = 844,

        // Friction
        FrictionCompensationSliding = 498,
        FrictionCompensationStatic = 499,
        FrictionCompensationViscous = 500,
        FrictionCompensationWindow = 826, //826 or 421

        // Observer
        LoadObserverConfiguration = 805,
        LoadObserverBandwidth = 806,
        LoadObserverIntegratorBandwidth = 807,

        // Position Loop
        PositionLoopBandwidth = 441,
        PositionIntegratorBandwidth = 442,
        PositionIntegratorControl = 446,
        VelocityFeedforwardGain = 440,
        PositionErrorTolerance = 444,
        PositionLockTolerance = 443,
        PositionErrorToleranceTime = 445,

        PositionLeadLagFilterBandwidth = 781,
        PositionLeadLagFilterGain = 782,

        PositionTrim = 431,

        PositionFeedback1 = 1402,
        PositionFeedback2 = 1452,

        // Velocity Loop
        VelocityLoopBandwidth = 461,
        VelocityIntegratorBandwidth = 462,
        VelocityIntegratorControl = 467,
        AccelerationFeedforwardGain = 460,
        VelocityErrorTolerance = 465,
        VelocityLockTolerance = 471,
        VelocityLimitPositive = 473, // 473 or 325
        VelocityLimitNegative = 474,

        VelocityLimitSource = 458,

        // Acceleration Loop
        AccelerationLimit = 485,
        DecelerationLimit = 486,

        // Torque Current Loop
        TorqueLoopBandwidth = 554,
        TorqueLimitPositive = 504,
        TorqueLimitNegative = 505,

        TorqueTrim = 491,

        // Planner
        MaximumSpeed = 114,
        MaximumAcceleration = 115,
        MaximumDeceleration = 116,
        MaximumAccelerationJerk = 118,
        MaximumDecelerationJerk = 119,
        CommandUpdateDelayOffset = 360,

        // Homing
        HomeMode = 85,
        HomePosition = 89,
        HomeOffset = 90,
        HomeSequence = 87,
        HomeConfigurationBits = 88,
        HomeDirection = 86,
        HomeSpeed = 112,
        HomeReturnSpeed = 113,

        // Actions
        StoppingAction = 610,
        MotorOverloadAction = 646,
        InverterOverloadAction = 647,
        InverterCapacity = 636,
        MechanicalBrakeControl = 614,
        MechanicalBrakeEngageDelay = 616,
        MechanicalBrakeReleaseDelay = 615,

        MotionExceptionAction = 29,
        CIPAxisExceptionAction = 672,
        CIPAxisExceptionActionMfg = 673,
        CIPAxisExceptionActionRA = 908,

        BrakeSlipTolerance = 594,
        BrakeTestTorque = 592,
        CoastingTimeLimit = 617,
        ProvingConfiguration = 590,
        TorqueProveCurrent = 591,
        ZeroSpeed = 608,
        ZeroSpeedTime = 609,

        // 
        ConverterOutputCurrent = 605,
        ConverterOutputPower = 606,

        // Virtual-Motion Planner
        OutputCamExecutionTargets = 14,
        ProgrammedStopMode = 117,
        MasterInputConfigurationBits = 21,
        MasterPositionFilterBandwidth = 22,

        // Virtual-Units
        AverageVelocityTimebase = 81,

        // Virtual-conversion
        ConversionConstant = 82,


        // Other
        DynamicsConfigurationBits = 120,
        InterpolatedPositionConfiguration = 108,
        BusConfiguration = 622,
        BusRegulatorThermalOverloadUL = 703,
        BusUndervoltageUserLimit = 705,
        CommutationAlignment = 564,

        ControlMethod = 41,
        ControlMode = 40,

        ConverterThermalOverloadUserLimit = 701,
        ConverterCapacity = 637,
        CurrentError = 527,

        InverterThermalOverloadUserLimit = 699,
        DampingFactor = 196,
        DriveModelTimeConstant = 200,

        // ReSharper disable once InconsistentNaming
        ExternalShuntRegulatorID = 882,
        FeedbackDataLossUserLimit = 708,

        FeedbackMode = 42,
        FluxCurrentReference = 525,

        LoadObserverFeedbackGain = 809,

        MotionResolution = 78,
        MotionScalingConfiguration = 45,
        MotorOverspeedUserLimit = 695,
        MotorThermalOverloadUserLimit = 697,

        CurrentVectorLimit = 553,
        OvertorqueLimit = 508,
        OvertorqueLimitTime = 509,

        PositionServoBandwidth = 197,

        RegistrationInputs = 356,

        StoppingTimeLimit = 612,
        StoppingTorque = 611,

        SystemBandwidth = 169,
        SystemDamping = 204,
        TorqueRateLimit = 506,
        TorqueThreshold = 507,
        UndertorqueLimit = 510,
        UndertorqueLimitTime = 511,
        VelocityErrorToleranceTime = 466,
        VelocityServoBandwidth = 198,
        VelocityStandstillWindow = 472,
        VelocityThreshold = 470,
        VelocityTrim = 451,
        VelocityDroop = 464,
        VelocityLowPassFilterBandwidth = 469,
        VelocityNegativeFeedforwardGain = 790,
        VelocityOffset = 231,

        // ReSharper disable InconsistentNaming
        SLATConfiguration = 833,
        SLATSetPoint = 834,
        SLATTimeDelay = 835,
        // ReSharper restore InconsistentNaming

        ShuntRegulatorResistorType = 881,

        // TODO: need edit
        BusRegulatorAction = 624,

        ActualPosition = 48,
        CommandPosition = 96,
        ActualVelocity = 52,
        CommandVelocity = 99,

        #endregion

        #region Command Reference Generation Attributes, rm003 page 154

        PositionFineCommand = 365,
        VelocityFineCommand = 366,
        AccelerationFineCommand = 367,

        #endregion

        #region Control Mode Attributes, rm003 page 164

        PositionReference = 432,
        VelocityFeedforwardCommand = 433,
        PositionError = 436,
        PositionIntegratorOutput = 437,
        PositionLoopOutput = 438,

        #endregion

        #region Velocity Loop Attributes, rm003 page 173

        AccelerationFeedforwardCommand = 452,
        VelocityReference = 453,
        VelocityFeedback = 454,
        VelocityError = 455,
        VelocityIntegratorOutput = 456,
        VelocityLoopOutput = 457,

        #endregion

        #region Acceleration Control Attributes, rm003 page 184

        AccelerationReference = 482,
        AccelerationFeedback = 483, // Acceleration Feedback 1
        LoadObserverAccelerationEstimate = 801,
        LoadObserverTorqueEstimate = 802,

        #endregion

        #region Torque/Force Control Signal Attributes, rm003 page 189

        TorqueReference = 492,
        TorqueReferenceFiltered = 493,
        TorqueReferenceLimited = 494,

        #endregion

        #region Current Control Attributes, rm003 page 203

        CurrentCommand = 520,
        OperativeCurrentLimit = 521,
        CurrentLimitSource = 522,
        CurrentReference = 524,
        CurrentFeedback = 529,
        FluxCurrentError = 528,
        FluxCurrentFeedback = 530,

        #endregion

        #region Drive Output Attributes, rm003 page 215

        OutputFrequency = 600,
        OutputCurrent = 601,
        OutputVoltage = 602,
        OutputPower = 603,

        #endregion

        #region DC Bus Control Attributes, rm003 page 228

        DCBusVoltage = 620,

        #endregion

        #region Power and Thermal Management Status Attributes, rm003 page 230

        MotorCapacity = 635, // vol9 page 70
        BusRegulatorCapacity = 638,

        #endregion

        #region Frequency Control Configuration Attributes

        FrequencyControlMethod = 570,
        MaximumVoltage = 572,
        MaximumFrequency = 573,
        BreakVoltage = 575,
        BreakFrequency = 576,
        StartBoost = 577,
        RunBoost = 578,

        FluxUpControl = 558,
        FluxUpTime = 559,
        SkipSpeed1 = 370,
        SkipSpeed2 = 371,
        SkipSpeed3 = 372,
        SkipSpeedBand = 373,
        #endregion
    }

    public enum MotionPolarityType : byte
    {
        [EnumMember(Value = "Normal")] Normal = 0,
    }

    public enum MotionUnitType : byte
    {
        [EnumMember(Value = "Motor Rev")] MotorRev = 0,
        [EnumMember(Value = "Load Rev")] LoadRev = 1,
        [EnumMember(Value = "Feedback Rev")] FeedbackRev = 2,

        [EnumMember(Value = "Motor Millimeter")]
        MotorMm = 3,

        [EnumMember(Value = "Load Millimeter")]
        LoadMm = 4,

        [EnumMember(Value = "Feedback Millimeter")]
        FeedbackMm = 5,
        [EnumMember(Value = "Motor Inch")] MotorInch = 6,
        [EnumMember(Value = "Load Inch")] LoadInch = 7,
        [EnumMember(Value = "Feedback Inch")] FeedbackInch = 8,
        [EnumMember(Value = "Motor Rev/s")] MotorRevPerS = 9,
        [EnumMember(Value = "Load Rev/s")] LoadRevPerS = 10,
        [EnumMember(Value = "Motor m/s")] MotorMPerS = 11,
        [EnumMember(Value = "Load m/s")] LoadMPerS = 12,
        [EnumMember(Value = "Motor Inch/s")] MotorInchPerS = 13,

        [EnumMember(Value = "Load Inch/s")] LoadInchPerS = 14
        //15-255 = Reserved
    }

    public enum ActuatorDiameterUnitType : byte
    {
        [EnumMember(Value = "Millimeter")] Mm = 0,

        [EnumMember(Value = "Inch")] Inch = 1
        // 2-255 = Reserved
    }

    public enum ActuatorLeadUnitType : byte
    {
        [EnumMember(Value = "Millimeter/Rev")] MmPerRev = 0,

        [EnumMember(Value = "Inch/Rev")] InchPerRev = 1
        // 2-255 = Reserved
    }

    public enum ActuatorType : byte
    {
        [EnumMember(Value = "<none>")] None = 0, // R
        [EnumMember(Value = "Screw")] Screw = 1, // O

        [EnumMember(Value = "Belt and Pulley")]
        BeltAndPulley = 2,

        [EnumMember(Value = "Chain and Sprocket")]
        ChainAndSprocket = 3,

        [EnumMember(Value = "Rack and Pinion")]
        RackAndPinion = 4

        //5-255 = Reserved
    }

    public enum AdaptiveTuningConfigurationType : byte
    {
        //0 = Disabled
        //1 = Tracking Notch
        //2 = Gain Stabilization
        //3 = Tracking Notch and Gain Stabilization
        //4…255 = Reserved
        [EnumMember(Value = "Disabled")] Disabled = 0,

        [EnumMember(Value = "Tracking Notch")] TrackingNotch = 1,

        [EnumMember(Value = "Gain Stabilization")]
        GainStabilization = 2,

        [EnumMember(Value = "Tracking Notch and Gain Stabilization")]
        TrackingNotchAndGainStabilization = 3,
    }

    public enum ApplicationType : byte
    {
        [EnumMember(Value = "Custom")] Custom = 0,
        [EnumMember(Value = "Basic")] Basic = 1,
        [EnumMember(Value = "Tracking")] Tracking = 2,
        [EnumMember(Value = "Point-to-Point")] PointToPoint = 3,
        [EnumMember(Value = "Constant Speed")] ConstantSpeed = 4
    }

    public enum AxisConfigurationType : byte
    {
        [EnumMember(Value = "Feedback Only")] FeedbackOnly = 0,

        [EnumMember(Value = "Frequency Control")]
        FrequencyControl = 1,
        [EnumMember(Value = "Position Loop")] PositionLoop = 2,
        [EnumMember(Value = "Velocity Loop")] VelocityLoop = 3,
        [EnumMember(Value = "Torque Loop")] TorqueLoop = 4,
        [EnumMember(Value = "Converter Only")] ConverterOnly = 5
    }

    public enum ControlMethodType : byte
    {
        // rm003, page 71
        //0 = No Control
        //1 = Frequency Control
        //2 = PI Vector Control
        //3…255 = Reserved
        NoControl = 0,
        FrequencyControl = 1,

        // ReSharper disable once InconsistentNaming
        PIVectorControl = 2,
    }

    public enum ControlModeType : byte
    {
        // rm003, page 70
        //0 = No Control
        //1 = Position Control
        //2 = Velocity Control
        //3 = Acceleration Control
        //4 = Torque Control
        //5…15 = Reserved
        //Bits 4…7 = Reserved
        NoControl = 0,
        PositionControl = 1,
        VelocityControl = 2,
        AccelerationControl = 3,
        TorqueControl = 4,
    }

    public enum FeedbackStartupMethodType : byte
    {
        [EnumMember(Value = "Incremental")] Incremental = 0,

        [EnumMember(Value = "Absolute")] Absolute = 1
        // 2-255 = Reserved
    }

    public enum FeedbackType : byte
    {
        [EnumMember(Value = "Not Specified")] NotSpecified = 0,
        [EnumMember(Value = "Digital AqB")] DigitalAqB = 1,

        [EnumMember(Value = "Digital AqB with UVW")]
        DigitalAqBWithUvw = 2,

        [EnumMember(Value = "Digital Parallel")]
        DigitalParallel = 3,
        [EnumMember(Value = "Sine/Cosine")] SineCosine = 4,

        [EnumMember(Value = "Sine/Cosine with UVW")]
        SineCosineWithUvw = 5,
        [EnumMember(Value = "Hiperface")] Hiperface = 6,

        [EnumMember(Value = "EnDat Sine/Cosine")]
        EnDatSineCosine = 7,
        [EnumMember(Value = "EnDat Digital")] EnDatDigital = 8,
        [EnumMember(Value = "Resolver")] Resolver = 9,
        [EnumMember(Value = "SSI Digital")] SsiDigital = 10,
        [EnumMember(Value = "LDT")] Ldt = 11,
        [EnumMember(Value = "Hiperface DSL")] HiperfaceDsl = 12,
        [EnumMember(Value = "BiSS Digital")] BissDigital = 13,
        [EnumMember(Value = "Integrated")] Integrated = 14,

        [EnumMember(Value = "SSI Sine/Cosine")]
        SsiSineCosine = 15,
        [EnumMember(Value = "SSI AqB")] SsiAqB = 16,

        [EnumMember(Value = "BiSS Sine/Cosine")]
        BissSineCosine = 17,

        //18-127 = Reserved
        //128-255 = Vendor Specific
        [EnumMember(Value = "Tamagawa Serial")]
        TamagawaSerial = 128,
        [EnumMember(Value = "Stahl SSI")] StahlSsi = 129
    }

    public enum FeedbackUnitType : byte
    {
        [EnumMember(Value = "Rev")] Rev = 0,

        [EnumMember(Value = "Meter")] Meter = 1
        //2-127 = Reserved
        //128-255 = Vendor
    }

    public enum FeedbackCommutationAlignedType : byte
    {
        //0 = Not Aligned (R)
        //1 = Controller Offset (R)
        //2 = Motor Offset (O)
        //3 = Self-Sense (O)
        //4 = Database Offset (O)
        //5…255 = Reserved
        [EnumMember(Value = "Not Aligned")] NotAligned = 0,

        [EnumMember(Value = "Controller Offset")]
        ControllerOffset = 1,
        [EnumMember(Value = "Motor Offset")] MotorOffset = 2,
        [EnumMember(Value = "Self-Sense")] SelfSense = 3,

        [EnumMember(Value = "Database Offset")]
        DatabaseOffset = 4
    }

    public enum FeedbackConfigurationType : byte
    {
        [EnumMember(Value = "No Feedback")] NoFeedback = 0,

        [EnumMember(Value = "Master Feedback")]
        MasterFeedback = 1,

        [EnumMember(Value = "Motor Feedback")] MotorFeedback = 2,
        [EnumMember(Value = "Load Feedback")] LoadFeedback = 3,
        [EnumMember(Value = "Dual Feedback")] DualFeedback = 4,

        [EnumMember(Value = "Dual Int Feedback")]
        DualIntFeedback = 8
    }



    public enum HomeDirectionType : byte
    {
        [EnumMember(Value = "Forward Uni-directional")]
        UnidirectionalForward = 0,

        [EnumMember(Value = "Forward Bi-directional")]
        BidirectionalForward = 1,

        [EnumMember(Value = "Reverse Uni-directional")]
        UnidirectionalReverse = 2,

        [EnumMember(Value = "Reverse Bi-directional")]
        BidirectionalReverse = 3

        //0 = Unidirectional Forward
        //1 = Bidirectional Forward
        //2 = Unidirectional Reverse
        //3 = Bidirectional Reverse
        //4…255 = Reserved
    }

    public enum HomeModeType : byte
    {
        [EnumMember(Value = "Passive")] Passive = 0,
        [EnumMember(Value = "Active")] Active = 1
    }

    public enum HomeSequenceType : byte
    {
        [EnumMember(Value = "Immediate")] Immediate = 0,
        [EnumMember(Value = "Switch")] HomeToSwitch = 1,
        [EnumMember(Value = "Marker")] HomeToMarker = 2,
        [EnumMember(Value = "Switch-Marker")] HomeToSwitchThenMarker = 3,
        [EnumMember(Value = "Torque")] HomeToTorque = 4,
        [EnumMember(Value = "Torque-Marker")] HomeToTorqueThenMarker = 5

        //0 = Immediate (default)
        //1 = Home to Switch (O)
        //2 = Home to Marker (O)
        //3 = Home to Switch then Marker (O)
        //4 = Home to Torque (O)
        //5 = Home to Torque then Marker (O)
        //6…255 = Reserved
    }

    public enum HomeLimitSwitchType : byte
    {
        [EnumMember(Value = "Normally Open")] NormallyOpen = 0,

        [EnumMember(Value = "Normally Closed")]
        NormallyClosed = 1,
    }

    [Flags]
    public enum IntegratorControlBitmap
    {
        IntegratorHoldEnable = 1 << 0,
        AutoPreset = 1 << 1
    }

    public enum InterpolatedPositionConfigurationType : byte
    {
        FirstOrder = 0,
        SecondOrder = 1,
    }

    public enum InverterOverloadActionType : byte
    {
        [EnumMember(Value = "<none>")] None = 0,

        [EnumMember(Value = "Current Foldback")]
        CurrentFoldback = 1,

        [EnumMember(Value = "Reduce PWM Rate")]
        ReducePWMRate = 128,

        [EnumMember(Value = "PWM - Foldback")] PWMFoldback = 129

        //0 = None (R)
        //1 = Current Foldback (O)
        //2…127 = Reserved
        //128…255 = Vendor Specific
        //128 = Reduce PWM Rate
        //129 = PWM - Foldback
    }

    public enum LoadObserverConfigurationType : byte
    {
        //0 = Disabled (R)
        //1 = Load Observer Only (O)
        //2 = Load Observer with Velocity Estimate (O)
        //3 = Velocity Estimate Only (O)
        //4 = Acceleration Feedback (O)
        //5…255 = Reserved

        Disabled = 0,

        [EnumMember(Value = "Load Observer Only")]
        LoadObserverOnly = 1,

        [EnumMember(Value = "Load Observer With Velocity Estimate")]
        LoadObserverWithVelocityEstimate = 2,

        [EnumMember(Value = "Velocity Estimate Only")]
        VelocityEstimateOnly = 3,

        [EnumMember(Value = "Acceleration Feedback")]
        AccelerationFeedback = 4
    }

    public enum LoadType : byte
    {
        [EnumMember(Value = "Direct Coupled Rotary")]
        DirectRotary = 0,

        [EnumMember(Value = "Direct Coupled Linear")]
        DirectLinear = 1,

        [EnumMember(Value = "Rotary Transmission")]
        RotaryTransmission = 2,

        [EnumMember(Value = "Linear Actuator")]
        LinearActuator = 3

        // 4-255 = Reserved
    }

    public enum LoopResponseType : byte
    {
        [EnumMember(Value = "Low")] Low = 0,
        [EnumMember(Value = "Medium")] Medium = 1,
        [EnumMember(Value = "High")] High = 2
    }

    public enum MechanicalBrakeControlType : byte
    {
        //Enumeration
        //0 = Automatic
        //1 = Brake Release
        //2…225 = Reserved
        [EnumMember(Value = "Automatic")] Automatic = 0,
        [EnumMember(Value = "Brake Released")] BrakeRelease = 1,
    }

    public enum PolarityType : byte
    {
        //0 = Normal Polarity
        //1 = Inverted Polarity
        //2…255 = Reserved
        [EnumMember(Value = "Normal")] Normal = 0,
        [EnumMember(Value = "Inverted")] Inverted = 1
    }

    public enum MotorType : byte
    {
        //0 = Not Specified (R)
        //1 = Rotary Permanent Magnet (O)
        //2 = Rotary Induction (O)
        //3 = Linear Permanent Magnet (O)
        //4 = Linear Induction (O)
        //5 = Rotary Interior Permanent Magnet (O)
        //6…127 = Reserved
        //128…255 = Vendor Specific
        [EnumMember(Value = "Not Specified")] NotSpecified = 0,

        [EnumMember(Value = "Rotary Permanent Magnet")]
        RotaryPermanentMagnet = 1,

        [EnumMember(Value = "Rotary Induction")]
        RotaryInduction = 2,

        [EnumMember(Value = "Linear Permanent Magnet")]
        LinearPermanentMagnet = 3,

        [EnumMember(Value = "Linear Induction")]
        LinearInduction = 4,

        [EnumMember(Value = "Rotary Interior Permanent Magnet")]
        RotaryInteriorPermanentMagnet = 5
    }

    public enum MotorUnitType : byte
    {
        [EnumMember(Value = "Rev")] Rev = 0,

        [EnumMember(Value = "Meter")] Meter = 1
        //2-127 = Reserved
        //128-255 = Vendor
    }

    public enum AxisUpdateScheduleType : byte
    {
        //0 = Base
        //1 = Alternate 1
        //2 = Alternate 2
        //3…255 = Reserved
        [EnumMember(Value = "Base")] Base = 0,
        [EnumMember(Value = "Alternate 1")] Alternate1 = 1,
        [EnumMember(Value = "Alternate 2")] Alternate2 = 2
    }

    public enum MotionScalingConfigurationType : byte
    {
        //Enumeration
        //0 = Control Scaling (R)
        //1 = Drive Scaling (O)
        //2…255 = Reserved
        [EnumMember(Value = "Control Scaling")]
        ControlScaling = 0,
        [EnumMember(Value = "Drive Scaling")] DriveScaling = 1
    }

    public enum MotorDataSourceType : byte
    {
        //0 = Datasheet (R)
        //1 = Database (O)
        //2 = Drive NV (O)
        //3 = Motor NV (O)
        //4…127 = Reserved
        //128…255 = Vendor specific
        [EnumMember(Value = "Nameplate Datasheet")]
        Datasheet = 0,

        [EnumMember(Value = "Catalog Number")] Database = 1,
        [EnumMember(Value = "Drive NV")] DriveNV = 2,
        [EnumMember(Value = "Motor NV")] MotorNV = 3
    }

    public enum MotorOverloadActionType : byte
    {
        [EnumMember(Value = "<none>")] None = 0,

        [EnumMember(Value = "Current Foldback")]
        CurrentFoldback = 1

        //0 = None (R)
        //1 = Current Foldback (O)
        //2…127 = Reserved
        //128…255 = Vendor specific
    }

    public enum ProgrammedStopModeType : byte
    {
        //0 = Fast Stop (default)
        //1 = Fast Disable
        //2 = Hard Disable
        //3 = Fast Shutdown
        //4 = Hard Shutdown
        [EnumMember(Value = "Fast Stop")] FastStop = 0,
        [EnumMember(Value = "Fast Disable")] FastDisable = 1,
        [EnumMember(Value = "Hard Disable")] HardDisable = 2,
        [EnumMember(Value = "Fast Shutdown")] FastShutdown = 3,
        [EnumMember(Value = "Hard Shutdown")] HardShutdown = 4
    }

    public enum ScalingSourceType : byte
    {
        //Enumeration
        //0 = From Calculator
        //1 = Direct Scaling Factor Entry
        //2…255 = Reserved
        [EnumMember(Value = "From Calculator")]
        FromCalculator = 0,

        [EnumMember(Value = "Direct Scaling Factor Entry")]
        DirectScalingFactorEntry = 1,

    }

    public enum StoppingActionType : byte
    {
        [EnumMember(Value = "Disable & Coast")]
        DisableCoast = 0,

        [EnumMember(Value = "Current Decel & Disable")]
        CurrentDecelDisable = 1,

        [EnumMember(Value = "Ramped Decel & Disable")]
        RampedDecelDisable = 2,

        [EnumMember(Value = "Current Decel & Hold")]
        CurrentDecelHold = 3,

        [EnumMember(Value = "Ramped Decel & Hold")]
        RampedDecelHold = 4,

        [EnumMember(Value = "DC Injection Brake")]
        DCInjectionBrake = 128,

        [EnumMember(Value = "AC Injection Brake")]
        ACInjectionBrake = 129

        //0 = Disabled and Coast
        //1 = Current Decel Disable
        //2 = Ramped Decel Disable
        //3 = Current Decel Hold
        //4 = Ramped Decel Hold
        //5…127 = Reserved
        //128…255 = Vendor Specific
        //128 = DC Injection Brake
        //129 = AC Injection Brake
    }

    public enum TravelModeType : byte
    {
        [EnumMember(Value = "Unlimited")] Unlimited = 0,
        [EnumMember(Value = "Limited")] Limited = 1,

        [EnumMember(Value = "Cyclic")] Cyclic = 2
        //3…255 = Reserved
    }



    public enum HookupTestFeedbackChannelType : byte
    {
        [EnumMember(Value = "Feedback 1")] Feedback1 = 1,
        [EnumMember(Value = "Feedback 2")] Feedback2 = 2
    }

    public enum ExceptionActionType : byte
    {
        //0 = Ignore (O)
        //1 = Alarm (O)
        //2 = Fault Status Only (O)
        //3 = Stop Planner (O)
        //4 = Disable (R)
        //5 = Shutdown (R)
        [EnumMember(Value = "Ignore")] Ignore = 0,
        [EnumMember(Value = "Alarm")] Alarm = 1,

        [EnumMember(Value = "Fault Status Only")]
        FaultStatusOnly = 2,
        [EnumMember(Value = "Stop Planner")] StopPlanner = 3,
        [EnumMember(Value = "Stop Drive")] StopDrive = 4, //v30
        [EnumMember(Value = "Disable")] Disable = 4,
        [EnumMember(Value = "Shutdown")] Shutdown = 5,

        [EnumMember(Value = "Unsupported")] Unsupported = 255,
    }

    [Flags]
    public enum MasterInputConfigurationBitmap
    {
        [EnumMember(Value = "Master Delay Compensation")]
        MasterDelayCompensation = 1 << 0,

        [EnumMember(Value = "Master Position Filter")]
        MasterPositionFilter = 1 << 1,
    }

    [Flags]
    public enum DynamicsConfigurationBitmap
    {
        //Bitmap
        //0 = Reduce S-Curve Stop Delay
        //1 = Prevent S-Curve Velocity Reversals
        //2 = Reduced Extreme Velocity Overshoot
        //3…31 = Reserved
        [EnumMember(Value = "Reduce S-Curve Stop Delay")]
        ReduceSCurveStopDelay = 1 << 0,

        [EnumMember(Value = "Prevent S-Curve Velocity Reversals")]
        PreventSCurveVelocityReversals = 1 << 1,

        [EnumMember(Value = "Reduced Extreme Velocity Overshoot")]
        ReducedExtremeVelocityOvershoot = 1 << 2,
    }

    public enum SLATConfigurationType : byte
    {
        //0 = SLAT Disabled
        //1 = SLAT Min Speed/Torque
        //2 = SLAT Max Speed/Torque
        [EnumMember(Value = "SLAT Disabled")] SLATDisabled = 0,

        [EnumMember(Value = "SLAT Min Speed/Torque")]
        SLATMinSpeedTorque,

        [EnumMember(Value = "SLAT Max Speed/Torque")]
        SLATMaxSpeedTorque

    }

    #region Autotune

    public enum LoadCouplingType : byte
    {
        //0 = Rigid
        //1 = Compliant
        //2…255 = Reserved
        [EnumMember(Value = "Rigid")] Rigid = 0,
        [EnumMember(Value = "Compliant")] Compliant = 1
    }

    [Flags]
    public enum GainTuningConfigurationType
    {
        //0 = Run Inertia Test
        //1 = Use Load Ratio
        //2 = Reserved
        //3 = Reserved
        //4 = Tune Pos Integrator
        //5 = Tune Vel Integrator
        //6 = Tune Vel Feedforward
        //7 = Tune Accel Feedforward
        //8 = Tune Torque Low Pass Filter
        //9…15 = Reserved
        RunInertiaTest = 1 << 0,
        UseLoadRatio = 1 << 1,
        TunePosIntegrator = 1 << 4,
        TuneVelIntegrator = 1 << 5,
        TuneVelFeedforward = 1 << 6,
        TuneAccelFeedforward = 1 << 7,
        TuneTorqueLowPassFilter = 1 << 8
    }

    public enum TuningSelectType : byte
    {
        //0 = Total Inertia
        //1 = Motor Inertia
        //2…255 = Reserved
        // TODO(gjc): need check
        [EnumMember(Value = "Motor with Load")]
        TotalInertia = 0,

        [EnumMember(Value = "Uncoupled Motor")]
        MotorInertia = 1
    }

    public enum TuningDirectionType : byte
    {
        //0 = Unidirectional Forward
        //1 = Unidirectional Reverse
        //2 = Bi-Directional Forward
        //3 = Bi-Directional Reverse
        //4…255 = Reserved
        [EnumMember(Value = "Forward Uni-directional")]
        UnidirectionalForward = 0,

        [EnumMember(Value = "Reverse Uni-directional")]
        UnidirectionalReverse = 1,

        [EnumMember(Value = "Forward Bi-directional")]
        BiDirectionalForward = 2,

        [EnumMember(Value = "Reverse Bi-directional")]
        BiDirectionalReverse = 3
    }

    public enum TuneStatusType : ushort
    {
        //0 = Tune Successful
        //1 = Tune in Progress
        //2 = Tune Aborted by User
        //3 = Tune Time-out Fault
        //4 = Tune Failed - Servo Fault
        //5 = Axis Reached Tuning Travel Limit
        //6 = Axis Polarity Set Incorrectly
        //7 = Tune Measurement Error
        //8 = Tune Configuration Error
        [EnumMember(Value = "Tune Successful")]
        TuneSuccessful = 0,

        [EnumMember(Value = "Tune in Progress")]
        TuneInProgress = 1,

        [EnumMember(Value = "Tune Aborted by User")]
        TuneAbortedByUser = 2,

        [EnumMember(Value = "Tune Time-out Fault")]
        TuneTimeoutFault = 3,

        [EnumMember(Value = "Tune Failed - Servo Fault")]
        TuneFailedServoFault = 4,

        [EnumMember(Value = "Axis Reached Tuning Travel Limit")]
        AxisReachedTuningTravelLimit = 5,

        [EnumMember(Value = "Axis Polarity Set Incorrectly")]
        AxisPolaritySetIncorrectly = 6,

        [EnumMember(Value = "Tune Measurement Error")]
        TuneMeasurementError = 7,

        [EnumMember(Value = "Tune Configuration Error")]
        TuneConfigurationError = 8

    }

    #endregion

    #region Exception

    public enum CIPStandardException : byte
    {
        [EnumMember(Value = "Motor Overcurrent")]
        MotorOvercurrent = 1,

        [EnumMember(Value = "Motor Commutation")]
        MotorCommutation = 2,

        [EnumMember(Value = "Motor Overspeed Factory Limit")]
        MotorOverspeedFL = 3,

        [EnumMember(Value = "Motor Overspeed User Limit")]
        MotorOverspeedUL = 4,

        [EnumMember(Value = "Motor Overtemperature Factory Limit")]
        MotorOvertemperatureFL = 5,

        [EnumMember(Value = "Motor Overtemperature User Limit")]
        MotorOvertemperatureUL = 6,

        [EnumMember(Value = "Motor Thermal Overload Factory Limit")]
        MotorThermalOverloadFL = 7,

        [EnumMember(Value = "Motor Thermal Overload User Limit")]
        MotorThermalOverloadUL = 8,

        [EnumMember(Value = "Motor Phase Loss")]
        MotorPhaseLoss = 9,

        [EnumMember(Value = "Inverter Overcurrent")]
        InverterOvercurrent = 10,

        [EnumMember(Value = "Inverter Overtemperature Factory Limit")]
        InverterOvertemperatureFL = 11,

        [EnumMember(Value = "Inverter Overtemperature User Limit")]
        InverterOvertemperatureUL = 12,

        [EnumMember(Value = "Inverter Thermal Overload Factory Limit")]
        InverterThermalOverloadFL = 13,

        [EnumMember(Value = "Inverter Thermal Overload User Limit")]
        InverterThermalOverloadUL = 14,

        [EnumMember(Value = "Converter Overcurrent")]
        ConverterOvercurrent = 15,

        [EnumMember(Value = "Converter Ground Current Factory Limit")]
        ConverterGroundCurrentFL = 16,

        [EnumMember(Value = "Converter Ground Current User Limit")]
        ConverterGroundCurrentUL = 17,

        [EnumMember(Value = "Converter Overtemperature Factory Limit")]
        ConverterOvertemperatureFL = 18,

        [EnumMember(Value = "Converter Overtemperature User Limit")]
        ConverterOvertemperatureUL = 19,

        [EnumMember(Value = "Converter Thermal Overload Factory Limit")]
        ConverterThermalOverloadFL = 20,

        [EnumMember(Value = "Converter Thermal Overload User Limit")]
        ConverterThermalOverloadUL = 21,

        [EnumMember(Value = "Converter AC Power Loss")]
        ConverterACPowerLoss = 22,

        [EnumMember(Value = "Converter AC Single Phase Loss")]
        ConverterACSinglePhaseLoss = 23,

        [EnumMember(Value = "Converter AC Phase Short")]
        ConverterACPhaseShort = 24,

        [EnumMember(Value = "Converter Pre-Charge Failure")]
        ConverterPreChargeFailure = 25,

        [EnumMember(Value = "Bus Regulator Overtemperature Factory Limit")]
        BusRegulatorOvertemperatureFL = 27,

        [EnumMember(Value = "Bus Regulator Overtemperature User Limit")]
        BusRegulatorOvertemperatureUL = 28,

        [EnumMember(Value = "Bus Regulator Thermal Overload Factory Limit")]
        BusRegulatorThermalOverloadFL = 29,

        [EnumMember(Value = "Bus Regulator Thermal Overload User Limit")]
        BusRegulatorThermalOverloadUL = 30,

        [EnumMember(Value = "Bus Regulator Failure")]
        BusRegulatorFailure = 31,

        [EnumMember(Value = "Bus Capacitor Module Failure")]
        BusCapacitorModuleFailure = 32,

        [EnumMember(Value = "Bus Undervoltage Factory Limit")]
        BusUndervoltageFL = 33,

        [EnumMember(Value = "Bus Undervoltage User Limit")]
        BusUndervoltageUL = 34,

        [EnumMember(Value = "Bus Overvoltage Factory Limit")]
        BusOvervoltageFL = 35,

        [EnumMember(Value = "Bus Overvoltage User Limit")]
        BusOvervoltageUL = 36,

        [EnumMember(Value = "Bus Power Loss")] BusPowerLoss = 37,

        [EnumMember(Value = "Bus Power Blown Fuse")]
        BusPowerBlownFuse = 38,

        [EnumMember(Value = "Bus Power Leakage")]
        BusPowerLeakage = 39,

        [EnumMember(Value = "Bus Power Sharing")]
        BusPowerSharing = 40,

        [EnumMember(Value = "Feedback Signal Noise Factory Limit")]
        FeedbackSignalNoiseFL = 41,

        [EnumMember(Value = "Feedback Signal Noise User Limit")]
        FeedbackSignalNoiseUL = 42,

        [EnumMember(Value = "Feedback Signal Loss Factory Limit")]
        FeedbackSignalLossFL = 43,

        [EnumMember(Value = "Feedback Signal Loss User Limit")]
        FeedbackSignalLossUL = 44,

        [EnumMember(Value = "Feedback Data Loss Factory Limit")]
        FeedbackDataLossFL = 45,

        [EnumMember(Value = "Feedback Data Loss User Limit")]
        FeedbackDataLossUL = 46,

        [EnumMember(Value = "Feedback Device Failure")]
        FeedbackDeviceFailure = 47,
        [EnumMember(Value = "Brake Slip")] BrakeSlip = 49,

        [EnumMember(Value = "Hardware Overtravel Positive")]
        HardwareOvertravelPositive = 50,

        [EnumMember(Value = "Hardware Overtravel Negative")]
        HardwareOvertravelNegative = 51,

        [EnumMember(Value = "Position Overtravel Positive")]
        PositionOvertravelPositive = 52,

        [EnumMember(Value = "Position Overtravel Negative")]
        PositionOvertravelNegative = 53,

        [EnumMember(Value = "Excessive Position Error")]
        ExcessivePositionError = 54,

        [EnumMember(Value = "Excessive Velocity Error")]
        ExcessiveVelocityError = 55,

        [EnumMember(Value = "Overtorque Limit")]
        OvertorqueLimit = 56,

        [EnumMember(Value = "Undertorque Limit")]
        UndertorqueLimit = 57,

        [EnumMember(Value = "Illegal Control Mode")]
        IllegalControlMode = 60,

        [EnumMember(Value = "Enable Input Deactivated")]
        EnableInputDeactivated = 61,

        [EnumMember(Value = "Controller Initiated Exception")]
        ControllerInitiatedException = 62,

        [EnumMember(Value = "External Exception Input")]
        ExternalExceptionInput = 63
    }

    public enum MotionException : byte
    {
        [EnumMember(Value = "Soft Travel Limit - Positive")]
        SoftTravelLimitPositive = 1,

        [EnumMember(Value = "Soft Travel Limit - Negative")]
        SoftTravelLimitNegative = 2,
    }

    public enum CIPStandardExceptionRA : byte
    {
        [EnumMember(Value = "Motor Voltage Mismatch")]
        MotorVoltageMismatch = 2,

        [EnumMember(Value = "Feedback Battery Loss")]
        FeedbackBatteryLoss = 5,

        [EnumMember(Value = "Feedback Battery Low")]
        FeedbackBatteryLow = 6,

        [EnumMember(Value = "Excessive Current Feedback Offset")]
        ExcessiveCurrentFeedbackOffset = 14,

        [EnumMember(Value = "DC Common Bus")] DCCommonBus = 25,

        [EnumMember(Value = "Runtime Error")] RuntimeError = 26,
    }

    #endregion

    #region Status

    [Flags]
    public enum CIPAxisStatusBitmap : uint
    {
        // rm003, page 235
        // Table 43 - CIP Axis Status Bit Descriptions
        //0 = Local Control
        //1 = Alarm
        //2 = DC Bus Up
        //3 = Power Structure Enabled
        //4 = Motor Flux Up
        //5 = Tracking Command
        //6 = Position Lock
        //7 = Velocity Lock
        //8 = Velocity Standstill
        //9 = Velocity Threshold
        //10 = Velocity Limit
        //11 = Acceleration Limit
        //12 = Deceleration Limit
        //13 = Torque Threshold
        //14 = Torque Limit
        //15 = Current Limit
        //16 = Thermal Limit
        //17 = Feedback Integrity
        //18 = Shutdown
        //19 = In Process
        //20…31 = Reserved
        LocalControl = 1 << 0,
        Alarm = 1 << 1,
        DCBusUp = 1 << 2,
        PowerStructureEnabled = 1 << 3,
        MotorFluxUp = 1 << 4,
        TrackingCommand = 1 << 5,
        PositionLock = 1 << 6,
        VelocityLock = 1 << 7,
        VelocityStandstill = 1 << 8,
        VelocityThreshold = 1 << 9,
        VelocityLimit = 1 << 10,
        AccelerationLimit = 1 << 11,
        DecelerationLimit = 1 << 12,
        TorqueThreshold = 1 << 13,
        TorqueLimit = 1 << 14,
        CurrentLimit = 1 << 15,
        ThermalLimit = 1 << 16,
        FeedbackIntegrity = 1 << 17,
        Shutdown = 1 << 18,
        InProcess = 1 << 19,
        DCBusUnload = 1 << 20,
        ACPowerLoss = 1 << 21,
        PositionControlMode = 1 << 22,
        VelocityControlMode = 1 << 23,
        TorqueControlMode = 1 << 24
    }

    [Flags]
    public enum CIPAxisIOStatusBitmap : uint
    {
        // rm003, page 240
        // Table 45 - CIP Axis I/O Status Bit Descriptions
        EnableInput = 1 << 0,
        HomeInput = 1 << 1,
        Registration1Input = 1 << 2,
        Registration2Input = 1 << 3,
        PositiveOvertravelInput = 1 << 4,
        NegativeOvertravelInput = 1 << 5,
        Feedback1Thermostat = 1 << 6,
        ResistiveBrakeOutput = 1 << 7,
        MechanicalBrakeOutput = 1 << 8,
        MotorThermostatInput = 1 << 9,
    }

    [Flags]
    public enum CIPAxisIOStatusRABitmap : uint
    {
        // rm003, page 240
        // Table 46 - CIP Axis I/O Status - RA Bit Descriptions
        RegenerativePowerInput = 1 << 0,
        BusCapacitorModuleInput = 1 << 1,
        ShuntThermalSwitchInput = 1 << 2,
        ContactorEnableOutput = 1 << 3,
        PreChargeInput = 1 << 4
    }

    #endregion

    [TypeConverter(typeof(EnumMemberValueConverter))]
    public enum CIPAxisStateType : byte
    {
        // Unconnected in help
        // Initializing in rm003
        [EnumMember(Value = "Unconnected")] Unconnected = 0,
        [EnumMember(Value = "Pre-charge")] Precharge,
        [EnumMember(Value = "Stopped")] Stopped,
        [EnumMember(Value = "Starting")] Starting,
        [EnumMember(Value = "Running")] Running,
        [EnumMember(Value = "Testing")] Testing,
        [EnumMember(Value = "Stopping")] Stopping,
        [EnumMember(Value = "Aborting")] Aborting,
        [EnumMember(Value = "Faulted")] Faulted,

        [EnumMember(Value = "Start Inhibited")]
        StartInhibited,
        [EnumMember(Value = "Shutdown")] Shutdown,

        [EnumMember(Value = "Axis Inhibited")] AxisInhibited,
        [EnumMember(Value = "Not Grouped")] NotGrouped,
        [EnumMember(Value = "No Module")] NoModule,
        [EnumMember(Value = "Configuring")] Configuring,
        [EnumMember(Value = "Synchronizing")] Synchronizing,

        [EnumMember(Value = "Waiting for Group")]
        WaitingForGroup
    }

    public enum BooleanType : byte
    {
        [EnumMember(Value = "Disabled")] Disabled = 0,
        [EnumMember(Value = "Enabled")] Enabled = 1
    }

    public enum BooleanTypeA : byte
    {
        [EnumMember(Value = "No")] No = 0,
        [EnumMember(Value = "Yes")] Yes = 1
    }

    public enum VoltageType : ushort
    {
        [EnumMember(Value = "200-240 VAC")] Voltage230VAC = 230,
        [EnumMember(Value = "400-480 VAC")] Voltage460VAC = 460
    }

    public enum ACInputPhasingType : byte
    {
        [EnumMember(Value = "Three Phase")] ThreePhase = 0,
        [EnumMember(Value = "Single Phase")] SinglePhase = 1
    }

    public enum BusConfigurationType : byte
    {
        [EnumMember(Value = "Standalone")] Standalone = 0,

        [EnumMember(Value = "Shared AC/DC")] SharedACDC,
        [EnumMember(Value = "Shared DC")] SharedDC,
    }

    public enum BusSharingGroupType : byte
    {
        [EnumMember(Value = "Standalone")] Standalone = 0,
        [EnumMember(Value = "Group1")] Group1,
        [EnumMember(Value = "Group2")] Group2,
        [EnumMember(Value = "Group3")] Group3,
        [EnumMember(Value = "Group4")] Group4,
        [EnumMember(Value = "Group5")] Group5,
        [EnumMember(Value = "Group6")] Group6,
        [EnumMember(Value = "Group7")] Group7,
        [EnumMember(Value = "Group8")] Group8,
        [EnumMember(Value = "Group9")] Group9,
        [EnumMember(Value = "Group10")] Group10,
        [EnumMember(Value = "Group11")] Group11,
        [EnumMember(Value = "Group12")] Group12,
        [EnumMember(Value = "Group13")] Group13,
        [EnumMember(Value = "Group14")] Group14,
        [EnumMember(Value = "Group15")] Group15,
        [EnumMember(Value = "Group16")] Group16,
        [EnumMember(Value = "Group17")] Group17,
        [EnumMember(Value = "Group18")] Group18,
        [EnumMember(Value = "Group19")] Group19,
        [EnumMember(Value = "Group20")] Group20,
        [EnumMember(Value = "Group21")] Group21,
        [EnumMember(Value = "Group22")] Group22,
        [EnumMember(Value = "Group23")] Group23,
        [EnumMember(Value = "Group24")] Group24,
        [EnumMember(Value = "Group25")] Group25,
    }

    public enum BusRegulatorActionType : byte
    {
        [EnumMember(Value = "Disabled")] Disabled = 0,

        [EnumMember(Value = "Shunt Regulator")]
        ShuntRegulator,
    }

    public enum ShuntRegulatorResistorType : byte
    {
        Internal = 0,
        External = 1
    }

    public enum DigitalInputType : byte
    {
        // page 271, rm003
        [EnumMember(Value = "Unassigned")] Unassigned = 0,
        [EnumMember(Value = "Enable")] Enable,
        [EnumMember(Value = "Home")] Home,

        [EnumMember(Value = "Registration 1")] Registration1,

        [EnumMember(Value = "Registration 2")] Registration2,

        [EnumMember(Value = "Positive Overtravel")]
        PositiveOvertravel,

        [EnumMember(Value = "Negative Overtravel")]
        NegativeOvertravel,

        [EnumMember(Value = "Regeneration OK")]
        RegenerationOK,

        [EnumMember(Value = "Bus Capacitor OK")]
        BusCapacitorOK,

        [EnumMember(Value = "Shunt Thermal Switch OK")]
        ShuntThermalSwitchOK,

        [EnumMember(Value = "Home & Registration 1")]
        HomeAndRegistration1,

        [EnumMember(Value = "Motor Thermostat OK")]
        MotorThermostatOK,
        [EnumMember(Value = "PreCharge OK")] PreChargeOK,
    }

    [TypeConverter(typeof(EnumMemberValueConverter))]
    public enum AxisStateType : byte
    {
        // rm003, page 66
        //0 = Ready
        //1 = Drive Enable, (direct drive control)
        //2 = Servo Control
        //3 = Faulted
        //4 = Shutdown
        //5 = Inhibited
        //6 = Ungrouped (axis unassigned)
        //7 = No Module
        //8 = Configuring
        //FW = default
        [EnumMember(Value = "Ready")] Ready = 0,
        [EnumMember(Value = "Drive Enable")] DriveEnable,
        [EnumMember(Value = "Servo Control")] ServoControl,
        [EnumMember(Value = "Faulted")] Faulted,
        [EnumMember(Value = "Shutdown")] Shutdown,
        [EnumMember(Value = "Inhibited")] Inhibited,
        [EnumMember(Value = "Ungrouped")] Ungrouped,
        [EnumMember(Value = "No Module")] NoModule,
        [EnumMember(Value = "Configuring")] Configuring
    }

    [Flags]
    public enum AxisFaultBitmap
    {
        // page 83
        PhysicalAxisFault = 1 << 0,
        ModuleFault = 1 << 1,
        ConfigurationFault = 1 << 2,
        GroupFault = 1 << 3,
        MotionFault = 1 << 4,
        GuardFault = 1 << 5,
        InitializationFault = 1 << 6,
        APRFault = 1 << 7,

        SafetyFault = 1 << 8
        //9…31 = Reserved
    }

    [Flags]
    public enum StandardStartInhibitBitmap : ushort
    {
        // 0: Reserved
        [EnumMember(Value = "Axis Enable Input")]
        AxisEnableInput = 1 << 1,

        [EnumMember(Value = "Motor Not Configured")]
        MotorNotConfigured = 1 << 2,

        [EnumMember(Value = "Feedback Not Configured")]
        FeedbackNotConfigured = 1 << 3,

        [EnumMember(Value = "Commutation Not Configured")]
        CommutationNotConfigured = 1 << 4,

        [EnumMember(Value = "Safe Torque Off Active")]
        SafeTorqueOffActive = 1 << 5

        // 6-15:Reserved
    }

    [Flags]
    public enum ModuleFaultBitmap : uint
    {
        ControlSyncFault = 1 << 0,
        ModuleSyncFault = 1 << 1,
        TimerEventFault = 1 << 2,
        ModuleHardwareFault = 1 << 3,
        ModuleConnFault = 1 << 6,
        ConnFormatFault = 1 << 7,
        LocalModeFault = 1 << 8,
        CPUWatchdogFault = 1 << 9,
        ClockJitterFault = 1 << 10,
        CyclicReadFault = 1 << 11,
        CyclicWriteFault = 1 << 12,
        ClockSkewFault = 1 << 13,
        ControlConnFault = 1 << 14,
        ClockSyncFault = 1 << 16,
        LogicWatchdogFault = 1 << 17,
        DuplicateAddressFault = 1 << 18,
        SystemConnectionFault = 1 << 19,
    }

    [Flags]
    public enum CIPInitializationFaultBitmap : uint
    {
        BootBlockChecksumFault = 1 << 1,
        MainBlockChecksumFault = 1 << 2,
        NonvolatileMemoryChecksumFault = 1 << 3,
    }

    [Flags]
    public enum CIPAPRFaultBitmap : ushort
    {
        MemoryWriteErrorAPRFault = 1 << 1,
        MemoryReadErrorAPRFault = 1 << 2,
        FeedbackSerialNumberMismatchAPRFault = 1 << 3,
        BufferAllocationAPRFault = 1 << 4,
        ScalingConfigurationChangedAPRFault = 1 << 5,
        FeedbackModeChangedAPRFault = 1 << 6,
        FeedbackIntegrityLossAPRFault = 1 << 7,
    }

    [Flags]
    public enum GuardFaultBitmap : uint
    {
        GuardInternalFault = 1 << 1,
        GuardConfigurationFault = 1 << 2,
        GuardGateDriveFault = 1 << 3,
        GuardResetFault = 1 << 4,
        GuardFeedback1Fault = 1 << 5,
        GuardFeedback2Fault = 1 << 6,
        GuardFeedbackSpeedCompareFault = 1 << 7,
        GuardFeedbackPositionCompareFault = 1 << 8,
        GuardStopInputFault = 1 << 9,
        GuardStopOutputFault = 1 << 10,
        GuardStopDecelFault = 1 << 11,
        GuardStopStandstillFault = 1 << 12,
        GuardStopMotionFault = 1 << 13,
        GuardLimitedSpeedInputFault = 1 << 14,
        GuardLimitedSpeedOutputFault = 1 << 15,
        GuardLimitedSpeedMonitorFault = 1 << 16,
        GuardMaxSpeedMonitorFault = 1 << 17,
        GuardMaxAccelMonitorFault = 1 << 18,
        GuardDirectionMonitorFault = 1 << 19,
        GuardDoorMonitorInputFault = 1 << 20,
        GuardDoorMonitorFault = 1 << 21,
        GuardDoorControlOutputFault = 1 << 22,
        GuardLockMonitorInputFault = 1 << 23,
        GuardLockMonitorFault = 1 << 24,
        GuardEnablingSwitchMonitorInputFault = 1 << 25,
        GuardEnablingSwitchMonitorFault = 1 << 26,
        GuardFeedback1VoltageMonitorFault = 1 << 27,
        GuardFeedback2VoltageMonitorFault = 1 << 28,

    }

    public enum FluxUpControlType : byte
    {
        [EnumMember(Value = "No Delay")] NoDelay = 0,
        [EnumMember(Value = "Manual Delay")] ManualDelay = 1,

        [EnumMember(Value = "Automatic Delay")]
        AutomaticDelay = 2,

        // 3-255=Reserved
    }

    public enum FrequencyControlMethodType : byte
    {
        [EnumMember(Value = "Basic Volts/Hertz")]
        BasicVoltsHertz = 0,

        //1-127 = Reserved
        [EnumMember(Value = "Fan/Pump Volts/Hertz")]
        FanPumpVoltsHertz = 128,

        [EnumMember(Value = "Sensorless Vector")]
        SensorlessVector = 129,

        [EnumMember(Value = "Sensorless Vector Economy")]
        SensorlessVectorEconomy = 130,
    }
}
