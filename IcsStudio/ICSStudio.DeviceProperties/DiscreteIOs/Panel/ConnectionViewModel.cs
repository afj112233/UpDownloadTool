using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.DeviceProfiles.Common;
using ICSStudio.DeviceProfiles.DIOModule;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties.DiscreteIOs.Panel
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

        public ConnectionViewModel(UserControl control, ModifiedDiscreteIO modifiedDiscreteIO) : base(control)
        {
            ModifiedDiscreteIO = modifiedDiscreteIO;

            UpdateConnection();
        }

        public ModifiedDiscreteIO ModifiedDiscreteIO { get; }

        public DiscreteIO OriginalDiscreteIO => ModifiedDiscreteIO?.OriginalDiscreteIO;

        public DIOModuleProfiles Profiles => OriginalDiscreteIO?.Profiles;

        public bool IsMajorFaultOnControllerEnabled => !IsOnline;

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
            get { return ModifiedDiscreteIO.RPI / 1000f; }
            set
            {
                if (ModifiedDiscreteIO.RPI != (uint) (value * 1000))
                {
                    ModifiedDiscreteIO.RPI = (uint) (value * 1000);
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
            get { return ModifiedDiscreteIO.Inhibited; }
            set
            {
                if (ModifiedDiscreteIO.Inhibited != value)
                {
                    ModifiedDiscreteIO.Inhibited = value;
                    RaisePropertyChanged();

                    CheckDirty();
                }
            }
        }

        public bool IsInhibitedEnabled => !IsOnline;

        public bool MajorFault
        {
            get { return ModifiedDiscreteIO.MajorFault; }
            set
            {
                if (ModifiedDiscreteIO.MajorFault != value)
                {
                    ModifiedDiscreteIO.MajorFault = value;
                    RaisePropertyChanged();
                    CheckDirty();
                }
            }
        }

        public bool Unicast
        {
            get { return ModifiedDiscreteIO.Unicast; }
            set
            {
                if (ModifiedDiscreteIO.Unicast != value)
                {
                    ModifiedDiscreteIO.Unicast = value;
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
                if (OriginalDiscreteIO.Communications?.Connections == null)
                {
                    IsDirty = true;
                    return;
                }

                if (ModifiedDiscreteIO.RPI != OriginalDiscreteIO.Communications.Connections[0].RPI)
                {
                    IsDirty = true;
                    return;
                }
            }

            if (UnicastVisibility == Visibility.Visible)
            {
                if (ModifiedDiscreteIO.Unicast !=
                    OriginalDiscreteIO.Communications.Connections[0].Unicast.GetValueOrDefault())
                {
                    IsDirty = true;
                    return;
                }
            }

            if (ModifiedDiscreteIO.Inhibited != OriginalDiscreteIO.Inhibited)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedDiscreteIO.MajorFault != OriginalDiscreteIO.MajorFault)
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
                OriginalDiscreteIO.Communications.Connections[0].RPI = ModifiedDiscreteIO.RPI;
            }

            if (UnicastVisibility == Visibility.Visible)
            {
                OriginalDiscreteIO.Communications.Connections[0].Unicast = ModifiedDiscreteIO.Unicast;
            }

            OriginalDiscreteIO.Inhibited = ModifiedDiscreteIO.Inhibited;
            OriginalDiscreteIO.MajorFault = ModifiedDiscreteIO.MajorFault;

            return true;
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateConnection();

            RaisePropertyChanged(nameof(IsInhibitedEnabled));
            RaisePropertyChanged(nameof(IsMajorFaultOnControllerEnabled));
        }

        private void UpdateConnection()
        {
            var definition =
                Profiles.DIOModuleTypes.GetConnectionConfigDefinitionByID(ModifiedDiscreteIO
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
                    OriginalDiscreteIO.ParentModule is LocalModule ? Visibility.Collapsed : Visibility.Visible;

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
                float rpi = GetParentModuleRPI();
                MinRPI = rpi;
                MaxRPI = rpi;
                RPI = rpi;
            }
            
            RaisePropertyChanged("IsInhibitedEnabled");
        }

        private float GetParentModuleRPI()
        {
            CommunicationsAdapter adapter = OriginalDiscreteIO.ParentModule as CommunicationsAdapter;
            if (adapter != null)
            {
                return adapter.Communications.Connections[0].RPI / 1000f;
            }

            return 0;
        }

        private DrivePort GetDrivePortInParentModule()
        {
            CommunicationsAdapter adapter = OriginalDiscreteIO.ParentModule as CommunicationsAdapter;
            return adapter?.GetDrivePortInProfiles(PortType.PointIO.ToString());
        }
    }
}
