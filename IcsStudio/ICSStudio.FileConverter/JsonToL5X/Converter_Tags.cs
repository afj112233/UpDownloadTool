using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Xml;
using ICSStudio.Cip.Objects;
using ICSStudio.FileConverter.JsonToL5X.Model;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.PredefinedType;
using MessageTypeEnum = ICSStudio.FileConverter.JsonToL5X.Model.MessageTypeEnum;
using TagType = ICSStudio.FileConverter.JsonToL5X.Model.TagType;

namespace ICSStudio.FileConverter.JsonToL5X
{
    public static partial class Converter
    {
        private static TagType ToTagType(Tag tag)
        {
            TagType tagType = new TagType();

            tagType.Name = tag.Name;
            tagType.TagType1 = ToTagTypeEnum(tag.TagType);
            tagType.DataType = tag.DataTypeInfo.DataType.Name;
            tagType.Dimensions = GetDimensions(tag.DataTypeInfo, " ");
            tagType.Usage = ToTagUsageEnum(tag.Usage);
            if (tagType.Usage == TagUsageEnum.Local || tag.ParentCollection.ParentProgram == null)
            {
                tagType.UsageSpecified = false;
            }
            else
            {
                tagType.UsageSpecified = true;
            }

            tagType.Radix = ToRadix(tag.DisplayStyle);
            if (tag.DataTypeInfo.DataType.IsAtomic)
            {
                tagType.RadixSpecified = true;
            }

            if (!tag.DataTypeInfo.DataType.IsMotionGroupType
                && !tag.DataTypeInfo.DataType.IsAxisType
                && !tag.DataTypeInfo.DataType.IsCoordinateSystemType
                && !tag.DataTypeInfo.DataType.IsMessageType)
            {
                tagType.Constant = tag.IsConstant ? BoolEnum.@true : BoolEnum.@false;
                tagType.ConstantSpecified = true;
            }

            tagType.ExternalAccess = ToExternalAccess(tag.ExternalAccess);

            tagType.Items = ToTagItems(tag);

            return tagType;
        }

        private static TagUsageEnum ToTagUsageEnum(Usage usage)
        {
            switch (usage)
            {
                case Usage.Static:
                    return TagUsageEnum.Static;
                case Usage.Input:
                    return TagUsageEnum.Input;
                case Usage.Output:
                    return TagUsageEnum.Output;
                case Usage.InOut:
                    return TagUsageEnum.InOut;
                case Usage.Local:
                    return TagUsageEnum.Local;
                case Usage.NullParameterType:
                    return TagUsageEnum.NULL;
                case Usage.SharedData:
                    return TagUsageEnum.Public;
                default:
                    throw new ArgumentOutOfRangeException(nameof(usage), usage, null);
            }
        }

        private static string GetDimensions(DataTypeInfo dataTypeInfo, string separator)
        {
            int dim1 = dataTypeInfo.Dim1;
            int dim2 = dataTypeInfo.Dim2;
            int dim3 = dataTypeInfo.Dim3;

            if (dim3 > 0)
                return $"{dim3}{separator}{dim2}{separator}{dim1}";
            if (dim2 > 0)
                return $"{dim2}{separator}{dim1}";
            if (dim1 > 0)
                return $"{dim1}";

            return null;
        }

        private static RadixEnum ToRadix(DisplayStyle displayStyle)
        {
            switch (displayStyle)
            {
                case DisplayStyle.NullStyle:
                    return RadixEnum.NullType;
                case DisplayStyle.General:
                    return RadixEnum.General;
                case DisplayStyle.Binary:
                    return RadixEnum.Binary;
                case DisplayStyle.Octal:
                    return RadixEnum.Octal;
                case DisplayStyle.Decimal:
                    return RadixEnum.Decimal;
                case DisplayStyle.Hex:
                    return RadixEnum.Hex;
                case DisplayStyle.Exponential:
                    return RadixEnum.Exponential;
                case DisplayStyle.Float:
                    return RadixEnum.Float;
                case DisplayStyle.Ascii:
                    return RadixEnum.Ascii;
                case DisplayStyle.Unicode:
                    return RadixEnum.Unicode;
                case DisplayStyle.DateTime:
                    return RadixEnum.DateTime;
                case DisplayStyle.UseTypeStyle:
                    return RadixEnum.UseTypeStyle;
                case DisplayStyle.DateTimeNS:
                    return RadixEnum.DateTimens;
                default:
                    throw new ArgumentOutOfRangeException(nameof(displayStyle), displayStyle, null);
            }
        }

        private static TagTypeEnum ToTagTypeEnum(Interfaces.Tags.TagType tagTagType)
        {
            switch (tagTagType)
            {
                case Interfaces.Tags.TagType.Base:
                    return TagTypeEnum.Base;
                case Interfaces.Tags.TagType.Alias:
                    return TagTypeEnum.Alias;
                case Interfaces.Tags.TagType.Produced:
                    return TagTypeEnum.Produced;
                case Interfaces.Tags.TagType.Consumed:
                    return TagTypeEnum.Consumed;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tagTagType), tagTagType, null);
            }
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static object[] ToTagItems(Tag tag)
        {
            List<object> tagItems = new List<object>();

            DescriptionType descriptionType = ToDescription(tag.Description);
            if (descriptionType != null)
                tagItems.Add(descriptionType);

            Data l5kData = ToL5KData(tag);
            if (l5kData != null)
                tagItems.Add(l5kData);

            Data decoratedData = ToDecoratedData(tag);
            if (decoratedData != null)
                tagItems.Add(decoratedData);

            Data stringData = ToStringData(tag);
            if (stringData != null)
                tagItems.Add(stringData);

            Data motionGroupData = ToMotionGroupData(tag);
            if (motionGroupData != null)
                tagItems.Add(motionGroupData);

            Data axisCIPDriveData = ToAxisCIPDriveData(tag);
            if (axisCIPDriveData != null)
                tagItems.Add(axisCIPDriveData);

            Data axisVirtualData = ToAxisVirtualData(tag);
            if (axisVirtualData != null)
                tagItems.Add(axisVirtualData);

            Data messageData = ToMessageData(tag);
            if (messageData != null)
                tagItems.Add(messageData);

            //TODO(gjc):add code here

            if (tagItems.Count == 0)
                return null;

            return tagItems.ToArray();

        }

        private static Data ToStringData(Tag tag)
        {
            if (tag.DataTypeInfo.DataType.IsStringType && tag.DataTypeInfo.Dim1 == 0)
            {
                Data stringData = new Data();

                stringData.Format = "String";

                var compositeField = tag.DataWrapper.Data as ICompositeField;
                Contract.Assert(compositeField != null);

                Int32Field lenField = compositeField.fields[0].Item1 as Int32Field;
                Contract.Assert(lenField != null);

                stringData.Length = (ulong)lenField.value;
                stringData.LengthSpecified = true;

                string value = compositeField.ToString(DisplayStyle.NullStyle);

                XmlDocument xmlDocument = new XmlDocument();

                stringData.Text = new XmlNode[] { xmlDocument.CreateCDataSection(value) };

                return stringData;
            }

            return null;
        }

        private static Data ToMessageData(Tag tag)
        {
            Data messageData = new Data();

            MessageDataWrapper dataWrapper = tag.DataWrapper as MessageDataWrapper;
            if (dataWrapper == null)
                return null;

            messageData.Format = "Message";

            MessageType messageType = GetMessageType(dataWrapper);

            messageData.Items = new object[] { messageType };

            return messageData;
        }

        private static MessageType GetMessageType(MessageDataWrapper dataWrapper)
        {
            MessageType messageType = new MessageType();

            messageType.MessageType1 = (MessageTypeEnum)dataWrapper.Parameters.MessageType;

            if (messageType.MessageType1 == MessageTypeEnum.Unconfigured)
            {
                messageType.RequestedLength = dataWrapper.Parameters.RequestedLength;
                messageType.RequestedLengthSpecified = true;

                messageType.CommTypeCode = (MsgDF1FlagEnum)dataWrapper.Parameters.CommTypeCode;
                messageType.CommTypeCodeSpecified = true;

                messageType.LocalIndex = dataWrapper.Parameters.LocalIndex;
                messageType.LocalIndexSpecified = true;
            }
            else
            {
                messageType.RequestedLength = dataWrapper.Parameters.RequestedLength;
                messageType.RequestedLengthSpecified = true;

                messageType.ConnectedFlag = ToConnectedEnum(dataWrapper.Parameters.ConnectedFlag);
                messageType.ConnectedFlagSpecified = true;

                messageType.ConnectionPath = dataWrapper.Parameters.ConnectionPath;

                messageType.CommTypeCode = (MsgDF1FlagEnum)dataWrapper.Parameters.CommTypeCode;
                messageType.CommTypeCodeSpecified = true;

                messageType.ServiceCode = dataWrapper.Parameters.ServiceCode.ToString(DisplayStyle.Hex);
                messageType.ObjectType = dataWrapper.Parameters.ObjectType.ToString(DisplayStyle.Hex);

                messageType.TargetObject = dataWrapper.Parameters.TargetObject;
                messageType.TargetObjectSpecified = true;

                messageType.AttributeNumber = dataWrapper.Parameters.AttributeNumber.ToString(DisplayStyle.Hex);

                messageType.LocalIndex = dataWrapper.Parameters.LocalIndex;
                messageType.LocalIndexSpecified = true;

                messageType.LocalElement = dataWrapper.Parameters.LocalElement;

                messageType.DestinationTag = dataWrapper.Parameters.DestinationTag;

                messageType.LargePacketUsage =
                    dataWrapper.Parameters.LargePacketUsage ? BoolEnum.@true : BoolEnum.@false;
                messageType.LargePacketUsageSpecified = true;
            }



            return messageType;
        }

        private static ConnectedEnum ToConnectedEnum(byte connectedFlag)
        {
            if (connectedFlag == 1)
                return ConnectedEnum.Item1;

            if (connectedFlag == 2)
                return ConnectedEnum.Item2;

            throw new NotImplementedException("Check here for ConnectedEnum!");
        }

        private static Data ToAxisVirtualData(Tag tag)
        {
            Data axisVirtualData = new Data();

            AxisVirtual axisVirtual = tag.DataWrapper as AxisVirtual;
            if (axisVirtual == null)
                return null;

            axisVirtualData.Format = "Axis";

            AxisType axisType = GetAxisType(axisVirtual);

            axisVirtualData.Items = new object[] { axisType };

            return axisVirtualData;
        }

        private static Data ToAxisCIPDriveData(Tag tag)
        {
            Data axisCIPDriveData = new Data();

            AxisCIPDrive axisCIPDrive = tag.DataWrapper as AxisCIPDrive;
            if (axisCIPDrive == null)
                return null;

            axisCIPDriveData.Format = "Axis";

            AxisType axisType = GetAxisType(axisCIPDrive);

            axisCIPDriveData.Items = new object[] { axisType };

            return axisCIPDriveData;
        }

        private static AxisType GetAxisType(AxisCIPDrive axisCIPDrive)
        {
            AxisType axisType = new AxisType();

            var parameters = axisCIPDrive.Parameters;

            axisType.MotionGroup = axisCIPDrive.AssignedGroup?.Name;

            axisType.MotionModule = axisCIPDrive.AssociatedModule != null
                ? $"{axisCIPDrive.AssociatedModule.Name}:Ch{axisCIPDrive.AxisNumber}"
                : "<NA>";

            axisType.AxisConfiguration = (AxisConfigurationEnum)parameters.AxisConfiguration;
            axisType.AxisConfigurationSpecified = true;

            axisType.FeedbackConfiguration = (AxisFeedbackConfigurationEnum)parameters.FeedbackConfiguration;
            axisType.FeedbackConfigurationSpecified = true;

            axisType.MotorDataSource = (AxisMotorDataSourceEnum)parameters.MotorDataSource;

            axisType.MotorCatalogNumber = parameters.MotorCatalogNumber;
            axisType.MotorDeviceCode = parameters.MotorDeviceCode.ToString(DisplayStyle.Hex);

            axisType.Feedback1Type = (AxisFeedbackTypeEnum)parameters.Feedback1Type;
            axisType.Feedback1TypeSpecified = true;

            axisType.MotorType = (AxisMotorTypeEnum)parameters.MotorType;

            axisType.MotionScalingConfiguration =
                (AxisMotionScalingConfigurationEnum)parameters.MotionScalingConfiguration;
            axisType.MotionScalingConfigurationSpecified = true;

            axisType.ConversionConstant = parameters.ConversionConstant;
            axisType.ConversionConstantSpecified = true;

            axisType.OutputCamExecutionTargets = parameters.OutputCamExecutionTargets;
            axisType.OutputCamExecutionTargetsSpecified = true;

            axisType.PositionUnits = parameters.PositionUnits;

            axisType.AverageVelocityTimebase = parameters.AverageVelocityTimebase;
            axisType.AverageVelocityTimebaseSpecified = true;

            axisType.PositionUnwind = parameters.PositionUnwind;

            axisType.HomeMode = (AxisHomeModeEnum)parameters.HomeMode;
            axisType.HomeModeSpecified = true;

            axisType.HomeDirection = (AxisHomeDirEnum)parameters.HomeDirection;
            axisType.HomeDirectionSpecified = true;

            axisType.HomeSequence = (AxisHomeSeqEnum)parameters.HomeSequence;
            axisType.HomeSequenceSpecified = true;

            axisType.HomeConfigurationBits = parameters.HomeConfigurationBits.ToString(DisplayStyle.Hex);

            axisType.HomePosition = parameters.HomePosition;
            axisType.HomePositionSpecified = true;

            axisType.HomeOffset = parameters.HomeOffset;
            axisType.HomeOffsetSpecified = true;

            axisType.HomeSpeed = parameters.HomeSpeed;
            axisType.HomeSpeedSpecified = true;

            axisType.HomeReturnSpeed = parameters.HomeReturnSpeed;
            axisType.HomeReturnSpeedSpecified = true;

            axisType.MaximumSpeed = parameters.MaximumSpeed;
            axisType.MaximumSpeedSpecified = true;

            axisType.MaximumAcceleration = parameters.MaximumAcceleration;
            axisType.MaximumAccelerationSpecified = true;

            axisType.MaximumDeceleration = parameters.MaximumDeceleration;
            axisType.MaximumDecelerationSpecified = true;

            axisType.ProgrammedStopMode = (AxisProgStopModeEnum)parameters.ProgrammedStopMode;
            axisType.ProgrammedStopModeSpecified = true;

            axisType.MasterInputConfigurationBits = parameters.MasterInputConfigurationBits;
            axisType.MasterInputConfigurationBitsSpecified = true;

            axisType.MasterPositionFilterBandwidth = parameters.MasterPositionFilterBandwidth;
            axisType.MasterPositionFilterBandwidthSpecified = true;

            axisType.VelocityFeedforwardGain = parameters.VelocityFeedforwardGain;
            axisType.VelocityFeedforwardGainSpecified = true;

            axisType.AccelerationFeedforwardGain = parameters.AccelerationFeedforwardGain;
            axisType.AccelerationFeedforwardGainSpecified = true;

            axisType.PositionErrorTolerance = parameters.PositionErrorTolerance;
            axisType.PositionErrorToleranceSpecified = true;

            axisType.PositionLockTolerance = parameters.PositionLockTolerance;
            axisType.PositionLockToleranceSpecified = true;

            axisType.VelocityOffset = parameters.VelocityOffset;
            axisType.VelocityOffsetSpecified = true;

            axisType.TorqueOffset = parameters.TorqueOffset;

            axisType.FrictionCompensationWindow = parameters.FrictionCompensationWindow;
            axisType.FrictionCompensationWindowSpecified = true;

            axisType.BacklashReversalOffset = parameters.BacklashReversalOffset;
            axisType.BacklashReversalOffsetSpecified = true;

            axisType.TuningTravelLimit = parameters.TuningTravelLimit;

            axisType.TuningSpeed = parameters.TuningSpeed;

            axisType.TuningTorque = parameters.TuningTorque;

            axisType.DampingFactor = parameters.DampingFactor;

            axisType.DriveModelTimeConstant = parameters.DriveModelTimeConstant;

            axisType.PositionServoBandwidth = parameters.PositionServoBandwidth;
            axisType.PositionServoBandwidthSpecified = true;

            axisType.VelocityServoBandwidth = parameters.VelocityServoBandwidth;
            axisType.VelocityServoBandwidthSpecified = true;

            axisType.VelocityDroop = parameters.VelocityDroop;
            axisType.VelocityDroopSpecified = true;

            axisType.VelocityLimitPositive = parameters.VelocityLimitPositive;
            axisType.VelocityLimitPositiveSpecified = true;

            axisType.VelocityLimitNegative = parameters.VelocityLimitNegative;
            axisType.VelocityLimitNegativeSpecified = true;

            axisType.VelocityThreshold = parameters.VelocityThreshold;
            axisType.VelocityThresholdSpecified = true;

            axisType.VelocityStandstillWindow = parameters.VelocityStandstillWindow;
            axisType.VelocityStandstillWindowSpecified = true;

            axisType.TorqueLimitPositive = parameters.TorqueLimitPositive;

            axisType.TorqueLimitNegative = parameters.TorqueLimitNegative;

            axisType.TorqueThreshold = parameters.TorqueThreshold;
            axisType.TorqueThresholdSpecified = true;

            axisType.StoppingTorque = parameters.StoppingTorque;

            axisType.StoppingTimeLimit = parameters.StoppingTimeLimit;
            axisType.StoppingTimeLimitSpecified = true;

            axisType.LoadInertiaRatio = parameters.LoadInertiaRatio;

            axisType.RegistrationInputs = parameters.RegistrationInputs;
            axisType.RegistrationInputsSpecified = true;

            axisType.MaximumAccelerationJerk = parameters.MaximumAccelerationJerk;
            axisType.MaximumAccelerationJerkSpecified = true;

            axisType.MaximumDecelerationJerk = parameters.MaximumDecelerationJerk;
            axisType.MaximumDecelerationJerkSpecified = true;

            axisType.DynamicsConfigurationBits = parameters.DynamicsConfigurationBits;
            axisType.DynamicsConfigurationBitsSpecified = true;

            axisType.FeedbackUnitRatio = parameters.FeedbackUnitRatio;
            axisType.FeedbackUnitRatioSpecified = true;

            axisType.AccelerationLimit = parameters.AccelerationLimit;
            axisType.AccelerationLimitSpecified = true;

            axisType.DecelerationLimit = parameters.DecelerationLimit;
            axisType.DecelerationLimitSpecified = true;

            axisType.PositionIntegratorBandwidth = parameters.PositionIntegratorBandwidth;
            axisType.PositionIntegratorBandwidthSpecified = true;

            axisType.PositionErrorToleranceTime = parameters.PositionErrorToleranceTime;
            axisType.PositionErrorToleranceTimeSpecified = true;

            axisType.PositionIntegratorControl = parameters.PositionIntegratorControl;
            axisType.PositionIntegratorControlSpecified = true;

            axisType.VelocityErrorTolerance = parameters.VelocityErrorTolerance;
            axisType.VelocityErrorToleranceSpecified = true;

            axisType.VelocityErrorToleranceTime = parameters.VelocityErrorToleranceTime;
            axisType.VelocityErrorToleranceTimeSpecified = true;

            axisType.VelocityIntegratorControl = parameters.VelocityIntegratorControl;
            axisType.VelocityIntegratorControlSpecified = true;

            axisType.VelocityLockTolerance = parameters.VelocityLockTolerance;
            axisType.VelocityLockToleranceSpecified = true;

            axisType.SystemInertia = parameters.SystemInertia;
            axisType.SystemInertiaSpecified = true;

            axisType.TorqueLowPassFilterBandwidth = parameters.TorqueLowPassFilterBandwidth;
            axisType.TorqueLowPassFilterBandwidthSpecified = true;

            axisType.TorqueNotchFilterFrequency = parameters.TorqueNotchFilterFrequency;
            axisType.TorqueNotchFilterFrequencySpecified = true;

            axisType.TorqueRateLimit = parameters.TorqueRateLimit;
            axisType.TorqueRateLimitSpecified = true;

            axisType.OvertorqueLimit = parameters.OvertorqueLimit;
            axisType.OvertorqueLimitSpecified = true;

            axisType.OvertorqueLimitTime = parameters.OvertorqueLimitTime;
            axisType.OvertorqueLimitTimeSpecified = true;

            axisType.UndertorqueLimit = parameters.UndertorqueLimit;
            axisType.UndertorqueLimitSpecified = true;

            axisType.UndertorqueLimitTime = parameters.UndertorqueLimitTime;
            axisType.UndertorqueLimitTimeSpecified = true;

            axisType.FluxCurrentReference = parameters.FluxCurrentReference;
            axisType.FluxCurrentReferenceSpecified = true;

            axisType.CurrentError = parameters.CurrentError;
            axisType.CurrentErrorSpecified = true;

            axisType.TorqueLoopBandwidth = parameters.TorqueLoopBandwidth;
            axisType.TorqueLoopBandwidthSpecified = true;

            axisType.StoppingAction = (AxisStoppingActionEnum)parameters.StoppingAction;

            axisType.MechanicalBrakeControl = (AxisMechanicalBrakeControlEnum)parameters.MechanicalBrakeControl;
            axisType.MechanicalBrakeControlSpecified = true;

            axisType.MechanicalBrakeReleaseDelay = parameters.MechanicalBrakeReleaseDelay;
            axisType.MechanicalBrakeReleaseDelaySpecified = true;

            axisType.MechanicalBrakeEngageDelay = parameters.MechanicalBrakeEngageDelay;
            axisType.MechanicalBrakeEngageDelaySpecified = true;

            axisType.InverterCapacity = parameters.InverterCapacity;
            axisType.InverterCapacitySpecified = true;

            axisType.ConverterCapacity = parameters.ConverterCapacity;
            axisType.ConverterCapacitySpecified = true;

            axisType.InverterOverloadAction = (AxisInverterOverloadActionEnum)parameters.InverterOverloadAction;

            axisType.MotorOverloadAction = (AxisMotorOverloadActionEnum)parameters.MotorOverloadAction;

            axisType.MotorOverspeedUserLimit = parameters.MotorOverspeedUserLimit;
            axisType.MotorOverspeedUserLimitSpecified = true;

            axisType.MotorThermalOverloadUserLimit = parameters.MotorThermalOverloadUserLimit;
            axisType.MotorThermalOverloadUserLimitSpecified = true;

            axisType.InverterThermalOverloadUserLimit = parameters.InverterThermalOverloadUserLimit;
            axisType.InverterThermalOverloadUserLimitSpecified = true;

            axisType.PositionLeadLagFilterBandwidth = parameters.PositionLeadLagFilterBandwidth;
            axisType.PositionLeadLagFilterBandwidthSpecified = true;

            axisType.PositionLeadLagFilterGain = parameters.PositionLeadLagFilterGain;
            axisType.PositionLeadLagFilterGainSpecified = true;

            axisType.VelocityNegativeFeedforwardGain = parameters.VelocityNegativeFeedforwardGain;
            axisType.VelocityNegativeFeedforwardGainSpecified = true;

            axisType.BacklashCompensationWindow = parameters.BacklashCompensationWindow;
            axisType.BacklashCompensationWindowSpecified = true;

            axisType.TorqueLeadLagFilterBandwidth = parameters.TorqueLeadLagFilterBandwidth;
            axisType.TorqueLeadLagFilterBandwidthSpecified = true;

            axisType.TorqueLeadLagFilterGain = parameters.TorqueLeadLagFilterGain;
            axisType.TorqueLeadLagFilterGainSpecified = true;

            axisType.MotorUnit = (AxisMotorUnitEnum)parameters.MotorUnit;

            axisType.MotorPolarity = (AxisPolarityEnum)parameters.MotorPolarity;
            axisType.MotorPolaritySpecified = true;

            axisType.MotorRatedVoltage = parameters.MotorRatedVoltage;
            axisType.MotorRatedVoltageSpecified = true;

            axisType.MotorRatedContinuousCurrent = parameters.MotorRatedContinuousCurrent;
            axisType.MotorRatedContinuousCurrentSpecified = true;

            axisType.MotorRatedPeakCurrent = parameters.MotorRatedPeakCurrent;
            axisType.MotorRatedPeakCurrentSpecified = true;

            axisType.MotorRatedOutputPower = parameters.MotorRatedOutputPower;
            axisType.MotorRatedOutputPowerSpecified = true;

            axisType.MotorOverloadLimit = parameters.MotorOverloadLimit;
            axisType.MotorOverloadLimitSpecified = true;

            axisType.MotorIntegralThermalSwitch =
                parameters.MotorIntegralThermalSwitch ? BoolEnum.@true : BoolEnum.@false;
            axisType.MotorIntegralThermalSwitchSpecified = true;

            axisType.MotorMaxWindingTemperature = parameters.MotorMaxWindingTemperature;
            axisType.MotorMaxWindingTemperatureSpecified = true;

            axisType.MotorWindingToAmbientCapacitance = parameters.MotorWindingToAmbientCapacitance;
            axisType.MotorWindingToAmbientCapacitanceSpecified = true;

            axisType.MotorWindingToAmbientResistance = parameters.MotorWindingToAmbientResistance;
            axisType.MotorWindingToAmbientResistanceSpecified = true;

            axisType.PMMotorResistance = parameters.PMMotorResistance;
            axisType.PMMotorResistanceSpecified = true;

            axisType.PMMotorInductance = parameters.PMMotorInductance;
            axisType.PMMotorInductanceSpecified = true;

            axisType.RotaryMotorPoles = parameters.RotaryMotorPoles;

            axisType.RotaryMotorInertia = parameters.RotaryMotorInertia;
            axisType.RotaryMotorInertiaSpecified = true;

            axisType.RotaryMotorRatedSpeed = parameters.RotaryMotorRatedSpeed;

            axisType.RotaryMotorMaxSpeed = parameters.RotaryMotorMaxSpeed;
            axisType.RotaryMotorMaxSpeedSpecified = true;

            //PM
            axisType.PMMotorRatedTorque = parameters.PMMotorRatedTorque;
            axisType.PMMotorRatedTorqueSpecified = true;

            axisType.PMMotorTorqueConstant = parameters.PMMotorTorqueConstant;
            axisType.PMMotorTorqueConstantSpecified = true;

            axisType.PMMotorRotaryVoltageConstant = parameters.PMMotorRotaryVoltageConstant;
            axisType.PMMotorRotaryVoltageConstantSpecified = true;

            //TODO(gjc):need check later: Feedback1

            axisType.Feedback1Unit = (AxisCIPFeedbackUnitEnum)parameters.Feedback1Unit;
            axisType.Feedback1UnitSpecified = true;

            axisType.Feedback1Polarity = (AxisPolarityEnum)parameters.Feedback1Polarity;

            axisType.Feedback1StartupMethod = (AxisFeedbackStartupMethodEnum)parameters.Feedback1StartupMethod;

            axisType.ScalingSource = (AxisScalingSourceEnum)parameters.ScalingSource;
            axisType.ScalingSourceSpecified = true;

            axisType.Feedback1CycleResolution = parameters.Feedback1CycleResolution;

            axisType.Feedback1CycleInterpolation = parameters.Feedback1CycleInterpolation;

            axisType.Feedback1Turns = parameters.Feedback1Turns;

            axisType.Feedback1VelocityFilterBandwidth = parameters.Feedback1VelocityFilterBandwidth;

            axisType.Feedback1AccelFilterBandwidth = parameters.Feedback1AccelFilterBandwidth;

            axisType.PMMotorFluxSaturation = string.Join(" ", parameters.PMMotorFluxSaturation);
            axisType.PMMotorFluxSaturationSpecified = true;

            axisType.Feedback1VelocityFilterTaps = parameters.Feedback1VelocityFilterTaps;

            axisType.Feedback1AccelFilterTaps = parameters.Feedback1AccelFilterTaps;

            //////
            axisType.LoadType = (AxisLoadTypeEnum)parameters.LoadType;
            axisType.LoadTypeSpecified = true;

            axisType.ActuatorType = (AxisActuatorTypeEnum)parameters.ActuatorType;
            axisType.ActuatorTypeSpecified = true;

            axisType.TravelMode = (AxisTravelModeEnum)parameters.TravelMode;
            axisType.TravelModeSpecified = true;

            axisType.PositionScalingNumerator = parameters.PositionScalingNumerator;
            axisType.PositionScalingNumeratorSpecified = true;

            axisType.PositionScalingDenominator = parameters.PositionScalingDenominator;
            axisType.PositionScalingDenominatorSpecified = true;

            axisType.PositionUnwindNumerator = parameters.PositionUnwindNumerator;

            axisType.PositionUnwindDenominator = parameters.PositionUnwindDenominator;

            axisType.TravelRange = parameters.TravelRange;

            axisType.MotionResolution = parameters.MotionResolution;
            axisType.MotionResolutionSpecified = true;

            axisType.MotionPolarity = (AxisPolarityEnum)parameters.MotionPolarity;
            axisType.MotionPolaritySpecified = true;

            axisType.MotorTestResistance = parameters.MotorTestResistance;
            axisType.MotorTestResistanceSpecified = true;

            axisType.MotorTestInductance = parameters.MotorTestInductance;
            axisType.MotorTestInductanceSpecified = true;

            axisType.TuneFriction = parameters.TuneFriction;

            axisType.TuneLoadOffset = parameters.TuneLoadOffset;

            axisType.TotalInertia = parameters.TotalInertia;

            axisType.TuningSelect = (AxisTuningSelectEnum)parameters.TuningSelect;

            axisType.TuningDirection = (AxisTuningDirectionEnum)parameters.TuningDirection;

            axisType.ApplicationType = (AxisApplicationTypeEnum)parameters.ApplicationType;
            axisType.ApplicationTypeSpecified = true;

            axisType.LoopResponse = (AxisLoopResponseEnum)parameters.LoopResponse;
            axisType.LoopResponseSpecified = true;

            axisType.FeedbackCommutationAligned =
                (AxisFeedbackCommutationAlignedEnum)parameters.FeedbackCommutationAligned;

            axisType.FrictionCompensationSliding = parameters.FrictionCompensationSliding;
            axisType.FrictionCompensationSlidingSpecified = true;

            axisType.FrictionCompensationStatic = parameters.FrictionCompensationStatic;
            axisType.FrictionCompensationStaticSpecified = true;

            axisType.FrictionCompensationViscous = parameters.FrictionCompensationViscous;
            axisType.FrictionCompensationViscousSpecified = true;

            axisType.PositionLoopBandwidth = parameters.PositionLoopBandwidth;
            axisType.PositionLoopBandwidthSpecified = true;

            axisType.VelocityLoopBandwidth = parameters.VelocityLoopBandwidth;
            axisType.VelocityLoopBandwidthSpecified = true;

            axisType.VelocityIntegratorBandwidth = parameters.VelocityIntegratorBandwidth;
            axisType.VelocityIntegratorBandwidthSpecified = true;

            axisType.FeedbackDataLossUserLimit = parameters.FeedbackDataLossUserLimit;

            axisType.SoftTravelLimitChecking = parameters.SoftTravelLimitChecking ? BoolEnum.@true : BoolEnum.@false;
            axisType.SoftTravelLimitCheckingSpecified = true;

            axisType.LoadRatio = parameters.LoadRatio;

            axisType.TuneInertiaMass = parameters.TuneInertiaMass;

            axisType.SoftTravelLimitPositive = parameters.SoftTravelLimitPositive;
            axisType.SoftTravelLimitPositiveSpecified = true;

            axisType.SoftTravelLimitNegative = parameters.SoftTravelLimitNegative;
            axisType.SoftTravelLimitNegativeSpecified = true;

            axisType.GainTuningConfigurationBits = parameters.GainTuningConfigurationBits.ToString(DisplayStyle.Hex);

            axisType.SystemBandwidth = parameters.SystemBandwidth;

            axisType.VelocityLowPassFilterBandwidth = parameters.VelocityLowPassFilterBandwidth;
            axisType.VelocityLowPassFilterBandwidthSpecified = true;

            axisType.TransmissionRatioInput = parameters.TransmissionRatioInput;
            axisType.TransmissionRatioInputSpecified = true;

            axisType.TransmissionRatioOutput = parameters.TransmissionRatioOutput;
            axisType.TransmissionRatioOutputSpecified = true;

            axisType.ActuatorLead = parameters.ActuatorLead;
            axisType.ActuatorLeadSpecified = true;

            axisType.ActuatorLeadUnit = (AxisActuatorLeadUnitEnum)parameters.ActuatorLeadUnit;
            axisType.ActuatorLeadUnitSpecified = true;

            axisType.ActuatorDiameter = parameters.ActuatorDiameter;
            axisType.ActuatorDiameterSpecified = true;

            axisType.ActuatorDiameterUnit = (AxisActuatorDiameterUnitEnum)parameters.ActuatorDiameterUnit;
            axisType.ActuatorDiameterUnitSpecified = true;

            axisType.SystemAccelerationBase = parameters.SystemAccelerationBase;
            axisType.SystemAccelerationBaseSpecified = true;

            axisType.DriveModelTimeConstantBase = parameters.DriveModelTimeConstantBase;
            axisType.DriveModelTimeConstantBaseSpecified = true;

            axisType.DriveRatedPeakCurrent = parameters.DriveRatedPeakCurrent;
            axisType.DriveRatedPeakCurrentSpecified = true;

            axisType.HookupTestDistance = parameters.HookupTestDistance;

            axisType.HookupTestFeedbackChannel =
                (AxisHookupTestFeedbackChannelEnum)parameters.HookupTestFeedbackChannel;

            axisType.LoadCoupling = (AxisLoadCouplingEnum)parameters.LoadCoupling;

            axisType.SystemDamping = parameters.SystemDamping;

            axisType.LoadObserverConfiguration =
                (AxisLoadObserverConfigurationEnum)parameters.LoadObserverConfiguration;

            axisType.LoadObserverBandwidth = parameters.LoadObserverBandwidth;
            axisType.LoadObserverBandwidthSpecified = true;

            axisType.LoadObserverIntegratorBandwidth = parameters.LoadObserverIntegratorBandwidth;

            axisType.LoadObserverFeedbackGain = parameters.LoadObserverFeedbackGain;
            axisType.LoadObserverFeedbackGainSpecified = true;

            axisType.AxisID = parameters.AxisID;
            axisType.AxisIDSpecified = true;

            axisType.InterpolatedPositionConfiguration =
                parameters.InterpolatedPositionConfiguration.ToString(DisplayStyle.Hex);

            axisType.AxisUpdateSchedule = (AxisUpdateScheduleEnum)parameters.AxisUpdateSchedule;

            axisType.ProvingConfiguration = (BooleanEnum)parameters.ProvingConfiguration;

            axisType.TorqueProveCurrent = parameters.TorqueProveCurrent;

            axisType.BrakeTestTorque = parameters.BrakeTestTorque;

            axisType.BrakeSlipTolerance = parameters.BrakeSlipTolerance;

            axisType.ZeroSpeed = parameters.ZeroSpeed;

            axisType.ZeroSpeedTime = parameters.ZeroSpeedTime;

            axisType.AdaptiveTuningConfiguration =
                (AxisAdaptiveTuningConfigurationEnum)parameters.AdaptiveTuningConfiguration;

            axisType.TorqueNotchFilterHighFrequencyLimit = parameters.TorqueNotchFilterHighFrequencyLimit;

            axisType.TorqueNotchFilterLowFrequencyLimit = parameters.TorqueNotchFilterLowFrequencyLimit;

            axisType.TorqueNotchFilterTuningThreshold = parameters.TorqueNotchFilterTuningThreshold;

            axisType.CoastingTimeLimit = parameters.CoastingTimeLimit;

            axisType.BusOvervoltageOperationalLimit = parameters.BusOvervoltageOperationalLimit;

            //TODO(gjc): need check later
            var motionDrive = axisCIPDrive.AssociatedModule as CIPMotionDrive;

            if ((MotorType)parameters.MotorType == MotorType.NotSpecified)
            {
                axisType.CurrentLoopBandwidthScalingFactor = 0.0f;
            }
            else
            {
                if (motionDrive != null)
                {
                    switch ((MotorType)parameters.MotorType)
                    {

                        case MotorType.RotaryPermanentMagnet:
                            axisType.CurrentLoopBandwidthScalingFactor =
                                motionDrive.Profiles.Schema.Attributes.CurrentLoopBandwidthScalingFactor
                                    .RotaryPermanentMagnet;
                            break;
                        case MotorType.RotaryInduction:
                            axisType.CurrentLoopBandwidthScalingFactor =
                                motionDrive.Profiles.Schema.Attributes.CurrentLoopBandwidthScalingFactor
                                    .RotaryInduction;
                            break;
                        case MotorType.LinearPermanentMagnet:
                            axisType.CurrentLoopBandwidthScalingFactor =
                                motionDrive.Profiles.Schema.Attributes.CurrentLoopBandwidthScalingFactor
                                    .LinearPermanentMagnet;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(parameters.MotorType));
                    }
                }
            }


            axisType.CurrentLoopBandwidth = 1000;


            if (motionDrive != null)
            {
                axisType.DriveRatedVoltage = motionDrive.ConfigData.ConverterACInputVoltage;
            }

            axisType.MaxOutputFrequency = axisCIPDrive.CIPAxis.DriveMaxOutputFrequency;

            //axisType.MotorTestDataValid = parameters.MotorTestDataValid;

            //axisType.HookupTestSpeed = parameters.HookupTestSpeed;


            axisType.CIPAxisExceptionAction =
                ToExceptionAction(parameters.MotionModule, parameters.CIPAxisExceptionAction);
            axisType.CIPAxisExceptionActionSpecified = true;

            //TODO(gjc): need add later
            //axisType.CIPAxisExceptionActionRA = ToExceptionAction(parameters.CIPAxisExceptionActionMfg);

            axisType.MotionExceptionAction =
                ToMotionExceptionAction(parameters.MotionModule, parameters.MotionExceptionAction);
            axisType.MotionExceptionActionSpecified = true;

            if (axisCIPDrive.CyclicReadUpdateList != null && axisCIPDrive.CyclicReadUpdateList.Count > 0)
            {
                axisType.CyclicReadUpdateList = string.Join(" ", axisCIPDrive.CyclicReadUpdateList);
                axisType.CyclicReadUpdateListSpecified = true;
            }

            if (axisCIPDrive.CyclicWriteUpdateList != null && axisCIPDrive.CyclicWriteUpdateList.Count > 0)
            {
                axisType.CyclicWriteUpdateList = string.Join(" ", axisCIPDrive.CyclicWriteUpdateList);
                axisType.CyclicWriteUpdateListSpecified = true;
            }

            axisType.CommutationOffset = parameters.CommutationOffset;

            axisType.CommutationPolarity = (AxisPolarityEnum)parameters.CommutationPolarity;

            return axisType;
        }

        private static AxisType GetAxisType(AxisVirtual axisVirtual)
        {
            AxisType axisType = new AxisType();

            var parameters = axisVirtual.Parameters;

            axisType.MotionGroup = axisVirtual.AssignedGroup?.Name;

            axisType.ConversionConstant = parameters.ConversionConstant;
            axisType.ConversionConstantSpecified = true;

            axisType.OutputCamExecutionTargets = (ulong)parameters.OutputCamExecutionTargets;
            axisType.OutputCamExecutionTargetsSpecified = true;

            axisType.PositionUnits = parameters.PositionUnits;

            axisType.AverageVelocityTimebase = parameters.AverageVelocityTimebase;
            axisType.AverageVelocityTimebaseSpecified = true;

            axisType.RotaryAxis = (RotaryAxisEnum)parameters.RotaryAxis;
            axisType.RotaryAxisSpecified = true;

            axisType.PositionUnwind = (ulong)parameters.PositionUnwind;
            
            axisType.HomeMode = (AxisHomeModeEnum)parameters.HomeMode;
            axisType.HomeModeSpecified = true;

            axisType.HomeDirection = (AxisHomeDirEnum)parameters.HomeDirection;
            axisType.HomeDirectionSpecified = true;

            axisType.HomeSequence = (AxisHomeSeqEnum)parameters.HomeSequence;
            axisType.HomeSequenceSpecified = true;

            axisType.HomeConfigurationBits = parameters.HomeConfigurationBits.ToString(DisplayStyle.Hex);

            axisType.HomePosition = parameters.HomePosition;
            axisType.HomePositionSpecified = true;

            axisType.HomeOffset = parameters.HomeOffset;
            axisType.HomeOffsetSpecified = true;

            axisType.MaximumSpeed = parameters.MaximumSpeed;
            axisType.MaximumSpeedSpecified = true;

            axisType.MaximumAcceleration = parameters.MaximumAcceleration;
            axisType.MaximumAccelerationSpecified = true;

            axisType.MaximumDeceleration = parameters.MaximumDeceleration;
            axisType.MaximumDecelerationSpecified = true;

            axisType.ProgrammedStopMode = (AxisProgStopModeEnum)parameters.ProgrammedStopMode;
            axisType.ProgrammedStopModeSpecified = true;

            axisType.MasterInputConfigurationBits = parameters.MasterInputConfigurationBits;
            axisType.MasterInputConfigurationBitsSpecified = true;

            axisType.MasterPositionFilterBandwidth = parameters.MasterPositionFilterBandwidth;
            axisType.MasterPositionFilterBandwidthSpecified = true;

            axisType.MaximumAccelerationJerk = parameters.MaximumAccelerationJerk;
            axisType.MaximumAccelerationJerkSpecified = true;

            axisType.MaximumDecelerationJerk = parameters.MaximumDecelerationJerk;
            axisType.MaximumDecelerationJerkSpecified = true;

            axisType.DynamicsConfigurationBits = parameters.DynamicsConfigurationBits;
            axisType.DynamicsConfigurationBitsSpecified = true;

            axisType.InterpolatedPositionConfiguration =
                parameters.InterpolatedPositionConfiguration.ToString(DisplayStyle.Hex);

            axisType.AxisUpdateSchedule = (AxisUpdateScheduleEnum)parameters.AxisUpdateSchedule;

            return axisType;
        }

        private static string ToExceptionAction(string motionModule, List<byte> bytes)
        {
            List<AxisExceptionActionEnum> motionExceptionActions = new List<AxisExceptionActionEnum>();

            if (string.IsNullOrEmpty(motionModule) || string.Equals(motionModule, "<NA>"))
            {
                for (int i = 0; i < bytes.Count; i++)
                {
                    motionExceptionActions.Add(AxisExceptionActionEnum.Unsupported);
                }
            }
            else
            {
                foreach (byte value in bytes)
                {
                    motionExceptionActions.Add((AxisExceptionActionEnum)value);
                }
            }

            string result = string.Join(" ", motionExceptionActions);

            return result;
        }

        private static string ToMotionExceptionAction(string motionModule, List<byte> bytes)
        {
            List<AxisExceptionActionEnum> motionExceptionActions = new List<AxisExceptionActionEnum>();

            if (string.IsNullOrEmpty(motionModule) || string.Equals(motionModule, "<NA>"))
            {
                for (int i = 0; i < bytes.Count; i++)
                {
                    motionExceptionActions.Add(AxisExceptionActionEnum.Unsupported);
                }

                motionExceptionActions[1] = AxisExceptionActionEnum.Disable;
                motionExceptionActions[2] = AxisExceptionActionEnum.Disable;
            }
            else
            {
                foreach (byte value in bytes)
                {
                    motionExceptionActions.Add((AxisExceptionActionEnum)value);
                }
            }

            string result = string.Join(" ", motionExceptionActions);

            return result;
        }

        private static Data ToMotionGroupData(Tag tag)
        {
            if (!tag.DataTypeInfo.DataType.IsMotionGroupType)
                return null;

            Data motionGroupData = new Data
            {
                Format = "MotionGroup"
            };

            MotionGroupType motionGroupType = GetMotionGroupType(tag);

            motionGroupData.Items = new object[] { motionGroupType };

            return motionGroupData;
        }

        private static MotionGroupType GetMotionGroupType(Tag tag)
        {
            MotionGroupType motionGroupType = new MotionGroupType();

            MotionGroup motionGroup = tag.DataWrapper as MotionGroup;
            Contract.Assert(motionGroup != null);

            motionGroupType.CoarseUpdatePeriod = (ulong)motionGroup.CoarseUpdatePeriod;
            motionGroupType.CoarseUpdatePeriodSpecified = true;

            motionGroupType.PhaseShift = (ulong)motionGroup.PhaseShift;
            motionGroupType.PhaseShiftSpecified = true;

            motionGroupType.GeneralFaultType = ToMGGeneralFaultEnum(motionGroup.GeneralFaultType);
            motionGroupType.GeneralFaultTypeSpecified = true;

            motionGroupType.AutoTagUpdate = motionGroup.AutoTagUpdate ? EnabledEnum.Enabled : EnabledEnum.Disabled;

            motionGroupType.Alternate1UpdateMultiplier = motionGroup.Alternate1UpdateMultiplier;
            motionGroupType.Alternate2UpdateMultiplier = motionGroup.Alternate2UpdateMultiplier;

            return motionGroupType;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static MGGeneralFaultEnum ToMGGeneralFaultEnum(GeneralFaultType generalFaultType)
        {
            switch (generalFaultType)
            {
                case GeneralFaultType.NonMajorFault:
                    return MGGeneralFaultEnum.NonMajorFault;
                case GeneralFaultType.MajorFault:
                    return MGGeneralFaultEnum.MajorFault;
                default:
                    throw new ArgumentOutOfRangeException(nameof(generalFaultType), generalFaultType, null);
            }
        }

        private static Data ToDecoratedData(Tag tag)
        {
            if (tag.Usage == Usage.InOut)
                return null;

            if (tag.DataTypeInfo.DataType.IsAtomic)
                return null;

            if (tag.DataTypeInfo.DataType.IsMotionGroupType)
                return null;

            if (tag.DataTypeInfo.DataType.IsAxisType)
                return null;

            if (tag.DataTypeInfo.DataType.IsMessageType)
                return null;

            if (tag.DataTypeInfo.DataType.IsStringType && tag.DataTypeInfo.Dim1 == 0)
                return null;

            Data decoratedData = new Data()
            {
                Format = "Decorated"
            };

            List<object> items = new List<object>();

            DataArray dataArray = ToDataArray(tag);
            if (dataArray != null)
                items.Add(dataArray);

            if (tag.DataTypeInfo.Dim1 == 0)
            {
                DataStructure dataStructure = ToDataStructure(null, tag.DataTypeInfo.DataType, tag.DataWrapper.Data);
                if (dataStructure != null)
                    items.Add(dataStructure);
            }

            if (items.Count > 0)
                decoratedData.Items = items.ToArray();

            return decoratedData;
        }

        public static DataStructure ToDataStructure(string memberName, IDataType dataType, IField dataField)
        {
            if (dataType.IsAtomic)
                return null;

            if (dataType.IsStringType)
            {
                return ToStringDataStructure(memberName, dataType, dataField);
            }

            DataStructure dataStructure = new DataStructure();

            dataStructure.Name = memberName;
            dataStructure.DataType = dataType.Name;

            var compositeField = dataField as ICompositeField;
            var compositiveType = dataType as CompositiveType;

            Contract.Assert(compositeField != null);
            Contract.Assert(compositiveType != null);

            List<object> dataValueMembers = new List<object>();
            foreach (var member in compositiveType.TypeMembers)
            {
                if (member.IsHidden)
                    continue;

                var memberDataTypeInfo = member.DataTypeInfo;

                var dataTypeMember = member as DataTypeMember;
                Contract.Assert(dataTypeMember != null);

                IField memberDataField = compositeField.fields[dataTypeMember.FieldIndex].Item1;

                if (memberDataTypeInfo.Dim1 == 0)
                {
                    if (dataTypeMember.IsBit && memberDataTypeInfo.Dim1 == 0)
                    {
                        DataValue dataValueMember = new DataValue
                        {
                            Name = dataTypeMember.Name,
                            DataType = memberDataTypeInfo.DataType.IsBool ? "BOOL" : memberDataTypeInfo.DataType.Name,
                            Value = memberDataField.GetBitValue(dataTypeMember.BitOffset) ? "1" : "0"
                        };

                        dataValueMembers.Add(dataValueMember);
                    }
                    else if (memberDataTypeInfo.DataType.IsAtomic)
                    {
                        DataValue dataValueMember = new DataValue
                        {
                            Name = dataTypeMember.Name,
                            DataType = memberDataTypeInfo.DataType.IsBool ? "BOOL" : memberDataTypeInfo.DataType.Name,
                            Value = memberDataField.GetString(dataTypeMember.DisplayStyle),
                            Radix = ToRadix(dataTypeMember.DisplayStyle),
                            RadixSpecified = true
                        };

                        dataValueMembers.Add(dataValueMember);
                    }
                    else
                    {
                        DataStructure memberDataStructure =
                            ToDataStructure(member.Name, memberDataTypeInfo.DataType, memberDataField);

                        dataValueMembers.Add(memberDataStructure);
                    }
                }
                else
                {
                    BoolArrayField boolArrayField = memberDataField as BoolArrayField;
                    ArrayField arrayField = memberDataField as ArrayField;

                    DataArray dataArray = new DataArray();

                    dataArray.Name = dataTypeMember.Name;
                    dataArray.DataType
                        = memberDataTypeInfo.DataType.IsBool ? "BOOL" : memberDataTypeInfo.DataType.Name;
                    dataArray.Dimensions = GetDimensions(memberDataTypeInfo, ",");

                    if (memberDataTypeInfo.DataType.IsAtomic)
                    {
                        dataArray.Radix = ToRadix(dataTypeMember.DisplayStyle);
                        dataArray.RadixSpecified = true;
                    }

                    List<DataArrayElement> elements = new List<DataArrayElement>();

                    var dim3 = Math.Max(memberDataTypeInfo.Dim3, 1);
                    var dim2 = Math.Max(memberDataTypeInfo.Dim2, 1);
                    var dim1 = memberDataTypeInfo.Dim1;

                    int index = 0;
                    for (var z = 0; z < dim3; z++)
                    for (var y = 0; y < dim2; y++)
                    for (var x = 0; x < dim1; x++)
                    {
                        string indexName;
                        if (memberDataTypeInfo.Dim3 > 0)
                            indexName = $"[{z},{y},{x}]";
                        else if (memberDataTypeInfo.Dim2 > 0)
                            indexName = $"[{y},{x}]";
                        else
                            indexName = $"[{x}]";

                        DataArrayElement dataArrayElement = new DataArrayElement();

                        dataArrayElement.Index = indexName;

                        if (memberDataTypeInfo.DataType.IsAtomic)
                        {
                            if (boolArrayField != null)
                            {
                                dataArrayElement.Value = boolArrayField.Get(index) ? "1" : "0";
                            }
                            else if (arrayField != null)
                            {
                                dataArrayElement.Value =
                                    arrayField.fields[index].Item1.GetString(dataTypeMember.DisplayStyle);
                            }
                            else
                            {
                                Contract.Assert(false);
                            }

                        }
                        else
                        {
                            if (arrayField != null)
                            {
                                DataStructure memberDataStructure =
                                    ToDataStructure(null, memberDataTypeInfo.DataType, arrayField.fields[index].Item1);

                                dataArrayElement.Structure = new[] { memberDataStructure };
                            }

                        }

                        elements.Add(dataArrayElement);

                        index++;
                    }

                    dataArray.Element = elements.ToArray();

                    dataValueMembers.Add(dataArray);
                }

            }

            if (dataValueMembers.Count > 0)
                dataStructure.Items = dataValueMembers.ToArray();

            return dataStructure;
        }

        private static DataStructure ToStringDataStructure(string memberName, IDataType dataType, IField dataField)
        {
            XmlDocument xmlDocument = new XmlDocument();

            var compositeField = dataField as ICompositeField;
            Contract.Assert(compositeField != null);

            Int32Field lenField = compositeField.fields[0].Item1 as Int32Field;
            Contract.Assert(lenField != null);

            DataStructure dataStructure = new DataStructure();

            dataStructure.Name = memberName;
            dataStructure.DataType = dataType.Name;

            DataValue lenMember = new DataValue();
            lenMember.Name = "LEN";
            lenMember.DataType = "DINT";
            lenMember.Radix = RadixEnum.Decimal;
            lenMember.RadixSpecified = true;
            lenMember.Value = lenField.value.ToString();

            DataValue dataMember = new DataValue();
            dataMember.Name = "DATA";
            dataMember.DataType = dataType.Name;
            dataMember.Radix = RadixEnum.Ascii;
            dataMember.RadixSpecified = true;
            dataMember.Text = new XmlNode[]
                { xmlDocument.CreateCDataSection(compositeField.ToString(DisplayStyle.NullStyle)) };

            dataStructure.Items = new object[] { lenMember, dataMember };

            return dataStructure;
        }

        public static DataArray ToDataArray(Tag tag)
        {
            if (tag.DataTypeInfo.Dim1 == 0)
                return null;

            if (tag.DataTypeInfo.DataType.IsAtomic)
                return null;

            ArrayField arrayField = tag.DataWrapper.Data as ArrayField;
            Contract.Assert(arrayField != null);

            DataArray dataArray = new DataArray();

            dataArray.DataType = tag.DataTypeInfo.DataType.Name;
            dataArray.Dimensions = GetDimensions(tag.DataTypeInfo, ",");

            List<DataArrayElement> elements = new List<DataArrayElement>();

            var dim3 = Math.Max(tag.DataTypeInfo.Dim3, 1);
            var dim2 = Math.Max(tag.DataTypeInfo.Dim2, 1);
            var dim1 = tag.DataTypeInfo.Dim1;

            int index = 0;
            for (var z = 0; z < dim3; z++)
            for (var y = 0; y < dim2; y++)
            for (var x = 0; x < dim1; x++)
            {
                string indexName;
                if (tag.DataTypeInfo.Dim3 > 0)
                    indexName = $"[{z},{y},{x}]";
                else if (tag.DataTypeInfo.Dim2 > 0)
                    indexName = $"[{y},{x}]";
                else
                    indexName = $"[{x}]";

                DataArrayElement dataArrayElement = new DataArrayElement();

                dataArrayElement.Index = indexName;

                if (tag.DataTypeInfo.DataType.IsAtomic)
                {
                    //todo(gjc): add code here
                    throw new NotImplementedException("add code here");
                }
                else
                {
                    DataStructure dataStructure =
                        ToDataStructure(null, tag.DataTypeInfo.DataType, arrayField.fields[index].Item1);

                    dataArrayElement.Structure = new[] { dataStructure };
                }

                elements.Add(dataArrayElement);

                index++;
            }

            dataArray.Element = elements.ToArray();

            return dataArray;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static Data ToL5KData(Tag tag)
        {
            if (tag.Usage == Usage.InOut)
                return null;

            if (tag.DataTypeInfo.DataType.IsMotionGroupType)
                return null;

            if (tag.DataTypeInfo.DataType.IsAxisType)
                return null;

            if (tag.DataTypeInfo.DataType.IsMessageType)
                return null;

            string value = null;

            if (tag.DataTypeInfo.DataType.IsAtomic)
            {
                value = ToL5XDataValue(tag.DataWrapper.Data);
            }

            if (tag.DataTypeInfo.DataType is NativeType)
            {
                value = NativeTypeToL5KDataValue(tag.DataWrapper.Data, tag.DataTypeInfo.DataType as NativeType);
            }

            // udt
            UserDefinedDataType udtDataType = tag.DataTypeInfo.DataType as UserDefinedDataType;
            if (udtDataType != null)
            {
                return UDTDataTypeToL5KData(tag.DataWrapper.Data, udtDataType);
            }

            // aoi
            AOIDataType aoiDataType = tag.DataTypeInfo.DataType as AOIDataType;
            if (aoiDataType != null)
            {
                return AOIDataTypeToL5KData(tag.DataWrapper.Data, aoiDataType);
            }

            ModuleDefinedDataType moduleDefinedDataType = tag.DataTypeInfo.DataType as ModuleDefinedDataType;
            if (moduleDefinedDataType != null)
            {
                value = ToL5XDataValue(tag.DataWrapper.Data);
            }

            if (string.IsNullOrEmpty(value))
                return null;

            Data l5kData = new Data
            {
                Format = "L5K"
            };

            XmlDocument xmlDocument = new XmlDocument();

            l5kData.Text = new XmlNode[] { xmlDocument.CreateCDataSection(value) };

            return l5kData;

        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static Data UDTDataTypeToL5KData(IField field, UserDefinedDataType dataType)
        {
            string value = UDTDataTypeToL5KDataValue(field, dataType);
            if (string.IsNullOrEmpty(value))
                return null;

            Data l5kData = new Data
            {
                Format = "L5K"
            };

            XmlDocument xmlDocument = new XmlDocument();

            l5kData.Text = new XmlNode[] { xmlDocument.CreateCDataSection(value) };

            return l5kData;
        }

        private static string UDTDataTypeToL5KDataValue(IField field, UserDefinedDataType dataType)
        {
            if (field == null)
                return null;

            ArrayField arrayField = field as ArrayField;
            ICompositeField compositeField = field as ICompositeField;

            if (arrayField != null)
            {
                List<string> valueList = new List<string>();
                foreach (var tuple in arrayField.fields)
                {
                    valueList.Add(UDTDataTypeToL5KDataValueCore(tuple.Item1 as ICompositeField, dataType));
                }

                return $"[{string.Join(",", valueList)}]";
            }

            if (compositeField != null)
            {
                return UDTDataTypeToL5KDataValueCore(compositeField, dataType);
            }

            return null;
        }

        private static string UDTDataTypeToL5KDataValueCore(ICompositeField field, UserDefinedDataType dataType)
        {
            // for udt string
            if (dataType.IsStringType)
            {
                return StringToL5KDataValue(field);
            }

            Dictionary<int, IDataType> fieldToDataTypes = new Dictionary<int, IDataType>();
            foreach (var member in dataType.TypeMembers.OfType<DataTypeMember>())
            {
                if (!member.DataType.IsAtomic)
                {
                    fieldToDataTypes.Add(member.FieldIndex, member.DataType);
                }
            }

            List<string> valueList = new List<string>();

            int index = 0;
            foreach (var tuple in field.fields)
            {
                if (fieldToDataTypes.ContainsKey(index))
                {
                    IDataType memberDataType = fieldToDataTypes[index];

                    if (memberDataType is NativeType)
                    {
                        valueList.Add(NativeTypeToL5KDataValue(tuple.Item1, memberDataType as NativeType));
                    }

                    if (memberDataType is UserDefinedDataType)
                    {
                        valueList.Add(UDTDataTypeToL5KDataValue(tuple.Item1, memberDataType as UserDefinedDataType));
                    }

                    if (memberDataType is AOIDataType)
                    {
                        valueList.Add(AOIDataTypeToL5KDataValue(tuple.Item1, memberDataType as AOIDataType));
                    }
                }
                else
                {
                    valueList.Add(ToL5XDataValue(tuple.Item1));
                }

                index++;
            }

            return $"[{string.Join(",", valueList)}]";
        }

        private static string NativeTypeToL5KDataValue(IField field, NativeType dataType)
        {
            ArrayField arrayField = field as ArrayField;
            ICompositeField compositeField = field as ICompositeField;

            if (arrayField != null)
            {
                List<string> valueList = new List<string>();
                foreach (var tuple in arrayField.fields)
                {
                    valueList.Add(NativeTypeToL5KDataValueCore(tuple.Item1 as ICompositeField, dataType));
                }

                return $"[{string.Join(",", valueList)}]";
            }

            if (compositeField != null)
            {
                return NativeTypeToL5KDataValueCore(compositeField, dataType);
            }

            return null;

        }

        private static string NativeTypeToL5KDataValueCore(ICompositeField field, NativeType dataType)
        {
            if (dataType is FBD_TIMER)
            {
                return FBD_TIMERToL5KDataValueCore(field);
            }

            if (dataType is STRING)
            {
                return StringToL5KDataValueCore(field);
            }

            if (dataType is FBD_ONESHOT)
            {
                return FBD_ONESHOTToL5KDataValueCore(field);
            }

            if (dataType is MOTION_INSTRUCTION)
            {
                return MOTION_INSTRUCTIONToL5KDataValueCore(field);
            }

            if (dataType is TIMER)
            {
                return TIMERToL5KDataValueCore(field);
            }

            if (dataType is FBD_COUNTER)
            {
                return FBD_COUNTERToL5KDataValueCore(field);
            }

            if (dataType is COUNTER)
            {
                return COUNTERToL5KDataValueCore(field);
            }

            if (dataType is DATALOG_INSTRUCTION)
            {
                return DATALOG_INSTRUCTIONToL5KDataValueCore(field);
            }

            if (dataType is CAM_PROFILE)
            {
                return CAM_PROFILEToL5KDataValueCore(field);
            }

            if (dataType is CAM)
            {
                return CAMToL5KDataValueCore(field);
            }

            if (dataType is CONTROL)
            {
                return CONTROLToL5KDataValueCore(field);
            }

            if (dataType is FBD_BIT_FIELD_DISTRIBUTE)
            {
                return FBD_BIT_FIELD_DISTRIBUTEToL5KDataValueCore(field);
            }

            if (dataType is FILTER_LOW_PASS || dataType is FILTER_HIGH_PASS)
            {
                return FILTER_LOW_PASSToL5KDataValueCore(field);
            }

            if (dataType is FILTER_NOTCH)
            {
                return FILTER_NOTCHToL5KDataValueCore(field);
            }

            if (dataType is FBD_MASKED_MOVE)
            {
                return FBD_MASKED_MOVEToL5KDataValueCore(field);
            }
            throw new NotImplementedException($"add datatype: {dataType.Name}");
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static Data AOIDataTypeToL5KData(IField field, AOIDataType dataType)
        {
            string value = AOIDataTypeToL5KDataValue(field, dataType);

            if (string.IsNullOrEmpty(value))
                return null;

            Data l5kData = new Data
            {
                Format = "L5K"
            };

            XmlDocument xmlDocument = new XmlDocument();

            l5kData.Text = new XmlNode[] { xmlDocument.CreateCDataSection(value) };

            return l5kData;
        }

        private static string AOIDataTypeToL5KDataValue(IField field, AOIDataType dataType)
        {
            ArrayField arrayField = field as ArrayField;
            ICompositeField compositeField = field as ICompositeField;

            if (arrayField != null)
            {
                List<string> valueList = new List<string>();
                foreach (var tuple in arrayField.fields)
                {
                    valueList.Add(AOIDataTypeToL5KDataValueCore(tuple.Item1 as ICompositeField, dataType));
                }

                return $"[{string.Join(",", valueList)}]";
            }

            if (compositeField != null)
            {
                return AOIDataTypeToL5KDataValueCore(compositeField, dataType);
            }

            return null;
        }

        private static string AOIDataTypeToL5KDataValueCore(ICompositeField field, AOIDataType dataType)
        {
            List<string> valueList = new List<string>();

            int boolIndex = -1;
            int boolOffset = 0;
            int boolValue = 0;


            foreach (var member in dataType.TypeMembers.OfType<AoiDataTypeMember>())
            {
                // bit
                if (member.IsBit && member.Dim1 == 0)
                {
                    if (boolIndex < 0)
                    {
                        boolOffset = 0;
                        boolValue = 0;

                        valueList.Add("0");
                        boolIndex = valueList.Count - 1;
                    }

                    int bitOffset = member.BitOffset;
                    IField memberField = field.fields[member.FieldIndex].Item1;
                    bool bitValue = memberField.GetBitValue(bitOffset);

                    if (bitValue)
                    {
                        boolValue |= 1 << boolOffset;

                        valueList[boolIndex] = boolValue.ToString();
                    }

                    boolOffset++;

                    if (boolOffset == 32)
                    {
                        boolIndex = -1;
                        boolOffset = 0;
                        boolValue = 0;
                    }

                    continue;
                }

                if (member.DataType.IsAtomic)
                {
                    valueList.Add(AtomicDataTypeToL5KDataValue(field.fields[member.FieldIndex].Item1, member.DataType));

                    continue;
                }

                if (member.DataType is NativeType)
                {
                    valueList.Add(NativeTypeToL5KDataValue(field.fields[member.FieldIndex].Item1,
                        member.DataType as NativeType));

                    continue;
                }

                // aoi
                if (member.DataType is AOIDataType)
                {
                    valueList.Add(AOIDataTypeToL5KDataValue(field.fields[member.FieldIndex].Item1,
                        member.DataType as AOIDataType));
                    continue;
                }

                // udt
                if (member.DataType is UserDefinedDataType)
                {
                    valueList.Add(UDTDataTypeToL5KDataValue(field.fields[member.FieldIndex].Item1,
                        member.DataType as UserDefinedDataType));
                    continue;
                }

                throw new NotImplementedException($"Add code for datatype: {member.DataType.Name}");
            }

            return $"[{string.Join(",", valueList)}]";
        }

        private static string AtomicDataTypeToL5KDataValue(IField field, IDataType dataType)
        {
            if (!dataType.IsAtomic)
                return null;

            IFieldAtomic fieldAtomic = field as IFieldAtomic;
            if (fieldAtomic != null)
            {
                return field.GetString();
            }

            ArrayField arrayField = field as ArrayField;
            if (arrayField != null)
            {
                List<string> valueList = new List<string>();
                foreach (var tuple in arrayField.fields)
                {
                    valueList.Add(AtomicDataTypeToL5KDataValue(tuple.Item1, dataType));
                }

                return $"[{string.Join(",", valueList)}]";
            }

            BoolArrayField boolArrayField = field as BoolArrayField;
            if (boolArrayField != null)
            {
                List<string> valueList = new List<string>();

                int count = boolArrayField.getBitCount();
                for (int i = 0; i < count; i++)
                {
                    if (boolArrayField.Get(i))
                    {
                        valueList.Add("2#1");
                    }
                    else
                    {
                        valueList.Add("2#0");
                    }
                }

                return $"[{string.Join(",", valueList)}]";

            }

            return null;
        }

        private static string ToL5XDataValue(IField field)
        {
            //IFieldAtomic
            //IArrayField
            //ICompositeField
            Contract.Assert(field != null);

            IFieldAtomic fieldAtomic = field as IFieldAtomic;
            if (fieldAtomic != null)
            {
                RealField realField = field as RealField;
                if (realField != null)
                {
                    return realField.GetString(DisplayStyle.Exponential);
                }

                return field.GetString();
            }

            ArrayField arrayField = field as ArrayField;
            if (arrayField != null)
            {
                List<string> valueList = new List<string>();
                foreach (var tuple in arrayField.fields)
                {
                    valueList.Add(ToL5XDataValue(tuple.Item1));
                }

                return $"[{string.Join(",", valueList)}]";
            }

            BoolArrayField boolArrayField = field as BoolArrayField;
            if (boolArrayField != null)
            {
                List<string> valueList = new List<string>();

                int count = boolArrayField.getBitCount();
                for (int i = 0; i < count; i++)
                {
                    if (boolArrayField.Get(i))
                    {
                        valueList.Add("2#1");
                    }
                    else
                    {
                        valueList.Add("2#0");
                    }
                }

                return $"[{string.Join(",", valueList)}]";

            }

            //TODO(gjc): need check here
            ICompositeField compositeField = field as ICompositeField;
            if (compositeField != null)
            {
                List<string> valueList = new List<string>();

                foreach (var tuple in compositeField.fields)
                {
                    valueList.Add(ToL5XDataValue(tuple.Item1));
                }

                return $"[{string.Join(",", valueList)}]";
            }

            throw new NotImplementedException("check here for ToL5XDataValue!");
        }

        private static string FBD_TIMERToL5KDataValueCore(ICompositeField field)
        {
            if (field == null)
                return null;

            string[] valueList = new string[12];

            // Reset,TimerEnable,EnableIn
            sbyte enableIn = ((Int8Field)field.fields[0].Item1).value;
            sbyte timerEnable = ((Int8Field)field.fields[1].Item1).value;
            sbyte reset = ((Int8Field)field.fields[3].Item1).value;

            int value0 = ((reset & 0x1) << 2) | ((timerEnable & 0x1) << 1) | (enableIn & 0x1);
            valueList[0] = value0.ToString();

            // DN,TT,EN,EnableOut
            sbyte enableOut = ((Int8Field)field.fields[4].Item1).value;
            sbyte en = ((Int8Field)field.fields[6].Item1).value;
            sbyte tt = ((Int8Field)field.fields[7].Item1).value;
            sbyte dn = ((Int8Field)field.fields[8].Item1).value;

            int value1 = ((dn & 0x1) << 3) | ((tt & 0x1) << 2) | ((en & 0x1) << 1) | (enableOut & 0x1);
            valueList[1] = value1.ToString();

            // PRE
            int pre = ((Int32Field)field.fields[2].Item1).value;
            valueList[2] = pre.ToString();

            // ACC
            int acc = ((Int32Field)field.fields[5].Item1).value;
            valueList[3] = acc.ToString();

            // Status
            int status = ((Int32Field)field.fields[9].Item1).value;
            valueList[4] = status.ToString();

            //
            valueList[5] = "0";
            valueList[6] = "0";
            valueList[7] = "0";
            valueList[8] = "0";
            valueList[9] = "4";
            valueList[10] = "0";
            valueList[11] = "0";

            return $"[{string.Join(",", valueList)}]";
        }

        private static string StringToL5KDataValue(IField field)
        {
            ArrayField arrayField = field as ArrayField;
            ICompositeField compositeField = field as ICompositeField;

            if (arrayField != null)
            {
                List<string> valueList = new List<string>();
                foreach (var tuple in arrayField.fields)
                {
                    valueList.Add(StringToL5KDataValueCore(tuple.Item1 as ICompositeField));
                }

                return $"[{string.Join(",", valueList)}]";
            }

            if (compositeField != null)
            {
                return StringToL5KDataValueCore(compositeField);
            }

            return null;
        }

        private static string StringToL5KDataValueCore(ICompositeField field)
        {
            if (field == null)
                return null;

            if (field.fields.Count != 2)
                return null;

            int length = ((Int32Field)field.fields[0].Item1).value;

            List<string> byteList = new List<string>();
            ArrayField arrayField = (ArrayField)(field.fields[1].Item1);

            foreach (var tuple in arrayField.fields)
            {
                Int8Field int8Field = (Int8Field)tuple.Item1;
                var byteValue = (byte)int8Field.value;
                byteList.Add(byteValue.ToAsciiDisplay());
            }

            return $"[{length},'{string.Join("", byteList)}']";
        }

        private static string FBD_ONESHOTToL5KDataValueCore(ICompositeField field)
        {
            if (field == null)
                return null;

            string[] valueList = new string[3];

            //InputBit,EnableIn
            sbyte enableIn = ((Int8Field)field.fields[1].Item1).value;
            sbyte inputBit = ((Int8Field)field.fields[2].Item1).value;

            int value0 = ((inputBit & 0x1) << 1) | (enableIn & 0x1);
            valueList[0] = value0.ToString();

            //OutputBit,EnableOut
            sbyte enableOut = ((Int8Field)field.fields[3].Item1).value;
            sbyte outputBit = ((Int8Field)field.fields[4].Item1).value;

            int value1 = ((outputBit & 0x1) << 1) | (enableOut & 0x1);
            valueList[1] = value1.ToString();

            //
            valueList[2] = "5.60519386e-045";

            return $"[{string.Join(",", valueList)}]";

        }

        private static string MOTION_INSTRUCTIONToL5KDataValueCore(ICompositeField field)
        {
            if (field == null)
                return null;

            string[] valueList = new string[9];

            // FLAGS
            // ERR
            // STATUS
            // STATE
            // SEGMENT
            // EXERR
            for (int i = 0; i < 6; i++)
            {
                IFieldAtomic fieldAtomic = field.fields[i].Item1 as IFieldAtomic;
                Contract.Assert(fieldAtomic != null);

                valueList[i] = fieldAtomic.GetString();
            }

            valueList[6] = "0";
            valueList[7] = "0";
            valueList[8] = "0";

            return $"[{string.Join(",", valueList)}]";
        }

        private static string TIMERToL5KDataValueCore(ICompositeField field)
        {
            if (field == null)
                return null;

            string[] valueList = new string[3];

            //EN，TT，DN
            sbyte en = ((Int8Field)field.fields[2].Item1).value;
            sbyte tt = ((Int8Field)field.fields[3].Item1).value;
            sbyte dn = ((Int8Field)field.fields[4].Item1).value;

            int temp0 = ((en & 0x1) << 31) | ((tt & 0x1) << 30) | ((dn & 0x1) << 29);
            valueList[0] = temp0.ToString();

            // PRE
            valueList[1] = field.fields[0].Item1.GetString();

            // ACC
            valueList[2] = field.fields[1].Item1.GetString();

            return $"[{string.Join(",", valueList)}]";
        }

        private static string FBD_COUNTERToL5KDataValueCore(ICompositeField field)
        {
            if (field == null)
                return null;

            string[] valueList = new string[6];

            // Reset,CDEnable,CUEnable,EnableIn
            sbyte enableIn = ((Int8Field)field.fields[0].Item1).value;
            sbyte cuEnable = ((Int8Field)field.fields[1].Item1).value;
            sbyte cdEnable = ((Int8Field)field.fields[2].Item1).value;
            sbyte reset = ((Int8Field)field.fields[4].Item1).value;

            int value0 = ((reset & 0x1) << 3) | ((cdEnable & 0x1) << 2) | ((cuEnable & 0x1) << 1) | (enableIn & 0x1);
            valueList[0] = value0.ToString();

            // PRE
            int pre = ((Int32Field)field.fields[3].Item1).value;
            valueList[1] = pre.ToString();

            // UN,OV,DN,CD,CU,EnableOut
            sbyte enableOut = ((Int8Field)field.fields[5].Item1).value;
            sbyte cu = ((Int8Field)field.fields[7].Item1).value;
            sbyte cd = ((Int8Field)field.fields[8].Item1).value;
            sbyte dn = ((Int8Field)field.fields[9].Item1).value;
            sbyte ov = ((Int8Field)field.fields[10].Item1).value;
            sbyte un = ((Int8Field)field.fields[11].Item1).value;

            int value2 = ((un & 0x1) << 5) | ((ov & 0x1) << 4) | ((dn & 0x1) << 3) | ((cd & 0x1) << 2) |
                         ((cu & 0x1) << 1) | (enableOut & 0x1);
            valueList[2] = value2.ToString();

            // ACC
            int acc = ((Int32Field)field.fields[6].Item1).value;
            valueList[3] = acc.ToString();

            valueList[4] = "4";
            valueList[5] = "0";

            return $"[{string.Join(",", valueList)}]";
        }

        private static string COUNTERToL5KDataValueCore(ICompositeField field)
        {
            if (field == null)
                return null;

            string[] valueList = new string[3];

            // CU(31),CD,DN,OV,UN
            sbyte cu = ((Int8Field)field.fields[2].Item1).value;
            sbyte cd = ((Int8Field)field.fields[3].Item1).value;
            sbyte dn = ((Int8Field)field.fields[4].Item1).value;
            sbyte ov = ((Int8Field)field.fields[5].Item1).value;
            sbyte un = ((Int8Field)field.fields[6].Item1).value;

            int value0 = ((cu & 0x1) << 31) | ((cd & 0x1) << 30) | ((dn & 0x1) << 29) | ((ov & 0x1) << 28) |
                         ((un & 0x1) << 27);
            valueList[0] = value0.ToString();

            // PRE
            int pre = ((Int32Field)field.fields[0].Item1).value;
            valueList[1] = pre.ToString();

            // ACC
            int acc = ((Int32Field)field.fields[1].Item1).value;
            valueList[2] = acc.ToString();

            return $"[{string.Join(",", valueList)}]";
        }

        private static string DATALOG_INSTRUCTIONToL5KDataValueCore(ICompositeField field)
        {
            if (field == null)
                return null;

            string[] valueList = new string[3];

            // FLAGS
            int flags = ((Int32Field)field.fields[0].Item1).value;
            valueList[0] = flags.ToString();

            // ERR
            int err = ((Int32Field)field.fields[1].Item1).value;
            valueList[1] = err.ToString();

            //
            valueList[2] = "0";

            return $"[{string.Join(",", valueList)}]";
        }

        private static string CAM_PROFILEToL5KDataValueCore(ICompositeField field)
        {
            if (field == null)
                return null;

            string[] valueList = new string[14];

            // Status
            int status = ((Int32Field)field.fields[0].Item1).value;
            valueList[0] = status.ToString();

            // X
            double x = ((LRealField)field.fields[2].Item1).value;
            var tuple = DoubleToInt(x);
            valueList[1] = tuple.Item1.ToString();
            valueList[2] = tuple.Item2.ToString();

            // Y
            double y = ((LRealField)field.fields[3].Item1).value;
            tuple = DoubleToInt(y);
            valueList[3] = tuple.Item1.ToString();
            valueList[4] = tuple.Item2.ToString();

            // SegmentType
            int type = ((Int32Field)field.fields[1].Item1).value;
            valueList[5] = type.ToString();

            // C0
            double c0 = ((LRealField)field.fields[4].Item1).value;
            tuple = DoubleToInt(c0);
            valueList[6] = tuple.Item1.ToString();
            valueList[7] = tuple.Item2.ToString();

            // C1
            double c1 = ((LRealField)field.fields[5].Item1).value;
            tuple = DoubleToInt(c1);
            valueList[8] = tuple.Item1.ToString();
            valueList[9] = tuple.Item2.ToString();

            // C2
            double c2 = ((LRealField)field.fields[6].Item1).value;
            tuple = DoubleToInt(c2);
            valueList[10] = tuple.Item1.ToString();
            valueList[11] = tuple.Item2.ToString();

            // C3
            double c3 = ((LRealField)field.fields[7].Item1).value;
            tuple = DoubleToInt(c3);
            valueList[12] = tuple.Item1.ToString();
            valueList[13] = tuple.Item2.ToString();

            return $"[{string.Join(",", valueList)}]";
        }

        private static string CAMToL5KDataValueCore(ICompositeField field)
        {
            if (field == null)
                return null;

            string[] valueList = new string[3];

            // x
            float x = ((RealField)field.fields[0].Item1).value;
            valueList[0] = x.ToString(CultureInfo.InvariantCulture);

            // y
            float y = ((RealField)field.fields[1].Item1).value;
            valueList[1] = x.ToString(CultureInfo.InvariantCulture);

            // type
            int type = ((Int32Field)field.fields[2].Item1).value;
            valueList[2] = type.ToString();

            return $"[{string.Join(",", valueList)}]";
        }

        private static string CONTROLToL5KDataValueCore(ICompositeField field)
        {
            if (field == null)
                return null;

            string[] valueList = new string[3];

            // EN,EU,DN,EM,ER,UL,IN,FD
            sbyte en = ((Int8Field)field.fields[2].Item1).value;
            sbyte eu = ((Int8Field)field.fields[3].Item1).value;
            sbyte dn = ((Int8Field)field.fields[4].Item1).value;
            sbyte em = ((Int8Field)field.fields[5].Item1).value;
            sbyte er = ((Int8Field)field.fields[6].Item1).value;
            sbyte ul = ((Int8Field)field.fields[7].Item1).value;
            sbyte @in = ((Int8Field)field.fields[8].Item1).value;
            sbyte fd = ((Int8Field)field.fields[9].Item1).value;

            int value0 = ((en & 0x1) << 31) |
                         ((eu & 0x1) << 30) |
                         ((dn & 0x1) << 29) |
                         ((em & 0x1) << 28) |
                         ((er & 0x1) << 27) |
                         ((ul & 0x1) << 26) |
                         ((@in & 0x1) << 25) |
                         ((fd & 0x1) << 24);
            valueList[0] = value0.ToString();

            // LEN
            int len = ((Int32Field)field.fields[0].Item1).value;
            valueList[1] = len.ToString();

            // POS
            int pos = ((Int32Field)field.fields[1].Item1).value;
            valueList[2] = pos.ToString();

            return $"[{string.Join(",", valueList)}]";
        }

        private static string FBD_BIT_FIELD_DISTRIBUTEToL5KDataValueCore(ICompositeField field)
        {
            if (field == null)
                return null;
            string[] valueList = new string[7];
            int value0 = (((Int8Field) field.fields[0].Item1).value & 0x1 )|
                         (((Int8Field) field.fields[6].Item1).value & 0x1 << 1);
            valueList[0] = value0.ToString();
            int value1= ((Int32Field)field.fields[1].Item1).value;
            valueList[1] = value1.ToString();
            int value2= ((Int32Field)field.fields[2].Item1).value;
            valueList[2] = value2.ToString();
            int value3= ((Int32Field)field.fields[3].Item1).value;
            valueList[3] = value3.ToString();
            int value4= ((Int32Field)field.fields[4].Item1).value;
            valueList[4] = value4.ToString();
            int value5= ((Int32Field)field.fields[5].Item1).value;
            valueList[5] = value5.ToString();
            int value6= ((Int32Field)field.fields[7].Item1).value;
            valueList[6] = value6.ToString();
            return $"[{string.Join(",", valueList)}]";
        }

        private static string FILTER_LOW_PASSToL5KDataValueCore(ICompositeField field)
        {
            if (field == null)
                return null;
            string[] valueList=new string[39];
            int value0 = ((((Int8Field) field.fields[0].Item1).value & 0x1) |
                          (((Int8Field) field.fields[2].Item1).value & 0x01) << 1);
            valueList[0] = value0.ToString();

            float value1 = ((RealField) field.fields[1].Item1).value;
            valueList[1] = value1.ToString(CultureInfo.InvariantCulture);

            float value2 = ((RealField)field.fields[3].Item1).value;
            valueList[2] = value2.ToString(CultureInfo.InvariantCulture);

            int value3 = ((Int32Field) field.fields[4].Item1).value;
            valueList[3] = value3.ToString();

            int value4 = ((Int32Field) field.fields[5].Item1).value;
            valueList[4] = value4.ToString();

            float value5 = ((RealField)field.fields[6].Item1).value;
            valueList[5] = value5.ToString(CultureInfo.InvariantCulture);

            int value6 = ((Int32Field)field.fields[7].Item1).value;
            valueList[6] = value6.ToString();

            int value7 = ((Int32Field)field.fields[8].Item1).value;
            valueList[7] = value7.ToString();

            int value8 = (((Int8Field) field.fields[9].Item1).value & 0x1);
            valueList[8] = value8.ToString();

            float value9 = ((RealField)field.fields[10].Item1).value;
            valueList[9] = value9.ToString(CultureInfo.InvariantCulture);

            float value10 = ((RealField)field.fields[11].Item1).value;
            valueList[10] = value10.ToString(CultureInfo.InvariantCulture);

            int value11 = ((Int32Field)field.fields[12].Item1).value;
            valueList[11] = value11.ToString();

            valueList[12] = "5.60519386e-045";

            for (int i = 13; i < valueList.Length; i++)
            {
                valueList[i] = "0.00000000e+000";
            }

            return $"[{string.Join(",", valueList)}]";
        }

        private static string FBD_MASKED_MOVEToL5KDataValueCore(ICompositeField field)
        {
            if (field == null)
                return null;
            string[] valueList = new string[5];
            var value0= ((((Int8Field)field.fields[0].Item1).value & 0x1) |
                         (((Int8Field)field.fields[4].Item1).value & 0x1) << 1);
            valueList[0] = value0.ToString();

            var value1 = ((Int32Field) field.fields[1].Item1).value;
            valueList[1] = value1.ToString();

            var value2 = ((Int32Field)field.fields[2].Item1).value;
            valueList[2] = value2.ToString();

            var value3 = ((Int32Field)field.fields[3].Item1).value;
            valueList[3] = value3.ToString();

            var value5 = ((Int32Field)field.fields[5].Item1).value;
            valueList[4] = value5.ToString();
            return $"[{string.Join(",", valueList)}]";
        }

        private static string FILTER_NOTCHToL5KDataValueCore(ICompositeField field)
        {
            if (field == null)
                return null;

            string[] valueList = new string[46];

            //EnableIn, Initialize
            int value0 = ((((Int8Field)field.fields[0].Item1).value & 0x1) |
                                       (((Int8Field)field.fields[2].Item1).value & 0x01) << 1);
            valueList[0] = value0.ToString();

            //In
            valueList[1] = ((RealField)field.fields[1].Item1).value.ToString(CultureInfo.InvariantCulture);

            //WNotch
            valueList[2] = ((RealField)field.fields[3].Item1).value.ToString(CultureInfo.InvariantCulture);

            //QFactor
            valueList[3] = ((RealField)field.fields[4].Item1).value.ToString(CultureInfo.InvariantCulture);

            //Order
            valueList[4] = ((Int32Field)field.fields[5].Item1).value.ToString();

            //TimingMode
            valueList[5] = ((Int32Field)field.fields[6].Item1).value.ToString();

            //OversampleDT
            valueList[6] = ((RealField)field.fields[7].Item1).value.ToString(CultureInfo.InvariantCulture);

            //RTSTime
            valueList[7] = ((Int32Field)field.fields[8].Item1).value.ToString();

            //RTSTimeStamp
            valueList[8] = ((Int32Field)field.fields[9].Item1).value.ToString();

            // EnableOut
            valueList[9] = (((Int8Field)field.fields[10].Item1).value & 0x1).ToString();

            //Out
            valueList[10] = ((RealField)field.fields[11].Item1).value.ToString(CultureInfo.InvariantCulture);

            //DeltaT
            valueList[11] = ((RealField)field.fields[12].Item1).value.ToString(CultureInfo.InvariantCulture);

            //Status
            valueList[12] = ((Int32Field)field.fields[13].Item1).value.ToString();

            valueList[13] = "5.60519386e-045";

            for (int i = 14; i < valueList.Length; i++)
            {
                valueList[i] = "0.00000000e+000";
            }

            return $"[{string.Join(",", valueList)}]";
        }

        private static Tuple<int, int> DoubleToInt(double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            int int0 = BitConverter.ToInt32(bytes, 0);
            int int1 = BitConverter.ToInt32(bytes, 4);

            return new Tuple<int, int>(int1, int0);
        }
    }
}
