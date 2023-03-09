using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DataType;

namespace ICSStudio.Dialogs.SelectDataType
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class SelectDataTypeViewModel : ViewModelBase
    {
        private readonly IController _controller;

        private List<DataTypeItem> _allDataTypeItems;
        private DataTypeItem _selectedDataTypeItem;

        private bool? _dialogResult;

        private string _dataType;

        private bool _dim2Enabled;
        private bool _dim1Enabled;
        private bool _dim0Enabled;
        private int _dim0;
        private int _dim1;
        private int _dim2;

        private bool _beShowByGroups;

        private readonly bool _supportsOneDimensionalArray;
        private readonly bool _supportsMultiDimensionalArrays;

        public SelectDataTypeViewModel(IController controller, string dataType,
            bool supportsOneDimensionalArray,
            bool supportsMultiDimensionalArrays)
        {
            _controller = controller;
            _beShowByGroups = true;
            _supportsOneDimensionalArray = supportsOneDimensionalArray;
            _supportsMultiDimensionalArrays = supportsMultiDimensionalArrays;

            DataType = dataType;

            OkCommand = new RelayCommand(ExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            HelpCommand = new RelayCommand(ExecuteHelpCommand);

            UpdateItemSource();

        }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        public string DataType
        {
            get { return _dataType; }
            set
            {
                Set(ref _dataType, value);

                UpdateDimensionsEnabled();
                UpdateSelectedItem();
            }
        }

        public int Dim0
        {
            get { return _dim0; }
            set
            {
                Set(ref _dim0, value);
                DimensionChanged();
            }
        }

        public int Dim1
        {
            get { return _dim1; }
            set
            {
                Set(ref _dim1, value);
                DimensionChanged();
            }
        }

        public int Dim2
        {
            get { return _dim2; }
            set
            {
                Set(ref _dim2, value);
                DimensionChanged();
            }
        }

        public bool Dim2Enabled => _dim2Enabled;

        public bool Dim1Enabled => _dim1Enabled;

        public bool Dim0Enabled => _dim0Enabled;

        public Visibility TreeViewVisibility
        {
            get
            {
                if (BeShowByGroups)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public Visibility ListBoxVisibility
        {
            get
            {
                if (!BeShowByGroups)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public ObservableCollection<DataTypeItem> ListBoxSource { get; private set; }
        public ObservableCollection<string> AllDataTypeNames { get; private set; }

        public ObservableCollection<DataTypeItem> TreeViewSource { get; private set; }

        public DataTypeItem SelectedDataTypeItem
        {
            get { return _selectedDataTypeItem; }
            set
            {
                Set(ref _selectedDataTypeItem, value);

                SelectedDataTypeItemToDataType();
            }
        }

        public bool BeShowByGroups
        {
            get { return _beShowByGroups; }
            set
            {
                Set(ref _beShowByGroups, value);

                RaisePropertyChanged("TreeViewVisibility");
                RaisePropertyChanged("ListBoxVisibility");
            }
        }

        public RelayCommand OkCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand HelpCommand { get; }

        #region Command

        // public for double click
        public void ExecuteOkCommand()
        {
            if (IsValidDataType(DataType))
                DialogResult = true;
        }

        private void ExecuteHelpCommand()
        {
            //TODO(gjc):add code here
        }

        private void ExecuteCancelCommand()
        {
            DialogResult = false;
        }

        #endregion

        private void UpdateDimensionsEnabled()
        {
            string dataType = DataType;
            string typeName;
            int dim0, dim1, dim2;
            int errorCode;

            var isValid = _controller.DataTypes.ParseDataType(
                dataType,
                out typeName,
                out dim0,
                out dim1,
                out dim2, out errorCode);

            if (isValid)
            {
                var foundDataType = _controller.DataTypes[typeName];
                if (!foundDataType.SupportsOneDimensionalArray)
                {
                    _dim0 = 0;
                    _dim1 = 0;
                    _dim2 = 0;

                    _dim0Enabled = false;
                    _dim1Enabled = false;
                    _dim2Enabled = false;
                }
                else if (foundDataType.SupportsMultiDimensionalArrays)
                {
                    _dim0 = dim0;
                    _dim0Enabled = true;

                    if (_dim0 > 0)
                    {
                        _dim1 = dim1;
                        _dim1Enabled = true;

                        if (_dim1 > 0)
                        {
                            _dim2 = dim2;
                            _dim2Enabled = true;
                        }
                        else
                        {
                            _dim2 = 0;
                            _dim2Enabled = false;
                        }
                    }
                    else
                    {
                        _dim1 = 0;
                        _dim1Enabled = false;
                        _dim2 = 0;
                        _dim2Enabled = false;
                    }
                }
                else if (foundDataType.SupportsOneDimensionalArray)
                {
                    _dim0 = dim0;
                    _dim0Enabled = true;

                    _dim1 = 0;
                    _dim1Enabled = false;
                    _dim2 = 0;
                    _dim2Enabled = false;
                }

            }
            
            if (!_supportsOneDimensionalArray)
            {
                _dim0 = 0;
                _dim1 = 0;
                _dim2 = 0;
                _dim0Enabled = false;
                _dim1Enabled = false;
                _dim2Enabled = false;
            }
            else if (!_supportsMultiDimensionalArrays)
            {
                _dim1 = 0;
                _dim2 = 0;
                _dim1Enabled = false;
                _dim2Enabled = false;
            }

            RaisePropertyChanged("Dim0");
            RaisePropertyChanged("Dim1");
            RaisePropertyChanged("Dim2");
            RaisePropertyChanged("Dim0Enabled");
            RaisePropertyChanged("Dim1Enabled");
            RaisePropertyChanged("Dim2Enabled");

        }

        private void DimensionChanged()
        {
            string dataType = DataType;

            int index = dataType.IndexOf('[');
            if (index > 0)
                dataType = dataType.Substring(0, index);

            var foundDataType = _controller.DataTypes[dataType];

            if (_dim0 == 0)
            {
                _dataType = dataType;
                _dim1 = 0;
                _dim1Enabled = false;
                _dim2 = 0;
                _dim2Enabled = false;
            }
            else
            {
                if (foundDataType != null && !foundDataType.SupportsMultiDimensionalArrays)
                {
                    _dataType = $"{dataType}[{_dim0}]";

                    _dim1 = 0;
                    _dim2 = 0;
                    _dim1Enabled = false;
                    _dim2Enabled = false;
                }
                else
                {
                    _dim1Enabled = true;

                    if (_dim1 == 0)
                    {
                        _dataType = $"{dataType}[{_dim0}]";
                        _dim2 = 0;
                        _dim2Enabled = false;
                    }
                    else
                    {
                        _dim2Enabled = true;

                        _dataType =
                            _dim2 == 0
                                ? $"{dataType}[{_dim1},{_dim0}]"
                                : $"{dataType}[{_dim2},{_dim1},{_dim0}]";
                    }
                }

            }

            if (!_supportsOneDimensionalArray)
            {
                _dim0 = 0;
                _dim1 = 0;
                _dim2 = 0;
                _dim0Enabled = false;
                _dim1Enabled = false;
                _dim2Enabled = false;
            }
            else if (!_supportsMultiDimensionalArrays)
            {
                _dim1 = 0;
                _dim2 = 0;
                _dim1Enabled = false;
                _dim2Enabled = false;
            }


            RaisePropertyChanged("DataType");
            RaisePropertyChanged("Dim0");
            RaisePropertyChanged("Dim1");
            RaisePropertyChanged("Dim2");
            RaisePropertyChanged("Dim0Enabled");
            RaisePropertyChanged("Dim1Enabled");
            RaisePropertyChanged("Dim2Enabled");
        }

        private void UpdateItemSource()
        {
            _allDataTypeItems = new List<DataTypeItem>();
            var _disableDataTypes = new DisableDataTypes();

            foreach (IDataType dataType in _controller.DataTypes)
            {
                if (dataType.Name.StartsWith("BOOL:", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (dataType.Name.Contains("$"))
                    continue;

                if (_disableDataTypes.DisableData.Contains(dataType.Name))
                    continue;

                DataTypeItem item = new DataTypeItem(dataType);
                item.PropertyChanged += ItemOnPropertyChanged;

                _allDataTypeItems.Add(item);
            }

            _allDataTypeItems.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));

            ListBoxSource = new ObservableCollection<DataTypeItem>(_allDataTypeItems);

            AllDataTypeNames = new ObservableCollection<string>(_allDataTypeItems.Select(item => item.Name).ToList());

            // 
            DataTypeItem userDefinedItem = new DataTypeItem("User-Defined") {IsExpanded = true};
            DataTypeItem addOnDefinedItem = new DataTypeItem("Add-On-Defined") {IsExpanded = true};
            DataTypeItem predefinedItem = new DataTypeItem("Predefined") {IsExpanded = true};
            DataTypeItem moduleDefinedItem = new DataTypeItem("Module-Defined") {IsExpanded = true};


            foreach (var item in _allDataTypeItems)
            {
                if (item.DataType is UserDefinedDataType)
                    userDefinedItem.AddItem(item);
                else if (item.DataType is AOIDataType)
                    addOnDefinedItem.AddItem(item);
                else if (item.DataType is ModuleDefinedDataType)
                    moduleDefinedItem.AddItem(item);
                else
                    predefinedItem.AddItem(item);

            }

            TreeViewSource = new ObservableCollection<DataTypeItem>();
            if (userDefinedItem.Items != null)
                TreeViewSource.Add(userDefinedItem);
            if (addOnDefinedItem.Items != null)
                TreeViewSource.Add(addOnDefinedItem);
            if (predefinedItem.Items != null)
                TreeViewSource.Add(predefinedItem);
            if (moduleDefinedItem.Items != null)
                TreeViewSource.Add(moduleDefinedItem);

            UpdateSelectedItem();
        }

        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsSelected"))
            {
                DataTypeItem item = sender as DataTypeItem;
                if (item != null && item.IsSelected)
                    SelectedDataTypeItem = item;
            }
        }

        private void UpdateSelectedItem()
        {
            // select item
            string inputDataType = DataType;
            string typeName;
            int dim0, dim1, dim2;
            int errorCode;

            var isValid = _controller.DataTypes.ParseDataType(
                inputDataType,
                out typeName,
                out dim0,
                out dim1,
                out dim2, out errorCode);

            if (isValid)
            {
                var selectedItem =
                    _allDataTypeItems?.Find(x => x.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
                if (selectedItem != null)
                {
                    selectedItem.IsSelected = true;
                    SelectedDataTypeItem = selectedItem;
                }
            }
        }

        private void SelectedDataTypeItemToDataType()
        {
            string inputDataType = DataType;
            string typeName;
            int dim0, dim1, dim2;
            int errorCode;

            var isValid = _controller.DataTypes.ParseDataType(
                inputDataType,
                out typeName,
                out dim0,
                out dim1,
                out dim2, out errorCode);

            if (!isValid)
            {
                _dataType = SelectedDataTypeItem.Name;
                UpdateDimensionsEnabled();
            }
            else if (!SelectedDataTypeItem.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase))
            {
                _dataType = SelectedDataTypeItem.Name;
                UpdateDimensionsEnabled();
            }

            RaisePropertyChanged("DataType");
        }

        private bool IsValidDataType(string dataType)
        {
            string warningMessage = "Failed to interpret data type.";
            string warningReason = "Tag data type string invalid.";

            string typeName;
            int dim0, dim1, dim2;
            int errorCode;

            var isValid = _controller.DataTypes.ParseDataType(
                dataType,
                out typeName,
                out dim0,
                out dim1,
                out dim2, out errorCode);

            if (errorCode == -5)
                warningReason = "Array index invalid.";

            if (isValid)
            {
                var foundDataType = _controller.DataTypes[typeName];
                if (dim1 > 0 && !foundDataType.SupportsOneDimensionalArray)
                {
                    isValid = false;
                    warningReason = "Array index invalid.";
                }
                else if (dim2 > 0 && !foundDataType.SupportsMultiDimensionalArrays)
                {
                    isValid = false;
                    warningReason = "Array index invalid.";
                }
            }


            //
            if (!isValid)
            {
                var warningDialog = new WarningDialog(
                    LanguageManager.GetInstance().ConvertSpecifier(warningMessage),
                    LanguageManager.GetInstance().ConvertSpecifier(warningReason), "")
                    {Owner = Application.Current.MainWindow};
                warningDialog.ShowDialog();
            }

            return isValid;
        }
    }

    public class DataTypeItem : ViewModelBase
    {
        private bool _isSelected;
        private bool _isExpanded;

        public DataTypeItem(IDataType dataType)
        {
            Contract.Assert(dataType != null);

            DataType = dataType;
            Name = dataType.Name;
            IconKind = "Stars";
        }

        public DataTypeItem(string name)
        {
            Name = name;
            IconKind = "Folder";
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { Set(ref _isSelected, value); }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { Set(ref _isExpanded, value); }
        }

        public string Name { get; }
        public IDataType DataType { get; }
        public List<DataTypeItem> Items { get; private set; }

        public string IconKind { get; }

        public void AddItem(DataTypeItem item)
        {
            if (Items == null)
                Items = new List<DataTypeItem>();

            Contract.Assert(item != null);
            Items.Add(item);
        }
    }
}
