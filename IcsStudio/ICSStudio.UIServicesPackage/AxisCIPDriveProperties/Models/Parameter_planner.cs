using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Controls.ExtendedPropertyGrid.Attributes;
using ICSStudio.Gui.Utils;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Models
{
    internal partial class AxisCIPParameters
    {
        private readonly Planner _planner;

        #region Public Property

        private float _averageVelocityTimebase;

        // rm003,page 101
        [Browsable(true)]
        [IsChanged(false)]
        [Unit("s")]
        [ReadOnly(false)]
        [Category("Planner")]
        public float AverageVelocityTimebase
        {
            get
            {
                _averageVelocityTimebase = Convert.ToSingle(_currentAxis.CIPAxis.AverageVelocityTimebase);
                return _averageVelocityTimebase;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.AverageVelocityTimebase) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.AverageVelocityTimebase = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private int _commandUpdateDelayOffset;

        // rm003, page 82
        [Browsable(true)]
        [IsChanged(false)]
        [Unit("us")]
        [ReadOnly(false)]
        [Category("Planner")]
        public int CommandUpdateDelayOffset
        {
            get
            {
                _commandUpdateDelayOffset = Convert.ToInt32(_currentAxis.CIPAxis.CommandUpdateDelayOffset);
                return _commandUpdateDelayOffset;
            }
            set
            {
                if (Convert.ToInt32(_currentAxis.CIPAxis.CommandUpdateDelayOffset) != value)
                {
                    _currentAxis.CIPAxis.CommandUpdateDelayOffset = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private InterpolatedPositionConfigurationType
            _interpolatedActualPositionConfiguration;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Planner")]
        public InterpolatedPositionConfigurationType
            InterpolatedActualPositionConfiguration
        {
            get
            {
                var bits = Convert.ToUInt32(_currentAxis.CIPAxis.InterpolatedPositionConfiguration);
                _interpolatedActualPositionConfiguration = (InterpolatedPositionConfigurationType) (bits & 0x1);
                return _interpolatedActualPositionConfiguration;
            }
            set
            {
                var bits = Convert.ToUInt32(_currentAxis.CIPAxis.InterpolatedPositionConfiguration);
                if ((InterpolatedPositionConfigurationType) (bits & 0x1) != value)
                {
                    if (Convert.ToBoolean(value))
                        bits |= 0x1;
                    else
                        bits &= ~(uint) 0x1;
                    _currentAxis.CIPAxis.InterpolatedPositionConfiguration = bits;

                    _planner.UpdateBitIsChanged();

                    OnPropertyChanged();
                }
            }
        }

        private InterpolatedPositionConfigurationType
            _interpolatedCommandPositionConfiguration;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Planner")]
        public InterpolatedPositionConfigurationType
            InterpolatedCommandPositionConfiguration
        {
            get
            {
                var bits = Convert.ToUInt32(_currentAxis.CIPAxis.InterpolatedPositionConfiguration);
                _interpolatedCommandPositionConfiguration =
                    (InterpolatedPositionConfigurationType) ((bits >> 1) & 0x1);
                return _interpolatedCommandPositionConfiguration;
            }
            set
            {
                var bits = Convert.ToUInt32(_currentAxis.CIPAxis.InterpolatedPositionConfiguration);

                if ((InterpolatedPositionConfigurationType) ((bits >> 1) & 0x1) != value)
                {
                    if (Convert.ToBoolean(value))
                        bits |= 0x2;
                    else
                        bits &= ~(uint) 0x2;

                    _currentAxis.CIPAxis.InterpolatedPositionConfiguration = bits;

                    _planner.UpdateBitIsChanged();

                    OnPropertyChanged();
                }

            }
        }

        private bool _masterDelayCompensation;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Planner")]
        public bool MasterDelayCompensation
        {
            get
            {
                var bits = Convert.ToUInt32(_currentAxis.CIPAxis.MasterInputConfigurationBits);
                _masterDelayCompensation = FlagsEnumHelper.ContainFlag(bits,
                    MasterInputConfigurationBitmap.MasterDelayCompensation);
                return _masterDelayCompensation;
            }
            set
            {
                var bits = Convert.ToUInt32(_currentAxis.CIPAxis.MasterInputConfigurationBits);
                if (FlagsEnumHelper.ContainFlag(bits, MasterInputConfigurationBitmap.MasterDelayCompensation) != value)
                {
                    FlagsEnumHelper.SelectFlag(MasterInputConfigurationBitmap.MasterDelayCompensation, value, ref bits);
                    _currentAxis.CIPAxis.MasterInputConfigurationBits = bits;

                    _planner.UpdateBitIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private bool _masterPositionFilter;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Planner")]
        public bool MasterPositionFilter
        {
            get
            {
                var bits = Convert.ToUInt32(_currentAxis.CIPAxis.MasterInputConfigurationBits);
                _masterPositionFilter = FlagsEnumHelper.ContainFlag(bits,
                    MasterInputConfigurationBitmap.MasterPositionFilter);
                return _masterPositionFilter;
            }
            set
            {
                var bits = Convert.ToUInt32(_currentAxis.CIPAxis.MasterInputConfigurationBits);
                if (FlagsEnumHelper.ContainFlag(bits, MasterInputConfigurationBitmap.MasterPositionFilter) != value)
                {
                    FlagsEnumHelper.SelectFlag(MasterInputConfigurationBitmap.MasterPositionFilter, value, ref bits);
                    _currentAxis.CIPAxis.MasterInputConfigurationBits = bits;

                    _planner.UpdateBitIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _masterPositionFilterBandwidth;

        // rm003, page 89
        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Hz")]
        [ReadOnly(false)]
        [Category("Planner")]
        public float MasterPositionFilterBandwidth
        {
            get
            {
                _masterPositionFilterBandwidth = Convert.ToSingle(_currentAxis.CIPAxis.MasterPositionFilterBandwidth);
                return _masterPositionFilterBandwidth;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.MasterPositionFilterBandwidth) - value) >
                    float.Epsilon)
                {
                    _currentAxis.CIPAxis.MasterPositionFilterBandwidth = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _maximumAcceleration;

        // rm003, page 110
        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Position Units/s^2")]
        [ReadOnly(false)]
        [Category("Planner")]
        public float MaximumAcceleration
        {
            get
            {
                _maximumAcceleration = Convert.ToSingle(_currentAxis.CIPAxis.MaximumAcceleration);
                return _maximumAcceleration;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.MaximumAcceleration) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.MaximumAcceleration = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _maximumAccelerationJerk;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Position Units/s^3")]
        [ReadOnly(false)]
        [Category("Planner")]
        public float MaximumAccelerationJerk
        {
            get
            {
                _maximumAccelerationJerk = Convert.ToSingle(_currentAxis.CIPAxis.MaximumAccelerationJerk);
                return _maximumAccelerationJerk;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.MaximumAccelerationJerk) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.MaximumAccelerationJerk = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _maximumDeceleration;

        // rm003, page 110
        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Position Units/s^2")]
        [ReadOnly(false)]
        [Category("Planner")]
        public float MaximumDeceleration
        {
            get
            {
                _maximumDeceleration = Convert.ToSingle(_currentAxis.CIPAxis.MaximumDeceleration);
                return _maximumDeceleration;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.MaximumDeceleration) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.MaximumDeceleration = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _maximumDecelerationJerk;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Position Units/s^3")]
        [ReadOnly(false)]
        [Category("Planner")]
        public float MaximumDecelerationJerk
        {
            get
            {
                _maximumDecelerationJerk = Convert.ToSingle(_currentAxis.CIPAxis.MaximumDecelerationJerk);
                return _maximumDecelerationJerk;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.MaximumDecelerationJerk) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.MaximumDecelerationJerk = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private float _maximumSpeed;

        [Browsable(true)]
        [IsChanged(false)]
        [Unit("Position Units/s")]
        [ReadOnly(false)]
        [Category("Planner")]
        public float MaximumSpeed
        {
            get
            {
                _maximumSpeed = Convert.ToSingle(_currentAxis.CIPAxis.MaximumSpeed);
                return _maximumSpeed;
            }
            set
            {
                if (Math.Abs(Convert.ToSingle(_currentAxis.CIPAxis.MaximumSpeed) - value) > float.Epsilon)
                {
                    _currentAxis.CIPAxis.MaximumSpeed = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private uint _outputCamExecutionTargets;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Planner")]
        public uint OutputCamExecutionTargets
        {
            get
            {
                _outputCamExecutionTargets = Convert.ToUInt32(_currentAxis.CIPAxis.OutputCamExecutionTargets);
                return _outputCamExecutionTargets;
            }
            set
            {
                if (Convert.ToUInt32(_currentAxis.CIPAxis.OutputCamExecutionTargets) != value)
                {
                    _currentAxis.CIPAxis.OutputCamExecutionTargets = value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private bool _preventSCurveVelocityOvershoot;

        //TODO(gjc): need check!!!
        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Planner")]
        public bool PreventSCurveVelocityOvershoot
        {
            get
            {
                var bits = Convert.ToUInt32(_currentAxis.CIPAxis.DynamicsConfigurationBits);
                _preventSCurveVelocityOvershoot = FlagsEnumHelper.ContainFlag(bits,
                    DynamicsConfigurationBitmap.ReducedExtremeVelocityOvershoot);
                return _preventSCurveVelocityOvershoot;
            }
            set
            {
                var bits = Convert.ToUInt32(_currentAxis.CIPAxis.DynamicsConfigurationBits);
                if (FlagsEnumHelper.ContainFlag(bits, DynamicsConfigurationBitmap.ReducedExtremeVelocityOvershoot) !=
                    value)
                {
                    FlagsEnumHelper.SelectFlag(DynamicsConfigurationBitmap.ReducedExtremeVelocityOvershoot, value,
                        ref bits);
                    _currentAxis.CIPAxis.DynamicsConfigurationBits = bits;

                    _planner.UpdateBitIsChanged();
                    OnPropertyChanged();

                }

            }
        }

        private bool _preventSCurveVelocityReversal;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Planner")]
        public bool PreventSCurveVelocityReversal
        {
            get
            {
                var bits = Convert.ToUInt32(_currentAxis.CIPAxis.DynamicsConfigurationBits);
                _preventSCurveVelocityReversal = FlagsEnumHelper.ContainFlag(bits,
                    DynamicsConfigurationBitmap.PreventSCurveVelocityReversals);
                return _preventSCurveVelocityReversal;
            }
            set
            {
                var bits = Convert.ToUInt32(_currentAxis.CIPAxis.DynamicsConfigurationBits);
                if (FlagsEnumHelper.ContainFlag(bits,
                    DynamicsConfigurationBitmap.PreventSCurveVelocityReversals) != value)
                {
                    FlagsEnumHelper.SelectFlag(DynamicsConfigurationBitmap.PreventSCurveVelocityReversals, value,
                        ref bits);
                    _currentAxis.CIPAxis.DynamicsConfigurationBits = bits;

                    _planner.UpdateBitIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private bool _reduceSCurveStopDelay;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Planner")]
        public bool ReduceSCurveStopDelay
        {
            get
            {
                var bits = Convert.ToUInt32(_currentAxis.CIPAxis.DynamicsConfigurationBits);
                _reduceSCurveStopDelay = FlagsEnumHelper.ContainFlag(bits,
                    DynamicsConfigurationBitmap.ReduceSCurveStopDelay);
                return _reduceSCurveStopDelay;
            }
            set
            {
                var bits = Convert.ToUInt32(_currentAxis.CIPAxis.DynamicsConfigurationBits);
                if (FlagsEnumHelper.ContainFlag(bits,
                    DynamicsConfigurationBitmap.ReduceSCurveStopDelay) != value)
                {
                    FlagsEnumHelper.SelectFlag(DynamicsConfigurationBitmap.ReduceSCurveStopDelay, value, ref bits);
                    _currentAxis.CIPAxis.DynamicsConfigurationBits = bits;

                    _planner.UpdateBitIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Enum

        private ObservableCollection<InterpolatedPositionConfigurationType>
            InterpolatedActualPositionConfigurationSource { get; set; } = new ObservableCollection
            <InterpolatedPositionConfigurationType>
            {
                InterpolatedPositionConfigurationType.FirstOrder,
                InterpolatedPositionConfigurationType.SecondOrder
            };


        private ObservableCollection<InterpolatedPositionConfigurationType>
            InterpolatedCommandPositionConfigurationSource { get; set; } = new ObservableCollection
            <InterpolatedPositionConfigurationType>
            {
                InterpolatedPositionConfigurationType.FirstOrder,
                InterpolatedPositionConfigurationType.SecondOrder
            };

        #endregion

        private class Planner
        {
            private readonly AxisCIPParameters _parameters;

            public Planner(AxisCIPParameters parameters)
            {
                _parameters = parameters;
            }

            public void Refresh()
            {
                UpdateVisibility();
                UpdateReadonly();
                UpdateIsChanged();
            }

            private void UpdateVisibility()
            {
                BaseRule.ApplyVisibilityRule(_parameters, GetVisibilityRules(_parameters));
            }

            private void UpdateReadonly()
            {
                BaseRule.ApplyReadonlyRule(_parameters, GetReadonlyRules(_parameters));
            }

            private void UpdateIsChanged()
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

                UpdateBitIsChanged();
            }

            public void UpdateBitIsChanged()
            {
                //InterpolatedActualPositionConfiguration
                var bits1 = Convert.ToUInt32(_parameters._currentAxis.CIPAxis.InterpolatedPositionConfiguration);
                var bits2 = Convert.ToUInt32(_parameters._originalAxis.CIPAxis.InterpolatedPositionConfiguration);
                PropertySetting.SetPropertyIsChanged(_parameters,
                    "InterpolatedActualPositionConfiguration",
                    (bits1 & 0x1) != (bits2 & 0x1));

                //InterpolatedCommandPositionConfiguration
                PropertySetting.SetPropertyIsChanged(_parameters,
                    "InterpolatedCommandPositionConfiguration",
                    (bits1 & 0x2) != (bits2 & 0x2));

                //MasterDelayCompensation
                bits1 = Convert.ToUInt32(_parameters._currentAxis.CIPAxis.MasterInputConfigurationBits);
                bits2 = Convert.ToUInt32(_parameters._originalAxis.CIPAxis.MasterInputConfigurationBits);
                PropertySetting.SetPropertyIsChanged(_parameters,
                    "MasterDelayCompensation",
                    (bits1 & 0x1) != (bits2 & 0x1));

                //MasterPositionFilter
                PropertySetting.SetPropertyIsChanged(_parameters,
                    "MasterPositionFilter",
                    (bits1 & 0x2) != (bits2 & 0x2));

                //PreventSCurveVelocityOvershoot
                bits1 = Convert.ToUInt32(_parameters._currentAxis.CIPAxis.DynamicsConfigurationBits);
                bits2 = Convert.ToUInt32(_parameters._originalAxis.CIPAxis.DynamicsConfigurationBits);
                PropertySetting.SetPropertyIsChanged(_parameters,
                    "PreventSCurveVelocityOvershoot",
                    (bits1 & 0x4) != (bits2 & 0x4));

                //PreventSCurveVelocityReversal
                PropertySetting.SetPropertyIsChanged(_parameters,
                    "PreventSCurveVelocityReversal",
                    (bits1 & 0x2) != (bits2 & 0x2));

                //ReduceSCurveStopDelay
                PropertySetting.SetPropertyIsChanged(_parameters,
                    "ReduceSCurveStopDelay",
                    (bits1 & 0x1) != (bits2 & 0x1));
            }

            private BaseRule[] GetVisibilityRules(AxisCIPParameters parameters)
            {
                var ruleList = new List<BaseRule>
                {
                    new BaseRule {Name = "AverageVelocityTimebase", Value = false},
                    new BaseRule {Name = "CommandUpdateDelayOffset", Value = false},
                    new BaseRule {Name = "InterpolatedActualPositionConfiguration", Value = false},
                    new BaseRule {Name = "InterpolatedCommandPositionConfiguration", Value = false},
                    new BaseRule {Name = "MasterDelayCompensation", Value = false},
                    new BaseRule {Name = "MasterPositionFilter", Value = false},
                    new BaseRule {Name = "MasterPositionFilterBandwidth", Value = false},
                    new BaseRule {Name = "MaximumAcceleration", Value = false},
                    new BaseRule {Name = "MaximumAccelerationJerk", Value = false},
                    new BaseRule {Name = "MaximumDeceleration", Value = false},
                    new BaseRule {Name = "MaximumDecelerationJerk", Value = false},
                    new BaseRule {Name = "MaximumSpeed", Value = false},
                    new BaseRule {Name = "OutputCamExecutionTargets", Value = false},
                    new BaseRule {Name = "PreventSCurveVelocityOvershoot", Value = false},
                    new BaseRule {Name = "PreventSCurveVelocityReversal", Value = false},
                    new BaseRule {Name = "ReduceSCurveStopDelay", Value = false}
                };

                var axisConfiguration =
                    (AxisConfigurationType) Convert.ToByte(parameters._currentAxis.CIPAxis.AxisConfiguration);

                switch (axisConfiguration)
                {
                    case AxisConfigurationType.FeedbackOnly:
                        ruleList.Add(new BaseRule {Name = "AverageVelocityTimebase", Value = true});
                        ruleList.Add(new BaseRule {Name = "InterpolatedActualPositionConfiguration", Value = true});
                        ruleList.Add(new BaseRule {Name = "InterpolatedCommandPositionConfiguration", Value = true});
                        ruleList.Add(new BaseRule {Name = "MasterDelayCompensation", Value = true});
                        ruleList.Add(new BaseRule {Name = "MasterPositionFilter", Value = true});
                        ruleList.Add(new BaseRule {Name = "MasterPositionFilterBandwidth", Value = true});
                        ruleList.Add(new BaseRule {Name = "OutputCamExecutionTargets", Value = true});
                        break;
                    case AxisConfigurationType.FrequencyControl:
                        ruleList.Add(new BaseRule { Name = "AverageVelocityTimebase", Value = true });
                        ruleList.Add(new BaseRule { Name = "MaximumAcceleration", Value = true });
                        ruleList.Add(new BaseRule { Name = "MaximumAccelerationJerk", Value = true });
                        ruleList.Add(new BaseRule { Name = "MaximumDeceleration", Value = true });
                        ruleList.Add(new BaseRule { Name = "MaximumDecelerationJerk", Value = true });
                        ruleList.Add(new BaseRule { Name = "MaximumSpeed", Value = true });
                        ruleList.Add(new BaseRule { Name = "PreventSCurveVelocityOvershoot", Value = true });
                        ruleList.Add(new BaseRule { Name = "PreventSCurveVelocityReversal", Value = true });
                        ruleList.Add(new BaseRule { Name = "ReduceSCurveStopDelay", Value = true });
                        break;
                    case AxisConfigurationType.PositionLoop:
                        ruleList.Add(new BaseRule {Name = "AverageVelocityTimebase", Value = true});
                        ruleList.Add(new BaseRule {Name = "CommandUpdateDelayOffset", Value = true});
                        ruleList.Add(new BaseRule {Name = "InterpolatedActualPositionConfiguration", Value = true});
                        ruleList.Add(new BaseRule {Name = "InterpolatedCommandPositionConfiguration", Value = true});
                        ruleList.Add(new BaseRule {Name = "MasterDelayCompensation", Value = true});
                        ruleList.Add(new BaseRule {Name = "MasterPositionFilter", Value = true});
                        ruleList.Add(new BaseRule {Name = "MasterPositionFilterBandwidth", Value = true});
                        ruleList.Add(new BaseRule {Name = "MaximumAcceleration", Value = true});
                        ruleList.Add(new BaseRule {Name = "MaximumAccelerationJerk", Value = true});
                        ruleList.Add(new BaseRule {Name = "MaximumDeceleration", Value = true});
                        ruleList.Add(new BaseRule {Name = "MaximumDecelerationJerk", Value = true});
                        ruleList.Add(new BaseRule {Name = "MaximumSpeed", Value = true});
                        ruleList.Add(new BaseRule {Name = "OutputCamExecutionTargets", Value = true});
                        ruleList.Add(new BaseRule {Name = "PreventSCurveVelocityOvershoot", Value = true});
                        ruleList.Add(new BaseRule {Name = "PreventSCurveVelocityReversal", Value = true});
                        ruleList.Add(new BaseRule {Name = "ReduceSCurveStopDelay", Value = true});
                        break;
                    case AxisConfigurationType.VelocityLoop:
                        ruleList.Add(new BaseRule {Name = "AverageVelocityTimebase", Value = true});
                        ruleList.Add(new BaseRule {Name = "CommandUpdateDelayOffset", Value = true});
                        ruleList.Add(new BaseRule {Name = "InterpolatedActualPositionConfiguration", Value = true});
                        ruleList.Add(new BaseRule {Name = "InterpolatedCommandPositionConfiguration", Value = true});
                        ruleList.Add(new BaseRule {Name = "MasterDelayCompensation", Value = true});
                        ruleList.Add(new BaseRule {Name = "MasterPositionFilter", Value = true});
                        ruleList.Add(new BaseRule {Name = "MasterPositionFilterBandwidth", Value = true});
                        ruleList.Add(new BaseRule {Name = "MaximumAcceleration", Value = true});
                        ruleList.Add(new BaseRule {Name = "MaximumAccelerationJerk", Value = true});
                        ruleList.Add(new BaseRule {Name = "MaximumDeceleration", Value = true});
                        ruleList.Add(new BaseRule {Name = "MaximumDecelerationJerk", Value = true});
                        ruleList.Add(new BaseRule {Name = "MaximumSpeed", Value = true});
                        ruleList.Add(new BaseRule {Name = "OutputCamExecutionTargets", Value = true});
                        ruleList.Add(new BaseRule {Name = "PreventSCurveVelocityOvershoot", Value = true});
                        ruleList.Add(new BaseRule {Name = "PreventSCurveVelocityReversal", Value = true});
                        ruleList.Add(new BaseRule {Name = "ReduceSCurveStopDelay", Value = true});
                        break;
                    case AxisConfigurationType.TorqueLoop:
                        ruleList.Add(new BaseRule {Name = "AverageVelocityTimebase", Value = true});
                        ruleList.Add(new BaseRule {Name = "InterpolatedActualPositionConfiguration", Value = true});
                        ruleList.Add(new BaseRule {Name = "InterpolatedCommandPositionConfiguration", Value = true});
                        ruleList.Add(new BaseRule {Name = "OutputCamExecutionTargets", Value = true});
                        break;
                    case AxisConfigurationType.ConverterOnly:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return ruleList.ToArray();
            }

            private BaseRule[] GetReadonlyRules(AxisCIPParameters parameters)
            {
                var ruleList = new List<BaseRule>();

                if (parameters._parentViewModel.IsPowerStructureEnabled || parameters._parentViewModel.IsHardRunMode)
                {
                    ruleList.Add(new BaseRule {Name = "AverageVelocityTimebase", Value = true});
                    ruleList.Add(new BaseRule {Name = "CommandUpdateDelayOffset", Value = true});
                    ruleList.Add(new BaseRule {Name = "InterpolatedActualPositionConfiguration", Value = true});
                    ruleList.Add(new BaseRule {Name = "InterpolatedCommandPositionConfiguration", Value = true});
                    ruleList.Add(new BaseRule {Name = "MasterDelayCompensation", Value = true});
                    ruleList.Add(new BaseRule {Name = "MasterPositionFilter", Value = true});
                    ruleList.Add(new BaseRule {Name = "MasterPositionFilterBandwidth", Value = true});
                    ruleList.Add(new BaseRule {Name = "MaximumAcceleration", Value = true});
                    ruleList.Add(new BaseRule {Name = "MaximumAccelerationJerk", Value = true});
                    ruleList.Add(new BaseRule {Name = "MaximumDeceleration", Value = true});
                    ruleList.Add(new BaseRule {Name = "MaximumDecelerationJerk", Value = true});
                    ruleList.Add(new BaseRule {Name = "MaximumSpeed", Value = true});
                    ruleList.Add(new BaseRule {Name = "OutputCamExecutionTargets", Value = true});
                    ruleList.Add(new BaseRule {Name = "PreventSCurveVelocityOvershoot", Value = true});
                    ruleList.Add(new BaseRule {Name = "PreventSCurveVelocityReversal", Value = true});
                    ruleList.Add(new BaseRule {Name = "ReduceSCurveStopDelay", Value = true});
                }
                else
                {
                    ruleList.Add(new BaseRule {Name = "AverageVelocityTimebase", Value = false});
                    ruleList.Add(new BaseRule {Name = "CommandUpdateDelayOffset", Value = false});
                    ruleList.Add(new BaseRule {Name = "InterpolatedActualPositionConfiguration", Value = false});
                    ruleList.Add(new BaseRule {Name = "InterpolatedCommandPositionConfiguration", Value = false});
                    ruleList.Add(new BaseRule {Name = "MasterDelayCompensation", Value = false});
                    ruleList.Add(new BaseRule {Name = "MasterPositionFilter", Value = false});
                    ruleList.Add(new BaseRule {Name = "MasterPositionFilterBandwidth", Value = false});
                    ruleList.Add(new BaseRule {Name = "MaximumAcceleration", Value = false});
                    ruleList.Add(new BaseRule {Name = "MaximumAccelerationJerk", Value = false});
                    ruleList.Add(new BaseRule {Name = "MaximumDeceleration", Value = false});
                    ruleList.Add(new BaseRule {Name = "MaximumDecelerationJerk", Value = false});
                    ruleList.Add(new BaseRule {Name = "MaximumSpeed", Value = false});
                    ruleList.Add(new BaseRule {Name = "OutputCamExecutionTargets", Value = false});
                    ruleList.Add(new BaseRule {Name = "PreventSCurveVelocityOvershoot", Value = false});
                    ruleList.Add(new BaseRule {Name = "PreventSCurveVelocityReversal", Value = false});
                    ruleList.Add(new BaseRule {Name = "ReduceSCurveStopDelay", Value = false});

                    if (parameters._parentViewModel.IsOnLine)
                    {
                        ruleList.Add(new BaseRule {Name = "OutputCamExecutionTargets", Value = true});
                    }
                }

                return ruleList.ToArray();
            }

            public string[] GetCompareProperties()
            {
                string[] compareProperties =
                {
                    "AverageVelocityTimebase", "CommandUpdateDelayOffset",
                    "MasterPositionFilterBandwidth",
                    "MaximumAcceleration", "MaximumAccelerationJerk",
                    "MaximumDeceleration", "MaximumDecelerationJerk",
                    "MaximumSpeed", "OutputCamExecutionTargets"
                };

                return compareProperties;
            }

            public string[] GetPeriodicRefreshProperties()
            {
                string[] periodicRefreshProperties = new[]
                {
                    "AverageVelocityTimebase",
                    "MaximumSpeed",
                    "MaximumAcceleration",
                    "MaximumDeceleration"
                };

                return periodicRefreshProperties;
            }
        }
    }
}
