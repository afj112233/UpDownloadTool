using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.Cip.Objects;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class AccelerationLoopViewModel : DefaultViewModel
    {
        public AccelerationLoopViewModel(
            UserControl panel,
            AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            CompareProperties = new[]
            {
                "AccelerationLimit", "DecelerationLimit"
            };

            PeriodicRefreshProperties = new[]
            {
                "AccelerationLimit", "DecelerationLimit"
            };
        }

        public bool IsAccelerationLoopEnabled
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
                    || axisConfiguration == AxisConfigurationType.VelocityLoop)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public float AccelerationLimit
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.AccelerationLimit); }
            set
            {
                ModifiedCIPAxis.AccelerationLimit = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public string PositionUnits => ModifiedCIPAxis.PositionUnits.GetString();

        public float DecelerationLimit
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.DecelerationLimit); }
            set
            {
                ModifiedCIPAxis.DecelerationLimit = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        private void UIVisibilityAndReadonly()
        {
            RaisePropertyChanged("IsAccelerationLoopEnabled");
        }

        private void UIRefresh()
        {
            RaisePropertyChanged("AccelerationLimit");
            RaisePropertyChanged("DecelerationLimit");
            RaisePropertyChanged("PositionUnits");
        }
    }
}