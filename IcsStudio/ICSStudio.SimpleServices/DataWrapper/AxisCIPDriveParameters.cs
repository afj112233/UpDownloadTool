using System.Collections.Generic;
using Newtonsoft.Json;

namespace ICSStudio.SimpleServices.DataWrapper
{
    public class AxisCIPDriveParameters
    {
        public string MotionGroup { get; set; }

        public string MotionModule { get; set; }
        //

        #region L5X

        // h003,D5D11 checked
        public byte AxisConfiguration { get; set; }
        public byte FeedbackConfiguration { get; set; }
        public byte MotorDataSource { get; set; }
        public string MotorCatalogNumber { get; set; }
        public byte Feedback1Type { get; set; }
        public byte Feedback2Type { get; set; }
        public byte MotorType { get; set; }
        public byte MotionScalingConfiguration { get; set; }
        public float ConversionConstant { get; set; }
        public uint OutputCamExecutionTargets { get; set; }
        public string PositionUnits { get; set; }
        public float AverageVelocityTimebase { get; set; }
        public uint PositionUnwind { get; set; }
        public byte HomeMode { get; set; }
        public byte HomeDirection { get; set; }
        public byte HomeSequence { get; set; }
        public uint HomeConfigurationBits { get; set; }
        public float HomePosition { get; set; }
        public float HomeOffset { get; set; }
        public float HomeSpeed { get; set; }
        public float HomeReturnSpeed { get; set; }
        public float MaximumSpeed { get; set; }
        public float MaximumAcceleration { get; set; }
        public float MaximumDeceleration { get; set; }
        public byte ProgrammedStopMode { get; set; }
        public uint MasterInputConfigurationBits { get; set; }
        public float MasterPositionFilterBandwidth { get; set; }
        public float VelocityFeedforwardGain { get; set; }
        public float AccelerationFeedforwardGain { get; set; }
        public float PositionErrorTolerance { get; set; }
        public float PositionLockTolerance { get; set; }
        public float VelocityOffset { get; set; }
        public float TorqueOffset { get; set; }
        public float FrictionCompensationWindow { get; set; }
        public float BacklashReversalOffset { get; set; }
        public float TuningTravelLimit { get; set; }
        public float TuningSpeed { get; set; }
        public float TuningTorque { get; set; }
        public float DampingFactor { get; set; }
        public float DriveModelTimeConstant { get; set; }
        public float PositionServoBandwidth { get; set; }
        public float VelocityServoBandwidth { get; set; }
        public float VelocityDroop { get; set; }
        public float VelocityLimitPositive { get; set; }
        public float VelocityLimitNegative { get; set; }
        public float VelocityThreshold { get; set; }
        public float VelocityStandstillWindow { get; set; }
        public float TorqueLimitPositive { get; set; }
        public float TorqueLimitNegative { get; set; }
        public float TorqueThreshold { get; set; }
        public float StoppingTorque { get; set; }
        public float StoppingTimeLimit { get; set; }
        public float LoadInertiaRatio { get; set; }
        public byte RegistrationInputs { get; set; }
        public float MaximumAccelerationJerk { get; set; }
        public float MaximumDecelerationJerk { get; set; }
        public uint DynamicsConfigurationBits { get; set; }
        public float FeedbackUnitRatio { get; set; }
        public float AccelerationLimit { get; set; }
        public float DecelerationLimit { get; set; }
        public float CommandTorque { get; set; }
        public float PositionIntegratorBandwidth { get; set; }
        public float PositionErrorToleranceTime { get; set; }
        public byte PositionIntegratorControl { get; set; }
        public float VelocityErrorTolerance { get; set; }
        public float VelocityErrorToleranceTime { get; set; }
        public byte VelocityIntegratorControl { get; set; }
        public float VelocityLockTolerance { get; set; }
        public float SystemInertia { get; set; }
        public float TorqueLowPassFilterBandwidth { get; set; }
        public float TorqueNotchFilterFrequency { get; set; }
        public float TorqueRateLimit { get; set; }
        public float OvertorqueLimit { get; set; }
        public float OvertorqueLimitTime { get; set; }
        public float UndertorqueLimit { get; set; }
        public float UndertorqueLimitTime { get; set; }
        public float FluxCurrentReference { get; set; }
        public float CurrentError { get; set; }
        public float TorqueLoopBandwidth { get; set; }
        public byte StoppingAction { get; set; }
        public byte MechanicalBrakeControl { get; set; }
        public float MechanicalBrakeReleaseDelay { get; set; }
        public float MechanicalBrakeEngageDelay { get; set; }
        public float InverterCapacity { get; set; }
        public float ConverterCapacity { get; set; }
        public byte InverterOverloadAction { get; set; }
        public byte MotorOverloadAction { get; set; }
        public List<byte> CIPAxisExceptionAction { get; set; }
        public List<byte> CIPAxisExceptionActionMfg { get; set; }
        //public List<byte> CIPAxisExceptionActionRA { get; set; }
        public float MotorOverspeedUserLimit { get; set; }
        public float MotorThermalOverloadUserLimit { get; set; }
        public float InverterThermalOverloadUserLimit { get; set; }
        public float PositionLeadLagFilterBandwidth { get; set; }
        public float PositionLeadLagFilterGain { get; set; }
        public float VelocityNegativeFeedforwardGain { get; set; }
        public float BacklashCompensationWindow { get; set; }
        public float TorqueLeadLagFilterBandwidth { get; set; }
        public float TorqueLeadLagFilterGain { get; set; }
        public byte SLATConfiguration { get; set; }
        public float SLATSetPoint { get; set; }
        public float SLATTimeDelay { get; set; }
        public uint MotorDeviceCode { get; set; }
        public byte MotorUnit { get; set; }
        public byte MotorPolarity { get; set; }
        public float MotorRatedVoltage { get; set; }
        public float MotorRatedContinuousCurrent { get; set; }
        public float MotorRatedPeakCurrent { get; set; }
        public float MotorRatedOutputPower { get; set; }
        public float MotorOverloadLimit { get; set; }
        public bool MotorIntegralThermalSwitch { get; set; }
        public float MotorMaxWindingTemperature { get; set; }
        public float MotorWindingToAmbientCapacitance { get; set; }
        public float MotorWindingToAmbientResistance { get; set; }
        public float PMMotorResistance { get; set; }
        public float PMMotorInductance { get; set; }
        public ushort RotaryMotorPoles { get; set; }
        public float RotaryMotorInertia { get; set; }
        public float RotaryMotorRatedSpeed { get; set; }
        public float RotaryMotorMaxSpeed { get; set; }
        public float PMMotorRatedTorque { get; set; }
        public float PMMotorTorqueConstant { get; set; }
        public float PMMotorRotaryVoltageConstant { get; set; }
        public byte Feedback1Unit { get; set; }
        public byte Feedback1Polarity { get; set; }
        public byte Feedback1StartupMethod { get; set; }
        public uint Feedback1CycleResolution { get; set; }
        public uint Feedback1CycleInterpolation { get; set; }
        public uint Feedback1Turns { get; set; }
        public float Feedback1VelocityFilterBandwidth { get; set; }
        public float Feedback1AccelFilterBandwidth { get; set; }
        public List<float> PMMotorFluxSaturation { get; set; }
        public ushort Feedback1VelocityFilterTaps { get; set; }
        public ushort Feedback1AccelFilterTaps { get; set; }
        public List<string> CyclicReadUpdateList { get; set; }
        public List<string> CyclicWriteUpdateList { get; set; }
        public byte ScalingSource { get; set; }
        public byte LoadType { get; set; }
        public byte ActuatorType { get; set; }
        public byte TravelMode { get; set; }
        public float PositionScalingNumerator { get; set; }
        public float PositionScalingDenominator { get; set; }
        public float PositionUnwindNumerator { get; set; }
        public float PositionUnwindDenominator { get; set; }
        public float TravelRange { get; set; }
        public uint MotionResolution { get; set; }
        public byte MotionPolarity { get; set; }
        public float MotorTestResistance { get; set; }
        public float MotorTestInductance { get; set; }
        public float TuneFriction { get; set; }
        public float TuneLoadOffset { get; set; }
        public float TotalInertia { get; set; }
        public byte TuningSelect { get; set; }
        public byte TuningDirection { get; set; }
        public byte ApplicationType { get; set; }
        public byte LoopResponse { get; set; }
        public byte FeedbackCommutationAligned { get; set; }
        public float FrictionCompensationSliding { get; set; }
        public float FrictionCompensationStatic { get; set; }
        public float FrictionCompensationViscous { get; set; }
        public float PositionLoopBandwidth { get; set; }
        public float VelocityLoopBandwidth { get; set; }
        public float VelocityIntegratorBandwidth { get; set; }
        public uint FeedbackDataLossUserLimit { get; set; }
        public List<byte> MotionExceptionAction { get; set; }

        public bool SoftTravelLimitChecking { get; set; }

        // feedback2
        public byte Feedback2Unit { get; set; }
        public byte Feedback2Polarity { get; set; }
        public byte Feedback2StartupMethod { get; set; }
        public uint Feedback2CycleResolution { get; set; }
        public uint Feedback2CycleInterpolation { get; set; }
        public uint Feedback2Turns { get; set; }
        public float Feedback2VelocityFilterBandwidth { get; set; }
        public float Feedback2AccelFilterBandwidth { get; set; }
        public ushort Feedback2VelocityFilterTaps { get; set; }

        public ushort Feedback2AccelFilterTaps { get; set; }

        //end feedback2
        public float LoadRatio { get; set; }
        public float TuneInertiaMass { get; set; }
        public float SoftTravelLimitPositive { get; set; }
        public float SoftTravelLimitNegative { get; set; }
        public ushort GainTuningConfigurationBits { get; set; }
        public float CommutationOffset { get; set; }
        public float SystemBandwidth { get; set; }
        public float VelocityLowPassFilterBandwidth { get; set; }
        public uint TransmissionRatioInput { get; set; }
        public uint TransmissionRatioOutput { get; set; }
        public float ActuatorLead { get; set; }
        public byte ActuatorLeadUnit { get; set; }
        public float ActuatorDiameter { get; set; }
        public byte ActuatorDiameterUnit { get; set; }
        public float SystemAccelerationBase { get; set; }
        public float DriveModelTimeConstantBase { get; set; }
        public float DriveRatedPeakCurrent { get; set; }
        public float HookupTestDistance { get; set; }
        public byte HookupTestFeedbackChannel { get; set; }
        public byte LoadCoupling { get; set; }
        public float SystemDamping { get; set; }
        public float CurrentVectorLimit { get; set; }
        public byte CommutationPolarity { get; set; }
        public byte LoadObserverConfiguration { get; set; }
        public float LoadObserverBandwidth { get; set; }
        public float LoadObserverIntegratorBandwidth { get; set; }
        public float LoadObserverFeedbackGain { get; set; }
        public uint AxisID { get; set; }
        public uint InterpolatedPositionConfiguration { get; set; }
        public byte AxisUpdateSchedule { get; set; }
        public byte ProvingConfiguration { get; set; }
        public float TorqueProveCurrent { get; set; }
        public float BrakeTestTorque { get; set; }
        public float BrakeSlipTolerance { get; set; }
        public float ZeroSpeed { get; set; }
        public float ZeroSpeedTime { get; set; }
        public byte AdaptiveTuningConfiguration { get; set; }
        public float TorqueNotchFilterHighFrequencyLimit { get; set; }
        public float TorqueNotchFilterLowFrequencyLimit { get; set; }
        public float TorqueNotchFilterTuningThreshold { get; set; }
        public float CoastingTimeLimit { get; set; }
        public float BusOvervoltageOperationalLimit { get; set; }

        public float SkipSpeed1 { get; set; }
        public float SkipSpeed2 { get; set; }
        public float SkipSpeedBand { get; set; }
        public byte FluxUpControl { get; set; }
        public float FluxUpTime { get; set; }
        public byte FrequencyControlMethod { get; set; }
        public float MaximumVoltage { get; set; }
        public float MaximumFrequency { get; set; }
        public float BreakVoltage { get; set; }
        public float BreakFrequency { get; set; }
        public float StartBoost { get; set; }
        public float RunBoost { get; set; }
        public float InductionMotorRatedFrequency { get; set; }
        public float InductionMotorFluxCurrent { get; set; }
        public float InductionMotorStatorResistance { get; set; }
        public float InductionMotorStatorLeakageReactance { get; set; }
        public float InductionMotorRotorLeakageReactance { get; set; }
        public float MotorTestFluxCurrent { get; set; }
        public float MotorTestSlipSpeed { get; set; }
        //public float HookupTestTime { get; set; }
        public float InductionMotorRatedSlipSpeed { get; set; }
        //public float DriveRatedVoltage { get; set; }
        //public float MaxOutputFrequency { get; set; }
        //public bool MotorTestDataValid { get; set; } //TODO(gjc): need check
        //public float HookupTestSpeed { get; set; }
        #endregion

        public byte CommutationAlignment { get; set; }
        public byte ControlMethod { get; set; }
        public byte ControlMode { get; set; }
        public byte FeedbackMode { get; set; }
        public float PositionTrim { get; set; }
        public float TorqueTrim { get; set; }
        public float VelocityTrim { get; set; }

        [JsonIgnore] public AxisCIPDrive AxisCIPDrive { get; set; }

        public bool SupportAttribute(string propertyName)
        {
            if (AxisCIPDrive != null)
                return AxisCIPDrive.SupportAttribute(propertyName);

            return false;
        }
    }

}
