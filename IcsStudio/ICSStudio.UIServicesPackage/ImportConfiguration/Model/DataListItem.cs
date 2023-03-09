using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using GalaSoft.MvvmLight;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.UIServicesPackage.Services;
using Newtonsoft.Json.Linq;

namespace ICSStudio.UIServicesPackage.ImportConfiguration.Model
{
    public class DataListItem:ViewModelBase
    {
        private const string None = "<none>";
        private readonly IController _controller;

        private OperationStatus _selectedOperation;
        private string _selectedScheduleIn;
        private string _selectedParent;
        private string _importName;
        private OperationStatus _operation;
        private bool _isDifferent;
        private string _finalName;
        private bool _inhibit;
        private string _parentProgram;
        private string _scheduleIn;
        private string _description;
        private string _revision;
        private string _revisionNote;
        private string _usage;
        private string _aliasFor;
        private string _dataType;
        private string _parentModule;

        public DataListItem(ProjectItemType projectItemType,IBaseObject baseObject,List<string> finalNameItems)
        {
            ProjectItemType = projectItemType;
            BaseObject = baseObject;
            FinalNameItems = finalNameItems;

            _controller = Controller.GetInstance();
        }

        public ProjectItemType ProjectItemType { get; set; }
        public IBaseObject BaseObject { get; set; }
        public List<string> FinalNameItems { get; set; }
        public JToken ImportConfig { get; set; }
        public IBaseComponent ExistingConfig { get; set; }

        public OperationStatus SelectedOperation
        {
            get { return _selectedOperation; }
            set
            {
                UpdateOperation(value);
                _selectedOperation = value;
            }
        }
        public string SelectedScheduleIn
        {
            get { return _selectedScheduleIn; }
            set
            {
                ScheduleIn = value == None ? "" : value;
                _selectedScheduleIn = value;
            }
        }
        public string SelectedParent
        {
            get { return _selectedParent; }
            set
            {
                ParentProgram = value == None ? "" : value;
                _selectedParent = value;
            }
        }

        public IsChangeExistingPrograms IsChangeExistingPrograms { get; set; } = new IsChangeExistingPrograms();

        public string ImportName
        {
            get { return _importName; }
            set
            {
                FinalName = value;
                Set(ref _importName,value);
            }
        }

        public OperationStatus Operation
        {
            get { return _operation; }
            set
            {
                ImportConfig["Operation"] = value.ToString();
                Set(ref _operation, value);
            }
        }

        public List<OperationStatus> OperationList { get; set; } =
            new List<OperationStatus>();

        public bool IsDifferent
        {
            get { return _isDifferent; }
            set { Set(ref _isDifferent, value); }
        }

        public string FinalName
        {
            get { return _finalName; }
            set
            {
                if (value != _finalName)
                {
                    var regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
                    if (!regex.IsMatch(value))
                    {
                        MessageBox.Show("The name is invalid !");
                        return;
                    }

                    if (FinalNameItems.Contains(value))
                    {
                        MessageBox.Show("The name is invalid !");
                    }
                    else
                    {
                        FinalNameItems.Remove(_finalName);
                        FinalNameItems.Add(value);

                        if (ProjectInfoService.IsExist(ProjectItemType, BaseObject, value))
                        {
                            OperationList.Clear();
                            OperationList.Add(OperationStatus.Overwrite);
                            OperationList.Add(OperationStatus.UseExisting);
                            RaisePropertyChanged(nameof(OperationList));
                            Operation = OperationStatus.Overwrite;

                            if (SelectedOperation == OperationStatus.Import)
                                Operation = OperationStatus.Overwrite;
                            else if (SelectedOperation == OperationStatus.Discard)
                                Operation = OperationStatus.UseExisting;

                            UpdateExistingConfig(value);
                        }
                        else
                        {
                            OperationList.Clear();
                            OperationList.Add(OperationStatus.Create);
                            OperationList.Add(OperationStatus.Discard);
                            RaisePropertyChanged(nameof(OperationList));

                            Operation = OperationStatus.Create;
                        }

                        ImportConfig["FinalName"] = value;
                        ImportConfig["Name"] = value;

                        Set(ref _finalName, value);
                    }
                }
            }
        }
        
        public bool Inhibit
        {
            get { return _inhibit; }
            set
            {
                ImportConfig["Inhibited"] = value;
                Set(ref _inhibit, value);
            }
        }

        public string ProgramType { get; set; }

        public string ParentProgram
        {
            get { return _parentProgram; }
            set
            {
                ImportConfig["Parent"] = ParentProgram;
                Set(ref _parentProgram, value);
            }
        }

        public string ScheduleIn
        {
            get { return _scheduleIn; }
            set
            {
                ImportConfig["ScheduleIn"] = value;
                Set(ref _scheduleIn, value);
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                ImportConfig["Description"] = value;
                Set(ref _description, value);
            }
        }

        public string Revision
        {
            get { return _revision; }
            set
            {
                ImportConfig["Revision"] = value;
                Set(ref _revision, value);
            }
        }

        public string RevisionNote
        {
            get { return _revisionNote; }
            set
            {
                ImportConfig["RevisionNote"] = value;
                Set(ref _revisionNote, value);
            }
        }

        public string Usage
        {
            get { return _usage; }
            set
            {
                ImportConfig["Usage"] = value;
                Set(ref _usage, value);
            }
        }

        public string AliasFor
        {
            get { return _aliasFor; }
            set
            {
                ImportConfig["AliasFor"] = value;
                Set(ref _aliasFor, value);
            }
        }

        public string DataType
        {
            get { return _dataType; }
            set
            {
                ImportConfig["DataType"] = value;
                Set(ref _dataType, value);
            }
        }

        public string ParentModule
        {
            get { return _parentModule; }
            set
            {
                ImportConfig["ParentModule"] = value;
                Set(ref _parentModule, value);
            }
        }

        private void UpdateOperation(OperationStatus operationStatus)
        {
            if (operationStatus == OperationStatus.Import)
            {
                if (ProjectInfoService.IsExist(ProjectItemType, BaseObject, FinalName))
                {
                    Operation = OperationStatus.Overwrite;
                }
                else
                {
                    Operation = OperationStatus.Create;
                }
            }
            else if (operationStatus == OperationStatus.Discard)
            {
                if (ProjectInfoService.IsExist(ProjectItemType, BaseObject, FinalName))
                {
                    Operation = OperationStatus.UseExisting;
                }
                else
                {
                    Operation = OperationStatus.Discard;
                }
            }
        }

        private void UpdateExistingConfig(string finalName)
        {
            if (ProjectItemType == ProjectItemType.Program)
            {
                ExistingConfig = _controller.Programs[finalName];
            }
            else if(ProjectItemType == ProjectItemType.UserDefined||ProjectItemType == ProjectItemType.Strings)
            {
                ExistingConfig = ProjectInfoService.GetExistObject(ProjectItemType, BaseObject, finalName);
            }
            else if (ProjectItemType == ProjectItemType.AddOnInstructions)
            {
                var baseObject = ((AoiDefinitionCollection)_controller.AOIDefinitionCollection).Find(finalName);
                ProjectItemType = ProjectItemType.AddOnDefined;
                ExistingConfig = ProjectInfoService.GetExistObject(ProjectItemType.AddOnDefined, baseObject, finalName);
            }
        }
    }

    public class ItemVisibility
    {
        public Visibility UserDefinedOrStringsVisibility { get; set; } = Visibility.Collapsed;
        public Visibility AoiVisibility { get; set; } = Visibility.Collapsed;
        public Visibility ProgramVisibility { get; set; } = Visibility.Collapsed;
    }

    public class IsChangeExistingPrograms
    {
        public bool IsChangeScheduling { get; set; }
        public bool IsEnabledChangeScheduling { get; set; } = true;
        public bool IsChangeParent { get; set; }
        public bool IsEnabledChangeParent { get; set; } = true;
    }
}
