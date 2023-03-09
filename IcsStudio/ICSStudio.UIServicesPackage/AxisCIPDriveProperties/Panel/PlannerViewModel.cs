using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.Objects;
using ICSStudio.UIServicesPackage.View;
using ICSStudio.UIServicesPackage.ViewModel;
using ICSStudio.Utils;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class PlannerViewModel : DefaultViewModel
    {
        public PlannerViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            CompareProperties = new[]
            {
                "MaximumSpeed", "MaximumAcceleration",
                "MaximumDeceleration", "MaximumAccelerationJerk",
                "MaximumDecelerationJerk",
                "AverageVelocityTimebase",
                //TODO(gjc): need add
            };

            PeriodicRefreshProperties = new[]
            {
                "MaximumSpeed",
                "MaximumAcceleration",
                "MaximumDeceleration"
            };

            CalculateAccelerationCommand =
                new RelayCommand(ExecuteCalculateAccelerationCommand, CanCalculateAccelerationCommand);
            AccelJerkCalculateCommand =
                new RelayCommand(ExecuteAccelJerkCalculateCommand, CanAccelJerkCalculateCommand);
            DecelJerkCalculateCommand =
                new RelayCommand(ExecuteDecelJerkCalculateCommand, CanDecelJerkCalculateCommand);
            ParametersCommand = new RelayCommand(ExecuteParametersCommand);
        }

        public bool IsPlannerEnabled
        {
            get
            {
                if (ParentViewModel.IsPowerStructureEnabled)
                    return false;

                if (ParentViewModel.IsHardRunMode)
                    return false;

                return true;
            }
        }

        public override void Show()
        {
            UIVisibilityAndReadonly();
            UIRefresh();
        }

        public override Visibility Visibility
        {
            get
            {
                var axisConfiguration =
                    (AxisConfigurationType) Convert.ToByte(ParentViewModel.ModifiedAxisCIPDrive.CIPAxis
                        .AxisConfiguration);

                if (axisConfiguration == AxisConfigurationType.PositionLoop
                    || axisConfiguration == AxisConfigurationType.VelocityLoop
                    || axisConfiguration == AxisConfigurationType.FrequencyControl)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public float MaximumSpeed
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.MaximumSpeed); }
            set
            {
                ModifiedCIPAxis.MaximumSpeed = value;

                AccelJerkCalculateCommand.RaiseCanExecuteChanged();
                DecelJerkCalculateCommand.RaiseCanExecuteChanged();
                CalculateAccelerationCommand.RaiseCanExecuteChanged();

                RaisePropertyChanged("AccelPercentTime");
                RaisePropertyChanged("DecelPercentTime");

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float MaximumAcceleration
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.MaximumAcceleration); }
            set
            {
                ModifiedCIPAxis.MaximumAcceleration = value;

                AccelJerkCalculateCommand.RaiseCanExecuteChanged();
                CalculateAccelerationCommand.RaiseCanExecuteChanged();

                RaisePropertyChanged("AccelPercentTime");

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float MaximumDeceleration
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.MaximumDeceleration); }
            set
            {
                ModifiedCIPAxis.MaximumDeceleration = value;

                DecelJerkCalculateCommand.RaiseCanExecuteChanged();
                CalculateAccelerationCommand.RaiseCanExecuteChanged();

                RaisePropertyChanged("DecelPercentTime");

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float MaximumAccelerationJerk
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.MaximumAccelerationJerk); }
            set
            {
                ModifiedCIPAxis.MaximumAccelerationJerk = value;

                RaisePropertyChanged("AccelPercentTime");

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float MaximumDecelerationJerk
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.MaximumDecelerationJerk); }
            set
            {
                ModifiedCIPAxis.MaximumDecelerationJerk = value;

                RaisePropertyChanged("DecelPercentTime");

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public string PositionUnits => ModifiedCIPAxis.PositionUnits.GetString();
        public string AccelPercentTime => GetPercentTime(MaximumSpeed, MaximumAcceleration, MaximumAccelerationJerk);
        public string DecelPercentTime => GetPercentTime(MaximumSpeed, MaximumDeceleration, MaximumDecelerationJerk);

        public RelayCommand ParametersCommand { get; }
        public RelayCommand AccelJerkCalculateCommand { get; }
        public RelayCommand DecelJerkCalculateCommand { get; }
        public RelayCommand CalculateAccelerationCommand { get; }

        #region Command

        private bool CanCalculateAccelerationCommand()
        {
            if (MaximumSpeed < float.Epsilon || MaximumAcceleration < float.Epsilon || MaximumDeceleration < float.Epsilon)
                return false;

            return true;
        }

        private void ExecuteCalculateAccelerationCommand()
        {
            var dialog = new DynamicsDialog();
            var vm = new DynamicsViewModel(PositionUnits,MaximumSpeed, MaximumAcceleration, MaximumAccelerationJerk);
            var applyAction = new Action(delegate
            {
                MaximumSpeed = Convert.ToSingle(vm.MaximumVelocity);
                MaximumAcceleration = Convert.ToSingle(vm.Acceleration);
                MaximumDeceleration = Convert.ToSingle(vm.Deceleration);
                MaximumAccelerationJerk = Convert.ToSingle(vm.Jerk);
                if (vm.IsDirectAsAbove || vm.IsIndirectAsAbove)
                    MaximumDecelerationJerk = MaximumAccelerationJerk;
            });
            vm.ApplyAction += applyAction;
            dialog.DataContext = vm;
            dialog.Owner = Application.Current.MainWindow;
            if (dialog.ShowDialog() ?? false)
            {
                if (vm.CanApplyCommand())
                {
                    applyAction?.Invoke();
                }
                vm.ApplyAction -= applyAction;
            }
        }

        private bool CanDecelJerkCalculateCommand()
        {
            if (MaximumSpeed < float.Epsilon || MaximumDeceleration < float.Epsilon)
                return false;

            return true;
        }

        private void ExecuteDecelJerkCalculateCommand()
        {
            var dialog = new CalculateJerkDialog(
                    "Maximum Deceleration Jerk",
                    PositionUnits,
                    MaximumSpeed, MaximumDeceleration, MaximumDecelerationJerk)
                {Owner = Application.Current.MainWindow};

            var dialogResult = dialog.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
                MaximumDecelerationJerk = (float)dialog.Jerk;
        }

        private bool CanAccelJerkCalculateCommand()
        {
            if (MaximumSpeed < float.Epsilon || MaximumAcceleration < float.Epsilon)
                return false;

            return true;
        }

        private void ExecuteAccelJerkCalculateCommand()
        {
            var dialog = new CalculateJerkDialog(
                    "Maximum Acceleration Jerk",
                    PositionUnits,
                    MaximumSpeed, MaximumAcceleration, MaximumAccelerationJerk)
                {Owner = Application.Current.MainWindow};

            var dialogResult = dialog.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
                MaximumAccelerationJerk = (float)dialog.Jerk;
        }

        private void ExecuteParametersCommand()
        {
            ParentViewModel.ShowPanel("Parameter List", "Planner");
        }

        #endregion

        #region Private

        private string GetPercentTime(
            float speed,
            float accel,
            float jerk)
        {
            if (speed < float.Epsilon || accel < float.Epsilon)
                return "< 1%";

            var percentTime = JerkRateCalculation.CalculatePercentTime(speed, accel, jerk);
            if (percentTime < 1)
                return "< 1%";
            if (percentTime > 100)
                return "> 100%";

            var result = (int) percentTime;
            return $"= {result}%";
        }

        private void UIVisibilityAndReadonly()
        {
            RaisePropertyChanged("IsPlannerEnabled");
        }

        private void UIRefresh()
        {
            RaisePropertyChanged("AccelPercentTime");
            RaisePropertyChanged("DecelPercentTime");
            RaisePropertyChanged("MaximumSpeed");
            RaisePropertyChanged("MaximumAcceleration");
            RaisePropertyChanged("MaximumDeceleration");
            RaisePropertyChanged("MaximumAccelerationJerk");
            RaisePropertyChanged("MaximumDecelerationJerk");

            RaisePropertyChanged("PositionUnits");

            AccelJerkCalculateCommand.RaiseCanExecuteChanged();
            DecelJerkCalculateCommand.RaiseCanExecuteChanged();
        }

        #endregion
    }
}