using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Controls.ExtendedPropertyGrid.Attributes;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Models
{
    [SuppressMessage("ReSharper", "NotResolvedInText")]
    [SuppressMessage("ReSharper", "UseNameofExpression")]
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
    internal partial class AxisCIPParameters
    {
        private readonly Scaling _scaling;

        #region Public Property

        private float _feedbackUnitRatio;

        [Browsable(false)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Unit("Motion Rev/Load Rev")]
        [Category("Scaling")]
        public float FeedbackUnitRatio
        {
            get
            {
                _feedbackUnitRatio = Convert.ToSingle(_currentAxis.CIPAxis.FeedbackUnitRatio);
                return _feedbackUnitRatio;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.FeedbackUnitRatio) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.FeedbackUnitRatio = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }


        private float _actuatorDiameter;

        [Browsable(true)]
        [ReadOnly(true)]
        [IsChanged(false)]
        [Category("Scaling")]
        public float ActuatorDiameter
        {
            get
            {
                _actuatorDiameter = Convert.ToSingle(_currentAxis.CIPAxis.ActuatorDiameter);
                return _actuatorDiameter;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.ActuatorDiameter) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.ActuatorDiameter = value;
                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private ActuatorDiameterUnitType _actuatorDiameterUnit;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(true)]
        [Category("Scaling")]
        public ActuatorDiameterUnitType ActuatorDiameterUnit
        {
            get
            {
                _actuatorDiameterUnit =
                    (ActuatorDiameterUnitType) Convert.ToByte(_currentAxis.CIPAxis.ActuatorDiameterUnit);
                return _actuatorDiameterUnit;
            }
            set
            {
                if (Convert.ToByte(_currentAxis.CIPAxis.ActuatorDiameterUnit) != (byte) value)
                {
                    _currentAxis.CIPAxis.ActuatorDiameterUnit = (byte) value;

                    AxisDefaultSetting.UpdateMotionUnit(_currentAxis.CIPAxis);
                    UpdateIsChanged("MotionUnit");
                    OnPropertyChanged("MotionUnit");

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<ActuatorDiameterUnitType> ActuatorDiameterUnitSource { get; } =
            new ObservableCollection<ActuatorDiameterUnitType>
            {
                ActuatorDiameterUnitType.Mm,
                ActuatorDiameterUnitType.Inch
            };

        private float _actuatorLead;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(true)]
        [Category("Scaling")]
        public float ActuatorLead
        {
            get
            {
                _actuatorLead = Convert.ToSingle(_currentAxis.CIPAxis.ActuatorLead);
                return _actuatorLead;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.ActuatorLead) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.ActuatorLead = value;
                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }


        private ActuatorLeadUnitType _actuatorLeadUnit;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(true)]
        [Category("Scaling")]
        public ActuatorLeadUnitType ActuatorLeadUnit
        {
            get
            {
                _actuatorLeadUnit = (ActuatorLeadUnitType) Convert.ToByte(_currentAxis.CIPAxis.ActuatorLeadUnit);
                return _actuatorLeadUnit;
            }
            set
            {
                if (Convert.ToByte(_currentAxis.CIPAxis.ActuatorLeadUnit) != (byte) value)
                {
                    _currentAxis.CIPAxis.ActuatorLeadUnit = (byte) value;

                    AxisDefaultSetting.UpdateMotionUnit(_currentAxis.CIPAxis);
                    UpdateIsChanged("MotionUnit");
                    OnPropertyChanged("MotionUnit");

                    UpdateIsChanged();
                    OnPropertyChanged();
                }


            }
        }


        private ObservableCollection<ActuatorLeadUnitType> ActuatorLeadUnitSource { get; } =
            new ObservableCollection<ActuatorLeadUnitType>
            {
                ActuatorLeadUnitType.MmPerRev,
                ActuatorLeadUnitType.InchPerRev
            };

        private float _travelRange;

        [Browsable(true)]
        [ReadOnly(true)]
        [IsChanged(false)]
        [Unit("Position Units")]
        [Category("Scaling")]
        public float TravelRange
        {
            get
            {
                _travelRange = Convert.ToSingle(_currentAxis.CIPAxis.TravelRange);
                return _travelRange;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.TravelRange) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.TravelRange = value;
                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private LoadType _loadType;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Scaling")]
        public LoadType LoadType
        {
            get
            {
                _loadType = (LoadType) Convert.ToByte(_currentAxis.CIPAxis.LoadType);
                return _loadType;
            }
            set
            {
                var loadType = (LoadType) Convert.ToByte(_currentAxis.CIPAxis.LoadType);
                if (loadType != value)
                {
                    _currentAxis.CIPAxis.LoadType = (byte) value;

                    AxisDefaultSetting.UpdateMotionUnit(_currentAxis.CIPAxis);
                    UpdateIsChanged("MotionUnit");
                    OnPropertyChanged("MotionUnit");

                    _scaling.UpdateReadonly();

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<LoadType> LoadTypeSource { get; set; }


        private ActuatorType _actuatorType;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(true)]
        [Category("Scaling")]
        public ActuatorType ActuatorType
        {
            get
            {
                _actuatorType = (ActuatorType) Convert.ToByte(_currentAxis.CIPAxis.ActuatorType);
                return _actuatorType;
            }
            set
            {
                var actuatorType = (ActuatorType) Convert.ToByte(_currentAxis.CIPAxis.ActuatorType);
                if (actuatorType != value)
                {
                    _currentAxis.CIPAxis.ActuatorType = (byte) value;

                    AxisDefaultSetting.UpdateMotionUnit(_currentAxis.CIPAxis);
                    UpdateIsChanged("MotionUnit");
                    OnPropertyChanged("MotionUnit");

                    _scaling.UpdateReadonly();

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }

        private ObservableCollection<ActuatorType> ActuatorTypeSource { get; } = new ObservableCollection<ActuatorType>
        {
            ActuatorType.None,
            ActuatorType.Screw,
            ActuatorType.BeltAndPulley,
            ActuatorType.ChainAndSprocket,
            ActuatorType.RackAndPinion
        };

        private float _conversionConstant;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(true)]
        [Unit("Motion Counts/Position Units")]
        [Category("Scaling")]
        public float ConversionConstant
        {
            get
            {
                _conversionConstant = Convert.ToSingle(_currentAxis.CIPAxis.ConversionConstant);
                return _conversionConstant;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.ConversionConstant) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.ConversionConstant = value;
                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }


        private ScalingSourceType _scalingSource;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Scaling")]
        public ScalingSourceType ScalingSource
        {
            get
            {
                _scalingSource = (ScalingSourceType) Convert.ToByte(_currentAxis.CIPAxis.ScalingSource);
                return _scalingSource;
            }
            set
            {
                var scalingSource = (ScalingSourceType) Convert.ToByte(_currentAxis.CIPAxis.ScalingSource);
                if (scalingSource != value)
                {
                    _currentAxis.CIPAxis.ScalingSource = (byte) value;

                    _scaling.UpdateReadonly();

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<ScalingSourceType> ScalingSourceSource { get; } =
            new ObservableCollection<ScalingSourceType>
            {
                ScalingSourceType.FromCalculator,
                ScalingSourceType.DirectScalingFactorEntry
            };

        private TravelModeType _travelMode;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Scaling")]
        public TravelModeType TravelMode
        {
            get
            {
                _travelMode = (TravelModeType) Convert.ToByte(_currentAxis.CIPAxis.TravelMode);
                return _travelMode;
            }
            set
            {
                var travelMode = (TravelModeType) Convert.ToByte(_currentAxis.CIPAxis.TravelMode);
                if (travelMode != value)
                {
                    _currentAxis.CIPAxis.TravelMode = (byte) value;

                    _scaling.UpdateReadonly();

                    UpdateIsChanged();
                    OnPropertyChanged();
                }


            }
        }

        private ObservableCollection<TravelModeType> TravelModeSource { get; set; }


        private BooleanTypeA _softTravelLimitChecking;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Scaling")]
        public BooleanTypeA SoftTravelLimitChecking
        {
            get
            {
                _softTravelLimitChecking = (BooleanTypeA) Convert.ToByte(_currentAxis.CIPAxis.SoftTravelLimitChecking);
                return _softTravelLimitChecking;
            }
            set
            {
                var softTravelLimitChecking =
                    (BooleanTypeA) Convert.ToByte(_currentAxis.CIPAxis.SoftTravelLimitChecking);
                if (softTravelLimitChecking != value)
                {
                    _currentAxis.CIPAxis.SoftTravelLimitChecking = (byte) value;

                    _scaling.UpdateReadonly();

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }

        private ObservableCollection<BooleanTypeA> SoftTravelLimitCheckingSource { get; } =
            new ObservableCollection<BooleanTypeA>
            {
                BooleanTypeA.No,
                BooleanTypeA.Yes
            };


        private uint _motionResolution;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(true)]
        [Unit("Motion Counts/Motor Rev")]
        [Category("Scaling")]
        public uint MotionResolution
        {
            get
            {
                _motionResolution = Convert.ToUInt32(_currentAxis.CIPAxis.MotionResolution);
                return _motionResolution;
            }
            set
            {
                if (Convert.ToUInt32(_currentAxis.CIPAxis.MotionResolution) != value)
                {
                    _currentAxis.CIPAxis.MotionResolution = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private MotionScalingConfigurationType _motionScalingConfiguration;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Scaling")]
        public MotionScalingConfigurationType MotionScalingConfiguration
        {
            get
            {
                _motionScalingConfiguration =
                    (MotionScalingConfigurationType) Convert.ToByte(_currentAxis.CIPAxis.MotionScalingConfiguration);
                return _motionScalingConfiguration;
            }
            set
            {
                if (Convert.ToByte(_currentAxis.CIPAxis.MotionScalingConfiguration) != (byte) value)
                {
                    _currentAxis.CIPAxis.MotionScalingConfiguration = (byte) value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }

        private ObservableCollection<MotionScalingConfigurationType> MotionScalingConfigurationSource { get; } =
            new ObservableCollection<MotionScalingConfigurationType>
            {
                MotionScalingConfigurationType.ControlScaling
            };

        private MotionUnitType _motionUnit;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(true)]
        [Category("Scaling")]
        public MotionUnitType MotionUnit
        {
            get
            {
                _motionUnit = (MotionUnitType) Convert.ToByte(_currentAxis.CIPAxis.MotionUnit);
                return _motionUnit;
            }
            set
            {
                if (Convert.ToByte(_currentAxis.CIPAxis.MotionUnit) != (byte) value)
                {
                    _currentAxis.CIPAxis.MotionUnit = (byte) value;
                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }


        private ObservableCollection<MotionUnitType> MotionUnitSource { get; } =
            new ObservableCollection<MotionUnitType>
            {
                MotionUnitType.MotorRev,
                MotionUnitType.LoadRev,
                MotionUnitType.FeedbackRev,
                MotionUnitType.MotorMm,
                MotionUnitType.LoadMm,
                MotionUnitType.FeedbackMm,
                MotionUnitType.MotorInch,
                MotionUnitType.LoadInch,
                MotionUnitType.FeedbackInch,
                MotionUnitType.MotorRevPerS,
                MotionUnitType.LoadRevPerS,
                MotionUnitType.MotorMPerS,
                MotionUnitType.LoadMPerS,
                MotionUnitType.MotorInchPerS,
                MotionUnitType.LoadInchPerS
            };


        private float _positionScalingDenominator;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Unit("Motor Rev")]
        [Category("Scaling")]
        public float PositionScalingDenominator
        {
            get
            {
                _positionScalingDenominator = Convert.ToSingle(_currentAxis.CIPAxis.PositionScalingDenominator);
                return _positionScalingDenominator;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.PositionScalingDenominator) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.PositionScalingDenominator = value;
                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _positionScalingNumerator;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Unit("Position Units")]
        [Category("Scaling")]
        public float PositionScalingNumerator
        {
            get
            {
                _positionScalingNumerator = Convert.ToSingle(_currentAxis.CIPAxis.PositionScalingNumerator);
                return _positionScalingNumerator;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.PositionScalingNumerator) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.PositionScalingNumerator = value;
                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private string _positionUnits;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Scaling")]
        public string PositionUnits
        {
            get
            {
                _positionUnits = _currentAxis.CIPAxis.PositionUnits.GetString();
                return _positionUnits;
            }
            set
            {
                var positionUnits = _currentAxis.CIPAxis.PositionUnits.GetString();

                if (!string.Equals(positionUnits, value))
                {
                    _currentAxis.CIPAxis.PositionUnits = value;

                    _scaling.UpdateUnit();

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private uint _positionUnwind;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(true)]
        [Unit("Motion Counts/Unwind Cycle")]
        [Category("Scaling")]
        public uint PositionUnwind
        {
            get
            {
                _positionUnwind = Convert.ToUInt32(_currentAxis.CIPAxis.PositionUnwind);
                return _positionUnwind;
            }
            set
            {
                if (Convert.ToUInt32(_currentAxis.CIPAxis.PositionUnwind) != value)
                {
                    _currentAxis.CIPAxis.PositionUnwind = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _positionUnwindDenominator;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(true)]
        [Unit("Unwind Cycles")]
        [Category("Scaling")]
        public float PositionUnwindDenominator
        {
            get
            {
                _positionUnwindDenominator = Convert.ToSingle(_currentAxis.CIPAxis.PositionUnwindDenominator);
                return _positionUnwindDenominator;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.PositionUnwindDenominator) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.PositionUnwindDenominator = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _positionUnwindNumerator;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(true)]
        [Unit("Position Units")]
        [Category("Scaling")]
        public float PositionUnwindNumerator
        {
            get
            {
                _positionUnwindNumerator = Convert.ToSingle(_currentAxis.CIPAxis.PositionUnwindNumerator);
                return _positionUnwindNumerator;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.PositionUnwindNumerator) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.PositionUnwindNumerator = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _softTravelLimitNegative;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(true)]
        [Unit("Position Units")]
        [Category("Scaling")]
        public float SoftTravelLimitNegative
        {
            get
            {
                _softTravelLimitNegative = Convert.ToSingle(_currentAxis.CIPAxis.SoftTravelLimitNegative);
                return _softTravelLimitNegative;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.SoftTravelLimitNegative) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.SoftTravelLimitNegative = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }

        private float _softTravelLimitPositive;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(true)]
        [Unit("Position Units")]
        [Category("Scaling")]
        public float SoftTravelLimitPositive
        {
            get
            {
                _softTravelLimitPositive = Convert.ToSingle(_currentAxis.CIPAxis.SoftTravelLimitPositive);
                return _softTravelLimitPositive;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.SoftTravelLimitPositive) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.SoftTravelLimitPositive = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private uint _transmissionRatioInput;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(true)]
        [Category("Scaling")]
        public uint TransmissionRatioInput
        {
            get
            {
                _transmissionRatioInput = Convert.ToUInt32(_currentAxis.CIPAxis.TransmissionRatioInput);
                return _transmissionRatioInput;
            }
            set
            {
                if (Convert.ToUInt32(_currentAxis.CIPAxis.TransmissionRatioInput) != value)
                {
                    _currentAxis.CIPAxis.TransmissionRatioInput = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private uint _transmissionRatioOutput;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(true)]
        [Category("Scaling")]
        public uint TransmissionRatioOutput
        {
            get
            {
                _transmissionRatioOutput = Convert.ToUInt32(_currentAxis.CIPAxis.TransmissionRatioOutput);
                return _transmissionRatioOutput;
            }
            set
            {
                if (Convert.ToUInt32(_currentAxis.CIPAxis.TransmissionRatioOutput) != value)
                {
                    _currentAxis.CIPAxis.TransmissionRatioOutput = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        private class Scaling
        {
            private readonly AxisCIPParameters _parameters;

            public Scaling(AxisCIPParameters parameters)
            {
                _parameters = parameters;
            }

            public void Refresh()
            {
                // update data source
                UpdateLoadTypeSource();

                AxisDefaultSetting.UpdateMotionUnit(_parameters._currentAxis.CIPAxis);

                UpdateTravelModeSource();


                // update Visibility
                UpdateVisibility();

                // update readonly
                UpdateReadonly();

                // update IsChanged
                UpdateIsChanged();

                // update unit
                UpdateUnit();
            }

            public void UpdateVisibility()
            {
                BaseRule.ApplyVisibilityRule(_parameters, GetVisibilityRules(_parameters));
            }

            public void UpdateReadonly()
            {
                BaseRule.ApplyReadonlyRule(_parameters, GetReadonlyRules(_parameters));
            }

            public void UpdateIsChanged()
            {
                var compareProperties = GetCompareProperties();

                foreach (var propertyName in compareProperties)
                {
                    var isChange =
                        !CipAttributeHelper.EqualByAttributeName(
                            _parameters._currentAxis.CIPAxis,
                            _parameters._originalAxis.CIPAxis,
                            propertyName);

                    PropertySetting.SetPropertyIsChanged(_parameters, propertyName, isChange);
                }
            }

            public void UpdateUnit()
            {
                var positionUnits = _parameters._currentAxis.CIPAxis.PositionUnits.GetString();
                // ConversionConstant
                PropertySetting.SetPropertyUnit(_parameters, "ConversionConstant", $"Motion Counts/{positionUnits}");
                // PositionScalingNumerator
                PropertySetting.SetPropertyUnit(_parameters, "PositionScalingNumerator", positionUnits);
                // PositionUnwindNumerator
                PropertySetting.SetPropertyUnit(_parameters, "PositionUnwindNumerator", positionUnits);
                // SoftTravelLimitNegative
                PropertySetting.SetPropertyUnit(_parameters, "SoftTravelLimitNegative", positionUnits);
                // SoftTravelLimitPositive
                PropertySetting.SetPropertyUnit(_parameters, "SoftTravelLimitPositive", positionUnits);
                // TravelRange
                PropertySetting.SetPropertyUnit(_parameters, "TravelRange", positionUnits);

                PropertySetting.SetPropertyUnit(_parameters, "BacklashCompensationWindow", positionUnits);
                PropertySetting.SetPropertyUnit(_parameters, "BacklashReversalOffset", positionUnits);

                PropertySetting.SetPropertyUnit(_parameters, "FrictionCompensationWindow", positionUnits);

                PropertySetting.SetPropertyUnit(_parameters, "PositionErrorTolerance", positionUnits);
                PropertySetting.SetPropertyUnit(_parameters, "PositionLockTolerance", positionUnits);

                PropertySetting.SetPropertyUnit(_parameters, "VelocityDroop", $"({positionUnits}/s)/% Rated");
                PropertySetting.SetPropertyUnit(_parameters, "VelocityErrorTolerance", $"{positionUnits}/s");
                PropertySetting.SetPropertyUnit(_parameters, "VelocityLimitNegative", $"{positionUnits}/s");
                PropertySetting.SetPropertyUnit(_parameters, "VelocityLimitPositive", $"{positionUnits}/s");
                PropertySetting.SetPropertyUnit(_parameters, "VelocityLockTolerance", $"{positionUnits}/s");
                PropertySetting.SetPropertyUnit(_parameters, "VelocityOffset", $"{positionUnits}/s");
                PropertySetting.SetPropertyUnit(_parameters, "SLATSetPoint", $"{positionUnits}/s");

                PropertySetting.SetPropertyUnit(_parameters, "AccelerationLimit", $"{positionUnits}/s^2");
                PropertySetting.SetPropertyUnit(_parameters, "DecelerationLimit", $"{positionUnits}/s^2");

                PropertySetting.SetPropertyUnit(_parameters, "MaximumAcceleration", $"{positionUnits}/s^2");
                PropertySetting.SetPropertyUnit(_parameters, "MaximumAccelerationJerk", $"{positionUnits}/s^3");
                PropertySetting.SetPropertyUnit(_parameters, "MaximumDeceleration", $"{positionUnits}/s^2");
                PropertySetting.SetPropertyUnit(_parameters, "MaximumDecelerationJerk", $"{positionUnits}/s^3");
                PropertySetting.SetPropertyUnit(_parameters, "MaximumSpeed", $"{positionUnits}/s");

                PropertySetting.SetPropertyUnit(_parameters, "HomeOffset", positionUnits);
                PropertySetting.SetPropertyUnit(_parameters, "HomePosition", positionUnits);
                PropertySetting.SetPropertyUnit(_parameters, "HomeReturnSpeed", $"{positionUnits}/s");
                PropertySetting.SetPropertyUnit(_parameters, "HomeSpeed", $"{positionUnits}/s");

                PropertySetting.SetPropertyUnit(_parameters, "BrakeSlipTolerance", positionUnits);
                PropertySetting.SetPropertyUnit(_parameters, "VelocityStandstillWindow", $"{positionUnits}/s");
                PropertySetting.SetPropertyUnit(_parameters, "VelocityThreshold", $"{positionUnits}/s");
            }

            private BaseRule[] GetVisibilityRules(AxisCIPParameters parameters)
            {
                var ruleList = new List<BaseRule>
                {
                    new BaseRule {Name = "FeedbackUnitRatio", Value = false},

                    new BaseRule {Name = nameof(_parameters.PositionUnwind), Value = false},
                    new BaseRule {Name = nameof(_parameters.PositionUnwindDenominator), Value = false},
                    new BaseRule {Name = nameof(_parameters.PositionUnwindNumerator), Value = false},
                    new BaseRule {Name = nameof(_parameters.SoftTravelLimitChecking), Value = false},
                    new BaseRule {Name = nameof(_parameters.SoftTravelLimitNegative), Value = false},
                    new BaseRule {Name = nameof(_parameters.SoftTravelLimitPositive), Value = false},
                    new BaseRule {Name = nameof(_parameters.TravelRange), Value = false},
                };

                var axisConfiguration =
                    (AxisConfigurationType)Convert.ToByte(_parameters._currentAxis.CIPAxis.AxisConfiguration);

                if (axisConfiguration == AxisConfigurationType.PositionLoop ||
                    axisConfiguration == AxisConfigurationType.VelocityLoop ||
                    axisConfiguration == AxisConfigurationType.TorqueLoop ||
                    axisConfiguration == AxisConfigurationType.FeedbackOnly)
                {
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.PositionUnwind), Value = true });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.PositionUnwindDenominator), Value = true });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.PositionUnwindNumerator), Value = true });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.SoftTravelLimitChecking), Value = true });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.SoftTravelLimitNegative), Value = true });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.SoftTravelLimitPositive), Value = true });
                    ruleList.Add(new BaseRule { Name = nameof(_parameters.TravelRange), Value = true });
                }

                var feedbackConfiguration =
                    (FeedbackConfigurationType) Convert.ToByte(parameters._currentAxis.CIPAxis.FeedbackConfiguration);

                if (feedbackConfiguration == FeedbackConfigurationType.LoadFeedback ||
                    feedbackConfiguration == FeedbackConfigurationType.DualFeedback)
                {
                    ruleList.Add(new BaseRule {Name = "FeedbackUnitRatio", Value = true});
                }


                return ruleList.ToArray();
            }

            private BaseRule[] GetReadonlyRules(AxisCIPParameters parameters)
            {
                List<BaseRule> ruleList;

                if (parameters._parentViewModel.IsPowerStructureEnabled || parameters._parentViewModel.IsHardRunMode)
                {
                    ruleList = new List<BaseRule>
                    {
                        new BaseRule {Name = "FeedbackUnitRatio", Value = true},
                        new BaseRule {Name = "LoadType", Value = true},
                        new BaseRule {Name = "PositionUnits", Value = true},
                        new BaseRule {Name = "TravelMode", Value = true},
                        new BaseRule {Name = "MotionScalingConfiguration", Value = true},
                        new BaseRule {Name = "ScalingSource", Value = true},
                        //
                        new BaseRule {Name = "ActuatorDiameter", Value = true},
                        new BaseRule {Name = "ActuatorDiameterUnit", Value = true},
                        new BaseRule {Name = "ActuatorLead", Value = true},
                        new BaseRule {Name = "ActuatorLeadUnit", Value = true},
                        new BaseRule {Name = "ActuatorType", Value = true},
                        new BaseRule {Name = "ConversionConstant", Value = true},
                        new BaseRule {Name = "MotionResolution", Value = true},
                        new BaseRule {Name = "MotionUnit", Value = true},
                        new BaseRule {Name = "PositionScalingDenominator", Value = true},
                        new BaseRule {Name = "PositionScalingNumerator", Value = true},
                        new BaseRule {Name = "PositionUnwind", Value = true},
                        new BaseRule {Name = "PositionUnwindDenominator", Value = true},
                        new BaseRule {Name = "PositionUnwindNumerator", Value = true},
                        new BaseRule {Name = "SoftTravelLimitChecking", Value = true},
                        new BaseRule {Name = "SoftTravelLimitNegative", Value = true},
                        new BaseRule {Name = "SoftTravelLimitPositive", Value = true},
                        new BaseRule {Name = "TransmissionRatioInput", Value = true},
                        new BaseRule {Name = "TransmissionRatioOutput", Value = true},
                        new BaseRule {Name = "TravelRange", Value = true}
                    };

                    return ruleList.ToArray();
                }

                ruleList = new List<BaseRule>
                {
                    new BaseRule {Name = "FeedbackUnitRatio", Value = false},
                    new BaseRule {Name = "LoadType", Value = false},
                    new BaseRule {Name = "PositionUnits", Value = false},
                    new BaseRule {Name = "TravelMode", Value = false},
                    new BaseRule {Name = "MotionScalingConfiguration", Value = false},
                    new BaseRule {Name = "ScalingSource", Value = false},
                    //
                    new BaseRule {Name = "ActuatorDiameter", Value = true},
                    new BaseRule {Name = "ActuatorDiameterUnit", Value = true},
                    new BaseRule {Name = "ActuatorLead", Value = true},
                    new BaseRule {Name = "ActuatorLeadUnit", Value = true},
                    new BaseRule {Name = "ActuatorType", Value = true},
                    new BaseRule {Name = "ConversionConstant", Value = true},
                    new BaseRule {Name = "MotionResolution", Value = true},
                    new BaseRule {Name = "MotionUnit", Value = true},
                    new BaseRule {Name = "PositionScalingDenominator", Value = true},
                    new BaseRule {Name = "PositionScalingNumerator", Value = true},
                    new BaseRule {Name = "PositionUnwind", Value = true},
                    new BaseRule {Name = "PositionUnwindDenominator", Value = true},
                    new BaseRule {Name = "PositionUnwindNumerator", Value = true},
                    new BaseRule {Name = "SoftTravelLimitChecking", Value = true},
                    new BaseRule {Name = "SoftTravelLimitNegative", Value = true},
                    new BaseRule {Name = "SoftTravelLimitPositive", Value = true},
                    new BaseRule {Name = "TransmissionRatioInput", Value = true},
                    new BaseRule {Name = "TransmissionRatioOutput", Value = true},
                    new BaseRule {Name = "TravelRange", Value = true}
                };

                // LoadType
                var loadType = parameters.LoadType;
                switch (loadType)
                {
                    case LoadType.DirectRotary:
                        break;
                    case LoadType.DirectLinear:
                        break;
                    case LoadType.RotaryTransmission:
                        ruleList.Add(new BaseRule {Name = "TransmissionRatioInput", Value = false});
                        ruleList.Add(new BaseRule {Name = "TransmissionRatioOutput", Value = false});
                        break;
                    case LoadType.LinearActuator:
                    {
                        ruleList.Add(new BaseRule {Name = "TransmissionRatioInput", Value = false});
                        ruleList.Add(new BaseRule {Name = "TransmissionRatioOutput", Value = false});
                        ruleList.Add(new BaseRule {Name = "ActuatorType", Value = false});

                        // ActuatorType
                        var actuatorType = parameters.ActuatorType;
                        switch (actuatorType)
                        {
                            case ActuatorType.None:
                                ruleList.Add(new BaseRule {Name = "ActuatorDiameter", Value = false});
                                ruleList.Add(new BaseRule {Name = "ActuatorDiameterUnit", Value = false});
                                break;
                            case ActuatorType.Screw:
                                ruleList.Add(new BaseRule {Name = "ActuatorLead", Value = false});
                                ruleList.Add(new BaseRule {Name = "ActuatorLeadUnit", Value = false});
                                break;
                            case ActuatorType.BeltAndPulley:
                                ruleList.Add(new BaseRule {Name = "ActuatorDiameter", Value = false});
                                ruleList.Add(new BaseRule {Name = "ActuatorDiameterUnit", Value = false});
                                break;
                            case ActuatorType.ChainAndSprocket:
                                ruleList.Add(new BaseRule {Name = "ActuatorDiameter", Value = false});
                                ruleList.Add(new BaseRule {Name = "ActuatorDiameterUnit", Value = false});
                                break;
                            case ActuatorType.RackAndPinion:
                                ruleList.Add(new BaseRule {Name = "ActuatorDiameter", Value = false});
                                ruleList.Add(new BaseRule {Name = "ActuatorDiameterUnit", Value = false});
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // ScalingSource
                var scalingSource = parameters.ScalingSource;
                switch (scalingSource)
                {
                    case ScalingSourceType.FromCalculator:
                        ruleList.Add(new BaseRule {Name = "PositionScalingDenominator", Value = false});
                        ruleList.Add(new BaseRule {Name = "PositionScalingNumerator", Value = false});
                        break;
                    case ScalingSourceType.DirectScalingFactorEntry:
                        ruleList.Add(new BaseRule {Name = "ConversionConstant", Value = false});
                        ruleList.Add(new BaseRule {Name = "MotionResolution", Value = false});
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // TravelMode
                var travelMode = parameters.TravelMode;
                switch (travelMode)
                {
                    case TravelModeType.Unlimited:
                        ruleList.Add(new BaseRule {Name = "SoftTravelLimitChecking", Value = false});
                        break;
                    case TravelModeType.Limited:
                        if (scalingSource == ScalingSourceType.FromCalculator)
                        {
                            ruleList.Add(new BaseRule {Name = "TravelRange", Value = false});
                        }

                        ruleList.Add(new BaseRule {Name = "SoftTravelLimitChecking", Value = false});
                        break;
                    case TravelModeType.Cyclic:
                        if (scalingSource == ScalingSourceType.FromCalculator)
                        {
                            ruleList.Add(new BaseRule {Name = "PositionUnwindDenominator", Value = false});
                            ruleList.Add(new BaseRule {Name = "PositionUnwindNumerator", Value = false});
                        }
                        else
                        {
                            ruleList.Add(new BaseRule {Name = "PositionUnwind", Value = false});
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // SoftTravelLimitChecking
                var softTravelLimitChecking = parameters.SoftTravelLimitChecking;
                if ((travelMode != TravelModeType.Cyclic) && (softTravelLimitChecking == BooleanTypeA.Yes))
                {
                    ruleList.Add(new BaseRule {Name = "SoftTravelLimitNegative", Value = false});
                    ruleList.Add(new BaseRule {Name = "SoftTravelLimitPositive", Value = false});
                }

                // online
                if (parameters._parentViewModel.IsOnLine)
                {
                    ruleList.Add(new BaseRule {Name = "FeedbackUnitRatio", Value = true});
                    ruleList.Add(new BaseRule {Name = "LoadType", Value = true});
                    ruleList.Add(new BaseRule {Name = "ScalingSource", Value = true});

                    ruleList.Add(new BaseRule {Name = "PositionScalingDenominator", Value = true});
                    ruleList.Add(new BaseRule {Name = "PositionScalingNumerator", Value = true});

                }

                return ruleList.ToArray();
            }

            public string[] GetCompareProperties()
            {
                string[] compareProperties =
                {
                    "ActuatorDiameter", "ActuatorDiameterUnit",
                    "ActuatorLead", "ActuatorLeadUnit", "ActuatorType", "ConversionConstant", "FeedbackUnitRatio",
                    "LoadType", "MotionResolution", "MotionScalingConfiguration",
                    "MotionUnit", "PositionScalingDenominator",
                    "PositionScalingNumerator", "PositionUnits", "PositionUnwind", "PositionUnwindDenominator",
                    "PositionUnwindNumerator", "ScalingSource",
                    "SoftTravelLimitChecking", "SoftTravelLimitNegative", "SoftTravelLimitPositive",
                    "TransmissionRatioInput", "TransmissionRatioOutput",
                    "TravelMode", "TravelRange"
                };
                return compareProperties;
            }

            private void UpdateLoadTypeSource()
            {
                var supportLoadTypes = new List<LoadType>();

                // TODO(gjc): need edit here
                var axisConfiguration =
                    (AxisConfigurationType) Convert.ToByte(_parameters._currentAxis.CIPAxis.AxisConfiguration);
                if (axisConfiguration == AxisConfigurationType.FeedbackOnly)
                {
                    var feedback1Unit =
                        (FeedbackUnitType) Convert.ToByte(_parameters._currentAxis.CIPAxis.Feedback1Unit);
                    if (feedback1Unit == FeedbackUnitType.Meter)
                    {
                        supportLoadTypes.Add(LoadType.DirectLinear);
                    }
                    else
                    {
                        supportLoadTypes.Add(LoadType.DirectRotary);
                        supportLoadTypes.Add(LoadType.RotaryTransmission);
                        supportLoadTypes.Add(LoadType.LinearActuator);
                    }
                }
                else
                {
                    var motorType = (MotorType) Convert.ToByte(_parameters._currentAxis.CIPAxis.MotorType);
                    if (motorType == MotorType.LinearPermanentMagnet)
                    {
                        supportLoadTypes.Add(LoadType.DirectLinear);
                    }
                    else
                    {
                        supportLoadTypes.Add(LoadType.DirectRotary);
                        supportLoadTypes.Add(LoadType.RotaryTransmission);
                        supportLoadTypes.Add(LoadType.LinearActuator);
                    }
                }

                var oldLoadType = (LoadType) Convert.ToByte(_parameters._currentAxis.CIPAxis.LoadType);

                _parameters.LoadTypeSource = new ObservableCollection<LoadType>(supportLoadTypes);

                if (!supportLoadTypes.Contains(oldLoadType))
                    _parameters.LoadType = supportLoadTypes[0];
                else
                {
                    _parameters._currentAxis.CIPAxis.LoadType = (byte) oldLoadType;
                    _parameters.OnPropertyChanged("LoadType");
                }

            }

            private void UpdateTravelModeSource()
            {
                var axisConfiguration =
                    (AxisConfigurationType)Convert.ToByte(_parameters._currentAxis.CIPAxis.AxisConfiguration);

                var supportLoadTypes = new List<TravelModeType>();

                if (_parameters.LoadType == LoadType.DirectLinear)
                    supportLoadTypes.Add(TravelModeType.Limited);
                else if (_parameters.LoadType == LoadType.LinearActuator
                         && (_parameters.ActuatorType == ActuatorType.Screw ||
                             _parameters.ActuatorType == ActuatorType.RackAndPinion))
                {
                    supportLoadTypes.Add(TravelModeType.Limited);
                }
                else
                {
                    supportLoadTypes.Add(TravelModeType.Unlimited);
                    supportLoadTypes.Add(TravelModeType.Limited);
                    supportLoadTypes.Add(TravelModeType.Cyclic);
                }

                if (axisConfiguration == AxisConfigurationType.FrequencyControl)
                {
                    supportLoadTypes = new List<TravelModeType> { TravelModeType.Unlimited };
                }

                var oldTravelMode = (TravelModeType) Convert.ToByte(_parameters._currentAxis.CIPAxis.TravelMode);

                _parameters.TravelModeSource = new ObservableCollection<TravelModeType>(supportLoadTypes);

                if (!supportLoadTypes.Contains(oldTravelMode))
                    _parameters.TravelMode = supportLoadTypes[0];
                else
                {
                    _parameters._currentAxis.CIPAxis.TravelMode = (byte) oldTravelMode;
                    _parameters.OnPropertyChanged("TravelMode");
                }
            }
        }

    }
}
