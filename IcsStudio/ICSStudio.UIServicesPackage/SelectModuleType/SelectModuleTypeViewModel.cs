using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Database.Database;
using ICSStudio.Database.Table;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.UIServicesPackage.SelectModuleType.Common;
using System.Linq;
using ICSStudio.Cip;
using ICSStudio.DeviceProfiles.DIOEnetAdapter;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.Interfaces.Notification;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.UIInterfaces.Dialog;

namespace ICSStudio.UIServicesPackage.SelectModuleType
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class SelectModuleTypeViewModel : ViewModelBase
    {
        private readonly IController _controller;
        private readonly IDeviceModule _parentModule;
        private readonly PortType _portType;
        private readonly DBHelper _dbHelper;
        private readonly Dictionary<int, string> _categoryDictionary;
        private readonly Dictionary<int, string> _descriptionDictionary;
        private readonly List<ProductDetail> _allProducts;

        private bool? _dialogResult;
        private bool _closeOnCreateChecked;

        private bool? _allCategoryFiltersChecked;
        private bool? _allVendorFiltersChecked;

        private string _searchText;
        private string _toggleButtonText;
        private string _searchResultStatus;
        private Visibility _filtersVisibility;

        private ObservableCollection<DisplayModuleItem> _searchItemsSource;
        private ObservableCollection<FilterItem> _categoryFiltersSource;
        private ObservableCollection<FilterItem> _vendorFiltersSource;

        private DisplayModuleItem _selectedSearchItem;

        private readonly DeviceModuleFactory _factory = new DeviceModuleFactory();

        public SelectModuleTypeViewModel(IController controller, IDeviceModule parentModule, PortType portType)
        {
            _controller = controller;
            _parentModule = parentModule;
            _portType = portType;
            _dbHelper = new DBHelper();
            _categoryDictionary = GetCategoryDictionary();
            _descriptionDictionary = GetDescriptionDictionary(1033);

            _allProducts = GetAllProductsByPortType(portType);

            //TODO(gjc): remove later
            RemoveUnsupported(_allProducts);

            // Command
            SelectCommand = new RelayCommand<DisplayModuleItem>(ExecuteSelectCommand);
            ClearFiltersCommand = new RelayCommand(ExecuteClearFiltersCommand);
            ToggleButtonClickCommand = new RelayCommand(ExecuteToggleButtonClickCommand);
            AddToFavoritesCommand = new RelayCommand(ExecuteAddToFavoritesCommand, CanAddToFavoritesCommand);
            CreateCommand = new RelayCommand(ExecuteCreateCommand, CanExecuteCreateCommand);
            CloseCommand = new RelayCommand(ExecuteCloseCommand);
            HelpCommand = new RelayCommand(ExecuteHelpCommand);

            UpdateCategoryFiltersSource(_allProducts);
            UpdateVendorFiltersSource(_allProducts);

            _searchText = string.Empty;
            _toggleButtonText = LanguageManager.GetInstance().ConvertSpecifier("Hide Filters") + " ∧";
            _filtersVisibility = Visibility.Visible;
            _allCategoryFiltersChecked = true;
            _allVendorFiltersChecked = true;

            UpdateSearchItemsSource();

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(ToggleButtonText));
            RaisePropertyChanged(nameof(SearchResultStatus));
        }

        private void RemoveUnsupported(List<ProductDetail> products)
        {
            if (products == null)
                return;

            if (products.Count == 0)
                return;

            List<string> catalogNumbers = new List<string>()
            {
                "2198-H003-ERS2",
                "2198-H008-ERS2",
                "2198-H015-ERS2",
                "2198-H025-ERS2",
                "2198-H040-ERS2",
                "2198-H070-ERS2",
                "2198-D006-ERS3",
                "2198-D012-ERS3",
                "2198-D020-ERS3",
                "2198-D032-ERS3",
                "2198-D057-ERS3",
                "2094-EN02D-M01-S0",
                "2094-EN02D-M01-S1"
            };

            for (int index = products.Count - 1; index > 0; index--)
            {
                if (catalogNumbers.Contains(products[index].CatalogNumber))
                {
                    products.RemoveAt(index);
                }
            }
        }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        public bool CloseOnCreateChecked
        {
            get { return _closeOnCreateChecked; }
            set { Set(ref _closeOnCreateChecked, value); }
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                Set(ref _searchText, value);
                UpdateSearchItemsSource();
            }
        }

        public string ToggleButtonText
        {
            get { return _toggleButtonText; }
            set { Set(ref _toggleButtonText, value); }
        }

        public string SearchResultStatus
        {
            get { return _searchResultStatus; }
            set { Set(ref _searchResultStatus, value); }
        }

        public Visibility FiltersVisibility
        {
            get { return _filtersVisibility; }
            set { Set(ref _filtersVisibility, value); }
        }

        public bool? AllCategoryFiltersChecked
        {
            get { return _allCategoryFiltersChecked; }
            set
            {
                Set(ref _allCategoryFiltersChecked, value);

                UpdateCategoryFiltersCheckedState();

                UpdateSearchItemsSource();
            }
        }

        public bool? AllVendorFiltersChecked
        {
            get { return _allVendorFiltersChecked; }
            set
            {
                Set(ref _allVendorFiltersChecked, value);

                UpdateVendorFiltersCheckedState();

                UpdateSearchItemsSource();
            }
        }

        public ObservableCollection<FilterItem> CategoryFiltersSource
        {
            get { return _categoryFiltersSource; }
            set { Set(ref _categoryFiltersSource, value); }
        }

        public ObservableCollection<FilterItem> VendorFiltersSource
        {
            get { return _vendorFiltersSource; }
            set { Set(ref _vendorFiltersSource, value); }
        }

        public ObservableCollection<DisplayModuleItem> SearchItemsSource
        {
            get { return _searchItemsSource; }
            set { Set(ref _searchItemsSource, value); }
        }

        public DisplayModuleItem SelectedSearchItem
        {
            get { return _selectedSearchItem; }
            set
            {
                Set(ref _selectedSearchItem, value);

                CreateCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand ClearFiltersCommand { get; set; }
        public RelayCommand ToggleButtonClickCommand { get; set; }

        public RelayCommand<DisplayModuleItem> SelectCommand { get; set; }
        public RelayCommand AddToFavoritesCommand { get; set; }
        public RelayCommand CreateCommand { get; set; }
        public RelayCommand CloseCommand { get; set; }
        public RelayCommand HelpCommand { get; set; }

        #region Command

        private bool CanAddToFavoritesCommand()
        {
            return SelectedSearchItem != null;
        }

        private void ExecuteAddToFavoritesCommand()
        {
            // TODO(gjc):add code here
        }

        private void ExecuteSelectCommand(DisplayModuleItem item)
        {
            ExecuteCreateCommand();
        }

        private void ExecuteClearFiltersCommand()
        {
            SearchText = string.Empty;
            AllCategoryFiltersChecked = true;
            AllVendorFiltersChecked = true;
        }

        private void ExecuteToggleButtonClickCommand()
        {
            switch (FiltersVisibility)
            {
                case Visibility.Visible:
                    ToggleButtonText = LanguageManager.GetInstance().ConvertSpecifier("Show Filters") + " ∨";
                    FiltersVisibility = Visibility.Collapsed;
                    break;
                case Visibility.Collapsed:
                    ToggleButtonText = LanguageManager.GetInstance().ConvertSpecifier("Hide Filters") + " ∧";
                    FiltersVisibility = Visibility.Visible;
                    break;
            }
        }

        private bool CanExecuteCreateCommand()
        {
            return SelectedSearchItem != null;
        }

        private void ExecuteCreateCommand()
        {
            if (SelectedSearchItem != null)
            {
                var productDetail = SelectedSearchItem.ProductDetail;

                //1. Create Device Module
                var deviceModule =
                    _factory.Create((CipDeviceType)productDetail.ProductType, productDetail.CatalogNumber);

                //2. Open Device Module Properties
                if (deviceModule != null)
                {
                    deviceModule.ParentModule = _parentModule;
                    deviceModule.ParentModuleName = _parentModule.Name;

                    var parentPort = ((DeviceModule)_parentModule).GetFirstPort(_portType);
                    Contract.Assert(parentPort != null);
                    deviceModule.ParentModPortId = parentPort.Id;

                    UpdateDeviceModuleInitialValue(_controller, deviceModule);

                    var dialog = new NewDeviceModuleDialog(_controller, deviceModule)
                    {
                        Owner = Application.Current.MainWindow
                    };

                    //3. Close
                    if (dialog.ShowDialog() == true)
                    {
                        DeviceModuleCollection deviceModules = _controller.DeviceModules as DeviceModuleCollection;
                        Contract.Assert(deviceModules != null);

                        deviceModule.ParentController = _controller;
                        deviceModules.AddDeviceModule(deviceModule);

                        deviceModule.RebuildDeviceTag();

                        //update config tag
                        DiscreteIO discreteIO = deviceModule as DiscreteIO;
                        AnalogIO analogIO = deviceModule as AnalogIO;
                        if (discreteIO != null || analogIO != null)
                        {
                            ICanApply canApply = dialog.DataContext as ICanApply;
                            canApply?.Apply();
                        }

                        // for EnhancedRack
                        if (discreteIO != null && discreteIO.IsEnhancedRack)
                        {
                            ((DeviceModule)_parentModule).RebuildDeviceTag();
                        }

                        Notifications.Publish(new MessageData() { Type = MessageData.MessageType.Verify });
                        if (CloseOnCreateChecked)
                            DialogResult = true;
                    }

                }

            }
        }

        private void UpdateDeviceModuleInitialValue(IController controller, DeviceModule deviceModule)
        {
            //
            DiscreteIO discreteIO = deviceModule as DiscreteIO;
            if (discreteIO != null)
            {
                // slot
                discreteIO.Slot = GetMinUnusedSlot(controller, discreteIO.ParentModule, PortType.PointIO);

                // connection
                uint connectionConfigID = GetMatchConnectionConfigID(discreteIO);
                discreteIO.ChangeConnectionConfigID(connectionConfigID);
            }

            AnalogIO analogIO = deviceModule as AnalogIO;
            if (analogIO != null)
            {
                // slot
                analogIO.Slot = GetMinUnusedSlot(controller, analogIO.ParentModule, PortType.PointIO);

                // connection
                uint connectionConfigID = GetMatchConnectionConfigID(analogIO);
                analogIO.ChangeConnectionConfigID(connectionConfigID);
            }

        }

        private uint GetMatchConnectionConfigID(DiscreteIO discreteIO)
        {
            var allConnectionConfigID = discreteIO.Profiles.GetConnectionConfigIDListByMajor(discreteIO.Major);

            if (discreteIO.ParentModule is LocalModule)
                return allConnectionConfigID[0];

            CommunicationsAdapter adapter =
                discreteIO.ParentModule as CommunicationsAdapter;
            if (adapter != null)
            {
                uint parentConnectionConfigID = adapter.ConfigID;

                uint mask = parentConnectionConfigID & (uint)(DIOConnectionTypeMask.ListenOnlyRack
                                                              | DIOConnectionTypeMask.Rack
                                                              | DIOConnectionTypeMask.EnhancedRack);

                foreach (var i in allConnectionConfigID)
                {
                    if ((i & mask) > 0)
                        return i;
                }
            }

            return allConnectionConfigID[0];
        }

        private uint GetMatchConnectionConfigID(AnalogIO analogIO)
        {
            var allConnectionConfigID = analogIO.Profiles.GetConnectionConfigIDListByMajor(analogIO.Major);

            if (analogIO.ParentModule is LocalModule)
                return allConnectionConfigID[0];

            CommunicationsAdapter adapter =
                analogIO.ParentModule as CommunicationsAdapter;
            if (adapter != null)
            {
                uint parentConnectionConfigID = adapter.ConfigID;

                uint mask = parentConnectionConfigID & (uint)(DIOConnectionTypeMask.ListenOnlyRack
                                                              | DIOConnectionTypeMask.Rack
                                                              | DIOConnectionTypeMask.EnhancedRack);

                foreach (var i in allConnectionConfigID)
                {
                    if ((i & mask) > 0)
                        return i;
                }
            }

            return allConnectionConfigID[0];
        }

        private int GetMinUnusedSlot(IController controller, IDeviceModule parentModule, PortType portType)
        {
            List<int> usedSlotList = new List<int>();

            foreach (var module in controller.DeviceModules)
            {
                DeviceModule deviceModule = module as DeviceModule;
                if (deviceModule != null && deviceModule != parentModule && deviceModule.ParentModule == parentModule)
                {
                    var port = deviceModule.GetFirstPort(portType);
                    if (port != null && port.Upstream)
                    {
                        usedSlotList.Add(int.Parse(port.Address));
                    }
                }
            }

            usedSlotList.Sort();

            int minSlot = 1;

            foreach (var slot in usedSlotList)
            {
                if (minSlot < slot)
                    break;

                minSlot++;
            }

            return minSlot;
        }

        private void ExecuteCloseCommand()
        {
            DialogResult = false;
        }

        private void ExecuteHelpCommand()
        {
            //TODO(gjc): add code here
        }

        #endregion

        #region Private

        private Dictionary<int, string> GetCategoryDictionary()
        {
            var categoryDictionary = new Dictionary<int, string>();

            var allCategories = _dbHelper.GetAllCategories();

            foreach (var category in allCategories)
                categoryDictionary.Add(category.No, category.Name);

            return categoryDictionary;
        }

        private Dictionary<int, string> GetDescriptionDictionary(int lcid)
        {
            var descriptionDictionary = new Dictionary<int, string>();

            var allDescriptions = _dbHelper.GetDescriptions(lcid);
            foreach (var description in allDescriptions)
                descriptionDictionary.Add(description.ProductID, description.Text);

            return descriptionDictionary;
        }

        private List<ProductDetail> GetAllProductsByPortType(PortType portType)
        {
            var products = _dbHelper.GetAllProducts();

            if (products != null)
            {
                var allProducts = new List<ProductDetail>();

                foreach (var product in products)
                {
                    var ports = product.Ports;
                    var portArray = ports.Split(',');
                    product.PortArray = Array.ConvertAll(portArray, int.Parse);

                    var categories = product.Categories;
                    var categoryArray = categories.Split(',');
                    product.CategoryArray = Array.ConvertAll(categoryArray, int.Parse);

                    if (_descriptionDictionary.ContainsKey(product.ID))
                        product.Description = _descriptionDictionary[product.ID];

                    if (product.PortArray.Contains((int)portType))
                        allProducts.Add(product);
                }

                return allProducts;
            }


            return null;
        }

        private void UpdateCategoryFiltersSource(List<ProductDetail> products)
        {
            var categoryFiltersList = new List<FilterItem>();

            //
            var categoryList = new List<int>();
            foreach (var product in products)
                categoryList.AddRange(product.CategoryArray);

            categoryList.Sort();
            categoryList = categoryList.Distinct().ToList();
            //

            foreach (var i in categoryList)
                if (_categoryDictionary.ContainsKey(i))
                {
                    var filerItem = new FilterItem
                    {
                        Checked = true,
                        Name = _categoryDictionary[i],
                        No = i,
                        Type = FilterItemType.Category
                    };

                    categoryFiltersList.Add(filerItem);
                }


            foreach (var item in categoryFiltersList)
            {
                item.PropertyChanged += FilterItemOnPropertyChanged;

            }

            CategoryFiltersSource = new ObservableCollection<FilterItem>(categoryFiltersList);
        }

        private void UpdateVendorFiltersSource(List<ProductDetail> products)
        {
            var vendorList = products.Select(product => new FilterItem
            {
                Checked = true,
                Name = product.VendorName,
                No = product.VendorID,
                Type = FilterItemType.Vendor
            }).ToList();

            var vendorFiltersList = vendorList.Distinct(new FilterItemComparer()).ToList();
            vendorFiltersList.Sort((x, y) => x.No.CompareTo(y.No));

            foreach (var item in vendorFiltersList)
            {
                item.PropertyChanged += FilterItemOnPropertyChanged;
            }

            VendorFiltersSource = new ObservableCollection<FilterItem>(vendorFiltersList);
        }

        private void UpdateCategoryFiltersCheckedState()
        {
            if (AllCategoryFiltersChecked.HasValue)
            {
                bool result = AllCategoryFiltersChecked.Value;
                foreach (var filterItem in CategoryFiltersSource)
                    filterItem.Checked = result;
            }

        }

        private void UpdateVendorFiltersCheckedState()
        {
            if (AllVendorFiltersChecked.HasValue)
            {
                bool result = AllVendorFiltersChecked.Value;
                foreach (var filterItem in VendorFiltersSource)
                    filterItem.Checked = result;
            }

        }


        private void UpdateSearchItemsSource()
        {
            var searchResult = new List<DisplayModuleItem>();

            var searchCategoryFilters = GetSearchCategoryFilters();
            var searchVendorFilters = GetSearchVendorFilters();
            var searchText = SearchText.Trim();

            // do search
            foreach (var product in _allProducts)
                if (product.Contains(searchText))
                    if (product.ContainsCategoryFilters(searchCategoryFilters))
                        if (product.ContainsVendorFilters(searchVendorFilters))
                        {
                            var displayItem = new DisplayModuleItem
                            {
                                CatalogNumber = product.CatalogNumber,
                                Category = GetCategoryString(product.CategoryArray),
                                Description = product.Description,
                                Vendor = product.VendorName,
                                ProductDetail = product
                            };

                            searchResult.Add(displayItem);
                        }

            SearchItemsSource = new ObservableCollection<DisplayModuleItem>(searchResult);

            SearchResultStatus = $"{searchResult.Count} / {_allProducts.Count} "
                                 + LanguageManager.GetInstance().ConvertSpecifier("Module Types Found");

        }

        private void FilterItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FilterItem item = sender as FilterItem;

            if (item != null && e.PropertyName.Equals("Checked"))
            {
                if (item.Type == FilterItemType.Category)
                    UpdateAllCategoryFiltersCheckedState();
                else if (item.Type == FilterItemType.Vendor)
                    UpdateAllVendorFiltersCheckedState();

                UpdateSearchItemsSource();
            }
        }

        private void UpdateAllCategoryFiltersCheckedState()
        {
            if (CategoryFiltersSource == null)
            {
                _allCategoryFiltersChecked = false;
            }
            else if (CategoryFiltersSource.Count == 0)
            {
                _allCategoryFiltersChecked = false;
            }
            else
            {
                var checkedCount = 0;
                foreach (var item in CategoryFiltersSource)
                    if (item.Checked)
                        checkedCount++;

                if (checkedCount == 0)
                    _allCategoryFiltersChecked = false;
                else if (checkedCount == CategoryFiltersSource.Count)
                    _allCategoryFiltersChecked = true;
                else
                    _allCategoryFiltersChecked = null;
            }

            RaisePropertyChanged("AllCategoryFiltersChecked");
        }

        private void UpdateAllVendorFiltersCheckedState()
        {
            if (VendorFiltersSource == null)
            {
                _allVendorFiltersChecked = false;
            }
            else if (VendorFiltersSource.Count == 0)
            {
                _allVendorFiltersChecked = false;
            }
            else
            {
                var checkedCount = 0;
                foreach (var item in VendorFiltersSource)
                    if (item.Checked)
                        checkedCount++;

                if (checkedCount == 0)
                    _allVendorFiltersChecked = false;
                else if (checkedCount == VendorFiltersSource.Count)
                    _allVendorFiltersChecked = true;
                else
                    _allVendorFiltersChecked = null;
            }

            RaisePropertyChanged("AllVendorFiltersChecked");
        }

        private string GetCategoryString(int[] categoryArray)
        {
            if (categoryArray == null)
                return string.Empty;

            if (categoryArray.Length == 0)
                return string.Empty;

            var categoryString = string.Empty;

            foreach (var i in categoryArray)
                if (_categoryDictionary.ContainsKey(i))
                    categoryString += $"{_categoryDictionary[i]},";

            categoryString = categoryString.TrimEnd(',');

            return categoryString;
        }

        private List<int> GetSearchCategoryFilters()
        {
            var searchCategoryFilters = new List<int>();

            foreach (var item in CategoryFiltersSource)
                if (item.Checked)
                    searchCategoryFilters.Add(item.No);

            return searchCategoryFilters;
        }

        private List<int> GetSearchVendorFilters()
        {
            var searchVendorFilters = new List<int>();

            foreach (var item in VendorFiltersSource)
                if (item.Checked)
                    searchVendorFilters.Add(item.No);

            return searchVendorFilters;
        }

        #endregion
    }
}
