using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ICSStudio.Cip.DataTypes;
using ICSStudio.Cip.EtherNetIP;

namespace ICSStudio.Cip.Objects
{
    public class CIPAxis : CipBaseObject, ICloneable
    {
        public CIPAxis(ushort instanceId, ICipMessager messager)
            : base((ushort)CipObjectClassCode.Axis, instanceId, messager)
        {
            AxisUpdateSchedule = 0; // Base

            SetDefaultAttributeValue(2);
        }

        #region Private Method

        private void SetDefaultAttributeValue(byte axisConfiguration)
        {
            // common default
            Feedback1Length = 1;

            switch (axisConfiguration)
            {
                case 0:
                    SetFeedbackOnlyDefaultValue();
                    break;
                case 2:
                    SetPositionLoopDefaultValue();
                    break;
                default:
                    // TODO(gjc): add other default value
                    throw new NotImplementedException();
            }
        }

        private void SetFeedbackOnlyDefaultValue()
        {
            AxisConfiguration = 0; // Feedback Only
            ControlMethod = 0;
            ControlMode = 0;

            FeedbackConfiguration = 1; // Master Feedback
            FeedbackMode = 1;

            MotorCatalogNumber = "<none>";
            Feedback1Type = 0; // Not Specified
            MotionScalingConfiguration = 0; // Control Scaling
            ConversionConstant = 1000000f;
            OutputCamExecutionTargets = 0;
            PositionUnits = "Position Units";
            AverageVelocityTimebase = 0.25f;
            PositionUnwind = 1000000;
            HomeMode = 0; // Passive
            HomeSequence = 0; // Immediate
            HomeConfigurationBits = 0;
            HomePosition = 0.0f;
            HomeOffset = 0.0f;

            ProgrammedStopMode = 0; // Fast Stop

            MasterInputConfigurationBits = 1;
            MasterPositionFilterBandwidth = 0.1f;
            VelocityStandstillWindow = 1.0f;

            RegistrationInputs = 1;

            if (CIPAxisExceptionAction == null)
                CIPAxisExceptionAction = new CipByteArray(64);
            if (CIPAxisExceptionActionMfg == null)
                CIPAxisExceptionActionMfg = new CipByteArray(64);
            if (CIPAxisExceptionActionRA == null)
                CIPAxisExceptionActionRA = new CipByteArray(64);
            for (var i = 0; i < 64; i++)
            {
                CIPAxisExceptionAction.SetValue(i, 255);
                CIPAxisExceptionActionMfg.SetValue(i, 255);
                CIPAxisExceptionActionRA.SetValue(i, 255);
            }

            Feedback1Unit = 0; // Rev
            ScalingSource = 0; // From Calculator
            LoadType = 0; // Direct Rotary
            ActuatorType = 0; // None
            TravelMode = 0; // Unlimited
            PositionScalingNumerator = 1.0f;
            PositionScalingDenominator = 1.0f;
            PositionUnwindNumerator = 1.0f;
            PositionUnwindDenominator = 1.0f;
            TravelRange = 1000.0f;
            MotionResolution = 1000000; // page 96,rm003
            MotionPolarity = 0; // Normal

            if (MotionExceptionAction == null)
                MotionExceptionAction = new CipByteArray(32);
            for (var i = 0; i < 32; i++)
                MotionExceptionAction.SetValue(i, 255);
            MotionExceptionAction.SetValue(1, 2);
            MotionExceptionAction.SetValue(2, 2);

            SoftTravelLimitChecking = 0; // false
            SoftTravelLimitPositive = 0.0f;
            SoftTravelLimitNegative = 0.0f;

            TransmissionRatioInput = 1;
            TransmissionRatioOutput = 1;
            ActuatorLead = 1.0f;
            ActuatorLeadUnit = 0; // Millimeter/Rev
            ActuatorDiameter = 1.0f;
            ActuatorDiameterUnit = 0; // Millimeter
            SystemAccelerationBase = 0.0f;
            DriveModelTimeConstantBase = 0.0015f;
            DriveRatedPeakCurrent = 0.0f;
            HookupTestDistance = 1.0f;
            HookupTestFeedbackChannel = 1; // Feedback 1

            InterpolatedPositionConfiguration = 0x2;
        }

        private void SetPositionLoopDefaultValue()
        {
            AxisConfiguration = 2; // Position Loop
            ControlMethod = 2;
            ControlMode = 1;

            FeedbackConfiguration = 2; // Motor Feedback
            FeedbackMode = 2;

            MotorDataSource = 0; // Nameplate Datasheet
            MotorCatalogNumber = "<none>";
            Feedback1Type = 0; // Not Specified
            MotionScalingConfiguration = 0; // Control Scaling
            ConversionConstant = 1000000f;
            OutputCamExecutionTargets = 0;
            PositionUnits = "Position Units";
            AverageVelocityTimebase = 0.25f;
            PositionUnwind = 1000000;
            HomeMode = 1; // Active
            HomeDirection = 1; // Bi-directional Forward
            HomeSequence = 0; // Immediate
            HomeConfigurationBits = 0;
            HomePosition = 0.0f;
            HomeOffset = 0.0f;
            HomeSpeed = 0.0f;
            HomeReturnSpeed = 0.0f;

            MaximumSpeed = 0.0f;
            MaximumAcceleration = 0.0f;
            MaximumDeceleration = 0.0f;

            ProgrammedStopMode = 0; // Fast Stop

            MasterInputConfigurationBits = 1;
            MasterPositionFilterBandwidth = 0.1f;
            VelocityFeedforwardGain = 0.0f;
            AccelerationFeedforwardGain = 0.0f;
            PositionErrorTolerance = 0.0f;
            PositionLockTolerance = 0.0f;
            VelocityOffset = 0.0f;
            TorqueOffset = 0.0f;
            BacklashReversalOffset = 0.0f;

            TuningTravelLimit = 0.0f;
            TuningSpeed = 0.0f;
            TuningTorque = 100.0f;
            DampingFactor = 1.0f;

            DriveModelTimeConstant = 0.0015f;
            PositionServoBandwidth = 16.0f;
            VelocityServoBandwidth = 40.0f;
            VelocityStandstillWindow = 1.0f;

            TorqueLimitPositive = 0.0f;
            TorqueLimitNegative = 0.0f;
            StoppingTorque = 0.0f;
            LoadInertiaRatio = 0.0f;
            RegistrationInputs = 1;
            MaximumAccelerationJerk = 0.0f;
            MaximumDecelerationJerk = 0.0f;
            DynamicsConfigurationBits = 7;
            PositionIntegratorBandwidth = 0.0f;
            PositionIntegratorControl = 0;
            VelocityIntegratorControl = 0;
            SystemInertia = 0.0f;
            StoppingAction = 1; // Current Decel Disable
            InverterCapacity = 0.0f;

            if (CIPAxisExceptionAction == null)
                CIPAxisExceptionAction = new CipByteArray(64);
            if (CIPAxisExceptionActionMfg == null)
                CIPAxisExceptionActionMfg = new CipByteArray(64);
            if (CIPAxisExceptionActionRA == null)
                CIPAxisExceptionActionRA = new CipByteArray(64);
            for (var i = 0; i < 64; i++)
            {
                CIPAxisExceptionAction.SetValue(i, 255);
                CIPAxisExceptionActionMfg.SetValue(i, 255);
                CIPAxisExceptionActionRA.SetValue(i, 255);
            }

            MotorUnit = 0; // Rev
            Feedback1Unit = 0; // Rev
            ScalingSource = 0; // From Calculator
            LoadType = 0; // Direct Rotary
            ActuatorType = 0; // None
            TravelMode = 0; // Unlimited
            PositionScalingNumerator = 1.0f;
            PositionScalingDenominator = 1.0f;
            PositionUnwindNumerator = 1.0f;
            PositionUnwindDenominator = 1.0f;
            TravelRange = 1000.0f;
            MotionResolution = 1000000;
            MotionPolarity = 0; // Normal
            MotorTestResistance = 0.0f;
            MotorTestInductance = 0.0f;
            TuneFriction = 0.0f;
            TuneLoadOffset = 0.0f;
            TuningSelect = 0; // Total Inertia
            TuningDirection = 0; // Unidirectional Forward
            ApplicationType = 1; // Basic
            LoopResponse = 1; // Medium
            PositionLoopBandwidth = 0.0f;
            VelocityLoopBandwidth = 0.0f;
            VelocityIntegratorBandwidth = 0.0f;

            // TODO(gjc): add Soft Travel Limit
            if (MotionExceptionAction == null)
                MotionExceptionAction = new CipByteArray(32);
            for (var i = 0; i < 32; i++)
                MotionExceptionAction.SetValue(i, 255);
            MotionExceptionAction.SetValue(1, 4);
            MotionExceptionAction.SetValue(2, 4);

            SoftTravelLimitChecking = 0; // false
            LoadRatio = 0.0f;
            TuneInertiaMass = 0.0f;
            SoftTravelLimitPositive = 0.0f;
            SoftTravelLimitNegative = 0.0f;
            GainTuningConfigurationBits = 0x141;
            SystemBandwidth = 0.0f;
            TransmissionRatioInput = 1;
            TransmissionRatioOutput = 1;
            ActuatorLead = 1.0f;
            ActuatorLeadUnit = 0; // Millimeter/Rev
            ActuatorDiameter = 1.0f;
            ActuatorDiameterUnit = 0; // Millimeter
            SystemAccelerationBase = 0.0f;
            DriveModelTimeConstantBase = 0.0015f;

            HookupTestDistance = 1.0f;
            HookupTestFeedbackChannel = 1; // Feedback 1
            LoadCoupling = 0; // Rigid
            SystemDamping = DampingFactor;
            InterpolatedPositionConfiguration = 0x2;

            BusOvervoltageOperationalLimit = 0.0f;
            DriveRatedPeakCurrent = 0.0f;

            // not found in xml
            MotionUnit = 0;
            CommutationOffset = 0;
            RotaryMotorPoles = 8;

            MotorOverloadLimit = 100;

            if (PMMotorFluxSaturation == null)
                PMMotorFluxSaturation = new CipRealArray(8);
            for (var i = 0; i < 8; i++)
                PMMotorFluxSaturation.SetValue(i, 100);

            TorqueLeadLagFilterGain = 1.0f;
            FeedbackDataLossUserLimit = 4;

            HookupTestTime = 10f;

            ConverterThermalOverloadUserLimit = 100f;
            InverterThermalOverloadUserLimit = 110f;

            LoadObserverFeedbackGain = 0.5f;

            MotorThermalOverloadUserLimit = 110f;

            OvertorqueLimit = 200f;

            TorqueRateLimit = 1000000f;

            TorqueNotchFilterHighFrequencyLimit = 2000;
            TorqueNotchFilterLowFrequencyLimit = 20;
            TorqueNotchFilterTuningThreshold = 5;

            Feedback1AccelFilterTaps = 1;
            Feedback1VelocityFilterTaps = 1;

            Feedback2AccelFilterTaps = 1;
            Feedback2VelocityFilterTaps = 1;
            FeedbackUnitRatio = 1f;

            MotorOverspeedUserLimit = 120;
            StoppingTimeLimit = 1;

            StoppingTorque = 100;
            TorqueThreshold = 90;

            UndertorqueLimit = 10;
            VelocityErrorToleranceTime = 0.01f;
            VelocityStandstillWindow = 1;
            VelocityThreshold = 0;

            ZeroSpeed = 1;

            // Frequency Control
            MaximumVoltage = 460.0f;
            MaximumFrequency = 130.0f;
            BreakVoltage = 230.0f;
            BreakFrequency = 30.0f;
            StartBoost = 8.5f;
            RunBoost = 8.5f;
        }

        #endregion

        #region Cip Attribute

        [CipDetailInfo((ushort)AxisAttributeId.AxisConfiguration, "Axis Configuration")]
        public CipUsint AxisConfiguration { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.FeedbackConfiguration, "Feedback Configuration")]
        public CipUsint FeedbackConfiguration { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.ApplicationType, "Application Type")]
        public CipUsint ApplicationType { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.LoopResponse, "Loop Response")]
        public CipUsint LoopResponse { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.AxisUpdateSchedule, "Axis Update Schedule")]
        public CipUsint AxisUpdateSchedule { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.AxisID, "Axis ID")]
        public CipUdint AxisID { get; set; }


        /// <summary>
        ///     Indicates the operating state of the axis.
        /// </summary>
        [CipDetailInfo((ushort)AxisAttributeId.AxisState, "Axis State")]
        public CipUsint AxisState { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MotionFaultBits, "Motion Fault Bits")]
        public CipUdint MotionFaultBits { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.AxisStatusBits, "Axis Status Bits")]
        public CipUdint AxisStatusBits { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.AxisFaultBits, "Axis Fault Bits")]
        public CipUdint AxisFaultBits { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.ModuleFaultBits, "Module Fault Bits")]
        public CipUdint ModuleFaultBits { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.AttributeErrorCode, "Attribute Error Code")]
        public CipUint AttributeErrorCode { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.AttributeErrorID, "Attribute Error ID")]
        // ReSharper disable once InconsistentNaming
        public CipUint AttributeErrorID { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.CIPAxisState, "CIP Axis State")]
        public CipUsint CIPAxisState { get; set; }

        /// <summary>
        ///     indicating the internal status conditions of the motion axis
        /// </summary>
        [CipDetailInfo((ushort)AxisAttributeId.CIPAxisStatus, "CIP Axis Status")]
        public CipUdint CIPAxisStatus { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.CIPAxisIOStatus, "CIP Axis I/O Status")]
        public CipUdint CIPAxisIOStatus { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.CIPAxisIOStatusRA, "CIP Axis I/O Status-RA")]
        public CipUdint CIPAxisIOStatusRA { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.CIPAxisFaults, "CIP Axis Faults")]
        public CipUlint CIPAxisFaults { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.CIPAxisFaultsRA, "CIP Axis Faults-RA")]
        public CipUlint CIPAxisFaultsRA { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.CIPInitializationFaults, "CIP Initialization Faults")]
        public CipUdint CIPInitializationFaults { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.CIPStartInhibits, "CIP Start Inhibits")]
        public CipUint CIPStartInhibits { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.CIPAPRFaults, "CIP APR Faults")]
        public CipUint CIPAPRFaults { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.GuardFaults, "Guard Faults")]
        public CipUdint GuardFaults { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.GuardStatus, "Guard Status")]
        public CipUdint GuardStatus { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.ConverterACInputPhasing, "Converter AC Input Phasing")]
        public CipUsint ConverterACInputPhasing { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.ConverterACInputVoltage, "Converter AC Input Voltage")]
        public CipUint ConverterACInputVoltage { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MotorElectricalAngle, "Motor Electrical Angle")]
        public CipReal MotorElectricalAngle { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.Feedback1Type, "Feedback 1 Type")]
        public CipUsint Feedback1Type { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.Feedback1Unit, "Feedback 1 Unit")]
        public CipUsint Feedback1Unit { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.Feedback1AccelFilterBandwidth, "Feedback 1 Accel Filter Bandwidth")]
        public CipReal Feedback1AccelFilterBandwidth { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.Feedback1AccelFilterTaps, "Feedback 1 Accel Filter Taps")]
        public CipUint Feedback1AccelFilterTaps { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.Feedback1CycleInterpolation, "Feedback 1 Cycle Interpolation")]
        public CipUdint Feedback1CycleInterpolation { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.Feedback1CycleResolution, "Feedback 1 Cycle Resolution")]
        public CipUdint Feedback1CycleResolution { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.Feedback1StartupMethod, "Feedback 1 Startup Method")]
        public CipUsint Feedback1StartupMethod { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.Feedback1Turns, "Feedback 1 Turns")]
        public CipUdint Feedback1Turns { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.Feedback1Length, "Feedback 1 Length")]
        public CipReal Feedback1Length { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.Feedback1VelocityFilterBandwidth,
            "Feedback 1 Velocity Filter Bandwidth")]
        public CipReal Feedback1VelocityFilterBandwidth { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.Feedback1VelocityFilterTaps, "Feedback 1 Velocity Filter Taps")]
        public CipUint Feedback1VelocityFilterTaps { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.Feedback1Polarity, "Feedback 1 Polarity")]
        public CipUsint Feedback1Polarity { get; set; }

        // Feedback 2
        [CipDetailInfo((ushort)AxisAttributeId.FeedbackUnitRatio, "Feedback Unit Ratio")]
        public CipReal FeedbackUnitRatio { get; set; }


        [CipDetailInfo((ushort)AxisAttributeId.Feedback2Type, "Feedback 2 Type")]
        public CipUsint Feedback2Type { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.Feedback2Unit, "Feedback 2 Unit")]
        public CipUsint Feedback2Unit { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.Feedback2CycleInterpolation, "Feedback 2 Cycle Interpolation")]
        public CipUdint Feedback2CycleInterpolation { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.Feedback2CycleResolution, "Feedback 2 Cycle Resolution")]
        public CipUdint Feedback2CycleResolution { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.Feedback2StartupMethod, "Feedback 2 Startup Method")]
        public CipUsint Feedback2StartupMethod { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.Feedback2Turns, "Feedback 2 Turns")]
        public CipUdint Feedback2Turns { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.Feedback2Length, "Feedback 2 Length")]
        public CipReal Feedback2Length { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.Feedback2AccelFilterBandwidth, "Feedback 2 Accel Filter Bandwidth")]
        public CipReal Feedback2AccelFilterBandwidth { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.Feedback2AccelFilterTaps, "Feedback 2 Accel Filter Taps")]
        public CipUint Feedback2AccelFilterTaps { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.Feedback2VelocityFilterBandwidth,
            "Feedback 2 Velocity Filter Bandwidth")]
        public CipReal Feedback2VelocityFilterBandwidth { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.Feedback2VelocityFilterTaps, "Feedback 2 Velocity Filter Taps")]
        public CipUint Feedback2VelocityFilterTaps { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.Feedback2Polarity, "Feedback 2 Polarity")]
        public CipUsint Feedback2Polarity { get; set; }

        //
        [CipDetailInfo((ushort)AxisAttributeId.LoadType, "Load Type")]
        public CipUsint LoadType { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TransmissionRatioInput, "Transmission Ratio Input")]
        public CipUdint TransmissionRatioInput { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TransmissionRatioOutput, "Transmission Ratio Output")]
        public CipUdint TransmissionRatioOutput { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.ActuatorType, "Actuator Type")]
        public CipUsint ActuatorType { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.ActuatorLead, "Actuator Lead")]
        public CipReal ActuatorLead { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.ActuatorLeadUnit, "Actuator Lead Unit")]
        public CipUsint ActuatorLeadUnit { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.ActuatorDiameter, "Actuator Diameter")]
        public CipReal ActuatorDiameter { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.ActuatorDiameterUnit, "Actuator Diameter Unit")]
        public CipUsint ActuatorDiameterUnit { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.PositionUnits, "Position Units")]
        public CipString PositionUnits { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.PositionScalingNumerator, "Position Scaling Numerator")]
        public CipReal PositionScalingNumerator { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.PositionScalingDenominator, "Position Scaling Denominator")]
        public CipReal PositionScalingDenominator { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MotionUnit, "Motion Unit")]
        public CipUsint MotionUnit { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TravelMode, "Travel Mode")]
        public CipUsint TravelMode { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TravelRange, "Travel Range")]
        public CipReal TravelRange { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.PositionUnwind, "Position Unwind")]
        public CipUdint PositionUnwind { get; set; } // 1-10^9

        [CipDetailInfo((ushort)AxisAttributeId.PositionUnwindNumerator, "Position Unwind Numerator")]
        public CipReal PositionUnwindNumerator { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.PositionUnwindDenominator, "Position Unwind Denominator")]
        public CipReal PositionUnwindDenominator { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.SoftTravelLimitChecking, "Soft Travel Limit Checking")]
        public CipUsint SoftTravelLimitChecking { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.SoftTravelLimitPositive, "Soft Travel Limit - Positive")]
        public CipReal SoftTravelLimitPositive { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.SoftTravelLimitNegative, "Soft Travel Limit - Negative")]
        public CipReal SoftTravelLimitNegative { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.ScalingSource, "Scaling Source")]
        public CipUsint ScalingSource { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.FeedbackCommutationAligned, "Feedback Commutation Aligned")]
        public CipUsint FeedbackCommutationAligned { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.CommutationOffset, "Commutation Offset")]
        public CipReal CommutationOffset { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.CommutationPolarity, "Commutation Polarity")]
        public CipUsint CommutationPolarity { get; set; }

        // Motor
        [CipDetailInfo((ushort)AxisAttributeId.MotorDataSource, "Motor Data Source")]
        public CipUsint MotorDataSource { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MotorCatalogNumber, "Motor Catalog Number")]
        public CipShortString MotorCatalogNumber { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MotorType, "Motor Type")]
        public CipUsint MotorType { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MotorUnit, "Motor Unit")]
        public CipUsint MotorUnit { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MotorDeviceCode, "Motor Device Code")]
        public CipUdint MotorDeviceCode { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MotorIntegralThermalSwitch, "Motor Integral Thermal Switch")]
        public CipUsint MotorIntegralThermalSwitch { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MotorMaxWindingTemperature, "Motor Max Winding Temperature")]
        public CipReal MotorMaxWindingTemperature { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MotorWindingToAmbientCapacitance, "Motor Winding To Ambient Capacitance"
        )]
        public CipReal MotorWindingToAmbientCapacitance { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MotorWindingToAmbientResistance, "Motor Winding To Ambient Resistance")]
        public CipReal MotorWindingToAmbientResistance { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MotorTestResistance, "Motor Test Resistance")]
        public CipReal MotorTestResistance { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MotorTestInductance, "Motor Test Inductance")]
        public CipReal MotorTestInductance { get; set; }


        [CipDetailInfo((ushort)AxisAttributeId.MotorRatedOutputPower, "Motor Rated Output Power")]
        public CipReal MotorRatedOutputPower { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MotorRatedVoltage, "Motor Rated Voltage")]
        public CipReal MotorRatedVoltage { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.RotaryMotorRatedSpeed, "Rotary Motor Rated Speed")]
        public CipReal RotaryMotorRatedSpeed { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MotorRatedContinuousCurrent, "Motor Rated Continuous Current")]
        public CipReal MotorRatedContinuousCurrent { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.PMMotorRatedTorque, "PM Motor Rated Torque")]
        public CipReal PMMotorRatedTorque { get; set; }

        //PM: 8
        //IM: 4
        [CipDetailInfo((ushort)AxisAttributeId.RotaryMotorPoles, "Rotary Motor Poles")]
        public CipUint RotaryMotorPoles { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.RotaryMotorMaxSpeed, "Rotary Motor Max Speed")]
        public CipReal RotaryMotorMaxSpeed { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MotorRatedPeakCurrent, "Motor Rated Peak Current")]
        public CipReal MotorRatedPeakCurrent { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MotorOverloadLimit, "Motor Overload Limit")]
        public CipReal MotorOverloadLimit { get; set; }

        // linear
        [CipDetailInfo((ushort)AxisAttributeId.LinearMotorRatedSpeed, "Linear Motor Rated Speed")]
        public CipReal LinearMotorRatedSpeed { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.PMMotorRatedForce, "PM Motor Rated Force")]
        public CipReal PMMotorRatedForce { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.LinearMotorPolePitch, "Linear Motor Pole Pitch")]
        public CipReal LinearMotorPolePitch { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.LinearMotorMaxSpeed, "Linear Motor Max Speed")]
        public CipReal LinearMotorMaxSpeed { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.InductionMotorRatedFrequency, "Induction Motor Rated Frequency")]
        public CipReal InductionMotorRatedFrequency { get; set; }

        // Model
        [CipDetailInfo((ushort)AxisAttributeId.PMMotorTorqueConstant, "PM Motor Torque Constant")]
        public CipReal PMMotorTorqueConstant { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.PMMotorRotaryVoltageConstant, "PM Motor Rotary Voltage Constant")]
        public CipReal PMMotorRotaryVoltageConstant { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.PMMotorResistance, "PM Motor Resistance")]
        public CipReal PMMotorResistance { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.PMMotorInductance, "PM Motor Inductance")]
        public CipReal PMMotorInductance { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.PMMotorFluxSaturation, "PM Motor Flux Saturation", ArraySize = 8)]
        public CipRealArray PMMotorFluxSaturation { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.PMMotorForceConstant, "PM Motor Force Constant")]
        public CipReal PMMotorForceConstant { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.PMMotorLinearVoltageConstant, "PM Motor Linear Voltage Constant")]
        public CipReal PMMotorLinearVoltageConstant { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.InductionMotorFluxCurrent, "Induction Motor Flux Current")]
        public CipReal InductionMotorFluxCurrent { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.InductionMotorRatedSlipSpeed, "Induction Motor Rated Slip Speed")]
        public CipReal InductionMotorRatedSlipSpeed { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.InductionMotorStatorLeakageReactance,
            "Induction Motor Stator Leakage Reactance")]
        public CipReal InductionMotorStatorLeakageReactance { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.InductionMotorRotorLeakageReactance,
            "Induction Motor Rotor Leakage Reactance")]
        public CipReal InductionMotorRotorLeakageReactance { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.InductionMotorStatorResistance, "Induction Motor Stator Resistance")]
        public CipReal InductionMotorStatorResistance { get; set; }


        // Polarity
        [CipDetailInfo((ushort)AxisAttributeId.MotionPolarity, "Motion Polarity")]
        public CipUsint MotionPolarity { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MotorPolarity, "Motor Polarity")]
        public CipUsint MotorPolarity { get; set; }


        // Autotune
        [CipDetailInfo((ushort)AxisAttributeId.LoadCoupling, "Load Coupling")]
        public CipUsint LoadCoupling { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.GainTuningConfigurationBits, "Gain Tuning Configuration Bits")]
        public CipUint GainTuningConfigurationBits { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TuningSelect, "Tuning Select")]
        public CipUsint TuningSelect { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TuningTravelLimit, "Tuning Travel Limit")]
        public CipReal TuningTravelLimit { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TuningSpeed, "Tuning Speed")]
        public CipReal TuningSpeed { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TuningTorque, "Tuning Torque")]
        public CipReal TuningTorque { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TuningDirection, "Tuning Direction")]
        public CipUsint TuningDirection { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TuneFriction, "Tune Friction", "% Rated")]
        public CipReal TuneFriction { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TuneInertiaMass, "Tune Inertia Mass", "% Rated/(Rev/s^2)")]
        public CipReal TuneInertiaMass { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TuneLoadOffset, "Tune Load Offset", "% Rated")]
        public CipReal TuneLoadOffset { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TuneAcceleration, "Tune Acceleration", "$Units/s^2")]
        public CipReal TuneAcceleration { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TuneDeceleration, "Tune Deceleration", "$Units/s^2")]
        public CipReal TuneDeceleration { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TuneAccelerationTime, "Tune Acceleration Time", "s")]
        public CipReal TuneAccelerationTime { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TuneDecelerationTime, "Tune Deceleration Time", "s")]
        public CipReal TuneDecelerationTime { get; set; }

        // Hookup Test
        [CipDetailInfo((ushort)AxisAttributeId.HookupTestDistance, "Hookup Test Distance")]
        public CipReal HookupTestDistance { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.HookupTestTime, "Hookup Test Time")]
        public CipReal HookupTestTime { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.HookupTestFeedbackChannel, "Hookup Test Feedback Channel")]
        public CipUsint HookupTestFeedbackChannel { get; set; }

        // Load
        [CipDetailInfo((ushort)AxisAttributeId.LoadRatio, "Load Ratio", "Load Inertia/Motor Inertia")]
        public CipReal LoadRatio { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.LoadInertiaRatio, "Load InertiaRatio")]
        public CipReal LoadInertiaRatio { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.RotaryMotorInertia, "Rotary Motor Inertia", "kg-m^2")]
        public CipReal RotaryMotorInertia { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.LinearMotorMass, "Linear Motor Mass")]
        public CipReal LinearMotorMass { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TotalInertia, "Total Inertia", "kg-m^2")]
        public CipReal TotalInertia { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TotalMass, "Total Mass")]
        public CipReal TotalMass { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.SystemInertia, "System Inertia", "% Rated/(Rev/s^2)")]
        public CipReal SystemInertia { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TorqueOffset, "Torque Offset", "% Motor Rated")]
        public CipReal TorqueOffset { get; set; }

        // Backlash
        [CipDetailInfo((ushort)AxisAttributeId.BacklashReversalOffset, "Backlash Reversal Offset")]
        public CipReal BacklashReversalOffset { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.BacklashCompensationWindow, "Backlash Compensation Window")]
        public CipReal BacklashCompensationWindow { get; set; }

        // Compliance
        [CipDetailInfo((ushort)AxisAttributeId.TorqueLowPassFilterBandwidth, "Torque Low Pass Filter Bandwidth", "Hz")]
        public CipReal TorqueLowPassFilterBandwidth { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TorqueNotchFilterFrequency, "Torque Notch Filter Frequency")]
        public CipReal TorqueNotchFilterFrequency { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TorqueLeadLagFilterGain, "Torque Lead Lag Filter Gain")]
        public CipReal TorqueLeadLagFilterGain { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TorqueLeadLagFilterBandwidth, "Torque Lead Lag Filter Bandwidth")]
        public CipReal TorqueLeadLagFilterBandwidth { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.AdaptiveTuningConfiguration, "Adaptive Tuning Configuration")]
        public CipUsint AdaptiveTuningConfiguration { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TorqueNotchFilterHighFrequencyLimit,
            "Torque Notch Filter High Frequency Limit")]
        public CipReal TorqueNotchFilterHighFrequencyLimit { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TorqueNotchFilterLowFrequencyLimit,
            "Torque Notch Filter Low Frequency Limit")]
        public CipReal TorqueNotchFilterLowFrequencyLimit { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TorqueNotchFilterTuningThreshold, "Torque Notch Filter Tuning Threshold"
        )]
        public CipReal TorqueNotchFilterTuningThreshold { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TorqueNotchFilterFrequencyEstimate,
            "Torque Notch Filter Frequency Estimate"
        )]
        public CipReal TorqueNotchFilterFrequencyEstimate { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TorqueNotchFilterMagnitudeEstimate,
            "Torque Notch Filter Magnitude Estimate"
        )]
        public CipReal TorqueNotchFilterMagnitudeEstimate { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TorqueLowPassFilterBandwidthEstimate,
            "Torque Low Pass Filter Bandwidth Estimate"
        )]
        public CipReal TorqueLowPassFilterBandwidthEstimate { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.AdaptiveTuningGainScalingFactor, "Adaptive Tuning Gain Scaling Factor"
        )]
        public CipReal AdaptiveTuningGainScalingFactor { get; set; }

        // Friction
        [CipDetailInfo((ushort)AxisAttributeId.FrictionCompensationSliding, "Friction Compensation Sliding",
            "% Motor Rated")]
        public CipReal FrictionCompensationSliding { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.FrictionCompensationStatic, "Friction Compensation Static")]
        public CipReal FrictionCompensationStatic { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.FrictionCompensationViscous, "Friction Compensation Viscous")]
        public CipReal FrictionCompensationViscous { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.FrictionCompensationWindow, "Friction Compensation Window")]
        public CipReal FrictionCompensationWindow { get; set; }

        // Observer
        [CipDetailInfo((ushort)AxisAttributeId.LoadObserverConfiguration, "Load Observer Configuration")]
        public CipUsint LoadObserverConfiguration { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.LoadObserverBandwidth, "Load Observer Bandwidth", "Hz")]
        public CipReal LoadObserverBandwidth { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.LoadObserverIntegratorBandwidth, "Load Observer Integrator Bandwidth",
            "Hz")]
        public CipReal LoadObserverIntegratorBandwidth { get; set; }

        // Position Loop
        [CipDetailInfo((ushort)AxisAttributeId.PositionLoopBandwidth, "Position Loop Bandwidth", "Hz")]
        public CipReal PositionLoopBandwidth { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.PositionIntegratorBandwidth, "Position Integrator Bandwidth", "Hz")]
        public CipReal PositionIntegratorBandwidth { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.PositionIntegratorControl, "Position Integrator Control")]
        public CipUsint PositionIntegratorControl { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.VelocityFeedforwardGain, "Velocity Feedforward Gain", "%")]
        public CipReal VelocityFeedforwardGain { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.PositionErrorTolerance, "Position Error Tolerance", "$Units")]
        public CipReal PositionErrorTolerance { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.PositionErrorToleranceTime, "Position Error Tolerance Time")]
        public CipReal PositionErrorToleranceTime { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.PositionLeadLagFilterBandwidth, "Position Lead Lag Filter Bandwidth")]
        public CipReal PositionLeadLagFilterBandwidth { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.PositionLeadLagFilterGain, "Position Lead Lag Filter Gain")]
        public CipReal PositionLeadLagFilterGain { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.PositionTrim, "Position Trim")]
        public CipReal PositionTrim { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.PositionFeedback1, "Position Feedback 1")]

        public CipDint PositionFeedback1 { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.PositionFeedback2, "Position Feedback 2")]

        public CipDint PositionFeedback2 { get; set; }


        [CipDetailInfo((ushort)AxisAttributeId.PositionLockTolerance, "Position Lock Tolerance")]
        public CipReal PositionLockTolerance { get; set; }

        // Velocity Loop
        [CipDetailInfo((ushort)AxisAttributeId.VelocityLoopBandwidth, "Velocity Loop Bandwidth", "Hz")]
        public CipReal VelocityLoopBandwidth { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.VelocityIntegratorBandwidth, "Velocity Integrator Bandwidth", "Hz")]
        public CipReal VelocityIntegratorBandwidth { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.VelocityIntegratorControl, "Velocity Integrator Control")]
        public CipUsint VelocityIntegratorControl { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.AccelerationFeedforwardGain, "Acceleration Feedforward Gain", "%")]
        public CipReal AccelerationFeedforwardGain { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.VelocityErrorTolerance, "Velocity Error Tolerance", "$Units/s")]
        public CipReal VelocityErrorTolerance { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.VelocityLockTolerance, "Velocity Lock Tolerance")]
        public CipReal VelocityLockTolerance { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.VelocityLimitPositive, "Velocity Limit Positive")]
        public CipReal VelocityLimitPositive { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.VelocityLimitNegative, "Velocity Limit Negative")]
        public CipReal VelocityLimitNegative { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.VelocityLimitSource, "Velocity Limit Source")]
        public CipDint VelocityLimitSource { get; set; }

        // Acceleration Loop
        [CipDetailInfo((ushort)AxisAttributeId.AccelerationLimit, "Acceleration Limit")]
        public CipReal AccelerationLimit { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.DecelerationLimit, "Deceleration Limit")]
        public CipReal DecelerationLimit { get; set; }

        // Torque Current Loop
        [CipDetailInfo((ushort)AxisAttributeId.TorqueLoopBandwidth, "Torque Loop Bandwidth")]
        public CipReal TorqueLoopBandwidth { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TorqueLimitPositive, "Torque Limit Positive")]
        public CipReal TorqueLimitPositive { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TorqueLimitNegative, "Torque Limit Negative")]
        public CipReal TorqueLimitNegative { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TorqueTrim, "Torque Trim")]
        public CipReal TorqueTrim { get; set; }

        // Planner
        [CipDetailInfo((ushort)AxisAttributeId.MaximumSpeed, "Maximum Speed")]
        public CipReal MaximumSpeed { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MaximumAcceleration, "Maximum Acceleration", "$Units/s^2")]
        public CipReal MaximumAcceleration { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MaximumDeceleration, "Maximum Deceleration", "$Units/s^2")]
        public CipReal MaximumDeceleration { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MaximumAccelerationJerk, "Maximum Acceleration Jerk", "$Units/s^3")]
        public CipReal MaximumAccelerationJerk { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MaximumDecelerationJerk, "Maximum Deceleration Jerk", "$Units/s^3")]
        public CipReal MaximumDecelerationJerk { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.CommandUpdateDelayOffset, "Command Update Delay Offset", "us")]

        public CipDint CommandUpdateDelayOffset { get; set; }


        // Homing
        [CipDetailInfo((ushort)AxisAttributeId.HomeMode, "Home Mode")]
        public CipUsint HomeMode { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.HomePosition, "Home Position")]
        public CipReal HomePosition { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.HomeOffset, "Home Offset")]
        public CipReal HomeOffset { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.HomeSequence, "Home Sequence")]
        public CipUsint HomeSequence { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.HomeConfigurationBits, "Home Configuration Bits")]
        public CipUdint HomeConfigurationBits { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.HomeDirection, "Home Direction")]
        public CipUsint HomeDirection { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.HomeSpeed, "Home Speed")]
        public CipReal HomeSpeed { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.HomeReturnSpeed, "Home Return Speed")]
        public CipReal HomeReturnSpeed { get; set; }

        // Actions
        [CipDetailInfo((ushort)AxisAttributeId.StoppingAction, "Stopping Action")]
        public CipUsint StoppingAction { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MotorOverloadAction, "Motor Overload Action")]
        public CipUsint MotorOverloadAction { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.InverterOverloadAction, "Inverter Overload Action")]
        public CipUsint InverterOverloadAction { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.InverterCapacity, "Inverter Capacity")]
        public CipReal InverterCapacity { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MechanicalBrakeControl, "Mechanical Brake Control")]
        public CipUsint MechanicalBrakeControl { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MechanicalBrakeEngageDelay, "Mechanical Brake Engage Delay")]
        public CipReal MechanicalBrakeEngageDelay { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MechanicalBrakeReleaseDelay, "Mechanical Brake Release Delay")]
        public CipReal MechanicalBrakeReleaseDelay { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MotionExceptionAction, "Motion Exception Action", ArraySize = 32)]
        public CipByteArray MotionExceptionAction { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.CIPAxisExceptionAction, "CIP Axis Exception Action", ArraySize = 64)]
        public CipByteArray CIPAxisExceptionAction { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.CIPAxisExceptionActionMfg, "CIP Axis Exception Action - Mfg",
            ArraySize = 64)]
        public CipByteArray CIPAxisExceptionActionMfg { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.CIPAxisExceptionActionRA, "CIP Axis Exception Action - RA",
            ArraySize = 64)]
        public CipByteArray CIPAxisExceptionActionRA { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.BrakeSlipTolerance, "Brake Slip Tolerance")]

        public CipReal BrakeSlipTolerance { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.BrakeTestTorque, "Brake Test Torque")]

        public CipReal BrakeTestTorque { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.CoastingTimeLimit, "Coasting Time Limit")]
        public CipReal CoastingTimeLimit { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.ProvingConfiguration, "Proving Configuration")]

        public CipUsint ProvingConfiguration { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TorqueProveCurrent, "Torque Prove Current")]
        public CipReal TorqueProveCurrent { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.ZeroSpeed, "Zero Speed")]
        public CipReal ZeroSpeed { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.ZeroSpeedTime, "Zero Speed Time")]
        public CipReal ZeroSpeedTime { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.ConverterOutputCurrent, "Converter Output Current")]
        public CipReal ConverterOutputCurrent { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.ConverterOutputPower, "Converter Output Power")]
        public CipReal ConverterOutputPower { get; set; }

        // Virtual-Motion Planner
        [CipDetailInfo((ushort)AxisAttributeId.OutputCamExecutionTargets, "Output Cam Execution Targets")]
        public CipUdint OutputCamExecutionTargets { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.ProgrammedStopMode, "Programmed Stop Mode")]
        public CipUsint ProgrammedStopMode { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MasterInputConfigurationBits, "Master Input Configuration Bits")]
        public CipUdint MasterInputConfigurationBits { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MasterPositionFilterBandwidth, "Master Position Filter Bandwidth")]
        public CipReal MasterPositionFilterBandwidth { get; set; }

        // Virtual-Units
        [CipDetailInfo((ushort)AxisAttributeId.AverageVelocityTimebase, "Average Velocity Timebase")]
        public CipReal AverageVelocityTimebase { get; set; }

        // Virtual-Conversion
        [CipDetailInfo((ushort)AxisAttributeId.ConversionConstant, "Conversion Constant")]
        public CipReal ConversionConstant { get; set; }

        // Other
        [CipDetailInfo((ushort)AxisAttributeId.DynamicsConfigurationBits, "Dynamics Configuration Bits")]
        public CipUdint DynamicsConfigurationBits { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.InterpolatedPositionConfiguration, "Interpolated Position Configuration"
        )]
        public CipUdint InterpolatedPositionConfiguration { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.BusConfiguration, "Bus Configuration")]
        public CipUsint BusConfiguration { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.BusRegulatorThermalOverloadUL, "Bus Regulator Thermal Overload UL")]
        public CipReal BusRegulatorThermalOverloadUL { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.BusUndervoltageUserLimit, "Bus Undervoltage User Limit")]
        public CipReal BusUndervoltageUserLimit { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.CommutationAlignment, "Commutation Alignment")]
        public CipUsint CommutationAlignment { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.ControlMethod, "Control Method")]
        public CipUsint ControlMethod { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.ControlMode, "Control Mode")]
        public CipUsint ControlMode { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.ConverterThermalOverloadUserLimit,
            "Converter Thermal Overload User Limit")]
        public CipReal ConverterThermalOverloadUserLimit { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.ConverterCapacity, "Converter Capacity")]
        public CipReal ConverterCapacity { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.CurrentError, "Current Error")]
        public CipReal CurrentError { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.InverterThermalOverloadUserLimit, "Inverter Thermal Overload User Limit"
        )]
        public CipReal InverterThermalOverloadUserLimit { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.DampingFactor, "Damping Factor")]
        public CipReal DampingFactor { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.DriveModelTimeConstant, "Drive Model Time Constant")]
        public CipReal DriveModelTimeConstant { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.ExternalShuntRegulatorID, "External Shunt Regulator ID")]
        // ReSharper disable once InconsistentNaming
        public CipInt ExternalShuntRegulatorID { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.FeedbackDataLossUserLimit, "Feedback Data Loss User Limit")]
        public CipUdint FeedbackDataLossUserLimit { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.FeedbackMode, "Feedback Mode")]
        public CipUsint FeedbackMode { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.FluxCurrentReference, "Flux Current Reference")]
        public CipReal FluxCurrentReference { get; set; }


        [CipDetailInfo((ushort)AxisAttributeId.LoadObserverFeedbackGain, "Load Observer Feedback Gain")]
        public CipReal LoadObserverFeedbackGain { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MotionResolution, "Motion Resolution")]
        public CipUdint MotionResolution { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MotionScalingConfiguration, "Motion Scaling Configuration")]
        public CipUsint MotionScalingConfiguration { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MotorOverspeedUserLimit, "Motor Overspeed User Limit")]
        public CipReal MotorOverspeedUserLimit { get; set; }

        //Motor Thermal Overload User Limit
        [CipDetailInfo((ushort)AxisAttributeId.MotorThermalOverloadUserLimit, "Motor Thermal Overload User Limit")]
        public CipReal MotorThermalOverloadUserLimit { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.CurrentVectorLimit, "Current Vector Limit")]
        public CipReal CurrentVectorLimit { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.OvertorqueLimit, "Overtorque Limit")]
        public CipReal OvertorqueLimit { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.OvertorqueLimitTime, "Overtorque Limit Time")]
        public CipReal OvertorqueLimitTime { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.PositionServoBandwidth, "Position Servo Bandwidth")]
        public CipReal PositionServoBandwidth { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.RegistrationInputs, "Registration Inputs")]
        public CipUsint RegistrationInputs { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.StoppingTimeLimit, "Stopping Time Limit")]
        public CipReal StoppingTimeLimit { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.StoppingTorque, "Stopping Torque")]
        public CipReal StoppingTorque { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.SystemBandwidth, "System Bandwidth")]
        public CipReal SystemBandwidth { get; set; }

        //Derived from Damping Factor
        [CipDetailInfo((ushort)AxisAttributeId.SystemDamping, "System Damping")]
        public CipReal SystemDamping { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TorqueRateLimit, "Torque Rate Limit")]
        public CipReal TorqueRateLimit { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TorqueThreshold, "Torque Threshold")]
        public CipReal TorqueThreshold { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.UndertorqueLimit, "Undertorque Limit")]
        public CipReal UndertorqueLimit { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.UndertorqueLimitTime, "Undertorque Limit Time")]
        public CipReal UndertorqueLimitTime { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.VelocityErrorToleranceTime, "Velocity Error Tolerance Time")]
        public CipReal VelocityErrorToleranceTime { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.VelocityServoBandwidth, "Velocity Servo Bandwidth")]
        public CipReal VelocityServoBandwidth { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.VelocityStandstillWindow, "Velocity Standstill Window")]
        public CipReal VelocityStandstillWindow { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.VelocityThreshold, "Velocity Threshold")]
        public CipReal VelocityThreshold { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.VelocityTrim, "Velocity Trim")]
        public CipReal VelocityTrim { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.VelocityDroop, "Velocity Droop")]
        public CipReal VelocityDroop { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.VelocityLowPassFilterBandwidth, "Velocity Low Pass Filter Bandwidth")]
        public CipReal VelocityLowPassFilterBandwidth { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.VelocityNegativeFeedforwardGain, "Velocity Negative Feedforward Gain")]
        public CipReal VelocityNegativeFeedforwardGain { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.VelocityOffset, "Velocity Offset")]
        public CipReal VelocityOffset { get; set; }

        // ReSharper disable InconsistentNaming
        [CipDetailInfo((ushort)AxisAttributeId.SLATConfiguration, "SLAT Configuration")]
        public CipUsint SLATConfiguration { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.SLATSetPoint, "SLAT SetPoint")]
        public CipReal SLATSetPoint { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.SLATTimeDelay, "SLAT TimeDelay")]
        public CipReal SLATTimeDelay { get; set; }
        // ReSharper restore InconsistentNaming


        [CipDetailInfo((ushort)AxisAttributeId.ShuntRegulatorResistorType, "Shunt Regulator Resistor Type")]
        public CipUsint ShuntRegulatorResistorType { get; set; }

        // TODO: need edit
        [CipDetailInfo((ushort)AxisAttributeId.BusRegulatorAction, "Bus Regulator Action")]
        public CipUsint BusRegulatorAction { get; set; }

        #endregion

        #region Command Reference Generation Attributes, rm003 page 154

        [CipDetailInfo((ushort)AxisAttributeId.PositionFineCommand, "Position Fine Command")]
        public CipReal PositionFineCommand { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.VelocityFineCommand, "Velocity Fine Command")]
        public CipReal VelocityFineCommand { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.AccelerationFineCommand, "Acceleration Fine Command")]
        public CipReal AccelerationFineCommand { get; set; }

        #endregion

        #region Control Mode Attributes, rm003 page 164

        [CipDetailInfo((ushort)AxisAttributeId.PositionReference, "Position Reference")]
        public CipReal PositionReference { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.VelocityFeedforwardCommand, "Velocity Feedforward Command")]
        public CipReal VelocityFeedforwardCommand { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.PositionError, "Position Error")]
        public CipReal PositionError { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.PositionIntegratorOutput, "Position Integrator Output")]
        public CipReal PositionIntegratorOutput { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.PositionLoopOutput, "Position Loop Output")]
        public CipReal PositionLoopOutput { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.ActualPosition, "Actual Position")]
        public CipReal ActualPosition { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.CommandPosition, "Command Position")]
        public CipReal CommandPosition { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.ActualVelocity, "Actual Velocity")]

        public CipReal ActualVelocity { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.CommandVelocity, "Command Velocity")]

        public CipReal CommandVelocity { get; set; }

        #endregion

        #region Velocity Loop Attributes, rm003 page 173

        [CipDetailInfo((ushort)AxisAttributeId.AccelerationFeedforwardCommand, "Acceleration Feedforward Command")]
        public CipReal AccelerationFeedforwardCommand { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.VelocityReference, "Velocity Reference")]
        public CipReal VelocityReference { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.VelocityFeedback, "Velocity Feedback")]
        public CipReal VelocityFeedback { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.VelocityError, "Velocity Error")]
        public CipReal VelocityError { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.VelocityIntegratorOutput, "Velocity Integrator Output")]
        public CipReal VelocityIntegratorOutput { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.VelocityLoopOutput, "Velocity Loop Output")]
        public CipReal VelocityLoopOutput { get; set; }

        #endregion

        #region Acceleration Control Attributes, rm003 page 184

        [CipDetailInfo((ushort)AxisAttributeId.AccelerationReference, "Acceleration Reference")]
        public CipReal AccelerationReference { get; set; }

        // AccelerationFeedback -> AccelerationFeedback1
        [CipDetailInfo((ushort)AxisAttributeId.AccelerationFeedback, "Acceleration Feedback")]
        public CipReal AccelerationFeedback { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.LoadObserverAccelerationEstimate, "Load Observer Acceleration Estimate")
        ]
        public CipReal LoadObserverAccelerationEstimate { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.LoadObserverTorqueEstimate, "Load Observer Torque Estimate")]
        public CipReal LoadObserverTorqueEstimate { get; set; }

        #endregion

        #region Torque/Force Control Signal Attributes, rm003 page 189

        [CipDetailInfo((ushort)AxisAttributeId.TorqueReference, "Torque Reference")]
        public CipReal TorqueReference { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TorqueReferenceFiltered, "Torque Reference Filtered")]
        public CipReal TorqueReferenceFiltered { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.TorqueReferenceLimited, "Torque Reference Limited")]
        public CipReal TorqueReferenceLimited { get; set; }

        #endregion

        #region Current Control Attributes, rm003 page 203

        [CipDetailInfo((ushort)AxisAttributeId.CurrentCommand, "Current Command")]
        public CipReal CurrentCommand { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.OperativeCurrentLimit, "Operative Current Limit")]
        public CipReal OperativeCurrentLimit { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.CurrentLimitSource, "Current Limit Source")]
        public CipReal CurrentLimitSource { get; set; }


        [CipDetailInfo((ushort)AxisAttributeId.CurrentReference, "Current Reference")]
        public CipReal CurrentReference { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.CurrentFeedback, "Current Feedback")]
        public CipReal CurrentFeedback { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.FluxCurrentError, "Flux Current Error")]
        public CipReal FluxCurrentError { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.FluxCurrentFeedback, "Flux Current Feedback")]
        public CipReal FluxCurrentFeedback { get; set; }

        #endregion

        #region Drive Output Attributes, rm003 page 215

        [CipDetailInfo((ushort)AxisAttributeId.OutputFrequency, "Output Frequency")]
        public CipReal OutputFrequency { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.OutputCurrent, "Output Current")]
        public CipReal OutputCurrent { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.OutputVoltage, "Output Voltage")]
        public CipReal OutputVoltage { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.OutputPower, "Output Power")]
        public CipReal OutputPower { get; set; }

        #endregion

        #region DC Bus Control Attributes, rm003 page 228

        [CipDetailInfo((ushort)AxisAttributeId.DCBusVoltage, "DC Bus Voltage")]
        public CipReal DCBusVoltage { get; set; }

        #endregion

        #region Power and Thermal Management Status Attributes, rm003 page 230

        [CipDetailInfo((ushort)AxisAttributeId.MotorCapacity, "Motor Capacity")]
        public CipReal MotorCapacity { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.BusRegulatorCapacity, "Bus Regulator Capacity")]
        public CipReal BusRegulatorCapacity { get; set; }

        #endregion

        #region Motion Database Storage Attributes

        // TODO(gjc): read from database
        // 0.000505819
        public float DriveModelTimeConstantBase { get; set; }
        public float SystemAccelerationBase { get; set; }
        public int DriveMaxOutputFrequency { get; set; }
        public float DriveRatedPeakCurrent { get; set; }
        public float BusOvervoltageOperationalLimit { get; set; }

        #endregion

        [CipDetailInfo((ushort)AxisAttributeId.FrequencyControlMethod, "Frequency Control Method")]
        public CipUsint FrequencyControlMethod { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MaximumVoltage, "Maximum Voltage")]
        public CipReal MaximumVoltage { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.MaximumFrequency, "Maximum Frequency")]
        public CipReal MaximumFrequency { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.BreakVoltage, "Break Voltage")]
        public CipReal BreakVoltage { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.BreakFrequency, "Break Frequency")]
        public CipReal BreakFrequency { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.StartBoost, "Start Boost")]
        public CipReal StartBoost { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.RunBoost, "Run Boost")]
        public CipReal RunBoost { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.FluxUpControl, "Flux Up Control")]
        public CipUsint FluxUpControl { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.FluxUpTime, "Flux Up Time")]
        public CipReal FluxUpTime { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.SkipSpeed1, "Skip Speed 1")]
        public CipReal SkipSpeed1 { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.SkipSpeed2, "Skip Speed 2")]
        public CipReal SkipSpeed2 { get; set; }

        [CipDetailInfo((ushort)AxisAttributeId.SkipSpeedBand, "Skip Speed Band")]
        public CipReal SkipSpeedBand { get; set; }

        public object Clone()
        {
            CIPAxis clone = (CIPAxis)MemberwiseClone();
            // PMMotorFluxSaturation
            clone.PMMotorFluxSaturation = new CipRealArray(8);
            for (int i = 0; i < 8; i++)
            {
                clone.PMMotorFluxSaturation.SetValue(i, PMMotorFluxSaturation.GetValue(i));
            }

            // MotionExceptionAction
            clone.MotionExceptionAction = new CipByteArray(32);
            for (int i = 0; i < 32; i++)
            {
                clone.MotionExceptionAction.SetValue(i, MotionExceptionAction.GetValue(i));
            }

            // CIPAxisExceptionAction
            clone.CIPAxisExceptionAction = new CipByteArray(64);
            for (int i = 0; i < 64; i++)
            {
                clone.CIPAxisExceptionAction.SetValue(i, CIPAxisExceptionAction.GetValue(i));
            }

            // CIPAxisExceptionActionMfg
            clone.CIPAxisExceptionActionMfg = new CipByteArray(64);
            for (int i = 0; i < 64; i++)
            {
                clone.CIPAxisExceptionActionMfg.SetValue(i, CIPAxisExceptionActionMfg.GetValue(i));
            }

            // CIPAxisExceptionActionRA
            clone.CIPAxisExceptionActionRA = new CipByteArray(64);
            for (int i = 0; i < 64; i++)
            {
                clone.CIPAxisExceptionActionRA.SetValue(i, CIPAxisExceptionActionRA.GetValue(i));
            }

            return clone;
        }

        public void Apply(CIPAxis cipAxis, List<ushort> differentAttributeList)
        {
            if ((differentAttributeList != null) && (differentAttributeList.Count > 0))
            {
                var attributeMap = CipAttributeHelper.GetAttributeMap(typeof(CIPAxis));

                foreach (var attributeId in differentAttributeList)
                    if (attributeMap.ContainsKey(attributeId))
                    {
                        var attributeName = attributeMap[attributeId];
                        var p = typeof(CIPAxis).GetProperty(attributeName);
                        if (p != null)
                        {
                            var cipDetailInfo =
                                (CipDetailInfoAttribute)
                                p.GetCustomAttributes(typeof(CipDetailInfoAttribute), true).FirstOrDefault();

                            if (cipDetailInfo != null)
                            {
                                var srcValue = p.GetValue(cipAxis) as ICipDataType;
                                if (srcValue != null)
                                {
                                    var startIndex = 0;
                                    var detValue =
                                        CipAttributeHelper.Parse(
                                            srcValue.GetType(),
                                            srcValue.GetBytes(),
                                            cipDetailInfo.ArraySize,
                                            ref startIndex);

                                    p.SetValue(this, detValue);
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"don't found attribute id:{attributeId}");
                    }
            }
        }

        #region cip message

        // new for SendUnitData
        public new async Task GetAttributeSingle(string attributeName)
        {
            await GetAttributeSingleWithSendUnitData(attributeName, this, Messager);
        }

        public async Task SetAttributeSingle(string attributeName)
        {
            var attributeId = CipAttributeHelper.AttributeNameToId(typeof(CIPAxis), attributeName);

            if (attributeId.HasValue)
            {
                var attributePairList =
                    CipAttributeHelper.AttributeIdListToAttributePairList(this,
                        new List<ushort>() { attributeId.Value });
                // request 
                var request = SetAttributeSingleRequest(attributeId.Value, attributePairList[0].AttributeValue);

                // send
                var response = await Messager.SendUnitData(request);

                if (response == null)
                    throw new Exception($"SetAttributeSingle {attributeName} failed! Response is NULL!");

                if (response.GeneralStatus == (byte)CipGeneralStatusCode.Success)
                    return;

                throw new Exception($"SetAttributeSingle {attributeName}:{response.GeneralStatus}");
            }
        }

        public async Task GetAttributeList(string[] attributeNames)
        {
            await GetAttributeList(attributeNames, this, Messager);
        }

        public async Task SetAttributeList(List<ushort> attributeIdList)
        {
            await SetAttributeList(attributeIdList, this, Messager);
        }

        public async Task SetCyclicWriteList(string[] attributeNames)
        {
            var attributeIdList = CipAttributeHelper.AttributeNamesToIdList<CIPAxis>(attributeNames);
            await SetCyclicList(CipAxisServiceCode.SetCyclicWriteList, attributeIdList);
        }

        public async Task SetCyclicReadList(string[] attributeNames)
        {
            var attributeIdList = CipAttributeHelper.AttributeNamesToIdList<CIPAxis>(attributeNames);
            await SetCyclicList(CipAxisServiceCode.SetCyclicReadList, attributeIdList);
        }

        private async Task SetCyclicList(CipAxisServiceCode serviceCode, List<ushort> attributeIdList)
        {
            ICipMessager messager = Messager;

            if (messager != null)
            {
                // request
                var request = CreateSetCyclicListRequest(serviceCode, attributeIdList);

                // send
                var response = await messager.SendUnitData(request);
                if ((response != null) && (response.GeneralStatus == (byte)CipGeneralStatusCode.Success))
                {
                    // TODO(gjc): add check result

                }
            }
        }

        private IMessageRouterRequest CreateSetCyclicListRequest(CipAxisServiceCode serviceCode,
            List<ushort> attributeIdList)
        {
            var request = new MessageRouterRequest
            {
                Service = (byte)serviceCode,
                RequestPath = new PaddedEPath(ClassId, InstanceId),
                RequestData = null
            };

            if (attributeIdList != null)
            {
                var attributeCount = (ushort)attributeIdList.Count;
                var byteList = new List<byte>((attributeCount + 1) * 2);

                byteList.AddRange(BitConverter.GetBytes(attributeCount));

                foreach (var attribute in attributeIdList)
                    byteList.AddRange(BitConverter.GetBytes(attribute));

                request.RequestData = byteList.ToArray();
            }
            else
            {
                request.RequestData = new byte[] { 0, 0 };
            }

            return request;

        }

        #endregion

        public string GetAttributeValueString(string attributeName)
        {
            var p = typeof(CIPAxis).GetProperty(attributeName);
            if (p == null)
                return string.Empty;

            var propertyValue = p.GetValue(this);
            if (propertyValue != null)
            {
                var array = propertyValue as CipByteArray;
                return array != null ? array.ToString() : Convert.ToString(propertyValue);
            }

            return string.Empty;
        }
    }
}
