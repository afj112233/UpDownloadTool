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
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.UIServicesPackage.ImportConfiguration.Dialog;
using ICSStudio.UIServicesPackage.ImportConfiguration.Panel.Validate;
using ICSStudio.UIServicesPackage.Services;
using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json.Linq;
using Type = System.Type;

namespace ICSStudio.UIServicesPackage.ImportConfiguration.Panel
{
    public class ReferenceViewModel : ViewModelBase, IVerify
    {
        private Visibility _revisionVisibility = Visibility.Collapsed;

        public ReferenceViewModel(List<JToken> configs, JObject except, ProjectItemType type, IBaseObject baseObject,
            bool isReadOnly = false)
        {
            IsReadOnly = isReadOnly;
            if (configs != null)
            {
                foreach (JObject config in configs)
                {
                    if (config == except) continue;
                    if (type == ProjectItemType.ProgramTags)
                    {
                        //var program = baseObject as IProgramModule;
                        //var exist = program?.Tags[config["Name"]?.ToString()];
                        var item = new ReferenceItem(config, type, baseObject);
                        Items.Add(item);
                    }
                    else if (type == ProjectItemType.ControllerTags)
                    {
                        var name = config["Name"]?.ToString();
                        var exit = Controller.GetInstance().Tags[name];
                        var item = new ReferenceItem(config, type, exit);
                        Items.Add(item);
                    }
                    else if (type == ProjectItemType.UserDefined)
                    {
                        var exit = Controller.GetInstance().DataTypes[config["Name"]?.ToString()];
                        //if ((exit as AssetDefinedDataType)?.IsTmp ?? true) exit = null;
                        var item = new ReferenceItem(config, type, exit);
                        Items.Add(item);
                    }
                    else if (type == ProjectItemType.AddOnDefined)
                    {
                        var exit =
                            ((AoiDefinitionCollection) Controller.GetInstance().AOIDefinitionCollection).Find(
                                config["Name"]?.ToString());
                        //if (exit.IsTmp) exit = null;
                        var item = new ReferenceItem(config, type, exit);
                        Items.Add(item);
                    }
                    else if (type == ProjectItemType.ModuleDefined)
                    {
                        var exit = Controller.GetInstance().DeviceModules[config["Name"]?.ToString()];
                        
                        var item = new ReferenceItem(config, type, exit);
                        Items.Add(item);
                    }
                    else
                    {
                        var item = new ReferenceItem(config, type, baseObject);
                        Items.Add(item);
                    }

                }
            }

            if (type == ProjectItemType.AddOnDefinedTags)
            {
                var exist = baseObject as AoiDefinition;
                if (exist != null)
                {
                    foreach (var existTag in exist.Tags)
                    {
                        if(configs?.Any(c=>existTag.Name.Equals((string)c["Name"]))??false)continue;
                        if("EnableIn".Equals(existTag.Name)||"EnableOut".Equals(existTag.Name))continue;
                        var item=new ReferenceItem(existTag);
                        Items.Add(item);
                    }
                }
            }
            if (type == ProjectItemType.AddOnDefinedTags || type == ProjectItemType.ControllerTags ||
                type == ProjectItemType.ProgramTags)
            {
                TagPropertiesVisibility = Visibility.Visible;
                if (type == ProjectItemType.ProgramTags || type == ProjectItemType.AddOnDefinedTags)
                {
                    IsReadOnly = true;
                }
                else
                {
                    IsReadOnly = false;
                }
            }

            if (type == ProjectItemType.AddOnDefined)
            {
                RevisionVisibility = Visibility.Visible;
            }
            else if (type == ProjectItemType.UserDefined)
            {

            }
            else if (type == ProjectItemType.Routine)
            {
                TypeVisibility = Visibility.Visible;
            }
            else if (type == ProjectItemType.ModuleDefined || type == ProjectItemType.Ethernet)
            {
                ModulePropertiesVisibility = Visibility.Visible;
            }
        }

        public ReferenceViewModel(List<Tuple<string,string>> otherComponents)
        {
            PropertiesCommandVisibility = Visibility.Collapsed;
            DescriptionVisibility = Visibility.Collapsed;
            ClassNameVisibility = Visibility.Visible;

            foreach (var item in otherComponents)
            {
                Items.Add(new OtherComponent(item.Item1,item.Item2));
            }

            IsReadOnly = true;
        }

        public bool IsReadOnly { set; get; } = false;

        public Visibility ModulePropertiesVisibility { set; get; } = Visibility.Collapsed;

        public Visibility TypeVisibility { set; get; } = Visibility.Collapsed;

        public Visibility TagPropertiesVisibility { set; get; } = Visibility.Collapsed;

        public void CollectionChangedName()
        {
            foreach (var item in Items)
            {
                (item as ReferenceItem)?.CollectionChangedName();
            }
        }

        public Visibility RevisionVisibility
        {
            set { Set(ref _revisionVisibility, value); }
            get { return _revisionVisibility; }
        }

        public Visibility PropertiesCommandVisibility { get; } = Visibility.Visible;
        public Visibility DescriptionVisibility { get; } = Visibility.Visible;
        public Visibility ClassNameVisibility { get; } = Visibility.Collapsed;

        public ObservableCollection<ImportComponent> Items { get; } = new ObservableCollection<ImportComponent>();

        public string Error
        {
            get
            {
                foreach (var item in Items)
                {
                    var referenceItem = item as ReferenceItem;
                    if (referenceItem != null && !string.IsNullOrEmpty(referenceItem.Error))
                    {
                        return referenceItem.Error;
                    }
                }

                return "";
            }
            set
            {
                //ignore
            }
        }
    }

    public abstract class ImportComponent : ViewModelBase
    {
        public abstract string Name { get; }
        public abstract string Operation { get; set; }
        public abstract string FinalName { get; set; }
        public abstract ObservableCollection<string> Operations { get; }
        protected abstract void ResetOperation();
    }

    public sealed class ReferenceItem : ImportComponent, IVerify
    {
        private JObject _config;
        private ProjectItemType _type;
        private string _finalName;
        private string _operation;
        private string _description;
        private IBaseComponent _baseComponent;
        private bool _descriptionReadonly = true;
        private string _type1;
        private IBaseObject _exist;
        private string _usage;
        private string _aliasFor;
        private string _dataType;
        private string _error;

        private readonly bool _isFormal = true;

        public ReferenceItem(JObject config, ProjectItemType type, IBaseObject exist)
        {
            _config = config;
            _exist = exist;
            _type = type;
            Name = config["Name"]?.ToString();

            if (config["CanChangeScope"] != null)
            {
                CanChangeScope = (bool) config["CanChangeScope"];
                if (Name.StartsWith("\\"))
                {
                    _programName = Name.Substring(1, Name.IndexOf(".") - 1);
                    Name = Name.Substring(_programName.Length + 2);
                    config["Name"] = Name;
                }
            }

            Description = config["Description"]?.ToString();
            Command = new RelayCommand(ExecuteCommand);
            //var controller = Controller.GetInstance();
            if (type == ProjectItemType.AddOnDefined)
            {
                Revision = config["Revision"]?.ToString();
                RevisionNote = config["RevisionNote"]?.ToString();
            }

            if (type == ProjectItemType.AddOnDefinedTags || type == ProjectItemType.ControllerTags ||
                type == ProjectItemType.ProgramTags)
            {
                Usage = config["Usage"] != null
                    ? ((Usage) (byte) config["Usage"]).ToString()
                    : Interfaces.Tags.Usage.Local.ToString();
                AliasFor = config["AliasFor"]?.ToString();
                DataType = config["DataType"]?.ToString();
                DescriptionReadonly = true;
            }

            if (type == ProjectItemType.Routine)
            {
                Type = ((RoutineType) (byte) config["Type"]).ToString();
            }

            if (type == ProjectItemType.ModuleDefined || type == ProjectItemType.Ethernet)
            {
                ParentModule = config["ParentModule"]?.ToString();
            }

            FinalName = Name;
        }

        public ReferenceItem(IBaseComponent baseComponent)
        {
            _isFormal = false;
            FinalName = baseComponent.Name;
            Description = baseComponent.Description;

            if (baseComponent is ITag)
            {
                var tag = baseComponent as ITag;
                _type = ProjectItemType.AddOnDefinedTags;
                Usage = tag.Usage.ToString();
                DataType = tag.DataTypeInfo.ToString();
            }

            if (baseComponent is IRoutine)
            {
                _type = ProjectItemType.Routine;
                var routine = baseComponent as IRoutine;
                Type = routine.Type.ToString();
            }
        }

        public ProjectItemType ProjectItemType => _type;

        public JObject Config => _config;

        public string ParentModule { set; get; }

        public string Error
        {
            set
            {
                if (string.Compare(_error, value, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    _error = value;
                    if (!string.IsNullOrEmpty(_error))
                    {
                        MessageBox.Show(_error, "ICSStudio", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            get { return _error; }
        }

        public string Usage
        {
            set { Set(ref _usage, value); }
            get { return _usage; }
        }

        public string AliasFor
        {
            set { Set(ref _aliasFor, value); }
            get { return _aliasFor; }
        }

        public string DataType
        {
            set { Set(ref _dataType, value); }
            get { return _dataType; }
        }

        public void CollectionChangedName()
        {
            if(!_isFormal)return;
            if (!Name.Equals(FinalName, StringComparison.OrdinalIgnoreCase))
            {
                Controller.GetInstance().Changed
                    .Add(new Tuple<Type, string, string>(ProjectInfoService.GetItemType(_type), Name, FinalName));
                if (_type == ProjectItemType.AddOnDefined)
                {
                    Controller.GetInstance().Changed
                        .Add(new Tuple<Type, string, string>(typeof(IDataType), Name, FinalName));
                }

                _config["Name"] = _finalName;
            }

            //if (_type == ProjectItemType.ControllerTags && FinalName.StartsWith("\\"))
            //{
            //    _config["FinalName"] = FinalName.Substring(FinalName.IndexOf(".") + 1);
            //    _config["Name"] = _config["FinalName"];
            //}

            if (Operation == "Use Existing")
            {
                _config["Description"] = _baseComponent?.Description;
            }
        }

        public string Type
        {
            set { _type1 = value; }
            get { return _type1; }
        }

        public RelayCommand Command { get; }

        private void ExecuteCommand()
        {
            try
            {

#pragma warning disable VSTHRD010 // 在主线程上调用单线程类型
                var uiShell = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
#pragma warning restore VSTHRD010 // 在主线程上调用单线程类型
                var vm = new ImportDialogVM(_type, Operation, _config,
                    (_type == ProjectItemType.ProgramTags ? (_exist as Program)?.Tags[_finalName] : _exist)
                    as BaseComponent);
                var dialog = new ImportPropertiesDialog(vm);
                dialog.ResizeMode = ResizeMode.NoResize;
                if (Operation.Equals("Create") || Operation.Equals("Discard"))
                {
                    dialog.Width = 488;
                    dialog.Height = 515;
                }
                else
                {
                    dialog.Width = 640;
                    dialog.Height = 515;
                }

                vm.CloseAction = dialog.Close;
                dialog.ShowDialog(uiShell);

                Operation = vm.CurrentOperation;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public override string Name { get; } = string.Empty;

        public override ObservableCollection<string> Operations { get; } = new ObservableCollection<string>();

        public override string Operation
        {
            set
            {
                Set(ref _operation, value);
                if(!_isFormal)return;
                _config["Operation"] = value;
                CommandVisibility = "Undefined".Equals(value) ? Visibility.Hidden : Visibility.Visible;
                RaisePropertyChanged("CommandVisibility");
                if (_operation == "Use Existing")
                {
                    DescriptionReadonly = true;
                    _baseComponent = ProjectInfoService.GetExistObject(_type, null, FinalName);
                    _description = _baseComponent?.Description;
                    RaisePropertyChanged("Description");
                }

                if (_operation == "Overwrite")
                {
                    DescriptionReadonly = false;
                    _description = _config["Description"]?.ToString();
                    RaisePropertyChanged("Description");
                }
                else
                {
                    DescriptionReadonly = true;
                    _description = _config["Description"]?.ToString();
                    RaisePropertyChanged("Description");
                }
            }
            get { return _operation; }
        }

        public bool CanChangeScope { set; get; } = true;

        private readonly string _programName = "";

        public override string FinalName
        {
            set
            {
                var finalName = value;
                if (!CanChangeScope)
                {
                    if (finalName.StartsWith("\\"))
                    {
                        finalName = finalName.Substring(finalName.IndexOf(".") + 1);
                    }
                }

                _finalName = finalName;
                ResetOperation();
                if (_config != null)
                {
                    _config["FinalName"] = finalName;
                }
            }
            get
            {
                if (!CanChangeScope && !string.IsNullOrEmpty(_programName))
                    return $"\\{_programName}.{_finalName}";
                return _finalName;
            }
        }

        public bool FinalNameEnable { set; get; } = true;

        public Visibility CommandVisibility { set; get; } = Visibility.Visible;

        protected override void ResetOperation()
        {
            if(string.IsNullOrEmpty(Name)) return;

            Operations.Clear();

            if (Name.Contains(":"))
            {
                Operations.Add("Undefined");
                Operation = "Undefined";
                return;
            }

            if (_type == ProjectItemType.AddOnDefined)
            {
                if (ProjectInfoService.IsExist(_type, _exist, FinalName))
                {
                    Operations.Add("Overwrite");
                    Operations.Add("Use Existing");
                    Operation = "Use Existing";
                    DescriptionReadonly = true;
                }
                else
                {
                    Operations.Add("Create");
                    Operations.Add("Discard");
                    Operation = "Create";
                    DescriptionReadonly = false;
                }
            }
            else if (_type == ProjectItemType.UserDefined)
            {
                if (ProjectInfoService.IsExist(_type, _exist, FinalName))
                {
                    Operations.Add("Overwrite");
                    Operations.Add("Use Existing");
                    Operation = "Use Existing";
                    DescriptionReadonly = true;
                }
                else
                {
                    Operations.Add("Create");
                    Operations.Add("Discard");
                    Operation = "Create";
                    DescriptionReadonly = false;
                }
            }
            else if (_type == ProjectItemType.AddOnDefinedTags)
            {
                if (!_isFormal)
                {
                    Operations.Add("Delete");
                    Operations.Add("Use Existing");
                    Operation = "Delete";
                }
                else
                {
                    if (ProjectInfoService.IsExist(_type, _exist, FinalName))
                    {
                        Operations.Add("Overwrite");
                        Operations.Add("Use Existing");
                        if ("EnableIn".Equals(FinalName) || "EnableOut".Equals(FinalName))
                        {
                            Operation = "Overwrite";
                        }
                        else
                        {
                            Operation = "Use Existing";
                        }
                    }
                    else
                    {
                        Operations.Add("Create");
                        Operations.Add("Discard");
                        Operation = "Create";
                    }
                }

                DescriptionReadonly = true;
            }
            else if (_type == ProjectItemType.Routine)
            {
                if (ProjectInfoService.IsExist(_type, _exist, FinalName))
                {
                    Operations.Add("Overwrite");
                    Operations.Add("Use Existing");
                    Operation = "Overwrite";
                }
                else
                {
                    Operations.Add("Create");
                    Operations.Add("Discard");
                    Operation = "Create";
                }

                DescriptionReadonly = true;
            }
            else if (_type == ProjectItemType.ControllerTags)
            {
                var name = FinalName;
                var tag = ObtainValue.NameToTag(name, null)?.Item1;
                if (tag == null)
                {
                    if (name.IndexOf(".") > 0)
                    {
                        var index = name.IndexOf(".");
                        var program = name.Substring(1, index == -1 ? name.Length : index - 1);
                        if (Controller.GetInstance().Programs[program] != null)
                        {
                            DataType = _config["DataType"]?.ToString();
                            Usage = _config["Usage"] != null
                                ? ((Usage) (byte) _config["Usage"]).ToString()
                                : Interfaces.Tags.Usage.Local.ToString();
                            AliasFor = _config["AliasFor"]?.ToString();
                            _description = _config["Description"]?.ToString();

                            Operations.Add("Create");
                            Operations.Add("Discard");
                            RaisePropertyChanged("Description");
                            Operation = "Create";
                            DescriptionReadonly = false;
                        }
                        else
                        {
                            Operations.Add("Discard");
                            Operation = "Discard";
                            DataType = _config["DataType"]?.ToString();
                            Usage = _config["Usage"] != null
                                ? ((Usage) (byte) _config["Usage"]).ToString()
                                : Interfaces.Tags.Usage.Local.ToString();
                            AliasFor = _config["AliasFor"]?.ToString();
                            _description = _config["Description"]?.ToString();
                            RaisePropertyChanged("Description");
                            DescriptionReadonly = true;
                        }

                    }
                    else
                    {
                        if (_config["Use"]?.ToString() == "Reference")
                        {
                            Operations.Add("Undefined");
                            DataType = "";
                            Usage = _config["Usage"] != null
                                ? ((Usage) (byte) _config["Usage"]).ToString()
                                : Interfaces.Tags.Usage.Local.ToString();
                            AliasFor = "";
                            _description = _config["Description"]?.ToString();
                            RaisePropertyChanged("Description");
                            Operation = "Undefined";
                            DescriptionReadonly = true;
                        }
                        else
                        {
                            DataType = _config["DataType"]?.ToString();
                            Usage = _config["Usage"] != null
                                ? ((Usage) (byte) _config["Usage"]).ToString()
                                : Interfaces.Tags.Usage.Local.ToString();
                            AliasFor = _config["AliasFor"]?.ToString();
                            _description = _config["Description"]?.ToString();

                            Operations.Add("Create");
                            Operations.Add("Discard");
                            RaisePropertyChanged("Description");
                            Operation = "Create";
                            DescriptionReadonly = false;
                        }
                    }
                }
                else
                {
                    DataType = tag.DataTypeInfo.ToString();
                    Usage = tag.Usage.ToString();
                    AliasFor = tag.AliasSpecifier;
                    _description = tag.Description;
                    _exist = tag;
                    RaisePropertyChanged("Description");
                    Operations.Add("Use Existing");
                    Operations.Add("Overwrite");
                    Operation = "Use Existing";
                    DescriptionReadonly = true;
                }

                if (DataType.Equals("MOTION_GROUP",StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var item in Controller.GetInstance().Tags)
                    {
                        if (item.DataTypeInfo.DataType.IsMotionGroupType)
                        {
                            if (!item.Name.Equals(_finalName, StringComparison.OrdinalIgnoreCase))
                            {
                                Operations.Clear();
                                Operations.Add("Discard");
                                Operation = "Discard";
                            }

                            break;
                        }
                    }
                }
            }
            else
            {
                if (ProjectInfoService.IsExist(_type, _exist, FinalName))
                {
                    Operations.Add("Overwrite");
                    Operations.Add("Use Existing");
                    Operation = "Use Existing";
                    DescriptionReadonly = true;
                }
                else
                {
                    Operations.Add("Create");
                    Operations.Add("Discard");
                    Operation = "Create";
                    DescriptionReadonly = false;
                }
            }
        }

        public string Revision { set; get; }

        public string RevisionNote { set; get; }

        public string Description
        {
            set
            {
                _description = value;
                if (_config != null)
                {
                    _config["Description"] = value;
                }
            }
            get { return _description; }
        }

        public bool DescriptionReadonly
        {
            set { Set(ref _descriptionReadonly, value); }
            get { return _descriptionReadonly; }
        }

    }

    public sealed class OtherComponent : ImportComponent
    {
        private string _operation;
        private string _finalName;
        private readonly ObservableCollection<string> _operations = new ObservableCollection<string>();

        public OtherComponent(string name,string className)
        {
            Name = name;
            ClassName = className;

            FinalName = Name;
        }
        public override string Name { get; }

        public override string Operation
        {
            get { return _operation; }
            set { Set(ref _operation, value); }
        }

        public override string FinalName
        {
            get { return _finalName; }
            set
            {
                Set(ref _finalName, value);
                ResetOperation();
            }
        }

        public string ClassName { get;}

        public override ObservableCollection<string> Operations => _operations;

        protected override void ResetOperation()
        {
            _operations.Clear();
            IBaseObject exist = null;
            var controller = Controller.GetInstance();
            if ("Task".EndsWith(ClassName, StringComparison.OrdinalIgnoreCase))
            {
                exist = controller.Tasks[FinalName];
            }
            else if ("Program".Equals(ClassName, StringComparison.OrdinalIgnoreCase))
            {
                exist = controller.Programs[FinalName];
            }
            else if ("Routine".Equals(ClassName, StringComparison.OrdinalIgnoreCase))
            {
                foreach (var item in controller.Programs)
                {
                    exist = item.Routines[FinalName];
                    if(exist != null) break;
                }
            }
            else if ("Module".Equals(ClassName, StringComparison.OrdinalIgnoreCase))
            {
                exist = controller.DeviceModules[FinalName];
            }
            if(exist != null)
            {
                _operations.Add("Use Existing");
                RaisePropertyChanged(nameof(Operations));
                Operation = "Use Existing";
            }
            else
            {
                _operations.Add("Undefined");
                RaisePropertyChanged(nameof(Operations));
                Operation = "Undefined";
            }
        }
    }
}
