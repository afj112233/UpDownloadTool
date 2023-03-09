using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Database.Database;
using ICSStudio.DeviceProfiles.DIOEnetAdapter;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.DeviceProperties.ModuleDefinition;
using ICSStudio.Gui.Utils;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties.Adapters.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class GeneralViewModel : DeviceOptionPanel
    {
        private EthernetAddressType _ethernetAddressType;
        private string _hostName;
        private string _ipAddress;


        public GeneralViewModel(UserControl control, ModifiedDIOEnetAdapter modifiedAdapter) : base(control)
        {
            ModifiedAdapter = modifiedAdapter;

            var dbHelper = new DBHelper();
            Vendor = dbHelper.GetVendorName(OriginalAdapter.Vendor);

            Parent = OriginalAdapter.ParentModule.Name;

            var address = ModifiedAdapter.EthernetAddress;
            _ethernetAddressType = EthernetAddressChecker.GetAddressType(address);
            switch (_ethernetAddressType)
            {
                case EthernetAddressType.PrivateNetwork:
                    _privateNetworkLast = GetPrivateNetworkLast(address);
                    break;
                case EthernetAddressType.IPAddress:
                    _ipAddress = address;
                    break;
                case EthernetAddressType.HostName:
                    _hostName = address;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            ChangeCommand = new RelayCommand(ExecuteChangeCommand, CanExecuteChangeCommand);
        }

        public ModifiedDIOEnetAdapter ModifiedAdapter { get; }
        public CommunicationsAdapter OriginalAdapter => ModifiedAdapter?.OriginalAdapter;

        public DIOEnetAdapterProfiles Profiles => OriginalAdapter?.Profiles;

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
            get { return ModifiedAdapter.Name; }
            set
            {
                if (ModifiedAdapter.Name != value)
                {
                    ModifiedAdapter.Name = value;
                    CheckDirty();

                    RaisePropertyChanged();
                }
            }
        }

        public bool NameEnabled
        {
            get
            {
                if (ModifiedAdapter.Controller.IsOnline)
                    return false;

                return true;
            }
        }

        public string Description
        {
            get { return ModifiedAdapter.Description; }
            set
            {
                if (ModifiedAdapter.Description != value)
                {
                    ModifiedAdapter.Description = value;
                    CheckDirty();

                    RaisePropertyChanged();
                }
            }
        }

        public bool IsDescriptionEnabled => !IsOnline;

        public EthernetAddressType EthernetAddressType
        {
            get { return _ethernetAddressType; }
            set
            {
                Set(ref _ethernetAddressType, value);

                CheckDirty();
            }
        }

        public string PrivateNetwork => $"192.168.1.{_privateNetworkLast}";

        private int _privateNetworkLast;

        public int PrivateNetworkLast
        {
            get { return _privateNetworkLast; }
            set
            {
                Set(ref _privateNetworkLast, value);
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

        public bool AddressEnabled
        {
            get
            {
                if (ModifiedAdapter.Controller.IsOnline)
                    return false;

                return true;
            }
        }

        public string Revision => $"{ModifiedAdapter.Major}.{ModifiedAdapter.Minor:D3}";
        public string ElectronicKeying => EnumHelper.GetEnumMember(ModifiedAdapter.EKey);

        public string Connection =>
            Profiles.GetConnectionStringByConfigID(ModifiedAdapter.ConnectionConfigID, ModifiedAdapter.Major);

        public int ChassisSize => ModifiedAdapter.ChassisSize;

        public RelayCommand ChangeCommand { get; }

        private void ExecuteChangeCommand()
        {
            var dialog = new DIOEnetAdapterDefinitionDialog(ModifiedAdapter)
            {
                Owner = Application.Current.MainWindow
            };

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                ModifiedAdapter.Series = dialog.Series;
                ModifiedAdapter.Major = dialog.Major;
                ModifiedAdapter.Minor = dialog.Minor;
                ModifiedAdapter.EKey = dialog.EKey;


                if (ModifiedAdapter.ConnectionConfigID != dialog.ConnectionConfigID ||
                    ModifiedAdapter.ChassisSize != dialog.ChassisSize)
                {
                    ModifiedAdapter.ConnectionConfigID = dialog.ConnectionConfigID;
                    ModifiedAdapter.ChassisSize = dialog.ChassisSize;

                    var definition = Profiles.GetConnectionConfigDefinitionByID(ModifiedAdapter.ConnectionConfigID);

                    if (definition.Connections != null && definition.Connections.Count > 0)
                    {
                        var connectionDefinition = Profiles.GetConnectionDefinitionByID(definition.Connections[0]);

                        ModifiedAdapter.RPI = connectionDefinition.RPI;
                        ModifiedAdapter.Unicast = true;
                    }
                    else
                    {
                        ModifiedAdapter.RPI = 0;
                        ModifiedAdapter.Unicast = false;
                    }

                }

                RaisePropertyChanged("Revision");
                RaisePropertyChanged("ElectronicKeying");
                RaisePropertyChanged("Connection");
                RaisePropertyChanged("ChassisSize");

                CheckDirty();
            }
        }

        private bool CanExecuteChangeCommand()
        {
            return !ModifiedAdapter.Controller.IsOnline;
        }

        public override void CheckDirty()
        {
            // 
            switch (EthernetAddressType)
            {
                case EthernetAddressType.PrivateNetwork:
                    ModifiedAdapter.EthernetAddress = PrivateNetwork;
                    break;
                case EthernetAddressType.IPAddress:
                    ModifiedAdapter.EthernetAddress = IPAddress;
                    break;
                case EthernetAddressType.HostName:
                    ModifiedAdapter.EthernetAddress = HostName;
                    break;
            }

            //
            var ethernetPort = OriginalAdapter.GetFirstPort(PortType.Ethernet);
            if (ethernetPort != null)
                if (!string.Equals(ModifiedAdapter.EthernetAddress, ethernetPort.Address))
                {
                    IsDirty = true;
                    return;
                }

            if (!string.Equals(ModifiedAdapter.Name, OriginalAdapter.Name))
            {
                IsDirty = true;
                return;
            }

            if (!string.Equals(ModifiedAdapter.Description, OriginalAdapter.Description))
            {
                IsDirty = true;
                return;
            }

            if (ModifiedAdapter.Major != OriginalAdapter.Major)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedAdapter.Minor != OriginalAdapter.Minor)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedAdapter.EKey != OriginalAdapter.EKey)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedAdapter.ConnectionConfigID != OriginalAdapter.ConfigID)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedAdapter.ChassisSize != OriginalAdapter.ChassisSize)
            {
                IsDirty = true;
                return;
            }

            //
            IsDirty = false;
        }

        public override int CheckValid()
        {
            var result = 0;

            // Name
            if (!string.Equals(OriginalAdapter.Name, ModifiedAdapter.Name)
                || string.IsNullOrEmpty(ModifiedAdapter.Name))
                result = Checking.CheckModuleName(ModifiedAdapter.Controller, ModifiedAdapter.Name);

            // Ethernet Address
            if (result == 0)
            {
                if (EthernetAddressType == EthernetAddressType.PrivateNetwork ||
                    EthernetAddressType == EthernetAddressType.IPAddress)
                    result = Checking.CheckIPAddress(ModifiedAdapter.EthernetAddress, ModifiedAdapter.Controller,
                        ModifiedAdapter.OriginalAdapter);

                if (result == 0)
                    result = Checking.CheckModuleAddress(ModifiedAdapter.Controller,
                        ModifiedAdapter.EthernetAddress);
            }

            return result;
        }

        public override bool SaveOptions()
        {
            OriginalAdapter.Name = ModifiedAdapter.Name;
            OriginalAdapter.Description = ModifiedAdapter.Description;

            var ethernetPort = OriginalAdapter.GetFirstPort(PortType.Ethernet);
            if (ethernetPort != null) ethernetPort.Address = ModifiedAdapter.EthernetAddress;

            OriginalAdapter.CatalogNumber = Profiles.CatalogNumber + "/" + ModifiedAdapter.Series;
            OriginalAdapter.Major = ModifiedAdapter.Major;
            OriginalAdapter.Minor = ModifiedAdapter.Minor;
            OriginalAdapter.EKey = ModifiedAdapter.EKey;

            OriginalAdapter.ChassisSize = ModifiedAdapter.ChassisSize;

            // Connection ConfigID
            if (OriginalAdapter.ConfigID != ModifiedAdapter.ConnectionConfigID)
            {
                OriginalAdapter.ChangeConnectionConfigID(ModifiedAdapter.ConnectionConfigID);
            }

            return true;
        }

        public override void Show()
        {
            RaisePropertyChanged("NameEnabled");
            RaisePropertyChanged("AddressEnabled");

            var address = ModifiedAdapter.EthernetAddress;
            _ethernetAddressType = EthernetAddressChecker.GetAddressType(address);
            switch (_ethernetAddressType)
            {
                case EthernetAddressType.PrivateNetwork:
                    _privateNetworkLast = GetPrivateNetworkLast(address);
                    RaisePropertyChanged("PrivateNetworkLast");
                    break;
                case EthernetAddressType.IPAddress:
                    _ipAddress = address;
                    RaisePropertyChanged("IPAddress");
                    break;
                case EthernetAddressType.HostName:
                    _hostName = address;
                    RaisePropertyChanged("HostName");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            RaisePropertyChanged("EthernetAddressType");

            ChangeCommand.RaiseCanExecuteChanged();

        }

        public override void Refresh()
        {
            base.Refresh();
            ChangeCommand.RaiseCanExecuteChanged();
            RaisePropertyChanged(nameof(IsDescriptionEnabled));
        }

        private int GetPrivateNetworkLast(string ipAddress)
        {
            try
            {
                IPAddress address = System.Net.IPAddress.Parse(ipAddress);
                var bytes = address.GetAddressBytes();
                if (bytes[0] == 192 && bytes[1] == 168 && bytes[2] == 1)
                    return bytes[3];

                return 1;
            }
            catch (Exception)
            {
                // ignore
            }

            return 1;
        }
    }
}