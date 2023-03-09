using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ICSStudio.Dialogs.Warning;
using ICSStudio.EditorPackage.MonitorEditTags.UI;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.SimpleServices.Tags;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Application = System.Windows.Application;

namespace ICSStudio.EditorPackage.MonitorEditTags.Models
{
    internal partial class PropertiesItem
    {
        private static void SetPropertyReadOnly(object obj, string propertyName, bool readOnly)
        {
            var type = typeof(ReadOnlyAttribute);
            var props = TypeDescriptor.GetProperties(obj);

            if (props[propertyName] == null)
                return;

            var attrs = props[propertyName].Attributes;
            var fld = type.GetField("isReadOnly",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance);
            if (fld != null)
            {
                fld.SetValue(attrs[type], readOnly);
            }
        }

        private string ChangeItemName(string oldName, string newName)
        {
            // check new oldName
            if (oldName == newName)
                return oldName;

            if (_tagItem.ParentCollection.Scope is AoiDefinition)
                _tagItem.Tag.Name = IsValidTagName(oldName, newName, (_tagItem.ParentCollection.Scope.Tags))
                    ? newName
                    : oldName;

            if (_tagItem.ParentCollection.Scope is Controller)
                _tagItem.Tag.Name = IsValidTagName(oldName, newName, (_tagItem.ParentCollection.Controller.Tags))
                    ? newName
                    : oldName;

            if (_tagItem.ParentCollection.Scope is Program)
                _tagItem.Tag.Name = IsValidTagName(oldName, newName,
                    (_tagItem.ParentCollection as MonitorTagCollection)?[0].ParentCollection.Scope.Tags)
                    ? newName
                    : oldName;

            return _tagItem.Tag.Name;
        }

        public void UpdateReadOnly()
        {
            SetPropertyReadOnly(this, "ItemName", IsItemNameReadOnly());
            SetPropertyReadOnly(this, "Description", IsDescriptionReadOnly());
            SetPropertyReadOnly(this, "Usage", IsUsageReadOnly());
            SetPropertyReadOnly(this, "Type", IsTypeReadOnly());
            SetPropertyReadOnly(this, "DataType", IsDataTypeReadOnly());
            SetPropertyReadOnly(this, "SelectDataTypeCommand", IsDataTypeReadOnly());
            SetPropertyReadOnly(this, "AliasFor", IsAliasForReadOnly());
            SetPropertyReadOnly(this, "ExternalAccess", IsExternalAccessReadOnly());
            SetPropertyReadOnly(this, "Style", IsStyleReadOnly());
            SetPropertyReadOnly(this, "Constant", IsConstantReadOnly());
            SetPropertyReadOnly(this, "Required", IsRequiredReadOnly());
            SetPropertyReadOnly(this, "Visible", IsVisibleReadOnly());
            SetPropertyReadOnly(this, "Value", IsValueReadOnly());
            SetPropertyReadOnly(this, "Producer", IsProducerReadOnly());
            SetPropertyReadOnly(this, "ParameterConnections", IsParameterConnectionsReadOnly()); 
        }

        private bool IsDescriptionReadOnly()
        {
            return _controller.IsOnline;
        }

        private bool IsParameterConnectionsReadOnly()
        {
            return _controller.IsOnline;
        }

        private bool IsProducerReadOnly()
        {
            return _controller.IsOnline;
        }

        private bool IsDataTypeReadOnly()
        {
            if (_controller.IsOnline)
                return true;

            if (_tagItem.ParentItem != null)
            {
                return true;
            }

            if (_tagItem.Tag.DataTypeInfo.DataType.IsIOType &&
                _tagItem.ParentCollection.Scope is IController)
            {
                return true;
            }

            if ((_tagItem.ParentCollection.Scope is AoiDefinition) &&
                (_tagItem.Tag.Name == "EnableOut" || _tagItem.Tag.Name == "EnableIn"))
            {
                return true;
            }

            return false;
        }

        private bool IsValueReadOnly()
        {
            ////同Style的限制一样
            //if ((_tagItem.ParentCollection.Controller.IsOnline &&
            //     _tagItem.ParentCollection.Scope is AoiDefinition) ||
            //    (_tagItem.Tag.DataTypeInfo.DataType.IsIOType ||
            //     _tagItem.Tag.DataTypeInfo.DataType.IsAxisType))
            //{
            //    return true;
            //}

            if (_controller.IsOnline)
                return true;

            if ((_tagItem.ParentCollection.Scope is AoiDefinition) &&
                (_tagItem.Tag.Name == "EnableOut" || _tagItem.Tag.Name == "EnableIn"))
            {
                return true;
            }

            if (_tagItem.Tag.DataTypeInfo.DataType.IsStruct &&
                _tagItem.Tag.DataTypeInfo.DataType.IsPredefinedType)
            {
                return true;
            }

            if (_tagItem.Tag.DataTypeInfo.DataType.IsStringType)
            {
                return true;
            }

            return false;
        }

        private bool IsVisibleReadOnly()
        {
            if (_tagItem.ParentItem != null)
            {
                return true;
            }

            //if ((_tagItem.ParentCollection.Controller.IsOnline && _tagItem.ParentCollection.Scope is AoiDefinition))
            //{
            //    return true;
            //}

            if (_controller.IsOnline)
                return true;

            //Editor和Monitor模式下一样
            if (_tagItem.ParentCollection.Scope is AoiDefinition)
            {
                return true;
            }

            return true;
        }

        private bool IsRequiredReadOnly()
        {
            //if (_tagItem.ParentCollection.Controller.IsOnline && _tagItem.ParentCollection.Scope is AoiDefinition)
            //{
            //    return true;
            //}

            if (_controller.IsOnline)
                return true;

            //Editor和Monitor模式下一样 
            if (_tagItem.ParentCollection.Scope is AoiDefinition && !_tagItem.Tag.DataTypeInfo.DataType.IsBool &&
                (_tagItem.Usage == Interfaces.Tags.Usage.Input || _tagItem.Usage == Interfaces.Tags.Usage.Output))
            {
                return false;
            }

            return true;
        }

        private bool IsUsageReadOnly()
        {
            //if (_tagItem.ParentCollection.Controller.IsOnline && _tagItem.ParentCollection.Scope is AoiDefinition)
            //{
            //    return true;
            //}

            if (_controller.IsOnline)
                return true;

            if (_selectedIndex == 1)
            {
                if (_tagItem.ParentCollection.Scope is IController)
                {
                    return true;
                }

                if ((_tagItem.ParentCollection.Scope is AoiDefinition) &&
                    (_tagItem.Tag.Name == "EnableOut" || _tagItem.Tag.Name == "EnableIn"))
                {
                    return true;
                }

                return false;
            }

            return true;
        }

        private bool IsConstantReadOnly()
        {
            //if (_tagItem.ParentCollection.Controller.IsOnline && _tagItem.ParentCollection.Scope is AoiDefinition)
            //{
            //    return true;
            //}

            if (_controller.IsOnline)
                return true;

            //Editor模式下
            if (_selectedIndex == 1)
            {
                if (_tagItem.ParentItem != null)
                {
                    return true;
                }

                if (_tagItem.ParentCollection.Scope is AoiDefinition ||
                    _tagItem.Tag.DataTypeInfo.DataType.IsAxisType)
                {
                    return true;
                }

                if (_tagItem.Tag.DataTypeInfo.DataType.IsMotionGroupType)
                {
                    return true;
                }

                return false;
            }

            //Monitor模式下一直为只读
            return true;
        }

        private bool IsStyleReadOnly()
        {
            //if (_tagItem.ParentCollection.Controller.IsOnline && _tagItem.ParentCollection.Scope is AoiDefinition)
            //{
            //    return true;
            //}

            if (_controller.IsOnline)
                return true;

            if (_tagItem.ParentItem != null)
            {
                return true;
            }

            //Editor模式下 和Monitor下一样的限制
            //当数据类型是CAM的时候 也要返回true 暂时没找到单独的CAM类型 用IsPredefinedType类型
            if (_tagItem.Tag.DataTypeInfo.DataType.IsIOType ||
                _tagItem.Tag.DataTypeInfo.DataType.IsAxisType)
            {
                return true;
            }

            if ((_tagItem.ParentCollection.Scope is AoiDefinition) &&
                (_tagItem.Tag.Name == "EnableOut" || _tagItem.Tag.Name == "EnableIn"))
            {
                return true;
            }

            if (_tagItem.Tag.DataTypeInfo.DataType.IsStruct && _tagItem.Tag.DataTypeInfo.DataType.IsPredefinedType)
            {
                return true;
            }

            return false;
        }

        private bool IsExternalAccessReadOnly()
        {
            //if (_tagItem.ParentCollection.Controller.IsOnline && _tagItem.ParentCollection.Scope is AoiDefinition)
            //{
            //    return true;
            //}

            if (_controller.IsOnline)
                return true;

            //Editor
            if (_selectedIndex == 1)
            {
                if ((_tagItem.ParentCollection.Scope is AoiDefinition) &&
                    (_tagItem.Tag.Name == "EnableOut" || _tagItem.Tag.Name == "EnableIn"))
                {
                    return true;
                }

                if (_tagItem.ParentCollection.Scope is AoiDefinition &&
                    (_tagItem.Usage == Interfaces.Tags.Usage.InOut ||
                     _tagItem.Usage == Interfaces.Tags.Usage.Input))
                {
                    return true;
                }

                if (_tagItem.ParentItem != null)
                {
                    return true;
                }

                return false;
            }

            return true;
        }

        private bool IsAliasForReadOnly()
        {
            //if (_tagItem.ParentCollection.Controller.IsOnline && _tagItem.ParentCollection.Scope is AoiDefinition)
            //{
            //    return true;
            //}

            if (_tagItem.ParentCollection.Controller.IsOnline)
            {
                return true;
            }

            //Editor模式下 和Monitor下一样的限制
            if (_tagItem.ParentCollection.Scope is AoiDefinition || _tagItem.Tag.DataTypeInfo.DataType.IsIOType)
            {
                return true;
            }

            return false;
        }

        private bool IsTypeReadOnly()
        {
            //Editor模式下 和Monitor下一样的限制
            if (_tagItem.ParentCollection.Controller.IsOnline)
            {
                return true;
            }

            if (_tagItem.ParentItem != null)
            {
                return true;
            }

            if (_tagItem.Tag.Usage == Interfaces.Tags.Usage.InOut ||
                _tagItem.Tag.DataTypeInfo.DataType.IsIOType)
            {
                return true;
            }

            if ((_tagItem.ParentCollection.Scope is AoiDefinition) &&
                (_tagItem.Tag.Name == "EnableOut" || _tagItem.Tag.Name == "EnableIn"))
            {
                return true;
            }

            if ((_tagItem.ParentCollection.Scope is AoiDefinition) &&
                _tagItem.Tag.DataTypeInfo.DataType.IsPredefinedType)
            {
                return true;
            }

            return false;
        }

        private bool IsItemNameReadOnly()
        {
            //if (_tagItem.ParentCollection.Controller.IsOnline && _tagItem.ParentCollection.Scope is AoiDefinition)
            //{
            //    return true;
            //}

            if (_tagItem.ParentCollection.Controller.IsOnline)
            {
                return true;
            }

            //Editor模式下 和Monitor下一样的限制
            if (_tagItem.Tag.DataTypeInfo.DataType.IsIOType ||
                ((_tagItem.ParentCollection.Scope is AoiDefinition) &&
                 (_tagItem.Tag.Name == "EnableOut" || _tagItem.Tag.Name == "EnableIn")))
                return true;

            if (_tagItem.ParentItem != null ||
                _tagItem.Tag.DataTypeInfo.DataType.IsVendorDefinedType)
                return true;

            return false;
        }


        private bool IsValidTagName(string oldName, string newName, ITagCollection tagCollection)
        {
            string warningMessage = "Failed to modify the properties for the tag '" + oldName + "'.";
            string warningReason = string.Empty;
            bool isValid = true;

            // Empty Or Null
            if (string.IsNullOrEmpty(newName))
            {
                isValid = false;
                warningReason = LanguageManager.GetInstance().ConvertSpecifier("VariableNameInvalid");
            }

            // Format
            if (isValid)
            {
                var regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
                if (!regex.IsMatch(newName))
                {
                    isValid = false;
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("VariableNameInvalid");
                }
            }

            // Key Word
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
                    if (keyWord.Equals(newName, StringComparison.OrdinalIgnoreCase))
                    {
                        isValid = false;
                        warningReason = LanguageManager.GetInstance().ConvertSpecifier("VariableNameInvalid");
                    }
                }
            }

            // Exists
            if (isValid)
            {
                //TODO:(tlm)可能会有问题
                if (tagCollection[newName] != null)
                {
                    isValid = false;
                    warningReason = "Object by the same name already exists in this collection.";
                }
            }

            // Error
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
                    _itemName = _tagItem.Name;
                    OnPropertyChanged(nameof(ItemName));
                    break;

                case "Description":
                    _description = _tagItem.Description;
                    OnPropertyChanged(nameof(Description));
                    break;

                case "DataType":
                    _dataType = _tagItem.DataType;
                    OnPropertyChanged(nameof(DataType));
                    break;

                case "DisplayStyle":
                    _style = _tagItem.DisplayStyle;
                    OnPropertyChanged(nameof(Style));
                    OnPropertyChanged(nameof(Value));
                    break;

                case "Value":
                    OnPropertyChanged(nameof(Value));
                    break;
            }
        }

        private void OnTagChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_tagItem.Tag != null)
            {
                switch (e.PropertyName)
                {
                    case "Usage":
                        _usage = _tagItem.Tag.Usage;
                        OnPropertyChanged(nameof(Usage));
                        UpdateReadOnly();
                        break;

                    case "TagType":
                        _type = _tagItem.Tag.TagType;
                        OnPropertyChanged(nameof(Type));
                        UpdateReadOnly();
                        break;

                    case "DataWrapper":
                        _dataType = _tagItem.Tag.DataTypeInfo.DataType.Name;
                        OnPropertyChanged(nameof(DataType));
                        break;

                    case "ExternalAccess":
                        _externalAccess = _tagItem.Tag.ExternalAccess;
                        OnPropertyChanged(nameof(ExternalAccess));
                        break;

                    case "IsConstant":
                        _constant = _tagItem.Tag.IsConstant ? PropertiesYesNo.Yes : PropertiesYesNo.No;
                        OnPropertyChanged(nameof(Constant));
                        OnPropertyChanged(nameof(IsPropertiesConstantVisibility));
                        UpdateReadOnly();
                        break;

                    case "IsVisible":
                        _visible = _tagItem.Tag.IsVisible ? PropertiesYesNo.Yes : PropertiesYesNo.No;
                        ;
                        OnPropertyChanged(nameof(Visible));
                        UpdateReadOnly();
                        break;

                    case "IsRequired":
                        _required = _tagItem.Tag.IsRequired ? PropertiesYesNo.Yes : PropertiesYesNo.No;
                        ;
                        OnPropertyChanged(nameof(Required));
                        break;
                }
            }
        }

        public void SetTagValue(object value)
        {
            if (_tagItem.ItemType == TagItemType.String)
            {
                // string and udt-string
                var compositeField = (ICompositeField) _tagItem.Tag.DataWrapper.Data;

                var byteList = ValueConverter.ToBytes(value.ToString());

                if (!compositeField.EqualsByteList(byteList))
                {
                    if (_tagItem.ParentCollection.Controller.IsOnline)
                    {
                        SetTagValueToPLC(_tagItem.Tag, ItemName, value.ToString());
                    }
                    else
                    {
                        compositeField.Set(byteList);
                        _tagItem.Tag?.RaisePropertyChanged(ItemName);

                        //TODO(gjc): add code for string
                    }
                }
            }
            else if (_tagItem.ItemType == TagItemType.BoolArrayMember)
            {
                var boolValue = ValueConverter.ToBoolean(value.ToString());
                var boolArrayField = (BoolArrayField) _tagItem.Tag.DataWrapper.Data;
                if (boolArrayField.Get(_tagItem.MemberIndex) != boolValue)
                {
                    if (_tagItem.ParentCollection.Controller.IsOnline)
                    {
                        SetTagValueToPLC(_tagItem.Tag, ItemName, value.ToString());
                    }
                    else
                    {
                        boolArrayField.Set(_tagItem.MemberIndex, boolValue);

                        Notifications.SendNotificationData(new TagNotificationData()
                        {
                            Tag = _tagItem.Tag,
                            Type = TagNotificationData.NotificationType.Value,
                            Field = boolArrayField,
                            IsBit = true,
                            BitOffset = _tagItem.MemberIndex
                        });
                    }
                }
            }
            else if (_tagItem.ItemType == TagItemType.IntegerMember
                     || _tagItem.ItemType == TagItemType.BitMember)
            {
                int bitOffset = _tagItem.BitOffset;

                bool boolValue = ValueConverter.ToBoolean(value.ToString());

                var boolField = _tagItem.Tag.DataWrapper.Data as BoolField;
                if (boolField != null)
                {
                    bool oldValue = boolField.value == 1;
                    if (oldValue != boolValue)
                    {
                        if (_tagItem.ParentCollection.Controller.IsOnline)
                        {
                            SetTagValueToPLC(_tagItem.Tag, ItemName, value.ToString());
                        }
                        else
                        {
                            boolField.value = (byte) (boolValue ? 1 : 0);

                            Notifications.SendNotificationData(new TagNotificationData()
                            {
                                Tag = _tagItem.Tag,
                                Type = TagNotificationData.NotificationType.Value,
                                Field = boolField,
                                IsBit = true,
                                BitOffset = bitOffset
                            });
                        }
                    }
                }

                var int8Field = _tagItem.Tag.DataWrapper.Data as Int8Field;
                if (int8Field != null)
                {
                    if (BitValue.Get(int8Field.value, bitOffset) != boolValue)
                    {
                        if (_tagItem.ParentCollection.Controller.IsOnline)
                        {
                            SetTagValueToPLC(_tagItem.Tag, ItemName, value.ToString());
                        }
                        else
                        {
                            sbyte newValue = BitValue.Set(int8Field.value, bitOffset, boolValue);
                            int8Field.value = newValue;

                            Notifications.SendNotificationData(new TagNotificationData()
                            {
                                Tag = _tagItem.Tag,
                                Type = TagNotificationData.NotificationType.Value,
                                Field = int8Field,
                                IsBit = true,
                                BitOffset = bitOffset
                            });
                        }
                    }
                }

                var int16Field = _tagItem.Tag.DataWrapper.Data as Int16Field;
                if (int16Field != null)
                {
                    if (BitValue.Get(int16Field.value, bitOffset) != boolValue)
                    {
                        if (_tagItem.ParentCollection.Controller.IsOnline)
                        {
                            SetTagValueToPLC(_tagItem.Tag, ItemName, value.ToString());
                        }
                        else
                        {
                            short newValue = BitValue.Set(int16Field.value, bitOffset, boolValue);
                            int16Field.value = newValue;

                            Notifications.SendNotificationData(new TagNotificationData()
                            {
                                Tag = _tagItem.Tag,
                                Type = TagNotificationData.NotificationType.Value,
                                Field = int16Field,
                                IsBit = true,
                                BitOffset = bitOffset
                            });
                        }
                    }
                }

                var int32Field = _tagItem.Tag.DataWrapper.Data as Int32Field;
                if (int32Field != null)
                {
                    if (BitValue.Get(int32Field.value, bitOffset) != boolValue)
                    {
                        if (_tagItem.ParentCollection.Controller.IsOnline)
                        {
                            SetTagValueToPLC(_tagItem.Tag, ItemName, value.ToString());
                        }
                        else
                        {
                            int newValue = BitValue.Set(int32Field.value, bitOffset, boolValue);
                            int32Field.value = newValue;

                            Notifications.SendNotificationData(new TagNotificationData()
                            {
                                Tag = _tagItem.Tag,
                                Type = TagNotificationData.NotificationType.Value,
                                Field = int32Field,
                                IsBit = true,
                                BitOffset = bitOffset
                            });
                        }
                    }
                }

                var int64Field = _tagItem.Tag.DataWrapper.Data as Int64Field;
                if (int64Field != null)
                {
                    if (BitValue.Get(int64Field.value, bitOffset) != boolValue)
                    {
                        if (_tagItem.ParentCollection.Controller.IsOnline)
                        {
                            SetTagValueToPLC(_tagItem.Tag, ItemName, value.ToString());
                        }
                        else
                        {
                            long newValue = BitValue.Set(int64Field.value, bitOffset, boolValue);
                            int64Field.value = newValue;

                            Notifications.SendNotificationData(new TagNotificationData()
                            {
                                Tag = _tagItem.Tag,
                                Type = TagNotificationData.NotificationType.Value,
                                Field = int64Field,
                                IsBit = true,
                                BitOffset = bitOffset
                            });
                        }
                    }
                }
            }
            else if (_tagItem.ItemType == TagItemType.Default || _tagItem.ItemType == TagItemType.Integer)
            {
                var boolField = _tagItem.Tag.DataWrapper.Data as BoolField;
                if (boolField != null)
                {
                    bool boolValue = ValueConverter.ToBoolean(value.ToString());
                    byte byteValue = (byte) (boolValue ? 1 : 0);
                    if (boolField.value != byteValue)
                    {
                        if (_tagItem.ParentCollection.Controller.IsOnline)
                        {
                            SetTagValueToPLC(_tagItem.Tag, ItemName, value.ToString());
                        }
                        else
                        {
                            boolField.value = byteValue;

                            Notifications.SendNotificationData(new TagNotificationData()
                            {
                                Tag = _tagItem.Tag,
                                Type = TagNotificationData.NotificationType.Value,
                                Field = boolField
                            });
                        }
                    }
                }

                var int8Field = _tagItem.Tag.DataWrapper.Data as Int8Field;
                if (int8Field != null)
                {
                    var sbyteValue = ValueConverter.ToSByte(value.ToString(), _tagItem.Tag.DisplayStyle);
                    if (int8Field.value != sbyteValue)
                    {
                        if (_tagItem.ParentCollection.Controller.IsOnline)
                        {
                            SetTagValueToPLC(_tagItem.Tag, ItemName, sbyteValue.ToString());
                        }
                        else
                        {
                            int8Field.value = sbyteValue;

                            Notifications.SendNotificationData(new TagNotificationData()
                            {
                                Tag = _tagItem.Tag,
                                Type = TagNotificationData.NotificationType.Value,
                                Field = int8Field
                            });
                        }
                    }
                }

                var int16Field = _tagItem.Tag.DataWrapper.Data as Int16Field;
                if (int16Field != null)
                {
                    var shortValue = ValueConverter.ToShort(value.ToString(), _tagItem.Tag.DisplayStyle);
                    if (int16Field.value != shortValue)
                    {
                        if (_tagItem.ParentCollection.Controller.IsOnline)
                        {
                            SetTagValueToPLC(_tagItem.Tag, ItemName, shortValue.ToString());
                        }
                        else
                        {
                            int16Field.value = shortValue;

                            Notifications.SendNotificationData(new TagNotificationData()
                            {
                                Tag = _tagItem.Tag,
                                Type = TagNotificationData.NotificationType.Value,
                                Field = int16Field
                            });
                        }
                    }
                }

                var int32Field = _tagItem.Tag.DataWrapper.Data as Int32Field;
                if (int32Field != null)
                {
                    var intValue = ValueConverter.ToInt(value.ToString(), _tagItem.Tag.DisplayStyle);
                    if (int32Field.value != intValue)
                    {
                        if (_tagItem.ParentCollection.Controller.IsOnline)
                        {
                            SetTagValueToPLC(_tagItem.Tag, ItemName, intValue.ToString());
                        }
                        else
                        {
                            int32Field.value = intValue;

                            Notifications.SendNotificationData(new TagNotificationData()
                            {
                                Tag = _tagItem.Tag,
                                Type = TagNotificationData.NotificationType.Value,
                                Field = int32Field
                            });
                        }
                    }
                }

                var int64Field = _tagItem.Tag.DataWrapper.Data as Int64Field;
                if (int64Field != null)
                {
                    var longValue = ValueConverter.ToLong(value.ToString(), _tagItem.Tag.DisplayStyle);
                    if (int64Field.value != longValue)
                    {
                        if (_tagItem.ParentCollection.Controller.IsOnline)
                        {
                            SetTagValueToPLC(_tagItem.Tag, ItemName, longValue.ToString());
                        }
                        else
                        {
                            int64Field.value = longValue;

                            Notifications.SendNotificationData(new TagNotificationData()
                            {
                                Tag = _tagItem.Tag,
                                Type = TagNotificationData.NotificationType.Value,
                                Field = int64Field
                            });
                        }
                    }
                }

                var realField = _tagItem.Tag.DataWrapper.Data as RealField;
                if (realField != null)
                {
                    var floatValue = ValueConverter.ToFloat(value.ToString());
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (realField.value != floatValue)
                    {
                        if (_tagItem.ParentCollection.Controller.IsOnline)
                        {
                            SetTagValueToPLC(_tagItem.Tag, ItemName, floatValue.ToString("g9"));
                        }
                        else
                        {
                            realField.value = floatValue;

                            Notifications.SendNotificationData(new TagNotificationData()
                            {
                                Tag = _tagItem.Tag,
                                Type = TagNotificationData.NotificationType.Value,
                                Field = realField
                            });
                        }
                    }
                }
            }

            OnPropertyChanged();
        }

        private object GetValue()
        {
            if (_tagItem.Usage == Interfaces.Tags.Usage.InOut) return string.Empty;

            if (_tagItem.ItemType == TagItemType.Array ||
                _tagItem.ItemType == TagItemType.Struct)
            {
                _value = new StructOrArrayValue();
                return _value;
            }

            if (_tagItem.ItemType == TagItemType.String)
            {
                // string and udt-string
                var compositeField = (ICompositeField) _tagItem.DataField;
                if (compositeField != null)
                {
                    _value = compositeField.ToString(_tagItem.DisplayStyle);
                    return _value;
                }
            }

            // bool array
            if (_tagItem.ItemType == TagItemType.BoolArrayMember)
            {
                var boolArrayField = (BoolArrayField) _tagItem.DataField;
                _value = boolArrayField.Get(_tagItem.MemberIndex);
                return _value;
            }

            if (_tagItem.ItemType == TagItemType.IntegerMember
                || _tagItem.ItemType == TagItemType.BitMember)
            {
                int bitOffset = _tagItem.BitOffset;

                var boolField = _tagItem.DataField as BoolField;
                if (boolField != null)
                {
                    Contract.Assert(bitOffset == 0);

                    _value = boolField.value != 0;
                    return _value;
                }

                var int8Field = _tagItem.DataField as Int8Field;
                if (int8Field != null)
                {
                    _value = BitValue.Get(int8Field.value, bitOffset);
                    return _value;
                }

                var int16Field = _tagItem.DataField as Int16Field;
                if (int16Field != null)
                {
                    _value = BitValue.Get(int16Field.value, bitOffset);
                    return _value;
                }

                var int32Field = _tagItem.DataField as Int32Field;
                if (int32Field != null)
                {
                    _value = BitValue.Get(int32Field.value, bitOffset);
                    return _value;
                }

                var int64Field = _tagItem.DataField as Int64Field;
                if (int64Field != null)
                {
                    _value = BitValue.Get(int64Field.value, bitOffset);
                    return _value;
                }
            }

            if (_tagItem.ItemType == TagItemType.Default || _tagItem.ItemType == TagItemType.Integer)
            {
                var boolField = _tagItem.DataField as BoolField;
                if (boolField != null)
                {
                    _value = boolField.value != 0;
                    return _value;
                }

                var int8Field = _tagItem.DataField as Int8Field;
                if (int8Field != null)
                {
                    _value = int8Field.value;
                    return _value;
                }

                var int16Field = _tagItem.DataField as Int16Field;
                if (int16Field != null)
                {
                    _value = int16Field.value;
                    return _value;
                }

                var int32Field = _tagItem.DataField as Int32Field;
                if (int32Field != null)
                {
                    _value = int32Field.value;
                    return _value;
                }

                var int64Field = _tagItem.DataField as Int64Field;
                if (int64Field != null)
                {
                    _value = int64Field.value;
                    return _value;
                }

                var realField = _tagItem.DataField as RealField;
                if (realField != null)
                {
                    _value = realField.value;
                    return _value;
                }
            }

            _value = "TODO";
            return _value;
        }


        private void SetTagValueToPLC(Tag tag, string specifier, string value)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await TaskScheduler.Default;
                await ((Controller) _tagItem.Tag.ParentController).SetTagValueToPLC(tag, specifier, value)
                    .ConfigureAwait(true);
            });
        }
    }
}
