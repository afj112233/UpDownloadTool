using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Windows.Data;
using ICSStudio.EditorPackage.MonitorEditTags.UI;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.SimpleServices.Tags;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;

namespace ICSStudio.EditorPackage.MonitorEditTags.Models
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    [SuppressMessage("ReSharper", "UseNameofExpression")]
    internal partial class MonitorTagItem : TagItem, IEditableObject
    {
        private bool _inTxn;

        private string _description;
        private DisplayStyle _displayStyle;
        private Usage _usage;

        private string _stringFormat;

        private object _value;
        private object _editValue;

        public bool IsChanged { get; set; }

        public object Value
        {
            get
            {
                // in aoi data context
                MonitorTagCollection monitorTagCollection = ParentCollection as MonitorTagCollection;
                var dataContext = monitorTagCollection?.DataContext;
                if (dataContext?.ReferenceAoi != null)
                {
                    return ValueInDataContext(dataContext);
                }

                if (Usage == Usage.InOut)
                {
                    if (ItemType == TagItemType.Array ||
                        ItemType == TagItemType.Struct)
                        return "{...}";
                    return string.Empty;
                }
                    

                if (ItemType == TagItemType.Array ||
                    ItemType == TagItemType.Struct)
                {
                    _value = new StructOrArrayValue();
                    return _value;
                }

                if (ItemType == TagItemType.String)
                {
                    // string and udt-string
                    var compositeField = (ICompositeField)DataField;
                    if (compositeField != null)
                    {
                        _value = compositeField.ToString(DisplayStyle);
                        return _value;
                    }
                }

                // bool array
                if (ItemType == TagItemType.BoolArrayMember)
                {
                    var boolArrayField = (BoolArrayField)DataField;
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

                    var uint8Field = DataField as UInt8Field;
                    if (uint8Field != null)
                    {
                        _value = BitValue.Get(uint8Field.value, bitOffset);
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

                    var uint8Field = DataField as UInt8Field;
                    if (uint8Field != null)
                    {
                        _value = uint8Field.value;
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

        private object ValueInDataContext(AoiDataReference dataContext)
        {
            if (ItemType == TagItemType.Array ||
                ItemType == TagItemType.Struct)
            {
                _value = new StructOrArrayValue();
                return _value;
            }

            var program = dataContext.GetReferenceProgram();
            var transformTable = dataContext.GetFinalTransformTable();

            var transformName = (string)transformTable[Tag.Name.ToUpper()];

            var targetName = transformName + Name.Remove(0, Tag.Name.Length);

            var dataOperand = DataServer.CreateDataOperand(program, targetName);
            if (!dataOperand.IsOperandValid)
                dataOperand = DataServer.CreateDataOperand(DataServer.ParentController, targetName);
            
            return dataOperand.OriginalValue;

            //if (ItemType == TagItemType.String)
            //{
            //}

            //_value = "TODO";
            //return _value;
        }

        private bool IsOnline => Tag.ParentController.IsOnline;
        private Controller Controller => Tag.ParentController as Controller;

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
                MonitorTagCollection monitorTagCollection = ParentCollection as MonitorTagCollection;
                var dataContext = monitorTagCollection?.DataContext;
                if (dataContext?.ReferenceAoi != null)
                {
                    //TODO(gjc): add code here

                    var program = dataContext.GetReferenceProgram();
                    var transformTable = dataContext.GetFinalTransformTable();
                    var transformName = (string)transformTable[Tag.Name.ToUpper()];
                    var targetName = transformName + Name.Remove(0, Tag.Name.Length);
                    var dataOperand = DataServer.CreateDataOperand(program, targetName);
                    if (!dataOperand.IsOperandValid)
                        dataOperand = DataServer.CreateDataOperand(DataServer.ParentController, targetName);

                    if (IsOnline)
                        SetTagValueToPLC(dataOperand.Tag, targetName, value.ToString());
                    else
                        dataOperand.SetValue(value.ToString());

                    return;
                }

                if (ItemType == TagItemType.String)
                {
                    // string and udt-string
                    var compositeField = (ICompositeField)DataField;

                    var byteList = ValueConverter.ToBytes(value.ToString());

                    if (!compositeField.EqualsByteList(byteList))
                    {
                        if (IsOnline)
                        {
                            SetTagValueToPLC(Tag, Name, value.ToString());
                        }
                        else
                        {
                            compositeField.Set(byteList);
                            Tag?.RaisePropertyChanged(Name);

                            //TODO(gjc): add code for string
                        }
                    }

                }
                else if (ItemType == TagItemType.BoolArrayMember)
                {
                    var boolValue = ValueConverter.ToBoolean(value.ToString());
                    var boolArrayField = (BoolArrayField)DataField;
                    if (boolArrayField.Get(MemberIndex) != boolValue)
                    {
                        if (IsOnline)
                        {
                            SetTagValueToPLC(Tag, Name, value.ToString());
                        }
                        else
                        {
                            boolArrayField.Set(MemberIndex, boolValue);

                            Notifications.SendNotificationData(new TagNotificationData()
                            {
                                Tag = Tag,
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
                                SetTagValueToPLC(Tag, Name, value.ToString());
                            }
                            else
                            {
                                boolField.value = (byte)(boolValue ? 1 : 0);

                                Notifications.SendNotificationData(new TagNotificationData()
                                {
                                    Tag = Tag,
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
                                SetTagValueToPLC(Tag, Name, value.ToString());
                            }
                            else
                            {
                                sbyte newValue = BitValue.Set(int8Field.value, bitOffset, boolValue);
                                int8Field.value = newValue;

                                Notifications.SendNotificationData(new TagNotificationData()
                                {
                                    Tag = Tag,
                                    Type = TagNotificationData.NotificationType.Value,
                                    Field = int8Field,
                                    IsBit = true,
                                    BitOffset = bitOffset
                                });
                            }
                        }
                    }

                    var uint8Field = DataField as UInt8Field;
                    if (uint8Field != null)
                    {
                        if (BitValue.Get(uint8Field.value, bitOffset) != boolValue)
                        {
                            if (IsOnline)
                            {
                                SetTagValueToPLC(Tag, Name, value.ToString());
                            }
                            else
                            {
                                byte newValue = BitValue.Set(uint8Field.value, bitOffset, boolValue);
                                uint8Field.value = newValue;

                                Notifications.SendNotificationData(new TagNotificationData()
                                {
                                    Tag = Tag,
                                    Type = TagNotificationData.NotificationType.Value,
                                    Field = uint8Field,
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
                                SetTagValueToPLC(Tag, Name, value.ToString());
                            }
                            else
                            {
                                short newValue = BitValue.Set(int16Field.value, bitOffset, boolValue);
                                int16Field.value = newValue;

                                Notifications.SendNotificationData(new TagNotificationData()
                                {
                                    Tag = Tag,
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
                                SetTagValueToPLC(Tag, Name, value.ToString());
                            }
                            else
                            {
                                int newValue = BitValue.Set(int32Field.value, bitOffset, boolValue);
                                int32Field.value = newValue;

                                Notifications.SendNotificationData(new TagNotificationData()
                                {
                                    Tag = Tag,
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
                                SetTagValueToPLC(Tag, Name, value.ToString());
                            }
                            else
                            {
                                long newValue = BitValue.Set(int64Field.value, bitOffset, boolValue);
                                int64Field.value = newValue;

                                Notifications.SendNotificationData(new TagNotificationData()
                                {
                                    Tag = Tag,
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
                        byte byteValue = (byte)(boolValue ? 1 : 0);
                        if (boolField.value != byteValue)
                        {
                            if (IsOnline)
                            {
                                SetTagValueToPLC(Tag, Name, value.ToString());
                            }
                            else
                            {
                                boolField.value = byteValue;

                                Notifications.SendNotificationData(new TagNotificationData()
                                {
                                    Tag = Tag,
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
                                SetTagValueToPLC(Tag, Name, sbyteValue.ToString());
                            }
                            else
                            {
                                int8Field.value = sbyteValue;

                                Notifications.SendNotificationData(new TagNotificationData()
                                {
                                    Tag = Tag,
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
                                SetTagValueToPLC(Tag, Name, shortValue.ToString());
                            }
                            else
                            {
                                int16Field.value = shortValue;

                                Notifications.SendNotificationData(new TagNotificationData()
                                {
                                    Tag = Tag,
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
                                SetTagValueToPLC(Tag, Name, intValue.ToString());
                            }
                            else
                            {
                                int32Field.value = intValue;

                                Notifications.SendNotificationData(new TagNotificationData()
                                {
                                    Tag = Tag,
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
                                SetTagValueToPLC(Tag, Name, longValue.ToString());
                            }
                            else
                            {
                                int64Field.value = longValue;

                                Notifications.SendNotificationData(new TagNotificationData()
                                {
                                    Tag = Tag,
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
                                SetTagValueToPLC(Tag, Name, floatValue.ToString("g9"));
                            }
                            else
                            {
                                realField.value = floatValue;

                                Notifications.SendNotificationData(new TagNotificationData()
                                {
                                    Tag = Tag,
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
            get
            {
                if (DataTypeInfo.DataType == null)
                    return DisplayStyle.NullStyle;

                if (DataTypeInfo.DataType.IsStruct)
                    return DisplayStyle.NullStyle;

                return _displayStyle;
            }
            set
            {
                _displayStyle = value;

                OnPropertyChanged();

                OnPropertyChanged("IsValueEnabled");

                if (ItemType == TagItemType.Array && Children != null)
                {
                    foreach (var child in Children)
                    {
                        child.DisplayStyle = _displayStyle;
                    }
                }
            }
        }

        public List<DisplayStyle> DisplayStyleSource
        {
            get
            {
                var dataType = DataTypeInfo.DataType;

                if (dataType == null)
                    return null;

                if (dataType.IsLINT)
                    return new List<DisplayStyle>
                    {
                        DisplayStyle.Binary,
                        DisplayStyle.Octal,
                        DisplayStyle.Decimal,
                        DisplayStyle.Hex,
                        DisplayStyle.Ascii,
                        DisplayStyle.DateTime,
                        DisplayStyle.DateTimeNS
                    };

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
                        _description = Tag.GetChildDescription(Tag.Description, Tag.DataTypeInfo, Tag.ChildDescription,
                            Tag.GetOperand(Name));
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
                //TODO(gjc): temp edit
                //{
                //    MonitorTagCollection monitorTagCollection = ParentCollection as MonitorTagCollection;
                //    var dataContext = monitorTagCollection?.DataContext;
                //    if (dataContext?.ReferenceAoi != null)
                //    {
                //        return false;
                //    }

                //    if (dataContext != null && IsOnline)
                //    {
                //        return false;
                //    }
                //}
                //

                if (ParentItem == null && Tag?.ParentCollection?.ParentProgram is AoiDefinition)
                {
                    if (Name.Equals("EnableIn", StringComparison.OrdinalIgnoreCase) ||
                        Name.Equals("EnableOut", StringComparison.OrdinalIgnoreCase))
                    {
                        MonitorTagCollection monitorTagCollection = ParentCollection as MonitorTagCollection;
                        var dataContext = monitorTagCollection?.DataContext;
                        if (dataContext?.ReferenceAoi != null)
                        {
                            return true;
                        }

                        return false;
                    }

                }

                if (DisplayStyle == DisplayStyle.NullStyle && ItemType != TagItemType.String)
                    return false;

                if (ItemType == TagItemType.Array)
                    return false;

                //TODO(gjc): temp add
                if (Tag != null)
                {
                    var dataType = Tag.DataTypeInfo.DataType;
                    if (dataType != null)
                    {
                        if (dataType.IsAxisType || dataType.IsMotionGroupType)
                            return false;
                    }

                }

                if (Usage == Usage.InOut)
                {
                    MonitorTagCollection monitorTagCollection = ParentCollection as MonitorTagCollection;
                    var dataContext = monitorTagCollection?.DataContext;
                    if (dataContext?.ReferenceAoi != null)
                    {
                        return true;
                    }

                    return false;

                }

                return true;
            }
        }

        private IDataOperand _dataOperand;

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

        protected override void OnTagPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnTagPropertyChanged(sender, e);

            if (sender == Tag)
            {
                if (e.PropertyName == "Data")
                {
                    OnPropertyChanged("Value");
                    return;
                }

                if (Name.StartsWith(e.PropertyName))
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
                    Description = Tag.GetChildDescription(Tag.Description, Tag.DataTypeInfo, Tag.ChildDescription,
                        Tag.GetOperand(Name));
                    CanSetDescription = true;
                }

                if (e.PropertyName.Equals("IsConstant") && ParentItem == null)
                {
                    OnPropertyChanged("IsConstant");
                }

                if (e.PropertyName.Equals("Usage") && ParentItem == null) Usage = Tag.Usage;

                if (e.PropertyName.Equals("DisplayStyle") && ParentItem == null) DisplayStyle = Tag.DisplayStyle;

                if (ParentItem == null)
                    if (e.PropertyName.Equals("DataWrapper"))
                    {
                        var parentCollection = ParentCollection as MonitorTagCollection;
                        var defaultView = CollectionViewSource.GetDefaultView(ParentCollection);

                        if (parentCollection != null && defaultView != null)
                        {
                            var index = parentCollection.IndexOf(this);
                            if (index < 0)
                                return;

                            bool needRestoreCurrent = defaultView.CurrentItem == this;

                            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                            if (index >= 0)
                            {
                                var newItem = TagToMonitorTagItem(Tag, DataServer);

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