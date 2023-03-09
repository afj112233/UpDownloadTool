using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.Objects;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Gui.Utils;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class ComplianceViewModel : DefaultViewModel
    {
        public ComplianceViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            CompareProperties = new[]
            {
                "TorqueLowPassFilterBandwidth", "TorqueNotchFilterFrequency",
                "TorqueLeadLagFilterGain", "TorqueLeadLagFilterBandwidth",
                "AdaptiveTuningConfiguration",
                "TorqueNotchFilterHighFrequencyLimit",
                "TorqueNotchFilterLowFrequencyLimit",
                "TorqueNotchFilterTuningThreshold"
            };

            PeriodicRefreshProperties = new[]
            {
                "TorqueNotchFilterFrequency",
                "TorqueNotchFilterHighFrequencyLimit",
                "TorqueNotchFilterLowFrequencyLimit",
                "TorqueNotchFilterTuningThreshold"
            };

            AdaptiveTuningConfigurationSource =
                EnumHelper.ToObservableCollectionSource<AdaptiveTuningConfigurationType>();

            ParametersCommand = new RelayCommand(ExecuteParametersCommand);
        }

        public bool IsComplianceEnabled
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

        public RelayCommand ParametersCommand { get; }

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

        public override int CheckValid()
        {
            int result = 0;

            string message = string.Empty;
            string reason = string.Empty;
            string errorCode = string.Empty;


            //TorqueNotchFilterHighFrequencyLimit
            double maxValue = 4000;
            double minValue = TorqueNotchFilterLowFrequencyLimit;

            if (!(TorqueNotchFilterHighFrequencyLimit >= minValue && TorqueNotchFilterHighFrequencyLimit <= maxValue))
            {
                message = $"Failed to modify properties for axis '{ParentViewModel.AxisTag.Name}'";
                reason = $"Enter a TorqueNotchFilterHighFrequencyLimit between {minValue:F2} and {maxValue:F2}.";
                errorCode = "Error 16358-80042034";

                result = -1;
            }

            if (result < 0)
            {
                // show page
                ParentViewModel.ShowPanel("Compliance");

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

        public float TorqueLowPassFilterBandwidth
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.TorqueLowPassFilterBandwidth); }
            set
            {
                ModifiedCIPAxis.TorqueLowPassFilterBandwidth = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float TorqueNotchFilterFrequency
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.TorqueNotchFilterFrequency); }
            set
            {
                ModifiedCIPAxis.TorqueNotchFilterFrequency = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float TorqueLeadLagFilterGain
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.TorqueLeadLagFilterGain); }
            set
            {
                ModifiedCIPAxis.TorqueLeadLagFilterGain = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float TorqueLeadLagFilterBandwidth
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.TorqueLeadLagFilterBandwidth); }
            set
            {
                ModifiedCIPAxis.TorqueLeadLagFilterBandwidth = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }
        
        public ObservableCollection<DisplayItem<AdaptiveTuningConfigurationType>> AdaptiveTuningConfigurationSource
        {
            get;
        }


        public AdaptiveTuningConfigurationType AdaptiveTuningConfiguration
        {
            get
            {
                return
                    (AdaptiveTuningConfigurationType) Convert.ToByte(ModifiedCIPAxis.AdaptiveTuningConfiguration);
            }

            set
            {
                ModifiedCIPAxis.AdaptiveTuningConfiguration = (byte) value;

                RaisePropertyChanged("AdaptiveTuningConfigurationEnabled");

                CheckDirty();

                RaisePropertyChanged();
            }
        }

        public float TorqueNotchFilterHighFrequencyLimit
        {
            get
            {
                return
                    Convert.ToSingle(ModifiedCIPAxis.TorqueNotchFilterHighFrequencyLimit);
            }
            set
            {
                ModifiedCIPAxis.TorqueNotchFilterHighFrequencyLimit = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float TorqueNotchFilterLowFrequencyLimit
        {
            get
            {
                return
                    Convert.ToSingle(ModifiedCIPAxis.TorqueNotchFilterLowFrequencyLimit);
            }
            set
            {
                ModifiedCIPAxis.TorqueNotchFilterLowFrequencyLimit = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float TorqueNotchFilterTuningThreshold
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.TorqueNotchFilterTuningThreshold); }
            set
            {
                ModifiedCIPAxis.TorqueNotchFilterTuningThreshold = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        private void UIVisibilityAndReadonly()
        {
            RaisePropertyChanged("IsComplianceEnabled");
            RaisePropertyChanged("AdaptiveTuningConfigurationEnabled");
        }

        private void UIRefresh()
        {
            RaisePropertyChanged("TorqueLowPassFilterBandwidth");
            RaisePropertyChanged("TorqueNotchFilterFrequency");
            RaisePropertyChanged("TorqueLeadLagFilterGain");
            RaisePropertyChanged("TorqueLeadLagFilterBandwidth");
            RaisePropertyChanged("AdaptiveTuningConfiguration");
            RaisePropertyChanged("TorqueNotchFilterHighFrequencyLimit");
            RaisePropertyChanged("TorqueNotchFilterLowFrequencyLimit");
            RaisePropertyChanged("TorqueNotchFilterTuningThreshold");
        }

        private void ExecuteParametersCommand()
        {
            ParentViewModel.ShowPanel("Parameter List", "Compliance");
        }
    }
}