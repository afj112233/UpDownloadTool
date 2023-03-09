using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Windows.Controls;
using ICSStudio.Database.Database;
using ICSStudio.DeviceProfiles.Generic;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.Gui.Utils;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties.Generic.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class GeneralViewModel : DeviceOptionPanel
    {
        private EthernetAddressType _ethernetAddressType;

        private string _hostName;
        private string _ipAddress;

        private int _inputMultiplier;
        private int _outputMultiplier;

        private int _minInputSize;
        private int _maxInputSize;
        private int _inputSize;

        private int _minOutputSize;
        private int _maxOutputSize;
        private int _outputSize;

        public GeneralViewModel(
            UserControl control,
            ModifiedGeneralEthernet modifiedModule,
            bool isCreating) : base(control)
        {
            ModifiedModule = modifiedModule;

            var dbHelper = new DBHelper();
            Vendor = dbHelper.GetVendorName(OriginalModule.Vendor);

            Parent = OriginalModule.ParentModule.Name;

            CommFormatEnabled = isCreating;

            CreateCommFormatSource();

            var address = ModifiedModule.EthernetAddress;
            var ethernetAddressType = EthernetAddressChecker.GetAddressType(address);
            switch (ethernetAddressType)
            {
                case EthernetAddressType.PrivateNetwork:
                case EthernetAddressType.IPAddress:
                    _ipAddress = address;
                    _ethernetAddressType = EthernetAddressType.IPAddress;
                    break;
                case EthernetAddressType.HostName:
                    _hostName = address;
                    _ethernetAddressType = EthernetAddressType.HostName;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            CommMethod commMethod = Profiles.GetCommMethodByID(ModifiedModule.CommMethod.ToString("X"));

            _inputMultiplier = commMethod.PrimaryConnectionInputSize.Multiplier;
            _outputMultiplier = commMethod.PrimaryConnectionOutputSize.Multiplier;

            _inputSize = ModifiedModule.InputSize / _inputMultiplier;
            _minInputSize = commMethod.PrimaryConnectionInputSize.Min;
            _maxInputSize = commMethod.PrimaryConnectionInputSize.Max;

            _outputSize = ModifiedModule.OutputSize / _outputMultiplier;
            _minOutputSize = commMethod.PrimaryConnectionOutputSize.Min;
            _maxOutputSize = commMethod.PrimaryConnectionOutputSize.Max;

            // config
            _configSize = ModifiedModule.ConfigSize;

            var configOption = commMethod.GetConfigOptionByID(commMethod.DefaultConfigOptionsID);
            Contract.Assert(configOption != null);

            var config = Profiles.ModuleProperties[0].GetConfigByID(configOption.ConfigID);

            _minConfigSize = config.ConfigSize.Min;
            _maxConfigSize = config.ConfigSize.Max;
        }

        private void CreateCommFormatSource()
        {
            List<DisplayItem<uint>> commFormatSource = new List<DisplayItem<uint>>();

            EnetModuleType moduleType = Profiles.ModuleProperties[0];

            foreach (var commMethod in moduleType.CommMethods)
            {
                DisplayItem<uint> displayItem = new DisplayItem<uint>
                {
                    DisplayName = commMethod.Description,
                    Value = uint.Parse(commMethod.ID, NumberStyles.HexNumber)
                };

                commFormatSource.Add(displayItem);
            }

            CommFormatSource = commFormatSource;
        }

        public ModifiedGeneralEthernet ModifiedModule { get; }
        public GeneralEthernet OriginalModule => ModifiedModule?.OriginalModule;
        public GenericEnetModuleProfiles Profiles => OriginalModule?.Profiles;

        public bool IsConnectionParametersEnabled => !IsOnline;

        public string DetailedType
        {
            get
            {
                // Catalog + Description
                if (Profiles != null) return $"{Profiles.CatalogNumber} {Profiles.GetDescription()}";

                return string.Empty;
            }
        }

        public string Vendor { get; }
        public string Parent { get; }

        public string Name
        {
            get { return ModifiedModule.Name; }
            set
            {
                if (ModifiedModule.Name != value)
                {
                    ModifiedModule.Name = value;

                    CheckDirty();

                    RaisePropertyChanged();
                }
            }
        }

        public bool NameEnabled => !ModifiedModule.Controller.IsOnline;

        public string Description
        {
            get { return ModifiedModule.Description; }
            set
            {
                if (ModifiedModule.Description != value)
                {
                    ModifiedModule.Description = value;

                    CheckDirty();

                    RaisePropertyChanged();
                }
            }
        }

        public bool IsDescriptionEnabled => !IsOnline;

        public uint CommFormat
        {
            get { return ModifiedModule.CommMethod; }
            set
            {
                if (ModifiedModule.CommMethod != value)
                {
                    ModifiedModule.CommMethod = value;

                    CheckDirty();

                    RaisePropertyChanged();

                    //
                    CommMethod commMethod = Profiles.GetCommMethodByID(ModifiedModule.CommMethod.ToString("X"));

                    _inputMultiplier = commMethod.PrimaryConnectionInputSize.Multiplier;
                    _outputMultiplier = commMethod.PrimaryConnectionOutputSize.Multiplier;

                    RaisePropertyChanged("InputSizeUnit");
                    RaisePropertyChanged("OutputSizeUnit");

                    InputInstance = 0;
                    OutputInstance = 0;
                    ConfigInstance = 0;

                    MinInputSize = commMethod.PrimaryConnectionInputSize.Min;
                    MaxInputSize = commMethod.PrimaryConnectionInputSize.Max;
                    InputSize = commMethod.PrimaryConnectionInputSize.Default;

                    MinOutputSize = commMethod.PrimaryConnectionOutputSize.Min;
                    MaxOutputSize = commMethod.PrimaryConnectionOutputSize.Max;
                    OutputSize = commMethod.PrimaryConnectionOutputSize.Default;

                    var configOption = commMethod.GetConfigOptionByID(commMethod.DefaultConfigOptionsID);
                    Contract.Assert(configOption != null);

                    var config = Profiles.ModuleProperties[0].GetConfigByID(configOption.ConfigID);
                    MinConfigSize = config.ConfigSize.Min;
                    MaxConfigSize = config.ConfigSize.Max;
                    ConfigSize = config.ConfigSize.Default;
                }
            }
        }

        public IList CommFormatSource { get; private set; }

        public bool CommFormatEnabled { get; }

        public bool AddressEnabled
        {
            get
            {
                if (ModifiedModule.Controller.IsOnline)
                    return false;

                return true;
            }
        }

        public EthernetAddressType EthernetAddressType
        {
            get { return _ethernetAddressType; }
            set
            {
                Set(ref _ethernetAddressType, value);

                CheckDirty();
            }
        }

        public string IPAddress
        {
            get { return _ipAddress; }
            set
            {
                Set(ref _ipAddress, value);

                CheckDirty();
            }
        }

        public string HostName
        {
            get { return _hostName; }
            set
            {
                Set(ref _hostName, value);

                CheckDirty();
            }
        }

        public int InputInstance
        {
            get { return ModifiedModule.InputCxnPoint; }
            set
            {
                if (ModifiedModule.InputCxnPoint != value)
                {
                    ModifiedModule.InputCxnPoint = value;

                    CheckDirty();

                    RaisePropertyChanged();
                }
            }
        }

        public int OutputInstance
        {
            get { return ModifiedModule.OutputCxnPoint; }
            set
            {
                if (ModifiedModule.OutputCxnPoint != value)
                {
                    ModifiedModule.OutputCxnPoint = value;

                    CheckDirty();

                    RaisePropertyChanged();
                }
            }
        }

        public int ConfigInstance
        {
            get { return ModifiedModule.ConfigCxnPoint; }
            set
            {
                if (ModifiedModule.ConfigCxnPoint != value)
                {
                    ModifiedModule.ConfigCxnPoint = value;

                    CheckDirty();

                    RaisePropertyChanged();
                }
            }
        }

        public string InputSizeUnit
        {
            get
            {
                switch (_inputMultiplier)
                {
                    case 4:
                        return "(32-bit)";
                    case 2:
                        return "(16-bit)";
                    case 1:
                        return "(8-bit)";
                    default:
                        return "(??-bit)";
                }
            }
        }

        public string OutputSizeUnit
        {
            get
            {
                switch (_outputMultiplier)
                {
                    case 4:
                        return "(32-bit)";
                    case 2:
                        return "(16-bit)";
                    case 1:
                        return "(8-bit)";
                    default:
                        return "(??-bit)";
                }
            }
        }

        public int MinInputSize
        {
            get { return _minInputSize; }
            set { Set(ref _minInputSize, value); }
        }

        public int MaxInputSize
        {
            get { return _maxInputSize; }
            set { Set(ref _maxInputSize, value); }
        }

        public int InputSize
        {
            get { return _inputSize; }
            set
            {
                if (_inputSize != value)
                {
                    _inputSize = value;

                    ModifiedModule.InputSize = _inputSize * _inputMultiplier;

                    CheckDirty();

                    RaisePropertyChanged();
                }
            }
        }



        public int MinOutputSize
        {
            get { return _minOutputSize; }
            set { Set(ref _minOutputSize, value); }
        }

        public int MaxOutputSize
        {
            get { return _maxOutputSize; }
            set { Set(ref _maxOutputSize, value); }
        }

        public int OutputSize
        {
            get { return _outputSize; }
            set
            {
                if (_outputSize != value)
                {
                    _outputSize = value;

                    ModifiedModule.OutputSize = _outputSize * _outputMultiplier;

                    CheckDirty();

                    RaisePropertyChanged();
                }
            }
        }

        private int _minConfigSize;
        private int _maxConfigSize;
        private int _configSize;

        public int MinConfigSize
        {
            get { return _minConfigSize; }
            set { Set(ref _minConfigSize, value); }
        }

        public int MaxConfigSize
        {
            get { return _maxConfigSize; }
            set { Set(ref _maxConfigSize, value); }
        }

        public int ConfigSize
        {
            get { return _configSize; }
            set
            {
                if (_configSize != value)
                {
                    _configSize = value;

                    ModifiedModule.ConfigSize = _configSize;

                    RaisePropertyChanged();
                }
            }
        }

        [SuppressMessage("ReSharper", "RedundantJumpStatement")]
        public override void CheckDirty()
        {
            // EthernetAddress
            switch (EthernetAddressType)
            {
                case EthernetAddressType.IPAddress:
                    ModifiedModule.EthernetAddress = IPAddress;
                    break;
                case EthernetAddressType.HostName:
                    ModifiedModule.EthernetAddress = HostName;
                    break;
            }

            var ethernetPort = OriginalModule.GetFirstPort(PortType.Ethernet);
            if (ethernetPort != null)
                if (!string.Equals(ModifiedModule.EthernetAddress, ethernetPort.Address))
                {
                    IsDirty = true;
                    return;
                }

            // Name
            if (!string.Equals(ModifiedModule.Name, OriginalModule.Name))
            {
                IsDirty = true;
                return;
            }

            if (!string.Equals(ModifiedModule.Description, OriginalModule.Description))
            {
                IsDirty = true;
                return;
            }

            if (ModifiedModule.ConfigCxnPoint != OriginalModule.ConfigCxnPoint)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedModule.InputCxnPoint != OriginalModule.Communications.Connections[0].InputCxnPoint)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedModule.OutputCxnPoint != OriginalModule.Communications.Connections[0].OutputCxnPoint)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedModule.InputSize != OriginalModule.Communications.Connections[0].InputSize)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedModule.OutputSize != OriginalModule.Communications.Connections[0].OutputSize)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedModule.ConfigSize != OriginalModule.Communications.ConfigTag.ConfigSize)
            {
                IsDirty = true;
                return;
            }

            IsDirty = false;
        }

        public override int CheckValid()
        {
            var result = 0;

            // Name
            if (!string.Equals(OriginalModule.Name, ModifiedModule.Name)
                || string.IsNullOrEmpty(ModifiedModule.Name))
                result = Checking.CheckModuleName(ModifiedModule.Controller, ModifiedModule.Name);

            // Ethernet Address
            if (result == 0)
            {
                if (EthernetAddressType == EthernetAddressType.PrivateNetwork ||
                    EthernetAddressType == EthernetAddressType.IPAddress)
                    result = Checking.CheckIPAddress(
                        ModifiedModule.EthernetAddress,
                        ModifiedModule.Controller,
                        OriginalModule);

                if (result == 0)
                    result = Checking.CheckModuleAddress(ModifiedModule.Controller,
                        ModifiedModule.EthernetAddress);
            }

            //TODO(gjc): add here

            return result;
        }

        public override bool SaveOptions()
        {
            OriginalModule.Name = ModifiedModule.Name;
            OriginalModule.Description = ModifiedModule.Description;

            var ethernetPort = OriginalModule.GetFirstPort(PortType.Ethernet);
            if (ethernetPort != null) ethernetPort.Address = ModifiedModule.EthernetAddress;

            if (OriginalModule.Communications.CommMethod != ModifiedModule.CommMethod)
            {
                OriginalModule.ChangeCommMethod(
                    ModifiedModule.CommMethod,
                    ModifiedModule.InputCxnPoint, ModifiedModule.InputSize,
                    ModifiedModule.OutputCxnPoint, ModifiedModule.OutputSize,
                    ModifiedModule.ConfigCxnPoint, ModifiedModule.ConfigSize);
            }
            else
            {
                OriginalModule.Communications.Connections[0].InputCxnPoint = ModifiedModule.InputCxnPoint;
                OriginalModule.Communications.Connections[0].OutputCxnPoint = ModifiedModule.OutputCxnPoint;

                if (OriginalModule.Communications.Connections[0].InputSize != ModifiedModule.InputSize)
                {
                    OriginalModule.ChangeInputSize(ModifiedModule.InputSize);
                }

                if (OriginalModule.Communications.Connections[0].OutputSize != ModifiedModule.OutputSize)
                {
                    OriginalModule.ChangeOutputSize(ModifiedModule.OutputSize);
                }

                if (OriginalModule.ConfigCxnPoint != ModifiedModule.ConfigCxnPoint ||
                    OriginalModule.Communications.ConfigTag.ConfigSize != ModifiedModule.ConfigSize)
                {
                    OriginalModule.ChangeConfigTag(ModifiedModule.ConfigCxnPoint, ModifiedModule.ConfigSize);
                }

            }

            return true;
        }


        public override void Show()
        {
            RaisePropertyChanged("NameEnabled");
            RaisePropertyChanged("AddressEnabled");
        }

        public override void Refresh()
        {
            base.Refresh();
            RaisePropertyChanged(nameof(IsConnectionParametersEnabled));
            RaisePropertyChanged(nameof(IsDescriptionEnabled));
        }
    }
}
