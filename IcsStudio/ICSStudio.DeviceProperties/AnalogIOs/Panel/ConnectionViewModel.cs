using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.DeviceProfiles.Common;
using ICSStudio.DeviceProfiles.DIOModule;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties.AnalogIOs.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class ConnectionViewModel : DeviceOptionPanel
    {
        private uint _maxRPI;

        private uint _minRPI;

        private string _moduleFault;
        private bool _rpiEnable;

        private Visibility _unicastVisibility;
        private Visibility _validationRangeVisibility;

        public ConnectionViewModel(UserControl control, ModifiedAnalogIO modifiedDiscreteIO) : base(control)
        {
            ModifiedAnalogIO = modifiedDiscreteIO;

            UpdateConnection();
        }

        public ModifiedAnalogIO ModifiedAnalogIO { get; }

        public AnalogIO OriginalAnalogIO => ModifiedAnalogIO?.OriginalAnalogIO;

        public DIOModuleProfiles Profiles => OriginalAnalogIO?.Profiles;

        public bool RPIEnable
        {
            get { return _rpiEnable; }
            set { Set(ref _rpiEnable, value); }
        }

        public float MinRPI
        {
            get { return _minRPI / 1000f; }
            set { Set(ref _minRPI, (uint) (value * 1000)); }
        }

        public float MaxRPI
        {
            get { return _maxRPI / 1000f; }
            set { Set(ref _maxRPI, (uint) (value * 1000)); }
        }

        public float RPI
        {
            get { return ModifiedAnalogIO.RPI / 1000f; }
            set
            {
                if (ModifiedAnalogIO.RPI != (uint) (value * 1000))
                {
                    ModifiedAnalogIO.RPI = (uint) (value * 1000);
                    RaisePropertyChanged();
                    CheckDirty();
                }
            }
        }

        public string ValidationRange => $"({MinRPI:F1} - {MaxRPI:F1})";

        public Visibility ValidationRangeVisibility
        {
            get { return _validationRangeVisibility; }
            set { Set(ref _validationRangeVisibility, value); }
        }

        public bool Inhibited
        {
            get { return ModifiedAnalogIO.Inhibited; }
            set
            {
                if (ModifiedAnalogIO.Inhibited != value)
                {
                    ModifiedAnalogIO.Inhibited = value;
                    RaisePropertyChanged();

                    CheckDirty();
                }
            }
        }

        public bool IsInhibitedEnabled => !IsOnline;

        public bool MajorFault
        {
            get { return ModifiedAnalogIO.MajorFault; }
            set
            {
                if (ModifiedAnalogIO.MajorFault != value)
                {
                    ModifiedAnalogIO.MajorFault = value;
                    RaisePropertyChanged();
                    CheckDirty();
                }
            }
        }

        public bool Unicast
        {
            get { return ModifiedAnalogIO.Unicast; }
            set
            {
                if (ModifiedAnalogIO.Unicast != value)
                {
                    ModifiedAnalogIO.Unicast = value;
                    RaisePropertyChanged();
                    CheckDirty();
                }
            }
        }

        public Visibility UnicastVisibility
        {
            get { return _unicastVisibility; }
            set { Set(ref _unicastVisibility, value); }
        }

        public string ModuleFault
        {
            get { return _moduleFault; }
            set { Set(ref _moduleFault, value); }
        }

        public override void Show()
        {
            UpdateConnection();
        }

        public override void CheckDirty()
        {
            if (RPIEnable)
            {
                if (OriginalAnalogIO.Communications?.Connections == null)
                {
                    IsDirty = true;
                    return;
                }

                if (ModifiedAnalogIO.RPI != OriginalAnalogIO.Communications.Connections[0].RPI)
                {
                    IsDirty = true;
                    return;
                }
            }

            if (UnicastVisibility == Visibility.Visible)
            {
                if (ModifiedAnalogIO.Unicast !=
                    OriginalAnalogIO.Communications.Connections[0].Unicast.GetValueOrDefault())
                {
                    IsDirty = true;
                    return;
                }
            }

            if (ModifiedAnalogIO.Inhibited != OriginalAnalogIO.Inhibited)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedAnalogIO.MajorFault != OriginalAnalogIO.MajorFault)
            {
                IsDirty = true;
                return;
            }

            IsDirty = false;
        }

        public override bool SaveOptions()
        {
            UpdateConnection();

            if (RPIEnable)
            {
                OriginalAnalogIO.Communications.Connections[0].RPI = ModifiedAnalogIO.RPI;
            }

            if (UnicastVisibility == Visibility.Visible)
            {
                OriginalAnalogIO.Communications.Connections[0].Unicast = ModifiedAnalogIO.Unicast;
            }

            OriginalAnalogIO.Inhibited = ModifiedAnalogIO.Inhibited;
            OriginalAnalogIO.MajorFault = ModifiedAnalogIO.MajorFault;

            return true;
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateConnection();

            RaisePropertyChanged(nameof(IsInhibitedEnabled));
        }

        private void UpdateConnection()
        {
            var definition =
                Profiles.DIOModuleTypes.GetConnectionConfigDefinitionByID(ModifiedAnalogIO
                    .ConnectionConfigID);

            var connectionDefinition =
                Profiles.DIOModuleTypes.GetConnectionDefinitionByID(definition.Connections[0]);

            if (connectionDefinition.RPI.HasValue)
            {
                RPIEnable = !IsOnline;

                _minRPI = connectionDefinition.MinRPI.GetValueOrDefault();
                _maxRPI = connectionDefinition.MaxRPI.GetValueOrDefault();

                var drivePort = GetDrivePortInParentModule();
                if (drivePort != null)
                {
                    _minRPI = (uint) Math.Max(drivePort.ExtendedProperties.MinRPIRestriction, _minRPI);
                    _maxRPI = (uint) Math.Min(drivePort.ExtendedProperties.MaxRPIRestriction, _maxRPI);
                }

                ValidationRangeVisibility = Visibility.Visible;

                UnicastVisibility =
                    OriginalAnalogIO.ParentModule is LocalModule ? Visibility.Collapsed : Visibility.Visible;

                RaisePropertyChanged("MinRPI");
                RaisePropertyChanged("MaxRPI");
                RaisePropertyChanged("RPI");
                RaisePropertyChanged("ValidationRange");
            }
            else
            {
                RPIEnable = false;
                ValidationRangeVisibility = Visibility.Collapsed;
                UnicastVisibility = Visibility.Collapsed;

                // use parent rpi
                float rpt = GetParentModuleRPI();
                MinRPI = rpt;
                MaxRPI = rpt;
                RPI = rpt;
            }

            RaisePropertyChanged("IsInhibitedEnabled");
        }

        private float GetParentModuleRPI()
        {
            CommunicationsAdapter adapter = OriginalAnalogIO.ParentModule as CommunicationsAdapter;
            if (adapter != null)
            {
                return adapter.Communications.Connections[0].RPI / 1000f;
            }

            return 0;
        }

        private DrivePort GetDrivePortInParentModule()
        {
            CommunicationsAdapter adapter = OriginalAnalogIO.ParentModule as CommunicationsAdapter;
            return adapter?.GetDrivePortInProfiles(PortType.PointIO.ToString());
        }
    }
}
