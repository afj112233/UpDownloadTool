using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Controls.ExtendedPropertyGrid.Attributes;
using ICSStudio.SimpleServices.DeviceModule;


namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Models
{
    [SuppressMessage("ReSharper", "NotResolvedInText")]
    internal partial class AxisCIPParameters
    {
        private readonly LoadFeedback _loadFeedback;

        #region Public Property

        private float _feedback2AccelFilterBandwidth;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Hz")]
        [ReadOnly(false)]
        [Category("Load Feedback")]
        public float Feedback2AccelFilterBandwidth
        {
            get
            {
                _feedback2AccelFilterBandwidth = Convert.ToSingle(_currentAxis.CIPAxis.Feedback2AccelFilterBandwidth);
                return _feedback2AccelFilterBandwidth;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.Feedback2AccelFilterBandwidth) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.Feedback2AccelFilterBandwidth = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private ushort _feedback2AccelFilterTaps;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("# of Delay Taps")]
        [ReadOnly(false)]
        [Category("Load Feedback")]
        public ushort Feedback2AccelFilterTaps
        {
            get
            {
                _feedback2AccelFilterTaps = Convert.ToUInt16(_currentAxis.CIPAxis.Feedback2AccelFilterTaps);
                return _feedback2AccelFilterTaps;
            }
            set
            {
                if (Convert.ToUInt16(_currentAxis.CIPAxis.Feedback2AccelFilterTaps) != value)
                {
                    _currentAxis.CIPAxis.Feedback2AccelFilterTaps = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private uint _feedback2CycleInterpolation;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Feedback Counts/Feedback Cycle")]
        [ReadOnly(false)]
        [Category("Load Feedback")]
        public uint Feedback2CycleInterpolation
        {
            get
            {
                _feedback2CycleInterpolation = Convert.ToUInt32(_currentAxis.CIPAxis.Feedback2CycleInterpolation);
                return _feedback2CycleInterpolation;
            }
            set
            {
                if (Convert.ToUInt32(_currentAxis.CIPAxis.Feedback2CycleInterpolation) != value)
                {
                    _currentAxis.CIPAxis.Feedback2CycleInterpolation = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }

            }
        }

        private uint _feedback2CycleResolution;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Feedback Cycles/Rev")]
        [ReadOnly(false)]
        [Category("Load Feedback")]
        public uint Feedback2CycleResolution
        {
            get
            {
                _feedback2CycleResolution = Convert.ToUInt32(_currentAxis.CIPAxis.Feedback2CycleResolution);
                return _feedback2CycleResolution;
            }
            set
            {
                if (Convert.ToUInt32(_currentAxis.CIPAxis.Feedback2CycleResolution) != value)
                {
                    _currentAxis.CIPAxis.Feedback2CycleResolution = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private FeedbackStartupMethodType _feedback2StartupMethod;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Load Feedback")]
        public FeedbackStartupMethodType Feedback2StartupMethod
        {
            get
            {
                _feedback2StartupMethod =
                    (FeedbackStartupMethodType) Convert.ToByte(_currentAxis.CIPAxis.Feedback2StartupMethod);
                return _feedback2StartupMethod;
            }
            set
            {
                var feedback2StartupMethod =
                    (FeedbackStartupMethodType) Convert.ToByte(_currentAxis.CIPAxis.Feedback2StartupMethod);
                if (feedback2StartupMethod != value)
                {
                    _currentAxis.CIPAxis.Feedback2StartupMethod = (byte) value;

                    _loadFeedback.UpdateVisibility();

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private uint _feedback2Turns;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Rev")]
        [ReadOnly(false)]
        [Category("Load Feedback")]
        public uint Feedback2Turns
        {
            get
            {
                _feedback2Turns = Convert.ToUInt32(_currentAxis.CIPAxis.Feedback2Turns);
                return _feedback2Turns;
            }
            set
            {
                if (Convert.ToUInt32(_currentAxis.CIPAxis.Feedback2Turns) != value)
                {
                    _currentAxis.CIPAxis.Feedback2Turns = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _feedback2Length;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("m")]
        [ReadOnly(false)]
        [Category("Load Feedback")]
        public float Feedback2Length
        {
            get
            {
                _feedback2Length = Convert.ToSingle(_currentAxis.CIPAxis.Feedback2Length);
                return _feedback2Length;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.Feedback2Length) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.Feedback2Length = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private FeedbackType _feedback2Type;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Load Feedback")]
        public FeedbackType Feedback2Type
        {
            get
            {
                _feedback2Type = (FeedbackType) Convert.ToByte(_currentAxis.CIPAxis.Feedback2Type);
                return _feedback2Type;
            }
            set
            {
                var feedback2Type = (FeedbackType) Convert.ToByte(_currentAxis.CIPAxis.Feedback2Type);
                var feedback2Unit = (FeedbackUnitType) Convert.ToByte(_currentAxis.CIPAxis.Feedback2Unit);
                if (feedback2Type != value)
                {
                    _currentAxis.CIPAxis.Feedback2Type = (byte) value;

                    AxisDefaultSetting.LoadDefaultFeedback2Setting(_currentAxis.CIPAxis, value, feedback2Unit);

                    _loadFeedback.UpdateVisibility();
                    _loadFeedback.UpdateReadonly();

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private FeedbackUnitType _feedback2Unit;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Load Feedback")]
        public FeedbackUnitType Feedback2Unit
        {
            get
            {
                _feedback2Unit = (FeedbackUnitType) Convert.ToByte(_currentAxis.CIPAxis.Feedback2Unit);
                return _feedback2Unit;
            }
            set
            {
                var feedback2Unit = (FeedbackUnitType) Convert.ToByte(_currentAxis.CIPAxis.Feedback2Unit);
                if (feedback2Unit != value)
                {
                    _currentAxis.CIPAxis.Feedback2Unit = (byte) value;

                    _loadFeedback.UpdateVisibility();
                    _loadFeedback.UpdateReadonly();
                    _loadFeedback.UpdateUnit();

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _feedback2VelocityFilterBandwidth;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Hz")]
        [ReadOnly(false)]
        [Category("Load Feedback")]
        public float Feedback2VelocityFilterBandwidth
        {
            get
            {
                _feedback2VelocityFilterBandwidth =
                    Convert.ToSingle(_currentAxis.CIPAxis.Feedback2VelocityFilterBandwidth);
                return _feedback2VelocityFilterBandwidth;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.Feedback2VelocityFilterBandwidth) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.Feedback2VelocityFilterBandwidth = value;

                    UpdateIsChanged();
                    OnPropertyChanged();

                }
            }
        }

        private ushort _feedback2VelocityFilterTaps;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("# of Delay Taps")]
        [ReadOnly(false)]
        [Category("Load Feedback")]
        public ushort Feedback2VelocityFilterTaps
        {
            get
            {
                _feedback2VelocityFilterTaps = Convert.ToUInt16(_currentAxis.CIPAxis.Feedback2VelocityFilterTaps);
                return _feedback2VelocityFilterTaps;
            }
            set
            {
                if (Convert.ToUInt16(_currentAxis.CIPAxis.Feedback2VelocityFilterTaps) != value)
                {
                    _currentAxis.CIPAxis.Feedback2VelocityFilterTaps = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Enum

        private ObservableCollection<FeedbackStartupMethodType> Feedback2StartupMethodSource { get; set; }
        private ObservableCollection<FeedbackType> Feedback2TypeSource { get; set; }
        private ObservableCollection<FeedbackUnitType> Feedback2UnitSource { get; set; }

        #endregion

        private class LoadFeedback
        {
            private readonly AxisCIPParameters _parameters;

            public LoadFeedback(AxisCIPParameters parameters)
            {
                _parameters = parameters;
            }

            public void Refresh()
            {
                UpdateFeedback2StartupMethodSource();
                UpdateFeedback2TypeSource();
                UpdateFeedback2UnitSource();

                UpdateVisibility();
                UpdateReadonly();
                UpdateIsChanged();
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
                var feedback2Unit = (FeedbackUnitType) Convert.ToByte(_parameters._currentAxis.CIPAxis.Feedback2Unit);
                switch (feedback2Unit)
                {
                    case FeedbackUnitType.Rev:
                        PropertySetting.SetPropertyUnit(_parameters, "Feedback2CycleResolution", "Feedback Cycles/Rev");
                        break;
                    case FeedbackUnitType.Meter:
                        PropertySetting.SetPropertyUnit(_parameters, "Feedback2CycleResolution",
                            "Feedback Cycles/Meter");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private BaseRule[] GetVisibilityRules(AxisCIPParameters parameters)
            {
                var ruleList = new List<BaseRule>
                {
                    new BaseRule {Name = "Feedback2AccelFilterBandwidth", Value = false},
                    new BaseRule {Name = "Feedback2AccelFilterTaps", Value = false},
                    new BaseRule {Name = "Feedback2CycleInterpolation", Value = false},
                    new BaseRule {Name = "Feedback2CycleResolution", Value = false},
                    new BaseRule {Name = "Feedback2StartupMethod", Value = false},
                    new BaseRule {Name = "Feedback2Turns", Value = false},
                    new BaseRule {Name = "Feedback2Length", Value = false},
                    new BaseRule {Name = "Feedback2Type", Value = false},
                    new BaseRule {Name = "Feedback2Unit", Value = false},
                    new BaseRule {Name = "Feedback2VelocityFilterBandwidth", Value = false},
                    new BaseRule {Name = "Feedback2VelocityFilterTaps", Value = false}
                };

                var feedbackConfiguration =
                    (FeedbackConfigurationType) Convert.ToByte(parameters._currentAxis.CIPAxis
                        .FeedbackConfiguration);
                if (feedbackConfiguration == FeedbackConfigurationType.LoadFeedback ||
                    feedbackConfiguration == FeedbackConfigurationType.DualFeedback)
                {
                    ruleList.Add(new BaseRule {Name = "Feedback2Type", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback2Unit", Value = true});

                    var feedback2Type = parameters.Feedback2Type;
                    if (feedback2Type != FeedbackType.NotSpecified)
                    {
                        ruleList.Add(new BaseRule {Name = "Feedback2AccelFilterBandwidth", Value = true});
                        ruleList.Add(new BaseRule {Name = "Feedback2AccelFilterTaps", Value = true});
                        ruleList.Add(new BaseRule {Name = "Feedback2CycleInterpolation", Value = true});
                        ruleList.Add(new BaseRule {Name = "Feedback2CycleResolution", Value = true});
                        ruleList.Add(new BaseRule {Name = "Feedback2StartupMethod", Value = true});
                        ruleList.Add(new BaseRule {Name = "Feedback2VelocityFilterBandwidth", Value = true});
                        ruleList.Add(new BaseRule {Name = "Feedback2VelocityFilterTaps", Value = true});

                        var feedback2StartupMethod = parameters.Feedback2StartupMethod;
                        var feedback2Unit = parameters.Feedback2Unit;
                        if (feedback2StartupMethod == FeedbackStartupMethodType.Absolute)
                        {
                            if (feedback2Unit == FeedbackUnitType.Rev)
                                ruleList.Add(new BaseRule {Name = "Feedback2Turns", Value = true});
                            else if (feedback2Unit == FeedbackUnitType.Meter)
                                ruleList.Add(new BaseRule {Name = "Feedback2Length", Value = true});
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
                    ruleList.Add(new BaseRule {Name = "Feedback2AccelFilterBandwidth", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback2AccelFilterTaps", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback2CycleInterpolation", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback2CycleResolution", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback2StartupMethod", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback2Turns", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback2Length", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback2Type", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback2Unit", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback2VelocityFilterBandwidth", Value = true});
                    ruleList.Add(new BaseRule {Name = "Feedback2VelocityFilterTaps", Value = true});
                }
                else
                {
                    ruleList.Add(new BaseRule {Name = "Feedback2AccelFilterBandwidth", Value = false});
                    ruleList.Add(new BaseRule {Name = "Feedback2AccelFilterTaps", Value = false});
                    ruleList.Add(new BaseRule {Name = "Feedback2CycleInterpolation", Value = false});
                    ruleList.Add(new BaseRule {Name = "Feedback2CycleResolution", Value = false});
                    ruleList.Add(new BaseRule {Name = "Feedback2StartupMethod", Value = false});
                    ruleList.Add(new BaseRule {Name = "Feedback2Turns", Value = false});
                    ruleList.Add(new BaseRule {Name = "Feedback2Length", Value = false});
                    ruleList.Add(new BaseRule {Name = "Feedback2Type", Value = false});
                    ruleList.Add(new BaseRule {Name = "Feedback2Unit", Value = false});
                    ruleList.Add(new BaseRule {Name = "Feedback2VelocityFilterBandwidth", Value = false});
                    ruleList.Add(new BaseRule {Name = "Feedback2VelocityFilterTaps", Value = false});
                }

                return ruleList.ToArray();
            }

            private void UpdateFeedback2StartupMethodSource()
            {
                var supportTypes = new List<FeedbackStartupMethodType>
                {
                    FeedbackStartupMethodType.Incremental,
                    FeedbackStartupMethodType.Absolute
                };

                var oldFeedback2StartupMethod =
                    (FeedbackStartupMethodType) Convert.ToByte(_parameters._currentAxis.CIPAxis.Feedback2StartupMethod);

                _parameters.Feedback2StartupMethodSource =
                    new ObservableCollection<FeedbackStartupMethodType>(supportTypes);

                if (!supportTypes.Contains(oldFeedback2StartupMethod))
                    _parameters._currentAxis.CIPAxis.Feedback2StartupMethod = (byte) supportTypes[0];
                else
                {
                    _parameters._currentAxis.CIPAxis.Feedback2StartupMethod = (byte) oldFeedback2StartupMethod;
                    _parameters.OnPropertyChanged("Feedback2StartupMethod");
                }
            }

            private void UpdateFeedback2UnitSource()
            {
                var supportTypes = new List<FeedbackUnitType>()
                {
                    FeedbackUnitType.Rev,
                    FeedbackUnitType.Meter
                };

                var oldFeedback2Unit =
                    (FeedbackUnitType) Convert.ToByte(_parameters._currentAxis.CIPAxis.Feedback2Unit);

                _parameters.Feedback2UnitSource = new ObservableCollection<FeedbackUnitType>(supportTypes);

                if (!supportTypes.Contains(oldFeedback2Unit))
                    _parameters.Feedback2Unit = supportTypes[0];
                else
                {
                    _parameters._currentAxis.CIPAxis.Feedback2Unit = (byte) oldFeedback2Unit;
                    _parameters.OnPropertyChanged("Feedback2Unit");
                }
            }

            private void UpdateFeedback2TypeSource()
            {
                IList supportFeedbackTypes = null;

                var cipMotionDrive = _parameters._currentAxis.AssociatedModule as CIPMotionDrive;
                if (cipMotionDrive != null)
                {
                    supportFeedbackTypes = cipMotionDrive.GetSupportFeedback2Types(_parameters._currentAxis.AxisNumber);
                }

                if (supportFeedbackTypes == null)
                    supportFeedbackTypes = new List<FeedbackType>();

                if (!supportFeedbackTypes.Contains(FeedbackType.NotSpecified))
                    supportFeedbackTypes.Insert(0, FeedbackType.NotSpecified);

                var oldFeedback2Type = (FeedbackType) Convert.ToByte(_parameters._currentAxis.CIPAxis.Feedback2Type);

                ((List<FeedbackType>) supportFeedbackTypes).Sort();
                _parameters.Feedback2TypeSource =
                    new ObservableCollection<FeedbackType>((List<FeedbackType>) supportFeedbackTypes);

                if (!supportFeedbackTypes.Contains(oldFeedback2Type))
                    _parameters.Feedback2Type = ((List<FeedbackType>) supportFeedbackTypes)[0];
                else
                {
                    _parameters._currentAxis.CIPAxis.Feedback2Type = (byte) oldFeedback2Type;
                    _parameters.OnPropertyChanged("Feedback2Type");
                }
            }

            public string[] GetCompareProperties()
            {
                string[] compareProperties =
                {
                    "Feedback2AccelFilterBandwidth", "Feedback2AccelFilterTaps",
                    "Feedback2CycleInterpolation", "Feedback2CycleResolution",
                    "Feedback2StartupMethod",
                    "Feedback2Length", "Feedback2Turns", "Feedback2Type", "Feedback2Unit",
                    "Feedback2VelocityFilterBandwidth", "Feedback2VelocityFilterTaps",
                };

                return compareProperties;
            }
        }
    }
}
