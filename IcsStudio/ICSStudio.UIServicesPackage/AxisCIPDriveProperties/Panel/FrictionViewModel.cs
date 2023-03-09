using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.Objects;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class FrictionViewModel : DefaultViewModel
    {
        public FrictionViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {

            CompareProperties = new[]
            {
                "FrictionCompensationSliding", "FrictionCompensationWindow"
            };

            PeriodicRefreshProperties = new[]
            {
                "FrictionCompensationSliding", "FrictionCompensationWindow"
            };

            ParametersCommand = new RelayCommand(ExecuteParametersCommand);
        }

        public bool IsFrictionEnabled
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

                if (ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule == null)
                    return Visibility.Collapsed;

                if (axisConfiguration == AxisConfigurationType.PositionLoop
                    || axisConfiguration == AxisConfigurationType.VelocityLoop
                    || axisConfiguration == AxisConfigurationType.TorqueLoop)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public float FrictionCompensationSliding
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.FrictionCompensationSliding); }
            set
            {
                ModifiedCIPAxis.FrictionCompensationSliding = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public Visibility FrictionCompensationWindowVisibility
        {
            get
            {
                var axisConfiguration = (AxisConfigurationType) Convert.ToByte(ModifiedCIPAxis.AxisConfiguration);
                if (axisConfiguration == AxisConfigurationType.PositionLoop)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public float FrictionCompensationWindow
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.FrictionCompensationWindow); }
            set
            {
                ModifiedCIPAxis.FrictionCompensationWindow = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public string PositionUnits => ModifiedCIPAxis.PositionUnits.GetString();

        public RelayCommand ParametersCommand { get; }

        private void ExecuteParametersCommand()
        {
            ParentViewModel.ShowPanel("Parameter List", "Friction");
        }

        private void UIVisibilityAndReadonly()
        {
            RaisePropertyChanged("IsFrictionEnabled");
        }

        private void UIRefresh()
        {
            RaisePropertyChanged("PositionUnits");
            RaisePropertyChanged("FrictionCompensationWindowVisibility");

            RaisePropertyChanged("FrictionCompensationSliding");
            RaisePropertyChanged("FrictionCompensationWindow");

        }
    }
}