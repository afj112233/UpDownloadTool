using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace ICSStudio.FileConverter.JsonToL5X.Model
{
    [GeneratedCode("xsd", "4.6.81.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class AxisType
    {
        [XmlAttribute] public string MotionGroup { get; set; }

        [XmlAttribute] public string MotionModule { get; set; }

        [XmlAttribute] public AxisConfigurationEnum AxisConfiguration { get; set; }

        [XmlIgnore] public bool AxisConfigurationSpecified { get; set; }

        [XmlAttribute] public AxisFeedbackConfigurationEnum FeedbackConfiguration { get; set; }

        [XmlIgnore] public bool FeedbackConfigurationSpecified { get; set; }

        [XmlAttribute] public AxisMotorDataSourceEnum MotorDataSource { get; set; }

        [XmlIgnore]
        public bool MotorDataSourceSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.FeedbackOnly ||
                    AxisConfiguration == AxisConfigurationEnum.ConverterOnly)
                    return false;

                return true;
            }
        }

        [XmlAttribute] public string MotorCatalogNumber { get; set; }

        [XmlIgnore]
        public bool MotorCatalogNumberSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.FeedbackOnly ||
                    AxisConfiguration == AxisConfigurationEnum.ConverterOnly)
                    return false;

                return true;
            }
        }

        [XmlAttribute] public AxisFeedbackTypeEnum Feedback1Type { get; set; }

        [XmlIgnore] public bool Feedback1TypeSpecified { get; set; }

        [XmlAttribute] public AxisMotorTypeEnum MotorType { get; set; }

        [XmlIgnore]
        public bool MotorTypeSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.FeedbackOnly ||
                    AxisConfiguration == AxisConfigurationEnum.ConverterOnly)
                    return false;

                return true;
            }
        }

        [XmlAttribute] public AxisMotionScalingConfigurationEnum MotionScalingConfiguration { get; set; }

        [XmlIgnore] public bool MotionScalingConfigurationSpecified { get; set; }

        [XmlAttribute] public float ConversionConstant { get; set; }

        [XmlIgnore] public bool ConversionConstantSpecified { get; set; }

        [XmlAttribute] public ulong OutputCamExecutionTargets { get; set; }

        [XmlIgnore] public bool OutputCamExecutionTargetsSpecified { get; set; }

        [XmlAttribute] public string PositionUnits { get; set; }

        [XmlAttribute] public float AverageVelocityTimebase { get; set; }

        [XmlIgnore] public bool AverageVelocityTimebaseSpecified { get; set; }

        [XmlAttribute] public RotaryAxisEnum RotaryAxis { get; set; }

        [XmlIgnore] public bool RotaryAxisSpecified { get; set; }

        [XmlAttribute] public ulong PositionUnwind { get; set; }

        [XmlIgnore]
        public bool PositionUnwindSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.ConverterOnly ||
                    AxisConfiguration == AxisConfigurationEnum.FrequencyControl)
                    return false;

                return true;
            }
        }

        [XmlAttribute] public AxisHomeModeEnum HomeMode { get; set; }

        [XmlIgnore] public bool HomeModeSpecified { get; set; }

        [XmlAttribute] public AxisHomeDirEnum HomeDirection { get; set; }

        [XmlIgnore] public bool HomeDirectionSpecified { get; set; }

        [XmlAttribute] public AxisHomeSeqEnum HomeSequence { get; set; }

        [XmlIgnore] public bool HomeSequenceSpecified { get; set; }

        [XmlAttribute] public string HomeConfigurationBits { get; set; }

        [XmlAttribute] public float HomePosition { get; set; }

        [XmlIgnore] public bool HomePositionSpecified { get; set; }

        [XmlAttribute] public float HomeOffset { get; set; }

        [XmlIgnore] public bool HomeOffsetSpecified { get; set; }

        [XmlAttribute] public float HomeSpeed { get; set; }

        [XmlIgnore] public bool HomeSpeedSpecified { get; set; }

        [XmlAttribute] public float HomeReturnSpeed { get; set; }

        [XmlIgnore] public bool HomeReturnSpeedSpecified { get; set; }

        [XmlAttribute] public float MaximumSpeed { get; set; }

        [XmlIgnore] public bool MaximumSpeedSpecified { get; set; }

        [XmlAttribute] public float MaximumAcceleration { get; set; }

        [XmlIgnore] public bool MaximumAccelerationSpecified { get; set; }

        [XmlAttribute] public float MaximumDeceleration { get; set; }

        [XmlIgnore] public bool MaximumDecelerationSpecified { get; set; }

        [XmlAttribute] public AxisProgStopModeEnum ProgrammedStopMode { get; set; }

        [XmlIgnore] public bool ProgrammedStopModeSpecified { get; set; }

        [XmlAttribute] public ulong MasterInputConfigurationBits { get; set; }

        [XmlIgnore] public bool MasterInputConfigurationBitsSpecified { get; set; }

        [XmlAttribute] public float MasterPositionFilterBandwidth { get; set; }

        [XmlIgnore] public bool MasterPositionFilterBandwidthSpecified { get; set; }

        [XmlAttribute] public float VelocityFeedforwardGain { get; set; }

        [XmlIgnore] public bool VelocityFeedforwardGainSpecified { get; set; }

        [XmlAttribute] public float AccelerationFeedforwardGain { get; set; }

        [XmlIgnore] public bool AccelerationFeedforwardGainSpecified { get; set; }

        [XmlAttribute] public float PositionErrorTolerance { get; set; }

        [XmlIgnore] public bool PositionErrorToleranceSpecified { get; set; }

        [XmlAttribute] public float PositionLockTolerance { get; set; }

        [XmlIgnore] public bool PositionLockToleranceSpecified { get; set; }

        [XmlAttribute] public float VelocityOffset { get; set; }

        [XmlIgnore] public bool VelocityOffsetSpecified { get; set; }

        [XmlAttribute] public float TorqueOffset { get; set; }

        [XmlIgnore]
        public bool TorqueOffsetSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.PositionLoop ||
                    AxisConfiguration == AxisConfigurationEnum.VelocityLoop ||
                    AxisConfiguration == AxisConfigurationEnum.TorqueLoop)
                    return true;

                return false;
            }
        }

        [XmlAttribute] public float FrictionCompensationWindow { get; set; }

        [XmlIgnore] public bool FrictionCompensationWindowSpecified { get; set; }

        [XmlAttribute] public float BacklashReversalOffset { get; set; }

        [XmlIgnore] public bool BacklashReversalOffsetSpecified { get; set; }

        [XmlAttribute] public float TuningTravelLimit { get; set; }

        [XmlIgnore]
        public bool TuningTravelLimitSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.PositionLoop ||
                    AxisConfiguration == AxisConfigurationEnum.VelocityLoop ||
                    AxisConfiguration == AxisConfigurationEnum.TorqueLoop)
                    return true;

                return false;
            }
        }

        [XmlAttribute] public float TuningSpeed { get; set; }

        [XmlIgnore]
        public bool TuningSpeedSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.PositionLoop ||
                    AxisConfiguration == AxisConfigurationEnum.VelocityLoop ||
                    AxisConfiguration == AxisConfigurationEnum.TorqueLoop)
                    return true;

                return false;
            }
        }

        [XmlAttribute] public float TuningTorque { get; set; }

        [XmlIgnore]
        public bool TuningTorqueSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.PositionLoop ||
                    AxisConfiguration == AxisConfigurationEnum.VelocityLoop ||
                    AxisConfiguration == AxisConfigurationEnum.TorqueLoop)
                    return true;

                return false;
            }
        }

        [XmlAttribute] public float DampingFactor { get; set; }

        [XmlIgnore]
        public bool DampingFactorSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.PositionLoop ||
                    AxisConfiguration == AxisConfigurationEnum.VelocityLoop ||
                    AxisConfiguration == AxisConfigurationEnum.TorqueLoop)
                    return true;

                return false;
            }
        }

        [XmlAttribute] public float DriveModelTimeConstant { get; set; }

        [XmlIgnore]
        public bool DriveModelTimeConstantSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.PositionLoop ||
                    AxisConfiguration == AxisConfigurationEnum.VelocityLoop ||
                    AxisConfiguration == AxisConfigurationEnum.TorqueLoop)
                    return true;

                return false;
            }
        }

        [XmlAttribute] public float PositionServoBandwidth { get; set; }

        [XmlIgnore] public bool PositionServoBandwidthSpecified { get; set; }

        [XmlAttribute] public float VelocityServoBandwidth { get; set; }

        [XmlIgnore] public bool VelocityServoBandwidthSpecified { get; set; }

        [XmlAttribute] public float VelocityDroop { get; set; }

        [XmlIgnore] public bool VelocityDroopSpecified { get; set; }

        [XmlAttribute] public float VelocityLimitPositive { get; set; }

        [XmlIgnore] public bool VelocityLimitPositiveSpecified { get; set; }

        [XmlAttribute] public float VelocityLimitNegative { get; set; }

        [XmlIgnore] public bool VelocityLimitNegativeSpecified { get; set; }

        [XmlAttribute] public float VelocityThreshold { get; set; }

        [XmlIgnore] public bool VelocityThresholdSpecified { get; set; }

        [XmlAttribute] public float VelocityStandstillWindow { get; set; }

        [XmlIgnore] public bool VelocityStandstillWindowSpecified { get; set; }
        [XmlAttribute] public float TorqueLimitPositive { get; set; }

        [XmlIgnore]
        public bool TorqueLimitPositiveSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.PositionLoop ||
                    AxisConfiguration == AxisConfigurationEnum.VelocityLoop ||
                    AxisConfiguration == AxisConfigurationEnum.TorqueLoop)
                    return true;

                return false;
            }
        }

        [XmlAttribute] public float TorqueLimitNegative { get; set; }

        [XmlIgnore]
        public bool TorqueLimitNegativeSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.PositionLoop ||
                    AxisConfiguration == AxisConfigurationEnum.VelocityLoop ||
                    AxisConfiguration == AxisConfigurationEnum.TorqueLoop)
                    return true;

                return false;
            }
        }

        [XmlAttribute] public float TorqueThreshold { get; set; }

        [XmlIgnore] public bool TorqueThresholdSpecified { get; set; }
        [XmlAttribute] public float StoppingTorque { get; set; }

        [XmlIgnore]
        public bool StoppingTorqueSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.PositionLoop ||
                    AxisConfiguration == AxisConfigurationEnum.VelocityLoop ||
                    AxisConfiguration == AxisConfigurationEnum.TorqueLoop)
                    return true;

                return false;
            }
        }

        [XmlAttribute] public float StoppingTimeLimit { get; set; }

        [XmlIgnore] public bool StoppingTimeLimitSpecified { get; set; }

        [XmlAttribute] public float LoadInertiaRatio { get; set; }

        [XmlIgnore]
        public bool LoadInertiaRatioSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.PositionLoop ||
                    AxisConfiguration == AxisConfigurationEnum.VelocityLoop ||
                    AxisConfiguration == AxisConfigurationEnum.TorqueLoop)
                    return true;

                return false;
            }
        }

        [XmlAttribute] public ushort RegistrationInputs { get; set; }

        [XmlIgnore] public bool RegistrationInputsSpecified { get; set; }

        [XmlAttribute] public float MaximumAccelerationJerk { get; set; }

        [XmlIgnore] public bool MaximumAccelerationJerkSpecified { get; set; }

        [XmlAttribute] public float MaximumDecelerationJerk { get; set; }

        [XmlIgnore] public bool MaximumDecelerationJerkSpecified { get; set; }

        [XmlAttribute] public ulong DynamicsConfigurationBits { get; set; }

        [XmlIgnore] public bool DynamicsConfigurationBitsSpecified { get; set; }

        [XmlAttribute] public float FeedbackUnitRatio { get; set; }

        [XmlIgnore] public bool FeedbackUnitRatioSpecified { get; set; }

        [XmlAttribute] public float AccelerationLimit { get; set; }

        [XmlIgnore] public bool AccelerationLimitSpecified { get; set; }

        [XmlAttribute] public float DecelerationLimit { get; set; }

        [XmlIgnore] public bool DecelerationLimitSpecified { get; set; }

        [XmlAttribute] public float PositionIntegratorBandwidth { get; set; }

        [XmlIgnore] public bool PositionIntegratorBandwidthSpecified { get; set; }

        [XmlAttribute] public float PositionErrorToleranceTime { get; set; }

        [XmlIgnore] public bool PositionErrorToleranceTimeSpecified { get; set; }

        [XmlAttribute] public byte PositionIntegratorControl { get; set; }

        [XmlIgnore] public bool PositionIntegratorControlSpecified { get; set; }

        [XmlAttribute] public float VelocityErrorTolerance { get; set; }

        [XmlIgnore] public bool VelocityErrorToleranceSpecified { get; set; }

        [XmlAttribute] public float VelocityErrorToleranceTime { get; set; }

        [XmlIgnore] public bool VelocityErrorToleranceTimeSpecified { get; set; }

        [XmlAttribute] public byte VelocityIntegratorControl { get; set; }

        [XmlIgnore] public bool VelocityIntegratorControlSpecified { get; set; }

        [XmlAttribute] public float VelocityLockTolerance { get; set; }

        [XmlIgnore] public bool VelocityLockToleranceSpecified { get; set; }

        [XmlAttribute] public float SystemInertia { get; set; }

        [XmlIgnore] public bool SystemInertiaSpecified { get; set; }

        [XmlAttribute] public float TorqueLowPassFilterBandwidth { get; set; }

        [XmlIgnore] public bool TorqueLowPassFilterBandwidthSpecified { get; set; }

        [XmlAttribute] public float TorqueNotchFilterFrequency { get; set; }

        [XmlIgnore] public bool TorqueNotchFilterFrequencySpecified { get; set; }

        [XmlAttribute] public float TorqueRateLimit { get; set; }

        [XmlIgnore] public bool TorqueRateLimitSpecified { get; set; }

        [XmlAttribute] public float OvertorqueLimit { get; set; }

        [XmlIgnore] public bool OvertorqueLimitSpecified { get; set; }

        [XmlAttribute] public float OvertorqueLimitTime { get; set; }

        [XmlIgnore] public bool OvertorqueLimitTimeSpecified { get; set; }

        [XmlAttribute] public float UndertorqueLimit { get; set; }

        [XmlIgnore] public bool UndertorqueLimitSpecified { get; set; }

        [XmlAttribute] public float UndertorqueLimitTime { get; set; }

        [XmlIgnore] public bool UndertorqueLimitTimeSpecified { get; set; }

        [XmlAttribute] public float FluxCurrentReference { get; set; }

        [XmlIgnore] public bool FluxCurrentReferenceSpecified { get; set; }

        [XmlAttribute] public float CurrentError { get; set; }

        [XmlIgnore] public bool CurrentErrorSpecified { get; set; }

        [XmlAttribute] public float TorqueLoopBandwidth { get; set; }

        [XmlIgnore] public bool TorqueLoopBandwidthSpecified { get; set; }

        [XmlAttribute] public AxisStoppingActionEnum StoppingAction { get; set; }

        [XmlIgnore]
        public bool StoppingActionSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.FeedbackOnly ||
                    AxisConfiguration == AxisConfigurationEnum.ConverterOnly)
                    return false;

                return true;
            }
        }

        [XmlAttribute] public AxisMechanicalBrakeControlEnum MechanicalBrakeControl { get; set; }

        [XmlIgnore] public bool MechanicalBrakeControlSpecified { get; set; }

        [XmlAttribute] public float MechanicalBrakeReleaseDelay { get; set; }

        [XmlIgnore] public bool MechanicalBrakeReleaseDelaySpecified { get; set; }

        [XmlAttribute] public float MechanicalBrakeEngageDelay { get; set; }

        [XmlIgnore] public bool MechanicalBrakeEngageDelaySpecified { get; set; }

        [XmlAttribute] public float InverterCapacity { get; set; }

        [XmlIgnore] public bool InverterCapacitySpecified { get; set; }

        [XmlAttribute] public float ConverterCapacity { get; set; }

        [XmlIgnore] public bool ConverterCapacitySpecified { get; set; }

        [XmlAttribute] public AxisInverterOverloadActionEnum InverterOverloadAction { get; set; }

        [XmlIgnore]
        public bool InverterOverloadActionSpecified
        {
            get
            {
                if (string.Equals(MotionModule, "<NA>"))
                    return false;

                if (AxisConfiguration == AxisConfigurationEnum.ConverterOnly ||
                    AxisConfiguration == AxisConfigurationEnum.FeedbackOnly)
                    return false;

                return true;
            }
        }

        [XmlAttribute] public AxisMotorOverloadActionEnum MotorOverloadAction { get; set; }

        [XmlIgnore]
        public bool MotorOverloadActionSpecified
        {
            get
            {
                if (string.Equals(MotionModule, "<NA>"))
                    return false;

                if (AxisConfiguration == AxisConfigurationEnum.ConverterOnly ||
                    AxisConfiguration == AxisConfigurationEnum.FeedbackOnly)
                    return false;

                return true;
            }
        }

        [XmlAttribute] public string CIPAxisExceptionAction { get; set; }

        [XmlIgnore] public bool CIPAxisExceptionActionSpecified { get; set; }

        [XmlAttribute] public string CIPAxisExceptionActionRA { get; set; }

        [XmlAttribute] public float MotorOverspeedUserLimit { get; set; }

        [XmlIgnore] public bool MotorOverspeedUserLimitSpecified { get; set; }

        [XmlAttribute] public float MotorThermalOverloadUserLimit { get; set; }

        [XmlIgnore] public bool MotorThermalOverloadUserLimitSpecified { get; set; }

        [XmlAttribute] public float InverterThermalOverloadUserLimit { get; set; }

        [XmlIgnore] public bool InverterThermalOverloadUserLimitSpecified { get; set; }

        [XmlAttribute] public float PositionLeadLagFilterBandwidth { get; set; }

        [XmlIgnore] public bool PositionLeadLagFilterBandwidthSpecified { get; set; }

        [XmlAttribute] public float PositionLeadLagFilterGain { get; set; }

        [XmlIgnore] public bool PositionLeadLagFilterGainSpecified { get; set; }

        [XmlAttribute] public float VelocityNegativeFeedforwardGain { get; set; }

        [XmlIgnore] public bool VelocityNegativeFeedforwardGainSpecified { get; set; }

        [XmlAttribute] public float BacklashCompensationWindow { get; set; }

        [XmlIgnore] public bool BacklashCompensationWindowSpecified { get; set; }

        [XmlAttribute] public float TorqueLeadLagFilterBandwidth { get; set; }

        [XmlIgnore] public bool TorqueLeadLagFilterBandwidthSpecified { get; set; }

        [XmlAttribute] public float TorqueLeadLagFilterGain { get; set; }

        [XmlIgnore] public bool TorqueLeadLagFilterGainSpecified { get; set; }

        [XmlAttribute] public string MotorDeviceCode { get; set; }

        [XmlAttribute] public AxisMotorUnitEnum MotorUnit { get; set; }

        [XmlIgnore]
        public bool MotorUnitSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.ConverterOnly ||
                    AxisConfiguration == AxisConfigurationEnum.FeedbackOnly)
                    return false;

                return true;
            }
        }

        [XmlAttribute] public AxisPolarityEnum MotorPolarity { get; set; }

        [XmlIgnore] public bool MotorPolaritySpecified { get; set; }

        [XmlAttribute] public float MotorRatedVoltage { get; set; }

        [XmlIgnore] public bool MotorRatedVoltageSpecified { get; set; }

        [XmlAttribute] public float MotorRatedContinuousCurrent { get; set; }

        [XmlIgnore] public bool MotorRatedContinuousCurrentSpecified { get; set; }

        [XmlAttribute] public float MotorRatedPeakCurrent { get; set; }

        [XmlIgnore] public bool MotorRatedPeakCurrentSpecified { get; set; }

        [XmlAttribute] public float MotorRatedOutputPower { get; set; }

        [XmlIgnore] public bool MotorRatedOutputPowerSpecified { get; set; }

        [XmlAttribute] public float MotorOverloadLimit { get; set; }

        [XmlIgnore] public bool MotorOverloadLimitSpecified { get; set; }

        [XmlAttribute] public BoolEnum MotorIntegralThermalSwitch { get; set; }

        [XmlIgnore] public bool MotorIntegralThermalSwitchSpecified { get; set; }

        [XmlAttribute] public float MotorMaxWindingTemperature { get; set; }

        [XmlIgnore] public bool MotorMaxWindingTemperatureSpecified { get; set; }

        [XmlAttribute] public float MotorWindingToAmbientCapacitance { get; set; }
        [XmlIgnore] public bool MotorWindingToAmbientCapacitanceSpecified { get; set; }

        [XmlAttribute] public float MotorWindingToAmbientResistance { get; set; }

        [XmlIgnore] public bool MotorWindingToAmbientResistanceSpecified { get; set; }


        [XmlAttribute] public float PMMotorResistance { get; set; }

        [XmlIgnore] public bool PMMotorResistanceSpecified { get; set; }

        [XmlAttribute] public float PMMotorInductance { get; set; }

        [XmlIgnore] public bool PMMotorInductanceSpecified { get; set; }

        [XmlAttribute] public ushort RotaryMotorPoles { get; set; }

        [XmlIgnore]
        public bool RotaryMotorPolesSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.ConverterOnly ||
                    AxisConfiguration == AxisConfigurationEnum.FeedbackOnly)
                    return false;

                if (MotorType == AxisMotorTypeEnum.RotaryInduction ||
                    MotorType == AxisMotorTypeEnum.RotaryPermanentMagnet ||
                    MotorType == AxisMotorTypeEnum.RotaryInteriorPermanentMagnet)
                    return true;

                return false;
            }
        }

        [XmlAttribute] public float RotaryMotorInertia { get; set; }

        [XmlIgnore] public bool RotaryMotorInertiaSpecified { get; set; }

        [XmlAttribute] public float RotaryMotorRatedSpeed { get; set; }

        [XmlIgnore]
        public bool RotaryMotorRatedSpeedSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.ConverterOnly ||
                    AxisConfiguration == AxisConfigurationEnum.FeedbackOnly)
                    return false;

                if (MotorType == AxisMotorTypeEnum.RotaryInduction ||
                    MotorType == AxisMotorTypeEnum.RotaryPermanentMagnet ||
                    MotorType == AxisMotorTypeEnum.RotaryInteriorPermanentMagnet)
                    return true;

                return false;
            }
        }

        [XmlAttribute] public float RotaryMotorMaxSpeed { get; set; }

        [XmlIgnore] public bool RotaryMotorMaxSpeedSpecified { get; set; }

        //PM
        [XmlAttribute] public float PMMotorRatedTorque { get; set; }

        [XmlIgnore] public bool PMMotorRatedTorqueSpecified { get; set; }

        [XmlAttribute] public float PMMotorTorqueConstant { get; set; }

        [XmlIgnore] public bool PMMotorTorqueConstantSpecified { get; set; }

        [XmlAttribute] public float PMMotorRotaryVoltageConstant { get; set; }

        [XmlIgnore] public bool PMMotorRotaryVoltageConstantSpecified { get; set; }

        //feedback1
        [XmlAttribute] public AxisCIPFeedbackUnitEnum Feedback1Unit { get; set; }

        [XmlIgnore] public bool Feedback1UnitSpecified { get; set; }

        [XmlAttribute] public AxisPolarityEnum Feedback1Polarity { get; set; }

        [XmlIgnore]
        public bool Feedback1PolaritySpecified
        {
            get
            {
                if (Feedback1Type == AxisFeedbackTypeEnum.NotSpecified)
                    return false;

                return true;
            }
        }

        [XmlAttribute] public AxisFeedbackStartupMethodEnum Feedback1StartupMethod { get; set; }

        [XmlIgnore]
        public bool Feedback1StartupMethodSpecified
        {
            get
            {
                if (Feedback1Type == AxisFeedbackTypeEnum.NotSpecified)
                    return false;

                return true;
            }
        }

        [XmlAttribute] public ulong Feedback1CycleResolution { get; set; }

        [XmlIgnore]
        public bool Feedback1CycleResolutionSpecified
        {
            get
            {
                if (Feedback1Type == AxisFeedbackTypeEnum.NotSpecified)
                    return false;

                return true;
            }
        }

        [XmlAttribute] public ulong Feedback1CycleInterpolation { get; set; }

        [XmlIgnore]
        public bool Feedback1CycleInterpolationSpecified
        {
            get
            {
                if (Feedback1Type == AxisFeedbackTypeEnum.NotSpecified)
                    return false;

                return true;
            }
        }

        [XmlAttribute] public ulong Feedback1Turns { get; set; }

        [XmlIgnore]
        public bool Feedback1TurnsSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.ConverterOnly ||
                    AxisConfiguration == AxisConfigurationEnum.FrequencyControl)
                    return false;

                if (Feedback1Type == AxisFeedbackTypeEnum.NotSpecified)
                    return false;


                if (Feedback1StartupMethod == AxisFeedbackStartupMethodEnum.Incremental)
                    return false;

                return true;
            }
        }

        [XmlAttribute] public float Feedback1VelocityFilterBandwidth { get; set; }

        [XmlIgnore]
        public bool Feedback1VelocityFilterBandwidthSpecified
        {
            get
            {
                if (Feedback1Type == AxisFeedbackTypeEnum.NotSpecified)
                    return false;

                return true;
            }
        }

        [XmlAttribute] public float Feedback1AccelFilterBandwidth { get; set; }

        [XmlIgnore]
        public bool Feedback1AccelFilterBandwidthSpecified
        {
            get
            {
                if (Feedback1Type == AxisFeedbackTypeEnum.NotSpecified)
                    return false;

                return true;
            }
        }


        [XmlAttribute] public string PMMotorFluxSaturation { get; set; }

        [XmlIgnore] public bool PMMotorFluxSaturationSpecified { get; set; }


        [XmlAttribute] public ushort Feedback1VelocityFilterTaps { get; set; }

        [XmlIgnore]
        public bool Feedback1VelocityFilterTapsSpecified
        {
            get
            {
                if (string.IsNullOrEmpty(MotionModule))
                    return false;

                if (string.Equals(MotionModule, "<NA>"))
                    return false;

                if (Feedback1Type == AxisFeedbackTypeEnum.NotSpecified)
                    return false;

                if (AxisConfiguration == AxisConfigurationEnum.ConverterOnly ||
                    AxisConfiguration == AxisConfigurationEnum.FrequencyControl)
                    return false;

                return true;
            }
        }

        [XmlAttribute] public ushort Feedback1AccelFilterTaps { get; set; }

        [XmlIgnore]
        public bool Feedback1AccelFilterTapsSpecified
        {
            get
            {
                if (string.IsNullOrEmpty(MotionModule))
                    return false;

                if (string.Equals(MotionModule, "<NA>"))
                    return false;

                if (Feedback1Type == AxisFeedbackTypeEnum.NotSpecified)
                    return false;

                return true;
            }
        }

        [XmlAttribute] public string CyclicReadUpdateList { get; set; }

        [XmlIgnore] public bool CyclicReadUpdateListSpecified { get; set; }

        [XmlAttribute] public string CyclicWriteUpdateList { get; set; }

        [XmlIgnore] public bool CyclicWriteUpdateListSpecified { get; set; }


        [XmlAttribute] public AxisScalingSourceEnum ScalingSource { get; set; }

        [XmlIgnore] public bool ScalingSourceSpecified { get; set; }

        [XmlAttribute] public AxisLoadTypeEnum LoadType { get; set; }

        [XmlIgnore] public bool LoadTypeSpecified { get; set; }

        [XmlAttribute] public AxisActuatorTypeEnum ActuatorType { get; set; }

        [XmlIgnore] public bool ActuatorTypeSpecified { get; set; }

        [XmlAttribute] public AxisTravelModeEnum TravelMode { get; set; }

        [XmlIgnore] public bool TravelModeSpecified { get; set; }

        [XmlAttribute] public float PositionScalingNumerator { get; set; }

        [XmlIgnore] public bool PositionScalingNumeratorSpecified { get; set; }

        [XmlAttribute] public float PositionScalingDenominator { get; set; }

        [XmlIgnore] public bool PositionScalingDenominatorSpecified { get; set; }

        [XmlAttribute] public float PositionUnwindNumerator { get; set; }

        [XmlIgnore]
        public bool PositionUnwindNumeratorSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.ConverterOnly ||
                    AxisConfiguration == AxisConfigurationEnum.FrequencyControl)
                    return false;

                return true;
            }
        }

        [XmlAttribute] public float PositionUnwindDenominator { get; set; }

        [XmlIgnore]
        public bool PositionUnwindDenominatorSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.ConverterOnly ||
                    AxisConfiguration == AxisConfigurationEnum.FrequencyControl)
                    return false;

                return true;
            }
        }

        [XmlAttribute] public float TravelRange { get; set; }

        [XmlIgnore]
        public bool TravelRangeSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.ConverterOnly ||
                    AxisConfiguration == AxisConfigurationEnum.FrequencyControl)
                    return false;

                return true;
            }
        }

        [XmlAttribute] public long MotionResolution { get; set; }

        [XmlIgnore] public bool MotionResolutionSpecified { get; set; }

        [XmlAttribute] public AxisPolarityEnum MotionPolarity { get; set; }

        [XmlIgnore] public bool MotionPolaritySpecified { get; set; }

        [XmlAttribute] public float MotorTestResistance { get; set; }

        [XmlIgnore] public bool MotorTestResistanceSpecified { get; set; }

        [XmlAttribute] public float MotorTestInductance { get; set; }

        [XmlIgnore] public bool MotorTestInductanceSpecified { get; set; }

        [XmlAttribute] public float TuneFriction { get; set; }

        [XmlIgnore]
        public bool TuneFrictionSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.PositionLoop ||
                    AxisConfiguration == AxisConfigurationEnum.VelocityLoop ||
                    AxisConfiguration == AxisConfigurationEnum.TorqueLoop)
                    return true;

                return false;
            }
        }

        [XmlAttribute] public float TuneLoadOffset { get; set; }

        [XmlIgnore]
        public bool TuneLoadOffsetSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.PositionLoop ||
                    AxisConfiguration == AxisConfigurationEnum.VelocityLoop ||
                    AxisConfiguration == AxisConfigurationEnum.TorqueLoop)
                    return true;

                return false;
            }
        }

        [XmlAttribute] public float TotalInertia { get; set; }

        [XmlIgnore]
        public bool TotalInertiaSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.PositionLoop ||
                    AxisConfiguration == AxisConfigurationEnum.VelocityLoop ||
                    AxisConfiguration == AxisConfigurationEnum.TorqueLoop)
                    return true;

                return false;
            }
        }


        [XmlAttribute] public AxisTuningSelectEnum TuningSelect { get; set; }

        [XmlIgnore]
        public bool TuningSelectSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.PositionLoop ||
                    AxisConfiguration == AxisConfigurationEnum.VelocityLoop ||
                    AxisConfiguration == AxisConfigurationEnum.TorqueLoop)
                    return true;

                return false;
            }
        }

        [XmlAttribute] public AxisTuningDirectionEnum TuningDirection { get; set; }

        [XmlIgnore]
        public bool TuningDirectionSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.PositionLoop ||
                    AxisConfiguration == AxisConfigurationEnum.VelocityLoop ||
                    AxisConfiguration == AxisConfigurationEnum.TorqueLoop)
                    return true;

                return false;
            }
        }

        [XmlAttribute] public AxisApplicationTypeEnum ApplicationType { get; set; }

        [XmlIgnore] public bool ApplicationTypeSpecified { get; set; }

        [XmlAttribute] public AxisLoopResponseEnum LoopResponse { get; set; }

        [XmlIgnore] public bool LoopResponseSpecified { get; set; }

        [XmlAttribute] public AxisFeedbackCommutationAlignedEnum FeedbackCommutationAligned { get; set; }

        [XmlIgnore]
        public bool FeedbackCommutationAlignedSpecified
        {
            get
            {
                if (!(AxisConfiguration == AxisConfigurationEnum.PositionLoop ||
                      AxisConfiguration == AxisConfigurationEnum.VelocityLoop ||
                      AxisConfiguration == AxisConfigurationEnum.TorqueLoop))
                    return false;

                if (Feedback1Type == AxisFeedbackTypeEnum.NotSpecified)
                    return false;

                return true;

            }
        }

        [XmlAttribute] public float FrictionCompensationSliding { get; set; }

        [XmlIgnore] public bool FrictionCompensationSlidingSpecified { get; set; }

        [XmlAttribute] public float FrictionCompensationStatic { get; set; }

        [XmlIgnore] public bool FrictionCompensationStaticSpecified { get; set; }

        [XmlAttribute] public float FrictionCompensationViscous { get; set; }

        [XmlIgnore] public bool FrictionCompensationViscousSpecified { get; set; }

        [XmlAttribute] public float PositionLoopBandwidth { get; set; }

        [XmlIgnore] public bool PositionLoopBandwidthSpecified { get; set; }

        [XmlAttribute] public float VelocityLoopBandwidth { get; set; }

        [XmlIgnore] public bool VelocityLoopBandwidthSpecified { get; set; }

        [XmlAttribute] public float VelocityIntegratorBandwidth { get; set; }

        [XmlIgnore] public bool VelocityIntegratorBandwidthSpecified { get; set; }

        [XmlAttribute] public float FeedbackDataLossUserLimit { get; set; }

        [XmlIgnore]
        public bool FeedbackDataLossUserLimitSpecified
        {
            get
            {
                if (string.IsNullOrEmpty(MotionModule))
                    return false;

                if (string.Equals(MotionModule, "<NA>"))
                    return false;


                if (AxisConfiguration == AxisConfigurationEnum.ConverterOnly ||
                    AxisConfiguration == AxisConfigurationEnum.FrequencyControl)
                    return false;

                return true;
            }
        }

        [XmlAttribute] public string MotionExceptionAction { get; set; }

        [XmlIgnore] public bool MotionExceptionActionSpecified { get; set; }

        [XmlAttribute] public BoolEnum SoftTravelLimitChecking { get; set; }

        [XmlIgnore] public bool SoftTravelLimitCheckingSpecified { get; set; }

        [XmlAttribute] public float LoadRatio { get; set; }

        [XmlIgnore]
        public bool LoadRatioSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.PositionLoop ||
                    AxisConfiguration == AxisConfigurationEnum.VelocityLoop ||
                    AxisConfiguration == AxisConfigurationEnum.TorqueLoop)
                    return true;

                return false;
            }
        }

        [XmlAttribute] public float TuneInertiaMass { get; set; }

        [XmlIgnore]
        public bool TuneInertiaMassSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.PositionLoop ||
                    AxisConfiguration == AxisConfigurationEnum.VelocityLoop ||
                    AxisConfiguration == AxisConfigurationEnum.TorqueLoop)
                    return true;

                return false;
            }
        }

        [XmlAttribute] public float SoftTravelLimitPositive { get; set; }

        [XmlIgnore] public bool SoftTravelLimitPositiveSpecified { get; set; }

        [XmlAttribute] public float SoftTravelLimitNegative { get; set; }

        [XmlIgnore] public bool SoftTravelLimitNegativeSpecified { get; set; }

        [XmlAttribute] public string GainTuningConfigurationBits { get; set; }

        [XmlAttribute] public float CommutationOffset { get; set; }

        [XmlIgnore]
        public bool CommutationOffsetSpecified
        {
            get
            {
                if (Feedback1Type == AxisFeedbackTypeEnum.NotSpecified)
                    return false;

                if (AxisConfiguration == AxisConfigurationEnum.FeedbackOnly)
                    return false;

                if (MotorType == AxisMotorTypeEnum.NotSpecified)
                    return false;

                if (Feedback1Type == AxisFeedbackTypeEnum.DigitalAqB)
                    return false;

                return true;
            }
        }


        [XmlAttribute] public float SystemBandwidth { get; set; }

        [XmlIgnore]
        public bool SystemBandwidthSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.PositionLoop ||
                    AxisConfiguration == AxisConfigurationEnum.VelocityLoop ||
                    AxisConfiguration == AxisConfigurationEnum.TorqueLoop)
                    return true;

                return false;
            }
        }

        [XmlAttribute] public float VelocityLowPassFilterBandwidth { get; set; }

        [XmlIgnore] public bool VelocityLowPassFilterBandwidthSpecified { get; set; }

        [XmlAttribute] public long TransmissionRatioInput { get; set; }

        [XmlIgnore] public bool TransmissionRatioInputSpecified { get; set; }

        [XmlAttribute] public long TransmissionRatioOutput { get; set; }

        [XmlIgnore] public bool TransmissionRatioOutputSpecified { get; set; }

        [XmlAttribute] public float ActuatorLead { get; set; }

        [XmlIgnore] public bool ActuatorLeadSpecified { get; set; }

        [XmlAttribute] public AxisActuatorLeadUnitEnum ActuatorLeadUnit { get; set; }

        [XmlIgnore] public bool ActuatorLeadUnitSpecified { get; set; }

        [XmlAttribute] public float ActuatorDiameter { get; set; }

        [XmlIgnore] public bool ActuatorDiameterSpecified { get; set; }

        [XmlAttribute] public AxisActuatorDiameterUnitEnum ActuatorDiameterUnit { get; set; }

        [XmlIgnore] public bool ActuatorDiameterUnitSpecified { get; set; }

        [XmlAttribute] public float SystemAccelerationBase { get; set; }

        [XmlIgnore] public bool SystemAccelerationBaseSpecified { get; set; }

        [XmlAttribute] public float DriveModelTimeConstantBase { get; set; }

        [XmlIgnore] public bool DriveModelTimeConstantBaseSpecified { get; set; }

        [XmlAttribute] public float DriveRatedPeakCurrent { get; set; }

        [XmlIgnore] public bool DriveRatedPeakCurrentSpecified { get; set; }

        [XmlAttribute] public float HookupTestDistance { get; set; }

        [XmlIgnore]
        public bool HookupTestDistanceSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.ConverterOnly ||
                    AxisConfiguration == AxisConfigurationEnum.FrequencyControl)
                    return false;

                return true;
            }
        }

        [XmlAttribute] public AxisHookupTestFeedbackChannelEnum HookupTestFeedbackChannel { get; set; }

        [XmlIgnore]
        public bool HookupTestFeedbackChannelSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.ConverterOnly ||
                    AxisConfiguration == AxisConfigurationEnum.FrequencyControl)
                    return false;

                return true;
            }
        }

        [XmlAttribute] public AxisLoadCouplingEnum LoadCoupling { get; set; }

        [XmlIgnore]
        public bool LoadCouplingSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.PositionLoop ||
                    AxisConfiguration == AxisConfigurationEnum.VelocityLoop ||
                    AxisConfiguration == AxisConfigurationEnum.TorqueLoop)
                    return true;

                return false;
            }
        }

        [XmlAttribute] public float SystemDamping { get; set; }

        [XmlIgnore]
        public bool SystemDampingSpecified
        {
            get
            {
                if (AxisConfiguration == AxisConfigurationEnum.PositionLoop ||
                    AxisConfiguration == AxisConfigurationEnum.VelocityLoop ||
                    AxisConfiguration == AxisConfigurationEnum.TorqueLoop)
                    return true;

                return false;
            }
        }

        [XmlAttribute] public AxisPolarityEnum CommutationPolarity { get; set; }

        [XmlIgnore]
        public bool CommutationPolaritySpecified
        {
            get
            {
                if (string.Equals(MotionModule, "<NA>"))
                    return false;

                if (AxisConfiguration == AxisConfigurationEnum.PositionLoop ||
                    AxisConfiguration == AxisConfigurationEnum.VelocityLoop ||
                    AxisConfiguration == AxisConfigurationEnum.TorqueLoop)
                    return true;

                return false;
            }
        }

        [XmlAttribute] public AxisLoadObserverConfigurationEnum LoadObserverConfiguration { get; set; }

        [XmlIgnore]
        public bool LoadObserverConfigurationSpecified
        {
            get
            {
                if (string.Equals(MotionModule, "<NA>"))
                    return false;

                if (AxisConfiguration == AxisConfigurationEnum.PositionLoop ||
                    AxisConfiguration == AxisConfigurationEnum.VelocityLoop ||
                    AxisConfiguration == AxisConfigurationEnum.TorqueLoop)
                    return true;

                return false;
            }
        }

        [XmlAttribute] public float LoadObserverBandwidth { get; set; }

        [XmlIgnore] public bool LoadObserverBandwidthSpecified { get; set; }

        [XmlAttribute] public float LoadObserverIntegratorBandwidth { get; set; }

        [XmlIgnore]
        public bool LoadObserverIntegratorBandwidthSpecified
        {
            get
            {
                if (string.IsNullOrEmpty(MotionModule))
                    return false;

                if (string.Equals(MotionModule, "<NA>"))
                    return false;

                return true;
            }
        }

        [XmlAttribute] public float LoadObserverFeedbackGain { get; set; }

        [XmlIgnore] public bool LoadObserverFeedbackGainSpecified { get; set; }

        [XmlAttribute] public ulong AxisID { get; set; }

        [XmlIgnore] public bool AxisIDSpecified { get; set; }

        [XmlAttribute] public string InterpolatedPositionConfiguration { get; set; }

        [XmlAttribute] public AxisUpdateScheduleEnum AxisUpdateSchedule { get; set; }

        [XmlAttribute] public BooleanEnum ProvingConfiguration { get; set; }

        [XmlIgnore]
        public bool ProvingConfigurationSpecified
        {
            get
            {
                if (string.IsNullOrEmpty(MotionModule))
                    return false;

                if (string.Equals(MotionModule, "<NA>"))
                    return false;

                return true;
            }
        }

        [XmlAttribute] public float TorqueProveCurrent { get; set; }

        [XmlIgnore]
        public bool TorqueProveCurrentSpecified
        {
            get
            {
                if (string.IsNullOrEmpty(MotionModule))
                    return false;

                if (string.Equals(MotionModule, "<NA>"))
                    return false;

                return true;
            }
        }

        [XmlAttribute] public float BrakeTestTorque { get; set; }

        [XmlIgnore]
        public bool BrakeTestTorqueSpecified
        {
            get
            {
                if (string.IsNullOrEmpty(MotionModule))
                    return false;

                if (string.Equals(MotionModule, "<NA>"))
                    return false;

                return true;
            }
        }

        [XmlAttribute] public float BrakeSlipTolerance { get; set; }

        [XmlIgnore]
        public bool BrakeSlipToleranceSpecified
        {
            get
            {
                if (string.IsNullOrEmpty(MotionModule))
                    return false;

                if (string.Equals(MotionModule, "<NA>"))
                    return false;

                return true;
            }
        }

        [XmlAttribute] public float ZeroSpeed { get; set; }

        [XmlIgnore]
        public bool ZeroSpeedSpecified
        {
            get
            {
                if (string.IsNullOrEmpty(MotionModule))
                    return false;

                if (string.Equals(MotionModule, "<NA>"))
                    return false;

                return true;
            }
        }

        [XmlAttribute] public float ZeroSpeedTime { get; set; }

        [XmlIgnore]
        public bool ZeroSpeedTimeSpecified
        {
            get
            {
                if (string.IsNullOrEmpty(MotionModule))
                    return false;

                if (string.Equals(MotionModule, "<NA>"))
                    return false;

                return true;
            }
        }

        [XmlAttribute] public AxisAdaptiveTuningConfigurationEnum AdaptiveTuningConfiguration { get; set; }

        [XmlIgnore]
        public bool AdaptiveTuningConfigurationSpecified
        {
            get
            {
                if (string.IsNullOrEmpty(MotionModule))
                    return false;

                if (string.Equals(MotionModule, "<NA>"))
                    return false;

                return true;
            }
        }

        [XmlAttribute] public float TorqueNotchFilterHighFrequencyLimit { get; set; }

        [XmlIgnore]
        public bool TorqueNotchFilterHighFrequencyLimitSpecified
        {
            get
            {
                if (string.IsNullOrEmpty(MotionModule))
                    return false;

                if (string.Equals(MotionModule, "<NA>"))
                    return false;

                return true;
            }
        }

        [XmlAttribute] public float TorqueNotchFilterLowFrequencyLimit { get; set; }

        [XmlIgnore]
        public bool TorqueNotchFilterLowFrequencyLimitSpecified
        {
            get
            {
                if (string.IsNullOrEmpty(MotionModule))
                    return false;

                if (string.Equals(MotionModule, "<NA>"))
                    return false;

                return true;
            }
        }

        [XmlAttribute] public float TorqueNotchFilterTuningThreshold { get; set; }

        [XmlIgnore]
        public bool TorqueNotchFilterTuningThresholdSpecified
        {
            get
            {
                if (string.IsNullOrEmpty(MotionModule))
                    return false;

                if (string.Equals(MotionModule, "<NA>"))
                    return false;

                return true;
            }
        }

        [XmlAttribute] public float CoastingTimeLimit { get; set; }

        [XmlIgnore]
        public bool CoastingTimeLimitSpecified
        {
            get
            {
                if (string.IsNullOrEmpty(MotionModule))
                    return false;

                if (string.Equals(MotionModule, "<NA>"))
                    return false;

                return true;
            }
        }

        [XmlAttribute] public float BusOvervoltageOperationalLimit { get; set; }

        [XmlIgnore]
        public bool BusOvervoltageOperationalLimitSpecified
        {
            get
            {
                if (string.IsNullOrEmpty(MotionModule))
                    return false;

                if (string.Equals(MotionModule, "<NA>"))
                    return false;

                return true;
            }
        }

        [XmlAttribute] public float CurrentLoopBandwidthScalingFactor { get; set; }

        [XmlIgnore]
        public bool CurrentLoopBandwidthScalingFactorSpecified
        {
            get
            {
                if (string.IsNullOrEmpty(MotionModule))
                    return false;

                if (string.Equals(MotionModule, "<NA>"))
                    return false;

                return true;
            }
        }

        [XmlAttribute] public float CurrentLoopBandwidth { get; set; }

        [XmlIgnore]
        public bool CurrentLoopBandwidthSpecified
        {
            get
            {
                if (string.IsNullOrEmpty(MotionModule))
                    return false;

                if (string.Equals(MotionModule, "<NA>"))
                    return false;

                return true;
            }
        }

        [XmlAttribute] public float DriveRatedVoltage { get; set; }

        [XmlIgnore]
        public bool DriveRatedVoltageSpecified
        {
            get
            {
                if (string.IsNullOrEmpty(MotionModule))
                    return false;

                if (string.Equals(MotionModule, "<NA>"))
                    return false;

                return true;
            }
        }

        [XmlAttribute] public float MaxOutputFrequency { get; set; }

        [XmlIgnore]
        public bool MaxOutputFrequencySpecified
        {
            get
            {
                if (string.IsNullOrEmpty(MotionModule))
                    return false;

                if (string.Equals(MotionModule, "<NA>"))
                    return false;

                return true;
            }
        }

        //v33
        [XmlAttribute] public bool MotorTestDataValid { get; set; } //False
        [XmlIgnore] public bool MotorTestDataValidSpecified { get; set; }

        [XmlAttribute] public float HookupTestSpeed { get; set; }

        [XmlIgnore] public bool HookupTestSpeedSpecified { get; set; }
        //end v33
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisConfigurationEnum
    {
        [XmlEnum("Feedback Only")] FeedbackOnly = 0,
        [XmlEnum("Frequency Control")] FrequencyControl = 1,
        [XmlEnum("Position Loop")] PositionLoop = 2,
        [XmlEnum("Velocity Loop")] VelocityLoop = 3,
        [XmlEnum("Torque Loop")] TorqueLoop = 4,
        [XmlEnum("Converter Only")] ConverterOnly = 5
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisFeedbackConfigurationEnum
    {
        [XmlEnum("No Feedback")] NoFeedback = 0,
        [XmlEnum("Master Feedback")] MasterFeedback = 1,
        [XmlEnum("Motor Feedback")] MotorFeedback = 2,
        [XmlEnum("Load Feedback")] LoadFeedback = 3,
        [XmlEnum("Dual Feedback")] DualFeedback = 4,
        [XmlEnum("Dual Integrator Feedback")] DualIntegratorFeedback = 8,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisMotorDataSourceEnum
    {
        [XmlEnum("Nameplate Datasheet")] NameplateDatasheet = 0,
        Database = 1,
        [XmlEnum("Drive NV")] DriveNV = 2,
        [XmlEnum("Motor NV")] MotorNV = 3,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum AxisFeedbackTypeEnum
    {
        [XmlEnum("Not Specified")] NotSpecified = 0,
        [XmlEnum("Digital AqB")] DigitalAqB = 1,
        [XmlEnum("Digital AqB with UVW")] DigitalAqBwithUVW = 2,
        [XmlEnum("Digital Parallel")] DigitalParallel = 3,
        [XmlEnum("Sine/Cosine")] SineCosine = 4,
        [XmlEnum("Sine/Cosine with UVW")] SineCosinewithUVW = 5,
        Hiperface = 6,
        [XmlEnum("EnDat Sine/Cosine")] EnDat21 = 7,
        [XmlEnum("EnDat Digital")] EnDat22 = 8,
        Resolver = 9,
        [XmlEnum("SSI Digital")] SSI = 10,
        LDT = 11,
        [XmlEnum("Hiperface DSL")]
        HiperfaceDsl = 12,
        [XmlEnum( "BiSS Digital")]
        BissDigital = 13,
        [XmlEnum( "Integrated")]
        Integrated = 14,

        [XmlEnum( "SSI Sine/Cosine")]
        SsiSineCosine = 15,
        [XmlEnum( "SSI AqB")]
        SsiAqB = 16,

        [XmlEnum( "BiSS Sine/Cosine")]
        BissSineCosine = 17,

        [XmlEnum("Tamagawa Serial")] TamagawaSerial = 128,
        [XmlEnum("Stahl SSI")] StahlSSI = 129,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisMotorTypeEnum
    {
        [XmlEnum("Not Specified")] NotSpecified = 0,
        [XmlEnum("Rotary Permanent Magnet")] RotaryPermanentMagnet = 1,
        [XmlEnum("Rotary Induction")] RotaryInduction = 2,
        [XmlEnum("Linear Permanent Magnet")] LinearPermanentMagnet = 3,
        [XmlEnum("Linear Induction")] LinearInduction = 4,

        [XmlEnum("Rotary Interior Permanent Magnet")]
        RotaryInteriorPermanentMagnet = 5
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisMotionScalingConfigurationEnum
    {
        [XmlEnum("Control Scaling")] ControlScaling = 0,
        [XmlEnum("Drive Scaling")] DriveScaling = 1,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisHomeModeEnum
    {
        Passive = 0,
        Active = 1,
        Absolute,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisHomeDirEnum
    {
        [XmlEnum("Uni-directional Forward")] UnidirectionalForward = 0,
        [XmlEnum("Bi-directional Forward")] BidirectionalForward = 1,
        [XmlEnum("Uni-directional Reverse")] UnidirectionalReverse = 2,
        [XmlEnum("Bi-directional Reverse")] BidirectionalReverse = 3,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisHomeSeqEnum
    {
        Immediate = 0,
        Switch = 1,
        Marker = 2,
        [XmlEnum("Switch-Marker")] SwitchMarker = 3,
        [XmlEnum("Torque Level")] TorqueLevel = 4,
        [XmlEnum("Torque Level-Marker")] TorqueLevelMarker = 5,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisProgStopModeEnum
    {
        [XmlEnum("Fast Stop")] FastStop = 0,
        [XmlEnum("Fast Disable")] FastDisable = 1,
        [XmlEnum("Hard Disable")] HardDisable = 2,
        [XmlEnum("Fast Shutdown")] FastShutdown = 3,
        [XmlEnum("Hard Shutdown")] HardShutdown = 4,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum AxisStoppingActionEnum
    {
        [XmlEnum("Disable Coast")] DisableCoast = 0,
        [XmlEnum("Current Decel Disable")] CurrentDecelDisable = 1,
        [XmlEnum("Ramped Decel Disable")] RampedDecelDisable = 2,
        [XmlEnum("Current Decel Hold")] CurrentDecelHold = 3,
        [XmlEnum("Ramped Decel Hold")] RampedDecelHold = 4,
        [XmlEnum("DC Injection Brake")] DCInjectionBrake = 128,
        [XmlEnum("AC Injection Brake")] ACInjectionBrake = 129,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisMechanicalBrakeControlEnum
    {
        Automatic = 0,
        [XmlEnum("Brake Released")] BrakeReleased = 1,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisInverterOverloadActionEnum
    {
        None = 0,
        [XmlEnum("Current Foldback")] CurrentFoldback = 1,
        [XmlEnum("Reduce PWM Rate")] ReducePWMRate = 128,
        [XmlEnum("PWM - Foldback")] PWMFoldback = 129,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisMotorOverloadActionEnum
    {
        None = 0,
        [XmlEnum("Current Foldback")] CurrentFoldback = 1,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisExceptionActionEnum
    {
        Ignore = 0,
        Alarm = 1,
        FaultStatusOnly = 2,
        StopPlanner = 3,
        Disable = 4,
        Shutdown = 5,

        Unsupported = 255,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisMotorUnitEnum
    {
        Rev = 0,
        Meter = 1,
        Inch,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisCIPFeedbackUnitEnum
    {
        Rev = 0,
        Meter = 1,
        Inch,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisScalingSourceEnum
    {
        [XmlEnum("From Calculator")] FromCalculator = 0,

        [XmlEnum("Direct Scaling Factor Entry")]
        DirectScalingFactorEntry = 1,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisLoadTypeEnum
    {
        [XmlEnum("Direct Coupled Rotary")] DirectCoupledRotary = 0,
        [XmlEnum("Direct Coupled Linear")] DirectCoupledLinear = 1,
        [XmlEnum("Rotary Transmission")] RotaryTransmission = 2,
        [XmlEnum("Linear Actuator")] LinearActuator = 3,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public enum AxisActuatorTypeEnum
    {
        [XmlEnum("<none>")] none = 0,
        Screw = 1,
        [XmlEnum("Belt and Pulley")] BeltandPulley = 2,
        [XmlEnum("Chain and Sprocket")] ChainandSprocket = 3,
        [XmlEnum("Rack and Pinion")] RackandPinion = 4,

    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisTravelModeEnum
    {
        Unlimited = 0,
        Limited = 1,
        Cyclic = 2,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisPolarityEnum
    {
        Normal = 0,
        Inverted = 1,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisTuningSelectEnum
    {
        [XmlEnum("Motor Inertia")] MotorInertia = 1,
        [XmlEnum("Total Inertia")] TotalInertia = 0,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisTuningDirectionEnum
    {
        [XmlEnum("Uni-directional Forward")] UnidirectionalForward = 0,
        [XmlEnum("Uni-directional Reverse")] UnidirectionalReverse = 1,
        [XmlEnum("Bi-directional Forward")] BidirectionalForward = 2,
        [XmlEnum("Bi-directional Reverse")] BidirectionalReverse = 3,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisApplicationTypeEnum
    {
        Custom = 0,
        Basic = 1,
        Tracking = 2,
        [XmlEnum("Point-to-Point")] PointtoPoint = 3,
        [XmlEnum("Constant Speed")] ConstantSpeed = 4,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisLoopResponseEnum
    {
        Low = 0,
        Medium = 1,
        High = 2,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisMotionExceptionActionEnum
    {
        Ignore = 0,
        Alarm,
        FaultStatusOnly,
        StopPlanner,
        StopDrive,
        Shutdown,
        Unsupported = 255,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisActuatorLeadUnitEnum
    {
        [XmlEnum("Millimeter/Rev")] MillimeterRev = 0,
        [XmlEnum("Inch/Rev")] InchRev = 1,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisActuatorDiameterUnitEnum
    {
        Millimeter = 0,
        Inch = 1,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisHookupTestFeedbackChannelEnum
    {
        [XmlEnum("Feedback 1")] Feedback1 = 1,
        [XmlEnum("Feedback 2")] Feedback2 = 2,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisLoadCouplingEnum
    {
        Rigid = 0,
        Compliant = 1,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisLoadObserverConfigurationEnum
    {
        Disabled = 0,
        [XmlEnum("Load Observer Only")] LoadObserverOnly = 1,

        [XmlEnum("Load Observer with Velocity Estimate")]
        LoadObserverwithVelocityEstimate = 2,
        [XmlEnum("Velocity Estimate Only")] VelocityEstimateOnly = 3,
        [XmlEnum("Acceleration Feedback")] AccelerationFeedback = 4,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisUpdateScheduleEnum
    {

        Base = 0,
        [XmlEnum("Alternate 1")] Alternate1 = 1,
        [XmlEnum("Alternate 2")] Alternate2 = 2
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum BooleanEnum
    {
        [XmlEnum("Disabled")] Disabled = 0,
        [XmlEnum("Enabled")] Enabled = 1
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisAdaptiveTuningConfigurationEnum
    {
        //0 = Disabled
        //1 = Tracking Notch
        //2 = Gain Stabilization
        //3 = Tracking Notch and Gain Stabilization
        //4…255 = Reserved
        [XmlEnum("Disabled")] Disabled = 0,

        [XmlEnum("Tracking Notch")] TrackingNotch = 1,

        [XmlEnum("Gain Stabilization")] GainStabilization = 2,

        [XmlEnum("Tracking Notch and Gain Stabilization")]
        TrackingNotchAndGainStabilization = 3,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum RotaryAxisEnum
    {
        Linear,
        Rotary,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisFeedbackStartupMethodEnum
    {
        Incremental = 0,
        Absolute = 1,
    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum AxisFeedbackCommutationAlignedEnum
    {
        [XmlEnum("Not Aligned")] NotAligned = 0,
        [XmlEnum("Controller Offset")] ControllerOffset = 1,
        [XmlEnum("Motor Offset")] MotorOffset = 2,
        [XmlEnum("Self-Sense")] SelfSense = 3,
        [XmlEnum("Database Offset")] DatabaseOffset = 4
    }
}