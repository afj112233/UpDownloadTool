using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Windows;
using GalaSoft.MvvmLight;
using ICSStudio.Descriptor;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.MultiLanguage;
using ICSStudio.UIInterfaces.Project;
using DeviceModule = ICSStudio.SimpleServices.DeviceModule.DeviceModule;

namespace ICSStudio.OrganizerPackage.ViewModel
{
    public class OrganizerItem : ObservableObject, IProjectItem
    {
        private bool _isSelected;
        private bool _isExpanded;
        private string _displayName;
        private string _toolTip;

        private ProjectItemType _kind;
        private string _iconKind;
        private string _iconForeground;

        private IBaseObject _associatedObject;

        private bool _isInhibited;
        private bool _isWarning;
        private bool _isMainRoutine;
        private bool _isFaultRoutine;
        private string _imageSource;

        public OrganizerItem()
        {
            _isExpanded = true;
            _iconForeground = "Black";
            ProjectItems = new OrganizerItems(this);
            UpdateIconKind();

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            DisplayNameConvert();
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { Set(ref _isSelected, value); }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                Set(ref _isExpanded, value);
                UpdateIconKind();
            }
        }

        [ReadOnly(true)]
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                Set(ref _displayName, value);
                ToolTip = _displayName;
            }
        }

        public string IconKind
        {
            get { return _iconKind; }
            set { Set(ref _iconKind, value); }
        }

        public string IconForeground
        {
            get { return _iconForeground; }
            set { Set(ref _iconForeground, value); }
        }

        [ReadOnly(true)]
        public IBaseObject AssociatedObject
        {
            get { return _associatedObject; }
            set
            {
                _associatedObject = value;
                UpdateIconKind();
            }
        }
        
        public virtual void Cleanup()
        {
            foreach (IProjectItem projectItem in ProjectItems)
            {
                projectItem.Cleanup();
            }

            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                DisplayNameConvert();
            }
        }

        public IProjectItems Collection { get; set; }
        public IProjectItems ProjectItems { get; set; }

        public string ToolTip
        {
            get { return _toolTip; }
            set { Set(ref _toolTip, value); }
        }

        public string ImageSource
        {
            get { return _imageSource; }
            set { Set(ref _imageSource, value); }
        }

        [ReadOnly(true)]
        public ProjectItemType Kind
        {
            get { return _kind; }
            set
            {
                Set(ref _kind, value);
                UpdateIconKind();
                NodeType = value.ToString();
            }
        }

        private string _nodeType = string.Empty;

        public string NodeType
        {
            get { return _nodeType; }
            set { Set(ref _nodeType, value); }
        }

        [ReadOnly(true)]
        public bool Inhibited
        {
            get { return _isInhibited; }
            set { Set(ref _isInhibited, value); }
        }

        [ReadOnly(true)]
        public bool IsWarning
        {
            get { return _isWarning; }
            set { Set(ref _isWarning, value); }
        }

        [ReadOnly(true)]
        public bool IsMainRoutine
        {
            get { return _isMainRoutine; }
            set { Set(ref _isMainRoutine, value); }
        }

        [ReadOnly(true)]
        public bool IsFaultRoutine
        {
            get { return _isFaultRoutine; }
            set { Set(ref _isFaultRoutine, value); }
        }

        protected void UpdateIconKind()
        {
            string folder = @"..\Resources\Images\";

            //TODO(gjc): edit here
            if (Kind == ProjectItemType.MotionGroups
                || Kind == ProjectItemType.ControllerModel
                || Kind == ProjectItemType.FaultHandler
                || Kind == ProjectItemType.PowerHandler
                || Kind == ProjectItemType.Tasks
                || Kind == ProjectItemType.UngroupedAxes
                || Kind == ProjectItemType.UnscheduledPrograms
                || Kind == ProjectItemType.Assets
                || Kind == ProjectItemType.EmbeddedIO
                || Kind == ProjectItemType.ExpansionIO
                || Kind == ProjectItemType.AddOnInstructions
                || Kind == ProjectItemType.DataTypes
                || Kind == ProjectItemType.Trends
            )
            {
                _imageSource = IsExpanded ? folder + @"Folder_Open.png" : folder + @"Folder_Close.png";
            }
            else if (Kind == ProjectItemType.MotionGroup)
            {
                _imageSource = folder + @"MG.png";
            }

            else if (Kind == ProjectItemType.AxisCIPDrive)
            {
                _imageSource = folder + @"CIP_Aixs.png";
            }
            else if (Kind == ProjectItemType.AxisVirtual)
            {
                _imageSource = folder + @"Virtual_Aixs.png";
            }
            else if (Kind == ProjectItemType.ControllerTags || Kind == ProjectItemType.ProgramTags)
            {
                _imageSource = folder + @"Tag.png";
            }
            else if (Kind == ProjectItemType.Program)
            {
                _imageSource = folder + @"Program.png";
            }
            else if (Kind == ProjectItemType.Ethernet)
            {
                //TODO(TLM):need new image
                _imageSource = folder + @"Ethernet.png";
            }
            else if (Kind == ProjectItemType.Task)
            {
                ITask task = AssociatedObject as ITask;
                if (task != null)
                {
                    switch (task.Type)
                    {
                        case TaskType.Event:
                            _imageSource = folder + @"Task_Event.png";
                            break;
                        case TaskType.Periodic:
                            _imageSource = folder + @"Task_Periodic.png";
                            break;
                        case TaskType.Continuous:
                            _imageSource = folder + @"Task_Continuous.png";
                            break;
                        case TaskType.NullType:
                            _imageSource = folder + @"Star.png";
                            break;
                        default:
                            _imageSource = folder + @"Star.png";
                            break;
                    }
                }
            }
            else if (Kind == ProjectItemType.Routine)
            {
                IRoutine routine = AssociatedObject as IRoutine;
                if (routine != null)
                {
                    switch (routine.Type)
                    {
                        case RoutineType.Typeless:
                            break;
                        case RoutineType.RLL:
                            _imageSource = folder + @"Routine_LD.png";
                            break;
                        case RoutineType.FBD:
                            _imageSource = folder + @"Routine_FBD.png";
                            break;
                        case RoutineType.SFC:
                            _imageSource = folder + @"Routine_SFC.png";
                            break;
                        case RoutineType.ST:
                            _imageSource = folder + @"Routine_ST.png";
                            break;
                        case RoutineType.External:
                            break;
                        case RoutineType.Sequence:
                            break;
                        case RoutineType.Encrypted:
                            break;
                        default:
                            _imageSource = folder + @"Star.png";
                            break;
                    }
                }

            }
            else if (Kind == ProjectItemType.AddOnInstruction)
            {
                _imageSource = folder + @"AOI.png";
            }
            else if (
                Kind == ProjectItemType.UserDefineds ||
                Kind == ProjectItemType.AddOnDefineds ||
                Kind == ProjectItemType.Strings ||
                Kind == ProjectItemType.Predefineds ||
                Kind == ProjectItemType.ModuleDefineds)
            {
                _imageSource = folder + @"Folder_UDT.png";
            }
            else if (Kind == ProjectItemType.UserDefined ||
                     Kind == ProjectItemType.AddOnDefined ||
                     Kind == ProjectItemType.String ||
                     Kind == ProjectItemType.Predefined ||
                     Kind == ProjectItemType.ModuleDefined)
            {
                _imageSource = folder + @"DataType.png";
            }
            else if (Kind == ProjectItemType.Trend)
            {
                _imageSource = folder + @"Trend.png";
            }
            //ICC控制器
            else if (Kind == ProjectItemType.LocalModule)
            {
                _imageSource = folder + @"ICC_Controller.png";
            }

            //ICD-BXX IO底板
            else if (Kind == ProjectItemType.Bus)
            {
                _imageSource = folder + @"ICD_B.png";
            }

            else if (Kind == ProjectItemType.IOConfiguration)
            {
                DeviceModule deviceModule = AssociatedObject as DeviceModule;

                ModuleDescriptor descriptor = new ModuleDescriptor(deviceModule);

                if (IsExpanded)
                {
                    _imageSource = folder + @"Folder_Open.png";
                }
                else
                {
                    _imageSource = folder + @"Folder_Close.png";
                }
            }

            else if (Kind == ProjectItemType.DeviceModule)
            {
                DeviceModule deviceModule = AssociatedObject as DeviceModule;

                if (deviceModule != null)
                {
                    //TODO(TLM):need add Warning triangle in WPF
                    switch (deviceModule.Type)
                    {
                        //ICD-IXX IO模块
                        case DeviceType.ChassisIO:
                            _imageSource = folder + @"ICD_I.png";
                            break;
                        //ICD-AENTR/A
                        case DeviceType.Adapter:
                            _imageSource = folder + @"ICD_I.png";
                            break;
                        //ICM驱动器和电机
                        case DeviceType.CIPMotionDrive:
                            _imageSource = folder + @"ICM_Servo.png";
                            break;
                        case DeviceType.CIPMotionDevice:
                            _imageSource = folder + @"ICM_Servo.png";
                            break;
                        case DeviceType.BlockIO:
                            _imageSource = folder + @"Eip.png";
                            break;
                        default:
                            _imageSource = folder + @"Star.png";
                            break;
                    }
                }
            }
            else
            {
                _imageSource = folder + @"Star.png";
            }

            RaisePropertyChanged("ImageSource");
        }

        protected void SortByName()
        {
            var parentItems = Collection as OrganizerItems;
            Contract.Assert(parentItems != null);

            int oldIndex = parentItems.IndexOf(this);

            int newIndex = 0;
            foreach (var item in parentItems)
            {
                int result = string.Compare(
                    item.Name,
                    Name,
                    StringComparison.OrdinalIgnoreCase);

                if (result == 0)
                {
                    newIndex--;
                }

                if (result > 0)
                    break;

                newIndex++;
            }

            if (oldIndex != newIndex)
                parentItems.Move(oldIndex, newIndex);
        }

        protected virtual void DisplayNameConvert()
        {
            DisplayName = LanguageManager.Instance.ConvertSpecifier(Name);
        }
    }
}
