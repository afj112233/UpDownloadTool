using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using ICSStudio.Cip.Objects;
using ICSStudio.Utils;
using Newtonsoft.Json.Linq;

namespace ICSStudio.L5XToJson.Objects
{
    public class AxisParameters
    {
        // CommutationAlignment,FeedbackCommutationAligned
        public static string[] FloatParameters =
        {
            "AccelerationFeedforwardGain", "AccelerationLimit", "ActuatorDiameter", "ActuatorLead",
            "AverageVelocityTimebase", "BacklashCompensationWindow", "BacklashReversalOffset",
            "BrakeSlipTolerance", "BrakeTestTorque", "BusOvervoltageOperationalLimit", "CoastingTimeLimit",
            "CommutationOffset", "ConversionConstant", "ConverterCapacity", "CurrentError", "CurrentVectorLimit",
            "DampingFactor", "DecelerationLimit", "DriveModelTimeConstant",
            "DriveModelTimeConstantBase", "DriveRatedPeakCurrent",
            "Feedback1AccelFilterBandwidth", "Feedback1VelocityFilterBandwidth",
            "FluxCurrentReference", "FrictionCompensationSliding",
            "FrictionCompensationStatic", "FrictionCompensationViscous",
            "FrictionCompensationWindow", "HomeOffset", "HomePosition",
            "HomeReturnSpeed", "HomeSpeed", "HookupTestDistance",
            "InverterCapacity", "InverterThermalOverloadUserLimit",
            "LoadInertiaRatio", "LoadObserverBandwidth", "LoadObserverFeedbackGain",
            "LoadObserverIntegratorBandwidth", "LoadRatio",
            "MasterPositionFilterBandwidth", "MaximumAcceleration",
            "MaximumAccelerationJerk", "MaximumDeceleration",
            "MaximumDecelerationJerk", "MaximumSpeed",
            "MechanicalBrakeEngageDelay", "MechanicalBrakeReleaseDelay",
            "MotorMaxWindingTemperature", "MotorOverloadLimit",
            "MotorOverspeedUserLimit", "MotorRatedContinuousCurrent",
            "MotorRatedOutputPower", "MotorRatedPeakCurrent",
            "MotorRatedVoltage", "MotorTestInductance",
            "MotorTestResistance", "MotorThermalOverloadUserLimit",
            "MotorWindingToAmbientCapacitance", "MotorWindingToAmbientResistance",
            "OvertorqueLimit", "OvertorqueLimitTime", "PMMotorInductance",
            "PMMotorRatedTorque", "PMMotorResistance", "PMMotorRotaryVoltageConstant",
            "PMMotorTorqueConstant", "PositionErrorTolerance", "PositionErrorToleranceTime",
            "PositionIntegratorBandwidth", "PositionLeadLagFilterBandwidth",
            "PositionLeadLagFilterGain", "PositionLockTolerance", "PositionLoopBandwidth",
            "PositionScalingDenominator", "PositionScalingNumerator", "PositionServoBandwidth",
            "PositionTrim", "PositionUnwindDenominator",
            "PositionUnwindNumerator", "RotaryMotorInertia", "RotaryMotorMaxSpeed",
            "RotaryMotorRatedSpeed", "SoftTravelLimitNegative",
            "SoftTravelLimitPositive", "StoppingTimeLimit", "StoppingTorque",
            "SystemAccelerationBase", "SystemBandwidth", "SystemDamping", "SystemInertia",
            "TorqueLeadLagFilterBandwidth", "TorqueLeadLagFilterGain", "TorqueLimitNegative",
            "TorqueLimitPositive", "TorqueLoopBandwidth",
            "TorqueLowPassFilterBandwidth", "TorqueNotchFilterFrequency",
            "TorqueNotchFilterHighFrequencyLimit", "TorqueNotchFilterLowFrequencyLimit",
            "TorqueNotchFilterTuningThreshold", "TorqueOffset",
            "TorqueProveCurrent", "TorqueRateLimit", "TorqueThreshold", "TorqueTrim",
            "TotalInertia", "TravelRange",
            "TuneFriction", "TuneInertiaMass", "TuneLoadOffset", "TuningSpeed", "TuningTorque",
            "TuningTravelLimit", "UndertorqueLimit", "UndertorqueLimitTime", "VelocityDroop",
            "VelocityErrorTolerance", "VelocityErrorToleranceTime", "VelocityFeedforwardGain",
            "VelocityIntegratorBandwidth", "VelocityLimitNegative", "VelocityLimitPositive",
            "VelocityLockTolerance", "VelocityLoopBandwidth",
            "VelocityLowPassFilterBandwidth", "VelocityNegativeFeedforwardGain", "VelocityOffset",
            "VelocityServoBandwidth", "VelocityStandstillWindow", "VelocityThreshold",
            "VelocityTrim", "ZeroSpeed", "ZeroSpeedTime"
        };

        public static string[] EnumParameters =
        {
            "ActuatorDiameterUnit", "ActuatorLeadUnit",
            "ActuatorType", "AdaptiveTuningConfiguration",
            "ApplicationType", "AxisConfiguration",
            "AxisUpdateSchedule", "Feedback1StartupMethod",
            "Feedback1Type", "Feedback1Unit",
            "FeedbackConfiguration", "HomeDirection",
            "HomeMode", "HomeSequence",
            "InverterOverloadAction", "LoadType",
            "LoopResponse", "MechanicalBrakeControl",
            "MotionPolarity", "MotionScalingConfiguration",
            "MotorDataSource", "MotorOverloadAction",
            "MotorPolarity", "MotorType", "MotorUnit",
            "ProgrammedStopMode", "ProvingConfiguration",
            "ScalingSource", "StoppingAction", "TravelMode",
            "TuningDirection", "TuningSelect",
            "FeedbackCommutationAligned", "HookupTestFeedbackChannel",
            "LoadCoupling", "LoadObserverConfiguration"
        };

        public static string[] IntParameters =
        {
            "AxisID", "CommutationAlignment",
            "DynamicsConfigurationBits", "Feedback1AccelFilterTaps",
            "Feedback1CycleInterpolation",
            "Feedback1CycleResolution", "Feedback1Turns",
            "Feedback1VelocityFilterTaps",
            "FeedbackDataLossUserLimit",
            "GainTuningConfigurationBits", "HomeConfigurationBits",
            "InterpolatedPositionConfiguration",
            "MasterInputConfigurationBits", "MotionResolution",
            "MotorDeviceCode", "OutputCamExecutionTargets",
            "PositionIntegratorControl", "PositionUnwind",
            "RegistrationInputs", "RotaryMotorPoles",
            "TransmissionRatioInput", "TransmissionRatioOutput",
            "VelocityIntegratorControl",
        };

        public static string[] ArrayParameters =
        {
            "CIPAxisExceptionAction", "CIPAxisExceptionActionRA",
            "MotionExceptionAction", "PMMotorFluxSaturation",
        };

        public static string[] StringParameters =
        {
            "MotionGroup", "MotionModule", "MotorCatalogNumber",
            "PositionUnits",
        };

        public static string[] BoolParameters = new[]
        {
            "MotorIntegralThermalSwitch", "SoftTravelLimitChecking",

        };

        public static string[] IgnoreParameters = {""};

        public static JObject ToJObject(XmlElement parameters)
        {
            JObject axisParameters = new JObject();

            List<string> attributeNames = new List<string>();
            foreach (XmlAttribute attribute in parameters.Attributes)
            {
                attributeNames.Add(attribute.Name);
            }

            attributeNames.Sort();

            foreach (var attributeName in attributeNames)
            {
                if (IgnoreParameters.Contains(attributeName))
                    continue;

                axisParameters.Add(
                    attributeName.Equals("CIPAxisExceptionActionRA") ? "CIPAxisExceptionActionMfg" : attributeName,
                    GetValue(attributeName, parameters.Attributes[attributeName].Value));
            }


            // CommutationAlignment
            if(axisParameters.ContainsKey("FeedbackCommutationAligned"))
                axisParameters.Add("CommutationAlignment", axisParameters["FeedbackCommutationAligned"]);

            // ControlMethod
            // ControlMode
            byte controlMode, controlMethod;
            UpdateControlMethodAndMode(
                (AxisConfigurationType) (byte) axisParameters["AxisConfiguration"], out controlMode,
                out controlMethod);
            axisParameters.Add("ControlMethod", controlMethod);
            axisParameters.Add("ControlMode", controlMode);

            // FeedbackMode
            axisParameters.Add("FeedbackMode", axisParameters["FeedbackConfiguration"]);

            // PositionTrim
            // VelocityTrim
            axisParameters.Add("PositionTrim", 0f);
            axisParameters.Add("VelocityTrim", 0f);

            return axisParameters;
        }

        private static JToken GetValue(string name, string value)
        {
            try
            {
                if (StringParameters.Contains(name))
                    return value;
                if (FloatParameters.Contains(name))
                    return float.Parse(value);
                if (IntParameters.Contains(name))
                {
                    if (value.StartsWith("16#"))
                    {
                        value = value.Replace("16#", "");
                        value = value.Replace("_", "");
                        return Convert.ToInt32(value, 16);
                    }

                    uint result;
                    if (uint.TryParse(value, out result))
                        return result;

                    return int.Parse(value);
                }

                if (BoolParameters.Contains(name))
                    return bool.Parse(value);

                if (EnumParameters.Contains(name))
                {
                    return EnumParser(name, value);
                }

                if (ArrayParameters.Contains(name))
                {
                    return ArrayParser(name, value);
                }

                throw new ApplicationException(name);
            }
            catch (Exception)
            {
                Console.WriteLine($"{name},{value} failed!");
                throw;
            }


        }

        private static JArray ArrayParser(string name, string value)
        {
            JArray array = new JArray();
            if (name.Equals("CIPAxisExceptionAction")
                || name.Equals("CIPAxisExceptionActionRA")
                || name.Equals("MotionExceptionAction"))
            {
                var values = value.Split(' ');
                foreach (var s in values)
                {
                    array.Add((int) EnumUtils.Parse<ExceptionActionType>(s));
                }
            }
            else if (name.Equals("PMMotorFluxSaturation"))
            {
                var values = value.Split(' ');
                foreach (var s in values)
                {
                    array.Add(float.Parse(s));
                }
            }

            return array;
        }

        private static int EnumParser(string name, string value)
        {
            switch (name)
            {
                case "ActuatorDiameterUnit":
                    return (int) EnumUtils.Parse<ActuatorDiameterUnitType>(value);
                case "ActuatorLeadUnit":
                    return (int) EnumUtils.Parse<ActuatorLeadUnitType>(value);
                case "ActuatorType":
                    return (int) EnumUtils.Parse<ActuatorType>(value);
                case "AdaptiveTuningConfiguration":
                    return (int) EnumUtils.Parse<AdaptiveTuningConfigurationType>(value);
                case "ApplicationType":
                    return (int) EnumUtils.Parse<ApplicationType>(value);
                case "AxisConfiguration":
                    return (int) EnumUtils.Parse<AxisConfigurationType>(value);
                case "AxisUpdateSchedule":
                    return (int) EnumUtils.Parse<AxisUpdateScheduleType>(value);
                case "Feedback1StartupMethod":
                    return (int) EnumUtils.Parse<FeedbackStartupMethodType>(value);
                case "Feedback1Type":
                    return (int) EnumUtils.Parse<FeedbackType>(value);
                case "Feedback1Unit":
                    return (int) EnumUtils.Parse<FeedbackUnitType>(value);
                case "FeedbackConfiguration":
                    return (int) EnumUtils.Parse<FeedbackConfigurationType>(value);
                case "HomeDirection":
                {
                    switch (value)
                    {
                        case "Uni-directional Forward":
                            return 0;
                        case "Bi-directional Forward":
                            return 1;
                        case "Uni-directional Reverse":
                            return 2;
                        case "Bi-directional Reverse":
                            return 3;
                        default:
                            throw new ApplicationException($"Parse {name}:{value} failed!");
                    }
                }
                case "HomeMode":
                    return (int) EnumUtils.Parse<HomeModeType>(value);
                case "HomeSequence":
                    return (int) EnumUtils.Parse<HomeSequenceType>(value);
                case "InverterOverloadAction":
                    return (int) EnumUtils.Parse<InverterOverloadActionType>(value);
                case "LoadType":
                    return (int) EnumUtils.Parse<LoadType>(value);
                case "LoopResponse":
                    return (int) EnumUtils.Parse<LoopResponseType>(value);
                case "MechanicalBrakeControl":
                    return (int) EnumUtils.Parse<MechanicalBrakeControlType>(value);
                case "MotionPolarity":
                    return (int) EnumUtils.Parse<PolarityType>(value);
                case "MotionScalingConfiguration":
                    return (int) EnumUtils.Parse<MotionScalingConfigurationType>(value);
                case "MotorDataSource":
                    return (int) EnumUtils.Parse<MotorDataSourceType>(value);
                case "MotorOverloadAction":
                    return (int) EnumUtils.Parse<MotorOverloadActionType>(value);
                case "MotorPolarity":
                    return (int) EnumUtils.Parse<PolarityType>(value);
                case "MotorType":
                    return (int) EnumUtils.Parse<MotorType>(value);
                case "MotorUnit":
                    return (int) EnumUtils.Parse<MotorUnitType>(value);
                case "ProgrammedStopMode":
                    return (int) EnumUtils.Parse<ProgrammedStopModeType>(value);
                case "ProvingConfiguration":
                    return (int) EnumUtils.Parse<BooleanType>(value);
                case "ScalingSource":
                    return (int) EnumUtils.Parse<ScalingSourceType>(value);
                case "StoppingAction":
                    return (int) EnumUtils.Parse<StoppingActionType>(value);
                case "TravelMode":
                    return (int) EnumUtils.Parse<TravelModeType>(value);
                case "TuningDirection":
                {
                    switch (value)
                    {
                        case "Uni-directional Forward":
                            return 0;
                        case "Uni-directional Reverse":
                            return 1;
                        case "Bi-directional Forward":
                            return 2;
                        case "Bi-directional Reverse":
                            return 3;
                        default:
                            throw new ApplicationException($"Parse {name}:{value} failed!");
                    }
                }
                case "TuningSelect":
                    return (int) EnumUtils.Parse<TuningSelectType>(value);
                case "FeedbackCommutationAligned":
                    return (int) EnumUtils.Parse<FeedbackCommutationAlignedType>(value);
                case "HookupTestFeedbackChannel":
                    return (int) EnumUtils.Parse<HookupTestFeedbackChannelType>(value);
                case "LoadCoupling":
                    return (int) EnumUtils.Parse<LoadCouplingType>(value);
                case "LoadObserverConfiguration":
                    return (int) EnumUtils.Parse<BooleanType>(value);

                default:
                    throw new ApplicationException($"add parse for {name}:{value}");
            }
        }

        private static void UpdateControlMethodAndMode(AxisConfigurationType axisConfiguration,
            out byte controlMode, out byte controlMethod)
        {
            // rm003, page69
            switch (axisConfiguration)
            {
                case AxisConfigurationType.FeedbackOnly:
                    controlMode = (byte) ControlModeType.NoControl;
                    controlMethod = (byte) ControlMethodType.NoControl;
                    break;
                case AxisConfigurationType.FrequencyControl:
                    controlMode = (byte) ControlModeType.VelocityControl;
                    controlMethod = (byte) ControlMethodType.FrequencyControl;
                    break;
                case AxisConfigurationType.PositionLoop:
                    controlMode = (byte) ControlModeType.PositionControl;
                    controlMethod = (byte) ControlMethodType.PIVectorControl;
                    break;
                case AxisConfigurationType.VelocityLoop:
                    controlMode = (byte) ControlModeType.VelocityControl;
                    controlMethod = (byte) ControlMethodType.PIVectorControl;
                    break;
                case AxisConfigurationType.TorqueLoop:
                    controlMode = (byte) ControlModeType.TorqueControl;
                    controlMethod = (byte) ControlMethodType.PIVectorControl;
                    break;
                case AxisConfigurationType.ConverterOnly:
                    controlMode = (byte) ControlModeType.NoControl;
                    controlMethod = (byte) ControlMethodType.NoControl;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axisConfiguration), axisConfiguration, null);
            }
        }
    }
}