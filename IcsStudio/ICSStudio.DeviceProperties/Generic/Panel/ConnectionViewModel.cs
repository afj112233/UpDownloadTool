using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using ICSStudio.DeviceProfiles.Generic;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties.Generic.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class ConnectionViewModel : DeviceOptionPanel
    {
        private uint _maxRPI;
        private uint _minRPI;

        private string _moduleFault;

        public ConnectionViewModel(UserControl control, ModifiedGeneralEthernet modifiedModule) : base(control)
        {
            ModifiedModule = modifiedModule;

            var moduleType = Profiles.ModuleProperties[0];

            _minRPI = (uint) moduleType.MinimumUpdateRate;
            _maxRPI = (uint) moduleType.MaximumUpdateRate;
            if (_maxRPI >= 3200000)
                _maxRPI = 3200000;
        }

        public ModifiedGeneralEthernet ModifiedModule { get; }
        public GeneralEthernet OriginalModule => ModifiedModule?.OriginalModule;
        public GenericEnetModuleProfiles Profiles => OriginalModule?.Profiles;

        public bool IsInhibitedEnabled => !IsOnline;

        public bool IsMajorFaultOnControllerEnabled => !IsOnline;

        public bool IsUseUnicastConnectionEnabled => !IsOnline;

        public bool IsRPIEnabled => !IsOnline;

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
            get { return ModifiedModule.RPI / 1000f; }
            set
            {
                if (ModifiedModule.RPI != (uint) (value * 1000))
                {
                    ModifiedModule.RPI = (uint) (value * 1000);
                    RaisePropertyChanged();
                    CheckDirty();
                }
            }
        }

        public string ValidationRange => $"({MinRPI:F1} - {MaxRPI:F1} ms)";

        public bool Inhibited
        {
            get { return ModifiedModule.Inhibited; }
            set
            {
                if (ModifiedModule.Inhibited != value)
                {
                    ModifiedModule.Inhibited = value;

                    RaisePropertyChanged();

                    CheckDirty();
                }
            }
        }

        public bool MajorFault
        {
            get { return ModifiedModule.MajorFault; }
            set
            {
                if (ModifiedModule.MajorFault != value)
                {
                    ModifiedModule.MajorFault = value;
                    RaisePropertyChanged();
                    CheckDirty();
                }
            }
        }

        public bool? Unicast
        {
            get { return ModifiedModule.Unicast; }
            set
            {
                if (ModifiedModule.Unicast != value)
                {
                    ModifiedModule.Unicast = value;
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

        [SuppressMessage("ReSharper", "RedundantJumpStatement")]
        public override void CheckDirty()
        {
            if (ModifiedModule.RPI != OriginalModule.Communications.Connections[0].RPI)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedModule.Inhibited != OriginalModule.Inhibited)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedModule.MajorFault != OriginalModule.MajorFault)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedModule.Unicast != OriginalModule.Communications.Connections[0].Unicast)
            {
                IsDirty = true;
                return;
            }

            IsDirty = false;
        }

        public override bool SaveOptions()
        {
            OriginalModule.Communications.Connections[0].RPI = ModifiedModule.RPI;
            OriginalModule.Communications.Connections[0].Unicast = ModifiedModule.Unicast;

            OriginalModule.Inhibited = ModifiedModule.Inhibited;
            OriginalModule.MajorFault = ModifiedModule.MajorFault;

            return true;
        }

        public override void Show()
        {
            RaisePropertyChanged("IsInhibitedEnabled");
        }

        public override void Refresh()
        {
            base.Refresh();
            RaisePropertyChanged(nameof(IsInhibitedEnabled));
            RaisePropertyChanged(nameof(IsMajorFaultOnControllerEnabled));
            RaisePropertyChanged(nameof(IsUseUnicastConnectionEnabled));
            RaisePropertyChanged(nameof(IsRPIEnabled));
        }
    }
}
