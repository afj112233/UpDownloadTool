using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Components.Controls;
using ICSStudio.Gui.View;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Annotations;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using Newtonsoft.Json.Linq;
using ICSStudio.Dialogs.Filter;

namespace ICSStudio.UIServicesPackage.AddOnInstruction.Panel
{
    public abstract class BaseTagRow : INotifyPropertyChanged
    {
        private string _name;
        private string _oldName;
        private string _description;
        private IDataType _dataTypeL;
        private DisplayStyle? _style;
        private ObservableCollection<int> _b = new ObservableCollection<int>();
        private Visibility _expanderCloseVis = Visibility.Collapsed;
        private Visibility _expanderVis = Visibility.Hidden;
        protected readonly IController _controller;
        private bool _req;
        private bool _vis;
        private bool _visEnable = true;
        private ExternalAccess? _externalAccess;
        private bool _constant;
        private List<DisplayStyle?> _displayStyleList = new List<DisplayStyle?>();
        private int _defaultValueGridHeight;
        private bool _defaultEnabled = true;
        protected bool IsChildChanged;
        protected bool IsBChanged;
        protected bool Flag;
        protected int Size =>DataTypeL.BitSize;
        protected bool IsInitial;
        protected bool NoActionChange { set; get; } = false;
        private Visibility _browseStringVisibility = Visibility.Collapsed;
        private bool _descriptionEnable = true;
        protected string _dataType;

        protected BaseTagRow(IAoiDefinition aoiDefinition, ITag tag,BaseTagRow parent)
        {
            TmpTag=new TmpTag(tag,aoiDefinition);
            Parent = parent;
            DescriptionEnable = true;
            ExpanderCommand = new RelayCommand(ExecuteExpanderCommand);
            ExpanderCloseCommand = new RelayCommand(ExecuteExpanderCloseCommand);
            DefaultCommand = new RelayCommand<Button>(ExecuteDefaultCommand);
            LostFocusCommand = new RelayCommand<ValidateNameControl>(ExecuteLostFocusCommand);
            InputCommand = new RelayCommand<TextCompositionEventArgs>(ExecuteInputCommand);
            ParentAddOnInstruction = (AoiDefinition)aoiDefinition;
            _controller = aoiDefinition.ParentController;
            for (int i = 0; i < 64; i++)
            {
                B.Add(0);
            }

            AddListener();
            IsDirty = false;
            IsStructChanged = false;
        }

        public bool IsBlank()
        {
            return string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(DataType) && Style == null;
        }

        public BaseTagRow GetBaseTagRow()
        {
            var parent = Parent;
            while (parent!=null)
            {
                parent = parent.Parent;
            }
            Debug.Assert(Parent.IsBase);
            return Parent;
        }

        public void SyncField()
        {
            if(Usage==Interfaces.Tags.Usage.InOut)return;
            var tag = Tag as Tag;
            if (tag?.DataWrapper?.Data != null)
            {
                tag.Description = _description;
                tag.DataWrapper.Data = Field?.DeepCopy();
            }
        }

        public bool IsDirty
        {
            set
            {
                if (_isDirty == value)   return;
                _isDirty = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDirty)));
            }
            get { return _isDirty; }
        }

        public bool IsStructChanged { set; get; }

        public TmpTag TmpTag { get; }
        
        public BaseTagRow Parent { get; }

        public void AddListener()
        {
            if (TmpTag.Tag != null && IsMember)
            {
                PropertyChangedEventManager.AddHandler(TmpTag.Tag, Tag_PropertyChanged, string.Empty);
            }
            CollectionChangedEventManager.AddHandler(B, OnCollectionChanged);
        }

        public void RemoveListener()
        {
            if (TmpTag.Tag != null && IsMember)
            {
                PropertyChangedEventManager.RemoveHandler(TmpTag.Tag, Tag_PropertyChanged, string.Empty);
            }
            CollectionChangedEventManager.RemoveHandler(B, OnCollectionChanged);
        }

        private void VerityName()
        {
            if(!IsMember)return;
            var error1 = "A Parameter or Local Tag by the same name already exists in this Add-On Instruction Definition.";
            var error2 = "Name is invalid.";
            var isExist = Parent.Child.Any(c => Name.Equals(c.Name) && c != this);
            if (isExist)
            {
                ErrorMessage = error1;
                ErrorReason = "Failed to save changes.";
                return;
            }
            Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
           
            if (!regex.IsMatch(Name))
            {
                ErrorMessage =error2;
                ErrorReason = $"Failed to set tag \"{Tag.Name}\" name.";
                return;
            }

            if (error1.Equals(ErrorMessage)||error2.Equals(ErrorMessage))
            {
                ErrorMessage = "";
                ErrorReason = "";
            }
        }

        private void Tag_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var tag = (Tag)sender;
            if ( e.PropertyName.StartsWith(tag.Name, StringComparison.OrdinalIgnoreCase))
            {
                Field = tag.DataWrapper.Data;
                if (tag.DataWrapper.Data.ToJToken() is JValue)
                {
                    Default = FormatOp.ConvertField(tag.DataWrapper.Data, tag.DisplayStyle);
                }
                else
                {
                    ExecuteExpanderCloseCommand();
                    Child.Clear();
                }
            }

            if (e.PropertyName == "Name")
            {
                Name = TmpTag.Tag.Name;
            }

            if (e.PropertyName == "Usage")
            {
                Usage = TmpTag.Tag.Usage;
            }

            if (e.PropertyName == "DataWrapper")
            {
                DataType = TmpTag.Tag.DataTypeInfo.ToString();
            }

            if (e.PropertyName == "Description")
            {
                Description = TmpTag.Tag.Description;
            }

            if (e.PropertyName == "ExternalAccess")
            {
                ExternalAccess = TmpTag.Tag.ExternalAccess;
            }

            if (e.PropertyName == "DisplayStyle")
            {
                Style = TmpTag.Tag.DisplayStyle;
            }

            if (e.PropertyName == "IsRequired")
            {
                Req = TmpTag.Tag.IsRequired;
            }

            if (e.PropertyName == "IsVisible")
            {
                Vis = TmpTag.Tag.IsVisible;
            }
            if (e.PropertyName == "IsConstant")
            {
                Constant = TmpTag.Tag.IsConstant;
            }
        }

        public Visibility BrowseStringVisibility
        {
            set
            {
                _browseStringVisibility = value;
                OnPropertyChanged();
            }
            get { return _browseStringVisibility; }
        }

        public bool DescriptionEnable
        {
            set { _descriptionEnable = value; }
            get { return _descriptionEnable; }
        }

        public ITag Tag => TmpTag.Tag;
        public IController Controller => _controller;

        public ItemCollection Child
        {
            set { _child = value; }
            get{return _child;}
        }

        public string OldName => _oldName;
        
        public JArray ChildDescription { set; get; }=new JArray();
        public AoiDefinition ParentAddOnInstruction { set; get; }
        public bool IsBase { set; get; } = false;
        public Visibility ButtonVisibility { set; get; }
        public bool IsMember { set; get; } = false;
        public bool NameEnabled { set; get; } = true;
        public bool DataTypeEnabled { set; get; } = true;
        public bool StyleEnabled { set; get; } = true;
        public bool IsBitSel { set; get; }
        public int FieldIndex { set; get; }
        public int BitOffset { set; get; }
        public Thickness Left { set; get; } = new Thickness(0, 0, 0, 0);
        public bool IsCorrect { set; get; } = true;
        public string ErrorMessage { set; get; }
        public string ErrorReason { set; get; }
        public int SortMemberIndex { protected set; get; }
        public int SortBitOffset { protected set; get; }
        public int SortDimIndex { protected set; get; }
        
        public virtual string Name
        {
            set
            {
                if (_name != value)
                {
                    _oldName = _name;
                    _name = value;
                    VerityName();
                    if (IsMember)
                        TmpTag.TmpName = value;
                    OnPropertyChanged();
                }
            }
            get { return _name; }
        }

        public virtual string Description
        {
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
            get
            {
                return GetChildDescription(Name);
            }
        }

        public virtual string DataType { set; get; }
        public virtual Usage? Usage { set; get; }

        public virtual bool IsOnline { get; set; }

        protected bool NoFormat { set; get; }

        protected string OldDefaultValue { set; get; }

        public DisplayStyle? Style
        {
            set
            {
                if (_style != value)
                {
                    if (IsInCreate||NoActionChange)
                    {
                        _style = value;
                        OnPropertyChanged();
                        return;
                    }

                    NoActionChange = true;
                    DisplayStyle old = _style ?? DisplayStyle.Decimal;
                    DisplayStyle newValue = value ?? DisplayStyle.Decimal;
                    var defaultValue = OldDefaultValue??Default;
                    _style = value;
                    if (DataTypeL?.FamilyType != FamilyType.StringFamily)
                        if (!string.IsNullOrEmpty(defaultValue) &&
                            (value == DisplayStyle.Float || value == DisplayStyle.Exponential))
                        {
                            string temp = defaultValue;
                            if (!(old == DisplayStyle.Float || old == DisplayStyle.Exponential))
                            {
                                temp = ValueConverter.ConvertValue(defaultValue, old, DisplayStyle.Decimal,
                                    Controller.DataTypes[DataType].BitSize,
                                    ValueConverter.SelectIntType(DataTypeL));
                            }

                            if (newValue == DisplayStyle.Float)
                            {
                                Default = (float.Parse(temp)).ToString("F");
                            }
                            else if (newValue == DisplayStyle.Exponential)
                            {
                                Default = (float.Parse(temp)).ToString("E");
                            }
                            else
                            {
                                Default = FormatOp.FormatValue(temp, (DisplayStyle)value, DataTypeL);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(defaultValue) && DataType?.IndexOf("[") < 0)
                                Default = ValueConverter.ConvertValue(defaultValue, old, newValue,
                                    Controller.DataTypes[DataType].BitSize,
                                    ValueConverter.SelectIntType(DataTypeL));
                        }

                    OnPropertyChanged();
                    OnPropertyChanged("StyleDisplay");
                    OnPropertyChanged("Default");
                    SetChildStyle();
                    NoActionChange = false;
                    //SynchronizeChildStyle();
                }
            }
            get { return _style; }
        }

        public bool Req
        {
            set
            {
                if (_req != value)
                {
                    _req = value;
                    if (_req)
                        Vis = true;
                    VisEnabled = !value;
                    OnPropertyChanged();
                }
            }
            get { return _req; }
        }

        public bool Vis
        {
            set
            {
                if (_vis != value)
                {
                    _vis = value;
                    OnPropertyChanged();
                }
            }
            get { return _vis; }
        }

        public bool VisEnabled
        {
            set
            {
                if (_visEnable != value)
                {
                    _visEnable = value;
                    OnPropertyChanged();
                }
            }
            get
            {
                if (IsOnline) return false;
                return _visEnable;
            }
        }

        public ExternalAccess? ExternalAccess
        {
            set
            {
                if (_externalAccess != value)
                {
                    _externalAccess = value;
                    OnPropertyChanged();
                    OnPropertyChanged("ExternalAccessDisplay");
                }
            }
            get { return _externalAccess; }
        }

        public bool Constant
        {
            set
            {
                if (_constant != value)
                {
                    _constant = value;
                    OnPropertyChanged();
                }
            }
            get { return _constant; }
        }

        public List<DisplayStyle?> DisplayStyleList
        {
            set
            {
                if (_displayStyleList != value)
                {
                    _displayStyleList = value;
                    OnPropertyChanged();
                }
            }
            get { return _displayStyleList; }
        }

        public Visibility ExpanderCloseVis
        {
            set
            {
                if (_expanderCloseVis != value)
                {
                    if (value == Visibility.Visible)
                    {
                        if (Child.Count == 0)
                        {
                            SetChild(this);
                        }
                    }
                    _expanderCloseVis = value;
                    OnPropertyChanged();
                }
            }
            get { return _expanderCloseVis; }
        }

        public Visibility ExpanderVis
        {
            set
            {
                if (ExpanderVis != value)
                {
                    _expanderVis = value;
                    OnPropertyChanged();
                }
            }
            get { return _expanderVis; }
        }

        public virtual string Default { set; get; }

        #region Command

        public RelayCommand ExpanderCommand { set; get; }

        public IField Field { set; get; }

        protected bool IsDataTypeInitialize = false;
        private ItemCollection _child = new ItemCollection();
        private bool _isDirty;

        public virtual IDataType DataTypeL
        {
            protected set
            {
                Flag = false;
                if (_dataTypeL != null)
                    PropertyChangedEventManager.RemoveHandler(_dataTypeL, DataTypePropertyChanged, string.Empty);
                //_dataTypeL.PropertyChanged -= DataTypePropertyChanged;
                _dataTypeL = value;
                if (_dataTypeL != null)
                    PropertyChangedEventManager.AddHandler(_dataTypeL, DataTypePropertyChanged, string.Empty);
                //_dataTypeL.PropertyChanged += DataTypePropertyChanged;
                if (_dataTypeL?.FamilyType == FamilyType.StringFamily&&!DataType.EndsWith("]"))
                {
                    ButtonVisibility = Visibility.Collapsed;
                    BrowseStringVisibility = Visibility.Visible;
                    IsChildChanged = true;
                    if (Field != null)
                    {
                        var data = (JArray)((JArray)Field.ToJToken())[1];
                        string str = "";
                        int len = (int)(JValue)((JArray)Field.ToJToken())[0];
                        foreach (JValue c in data)
                        {
                            if (len == 0) break;
                            var temp = Encoding.ASCII.GetString(new[] { ((byte)Convert.ToInt32(c.Value)) });
                            str = str + FormatOp.UnFormatSpecial(temp);
                            len--;
                        }

                        Default = $"'{str}'";
                    }

                    IsChildChanged = false;
                }

                IsDataTypeInitialize = true;
                ResetStyle();
                OldDefaultValue = null;
                IsDataTypeInitialize = false;
            }
            get { return _dataTypeL; }
        }

        private void ExecuteExpanderCommand()
        {
            ExpanderCloseVis = Visibility.Visible;
            ExpanderVis = Visibility.Collapsed;
        }

        public RelayCommand ExpanderCloseCommand { set; get; }

        private void ExecuteExpanderCloseCommand()
        {
            ExpanderCloseVis = Visibility.Collapsed;
            ExpanderVis = Visibility.Visible;
        }

        public RelayCommand<Button> DefaultCommand { set; get; }

        private void ExecuteDefaultCommand(Button sender)
        {
            var contextMenu = ((FrameworkElement) sender).ContextMenu;
            contextMenu.PlacementTarget = sender as Button;
            contextMenu.Placement = PlacementMode.Bottom;
            contextMenu.IsOpen = !contextMenu.IsOpen;
        }

        public RelayCommand<ValidateNameControl> LostFocusCommand { set; get; }

        private void ExecuteLostFocusCommand(ValidateNameControl sender)
        {
            var textBox = (TextBox) sender;
            var cell = VisualTreeHelpers.FindVisualParentOfType<DataGridCell>(textBox);
            if (cell == null) return;
            if (cell.IsEditing)
                cell.IsEditing = false;
        }

        public RelayCommand<TextCompositionEventArgs> InputCommand { set; get; }

        private void ExecuteInputCommand(TextCompositionEventArgs e)
        {
            var sender = e.Source;
            string oldValue = ((TextBox) sender).Text.Replace(e.Text, "");
            string newValue = e.Text;
            if (e.Text != "1" && e.Text != "0")
            {
                e.Handled = true;
                return;
            }

            ((TextBox) sender).Text = int.Parse(newValue) < 2 ? newValue : oldValue;
            if (int.Parse(newValue) >= 2)
            {
                e.Handled = true;
            }
        }

        #endregion

        public ObservableCollection<int> B
        {
            set
            {
                if (_b != value)
                {
                    _b = value;
                    OnPropertyChanged();
                }
            }
            get { return _b; }
        }

        public int DefaultValueGridHeight
        {
            set
            {
                if (_defaultValueGridHeight != value)
                {
                    _defaultValueGridHeight = value;
                    OnPropertyChanged();
                }
            }
            get { return _defaultValueGridHeight; }
        }

        public string StyleDisplay
        {
            get
            {
                if (Style == DisplayStyle.NullStyle || Style == null )
                    return "";
                else
                    return Style.ToString();
            }
        }

        public void ActivePropertyChangedEventHandler(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            NoActionChange = false;
        }

        public bool DefaultEnabled
        {
            set
            {
                _defaultEnabled = value;
                OnPropertyChanged();
            }
            get { return _defaultEnabled; }
        }

        public void OnChildPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.Child.Contains(sender))
            {
                var baseTagRow = sender as BaseTagRow;
                if(baseTagRow.NoActionChange)return;
                if (e.PropertyName == "ExpanderCloseVis")
                {
                    if (IsBase)
                    {
                        ClearBaseChild(baseTagRow);
                        int index = Child.IndexOf(baseTagRow) + 1;
                        SetBaseChild(ref index, baseTagRow);
                    }
                    else
                    {
                        OnPropertyChanged("ExpanderCloseVis");
                    }

                    return;
                }

                if (e.PropertyName == "Default" && DefaultEnabled && Name != null &&
                    Usage != Interfaces.Tags.Usage.InOut)
                {
                    string binary = "";
                    foreach (var item in Child)
                    {
                        binary = item.Default + binary;
                    }

                    if (!string.IsNullOrEmpty(binary))
                    {
                        IsChildChanged = true;
                        Default = ValueConverter.ConvertGenericBinaryToAny(binary, Style,
                            ValueConverter.SelectIntType(DataTypeL), DataTypeL.ByteSize);
                        IsChildChanged = false;
                    }
                }

                if ((e.PropertyName == "Dirty"|| e.PropertyName == "Default") && DataTypeL != null && DataTypeL.FamilyType == FamilyType.StringFamily&&!DataType.EndsWith("]"))
                {
                    if (Field == null)
                    {
                        IsChildChanged = false;
                        return;
                    }
                    IsChildChanged = true;
                    var data = Child[1];
                    string str = "";
                    int len = (int)(JValue)((JArray)Field.ToJToken())[0];
                    foreach (var tagRow in data.Child)
                    {
                        if(len==0)break;
                        str = str + tagRow.Default.Substring(0, tagRow.Default.Length - 1).Substring(1);
                        len--;
                    }

                    Default = $"'{str}'";
                    IsChildChanged = false;
                }

                if (!e.PropertyName.Equals("ExpanderVis"))
                    OnPropertyChanged("Dirty");
            }
        }

        public abstract void SetDefaultProperties();
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender == this.B)
            {
                ReSetDefaultValue();
            }
        }

        protected abstract void SetChild(BaseTagRow baseTagRow);

        protected void ReSetDefaultValue()
        {
            var b = B;
            string binary = "";
            for (int i = 0; i < b.Count; i++)
            {
                binary = b[i] + binary;
            }

            IsBChanged = true;
            Default = ValueConverter.ConvertGenericBinaryToAny(FixBinary(binary), Style,
                ValueConverter.SelectIntType(DataTypeL), DataTypeL.ByteSize);
            IsBChanged = false;
        }

        private string FixBinary(string value)
        {
            if (DataTypeL.IsLINT) return value;
            if (DataTypeL is DINT) return value.Substring(32);
            if (DataTypeL is INT) return value.Substring(32 + 16);
            if (DataTypeL is SINT) return value.Substring(32 + 16 + 8);
            if (DataTypeL.IsBool) return value.Substring(63);
            Debug.Assert(false);
            return value.Substring(31);
        }

        protected void SetB()
        {
            try
            {
                if (ButtonVisibility == Visibility.Visible && DataTypeL.IsInteger)
                {
                    DisplayStyle old = Style ?? DisplayStyle.Decimal;
                    string binary = string.IsNullOrEmpty(Default)
                        ? ""
                        : ValueConverter.ConvertValue(Default, old, DisplayStyle.Binary, DataTypeL.BitSize,
                            ValueConverter.SelectIntType(DataTypeL));
                    binary = binary.PadLeft(DataTypeL.BitSize, '0');
                    for (int i = 0; i < binary.Length; i++)
                    {
                        B[binary.Length - i - 1] = int.Parse(binary.Substring(i, 1));
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Assert(false,e.ToString());
            }
        }

        protected void ResetChildDefault()
        {
            try
            {
                if (Child.Count > 0)
                {
                    if (DataType.Equals("SINT", StringComparison.OrdinalIgnoreCase) ||
                        DataType.Equals("INT", StringComparison.OrdinalIgnoreCase) || 
                        DataType.Equals("LINT", StringComparison.OrdinalIgnoreCase) ||
                        DataType.Equals("DINT", StringComparison.OrdinalIgnoreCase))
                    {
                        var b = B;
                        for (int i = 0; i < b.Count; i++)
                        {
                            if (Size > i)
                                Child[i].Default = b[i].ToString();
                        }
                    }
                    else if (DataTypeL.FamilyType == FamilyType.StringFamily)
                    {
                        var len = Child[0];
                        var v = FormatOp.FormatSpecial(Default.Substring(0, Default.Length - 1).Substring(1));
                        len.Default = v.Length.ToString();

                        var data = Child[1];
                        // var field = (Field as ICompositeField)?.fields[1].Item1.ToJToken() as JArray;
                        if (data.Child.Count > 0)
                        {
                            for (int i = 0; i < v.Length; i++)
                            {
                                var child = data.Child[i];
                                if (child.Style == DisplayStyle.Ascii)
                                {
                                    child.Default = FormatOp.UnFormatSpecial(v[i].ToString());
                                }
                                else
                                {

                                    child.Default = ValueConverter.ConvertValue(FormatOp.UnFormatSpecial(((sbyte)v[i]).ToString()), DisplayStyle.Ascii, (DisplayStyle)child.Style, 8, ValueConverter.SelectIntType(child.DataTypeL));
                                }
                            }
                        }
                        //for (int i = 0; i < field.Count; i++)
                        //{
                        //    var value = ((sbyte) (JValue) field[i]).ToString(DisplayStyle.Ascii);

                        //    value =FormatOp.FormatSpecial(value.Substring(0, value.Length - 1).Substring(1));


                        //    data.Child[i].Default = FormatOp.UnFormatSpecial(value);
                        //}

                        //if (field.Count < data.Child.Count)
                        //{
                        //    for (int i = field.Count; i < data.Child.Count; i++)
                        //    {
                        //        data.Child[i].Default = FormatOp.UnFormatSpecial("\0");
                        //    }
                        //}
                    }
                }
                else
                {
                    if (DataTypeL.FamilyType == FamilyType.StringFamily)
                    {
                        if(string.IsNullOrEmpty(Default))return;
                        var v = FormatOp.FormatSpecial(Default.Substring(0, Default.Length - 1).Substring(1));
                        if (Field is PreDefinedField)
                        {
                            var field = (PreDefinedField)Field;
                            ((Int32Field)field.fields[0].Item1).value = v.Length;
                            var data = (ArrayField)field.fields[1].Item1;
                            for (int i = 0; i < v.Length; i++)
                            {
                                if (i >= data.fields.Count) break;
                                var charField = (Int8Field)data.fields[i].Item1;
                                charField.value = (sbyte)v[i];
                            }
                        }
                        else if (Field is UserDefinedField)
                        {
                            var field = (UserDefinedField)Field;
                            ((Int32Field)field.fields[0].Item1).value = v.Length;
                            var data = (ArrayField)field.fields[1].Item1;
                            for (int i = 0; i < v.Length; i++)
                            {
                                if (i >= data.fields.Count) break;
                                var charField = (Int8Field)data.fields[i].Item1;
                                charField.value = (sbyte)v[i];
                            }
                        }
                        else
                        {
                            Debug.Assert(false, Field?.GetType().FullName ?? "");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Assert(false,e.ToString());
            }

        }

        protected void ChildOnMonitor()
        {
            foreach (var item in Child)
            {
                PropertyChangedEventManager.AddHandler(item, OnChildPropertyChanged, string.Empty);
                //item.PropertyChanged += OnChildPropertyChanged;
            }
        }

        protected void ChildOffMonitor()
        {
            foreach (var item in Child)
            {
                PropertyChangedEventManager.RemoveHandler(item, OnChildPropertyChanged, String.Empty);
                //item.PropertyChanged -= OnChildPropertyChanged;
            }
        }

        public string GetChildDescription(string name)
        {
            string description = "";
            if (string.IsNullOrEmpty(name)) return "";
            var baseName = name;
            var array = ChildDescription as JArray;
            if (array != null)
            {
                if (name.IndexOf(".", StringComparison.Ordinal) > 0)
                {
                    name = name.Substring(name.IndexOf(".", StringComparison.Ordinal));
                    baseName = baseName.Substring(0, baseName.IndexOf("."));
                }

                var item = array.AsParallel()
                    .FirstOrDefault(t => SimpleServices.Tags.Tag.GetOperand(name).Equals((string)(t as JObject)?["Operand"]));
                if (item != null)
                {
                    return item["Value"]?.ToString();
                }
            }

            if (string.IsNullOrEmpty(description) && DataTypeL != null)
            {
                var topDesc = (string)(ChildDescription?.AsParallel().FirstOrDefault(d =>
                    ((JObject)d)["Operand"].ToString().Equals(SimpleServices.Tags.Tag.GetOperand(baseName), StringComparison.OrdinalIgnoreCase)) as JObject)?["Value"];
                if (string.IsNullOrEmpty(topDesc))
                {
                    topDesc = DataTypeL?.Description;
                }

                return string.IsNullOrEmpty(topDesc)
                    ? (DataTypeL as CompositiveType)?.GetDescription(name) ?? ""
                    : $"{topDesc} {(DataTypeL as CompositiveType)?.GetDescription(name) ?? ""}";
            }
            return description;
        }
        
        protected void ChangeDescription()
        {
            if (!CanSetDescription)
            {
                ResetChildDescription();
                return;
            }
            string name = Name;
             if (name.IndexOf(".", StringComparison.Ordinal) > 0)
            {
                name = name.Substring(name.IndexOf(".", StringComparison.Ordinal));
            }

            Debug.Assert(ChildDescription is JArray);
            foreach (var item in (JArray) ChildDescription)
            {
                if (SimpleServices.Tags.Tag.GetOperand(name).Equals((item as JObject)?["Operand"]?.ToString(),StringComparison.OrdinalIgnoreCase))
                {
                    if(string.IsNullOrEmpty(_description))
                    {
                        item.Remove();
                        CanSetDescription = false;
                        Description = GetChildDescription(name);
                        CanSetDescription = true;
                        return;
                    }
                    item["Value"] = _description;
                    ResetChildDescription();
                    return;
                }
            }

            JObject jObject = new JObject();

            if (!string.IsNullOrEmpty(name))
            {
                jObject["Operand"] = SimpleServices.Tags.Tag.GetOperand(name);
                jObject["Value"] = _description;
                ((JArray) ChildDescription).Add(jObject);
            }
            else
            {
                CanSetDescription = false;
                Description = GetChildDescription(Name);
                CanSetDescription = false;
            }
            ResetChildDescription();
        }

        public void ResetChildDescription()
        {
            foreach (var row in Child)
            {
                row.CanSetDescription = false;
                row.Description = GetChildDescription(row.Name);
                row.CanSetDescription = true;
            }
        }

        public bool CanSetDescription { set; get; } = true;

        protected void ResetStyle()
        {
            DisplayStyleList.Clear();
            if (DataTypeL.Name.Equals("REAL", StringComparison.OrdinalIgnoreCase))
            {
                ButtonVisibility = Visibility.Collapsed;
                DisplayStyleList.Add(DisplayStyle.Float);
                DisplayStyleList.Add(DisplayStyle.Exponential);
                if (!DisplayStyleList.Contains(Style))
                    Style = DisplayStyle.Float;
            }
            else if (DataTypeL.Name.Equals("BOOL", StringComparison.OrdinalIgnoreCase))
            {
                ButtonVisibility = Visibility.Collapsed;
                DisplayStyleList.Add(DisplayStyle.Binary);
                DisplayStyleList.Add(DisplayStyle.Octal);
                DisplayStyleList.Add(DisplayStyle.Decimal);
                DisplayStyleList.Add(DisplayStyle.Hex);
                if (!DisplayStyleList.Contains(Style))
                    Style = DisplayStyle.Decimal;
            }
            else if (DataTypeL.Name.Equals("DINT", StringComparison.OrdinalIgnoreCase) ||
                     DataTypeL.Name.Equals("SINT", StringComparison.OrdinalIgnoreCase) ||
                     DataTypeL.Name.Equals("INT", StringComparison.OrdinalIgnoreCase) ||
                     DataTypeL.Name.Equals("LINT", StringComparison.OrdinalIgnoreCase))
            {
                ButtonVisibility = Visibility.Visible;
                DisplayStyleList.Add(DisplayStyle.Binary);
                DisplayStyleList.Add(DisplayStyle.Octal);
                DisplayStyleList.Add(DisplayStyle.Decimal);
                DisplayStyleList.Add(DisplayStyle.Hex);
                DisplayStyleList.Add(DisplayStyle.Ascii);
                if (!DisplayStyleList.Contains(Style))
                    Style = DisplayStyle.Decimal;

                if (DataTypeL.Name.Equals("SINT", StringComparison.OrdinalIgnoreCase))
                {
                    DefaultValueGridHeight = 18 * 2;
                }
                else if (DataTypeL.Name.Equals("INT", StringComparison.OrdinalIgnoreCase))
                {
                    DefaultValueGridHeight = 18 * 3;
                }
                else if (DataTypeL.Name.Equals("DINT", StringComparison.OrdinalIgnoreCase))
                {
                    DefaultValueGridHeight = 18 * 5;
                }
                else if (DataTypeL.Name.Equals("LINT", StringComparison.OrdinalIgnoreCase))
                {
                    DefaultValueGridHeight = 18 * 9;
                }

            }
            else if (DataTypeL.FamilyType == FamilyType.StringFamily&&!DataType.EndsWith("]"))
            {
                ButtonVisibility = Visibility.Collapsed;
                BrowseStringVisibility = Visibility.Visible;
            }
            else
            {
                Style = (DisplayStyle?)null;
            }
        }
        
        private void DataTypePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Name"))
            {
                Flag = true;
                _dataType = _dataTypeL.Name;
                OnPropertyChanged(nameof(DataType));
                return;
            }

            if (ExpanderCloseVis == Visibility.Visible)
            {
                ExpanderVis = Visibility.Visible;
                ExpanderCloseVis = Visibility.Collapsed;
            }
            Child.Clear();
            Field = null;
        }

        private void SetBaseChild(ref int point, BaseTagRow baseTagRow)
        {
            if (baseTagRow == null) return;
            if (baseTagRow.ExpanderCloseVis == Visibility.Visible)
            {
                foreach (var item in baseTagRow.Child)
                {
                    item.TmpTag.TmpName = item.Name;
                    Child.Insert(point, item);
                    point++;
                    if (item.Child.Count > 0)
                    {
                        if (item.ExpanderCloseVis == Visibility.Visible)
                            SetBaseChild(ref point, item);
                    }
                }
            }
        }

        public void ClearBaseChild(BaseTagRow baseTagRow)
        {
            if (baseTagRow == null||!baseTagRow.IsMember) return;
            foreach (var item in baseTagRow.Child)
            {
                Child.Remove(item);
                if (item.Child.Count > 0)
                {
                    ClearBaseChild(item);
                }
            }
        }

        private void SetChildStyle()
        {
            if (DataType?.IndexOf("[") > 0)
            {
                foreach (var baseTagRow in Child)
                {
                    baseTagRow.Style = Style;
                }
            }
        }

        protected string GetIntegerValue(IField field)
        {
            var boolField = field as BoolField;
            if (boolField != null)
            {
                return boolField.value==1?"1":"0";
            }

            var int8Field = field as Int8Field;
            if (int8Field != null)
            {
                return int8Field.value.ToString();
            }

            var int16Field = field as Int16Field;
            if (int16Field != null)
            {
                return int16Field.value.ToString();
            }

            var int32Field = field as Int32Field;
            if (int32Field != null)
            {
                return int32Field.value.ToString();
            }

            var int64Field = field as Int64Field;
            if (int64Field != null)
            {
                return int64Field.value.ToString();
            }

            var realField = field as RealField;
            if (realField != null)
            {
                return realField.value.ToString();
            }
            Debug.Assert(false);
            return "0";
        }

        protected bool IsInCreate { set; get; } = false;
    }

    public class ItemCollection : LargeCollection<BaseTagRow>
    {
        public override void AddRange(int index, List<BaseTagRow> insertItems)
        {
            if (index > Count || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (insertItems == null || insertItems.Count == 0)
                return;

            CheckReentrancy();

            if (insertItems.Count == 1)
            {
                Insert(index, insertItems[0]);
                return;
            }

            List<BaseTagRow> items = (List<BaseTagRow>)Items;
            int startIndex = index;

            foreach (var item in insertItems)
            {
                items.Insert(index, item);
                index++;
            }

            OnPropertyChanged(EventArgsCache.CountPropertyChanged);
            OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add,
                    insertItems,
                    startIndex));
        }

        public override void RemoveTagItems(List<BaseTagRow> listItem)
        {
            if (listItem.Count == 0)
                return;

            CheckReentrancy();

            if (listItem.Count == 1)
            {
                RemoveTagItem(listItem[0]);
                listItem[0].ExpanderCloseCommand.Execute(listItem[0]);
                return;
            }

            List<BaseTagRow> items = (List<BaseTagRow>)Items;
            foreach (var item in listItem)
            {
                items.Remove(item);
                item.ExpanderCloseCommand.Execute(item);
            }

            OnPropertyChanged(EventArgsCache.CountPropertyChanged);
            OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);
            OnCollectionChanged(EventArgsCache.ResetCollectionChanged);
        }
    }
    public static class EventArgsCache
    {
        internal static readonly PropertyChangedEventArgs CountPropertyChanged = new PropertyChangedEventArgs("Count");

        internal static readonly PropertyChangedEventArgs IndexerPropertyChanged =
            new PropertyChangedEventArgs("Item[]");

        internal static readonly NotifyCollectionChangedEventArgs ResetCollectionChanged =
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
    }
}
