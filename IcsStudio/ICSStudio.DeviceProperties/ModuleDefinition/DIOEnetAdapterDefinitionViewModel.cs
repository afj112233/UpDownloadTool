using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.DeviceProfiles.DIOEnetAdapter;
using ICSStudio.DeviceProperties.Adapters;
using ICSStudio.Gui.Utils;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties.ModuleDefinition
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class DIOEnetAdapterDefinitionViewModel : ViewModelBase
    {
        private int _chassisSize;
        private List<int> _chassisSizeSource;
        private uint _connectionConfigID;
        private bool? _dialogResult;

        private ElectronicKeyingType _eKey;
        private int _major;
        private List<int> _majorSource;
        private int _minor;
        private string _series;

        private List<string> _seriesSource;
        private IList _eKeySource;
        private IList _connectionSource;

        public DIOEnetAdapterDefinitionViewModel(ModifiedDIOEnetAdapter modifiedAdapter)
        {
            ModifiedAdapter = modifiedAdapter;

            _series = ModifiedAdapter.Series;
            _major = ModifiedAdapter.Major;
            _minor = ModifiedAdapter.Minor;
            _eKey = ModifiedAdapter.EKey;
            _connectionConfigID = ModifiedAdapter.ConnectionConfigID;
            _chassisSize = ModifiedAdapter.ChassisSize;

            UpdateSeriesSource();
            UpdateMajorSource();
            UpdateEKeySource();
            UpdateConnectionSource();
            UpdateChassisSizeSource();

            OkCommand = new RelayCommand(ExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public ModifiedDIOEnetAdapter ModifiedAdapter { get; }
        public DIOEnetAdapterProfiles Profiles => ModifiedAdapter.Profiles;

        public string Title
        {
            get
            {
                var title = LanguageManager.GetInstance().ConvertSpecifier("ModuleDefinition");
                if (string.IsNullOrEmpty(title)) title = "Module Definition";
                if (CheckIsDirty())
                    return title + "*";

                return title;
            }
        }

        public string Series
        {
            get { return _series; }
            set
            {
                Set(ref _series, value);

                UpdateMajorSource();
                UpdateConnectionSource();

                RaisePropertyChanged("Title");
            }
        }

        public List<string> SeriesSource
        {
            get { return _seriesSource; }
            set { Set(ref _seriesSource, value); }
        }

        public int Major
        {
            get { return _major; }
            set
            {
                Set(ref _major, value);

                UpdateConnectionSource();

                RaisePropertyChanged("Title");
            }
        }

        public List<int> MajorSource
        {
            get { return _majorSource; }
            set { Set(ref _majorSource, value); }
        }

        public int Minor
        {
            get { return _minor; }
            set
            {
                Set(ref _minor, value);
                RaisePropertyChanged("Title");
            }
        }

        public ElectronicKeyingType EKey
        {
            get { return _eKey; }
            set
            {
                Set(ref _eKey, value);
                RaisePropertyChanged("Title");
            }
        }

        public IList EKeySource
        {
            get { return _eKeySource; }
            set { Set(ref _eKeySource, value); }
        }

        public uint ConnectionConfigID
        {
            get { return _connectionConfigID; }
            set
            {
                Set(ref _connectionConfigID, value);
                RaisePropertyChanged("Title");
            }
        }

        public IList ConnectionSource
        {
            get { return _connectionSource; }
            set { Set(ref _connectionSource, value); }
        }

        public int ChassisSize
        {
            get { return _chassisSize; }
            set
            {
                Set(ref _chassisSize, value);
                RaisePropertyChanged("Title");
            }
        }

        public List<int> ChassisSizeSource
        {
            get { return _chassisSizeSource; }
            set { Set(ref _chassisSizeSource, value); }
        }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        public RelayCommand OkCommand { get; }
        public RelayCommand CancelCommand { get; }

        private void LanguageChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(Title));
        }

        public override void Cleanup()
        {
            base.Cleanup();
            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        private void UpdateChassisSizeSource()
        {
            int maxChildSlot = ModifiedAdapter.OriginalAdapter.GetMaxChildSlot();
            var minAddress = Profiles.AOPModuleTypes.Bus.Downstream.Exclusions.ExcludeAddressRange.MinAddress;
            var maxAddress = Profiles.AOPModuleTypes.Bus.Downstream.Exclusions.ExcludeAddressRange.MaxAddress;

            if (maxChildSlot + 1 > minAddress)
                minAddress = maxChildSlot + 1;

            var chassisList = new List<int>();
            for (var i = minAddress; i <= maxAddress + 1; i++) chassisList.Add(i);

            ChassisSizeSource = chassisList;
        }

        private void UpdateConnectionSource()
        {
            var allConnectionConfigID = Profiles.GetConnectionConfigIDListByMajor(_major);

            uint childConnectionMask = ModifiedAdapter.OriginalAdapter.GetChildRackConnectionMask();

            var oldConnectionConfigID = _connectionConfigID;
            var defaultConnectionConfigID = Profiles.GetDefaultConnectionConfigByMajor(_major).Item1;

            if (childConnectionMask == 0)
            {
                ConnectionSource =
                    allConnectionConfigID.Select(x => new
                        {DisplayName = Profiles.GetConnectionStringByConfigID(x, _major), Value = x}).ToList();

                if (!allConnectionConfigID.Contains(oldConnectionConfigID))
                {
                    _connectionConfigID = allConnectionConfigID.Contains(defaultConnectionConfigID)
                        ? defaultConnectionConfigID
                        : allConnectionConfigID[0];
                }
            }
            else
            {
                foreach (var i in allConnectionConfigID)
                {
                    if ((i & childConnectionMask) > 0)
                    {
                        defaultConnectionConfigID = i;
                        break;
                    }
                }

                List<uint> finalList = new List<uint> {defaultConnectionConfigID};

                ConnectionSource = finalList.Select(x => new
                    {DisplayName = Profiles.GetConnectionStringByConfigID(x, _major), Value = x}).ToList();

                _connectionConfigID = defaultConnectionConfigID;

            }

            RaisePropertyChanged("ConnectionConfigID");
        }

        private void UpdateEKeySource()
        {
            var eKeyList = new List<ElectronicKeyingType>
            {
                ElectronicKeyingType.ExactMatch,
                ElectronicKeyingType.CompatibleModule,
                ElectronicKeyingType.Disabled
            };

            EKeySource = EnumHelper.ToDataSource<ElectronicKeyingType>(eKeyList);
        }

        private void UpdateSeriesSource()
        {
            var supportMajorList = Profiles.GetSupportMajorListByConnectionConfigID(_connectionConfigID);

            var seriesList = new List<string>();
            foreach (var major in supportMajorList)
            foreach (var majorRev in Profiles.MajorRevs)
                if (majorRev.MajorRev == major)
                    if (!seriesList.Contains(majorRev.Series))
                        seriesList.Add(majorRev.Series);

            seriesList.Sort();

            var oldSeries = _series;

            SeriesSource = seriesList;

            if (!seriesList.Contains(oldSeries))
            {
                _series = seriesList[0];
            }

            RaisePropertyChanged("Series");
        }

        private void UpdateMajorSource()
        {
            var supportMajorList = Profiles.GetSupportMajorListByConnectionConfigID(_connectionConfigID);

            var majorList = new List<int>();
            foreach (var major in supportMajorList)
            foreach (var majorRev in Profiles.MajorRevs)
                if (majorRev.MajorRev == major && majorRev.Series == _series)
                    if (!majorList.Contains(major))
                        majorList.Add(major);

            majorList.Sort();

            var oldMajor = _major;

            MajorSource = majorList;

            if (!majorList.Contains(oldMajor))
            {
                _major = majorList[0];
            }

            RaisePropertyChanged("Major");
        }

        private bool CheckIsDirty()
        {
            if (Series != ModifiedAdapter.Series)
                return true;

            if (Major != ModifiedAdapter.Major)
                return true;

            if (Minor != ModifiedAdapter.Minor)
                return true;

            if (EKey != ModifiedAdapter.EKey)
                return true;

            if (ConnectionConfigID != ModifiedAdapter.ConnectionConfigID)
                return true;

            if (ChassisSize != ModifiedAdapter.ChassisSize)
                return true;

            return false;
        }

        private void ExecuteCancelCommand()
        {
            DialogResult = false;
        }

        private void ExecuteOkCommand()
        {
            if (CheckIsDirty())
            {
                // warning
                //StringBuilder messageBuilder = new StringBuilder();
                //messageBuilder.AppendLine("These changes will cause module data types and properties to change.");
                //messageBuilder.AppendLine(
                //    "Data will be set to default values unless it can be recovered from the existing module properties.");
                //messageBuilder.AppendLine("Verify module properties before Applying changes.");
                //messageBuilder.AppendLine();
                //messageBuilder.AppendLine("Change module definition?");

                DialogResult = MessageBox.Show(LanguageManager.GetInstance().ConvertSpecifier("ModuleDefinition.DIOEnetAdapter"), "ICS Studio",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Exclamation) == MessageBoxResult.Yes;
            }
            else
            {
                DialogResult = false;
            }
        }
    }
}