using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.Objects;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class TorqueCurrentLoopViewModel : DefaultViewModel
    {
        public TorqueCurrentLoopViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            CompareProperties = new[]
            {
                "TorqueLoopBandwidth",
                "TorqueLimitPositive",
                "TorqueLimitNegative", "TorqueThreshold",
            };

            PeriodicRefreshProperties = new[]
            {
                "TorqueLoopBandwidth",
                "TorqueLimitPositive",
                "TorqueLimitNegative",
            };

            ParametersCommand = new RelayCommand(ExecuteParametersCommand);
        }

        public bool IsTorqueCurrentLoopEnabled
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
                    || axisConfiguration == AxisConfigurationType.TorqueLoop)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public Visibility GainsVisibility =>
            ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule == null
                ? Visibility.Hidden
                : Visibility.Visible;

        public float TorqueLoopBandwidth
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.TorqueLoopBandwidth); }
            set
            {
                ModifiedCIPAxis.TorqueLoopBandwidth = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float TorqueLimitPositive
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.TorqueLimitPositive); }
            set
            {
                ModifiedCIPAxis.TorqueLimitPositive = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float TorqueLimitNegative
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.TorqueLimitNegative); }
            set
            {
                ModifiedCIPAxis.TorqueLimitNegative = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }


        public RelayCommand ParametersCommand { get; }

        private void ExecuteParametersCommand()
        {
            ParentViewModel.ShowPanel("Parameter List", "Torque/Current Loop");
        }

        private void UIVisibilityAndReadonly()
        {
            RaisePropertyChanged("GainsVisibility");
            RaisePropertyChanged("IsTorqueCurrentLoopEnabled");
        }

        private void UIRefresh()
        {
            RaisePropertyChanged("TorqueLoopBandwidth");
            RaisePropertyChanged("TorqueLimitPositive");
            RaisePropertyChanged("TorqueLimitNegative");
        }
    }
}