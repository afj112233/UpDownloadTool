using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Components.Controls;
using ICSStudio.Dialogs.ConfigDialogs;
using ICSStudio.Dialogs.Filter;
using ICSStudio.Dialogs.SelectDataType;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Gui.Utils;
using ICSStudio.Gui.View;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Notification;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Notification;
using Microsoft.VisualStudio.Shell;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Dialog;
using Microsoft.VisualStudio.Shell.Interop;

namespace ICSStudio.Dialogs.NewTag
{
    public enum CreateCommandType
    {
        CreateAndClose,
        CreateAndOpenNew,
        CreateAndKeepOpen
    }

    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class NewTagViewModel : ViewModelBase
    {
        private bool? _dialogResult;
        private string _name;
        private string _description;

        private TagType _tagType;
        private string _dataType;

        // ReSharper disable once NotAccessedField.Local
        private readonly ITag _assignedGroup;
        private readonly IController _controller;
        private ITagCollectionContainer _tagCollectionContainer;

        private Usage _usage;
        private List<DisplayItem<Usage>> _usages;
        private ExternalAccess _externalAccess;
        private List<DisplayItem<ExternalAccess>> _externalAccessSource;

        private bool _displayStyleEnabled;
        private DisplayStyle _displayStyle;
        private List<DisplayItem<DisplayStyle>> _displayStyleSource;

        public NewTagViewModel(string dataType,
            ITagCollection parentCollection,
            Usage usage, ITag assignedGroup)
            : this(dataType, parentCollection, usage, assignedGroup, string.Empty, false)
        {

        }

        public NewTagViewModel(
            string dataType,
            ITagCollection parentCollection,
            Usage usage, ITag assignedGroup, string name) :
            this(dataType, parentCollection, usage, assignedGroup, name, false)
        {

        }

        public NewTagViewModel(
            string dataType,
            ITagCollection parentCollection,
            Usage usage, ITag assignedGroup,
            string name, bool isOnlyInController)
        {
            _controller = parentCollection.ParentController;
            _tagType = TagType.Base;
            _externalAccess = ExternalAccess.ReadWrite;
            _usage = usage;
            _assignedGroup = assignedGroup;
            _name = name;

            DataType = string.IsNullOrEmpty(dataType) ? "DINT" : dataType;

            var tagCollectionContainers = new List<DisplayItem<ITagCollectionContainer>>
            {
                new DisplayItem<ITagCollectionContainer> { DisplayName = _controller.Name, Value = _controller }
            };

            if (!isOnlyInController)
            {
                foreach (var program in _controller.Programs)
                {
                    tagCollectionContainers.Add(new DisplayItem<ITagCollectionContainer>()
                    {
                        DisplayName = program.Name,
                        Value = program
                    });
                }
            }

            TagCollectionContainers = tagCollectionContainers;

            if (parentCollection.IsControllerScoped)
            {
                TagCollectionContainer = _controller;
            }
            else
            {
                TagCollectionContainer = parentCollection.ParentProgram;
            }

            //暂时不支持TagType.Alias,在线或离线不可创建TagType.Produced, TagType.Consumed会导致upload失败
            TagTypes = new List<TagType>
            {
                TagType.Base
            };

            var externalAccessSource = new List<DisplayItem<ExternalAccess>>
            {
                new DisplayItem<ExternalAccess> { DisplayName = "Read/Write", Value = ExternalAccess.ReadWrite },
                new DisplayItem<ExternalAccess> { DisplayName = "Read Only", Value = ExternalAccess.ReadOnly },
                new DisplayItem<ExternalAccess> { DisplayName = "None", Value = ExternalAccess.None },
            };
            ExternalAccessSource = externalAccessSource;

            SelectDataTypeCommand = new RelayCommand(ExecuteSelectDataTypeCommand);
            CreateCommand = new RelayCommand<CreateCommandType>(ExecuteCreateCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            HelpCommand = new RelayCommand(ExecuteHelpCommand);
            NameFilterCommand = new RelayCommand<Button>(ExecuteNameFilterCommand);
            foreach (var item in _controller.DataTypes)
            {
                if (item.Name.StartsWith("BOOL:", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (item.Name.Contains("$"))
                    continue;
                AllDataTypeNames.Add(item.Name);
            }

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("Title");
        }

        public override void Cleanup()
        {
            NameFilterPopup.FilterViewModel.Cleanup();
            
            PropertyChangedEventManager.RemoveHandler(NameFilterPopup.FilterViewModel, FilterViewModel_PropertyChanged,
                "");

            NameFilterPopup = null;
        }

        private void FilterViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                RaisePropertyChanged("Connection");
            }

            if (e.PropertyName == "Hide")
            {
                NameFilterPopup.IsOpen = false;
            }
        }

        public ITag NewTag { get; private set; }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        public string Title
        {
            get
            {
                if (Usage == Usage.NullParameterType)
                    return LanguageManager.GetInstance().ConvertSpecifier("New Tag");

                return LanguageManager.GetInstance().ConvertSpecifier("New Parameter or Tag");
            }

        }

        public string ConfigurationContent
        {
            get
            {
                if (OpenConfigurationEnabled)
                {
                    string displayDataType = _dataType.ToUpper();
                    displayDataType = displayDataType.Replace("_", "__");
                    if (displayDataType.Contains('['))
                    {
                        displayDataType = displayDataType.Substring(0, displayDataType.IndexOf('['));
                    }

                    return "Open " + displayDataType + " Configuration";
                }

                return "Open Configuration";
            }
        }

        public bool OpenConfigurationEnabled
        {
            get
            {
                if (_usage == Usage.InOut)
                    return false;

                string typeName;
                int dim1, dim2, dim3;
                int errorCode;
                var isValid = _controller.DataTypes.ParseDataType(
                    _dataType, out typeName,
                    out dim1, out dim2, out dim3, out errorCode);

                if (isValid)
                {
                    if (typeName.Equals("CAM", StringComparison.OrdinalIgnoreCase)
                        || typeName.Equals("CAM_PROFILE", StringComparison.OrdinalIgnoreCase))
                    {
                        if (dim1 > 0)
                            return true;
                    }

                    if (typeName.Equals("PID", StringComparison.OrdinalIgnoreCase)
                        || typeName.Equals("MESSAGE", StringComparison.OrdinalIgnoreCase)
                        || typeName.Equals("AXIS_CONSUMED", StringComparison.OrdinalIgnoreCase)
                        || typeName.Equals("AXIS_GENERIC", StringComparison.OrdinalIgnoreCase)
                        || typeName.Equals("AXIS_GENERIC_DRIVE", StringComparison.OrdinalIgnoreCase)
                        || typeName.Equals("AXIS_CIP_DRIVE", StringComparison.OrdinalIgnoreCase)
                        || typeName.Equals("AXIS_VIRTUAL", StringComparison.OrdinalIgnoreCase)
                        || typeName.Equals("AXIS_SERVO", StringComparison.OrdinalIgnoreCase)
                        || typeName.Equals("AXIS_SERVO_DRIVE", StringComparison.OrdinalIgnoreCase)
                        || typeName.Equals("Motion_Group", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private bool _openConfiguration;
        private bool _selectedTagEnable;
        private Visibility _selectedTagVisibility;
        private Visibility _selectedTagVisibility2;
        private NameFilterPopup _nameFilterPopup;

        public bool OpenConfiguration
        {
            get
            {
                if (!OpenConfigurationEnabled)
                    _openConfiguration = false;
                return _openConfiguration;
            }
            set { _openConfiguration = value; }
        }

        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        public string Description
        {
            get { return _description; }
            set { Set(ref _description, value); }
        }

        public List<TagType> TagTypes { get; }

        public TagType TagType
        {
            get { return _tagType; }
            set { Set(ref _tagType, value); }
        }

        public List<string> AllDataTypeNames { get; } = new List<string>();

        public string DataType
        {
            get { return _dataType; }
            set
            {
                Set(ref _dataType, value);

                RaisePropertyChanged("OpenConfigurationEnabled");
                RaisePropertyChanged("OpenConfiguration");
                RaisePropertyChanged("ConfigurationContent");
                UpdateDisplayStyle();
            }
        }

        public IList TagCollectionContainers { get; }

        public ITagCollectionContainer TagCollectionContainer
        {
            get { return _tagCollectionContainer; }
            set
            {
                Set(ref _tagCollectionContainer, value);

                RaisePropertyChanged("UsageEnabled");

                UpdateUsageSource();
            }
        }

        public bool UsageEnabled => TagCollectionContainer != _controller;

        public List<DisplayItem<Usage>> Usages
        {
            get { return _usages; }
            set { Set(ref _usages, value); }
        }

        public Usage Usage
        {
            get { return _usage; }
            set
            {
                Set(ref _usage, value);

                _externalAccess = ExternalAccess.ReadWrite;
                if (_usage == Usage.Local)
                {
                    SelectedTagEnable = false;
                    SelectedTagVisibility = Visibility.Collapsed;
                    SelectedTagVisibility2 = Visibility.Visible;
                }
                else
                {
                    if (_usage == Usage.Output)
                    {
                        _externalAccess = ExternalAccess.ReadOnly;
                    }

                    SelectedTagEnable = TagCollectionContainer?.Tags.ParentProgram != null;
                    SelectedTagVisibility = SelectedTagEnable?Visibility.Visible:Visibility.Collapsed;
                    SelectedTagVisibility2 = SelectedTagEnable ? Visibility.Collapsed : Visibility.Visible;
                }

                RaisePropertyChanged("ExternalAccess");
                RaisePropertyChanged("Title");
                RaisePropertyChanged("OpenConfigurationEnabled");
                RaisePropertyChanged("OpenConfiguration");
            }
        }

        public bool SelectedTagEnable
        {
            set { Set(ref _selectedTagEnable, value); }
            get { return _selectedTagEnable; }
        }

        public Visibility SelectedTagVisibility
        {
            set { Set(ref _selectedTagVisibility, value); }
            get { return _selectedTagVisibility; }
        }

        public Visibility SelectedTagVisibility2
        {
            set { Set(ref _selectedTagVisibility2, value); }
            get { return _selectedTagVisibility2; }
        }

        public string Connection
        {
            set { NameFilterPopup.FilterViewModel.Name = value; }
            get { return NameFilterPopup.FilterViewModel.Name; }
        }

        public ExternalAccess ExternalAccess
        {
            get { return _externalAccess; }
            set { Set(ref _externalAccess, value); }
        }

        public List<DisplayItem<ExternalAccess>> ExternalAccessSource
        {
            get { return _externalAccessSource; }
            set { Set(ref _externalAccessSource, value); }
        }

        public bool DisplayStyleEnabled
        {
            get { return _displayStyleEnabled; }
            set { Set(ref _displayStyleEnabled, value); }
        }

        public DisplayStyle DisplayStyle
        {
            get { return _displayStyle; }
            set { Set(ref _displayStyle, value); }
        }

        public List<DisplayItem<DisplayStyle>> DisplayStyleSource
        {
            get { return _displayStyleSource; }
            set { Set(ref _displayStyleSource, value); }
        }

        public RelayCommand SelectDataTypeCommand { get; }
        public RelayCommand<CreateCommandType> CreateCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand HelpCommand { get; }

        public RelayCommand<Button> NameFilterCommand { set; get; }

        public NameFilterPopup NameFilterPopup
        {
            internal set
            {
                if (value != null)
                {
                    _nameFilterPopup = value;

                    PropertyChangedEventManager.AddHandler(_nameFilterPopup.FilterViewModel,
                        FilterViewModel_PropertyChanged, "");
                }
            }
            get { return _nameFilterPopup; }
        }

        public Dictionary<ITag, TagNameNode> NameList => NameFilterPopup.FilterViewModel.AutoCompleteData;

        private void ExecuteNameFilterCommand(Button sender)
        {
            var parentGrid = VisualTreeHelpers.FindVisualParentOfType<Grid>(sender);
            var autoCompleteBox = VisualTreeHelpers.FindFirstVisualChildOfType<AutoCompleteBox>(parentGrid);

            if (!NameFilterPopup.IsOpen)
                NameFilterPopup.ResetPosition(autoCompleteBox, PlacementMode.Top);
            NameFilterPopup.IsOpen = !NameFilterPopup.IsOpen;
        }

        private void ExecuteSelectDataTypeCommand()
        {
            var selectDataTypeDialog = new SelectDataTypeDialog(
                _controller, DataType, true, true)
            {
                Owner = Application.Current.MainWindow
            };

            var dialogResult = selectDataTypeDialog.ShowDialog();
            if (dialogResult != null && dialogResult.Value)
            {
                DataType = selectDataTypeDialog.DataType;
            }
        }

        private void ExecuteCreateCommand(CreateCommandType createCommandType)
        {
            //0. check keyswitch
            if (_controller.KeySwitchPosition == ControllerKeySwitch.RunKeySwitch)
            {
                var result = "Failed to create a new tag.";
                var reason = "CIP Error: Keyswitch position invalid for this operation.";
                var message = LanguageManager.GetInstance().ConvertSpecifier(result)+"\n"+LanguageManager.GetInstance().ConvertSpecifier(reason);
                MessageBox.Show(message, "ICSStudio", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
                return;
            }

            //1. check input
            if (IsValidTagName(Name)
                && IsValidDataType(DataType)
                && IsValidTagType(TagType))
            {
                NameFilterPopup.FilterViewModel.Cleanup();
                PropertyChangedEventManager.RemoveHandler(NameFilterPopup.FilterViewModel,
                    FilterViewModel_PropertyChanged, "");
                //2. create and add
                if (CreateAndAdd() == 0)
                {
                    SaveConnection(NewTag);
                    //3. update ui
                    //IStudioUIService studioUIService =
                    //    Package.GetGlobalService(typeof(SStudioUIService)) as IStudioUIService;
                    //studioUIService?.UpdateUI();

                    //4. close current dialog
                    if (createCommandType == CreateCommandType.CreateAndClose
                        || createCommandType == CreateCommandType.CreateAndOpenNew)
                        DialogResult = true;

                    //5. open config dialog
                    OpenConfigDialog();

                    //6. open new tag dialog
                    if (createCommandType == CreateCommandType.CreateAndOpenNew)
                    {
                        var dialog = new NewTagDialog(
                            new NewTagViewModel(
                                "DINT",
                                TagCollectionContainer.Tags,
                                Usage.NullParameterType, null))
                        {
                            Owner = Application.Current.MainWindow
                        };

                        dialog.ShowDialog();
                    }
                }
            }
        }

        private void ExecuteCancelCommand()
        {
            DialogResult = false;
        }

        private void ExecuteHelpCommand()
        {
            //TODO(gjc): add code here
        }

        private void UpdateDisplayStyle()
        {
            DisplayStyle oldDisplayStyle = DisplayStyle;
            DisplayStyle defaultDisplayStyle = DisplayStyle.NullStyle;

            string typeName;
            int dim1, dim2, dim3;
            int errorCode;

            var isValid = _controller.DataTypes.ParseDataType(
                DataType, out typeName, out dim1,
                out dim2,
                out dim3, out errorCode);

            DisplayStyleEnabled = false;
            if (isValid)
            {
                var dataType = _controller.DataTypes[typeName];
                if (dataType is BOOL)
                {
                    DisplayStyleEnabled = true;
                    defaultDisplayStyle = DisplayStyle.Decimal;
                    DisplayStyleSource = new List<DisplayItem<DisplayStyle>>
                    {
                        new DisplayItem<DisplayStyle>
                        {
                            DisplayName = "Binary", Value = DisplayStyle.Binary
                        },
                        new DisplayItem<DisplayStyle>
                        {
                            DisplayName = "Octal", Value = DisplayStyle.Octal
                        },
                        new DisplayItem<DisplayStyle>
                        {
                            DisplayName = "Decimal", Value = DisplayStyle.Decimal
                        },
                        new DisplayItem<DisplayStyle>
                        {
                            DisplayName = "Hex", Value = DisplayStyle.Hex
                        }
                    };
                }
                else if (dataType.IsInteger)
                {
                    DisplayStyleEnabled = true;
                    defaultDisplayStyle = DisplayStyle.Decimal;
                    DisplayStyleSource = new List<DisplayItem<DisplayStyle>>
                    {
                        new DisplayItem<DisplayStyle>
                        {
                            DisplayName = "Binary", Value = DisplayStyle.Binary
                        },
                        new DisplayItem<DisplayStyle>
                        {
                            DisplayName = "Octal", Value = DisplayStyle.Octal
                        },
                        new DisplayItem<DisplayStyle>
                        {
                            DisplayName = "Decimal", Value = DisplayStyle.Decimal
                        },
                        new DisplayItem<DisplayStyle>
                        {
                            DisplayName = "Hex", Value = DisplayStyle.Hex
                        },
                        new DisplayItem<DisplayStyle>
                        {
                            DisplayName = "Ascii", Value = DisplayStyle.Ascii
                        }
                    };

                    if (dataType is LINT)
                    {
                        DisplayStyleSource.Add(new DisplayItem<DisplayStyle>()
                            { DisplayName = "Date/Time", Value = DisplayStyle.DateTime });
                        DisplayStyleSource.Add(new DisplayItem<DisplayStyle>()
                            { DisplayName = "Date/Time (ns)", Value = DisplayStyle.DateTimeNS });
                    }
                }
                else if (dataType is REAL)
                {
                    DisplayStyleEnabled = true;
                    defaultDisplayStyle = DisplayStyle.Float;
                    DisplayStyleSource = new List<DisplayItem<DisplayStyle>>
                    {
                        new DisplayItem<DisplayStyle>
                        {
                            DisplayName = "Float", Value = DisplayStyle.Float
                        },
                        new DisplayItem<DisplayStyle>
                        {
                            DisplayName = "Exponential", Value = DisplayStyle.Exponential
                        },
                    };
                }
            }

            if (DisplayStyleEnabled)
            {
                bool beContained = false;
                foreach (var item in DisplayStyleSource)
                {
                    if (item.Value == oldDisplayStyle)
                    {
                        beContained = true;
                        break;
                    }
                }

                DisplayStyle = beContained ? oldDisplayStyle : defaultDisplayStyle;
            }
            else
            {
                DisplayStyleSource = new List<DisplayItem<DisplayStyle>>()
                {
                    new DisplayItem<DisplayStyle>()
                    {
                        DisplayName = "",
                        Value = DisplayStyle.NullStyle
                    }
                };

                DisplayStyle = DisplayStyle.NullStyle;
            }

            RaisePropertyChanged("DisplayStyle");
        }

        private void UpdateUsageSource()
        {
            var oldUsage = Usage;

            var usageList = new List<DisplayItem<Usage>>();
            if (UsageEnabled)
            {
                usageList.Add(new DisplayItem<Usage>
                {
                    DisplayName = "Local Tag",
                    Value = Usage.Local
                });
                usageList.Add(new DisplayItem<Usage>
                {
                    DisplayName = "Input Parameter",
                    Value = Usage.Input
                });
                usageList.Add(new DisplayItem<Usage>
                {
                    DisplayName = "Output Parameter",
                    Value = Usage.Output
                });
                usageList.Add(new DisplayItem<Usage>
                {
                    DisplayName = "InOut Parameter",
                    Value = Usage.InOut
                });
                usageList.Add(new DisplayItem<Usage>
                {
                    DisplayName = "Public Parameter",
                    Value = Usage.SharedData
                });
            }
            else
            {
                usageList.Add(new DisplayItem<Usage>
                {
                    DisplayName = "<controller>",
                    Value = Usage.NullParameterType
                });
            }

            Usages = usageList;

            bool beContain = false;

            foreach (var item in usageList)
            {
                if (item.Value == oldUsage)
                {
                    beContain = true;
                    break;
                }
            }

            Usage = beContain ? oldUsage : usageList[0].Value;

            RaisePropertyChanged("Usage");
        }

        private bool IsValidTagName(string name)
        {
            string warningMessage = LanguageManager.GetInstance().ConvertSpecifier("Failed to create a new tag.");
            string warningReason = string.Empty;
            bool isValid = true;

            if (string.IsNullOrEmpty(name))
            {
                isValid = false;
                warningReason = LanguageManager.GetInstance().ConvertSpecifier("Tag name is empty.");
            }

            if (isValid)
            {
                if (name.Length > 40 || name.EndsWith("_") ||
                    name.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    isValid = false;
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("VariableNameInvalid");
                }
            }

            if (isValid)
            {
                Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
                if (!regex.IsMatch(name))
                {
                    isValid = false;
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("VariableNameInvalid");
                }
            }

            // key word
            if (isValid)
            {
                string[] keyWords =
                {
                    "goto",
                    "repeat", "until", "or", "end_repeat",
                    "return", "exit",
                    "if", "then", "elsif", "else", "end_if",
                    "case", "of", "end_case",
                    "for", "to", "by", "do", "end_for",
                    "while", "end_while",
                    "not", "mod", "and", "xor", "or",
                    "ABS","SQRT",
                    "LOG","LN",
                    "DEG","RAD","TRN",
                    "ACS","ASN","ATN","COS","SIN","TAN"
                };
                foreach (var keyWord in keyWords)
                {
                    if (keyWord.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        isValid = false;
                        warningReason = LanguageManager.GetInstance().ConvertSpecifier("VariableNameInvalid");
                    }
                }
            }

            if (isValid)
            {
                var tag = TagCollectionContainer.Tags[name];
                if (tag != null)
                {
                    isValid = false;
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("Already exists.");
                }
            }


            //
            if (!isValid)
            {
                var warningDialog = new WarningDialog(warningMessage, warningReason)
                    { Owner = Application.Current.MainWindow };
                warningDialog.ShowDialog();
            }

            return isValid;
        }

        private bool IsValidDataType(string dataType)
        {
            string warningMessage = "Failed to create a new tag.";
            string warningReason = "Data type could not be found.";

            string typeName;
            int dim1, dim2, dim3;
            int errorCode;

            var isValid = _controller.DataTypes.ParseDataType(
                dataType, out typeName,
                out dim1, out dim2, out dim3, out errorCode);

            if (errorCode == -5)
                warningReason = "Number,size,or format of dimensions specified for this tag or type is invalid.";

            if (isValid)
            {
                var foundDataType = _controller.DataTypes[typeName];
                if (dim1 > 0 && !foundDataType.SupportsOneDimensionalArray)
                {
                    isValid = false;
                    warningReason = "Cannot create arrays of this data type.";
                }
                else if (dim2 > 0 && !foundDataType.SupportsMultiDimensionalArrays)
                {
                    isValid = false;
                    warningReason = "Cannot create multi dimensional arrays of this data type.";
                }

                if (isValid)
                {
                    // check scope
                    // foundDataType.IsGlobalScopeOnly
                    //软件层面暂时禁止在Program下创建MSG，因为Program下的MSG类型Tag不能运行
                    if ((foundDataType.IsAxisType
                         || foundDataType.IsMotionGroupType
                         || foundDataType.IsCoordinateSystemType
                         || foundDataType.IsMessageType)
                        && TagCollectionContainer != _controller)
                    {
                        isValid = false;
                        warningReason = "Tag can only be created at the controller scope.";
                    }
                }

                //TODO(clx): 暂时不支持ULINT/USINT/UINT/UDINT/LREAL/LINT
                if (isValid)
                {
                    var dataTypeName = foundDataType.Name;
                    string[] unsupportedTypes =
                    {
                        LREAL.Inst.Name,
                        ULINT.Inst.Name,
                        UDINT.Inst.Name,
                        UINT.Inst.Name,
                        USINT.Inst.Name,
                        LINT.Inst.Name
                    };
                    if (unsupportedTypes.Contains(dataTypeName, StringComparer.OrdinalIgnoreCase))
                    {
                        isValid = false;
                        warningReason = $"The {dataTypeName} data type is not supported by this controller type.";
                    }
                }
                
                if (isValid)
                {
                    // check size, 2MBytes
                    int totalDimension = Math.Max(dim1, 1) * Math.Max(dim2, 1) * Math.Max(dim3, 1);
                    int maxDimension = 2 * 1024 * 1024 / foundDataType.ByteSize;
                    if (totalDimension > maxDimension)
                    {
                        isValid = false;
                        warningReason = "The array size exceeds 2MBytes.";
                    }
                }

                if (isValid)
                {
                    // max number
                    // motion group(1)
                    // coordinate system(32)
                    if (foundDataType.IsMotionGroupType)
                    {
                        if (TagCollectionContainer.Tags.Count(tag =>
                                tag.DataTypeInfo.DataType.IsMotionGroupType) >= 1)
                        {
                            isValid = false;
                            warningReason = "The maximum number of motion group tags (1) has been reached.";
                        }
                    }

                    if (foundDataType.IsCoordinateSystemType)
                    {
                        if (TagCollectionContainer.Tags.Count(tag =>
                                tag.DataTypeInfo.DataType.IsCoordinateSystemType) >= 32)
                        {
                            isValid = false;
                            warningReason = "The maximum number of coordinate system tags (32) has been reached.";
                        }
                    }
                }
            }

            //
            if (!isValid)
            {
                var warningDialog = new WarningDialog(
                        warningMessage, warningReason)
                    { Owner = Application.Current.MainWindow };
                warningDialog.ShowDialog();
            }

            return isValid;
        }

        private bool IsValidTagType(TagType tagType)
        {
            string warningMessage = "Failed to create a new tag.";
            string warningReason = "Data type could not be found.";

            bool isValid = true;

            if (TagCollectionContainer != _controller &&
                (tagType == TagType.Produced || tagType == TagType.Consumed))
            {
                isValid = false;
                warningReason = "Tag can only be created at the controller scope.";
            }

            if (isValid)
            {
                if (tagType == TagType.Alias || tagType == TagType.Produced || tagType == TagType.Consumed)
                {
                    isValid = false;
                    warningReason = $"Controller cannot support {tagType} type.";
                }
            }

            if (!isValid)
            {
                var warningDialog = new WarningDialog(
                        warningMessage, warningReason)
                    { Owner = Application.Current.MainWindow };
                warningDialog.ShowDialog();
            }

            return isValid;
        }

        private int CreateAndAdd()
        {
            try
            {
                var tagCollection = TagCollectionContainer.Tags as TagCollection;
                if (tagCollection != null)
                {
                    string typeName;
                    int dim1, dim2, dim3;
                    int errorCode;

                    _controller.DataTypes.ParseDataType(
                        DataType,
                        out typeName,
                        out dim1, out dim2, out dim3,
                        out errorCode);

                    // For BOOL
                    if (string.Equals(typeName, "BOOL", StringComparison.OrdinalIgnoreCase)
                        && dim1 > 0 && dim2 == 0 && dim3 == 0)
                    {
                        if (dim1 % 32 != 0)
                        {
                            dim1 = (dim1 / 32 + 1) * 32;
                        }
                    }

                    Tag tag = TagsFactory.CreateTag(tagCollection, Name, typeName, dim1, dim2, dim3);
                    tag.Description = Description;

                    tag.Usage = Usage;
                    tag.TagType = TagType;
                    tag.ExternalAccess = ExternalAccess;
                    tag.DisplayStyle = DisplayStyle;

                    if (_assignedGroup != null)
                    {
                        var axisCIPDrive = tag.DataWrapper as AxisCIPDrive;
                        if (axisCIPDrive != null)
                        {
                            axisCIPDrive.AssignedGroup = _assignedGroup;
                        }

                        var axisVirtual = tag.DataWrapper as AxisVirtual;
                        if (axisVirtual != null)
                        {
                            axisVirtual.AssignedGroup = _assignedGroup;
                        }

                        //TODO(gjc):add code here
                    }

                    tagCollection.AddTag(tag, false, false);
                    Notifications.Publish(new MessageData() { Object = tag, Type = MessageData.MessageType.AddTag });
                    NewTag = tag;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                string warningMessage = "Failed to create a new tag.";
                var warningReason = e.Message;
                var warningDialog = new WarningDialog(warningMessage, warningReason)
                    { Owner = Application.Current.MainWindow };
                warningDialog.ShowDialog();

                return -1;
            }

            return 0;
        }

        private void SaveConnection(ITag tag)
        {
            if (Usage == Usage.Local ||
                string.IsNullOrEmpty(Connection) || TagCollectionContainer.Tags.ParentProgram == null) return;
            _controller.ParameterConnections.CreateConnection(tag, Connection);
        }

        private void OpenConfigDialog()
        {
            if (OpenConfiguration)
            {
                string typeName;
                int dim1, dim2, dim3;
                int errorCode;
                _controller.DataTypes.ParseDataType(
                    _dataType, out typeName,
                    out dim1, out dim2, out dim3, out errorCode);

                if (typeName.Equals("PID", StringComparison.OrdinalIgnoreCase))
                {
                    ArrayField field = (NewTag as Tag)?.DataWrapper.Data as ArrayField;
                    PidSetupDialog pidSetupDialog = new PidSetupDialog(new PidSetUpViewModel(field, NewTag))
                    {
                        Owner = Application.Current.MainWindow
                    };
                    pidSetupDialog.ShowDialog();
                }

                if (typeName.Equals("CAM", StringComparison.OrdinalIgnoreCase)
                    || typeName.Equals("CAM_PROFILE", StringComparison.OrdinalIgnoreCase))
                {
                    CamEditorDialog dialog = new CamEditorDialog(new CamEditorViewModel(NewTag))
                    {
                        Owner = Application.Current.MainWindow
                    };
                    dialog.ShowDialog();
                }

                if (typeName.Equals("AXIS_CIP_DRIVE", StringComparison.OrdinalIgnoreCase))
                {
                    ICreateDialogService createDialogService =
                        Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;
                    var uiShell = (IVsUIShell)Package.GetGlobalService(typeof(SVsUIShell));

                    var window =
                        createDialogService?.CreateAxisCIPDriveProperties(NewTag);
                    window?.Show(uiShell);
                }

                if (typeName.Equals("Motion_Group", StringComparison.OrdinalIgnoreCase))
                {
                    var createDialogService =
                        (ICreateDialogService)Package.GetGlobalService(typeof(SCreateDialogService));

                    var uiShell = (IVsUIShell)Package.GetGlobalService(typeof(SVsUIShell));

                    var window =
                        createDialogService?.CreateMotionGroupProperties(NewTag);
                    window?.Show(uiShell);
                }

                if (typeName.Equals("MESSAGE", StringComparison.OrdinalIgnoreCase))
                {
                    var title= $"Message {LanguageManager.GetInstance().ConvertSpecifier("Configuration")} - {NewTag.Name}";
                    MessageConfigurationDialog dialog = new MessageConfigurationDialog(NewTag,title)
                    {
                        Owner = Application.Current.MainWindow
                    };

                    dialog.ShowDialog();
                }
            }
        }
    }
}
