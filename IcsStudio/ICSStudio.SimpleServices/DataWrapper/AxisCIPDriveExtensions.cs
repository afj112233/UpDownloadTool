using System;
using System.Linq;
using ICSStudio.Cip.Objects;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.SimpleServices.DataWrapper
{
    public static class AxisCIPDriveExtensions
    {
        public static bool SupportAttribute(this AxisCIPDrive axisCIPDrive, string attribute)
        {
            // Required
            string[] requiredAttributes =
            {
                // for config file
                "MotionGroup", "MotionModule", "Name", "InstanceNumber",
                // motion-rm003, page 96 - 110
                // vol9-1
                "AxisConfiguration", "FeedbackConfiguration",
                "ProgrammedStopMode",
                "CIPAxisExceptionAction", "CIPAxisExceptionActionMfg", "CIPAxisExceptionActionRA",
                "AxisID", "AxisUpdateSchedule",
                "ControlMethod", "ControlMode",
                "CyclicReadUpdateList", "CyclicWriteUpdateList",
                // other
                "BusConfiguration", "BusRegulatorAction", "BusRegulatorThermalOverloadUL",
                "BusUndervoltageUserLimit", "ConverterACInputPhasing", "ConverterACInputVoltage",
                "ConverterThermalOverloadUserLimit", "ExternalShuntRegulatorID", "ShuntRegulatorResistorType",

            };
            if (requiredAttributes.Contains(attribute))
                return true;

            var axisConfiguration =
                (AxisConfigurationType)Convert.ToByte(axisCIPDrive.CIPAxis.AxisConfiguration);
            var motorType =
                (MotorType)Convert.ToByte(axisCIPDrive.CIPAxis.MotorType);
            var feedback1StartupMethod =
                (FeedbackStartupMethodType)Convert.ToByte(axisCIPDrive.CIPAxis.Feedback1StartupMethod);

            #region Conditional

            if (attribute.Equals("Feedback1Type"))
            {
                if (axisConfiguration == AxisConfigurationType.PositionLoop ||
                    axisConfiguration == AxisConfigurationType.VelocityLoop ||
                    axisConfiguration == AxisConfigurationType.TorqueLoop ||
                    axisConfiguration == AxisConfigurationType.FeedbackOnly)
                    return true;

                return false;
            }

            if (attribute.Equals("MotorCatalogNumber"))
            {
                if (axisConfiguration == AxisConfigurationType.ConverterOnly ||
                    axisConfiguration == AxisConfigurationType.FeedbackOnly)
                    return false;

                var motorDataSource =
                    (MotorDataSourceType)Convert.ToByte(axisCIPDrive.CIPAxis.MotorDataSource);
                if (motorDataSource == MotorDataSourceType.Database ||
                    motorDataSource == MotorDataSourceType.Datasheet)
                    return true;

                return false;
            }

            if (attribute.StartsWith("Feedback2"))
            {
                var feedbackConfiguration =
                    (FeedbackConfigurationType)Convert.ToByte(axisCIPDrive.CIPAxis.FeedbackConfiguration);
                if (feedbackConfiguration == FeedbackConfigurationType.LoadFeedback ||
                    feedbackConfiguration == FeedbackConfigurationType.DualFeedback)
                    return true;

                return false;
            }

            // EFPVT
            if (attribute.Equals("MotionScalingConfiguration") ||
                attribute.Equals("ConversionConstant") ||
                attribute.Equals("PositionUnits") ||
                attribute.Equals("AverageVelocityTimebase") ||
                attribute.Equals("VelocityStandstillWindow") ||
                attribute.Equals("ScalingSource") ||
                attribute.Equals("LoadType") ||
                attribute.Equals("ActuatorType") ||
                attribute.Equals("TravelMode") ||
                attribute.Equals("PositionScalingNumerator") ||
                attribute.Equals("PositionScalingDenominator") ||
                attribute.Equals("MotionResolution") ||
                attribute.Equals("MotionPolarity") ||
                attribute.Equals("MotionExceptionAction") ||
                attribute.Equals("TransmissionRatioInput") ||
                attribute.Equals("TransmissionRatioOutput") ||
                attribute.Equals("ActuatorLead") ||
                attribute.Equals("ActuatorLeadUnit") ||
                attribute.Equals("ActuatorDiameter") ||
                attribute.Equals("ActuatorDiameterUnit") ||
                attribute.Equals("DriveRatedPeakCurrent") ||
                attribute.Equals("FeedbackMode"))
            {
                if (axisConfiguration == AxisConfigurationType.ConverterOnly)
                    return false;

                return true;
            }

            // Required - C
            if (attribute.Equals("SystemAccelerationBase")||
                attribute.Equals("DriveModelTimeConstantBase"))
            {
                if (axisConfiguration == AxisConfigurationType.PositionLoop ||
                    axisConfiguration == AxisConfigurationType.VelocityLoop ||
                    axisConfiguration == AxisConfigurationType.TorqueLoop)
                    return true;

                return false;
            }

            // Required - E
            if (attribute.Equals("OutputCamExecutionTargets"))
            {
                if (axisConfiguration == AxisConfigurationType.ConverterOnly ||
                    axisConfiguration == AxisConfigurationType.FrequencyControl)
                    return false;

                return true;
            }

            // FPVT
            if (attribute.Equals("MotorType") ||
                attribute.Equals("MotorDataSource") ||
                attribute.Equals("StoppingAction") ||
                attribute.Equals("InverterCapacity") ||
                attribute.Equals("MotorDeviceCode") ||
                attribute.Equals("MotorUnit") ||
                attribute.Equals("MotorRatedVoltage") ||
                attribute.Equals("MotorRatedContinuousCurrent") ||
                attribute.Equals("MotorTestResistance") ||
                attribute.Equals("MotorTestInductance"))
            {
                if (axisConfiguration == AxisConfigurationType.ConverterOnly ||
                    axisConfiguration == AxisConfigurationType.FeedbackOnly)
                    return false;

                return true;
            }


            // EPVT - R
            if (attribute.Equals("PositionUnwind") ||
                attribute.Equals("HomeMode") ||
                attribute.Equals("HomeSequence") ||
                attribute.Equals("HomeConfigurationBits") ||
                attribute.Equals("HomePosition") ||
                attribute.Equals("HomeOffset") ||
                attribute.Equals("RegistrationInputs") ||
                attribute.Equals("Feedback1Unit") ||
                attribute.Equals("Feedback1StartupMethod") ||
                attribute.Equals("Feedback1CycleResolution") || // Not Linear Displacement Transducer, need edit
                attribute.Equals("Feedback1CycleInterpolation") ||
                attribute.Equals("PositionUnwindNumerator") ||
                attribute.Equals("PositionUnwindDenominator") ||
                attribute.Equals("TravelRange") ||
                attribute.Equals("SoftTravelLimitChecking") ||
                attribute.Equals("SoftTravelLimitPositive") ||
                attribute.Equals("SoftTravelLimitNegative") ||
                attribute.Equals("HookupTestDistance") ||
                attribute.Equals("HookupTestFeedbackChannel") ||
                attribute.Equals("InterpolatedPositionConfiguration")
               )
            {
                if (axisConfiguration == AxisConfigurationType.ConverterOnly ||
                    axisConfiguration == AxisConfigurationType.FrequencyControl)
                    return false;

                return true;
            }

            // FPV
            if (attribute.Equals("MaximumSpeed") ||
                attribute.Equals("MaximumAcceleration") ||
                attribute.Equals("MaximumDeceleration") ||
                attribute.Equals("MaximumAccelerationJerk") ||
                attribute.Equals("MaximumDecelerationJerk") ||
                attribute.Equals("DynamicsConfigurationBits") ||
                attribute.Equals("VelocityTrim"))
            {
                if (axisConfiguration == AxisConfigurationType.FrequencyControl ||
                    axisConfiguration == AxisConfigurationType.PositionLoop ||
                    axisConfiguration == AxisConfigurationType.VelocityLoop)
                    return true;

                return false;
            }

            // EPV
            if (attribute.Equals("MasterInputConfigurationBits") ||
                attribute.Equals("MasterPositionFilterBandwidth"))
            {
                if (axisConfiguration == AxisConfigurationType.FeedbackOnly ||
                    axisConfiguration == AxisConfigurationType.PositionLoop ||
                    axisConfiguration == AxisConfigurationType.VelocityLoop)
                    return true;

                return false;
            }

            // PVT - R
            if (attribute.Equals("TorqueOffset") ||
                attribute.Equals("TuningTravelLimit") ||
                attribute.Equals("TuningSpeed") ||
                attribute.Equals("TuningTorque") ||
                attribute.Equals("DampingFactor") ||
                attribute.Equals("DriveModelTimeConstant") ||
                attribute.Equals("TorqueLimitPositive") ||
                attribute.Equals("TorqueLimitNegative") ||
                attribute.Equals("StoppingTorque") ||
                attribute.Equals("LoadInertiaRatio") ||
                attribute.Equals("TuneFriction") ||
                attribute.Equals("TuneLoadOffset") ||
                attribute.Equals("TotalInertia") ||
                attribute.Equals("TuningSelect") ||
                attribute.Equals("TuningDirection") ||
                attribute.Equals("LoadRatio") ||
                attribute.Equals("TuneInertiaMass") ||
                attribute.Equals("GainTuningConfigurationBits") ||
                attribute.Equals("SystemBandwidth") ||
                attribute.Equals("LoadCoupling") ||
                attribute.Equals("SystemDamping") ||
                attribute.Equals("BusOvervoltageOperationalLimit") ||
                attribute.Equals("TorqueTrim"))
            {
                if (axisConfiguration == AxisConfigurationType.PositionLoop ||
                    axisConfiguration == AxisConfigurationType.VelocityLoop ||
                    axisConfiguration == AxisConfigurationType.TorqueLoop)
                    return true;

                return false;
            }

            // PV - R
            if (attribute.Equals("HomeDirection") ||
                attribute.Equals("HomeSpeed") ||
                attribute.Equals("HomeReturnSpeed") ||
                attribute.Equals("AccelerationFeedforwardGain") ||
                attribute.Equals("VelocityOffset") ||
                attribute.Equals("VelocityServoBandwidth") ||
                attribute.Equals("VelocityIntegratorControl") ||
                attribute.Equals("ApplicationType") ||
                attribute.Equals("LoopResponse") ||
                attribute.Equals("VelocityLoopBandwidth") ||
                attribute.Equals("VelocityIntegratorBandwidth"))
            {
                if (axisConfiguration == AxisConfigurationType.PositionLoop ||
                    axisConfiguration == AxisConfigurationType.VelocityLoop)
                    return true;

                return false;
            }

            // VT - R
            if (attribute.Equals("CommandTorque"))
            {
                if (axisConfiguration == AxisConfigurationType.TorqueLoop ||
                    axisConfiguration == AxisConfigurationType.VelocityLoop)
                    return true;

                return false;
            }

            // P
            if (attribute.Equals("VelocityFeedforwardGain") ||
                attribute.Equals("PositionErrorTolerance") ||
                attribute.Equals("PositionLockTolerance") ||
                attribute.Equals("BacklashReversalOffset") ||
                attribute.Equals("PositionServoBandwidth") ||
                attribute.Equals("PositionIntegratorBandwidth") ||
                attribute.Equals("PositionIntegratorControl") ||
                attribute.Equals("PositionLoopBandwidth") ||
                attribute.Equals("PositionTrim"))
            {
                if (axisConfiguration == AxisConfigurationType.PositionLoop)
                    return true;

                return false;
            }

            // F
            if (attribute.Equals("BreakFrequency"))
            {
                if (axisConfiguration == AxisConfigurationType.FrequencyControl)
                {
                    var frequencyControlMethod =
                        (FrequencyControlMethodType)Convert.ToByte(axisCIPDrive.CIPAxis.FrequencyControlMethod);

                    if (frequencyControlMethod == FrequencyControlMethodType.BasicVoltsHertz)
                        return true;
                }

                return false;
            }

            if (attribute.Equals("BreakVoltage")||
                attribute.Equals("FrequencyControlMethod")||
                attribute.Equals("MaximumFrequency")||
                attribute.Equals("MaximumVoltage")||
                attribute.Equals("RunBoost")||
                attribute.Equals("StartBoost"))
            {
                if (axisConfiguration == AxisConfigurationType.FrequencyControl)
                    return true;

                return false;
            }

            // D?
            if (attribute.Equals("DriveRatedVoltage") ||
                attribute.Equals("HookupTestSpeed")||
                attribute.Equals("MaxOutputFrequency")||
                attribute.Equals("MotorTestDataValid"))
            {
                if (axisConfiguration == AxisConfigurationType.FrequencyControl)
                    return true;

                return false;
            }

            if (attribute.Equals("MotorTestFluxCurrent") ||
                attribute.Equals("MotorTestSlipSpeed"))
            {
                if (axisConfiguration == AxisConfigurationType.FrequencyControl)
                {
                    if (motorType == MotorType.RotaryInduction || motorType == MotorType.LinearInduction)
                        return true;
                }

                return false;
            }

            //TODO(gjc): need edit later, Required - !E
            if (attribute.Equals("HookupTestTime"))
            {
                if (axisConfiguration == AxisConfigurationType.FrequencyControl)
                    return true;

                return false;
            }

            #endregion

            #region Optional
            // Optional - E
            if (attribute.Equals("FeedbackDataLossUserLimit"))
            {
                if (axisConfiguration == AxisConfigurationType.ConverterOnly ||
                    axisConfiguration == AxisConfigurationType.FrequencyControl)
                    return false;
            }

            // P
            if (attribute.Equals("FrictionCompensationWindow") ||
                attribute.Equals("PositionErrorToleranceTime") ||
                attribute.Equals("PositionLeadLagFilterBandwidth") ||
                attribute.Equals("PositionLeadLagFilterGain") ||
                attribute.Equals("BacklashCompensationWindow"))
            {
                if (axisConfiguration != AxisConfigurationType.PositionLoop)
                    return false;
            }

            // V
            if (attribute.Equals("SLATConfiguration") ||
                attribute.Equals("SLATSetPoint") ||
                attribute.Equals("SLATTimeDelay"))
            {
                if (axisConfiguration != AxisConfigurationType.VelocityLoop)
                    return false;
            }

            // PV
            if (attribute.Equals("FeedbackUnitRatio") ||
                attribute.Equals("VelocityErrorTolerance") ||
                attribute.Equals("VelocityErrorToleranceTime") ||
                attribute.Equals("VelocityNegativeFeedforwardGain") ||
                attribute.Equals("VelocityLowPassFilterBandwidth"))
            {
                if (!(axisConfiguration == AxisConfigurationType.PositionLoop ||
                      axisConfiguration == AxisConfigurationType.VelocityLoop))
                    return false;
            }

            // FPV
            if (attribute.Equals("VelocityDroop") ||
                attribute.Equals("VelocityLimitPositive") ||
                attribute.Equals("VelocityLimitNegative") ||
                attribute.Equals("VelocityLockTolerance"))
            {
                if (!(axisConfiguration == AxisConfigurationType.FrequencyControl ||
                      axisConfiguration == AxisConfigurationType.PositionLoop ||
                      axisConfiguration == AxisConfigurationType.VelocityLoop))
                    return false;
            }

            // PVT
            if (attribute.Equals("TorqueThreshold") ||
                attribute.Equals("TorqueLowPassFilterBandwidth") ||
                attribute.Equals("TorqueNotchFilterFrequency") ||
                attribute.Equals("TorqueRateLimit") ||
                attribute.Equals("FluxCurrentReference") ||
                attribute.Equals("CurrentError") ||
                attribute.Equals("TorqueLoopBandwidth") ||
                attribute.Equals("TorqueLeadLagFilterBandwidth") ||
                attribute.Equals("TorqueLeadLagFilterGain") ||
                attribute.Equals("FeedbackCommutationAligned") ||
                attribute.Equals("FrictionCompensationSliding") ||
                attribute.Equals("FrictionCompensationStatic") ||
                attribute.Equals("FrictionCompensationViscous") ||
                attribute.Equals("CommutationPolarity") ||
                attribute.Equals("LoadObserverConfiguration") ||
                attribute.Equals("LoadObserverBandwidth") ||
                attribute.Equals("LoadObserverIntegratorBandwidth") ||
                attribute.Equals("LoadObserverFeedbackGain") ||
                attribute.Equals("AdaptiveTuningConfiguration") ||
                attribute.Equals("TorqueNotchFilterHighFrequencyLimit") ||
                attribute.Equals("TorqueNotchFilterLowFrequencyLimit") ||
                attribute.Equals("TorqueNotchFilterTuningThreshold"))
            {
                if (!(axisConfiguration == AxisConfigurationType.PositionLoop ||
                      axisConfiguration == AxisConfigurationType.VelocityLoop ||
                      axisConfiguration == AxisConfigurationType.TorqueLoop))
                    return false;
            }

            // FPVT
            if (attribute.Equals("StoppingTimeLimit") ||
                attribute.Equals("AccelerationLimit") ||
                attribute.Equals("DecelerationLimit") ||
                attribute.Equals("OvertorqueLimit") ||
                attribute.Equals("OvertorqueLimitTime") ||
                attribute.Equals("UndertorqueLimit") ||
                attribute.Equals("UndertorqueLimitTime") ||
                attribute.Equals("MechanicalBrakeControl") ||
                attribute.Equals("MechanicalBrakeReleaseDelay") ||
                attribute.Equals("MechanicalBrakeEngageDelay") ||
                attribute.Equals("InverterOverloadAction") ||
                attribute.Equals("MotorOverloadAction") ||
                attribute.Equals("MotorOverspeedUserLimit") ||
                attribute.Equals("MotorThermalOverloadUserLimit") ||
                attribute.Equals("InverterThermalOverloadUserLimit") ||
                attribute.Equals("MotorPolarity") ||
                attribute.Equals("MotorOverloadLimit") ||
                attribute.Equals("MotorIntegralThermalSwitch") ||
                attribute.Equals("MotorMaxWindingTemperature") ||
                attribute.Equals("MotorWindingToAmbientCapacitance") ||
                attribute.Equals("MotorWindingToAmbientResistance") ||
                attribute.Equals("CurrentVectorLimit") ||
                attribute.Equals("ProvingConfiguration") ||
                attribute.Equals("TorqueProveCurrent") ||
                attribute.Equals("BrakeTestTorque") ||
                attribute.Equals("BrakeSlipTolerance") ||
                attribute.Equals("ZeroSpeed") ||
                attribute.Equals("ZeroSpeedTime") ||
                attribute.Equals("CoastingTimeLimit"))
            {
                if (axisConfiguration == AxisConfigurationType.ConverterOnly ||
                    axisConfiguration == AxisConfigurationType.FeedbackOnly)
                    return false;
            }

            // EPVT
            if (attribute.Equals("Feedback1Polarity") ||
                attribute.Equals("Feedback1VelocityFilterBandwidth") ||
                attribute.Equals("Feedback1AccelFilterBandwidth") ||
                attribute.Equals("Feedback1VelocityFilterTaps") ||
                attribute.Equals("Feedback1AccelFilterTaps"))
            {
                if (axisConfiguration == AxisConfigurationType.ConverterOnly ||
                    axisConfiguration == AxisConfigurationType.FrequencyControl)
                    return false;
            }

            // EFPVT
            if (attribute.Equals("VelocityThreshold"))
            {
                if (axisConfiguration == AxisConfigurationType.ConverterOnly)
                    return false;
            }

            // BFPVT
            if (attribute.Equals("ConverterCapacity"))
            {
                if (axisConfiguration == AxisConfigurationType.FeedbackOnly)
                    return false;
            }

            // Optional- D
            if (attribute.Equals("FluxUpControl") ||
                attribute.Equals("FluxUpTime") ||
                attribute.Equals("InductionMotorStatorLeakageReactance") ||
                attribute.Equals("InductionMotorRotorLeakageReactance"))
            {
                if (axisConfiguration != AxisConfigurationType.FrequencyControl)
                    return false;

                if (!(motorType == MotorType.RotaryInduction || motorType == MotorType.LinearInduction))
                    return false;
            }

            #endregion

            #region special

            // special
            if (attribute.Equals("SystemInertia"))
            {
                if (axisConfiguration == AxisConfigurationType.PositionLoop ||
                    axisConfiguration == AxisConfigurationType.VelocityLoop)
                    return true;

                if (axisConfiguration != AxisConfigurationType.TorqueLoop)
                    return false;
            }

            if (attribute.Equals("MotorRatedPeakCurrent"))
            {
                if (axisConfiguration == AxisConfigurationType.ConverterOnly ||
                    axisConfiguration == AxisConfigurationType.FeedbackOnly)
                    return false;

                // PM-R
                if (motorType == MotorType.LinearPermanentMagnet ||
                    motorType == MotorType.RotaryPermanentMagnet ||
                    motorType == MotorType.RotaryInteriorPermanentMagnet)
                    return true;
            }

            if (attribute.Equals("MotorRatedOutputPower")||
                attribute.Equals("InductionMotorFluxCurrent") ||
                attribute.Equals("InductionMotorRatedFrequency") ||
                attribute.Equals("InductionMotorStatorResistance"))
            {
                if (axisConfiguration == AxisConfigurationType.ConverterOnly ||
                    axisConfiguration == AxisConfigurationType.FeedbackOnly)
                    return false;

                // IM-R
                if (motorType == MotorType.LinearInduction ||
                    motorType == MotorType.RotaryInduction)
                    return true;
            }

            // PM Motor only
            if (attribute.Equals("PMMotorResistance") ||
                attribute.Equals("CommutationOffset"))
            {
                if (axisConfiguration == AxisConfigurationType.ConverterOnly ||
                    axisConfiguration == AxisConfigurationType.FeedbackOnly)
                    return false;

                // PM-R
                if (motorType == MotorType.LinearPermanentMagnet ||
                    motorType == MotorType.RotaryPermanentMagnet ||
                    motorType == MotorType.RotaryInteriorPermanentMagnet)
                    return true;

                return false;
            }

            // SPM Motor only - R
            if (attribute.Equals("PMMotorInductance"))
            {
                if (axisConfiguration == AxisConfigurationType.ConverterOnly ||
                    axisConfiguration == AxisConfigurationType.FeedbackOnly)
                    return false;

                // SPM-R ???
                if (motorType == MotorType.LinearPermanentMagnet ||
                    motorType == MotorType.RotaryPermanentMagnet)
                    return true;

                return false;
            }

            // SPM Motor only - O
            if (attribute.Equals("PMMotorFluxSaturation"))
            {
                if (axisConfiguration == AxisConfigurationType.ConverterOnly ||
                    axisConfiguration == AxisConfigurationType.FeedbackOnly)
                    return false;

                // SPM-R ???
                if (!(motorType == MotorType.LinearPermanentMagnet ||
                      motorType == MotorType.RotaryPermanentMagnet))
                    return false;
            }

            // Rotary Motor only - R
            if (attribute.Equals("RotaryMotorPoles") ||
                attribute.Equals("RotaryMotorRatedSpeed"))
            {
                if (axisConfiguration == AxisConfigurationType.ConverterOnly ||
                    axisConfiguration == AxisConfigurationType.FeedbackOnly)
                    return false;

                // Rotary Motor
                if (motorType == MotorType.RotaryInduction ||
                    motorType == MotorType.RotaryPermanentMagnet ||
                    motorType == MotorType.RotaryInteriorPermanentMagnet)
                    return true;

                return false;
            }

            // Rotary Motor only - O
            if (attribute.Equals("RotaryMotorInertia") ||
                attribute.Equals("RotaryMotorMaxSpeed") ||
                attribute.Equals("PMMotorRatedTorque"))
            {
                if (axisConfiguration == AxisConfigurationType.ConverterOnly ||
                    axisConfiguration == AxisConfigurationType.FeedbackOnly)
                    return false;

                // Rotary Motor
                if (!(motorType == MotorType.RotaryInduction ||
                      motorType == MotorType.RotaryPermanentMagnet ||
                      motorType == MotorType.RotaryInteriorPermanentMagnet))
                    return false;
            }

            // Rotary PM Motor only - O
            if (attribute.Equals("PMMotorTorqueConstant"))
            {
                if (axisConfiguration == AxisConfigurationType.ConverterOnly ||
                    axisConfiguration == AxisConfigurationType.FeedbackOnly)
                    return false;

                // Rotary PM Motor
                if (!(motorType == MotorType.RotaryPermanentMagnet ||
                      motorType == MotorType.RotaryInteriorPermanentMagnet))
                    return false;
            }

            // Rotary PM Motor only - R
            if (attribute.Equals("PMMotorRotaryVoltageConstant"))
            {
                if (axisConfiguration == AxisConfigurationType.ConverterOnly ||
                    axisConfiguration == AxisConfigurationType.FeedbackOnly)
                    return false;

                // Rotary PM Motor
                if (motorType == MotorType.RotaryPermanentMagnet ||
                    motorType == MotorType.RotaryInteriorPermanentMagnet)
                    return true;

                return false;
            }

            // Rotary Absolute Only, need check here
            if (attribute.Equals("Feedback1Turns"))
            {
                if (axisConfiguration == AxisConfigurationType.ConverterOnly ||
                    axisConfiguration == AxisConfigurationType.FrequencyControl)
                    return false;

                if (axisConfiguration == AxisConfigurationType.FeedbackOnly)
                {
                    if (feedback1StartupMethod == FeedbackStartupMethodType.Absolute)
                        return true;
                }
                else
                {
                    if (motorType == MotorType.RotaryInduction ||
                        motorType == MotorType.RotaryInteriorPermanentMagnet ||
                        motorType == MotorType.RotaryPermanentMagnet)
                    {
                        if (feedback1StartupMethod == FeedbackStartupMethodType.Absolute)
                            return true;
                    }
                }

                return false;
            }

            // PVT-PM Motors only-O
            if (attribute.Equals("CommutationAlignment"))
            {
                if (axisConfiguration == AxisConfigurationType.ConverterOnly ||
                    axisConfiguration == AxisConfigurationType.FeedbackOnly ||
                    axisConfiguration == AxisConfigurationType.FrequencyControl)
                    return false;

                if (motorType == MotorType.LinearInduction ||
                    motorType == MotorType.RotaryInduction)
                    return false;

                //TODO(gjc):need check here
                attribute = "FeedbackCommutationAligned";
            }

            #endregion

            var cipMotionDrive = axisCIPDrive.AssociatedModule as CIPMotionDrive;
            if (cipMotionDrive?.Profiles != null)
            {
                return cipMotionDrive.Profiles.SupportAxisAttribute(
                    axisConfiguration,
                    attribute,
                    cipMotionDrive.Major);
            }

            return false;
        }
    }
}
