using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Gui.Controls;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.Interfaces.DataType;
using ICSStudio.MultiLanguage;

namespace ICSStudio.Dialogs.Filter.Configure
{
    public class DefineTagFilterDialogViewModel:ViewModelBase
    {
        private readonly Controller _controller;
        private bool? _dialogResult;
        private string _selectedFilterOnList;
        private string _filerTypes = LanguageManager.GetInstance().ConvertSpecifier("Filter:None");
        public DefineTagFilterDialogViewModel(bool isAoi,string[] filterTypes)
        {
            _controller=Controller.GetInstance();
            OKCommand=new RelayCommand(ExecuteOKCommand);
            CancelCommand=new RelayCommand(ExecuteCancelCommand);
            ClearCommand=new RelayCommand(ExecuteClearCommand, CanExecuteClearCommand);
            CollectionChangedEventManager.AddHandler(FilterDataTypes,FilterDataTypes_CollectionChanged);
            FilterOnList.Add("<all>");
            //FilterOnList.Add("Unused");
            if (isAoi)
            {
                FilterOnList.Add("Input");
                FilterOnList.Add("Output");
                FilterOnList.Add("InOut");
                FilterOnList.Add("Local");
            }
            else
            {
                FilterOnList.Add("Produced");
                FilterOnList.Add("Consumed");
            }
            //FilterOnList.Add("Can Be Forced");
            FilterOnList.Add("Alias");
            if (filterTypes.Length > 0)
            {
                var type = filterTypes[0];
                if ("All Variables".Equals(type))
                {
                    SelectedFilterOnList = "<all>";
                }
                else if (FilterOnList.Contains(type))
                {
                    SelectedFilterOnList = type;
                }
                else
                {
                    SelectedFilterOnList = "<all>";
                }
            }
            
            SetDataTypeItem(filterTypes);

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(FilerTypes));
        }

        public string GetFilterInfo()
        {
            var info = "";
            if (SelectedFilterOnList == "<all>")
            {
                if (FilterDataTypes.Count == 0)
                {
                   
                }
                else
                {
                    info = string.Join(",", FilterDataTypes);
                }
            }
            else
            {
                if (FilterDataTypes.Count == 0)
                {
                    info = $"{SelectedFilterOnList}";
                }
                else
                {
                    info = $"{SelectedFilterOnList},{string.Join(",", FilterDataTypes)}";
                }
            }

            return info;
        }

        public List<string> FilterOnList { get; }=new List<string>();

        public string SelectedFilterOnList
        {
            set
            {
                Set(ref _selectedFilterOnList , value);
                UpdateFilterTypes();
            }
            get { return _selectedFilterOnList; }
        }

        public List<DataTypeItem> DataTypeItems { set; get; } = new List<DataTypeItem>();
        public ObservableCollection<string> FilterDataTypes { get; } = new ObservableCollection<string>();
        private void SetDataTypeItem(string[] filterTypes)
        {
            bool isFirst = true;
            DataTypeItems.Clear();

            #region UserDefined

            var userDefined = _controller.DataTypes
                .Where(d => d is UserDefinedDataType && d.FamilyType != FamilyType.StringFamily).ToList();
            if (userDefined.Count > 0)
            {
                var userDefinedRoot = new DataTypeItem(FilterDataTypes, null)
                { Name = "User-Defined", CanChooseAll = true };
                isFirst = false;
                userDefinedRoot.Index = 1;
                foreach (var dataType in userDefined)
                {
                    var d = new DataTypeItem(FilterDataTypes, null) { Name = dataType.Name };
                    userDefinedRoot.AddChild(d);
                    if (filterTypes.Contains(d.Name))
                        d.CheckType = CheckType.Half;
                }

                DataTypeItems.Add(userDefinedRoot);
            }

            #endregion

            #region String

            var stringRoot = new DataTypeItem(FilterDataTypes, null) { Name = "Strings", CanChooseAll = true };
            if (isFirst)
            {
                stringRoot.Index = 1;
            }
            
            var strings = _controller.DataTypes.Where(d => d.FamilyType == FamilyType.StringFamily);
            foreach (var dataType in strings)
            {
                var s1 = new DataTypeItem(FilterDataTypes, null) { Name = dataType.Name };
                stringRoot.AddChild(s1);
                if (filterTypes.Contains(s1.Name))
                    s1.CheckType = CheckType.Half;
            }

            DataTypeItems.Add(stringRoot);

            #endregion

            #region Add-On-Defined

            var aois = _controller.AOIDefinitionCollection.ToList();
            if (aois.Count > 0)
            {
                var aoiRoot = new DataTypeItem(FilterDataTypes, null)
                { Name = "Add-On-Defined", CanChooseAll = true };
                foreach (var dataType in aois)
                {
                    var d = new DataTypeItem(FilterDataTypes, null) { Name = dataType.Name };
                    aoiRoot.AddChild(d);
                    if (filterTypes.Contains(d.Name))
                        d.CheckType = CheckType.Half;
                }

                DataTypeItems.Add(aoiRoot);
            }

            #endregion

            #region Predefined

            var dataTypeList = new List<string>();
            var predefined = _controller.DataTypes.Where(d => d.IsPredefinedType).ToList();
            if (predefined.Count > 0)
            {
                var predefinedRoot = new DataTypeItem(FilterDataTypes, null)
                { Name = "Predefined", CanChooseAll = true };
                foreach (var dataType in predefined)
                {
                    if (dataType.FamilyType == FamilyType.StringFamily ||
                        dataType.Name.Equals("string", StringComparison.OrdinalIgnoreCase)) continue;
                    var dataTypeName = dataType.Name;
                    if (dataTypeName.IndexOf(":") > 0)
                    {
                        dataTypeName = dataTypeName.Substring(0, dataTypeName.IndexOf(":"));
                    }

                    if (dataTypeList.Contains(dataTypeName)) continue;
                    dataTypeList.Add(dataTypeName);
                    var d = new DataTypeItem(FilterDataTypes, null) { Name = dataTypeName };
                    predefinedRoot.AddChild(d);
                    if (filterTypes.Contains(d.Name))
                        d.CheckType = CheckType.Half;
                }

                DataTypeItems.Add(predefinedRoot);
            }

            #endregion

            #region Module-Defined

            var moduleDefined = _controller.DataTypes.Where(d => d is ModuleDefinedDataType).ToList();
            if (moduleDefined.Count > 0)
            {
                var moduleRoot = new DataTypeItem(FilterDataTypes, null)
                { Name = "Module-Defined", CanChooseAll = true };
                foreach (var dataType in moduleDefined)
                {
                    var d = new DataTypeItem(FilterDataTypes, null) { Name = dataType.Name };
                    moduleRoot.AddChild(d);
                    if (filterTypes.Contains(d.Name))
                        d.CheckType = CheckType.Half;
                }

                DataTypeItems.Add(moduleRoot);
            }

            #endregion
        }

        public bool CanExecuteClearCommand()
        {
            return FilerTypes != "Filter:None" && FilerTypes != "滤波器：无";
        } 

        public string FilerTypes
        {
            set { Set(ref _filerTypes , value); }
            get { return _filerTypes; }
        }

        public bool? DialogResult
        {
            set { Set(ref _dialogResult , value); }
            get { return _dialogResult; }
        }
        

        public RelayCommand OKCommand { get; }

        private void ExecuteOKCommand()
        {
            DialogResult = true;
        }

        public RelayCommand CancelCommand { get; }

        private void ExecuteCancelCommand()
        {
            DialogResult = false;
        }

        public RelayCommand ClearCommand { get; }

        private void ExecuteClearCommand()
        {
            SelectedFilterOnList = "<all>";
            foreach (var dataTypeItem in DataTypeItems)
            {
                dataTypeItem.CheckType = CheckType.Null;
            }
            FilerTypes = LanguageManager.GetInstance().ConvertSpecifier("Filter:None");
        }
        
        private void FilterDataTypes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateFilterTypes();
        }

        private void UpdateFilterTypes()
        {
            if (SelectedFilterOnList == "<all>")
            {
                if (FilterDataTypes.Count == 0)
                {
                    FilerTypes = LanguageManager.GetInstance().ConvertSpecifier("Filter:None");
                }
                else
                {
                    FilerTypes = $"{LanguageManager.GetInstance().ConvertSpecifier("Filter:")}" +
                                 $"{string.Join(LanguageManager.GetInstance().ConvertSpecifier("or"), FilterDataTypes)}";
                }
            }
            else
            {
                if (FilterDataTypes.Count == 0)
                {
                    FilerTypes = $"{LanguageManager.GetInstance().ConvertSpecifier("Filter:")}{SelectedFilterOnList}";
                }
                else
                {
                    FilerTypes = $"{LanguageManager.GetInstance().ConvertSpecifier("Filter:")}{SelectedFilterOnList}" +
                                 $"{LanguageManager.GetInstance().ConvertSpecifier("and")}" +
                                 $"({string.Join(LanguageManager.GetInstance().ConvertSpecifier("or"), FilterDataTypes)})";
                }
            }
            ClearCommand.RaiseCanExecuteChanged();
        }

        public override void Cleanup()
        {
            base.Cleanup();

            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }
    }
}
