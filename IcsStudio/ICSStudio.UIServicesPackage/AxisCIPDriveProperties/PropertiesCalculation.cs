using System;
using ICSStudio.Cip.Objects;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.Utils;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties
{
    public class PropertiesCalculation
    {
        public PropertiesCalculation(AxisCIPDrive axisCIPDrive)
        {
            AxisCIPDrive = axisCIPDrive;
        }

        public AxisCIPDrive AxisCIPDrive { get; }

        public void CalculateSystemInertia()
        {
            if (AxisCIPDrive.AssociatedModule == null)
                return;

            var axisConfiguration = (AxisConfigurationType)Convert.ToByte(AxisCIPDrive.CIPAxis.AxisConfiguration);

            if (axisConfiguration == AxisConfigurationType.PositionLoop ||
                axisConfiguration == AxisConfigurationType.VelocityLoop)
            {
                var systemAcceleration = (float)CalculateSystemAcceleration();

                float systemInertia;
                if (Math.Abs(systemAcceleration) < float.Epsilon)
                    systemInertia = 0;
                else
                    systemInertia = 100 / systemAcceleration;

                AxisCIPDrive.CIPAxis.SystemInertia = systemInertia;
            }

        }

        public int CalculateScaling()
        {
            var scalingSource = (ScalingSourceType)Convert.ToByte(AxisCIPDrive.CIPAxis.ScalingSource);
            if (scalingSource == ScalingSourceType.DirectScalingFactorEntry)
                return 0;

            double numerator = Convert.ToSingle(AxisCIPDrive.CIPAxis.PositionScalingNumerator);
            double denominator = Convert.ToSingle(AxisCIPDrive.CIPAxis.PositionScalingDenominator);

            double unwindNumerator = Convert.ToSingle(AxisCIPDrive.CIPAxis.PositionUnwindNumerator);
            double unwindDenominator = Convert.ToSingle(AxisCIPDrive.CIPAxis.PositionUnwindDenominator);

            double travelRange = Convert.ToSingle(AxisCIPDrive.CIPAxis.TravelRange);

            var travelMode = (TravelModeType)Convert.ToByte(AxisCIPDrive.CIPAxis.TravelMode);

            // motion-rm003 2018,page 376
            // TODO(gjc): fix here
            var motionUnit = (MotionUnitType)Convert.ToByte(AxisCIPDrive.CIPAxis.MotionUnit);
            uint defaultMotionResolution = GetDefaultMotionResolution(motionUnit);

            //var baseResolution = EstimateBaseMotionResolution(defaultMotionResolution, travelMode, numerator,
            //    denominator, travelRange,
            //    unwindNumerator, unwindDenominator);

            ////TODO(gjc):need check float valid 

            //// MotionResolution
            //var motionResolution =
            //    (uint) Math.Round(baseResolution * numerator * unwindDenominator, MidpointRounding.AwayFromZero);
            //// ConversionConstant
            //var conversionConstant = Math.Round(baseResolution * denominator * unwindDenominator,
            //    MidpointRounding.AwayFromZero);

            //if (travelMode == TravelModeType.Cyclic)
            //{
            //    // PositionUnwind
            //    var positionUnwind =
            //        (uint) Math.Round(baseResolution * denominator * unwindNumerator, MidpointRounding.AwayFromZero);

            //    AxisCIPDrive.CIPAxis.PositionUnwind = positionUnwind;
            //}

            uint motionResolution = 0;
            float conversionConstant = 0;
            uint positionUnwind = 0;

            EstimateMotionResolution(defaultMotionResolution, travelMode, numerator,
                denominator, travelRange,
                unwindNumerator, unwindDenominator,
                ref motionResolution, ref conversionConstant, ref positionUnwind);

            if (conversionConstant > 1e12f)
                conversionConstant = 1e12f;
            if (conversionConstant < 1e-12f)
                conversionConstant = 1e-12f;

            AxisCIPDrive.CIPAxis.MotionResolution = motionResolution;
            AxisCIPDrive.CIPAxis.ConversionConstant = conversionConstant;

            if (travelMode == TravelModeType.Cyclic)
            {
                AxisCIPDrive.CIPAxis.PositionUnwind = positionUnwind;
            }

            return 0;
        }

        public void CalculateOutOfBoxTuning()
        {
            if (AxisCIPDrive.AssociatedModule == null)
                return;

            if (AxisCIPDrive.CIPAxis == null)
                return;

            UpdateDerivedProperties();
            // 
            var axisConfiguration = (AxisConfigurationType)Convert.ToByte(AxisCIPDrive.CIPAxis.AxisConfiguration);

            // Calculate DMTC
            var dmtc = RecalculateDmtc();

            double dampingFactor = Convert.ToSingle(AxisCIPDrive.CIPAxis.DampingFactor);

            double transmissionRatio = CalculateTransmissionRatio();

            // at005, page 17
            // ReSharper disable once UnusedVariable
            var torqueLoopBandwidth = 1 / (2 * Math.PI * dmtc);

            var velocityLoopBandwidth = CalculateVelocityLoopBandwidth(torqueLoopBandwidth, dampingFactor);
            var positionLoopBandwidth = CalculatePositionLoopBandwidth(velocityLoopBandwidth, dampingFactor);

            //
            var ratedSpeed = CalculateRatedSpeed();
            var positionErrorTolerance = CalculatePositionErrorTolerance(ratedSpeed, positionLoopBandwidth);

            var maxAcceleration = CalculateMaxAcceleration();
            var velocityErrorTolerance = maxAcceleration / (velocityLoopBandwidth * Math.PI);

            var torqueLowPassFilterBandwidth =
                CalculateTorqueLowPassFilterBandwidth(torqueLoopBandwidth, velocityLoopBandwidth);
            var torqueNotchFilterLowFrequencyLimit = torqueLoopBandwidth;
            // page 192, rm003
            if (torqueNotchFilterLowFrequencyLimit < 20)
                torqueNotchFilterLowFrequencyLimit = 20;
            if (torqueNotchFilterLowFrequencyLimit > 2000)
                torqueNotchFilterLowFrequencyLimit = 2000;

            var loadObserverBandwidth = CalculateLoadObserverBandwidth(torqueLoopBandwidth, velocityLoopBandwidth);

            // other,may remove 
            var positionLockTolerance = CalculatePositionLockTolerance();
            var velocityLockTolerance = CalculateVelocityLockTolerance();

            var velocityLimit = CalculateVelocityLimit();
            var torqueLimit = CalculateTorqueLimit();

            var feedback1VelocityFilterBandwidth = CalculateFeedback1VelocityFilterBandwidth();

            // Set Properties
            AxisCIPDrive.CIPAxis.PositionLoopBandwidth = (float)positionLoopBandwidth;
            AxisCIPDrive.CIPAxis.VelocityLoopBandwidth = (float)velocityLoopBandwidth;

            AxisCIPDrive.CIPAxis.PositionServoBandwidth = (float)positionLoopBandwidth;
            AxisCIPDrive.CIPAxis.VelocityServoBandwidth = (float)velocityLoopBandwidth;

            AxisCIPDrive.CIPAxis.PositionErrorTolerance = (float)(positionErrorTolerance * transmissionRatio);
            AxisCIPDrive.CIPAxis.VelocityErrorTolerance = (float)(velocityErrorTolerance * transmissionRatio);
            AxisCIPDrive.CIPAxis.AccelerationLimit = (float)(maxAcceleration * 2 * transmissionRatio);
            AxisCIPDrive.CIPAxis.DecelerationLimit = (float)(maxAcceleration * 2 * transmissionRatio);

            AxisCIPDrive.CIPAxis.TorqueLowPassFilterBandwidth = (float)torqueLowPassFilterBandwidth;
            AxisCIPDrive.CIPAxis.TorqueNotchFilterLowFrequencyLimit = (float)torqueNotchFilterLowFrequencyLimit;

            AxisCIPDrive.CIPAxis.LoadObserverBandwidth = (float)loadObserverBandwidth;

            AxisCIPDrive.CIPAxis.PositionLockTolerance = (float)(positionLockTolerance * transmissionRatio);
            AxisCIPDrive.CIPAxis.VelocityLockTolerance = (float)(velocityLockTolerance * transmissionRatio);

            AxisCIPDrive.CIPAxis.VelocityLimitPositive = (float)(velocityLimit * transmissionRatio);
            AxisCIPDrive.CIPAxis.VelocityLimitNegative = (float)(-velocityLimit * transmissionRatio);

            AxisCIPDrive.CIPAxis.VelocityStandstillWindow = (float)(ratedSpeed * 0.01 * transmissionRatio);
            AxisCIPDrive.CIPAxis.VelocityThreshold = (float)(ratedSpeed * 0.9 * transmissionRatio);

            AxisCIPDrive.CIPAxis.TorqueLimitPositive = (float)torqueLimit * 100;
            AxisCIPDrive.CIPAxis.TorqueLimitNegative = (float)(-torqueLimit * 100);
            AxisCIPDrive.CIPAxis.StoppingTorque = (float)torqueLimit * 100;
            AxisCIPDrive.CIPAxis.TorqueThreshold = (float)(torqueLimit * 90);
            AxisCIPDrive.CIPAxis.CurrentVectorLimit = (float)torqueLimit * 100;

            AxisCIPDrive.CIPAxis.MaximumSpeed = (float)(ratedSpeed * 0.85 * transmissionRatio);
            AxisCIPDrive.CIPAxis.MaximumAcceleration = (float)(maxAcceleration * 0.7 * transmissionRatio);
            AxisCIPDrive.CIPAxis.MaximumDeceleration = (float)(maxAcceleration * 0.7 * transmissionRatio);
            var jerk = JerkRateCalculation.CalculateJerk(ratedSpeed * 0.85 * transmissionRatio,
                maxAcceleration * 0.7 * transmissionRatio, 100);
            AxisCIPDrive.CIPAxis.MaximumAccelerationJerk = (float)jerk;
            AxisCIPDrive.CIPAxis.MaximumDecelerationJerk = (float)jerk;

            //TODO(gjc):need edit
            AxisCIPDrive.CIPAxis.SystemAccelerationBase =
                (float)(CalculateSystemAcceleration() * transmissionRatio);

            // SystemBandwidth
            if (axisConfiguration == AxisConfigurationType.PositionLoop)
                AxisCIPDrive.CIPAxis.SystemBandwidth = (float)positionLoopBandwidth;
            else if (axisConfiguration == AxisConfigurationType.VelocityLoop)
                AxisCIPDrive.CIPAxis.SystemBandwidth = (float)velocityLoopBandwidth;

            // rm003, page 166
            AxisCIPDrive.CIPAxis.VelocityFeedforwardGain = 100;
            AxisCIPDrive.CIPAxis.TorqueLoopBandwidth = 1000;

            // other
            AxisCIPDrive.CIPAxis.Feedback1VelocityFilterBandwidth = (float)feedback1VelocityFilterBandwidth;

            AxisCIPDrive.CIPAxis.PositionIntegratorBandwidth = 0;
            AxisCIPDrive.CIPAxis.VelocityIntegratorBandwidth = 0;
            AxisCIPDrive.CIPAxis.AccelerationFeedforwardGain = 0;
        }

        private double CalculateTransmissionRatio()
        {
            // load type and transmission
            double transmissionRatio = 1;

            var loadType = (LoadType)Convert.ToByte(AxisCIPDrive.CIPAxis.LoadType);
            if ((loadType == LoadType.RotaryTransmission) ||
                (loadType == LoadType.LinearActuator))
            {
                var transmissionRatioInput = Convert.ToUInt32(AxisCIPDrive.CIPAxis.TransmissionRatioInput);
                var transmissionRatioOutput = Convert.ToUInt32(AxisCIPDrive.CIPAxis.TransmissionRatioOutput);

                transmissionRatio = (double)transmissionRatioOutput / transmissionRatioInput;
            }

            if (loadType == LoadType.LinearActuator)
            {
                var actuatorType = (ActuatorType)Convert.ToByte(AxisCIPDrive.CIPAxis.ActuatorType);
                if (actuatorType == ActuatorType.BeltAndPulley ||
                    actuatorType == ActuatorType.ChainAndSprocket ||
                    actuatorType == ActuatorType.RackAndPinion)
                {
                    var diameter = Convert.ToSingle(AxisCIPDrive.CIPAxis.ActuatorDiameter);
                    transmissionRatio = transmissionRatio * diameter * Math.PI;
                }
                else if (actuatorType == ActuatorType.Screw)
                {
                    var lead = Convert.ToSingle(AxisCIPDrive.CIPAxis.ActuatorLead);
                    transmissionRatio = transmissionRatio * lead;
                }
            }

            return transmissionRatio;
        }

        public void CalculateFeedbackUnitRatio()
        {
            var loadType = (LoadType)Convert.ToByte(AxisCIPDrive.CIPAxis.LoadType);

            var transmissionRatioInput = Convert.ToUInt32(AxisCIPDrive.CIPAxis.TransmissionRatioInput);
            var transmissionRatioOutput = Convert.ToUInt32(AxisCIPDrive.CIPAxis.TransmissionRatioOutput);

            var actuatorType = (ActuatorType)Convert.ToByte(AxisCIPDrive.CIPAxis.ActuatorType);
            var actuatorLead = Convert.ToSingle(AxisCIPDrive.CIPAxis.ActuatorLead);
            var actuatorLeadUnit = (ActuatorLeadUnitType)Convert.ToByte(AxisCIPDrive.CIPAxis.ActuatorLeadUnit);
            var actuatorDiameter = Convert.ToSingle(AxisCIPDrive.CIPAxis.ActuatorDiameter);
            var actuatorDiameterUnit =
                (ActuatorDiameterUnitType)Convert.ToByte(AxisCIPDrive.CIPAxis.ActuatorDiameterUnit);


            switch (loadType)
            {
                case LoadType.DirectRotary:
                    AxisCIPDrive.CIPAxis.FeedbackUnitRatio = 1;
                    break;
                case LoadType.DirectLinear:
                    //TODO(gjc): need edit here
                    break;
                case LoadType.RotaryTransmission:
                    AxisCIPDrive.CIPAxis.FeedbackUnitRatio = (float)transmissionRatioInput / transmissionRatioOutput;
                    break;
                case LoadType.LinearActuator:
                {
                    double feedbackUnitRatio;
                    if (actuatorType == ActuatorType.Screw)
                    {
                        feedbackUnitRatio = transmissionRatioInput * 1000.0 / transmissionRatioOutput / actuatorLead;

                        if (actuatorLeadUnit == ActuatorLeadUnitType.InchPerRev)
                            feedbackUnitRatio /= 25.4;

                        AxisCIPDrive.CIPAxis.FeedbackUnitRatio = (float)feedbackUnitRatio;
                    }
                    else
                    {
                        feedbackUnitRatio = transmissionRatioInput * 1000.0 / transmissionRatioOutput /
                                            (actuatorDiameter * Math.PI);

                        if (actuatorDiameterUnit == ActuatorDiameterUnitType.Inch)
                            feedbackUnitRatio /= 25.4;

                        AxisCIPDrive.CIPAxis.FeedbackUnitRatio = (float)feedbackUnitRatio;
                    }
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void CalculateScalingFactor()
        {
            double transmissionRatio = CalculateTransmissionRatio();

            double ratedSpeed = CalculateRatedSpeed();

            var axisConfiguration = (AxisConfigurationType)Convert.ToByte(AxisCIPDrive.CIPAxis.AxisConfiguration);

            if (axisConfiguration == AxisConfigurationType.FrequencyControl)
            {
                double rotaryMotorRatedSpeed = Convert.ToSingle(AxisCIPDrive.CIPAxis.RotaryMotorRatedSpeed);
                double positionScalingNumerator = Convert.ToSingle(AxisCIPDrive.CIPAxis.PositionScalingNumerator);
                double positionScalingDenominator = Convert.ToSingle(AxisCIPDrive.CIPAxis.PositionScalingDenominator);

                double scalingRatio = positionScalingNumerator / positionScalingDenominator;

                double maximumSpeed = rotaryMotorRatedSpeed / 50 * scalingRatio * transmissionRatio;
                double maximumAcceleration =
                    rotaryMotorRatedSpeed * 7 / 6000 * scalingRatio * scalingRatio * transmissionRatio *
                    transmissionRatio;
                double maximumJerk = JerkRateCalculation.CalculateJerk(maximumSpeed, maximumAcceleration, 100);

                AxisCIPDrive.CIPAxis.MaximumSpeed = (float)(maximumSpeed);
                AxisCIPDrive.CIPAxis.MaximumAcceleration = (float)(maximumAcceleration);
                AxisCIPDrive.CIPAxis.MaximumDeceleration = (float)(maximumAcceleration);
                AxisCIPDrive.CIPAxis.MaximumAccelerationJerk = (float)(maximumJerk);
                AxisCIPDrive.CIPAxis.MaximumDecelerationJerk = (float)(maximumJerk);

                double maxSpeed = Convert.ToSingle(AxisCIPDrive.CIPAxis.RotaryMotorMaxSpeed);
                AxisCIPDrive.CIPAxis.VelocityLimitPositive = (float)(maxSpeed * 2 / 60 * scalingRatio * transmissionRatio);
                AxisCIPDrive.CIPAxis.VelocityLimitNegative = (float)(-maxSpeed * 2 / 60 * scalingRatio * transmissionRatio);

                return;
            }

            //SystemBandwidth?
            //VelocityLoopBandwidth?
            //VelocityIntegratorBandwidth?
            //FeedbackUnitRatio?
            //SystemInertia?
            //TorqueLimitPositive?
            //TorqueLimitNegative?
            //TorqueThreshold?
            //Rated Slip Speed?
            //StoppingTorque?

            // Calculate DMTC
            var dmtc = RecalculateDmtc();

            double dampingFactor = Convert.ToSingle(AxisCIPDrive.CIPAxis.DampingFactor);

            var torqueLoopBandwidth = 1 / (2 * Math.PI * dmtc);

            var velocityLoopBandwidth = CalculateVelocityLoopBandwidth(torqueLoopBandwidth, dampingFactor);
            var positionLoopBandwidth = CalculatePositionLoopBandwidth(velocityLoopBandwidth, dampingFactor);

            
            var positionErrorTolerance = CalculatePositionErrorTolerance(ratedSpeed, positionLoopBandwidth);

            double maxAcceleration = CalculateMaxAcceleration();
            var velocityErrorTolerance = maxAcceleration / (velocityLoopBandwidth * Math.PI);

            var positionLockTolerance = CalculatePositionLockTolerance();
            var velocityLockTolerance = CalculateVelocityLockTolerance();

            var velocityLimit = CalculateVelocityLimit();

            //Set Properties
            //MaximumSpeed,MaximumAcceleration,MaximumDeceleration
            AxisCIPDrive.CIPAxis.MaximumSpeed = (float)(ratedSpeed * 0.85 * transmissionRatio);
            AxisCIPDrive.CIPAxis.MaximumAcceleration = (float)(maxAcceleration * 0.7 * transmissionRatio);
            AxisCIPDrive.CIPAxis.MaximumDeceleration = (float)(maxAcceleration * 0.7 * transmissionRatio);

            //MaximumAccelerationJerk,MaximumDecelerationJerk
            var jerk = JerkRateCalculation.CalculateJerk(ratedSpeed * 0.85 * transmissionRatio,
                maxAcceleration * 0.7 * transmissionRatio, 100);
            AxisCIPDrive.CIPAxis.MaximumAccelerationJerk = (float)jerk;
            AxisCIPDrive.CIPAxis.MaximumDecelerationJerk = (float)jerk;

            //PositionErrorTolerance,VelocityErrorTolerance
            AxisCIPDrive.CIPAxis.PositionErrorTolerance = (float)(positionErrorTolerance * transmissionRatio);
            AxisCIPDrive.CIPAxis.VelocityErrorTolerance = (float)(velocityErrorTolerance * transmissionRatio);

            //PositionLockTolerance,VelocityLockTolerance
            AxisCIPDrive.CIPAxis.PositionLockTolerance = (float)(positionLockTolerance * transmissionRatio);
            AxisCIPDrive.CIPAxis.VelocityLockTolerance = (float)(velocityLockTolerance * transmissionRatio);

            //AccelerationLimit,DecelerationLimit
            AxisCIPDrive.CIPAxis.AccelerationLimit = (float)(maxAcceleration * 2 * transmissionRatio);
            AxisCIPDrive.CIPAxis.DecelerationLimit = (float)(maxAcceleration * 2 * transmissionRatio);

            //VelocityLimitPositive,VelocityLimitNegative
            AxisCIPDrive.CIPAxis.VelocityLimitPositive = (float)(velocityLimit * transmissionRatio);
            AxisCIPDrive.CIPAxis.VelocityLimitNegative = (float)(-velocityLimit * transmissionRatio);

            //VelocityThreshold,VelocityStandstillWindow
            AxisCIPDrive.CIPAxis.VelocityStandstillWindow = (float)(ratedSpeed * 0.01 * transmissionRatio);
            AxisCIPDrive.CIPAxis.VelocityThreshold = (float)(ratedSpeed * 0.9 * transmissionRatio);
        }

        public LoadParametersTuned CalculateLoadParametersTuned2()
        {
            // TODO(gjc): Rotary PM Motor
            double tuningSpeed = Convert.ToSingle(AxisCIPDrive.CIPAxis.TuningSpeed);
            double tuningTorquePercent = Convert.ToSingle(AxisCIPDrive.CIPAxis.TuningTorque) / 100;

            double tuneAccelerationTime = Convert.ToSingle(AxisCIPDrive.CIPAxis.TuneAccelerationTime);
            double tuneDecelerationTime = Convert.ToSingle(AxisCIPDrive.CIPAxis.TuneDecelerationTime);

            var loadParametersTuned = new LoadParametersTuned
            {
                TuneAccelerationTime = tuneAccelerationTime,
                TuneDecelerationTime = tuneDecelerationTime
            };

            //
            // ReSharper disable once InconsistentNaming
            //double PMMotorRatedTorque = Convert.ToSingle(AxisGeneric.CipAxis.PMMotorRatedTorque);
            double torqueConstant = Convert.ToSingle(AxisCIPDrive.CIPAxis.PMMotorTorqueConstant);
            double maximumSpeed = Convert.ToSingle(AxisCIPDrive.CIPAxis.MaximumSpeed);
            double drivePeakCurrent = Convert.ToSingle(AxisCIPDrive.CIPAxis.DriveRatedPeakCurrent);
            double motorRatedPeakCurrent = Convert.ToSingle(AxisCIPDrive.CIPAxis.MotorRatedPeakCurrent);
            double motorRatedCurrent = Convert.ToSingle(AxisCIPDrive.CIPAxis.MotorRatedContinuousCurrent);

            var peakCurrent = drivePeakCurrent < motorRatedPeakCurrent ? drivePeakCurrent : motorRatedPeakCurrent;
            double torqueScaling = peakCurrent / motorRatedCurrent;


            // TuneAcceleration,TuneDeceleration,TuneFriction
            double tuneAcceleration;
            double tuneDeceleration;
            double systemAcceleration;
            double tuneFriction;

            if (tuneAccelerationTime > tuneDecelerationTime)
            {
                // normal
                tuneAcceleration = tuningSpeed / tuneAccelerationTime / tuningTorquePercent;
                tuneDeceleration = tuningSpeed / tuneDecelerationTime / tuningTorquePercent;
                systemAcceleration = (tuneAcceleration + tuneDeceleration) / 2;
                var frictionAcceleration = systemAcceleration - tuneAcceleration;
                tuneFriction = frictionAcceleration / systemAcceleration;

                //
                loadParametersTuned.TuneAcceleration = tuneAcceleration * torqueScaling;
                loadParametersTuned.TuneDeceleration = tuneDeceleration * torqueScaling;
            }
            else
            {
                // special
                tuneAcceleration = tuningSpeed / tuneAccelerationTime / tuningTorquePercent;
                tuneDeceleration = tuningSpeed / tuneDecelerationTime;
                systemAcceleration = tuneAcceleration;
                tuneFriction = 0;

                // 
                loadParametersTuned.TuneAcceleration = tuneAcceleration * torqueScaling;
                loadParametersTuned.TuneDeceleration = tuneDeceleration;
            }

            var totalInertia =
                torqueConstant * motorRatedCurrent / (2 * Math.PI * systemAcceleration);
            loadParametersTuned.TotalInertia = totalInertia;


            loadParametersTuned.TuneFriction = tuneFriction * 100;
            loadParametersTuned.TuneLoadOffset = 0;
            loadParametersTuned.TuneInertiaMass = 100 / systemAcceleration;
            loadParametersTuned.FrictionCompensationSliding = 0;
            loadParametersTuned.TorqueOffset = 0;
            loadParametersTuned.MaximumAcceleration = loadParametersTuned.TuneAcceleration * 0.7;
            loadParametersTuned.MaximumDeceleration = loadParametersTuned.TuneDeceleration * 0.7;
            loadParametersTuned.SystemInertia = loadParametersTuned.TuneInertiaMass;

            loadParametersTuned.MaximumAccelerationJerk =
                JerkRateCalculation.CalculateJerk(maximumSpeed, loadParametersTuned.MaximumAcceleration, 100);
            loadParametersTuned.MaximumDecelerationJerk =
                JerkRateCalculation.CalculateJerk(maximumSpeed, loadParametersTuned.MaximumDeceleration, 100);

            //RotaryMotorInertia
            //double loadScaling = 0.9959679;
            // TODO(gjc): I don't know how to calculate loadScaling
            double loadScaling = 1;

            double rotaryMotorInertia = Convert.ToSingle(AxisCIPDrive.CIPAxis.RotaryMotorInertia);
            double loadRatio;
            var tuningSelect = (TuningSelectType)Convert.ToByte(AxisCIPDrive.CIPAxis.TuningSelect);
            switch (tuningSelect)
            {
                case TuningSelectType.TotalInertia:
                    loadRatio = (totalInertia * loadScaling - rotaryMotorInertia) / rotaryMotorInertia;
                    break;
                case TuningSelectType.MotorInertia:
                    // TODO(gjc): need check
                    rotaryMotorInertia = totalInertia;
                    loadRatio = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            loadParametersTuned.RotaryMotorInertia = rotaryMotorInertia;
            loadParametersTuned.LoadRatio = loadRatio;

            return loadParametersTuned;
        }

        public LoopParametersTuned CalculateLoopParametersTuned2(LoadParametersTuned loadParametersTuned)
        {
            var loopParametersTuned = new LoopParametersTuned();

            //
            double scaling = 1.00050722;
            double dampingFactor = Convert.ToSingle(AxisCIPDrive.CIPAxis.DampingFactor);
            var dmtc = RecalculateDmtc();

            // load type and transmission
            double transmissionRatio = 1;
            var loadType = (LoadType)Convert.ToByte(AxisCIPDrive.CIPAxis.LoadType);
            if ((loadType == LoadType.RotaryTransmission) ||
                (loadType == LoadType.LinearActuator))
            {
                var transmissionRatioInput = Convert.ToUInt32(AxisCIPDrive.CIPAxis.TransmissionRatioInput);
                var transmissionRatioOutput = Convert.ToUInt32(AxisCIPDrive.CIPAxis.TransmissionRatioOutput);

                transmissionRatio = (double)transmissionRatioOutput / transmissionRatioInput;
            }

            // PositionLoopBandwidth,VelocityLoopBandwidth
            // recalculate
            var torqueLoopBandwidth = 1 / (2 * Math.PI * dmtc);
            var velocityLoopBandwidth = CalculateVelocityLoopBandwidth(torqueLoopBandwidth, dampingFactor);
            var positionLoopBandwidth = CalculatePositionLoopBandwidth(velocityLoopBandwidth, dampingFactor);

            loopParametersTuned.PositionLoopBandwidth = positionLoopBandwidth * scaling;
            loopParametersTuned.VelocityLoopBandwidth = velocityLoopBandwidth * scaling;

            // PositionErrorTolerance,VelocityErrorTolerance
            var ratedSpeed = CalculateRatedSpeed();
            var positionErrorTolerance = CalculatePositionErrorTolerance(ratedSpeed, positionLoopBandwidth * scaling);

            double maximumAcceleration;
            double maximumDeceleration;
            if (loadParametersTuned != null)
            {
                maximumAcceleration = loadParametersTuned.MaximumAcceleration;
                maximumDeceleration = loadParametersTuned.MaximumDeceleration;

            }
            else
            {
                maximumAcceleration = Convert.ToSingle(AxisCIPDrive.CIPAxis.MaximumAcceleration);
                maximumDeceleration = Convert.ToSingle(AxisCIPDrive.CIPAxis.MaximumDeceleration);
            }

            var acceleration = maximumAcceleration > maximumDeceleration
                ? maximumAcceleration
                : maximumDeceleration;

            var velocityErrorTolerance = acceleration / (loopParametersTuned.VelocityLoopBandwidth * Math.PI);

            loopParametersTuned.PositionErrorTolerance = positionErrorTolerance * transmissionRatio * 0.85;
            loopParametersTuned.VelocityErrorTolerance = velocityErrorTolerance * transmissionRatio;

            // 
            var bits = (GainTuningConfigurationType)Convert.ToUInt16(AxisCIPDrive.CIPAxis.GainTuningConfigurationBits);
            if ((bits & GainTuningConfigurationType.TunePosIntegrator) > 0)
            {
                // PositionIntegratorBandwidth
                // loopParametersTuned.PositionIntegratorBandwidth
            }

            if ((bits & GainTuningConfigurationType.TuneVelIntegrator) > 0)
            {
                // VelocityIntegratorBandwidth
                // loopParametersTuned.VelocityIntegratorBandwidth
            }

            if ((bits & GainTuningConfigurationType.TuneVelFeedforward) > 0)
            {
                // VelocityFeedforwardGain
                loopParametersTuned.VelocityFeedforwardGain = 100;
            }

            if ((bits & GainTuningConfigurationType.TuneAccelFeedforward) > 0)
            {
                // AccelerationFeedforwardGain
                // loopParametersTuned.AccelerationFeedforwardGain
            }

            if ((bits & GainTuningConfigurationType.TuneTorqueLowPassFilter) > 0)
            {
                // TorqueLowPassFilterBandwidth
                loopParametersTuned.TorqueLowPassFilterBandwidth = loopParametersTuned.VelocityLoopBandwidth * 5;
            }

            var loadObserverConfiguration =
                (LoadObserverConfigurationType)Convert.ToByte(AxisCIPDrive.CIPAxis.LoadObserverConfiguration);
            if (loadObserverConfiguration != LoadObserverConfigurationType.Disabled)
            {
                // LoadObserverBandwidth,LoadObserverIntegratorBandwidth\
                // TODO(gjc):add code here
                // loopParametersTuned.LoadObserverBandwidth
                // loopParametersTuned.LoadObserverIntegratorBandwidth
            }


            // DampingFactor
            loopParametersTuned.DampingFactor = dampingFactor;

            return loopParametersTuned;
        }

        public void CalculateFrequencyControl()
        {
            var axisConfiguration = (AxisConfigurationType)Convert.ToByte(AxisCIPDrive.CIPAxis.AxisConfiguration);
            if (axisConfiguration != AxisConfigurationType.FrequencyControl)
                return;

            double ratedSlipSpeed = CalculateInductionMotorRatedSlipSpeed();

            double fluxCurrent = CalculateInductionMotorFluxCurrent();

            double leakageReactance = CalculateInductionMotorLeakageReactance();

            double statorResistance = CalculateStatorResistance();

            double maximumVoltage = CalculateMaximumVoltage();

            double maximumFrequency = CalculateMaximumFrequency();

            double startBoost = CalculateStartBoost();

            double transmissionRatio = CalculateTransmissionRatio();

            AxisCIPDrive.CIPAxis.InductionMotorRatedSlipSpeed = (float)ratedSlipSpeed;
            AxisCIPDrive.CIPAxis.InductionMotorFluxCurrent = (float)fluxCurrent;
            AxisCIPDrive.CIPAxis.InductionMotorStatorLeakageReactance = (float)leakageReactance;
            AxisCIPDrive.CIPAxis.InductionMotorRotorLeakageReactance = (float)leakageReactance;
            AxisCIPDrive.CIPAxis.InductionMotorStatorResistance = (float)statorResistance;

            AxisCIPDrive.CIPAxis.MaximumVoltage = (float)maximumVoltage;
            AxisCIPDrive.CIPAxis.MaximumFrequency = (float)maximumFrequency;
            AxisCIPDrive.CIPAxis.BreakVoltage = (float)(maximumVoltage / 2);
            AxisCIPDrive.CIPAxis.BreakFrequency =
                Convert.ToSingle(AxisCIPDrive.CIPAxis.InductionMotorRatedFrequency) / 2;

            AxisCIPDrive.CIPAxis.StartBoost = (float)startBoost;
            AxisCIPDrive.CIPAxis.RunBoost = (float)startBoost;


            double positionScalingNumerator = Convert.ToSingle(AxisCIPDrive.CIPAxis.PositionScalingNumerator);
            double positionScalingDenominator = Convert.ToSingle(AxisCIPDrive.CIPAxis.PositionScalingDenominator);

            double scalingRatio = positionScalingNumerator / positionScalingDenominator;

            //
            double maxSpeed = Convert.ToSingle(AxisCIPDrive.CIPAxis.RotaryMotorMaxSpeed);
            AxisCIPDrive.CIPAxis.VelocityLimitPositive = (float)(maxSpeed * 2 / 60 * scalingRatio * transmissionRatio);
            AxisCIPDrive.CIPAxis.VelocityLimitNegative = (float)(-maxSpeed * 2 / 60 * scalingRatio * transmissionRatio);

            // 
            double rotaryMotorRatedSpeed = Convert.ToSingle(AxisCIPDrive.CIPAxis.RotaryMotorRatedSpeed);


            double maximumSpeed = rotaryMotorRatedSpeed / 50 * scalingRatio * transmissionRatio;
            double maximumAcceleration =
                rotaryMotorRatedSpeed * 7 / 6000 * scalingRatio * scalingRatio * transmissionRatio * transmissionRatio;
            double maximumJerk = JerkRateCalculation.CalculateJerk(maximumSpeed, maximumAcceleration, 100);

            AxisCIPDrive.CIPAxis.MaximumSpeed = (float)(maximumSpeed);
            AxisCIPDrive.CIPAxis.MaximumAcceleration = (float)(maximumAcceleration);
            AxisCIPDrive.CIPAxis.MaximumDeceleration = (float)(maximumAcceleration);
            AxisCIPDrive.CIPAxis.MaximumAccelerationJerk = (float)(maximumJerk);
            AxisCIPDrive.CIPAxis.MaximumDecelerationJerk = (float)(maximumJerk);
        }

        private double CalculateStartBoost()
        {
            //Start Boost = 0.011295984 * Rated Voltage* Rated Current / (Rated Power ^ 1.3)
            double ratedVoltage = Convert.ToSingle(AxisCIPDrive.CIPAxis.MotorRatedVoltage);
            double ratedContinuousCurrent = Convert.ToSingle(AxisCIPDrive.CIPAxis.MotorRatedContinuousCurrent);
            double ratedOutputPower = Convert.ToSingle(AxisCIPDrive.CIPAxis.MotorRatedOutputPower);

            if (Math.Abs(ratedOutputPower) < double.Epsilon)
                return 0;

            double boost = 0.011295984 * ratedVoltage * ratedContinuousCurrent / Math.Pow(ratedOutputPower, 1.3);

            return boost;
        }

        private double CalculateMaximumFrequency()
        {
            double ratedFrequency = Convert.ToSingle(AxisCIPDrive.CIPAxis.InductionMotorRatedFrequency);

            return ratedFrequency * 2 + 10;
        }

        private double CalculateMaximumVoltage()
        {
            double ratedVoltage = Convert.ToSingle(AxisCIPDrive.CIPAxis.MotorRatedVoltage);

            CIPMotionDrive motionDrive = AxisCIPDrive.AssociatedModule as CIPMotionDrive;
            if (motionDrive == null)
                return 460;

            double inputVoltage = motionDrive.ConfigData.ConverterACInputVoltage;

            return inputVoltage < ratedVoltage ? inputVoltage : ratedVoltage;
        }

        private double CalculateStatorResistance()
        {
            double ratedOutputPower = Convert.ToSingle(AxisCIPDrive.CIPAxis.MotorRatedOutputPower);
            double ratedVoltage = Convert.ToSingle(AxisCIPDrive.CIPAxis.MotorRatedVoltage);

            double statorResistance = 0.00652174 * ratedVoltage * Math.Pow(ratedOutputPower, -1.3);

            return statorResistance;
        }

        private double CalculateInductionMotorLeakageReactance()
        {
            double ratedOutputPower = Convert.ToSingle(AxisCIPDrive.CIPAxis.MotorRatedOutputPower);
            double ratedVoltage = Convert.ToSingle(AxisCIPDrive.CIPAxis.MotorRatedVoltage);

            double leakageReactance = ratedVoltage * ratedVoltage / (11764.70588 * ratedOutputPower);

            return leakageReactance;
        }

        private double CalculateInductionMotorFluxCurrent()
        {
            double ratedOutputPower = Convert.ToSingle(AxisCIPDrive.CIPAxis.MotorRatedOutputPower);
            double ratedContinuousCurrent = Convert.ToSingle(AxisCIPDrive.CIPAxis.MotorRatedContinuousCurrent);

            double fluxCurrent = ratedContinuousCurrent * (0.47 - 0.04 * Math.Log(ratedOutputPower, Math.E));

            return fluxCurrent;
        }

        private double CalculateInductionMotorRatedSlipSpeed()
        {
            double ratedFrequency = Convert.ToSingle(AxisCIPDrive.CIPAxis.InductionMotorRatedFrequency);
            ushort poles = Convert.ToUInt16(AxisCIPDrive.CIPAxis.RotaryMotorPoles);
            double ratedSpeed = Convert.ToSingle(AxisCIPDrive.CIPAxis.RotaryMotorRatedSpeed);

            double ratedSlipSpeed = ratedFrequency * 120 / poles - ratedSpeed;
            if (ratedSlipSpeed < 0)
                ratedSlipSpeed = 0;

            return ratedSlipSpeed;
        }

        #region Private

        private double CalculateSystemAcceleration()
        {
            var motorType = (MotorType)Convert.ToByte(AxisCIPDrive.CIPAxis.MotorType);

            if (motorType == MotorType.NotSpecified)
                return 0;

            // Rotary PM
            if (motorType == MotorType.RotaryPermanentMagnet)
            {
                // Total Inertia
                double totalInertia;
                var gainTuningConfigurationBits = Convert.ToUInt16(AxisCIPDrive.CIPAxis.GainTuningConfigurationBits);
                if ((gainTuningConfigurationBits & (1 << 1)) > 0)
                {
                    //Use Load Ratio
                    //Recalculate  Total Inertia
                    double rotaryMotorInertia = Convert.ToSingle(AxisCIPDrive.CIPAxis.RotaryMotorInertia);
                    double loadRatio = Convert.ToSingle(AxisCIPDrive.CIPAxis.LoadRatio);
                    totalInertia = (loadRatio + 1) * rotaryMotorInertia;

                    AxisCIPDrive.CIPAxis.TotalInertia = (float)totalInertia;
                }
                else
                {
                    totalInertia = Convert.ToSingle(AxisCIPDrive.CIPAxis.TotalInertia);
                }

                // Total Inertia == 0
                if (Math.Abs(totalInertia) < double.Epsilon)
                    return 150f;

                // Calculate
                double torqueConstant = Convert.ToSingle(AxisCIPDrive.CIPAxis.PMMotorTorqueConstant);
                double ratedContinuousCurrent = Convert.ToSingle(AxisCIPDrive.CIPAxis.MotorRatedContinuousCurrent);
                var systemAcceleration = torqueConstant * ratedContinuousCurrent / (totalInertia * Math.PI * 2);

                return systemAcceleration;
            }

            // Linear PM
            if (motorType == MotorType.LinearPermanentMagnet)
            {
                // Total Mass
                double totalMass;
                var gainTuningConfigurationBits = Convert.ToUInt16(AxisCIPDrive.CIPAxis.GainTuningConfigurationBits);
                if ((gainTuningConfigurationBits & (1 << 1)) > 0)
                {
                    //Use Load Ratio
                    //Recalculate  Total Mass
                    double linearMotorMass = Convert.ToSingle(AxisCIPDrive.CIPAxis.LinearMotorMass);
                    double loadRatio = Convert.ToSingle(AxisCIPDrive.CIPAxis.LoadRatio);
                    totalMass = (loadRatio + 1) * linearMotorMass;

                    AxisCIPDrive.CIPAxis.TotalMass = (float)totalMass;
                }
                else
                {
                    totalMass = Convert.ToSingle(AxisCIPDrive.CIPAxis.TotalMass);
                }

                // Total Mass == 0
                if (Math.Abs(totalMass) < double.Epsilon)
                    return 5;

                // Calculate
                double forceConstant = Convert.ToSingle(AxisCIPDrive.CIPAxis.PMMotorForceConstant);
                double ratedContinuousCurrent = Convert.ToSingle(AxisCIPDrive.CIPAxis.MotorRatedContinuousCurrent);
                var systemAcceleration = forceConstant * ratedContinuousCurrent / totalMass;

                return systemAcceleration;
            }


            throw new ArgumentOutOfRangeException();
        }

        public static uint GetDefaultMotionResolution(MotionUnitType motionUnit)
        {
            // motion-rm003 2018,page 381
            switch (motionUnit)
            {
                case MotionUnitType.MotorRev:
                    return 1000000;
                case MotionUnitType.LoadRev:
                    return 1000000;
                case MotionUnitType.FeedbackRev:
                    return 1000000;
                case MotionUnitType.MotorMm:
                    return 10000;
                case MotionUnitType.LoadMm:
                    return 10000;
                case MotionUnitType.FeedbackMm:
                    return 10000;
                case MotionUnitType.MotorInch:
                    return 200000;
                case MotionUnitType.LoadInch:
                    return 200000;
                case MotionUnitType.FeedbackInch:
                    return 200000;
                case MotionUnitType.MotorRevPerS:
                    return 1000000;
                case MotionUnitType.LoadRevPerS:
                    return 1000000;
                case MotionUnitType.MotorMPerS:
                    return 10000000;
                case MotionUnitType.LoadMPerS:
                    return 10000000;
                case MotionUnitType.MotorInchPerS:
                    return 200000;
                case MotionUnitType.LoadInchPerS:
                    return 200000;
                default:
                    return 1000000;
            }

        }

        private uint EstimateBaseMotionResolution(
            uint defaultValue,
            TravelModeType travelMode,
            double numerator, double denominator,
            double travelRange,
            double unwindNumerator, double unwindDenominator)
        {
            //TODO(gjc): need edit here
            var tempRange = numerator * travelRange * unwindDenominator;
            var tempUnwind = denominator * unwindNumerator / unwindDenominator;

            uint temp = 1;
            while (true)
            {
                if ((uint)(numerator * unwindDenominator * temp) > defaultValue)
                    break;

                if (travelMode == TravelModeType.Cyclic)
                    if (tempUnwind * temp > int.MaxValue)
                        break;

                if (travelMode == TravelModeType.Limited)
                    if (tempRange * temp > int.MaxValue)
                        break;

                temp *= 10;
            }

            if (temp > 1)
                temp /= 10;

            return temp;
        }

        private void EstimateMotionResolution(
            uint defaultValue,
            TravelModeType travelMode,
            double numerator, double denominator,
            double travelRange,
            double unwindNumerator, double unwindDenominator,
            ref uint motionResolution,
            ref float conversionConstant,
            ref uint positionUnwind)
        {
            uint maxResolution;
            uint baseResolution;

            if (travelMode == TravelModeType.Cyclic)
            {
                maxResolution = (uint)((2147483648 - 1) * (numerator / denominator) /
                                       (unwindNumerator / unwindDenominator));
                baseResolution = Math.Min(defaultValue, maxResolution);

                motionResolution = (uint)((numerator * unwindDenominator) *
                                          Math.Pow(10,
                                              (int)(Math.Log10(baseResolution / (numerator * unwindDenominator)))));
                conversionConstant = (float)(motionResolution * (denominator / numerator));
                positionUnwind = (uint)(conversionConstant * (unwindNumerator / unwindDenominator));
            }

            if (travelMode == TravelModeType.Limited)
            {
                maxResolution = (uint)((2147483648 - 1) * (numerator / denominator) / travelRange);
                baseResolution = Math.Min(defaultValue, maxResolution);
                motionResolution = (uint)(numerator * Math.Pow(10, (int)(Math.Log10(baseResolution / numerator))));
                conversionConstant = (float)(motionResolution * (denominator / numerator));
            }

            if (travelMode == TravelModeType.Unlimited)
            {
                baseResolution = defaultValue;
                motionResolution = (uint)(numerator * Math.Pow(10, (int)(Math.Log10(baseResolution / numerator))));
                conversionConstant = (float)(motionResolution * (denominator / numerator));
            }

        }

        private void UpdateDerivedProperties()
        {
            // Derived Properties

            // SystemDamping
            var loopResponse = (LoopResponseType)Convert.ToByte(AxisCIPDrive.CIPAxis.LoopResponse);
            switch (loopResponse)
            {
                case LoopResponseType.Low:
                    AxisCIPDrive.CIPAxis.DampingFactor = 1.5f;
                    AxisCIPDrive.CIPAxis.SystemDamping = 1.5f;
                    break;
                case LoopResponseType.Medium:
                    AxisCIPDrive.CIPAxis.DampingFactor = 1.0f;
                    AxisCIPDrive.CIPAxis.SystemDamping = 1.0f;
                    break;
                case LoopResponseType.High:
                    AxisCIPDrive.CIPAxis.DampingFactor = 0.8f;
                    AxisCIPDrive.CIPAxis.SystemDamping = 0.8f;
                    break;
            }
        }

        private double RecalculateDmtc()
        {
            // TODO(gjc):edit, load from database in switch axis channel
            // 0.000505819
            // double driveModelTimeConstantBase = 0.000505819;
            double driveModelTimeConstantBase = Convert.ToSingle(AxisCIPDrive.CIPAxis.DriveModelTimeConstantBase);

            // Feedback time
            var feedbackTime = CalculateFeedbackTime();

            // Total time
            var driveModelTimeConstant = driveModelTimeConstantBase + feedbackTime;

            AxisCIPDrive.CIPAxis.DriveModelTimeConstant = (float)driveModelTimeConstant;

            return driveModelTimeConstant;
        }

        private double CalculateFeedbackTime()
        {
            double specialValue;

            // Feedback time
            ulong cycleInterpolation = Convert.ToUInt32(AxisCIPDrive.CIPAxis.Feedback1CycleInterpolation);
            ulong cycleResolution = Convert.ToUInt32(AxisCIPDrive.CIPAxis.Feedback1CycleResolution);
            var resolution = cycleInterpolation * cycleResolution;

            //Feedback 1 Type
            var feedback1Type = (FeedbackType)Convert.ToByte(AxisCIPDrive.CIPAxis.Feedback1Type);
            if (feedback1Type == FeedbackType.NotSpecified)
                resolution = 4000;

            // Motor Type
            var motorType = (MotorType)Convert.ToByte(AxisCIPDrive.CIPAxis.MotorType);
            switch (motorType)
            {
                case MotorType.NotSpecified:
                    specialValue = 40.96;
                    break;
                case MotorType.RotaryPermanentMagnet:
                    var rotaryMotorPoles = Convert.ToUInt16(AxisCIPDrive.CIPAxis.RotaryMotorPoles);
                    specialValue = 1.024 * rotaryMotorPoles;
                    break;
                case MotorType.LinearPermanentMagnet:
                    specialValue = 40.96;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return specialValue / resolution;
        }

        private double CalculateVelocityLoopBandwidth(double torqueLoopBandwidth, double dampingFactor)
        {
            var velocityLoopBandwidth = torqueLoopBandwidth / (4 * dampingFactor * dampingFactor);

            // Load Coupling(Compliant) + Use Load Ratio
            var loadCoupling = (LoadCouplingType)Convert.ToByte(AxisCIPDrive.CIPAxis.LoadCoupling);
            var gainTuningConfigurationBits = Convert.ToUInt16(AxisCIPDrive.CIPAxis.GainTuningConfigurationBits);
            if ((loadCoupling == LoadCouplingType.Compliant)
                && ((gainTuningConfigurationBits & (1 << 1)) > 0))
            {
                double loadRatio = Convert.ToSingle(AxisCIPDrive.CIPAxis.LoadRatio);
                velocityLoopBandwidth /= loadRatio + 1;
            }

            return velocityLoopBandwidth;
        }

        private double CalculatePositionLoopBandwidth(double velocityLoopBandwidth, double dampingFactor)
        {
            var positionLoopBandwidth = velocityLoopBandwidth / (4 * dampingFactor * dampingFactor);
            var loadObserverConfiguration =
                (LoadObserverConfigurationType)Convert.ToByte(AxisCIPDrive.CIPAxis.LoadObserverConfiguration);

            // Load Coupling(Compliant)  
            var loadCoupling = (LoadCouplingType)Convert.ToByte(AxisCIPDrive.CIPAxis.LoadCoupling);
            if (loadCoupling == LoadCouplingType.Compliant
                && loadObserverConfiguration != LoadObserverConfigurationType.LoadObserverWithVelocityEstimate)
            {
                var gainTuningConfigurationBits = Convert.ToUInt16(AxisCIPDrive.CIPAxis.GainTuningConfigurationBits);
                if ((gainTuningConfigurationBits & (1 << 1)) > 0)
                {
                    // Use Load Ratio
                    var loadRatio = Convert.ToSingle(AxisCIPDrive.CIPAxis.LoadRatio);
                    // loadRatio == 0
                    if (Math.Abs(loadRatio) < float.Epsilon)
                        positionLoopBandwidth /= 10;
                }
                else
                {
                    var totalInertia = Convert.ToSingle(AxisCIPDrive.CIPAxis.TotalInertia);
                    // totalInertia == 0
                    if (Math.Abs(totalInertia) < float.Epsilon)
                        positionLoopBandwidth /= 10;
                }
            }

            // Load Rigid

            if (loadCoupling == LoadCouplingType.Rigid
                && loadObserverConfiguration == LoadObserverConfigurationType.LoadObserverOnly)
            {
                var gainTuningConfigurationBits = Convert.ToUInt16(AxisCIPDrive.CIPAxis.GainTuningConfigurationBits);
                if ((gainTuningConfigurationBits & (1 << 1)) > 0)
                {
                    // Use Load Ratio
                    var loadRatio = Convert.ToSingle(AxisCIPDrive.CIPAxis.LoadRatio);
                    // loadRatio == 0
                    if (Math.Abs(loadRatio) < float.Epsilon)
                        positionLoopBandwidth /= 10;
                }
                else
                {
                    var totalInertia = Convert.ToSingle(AxisCIPDrive.CIPAxis.TotalInertia);
                    // totalInertia == 0
                    if (Math.Abs(totalInertia) < float.Epsilon)
                        positionLoopBandwidth /= 10;
                }
            }


            return positionLoopBandwidth;
        }

        private double CalculateRatedSpeed()
        {
            // TODO(gjc): for rotary
            double rotaryMotorRatedSpeed = Convert.ToSingle(AxisCIPDrive.CIPAxis.RotaryMotorRatedSpeed);

            // scaling
            double positionScalingNumerator = Convert.ToSingle(AxisCIPDrive.CIPAxis.PositionScalingNumerator);
            double positionScalingDenominator = Convert.ToSingle(AxisCIPDrive.CIPAxis.PositionScalingDenominator);

            return rotaryMotorRatedSpeed * positionScalingNumerator / (60 * positionScalingDenominator);
        }

        private double CalculatePositionErrorTolerance(double ratedSpeed, double positionLoopBandwidth)
        {
            // rm003,page293
            // Position Error Tolerance = 2 * Max Speed / Position Loop Bandwidth (rad / s)

            return ratedSpeed / (positionLoopBandwidth * Math.PI);
        }

        // Position Units/s^2
        private double CalculateMaxAcceleration()
        {
            // recalculate total inertia or total mass
            CalculateSystemInertia();

            // for Rotary PM
            // rm003,page293
            double drivePeakCurrent = Convert.ToSingle(AxisCIPDrive.CIPAxis.DriveRatedPeakCurrent);
            double motorRatedPeakCurrent = Convert.ToSingle(AxisCIPDrive.CIPAxis.MotorRatedPeakCurrent);
            var peakCurrent = drivePeakCurrent < motorRatedPeakCurrent ? drivePeakCurrent : motorRatedPeakCurrent;

            double torqueConstant = Convert.ToSingle(AxisCIPDrive.CIPAxis.PMMotorTorqueConstant);
            double totalInertia = Convert.ToSingle(AxisCIPDrive.CIPAxis.TotalInertia);

            double positionScalingNumerator = Convert.ToSingle(AxisCIPDrive.CIPAxis.PositionScalingNumerator);
            double positionScalingDenominator = Convert.ToSingle(AxisCIPDrive.CIPAxis.PositionScalingDenominator);

            //
            double maxAcceleration;
            if (totalInertia * 2 * Math.PI * positionScalingDenominator < double.Epsilon)
            {
                double ratedContinuousCurrent = Convert.ToSingle(AxisCIPDrive.CIPAxis.MotorRatedContinuousCurrent);

                maxAcceleration = 150 * peakCurrent * positionScalingNumerator /
                                  (ratedContinuousCurrent * positionScalingDenominator);
            }
            else
            {
                maxAcceleration = torqueConstant * peakCurrent * positionScalingNumerator /
                                  (totalInertia * 2 * Math.PI * positionScalingDenominator);
            }


            if (double.IsNaN(maxAcceleration))
                return 0;

            return maxAcceleration;
        }

        private double CalculateTorqueLowPassFilterBandwidth(double torqueLoopBandwidth, double velocityLoopBandwidth)
        {

            var loadObserverConfiguration =
                (LoadObserverConfigurationType)Convert.ToByte(AxisCIPDrive.CIPAxis.LoadObserverConfiguration);
            if (loadObserverConfiguration == LoadObserverConfigurationType.LoadObserverWithVelocityEstimate
                || loadObserverConfiguration == LoadObserverConfigurationType.VelocityEstimateOnly)
            {

                var gainTuningConfigurationBits = Convert.ToUInt16(AxisCIPDrive.CIPAxis.GainTuningConfigurationBits);
                if ((gainTuningConfigurationBits & (1 << 1)) > 0)
                {
                    // Use Load Ratio
                    var loadRatio = Convert.ToSingle(AxisCIPDrive.CIPAxis.LoadRatio);
                    // loadRatio == 0
                    if (Math.Abs(loadRatio) < float.Epsilon)
                        return torqueLoopBandwidth * 5;
                }
                else
                {
                    var totalInertia = Convert.ToSingle(AxisCIPDrive.CIPAxis.TotalInertia);
                    // totalInertia == 0
                    if (Math.Abs(totalInertia) < float.Epsilon)
                        return torqueLoopBandwidth * 5;
                }
            }

            return velocityLoopBandwidth * 5;

        }

        private double CalculateLoadObserverBandwidth(double torqueLoopBandwidth, double velocityLoopBandwidth)
        {
            var loadObserverConfiguration =
                (LoadObserverConfigurationType)Convert.ToByte(AxisCIPDrive.CIPAxis.LoadObserverConfiguration);

            if (loadObserverConfiguration == LoadObserverConfigurationType.Disabled)
                return 0;

            if (loadObserverConfiguration == LoadObserverConfigurationType.LoadObserverWithVelocityEstimate
                || loadObserverConfiguration == LoadObserverConfigurationType.VelocityEstimateOnly)
            {

                var gainTuningConfigurationBits = Convert.ToUInt16(AxisCIPDrive.CIPAxis.GainTuningConfigurationBits);
                if ((gainTuningConfigurationBits & (1 << 1)) > 0)
                {
                    // Use Load Ratio
                    var loadRatio = Convert.ToSingle(AxisCIPDrive.CIPAxis.LoadRatio);
                    // loadRatio == 0
                    if (Math.Abs(loadRatio) < float.Epsilon)
                        return torqueLoopBandwidth;
                }
                else
                {
                    var totalInertia = Convert.ToSingle(AxisCIPDrive.CIPAxis.TotalInertia);
                    // totalInertia == 0
                    if (Math.Abs(totalInertia) < float.Epsilon)
                        return torqueLoopBandwidth;
                }

            }

            return velocityLoopBandwidth;
        }

        private double CalculatePositionLockTolerance()
        {
            // scaling
            double positionScalingNumerator = Convert.ToSingle(AxisCIPDrive.CIPAxis.PositionScalingNumerator);
            double positionScalingDenominator = Convert.ToSingle(AxisCIPDrive.CIPAxis.PositionScalingDenominator);

            double baseTolerance;
            // Motor Type
            var motorType = (MotorType)Convert.ToByte(AxisCIPDrive.CIPAxis.MotorType);
            switch (motorType)
            {
                case MotorType.NotSpecified:
                    baseTolerance = 0.001;
                    break;
                case MotorType.RotaryPermanentMagnet:
                    baseTolerance = 0.01;
                    break;

                case MotorType.LinearPermanentMagnet:
                    baseTolerance = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return baseTolerance * positionScalingNumerator / positionScalingDenominator;
        }

        private double CalculateVelocityLockTolerance()
        {
            // TODO(gjc): need edit
            double ratedSpeed = Convert.ToSingle(AxisCIPDrive.CIPAxis.RotaryMotorRatedSpeed);

            // scaling
            double positionScalingNumerator = Convert.ToSingle(AxisCIPDrive.CIPAxis.PositionScalingNumerator);
            double positionScalingDenominator = Convert.ToSingle(AxisCIPDrive.CIPAxis.PositionScalingDenominator);

            return ratedSpeed * positionScalingNumerator / (6000 * positionScalingDenominator);
        }

        private double CalculateVelocityLimit()
        {
            // TODO(gjc): // for Rotary PM

            // scaling
            double positionScalingNumerator = Convert.ToSingle(AxisCIPDrive.CIPAxis.PositionScalingNumerator);
            double positionScalingDenominator = Convert.ToSingle(AxisCIPDrive.CIPAxis.PositionScalingDenominator);

            double maxSpeed = Convert.ToSingle(AxisCIPDrive.CIPAxis.RotaryMotorMaxSpeed);
            var rotaryMotorPoles = Convert.ToUInt16(AxisCIPDrive.CIPAxis.RotaryMotorPoles);
            var driveMaxOutputFrequency = Convert.ToInt32(AxisCIPDrive.CIPAxis.DriveMaxOutputFrequency);

            // maxSpeed * 2
            var velocityLimit = maxSpeed / 30;
            var maxVelocityLimit = (double)driveMaxOutputFrequency * 2 / rotaryMotorPoles;


            if (velocityLimit > maxVelocityLimit)
                velocityLimit = maxVelocityLimit;

            return velocityLimit * positionScalingNumerator / positionScalingDenominator;
        }

        private double CalculateTorqueLimit()
        {
            double drivePeakCurrent = Convert.ToSingle(AxisCIPDrive.CIPAxis.DriveRatedPeakCurrent);
            double motorRatedPeakCurrent = Convert.ToSingle(AxisCIPDrive.CIPAxis.MotorRatedPeakCurrent);
            var peakCurrent = drivePeakCurrent < motorRatedPeakCurrent ? drivePeakCurrent : motorRatedPeakCurrent;

            double ratedCurrent = Convert.ToSingle(AxisCIPDrive.CIPAxis.MotorRatedContinuousCurrent);

            if (Math.Abs(ratedCurrent) < double.Epsilon)
                return 0;

            // max 10
            var torqueLimit = peakCurrent / ratedCurrent;
            if (torqueLimit > 10)
                torqueLimit = 10;

            return torqueLimit;
        }

        private double CalculateFeedback1VelocityFilterBandwidth()
        {
            ulong cycleInterpolation = Convert.ToUInt32(AxisCIPDrive.CIPAxis.Feedback1CycleInterpolation);
            ulong cycleResolution = Convert.ToUInt32(AxisCIPDrive.CIPAxis.Feedback1CycleResolution);
            ushort motorPoles = Convert.ToUInt16(AxisCIPDrive.CIPAxis.RotaryMotorPoles);

            var resolution = cycleInterpolation * cycleResolution;

            double velocityFilterBandwidth = resolution;
            // TODO(gjc): special value, I don't know why, need edit here
            // 0.155424749851226806640625
            velocityFilterBandwidth = velocityFilterBandwidth * 1.9428093 / 100;

            var axisConfiguration = (AxisConfigurationType)Convert.ToByte(AxisCIPDrive.CIPAxis.AxisConfiguration);
            if (axisConfiguration == AxisConfigurationType.FeedbackOnly)
                return velocityFilterBandwidth / 5;

            var motorType = (MotorType)Convert.ToByte(AxisCIPDrive.CIPAxis.MotorType);
            switch (motorType)
            {
                case MotorType.NotSpecified:
                    return velocityFilterBandwidth / 5;

                case MotorType.RotaryPermanentMagnet:
                    return resolution * 0.155424749851226806640625 / motorPoles;

                case MotorType.RotaryInduction:
                    break;

                case MotorType.LinearPermanentMagnet:
                    return velocityFilterBandwidth / 5;

                case MotorType.LinearInduction:
                    break;

                case MotorType.RotaryInteriorPermanentMagnet:
                    return resolution * 0.155424749851226806640625 / motorPoles;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            throw new ArgumentOutOfRangeException();
        }

        #endregion


    }

    public class LoadParametersTuned
    {
        public double TuneAccelerationTime { get; set; }
        public double TuneDecelerationTime { get; set; }
        public double TuneAcceleration { get; set; }
        public double TuneDeceleration { get; set; }
        public double TuneInertiaMass { get; set; }
        public double TuneFriction { get; set; }
        public double TuneLoadOffset { get; set; }

        public double TotalInertia { get; set; }
        public double SystemInertia { get; set; }
        public double RotaryMotorInertia { get; set; }
        public double LoadRatio { get; set; }

        public double FrictionCompensationSliding { get; set; }
        public double TorqueOffset { get; set; }

        public double MaximumAcceleration { get; set; }
        public double MaximumDeceleration { get; set; }
        public double MaximumAccelerationJerk { get; set; }
        public double MaximumDecelerationJerk { get; set; }
    }

    public class LoopParametersTuned
    {
        public double PositionLoopBandwidth { get; set; }
        public double PositionIntegratorBandwidth { get; set; }
        public double VelocityLoopBandwidth { get; set; }
        public double VelocityIntegratorBandwidth { get; set; }
        public double VelocityFeedforwardGain { get; set; }
        public double AccelerationFeedforwardGain { get; set; }
        public double LoadObserverBandwidth { get; set; }
        public double LoadObserverIntegratorBandwidth { get; set; }
        public double TorqueLowPassFilterBandwidth { get; set; }
        public double PositionErrorTolerance { get; set; }
        public double VelocityErrorTolerance { get; set; }
        public double DampingFactor { get; set; }
    }
}
