using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.UIServicesPackage.ImportConfiguration.Panel.Validate;
using Newtonsoft.Json.Linq;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.UIServicesPackage.ImportConfiguration.Dialog;
using ICSStudio.UIServicesPackage.Services;
using ICSStudio.UIServicesPackage.ImportConfiguration.Model;
using Microsoft.VisualStudio.Shell.Interop;
using ProgramType = ICSStudio.UIServicesPackage.ImportConfiguration.Model.ProgramType;

namespace ICSStudio.UIServicesPackage.ImportConfiguration.MultiplePanel
{
    class MultipleItemsViewModel:ViewModelBase,IVerify
    {
        private ItemVisibility _itemVisibility = new ItemVisibility();
        private const string None = "<none>";
        private string _importedItemsTitle;
        private readonly ProjectItemType _type;
        private readonly IBaseObject _baseObject;
        private readonly Controller _controller;
        private JArray _dataList;
        private IsChangeExistingPrograms _isChangeExistingPrograms=new IsChangeExistingPrograms();
        private OperationStatus _selectedOperation;
        private string _selectedTask;
        private string _selectedParent;
        private List<OperationStatus> _operationItems = new List<OperationStatus>();
        private List<string> _taskItems = new List<string>();
        private List<string> _parentItems = new List<string>();

        public MultipleItemsViewModel(ProjectItemType type, JArray dataList, IBaseObject baseObject)
        {
            _type = type;
            _dataList = dataList;
            _baseObject = baseObject;
            _controller = Controller.GetInstance();

            CollisionCommand = new RelayCommand(ExecuteCollisionCommand);

            MultipleItemsInit();
        }

        public ItemVisibility ItemVisibility
        {
            get { return _itemVisibility; }
            set { Set(ref _itemVisibility, value); }
        }

        public string ImportedItemsTitle
        {
            get { return _importedItemsTitle; }
            set { Set(ref _importedItemsTitle, value); }
        }
        public IsChangeExistingPrograms IsChangeExistingPrograms
        {
            get { return _isChangeExistingPrograms; }
            set
            {
                foreach (var item in DataItems)
                {
                    item.IsChangeExistingPrograms = value;
                }
                Set(ref _isChangeExistingPrograms, value);
            }
        }
        public List<OperationStatus> OperationItems
        {
            get { return _operationItems; }
            set { Set(ref _operationItems, value); }
        }
        public OperationStatus SelectedOperation
        {
            get { return _selectedOperation; }
            set
            {
                foreach (var item in DataItems)
                {
                    item.SelectedOperation = value;
                }
                Set(ref _selectedOperation, value);
            }
        }
        public List<string> TaskItems
        {
            get { return _taskItems; }
            set { Set(ref _taskItems, value); }
        }
        public string SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                foreach (var item in DataItems)
                {
                    item.SelectedScheduleIn = value;
                }
                Set(ref _selectedTask, value);
            }
        }

        public List<string> ParentItems
        {
            get { return _parentItems; }
            set { Set(ref _parentItems, value); }
        }
        public ObservableCollection<string> ReferenceFinalNameList { get; set; } = new ObservableCollection<string>();

        public string SelectedParent
        {
            get { return _selectedParent; }
            set
            {
                foreach (var item in DataItems)
                {
                    item.SelectedParent = value;
                }
                Set(ref _selectedParent, value);
            }
        }
        public string Error { get; set; }
        public ObservableCollection<DataListItem> DataItems { get; set; }= new ObservableCollection<DataListItem>();
        public List<string> FinalNameItems { get; set; }= new List<string>();
        public DataListItem SelectedDataItem { get; set; }

        public RelayCommand CollisionCommand { get; }

        private void ExecuteCollisionCommand()
        {

#pragma warning disable VSTHRD010 // 在主线程上调用单线程类型
            var uiShell = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
#pragma warning restore VSTHRD010 // 在主线程上调用单线程类型
            ImportDialogVM vm;
            var operation = SelectedDataItem.Operation.ToString();
            if (operation.Equals("Create") || operation.Equals("Discard"))
            {
                vm = new ImportDialogVM(_type, operation, (JObject)SelectedDataItem.ImportConfig, null);
            }
            else
            {
                vm = new ImportDialogVM(SelectedDataItem.ProjectItemType, operation, (JObject)SelectedDataItem.ImportConfig, SelectedDataItem.ExistingConfig);
            }
            var dialog = new ImportPropertiesDialog(vm);
            vm.CloseAction = dialog.Close;
            dialog.ResizeMode = ResizeMode.NoResize;
            if (operation.Equals("Create") || operation.Equals("Discard"))
            {
                dialog.Width = 488;
                dialog.Height = 515;
            }
            else
            {
                dialog.Width = 640;
                dialog.Height = 515;
            }
            dialog.ShowDialog(uiShell);
        }

        private void MultipleItemsInit()
        {
            if (_type == ProjectItemType.UserDefined || _type == ProjectItemType.Strings)
            {
                ImportedItemsTitle = $"{_dataList.Count()} "+ LanguageManager.GetInstance().ConvertSpecifier("selected, 0 others as references"); ;
                ItemVisibility.UserDefinedOrStringsVisibility = Visibility.Visible;

                DataItems.Clear();
                FinalNameItems.Clear();
                foreach (var item in _dataList)
                {
                    var dataTypes = (JObject)item;
                    var dataTypesItem = new DataListItem(_type,_baseObject,FinalNameItems)
                    {
                        ImportConfig = item,
                        ImportName = dataTypes["Name"]?.ToString(),
                        FinalName = dataTypes["Name"]?.ToString(),
                        Description = dataTypes["Description"]?.ToString()
                    };
                    DataItems.Add(dataTypesItem);
                }

                ReferenceFinalNameList.Clear();
                foreach (var item in _controller.DataTypes)
                {
                    ReferenceFinalNameList.Add(item.Name);
                }
            }
            else if (_type == ProjectItemType.AddOnDefined || _type == ProjectItemType.AddOnInstructions)
            {
                ImportedItemsTitle = $"{_dataList.Count()} " + LanguageManager.GetInstance().ConvertSpecifier("selected, 0 others as references");
                ItemVisibility.AoiVisibility = Visibility.Visible;

                DataItems.Clear();
                FinalNameItems.Clear();
                foreach (var item in _dataList)
                {
                    var aoi = (JObject)item;
                    var aoiItem = new DataListItem(_type,_baseObject,FinalNameItems)
                    {
                        ImportConfig = item,
                        ImportName = aoi["Name"]?.ToString(),
                        FinalName = aoi["Name"]?.ToString(),
                        Revision = aoi["Revision"]?.ToString(),
                        RevisionNote = aoi["RevisionNote"]?.ToString(),
                        Description = aoi["Description"]?.ToString()

                    };
                    DataItems.Add(aoiItem);
                }

                ReferenceFinalNameList.Clear();
                foreach (var item in _controller.AOIDefinitionCollection)
                {
                    ReferenceFinalNameList.Add(item.Name);
                }
            }
            else if (_type == ProjectItemType.Program)
            {
                ImportedItemsTitle = $"{_dataList.Count()} " + LanguageManager.GetInstance().ConvertSpecifier("target programs, 0 child program(s)");
                ItemVisibility.ProgramVisibility = Visibility.Visible;
                CreateProgramItems(_dataList);
                CreateOperationItems();
                CreateTaskItems();
                CreateParentItems();
            }
        }

        #region program

        private void CreateProgramItems(JArray programList)
        {
            DataItems.Clear();
            FinalNameItems.Clear();
            IsChangeExistingPrograms.IsEnabledChangeParent = false;
            IsChangeExistingPrograms.IsEnabledChangeScheduling = false;
            foreach (var item in programList)
            {
                var program = (JObject)item;
                var programItem = new DataListItem(_type,_baseObject,FinalNameItems)
                {
                    ImportConfig = item,
                    ImportName = program["Name"]?.ToString(),
                    Operation = ProjectInfoService.IsExist(_type, _baseObject, program["Name"]?.ToString())
                        ? OperationStatus.Overwrite : OperationStatus.Create,
                    FinalName = program["Name"]?.ToString(),
                    Inhibit = (bool)program["Inhibited"],
                    ProgramType = ((ProgramType)(int)program["Type"]).ToString(),
                    ScheduleIn = SelectedTask,
                    Description = program["Description"]?.ToString(),
                };
                DataItems.Add(programItem);
            }
        }

        private void CreateOperationItems()
        {
            OperationItems.Add(OperationStatus.Import);
            SelectedOperation = OperationStatus.Import;
            OperationItems.Add(OperationStatus.Discard);
        }
        private void CreateTaskItems()
        {
            TaskItems.Clear();
            TaskItems.Add(None);
            SelectedTask = (_baseObject as ITask)?.Name ?? None;
            var tasks = _controller.Tasks;
            foreach (var item in tasks)
            {
                TaskItems.Add(item.Name);
            }
            TaskItems.Add("Unscheduled");
        }

        private void CreateParentItems()
        {
            ParentItems.Clear();
            ReferenceFinalNameList.Clear();
            ParentItems.Add(None);
            SelectedParent = None;
            var programs = _controller.Programs;
            foreach (var item in programs)
            {
                ReferenceFinalNameList.Add(item.Name);

                if(item.Name == _controller.PowerLossProgram|| item.Name == _controller.MajorFaultProgram) continue;
                ParentItems.Add(item.Name);
            }
        }

        #endregion
    }
}
