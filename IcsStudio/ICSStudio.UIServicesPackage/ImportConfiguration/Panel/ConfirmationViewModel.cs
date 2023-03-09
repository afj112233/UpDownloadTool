using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.UIServicesPackage.ImportConfiguration.Dialog;
using ICSStudio.UIServicesPackage.ImportConfiguration.Panel.Validate;
using ICSStudio.UIServicesPackage.Services;
using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json.Linq;
using Type = System.Type;
using ICSStudio.MultiLanguage;

namespace ICSStudio.UIServicesPackage.ImportConfiguration.Panel
{
    public class ConfirmationViewModel : ViewModelBase,IVerify
    {
        private readonly ProjectItemType _type;
        private readonly JObject _config;
        private string _selectedOperation;
        private string _finalName;
        private string _commandContent;
        private string _description;
        private bool _descriptionReadOnly;
        private readonly IBaseObject _baseObject;


        public ConfirmationViewModel(ProjectItemType type, JObject config, IBaseObject baseObject)
        {
            _type = type;
            _baseObject = baseObject;
            _config = config;
            PropertiesCommand = new RelayCommand(ExecutePropertiesCommand);
            Name = config["Name"]?.ToString();
            FinalName = Name;

            var controller = Controller.GetInstance();
            if (_type == ProjectItemType.AddOnDefined || _type == ProjectItemType.AddOnInstructions)
            {
                AoiVisibility = Visibility.Visible;
                Revision = config["Revision"]?.ToString();
                RevisionNote = config["RevisionNote"]?.ToString();
                Vendor = config["Vendor"]?.ToString();
            }
            else if (_type == ProjectItemType.Program)
            {
                ProgramPropertiesVisibility = Visibility.Visible;
                Tasks.Add("<none>");
                foreach (var task in controller.Tasks) Tasks.Add(task.Name);

                SelectedTask = (_baseObject as ITask)?.Name ?? "<none>";

                Parents.Add("<none>");
                foreach (var program in controller.Programs)
                {
                    if (program.Name == controller.PowerLossProgram ||
                        program.Name == controller.MajorFaultProgram) continue;
                    Parents.Add(program.Name);
                }

                SelectedParent = "<none>";

                _inhibit = (bool)config["Inhibited"];

                MainRoutine = config["MainRoutineName"]?.ToString() ?? "<none>";

                FaultRoutine = config["FaultRoutineName"]?.ToString() ?? "<none>";
            }
            else if (_type == ProjectItemType.Routine)
            {
                RoutinePropertiesVisibility = Visibility.Visible;
                Type = ((RoutineType)(byte)config["Type"]).ToString();
                foreach (var program in controller.Programs) Programs.Add(program.Name);

                SelectedProgram = (baseObject as IProgramModule)?.Name;

                Lines = config["CodeText"]?.Count().ToString();
            }
            else if (type == ProjectItemType.ModuleDefined || type == ProjectItemType.Ethernet)
            {
                ModuleVisibility = Visibility.Visible;
                ParentModule = (baseObject as DeviceModule)?.Name;
            }
        }

        public event EventHandler SelectedOperationChanged;
        
        public JObject Config => _config;

        public ProjectItemType ProjectItemType => _type;

        #region module

        public string ParentModule { set; get; }

        public Visibility ModuleVisibility { set; get; } = Visibility.Collapsed;

        #endregion

        #region program

        public Visibility ProgramPropertiesVisibility { set; get; } = Visibility.Collapsed;

        public string SelectedTask
        {
            set
            {
                _selectedTask = value;
                _config["ScheduleIn"] = value;
            }
            get { return _selectedTask; }
        }

        public List<string> Tasks { set; get; }=new List<string>();

        public List<string> Parents { set; get; } = new List<string>();

        public string SelectedParent
        {
            set
            {
                _selectedParent = value;
                _config["Parent"] = value;
            }
            get { return _selectedParent; }
        }

        public bool Inhibit
        {
            set
            {
                _inhibit = value;
                _config["Inhibited"] = value;
            }
            get { return _inhibit; }
        }

        public string MainRoutine { set; get; }

        public string FaultRoutine { set; get; }

        #endregion

        #region Routine

        public Visibility RoutinePropertiesVisibility { set; get; } = Visibility.Collapsed;

        public string Type { set; get; }

        public List<string> Programs { set; get; } = new List<string>();

        public string SelectedProgram { set; get; }

        public string Lines { set; get; }

        #endregion

        #region aoi

        public string Revision { set; get; }

        public string RevisionNote { set; get; }

        public string Vendor { set; get; }

        public Visibility AoiVisibility { set; get; } = Visibility.Collapsed;

        #endregion

        public void CollectionChangedName()
        {
            if (!Name.Equals(FinalName, StringComparison.OrdinalIgnoreCase))
            {
                Controller.GetInstance().Changed
                    .Add(new Tuple<Type, string, string>(ProjectInfoService.GetItemType(_type), Name, FinalName));
                _config["Name"] = FinalName;
            }

            if (_selectedOperation == "Use Existing")
            {
                _config["Description"] = _baseComponent?.Description;
            }
        }


        public string Name { set; get; }

        //TODO(zyl):前端限制
        public string FinalName
        {
            set
            {
                _finalName = value;
                _config["FinalName"] = value;
                Verify();
            }
            get { return _finalName; }
        }

        public string Error
        {
            get { return _error; }
            set {
                if (string.Compare(_error, value, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    _error = value;
                    if (!string.IsNullOrEmpty(_error))
                    {
                        MessageBox.Show(_error, "ICSStudio", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        public void Verify()
        {
            Operations.Clear();
            if (ProjectInfoService.IsExist(_type, _baseObject, FinalName))
            {
                Operations.Add("Overwrite");
                Operations.Add("Use Existing");
                SelectedOperation = ProjectInfoService.IsSame(_type, _baseObject, FinalName) ? "Use Existing" : "Overwrite";
                CommandContent = "Collision Detail...";
            }
            else
            {
                Operations.Add("Create");
                Operations.Add("Discard");
                SelectedOperation = "Create";
                CommandContent = "Properties...";
            }
        }
        
        public string Description
        {
            set
            {
                _description = value;
                _config["Description"] = value;
            }
            get { return _description; }
        }

        public ObservableCollection<string> Operations { get; } = new ObservableCollection<string>();
        private IBaseComponent _baseComponent;
        private bool _inhibit;
        private string _selectedTask;
        private string _selectedParent;
        private string _error;

        public string SelectedOperation
        {
            set
            {
                Set(ref _selectedOperation, value);
                _config["Operation"] = value;
                if (_selectedOperation == "Use Existing")
                {
                    DescriptionReadOnly = true;
                    _baseComponent = ProjectInfoService.GetExistObject(_type, null, FinalName);
                    _description = _baseComponent?.Description;
                    RaisePropertyChanged("Description");
                }
                else
                {
                    DescriptionReadOnly = false;
                    _description = _config["Description"]?.ToString();
                    RaisePropertyChanged("Description");
                }
                SelectedOperationChanged?.Invoke(this,new EventArgs());
            }
            get { return _selectedOperation; }
        }

        public bool DescriptionReadOnly
        {
            set { Set(ref _descriptionReadOnly, value); }
            get { return _descriptionReadOnly; }
        }

        public RelayCommand PropertiesCommand { get; }

        public string CommandContent
        {
            set { Set(ref _commandContent, value); }
            get
            {
                var text = LanguageManager.GetInstance().ConvertSpecifier(_commandContent);
                return string.IsNullOrEmpty(text)? _commandContent:text;
            }
        }

        private void ExecutePropertiesCommand()
        {
            try
            {
#pragma warning disable VSTHRD010 // 在主线程上调用单线程类型
                var uiShell = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
#pragma warning restore VSTHRD010 // 在主线程上调用单线程类型
                ImportDialogVM vm;
                if (SelectedOperation.Equals("Create") || SelectedOperation.Equals("Discard"))
                {
                    vm = new ImportDialogVM(_type, SelectedOperation, _config, null);
                }
                else
                {
                    vm = new ImportDialogVM(_type, SelectedOperation, _config,
                        ProjectInfoService.GetExistObject(_type, _baseObject, FinalName));
                }

                var dialog = new ImportPropertiesDialog(vm);
                vm.CloseAction = dialog.Close;
                dialog.ResizeMode = ResizeMode.NoResize;
                if (SelectedOperation.Equals("Create") || SelectedOperation.Equals("Discard"))
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

                SelectedOperation = vm.CurrentOperation;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}
