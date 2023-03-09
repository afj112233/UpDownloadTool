using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.DeviceProfiles.DIOEnetAdapter;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties.Adapters.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class ConnectionViewModel : DeviceOptionPanel
    {
        private uint _maxRPI;
        private uint _minRPI;

        private string _moduleFault;
        private bool _rpiEnable;

        private Visibility _unicastVisibility;

        public ConnectionViewModel(UserControl control, ModifiedDIOEnetAdapter modifiedAdapter) 
            : base(control)
        {
            ModifiedAdapter = modifiedAdapter;

            UpdateConnection();
        }

        public ModifiedDIOEnetAdapter ModifiedAdapter { get; }
        public CommunicationsAdapter OriginalAdapter => ModifiedAdapter?.OriginalAdapter;

        public DIOEnetAdapterProfiles Profiles => OriginalAdapter?.Profiles;

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
            get { return ModifiedAdapter.RPI / 1000f; }
            set
            {
                if (ModifiedAdapter.RPI != (uint) (value * 1000))
                {
                    ModifiedAdapter.RPI = (uint) (value * 1000);
                    RaisePropertyChanged();
                    CheckDirty();
                }
            }
        }

        public string ValidationRange => $"({MinRPI:F1} - {MaxRPI:F1})";

        public Visibility ValidationRangeVisibility { get; set; }

        public bool Inhibited
        {
            get { return ModifiedAdapter.Inhibited; }
            set
            {
                if (ModifiedAdapter.Inhibited != value)
                {
                    ModifiedAdapter.Inhibited = value;
                    RaisePropertyChanged();

                    CheckDirty();
                }
            }
        }

        public bool IsInhibitedEnabled => !IsOnline;

        public bool IsMajorFaultOnControllerEnabled => !IsOnline;

        public bool IsUseUnicastConnectionEnabled => !IsOnline;

        public bool MajorFault
        {
            get { return ModifiedAdapter.MajorFault; }
            set
            {
                if (ModifiedAdapter.MajorFault != value)
                {
                    ModifiedAdapter.MajorFault = value;
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

        public bool? Unicast
        {
            get { return ModifiedAdapter.Unicast; }
            set
            {
                if (ModifiedAdapter.Unicast != value)
                {
                    ModifiedAdapter.Unicast = value;
                    RaisePropertyChanged();
                    CheckDirty();
                }
            }
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
            if (RPIEnable
                && OriginalAdapter.Communications.Connections != null
                && OriginalAdapter.Communications.Connections.Count > 0)
            {
                if (ModifiedAdapter.RPI != OriginalAdapter.Communications.Connections[0].RPI)
                {
                    IsDirty = true;
                    return;
                }

                if (ModifiedAdapter.Unicast != OriginalAdapter.Communications.Connections[0].Unicast)
                {
                    IsDirty = true;
                    return;
                }
            }

            if (ModifiedAdapter.Inhibited != OriginalAdapter.Inhibited)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedAdapter.MajorFault != OriginalAdapter.MajorFault)
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
                OriginalAdapter.Communications.Connections[0].RPI = ModifiedAdapter.RPI;
                OriginalAdapter.Communications.Connections[0].Unicast = ModifiedAdapter.Unicast;
            }

            OriginalAdapter.Inhibited = ModifiedAdapter.Inhibited;
            OriginalAdapter.MajorFault = ModifiedAdapter.MajorFault;

            return true;
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateConnection();

            RaisePropertyChanged(nameof(IsMajorFaultOnControllerEnabled));
            RaisePropertyChanged(nameof(IsUseUnicastConnectionEnabled));
        }

        private void UpdateConnection()
        {
            var definition = Profiles.GetConnectionConfigDefinitionByID(ModifiedAdapter.ConnectionConfigID);
            Contract.Assert(definition != null);

            if (definition.Connections == null || definition.Connections.Count == 0)
            {
                RPIEnable = false;
                MinRPI = 0;
                MaxRPI = 0;
                RPI = 0;
                ValidationRangeVisibility = Visibility.Collapsed;
                UnicastVisibility = Visibility.Collapsed;
            }
            else
            {
                RPIEnable = !IsOnline;
                var connectionDefinition = Profiles.GetConnectionDefinitionByID(definition.Connections[0]);
                _minRPI = connectionDefinition.MinRPI;
                _maxRPI = connectionDefinition.MaxRPI;

                RaisePropertyChanged("MinRPI");
                RaisePropertyChanged("MaxRPI");
                RaisePropertyChanged("RPI");
                RaisePropertyChanged("ValidationRange");

                ValidationRangeVisibility = Visibility.Visible;
                UnicastVisibility = Visibility.Visible;
            }

            RaisePropertyChanged("IsInhibitedEnabled");
        }
    }
}