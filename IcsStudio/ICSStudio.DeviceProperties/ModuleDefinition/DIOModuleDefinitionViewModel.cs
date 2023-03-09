using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.DeviceProfiles.DIOEnetAdapter;
using ICSStudio.DeviceProfiles.DIOModule;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.Gui.Utils;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties.ModuleDefinition
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class DIOModuleDefinitionViewModel : ViewModelBase
    {
        private bool? _dialogResult;

        private string _series;
        private int _major;
        private int _minor;
        private ElectronicKeyingType _eKey;
        private uint _connectionConfigID;
        private string _dataFormat;

        private List<string> _seriesSource;
        private List<int> _majorSource;
        private IList _eKeySource;
        private IList _connectionSource;
        private IList _dataFormatSource;

        public DIOModuleDefinitionViewModel(ModifiedDIOModule modifiedDIOModule)
        {
            ModifiedDIOModule = modifiedDIOModule;

            _series = ModifiedDIOModule.Series;
            _major = ModifiedDIOModule.Major;
            _minor = ModifiedDIOModule.Minor;
            _eKey = ModifiedDIOModule.EKey;
            _connectionConfigID = ModifiedDIOModule.ConnectionConfigID;
            _dataFormat = Profiles.GetDataFormatStringByConfigID(_connectionConfigID, _major);

            UpdateSeriesSource();
            UpdateMajorSource();
            UpdateEKeySource();
            UpdateConnectionSource();

            CreateDataFormatSource();

            OkCommand = new RelayCommand(ExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        private void CreateDataFormatSource()
        {
            DataFormatSource = new List<string> {_dataFormat};
        }

        private void UpdateConnectionSource()
        {
            List<uint> finalList = new List<uint>();

            var oldConnectionConfigID = _connectionConfigID;
            var allConnectionConfigID = Profiles.GetConnectionConfigIDListByMajor(_major);

            LocalModule localModule = ModifiedDIOModule.OriginalDeviceModule.ParentModule as LocalModule;
            if (localModule != null)
            {
                // only Data
                finalList.Add(allConnectionConfigID[0]);
            }
            else
            {
                CommunicationsAdapter adapter =
                    ModifiedDIOModule.OriginalDeviceModule.ParentModule as CommunicationsAdapter;

                uint mask = 0;
                if (adapter != null)
                {
                    uint parentConnectionConfigID = adapter.ConfigID;

                    mask = parentConnectionConfigID & (uint) (DIOConnectionTypeMask.ListenOnlyRack
                                                              | DIOConnectionTypeMask.Rack
                                                              | DIOConnectionTypeMask.EnhancedRack);
                }

                foreach (var i in allConnectionConfigID)
                {
                    if (i < (uint) DIOConnectionTypeMask.Rack)
                        finalList.Add(i);

                    else if ((i & mask) > 0)
                        finalList.Add(i);
                }
            }

            ConnectionSource = finalList.Select(x => new
                {DisplayName = Profiles.GetConnectionStringByConfigID(x, _major), Value = x}).ToList();

            if (!finalList.Contains(oldConnectionConfigID))
                _connectionConfigID = finalList[0];


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

            if (Profiles.CatalogNumber == "Embedded")
            {
                majorList.Clear();

                var parentModule = ModifiedDIOModule.OriginalDeviceModule.ParentModule as DeviceModule;
                Contract.Assert(parentModule != null);
                majorList.Add(parentModule.Major);
            }

            var oldMajor = _major;

            MajorSource = majorList;

            if (!majorList.Contains(oldMajor))
            {
                _major = majorList[0];
            }

            RaisePropertyChanged("Major");
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

        public ModifiedDIOModule ModifiedDIOModule { get; set; }
        public DIOModuleProfiles Profiles => ModifiedDIOModule.Profiles;

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

        public Visibility SeriesVisibility
        {
            get
            {
                if (Profiles.CatalogNumber == "Embedded")
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
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
                if (_connectionConfigID != value)
                {
                    Set(ref _connectionConfigID, value);
                    RaisePropertyChanged("Title");
                }

            }
        }

        public IList ConnectionSource
        {
            get { return _connectionSource; }
            set { Set(ref _connectionSource, value); }
        }

        public string DataFormat
        {
            get { return _dataFormat; }
            set { Set(ref _dataFormat, value); }
        }

        public IList DataFormatSource
        {
            get { return _dataFormatSource; }
            set { Set(ref _dataFormatSource, value); }
        }

        public Visibility DataFormatVisibility
        {
            get
            {
                if (Profiles.CatalogNumber == "Embedded")
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        public RelayCommand OkCommand { get; }
        public RelayCommand CancelCommand { get; }

        private bool CheckIsDirty()
        {
            if (SeriesVisibility == Visibility.Visible
                && Series != ModifiedDIOModule.Series)
                return true;

            if (Major != ModifiedDIOModule.Major)
                return true;

            if (Minor != ModifiedDIOModule.Minor)
                return true;

            if (EKey != ModifiedDIOModule.EKey)
                return true;

            if (ConnectionConfigID != ModifiedDIOModule.ConnectionConfigID)
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
    }
}
