using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Database.Database;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.DeviceProperties.ModuleDefinition;
using ICSStudio.Gui.Utils;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties.ServoDrives.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    class GeneralViewModel : DeviceOptionPanel
    {
        private EthernetAddressType _ethernetAddressType;
        private string _ipAddress;
        private string _hostName;

        public GeneralViewModel(UserControl panel, ModifiedMotionDrive modifiedMotionDrive) : base(panel)
        {
            ModifiedMotionDrive = modifiedMotionDrive;

            DBHelper dbHelper = new DBHelper();
            Vendor = dbHelper.GetVendorName(ModifiedMotionDrive.OriginalMotionDrive.Vendor);

            Parent = ModifiedMotionDrive.OriginalMotionDrive.ParentModule.Name;

            var address = ModifiedMotionDrive.EthernetAddress;
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

        public ModifiedMotionDrive ModifiedMotionDrive { get; }
        public CIPMotionDrive OriginalMotionDrive => ModifiedMotionDrive.OriginalMotionDrive;

        public string DetailedType
        {
            get
            {
                // PowerStructure: CatalogNumber + Description
                if (ModifiedMotionDrive?.Profiles != null)
                {
                    var powerStructure = ModifiedMotionDrive.Profiles.Schema.PowerStructures[0];
                    return $"{powerStructure.GetCatalogNumber()} {powerStructure.GetDescription()}";
                }

                return string.Empty;

            }
        }

        public string Vendor { get; }

        public string Parent { get; }

        public string Name
        {
            get { return ModifiedMotionDrive.Name; }
            set
            {
                if (ModifiedMotionDrive.Name != value)
                {
                    ModifiedMotionDrive.Name = value;
                    CheckDirty();

                    RaisePropertyChanged();
                }

            }
        }

        public bool NameEnabled
        {
            get
            {
                if (ModifiedMotionDrive.Controller.IsOnline)
                    return false;

                return true;
            }
        }


        public string Description
        {
            get { return ModifiedMotionDrive.Description; }
            set
            {
                if (ModifiedMotionDrive.Description != value)
                {
                    ModifiedMotionDrive.Description = value;
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
                if (ModifiedMotionDrive.Controller.IsOnline)
                    return false;

                return true;
            }
        }

        public string Revision => $"{ModifiedMotionDrive.Major}.{ModifiedMotionDrive.Minor:D3}";
        public string ElectronicKeying => EnumHelper.GetEnumMember(ModifiedMotionDrive.EKey);

        public string PowerStructure =>
            ModifiedMotionDrive.OriginalMotionDrive.GetPowerStructureCatalogNumberByID(ModifiedMotionDrive
                .PowerStructureID);

        public string Connection => EnumHelper.GetEnumMember(ModifiedMotionDrive.Connection);

        public RelayCommand ChangeCommand { get; }

        private void ExecuteChangeCommand()
        {
            var dialog = new ServoDriveDefinitionDialog(ModifiedMotionDrive.Profiles,
                new ServoDriveDefinition()
                {
                    Connection = ModifiedMotionDrive.Connection,
                    EKey = ModifiedMotionDrive.EKey,
                    Major = ModifiedMotionDrive.Major,
                    Minor = ModifiedMotionDrive.Minor,
                    PowerStructureID = ModifiedMotionDrive.PowerStructureID,
                    VerifyPowerRating = ModifiedMotionDrive.VerifyPowerRating
                })
            {
                Owner = Application.Current.MainWindow
            };

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                ModifiedMotionDrive.Major = dialog.Major;
                ModifiedMotionDrive.Minor = dialog.Minor;
                ModifiedMotionDrive.EKey = dialog.EKey;
                ModifiedMotionDrive.PowerStructureID = dialog.PowerStructureID;
                ModifiedMotionDrive.VerifyPowerRating = dialog.VerifyPowerRating;
                ModifiedMotionDrive.Connection = dialog.Connection;

                RaisePropertyChanged("Revision");
                RaisePropertyChanged("ElectronicKeying");
                RaisePropertyChanged("PowerStructure");
                RaisePropertyChanged("Connection");

                CheckDirty();
            }
        }

        private bool CanExecuteChangeCommand()
        {
            if (ModifiedMotionDrive.Controller.IsOnline)
                return false;

            return true;
        }

        public override void CheckDirty()
        {
            // 
            switch (EthernetAddressType)
            {
                case EthernetAddressType.PrivateNetwork:
                    ModifiedMotionDrive.EthernetAddress = PrivateNetwork;
                    break;
                case EthernetAddressType.IPAddress:
                    ModifiedMotionDrive.EthernetAddress = IPAddress;
                    break;
                case EthernetAddressType.HostName:
                    ModifiedMotionDrive.EthernetAddress = HostName;
                    break;
            }

            //
            if (!string.Equals(ModifiedMotionDrive.EthernetAddress, OriginalMotionDrive.Ports[0].Address))
            {
                IsDirty = true;
                return;
            }

            if (!string.Equals(ModifiedMotionDrive.Name, OriginalMotionDrive.Name))
            {
                IsDirty = true;
                return;
            }

            if (!string.Equals(ModifiedMotionDrive.Description, OriginalMotionDrive.Description))
            {
                IsDirty = true;
                return;
            }

            if (ModifiedMotionDrive.Major != OriginalMotionDrive.Major)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedMotionDrive.Minor != OriginalMotionDrive.Minor)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedMotionDrive.EKey != OriginalMotionDrive.EKey)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedMotionDrive.PowerStructureID != OriginalMotionDrive.PowerStructureID)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedMotionDrive.Connection != OriginalMotionDrive.Connection)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedMotionDrive.VerifyPowerRating != OriginalMotionDrive.VerifyPowerRating)
            {
                IsDirty = true;
                return;
            }

            IsDirty = false;
        }

        public override int CheckValid()
        {
            int result = 0;

            // Name
            if (!string.Equals(OriginalMotionDrive.Name, ModifiedMotionDrive.Name)
                || string.IsNullOrEmpty(OriginalMotionDrive.Name))
            {
                result = Checking.CheckModuleName(ModifiedMotionDrive.Controller, ModifiedMotionDrive.Name);
            }

            // Ethernet Address
            if (result == 0)
            {
                if (EthernetAddressType == EthernetAddressType.PrivateNetwork ||
                    EthernetAddressType == EthernetAddressType.IPAddress)
                {
                    result = Checking.CheckIPAddress(ModifiedMotionDrive.EthernetAddress,
                        ModifiedMotionDrive.Controller, ModifiedMotionDrive.OriginalMotionDrive);
                }

                if (result == 0)
                {
                    result = Checking.CheckModuleAddress(ModifiedMotionDrive.Controller,
                        ModifiedMotionDrive.EthernetAddress);
                }
            }

            return result;
        }

        public override bool SaveOptions()
        {
            if (!string.Equals(OriginalMotionDrive.Name, ModifiedMotionDrive.Name))
            {
                OriginalMotionDrive.Name = ModifiedMotionDrive.Name;
                CodeSynchronization.GetInstance().UpdateModuleName(OriginalMotionDrive);
                CodeSynchronization.GetInstance().Update();

            }

            if (!string.Equals(OriginalMotionDrive.Description, ModifiedMotionDrive.Description))
            {
                OriginalMotionDrive.Description = ModifiedMotionDrive.Description;
            }

            OriginalMotionDrive.Major = ModifiedMotionDrive.Major;
            OriginalMotionDrive.Minor = ModifiedMotionDrive.Minor;
            OriginalMotionDrive.EKey = ModifiedMotionDrive.EKey;
            OriginalMotionDrive.PowerStructureID = ModifiedMotionDrive.PowerStructureID;
            OriginalMotionDrive.Connection = ModifiedMotionDrive.Connection;
            OriginalMotionDrive.VerifyPowerRating = ModifiedMotionDrive.VerifyPowerRating;
            OriginalMotionDrive.Ports[0].Address = ModifiedMotionDrive.EthernetAddress;

            return true;
        }

        public override void Show()
        {
            RaisePropertyChanged("NameEnabled");
            RaisePropertyChanged("AddressEnabled");

            var address = ModifiedMotionDrive.EthernetAddress;
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

        public override void Refresh()
        {
            base.Refresh();
            RaisePropertyChanged(nameof(IsDescriptionEnabled));
        }
    }
}
