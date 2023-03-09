using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.DeviceProfiles.MotionDrive2;
using ICSStudio.Gui.Utils;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties.ModuleDefinition
{
    public struct ServoDriveDefinition
    {
        public int Major;

        public int Minor;

        public ElectronicKeyingType EKey;

        public int PowerStructureID;

        public ConnectionType Connection;

        public bool VerifyPowerRating;
    }

    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class ServoDriveDefinitionViewModel : ViewModelBase
    {

        private bool? _dialogResult;

        private int _major;
        private int _minor;
        private ElectronicKeyingType _eKey;
        private int _powerStructureID;
        private ConnectionType _connection;
        private bool _verifyPowerRating;

        public ServoDriveDefinitionViewModel(MotionDriveProfiles profiles, ServoDriveDefinition definition)
        {
            Profiles = profiles;
            InputDefinition = definition;

            _major = definition.Major;
            _minor = definition.Minor;
            _eKey = definition.EKey;
            _powerStructureID = definition.PowerStructureID;
            _connection = definition.Connection;
            _verifyPowerRating = definition.VerifyPowerRating;

            UpdateMajorSource();
            UpdateEKeySource();
            UpdatePowerStructureSource();
            UpdateConnectionSource();

            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            OkCommand = new RelayCommand(ExecuteOkCommand);

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public MotionDriveProfiles Profiles { get; }
        public ServoDriveDefinition InputDefinition { get; }

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

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        public int Major
        {
            get { return _major; }
            set
            {
                Set(ref _major, value);
                RaisePropertyChanged("Title");
            }
        }

        public List<int> MajorSource { get; set; }

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

        public IList EKeySource { get; set; }

        public int PowerStructureID
        {
            get { return _powerStructureID; }
            set
            {
                Set(ref _powerStructureID, value);
                RaisePropertyChanged("Title");
            }
        }

        public IList PowerStructureSource { get; set; }

        public bool VerifyPowerRating
        {
            get { return _verifyPowerRating; }
            set
            {
                Set(ref _verifyPowerRating, value);
                RaisePropertyChanged("Title");
            }
        }

        public ConnectionType Connection
        {
            get { return _connection; }
            set
            {
                Set(ref _connection, value);
                RaisePropertyChanged("Title");
            }
        }

        public IList ConnectionSource { get; set; }

        public RelayCommand CancelCommand { get; }
        public RelayCommand OkCommand { get; }

        private void UpdateMajorSource()
        {
            var majorList = new List<int>();

            foreach (var rev in Profiles.Identity.MajorRevs)
            {
                majorList.Add(rev);
            }

            MajorSource = majorList;
        }

        private void UpdateEKeySource()
        {
            var eKeyList = new List<ElectronicKeyingType>()
            {
                ElectronicKeyingType.ExactMatch,
                ElectronicKeyingType.CompatibleModule,
                ElectronicKeyingType.Disabled
            };

            EKeySource = EnumHelper.ToDataSource<ElectronicKeyingType>(eKeyList);
        }

        private void UpdatePowerStructureSource()
        {
            var powerStructureList = new List<DisplayItem<int>>();
            foreach (var powerStructure in Profiles.Schema.PowerStructures)
            {
                var item = new DisplayItem<int>
                {
                    Value = powerStructure.ID,
                    DisplayName = powerStructure.GetCatalogNumber()
                };

                powerStructureList.Add(item);
            }

            PowerStructureSource = powerStructureList;
        }

        private void UpdateConnectionSource()
        {
            ConnectionSource
                = EnumHelper.ToDataSource<ConnectionType>(new List<ConnectionType>
                {
                    ConnectionType.Motion
                });
        }

        private void ExecuteCancelCommand()
        {
            DialogResult = false;
        }

        private void ExecuteOkCommand()
        {
            DialogResult = true;
        }

        private bool CheckIsDirty()
        {
            if (Major != InputDefinition.Major)
                return true;

            if (Minor != InputDefinition.Minor)
                return true;

            if (EKey != InputDefinition.EKey)
                return true;

            if (PowerStructureID != InputDefinition.PowerStructureID)
                return true;

            if (VerifyPowerRating != InputDefinition.VerifyPowerRating)
                return true;

            if (Connection != InputDefinition.Connection)
                return true;

            return false;
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
