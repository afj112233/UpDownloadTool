using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ICSStudio.SimpleServices.Instruction
{
    public partial class InstrEnum
    {
        public enum AddOnInstructionDefinition
        {
            LastEditDate,
            MajorRevision,
            MinorRevision,
            Name,
            RevisionExtendText,
            SafetySignatureID,
            SignatureID,
            Vendor
        }

        public enum Axis
        {
            //status
            ACLineCurrent = 0x07DB,
            ACLineElectricalAngle = 0x08B1,
            ACLineFrequency = 0x07DA,
            ACLineVoltage = 0x07DC,
            AccelerationCommand = 0x01E0,
            AccelerationFeedback = 0x01E3,
            AccelerationFeedforwardCommand = 0x01C4,
            AccelerationFineCommand = 0x016F,
            AccelerationReference = 0x01E2,
            ActiveCurrentError = 0x083A,
            ActiveCurrentFeedback = 0x0846,
            ActiveCurrentReference = 0x0820,
            ActiveCurrentReferenceCompensated = 0x0822,
            ActiveCurrentReferenceFiltered = 0x0821,
            ActiveCurrentReferenceLimited = 0x0838,
            ActualAcceleration = 0x0035,
            ActualPosition = 0x0030,
            ActualVelocity = 0x0034,
            AdaptiveTuningGainScalingFactor = 0x034C,
            AnalogInput1 = 0x02DC,
            AnalogInput2 = 0x02DD,
            AttributeErrorCode = 0x00A4,
            AttributeErrorID = 0x00A5,
            AuxPositionFeedback = 0x1001,
            AverageVelocity = 0x0033,
            AxisControlBits = 0x1002,
            AxisEventBits = 0x0023,
            AxisFaultBits = 0x0022,
            AxisFeatures = 0x0013,
            AxisID = 0x006A,
            AxisResponseBits = 0x1003,
            AxisSafetyAlarms = 0x02F1,
            AxisSafetyAlarmsRA = 0x03DC,
            AxisSafetyDataA = 0x03DA,
            AxisSafetyDataB = 0x03DB,
            AxisSafetyFaults = 0x02FB,
            AxisSafetyFaultsRA = 0x03D9,
            AxisSafetyState = 0x02F8,
            AxisSafetyStatus = 0x02F9,
            AxisSafetyStatusRA = 0x03D8,
            AxisStatusBits = 0x0021,
            BusObserverCurrentEstimate = 0x032C,
            BusObserverVoltageRateEstimate = 0x032B,
            BusRegulatorCapacity = 0x027E,
            BusVoltageError = 0x0806,
            BusVoltageFeedback = 0x0805,
            BusVoltageReference = 0x0802,
            CIPAPRFaults = 0x02F4,
            CIPAPRFaultsRA = 0x0389,
            CIPAxisAlarms = 0x0293,
            CIPAxisAlarms2 = 0x02EA,
            CIPAxisAlarms2RA = 0x039F,
            CIPAxisAlarmsRA = 0x0388,
            CIPAxisFaults = 0x0291,
            CIPAxisFaults2 = 0x02E8,
            CIPAxisFaults2RA = 0x039E,
            CIPAxisFaultsRA = 0x0387,
            CIPAxisIOStatus = 0x028D,
            CIPAxisIOStatusRA = 0x0385,
            CIPAxisState = 0x028A,
            CIPAxisStatus = 0x028B,
            CIPAxisStatus2 = 0x02E4,
            CIPAxisStatus2RA = 0x039C,
            CIPAxisStatusRA = 0x0384,
            CIPInitializationFaults = 0x02A2,
            CIPInitializationFaultsRA = 0x038E,
            CIPStartInhibits = 0x02A4,
            CIPStartInhibitsRA = 0x0390,
            CommandAcceleration = 0x0064,
            CommandPosition = 0x0060,
            CommandVelocity = 0x0063,
            CommutationSelfSensingCurrent = 0x0232,
            ControlMethod = 0x0029,
            ConverterCapacity = 0x027D,
            ConverterOutputCurrent = 0x025D,
            ConverterOutputPower = 0x025E,
            CurrentCommand = 0x0208,
            CurrentError = 0x020F,
            CurrentFeedback = 0x0211,
            CurrentLimitSource = 0x020A,
            CurrentReference = 0x020C,
            DCBusVoltage = 0x026C,
            DigitalInputs = 0x02DA,
            DriveCapacity = 0x1004,
            DriveFaultBits = 0x1005,
            DriveStatusBits = 0x1006,
            DriveWarningBits = 0x1007,
            FluxCurrentError = 0x0210,
            FluxCurrentFeedback = 0x0212,
            FluxCurrentReference = 0x020D,
            FrequencyControlMethod = 0x023A,
            GuardFaults = 0x03D5,
            GuardStatus = 0x03D4,
            HookupTestCommutationOffset = 0x00F5,
            HookupTestCommutationPolarity = 0x00F6,
            HookupTestFeedback1Direction = 0x00F7,
            HookupTestFeedback2Direction = 0x00F8,
            HookupTestStatus = 0x00F4,
            InterpolatedActualPosition = 0x003C,
            InterpolatedCommandPosition = 0x0065,
            InverterCapacity = 0x027C,
            LoadObserverAccelerationEstimate = 0x0321,
            LoadObserverTorqueEstimate = 0x0322,
            MarkerDistance = 0x1008,
            MasterOffset = 0x0066,
            ModuleAlarmBits = 0x009F,
            ModuleFaultBits = 0x00A3,
            MotionAlarmBits = 0x0017,
            MotionFaultBits = 0x0018,
            MotionStatusBits = 0x0020,
            MotorCapacity = 0x027B,
            MotorElectricalAngle = 0x020B,
            MotorTestBusOvervoltageSpeed = 0x03E8,
            MotorTestCommutationOffsetComp = 0x03E9,
            MotorTestCounterEMF = 0x00AE,
            MotorTestFluxCurrent = 0x00AC,
            MotorTestInductance = 0x00AB,
            MotorTestLdFluxSaturation = 0x03E7,
            MotorTestLdInductance = 0x03E5,
            MotorTestLqFluxSaturation = 0x03E6,
            MotorTestLqInductance = 0x03e4,
            MotorTestResistance = 0x00AA,
            MotorTestSlipSpeed = 0x00AD,
            MotorTestStatus = 0x00AF,
            MotorUnit = 0x0524,
            NegativeDynamicTorqueLimit = 0x1009,
            OperativeCurrentLimit = 0x0209,
            OutputCamLockStatus = 0x0026,
            OutputCamPendingStatus = 0x0025,
            OutputCamStatus = 0x0024,
            OutputCamTransitionStatus = 0x0027,
            OutputCurrent = 0x0259,
            OutputFrequency = 0x0258,
            OutputPower = 0x025B,
            OutputVoltage = 0x025A,
            PlannerActualPosition = 0x043A,
            PlannerCommandPositionFractional = 0x0439,
            PlannerCommandPositionInteger = 0x0438,
            PositionCommand = 0x1010,
            PositionError = 0x01B4,
            PositionFeedback = 0x1011,
            PositionFeedback1 = 0x057A,
            PositionFeedback2 = 0x05AC,
            PositionFineCommand = 0x016D,
            PositionIntegratorError = 0x1012,
            PositionIntegratorOutput = 0x01B5,
            PositionLoopOutput = 0x01B6,
            PositionReference = 0x01B0,
            PositiveDynamicTorqueLimit = 0x1013,
            PowerCapacity = 0x1014,
            ReactiveCurrentError = 0x083B,
            ReactiveCurrentFeedback = 0x0847,
            ReactiveCurrentReference = 0x0823,
            ReactiveCurrentReferenceCompensated = 0x0824,
            ReactiveCurrentReferenceLimited = 0x0839,
            Registration1NegativeEdgePosition = 0x003F,
            Registration1NegativeEdgeTime = 0x0043,
            Registration1Position = 0x0037,
            Registration1PositiveEdgePosition = 0x003E,
            Registration1PositiveEdgeTime = 0x0042,
            Registration1Time = 0x0039,
            Registration2NegativeEdgePosition = 0x0041,
            Registration2NegativeEdgeTime = 0x0045,
            Registration2Position = 0x0038,
            Registration2PositiveEdgePosition = 0x0040,
            Registration2PositiveEdgeTime = 0x0044,
            Registration2Time = 0x003A,
            SercosErrorCode = 0x1015,
            ServoFaultBits = 0x1016,
            ServoOutputLevel = 0x1017,
            ServoStatusBits = 0x1018,
            SlipCompensation = 0x0235,
            StartActualPosition = 0x0032,
            StartCommandPosition = 0x0062,
            StartMasterOffset = 0x0068,
            StrobeActualPosition = 0x0031,
            StrobeCommandPosition = 0x0061,
            StrobeMasterOffset = 0x0067,
            TestDirectionForward = 0x1019,
            TestStatus = 0x1020,
            TorqueFeedback = 0x1021,
            TorqueLimitSource = 0x1022,
            TorqueLowPassFilterBandwidthEstimate = 0x034B,
            TorqueNotchFilterFrequencyEstimate = 0x0349,
            TorqueNotchFilterMagnitudeEstimate = 0x034A,
            TorqueReference = 0x01EC,
            TorqueReferenceFiltered = 0x01ED,
            TorqueReferenceLimited = 0x01EF,
            TuneAcceleration = 0x00B5,
            TuneAccelerationTime = 0x00B3,
            TuneDeceleration = 0x00B6,
            TuneDecelerationTime = 0x00B4,
            TuneInertia = 0x1023,
            TuneRiseTime = 0x1024,
            TuneSpeedScaling = 0x1025,
            TuneStatus = 0x00B2,
            VelocityCommand = 0x1026,
            VelocityError = 0x01C7,
            VelocityFeedback = 0x01C6,
            VelocityFeedforwardCommand = 0x01B1,
            VelocityFineCommand = 0x016E,
            VelocityIntegratorError = 0x1027,
            VelocityIntegratorOutput = 0x01C8,
            VelocityLimitSource = 0x01CA,
            VelocityLoopOutput = 0x01C9,
            VelocityReference = 0x01C5,

            //interface
            AxisConfigurationState = 0x000C,
            AxisInstance = 0x0002,
            AxisState = 0x000D,
            C2CConnectionInstance = 0x0008,
            C2CMapInstance = 0x0007,
            GroupInstance = 0x0003,
            MapInstance = 0x0004,
            MemoryUse = 0x0009,
            ModuleChannel = 0x0005,
            ModuleClassCode = 0x0006,
            OutputCamExecutionTargets = 0x000E,

            //configuration
            ACLineCurrentUnbalanceLimit = 0x07F2,
            ACLineFrequencyChangeAction = 0x08C5,
            ACLineFrequencyChangeThreshold = 0x08C6,
            ACLineFrequencyChangeTime = 0x08C7,
            ACLineHighFreqUserLimit = 0x08EC,
            ACLineHighFreqUserLimitAlternate = 0x08EE,
            ACLineLowFreqUserLimit = 0x08ED,
            ACLineLowFreqUserLimitAlternate = 0x08EF,
            ACLineOverloadUserLimit = 0x08E8,
            ACLineOvervoltageUserLimit = 0x1028,
            ACLineOvervoltageUserLimitAlternate = 0x08EA,
            ACLineResonanceUserLimit = 0x1029,
            ACLineSourceImpedance = 0x07F9,
            ACLineSourceImpedanceAlternate = 0x07FB,
            ACLineSourcePower = 0x07FA,
            ACLineSourcePowerAlternate = 0x07FC,
            ACLineSourceSelect = 0x07F8,
            ACLineSyncErrorTolerance = 0x07F3,
            ACLineSyncLossAction = 0x08C8,
            ACLineSyncLossTime = 0x08C9,
            ACLineUndervoltageUserLimit = 0x08E9,
            ACLineUndervoltageUserLimitAlternate = 0x08EB,
            ACLineVoltageSagAction = 0x08C2,
            ACLineVoltageSagThreshold = 0x08C1,
            ACLineVoltageSagTime = 0x08C1,
            ACLineVoltageTimeConstant = 0x07DE,
            ACLineVoltageUnbalanceLimit = 0x07F1,
            AbsoluteFeedbackEnable = 0x1030,
            AbsoluteFeedbackOffset = 0x1031,
            AccelerationDataScaling = 0x1032,
            AccelerationDataScalingExp = 0x1033,
            AccelerationDataScalingFactor = 0x1034,
            AccelerationFeedforwardGain = 0x01CC,
            AccelerationLimit = 0x01E5,
            AccelerationLimitBipolar = 0x1035,
            AccelerationLimitNegative = 0x1036,
            AccelerationLimitPositive = 0x1037,
            AccelerationTrim = 0x01E1,
            ActiveCurrentCommand = 0x082B,
            ActiveCurrentLowPassFilterBandwidth = 0x082E,
            ActiveCurrentNotchFilterFrequency = 0x082F,
            ActiveCurrentRateLimit = 0x0830,
            ActiveCurrentTrim = 0x082D,
            ActuatorDiameter = 0x0560,
            ActuatorDiameterUnit = 0x0561,
            ActuatorLead = 0x055E,
            ActuatorLeadUnit = 0x055F,
            ActuatorType = 0x055D,
            AdaptiveTuningConfiguration = 0x0344,
            AnalogOutput1 = 0x02DE,
            AnalogOutput2 = 0x02DF,
            AutoSagConfiguration = 0x0369,
            AutoSagSlipIncrement = 0x036A,
            AutoSagSlipTimeLimit = 0x036B,
            AutoSagStart = 0x036C,
            AuxFeedbackConfiguration = 0x1038,
            AuxFeedbackInterpolationFactor = 0x1039,
            AuxFeedbackRatio = 0x1040,
            AuxFeedbackResolution = 0x1041,
            AuxFeedbackType = 0x1042,
            AuxFeedbackUnit = 0x1043,
            AverageVelocityTimebase = 0x0051,
            AxisConfiguration = 0x001E,
            AxisInfoSelect1 = 0x1044,
            AxisInfoSelect2 = 0x1045,
            AxisType = 0x1046,
            AxisUpdateSchedule = 0x007C,
            BacklashCompensationWindow = 0x1047,
            BacklashReversalOffset = 0x01A7,
            BacklashStabilizationWindow = 0x011E,
            BrakeEngageDelayTime = 0x1048,
            BrakeProveRampTime = 0x0251,
            BrakeReleaseDelayTime = 0x1049,
            BrakeSlipTolerance = 0x0252,
            BrakeTestTorque = 0x0250,
            BreakFrequency = 0x0240,
            BreakVoltage = 0x023F,
            BusObserverBandwidth = 0x0330,
            BusObserverConfiguration = 0x032F,
            BusObserverIntegratorBandwidth = 0x0331,

            BusRegulatorID = 0x1050,

            BusVoltageErrorTolerance = 0x0811,

            BusVoltageErrorToleranceTime = 0x0812,

            BusVoltageIntegratorBandwidth = 0x080F,

            BusVoltageLoopBandwidth = 0x080E,

            BusVoltageRateLimit = 0x0810,

            BusVoltageReferenceSource = 0x080D,

            BusVoltageSetPoint = 0x080C,

            CIPAxisAlarmLogReset = 0x001C,

            CIPAxisFaultLogReset = 0x001B,

            CoastingTimeLimit = 0x0269,

            CommandTorque = 0x005F,

            CommandUpdateDelayOffset = 0x0168,

            CommutationOffset = 0x0231,

            CommutationOffsetCompensation = 0x0352,

            CommutationPolarity = 0x0233,

            ConfigurationProfile = 0x1051,

            ConnectionLossStoppingAction = 0x026A,

            ContinuousTorqueLimit = 0x1052,

            ControlMode = 0x0028,

            ConversionConstant = 0x0052,

            ConverterACInputFrequency = 0x07EE,

            ConverterACInputPhasing = 0x07EF,

            ConverterACInputVoltage = 0x07F0,

            ConverterConfiguration = 0x0500,

            ConverterControlMode = 0x07D1,

            ConverterCurrentIntegratorBandwidth = 0x08B7,

            ConverterCurrentLimitSource = 0x0837,

            ConverterCurrentLoopBandwidth = 0x08B6,

            ConverterCurrentLoopDamping = 0x0912,

            ConverterCurrentLoopTuningMethod = 0x0911,

            ConverterCurrentVectorLimit = 0x08B8,

            ConverterGroundCurrentUserLimit = 0x02C5,

            ConverterHeatsinkOvertempUserLimit = 0x08F0,

            ConverterInputPhaseLossAction = 0x08C3,

            ConverterInputPhaseLossTime = 0x08C4,

            ConverterModelTimeConstant = 0x0419,

            ConverterMotoringPowerLimit = 0x0254,

            ConverterOperativeCurrentLimit = 0x0834,

            ConverterOverloadAction = 0x08DC,

            ConverterOvertemperatureUserLimit = 0x02BC,

            ConverterPreChargeOverloadUserLimit = 0x0399,

            ConverterRegenerativePowerLimit = 0x0272,

            ConverterStartupMethod = 0x07D3,

            ConverterThermalOverloadUserLimit = 0x02BD,

            CurrentDisturbance = 0x0348,

            CurrentVectorLimit = 0x0229,

            DCInjectionBrakeCurrent = 0x0366,

            DCInjectionBrakeTime = 0x0368,

            DampingFactor = 0x00C4,

            DecelerationLimit = 0x01E6,

            DigitalOutputs = 0x02DB,

            DirectCommandVelocity = 0x0069,

            DirectDriveRampRate = 0x1053,

            DirectionalScalingRatio = 0x1054,

            DriveAxisID = 0x1055,

            DriveEnableInputFaultAction = 0x1056,

            DriveFaultAction = 0x1057,

            DriveModelTimeConstant = 0x00C8,

            DrivePolarity = 0x1058,

            DriveResolution = 0x1059,

            DriveScalingBits = 0x1060,

            DriveThermalFaultAction = 0x1061,

            DriveUnit = 0x1062,

            DynamicsConfigurationBits = 0x0078,

            ExternalDCBusCapacitance = 0x0421,

            ExternalDriveType = 0x016B,

            FaultConfigurationBits = 0x1063,

            Feedback1AccelFilterBandwidth = 0x059B,

            Feedback1AccelFilterTaps = 0x0964,

            Feedback1BatteryAbsolute = 0x0965,

            Feedback1CycleInterpolation = 0x0589,

            Feedback1CycleResolution = 0x0588,

            Feedback1DataCode = 0x058D,

            Feedback1DataLength = 0x058C,

            Feedback1Length = 0x058B,

            Feedback1LossAction = 0x0960,

            Feedback1Polarity = 0x0586,

            Feedback1ResolverCableBalance = 0x0591,

            Feedback1ResolverExcitationFrequency = 0x0590,

            Feedback1ResolverExcitationVoltage = 0x058F,

            Feedback1ResolverTransformerRatio = 0x058E,

            Feedback1StartupMethod = 0x0587,

            Feedback1Turns = 0x058A,

            Feedback1Type = 0x0585,

            Feedback1Unit = 0x0583,

            Feedback1VelocityFilterBandwidth = 0x059A,

            Feedback1VelocityFilterTaps = 0x0963,

            Feedback2AccelFilterBandwidth = 0x05CD,

            Feedback2AccelFilterTaps = 0x0996,

            Feedback2BatteryAbsolute = 0x0997,

            Feedback2CycleInterpolation = 0x05BB,

            Feedback2CycleResolution = 0x05BA,

            Feedback2DataCode = 0x05BF,

            Feedback2DataLength = 0x05BE,

            Feedback2Length = 0x05BD,

            Feedback2LossAction = 0x0992,

            Feedback2Polarity = 0x05B8,

            Feedback2ResolverCableBalance = 0x05C3,

            Feedback2ResolverExcitationFrequency = 0x05C2,

            Feedback2ResolverExcitationVoltage = 0x05C1,

            Feedback2ResolverTransformerRatio = 0x05C0,

            Feedback2StartupMethod = 0x05B9,

            Feedback2Turns = 0x05BC,

            Feedback2Type = 0x05B7,

            Feedback2Unit = 0x05B5,

            Feedback2VelocityFilterBandwidth = 0x05CC,

            Feedback2VelocityFilterTaps = 0x0995,

            FeedbackCommutationAligned = 0x00FA,

            FeedbackConfiguration = 0x001F,

            FeedbackDataLossUserLimit = 0x02C4,

            FeedbackFaultAction = 0x1064,

            FeedbackMasterSelect = 0x1065,

            FeedbackMode = 0x002A,

            FeedbackNoiseFaultAction = 0x1066,

            FeedbackNoiseUserLimit = 0x02C2,

            FeedbackSignalLossUserLimit = 0x02C3,

            FeedbackUnitRatio = 0x002C,

            FluxBrakingEnable = 0x0367,

            FluxIntegralTimeConstant = 0x022D,

            FluxLoopBandwidth = 0x022C,

            FluxUpControl = 0x022E,

            FluxUpTime = 0x022F,

            FlyingStartEnable = 0x017C,

            FlyingStartMethod = 0x017D,

            FrictionCompensation = 0x1067,

            FrictionCompensationSliding = 0x01F2,

            FrictionCompensationStatic = 0x01F3,

            FrictionCompensationViscous = 0x01F4,

            FrictionCompensationWindow = 0x0320,

            GainTuningConfigurationBits = 0x00BD,

            HardOvertravelFaultAction = 0x1068,

            HomeConfigurationBits = 0x0058,

            HomeDirection = 0x0056,

            HomeMode = 0x0055,

            HomeOffset = 0x005A,

            HomePosition = 0x0059,

            HomeReturnSpeed = 0x0071,

            HomeSequence = 0x0057,

            HomeSpeed = 0x0070,

            HomeTorqueLevel = 0x1069,

            HookupTestDistance = 0x006D,

            HookupTestFeedbackChannel = 0x006F,

            HookupTestTime = 0x006E,

            InductionMotorFluxCurrent = 0x0542,

            InductionMotorMagnetizationReactance = 0x0545,

            InductionMotorRatedFrequency = 0x0541,

            InductionMotorRatedSlipSpeed = 0x0548,

            InductionMotorRotorLeakageReactance = 0x0547,

            InductionMotorRotorResistance = 0x0546,

            InductionMotorStatorLeakageReactance = 0x0544,

            InductionMotorStatorResistance = 0x0543,

            InhibitAxis = 0x0014,

            InputPowerPhase = 0x1070,

            IntegratorHoldEnable = 0x1071,

            InterpolatedPositionConfiguration = 0x006C,

            InterpolationTime = 0x003B,

            InverterOverloadAction = 0x0287,

            InverterThermalOverloadUserLimit = 0x02BB,

            LDTCalibrationConstant = 0x1072,

            LDTCalibrationConstantUnits = 0x1073,

            LDTLength = 0x1074,

            LDTLengthUnits = 0x1075,

            LDTRecirculations = 0x1076,

            LDTScaling = 0x1077,

            LDTScalingUnits = 0x1078,

            LDTType = 0x1079,

            LinearMotorDampingCoefficient = 0x053A,

            LinearMotorIntegralLimitSwitch = 0x0909,

            LinearMotorMass = 0x0538,

            LinearMotorMaxSpeed = 0x0539,

            LinearMotorPolePitch = 0x0536,

            LinearMotorRatedSpeed = 0x0537,

            LoadInertiaRatio = 0x0160,

            LoadObserverBandwidth = 0x0326,

            LoadObserverConfiguration = 0x0325,

            LoadObserverFeedbackGain = 0x0329,

            LoadObserverIntegratorBandwidth = 0x0327,

            LoadRatio = 0x00CD,

            LoadType = 0x055a,

            MasterInputConfigurationBits = 0x0015,

            MasterPositionFilterBandwidth = 0x0016,

            MaximumAcceleration = 0x0073,

            MaximumAccelerationJerk = 0x0076,

            MaximumDeceleration = 0x0074,

            MaximumDecelerationJerk = 0x0077,

            MaximumFrequency = 0x023D,

            MaximumNegativeTravel = 0x1080,

            MaximumPositiveTravel = 0x1081,

            MaximumSpeed = 0x0072,

            MaximumVoltage = 0x023C,

            MechanicalBrakeControl = 0x0266,

            MechanicalBrakeEngageDelay = 0x0268,

            MechanicalBrakeReleaseDelay = 0x0267,

            MotionPolarity = 0x004F,

            MotionResolution = 0x004E,

            MotionScalingConfiguration = 0x002D,

            MotionUnit = 0x004D,

            MotorData = 0x1082,

            MotorDataSource = 0x0521,

            MotorDeviceCode = 0x0522,

            MotorFeedbackConfiguration = 0x1083,

            MotorFeedbackInterpolationFactor = 0x1084,

            MotorFeedbackResolution = 0x1085,

            MotorFeedbackType = 0x1086,

            MotorFeedbackUnit = 0x1087,

            MotorID = 0x1088,

            MotorInertia = 0x1089,

            MotorIntegralThermalSwitch = 0x052B,

            MotorMaxWindingTemperature = 0x052C,

            MotorOverloadAction = 0x0286,

            MotorOverloadLimit = 0x052A,

            MotorOverspeedUserLimit = 0x02B7,

            MotorPhaseLossLimit = 0x02B6,

            MotorPolarity = 0x0525,

            MotorRatedContinuousCurrent = 0x0527,

            MotorRatedOutputPower = 0x0529,

            MotorRatedPeakCurrent = 0x0528,

            MotorRatedVoltage = 0x0526,

            MotorThermalFaultAction = 0x1090,

            MotorThermalOverloadUserLimit = 0x02B9,

            MotorType = 0x0523,

            MotorWindingToAmbientCapacitance = 0x052D,

            MotorWindingToAmbientResistance = 0x052E,

            OutputLPFilterBandwidth = 0x1091,

            OutputLimit = 0x1092,

            OutputNotchFilterFrequency = 0x1093,

            OutputOffset = 0x1094,

            OvertorqueLimit = 0x01FC,

            OvertorqueLimitTime = 0x01FD,

            PMMotorExtendedSpeedPermissive = 0x054B,

            PMMotorForceConstant = 0x053F,

            PMMotorInductance = 0x0530,

            PMMotorLdInductance = 0x054A,

            PMMotorLinearBusOvervoltageSpeed = 0x054E,

            PMMotorLinearMaxExtendedSpeed = 0x054F,

            PMMotorLinearVoltageConstant = 0x0540,

            PMMotorLqInductance = 0x0549,

            PMMotorRatedForce = 0x053E,

            PMMotorRatedTorque = 0x053B,

            PMMotorResistance = 0x052F,

            PMMotorRotaryBusOvervoltageSpeed = 0x054C,

            PMMotorRotaryMaxExtendedSpeed = 0x054D,

            PMMotorRotaryVoltageConstant = 0x053D,

            PMMotorTorqueConstant = 0x053C,

            PWMFrequencySelect = 0x1095,

            PhaseLossFaultAction = 0x1096,

            PositionDataScaling = 0x1097,

            PositionDataScalingExp = 0x1098,

            PositionDataScalingFactor = 0x1099,

            PositionDifferentialGain = 0x1100,

            PositionErrorFaultAction = 0x1101,

            PositionErrorTolerance = 0x01BC,

            PositionErrorToleranceTime = 0x01BD,

            PositionIntegralGain = 0x1102,

            PositionIntegratorBandwidth = 0x01BA,

            PositionIntegratorControl = 0x01BE,

            PositionIntegratorPreload = 0x01BF,

            PositionLeadLagFilterBandwidth = 0x030D,

            PositionLeadLagFilterGain = 0x030E,

            PositionLockTolerance = 0x01BB,

            PositionLoopBandwidth = 0x01B9,

            PositionNotchFilterFrequency = 0x030F,

            PositionPolarity = 0x1103,

            PositionProportionalGain = 0x1104,

            PositionScalingDenominator = 0x0049,

            PositionScalingNumerator = 0x0048,

            PositionServoBandwidth = 0x00C5,

            PositionTrim = 0x01AF,

            PositionUnwind = 0x0054,

            PositionUnwindDenominator = 0x004B,

            PositionUnwindNumerator = 0x004A,

            PowerLossAction = 0x0273,

            PowerLossThreshold = 0x0274,

            PowerLossTime = 0x0276,

            PowerSupplyID = 0x1105,

            PrimaryOperationMode = 0x1106,

            ProgrammedStopMode = 0x0075,

            ProvingConfiguration = 0x024E,

            RampAcceleration = 0x0178,

            RampDeceleration = 0x0179,

            RampJerkControl = 0x017A,

            RampVelocityNegative = 0x0177,

            RampVelocityPositive = 0x0176,

            ReactiveCurrentCommand = 0x082C,

            ReactiveCurrentRateLimit = 0x0831,

            ReactivePowerControl = 0x07D2,

            ReactivePowerRateLimit = 0x0819,

            ReactivePowerSetPoint = 0x0816,

            RegistrationInputs = 0x0164,

            ResistiveBrakeContactDelay = 0x0265,

            RotaryAxis = 0x1107,

            RotaryMotorDampingCoefficient = 0x0535,

            RotaryMotorFanCoolingDerating = 0x0909,

            RotaryMotorFanCoolingSpeed = 0x0907,

            RotaryMotorInertia = 0x046A,

            RotaryMotorMaxSpeed = 0x0534,

            RotaryMotorPoles = 0x0531,

            RotaryMotorRatedSpeed = 0x0533,

            RotationalPosResolution = 0x1108,

            RunBoost = 0x0242,

            SLATConfiguration = 0x0341,

            SLATSetPoint = 0x0342,

            SLATTimeDelay = 0x0343,

            SSIClockFrequency = 0x1109,

            SSICodeType = 0x1110,

            SSIDataLength = 0x1111,

            SafeStoppingAction = 0x02FE,

            SafeStoppingActionSource = 0x02FF,

            SafeTorqueOffAction = 0x02FD,

            SafeTorqueOffActionSource = 0x02F7,

            SafetyFaultAction = 0x02F6,

            ScalingSource = 0x0046,

            ServoFeedbackType = 0x1112,

            ServoLoopConfiguration = 0x1113,

            ServoPolarityBits = 0x1114,

            ShutdownAction = 0x0275,

            SkipSpeed1 = 0x0172,

            SkipSpeed2 = 0x0173,

            SkipSpeed3 = 0x0174,

            SkipSpeedBand = 0x0175,

            SoftOvertravelFaultAction = 0x1115,

            SoftTravelLimitChecking = 0x005C,

            SoftTravelLimitNegative = 0x005E,

            SoftTravelLimitPositive = 0x005D,

            StartBoost = 0x0241,

            StoppingAction = 0x0262,

            StoppingTimeLimit = 0x0264,

            StoppingTorque = 0x0263,

            SystemBandwidth = 0x00A9,

            SystemCapacitance = 0x082A,

            SystemDamping = 0x00CC,

            SystemInertia = 0x01F0,

            TelegramType = 0x1116,

            TestIncrement = 0x1117,

            TorqueCommand = 0x1118,

            TorqueDataScaling = 0x1119,

            TorqueDataScalingExp = 0x1120,

            TorqueDataScalingFactor = 0x1121,

            TorqueIntegralTimeConstant = 0x022B,

            TorqueLeadLagFilterBandwidth = 0x033B,

            TorqueLeadLagFilterGain = 0x033C,

            TorqueLimitBipolar = 0x1122,

            TorqueLimitNegative = 0x01F9,

            TorqueLimitPositive = 0x01F8,

            TorqueLoopBandwidth = 0x022A,

            TorqueLowPassFilterBandwidth = 0x01F6,

            TorqueNotchFilterFrequency = 0x01F7,

            TorqueNotchFilterHighFrequencyLimit = 0x0345,

            TorqueNotchFilterLowFrequencyLimit = 0x0346,

            TorqueNotchFilterTuningThreshold = 0x0347,

            TorqueOffset = 0x00E8,

            TorquePolarity = 0x1123,

            TorqueProveCurrent = 0x024F,

            TorqueRateLimit = 0x01FA,

            TorqueScaling = 0x1124,

            TorqueThreshold = 0x01FB,

            TorqueTrim = 0x01EB,

            TotalDCBusCapacitance = 0x0420,

            TotalInertia = 0x00CE,

            TotalMass = 0x00CF,

            TransmissionRatioInput = 0x055B,

            TransmissionRatioOutput = 0x055C,

            TravelMode = 0x0047,

            TravelRange = 0x004C,

            TuneFriction = 0x00BB,

            TuneInertiaMass = 0x00BA,

            TuneLoadOffset = 0x00BC,

            TuningConfigurationBits = 0x1125,

            TuningDirection = 0x00BF,

            TuningSelect = 0x00BE,

            TuningSpeed = 0x00C2,

            TuningTorque = 0x00C3,

            TuningTravelLimit = 0x00C1,

            UndertorqueLimit = 0x01FE,

            UndertorqueLimitTime = 0x01FF,

            VelocityDataScaling = 0x1126,

            VelocityDataScalingExp = 0x1127,

            VelocityDataScalingFactor = 0x1128,

            VelocityDroop = 0x01D0,

            VelocityErrorTolerance = 0x01D1,

            VelocityErrorToleranceTime = 0x01D2,

            VelocityFeedforwardGain = 0x01B8,

            VelocityIntegralGain = 0x1129,

            VelocityIntegratorBandwidth = 0x01CE,

            VelocityIntegratorControl = 0x01D3,

            VelocityIntegratorPreload = 0x01D4,

            VelocityLimitBipolar = 0x1130,

            VelocityLimitNegative = 0x01DA,

            VelocityLimitPositive = 0x01D9,

            VelocityLockTolerance = 0x01D7,

            VelocityLoopBandwidth = 0x01CD,

            VelocityLowPassFilterBandwidth = 0x01D5,

            VelocityNegativeFeedforwardGain = 0x0316,

            VelocityOffset = 0x00E7,

            VelocityPolarity = 0x1131,

            VelocityProportionalGain = 0x1132,

            VelocityScaling = 0x1133,

            VelocityServoBandwidth = 0x00C6,

            VelocityStandstillWindow = 0x01D8,

            VelocityThreshold = 0x01D6,

            VelocityTrim = 0x01C3,

            VelocityWindow = 0x1134,

            VerticalLoadControl = 0x024D,

            ZeroSpeed = 0x0260,

            ZeroSpeedTime = 0x0261,

            WatchPosition = 0x0036,
        }

        public enum SSVAxis
        {
            ACLineCurrentUnbalanceLimit = 0x07F2,

            ACLineFrequencyChangeAction = 0x08C5,

            ACLineFrequencyChangeThreshold = 0x08C6,

            ACLineFrequencyChangeTime = 0x08C7,

            ACLineHighFreqUserLimit = 0x08EC,

            ACLineHighFreqUserLimitAlternate = 0x08EE,

            ACLineLowFreqUserLimit = 0x08ED,

            ACLineLowFreqUserLimitAlternate = 0x08EF,

            ACLineOverloadUserLimit = 0x08E8,

            ACLineOvervoltageUserLimit = 0x1028,

            ACLineOvervoltageUserLimitAlternate = 0x08EA,

            ACLineResonanceUserLimit = 0x1029,

            ACLineSourceImpedance = 0x07F9,

            ACLineSourceImpedanceAlternate = 0x07FB,

            ACLineSourcePower = 0x07FA,

            ACLineSourcePowerAlternate = 0x07FC,

            ACLineSourceSelect = 0x07F8,

            ACLineSyncErrorTolerance = 0x07F3,

            ACLineSyncLossAction = 0x08C8,

            ACLineSyncLossTime = 0x08C9,

            ACLineUndervoltageUserLimit = 0x08E9,

            ACLineUndervoltageUserLimitAlternate = 0x08EB,

            ACLineVoltageSagAction = 0x08C2,

            ACLineVoltageSagThreshold = 0x08C1,

            ACLineVoltageSagTime = 0x08C0,

            ACLineVoltageTimeConstant = 0x07DE,

            ACLineVoltageUnbalanceLimit = 0x07F1,

            AbsoluteFeedbackEnable = 0x1030,

            AbsoluteFeedbackOffset = 0x1031,

            AccelerationDataScaling = 0x1032,

            AccelerationDataScalingExp = 0x1033,

            AccelerationDataScalingFactor = 0x1034,

            AccelerationFeedforwardGain = 0x01CC,

            AccelerationLimit = 0x01E5,

            AccelerationLimitBipolar = 0x1035,

            AccelerationLimitNegative = 0x1036,

            AccelerationLimitPositive = 0x1037,

            AccelerationTrim = 0x01E1,

            ActiveCurrentCommand = 0x082B,

            ActiveCurrentLowPassFilterBandwidth = 0x082E,

            ActiveCurrentNotchFilterFrequency = 0x082F,

            ActiveCurrentRateLimit = 0x0830,

            ActiveCurrentTrim = 0x082D,

            ActuatorDiameter = 0x0560,

            ActuatorDiameterUnit = 0x0561,

            ActuatorLead = 0x055E,

            ActuatorLeadUnit = 0x055F,

            ActuatorType = 0x055D,

            AdaptiveTuningConfiguration = 0x0344,

            AnalogOutput1 = 0x02DE,

            AnalogOutput2 = 0x02DF,

            AutoSagConfiguration = 0x0369,

            AutoSagSlipIncrement = 0x036A,

            AutoSagSlipTimeLimit = 0x036B,

            AutoSagStart = 0x036C,

            AuxFeedbackConfiguration = 0x1038,

            AuxFeedbackInterpolationFactor = 0x1039,

            AuxFeedbackRatio = 0x1040,

            AuxFeedbackResolution = 0x1041,

            AuxFeedbackType = 0x1042,

            AuxFeedbackUnit = 0x1043,

            AverageVelocityTimebase = 0x0051,

            AxisConfiguration = 0x001E,

            AxisInfoSelect1 = 0x1044,

            AxisInfoSelect2 = 0x1045,

            AxisType = 0x1046,

            AxisUpdateSchedule = 0x007C,

            BacklashCompensationWindow = 0x1047,

            BacklashReversalOffset = 0x01A7,

            BacklashStabilizationWindow = 0x011E,

            BrakeEngageDelayTime = 0x1048,

            BrakeProveRampTime = 0x0251,

            BrakeReleaseDelayTime = 0x1049,

            BrakeSlipTolerance = 0x0252,

            BrakeTestTorque = 0x0250,

            BreakFrequency = 0x0240,

            BreakVoltage = 0x023F,

            BusObserverBandwidth = 0x0330,

            BusObserverConfiguration = 0x032F,

            BusObserverIntegratorBandwidth = 0x0331,

            BusRegulatorID = 0x1050,

            BusVoltageErrorTolerance = 0x0811,

            BusVoltageErrorToleranceTime = 0x0812,

            BusVoltageIntegratorBandwidth = 0x080F,

            BusVoltageLoopBandwidth = 0x080E,

            BusVoltageRateLimit = 0x0810,

            BusVoltageReferenceSource = 0x080D,

            BusVoltageSetPoint = 0x080C,

            CIPAxisAlarmLogReset = 0x001C,

            CIPAxisFaultLogReset = 0x001B,

            CoastingTimeLimit = 0x0269,

            CommandTorque = 0x005F,

            CommandUpdateDelayOffset = 0x0168,

            CommutationOffset = 0x0231,

            CommutationOffsetCompensation = 0x0352,

            CommutationPolarity = 0x0233,

            ConfigurationProfile = 0x1051,

            ConnectionLossStoppingAction = 0x026A,

            ContinuousTorqueLimit = 0x1052,

            ControlMode = 0x0028,

            ConversionConstant = 0x0052,

            ConverterACInputFrequency = 0x07EE,

            ConverterACInputPhasing = 0x07EF,

            ConverterACInputVoltage = 0x07F0,

            ConverterConfiguration = 0x0500,

            ConverterControlMode = 0x07D1,

            ConverterCurrentIntegratorBandwidth = 0x08B7,

            ConverterCurrentLimitSource = 0x0837,

            ConverterCurrentLoopBandwidth = 0x08B6,

            ConverterCurrentLoopDamping = 0x0912,

            ConverterCurrentLoopTuningMethod = 0x0911,

            ConverterCurrentVectorLimit = 0x0912,

            ConverterGroundCurrentUserLimit = 0x02C5,

            ConverterHeatsinkOvertempUserLimit = 0x08F0,

            ConverterInputPhaseLossAction = 0x08C3,

            ConverterInputPhaseLossTime = 0x08C4,

            ConverterModelTimeConstant = 0x0419,

            ConverterMotoringPowerLimit = 0x0254,

            ConverterOperativeCurrentLimit = 0x0834,

            ConverterOverloadAction = 0x08DC,

            ConverterOvertemperatureUserLimit = 0x02BC,

            ConverterPreChargeOverloadUserLimit = 0x0399,

            ConverterRegenerativePowerLimit = 0x0272,

            ConverterStartupMethod = 0x07D3,

            ConverterThermalOverloadUserLimit = 0x02BD,

            CurrentDisturbance = 0x0348,

            CurrentVectorLimit = 0x0229,

            DCInjectionBrakeCurrent = 0x0366,

            DCInjectionBrakeTime = 0x0368,

            DampingFactor = 0x00C4,

            DecelerationLimit = 0x01E6,

            DigitalOutputs = 0x02DB,

            DirectCommandVelocity = 0x0069,

            DirectDriveRampRate = 0x1053,

            DirectionalScalingRatio = 0x1054,

            DriveAxisID = 0x1055,

            DriveEnableInputFaultAction = 0x1056,

            DriveFaultAction = 0x1057,

            DriveModelTimeConstant = 0x00C8,

            DrivePolarity = 0x1058,

            DriveResolution = 0x1059,

            DriveScalingBits = 0x1060,

            DriveThermalFaultAction = 0x1061,

            DriveUnit = 0x1062,

            DynamicsConfigurationBits = 0x0078,

            ExternalDCBusCapacitance = 0x0421,

            ExternalDriveType = 0x016B,

            FaultConfigurationBits = 0x1063,

            Feedback1AccelFilterBandwidth = 0x059B,

            Feedback1AccelFilterTaps = 0x0964,

            Feedback1BatteryAbsolute = 0x0965,

            Feedback1CycleInterpolation = 0x0589,

            Feedback1CycleResolution = 0x0588,

            Feedback1DataCode = 0x058D,

            Feedback1DataLength = 0x058C,

            Feedback1Length = 0x058B,

            Feedback1LossAction = 0x0960,

            Feedback1Polarity = 0x0586,

            Feedback1ResolverCableBalance = 0x0591,

            Feedback1ResolverExcitationFrequency = 0x0590,

            Feedback1ResolverExcitationVoltage = 0x058F,

            Feedback1ResolverTransformerRatio = 0x058E,

            Feedback1StartupMethod = 0x0587,

            Feedback1Turns = 0x058A,

            Feedback1Type = 0x0585,

            Feedback1Unit = 0x0583,

            Feedback1VelocityFilterBandwidth = 0x059A,

            Feedback1VelocityFilterTaps = 0x0963,

            Feedback2AccelFilterBandwidth = 0x05CD,

            Feedback2AccelFilterTaps = 0x0996,

            Feedback2BatteryAbsolute = 0x0997,

            Feedback2CycleInterpolation = 0x05BB,

            Feedback2CycleResolution = 0x05BA,

            Feedback2DataCode = 0x05BF,

            Feedback2DataLength = 0x05BE,

            Feedback2Length = 0x05BD,

            Feedback2LossAction = 0x0992,

            Feedback2Polarity = 0x05B8,

            Feedback2ResolverCableBalance = 0x05C3,

            Feedback2ResolverExcitationFrequency = 0x05C2,

            Feedback2ResolverExcitationVoltage = 0x05C1,

            Feedback2ResolverTransformerRatio = 0x05C0,

            Feedback2StartupMethod = 0x05B9,

            Feedback2Turns = 0x05BC,

            Feedback2Type = 0x05B7,

            Feedback2Unit = 0x05B5,

            Feedback2VelocityFilterBandwidth = 0x05CC,

            Feedback2VelocityFilterTaps = 0x0995,

            FeedbackCommutationAligned = 0x00FA,

            FeedbackConfiguration = 0x001F,

            FeedbackDataLossUserLimit = 0x02C4,

            FeedbackFaultAction = 0x1064,

            FeedbackMasterSelect = 0x1065,

            FeedbackMode = 0x002A,

            FeedbackNoiseFaultAction = 0x1066,

            FeedbackNoiseUserLimit = 0x02C2,

            FeedbackSignalLossUserLimit = 0x02C3,

            FeedbackUnitRatio = 0x002C,

            FluxBrakingEnable = 0x0367,

            FluxIntegralTimeConstant = 0x022D,

            FluxLoopBandwidth = 0x022C,

            FluxUpControl = 0x022E,

            FluxUpTime = 0x022F,

            FlyingStartEnable = 0x017C,

            FlyingStartMethod = 0x017D,

            FrictionCompensation = 0x1067,

            FrictionCompensationSliding = 0x01F2,

            FrictionCompensationStatic = 0x01F3,

            FrictionCompensationViscous = 0x01F4,

            FrictionCompensationWindow = 0x0320,

            GainTuningConfigurationBits = 0x00BD,

            HardOvertravelFaultAction = 0x1068,

            HomeConfigurationBits = 0x0058,

            HomeDirection = 0x0056,

            HomeMode = 0x0055,

            HomeOffset = 0x005A,

            HomePosition = 0x0059,

            HomeReturnSpeed = 0x0071,

            HomeSequence = 0x0057,

            HomeSpeed = 0x0070,

            HomeTorqueLevel = 0x1069,

            HookupTestDistance = 0x006D,

            HookupTestFeedbackChannel = 0x006F,

            HookupTestTime = 0x006E,

            InductionMotorFluxCurrent = 0x0542,

            InductionMotorMagnetizationReactance = 0x0545,

            InductionMotorRatedFrequency = 0x0541,

            InductionMotorRatedSlipSpeed = 0x0548,

            InductionMotorRotorLeakageReactance = 0x0547,

            InductionMotorRotorResistance = 0x0546,

            InductionMotorStatorLeakageReactance = 0x0544,

            InductionMotorStatorResistance = 0x0543,

            InhibitAxis = 0x0014,

            InputPowerPhase = 0x1070,

            IntegratorHoldEnable = 0x1071,

            InterpolatedPositionConfiguration = 0x006C,

            InterpolationTime = 0x003B,

            InverterOverloadAction = 0x0287,

            InverterThermalOverloadUserLimit = 0x02BB,

            LDTCalibrationConstant = 0x1072,

            LDTCalibrationConstantUnits = 0x1073,

            LDTLength = 0x1074,

            LDTLengthUnits = 0x1075,

            LDTRecirculations = 0x1076,

            LDTScaling = 0x1077,

            LDTScalingUnits = 0x1078,

            LDTType = 0x1079,

            LinearMotorDampingCoefficient = 0x053A,

            LinearMotorIntegralLimitSwitch = 0x0909,

            LinearMotorMass = 0x0538,

            LinearMotorMaxSpeed = 0x0539,

            LinearMotorPolePitch = 0x0536,

            LinearMotorRatedSpeed = 0x0537,

            LoadInertiaRatio = 0x0160,

            LoadObserverBandwidth = 0x0326,

            LoadObserverConfiguration = 0x0325,

            LoadObserverFeedbackGain = 0x0329,

            LoadObserverIntegratorBandwidth = 0x0327,

            LoadRatio = 0x00CD,

            LoadType = 0x055a,

            MasterInputConfigurationBits = 0x0015,

            MasterPositionFilterBandwidth = 0x0016,

            MaximumAcceleration = 0x0073,

            MaximumAccelerationJerk = 0x0076,

            MaximumDeceleration = 0x0074,

            MaximumDecelerationJerk = 0x0077,

            MaximumFrequency = 0x023D,

            MaximumNegativeTravel = 0x1080,

            MaximumPositiveTravel = 0x1081,

            MaximumSpeed = 0x0072,

            MaximumVoltage = 0x023C,

            MechanicalBrakeControl = 0x0266,

            MechanicalBrakeEngageDelay = 0x0268,

            MechanicalBrakeReleaseDelay = 0x0267,

            MotionPolarity = 0x004F,

            MotionResolution = 0x004E,

            MotionScalingConfiguration = 0x002D,

            MotionUnit = 0x004D,

            MotorData = 0x1082,

            MotorDataSource = 0x0521,

            MotorDeviceCode = 0x0522,

            MotorFeedbackConfiguration = 0x1083,

            MotorFeedbackInterpolationFactor = 0x1084,

            MotorFeedbackResolution = 0x1085,

            MotorFeedbackType = 0x1086,

            MotorFeedbackUnit = 0x1087,

            MotorID = 0x1088,

            MotorInertia = 0x1089,

            MotorIntegralThermalSwitch = 0x052B,

            MotorMaxWindingTemperature = 0x052C,

            MotorOverloadAction = 0x0286,

            MotorOverloadLimit = 0x052A,

            MotorOverspeedUserLimit = 0x02B7,

            MotorPhaseLossLimit = 0x02B6,

            MotorPolarity = 0x0525,

            MotorRatedContinuousCurrent = 0x0527,

            MotorRatedOutputPower = 0x0529,

            MotorRatedPeakCurrent = 0x0528,

            MotorRatedVoltage = 0x0526,

            MotorThermalFaultAction = 0x1090,

            MotorThermalOverloadUserLimit = 0x02B9,

            MotorType = 0x0523,

            MotorWindingToAmbientCapacitance = 0x052D,

            MotorWindingToAmbientResistance = 0x052E,

            OutputLPFilterBandwidth = 0x1091,

            OutputLimit = 0x1092,

            OutputNotchFilterFrequency = 0x1093,

            OutputOffset = 0x1094,

            OvertorqueLimit = 0x01FC,

            OvertorqueLimitTime = 0x01FD,

            PMMotorExtendedSpeedPermissive = 0x054B,

            PMMotorForceConstant = 0x053F,

            PMMotorInductance = 0x0530,

            PMMotorLdInductance = 0x054A,

            PMMotorLinearBusOvervoltageSpeed = 0x054E,

            PMMotorLinearMaxExtendedSpeed = 0x054F,

            PMMotorLinearVoltageConstant = 0x0540,

            PMMotorLqInductance = 0x0549,

            PMMotorRatedForce = 0x053E,

            PMMotorRatedTorque = 0x053B,

            PMMotorResistance = 0x052F,

            PMMotorRotaryBusOvervoltageSpeed = 0x054C,

            PMMotorRotaryMaxExtendedSpeed = 0x054D,

            PMMotorRotaryVoltageConstant = 0x053D,

            PMMotorTorqueConstant = 0x053C,

            PWMFrequencySelect = 0x1095,

            PhaseLossFaultAction = 0x1096,

            PositionDataScaling = 0x1097,

            PositionDataScalingExp = 0x1098,

            PositionDataScalingFactor = 0x1099,

            PositionDifferentialGain = 0x1100,

            PositionErrorFaultAction = 0x1101,

            PositionErrorTolerance = 0x01BC,

            PositionErrorToleranceTime = 0x01BD,

            PositionIntegralGain = 0x1102,

            PositionIntegratorBandwidth = 0x01BA,

            PositionIntegratorControl = 0x01BE,

            PositionIntegratorPreload = 0x01BF,

            PositionLeadLagFilterBandwidth = 0x030D,

            PositionLeadLagFilterGain = 0x030E,

            PositionLockTolerance = 0x01BB,

            PositionLoopBandwidth = 0x01B9,

            PositionNotchFilterFrequency = 0x030F,

            PositionPolarity = 0x1103,

            PositionProportionalGain = 0x1104,

            PositionScalingDenominator = 0x0049,

            PositionScalingNumerator = 0x0048,

            PositionServoBandwidth = 0x00C5,

            PositionTrim = 0x01AF,

            PositionUnwind = 0x0054,

            PositionUnwindDenominator = 0x004B,

            PositionUnwindNumerator = 0x004A,

            PowerLossAction = 0x0273,

            PowerLossThreshold = 0x0274,

            PowerLossTime = 0x0276,

            PowerSupplyID = 0x1105,

            PrimaryOperationMode = 0x1106,

            ProgrammedStopMode = 0x0075,

            ProvingConfiguration = 0x024E,

            RampAcceleration = 0x0178,

            RampDeceleration = 0x0179,

            RampJerkControl = 0x017A,

            RampVelocityNegative = 0x0177,

            RampVelocityPositive = 0x0176,

            ReactiveCurrentCommand = 0x082C,

            ReactiveCurrentRateLimit = 0x0831,

            ReactivePowerControl = 0x07D2,

            ReactivePowerRateLimit = 0x0819,

            ReactivePowerSetPoint = 0x0816,

            RegistrationInputs = 0x0164,

            ResistiveBrakeContactDelay = 0x0265,

            RotaryAxis = 0x1107,

            RotaryMotorDampingCoefficient = 0x0535,

            RotaryMotorFanCoolingDerating = 0x0908,

            RotaryMotorFanCoolingSpeed = 0x0907,

            RotaryMotorInertia = 0x046A,

            RotaryMotorMaxSpeed = 0x0534,

            RotaryMotorPoles = 0x0531,

            RotaryMotorRatedSpeed = 0x0533,

            RotationalPosResolution = 0x1108,

            RunBoost = 0x0242,

            SLATConfiguration = 0x0341,

            SLATSetPoint = 0x0342,

            SLATTimeDelay = 0x0343,

            SSIClockFrequency = 0x1109,

            SSICodeType = 0x1110,

            SSIDataLength = 0x1111,

            SafeStoppingAction = 0x02FE,

            SafeStoppingActionSource = 0x02FF,

            SafeTorqueOffAction = 0x02FD,

            SafeTorqueOffActionSource = 0x02F7,

            SafetyFaultAction = 0x02F6,

            ScalingSource = 0x0046,

            ServoFeedbackType = 0x1112,

            ServoLoopConfiguration = 0x1113,

            ServoPolarityBits = 0x1114,

            ShutdownAction = 0x0275,

            SkipSpeed1 = 0x0172,

            SkipSpeed2 = 0x0173,

            SkipSpeed3 = 0x0174,

            SkipSpeedBand = 0x0175,

            SoftOvertravelFaultAction = 0x1115,

            SoftTravelLimitChecking = 0x005C,

            SoftTravelLimitNegative = 0x005E,

            SoftTravelLimitPositive = 0x005D,

            StartBoost = 0x0241,

            StoppingAction = 0x0262,

            StoppingTimeLimit = 0x0264,

            StoppingTorque = 0x0263,

            SystemBandwidth = 0x00A9,

            SystemCapacitance = 0x082A,

            SystemDamping = 0x00CC,

            SystemInertia = 0x01F0,

            TelegramType = 0x1116,

            TestIncrement = 0x1117,

            TorqueCommand = 0x1118,

            TorqueDataScaling = 0x1119,

            TorqueDataScalingExp = 0x1120,

            TorqueDataScalingFactor = 0x1121,

            TorqueIntegralTimeConstant = 0x022B,

            TorqueLeadLagFilterBandwidth = 0x033B,

            TorqueLeadLagFilterGain = 0x033C,

            TorqueLimitBipolar = 0x1122,

            TorqueLimitNegative = 0x01F9,

            TorqueLimitPositive = 0x01F8,

            TorqueLoopBandwidth = 0x022A,

            TorqueLowPassFilterBandwidth = 0x01F6,

            TorqueNotchFilterFrequency = 0x01F7,

            TorqueNotchFilterHighFrequencyLimit = 0x0345,

            TorqueNotchFilterLowFrequencyLimit = 0x0346,

            TorqueNotchFilterTuningThreshold = 0x0347,

            TorqueOffset = 0x00E8,

            TorquePolarity = 0x1123,

            TorqueProveCurrent = 0x024F,

            TorqueRateLimit = 0x01FA,

            TorqueScaling = 0x1124,

            TorqueThreshold = 0x01FB,

            TorqueTrim = 0x01EB,

            TotalDCBusCapacitance = 0x0420,

            TotalInertia = 0x00CE,

            TotalMass = 0x00CF,

            TransmissionRatioInput = 0x055B,

            TransmissionRatioOutput = 0x055C,

            TravelMode = 0x0047,

            TravelRange = 0x004C,

            TuneFriction = 0x00BB,

            TuneInertiaMass = 0x00BA,

            TuneLoadOffset = 0x00BC,

            TuningConfigurationBits = 0x1125,

            TuningDirection = 0x00BF,

            TuningSelect = 0x00BE,

            TuningSpeed = 0x00C2,

            TuningTorque = 0x00C3,

            TuningTravelLimit = 0x00C1,

            UndertorqueLimit = 0x01FE,

            UndertorqueLimitTime = 0x01FF,

            VelocityDataScaling = 0x1126,

            VelocityDataScalingExp = 0x1127,

            VelocityDataScalingFactor = 0x1128,

            VelocityDroop = 0x01D0,

            VelocityErrorTolerance = 0x01D1,

            VelocityErrorToleranceTime = 0x01D2,

            VelocityFeedforwardGain = 0x01B8,

            VelocityIntegralGain = 0x1129,

            VelocityIntegratorBandwidth = 0x01CE,

            VelocityIntegratorControl = 0x01D3,

            VelocityIntegratorPreload = 0x01D4,

            VelocityLimitBipolar = 0x1130,

            VelocityLimitNegative = 0x01DA,

            VelocityLimitPositive = 0x01D9,

            VelocityLockTolerance = 0x01D7,

            VelocityLoopBandwidth = 0x01CD,

            VelocityLowPassFilterBandwidth = 0x01D5,

            VelocityNegativeFeedforwardGain = 0x0316,

            VelocityOffset = 0x00E7,

            VelocityPolarity = 0x1131,

            VelocityProportionalGain = 0x1132,

            VelocityScaling = 0x1133,

            VelocityServoBandwidth = 0x00C6,

            VelocityStandstillWindow = 0x01D8,

            VelocityThreshold = 0x01D6,

            VelocityTrim = 0x01C3,

            VelocityWindow = 0x1134,

            VerticalLoadControl = 0x024D,

            ZeroSpeed = 0x0260,

            ZeroSpeedTime = 0x0261,

            //PositionLoopProportionalGain = 0x01b9,
            //PositionLoopIntegratorGain = 0x01ba,
        }

        public enum Controller
        {
            AuditValue,
            CanUseRPIFromProducer,

            ChangesToDetect,

            ControllerLogExecutionModificationCount,

            ControllerLogTotalEntryCount,
            ControllerLogUnsavedEntryCount,
            DataTablePadPercentage,

            IgnoreArrayFaultsDuringPostScan,

            InhibitAutomaticFirmwareUpdate,
            KeepTestEditsOnSwitchOver,
            Name,
            RedundancyEnabled,

            ShareUnusedTimeSlice,

            TimeSlice,
            Rand=128
        }

        public enum SSVController
        {
            ChangesToDetect,

            ControllerLogExecutionModificationCount,

            ControllerLogTotalEntryCount,

            IgnoreArrayFaultsDuringPostScan,

            InhibitAutomaticFirmwareUpdate,

            ShareUnusedTimeSlice,

            TimeSlice,
        }

        public enum ControllerDevice
        {
            [EnumMember(Value = "DeviceName")]
            DeviceName,
            [EnumMember(Value = "ProductCode")]
            ProductCode,
            [EnumMember(Value = "ProductRev")]
            ProductRev,
            [EnumMember(Value = "SerialNumber")]
            SerialNumber,
            [EnumMember(Value = "Status")]
            Status,
            [EnumMember(Value = "Type")]
            Type,
            [EnumMember(Value = "Vendor")]
            Vendor,
        }

        public enum CoordinateSystem
        {
            ActiveToolFrameID = 0x0000,
            ActiveToolFrameOffset = 0x0001,
            ActiveWorkFrameID = 0x0002,
            ActiveWorkFrameOffset = 0x0003,

            ActualPositionTolerance = 0x0004,

            BallScrewPitch = 0x0005,

            BaseOffset1 = 0x0006,

            BaseOffset2 = 0x0007,

            BaseOffset3 = 0x0008,

            CommandPositionTolerance = 0x0009,
            CoordinateDefinition = 0x000A,

            CoordinateSystemAutoTagUpdate = 0x000B,

            DynamicsConfigurationBits = 0x0078,

            EndEffectorOffset1 = 0x000D,

            EndEffectorOffset2 = 0x000E,

            EndEffectorOffset3 = 0x000F,

            LinkLength1 = 0x0010,

            LinkLength2 = 0x0011,

            LinkLength3 = 0x0012,

            MasterInputConfigurationBits = 0x0013,

            MasterPositionFilterBandwidth = 0x0014,

            MaximumAcceleration = 0x0073,

            MaximumAccelerationJerk = 0x0076,

            MaximumDeceleration = 0x0074,

            MaximumDecelerationJerk = 0x0077,

            MaximumOrientationAcceleration = 0x0019,

            MaximumOrientationDeceleration = 0x001A,

            MaximumOrientationSpeed = 0x001B,
            MaximumPendingMoves = 0x001C,

            MaximumSpeed = 0x0072,
            RobotConfiguration = 0x001E,

            SwingArmA3 = 0x001F,

            SwingArmA4 = 0x0020,

            SwingArmCouplingDirection = 0x0021,

            SwingArmCouplingRatioDenominator = 0x0022,

            SwingArmCouplingRatioNumerator = 0x0023,

            SwingArmD3 = 0x0024,

            SwingArmD4 = 0x0025,

            SwingArmD5 = 0x0026,
            TurnsCounters = 0x0027,

            ZeroAngleOffset1 = 0x0028,

            ZeroAngleOffset2 = 0x0029,

            ZeroAngleOffset3 = 0x002A,

            ZeroAngleOffset4 = 0x002B,

            ZeroAngleOffset5 = 0x002C,

            ZeroAngleOffset6 = 0x002D,
        }

        public enum SSVCoordinateSystem
        {
            ActualPositionTolerance = 0x0000,

            BallScrewPitch = 0x0001,

            BaseOffset1 = 0x0002,

            BaseOffset2 = 0x0003,

            BaseOffset3 = 0x0004,

            CommandPositionTolerance = 0x0005,

            CoordinateSystemAutoTagUpdate = 0x0006,

            DynamicsConfigurationBits = 0x0078,

            EndEffectorOffset1 = 0x0008,

            EndEffectorOffset2 = 0x0009,

            EndEffectorOffset3 = 0x000A,

            LinkLength1 = 0x000B,

            LinkLength2 = 0x000C,

            LinkLength3 = 0x000D,
            MasterInputConfigurationBits = 0x000E,
            MasterPositionFilterBandwidth = 0x000F,
            MaximumAcceleration = 0x0073,
            MaximumAccelerationJerk = 0x0076,
            MaximumDeceleration = 0x0074,
            MaximumDecelerationJerk = 0x0077,
            MaximumOrientationAcceleration = 0x0014,
            MaximumOrientationDeceleration = 0x0015,
            MaximumOrientationSpeed = 0x0016,
            MaximumSpeed = 0x0072,
            SwingArmA3 = 0x0018,
            SwingArmA4 = 0x0019,
            SwingArmCouplingDirection = 0x001A,
            SwingArmCouplingRatioDenominator = 0x001B,
            SwingArmCouplingRatioNumerator = 0x001C,
            SwingArmD3 = 0x001D,
            SwingArmD4 = 0x001E,
            SwingArmD5 = 0x001F,
            ZeroAngleOffset1 = 0x0020,
            ZeroAngleOffset2 = 0x0021,
            ZeroAngleOffset3 = 0x0022,
            ZeroAngleOffset4 = 0x0023,
            ZeroAngleOffset5 = 0x0024,
            ZeroAngleOffset6 = 0x0025,
        }

        public enum CST
        {
            CurrentStatus,
            CurrentValue,
        }

        public enum DataLog
        {
            CollectionStatus,
            CurrentCaptureNumber,
            DataCapturesToKeep,
            Enabled,
            Fault,
            PreviousCaptureUsedStorage,
            ProcessingBandwidth,
            ReservedStorage,
            UsedStorage,
        }

        public enum DF1
        {
            ACKTimeout,
            DiagnosticCounters,
            DuplicateDetection,
            ENQTransmitLimit,
            EOTSuppression,
            EmbeddedResponseEnable,
            EnableStoreFwd,
            ErrorDetection,
            MasterMessageTransmit,
            MaxStationAddress,
            NAKReceiveLimit,
            NormalPollGroupSize,
            PollingMode,
            ReplyMessageWait,
            SlavePollTimeout,
            StationAddress,
            TokenHoldFactor,
            TransmitRetries,
        }

        public enum SSVDF1
        {
            PendingACKTimeout,
            PendingDiagnosticCounters,
            PendingDuplicateDetection,
            PendingENQTransmitLimit,
            PendingEOTSuppression,
            PendingEmbeddedResponseEnable,
            PendingEnableStoreFwd,
            PendingErrorDetection,
            PendingMasterMessageTransmit,
            PendingMaxStationAddress,
            PendingNAKReceiveLimit,
            PendingNormalPollGroupSize,
            PendingPollingMode,
            PendingReplyMessageWait,
            PendingSlavePollTimeout,
            PendingStationAddress,
            PendingTokenHoldFactor,
            PendingTransmitRetries,
        }

        public enum FaultLog
        {
            MajorEvents,
            MajorFaultBits,
            MinorEvents,
            MinorFaultBits,
        }

        public enum Message
        {
            ConnectionPath,
            ConnectionRate,
            MessageType,
            Port,
            TimeoutMultiplier,
            UnconnectedTimeout,
        }

        public enum Module
        {
            EntryStatus,
            FaultCode,
            FaultInfo,
            FirmwareSupervisorStatus,
            ForceStatus,
            INSTANCE,
            LedStatus,
            Mode,
            Path,
        }

        public enum SSVModule
        {
            Mode,
        }

        public enum MotionGroup
        {
            Alternate1UpdateMultiplier,
            Alternate1UpdatePeriod,
            Alternate2UpdateMultiplier,
            Alternate2UpdatePeriod,
            AutoTagUpdate,
            CoarseUpdatePeriod,
            CycleStartTime,
            INSTANCE,
            MaximumInterval,
            MinimumInterval,
            StartTime,
            TaskAverageIOTime,
            TaskAverageScanTime,
            TaskLastIOTime,
            TaskLastScanTime,
            TaskMaximumIOTime,
            TaskMaximumScanTime,
            TimeOffset,
            TimingModel,
        }

        public enum SSVMotionGroup
        {
            AutoTagUpdate,
            MaximumInterval,
            TaskAverageIOTime,
            TaskAverageScanTime,
            TaskLastIOTime,
            TaskLastScanTime,
        }

        public enum Program
        {
            DisableFlag,
            Instance,
            LASTSCANTIME,
            MAXSCANTIME,
            MajorFaultRecord,
            MinorFaultRecord,
            Name,
        }

        public enum SSVProgram
        {
            DisableFlag,
            LASTSCANTIME,
            MAXSCANTIME,
            MajorFaultRecord,
            MinorFaultRecord,
        }

        public enum Routine
        {
            INSTANCE,
            Name,
            SFCPaused,
            SFCResuming,
        }

        public enum SSVRoutine
        {
            SFCResuming,
        }

        public enum SerialPort
        {
            BaudRate,
            ComDriverId,
            DCDDelay,
            DataBits,
            Parity,
            RTSOffDelay,
            RTSSendDelay,
            StopBits,
        }

        public enum SSVSerialPort
        {
            PendingBaudRate,
            PendingComDriverId,
            PendingDCDDelay,
            PendingDataBits,
            PendingParity,
            PendingRTSOffDelay,
            PendingRTSSendDelay,
            PendingStopBits,
        }

        public enum Task
        {
            DisableUpdateOutputs,
            EnableTimeout,
            InhibitTask,
            Instance,
            LastScanTime,
            MaxInterval,
            MaxScanTime,
            MinInterval,
            Name,
            OverlapCount,
            Priority,
            Rate,
            StartTime,
            Status,
            Watchdog,
        }

        public enum TimeSynchronize
        {
            ClockType,
            CurrentTimeMicroseconds,
            CurrentTimeNanoseconds,
            DomainNumber,
            GrandMasterClockInfo,
            IsSynchronized,
            LocalClockInfo,
            ManufactureIdentity,
            MaxOffsetFromMaster,
            MeanPathDelayToMaster,
            NumberOfPorts,
            OffsetFromMaster,
            PTPEnable,
            ParentClockInfo,
            PortEnableInfo,
            PortLogAnnounceIntervalInfo,
            PortLogSyncIntervalInfo,
            PortPhysicalAddressInfo,
            PortProfileIdentityInfo,
            PortProtocolAddressInfo,
            PortStateInfo,
            Priority1,
            Priority2,
            ProductDescription,
            RevisionData,
            StepsRemoved,
            SystemTimeAndOffset,
            UserDescription,
        }

        public enum SSVTimeSynchronize
        {
            MaxOffsetFromMaster,
            PTPEnable,
            Priority1,
            Priority2,
        }

        public enum WallClockTime
        {
            ApplyDST,
            CSTOffset,
            CurrentValue,
            DSTAdjustment,
            DateTime,
            LocalDateTime,
            TimeZoneString,
        }

        public enum ModuleAttr
        {
            EntryStatus,
            FaultCode,
            FirmwareSupervisorStatus,
            ForceStatus,
            INSTANCE,
            LedStatus,
            Mode,
            Path
        }

        public enum SSVModuleAttr
        {
            Mode
        }
    }
}
