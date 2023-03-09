using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.DataTypes;
using ICSStudio.Cip.Objects;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Gui.Utils;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Models;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal partial class ParameterListViewModel : DefaultViewModel
    {
        private readonly AxisCIPParameters _axisCIPParameters;

        private string _parameterGroup;
        private List<DisplayItem<string>> _parameterGroupSource;

        private class CheckParameterInfo
        {
            public string Name { get; set; }
            public string Group { get; set; }
            public float MinValue { get; set; }
            public float MaxValue { get; set; }
        }

        public ParameterListViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            AssociatedPageCommand = new RelayCommand(ExecuteAssociatedPageCommand, CanAssociatedPageCommand);

            UpdateParameterGroupSource();

            _axisCIPParameters =
                new AxisCIPParameters(
                    ParentViewModel.ModifiedAxisCIPDrive,
                    ParentViewModel.OriginalAxisCIPDrive,
                    ParentViewModel);

            Collection = new ParameterItemCollection(_axisCIPParameters);

            DefaultView = CollectionViewSource.GetDefaultView(Collection) as ListCollectionView;

            if (DefaultView != null)
                DefaultView.Filter = Filter;

            PropertyChangedEventManager.AddHandler(_axisCIPParameters, OnParameterChanged, string.Empty);

            CompareProperties = _axisCIPParameters.GetCompareProperties();

            PeriodicRefreshProperties = _axisCIPParameters.GetPeriodicRefreshProperties();
        }

        public override void Cleanup()
        {
            PropertyChangedEventManager.RemoveHandler(_axisCIPParameters, OnParameterChanged, string.Empty);
            base.Cleanup();
        }

        public override void Show()
        {
            UpdateParameterGroupSource();

            AssociatedPageCommand.RaiseCanExecuteChanged();

            _axisCIPParameters.Refresh();

            Collection.Refresh();

            //DefaultView.Refresh();
        }

        public override int CheckValid()
        {
            _axisCIPParameters.Refresh();

            var result = 0;

            var errorCheckSubParameter = string.Empty;
            var message = $"Failed to modify properties for axis '{ParentViewModel.AxisTag.Name}'.";
            var reason = string.Empty;
            var errorCode = "Error 16358-80044219";

            var subParameters = GetSubParameters();

            foreach (var info in CheckParameters)
            {
                if (subParameters.Contains(info.Group))
                {
                    bool visible = PropertySetting.GetPropertyVisibility(_axisCIPParameters, info.Name);
                    if (!visible)
                        continue;

                    // calc min and max for special
                    if (info.Name.Equals("AverageVelocityTimebase"))
                    {
                        MotionGroup motionGroup =
                            ((Tag)ParentViewModel.ModifiedAxisCIPDrive.AssignedGroup)?.DataWrapper as MotionGroup;

                        if (motionGroup != null)
                        {
                            info.MinValue = motionGroup.GetUpdatePeriod(AxisUpdateScheduleType.Base) / 1000;
                            info.MaxValue = motionGroup.GetUpdatePeriod(AxisUpdateScheduleType.Base);
                        }
                    }

                    if (info.Name.Equals("BacklashReversalOffset"))
                    {
                        var motionResolution = Convert.ToUInt32(ModifiedCIPAxis.MotionResolution);
                        double travelRangeLimit = (double)int.MaxValue / motionResolution;
                        info.MaxValue = (float)travelRangeLimit;
                    }

                    if (info.Name.Equals("VelocityThreshold") || info.Name.Equals("VelocityLimitPositive"))
                    {
                        //TODO(gjc): need more check
                        double numerator = Convert.ToSingle(ModifiedCIPAxis.PositionScalingNumerator);
                        double denominator = Convert.ToSingle(ModifiedCIPAxis.PositionScalingDenominator);

                        double maxVelocityThreshold =
                            int.MaxValue * Math.Pow(10, Math.Ceiling(Math.Log10(numerator))) / denominator / 1000;

                        info.MaxValue = (float)maxVelocityThreshold;
                    }

                    if (info.Name.Equals("VelocityLimitNegative"))
                    {
                        //TODO(gjc): need more check
                        double numerator = Convert.ToSingle(ModifiedCIPAxis.PositionScalingNumerator);
                        double denominator = Convert.ToSingle(ModifiedCIPAxis.PositionScalingDenominator);

                        double maxVelocityThreshold =
                            int.MaxValue * Math.Pow(10, Math.Ceiling(Math.Log10(numerator))) / denominator / 1000;

                        info.MinValue = -(float)maxVelocityThreshold;
                    }

                    if (info.Name.Equals("PositionErrorTolerance") || info.Name.Equals("PositionLockTolerance"))
                    {
                        //TODO(gjc): need more check
                        double numerator = Convert.ToSingle(ModifiedCIPAxis.PositionScalingNumerator);
                        double denominator = Convert.ToSingle(ModifiedCIPAxis.PositionScalingDenominator);

                        double maxVelocityThreshold =
                            int.MaxValue * Math.Pow(10, Math.Ceiling(Math.Log10(numerator))) / denominator / 1000000;

                        info.MaxValue = (float)maxVelocityThreshold;
                    }

                    //
                    var propertyInfo = typeof(CIPAxis).GetProperty(info.Name);
                    if (propertyInfo != null)
                    {
                        // get value
                        object valueObject = propertyInfo.GetValue(ModifiedCIPAxis);

                        float value;

                        if (valueObject is CipReal)
                        {
                            value = Convert.ToSingle((CipReal)valueObject);
                        }
                        else if (valueObject is CipUint)
                        {
                            value = Convert.ToUInt16((CipUint)valueObject);
                        }
                        else if (valueObject is CipUdint)
                        {
                            value = Convert.ToUInt32((CipUdint)valueObject);
                        }
                        else
                        {
                            throw new NotImplementedException("Add code for CheckValid!");
                        }


                        if (!(value >= info.MinValue && value <= info.MaxValue))
                        {
                            errorCheckSubParameter = info.Group;
                            reason =
                                $"Enter a {info.Name} between {info.MinValue:r} and {info.MaxValue:r}.";

                            result = -1;
                            break;
                        }

                    }

                }
            }

            if (result < 0)
            {
                // show page
                ParentViewModel.ShowPanel("Parameter List", errorCheckSubParameter);

                // show warning
                var warningDialog =
                    new WarningDialog(
                        message,
                        reason,
                        errorCode)
                    {
                        Owner = Application.Current.MainWindow
                    };
                warningDialog.ShowDialog();
            }

            return result;
        }

        public List<DisplayItem<string>> ParameterGroupSource
        {
            get { return _parameterGroupSource; }
            set { Set(ref _parameterGroupSource, value); }
        }

        public string ParameterGroup
        {
            get { return _parameterGroup; }
            set
            {
                if (!string.Equals(_parameterGroup, value))
                {
                    Set(ref _parameterGroup, value);

                    AssociatedPageCommand.RaiseCanExecuteChanged();

                    if (!string.IsNullOrEmpty(_parameterGroup))
                        DefaultView?.Refresh();
                }
            }
        }

        public ParameterItemCollection Collection { get; }

        public ListCollectionView DefaultView { get; }

        #region Command

        public RelayCommand AssociatedPageCommand { get; }

        private bool CanAssociatedPageCommand()
        {
            if (string.IsNullOrEmpty(_parameterGroup))
                return false;

            if (string.Equals(_parameterGroup, "All"))
                return false;

            return true;
        }

        private void ExecuteAssociatedPageCommand()
        {
            ParentViewModel.ShowPanel(_parameterGroup);
        }

        #endregion


        #region Private

        private void UpdateParameterGroupSource()
        {
            // update source
            var source = new List<DisplayItem<string>>
            {
                new DisplayItem<string> { DisplayName = "All", Value = "All" }
            };

            var subParameters = GetSubParameters();
            foreach (var subParameter in subParameters)
                source.Add(new DisplayItem<string> { DisplayName = subParameter, Value = subParameter });

            // keep selected
            var oldParameterGroup = ParameterGroup;

            ParameterGroupSource = source;

            if (oldParameterGroup == null)
            {
                ParameterGroup = ParameterGroupSource[0].Value;
            }
            else
            {
                var beContained = false;
                foreach (var item in source)
                    if (string.Equals(item.Value, oldParameterGroup))
                    {
                        beContained = true;
                        break;
                    }

                ParameterGroup = beContained ? oldParameterGroup : ParameterGroupSource[0].Value;
            }
        }

        private List<string> GetSubParameters()
        {
            var subParameters = new List<string>();

            var axisConfiguration =
                (AxisConfigurationType)Convert.ToByte(ModifiedCIPAxis.AxisConfiguration);

            var feedbackConfiguration =
                (FeedbackConfigurationType)Convert.ToByte(ModifiedCIPAxis.FeedbackConfiguration);

            var associatedModule = ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule;

            switch (axisConfiguration)
            {
                case AxisConfigurationType.FeedbackOnly:
                    subParameters.AddRange(new[]
                        { "Master Feedback", "Scaling", "Polarity", "Planner", "Homing", "Actions", "Exceptions" });
                    break;

                case AxisConfigurationType.PositionLoop:
                    if (associatedModule != null)
                    {
                        subParameters.AddRange(new[]
                        {
                            "Motor", "Model", "Motor Feedback"
                        });
                        if (feedbackConfiguration == FeedbackConfigurationType.LoadFeedback ||
                            feedbackConfiguration == FeedbackConfigurationType.DualFeedback)
                            subParameters.Add("Load Feedback");

                        subParameters.AddRange(new[]
                        {
                            "Scaling", "Polarity", "Load", "Backlash", "Compliance", "Friction", "Observer",
                            "Position Loop", "Velocity Loop", "Acceleration Loop", "Torque/Current Loop", "Planner",
                            "Homing", "Actions", "Exceptions"
                        });
                    }
                    else
                    {
                        subParameters.AddRange(new[]
                        {
                            "Motor", "Motor Feedback", "Scaling", "Polarity", "Load", "Backlash",
                            "Position Loop", "Velocity Loop", "Torque/Current Loop", "Planner",
                            "Homing", "Actions"
                        });
                    }


                    break;
                case AxisConfigurationType.VelocityLoop:
                    subParameters.AddRange(new[]
                    {
                        "Motor", "Model", "Motor Feedback"
                    });
                    if (feedbackConfiguration == FeedbackConfigurationType.LoadFeedback ||
                        feedbackConfiguration == FeedbackConfigurationType.DualFeedback)
                        subParameters.Add("Load Feedback");
                    subParameters.AddRange(new[]
                    {
                        "Scaling", "Polarity", "Load", "Compliance", "Friction", "Observer",
                        "Velocity Loop", "Acceleration Loop", "Torque/Current Loop", "Planner",
                        "Homing", "Actions", "Exceptions"
                    });
                    break;
                case AxisConfigurationType.TorqueLoop:
                    subParameters.AddRange(new[]
                    {
                        "Motor", "Model", "Motor Feedback",
                        "Scaling", "Polarity", "Load", "Compliance", "Friction", "Observer",
                        "Torque/Current Loop", "Planner",
                        "Homing", "Actions", "Exceptions"
                    });
                    break;

                case AxisConfigurationType.FrequencyControl:
                    subParameters.AddRange(new[]
                    {
                        "Motor", "Model", "Scaling", "Polarity",
                        "Planner", "Frequency Control",
                        "Actions", "Exceptions"
                    });
                    break;
            }

            return subParameters;
        }

        private bool Filter(object obj)
        {
            if (string.IsNullOrEmpty(_parameterGroup))
                return true;

            ParameterItem item = obj as ParameterItem;

            if (item != null)
            {
                Contract.Assert(!string.IsNullOrEmpty(item.Category));

                if (string.Equals(_parameterGroup, "All"))
                {
                    var subParameters = GetSubParameters();

                    if (item.Category.Contains(","))
                    {
                        var categories = item.Category.Split(',');
                        foreach (var category in categories)
                        {
                            if (subParameters.Contains(category))
                                return true;
                        }
                    }
                    else if (subParameters.Contains(item.Category))
                    {
                        return true;
                    }
                }
                else
                {
                    if (item.Category.Contains(","))
                    {
                        var categories = item.Category.Split(',');
                        foreach (var category in categories)
                        {
                            if (category.Equals(_parameterGroup))
                                return true;
                        }
                    }
                    else if (item.Category.Equals(_parameterGroup))
                    {
                        return true;
                    }
                }
                
            }

            return false;
        }

        private void OnParameterChanged(object sender, PropertyChangedEventArgs args)
        {
            //TODO(gjc): need edit here
            Collection.Refresh();

            CheckDirty();

        }

        #endregion

    }
}