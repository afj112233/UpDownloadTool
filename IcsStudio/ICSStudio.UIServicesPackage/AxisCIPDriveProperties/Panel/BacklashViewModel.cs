using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.Cip.Objects;
using ICSStudio.Dialogs.Warning;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class BacklashViewModel : DefaultViewModel
    {
        public BacklashViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            CompareProperties = new[]
            {
                "BacklashReversalOffset", "BacklashCompensationWindow"
            };

            PeriodicRefreshProperties = new[]
            {
                "BacklashCompensationWindow"
            };
        }

        public bool IsBacklashEnabled
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

                if (axisConfiguration == AxisConfigurationType.PositionLoop)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public string PositionUnits => ModifiedCIPAxis.PositionUnits.GetString();

        public float BacklashReversalOffset
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.BacklashReversalOffset); }
            set
            {
                ModifiedCIPAxis.BacklashReversalOffset = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public Visibility CompensationWindowVisibility =>
            ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule == null
                ? Visibility.Hidden
                : Visibility.Visible;

        public float BacklashCompensationWindow
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.BacklashCompensationWindow); }
            set
            {
                ModifiedCIPAxis.BacklashCompensationWindow = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public override int CheckValid()
        {
            int result = 0;

            string message = string.Empty;
            string reason = string.Empty;
            string errorCode = string.Empty;

            var motionResolution = Convert.ToUInt32(ModifiedCIPAxis.MotionResolution);
            double travelRangeLimit = (double) int.MaxValue / motionResolution;

            double maxValue = travelRangeLimit;
            double minValue = 0;

            if (!(BacklashReversalOffset >= minValue && BacklashReversalOffset <= maxValue))
            {
                message = $"Failed to modify properties for axis '{ParentViewModel.AxisTag.Name}'";
                reason = $"Enter a BacklashReversalOffset between {minValue:F2} and {maxValue:F2}.";
                errorCode = "Error 16358-80042034";

                result = -1;
            }

            if (result < 0)
            {
                // show page
                ParentViewModel.ShowPanel("Backlash");

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

        #region Private

        private void UIRefresh()
        {
            RaisePropertyChanged("PositionUnits");
            RaisePropertyChanged("BacklashReversalOffset");
            RaisePropertyChanged("BacklashCompensationWindow");
        }

        private void UIVisibilityAndReadonly()
        {
            RaisePropertyChanged("CompensationWindowVisibility");

            RaisePropertyChanged("IsBacklashEnabled");
        }

        #endregion
    }
}