using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Database.Database;
using ICSStudio.DeviceProfiles.DIOModule;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.DeviceProperties.ModuleDefinition;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties.DiscreteIOs.Panel
{
    //TODO(gjc)
    // 1.parent chassis size changed

    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class GeneralViewModel : DeviceOptionPanel
    {
        private List<int> _slotSource;

        public GeneralViewModel(UserControl control, ModifiedDiscreteIO modifiedDiscreteIO) : base(control)
        {
            ModifiedDiscreteIO = modifiedDiscreteIO;

            var dbHelper = new DBHelper();
            Vendor = dbHelper.GetVendorName(OriginalDiscreteIO.Vendor);

            Parent = OriginalDiscreteIO.ParentModule.Name;

            UpdateSlotSource();

            ChangeCommand = new RelayCommand(ExecuteChangeCommand,CanExecuteChangeCommand);
        }

        public ModifiedDiscreteIO ModifiedDiscreteIO { get; }
        public DiscreteIO OriginalDiscreteIO => ModifiedDiscreteIO?.OriginalDiscreteIO;

        public DIOModuleProfiles Profiles => OriginalDiscreteIO?.Profiles;

        public IController Controller => ModifiedDiscreteIO.Controller;

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
            get { return ModifiedDiscreteIO.Name; }
            set
            {
                if (ModifiedDiscreteIO.Name != value)
                {
                    ModifiedDiscreteIO.Name = value;
                    CheckDirty();

                    RaisePropertyChanged();
                }
            }
        }

        public bool NameEnabled
        {
            get
            {
                var ioModule = Profiles.GetModule(ModifiedDiscreteIO.Major);

                if (ioModule.FixedUserName)
                    return false;

                if (Controller.IsOnline)
                    return false;

                return true;
            }
        }

        public string Description
        {
            get { return ModifiedDiscreteIO.Description; }
            set
            {
                if (ModifiedDiscreteIO.Description != value)
                {
                    ModifiedDiscreteIO.Description = value;
                    CheckDirty();

                    RaisePropertyChanged();
                }
            }
        }

        public bool IsDescriptionEnabled => !IsOnline;

        public string Series => ModifiedDiscreteIO.Series;

        public Visibility SeriesVisibility
        {
            get
            {
                if (Profiles.CatalogNumber == "Embedded")
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        public string Revision => $"{ModifiedDiscreteIO.Major}.{ModifiedDiscreteIO.Minor:D3}";
        public string ElectronicKeying => EnumHelper.GetEnumMember(ModifiedDiscreteIO.EKey);

        public string Connection =>
            Profiles.GetConnectionStringByConfigID(ModifiedDiscreteIO.ConnectionConfigID, ModifiedDiscreteIO.Major);

        public string DataFormat =>
            Profiles.GetDataFormatStringByConfigID(ModifiedDiscreteIO.ConnectionConfigID, ModifiedDiscreteIO.Major);

        public Visibility DataFormatVisibility
        {
            get
            {
                if (Profiles.CatalogNumber == "Embedded")
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        public int Slot
        {
            get { return ModifiedDiscreteIO.Slot; }
            set
            {
                if (ModifiedDiscreteIO.Slot != value)
                {
                    ModifiedDiscreteIO.Slot = value;
                    CheckDirty();
                    RaisePropertyChanged();
                }
            }
        }

        public List<int> SlotSource
        {
            get { return _slotSource; }
            set { Set(ref _slotSource, value); }
        }

        public bool SlotEnabled
        {
            get
            {
                if (Profiles.CatalogNumber == "Embedded")
                    return false;

                if (Controller.IsOnline)
                    return false;

                return true;
            }
        }

        public RelayCommand ChangeCommand { get; }


        private void UpdateSlotSource()
        {
            var maxSlot = GetMaxSlot();
            if (maxSlot > 0)
            {
                var slotList = new List<int>();
                for (var i = 1; i <= maxSlot; i++)
                    slotList.Add(i);

                var usedSlotList = GetUsedSlotList();

                SlotSource = slotList.Except(usedSlotList).ToList();
            }
        }

        private int GetMaxSlot()
        {
            var parentModule = OriginalDiscreteIO.ParentModule as DeviceModule;
            var pointIOPort = parentModule?.GetFirstPort(PortType.PointIO);
            if (pointIOPort?.Bus != null)
                return pointIOPort.Bus.Size - 1;

            return 0;
        }

        private List<int> GetUsedSlotList()
        {
            var usedSlotList = new List<int>();
            if (ModifiedDiscreteIO.Controller != null)
                foreach (var item in ModifiedDiscreteIO.Controller.DeviceModules)
                {
                    var deviceModule = item as DeviceModule;
                    if (deviceModule != null
                        && deviceModule.ParentModule == OriginalDiscreteIO.ParentModule
                        && deviceModule != OriginalDiscreteIO)
                    {
                        var port = deviceModule.GetFirstPort(PortType.PointIO);
                        if (port != null) usedSlotList.Add(int.Parse(port.Address));
                    }
                }

            return usedSlotList;
        }

        private void ExecuteChangeCommand()
        {
            var dialog = new DIOModuleDefinitionDialog(ModifiedDiscreteIO)
            {
                Owner = Application.Current.MainWindow
            };

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                ModifiedDiscreteIO.Series = dialog.Series;
                ModifiedDiscreteIO.Major = dialog.Major;
                ModifiedDiscreteIO.Minor = dialog.Minor;
                ModifiedDiscreteIO.EKey = dialog.EKey;



                if (ModifiedDiscreteIO.ConnectionConfigID != dialog.ConnectionConfigID)
                {
                    ModifiedDiscreteIO.ChangeConnectionConfigID(dialog.ConnectionConfigID);

                    RaisePropertyChanged("Connection");
                    RaisePropertyChanged("DataFormat");
                }

                RaisePropertyChanged("Series");
                RaisePropertyChanged("Revision");
                RaisePropertyChanged("ElectronicKeying");

                CheckDirty();
            }
        }

        private bool CanExecuteChangeCommand()
        {
            return !ModifiedDiscreteIO.Controller.IsOnline;
        }

        public override void CheckDirty()
        {
            if (!string.Equals(ModifiedDiscreteIO.Name, OriginalDiscreteIO.Name))
            {
                IsDirty = true;
                return;
            }

            if (!string.Equals(ModifiedDiscreteIO.Description, OriginalDiscreteIO.Description))
            {
                IsDirty = true;
                return;
            }

            if (ModifiedDiscreteIO.Slot != OriginalDiscreteIO.Slot)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedDiscreteIO.Major != OriginalDiscreteIO.Major)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedDiscreteIO.Minor != OriginalDiscreteIO.Minor)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedDiscreteIO.EKey != OriginalDiscreteIO.EKey)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedDiscreteIO.ConnectionConfigID != OriginalDiscreteIO.ConfigID)
            {
                IsDirty = true;
                return;
            }

            IsDirty = false;
        }

        public override int CheckValid()
        {
            var result = 0;
            string warningReason;
            string baseReason = LanguageManager.GetInstance().ConvertSpecifier("Failed to modify properties.");

            // Name check
            if (!string.Equals(OriginalDiscreteIO.Name, ModifiedDiscreteIO.Name) ||
                string.IsNullOrEmpty(ModifiedDiscreteIO.Name))
                result = Checking.CheckModuleName(ModifiedDiscreteIO.Controller, ModifiedDiscreteIO.Name);

            // Slot check
            if (result == 0)
            {
                int maxSlot = GetMaxSlot();
                if (ModifiedDiscreteIO.Slot > maxSlot)
                {
                    warningReason = baseReason + "\n" + LanguageManager.GetInstance().ConvertSpecifier("Slot out of range for current chassis size.");

                    MessageBox.Show(warningReason, "ICS Studio",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    result = -1;
                }
            }

            if (result == 0)
            {
                var deviceModule = GetDeviceModuleBySlot(ModifiedDiscreteIO.Slot);
                if (deviceModule != null && deviceModule != OriginalDiscreteIO)
                {
                    warningReason = baseReason + "\n" + LanguageManager.GetInstance().ConvertSpecifier("Slot number in use by another module.");

                    MessageBox.Show(warningReason, "ICS Studio",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    result = -2;
                }
            }

            return result;
        }

        private IDeviceModule GetDeviceModuleBySlot(int slot)
        {
            if (ModifiedDiscreteIO.Controller != null)
                foreach (var item in ModifiedDiscreteIO.Controller.DeviceModules)
                {
                    var deviceModule = item as DeviceModule;
                    if (deviceModule != null
                        && deviceModule.ParentModule == OriginalDiscreteIO.ParentModule)
                    {
                        var port = deviceModule.GetFirstPort(PortType.PointIO);
                        if (port != null && int.Parse(port.Address) == slot)
                            return deviceModule;
                    }
                }


            return null;

        }


        public override bool SaveOptions()
        {
            OriginalDiscreteIO.Name = ModifiedDiscreteIO.Name ?? string.Empty;

            OriginalDiscreteIO.Description = ModifiedDiscreteIO.Description;

            OriginalDiscreteIO.Slot = ModifiedDiscreteIO.Slot;

            OriginalDiscreteIO.CatalogNumber = Profiles.CatalogNumber + "/" + ModifiedDiscreteIO.Series;
            OriginalDiscreteIO.Major = ModifiedDiscreteIO.Major;
            OriginalDiscreteIO.Minor = ModifiedDiscreteIO.Minor;
            OriginalDiscreteIO.EKey = ModifiedDiscreteIO.EKey;

            // Connection ConfigID
            if (OriginalDiscreteIO.ConfigID != ModifiedDiscreteIO.ConnectionConfigID)
            {
                OriginalDiscreteIO.ChangeConnectionConfigID(ModifiedDiscreteIO.ConnectionConfigID);
            }

            return true;
        }

        public override void Show()
        {
            UpdateSlotSource();

            RaisePropertyChanged("NameEnabled");
            RaisePropertyChanged("SlotEnabled");
        }

        public override void Refresh()
        {
            base.Refresh();
            ChangeCommand.RaiseCanExecuteChanged();
            RaisePropertyChanged(nameof(IsDescriptionEnabled));
        }
    }
}