using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Windows.Data;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Dialogs.BrowseString;
using ICSStudio.Dialogs.BrowseString.RichTextBoxExtend;
using ICSStudio.QuickWatchPackage.View.UI;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.SimpleServices.Tags;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;

namespace ICSStudio.QuickWatchPackage.View.Models
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    [SuppressMessage("ReSharper", "UseNameofExpression")]
    internal partial class MonitorTagItem : TagItem, IEditableObject
    {
        public MonitorTagItem()
        {
            StringBrowseCommand = new RelayCommand(ExecuteStringBrowseCommand);
        }
        public RelayCommand StringBrowseCommand { get; }

        private void ExecuteStringBrowseCommand()
        {
            if (string.IsNullOrEmpty(DataType))
            {
                Debug.Assert(false);
                return;
            }
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            var message = new Message(null,Value?.ToString(), Name);
            PropertyChangedEventManager.AddHandler(message, Message_PropertyChanged, "Text");
            //var vm = new BrowseStringViewModel(DataTypeL, message);
            var dialog = new BrowseStringDialog(Controller.DataTypes.ParseDataTypeInfo(DataType).DataType, message);
            //dialog.DataContext = vm;
            var uiShell = (IVsUIShell)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsUIShell));
            if (dialog.ShowDialog(uiShell) ?? false)
            {

            }

            PropertyChangedEventManager.RemoveHandler(message, Message_PropertyChanged, "Text");
        }
        
        private void Message_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Text")
            {
                EditValue = ((Message)sender).Text;
            }
        }
        
        public override bool IsReadOnly
        {
            protected set { _isReadOnly = value; }
            get
            {
                if (Tag?.ParentCollection.ParentProgram is AoiDefinition)
                {
                    if (Tag.Usage == Usage.InOut)
                        return false;
                    if ("EnableIn".Equals(Tag.Name) || "EnableOut".Equals(Tag.Name))
                        return false;
                }
                return _isReadOnly;
            }
        }

        private bool _inTxn;

        private string _description;
        private DisplayStyle _displayStyle = DisplayStyle.Decimal;
        private Usage _usage;

        private string _stringFormat;

        private object _value;
        private object _editValue;

        public bool IsChanged { get; set; }

        public IProgramModule ParentScope { set; get; } = null;

        public override string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                if (IsGetDetail)
                {
                    if (!IsReadOnly && Tag == null)
                    {
                       GetInfo();

                        IsReadOnly = true;
                        OnPropertyChanged("IsReadOnly");
                    }

                }
                OnPropertyChanged();
            }
        }

        private bool _isError = false;
        private void GetInfo()
        {
            var info = ObtainValue.GetSpecifierDataInfo(Name, null, ParentScope, null);
            if (info != null)
            {
                _isError = false;
                var tag = info.Item4;
                if (!tag.Name.Equals(_name, StringComparison.OrdinalIgnoreCase))
                {
                    var memberTag = new MemberTag(tag);
                    memberTag.Name = _name;
                    memberTag.DataWrapper = new DataWrapper(info.Item2, info.Item3);
                    memberTag.Index = info.Item1;
                    memberTag.IsInCreate = false;
                    memberTag.AddListen();
                    memberTag.DisplayStyle = info.Item5;
                    Reset(info.Item2, memberTag, info.Item1, info.Item3);
                }
                else
                {
                    Reset(info.Item2, info.Item4, info.Item1, info.Item3);
                }
            }
            else
            {
                if (Tag != null)
                {
                    Tag.DataWrapper = null;
                }
                _isError = true;
                OnPropertyChanged("Value");
            }
        }
        
        public bool IsGetDetail { set; get; } = true;

        public ITagCollectionContainer TagCollectionContainer { set; get; }

        private void Reset(DataTypeInfo dataTypeInfo, Tag tag, int index, IField field)
        {
            CanSetDescription = false;
            //DataServer = dataServer,
            //Name = name,
            //Usage = tag.Usage,
            DataType = dataTypeInfo.ToString();
            Description = Tag.GetChildDescription(tag.Description, tag.DataTypeInfo, tag.ChildDescription, Tag.GetOperand(Name));
            DisplayStyle = tag.DisplayStyle;
            
            Tag = tag;
            ((MonitorTagCollection)(TagCollectionContainer as MonitorContainer)?.Tags)?.AddTag(tag);
            //ParentItem = parentItem,
            MemberIndex = index;

            DataTypeInfo = dataTypeInfo;
            DataField = field;
            if (dataTypeInfo.Dim1 == 0)
            {
                var dataType = dataTypeInfo.DataType;

                if (dataType.IsBool)
                {
                    DataType = "BOOL";

                    if (ParentItem != null)
                    {
                        if (ParentItem.ItemType == TagItemType.Array)
                            ItemType = TagItemType.BoolArrayMember;

                        if (ParentItem.ItemType == TagItemType.Integer)
                            ItemType = TagItemType.IntegerMember;
                    }

                    if(dataTypeInfo.DataType.IsBool&&dataTypeInfo.Dim1==0)
                    {
                        ItemType = TagItemType.BitMember;
                        BitOffset = index;
                        MemberIndex = 0;
                    }

                    
                }

                if (dataType.IsInteger)
                {
                    ItemType = TagItemType.Integer;
                }

                if (dataType.IsStruct)
                {
                    ItemType = TagItemType.Struct;
                }

                if (dataType is UserDefinedDataType || dataType is ModuleDefinedDataType || dataType is AOIDataType)
                {
                    ItemType = TagItemType.Struct;
                }

                if (dataType.IsStringType)
                {
                    ItemType = TagItemType.String;
                }

            }
            else
            {
                ItemType = TagItemType.Array;
            }

            OnPropertyChanged("Value");
        }

        public object Value
        {
            get
            {
                if (_isError) return "(error)";
                if (string.IsNullOrEmpty(Name)) return "";
                if (Usage == Usage.InOut) return string.Empty;
                
                if (ItemType == TagItemType.Array ||
                    ItemType == TagItemType.Struct)
                {
                    _value = new StructOrArrayValue();
                    return _value;
                }

                if (ItemType == TagItemType.String)
                {
                    // string and udt-string
                    var compositeField = (ICompositeField) DataField;
                    if (compositeField != null)
                    {
                        _value = compositeField.ToString(DisplayStyle);
                        return _value;
                    }
                }

                // bool array
                if (ItemType == TagItemType.BoolArrayMember)
                {
                    var boolArrayField = (BoolArrayField) DataField;
                    _value = boolArrayField.Get(MemberIndex);
                    return _value;
                }

                if (ItemType == TagItemType.IntegerMember
                    || ItemType == TagItemType.BitMember)
                {
                    int bitOffset = BitOffset;

                    var boolField = DataField as BoolField;
                    if (boolField != null)
                    {
                        Contract.Assert(bitOffset == 0);

                        _value = boolField.value != 0;
                        return _value;
                    }

                    var int8Field = DataField as Int8Field;
                    if (int8Field != null)
                    {
                        _value = BitValue.Get(int8Field.value, bitOffset);
                        return _value;
                    }

                    var int16Field = DataField as Int16Field;
                    if (int16Field != null)
                    {
                        _value = BitValue.Get(int16Field.value, bitOffset);
                        return _value;
                    }

                    var int32Field = DataField as Int32Field;
                    if (int32Field != null)
                    {
                        _value = BitValue.Get(int32Field.value, bitOffset);
                        return _value;
                    }

                    var int64Field = DataField as Int64Field;
                    if (int64Field != null)
                    {
                        _value = BitValue.Get(int64Field.value, bitOffset);
                        return _value;
                    }
                }

                if (ItemType == TagItemType.Default || ItemType == TagItemType.Integer)
                {
                    var boolField = DataField as BoolField;
                    if (boolField != null)
                    {
                        _value = boolField.value != 0;
                        return _value;
                    }

                    var int8Field = DataField as Int8Field;
                    if (int8Field != null)
                    {
                        _value = int8Field.value;
                        return _value;
                    }

                    var int16Field = DataField as Int16Field;
                    if (int16Field != null)
                    {
                        _value = int16Field.value;
                        return _value;
                    }

                    var int32Field = DataField as Int32Field;
                    if (int32Field != null)
                    {
                        _value = int32Field.value;
                        return _value;
                    }

                    var int64Field = DataField as Int64Field;
                    if (int64Field != null)
                    {
                        _value = int64Field.value;
                        return _value;
                    }

                    var realField = DataField as RealField;
                    if (realField != null)
                    {
                        _value = realField.value;
                        return _value;
                    }
                }

                _value = "TODO";
                return _value;
            }

        }

        private bool IsOnline => Controller.GetInstance().IsOnline;
        private Controller Controller => Controller.GetInstance();

        private void SetTagValueToPLC(ITag tag, string specifier, string value)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await TaskScheduler.Default;
                await Controller.SetTagValueToPLC(tag, specifier, value);
            });
        }

        public object EditValue
        {
            get
            {
                _editValue = Value;
                return _editValue;
            }
            set
            {
                var tag = (Tag)(Tag as MemberTag)?.ParentTag ?? Tag;
                var name = (string)(Tag as MemberTag)?.Transform?[Name] ?? Name;
                if (ItemType == TagItemType.String)
                {
                    // string and udt-string
                    var compositeField = (ICompositeField) DataField;

                    var byteList = ValueConverter.ToBytes(value.ToString());

                    if (!compositeField.EqualsByteList(byteList))
                    {
                        if (IsOnline)
                        {
                            SetTagValueToPLC(tag, name, value.ToString());
                        }
                        else
                        {
                            compositeField.Set(byteList);
                            tag?.RaisePropertyChanged(Name);

                            //TODO(gjc): add code for string
                        }
                    }

                }
                else if (ItemType == TagItemType.BoolArrayMember)
                {
                    var boolValue = ValueConverter.ToBoolean(value.ToString());
                    var boolArrayField = (BoolArrayField) DataField;
                    if (boolArrayField.Get(MemberIndex) != boolValue)
                    {
                        if (IsOnline)
                        {
                            SetTagValueToPLC(tag, name, value.ToString());
                        }
                        else
                        {
                            boolArrayField.Set(MemberIndex, boolValue);

                            Notifications.SendNotificationData(new TagNotificationData()
                            {
                                Tag = tag,
                                Type = TagNotificationData.NotificationType.Value,
                                Field = boolArrayField,
                                IsBit = true,
                                BitOffset = MemberIndex
                            });
                        }

                    }

                }
                else if (ItemType == TagItemType.IntegerMember
                         || ItemType == TagItemType.BitMember)
                {
                    int bitOffset = BitOffset;

                    bool boolValue = ValueConverter.ToBoolean(value.ToString());

                    var boolField = DataField as BoolField;
                    if (boolField != null)
                    {
                        bool oldValue = boolField.value == 1;
                        if (oldValue != boolValue)
                        {
                            if (IsOnline)
                            {
                                SetTagValueToPLC(tag, name, value.ToString());
                            }
                            else
                            {
                                boolField.value = (byte) (boolValue ? 1 : 0);

                                Notifications.SendNotificationData(new TagNotificationData()
                                {
                                    Tag = tag,
                                    Type = TagNotificationData.NotificationType.Value,
                                    Field = boolField,
                                    IsBit = true,
                                    BitOffset = bitOffset
                                });
                            }
                        }
                    }

                    var int8Field = DataField as Int8Field;
                    if (int8Field != null)
                    {
                        if (BitValue.Get(int8Field.value, bitOffset) != boolValue)
                        {
                            if (IsOnline)
                            {
                                SetTagValueToPLC(tag, name, value.ToString());
                            }
                            else
                            {
                                sbyte newValue = BitValue.Set(int8Field.value, bitOffset, boolValue);
                                int8Field.value = newValue;

                                Notifications.SendNotificationData(new TagNotificationData()
                                {
                                    Tag = tag,
                                    Type = TagNotificationData.NotificationType.Value,
                                    Field = int8Field,
                                    IsBit = true,
                                    BitOffset = bitOffset
                                });
                            }
                        }
                    }

                    var int16Field = DataField as Int16Field;
                    if (int16Field != null)
                    {
                        if (BitValue.Get(int16Field.value, bitOffset) != boolValue)
                        {
                            if (IsOnline)
                            {
                                SetTagValueToPLC(tag, name, value.ToString());
                            }
                            else
                            {
                                short newValue = BitValue.Set(int16Field.value, bitOffset, boolValue);
                                int16Field.value = newValue;

                                Notifications.SendNotificationData(new TagNotificationData()
                                {
                                    Tag = tag,
                                    Type = TagNotificationData.NotificationType.Value,
                                    Field = int16Field,
                                    IsBit = true,
                                    BitOffset = bitOffset
                                });
                            }

                        }

                    }

                    var int32Field = DataField as Int32Field;
                    if (int32Field != null)
                    {
                        if (BitValue.Get(int32Field.value, bitOffset) != boolValue)
                        {
                            if (IsOnline)
                            {
                                SetTagValueToPLC(tag, name, value.ToString());
                            }
                            else
                            {
                                int newValue = BitValue.Set(int32Field.value, bitOffset, boolValue);
                                int32Field.value = newValue;

                                Notifications.SendNotificationData(new TagNotificationData()
                                {
                                    Tag = tag,
                                    Type = TagNotificationData.NotificationType.Value,
                                    Field = int32Field,
                                    IsBit = true,
                                    BitOffset = bitOffset
                                });
                            }

                        }

                    }

                    var int64Field = DataField as Int64Field;
                    if (int64Field != null)
                    {
                        if (BitValue.Get(int64Field.value, bitOffset) != boolValue)
                        {
                            if (IsOnline)
                            {
                                SetTagValueToPLC(tag, name, value.ToString());
                            }
                            else
                            {
                                long newValue = BitValue.Set(int64Field.value, bitOffset, boolValue);
                                int64Field.value = newValue;

                                Notifications.SendNotificationData(new TagNotificationData()
                                {
                                    Tag = tag,
                                    Type = TagNotificationData.NotificationType.Value,
                                    Field = int64Field,
                                    IsBit = true,
                                    BitOffset = bitOffset
                                });
                            }

                        }

                    }
                }
                else if (ItemType == TagItemType.Default || ItemType == TagItemType.Integer)
                {
                    var boolField = DataField as BoolField;
                    if (boolField != null)
                    {
                        bool boolValue = ValueConverter.ToBoolean(value.ToString());
                        byte byteValue = (byte) (boolValue ? 1 : 0);
                        if (boolField.value != byteValue)
                        {
                            if (IsOnline)
                            {
                                SetTagValueToPLC(tag, name, value.ToString());
                            }
                            else
                            {
                                boolField.value = byteValue;

                                Notifications.SendNotificationData(new TagNotificationData()
                                {
                                    Tag = tag,
                                    Type = TagNotificationData.NotificationType.Value,
                                    Field = boolField
                                });
                            }

                        }
                    }

                    var int8Field = DataField as Int8Field;
                    if (int8Field != null)
                    {
                        var sbyteValue = ValueConverter.ToSByte(value.ToString(), DisplayStyle);
                        if (int8Field.value != sbyteValue)
                        {
                            if (IsOnline)
                            {
                                SetTagValueToPLC(tag, name, sbyteValue.ToString());
                            }
                            else
                            {
                                int8Field.value = sbyteValue;

                                Notifications.SendNotificationData(new TagNotificationData()
                                {
                                    Tag = tag,
                                    Type = TagNotificationData.NotificationType.Value,
                                    Field = int8Field
                                });
                            }

                        }
                    }

                    var int16Field = DataField as Int16Field;
                    if (int16Field != null)
                    {
                        var shortValue = ValueConverter.ToShort(value.ToString(), DisplayStyle);
                        if (int16Field.value != shortValue)
                        {
                            if (IsOnline)
                            {
                                SetTagValueToPLC(tag, name, shortValue.ToString());
                            }
                            else
                            {
                                int16Field.value = shortValue;

                                Notifications.SendNotificationData(new TagNotificationData()
                                {
                                    Tag = tag,
                                    Type = TagNotificationData.NotificationType.Value,
                                    Field = int16Field
                                });
                            }

                        }

                    }

                    var int32Field = DataField as Int32Field;
                    if (int32Field != null)
                    {
                        var intValue = ValueConverter.ToInt(value.ToString(), DisplayStyle);
                        if (int32Field.value != intValue)
                        {
                            if (IsOnline)
                            {
                                SetTagValueToPLC(tag, name, intValue.ToString());
                            }
                            else
                            {
                                int32Field.value = intValue;

                                Notifications.SendNotificationData(new TagNotificationData()
                                {
                                    Tag = tag,
                                    Type = TagNotificationData.NotificationType.Value,
                                    Field = int32Field
                                });
                            }

                        }

                    }

                    var int64Field = DataField as Int64Field;
                    if (int64Field != null)
                    {
                        var longValue = ValueConverter.ToLong(value.ToString(), DisplayStyle);
                        if (int64Field.value != longValue)
                        {
                            if (IsOnline)
                            {
                                SetTagValueToPLC(tag, name, longValue.ToString());
                            }
                            else
                            {
                                int64Field.value = longValue;

                                Notifications.SendNotificationData(new TagNotificationData()
                                {
                                    Tag = tag,
                                    Type = TagNotificationData.NotificationType.Value,
                                    Field = int64Field
                                });
                            }

                        }

                    }

                    var realField = DataField as RealField;
                    if (realField != null)
                    {
                        var floatValue = ValueConverter.ToFloat(value.ToString());
                        // ReSharper disable once CompareOfFloatsByEqualityOperator
                        if (realField.value != floatValue)
                        {
                            if (IsOnline)
                            {
                                SetTagValueToPLC(tag, name, floatValue.ToString("g9"));
                            }
                            else
                            {
                                realField.value = floatValue;

                                Notifications.SendNotificationData(new TagNotificationData()
                                {
                                    Tag = tag,
                                    Type = TagNotificationData.NotificationType.Value,
                                    Field = realField
                                });
                            }

                        }

                    }
                }

                //TODO(gjc): discard later
                Tag?.RaisePropertyChanged(Name);

                OnPropertyChanged();
            }
        }

        public string StringFormat
        {
            get { return _stringFormat; }
            set
            {
                _stringFormat = value;
                OnPropertyChanged();
            }
        }

        public override DisplayStyle DisplayStyle
        {
            get { return _displayStyle==DisplayStyle.NullStyle?DisplayStyle.Decimal:_displayStyle; }
            set
            {
                _displayStyle = value;

                OnPropertyChanged();

                OnPropertyChanged("IsValueEnabled");
            }
        }

        public List<DisplayStyle> DisplayStyleSource
        {
            get
            {
                var dataType = DataTypeInfo.DataType;

                if (dataType == null)
                    return null;

                if (dataType.IsInteger)
                    return new List<DisplayStyle>
                    {
                        DisplayStyle.Binary,
                        DisplayStyle.Octal,
                        DisplayStyle.Decimal,
                        DisplayStyle.Hex,
                        DisplayStyle.Ascii
                    };

                if (dataType.IsBool)
                    return new List<DisplayStyle>
                    {
                        DisplayStyle.Binary,
                        DisplayStyle.Octal,
                        DisplayStyle.Decimal,
                        DisplayStyle.Hex
                    };

                if (dataType.IsReal)
                    return new List<DisplayStyle>
                    {
                        DisplayStyle.Float,
                        DisplayStyle.Exponential
                    };

                return null;
            }
        }

        public override string Description
        {
            get { return _description; }
            set
            {
                if (!string.Equals(_description, value))
                {
                    _description = value;
                    if (CanSetDescription)
                        Tag?.SetChildDescription(Tag.GetOperand(Name), value);
                    if (Tag != null)
                        _description = Tag.GetChildDescription(Tag.Description, Tag.DataTypeInfo, Tag.ChildDescription, Tag.GetOperand(Name));
                    OnPropertyChanged();
                    
                }

                //TODO(gjc): need edit here
                //if (ParentItem == null && Tag != null && !string.Equals(Tag.Description, _description))
                //    Tag.Description = _description;
            }
        }

        public override Usage Usage
        {
            get { return _usage; }
            set
            {
                if (_usage == value) return;

                _usage = value;
                OnPropertyChanged();
                OnPropertyChanged("IsValueEnabled");
            }
        }

        public bool IsValueEnabled
        {
            get
            {
                if (ParentItem == null && Tag?.ParentCollection?.ParentProgram is AoiDefinition)
                {
                    if (Name.Equals("EnableIn", StringComparison.OrdinalIgnoreCase) ||
                        Name.Equals("EnableOut", StringComparison.OrdinalIgnoreCase))
                        return false;
                }
                if (Tag != null)
                {
                    if (Tag is MemberTag)
                    {
                        var dataType = ((MemberTag)Tag).ParentTag?.DataTypeInfo.DataType;
                        if (dataType != null)
                        {
                            if (dataType.IsAxisType || dataType.IsMotionGroupType)
                                return false;
                        }
                    }
                    else
                    {
                        var dataType = Tag.DataTypeInfo.DataType;
                        if (dataType != null)
                        {
                            if (dataType.IsAxisType || dataType.IsMotionGroupType)
                                return false;
                        }
                    }
                    

                }
                if (Usage == Usage.InOut)
                    return false;

                if (DisplayStyle == DisplayStyle.NullStyle && ItemType != TagItemType.String)
                    return false;

                if (ItemType == TagItemType.Array)
                    return false;

                return true;
            }
        }

        private IDataOperand _dataOperand;
        private bool _isReadOnly;

        public IDataOperand DataOperand
        {
            get { return _dataOperand; }
            set
            {
                if (_dataOperand != value)
                {
                    if (_dataOperand != null)
                    {
                        PropertyChangedEventManager.RemoveHandler(_dataOperand, OnDataOperandValueChanged, "RawValue");
                    }

                    _dataOperand = value;

                    if (_dataOperand != null)
                    {
                        PropertyChangedEventManager.AddHandler(_dataOperand, OnDataOperandValueChanged, "RawValue");
                    }
                }
            }
        }

        private void OnDataOperandValueChanged(object sender, PropertyChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                OnPropertyChanged("Value");
            });


        }

        //可删除
        protected override void OnTagPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnTagPropertyChanged(sender, e);

            if (sender == Tag)
            {
                if (Name.StartsWith(e.PropertyName)||e.PropertyName=="Data")
                    OnPropertyChanged("Value");

                if (ItemType == TagItemType.String)
                {
                    if (string.Equals(Name + ".LEN", e.PropertyName, StringComparison.OrdinalIgnoreCase))
                        OnPropertyChanged("Value");

                    if (e.PropertyName.StartsWith(Name + ".DATA", StringComparison.OrdinalIgnoreCase))
                        OnPropertyChanged("Value");
                }

                if (e.PropertyName.Equals("Description"))
                {
                    CanSetDescription = false;
                    Description = Tag.GetChildDescription(Tag.Description, Tag.DataTypeInfo, Tag.ChildDescription, Tag.GetOperand(Name));
                    CanSetDescription = true;
                }
                
                if (e.PropertyName.Equals("IsConstant") && ParentItem == null) OnPropertyChanged("IsConstant");

                if (e.PropertyName.Equals("Usage") && ParentItem == null) Usage = Tag.Usage;

                if (e.PropertyName.Equals("DisplayStyle") && ParentItem == null) DisplayStyle = Tag.DisplayStyle;

                if (ParentItem == null)
                    try
                    {
                        if (e.PropertyName.Equals("DataWrapper"))
                        {
                            if (Tag is MemberTag)
                            {
                                GetInfo();
                            }
                            var parentCollection = ParentCollection as MonitorTagItemCollection;
                            var defaultView = CollectionViewSource.GetDefaultView(ParentCollection);

                            if (parentCollection != null && defaultView != null)
                            {
                                var index = parentCollection.IndexOf(this);
                                //Contract.Assert(index >= 0);

                                bool needRestoreCurrent = defaultView.CurrentItem == this;

                                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                                if (index >= 0)
                                {
                                    var newItem = TagToMonitorTagItem(Tag, DataServer,ParentCollection);

                                    parentCollection.RemoveItemsByTag(Tag);

                                    // insert
                                    newItem.ParentCollection = parentCollection;

                                    parentCollection.Insert(index, newItem);

                                    if (needRestoreCurrent)
                                    {
                                        defaultView.MoveCurrentTo(newItem);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                        
                    }
            }
        }

        public void BeginEdit()
        {
            if (!_inTxn)
            {
                _inTxn = true;
            }

        }

        public void EndEdit()
        {
            if (_inTxn)
            {
                _inTxn = false;
            }
        }

        public void CancelEdit()
        {
            if (_inTxn)
            {
                OnPropertyChanged("EditValue");
                _inTxn = false;
            }

        }
    }
}