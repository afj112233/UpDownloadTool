using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Utils;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class ObserverViewModel : DefaultViewModel
    {
        private IList _loadObserverConfigurationSource;

        public ObserverViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            CompareProperties = new[]
            {
                "LoadObserverConfiguration",
                "LoadObserverBandwidth",
                "LoadObserverIntegratorBandwidth"
            };

            PeriodicRefreshProperties = new[]
            {
                "LoadObserverConfiguration",
                "LoadObserverBandwidth",
                "LoadObserverIntegratorBandwidth"
            };

            ParametersCommand = new RelayCommand(ExecuteParametersCommand);

            UpdateLoadObserverConfigurationSource();
        }

        public bool IsObserverEnabled
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
            UpdateLoadObserverConfigurationSource();

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

        public IList LoadObserverConfigurationSource
        {
            get { return _loadObserverConfigurationSource; }
            set { Set(ref _loadObserverConfigurationSource, value); }
        }

        public LoadObserverConfigurationType LoadObserverConfiguration
        {
            get
            {
                return
                    (LoadObserverConfigurationType) Convert.ToByte(ModifiedCIPAxis.LoadObserverConfiguration);
            }
            set
            {
                ModifiedCIPAxis.LoadObserverConfiguration = (byte) value;

                // 
                UIVisibilityAndReadonly();

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public bool ConfigurationEnabled => LoadObserverConfiguration != LoadObserverConfigurationType.Disabled;

        public float LoadObserverBandwidth
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.LoadObserverBandwidth); }
            set
            {
                ModifiedCIPAxis.LoadObserverBandwidth = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float LoadObserverIntegratorBandwidth
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.LoadObserverIntegratorBandwidth); }
            set
            {
                ModifiedCIPAxis.LoadObserverIntegratorBandwidth = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public RelayCommand ParametersCommand { get; }

        private void ExecuteParametersCommand()
        {
            ParentViewModel.ShowPanel("Parameter List", "Observer");
        }

        #region Private

        private void UpdateLoadObserverConfigurationSource()
        {
            var supportTypes = new List<LoadObserverConfigurationType>
            {
                LoadObserverConfigurationType.Disabled,
                LoadObserverConfigurationType.LoadObserverOnly,
                LoadObserverConfigurationType.LoadObserverWithVelocityEstimate,
                LoadObserverConfigurationType.VelocityEstimateOnly
            };

            var oldLoadObserverConfiguration = LoadObserverConfiguration;


            LoadObserverConfigurationSource = EnumHelper.ToDataSource<LoadObserverConfigurationType>(supportTypes);

            if (!supportTypes.Contains(oldLoadObserverConfiguration))
                ModifiedCIPAxis.LoadObserverConfiguration = (byte) supportTypes[0];
            else
                ModifiedCIPAxis.LoadObserverConfiguration = (byte) oldLoadObserverConfiguration;

            RaisePropertyChanged("LoadObserverConfiguration");
        }

        private void UIVisibilityAndReadonly()
        {
            RaisePropertyChanged("IsObserverEnabled");
            RaisePropertyChanged("ConfigurationEnabled");
        }

        private void UIRefresh()
        {
            RaisePropertyChanged("LoadObserverConfiguration");
            RaisePropertyChanged("LoadObserverBandwidth");
            RaisePropertyChanged("LoadObserverIntegratorBandwidth");
        }

        #endregion

    }
}