using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using Imagin.Common.Extensions;

namespace ICSStudio.UIServicesPackage.TagProperties.Panel
{
    [SuppressMessage("ReSharper", "ArrangeAccessorOwnerBody")]
    class GeneralViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private readonly IController _controller;
        private readonly IProgramModule _parentProgram;

        private readonly Tag _tag;

        private string _dataType;

        private DisplayStyle? _style;
        private string _name;
        private string _description;
        private Usage _usage;
        private TagType _type;
        private ExternalAccess _externalAccess;
        private bool _isConstant;
        private bool _styleEnabled;
        private bool _openParameterConnectionsEnabled;

        public GeneralViewModel(General panel, ITag tag, IProgramModule parentProgram)
        {
            Control = panel;
            panel.DataContext = this;

            _parentProgram = parentProgram;
            _tag = tag as Tag;
            Debug.Assert(_tag != null);

            _controller = _tag.ParentController;
            _name = _tag.Name;
            _description = _tag.Description;
            _usage = _tag.Usage;
            _type = _tag.TagType;
            _dataType = _tag.DataTypeInfo.ToString();
            _externalAccess = _tag.ExternalAccess;
            _style = _tag.DisplayStyle;

            DataTypeCommand = new RelayCommand(ExecuteDataTypeCommand, CanExecute);

            //暂时不支持TagType.Alias,在线或离线不可创建TagType.Produced, TagType.Consumed会导致upload失败
            var typeList = new List<TagType> { TagType.Base };
            Types = typeList.Select(t => new { Display = t.ToString(), Value = t }).ToList();

            var externalAccessList = new List<ExternalAccess>
                { ExternalAccess.ReadWrite, ExternalAccess.ReadOnly, ExternalAccess.None };

            ExternalAccesses = externalAccessList.Select(e => { return new { Display = e.ToString(), Value = e }; })
                .ToList();

            ResetStyle();

            var usageList = new List<DisplayItem<Usage>>();

            var oldUsage = Usage;

            if (_tag.ParentCollection.ParentProgram == null)
            {
                //Controller范围下
                _isConstant = _tag.IsConstant;

                Program = _tag.ParentController.Name;

                usageList.Add(new DisplayItem<Usage>
                {
                    DisplayName = "<controller>",
                    Value = Usage.Static
                });

                Usages = usageList;
            }
            else
            {
                //Program范围下
                _isConstant = _tag.IsConstant;

                Program = _tag.ParentCollection.ParentProgram.Name;

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
               
                Usages = usageList;
            }

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

            foreach (var item in _controller.DataTypes)
            {
                if (item.Name.StartsWith("BOOL:", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (item.Name.Contains("$"))
                    continue;
                AutoCompleteSource.Add(item.Name);
            }

            PropertyChangedEventManager.AddHandler(tag, OnTagItemChanged, "");
        }

        private bool CanExecute()
        {
            //TODO(TLM):暂未测试在线情况下是否可用
            return (!_tag.DataTypeInfo.DataType.IsIOType) && !_tag.ParentController.IsOnline;
        }

        public RelayCommand DataTypeCommand { get; }

        private void ExecuteDataTypeCommand()
        {
            var dialog = new Dialogs.SelectDataType.SelectDataTypeDialog(_controller, DataType, true, true)
            {
                Height = 350, Width = 400, Owner = Application.Current.MainWindow
            };
            var dialogResult = dialog.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                DataType = dialog.DataType;
                RaisePropertyChanged(nameof(DataType));
            }
        }

        public bool DataTypeIsEnabled
        {
            get { return (!_tag.DataTypeInfo.DataType.IsIOType) && (!_tag.ParentController.IsOnline); }
        }

        public bool UsageEnabled
        {
            get
            {
                return _tag.ParentCollection.ParentProgram != null ;
            }
        }

        public List<string> AutoCompleteSource { get; } = new List<string>();

        public object Owner { get; set; }

        public object Control { get; }

        public bool ConstantEnabled
        {
            get
            {
                if (_tag.ParentController.IsOnline)
                    return false;

                return (Type != TagType.Alias && Type != TagType.Consumed) ||
                       !(_tag.DataTypeInfo.DataType is AXIS_COMMON);
            }
        }

        public bool OpenParameterConnectionsEnabled
        {
            get
            {
                if (_tag.ParentCollection.ParentProgram != null)
                {
                    _openParameterConnectionsEnabled = _usage != Usage.Local;
                }

                return _openParameterConnectionsEnabled;
            }
        }

        public bool IsOpenParameterConnections { get; set; }

        public string Name
        {
            set
            {
                if (_name != value)
                {
                    _name = value;

                    RaisePropertyChanged();

                    UpdateIsDirty();
                }
            }
            get { return _name; }
        }

        public bool NameIsEnabled
        {
            get { return !_tag.DataTypeInfo.DataType.IsIOType && (!_tag.ParentController.IsOnline); }
        }

        public string Description
        {
            set
            {
                if (_description != value)
                {
                    _description = value;
                    RaisePropertyChanged();
                    UpdateIsDirty();
                }
            }
            get { return _description; }
        }

        public bool IsDescriptionEnabled
        {
            get
            {
                return !_tag.ParentController.IsOnline;
            }
        }

        public string DataType
        {
            set
            {
                if (_dataType != value)
                {
                    _dataType = value;
                    ResetStyle();
                    RaisePropertyChanged();
                    UpdateIsDirty();
                }
            }
            get { return _dataType; }
        }

        public string Program { get; }

        //public object Usages { get; }
        public List<DisplayItem<Usage>> Usages { get; }

        public Usage Usage
        {
            set
            {
                if (_usage != value)
                {
                    _usage = value;

                    RaisePropertyChanged("OpenParameterConnectionsEnabled");
                    RaisePropertyChanged();
                    UpdateIsDirty();
                }
            }
            get { return _usage; }
        }

        public IList Types { get; }

        public TagType Type
        {
            set
            {
                if (_type != value)
                {
                    _type = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged("ExternalAccessEnabled");
                    RaisePropertyChanged("ConstantEnabled");
                    UpdateIsDirty();
                }
            }
            get { return _type; }
        }

        public IList ExternalAccesses { get; }

        public ExternalAccess ExternalAccess
        {
            set
            {
                if (_externalAccess != value)
                {
                    _externalAccess = value;
                    RaisePropertyChanged();
                    UpdateIsDirty();
                }
            }
            get { return _externalAccess; }
        }

        public bool IsConstant
        {
            set
            {
                if (_isConstant != value)
                {
                    _isConstant = value;
                    RaisePropertyChanged();
                    UpdateIsDirty();
                }
            }
            get { return _isConstant; }
        }


        public ObservableCollection<DisplayStyle> Styles { get; } = new ObservableCollection<DisplayStyle>();

        public DisplayStyle? Style
        {
            set
            {
                if (_style != value)
                {
                    _style = value;
                    RaisePropertyChanged();
                    UpdateIsDirty();
                }
            }
            get { return _style; }
        }

        public void LoadOptions()
        {

        }

        public bool SaveOptions()
        {
            if (CheckDataType() && CheckName())
            {
                string typeName = "";
                int dim1 = 0, dim2 = 0, dim3 = 0, errorCode = 0;
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

                if (_tag.DataTypeInfo.ToString() != DataType)
                {
                    var dataWrapper = new DataWrapper(
                        _controller.DataTypes[typeName],
                        dim1,
                        dim2,
                        dim3, null);
                    _tag.UpdateDataWrapper(dataWrapper, Style ?? DisplayStyle.NullStyle);
                    if (string.Equals(typeName, "BOOL", StringComparison.OrdinalIgnoreCase))
                    {
                        DataType = dataWrapper.DataTypeInfo.ToString();
                    }
                }
                else
                {
                    if (_tag.DisplayStyle != Style)
                    {
                        _tag.DisplayStyle = Style ?? DisplayStyle.NullStyle;
                    }
                }

                if (!_tag.Name.Equals(Name))
                {
                    _tag.Name = Name;
                }

                if (_tag.Description != null)
                {
                    if (!_tag.Description.Equals(Description))
                    {
                        _tag.Description = Description;
                    }
                }
                else
                {
                    if (Description != null)
                    {
                        _tag.Description = Description;
                    }
                }

                if (_tag.Usage != Usage)
                {
                    if (_tag.ParentCollection.ParentProgram != null)
                    {
                        _tag.Usage = Usage;
                    }
                }

                if (_tag.TagType != Type)
                {
                    _tag.TagType = Type;
                }

                if (_tag.ExternalAccess != ExternalAccess)
                {
                    _tag.ExternalAccess = ExternalAccess;
                }

                if (_tag.IsConstant != IsConstant)
                {
                    _tag.IsConstant = IsConstant;
                }

                //var aoiDefinition = _parentProgram as AoiDefinition;
                //if (aoiDefinition != null)
                //{
                //    //修改config
                //    aoiDefinition.Reset();
                //}

                IsDirty = false;
                IsDirtyChanged?.Invoke(this, new EventArgs());
                return true;
            }

            return false;
        }


        public string ErrorMessage { set; get; }

        public bool StyleEnabled
        {
            set { Set(ref _styleEnabled, value); }
            get
            {
                //TODO(gjc): edit later
                if (_tag.ParentController.IsOnline)
                    return false;

                return _styleEnabled;
            }
        }

        public bool TypeEnabled
        {
            get { return (!_tag.DataTypeInfo.DataType.IsIOType) && (!_tag.ParentController.IsOnline); }
        }

        public bool ExternalAccessEnabled
        {
            get
            {
                if (Type == TagType.Alias)
                {
                    return false;
                }

                if (_tag.ParentController.IsOnline)
                {
                    //TODO(gjc): edit later
                    return false;
                    //end

                    //if (_tag.DataTypeInfo.DataType.IsIOType)
                    //{
                    //    return false;
                    //}
                }

                return true;
            }
        }

        private void ResetStyle()
        {
            Styles.Clear();

            string dataType = DataType;

            int index = dataType.IndexOf("[", StringComparison.OrdinalIgnoreCase);
            if (index > 0)
            {
                dataType = dataType.Substring(0, index);
            }

            if (dataType.Equals("REAL", StringComparison.OrdinalIgnoreCase))
            {
                Styles.Add(DisplayStyle.Float);
                Styles.Add(DisplayStyle.Exponential);
                if (Style == null || !Styles.Contains((DisplayStyle)Style))
                    Style = DisplayStyle.Float;
            }
            else if (dataType.Equals("BOOL", StringComparison.OrdinalIgnoreCase))
            {
                Styles.Add(DisplayStyle.Binary);
                Styles.Add(DisplayStyle.Octal);
                Styles.Add(DisplayStyle.Decimal);
                Styles.Add(DisplayStyle.Hex);
                if (Style == null || !Styles.Contains((DisplayStyle)Style))
                    Style = DisplayStyle.Decimal;
            }
            else if (dataType.Equals("DINT", StringComparison.OrdinalIgnoreCase) ||
                     dataType.Equals("SINT", StringComparison.OrdinalIgnoreCase) ||
                     dataType.Equals("INT", StringComparison.OrdinalIgnoreCase))
            {
                Styles.Add(DisplayStyle.Binary);
                Styles.Add(DisplayStyle.Octal);
                Styles.Add(DisplayStyle.Decimal);
                Styles.Add(DisplayStyle.Hex);
                Styles.Add(DisplayStyle.Ascii);
                if (Style == null || !Styles.Contains((DisplayStyle)Style))
                    Style = DisplayStyle.Decimal;
            }
            else if (dataType.Equals("LINT", StringComparison.OrdinalIgnoreCase))
            {
                Styles.Add(DisplayStyle.Binary);
                Styles.Add(DisplayStyle.Octal);
                Styles.Add(DisplayStyle.Decimal);
                Styles.Add(DisplayStyle.Hex);
                Styles.Add(DisplayStyle.Ascii);
                Styles.Add(DisplayStyle.DateTime);
                Styles.Add(DisplayStyle.DateTimeNS);
                if (Style == null || !Styles.Contains((DisplayStyle)Style))
                    Style = DisplayStyle.Decimal;
            }

            StyleEnabled = true;
            if (Styles.Count == 0)
            {
                Style = null;
                StyleEnabled = false;
            }
        }

        private bool CheckName()
        {
            if (_tag.Name.Equals(Name, StringComparison.OrdinalIgnoreCase))
                return true;

            bool isValid = true;

            string warningReason = "";

            Regex regex = new Regex("^[a-zA-Z_][a-zA-Z0-9_]*$");
            if (string.IsNullOrEmpty(Name) || !regex.IsMatch(Name))
            {
                warningReason = LanguageManager.GetInstance().ConvertSpecifier("VariableNameInvalid");
                isValid = false;
            }

            if (isValid)
            {
                if (Name.Length > 40 || Name.EndsWith("_") ||
                    Name.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    isValid = false;
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("VariableNameInvalid");
                }
            }

            if (_tag.ParentCollection.ParentProgram == null)
            {
                if (_tag.ParentController.Tags.FirstOrDefault(t =>
                        t.Name.Equals(Name, StringComparison.OrdinalIgnoreCase)) != null)
                {
                    isValid = false;
                    warningReason = "Object by the same name already exists in this collection.";
                }
            }
            else
            {
                if ((_parentProgram as Program)?.Tags.FirstOrDefault(t =>
                        t.Name.Equals(Name, StringComparison.OrdinalIgnoreCase)) != null)
                {
                    isValid = false;
                    warningReason = "Object by the same name already exists in this collection.";
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
                    if (keyWord.Equals(Name, StringComparison.OrdinalIgnoreCase))
                    {
                        isValid = false;
                        warningReason = LanguageManager.GetInstance().ConvertSpecifier("VariableNameInvalid");
                    }
                }
            }

            if (!isValid)
                //ErrorMessage = _tag.ParentCollection.ParentProgram == _parentProgram
                //    ? $"Failed to modify the properties for the tag '{_tag.Name}'.\n" + "Object by the same name already exists in this collection."
                //    : $"Failed to modify the properties for the tag \"{_tag.Name}\".\nOperation creates a scope conflict for a tag reference.";
                ErrorMessage = _tag.ParentCollection.ParentProgram == _parentProgram
                    ? $"Failed to modify the properties for the tag '{_tag.Name}'.\n" + warningReason
                    : $"Failed to modify the properties for the tag '{_tag.Name}'.\nOperation creates a scope conflict for a tag reference.";

            return isValid;
        }

        private bool CheckDataType()
        {
            if (_tag.DataTypeInfo.ToString().Equals(DataType))
                return true;

            string warningReason = "The data Type could not be found or the format is invalid.";

            string typeName;
            int dim1, dim2, dim3;
            int errorCode;

            var isValid = _controller.DataTypes.ParseDataType(
                DataType, out typeName,
                out dim1, out dim2, out dim3, out errorCode);

            if (errorCode == -5)
            {
                warningReason = "Number,size,or format of dimensions specified for this tag or type is invalid.";
                isValid = false;
            }

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
                    if ((foundDataType.IsAxisType
                         || foundDataType.IsMotionGroupType
                         || foundDataType.IsCoordinateSystemType
                         || foundDataType.IsMessageType)
                        && !_tag.IsControllerScoped)
                    {
                        isValid = false;
                        warningReason = "Tag can only be created at the controller scope.";
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
            }

            if (!isValid)
            {
                ErrorMessage = $"Failed to modify the properties for the tag \"{_tag.Name}\".\n" + warningReason;
            }

            return isValid;
        }

        private void OnTagItemChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Name":
                    if (_name != _tag.Name)
                    {
                        _name = _tag.Name;
                        RaisePropertyChanged(nameof(Name));
                    }

                    break;

                case "Description":
                    if (_description != _tag.Description)
                    {
                        _description = _tag.Description;
                        RaisePropertyChanged(nameof(Description));
                    }

                    break;

                case "DataType":
                    if (_dataType != _tag.DataTypeInfo.DataType.ToString())
                    {
                        _dataType = _tag.DataTypeInfo.DataType.ToString();
                        RaisePropertyChanged(nameof(DataType));
                    }

                    break;

                case "Usage":
                    if (_usage != _tag.Usage)
                    {
                        _usage = _tag.Usage;
                        RaisePropertyChanged(nameof(Usage));
                    }

                    break;

                case "TagType":
                    if (_type != _tag.TagType)
                    {
                        _type = _tag.TagType;
                        RaisePropertyChanged(nameof(Type));
                    }

                    break;

                case "ExternalAccess":
                    if (_externalAccess != _tag.ExternalAccess)
                    {
                        _externalAccess = _tag.ExternalAccess;
                        RaisePropertyChanged(nameof(ExternalAccess));
                    }

                    break;

                case "DisplayStyle":
                    if (_style != _tag.DisplayStyle)
                    {
                        _style = _tag.DisplayStyle;
                        RaisePropertyChanged(nameof(Style));
                    }

                    break;

                case "IsConstant":
                    if (_isConstant != _tag.IsConstant)
                    {
                        _isConstant = _tag.IsConstant;
                        RaisePropertyChanged(nameof(IsConstant));
                    }

                    break;
            }
        }

        private bool _isDirty;

        public bool IsDirty
        {
            get { return _isDirty; }
            private set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;

                    RaisePropertyChanged();

                    IsDirtyChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        public event EventHandler IsDirtyChanged;

        private void UpdateIsDirty()
        {
            if (!string.Equals(_name, _tag.Name))
            {
                IsDirty = true;
                return;
            }

            if (!string.Equals(_description, _tag.Description))
            {
                IsDirty = true;
                return;
            }

            if (_tag.ParentCollection.ParentProgram != null && _usage != _tag.Usage)
            {
                IsDirty = true;
                return;
            }

            if (_type != _tag.TagType)
            {
                IsDirty = true;
                return;
            }

            if (_dataType != _tag.DataTypeInfo.ToString())
            {
                IsDirty = true;
                return;
            }

            if (_externalAccess != _tag.ExternalAccess)
            {
                IsDirty = true;
                return;
            }

            if (_style != _tag.DisplayStyle)
            {
                if (!(_tag.DisplayStyle == DisplayStyle.NullStyle && _style == null))
                {
                    IsDirty = true;
                    return;
                }
            }

            if (IsConstant != _tag.IsConstant)
            {
                IsDirty = true;
                return;
            }

            IsDirty = false;
        }

        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public void UpdateUI()
        {
            DataTypeCommand.RaiseCanExecuteChanged();

            RaisePropertyChanged("NameIsEnabled");
            RaisePropertyChanged("IsDescriptionEnabled");
            RaisePropertyChanged("DataTypeIsEnabled");
            RaisePropertyChanged("TypeEnabled");
            RaisePropertyChanged("ExternalAccessEnabled");
            RaisePropertyChanged("StyleEnabled");
            RaisePropertyChanged("ConstantEnabled");
        }
    }
}

