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

namespace ICSStudio.DeviceProperties.AnalogIOs.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class GeneralViewModel : DeviceOptionPanel
    {
        private List<int> _slotSource;

        public GeneralViewModel(UserControl control, ModifiedAnalogIO modifiedAnalogIO) : base(control)
        {
            ModifiedAnalogIO = modifiedAnalogIO;

            var dbHelper = new DBHelper();
            Vendor = dbHelper.GetVendorName(OriginalAnalogIO.Vendor);

            Parent = OriginalAnalogIO.ParentModule.Name;

            UpdateSlotSource();

            ChangeCommand = new RelayCommand(ExecuteChangeCommand);
        }

        public ModifiedAnalogIO ModifiedAnalogIO { get; }
        public AnalogIO OriginalAnalogIO => ModifiedAnalogIO?.OriginalAnalogIO;
        public DIOModuleProfiles Profiles => OriginalAnalogIO?.Profiles;
        public IController Controller => ModifiedAnalogIO.Controller;

        #region Override

        public override void Show()
        {
            UpdateSlotSource();

            RaisePropertyChanged("NameEnabled");
            RaisePropertyChanged("SlotEnabled");
        }

        public override bool SaveOptions()
        {
            OriginalAnalogIO.Name = ModifiedAnalogIO.Name ?? string.Empty;

            OriginalAnalogIO.Description = ModifiedAnalogIO.Description;

            OriginalAnalogIO.Slot = ModifiedAnalogIO.Slot;

            OriginalAnalogIO.CatalogNumber = Profiles.CatalogNumber + "/" + ModifiedAnalogIO.Series;
            OriginalAnalogIO.Major = ModifiedAnalogIO.Major;
            OriginalAnalogIO.Minor = ModifiedAnalogIO.Minor;
            OriginalAnalogIO.EKey = ModifiedAnalogIO.EKey;

            // Connection ConfigID
            if (OriginalAnalogIO.ConfigID != ModifiedAnalogIO.ConnectionConfigID)
            {
                OriginalAnalogIO.ChangeConnectionConfigID(ModifiedAnalogIO.ConnectionConfigID);
            }

            return true;
        }

        public override void CheckDirty()
        {
            if (!string.Equals(ModifiedAnalogIO.Name, OriginalAnalogIO.Name))
            {
                IsDirty = true;
                return;
            }

            if (!string.Equals(ModifiedAnalogIO.Description, OriginalAnalogIO.Description))
            {
                IsDirty = true;
                return;
            }

            if (ModifiedAnalogIO.Slot != OriginalAnalogIO.Slot)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedAnalogIO.Major != OriginalAnalogIO.Major)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedAnalogIO.Minor != OriginalAnalogIO.Minor)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedAnalogIO.EKey != OriginalAnalogIO.EKey)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedAnalogIO.ConnectionConfigID != OriginalAnalogIO.ConfigID)
            {
                IsDirty = true;
                return;
            }

            IsDirty = false;
        }

        public override int CheckValid()
        {
            var result = 0;
            string baseReason = LanguageManager.GetInstance().ConvertSpecifier("Failed to modify properties.");
            string warningReason;

            // Name check
            if (!string.Equals(OriginalAnalogIO.Name, ModifiedAnalogIO.Name) ||
                string.IsNullOrEmpty(ModifiedAnalogIO.Name))
                result = Checking.CheckModuleName(ModifiedAnalogIO.Controller, ModifiedAnalogIO.Name);

            // Slot check
            if (result == 0)
            {
                int maxSlot = GetMaxSlot();
                if (ModifiedAnalogIO.Slot > maxSlot)
                {
                    warningReason = baseReason + "\n" + LanguageManager.GetInstance().ConvertSpecifier("Slot out of range for current chassis size.");

                    MessageBox.Show(warningReason, "ICS Studio",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    result = -1;
                }
            }

            if (result == 0)
            {
                var deviceModule = GetDeviceModuleBySlot(ModifiedAnalogIO.Slot);
                if (deviceModule != null && deviceModule != OriginalAnalogIO)
                {
                    warningReason = baseReason + "\n" + LanguageManager.GetInstance().ConvertSpecifier("Slot number in use by another module.");

                    MessageBox.Show(warningReason, "ICS Studio",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    result = -2;
                }
            }

            return result;
        }

        #endregion


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
            get { return ModifiedAnalogIO.Name; }
            set
            {
                if (ModifiedAnalogIO.Name != value)
                {
                    ModifiedAnalogIO.Name = value;
                    CheckDirty();

                    RaisePropertyChanged();
                }
            }
        }

        public bool NameEnabled
        {
            get
            {
                var ioModule = Profiles.GetModule(ModifiedAnalogIO.Major);

                if (ioModule.FixedUserName)
                    return false;

                if (Controller.IsOnline)
                    return false;

                return true;
            }
        }

        public string Description
        {
            get { return ModifiedAnalogIO.Description; }
            set
            {
                if (ModifiedAnalogIO.Description != value)
                {
                    ModifiedAnalogIO.Description = value;
                    CheckDirty();

                    RaisePropertyChanged();
                }
            }
        }

        public bool IsDescriptionEnabled => !IsOnline;

        public string Series => ModifiedAnalogIO.Series;

        public Visibility SeriesVisibility
        {
            get
            {
                if (Profiles.CatalogNumber == "Embedded")
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        public string Revision => $"{ModifiedAnalogIO.Major}.{ModifiedAnalogIO.Minor:D3}";
        public string ElectronicKeying => EnumHelper.GetEnumMember(ModifiedAnalogIO.EKey);

        public string Connection =>
            Profiles.GetConnectionStringByConfigID(ModifiedAnalogIO.ConnectionConfigID, ModifiedAnalogIO.Major);

        public string DataFormat =>
            Profiles.GetDataFormatStringByConfigID(ModifiedAnalogIO.ConnectionConfigID, ModifiedAnalogIO.Major);

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
            get { return ModifiedAnalogIO.Slot; }
            set
            {
                if (ModifiedAnalogIO.Slot != value)
                {
                    ModifiedAnalogIO.Slot = value;
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

        #region Command




        public RelayCommand ChangeCommand { get; }

        private void ExecuteChangeCommand()
        {
            var dialog = new DIOModuleDefinitionDialog(ModifiedAnalogIO)
            {
                Owner = Application.Current.MainWindow
            };

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                ModifiedAnalogIO.Series = dialog.Series;
                ModifiedAnalogIO.Major = dialog.Major;
                ModifiedAnalogIO.Minor = dialog.Minor;
                ModifiedAnalogIO.EKey = dialog.EKey;



                if (ModifiedAnalogIO.ConnectionConfigID != dialog.ConnectionConfigID)
                {
                    ModifiedAnalogIO.ChangeConnectionConfigID(dialog.ConnectionConfigID);

                    RaisePropertyChanged("Connection");
                    RaisePropertyChanged("DataFormat");
                }

                RaisePropertyChanged("Series");
                RaisePropertyChanged("Revision");
                RaisePropertyChanged("ElectronicKeying");

                CheckDirty();
            }
        }

        #endregion

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
            var parentModule = OriginalAnalogIO.ParentModule as DeviceModule;
            var pointIOPort = parentModule?.GetFirstPort(PortType.PointIO);
            if (pointIOPort?.Bus != null)
                return pointIOPort.Bus.Size - 1;

            return 0;
        }

        private List<int> GetUsedSlotList()
        {
            var usedSlotList = new List<int>();
            if (ModifiedAnalogIO.Controller != null)
                foreach (var item in ModifiedAnalogIO.Controller.DeviceModules)
                {
                    var deviceModule = item as DeviceModule;
                    if (deviceModule != null
                        && deviceModule.ParentModule == OriginalAnalogIO.ParentModule
                        && deviceModule != OriginalAnalogIO)
                    {
                        var port = deviceModule.GetFirstPort(PortType.PointIO);
                        if (port != null) usedSlotList.Add(int.Parse(port.Address));
                    }
                }

            return usedSlotList;
        }

        private IDeviceModule GetDeviceModuleBySlot(int slot)
        {
            if (ModifiedAnalogIO.Controller != null)
                foreach (var item in ModifiedAnalogIO.Controller.DeviceModules)
                {
                    var deviceModule = item as DeviceModule;
                    if (deviceModule != null
                        && deviceModule.ParentModule == OriginalAnalogIO.ParentModule)
                    {
                        var port = deviceModule.GetFirstPort(PortType.PointIO);
                        if (port != null && int.Parse(port.Address) == slot)
                            return deviceModule;
                    }
                }


            return null;
        }

        public override void Refresh()
        {
            base.Refresh();
            RaisePropertyChanged(nameof(IsDescriptionEnabled));
        }
    }
}
