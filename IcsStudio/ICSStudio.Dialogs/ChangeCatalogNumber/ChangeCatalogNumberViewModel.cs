using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.Objects;
using ICSStudio.Database.Database;
using ICSStudio.Database.Table.Motion;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Gui.Utils;
using ICSStudio.MultiLanguage;

namespace ICSStudio.Dialogs.ChangeCatalogNumber
{
    public class ChangeCatalogNumberViewModel : ViewModelBase
    {
        private readonly MotionDbHelper _motionDbHelper;

        private readonly IEnumerable<MotorSearchView> _allSupportMotors;
        // 0 for <all>

        private readonly ChangeCatalogNumberDialog _dialog;

        private int _feedbackType;

        private int _motorFamily;

        private ObservableCollection<string> _searchCatalogNumbers;

        private ObservableCollection<MotorDisplayItem> _searchResultSource;

        private MotorDisplayItem _selectedMotor;

        private string _selectedMotorCatalogNumber;

        private float _voltage;

        /// <summary>
        ///     Initializes a new instance of the ChangeCatalogNumberVM class.
        /// </summary>
        public ChangeCatalogNumberViewModel(
            int driveTypeId,
            List<FeedbackType> feedbackTypes,
            List<MotorType> motorTypes,
            string catalogNumber,
            ChangeCatalogNumberDialog dialog)
        {
            _dialog = dialog;
            _motionDbHelper = new MotionDbHelper();

            var feedbackList = feedbackTypes.Select(feedbackType => (int) feedbackType).ToList();
            var motorTypesList = motorTypes.Select(motorType => (int) motorType).ToList();


            _allSupportMotors = _motionDbHelper.GetSupportMotors(driveTypeId, feedbackList, motorTypesList);

            // update source
            UpdateFeedbackTypeSource(feedbackTypes);
            UpdateVoltageSource();
            UpdateMotorFamilySource(driveTypeId);

            ReloadSearchResultSource();

            //
            SelectedMotorCatalogNumber = catalogNumber;

            OkCommand = new RelayCommand(ExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            DoubleClickCommand = new RelayCommand(ExecuteDoubleClickCommand);
        }


        public RelayCommand OkCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand DoubleClickCommand { get; }

        public ObservableCollection<DisplayItem<int>> FeedbackTypeSource { get; set; }
        public ObservableCollection<DisplayItem<int>> MotorFamilySource { get; set; }

        public ObservableCollection<MotorDisplayItem> SearchResultSource
        {
            get { return _searchResultSource; }
            set { Set(ref _searchResultSource, value); }
        }

        public ObservableCollection<string> SearchCatalogNumbers
        {
            get { return _searchCatalogNumbers; }
            set { Set(ref _searchCatalogNumbers, value); }
        }


        public MotorDisplayItem SelectedMotor
        {
            get { return _selectedMotor; }
            set
            {
                Set(ref _selectedMotor, value);
                SelectedMotorCatalogNumber = value != null ? value.CatalogNumber : string.Empty;
            }
        }

        public string SelectedMotorCatalogNumber
        {
            get { return _selectedMotorCatalogNumber; }
            set
            {
                Set(ref _selectedMotorCatalogNumber, value);
                UpdateSelectedMotor(value);
            }
        }

        public int FeedbackType
        {
            get { return _feedbackType; }
            set
            {
                Set(ref _feedbackType, value);
                ReloadSearchResultSource();
            }
        }

        public ObservableCollection<DisplayItem<float>> VoltageSource { get; set; }

        public float Voltage
        {
            get { return _voltage; }
            set
            {
                Set(ref _voltage, value);
                ReloadSearchResultSource();
            }
        }

        public int MotorFamily
        {
            get { return _motorFamily; }
            set
            {
                Set(ref _motorFamily, value);
                ReloadSearchResultSource();
            }
        }

        public MotorDisplayItem SearchResult { get; set; }

        private void UpdateSelectedMotor(string catalogNumber)
        {
            if (SearchResultSource == null)
                return;

            foreach (var motorDisplayItem in SearchResultSource)
                if (motorDisplayItem.CatalogNumber.StartsWith(catalogNumber))
                {
                    _selectedMotor = motorDisplayItem;
                    // ReSharper disable once ExplicitCallerInfoArgument
                    RaisePropertyChanged("SelectedMotor");
                    return;
                }
        }

        private void ReloadSearchResultSource()
        {
            var motorList = new List<MotorDisplayItem>();


            MotorDisplayItem displayItem;

            var searchResult = _allSupportMotors;

            if (Voltage > float.Epsilon)
                searchResult = searchResult.Where(motor => Math.Abs(motor.RatedVoltage - Voltage) < float.Epsilon);

            if (MotorFamily != 0)
                searchResult = searchResult.Where(motor => motor.MotorFamilyID == MotorFamily);

            if (FeedbackType != 0)
                searchResult = searchResult.Where(motor => motor.CipID == FeedbackType);

            foreach (var supportMotor in searchResult)
            {
                displayItem = new MotorDisplayItem
                {
                    MotorId = supportMotor.ID,
                    CatalogNumber = supportMotor.CatalogNumber
                };

                motorList.Add(displayItem);
            }

            // sort
            motorList.Sort((x, y) => string.Compare(x.CatalogNumber, y.CatalogNumber, StringComparison.Ordinal));

            motorList = motorList.Distinct(new MotorDisplayItemComparer()).ToList();

            var catalogNumberList = motorList.Select(x => x.CatalogNumber).ToList();

            // insert none item
            displayItem = new MotorDisplayItem
            {
                CatalogNumber = "<none>",
                MotorId = 0
            };
            motorList.Insert(0, displayItem);

            SearchResultSource = new ObservableCollection<MotorDisplayItem>(motorList);

            SearchCatalogNumbers = new ObservableCollection<string>(catalogNumberList);
        }

        private void UpdateMotorFamilySource(int driveTypeId)
        {
            var motorFamilyList = new List<DisplayItem<int>>();
            var supportMotorFamilies = _motionDbHelper.GetSupportMotorFamilies(driveTypeId);

            DisplayItem<int> displayItem;
            foreach (var motorFamily in supportMotorFamilies)
            {
                displayItem = new DisplayItem<int>
                {
                    Value = motorFamily.MotorFamilyID,
                    DisplayName = motorFamily.Name
                };

                motorFamilyList.Add(displayItem);
            }

            // sort
            motorFamilyList.Sort((x, y) => string.Compare(x.DisplayName, y.DisplayName, StringComparison.Ordinal));

            displayItem = new DisplayItem<int>
            {
                DisplayName = "<all>",
                Value = 0
            };
            motorFamilyList.Insert(0, displayItem);

            MotorFamilySource = new ObservableCollection<DisplayItem<int>>(motorFamilyList);
        }

        private void UpdateVoltageSource()
        {
            var ratedVoltageList =
                (from motor in _allSupportMotors select motor.RatedVoltage).Distinct().ToList();
            ratedVoltageList.Sort();

            var voltageList = new List<DisplayItem<float>>();
            var displayItem = new DisplayItem<float>
            {
                DisplayName = "<all>",
                Value = 0
            };
            voltageList.Add(displayItem);

            foreach (var rateVoltage in ratedVoltageList)
            {
                displayItem = new DisplayItem<float>
                {
                    Value = rateVoltage,
                    DisplayName = rateVoltage.ToString("G")
                };

                voltageList.Add(displayItem);
            }

            VoltageSource = new ObservableCollection<DisplayItem<float>>(voltageList);
        }

        private void UpdateFeedbackTypeSource(List<FeedbackType> feedbackTypes)
        {
            var feedbackTypeList = new List<DisplayItem<int>>();
            var displayItem = new DisplayItem<int>
            {
                DisplayName = "<all>",
                Value = 0
            };
            feedbackTypeList.Add(displayItem);

            foreach (var feedbackType in feedbackTypes)
            {
                displayItem = new DisplayItem<int>
                {
                    Value = (int) feedbackType,
                    DisplayName = EnumHelper.GetEnumMember(feedbackType)
                };

                feedbackTypeList.Add(displayItem);
            }

            FeedbackTypeSource = new ObservableCollection<DisplayItem<int>>(feedbackTypeList);
        }

        private void ExecuteOkCommand()
        {
            SearchResult = null;

            // check in list
            foreach (var motorDisplayItem in SearchResultSource)
                if (string.Equals(motorDisplayItem.CatalogNumber, SelectedMotorCatalogNumber))
                {
                    SearchResult = motorDisplayItem;
                    break;
                }

            if (SearchResult == null)
            {
                // show warning 
                var warningDialog =
                    new WarningDialog(
                        LanguageManager.GetInstance().ConvertSpecifier("MotorCatalogNumberSelected"),
                        "",
                        "Error 16770-0")
                    {
                        Owner = Application.Current.MainWindow
                    };
                warningDialog.ShowDialog();
            }
            else
            {
                _dialog.DialogResult = true;
            }
        }

        private void ExecuteCancelCommand()
        {
            _dialog.DialogResult = false;
        }

        private void ExecuteDoubleClickCommand()
        {
            ExecuteOkCommand();
        }
    }

    public class MotorDisplayItem
    {
        public int MotorId { get; set; }
        public string CatalogNumber { get; set; }
    }

    public class MotorDisplayItemComparer : IEqualityComparer<MotorDisplayItem>
    {
        public bool Equals(MotorDisplayItem x, MotorDisplayItem y)
        {
            if (x == null)
                return y == null;
            return (y != null) && (x.MotorId == y.MotorId);
        }

        public int GetHashCode(MotorDisplayItem obj)
        {
            return obj.MotorId.GetHashCode();
        }
    }
}
