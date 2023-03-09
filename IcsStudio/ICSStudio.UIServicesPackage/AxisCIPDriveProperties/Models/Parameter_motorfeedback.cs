using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Controls.ExtendedPropertyGrid.Attributes;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Models
{
    // Motor Feedback,Master Feedback
    [SuppressMessage("ReSharper", "NotResolvedInText")]
    internal partial class AxisCIPParameters
    {
        private readonly MotorFeedback _motorFeedback;


        #region Public Property

        // motor and maser
        //Feedback1AccelFilterBandwidth
        //Feedback1AccelFilterTaps
        //Feedback1CycleInterpolation
        //Feedback1CycleResolution
        //Feedback1StartupMethod
        //Feedback1Length
        //Feedback1Turns
        //Feedback1Type
        //Feedback1Unit
        //Feedback1VelocityFilterBandwidth
        //Feedback1VelocityFilterTaps

        // motor
        //CommutationOffset
        //CommutationPolarity
        //FeedbackCommutationAligned

        private float _commutationOffset;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Degrees")]
        [ReadOnly(true)]
        [Category("Motor Feedback")]
        public float CommutationOffset
        {
            get
            {
                _commutationOffset = Convert.ToSingle(_currentAxis.CIPAxis.CommutationOffset);
                return _commutationOffset;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.CommutationOffset) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.CommutationOffset = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private PolarityType _commutationPolarity;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Motor Feedback")]
        public PolarityType CommutationPolarity
        {
            get
            {
                _commutationPolarity = (PolarityType) Convert.ToByte(_currentAxis.CIPAxis.CommutationPolarity);
                return _commutationPolarity;
            }
            set
            {
                var commutationPolarity = (PolarityType) Convert.ToByte(_currentAxis.CIPAxis.CommutationPolarity);
                if (commutationPolarity != value)
                {
                    _currentAxis.CIPAxis.CommutationPolarity = (byte) value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private FeedbackCommutationAlignedType _feedbackCommutationAligned;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Motor Feedback")]
        public FeedbackCommutationAlignedType FeedbackCommutationAligned
        {
            get
            {
                _feedbackCommutationAligned =
                    (FeedbackCommutationAlignedType) Convert.ToByte(_currentAxis.CIPAxis.FeedbackCommutationAligned);
                return _feedbackCommutationAligned;
            }
            set
            {
                var feedbackCommutationAligned =
                    (FeedbackCommutationAlignedType) Convert.ToByte(_currentAxis.CIPAxis.FeedbackCommutationAligned);
                if (feedbackCommutationAligned != value)
                {
                    _currentAxis.CIPAxis.FeedbackCommutationAligned = (byte) value;

                    _motorFeedback.UpdateReadonly();

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _feedback1AccelFilterBandwidth;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Hz")]
        [ReadOnly(false)]
        [Category("Motor Feedback,Master Feedback")]
        public float Feedback1AccelFilterBandwidth
        {
            get
            {
                _feedback1AccelFilterBandwidth = Convert.ToSingle(_currentAxis.CIPAxis.Feedback1AccelFilterBandwidth);
                return _feedback1AccelFilterBandwidth;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.Feedback1AccelFilterBandwidth) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.Feedback1AccelFilterBandwidth = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private ushort _feedback1AccelFilterTaps;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("# of Delay Taps")]
        [ReadOnly(false)]
        [Category("Motor Feedback,Master Feedback")]
        public ushort Feedback1AccelFilterTaps
        {
            get
            {
                _feedback1AccelFilterTaps = Convert.ToUInt16(_currentAxis.CIPAxis.Feedback1AccelFilterTaps);
                return _feedback1AccelFilterTaps;
            }
            set
            {
                if (Convert.ToUInt16(_currentAxis.CIPAxis.Feedback1AccelFilterTaps) != value)
                {
                    _currentAxis.CIPAxis.Feedback1AccelFilterTaps = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private uint _feedback1CycleInterpolation;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Feedback Counts/Feedback Cycle")]
        [ReadOnly(false)]
        [Category("Motor Feedback,Master Feedback")]
        public uint Feedback1CycleInterpolation
        {
            get
            {
                _feedback1CycleInterpolation = Convert.ToUInt32(_currentAxis.CIPAxis.Feedback1CycleInterpolation);
                return _feedback1CycleInterpolation;
            }
            set
            {
                if (Convert.ToUInt32(_currentAxis.CIPAxis.Feedback1CycleInterpolation) != value)
                {
                    _currentAxis.CIPAxis.Feedback1CycleInterpolation = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private uint _feedback1CycleResolution;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Feedback Cycles/Rev")]
        [ReadOnly(false)]
        [Category("Motor Feedback,Master Feedback")]
        public uint Feedback1CycleResolution
        {
            get
            {
                _feedback1CycleResolution = Convert.ToUInt32(_currentAxis.CIPAxis.Feedback1CycleResolution);
                return _feedback1CycleResolution;
            }
            set
            {
                if (Convert.ToUInt32(_currentAxis.CIPAxis.Feedback1CycleResolution) != value)
                {
                    _currentAxis.CIPAxis.Feedback1CycleResolution = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private FeedbackStartupMethodType _feedback1StartupMethod;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Motor Feedback,Master Feedback")]
        public FeedbackStartupMethodType Feedback1StartupMethod
        {
            get
            {
                _feedback1StartupMethod =
                    (FeedbackStartupMethodType) Convert.ToByte(_currentAxis.CIPAxis.Feedback1StartupMethod);
                return _feedback1StartupMethod;
            }
            set
            {
                var feedback1StartupMethod =
                    (FeedbackStartupMethodType) Convert.ToByte(_currentAxis.CIPAxis.Feedback1StartupMethod);
                if (feedback1StartupMethod != value)
                {
                    _currentAxis.CIPAxis.Feedback1StartupMethod = (byte) value;

                    _motorFeedback.UpdateVisibility();

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private uint _feedback1Turns;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Rev")]
        [ReadOnly(false)]
        [Category("Motor Feedback,Master Feedback")]
        public uint Feedback1Turns
        {
            get
            {
                _feedback1Turns = Convert.ToUInt32(_currentAxis.CIPAxis.Feedback1Turns);
                return _feedback1Turns;
            }
            set
            {
                if (Convert.ToUInt32(_currentAxis.CIPAxis.Feedback1Turns) != value)
                {
                    _currentAxis.CIPAxis.Feedback1Turns = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _feedback1Length;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("m")]
        [ReadOnly(false)]
        [Category("Motor Feedback,Master Feedback")]
        public float Feedback1Length
        {
            get
            {
                _feedback1Length = Convert.ToSingle(_currentAxis.CIPAxis.Feedback1Length);
                return _feedback1Length;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.Feedback1Length) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.Feedback1Length = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private FeedbackType _feedback1Type;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Motor Feedback,Master Feedback")]
        public FeedbackType Feedback1Type
        {
            get
            {
                _feedback1Type = (FeedbackType) Convert.ToByte(_currentAxis.CIPAxis.Feedback1Type);
                return _feedback1Type;
            }
            set
            {
                var feedback1Type = (FeedbackType) Convert.ToByte(_currentAxis.CIPAxis.Feedback1Type);
                var feedback1Unit = (FeedbackUnitType) Convert.ToByte(_currentAxis.CIPAxis.Feedback1Unit);
                if (feedback1Type != value)
                {
                    _currentAxis.CIPAxis.Feedback1Type = (byte) value;

                    AxisDefaultSetting.LoadDefaultFeedback1Setting(_currentAxis.CIPAxis, value, feedback1Unit);

                    _motorFeedback.UpdateVisibility();
                    _motorFeedback.UpdateReadonly();

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private FeedbackUnitType _feedback1Unit;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Motor Feedback,Master Feedback")]
        public FeedbackUnitType Feedback1Unit
        {
            get
            {
                _feedback1Unit = (FeedbackUnitType) Convert.ToByte(_currentAxis.CIPAxis.Feedback1Unit);
                return _feedback1Unit;
            }
            set
            {
                var feedback1Unit = (FeedbackUnitType) Convert.ToByte(_currentAxis.CIPAxis.Feedback1Unit);
                if (feedback1Unit != value)
                {
                    _currentAxis.CIPAxis.Feedback1Unit = (byte) value;

                    _motorFeedback.UpdateVisibility();
                    _motorFeedback.UpdateReadonly();
                    _motorFeedback.UpdateUnit();

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _feedback1VelocityFilterBandwidth;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Hz")]
        [ReadOnly(false)]
        [Category("Motor Feedback,Master Feedback")]
        public float Feedback1VelocityFilterBandwidth
        {
            get
            {
                _feedback1VelocityFilterBandwidth =
                    Convert.ToSingle(_currentAxis.CIPAxis.Feedback1VelocityFilterBandwidth);
                return _feedback1VelocityFilterBandwidth;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.Feedback1VelocityFilterBandwidth) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.Feedback1VelocityFilterBandwidth = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private ushort _feedback1VelocityFilterTaps;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("# of Delay Taps")]
        [ReadOnly(false)]
        [Category("Motor Feedback,Master Feedback")]
        public ushort Feedback1VelocityFilterTaps
        {
            get
            {
                _feedback1VelocityFilterTaps = Convert.ToUInt16(_currentAxis.CIPAxis.Feedback1VelocityFilterTaps);
                return _feedback1VelocityFilterTaps;
            }
            set
            {
                if (Convert.ToUInt16(_currentAxis.CIPAxis.Feedback1VelocityFilterTaps) != value)
                {
                    _currentAxis.CIPAxis.Feedback1VelocityFilterTaps = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Enum

        private ObservableCollection<PolarityType> CommutationPolaritySource { get; set; }
            = new ObservableCollection<PolarityType>
            {
                PolarityType.Normal,
                PolarityType.Inverted
            };

        private ObservableCollection<FeedbackCommutationAlignedType> FeedbackCommutationAlignedSource { get; set; }

        private ObservableCollection<FeedbackStartupMethodType> Feedback1StartupMethodSource { get; set; }

        private ObservableCollection<FeedbackType> Feedback1TypeSource { get; set; }

        private ObservableCollection<FeedbackUnitType> Feedback1UnitSource { get; set; }

        #endregion

        private class MotorFeedback
        {
            private readonly AxisCIPParameters _parameters;

            public MotorFeedback(AxisCIPParameters parameters)
            {
                _parameters = parameters;
            }

            public void Refresh()
            {
                UpdateFeedback1StartupMethodSource();
                UpdateFeedback1TypeSource();
                UpdateFeedback1UnitSource();
                UpdateFeedbackCommutationAlignedSource();

                UpdateVisibility();
                UpdateReadonly();
                UpdateIsChanged();
                UpdateUnit();
            }

            public void UpdateUnit()
            {
                var feedback1Unit = (FeedbackUnitType) Convert.ToByte(_parameters._currentAxis.CIPAxis.Feedback1Unit);
                switch (feedback1Unit)
                {
                    case FeedbackUnitType.Rev:
                        PropertySetting.SetPropertyUnit(_parameters, "Feedback1CycleResolution", "Feedback Cycles/Rev");
                        break;
                    case FeedbackUnitType.Meter:
                        PropertySetting.SetPropertyUnit(_parameters, "Feedback1CycleResolution",
                            "Feedback Cycles/Meter");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
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

            private void UpdateFeedback1StartupMethodSource()
            {
                var supportTypes = new List<FeedbackStartupMethodType>
                {
                    FeedbackStartupMethodType.Incremental,
                    FeedbackStartupMethodType.Absolute
                };

                var oldFeedback1StartupMethod =
                    (FeedbackStartupMethodType) Convert.ToByte(_parameters._currentAxis.CIPAxis.Feedback1StartupMethod);

                _parameters.Feedback1StartupMethodSource =
                    new ObservableCollection<FeedbackStartupMethodType>(supportTypes);

                if (!supportTypes.Contains(oldFeedback1StartupMethod))
                    _parameters._currentAxis.CIPAxis.Feedback1StartupMethod = (byte) supportTypes[0];
                else
                    _parameters._currentAxis.CIPAxis.Feedback1StartupMethod = (byte) oldFeedback1StartupMethod;

                _parameters.OnPropertyChanged("Feedback1StartupMethod");

            }

            private void UpdateFeedback1TypeSource()
            {
                IList supportFeedbackTypes = null;

                var cipMotionDrive = _parameters._currentAxis.AssociatedModule as CIPMotionDrive;
                if (cipMotionDrive != null)
                    supportFeedbackTypes = cipMotionDrive.GetSupportFeedback1Types(_parameters._currentAxis.AxisNumber);

                if (supportFeedbackTypes == null)
                    supportFeedbackTypes = new List<FeedbackType>();

                if (!supportFeedbackTypes.Contains(FeedbackType.NotSpecified))
                    supportFeedbackTypes.Insert(0, FeedbackType.NotSpecified);

                var oldFeedback1Type = (FeedbackType) Convert.ToByte(_parameters._currentAxis.CIPAxis.Feedback1Type);

                ((List<FeedbackType>) supportFeedbackTypes).Sort();
                _parameters.Feedback1TypeSource =
                    new ObservableCollection<FeedbackType>((List<FeedbackType>) supportFeedbackTypes);

                if (!supportFeedbackTypes.Contains(oldFeedback1Type))
                    _parameters.Feedback1Type = ((List<FeedbackType>) supportFeedbackTypes)[0];
                else
                {
                    _parameters._currentAxis.CIPAxis.Feedback1Type = (byte) oldFeedback1Type;
                    _parameters.OnPropertyChanged("Feedback1Type");
                }
            }

            private void UpdateFeedback1UnitSource()
            {
                var supportTypes = new List<FeedbackUnitType>();
                var motorType = (MotorType) Convert.ToByte(_parameters._currentAxis.CIPAxis.MotorType);
                switch (motorType)
                {
                    case MotorType.NotSpecified:
                        supportTypes.Add(FeedbackUnitType.Rev);
                        supportTypes.Add(FeedbackUnitType.Meter);
                        break;
                    case MotorType.RotaryPermanentMagnet:
                        supportTypes.Add(FeedbackUnitType.Rev);
                        break;
                    case MotorType.RotaryInduction:
                        supportTypes.Add(FeedbackUnitType.Rev);
                        break;
                    case MotorType.LinearPermanentMagnet:
                        supportTypes.Add(FeedbackUnitType.Meter);
                        break;
                    case MotorType.LinearInduction:
                        supportTypes.Add(FeedbackUnitType.Meter);
                        break;
                    case MotorType.RotaryInteriorPermanentMagnet:
                        supportTypes.Add(FeedbackUnitType.Rev);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var oldFeedback1Unit =
                    (FeedbackUnitType) Convert.ToByte(_parameters._currentAxis.CIPAxis.Feedback1Unit);

                _parameters.Feedback1UnitSource = new ObservableCollection<FeedbackUnitType>(supportTypes);

                if (!supportTypes.Contains(oldFeedback1Unit))
                    _parameters.Feedback1Unit = supportTypes[0];
                else
                {
                    _parameters._currentAxis.CIPAxis.Feedback1Unit = (byte) oldFeedback1Unit;
                    _parameters.OnPropertyChanged("Feedback1Unit");
                }
            }

            private void UpdateFeedbackCommutationAlignedSource()
            {
                var supportTypes = new List<FeedbackCommutationAlignedType>
                {
                    FeedbackCommutationAlignedType.NotAligned,
                    FeedbackCommutationAlignedType.ControllerOffset,
                    FeedbackCommutationAlignedType.MotorOffset
                };

                var oldFeedbackCommutationAligned =
                    (FeedbackCommutationAlignedType) Convert.ToByte(_parameters._currentAxis.CIPAxis
                        .FeedbackCommutationAligned);

                _parameters.FeedbackCommutationAlignedSource =
                    new ObservableCollection<FeedbackCommutationAlignedType>(supportTypes);

                if (!supportTypes.Contains(oldFeedbackCommutationAligned))
                    _parameters.FeedbackCommutationAligned = supportTypes[0];
                else
                {
                    _parameters._currentAxis.CIPAxis.FeedbackCommutationAligned = (byte) oldFeedbackCommutationAligned;
                    _parameters.OnPropertyChanged("FeedbackCommutationAligned");
                }
            }

            private BaseRule[] GetVisibilityRules(AxisCIPParameters parameters)
            {
                var ruleList = new List<BaseRule>
                {
                    new BaseRule {Name = "CommutationOffset", Value = false},
                    new BaseRule {Name = "CommutationPolarity", Value = false},
                    new BaseRule {Name = "FeedbackCommutationAligned", Value = false},
                    new BaseRule {Name = "Feedback1AccelFilterBandwidth", Value = false},
                    new BaseRule {Name = "Feedback1AccelFilterTaps", Value = false},
                    new BaseRule {Name = "Feedback1CycleInterpolation", Value = false},
                    new BaseRule {Name = "Feedback1CycleResolution", Value = false},
                    new BaseRule {Name = "Feedback1StartupMethod", Value = false},
                    new BaseRule {Name = "Feedback1Turns", Value = false},
                    new BaseRule {Name = "Feedback1Length", Value = false},
                    new BaseRule {Name = "Feedback1Type", Value = false},
                    new BaseRule {Name = "Feedback1Unit", Value = false},
                    new BaseRule {Name = "Feedback1VelocityFilterBandwidth", Value = false},
                    new BaseRule {Name = "Feedback1VelocityFilterTaps", Value = false}
                };

                var axisConfiguration =
                    (AxisConfigurationType) Convert.ToByte(parameters._currentAxis.CIPAxis.AxisConfiguration);

                if ((axisConfiguration == AxisConfigurationType.PositionLoop)
                    || (axisConfiguration == AxisConfigurationType.VelocityLoop)
                    || (axisConfiguration == AxisConfigurationType.TorqueLoop)
                    || (axisConfiguration == AxisConfigurationType.FeedbackOnly))
                {
                    ruleList.Add(new BaseRule {Name = "Feedback1Type", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback1Unit", Value = true});

                    var feedback1Type = parameters.Feedback1Type;
                    if (feedback1Type != FeedbackType.NotSpecified)
                    {
                        ruleList.Add(new BaseRule {Name = "Feedback1AccelFilterBandwidth", Value = true});
                        ruleList.Add(new BaseRule {Name = "Feedback1AccelFilterTaps", Value = true});
                        ruleList.Add(new BaseRule {Name = "Feedback1CycleInterpolation", Value = true});
                        ruleList.Add(new BaseRule {Name = "Feedback1CycleResolution", Value = true});
                        ruleList.Add(new BaseRule {Name = "Feedback1StartupMethod", Value = true});
                        ruleList.Add(new BaseRule {Name = "Feedback1VelocityFilterBandwidth", Value = true});
                        ruleList.Add(new BaseRule {Name = "Feedback1VelocityFilterTaps", Value = true});

                        var feedback1StartupMethod = parameters.Feedback1StartupMethod;
                        var feedback1Unit = parameters.Feedback1Unit;
                        if (feedback1StartupMethod == FeedbackStartupMethodType.Absolute)
                            if (feedback1Unit == FeedbackUnitType.Rev)
                                ruleList.Add(new BaseRule {Name = "Feedback1Turns", Value = true});
                            else if (feedback1Unit == FeedbackUnitType.Meter)
                                ruleList.Add(new BaseRule {Name = "Feedback1Length", Value = true});

                        //
                        if (axisConfiguration != AxisConfigurationType.FeedbackOnly)
                        {
                            var motorType = (MotorType) Convert.ToByte(parameters._currentAxis.CIPAxis.MotorType);
                            if (motorType != MotorType.NotSpecified)
                            {
                                if (feedback1Type != FeedbackType.DigitalAqB)
                                {
                                    ruleList.Add(new BaseRule {Name = "CommutationOffset", Value = true});
                                    ruleList.Add(new BaseRule {Name = "FeedbackCommutationAligned", Value = true});

                                    if (parameters._currentAxis.SupportAttribute("CommutationPolarity"))
                                        ruleList.Add(new BaseRule {Name = "CommutationPolarity", Value = true});
                                }
                            }
                        }

                    }
                }

                return ruleList.ToArray();
            }

            private BaseRule[] GetReadonlyRules(AxisCIPParameters parameters)
            {
                var ruleList = new List<BaseRule>();

                if (parameters._parentViewModel.IsPowerStructureEnabled || parameters._parentViewModel.IsHardRunMode)
                {
                    ruleList.Add(new BaseRule {Name = "CommutationOffset", Value = true});
                    ruleList.Add(new BaseRule {Name = "CommutationPolarity", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback1AccelFilterBandwidth", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback1AccelFilterTaps", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback1CycleInterpolation", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback1CycleResolution", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback1StartupMethod", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback1Turns", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback1Length", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback1Type", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback1Unit", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback1VelocityFilterBandwidth", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback1VelocityFilterTaps", Value = true});
                    ruleList.Add(new BaseRule {Name = "FeedbackCommutationAligned", Value = true});
                }
                else
                {
                    ruleList.Add(new BaseRule {Name = "CommutationOffset", Value = true});
                    ruleList.Add(new BaseRule {Name = "CommutationPolarity", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback1AccelFilterBandwidth", Value = false});
                    ruleList.Add(new BaseRule {Name = "Feedback1AccelFilterTaps", Value = false});
                    ruleList.Add(new BaseRule {Name = "Feedback1CycleInterpolation", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback1CycleResolution", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback1StartupMethod", Value = false});
                    ruleList.Add(new BaseRule {Name = "Feedback1Turns", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback1Length", Value = false});
                    ruleList.Add(new BaseRule {Name = "Feedback1Type", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback1Unit", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback1VelocityFilterBandwidth", Value = false});
                    ruleList.Add(new BaseRule {Name = "Feedback1VelocityFilterTaps", Value = false});
                    ruleList.Add(new BaseRule {Name = "FeedbackCommutationAligned", Value = false});

                    if (parameters._parentViewModel.IsOnLine)
                    {
                        ruleList.Add(new BaseRule {Name = "Feedback1AccelFilterBandwidth", Value = true});
                        ruleList.Add(new BaseRule {Name = "Feedback1AccelFilterTaps", Value = true});
                        ruleList.Add(new BaseRule {Name = "Feedback1StartupMethod", Value = true});
                        ruleList.Add(new BaseRule {Name = "Feedback1VelocityFilterBandwidth", Value = true});
                        ruleList.Add(new BaseRule {Name = "Feedback1VelocityFilterTaps", Value = true});
                        ruleList.Add(new BaseRule {Name = "FeedbackCommutationAligned", Value = true});

                        return ruleList.ToArray();
                    }


                    // FeedbackCommutationAligned
                    var feedbackCommutationAligned = parameters.FeedbackCommutationAligned;
                    switch (feedbackCommutationAligned)
                    {
                        case FeedbackCommutationAlignedType.NotAligned:
                            break;
                        case FeedbackCommutationAlignedType.ControllerOffset:
                            ruleList.Add(new BaseRule {Name = "CommutationOffset", Value = false});
                            break;
                        case FeedbackCommutationAlignedType.MotorOffset:
                            break;
                        case FeedbackCommutationAlignedType.SelfSense:
                            break;
                        case FeedbackCommutationAlignedType.DatabaseOffset:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    // DataSource
                    var motorDataSource =
                        (MotorDataSourceType) Convert.ToByte(parameters._currentAxis.CIPAxis.MotorDataSource);

                    switch (motorDataSource)
                    {
                        case MotorDataSourceType.Datasheet:
                            ruleList.Add(new BaseRule {Name = "Feedback1CycleInterpolation", Value = false});
                            ruleList.Add(new BaseRule {Name = "Feedback1CycleResolution", Value = false});
                            ruleList.Add(new BaseRule {Name = "Feedback1Turns", Value = false});
                            ruleList.Add(new BaseRule {Name = "Feedback1Type", Value = false});
                            ruleList.Add(new BaseRule {Name = "Feedback1Unit", Value = false});
                            ruleList.Add(new BaseRule {Name = "CommutationPolarity", Value = false});
                            break;
                        case MotorDataSourceType.Database:
                            break;
                        case MotorDataSourceType.DriveNV:
                            break;
                        case MotorDataSourceType.MotorNV:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                return ruleList.ToArray();
            }

            public string[] GetCompareProperties()
            {
                string[] compareProperties =
                {
                    "Feedback1AccelFilterBandwidth",
                    "Feedback1AccelFilterTaps", "Feedback1CycleInterpolation",
                    "Feedback1CycleResolution", "Feedback1StartupMethod",
                    "Feedback1Length",
                    "Feedback1Turns", "Feedback1Type",
                    "Feedback1Unit", "Feedback1VelocityFilterBandwidth",
                    "Feedback1VelocityFilterTaps",

                    "CommutationOffset",
                    "CommutationPolarity",
                    "FeedbackCommutationAligned"
                };
                return compareProperties;
            }
        }
    }
}
