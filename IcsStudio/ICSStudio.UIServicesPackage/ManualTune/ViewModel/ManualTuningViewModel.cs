using System;
using System.Collections;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Utils;
using ICSStudio.SimpleServices.DataWrapper;

namespace ICSStudio.UIServicesPackage.ManualTune.ViewModel
{
    public class ManualTuningViewModel : ViewModelBase
    {
        private readonly ManualTuneViewModel _parentViewModel;
        private bool _tuningConfigurationChecked;
        private Visibility _tuningConfigurationVisibility;

        public ManualTuningViewModel(ManualTuneViewModel parentViewModel)
        {
            _parentViewModel = parentViewModel;
            
            _tuningConfigurationChecked = false;
            _tuningConfigurationVisibility = Visibility.Collapsed;

            PositionIntegratorHoldSource = EnumHelper.ToDataSource<BooleanType>();
            VelocityIntegratorHoldSource = EnumHelper.ToDataSource<BooleanType>();

            ResetCommand = new RelayCommand(ExecuteResetCommand, CanExecuteResetCommand);
        }

        public AxisCIPDrive ModifiedAxisCIPDrive => _parentViewModel.ModifiedAxisCIPDrive;

        public bool EditEnabled => ModifiedAxisCIPDrive.Controller.IsOnline;

        public string PositionUnits => ModifiedAxisCIPDrive.CIPAxis.PositionUnits.GetString();

        public float SystemBandwidth
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.SystemBandwidth); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.SystemBandwidth) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.SystemBandwidth = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public float SystemDamping
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.SystemDamping); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.SystemDamping) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.SystemDamping = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public bool TuningConfigurationChecked
        {
            get { return _tuningConfigurationChecked; }
            set
            {
                Set(ref _tuningConfigurationChecked, value);
                TuningConfigurationVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility TuningConfigurationVisibility
        {
            get { return _tuningConfigurationVisibility; }
            set { Set(ref _tuningConfigurationVisibility, value); }
        }

        public string ApplicationType
        {
            get
            {
                var applicationType = (ApplicationType) Convert.ToByte(ModifiedAxisCIPDrive.CIPAxis.ApplicationType);
                return EnumHelper.GetEnumMember(applicationType);
            }
        }

        public string LoadCoupling
        {
            get
            {
                var loadCoupling = (LoadCouplingType) Convert.ToByte(ModifiedAxisCIPDrive.CIPAxis.LoadCoupling);
                return EnumHelper.GetEnumMember(loadCoupling);
            }
        }

        public bool TunePosIntegratorChecked
        {
            get
            {
                var bits = Convert.ToUInt16(ModifiedAxisCIPDrive.CIPAxis.GainTuningConfigurationBits);
                return FlagsEnumHelper.ContainFlag(bits,
                    GainTuningConfigurationType.TunePosIntegrator);
            }
        }

        public bool TuneVelIntegratorChecked
        {
            get
            {
                var bits = Convert.ToUInt16(ModifiedAxisCIPDrive.CIPAxis.GainTuningConfigurationBits);
                return FlagsEnumHelper.ContainFlag(bits,
                    GainTuningConfigurationType.TuneVelIntegrator);
            }
        }


        public float PositionLoopBandwidth
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.PositionLoopBandwidth); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.PositionLoopBandwidth) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.PositionLoopBandwidth = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public float PositionIntegratorBandwidth
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.PositionIntegratorBandwidth); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.PositionIntegratorBandwidth) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.PositionIntegratorBandwidth = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public IList PositionIntegratorHoldSource { get; }

        public BooleanType PositionIntegratorHold
        {
            get
            {
                var bits = Convert.ToByte(ModifiedAxisCIPDrive.CIPAxis.PositionIntegratorControl);
                return (BooleanType) Convert.ToByte(FlagsEnumHelper.ContainFlag(bits,
                    IntegratorControlBitmap.IntegratorHoldEnable));
            }
            set
            {
                var bits = Convert.ToByte(ModifiedAxisCIPDrive.CIPAxis.PositionIntegratorControl);
                FlagsEnumHelper.SelectFlag(IntegratorControlBitmap.IntegratorHoldEnable, Convert.ToBoolean(value),
                    ref bits);

                ModifiedAxisCIPDrive.CIPAxis.PositionIntegratorControl = bits;

                _parentViewModel.SetAttributeSingle();

                RaisePropertyChanged();
            }
        }

        public float PositionErrorTolerance
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.PositionErrorTolerance); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.PositionErrorTolerance) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.PositionErrorTolerance = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public float VelocityLoopBandwidth
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.VelocityLoopBandwidth); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.VelocityLoopBandwidth) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.VelocityLoopBandwidth = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public float VelocityIntegratorBandwidth
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.VelocityIntegratorBandwidth); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.VelocityIntegratorBandwidth) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.VelocityIntegratorBandwidth = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        public IList VelocityIntegratorHoldSource { get; }

        public BooleanType VelocityIntegratorHold
        {
            get
            {
                var bits = Convert.ToByte(ModifiedAxisCIPDrive.CIPAxis.VelocityIntegratorControl);
                return (BooleanType) Convert.ToByte(FlagsEnumHelper.ContainFlag(bits,
                    IntegratorControlBitmap.IntegratorHoldEnable));
            }
            set
            {
                var bits = Convert.ToByte(ModifiedAxisCIPDrive.CIPAxis.VelocityIntegratorControl);
                FlagsEnumHelper.SelectFlag(IntegratorControlBitmap.IntegratorHoldEnable, Convert.ToBoolean(value),
                    ref bits);

                ModifiedAxisCIPDrive.CIPAxis.VelocityIntegratorControl = bits;

                _parentViewModel.SetAttributeSingle();

                RaisePropertyChanged();
            }
        }

        public float VelocityErrorTolerance
        {
            get { return Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.VelocityErrorTolerance); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedAxisCIPDrive.CIPAxis.VelocityErrorTolerance) - value) >
                    float.Epsilon)
                {
                    ModifiedAxisCIPDrive.CIPAxis.VelocityErrorTolerance = value;

                    _parentViewModel.SetAttributeSingle();

                    RaisePropertyChanged();
                }
            }
        }

        #region Command

        public RelayCommand ResetCommand { get; }

        private bool CanExecuteResetCommand()
        {
            //TODO(gjc): add code here
            return false;
        }

        private void ExecuteResetCommand()
        {
            //TODO(gjc): add code here
        }

        #endregion

        public void Refresh()
        {
            RaisePropertyChanged(nameof(EditEnabled));
            RaisePropertyChanged(nameof(PositionUnits));
            RaisePropertyChanged(nameof(SystemBandwidth));
            RaisePropertyChanged(nameof(SystemDamping));
            RaisePropertyChanged(nameof(TuningConfigurationChecked));
            RaisePropertyChanged(nameof(TuningConfigurationVisibility));
            RaisePropertyChanged(nameof(ApplicationType));
            RaisePropertyChanged(nameof(LoadCoupling));
            RaisePropertyChanged(nameof(TunePosIntegratorChecked));
            RaisePropertyChanged(nameof(TuneVelIntegratorChecked));
            RaisePropertyChanged(nameof(PositionLoopBandwidth));
            RaisePropertyChanged(nameof(PositionIntegratorBandwidth));
            RaisePropertyChanged(nameof(PositionIntegratorHold));
            RaisePropertyChanged(nameof(PositionErrorTolerance));
            RaisePropertyChanged(nameof(VelocityLoopBandwidth));
            RaisePropertyChanged(nameof(VelocityIntegratorBandwidth));
            RaisePropertyChanged(nameof(VelocityIntegratorHold));
            RaisePropertyChanged(nameof(VelocityErrorTolerance));
        }

    }
}
