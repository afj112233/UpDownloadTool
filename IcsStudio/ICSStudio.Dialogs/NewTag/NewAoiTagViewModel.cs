using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Dialogs.ConfigDialogs;
using ICSStudio.Dialogs.SelectDataType;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Notification;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Dialog;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Application = System.Windows.Application;
using System.Windows;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DataType;

namespace ICSStudio.Dialogs.NewTag
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class NewAoiTagViewModel : ViewModelBase
    {
        private string _name;
        private string _dataType;
        private string _initialDataType;
        private string _description;
        private string _aoiCollectionContainer;
        private bool _canRefreshDataType;
        private bool? _dialogResult;
        private bool _isVisibleChecked;
        private bool _isRequiredChecked;
        private bool _isConstantChecked;
        private bool _isDisplayStyleEnabled;
        private readonly IController _controller;
        private readonly IAoiDefinition _aoiDefinition;
        private Usage _usage;
        private TagType _tagType;
        private DisplayStyle _displayStyle;
        private ExternalAccess _externalAccess;
        private List<DisplayItem<Usage>> _usages;
        private List<DisplayItem<DisplayStyle>> _displayStyleSource;
        private List<DisplayItem<ExternalAccess>> _externalAccessSource;

        public NewAoiTagViewModel(string dataType,
            IAoiDefinition aoiDefinition,
            Usage usage,bool canRefreshDataType, string name = "")
        {
            _canRefreshDataType = canRefreshDataType;
            _name = name;
            _usage = usage;
            _tagType = TagType.Base;
            _aoiDefinition = aoiDefinition;
            _controller = aoiDefinition.ParentController;
            _aoiCollectionContainer = _aoiDefinition.Name;
            _isConstantChecked = false;

            OKCommand = new RelayCommand(ExecuteOKCommand);
            HelpCommand = new RelayCommand(ExecuteHelpCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            SelectDataTypeCommand = new RelayCommand(ExecuteSelectDataTypeCommand, CanExecuteSelectDataTypeCommand);

            DataType = string.IsNullOrEmpty(dataType) ? "DINT" : dataType;
            IsDataTypeEnabled = true;

            _initialDataType = canRefreshDataType ? "DINT" : DataType;

            //暂时不支持TagType.Alias
            TagTypes = new List<TagType>
            {
                TagType.Base
            };
            var externalAccessSource = new List<DisplayItem<ExternalAccess>>
            {
                new DisplayItem<ExternalAccess> {DisplayName = "Read/Write", Value = ExternalAccess.ReadWrite},
                new DisplayItem<ExternalAccess> {DisplayName = "Read Only", Value = ExternalAccess.ReadOnly},
                new DisplayItem<ExternalAccess> {DisplayName = "None", Value = ExternalAccess.None},
                new DisplayItem<ExternalAccess> {DisplayName = "", Value = ExternalAccess.NullExternalAccess},
            };
            ExternalAccessSource = externalAccessSource;

            foreach (var item in _controller.DataTypes)
            {
                if (item.Name.StartsWith("BOOL:", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (item.Name.Contains("$"))
                    continue;
                AllDataTypeNames.Add(item.Name);
            }

            UpdateUsageSource();

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("ConfigurationContent");
            RaisePropertyChanged("Title");
        }

        public string Title => LanguageManager.GetInstance().ConvertSpecifier("New Add-On Instruction Parameter or Local Tag");

        public List<string> AllDataTypeNames { get; } = new List<string>();

        public ITag NewTag { get; set; }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        public bool IsDisplayStyleEnabled
        {
            get { return _isDisplayStyleEnabled; }
            set { Set(ref _isDisplayStyleEnabled, value); }
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

        public List<DisplayItem<Usage>> Usages
        {
            get { return _usages; }
            set { Set(ref _usages, value); }
        }

        public ExternalAccess ExternalAccess
        {
            get { return _externalAccess; }
            set { Set(ref _externalAccess, value); }
        }

        public bool IsConstantChecked
        {
            get { return _isConstantChecked; }
            set { Set(ref _isConstantChecked, value); }
        }

        public string AoiCollectionContainer
        {
            get { return _aoiCollectionContainer; }
            set { Set(ref _aoiCollectionContainer, value); }
        }

        public bool IsVisibleChecked
        {
            get { return _isVisibleChecked; }
            set { Set(ref _isVisibleChecked, value); }
        }

        public bool IsOpenConfigurationChecked { get; set; }

        public bool IsRequiredChecked
        {
            get { return _isRequiredChecked; }
            set
            {
                Set(ref _isRequiredChecked, value);
                if (_isRequiredChecked != value)
                {
                    _isRequiredChecked = value;
                    if (value)
                    {
                        IsVisibleChecked = true;
                        RaisePropertyChanged("IsVisibleEnabled");
                    }
                }
            }
        }

        public TagType TagType
        {
            get { return _tagType; }
            set
            {
                if (_tagType != value)
                {
                    _tagType = value;
                    switch (value)
                    {
                        case TagType.Alias:
                            if (_usage == Usage.Input || _usage == Usage.Output)
                            {
                                IsDisplayStyleEnabled = false;
                                DataType = "";
                                IsDataTypeEnabled = false;
                            }

                            break;
                        case TagType.Base:
                            IsDisplayStyleEnabled = true;
                            break;
                        default:
                            IsDisplayStyleEnabled = false;
                            break;
                    }
                }

                RaisePropertyChanged("IsDataTypeEnabled");
                RaisePropertyChanged("IsAliasForEnabled");
                RaisePropertyChanged("IsExternalAccessEnabled");
                SelectDataTypeCommand.RaiseCanExecuteChanged();
            }
        }

        public string DataType
        {
            get { return _dataType; }
            set
            {
                Set(ref _dataType, value);

                var dataTypeName = _dataType;
                if (_dataType.Contains("["))
                {
                    Match match = Regex.Match(_dataType, @"^[a-z0-9_:]+",
                        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
                    dataTypeName = match.Value;
                }

                var dataTypeInfo = _controller.DataTypes.ParseDataTypeInfo(dataTypeName);
                IsDisplayStyleEnabled = dataTypeInfo.DataType != null && dataTypeInfo.DataType.IsAtomic;

                RaisePropertyChanged("IsOpenConfigurationEnabled");
                RaisePropertyChanged("ConfigurationContent");
                UpdateDisplayStyle();
            }
        }

        public bool IsOpenConfigurationEnabled
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
                    if (typeName.Equals("PID", StringComparison.OrdinalIgnoreCase))
                        return true;
                }

                return false;
            }
        }

        public string ConfigurationContent
        {
            get
            {
                if (IsOpenConfigurationEnabled)
                {
                    string displayDataType = _dataType.ToUpper();
                    displayDataType = displayDataType.Replace("_", "__");
                    if (displayDataType.Contains('['))
                    {
                        displayDataType = displayDataType.Substring(0, displayDataType.IndexOf('['));
                    }

                    return LanguageManager.GetInstance().ConvertSpecifier("Open") + " " + displayDataType + " " + LanguageManager.GetInstance().ConvertSpecifier("Aoi Tag Configuration");
                }

                return LanguageManager.GetInstance().ConvertSpecifier("Open Configuration");
            }
        }

        public bool IsExternalAccessEnabled =>
            _usage == Usage.Input && _tagType != TagType.Alias
            || _usage == Usage.Output && _tagType != TagType.Alias;

        public bool IsAliasForEnabled =>
            _tagType == TagType.Alias;

        public bool IsRequiredEnabled =>
            _usage == Usage.Input || _usage == Usage.Output;

        public bool IsVisibleEnabled =>
            (_usage == Usage.Input || _usage == Usage.Output) && _isRequiredChecked == false;

        public bool IsConstantEnabled =>
            _usage == Usage.InOut;

        public bool IsTagTypeEnabled =>
            _usage == Usage.Input || _usage == Usage.Output;

        public bool IsDataTypeEnabled { get; set; }

        public Usage Usage
        {
            get { return _usage; }
            set
            {
                if (_usage != value)
                {
                    _usage = value;
                    _tagType = TagType.Base;
                    _isConstantChecked = false;
                    IsDisplayStyleEnabled = true;
                    IsOpenConfigurationChecked = false;
                    DataType = _canRefreshDataType ? "DINT" : _initialDataType;
                    IsDataTypeEnabled = true;
                    UpdateDisplayStyle();

                    switch (_usage)
                    {
                        case Usage.Local:
                            ExternalAccess = ExternalAccess.None;
                            IsRequiredChecked = false;
                            IsVisibleChecked = false;

                            break;
                        case Usage.Input:
                            ExternalAccess = ExternalAccess.ReadWrite;
                            IsRequiredChecked = false;
                            IsVisibleChecked = false;

                            break;
                        case Usage.Output:
                            ExternalAccess = ExternalAccess.ReadOnly;
                            IsRequiredChecked = false;
                            IsVisibleChecked = false;

                            break;
                        case Usage.InOut:
                            ExternalAccess = ExternalAccess.None;
                            IsRequiredChecked = true;
                            IsVisibleChecked = true;
                            break;
                    }
                }
                
                RaisePropertyChanged("IsDataTypeEnabled");
                RaisePropertyChanged("IsTagTypeEnabled");
                RaisePropertyChanged("IsExternalAccessEnabled");
                RaisePropertyChanged("IsRequiredEnabled");
                RaisePropertyChanged("IsVisibleEnabled");
                RaisePropertyChanged("IsConstantEnabled");
                RaisePropertyChanged("IsDisplayStyleEnabled");
                RaisePropertyChanged("IsAliasForEnabled");
            }
        }

        public List<DisplayItem<ExternalAccess>> ExternalAccessSource
        {
            get { return _externalAccessSource; }
            set { Set(ref _externalAccessSource, value); }
        }

        public RelayCommand SelectDataTypeCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand HelpCommand { get; }
        public RelayCommand OKCommand { get; }

        private bool CanExecuteSelectDataTypeCommand()
        {
            return TagType != TagType.Alias;
        }

        private void ExecuteSelectDataTypeCommand()
        {
            bool supportsOneDimensionalArray = true;
            bool supportsMultiDimensionalArrays = true;

            switch (_usage)
            {
                case Usage.Input:
                case Usage.Output:
                    supportsOneDimensionalArray = false;
                    supportsMultiDimensionalArrays = false;
                    break;

                case Usage.Local:
                    supportsMultiDimensionalArrays = false;
                    break;
            }

            var selectDataTypeDialog = new SelectDataTypeDialog(
                _controller, DataType,
                supportsOneDimensionalArray, supportsMultiDimensionalArrays)
            {
                Owner = Application.Current.MainWindow
            };

            var dialogResult = selectDataTypeDialog.ShowDialog();
            if (dialogResult != null && dialogResult.Value)
            {
                DataType = selectDataTypeDialog.DataType;
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

        private void ExecuteOKCommand()
        {
            if (IsValidTagName(Name)
                && IsValidDataType(DataType))
            {
                if (CreateAndAdd() == 0)
                {
                    DialogResult = true;

                    OpenConfigDialog();
                }
            }
        }

        private int CreateAndAdd()
        {
            try
            {
                var aoiCollection = _aoiDefinition.Tags as TagCollection;

                if (aoiCollection != null)
                {
                    string typeName;
                    int dim1, dim2, dim3;
                    int errorCode;

                    _controller.DataTypes.ParseDataType(
                        DataType,
                        out typeName,
                        out dim1, out dim2, out dim3,
                        out errorCode);

                    if (string.Equals(typeName, "BOOL", StringComparison.OrdinalIgnoreCase)
                        && dim1 > 0 && dim2 == 0 && dim3 == 0)
                    {
                        if (dim1 % 32 != 0)
                        {
                            dim1 = (dim1 / 32 + 1) * 32;
                        }
                    }

                    Tag tag = TagsFactory.CreateTag(aoiCollection, Name, typeName, dim1, dim2, dim3);
                    tag.Description = Description;
                    tag.Usage = Usage;
                    tag.TagType = TagType;
                    tag.ExternalAccess = ExternalAccess;
                    tag.DisplayStyle = DisplayStyle;
                    tag.IsVisible = IsVisibleChecked;
                    tag.IsRequired = IsRequiredChecked;

                    aoiCollection.AddTag(tag, true, false);
                    Notifications.Publish(new MessageData() {Object = tag, Type = MessageData.MessageType.AddTag});
                    NewTag = tag;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                string warningMessage = LanguageManager.GetInstance().ConvertSpecifier("Failed to create a new tag.");
                var warningReason = e.Message;
                var warningDialog = new WarningDialog(warningMessage, warningReason)
                    {Owner = Application.Current.MainWindow};
                warningDialog.ShowDialog();
                return -1;
            }

            return 0;
        }

        private bool IsValidDataType(string dataType)
        {
            string warningMessage = LanguageManager.GetInstance().ConvertSpecifier("Failed to create a new tag.");
            string warningReason = LanguageManager.GetInstance().ConvertSpecifier("Data type could not be found.");

            string typeName;
            int dim1, dim2, dim3;
            int errorCode;

            var isValid = _controller.DataTypes.ParseDataType(
                dataType, out typeName,
                out dim1, out dim2, out dim3, out errorCode);

            if (errorCode == -5)
                warningReason = LanguageManager.GetInstance().ConvertSpecifier(
                    "Number,size,or format of dimensions specified for this tag or type is invalid.");

            if (isValid)
            {
                var foundDataType = _controller.DataTypes[typeName];
                if (dim1 > 0 && !foundDataType.SupportsOneDimensionalArray)
                {
                    isValid = false;
                    warningReason = warningReason = LanguageManager.GetInstance().ConvertSpecifier(
                        "Cannot create arrays of this data type.");
                }
                else if (dim2 > 0 && !foundDataType.SupportsMultiDimensionalArrays)
                {
                    isValid = false;
                    warningReason = "Cannot create multi dimensional arrays of this data type.";
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
                        warningReason = $"{LanguageManager.GetInstance().ConvertSpecifier("The")}" +
                                        $" {dataTypeName} " +
                                        $"{LanguageManager.GetInstance().ConvertSpecifier("data type is not supported by this controller type.")}";
                    }
                }
                
                if (isValid)
                {
                    if (_usage != Usage.InOut)
                    {
                        if (foundDataType.IsMotionGroupType ||
                            foundDataType.IsAxisType ||
                            foundDataType.IsCoordinateSystemType ||
                            foundDataType.IsMessageType)
                        {
                            isValid = false;
                            warningReason = LanguageManager.GetInstance().ConvertSpecifier(
                                "Tag can only be created as InOut parameter in Add-On Instruction.");
                        }
                    }
                }

                if (isValid)
                {
                    if (_usage == Usage.Input || _usage == Usage.Output)
                    {
                        if (!foundDataType.IsAtomic)
                        {
                            isValid = false;
                            warningReason = LanguageManager.GetInstance().ConvertSpecifier(
                                "Input or output parameter must be of supported elementary data type.");
                        }
                        else if (dim1 > 0)
                        {
                            isValid = false;
                            warningReason = LanguageManager.GetInstance().ConvertSpecifier(
                                "Invalid array. Input or output parameter must be of supported elementary data type with on dimensions.");
                        }
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
                        warningReason = LanguageManager.GetInstance().ConvertSpecifier("The array size exceeds 2MBytes.");
                    }
                }
            }

            if (!isValid)
            {
                var warningDialog = new WarningDialog(
                        warningMessage, warningReason)
                    {Owner = Application.Current.MainWindow};
                warningDialog.ShowDialog();
            }

            return isValid;
        }

        private bool IsValidTagName(string name)
        {
            string warningMessage = "Failed to create a new tag.";
            string warningReason = string.Empty;
            bool isValid = true;

            if (string.IsNullOrEmpty(name))
            {
                isValid = false;
                warningReason = "Name is invalid.";
            }

            if (isValid)
            {
                if (name.Length > 40 || name.EndsWith("_") ||
                    name.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    isValid = false;
                    warningReason = "Name is invalid.";
                }
            }

            if (isValid)
            {
                Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
                if (!regex.IsMatch(name))
                {
                    isValid = false;
                    warningReason = "Name is invalid.";
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
                        warningReason = "Name is invalid.";
                    }
                }
            }

            if (isValid)
            {
                var tag = _aoiDefinition.Tags[Name];
                if (tag != null)
                {
                    isValid = false;
                    warningReason = "Already exists.";
                }
            }

            if (!isValid)
            {
                var warningDialog = new WarningDialog(
                        LanguageManager.GetInstance().ConvertSpecifier(warningMessage),
                        LanguageManager.GetInstance().ConvertSpecifier(warningReason))
                    {Owner = Application.Current.MainWindow};
                warningDialog.ShowDialog();
            }

            return isValid;
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

            IsDisplayStyleEnabled = false;
            if (isValid)
            {
                var dataType = _controller.DataTypes[typeName];
                if (dataType is BOOL)
                {
                    IsDisplayStyleEnabled = true;
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
                    IsDisplayStyleEnabled = true;
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
                            DisplayName = "ASCII", Value = DisplayStyle.Ascii
                        }
                    };
                    if (dataType is LINT)
                    {
                        DisplayStyleSource.Add(new DisplayItem<DisplayStyle>()
                            {DisplayName = "Date/Time", Value = DisplayStyle.DateTime});
                        DisplayStyleSource.Add(new DisplayItem<DisplayStyle>()
                            {DisplayName = "Date/Time (ns)", Value = DisplayStyle.DateTimeNS});
                    }
                }
                else if (dataType is REAL)
                {
                    IsDisplayStyleEnabled = true;
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

            if (IsDisplayStyleEnabled)
            {
                bool beContained = false;
                if (DisplayStyleSource != null)
                {
                    foreach (var item in DisplayStyleSource)
                    {
                        if (item.Value == oldDisplayStyle)
                        {
                            beContained = true;
                            break;
                        }
                    }
                }
                else
                {
                    IsDisplayStyleEnabled = false;
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
                IsDisplayStyleEnabled = false;
            }

            RaisePropertyChanged("DisplayStyle");
            RaisePropertyChanged("IsDisplayStyleEnabled");
        }

        private void UpdateUsageSource()
        {
            var oldUsage = _usage;
            var usageList = new List<DisplayItem<Usage>>
            {
                new DisplayItem<Usage> {DisplayName = "Local Tag", Value = Usage.Local},
                new DisplayItem<Usage> {DisplayName = "Input Parameter", Value = Usage.Input},
                new DisplayItem<Usage> {DisplayName = "Output Parameter", Value = Usage.Output},
                new DisplayItem<Usage> {DisplayName = "InOut Parameter", Value = Usage.InOut}
            };

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

            _usage = beContain ? oldUsage : usageList[0].Value;
        }

        private void OpenConfigDialog()
        {
            if (IsOpenConfigurationChecked)
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
                    PidSetupDialog pidSetupDialog = new PidSetupDialog(new PidSetUpViewModel(field,NewTag))
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
                    var uiShell = (IVsUIShell) Package.GetGlobalService(typeof(SVsUIShell));

                    var window =
                        createDialogService?.CreateAxisCIPDriveProperties(NewTag);
                    window?.Show(uiShell);
                }

                if (typeName.Equals("Motion_Group", StringComparison.OrdinalIgnoreCase))
                {
                    var createDialogService =
                        (ICreateDialogService) Package.GetGlobalService(typeof(SCreateDialogService));

                    var uiShell = (IVsUIShell) Package.GetGlobalService(typeof(SVsUIShell));

                    var window =
                        createDialogService?.CreateMotionGroupProperties(NewTag);
                    window?.Show(uiShell);
                }
            }
        }
    }
}