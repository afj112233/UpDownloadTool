using System;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Utils;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties
{
    class AxisDefaultSetting
    {
        public static void LoadDefaultIntegratorHold(CIPAxis axis, ApplicationType applicationType)
        {
            var hold = applicationType == ApplicationType.PointToPoint;

            var bits = Convert.ToByte(axis.PositionIntegratorControl);
            FlagsEnumHelper.SelectFlag(IntegratorControlBitmap.IntegratorHoldEnable, hold,
                ref bits);
            axis.PositionIntegratorControl = bits;

            bits = Convert.ToByte(axis.VelocityIntegratorControl);
            FlagsEnumHelper.SelectFlag(IntegratorControlBitmap.IntegratorHoldEnable, hold,
                ref bits);
            axis.VelocityIntegratorControl = bits;

            // VelocityFeedforwardGain
            if (applicationType == ApplicationType.PointToPoint)
            {
                axis.VelocityFeedforwardGain = 0;
            }
        }

        public static void LoadDefaultDamping(CIPAxis axis, LoopResponseType value)
        {
            float damping;

            switch (value)
            {
                case LoopResponseType.Low:
                    damping = 1.5f;
                    break;
                case LoopResponseType.Medium:
                    damping = 1.0f;
                    break;
                case LoopResponseType.High:
                    damping = 0.8f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }

            axis.DampingFactor = damping;
            axis.SystemDamping = damping;
        }

        public static void ResetMotorProperty(CIPAxis axis)
        {
            axis.Feedback1Type = (byte) FeedbackType.NotSpecified;

            axis.MotorCatalogNumber = "<none>";
            axis.MotorDeviceCode = 0;
            axis.MotorType = (byte) MotorType.NotSpecified;
            axis.MotorUnit = (byte) MotorUnitType.Rev;

            // Load
            var bits = Convert.ToUInt16(axis.GainTuningConfigurationBits);
            FlagsEnumHelper.SelectFlag(GainTuningConfigurationType.UseLoadRatio, false, ref bits);
            axis.GainTuningConfigurationBits = bits;

            axis.LoadRatio = 0f;
            axis.SystemInertia = 0f;
            axis.TorqueOffset = 0f;

            axis.RotaryMotorInertia = 0f;
            axis.TotalInertia = 0f;

            axis.LinearMotorMass = 0f;
            axis.TotalMass = 0f;

        }

        public static void LoadDefaultMotorValues(CIPAxis axis, MotorType value)
        {
            axis.MotorUnit = (byte) MotorUnitType.Rev;
            switch (value)
            {
                case MotorType.NotSpecified:
                    break;
                case MotorType.RotaryPermanentMagnet:
                    LoadDefaultRotaryPMMotorValues(axis);
                    break;
                case MotorType.RotaryInduction:
                    LoadDefaultRotaryInductionMotorValues(axis);
                    break;
                case MotorType.LinearPermanentMagnet:
                    LoadDefaultLinearPMMotorValues(axis);
                    break;
                case MotorType.LinearInduction:
                    break;
                case MotorType.RotaryInteriorPermanentMagnet:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }

        }


        #region Motor

        private static void LoadDefaultRotaryPMMotorValues(CIPAxis axis)
        {
            axis.MotorUnit = (byte) MotorUnitType.Rev;

            axis.MotorIntegralThermalSwitch = 0;
            axis.MotorWindingToAmbientCapacitance = 0f;
            axis.MotorWindingToAmbientResistance = 0f;

            axis.MotorRatedOutputPower = 0.0f;
            axis.MotorRatedVoltage = 0.0f;
            axis.RotaryMotorRatedSpeed = 0.0f;
            axis.MotorRatedContinuousCurrent = 0.0f;
            axis.PMMotorRatedTorque = 0.0f;

            axis.RotaryMotorPoles = 8;
            axis.RotaryMotorMaxSpeed = 0.0f;
            axis.MotorRatedPeakCurrent = 0.0f;
            axis.MotorOverloadLimit = 100.0f;

            axis.PMMotorTorqueConstant = 0.0f;
            axis.PMMotorRotaryVoltageConstant = 0.0f;
            axis.PMMotorResistance = 0.0f;
            axis.PMMotorInductance = 0.0f;

            for (var i = 0; i < 8; i++)
                axis.PMMotorFluxSaturation.SetValue(i, 100);
        }

        private static void LoadDefaultLinearPMMotorValues(CIPAxis axis)
        {
            axis.MotorUnit = (byte) MotorUnitType.Meter;

            axis.MotorIntegralThermalSwitch = 0;
            axis.MotorWindingToAmbientCapacitance = 0f;
            axis.MotorWindingToAmbientResistance = 0f;

            axis.MotorRatedOutputPower = 0.0f;
            axis.MotorRatedVoltage = 0.0f;
            axis.LinearMotorRatedSpeed = 0.0f;
            axis.MotorRatedContinuousCurrent = 0.0f;
            axis.PMMotorRatedForce = 0.0f;

            axis.LinearMotorPolePitch = 50.0f;
            axis.LinearMotorMaxSpeed = 0.0f;
            axis.MotorRatedPeakCurrent = 0.0f;
            axis.MotorOverloadLimit = 100.0f;

            // TODO(gjc): add code here
        }

        private static void LoadDefaultRotaryInductionMotorValues(CIPAxis axis)
        {
            axis.MotorUnit = (byte) MotorUnitType.Rev;

            axis.MotorRatedOutputPower = 0.0f;
            axis.MotorRatedVoltage = 0.0f;
            axis.RotaryMotorRatedSpeed = 0.0f;
            axis.MotorRatedContinuousCurrent = 0.0f;

            axis.RotaryMotorPoles = 4;
            axis.InductionMotorRatedFrequency = 60.0f;
            axis.RotaryMotorMaxSpeed = 0.0f;
            axis.MotorOverloadLimit = 100.0f;

            // TODO(gjc): add code here
        }

        #endregion

        #region Feedback

        private class FeedbackSetting
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public uint CycleResolution { get; set; }
            public uint CycleInterpolation { get; set; }
            public FeedbackStartupMethodType StartupMethod { get; set; }

            public uint Turns { get; set; }
            public float Length { get; set; }

            public float VelocityFilterBandwidth { get; set; }

            public ushort AccelFilterTaps { get; set; } = 1;

            public ushort VelocityFilterTaps { get; set; } = 1;
            // ReSharper restore UnusedAutoPropertyAccessor.Local
        }


        public static void LoadDefaultFeedback1Setting(
            CIPAxis axis,
            FeedbackType feedbackType,
            FeedbackUnitType feedbackUnit)
        {
            var feedbackSetting = LoadDefaultFeedbackSetting(feedbackType, feedbackUnit);
            if (feedbackSetting != null)
            {
                axis.Feedback1CycleResolution = feedbackSetting.CycleResolution;
                axis.Feedback1CycleInterpolation = feedbackSetting.CycleInterpolation;
                axis.Feedback1StartupMethod = (byte)feedbackSetting.StartupMethod;
                axis.Feedback1Turns = feedbackSetting.Turns;
                axis.Feedback1VelocityFilterBandwidth = feedbackSetting.VelocityFilterBandwidth;
                axis.Feedback1VelocityFilterTaps = feedbackSetting.VelocityFilterTaps;
                axis.Feedback1AccelFilterTaps = feedbackSetting.AccelFilterTaps;
            }
        }

        public static void LoadDefaultFeedback2Setting(
            CIPAxis axis,
            FeedbackType feedbackType,
            FeedbackUnitType feedbackUnit)
        {
            var feedbackSetting = LoadDefaultFeedbackSetting(feedbackType, feedbackUnit);

            if (feedbackSetting != null)
            {
                axis.Feedback2CycleResolution = feedbackSetting.CycleResolution;
                axis.Feedback2CycleInterpolation = feedbackSetting.CycleInterpolation;
                axis.Feedback2StartupMethod = (byte)feedbackSetting.StartupMethod;
                axis.Feedback2Turns = feedbackSetting.Turns;
                axis.Feedback2VelocityFilterBandwidth = feedbackSetting.VelocityFilterBandwidth;
                axis.Feedback2VelocityFilterTaps = feedbackSetting.VelocityFilterTaps;
                axis.Feedback2AccelFilterTaps = feedbackSetting.AccelFilterTaps;
            }
        }

        private static FeedbackSetting LoadDefaultFeedbackSetting(
            FeedbackType feedbackType,
            FeedbackUnitType feedbackUnit)
        {
            switch (feedbackType)
            {
                case FeedbackType.NotSpecified:
                    break;
                case FeedbackType.DigitalAqB:
                    return DigitalAqBSetting(feedbackUnit);
                case FeedbackType.DigitalAqBWithUvw:
                    return DigitalAqBWithUvwSetting(feedbackUnit);
                case FeedbackType.DigitalParallel:
                    Console.WriteLine("add DigitalParallel setting");
                    break;
                case FeedbackType.SineCosine:
                    Console.WriteLine("add SineCosine setting");
                    break;
                case FeedbackType.SineCosineWithUvw:
                    return SineCosineWithUvwSetting(feedbackUnit);
                case FeedbackType.Hiperface:
                    return HiperfaceSetting(feedbackUnit);
                case FeedbackType.EnDatSineCosine:
                    return EnDatSineCosineSetting(feedbackUnit);
                case FeedbackType.EnDatDigital:
                    return EnDatDigitalSetting(feedbackUnit);
                case FeedbackType.Resolver:
                    Console.WriteLine("add Resolver setting");
                    break;
                case FeedbackType.SsiDigital:
                    return SsiDigitalSetting(feedbackUnit);
                case FeedbackType.Ldt:
                    Console.WriteLine("add Ldt setting");
                    break;
                case FeedbackType.HiperfaceDsl:
                    return HiperfaceDslSetting(feedbackUnit);
                case FeedbackType.BissDigital:
                    Console.WriteLine("add BissDigital setting");
                    break;
                case FeedbackType.Integrated:
                    Console.WriteLine("add Integrated setting");
                    break;
                case FeedbackType.SsiSineCosine:
                    Console.WriteLine("add SsiSineCosine setting");
                    break;
                case FeedbackType.SsiAqB:
                    Console.WriteLine("add SsiAqB setting");
                    break;
                case FeedbackType.BissSineCosine:
                    Console.WriteLine("add BissSineCosine setting");
                    break;
                case FeedbackType.TamagawaSerial:
                    return TamagawaSerialSetting(feedbackUnit);
                case FeedbackType.StahlSsi:
                    Console.WriteLine("add StahlSsi setting");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(feedbackType), feedbackType, null);
            }

            return null;
        }

        private static FeedbackSetting DigitalAqBSetting(FeedbackUnitType feedbackUnit)
        {
            var feedbackSetting = new FeedbackSetting
            {
                StartupMethod = FeedbackStartupMethodType.Incremental
            };

            switch (feedbackUnit)
            {
                case FeedbackUnitType.Rev:
                    feedbackSetting.CycleResolution = 1024;
                    feedbackSetting.CycleInterpolation = 4;
                    break;
                case FeedbackUnitType.Meter:
                    feedbackSetting.CycleResolution = 4096;
                    feedbackSetting.CycleInterpolation = 4;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(feedbackUnit), feedbackUnit, null);
            }

            return feedbackSetting;
        }

        private static FeedbackSetting DigitalAqBWithUvwSetting(FeedbackUnitType feedbackUnit)
        {
            var feedbackSetting = new FeedbackSetting
            {
                StartupMethod = FeedbackStartupMethodType.Incremental
            };

            switch (feedbackUnit)
            {
                case FeedbackUnitType.Rev:
                    feedbackSetting.CycleResolution = 1024;
                    feedbackSetting.CycleInterpolation = 4;
                    break;
                case FeedbackUnitType.Meter:
                    feedbackSetting.CycleResolution = 4096;
                    feedbackSetting.CycleInterpolation = 4;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(feedbackUnit), feedbackUnit, null);
            }

            return feedbackSetting;
        }

        private static FeedbackSetting SineCosineWithUvwSetting(FeedbackUnitType feedbackUnit)
        {
            var feedbackSetting = new FeedbackSetting
            {
                StartupMethod = FeedbackStartupMethodType.Incremental
            };

            switch (feedbackUnit)
            {
                case FeedbackUnitType.Rev:
                    feedbackSetting.CycleResolution = 1024;
                    feedbackSetting.CycleInterpolation = 2048;
                    break;
                case FeedbackUnitType.Meter:
                    feedbackSetting.CycleResolution = 4096;
                    feedbackSetting.CycleInterpolation = 2048;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(feedbackUnit), feedbackUnit, null);
            }

            return feedbackSetting;
        }

        private static FeedbackSetting EnDatSineCosineSetting(FeedbackUnitType feedbackUnit)
        {
            var feedbackSetting = new FeedbackSetting
            {
                StartupMethod = FeedbackStartupMethodType.Absolute
            };

            switch (feedbackUnit)
            {
                case FeedbackUnitType.Rev:
                    feedbackSetting.CycleResolution = 2048;
                    feedbackSetting.CycleInterpolation = 2048;
                    feedbackSetting.Turns = 1;
                    break;
                case FeedbackUnitType.Meter:
                    feedbackSetting.CycleResolution = 8192;
                    feedbackSetting.CycleInterpolation = 2048;
                    feedbackSetting.Length = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(feedbackUnit), feedbackUnit, null);
            }

            return feedbackSetting;
        }

        private static FeedbackSetting EnDatDigitalSetting(FeedbackUnitType feedbackUnit)
        {
            var feedbackSetting = new FeedbackSetting
            {
                StartupMethod = FeedbackStartupMethodType.Absolute
            };

            switch (feedbackUnit)
            {
                case FeedbackUnitType.Rev:
                    feedbackSetting.CycleResolution = 131072;
                    feedbackSetting.CycleInterpolation = 1;
                    feedbackSetting.Turns = 1;
                    break;
                case FeedbackUnitType.Meter:
                    feedbackSetting.CycleResolution = 655360;
                    feedbackSetting.CycleInterpolation = 1;
                    feedbackSetting.Length = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(feedbackUnit), feedbackUnit, null);
            }

            return feedbackSetting;
        }

        private static FeedbackSetting SsiDigitalSetting(FeedbackUnitType feedbackUnit)
        {
            var feedbackSetting = new FeedbackSetting
            {
                StartupMethod = FeedbackStartupMethodType.Absolute
            };

            switch (feedbackUnit)
            {
                case FeedbackUnitType.Rev:
                    feedbackSetting.CycleResolution = 524288;
                    feedbackSetting.CycleInterpolation = 4;
                    feedbackSetting.Turns = 1;
                    break;
                case FeedbackUnitType.Meter:
                    feedbackSetting.CycleResolution = 2097152;
                    feedbackSetting.CycleInterpolation = 4;
                    feedbackSetting.Length = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(feedbackUnit), feedbackUnit, null);
            }

            return feedbackSetting;
        }

        private static FeedbackSetting HiperfaceDslSetting(FeedbackUnitType feedbackUnit)
        {
            var feedbackSetting = new FeedbackSetting
            {
                StartupMethod = FeedbackStartupMethodType.Absolute
            };

            switch (feedbackUnit)
            {
                case FeedbackUnitType.Rev:
                    feedbackSetting.CycleResolution = 131072;
                    feedbackSetting.CycleInterpolation = 1;
                    feedbackSetting.Turns = 1;
                    break;
                case FeedbackUnitType.Meter:
                    feedbackSetting.CycleResolution = 655360;
                    feedbackSetting.CycleInterpolation = 1;
                    feedbackSetting.Length = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(feedbackUnit), feedbackUnit, null);
            }

            return feedbackSetting;
        }

        private static FeedbackSetting TamagawaSerialSetting(FeedbackUnitType feedbackUnit)
        {
            var feedbackSetting = new FeedbackSetting
            {
                StartupMethod = FeedbackStartupMethodType.Absolute
            };

            switch (feedbackUnit)
            {
                case FeedbackUnitType.Rev:
                    feedbackSetting.CycleResolution = 131072;
                    feedbackSetting.CycleInterpolation = 1;
                    feedbackSetting.Turns = 1;
                    break;
                case FeedbackUnitType.Meter:
                    feedbackSetting.CycleResolution = 655360;
                    feedbackSetting.CycleInterpolation = 1;
                    feedbackSetting.Length = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(feedbackUnit), feedbackUnit, null);
            }

            return feedbackSetting;
        }

        private static FeedbackSetting HiperfaceSetting(FeedbackUnitType feedbackUnit)
        {
            var feedbackSetting = new FeedbackSetting
            {
                StartupMethod = FeedbackStartupMethodType.Absolute
            };

            switch (feedbackUnit)
            {
                case FeedbackUnitType.Rev:
                    feedbackSetting.CycleResolution = 1024;
                    feedbackSetting.CycleInterpolation = 2048;
                    feedbackSetting.Turns = 1;
                    break;
                case FeedbackUnitType.Meter:
                    feedbackSetting.CycleResolution = 4096;
                    feedbackSetting.CycleInterpolation = 2048;
                    feedbackSetting.Length = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(feedbackUnit), feedbackUnit, null);
            }

            return feedbackSetting;
        }

        #endregion

        public static void UpdateMotionUnit(CIPAxis axis)
        {
            // TODO(gjc): need edit
            // motion-rm003 2018, page 380
            // Table 23 - Feedback and Load Type Unit Description
            var feedbackConfiguration = (FeedbackConfigurationType) Convert.ToByte(axis.FeedbackConfiguration);
            var loadType = (LoadType) Convert.ToByte(axis.LoadType);

            var motionUnit = MotionUnitType.MotorRev;

            switch (feedbackConfiguration)
            {
                case FeedbackConfigurationType.NoFeedback:
                    switch (loadType)
                    {
                        case LoadType.DirectRotary:
                            motionUnit = MotionUnitType.MotorRevPerS;
                            break;
                        case LoadType.DirectLinear:
                            throw new NotImplementedException("No run here!");
                        case LoadType.RotaryTransmission:
                            motionUnit = MotionUnitType.LoadRevPerS;
                            break;
                        case LoadType.LinearActuator:
                        {
                            {
                                var actuatorType = (ActuatorType)Convert.ToByte(axis.ActuatorType);
                                var actuatorDiameterUnit =
                                    (ActuatorDiameterUnitType)Convert.ToByte(axis.ActuatorDiameterUnit);
                                var actuatorLeadUnit =
                                    (ActuatorLeadUnitType)Convert.ToByte(axis.ActuatorLeadUnit);

                                if (actuatorType == ActuatorType.Screw)
                                    switch (actuatorLeadUnit)
                                    {
                                        case ActuatorLeadUnitType.MmPerRev:
                                            motionUnit = MotionUnitType.LoadMPerS;
                                            break;
                                        case ActuatorLeadUnitType.InchPerRev:
                                            motionUnit = MotionUnitType.LoadInchPerS;
                                            break;
                                        default:
                                            throw new ArgumentOutOfRangeException();
                                    }
                                else
                                    switch (actuatorDiameterUnit)
                                    {
                                        case ActuatorDiameterUnitType.Mm:
                                            motionUnit = MotionUnitType.LoadMPerS;
                                            break;
                                        case ActuatorDiameterUnitType.Inch:
                                            motionUnit = MotionUnitType.LoadInchPerS;
                                            break;
                                        default:
                                            throw new ArgumentOutOfRangeException();
                                    }
                            }
                        }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case FeedbackConfigurationType.MasterFeedback:
                    switch (loadType)
                    {
                        case LoadType.DirectRotary:
                            motionUnit = MotionUnitType.FeedbackRev;
                            break;
                        case LoadType.DirectLinear:
                            motionUnit = MotionUnitType.FeedbackMm;
                            break;
                        case LoadType.RotaryTransmission:
                            motionUnit = MotionUnitType.LoadRev;
                            break;
                        case LoadType.LinearActuator:
                        {
                            var actuatorType = (ActuatorType) Convert.ToByte(axis.ActuatorType);
                            var actuatorDiameterUnit =
                                (ActuatorDiameterUnitType) Convert.ToByte(axis.ActuatorDiameterUnit);
                            var actuatorLeadUnit =
                                (ActuatorLeadUnitType) Convert.ToByte(axis.ActuatorLeadUnit);

                            if (actuatorType == ActuatorType.Screw)
                                switch (actuatorLeadUnit)
                                {
                                    case ActuatorLeadUnitType.MmPerRev:
                                        motionUnit = MotionUnitType.LoadMm;
                                        break;
                                    case ActuatorLeadUnitType.InchPerRev:
                                        motionUnit = MotionUnitType.LoadInch;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            else
                                switch (actuatorDiameterUnit)
                                {
                                    case ActuatorDiameterUnitType.Mm:
                                        motionUnit = MotionUnitType.LoadMm;
                                        break;
                                    case ActuatorDiameterUnitType.Inch:
                                        motionUnit = MotionUnitType.LoadInch;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                        }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case FeedbackConfigurationType.MotorFeedback:
                {
                    switch (loadType)
                    {
                        case LoadType.DirectRotary:
                            motionUnit = MotionUnitType.MotorRev;
                            break;
                        case LoadType.DirectLinear:
                            motionUnit = MotionUnitType.MotorMm;
                            break;
                        case LoadType.RotaryTransmission:
                            motionUnit = MotionUnitType.LoadRev;
                            break;
                        case LoadType.LinearActuator:
                        {
                            var actuatorType = (ActuatorType) Convert.ToByte(axis.ActuatorType);
                            var actuatorDiameterUnit =
                                (ActuatorDiameterUnitType) Convert.ToByte(axis.ActuatorDiameterUnit);
                            var actuatorLeadUnit =
                                (ActuatorLeadUnitType) Convert.ToByte(axis.ActuatorLeadUnit);

                            if (actuatorType == ActuatorType.Screw)
                                switch (actuatorLeadUnit)
                                {
                                    case ActuatorLeadUnitType.MmPerRev:
                                        motionUnit = MotionUnitType.LoadMm;
                                        break;
                                    case ActuatorLeadUnitType.InchPerRev:
                                        motionUnit = MotionUnitType.LoadInch;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            else
                                switch (actuatorDiameterUnit)
                                {
                                    case ActuatorDiameterUnitType.Mm:
                                        motionUnit = MotionUnitType.LoadMm;
                                        break;
                                    case ActuatorDiameterUnitType.Inch:
                                        motionUnit = MotionUnitType.LoadInch;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                        }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                    break;
                case FeedbackConfigurationType.LoadFeedback:
                {
                    switch (loadType)
                    {
                        case LoadType.DirectRotary:
                            motionUnit = MotionUnitType.LoadRev;
                            break;
                        case LoadType.DirectLinear:
                            motionUnit = MotionUnitType.LoadMm;
                            break;
                        case LoadType.RotaryTransmission:
                            motionUnit = MotionUnitType.LoadRev;
                            break;
                        case LoadType.LinearActuator:
                        {
                            var actuatorType = (ActuatorType) Convert.ToByte(axis.ActuatorType);
                            var actuatorDiameterUnit =
                                (ActuatorDiameterUnitType) Convert.ToByte(axis.ActuatorDiameterUnit);
                            var actuatorLeadUnit =
                                (ActuatorLeadUnitType) Convert.ToByte(axis.ActuatorLeadUnit);

                            if (actuatorType == ActuatorType.Screw)
                                switch (actuatorLeadUnit)
                                {
                                    case ActuatorLeadUnitType.MmPerRev:
                                        motionUnit = MotionUnitType.LoadMm;
                                        break;
                                    case ActuatorLeadUnitType.InchPerRev:
                                        motionUnit = MotionUnitType.LoadInch;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            else
                                switch (actuatorDiameterUnit)
                                {
                                    case ActuatorDiameterUnitType.Mm:
                                        motionUnit = MotionUnitType.LoadMm;
                                        break;
                                    case ActuatorDiameterUnitType.Inch:
                                        motionUnit = MotionUnitType.LoadInch;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                        }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                    break;
                case FeedbackConfigurationType.DualFeedback:
                {
                    switch (loadType)
                    {
                        case LoadType.DirectRotary:
                            motionUnit = MotionUnitType.LoadRev;
                            break;
                        case LoadType.DirectLinear:
                            motionUnit = MotionUnitType.LoadMm;
                            break;
                        case LoadType.RotaryTransmission:
                            motionUnit = MotionUnitType.LoadRev;
                            break;
                        case LoadType.LinearActuator:
                        {
                            var actuatorType = (ActuatorType) Convert.ToByte(axis.ActuatorType);
                            var actuatorDiameterUnit =
                                (ActuatorDiameterUnitType) Convert.ToByte(axis.ActuatorDiameterUnit);
                            var actuatorLeadUnit =
                                (ActuatorLeadUnitType) Convert.ToByte(axis.ActuatorLeadUnit);

                            if (actuatorType == ActuatorType.Screw)
                                switch (actuatorLeadUnit)
                                {
                                    case ActuatorLeadUnitType.MmPerRev:
                                        motionUnit = MotionUnitType.LoadMm;
                                        break;
                                    case ActuatorLeadUnitType.InchPerRev:
                                        motionUnit = MotionUnitType.LoadInch;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            else
                                switch (actuatorDiameterUnit)
                                {
                                    case ActuatorDiameterUnitType.Mm:
                                        motionUnit = MotionUnitType.LoadMm;
                                        break;
                                    case ActuatorDiameterUnitType.Inch:
                                        motionUnit = MotionUnitType.LoadInch;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                        }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                    break;
                case FeedbackConfigurationType.DualIntFeedback:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            axis.MotionUnit = (byte) motionUnit;
        }

    }
}
