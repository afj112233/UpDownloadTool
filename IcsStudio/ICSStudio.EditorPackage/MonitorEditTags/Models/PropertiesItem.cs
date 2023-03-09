using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Annotations;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Tags;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Dialogs.SelectDataType;
using MessageBox = System.Windows.MessageBox;
using System.Collections.Generic;
using System.Windows;
using GalaSoft.MvvmLight;
using ICSStudio.Dialogs.Warning;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.PredefinedType;
using Application = System.Windows.Application;

namespace ICSStudio.EditorPackage.MonitorEditTags.Models
{
    [CategoryOrder("General", 1)]
    [CategoryOrder("Data ", 2)]
    [CategoryOrder("Produced Connection", 3)]
    [CategoryOrder("Consumed Connection", 4)]
    [CategoryOrder("Parameter Connections{0:0}", 5)]
    [DisplayNameExtension("Properties")]

    public class DisplayNameExtension :  DisplayNameAttribute, INotifyPropertyChanged
    {
        private string _displayName;

        public DisplayNameExtension(string dispalyName) : base(dispalyName)
        {
            _displayName = dispalyName;
        }

        public override string DisplayName
        {
            get { return LanguageManager.GetInstance().ConvertSpecifier(_displayName); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class DescriptionExtension : DescriptionAttribute, INotifyPropertyChanged
    {
        private string _description;

        public DescriptionExtension(string description) : base(description)
        {
            _description = description;
        }

        public override string Description
        {
            get
            {
                return LanguageManager.GetInstance().ConvertSpecifier(_description);
            }
        }
       
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    internal partial class PropertiesItem : INotifyPropertyChanged
    {
        private string _itemName;
        private readonly IController _controller;
        private string _description;
        private readonly TagItem _tagItem;
        private TagType _type;
        private ExternalAccess _externalAccess;
        private string _dataType;
        private string _scope;
        private PropertiesYesNo _constant;
        private DisplayStyle _style;
        private readonly int _selectedIndex;
        private PropertiesYesNo _required;
        private PropertiesYesNo _visible;
        private object _value;
        private object _usage;

        public PropertiesItem(TagItem tagItem, int selectedIndex)
        {
            Contract.Assert(tagItem?.Tag != null);

            _tagItem = tagItem;

            _itemName = tagItem.Name;

            _selectedIndex = selectedIndex;

            _description = tagItem.Description;

            _style = tagItem.DisplayStyle;

            _value = GetValue();

            _type = tagItem.Tag.TagType;

            _dataType = tagItem.DataType;

            _externalAccess = tagItem.Tag.ExternalAccess;

            _controller = Controller.GetInstance();

            if (tagItem.ParentCollection.Scope is IController)
            {
                _usage = "<controller>";
            }
            else
            {
                _usage = tagItem.Usage;
            }

            foreach (var item in _controller.DataTypes)
            {
                if (item.Name.StartsWith("BOOL:", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (item.Name.Contains("$"))
                    continue;

                AllDataTypeNames.Add(item.Name);
            }

            //DataTypeIsEnabled = CheckDataType();
            SelectDataTypeCommand = new RelayCommand(ExecuteSelectDataTypeCommand);
            var controllerScope = (tagItem.ParentCollection.Scope as IController)?.Name;
            AssignmentScope(controllerScope);
            var aoiScope = (tagItem.ParentCollection.Scope as AoiDefinition)?.Name;
            AssignmentScope(aoiScope);
            var programScope = (tagItem.ParentCollection.Scope as Program)?.Name;
            AssignmentScope(programScope);

            Constant = tagItem.IsConstant ? PropertiesYesNo.Yes : PropertiesYesNo.No;

            Required = tagItem.Tag.IsRequired ? PropertiesYesNo.Yes : PropertiesYesNo.No;

            Visible = tagItem.Tag.IsVisible ? PropertiesYesNo.Yes : PropertiesYesNo.No;

            PropertyChangedEventManager.AddHandler(tagItem, OnTagItemChanged, "");
            PropertyChangedEventManager.AddHandler(tagItem.Tag, OnTagChanged, "");

            UpdateReadOnly();
        }

        [Category("General"),
         DisplayNameExtension("Name"),
         ReadOnly(true),
         PropertyOrder(0),
         DescriptionExtension("Specifies the name of the tag")]
        public string ItemName
        {
            set { _itemName = ChangeItemName(_itemName, value); }
            get { return _itemName; }
        }

        [Category("General"),
         DisplayNameExtension("Description"),
         ReadOnly(false),
         DescriptionExtension("Specifies up to 512 characters of the tag"),
         PropertyOrder(1)]
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    _tagItem.Tag.Description = _description;
                }
            }
        }

        [Category("General"),
         DisplayNameExtension("Usage"),
         ReadOnly(true),
         DescriptionExtension("Specifies the usage of the tag"),
         PropertyOrder(2),
         TypeConverter(typeof(UsageConverter))]
        public object Usage
        {
            get { return _usage; }
            set
            {
                if (_usage != value)
                {
                    if (_tagItem.ParentCollection.Scope is IController)
                    {
                        _usage = "<controller>";
                    }
                    else
                    {
                        _usage = (Usage) value;
                        _tagItem.Tag.Usage = (Usage) _usage;
                    }
                }
            }
        }

        [Category("General"),
         DisplayNameExtension("monitor/edit tag.Type"),
         ReadOnly(true),
         DescriptionExtension("Specifies whether the tag is an alias, base, produced or consumed tag"),
         PropertyOrder(3),
         TypeConverter(typeof(TagTypeConverter))]
        public TagType Type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    _tagItem.Tag.TagType = _type;
                }
            }
        }

        [Category("General"),
         DisplayNameExtension("Alias For"),
         ReadOnly(false),
         DescriptionExtension("Specifies what the tag is an alias for"),
         PropertyOrder(4)]
        public string AliasFor { get; set; }

        [Category("General"),
         DisplayNameExtension("Base Tag"),
         ReadOnly(true),
         DescriptionExtension("Specifies the ultimate base tag for the alias tag"),
         PropertyOrder(5)]
        public string BaseTag { get; set; }

        [Category("General"),
         DisplayNameExtension("Data Type"),
         ReadOnly(false),
         DescriptionExtension("Specifies the data type of the tag"),
         PropertyOrder(6)]
        public string DataType
        {
            get { return _dataType; }
            set
            {
                if (_dataType != value)
                {

                    if (IsValidDataType(value))
                    {
                        Tag tag = _tagItem.Tag;

                        ChangeTagDataType(tag, value);

                        _dataType = value;
                        OnPropertyChanged();

                        _style = tag.DisplayStyle;
                        OnPropertyChanged(nameof(Style));
                    }

                    //string tempDataType = _dataType;
                    //_dataType = value;

                    //if (IsValidDataType(_dataType))
                    //{
                    //    SetTagDataType();
                    //    OnPropertyChanged(nameof(Style));
                    //    OnPropertyChanged(nameof(DataType));
                    //}
                    //else
                    //{
                    //    _dataType = tempDataType;
                    //}
                }
            }
        }



        [Category("General"),
         DisplayNameExtension("Scope"),
         ReadOnly(true),
         DescriptionExtension("Specifies the container and thus accessibility of the tag"),
         PropertyOrder(7)]
        public string Scope
        {
            get { return _scope; }
            set
            {
                if (_scope != value)
                {
                    _scope = value;
                }
            }
        }

        [Category("General"),
         DisplayNameExtension("External Access"),
         ReadOnly(true),
         DescriptionExtension("Specifies how the tag can be viewed and changed from application connected to the controller"),
         PropertyOrder(8)]
        public ExternalAccess ExternalAccess
        {
            get { return _externalAccess; }
            set
            {
                if (_externalAccess != value)
                {
                    _externalAccess = value;
                    _tagItem.Tag.ExternalAccess = _externalAccess;
                }
            }
        }

        [Category("General"),
         DisplayNameExtension("Style"),
         ReadOnly(false),
         DescriptionExtension("Specifies the display format for the value of the tag"),
         PropertyOrder(9),
         TypeConverter(typeof(StyleConverter))]
        public DisplayStyle Style
        {
            //TODO(TLM):Style不应该随着MonitorTag的Style而变化
            get { return _style; }
            set
            {
                if (_style != value)
                {
                    _style = value;
                    _tagItem.DisplayStyle = _style;
                }
            }
        }

        [Category("General"),
         DisplayNameExtension("Constant"),
         ReadOnly(false),
         DescriptionExtension("Specifies that the value of the tag can be changed"),
         PropertyOrder(10)]
        public PropertiesYesNo Constant
        {
            get { return _constant; }
            set
            {
                if (_constant != value)
                {
                    _constant = value;
                    switch (_constant)
                    {
                        case PropertiesYesNo.Yes:
                            _tagItem.Tag.IsConstant = true;
                            break;
                        case PropertiesYesNo.No:
                            _tagItem.Tag.IsConstant = false;
                            break;
                    }
                }
            }
        }

        public bool IsPropertiesConstantVisibility
        {
            get
            {
                if (_tagItem.Tag != null)
                    return _tagItem.Tag.IsConstant;
                return false;
            }
        }

        [Category("General"),
         DisplayNameExtension("monitor/edit tag.Required"),
         ReadOnly(true),
         DescriptionExtension("Specifies that the parameter requires an argument."),
         PropertyOrder(11)]
        public PropertiesYesNo Required
        {
            get { return _required; }
            set
            {
                _required = value;
                if (_required == PropertiesYesNo.Yes)
                {
                    _tagItem.Tag.IsRequired = true;
                    Visible = PropertiesYesNo.Yes;
                }

                if (_required == PropertiesYesNo.No)
                {
                    _tagItem.Tag.IsRequired = false;
                }
            }
        }

        [Category("General"),
         DisplayNameExtension("monitor/edit tag.Visible"),
         ReadOnly(true),
         DescriptionExtension("Specifies that the parameter is visible."),
         PropertyOrder(12)]
        public PropertiesYesNo Visible
        {
            get { return _visible; }
            set
            {
                if (_visible != value)
                {
                    if (Required == PropertiesYesNo.Yes && value == PropertiesYesNo.No)
                    {
                        _visible = PropertiesYesNo.Yes;
                        MessageBox.Show($"Failed to modify the properties for the tag \"{ItemName}\".\n" +
                                        "Sequence of operations not allowed.");

                    }
                    else
                    {
                        _visible = Required == PropertiesYesNo.Yes ? PropertiesYesNo.Yes : value;

                        if (_visible == PropertiesYesNo.Yes)
                        {
                            _tagItem.Tag.IsVisible = true;
                        }

                        if (_required == PropertiesYesNo.No)
                        {
                            _tagItem.Tag.IsVisible = false;
                        }
                    }
                }
            }
        }

        [Category("Data "),
         ReadOnly(true),
         DisplayNameExtension("Value"),
         DescriptionExtension("Specifies the value of the tag"),
         PropertyOrder(13)]
        public object Value
        {
            get { return GetValue(); }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    SetTagValue(_value);
                }
            }
        }

        [Category("Data "), DisplayNameExtension("Force Mask"),
         DescriptionExtension(
             "Specifies what bits within the value are forced;'1'indicates forced on; '0'indicates forces off; '.'indicates not forced"),
         PropertyOrder(14)]
        public string ForceMask { get; set; }

        [Category("Produced Connection"), DisplayNameExtension("Max Consumers"),
         DescriptionExtension("Specifies the Max Consumers of tags that can consume this tag"), PropertyOrder(15)]
        public string StationMaxConsumersName { get; set; }

        [Category("Produced Connection"), DisplayNameExtension("Send Data State Change"),
         DescriptionExtension("Specifies to use IOT instruction to send event trigger information to consumers of the tag"),
         PropertyOrder(16)]
        public string SendDataStateChange { get; set; }

        [Category("Produced Connection"), DisplayNameExtension("Unicast Connections"),
         DescriptionExtension("Specifies to enable multiple unicast consumers to consume from the tag"), PropertyOrder(17)]
        public string UnicastConnections { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Produced Connection"), DisplayNameExtension("Multicast"), PropertyOrder(18)]
        public string Multicast { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Produced Connection"), DisplayNameExtension("Connection Status"), PropertyOrder(19)]
        public string ConnectionStatus { get; set; }

        //[Category("ConnectionStatus"), DisplayName("Included"), Description("Indicates if connection status is included in the tag")]
        //public string Included
        //{
        //    get;
        //    set;
        //}

        [Category("Consumed Connection"), DisplayNameExtension("Producer"),
         DescriptionExtension("Specifies a remote device containing a data to consume"), PropertyOrder(20)]
        public double Producer { get; set; }

        [Category("Parameter Connections{0:0}"), DisplayNameExtension(" "),
         DescriptionExtension("Cell used to create onr additional Parameter Connection for the tag"), PropertyOrder(21)]
        public double ParameterConnections { get; set; }

        //[Category("Multicast"), DisplayName("Minimum RPI(ms)"), Description("Specifies the minimum RPI that a consumer tag of the tag may request")]
        //public string MinimumRPI
        //{
        //    get;
        //    set;
        //}

        //[Category("Multicast"), DisplayName("Maximum RPI(ms)"), Description("Specifies the maximum RPI that a consumer tag of the tag may request")]
        //public string MaximumRPI
        //{
        //    get;
        //    set;
        //}

        //[Category("Multicast"), DisplayName("Use Default"), Description("Specifies to provide an RPI to an out of range consumer of the tag")]
        //public string UseDefault
        //{
        //    get;
        //    set;
        //}

        //[Category("Multicast"), DisplayName("Default RPI(ms)"), Description("Specifies the RPI to use for an out of range consumer of the tag")]
        //public string DefaultRPI
        //{
        //    get;
        //    set;
        //}


        private void AssignmentScope(string scope)
        {
            if (scope != null)
            {
                Scope = scope;
            }
        }

        public enum PropertiesYesNo
        {
            Yes,
            No
        }

        private void ChangeTagDataType(Tag tag, string newDataType)
        {
            var newDataTypeInfo = _controller.DataTypes.ParseDataTypeInfo(newDataType);

            var oldDataWrapper = tag.DataWrapper;

            // For BOOL
            if (newDataTypeInfo.DataType.IsBool
                && newDataTypeInfo.Dim1 > 0 && newDataTypeInfo.Dim2 == 0 && newDataTypeInfo.Dim3 == 0)
                if (newDataTypeInfo.Dim1 % 32 != 0)
                    newDataTypeInfo.Dim1 = (newDataTypeInfo.Dim1 / 32 + 1) * 32;

            var newDataWrapper = TagsFactory.CreateDataWrapper(
                _controller,
                newDataTypeInfo.DataType,
                newDataTypeInfo.Dim1, newDataTypeInfo.Dim2, newDataTypeInfo.Dim3);

            tag.UpdateDataWrapper(newDataWrapper,GetDefaultDisplayStyle(newDataTypeInfo.DataType));

            // post change 
            if (oldDataWrapper is MotionGroup)
                foreach (var item in _controller.Tags)
                {
                    var axis = tag as Tag;
                    var axisCIPDrive = axis?.DataWrapper as AxisCIPDrive;
                    if (axisCIPDrive != null && axisCIPDrive.AssignedGroup == tag) axisCIPDrive.AssignedGroup = null;

                    var axisVirtual = axis?.DataWrapper as AxisVirtual;
                    if (axisVirtual != null && axisVirtual.AssignedGroup == tag) axisVirtual.AssignedGroup = null;
                }

            // ReSharper disable once MergeCastWithTypeCheck
            if (oldDataWrapper is AxisCIPDrive)
            {
                var axisCIPDrive = (AxisCIPDrive) oldDataWrapper;
                var cipMotionDrive = axisCIPDrive.AssociatedModule as CIPMotionDrive;
                cipMotionDrive?.RemoveAxis(tag, axisCIPDrive.AxisNumber);
            }
            
        }

        private DisplayStyle GetDefaultDisplayStyle(IDataType dataType)
        {
            var defaultDisplayStyle = DisplayStyle.NullStyle;

            if (dataType is BOOL || dataType is SINT || dataType is INT || dataType is DINT || dataType is LINT)
                defaultDisplayStyle = DisplayStyle.Decimal;
            else if (dataType is REAL) defaultDisplayStyle = DisplayStyle.Float;

            return defaultDisplayStyle;
        }

        [Browsable(false)] public RelayCommand SelectDataTypeCommand { get; }

        private void ExecuteSelectDataTypeCommand()
        {
            bool supportsOneDimensionalArray = true;
            bool supportsMultiDimensionalArrays = true;

            if (IsInAOI)
            {
                Usage usage = (Usage) _usage;

                switch (usage)
                {
                    case Interfaces.Tags.Usage.Input:
                    case Interfaces.Tags.Usage.Output:
                        supportsOneDimensionalArray = false;
                        supportsMultiDimensionalArrays = false;
                        break;

                    case Interfaces.Tags.Usage.Local:
                        supportsMultiDimensionalArrays = false;
                        break;
                }
            }

            var selectDataTypeDialog = new SelectDataTypeDialog(
                _controller, DataType, supportsOneDimensionalArray, supportsMultiDimensionalArrays)
            {
                Owner = Application.Current.MainWindow
            };

            var dialogResult = selectDataTypeDialog.ShowDialog();
            if (dialogResult != null && dialogResult.Value)
            {
                DataType = selectDataTypeDialog.DataType;
            }
        }

        private bool IsInAOI => _tagItem.ParentCollection.Scope is AoiDefinition;

        [Browsable(false), ReadOnly(true)] public List<string> AllDataTypeNames { get; } = new List<string>();

        private bool IsValidDataType(string dataType)
        {
            string warningMessage = "Failed to modify the properties for the tag '" + ItemName + "'.";
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
                    if (foundDataType.IsAxisType
                        || foundDataType.IsMotionGroupType
                        || foundDataType.IsCoordinateSystemType)
                    {
                        isValid = false;
                        warningReason = "Tag can only be created at the controller scope.";
                    }
                }

                if (isValid)
                {
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
                var warningDialog = new WarningDialog(
                        warningMessage, warningReason)
                    {Owner = Application.Current.MainWindow};
                warningDialog.ShowDialog();
            }

            return isValid;
        }

        class StyleConverter : TypeConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                var item = context.Instance as PropertiesItem;

                switch (item.DataType)
                {
                    case "REAL":
                        return new StandardValuesCollection(new[]
                        {
                            DisplayStyle.Float,
                            DisplayStyle.Exponential
                        });
                    case "BOOL":
                        return new StandardValuesCollection(new[]
                        {
                            DisplayStyle.Binary,
                            DisplayStyle.Octal,
                            DisplayStyle.Decimal,
                            DisplayStyle.Hex
                        });
                    case "DINT":
                    case "SINT":
                    case "INT":
                        return new StandardValuesCollection(new[]
                        {
                            DisplayStyle.Binary,
                            DisplayStyle.Octal,
                            DisplayStyle.Decimal,
                            DisplayStyle.Hex,
                            DisplayStyle.Ascii
                        });
                    default:
                        return new StandardValuesCollection(new[]
                        {
                            DisplayStyle.Binary, DisplayStyle.Octal, DisplayStyle.Decimal, DisplayStyle.Hex,
                            DisplayStyle.Ascii
                        });
                }
            }
        }

        class TagTypeConverter : TypeConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                //暂时不支持TagType.Alias,在线或离线不可创建TagType.Produced, TagType.Consumed会导致upload失败
                return new StandardValuesCollection(new[]{TagType.Base});
            }
        }

        class UsageConverter : TypeConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {

                var item = context.Instance as PropertiesItem;

                //TODO(gjc): need check here
                if (item.Scope == "MainProgram")
                {
                    return new StandardValuesCollection(new[]
                    {
                        Interfaces.Tags.Usage.Local, Interfaces.Tags.Usage.Input, Interfaces.Tags.Usage.Output,
                        Interfaces.Tags.Usage.InOut, Interfaces.Tags.Usage.SharedData
                    });
                }

                return new StandardValuesCollection(new[]
                {
                    Interfaces.Tags.Usage.Local, Interfaces.Tags.Usage.Input, Interfaces.Tags.Usage.Output,
                    Interfaces.Tags.Usage.InOut
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
