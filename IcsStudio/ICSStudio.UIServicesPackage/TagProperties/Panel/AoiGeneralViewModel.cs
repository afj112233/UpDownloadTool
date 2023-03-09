using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Dialogs.SelectDataType;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using Application = System.Windows.Application;
using ICSStudio.SimpleServices.Compiler;

namespace ICSStudio.UIServicesPackage.TagProperties.Panel
{
    public class AoiGeneralViewModel : ViewModelBase, IOptionPanel
    {
        private string _name;
        private string _dataType;
        private string _description;
        private string _aoiCollectionContainer;
        private bool _isVisibleChecked;
        private bool _isRequiredChecked;
        private bool _isConstantChecked;

        private bool _isDirty;
        private bool _isNameDirty = false;
        public bool IsDescriptionDirty = false;
        public bool IsDataTypeDirty = false;
        public bool IsUsageDirty = false;
        public bool IsTypeDirty = false;
        public bool IsExternalAccessDirty = false;
        public bool IsConstantDirty = false;
        public bool IsStyleDirty = false;
        public bool IsRequiredDirty = false;
        public bool IsVisibleDirty = false;

        private readonly IController _controller;
        private readonly IAoiDefinition _aoiDefinition;
        private Usage _usage;
        private TagType _tagType;
        private DisplayStyle _displayStyle;
        private ExternalAccess _externalAccess;
        private List<DisplayItem<Usage>> _usages;
        private List<DisplayItem<DisplayStyle>> _displayStyleSource;
        private List<DisplayItem<ExternalAccess>> _externalAccessSource;
        private Tag _tag;
        private bool _isAoiGeneralEnabled = true;

        public AoiGeneralViewModel(AoiGeneral panel, ITag tag)
        {
            if (tag == null)
                throw new Exception("tag is null.");
            Control = panel;
            panel.DataContext = this;
            _externalAccess = tag.ExternalAccess;
            _isRequiredChecked = tag.IsRequired;
            _isVisibleChecked = tag.IsVisible;
            _isConstantChecked = tag.IsConstant;
            _displayStyle = tag.DisplayStyle;
            _tag = (Tag) tag;
            _name = tag.Name;
            _usage = tag.Usage;
            _tagType = _tag.TagType;
            _description = _tag.Description;
            _aoiDefinition = tag.ParentCollection.ParentProgram as IAoiDefinition;
            _controller = Controller.GetInstance();

            if (_aoiDefinition != null) _aoiCollectionContainer = _aoiDefinition.Name;
            SelectDataTypeCommand = new RelayCommand(ExecuteSelectDataTypeCommand, CanSelectDataTypeCommandExecute);
            _dataType = tag.DataTypeInfo.ToString();

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

            if (_aoiDefinition != null)
            {
                IsAoiGeneralEnabled = !_aoiDefinition.IsSealed;
                PropertyChangedEventManager.AddHandler(_aoiDefinition, _aoiDefinition_PropertyChanged, "IsSealed");
            }

            UpdateUsageSource();
            UpdateDisplayStyle();
            IsDirty = false;
            PropertyChangedEventManager.AddHandler(tag, OnTagItemChanged, "");
        }

        private bool CanSelectDataTypeCommandExecute()
        {
            if (_tag.ParentController.IsOnline)
            {
                return false;
            }
            if (!_tag.DataTypeInfo.DataType.IsBool && TagType != TagType.Alias)
            {
                return true;
            }

            return _tag.DataTypeInfo.DataType.IsBool && (_tag.Name != "EnableIn" || _tag.Name != "EnableOut");
        }

        public string AoiCollectionContainer
        {
            get { return _aoiCollectionContainer; }
            set
            {
                Set(ref _aoiCollectionContainer, value);
                UpdateUsageSource();
            }
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (_isDirty != value)
                {
                    Set(ref _isDirty, value);
                }
            }
        }

        public string ErrorMessage { private set; get; }

        public ITag NewTag { get; set; }

        public object Owner { get; set; }

        public object Control { get; }

        public bool IsAoiGeneralEnabled
        {
            set { Set(ref _isAoiGeneralEnabled, value); }
            get { return _isAoiGeneralEnabled; }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    Set(ref _name, value);
                    IsDirty = true;
                    _isNameDirty = true;
                }
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    Set(ref _description, value);
                    IsDirty = true;
                    IsDescriptionDirty = true;
                }
            }
        }

        public List<TagType> TagTypes { get; }

        public TagType TagType
        {
            get { return _tagType; }
            set
            {
                if (_tagType != value)
                {
                    Set(ref _tagType, value);
                    RaisePropertyChanged("AliasForIsEnabled");
                    RaisePropertyChanged("IsExternalAccessEnabled");
                    RaisePropertyChanged("IsDisplayStyleEnabled");
                    RaisePropertyChanged("IsDataTypeEnabled");
                    IsDirty = true;
                    IsTypeDirty = true;
                    SelectDataTypeCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public List<string> AllDataTypeNames { get; } = new List<string>();

        public string DataType
        {
            get { return _dataType; }
            set
            {
                if (_dataType != value)
                {
                    _dataType = value;
                    UpdateDisplayStyle();
                    IsDirty = true;
                    IsDataTypeDirty = true;
                }
            }
        }

        public List<DisplayItem<Usage>> Usages
        {
            get { return _usages; }
            set { Set(ref _usages, value); }
        }

        public ExternalAccess ExternalAccess
        {
            get { return _externalAccess; }
            set
            {
                if (_externalAccess != value)
                {
                    _externalAccess = value;
                    IsDirty = true;
                    IsExternalAccessDirty = true;
                }
            }
        }

        public DisplayStyle DisplayStyle
        {
            get { return _displayStyle; }
            set
            {
                if (_displayStyle != value)
                {
                    _displayStyle = value;
                    IsDirty = true;
                    IsStyleDirty = true;
                }
            }
        }

        public List<DisplayItem<DisplayStyle>> DisplayStyleSource
        {
            get { return _displayStyleSource; }
            set { Set(ref _displayStyleSource, value); }
        }

        public Usage Usage
        {
            get { return _usage; }
            set
            {
                if (_usage != value)
                {
                    _usage = value;
                    DataType = "DINT";
                    DisplayStyle = DisplayStyle.Decimal;
                    _isConstantChecked = false;

                    if (_usage == Usage.InOut)
                    {
                        IsRequiredChecked = true;
                        IsVisibleChecked = true;
                    }
                    else
                    {
                        IsRequiredChecked = false;
                        IsVisibleChecked = false;
                    }

                    RaisePropertyChanged("IsTagTypeEnabled");
                    RaisePropertyChanged("AliasForIsEnabled");
                    RaisePropertyChanged("IsDataTypeEnabled");
                    RaisePropertyChanged("IsExternalAccessEnabled");
                    RaisePropertyChanged("IsDisplayStyleEnabled");
                    RaisePropertyChanged("IsRequiredEnabled");
                    RaisePropertyChanged("IsVisibleEnabled");
                    RaisePropertyChanged("IsConstantEnabled");

                    IsDirty = true;
                    IsUsageDirty = true;
                    SelectDataTypeCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsUsagesEnabled
        {
            get
            {
                return !_tag.ParentController.IsOnline &&
                       (!_tag.DataTypeInfo.DataType.IsBool ||
                        (_tag.Name != "EnableIn" && _tag.Name != "EnableOut"));
            }
        }

        public bool IsNameEnabled
        {
            get
            {
                return !Name.Equals("EnableIn") && !Name.Equals("EnableOut") &&
                       !_tag.ParentController.IsOnline;
            }
        }

        public bool IsTagTypeEnabled
        {
            get
            {
                return (Usage != Usage.Local && Usage != Usage.InOut) &&
                       (!Name.Equals("EnableIn") && !Name.Equals("EnableOut")) &&
                       !_tag.ParentController.IsOnline &&
                       !_tag.Name.Equals("EnableIn", StringComparison.OrdinalIgnoreCase) &&
                       !_tag.Name.Equals("EnableOut", StringComparison.OrdinalIgnoreCase);
            }
        }

        public bool AliasForIsEnabled =>
            _tagType == TagType.Alias;

        public bool IsDataTypeEnabled
        {
            get
            {
                if (_tag.ParentController.IsOnline)
                {
                    return false;
                }

                if (!_tag.DataTypeInfo.DataType.IsBool && TagType != TagType.Alias)
                {
                    return true;
                }

                return _tag.DataTypeInfo.DataType.IsBool && (_tag.Name != "EnableIn" || _tag.Name != "EnableOut");
            }
        }

        public bool IsExternalAccessEnabled
        {
            get
            {
                if (_tag.ParentController.IsOnline)
                {
                    return false;
                }

                return _usage == Usage.Input && _tagType != TagType.Alias
                       || _usage == Usage.Output && _tagType != TagType.Alias;
            }
        }

        public bool IsDisplayStyleEnabled
        {
            get
            {
                if (_tag.ParentController.IsOnline)
                {
                    return false;
                }

                if (_tag.Name.Equals("EnableIn", StringComparison.OrdinalIgnoreCase) ||
                    _tag.Name.Equals("EnableOut", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (!((_usage == Usage.Input && _tagType != TagType.Alias)
                    || (_usage == Usage.Output && _tagType != TagType.Alias)
                    || _usage == Usage.InOut || _usage == Usage.Local))
                {
                    return false;
                }

                string typeName;
                int dim1, dim2, dim3;
                int errorCode;
                _controller.DataTypes.ParseDataType(
                    DataType,
                    out typeName,
                    out dim1, out dim2, out dim3,
                    out errorCode);

                if (_controller.DataTypes[typeName] != null)
                {
                    return (_controller.DataTypes[typeName] is BOOL ||
                            _controller.DataTypes[typeName].IsInteger ||
                            _controller.DataTypes[typeName] is REAL);
                }

                return false;
            }
        }

        public bool IsRequiredEnabled
        {
            get
            {
                if (_tag.ParentController.IsOnline)
                {
                    return false;
                }

                if (_tag.Name.Equals("EnableIn", StringComparison.OrdinalIgnoreCase) ||
                    _tag.Name.Equals("EnableOut", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                return _usage == Usage.Input || _usage == Usage.Output;

            }
        }

        public bool IsVisibleEnabled
        {
            get
            {
                if (_tag.ParentController.IsOnline)
                {
                    return false;
                }

                if (_tag.Name.Equals("EnableIn", StringComparison.OrdinalIgnoreCase) ||
                    _tag.Name.Equals("EnableOut", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                return (_usage == Usage.Input || _usage == Usage.Output) && _isRequiredChecked == false;
            }
        }

        public bool IsConstantEnabled
        {
            get
            {
                if (_tag.ParentController.IsOnline)
                {
                    return false;
                }

                if (_tag.Name.Equals("EnableIn", StringComparison.OrdinalIgnoreCase) ||
                    _tag.Name.Equals("EnableOut", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                return _usage == Usage.InOut;
            }
        }

        public bool IsConstantChecked
        {
            set
            {
                if (_isConstantChecked != value)
                {
                    _isConstantChecked = value;
                    IsDirty = true;
                }
            }
            get { return _isConstantChecked; }
        }

        public bool IsRequiredChecked
        {
            get { return _isRequiredChecked; }
            set
            {
                if (IsRequiredEnabled != value)
                {
                    IsDirty = true;
                    IsRequiredDirty = true;
                }

                Set(ref _isRequiredChecked, value);

                if (_isRequiredChecked)
                {
                    IsVisibleChecked = true;
                }
            }
        }

        public bool IsVisibleChecked
        {
            get { return _isVisibleChecked; }
            set
            {
                if (_isVisibleChecked != value)
                {
                    Set(ref _isVisibleChecked, value);
                    IsDirty = true;
                    IsVisibleDirty = true;

                    RaisePropertyChanged("IsVisibleEnabled");
                }
            }
        }

        public List<DisplayItem<ExternalAccess>> ExternalAccessSource
        {
            get { return _externalAccessSource; }
            set { Set(ref _externalAccessSource, value); }
        }

        public RelayCommand SelectDataTypeCommand { get; }

        private void ExecuteSelectDataTypeCommand()
        {
            bool supportsOneDimensionalArray = true;
            bool supportsMultiDimensionalArrays = true;

            if (_usage == Usage.Output)
            {
                supportsOneDimensionalArray = false;
                supportsMultiDimensionalArrays = false;
            }
            else if (_usage == Usage.Local)
            {
                supportsMultiDimensionalArrays = false;
            }

            var selectDataTypeDialog = new SelectDataTypeDialog(_controller, DataType, supportsOneDimensionalArray,
                supportsMultiDimensionalArrays)
            {
                Owner = Application.Current.MainWindow
            };

            var dialogResult = selectDataTypeDialog.ShowDialog();
            if (dialogResult != null && dialogResult.Value)
            {
                DataType = selectDataTypeDialog.DataType;
            }
        }

        public override void Cleanup()
        {
            if (_aoiDefinition != null)
                PropertyChangedEventManager.RemoveHandler(_aoiDefinition, _aoiDefinition_PropertyChanged, "IsSealed");
            PropertyChangedEventManager.RemoveHandler(_tag, OnTagItemChanged, "");
        }

        private void _aoiDefinition_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsAoiGeneralEnabled = !_aoiDefinition.IsSealed;
        }

        public bool Save()
        {
            if (IsValidTagName(Name)
                && IsValidDataType(DataType))
            {
                if (ApplyChanges() != 0)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public void LoadOptions()
        {
        }

        public bool SaveOptions()
        {
            return true;
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

            var IsDisplayStyleSourceInitialization = false;
            if (isValid)
            {
                var dataType = _controller.DataTypes[typeName];
                if (dataType is BOOL)
                {
                    IsDisplayStyleSourceInitialization = true;
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
                else if (dataType is SINT || dataType is INT || dataType is DINT || dataType is LINT)
                {
                    IsDisplayStyleSourceInitialization = true;
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
                            DisplayName = "Hex", Value = DisplayStyle.Hex
                        },
                        new DisplayItem<DisplayStyle>
                        {
                            DisplayName = "ASCII", Value = DisplayStyle.Ascii
                        }
                    };
                }
                else if (dataType is REAL)
                {
                    IsDisplayStyleSourceInitialization = true;
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

            if (IsDisplayStyleSourceInitialization)
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
            RaisePropertyChanged("DisplayStyleSource");
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

            Usage = beContain ? oldUsage : usageList[0].Value;

        }

        private int ApplyChanges()
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

                    var newDataWrapper = new DataWrapper(_controller.DataTypes[typeName], dim1, dim2, dim3, null);
                    DataType = newDataWrapper.DataTypeInfo.ToString();
                    Tag tag = _tag;
                    DataHelper.Copy(newDataWrapper, tag.DataWrapper);
                    bool isNeedReset = false;
                    if (!_tag.DataWrapper.DataTypeInfo.ToString().Equals(newDataWrapper.DataTypeInfo.ToString()))
                    {
                        aoiCollection.IsNeedVerifyRoutine = false;
                        _tag.UpdateDataWrapper(newDataWrapper,DisplayStyle);
                        isNeedReset = true;
                    }
                    else
                    {
                        if (_tag.DisplayStyle != DisplayStyle)
                        {
                            _tag.DisplayStyle = DisplayStyle;
                        }
                    }

                    if (!_tag.Name.Equals(Name))
                    {
                        _tag.Name = Name;
                    }

                    if (!_tag.Description?.Equals(Description) ?? true)
                    {
                        _tag.Description = Description;
                    }

                    if (_tag.Usage != Usage)
                    {
                        _tag.Usage = Usage;
                        isNeedReset = true;
                    }

                    if (tag.TagType != TagType)
                    {
                        tag.TagType = TagType;
                    }
                    
                    if (_tag.ExternalAccess != ExternalAccess)
                    {
                        _tag.ExternalAccess = ExternalAccess;
                    }
                    
                    if (_tag.IsConstant != IsConstantChecked)
                    {
                        _tag.IsConstant = IsConstantChecked;
                    }

                    if (_tag.IsRequired != IsRequiredChecked)
                    {
                        _tag.IsRequired = IsRequiredChecked;
                        isNeedReset = true;
                    }

                    if (_tag.IsVisible != IsVisibleChecked)
                    {
                        _tag.IsVisible = IsVisibleChecked;
                    }

                    if (isNeedReset)
                    {
                        ((AoiDefinition)_aoiDefinition).datatype.Reset();
                        PendingCompileRoutine.GetInstance()?.CompilePendingRoutines();
                        aoiCollection.IsNeedVerifyRoutine = true;
                    }
                    //UpdateConfig();
                    NewTag = tag;
                    IsDirty = false;
                    ResetIsDirty();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorMessage = "Failed to create a new tag.";
                return -1;
            }

            return 0;
        }
        
        public void ResetIsDirty()
        {
            _isNameDirty = false;
            IsDescriptionDirty = false;
            IsDataTypeDirty = false;
            IsUsageDirty = false;
            IsTypeDirty = false;
            IsExternalAccessDirty = false;
            IsConstantDirty = false;
            IsStyleDirty = false;
            IsRequiredDirty = false;
            IsVisibleDirty = false;
        }

        private bool IsValidDataType(string dataType)
        {
            string warningReason = "The data Type could not be found or the format is invalid.";

            string typeName;
            int dim1, dim2, dim3;
            int errorCode;

            var isValid = _controller.DataTypes.ParseDataType(
                dataType, out typeName,
                out dim1, out dim2, out dim3, out errorCode);

            if (DataType == "")
            {
                warningReason = "The data Type could not be found or the format is invalid.";
                isValid = false;
            }

            if (dataType == null)
            {
                warningReason = "Data type is empty.";
                isValid = false;
            }

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
                    if (foundDataType.IsMessageType && Usage != Usage.InOut)
                    {
                        warningReason = "Tag can only be create as InOut parameter in Add-On Instruction.";
                        isValid = false;
                    }
                }

                if (isValid)
                {
                    if (!AOIExtend.VerifyLocalDataType(typeName))
                    {
                        if (!foundDataType.IsMessageType)
                        {
                            warningReason = "Tag can only be create as InOut parameter in Add-On Instruction.";
                            isValid = false;
                        }
                    }
                }

                if (isValid)
                {
                    if (Usage == Usage.Local)
                    {
                        if (dim2 > 0)
                        {
                            warningReason =
                                "Number,size,or format of dimensions specified for this tag or type is invalid.";
                            isValid = false;
                        }
                    }
                }

                if (isValid)
                {
                    if (Usage == Usage.Input || Usage == Usage.Output)
                    {
                        if (dim1 > 0 || dim2 > 0 || dim3 > 0 || !foundDataType.IsAtomic)
                        {
                            warningReason = "Input or output parameter must be of supported elementary data type.";
                            isValid = false;
                        }
                    }

                    //有需要的话需要在加一个判断isvalid
                    if (Usage == Usage.InOut)
                    {

                    }
                }
            }

            if (!isValid)
            {
                ErrorMessage = $"Failed to modify the properties for the tag \"{_tag.Name}\".\n" + warningReason;
            }

            return isValid;
        }

        private bool IsValidTagName(string name)
        {
            string warningMessage = "Failed to modify the properties for the tag \" " + name + "\".";
            string warningReason = string.Empty;
            bool isValid = true;

            if (string.IsNullOrEmpty(name))
            {
                isValid = false;
                warningReason = LanguageManager.GetInstance().ConvertSpecifier("VariableNameInvalid");
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
                    "not", "mod", "and", "xor", "or"
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
                var tag = _aoiDefinition.Tags[name];
                if (tag != null && tag != _tag)
                {
                    isValid = false;
                    warningReason = "Object by the same name already exists in the collection.";
                }
            }

            if (!isValid)
            {
                var warningDialog = new WarningDialog(warningMessage, warningReason)
                    {Owner = Application.Current.MainWindow};
                warningDialog.ShowDialog();
            }

            return isValid;
        }

        private void OnTagItemChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Name":
                    if (!_isNameDirty)
                    {
                        _name = _tag.Name;
                        RaisePropertyChanged(nameof(Name));
                    }

                    break;

                case "Description":
                    if (!IsDescriptionDirty)
                    {
                        _description = _tag.Description;
                        RaisePropertyChanged(nameof(Description));
                    }

                    break;

                case "DataType":
                    if (!IsDataTypeDirty)
                    {
                        _dataType = _tag.DataTypeInfo.DataType.ToString();
                        RaisePropertyChanged(nameof(DataType));
                    }

                    break;

                case "Usage":
                    if (!IsUsageDirty)
                    {
                        _usage = _tag.Usage;
                        RaisePropertyChanged(nameof(Usage));
                    }

                    break;

                case "TagType":
                    if (!IsTypeDirty)
                    {
                        _tagType = _tag.TagType;
                        RaisePropertyChanged(nameof(TagType));
                    }

                    break;

                case "ExternalAccess":
                    if (!IsExternalAccessDirty)
                    {
                        _externalAccess = _tag.ExternalAccess;
                        RaisePropertyChanged(nameof(ExternalAccess));
                    }

                    break;

                case "DisplayStyle":
                    if (!IsStyleDirty)
                    {
                        _displayStyle = _tag.DisplayStyle;
                        RaisePropertyChanged(nameof(DisplayStyle));
                    }

                    break;

                case "IsConstant":
                    if (!IsConstantDirty)
                    {
                        _isConstantChecked = _tag.IsConstant;
                        RaisePropertyChanged(nameof(IsConstantChecked));
                    }

                    break;
                case "IsRequired":
                    if (!IsRequiredDirty)
                    {
                        _isRequiredChecked = _tag.IsRequired;
                        RaisePropertyChanged(nameof(IsRequiredChecked));
                    }

                    break;

                case "IsVisible":
                    if (!IsVisibleDirty)
                    {
                        _isVisibleChecked = _tag.IsVisible;
                        RaisePropertyChanged(nameof(IsVisibleChecked));
                    }

                    break;
            }
        }
    }
}
