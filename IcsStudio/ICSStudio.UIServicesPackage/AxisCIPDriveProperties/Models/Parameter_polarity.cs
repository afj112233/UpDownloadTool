using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Controls.ExtendedPropertyGrid.Attributes;
using ICSStudio.SimpleServices.DataWrapper;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Models
{
    internal partial class AxisCIPParameters
    {
        private readonly Polarity _polarity;

        #region Public Property

        private PolarityType _motionPolarity;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Polarity")]
        public PolarityType MotionPolarity
        {
            get
            {
                _motionPolarity = (PolarityType) Convert.ToByte(_currentAxis.CIPAxis.MotionPolarity);
                return _motionPolarity;
            }
            set
            {
                if (Convert.ToByte(_currentAxis.CIPAxis.MotionPolarity) != (byte) value)
                {
                    _currentAxis.CIPAxis.MotionPolarity = (byte) value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private PolarityType _motorPolarity;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Polarity")]
        public PolarityType MotorPolarity
        {
            get
            {
                _motorPolarity = (PolarityType) Convert.ToByte(_currentAxis.CIPAxis.MotorPolarity);
                return _motorPolarity;
            }
            set
            {
                if (Convert.ToByte(_currentAxis.CIPAxis.MotorPolarity) != (byte) value)
                {
                    _currentAxis.CIPAxis.MotorPolarity = (byte) value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private PolarityType _feedback1Polarity;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Polarity")]
        public PolarityType Feedback1Polarity
        {
            get
            {
                _feedback1Polarity = (PolarityType) Convert.ToByte(_currentAxis.CIPAxis.Feedback1Polarity);
                return _feedback1Polarity;
            }
            set
            {
                if (Convert.ToByte(_currentAxis.CIPAxis.Feedback1Polarity) != (byte) value)
                {
                    _currentAxis.CIPAxis.Feedback1Polarity = (byte) value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        private PolarityType _feedback2Polarity;

        [Browsable(true)]
        [IsChanged(false)]
        [ReadOnly(false)]
        [Category("Polarity")]
        public PolarityType Feedback2Polarity
        {
            get
            {
                _feedback2Polarity = (PolarityType) Convert.ToByte(_currentAxis.CIPAxis.Feedback2Polarity);
                return _feedback2Polarity;
            }
            set
            {
                if (Convert.ToByte(_currentAxis.CIPAxis.Feedback2Polarity) != (byte) value)
                {
                    _currentAxis.CIPAxis.Feedback2Polarity = (byte) value;

                    UpdateIsChanged();
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Enum

        private ObservableCollection<PolarityType> MotionPolaritySource { get; set; }
            = new ObservableCollection<PolarityType>
            {
                PolarityType.Normal,
                PolarityType.Inverted
            };

        private ObservableCollection<PolarityType> MotorPolaritySource { get; set; }
            = new ObservableCollection<PolarityType>
            {
                PolarityType.Normal,
                PolarityType.Inverted
            };

        private ObservableCollection<PolarityType> Feedback1PolaritySource { get; set; }
            = new ObservableCollection<PolarityType>
            {
                PolarityType.Normal,
                PolarityType.Inverted
            };

        private ObservableCollection<PolarityType> Feedback2PolaritySource { get; set; }
            = new ObservableCollection<PolarityType>
            {
                PolarityType.Normal,
                PolarityType.Inverted
            };


        #endregion

        private class Polarity
        {
            private readonly AxisCIPParameters _parameters;

            public Polarity(AxisCIPParameters parameters)
            {
                _parameters = parameters;
            }

            public void Refresh()
            {
                UpdateVisibility();
                UpdateReadonly();
                UpdateIsChanged();
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

            private BaseRule[] GetVisibilityRules(AxisCIPParameters parameters)
            {
                var ruleList = new List<BaseRule>
                {
                    new BaseRule {Name = "MotionPolarity", Value = true},
                    new BaseRule {Name = "MotorPolarity", Value = false},
                    new BaseRule {Name = "Feedback1Polarity", Value = false},
                    new BaseRule {Name = "Feedback2Polarity", Value = false}
                };

                // MotorType
                var motorType = (MotorType) Convert.ToByte(parameters._currentAxis.CIPAxis.MotorType);
                if (motorType != MotorType.NotSpecified)
                    ruleList.Add(new BaseRule {Name = "MotorPolarity", Value = true});

                var feedback1Type = (FeedbackType) Convert.ToByte(parameters._currentAxis.CIPAxis.Feedback1Type);
                if (parameters._currentAxis.SupportAttribute("Feedback1Polarity") &&
                    feedback1Type != FeedbackType.NotSpecified)
                    ruleList.Add(new BaseRule {Name = "Feedback1Polarity", Value = true});

                //TODO(gjc): need rewrite here 
                if (parameters._currentAxis.SupportAttribute("Feedback2Polarity"))
                    ruleList.Add(new BaseRule {Name = "Feedback2Polarity", Value = true});

                return ruleList.ToArray();
            }

            private BaseRule[] GetReadonlyRules(AxisCIPParameters parameters)
            {
                var ruleList = new List<BaseRule>();

                //TODO(gjc): edit later
                if (parameters._parentViewModel.IsPowerStructureEnabled || parameters._parentViewModel.IsOnLine)
                {
                    ruleList.Add(new BaseRule { Name = "MotionPolarity", Value = true });
                    ruleList.Add(new BaseRule { Name = "MotorPolarity", Value = true });
                    ruleList.Add(new BaseRule { Name = "Feedback1Polarity", Value = true });
                    ruleList.Add(new BaseRule { Name = "Feedback2Polarity", Value = true });
                }
                else
                {
                    ruleList.Add(new BaseRule { Name = "MotionPolarity", Value = false });
                    ruleList.Add(new BaseRule { Name = "MotorPolarity", Value = false });
                    ruleList.Add(new BaseRule { Name = "Feedback1Polarity", Value = false });
                    ruleList.Add(new BaseRule { Name = "Feedback2Polarity", Value = false });
                }

                return ruleList.ToArray();
            }

            public string[] GetCompareProperties()
            {
                string[] compareProperties =
                {
                    "MotionPolarity", "MotorPolarity", "Feedback1Polarity", "Feedback2Polarity"
                };
                return compareProperties;
            }
        }
    }
}
