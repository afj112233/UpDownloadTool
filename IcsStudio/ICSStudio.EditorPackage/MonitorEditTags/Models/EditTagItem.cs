using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Dialogs.SelectDataType;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Notification;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;

namespace ICSStudio.EditorPackage.MonitorEditTags.Models
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    [SuppressMessage("ReSharper", "UseNameofExpression")]
    internal partial class EditTagItem : TagItem, IEditableObject
    {
        private EditTagItem _copyItem;

        private string _description;

        private DisplayStyle _displayStyle;

        private ExternalAccess _externalAccess;

        private bool _inTxn;

        public EditTagItem()
        {
            SelfProperty = this;

            Name = string.Empty;
            ParentItem = null;
            ParentCollection = null;
            Children = null;

            //DisplayStyle = DisplayStyle.Binary;
            //ExternalAccess = ExternalAccess.NullExternalAccess;


            //base.Usage = Usage.Empty;
            _displayStyle = DisplayStyle.Decimal;
            _externalAccess = ExternalAccess.ReadWrite;

            DataType = "DINT";
            base.Usage = Usage.Local;

            ExternalAccessSource = EnumHelper.ToDataSource<ExternalAccess>(new List<ExternalAccess>
            {
                ExternalAccess.ReadWrite,
                ExternalAccess.ReadOnly,
                ExternalAccess.None
            });

            ChangeDataTypeCommand = new RelayCommand(ExecuteChangeDataTypeCommand);
        }

        // for create
        public IController Controller { get; set; }
        public ITagCollectionContainer Scope { get; set; }

        public string AliasFor
        {
            get
            {
                if (ParentItem == null)
                {
                    if (Tag != null && Tag.IsAlias && Tag.IsVerified)
                    {
                        return Tag.AliasSpecifier;
                    }
                }

                return string.Empty;
            }
        }

        public string BaseTag
        {
            get
            {
                if (ParentItem == null)
                {
                    if (Tag != null && Tag.IsAlias && Tag.IsVerified)
                    {
                        return Tag.AliasBaseSpecifier;
                    }
                }

                return string.Empty;
            }
        }

        public override DisplayStyle DisplayStyle
        {
            get { return _displayStyle; }
            set
            {
                if (_displayStyle != value)
                {
                    _displayStyle = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<DisplayStyle> DisplayStyleSource
        {
            get
            {
                IDataType dataType = DataTypeInfo.DataType;

                if (dataType == null)
                {
                    if (Controller != null) dataType = Controller.DataTypes[DataType];

                    if (dataType == null)
                        return null;
                }

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
            }
        }

        public override ExternalAccess ExternalAccess
        {
            get { return _externalAccess; }
            set
            {
                if (_externalAccess != value)
                {
                    _externalAccess = value;
                    OnPropertyChanged();
                }
            }
        }

        public IList ExternalAccessSource { get; }

        private IList _usageSource;

        public IList UsageSource => _usageSource ?? (_usageSource = CreateUsageSource());

        public EditTagItem SelfProperty { get; private set; }

        public bool IsConstantEnabled
        {
            get
            {
                //TODO(gjc): edit later
                if (Tag != null && Tag.ParentController.IsOnline)
                    return false;
                //end

                IDataType dataType = DataTypeInfo.DataType;

                if (IsConstantVisibility == Visibility.Visible)
                {

                    if (Tag?.ParentCollection?.ParentProgram is AoiDefinition)
                    {
                        if (Usage != Usage.InOut)
                            return false;
                    }


                    if (dataType != null)
                        if (dataType.IsMotionGroupType ||
                            dataType.IsAxisType)
                            return false;


                    return true;
                }

                return false;


            }
        }

        public bool IsNameEnabled
        {
            get
            {
                //TODO(gjc): edit later
                if (Tag != null && Tag.ParentController.IsOnline)
                    return false;
                //end

                if ((Controller ?? SimpleServices.Common.Controller.GetInstance())?.KeySwitchPosition == ControllerKeySwitch.RunKeySwitch) return false;

                if (IsInAOI)
                {
                    if (Name.Equals("EnableIn", StringComparison.OrdinalIgnoreCase) ||
                        Name.Equals("EnableOut", StringComparison.OrdinalIgnoreCase))
                        return false;

                    if (Tag != null && Tag.ParentController.IsOnline)
                        return false;
                }

                if (ParentItem != null)
                    return false;

                if (Tag != null && Tag.DataTypeInfo.DataType.IsVendorDefinedType)
                    return false;

                if (Tag != null && Tag.IsModuleTag)
                    return false;

                return true;
            }
        }

        public bool IsUsageEnabled
        {
            get
            {
                if (IsInAOI)
                {
                    if (Name.Equals("EnableIn", StringComparison.OrdinalIgnoreCase) ||
                        Name.Equals("EnableOut", StringComparison.OrdinalIgnoreCase))
                        return false;

                    if (Tag != null && Tag.ParentController.IsOnline)
                        return false;
                }

                if (ParentItem != null)
                    return false;

                return true;
            }
        }

        public bool IsDataTypeEnabled
        {
            get
            {
                if (IsInAOI)
                {
                    if (Name.Equals("EnableIn", StringComparison.OrdinalIgnoreCase) ||
                        Name.Equals("EnableOut", StringComparison.OrdinalIgnoreCase))
                        return false;

                    if (Tag != null && Tag.ParentController.IsOnline)
                        return false;
                }

                if (ParentItem != null)
                    return false;

                if (Tag != null && Tag.DataTypeInfo.DataType.IsVendorDefinedType)
                    return false;

                if (Tag != null && Tag.IsModuleTag)
                    return false;

                return true;
            }
        }

        public bool IsExternalAccessEnabled
        {
            get
            {
                //TODO(gjc): edit later
                if (Tag != null && Tag.ParentController.IsOnline)
                    return false;
                //end

                if (IsInAOI)
                {
                    if (Name.Equals("EnableIn", StringComparison.OrdinalIgnoreCase) ||
                        Name.Equals("EnableOut", StringComparison.OrdinalIgnoreCase))
                        return false;

                    if (Tag != null && Tag.ParentController.IsOnline)
                        return false;

                    if (Usage == Usage.InOut)
                        return false;
                }

                if (ParentItem != null)
                    return false;

                return true;
            }
        }

        public bool IsDisplayStyleEnabled
        {
            get
            {
                //TODO(gjc): edit later
                if (Tag != null && Tag.ParentController.IsOnline)
                    return false;
                //end

                if (IsInAOI)
                {
                    if (Name.Equals("EnableIn", StringComparison.OrdinalIgnoreCase) ||
                        Name.Equals("EnableOut", StringComparison.OrdinalIgnoreCase))
                        return false;
                }

                if (ParentItem != null)
                    return false;


                return true;
            }
        }

        public bool IsInAOI
        {
            get
            {
                if (Tag != null)
                {
                    return Tag.ParentCollection.ParentProgram is AoiDefinition;
                }

                return Scope is AoiDefinition;
            }
        }

        public RelayCommand ChangeDataTypeCommand { get; }



        public void BeginEdit()
        {
            if (!_inTxn)
            {
                if (_copyItem == null) _copyItem = new EditTagItem();
                _copyItem.Name = Name;
                _copyItem.Usage = Usage;
                _copyItem.DataType = DataType;
                _copyItem.Description = Description;
                _copyItem.ExternalAccess = ExternalAccess;
                _copyItem.IsConstant = IsConstant;
                _copyItem.DisplayStyle = DisplayStyle;
                _inTxn = true;
                //CanChange = true;
            }
        }

        public void EndEdit()
        {
            var dataTypeChanged = false;
            var tagNameChanged = false;
            var usageChanged = false;
            if (_inTxn)
            {
                if (Tag != null)
                {
                    var controller = ParentCollection?.Controller ?? Controller;
                    Contract.Assert(controller != null);

                    var newDataTypeInfo = controller.DataTypes.ParseDataTypeInfo(DataType);
                    var oldDataTypeInfo = controller.DataTypes.ParseDataTypeInfo(_copyItem.DataType);

                    if (newDataTypeInfo != oldDataTypeInfo)
                    {
                        // change data type
                        if (DataHelper.IsTagDataTruncatedOrLost(Tag.DataWrapper, newDataTypeInfo))
                            if (MessageBox.Show("Tag data will be truncated/lost.", "ICS Studio",
                                    MessageBoxButton.OKCancel,
                                    MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                            {
                                CancelEdit();
                                return;
                            }

                        ChangeDataType(newDataTypeInfo);

                        dataTypeChanged = true;
                    }

                    if (!string.Equals(Name, _copyItem.Name))
                    {
                        Tag.Name = Name;
                        
                        tagNameChanged = true;
                    }

                    if (Usage != _copyItem.Usage)
                    {
                        Tag.Usage = Usage;

                        bool isInAOI = Tag.ParentCollection.ParentProgram is AoiDefinition;
                        Tag.ExternalAccess = GetDefaultExternalAccess(Usage, isInAOI);
                        ExternalAccess = Tag.ExternalAccess;

                        if (Usage == Usage.InOut)
                        {
                            Tag.IsRequired = true;
                            Tag.IsVisible = true;
                        }

                        UpdateSelfProperty();

                        usageChanged = true;
                    }

                    if (!string.Equals(Description, _copyItem.Description))
                    {
                        //TODO(gjc): add code here
                        //ignore
                    }

                    if (ExternalAccess != _copyItem.ExternalAccess) Tag.ExternalAccess = ExternalAccess;

                    if (IsConstant != _copyItem.IsConstant) Tag.IsConstant = IsConstant;

                    if (DisplayStyle != _copyItem.DisplayStyle) Tag.DisplayStyle = DisplayStyle;
                }
                else
                {
                    // create a new tag
                    var newTag = CreateNewTag();
                    var newItem = TagToEditTagItem(newTag);
                    Tag = newItem.Tag;
                    Name = newItem.Name;
                    DataType = newItem.DataType;
                    DisplayStyle = newItem.DisplayStyle;
                    ItemType = newItem.ItemType;
                    DataTypeInfo = newItem.DataTypeInfo;
                    DataField = newItem.DataField;
                    newItem.Cleanup();

                    OnPropertyChanged("HasChildren");
                }

                if (IsInAOI)
                {
                    AoiDefinition aoi;

                    if (Tag != null)
                    {
                        aoi = Tag.ParentCollection.ParentProgram as AoiDefinition;
                    }
                    else
                    {
                        aoi = Scope as AoiDefinition;
                    }

                    if (aoi != null)
                    {
                        if(dataTypeChanged || tagNameChanged || usageChanged)
                        {
                            aoi.datatype.Reset();
                            CodeSynchronization.GetInstance().Update();
                        }
                    }
                    
                }

                _inTxn = false;
            }


            if (dataTypeChanged)
            {
                var parentCollection = ParentCollection as EditTagCollection;
                Contract.Assert(parentCollection != null);

                int index = parentCollection.IndexOf(this);
                var newItem = TagToEditTagItem(Tag);

                parentCollection.RemoveEditTagItem(this);

                newItem.ParentCollection = parentCollection;
                parentCollection.Insert(index, newItem);

            }
        }

        private ExternalAccess GetDefaultExternalAccess(Usage usage, bool isInAOI)
        {
            if (isInAOI)
            {
                switch (usage)
                {
                    case Usage.Input:
                        return ExternalAccess.ReadWrite;
                    case Usage.Output:
                        return ExternalAccess.ReadWrite;
                    case Usage.InOut:
                        return ExternalAccess.NullExternalAccess;
                    case Usage.Local:
                        return ExternalAccess.None;
                    default:
                        throw new NotImplementedException();
                }
            }

            return ExternalAccess.ReadWrite;

        }

        private Tag CreateNewTag()
        {
            string typeName;
            int dim1, dim2, dim3;
            int errorCode;

            var controller = ParentCollection?.Controller ?? Controller;
            Contract.Assert(controller != null);

            var tagCollection = Scope.Tags as TagCollection;
            Contract.Assert(tagCollection != null);

            controller.DataTypes.ParseDataType(
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
            tag.TagType = TagType.Base;
            tag.ExternalAccess = ExternalAccess;
            tag.DisplayStyle = GetDefaultDisplayStyle(tag.DataTypeInfo.DataType);

            if (IsInAOI)
            {
                if (Usage == Usage.InOut)
                {
                    tag.IsRequired = true;
                    tag.IsVisible = true;
                }
            }

            tagCollection.AddTag(tag, tagCollection.ParentProgram is AoiDefinition, false);
            Notifications.Publish(new MessageData() { Object = tag, Type = MessageData.MessageType.AddTag });
            return tag;
        }

        public void CancelEdit()
        {
            if (_inTxn)
            {
                Name = _copyItem.Name;
                Usage = _copyItem.Usage;
                DataType = _copyItem.DataType;
                Description = _copyItem.Description;
                ExternalAccess = _copyItem.ExternalAccess;
                IsConstant = _copyItem.IsConstant;
                DisplayStyle = _copyItem.DisplayStyle;
                _inTxn = false;
            }
        }

        private List<DisplayItem<Usage>> CreateUsageSource()
        {

            var usageList = new List<DisplayItem<Usage>>
            {
                new DisplayItem<Usage> { DisplayName = "Local Tag", Value = Usage.Local },
                new DisplayItem<Usage> { DisplayName = "Input Parameter", Value = Usage.Input },
                new DisplayItem<Usage> { DisplayName = "Output Parameter", Value = Usage.Output },
                new DisplayItem<Usage> { DisplayName = "InOut Parameter", Value = Usage.InOut }
            };

            if (!IsInAOI)
                usageList.Add(new DisplayItem<Usage> { DisplayName = "Public Parameter", Value = Usage.SharedData });

            return usageList;
        }

        [SuppressMessage("ReSharper", "MergeCastWithTypeCheck")]
        private void ChangeDataType(DataTypeInfo newDataTypeInfo)
        {
            Contract.Assert(Tag != null);

            var controller = ParentCollection?.Controller ?? Controller;
            Contract.Assert(controller != null);

            Contract.Assert(newDataTypeInfo.DataType != null);

            var oldDataWrapper = Tag.DataWrapper;

            // For BOOL
            if (newDataTypeInfo.DataType.IsBool
                && newDataTypeInfo.Dim1 > 0 && newDataTypeInfo.Dim2 == 0 && newDataTypeInfo.Dim3 == 0)
                if (newDataTypeInfo.Dim1 % 32 != 0)
                    newDataTypeInfo.Dim1 = (newDataTypeInfo.Dim1 / 32 + 1) * 32;

            var newDataWrapper = TagsFactory.CreateDataWrapper(
                controller,
                newDataTypeInfo.DataType,
                newDataTypeInfo.Dim1, newDataTypeInfo.Dim2, newDataTypeInfo.Dim3);

            // pre change 
            if (oldDataWrapper is MotionGroup)
                foreach (var tag in controller.Tags)
                {
                    var axis = tag as Tag;
                    var axisCIPDrive = axis?.DataWrapper as AxisCIPDrive;
                    if (axisCIPDrive != null && axisCIPDrive.AssignedGroup == Tag) axisCIPDrive.AssignedGroup = null;

                    var axisVirtual = axis?.DataWrapper as AxisVirtual;
                    if (axisVirtual != null && axisVirtual.AssignedGroup == Tag) axisVirtual.AssignedGroup = null;
                }

            if (oldDataWrapper is AxisCIPDrive)
            {
                var axisCIPDrive = (AxisCIPDrive)oldDataWrapper;
                var cipMotionDrive = axisCIPDrive.AssociatedModule as CIPMotionDrive;
                cipMotionDrive?.RemoveAxis(Tag, axisCIPDrive.AxisNumber);
            }

            // change
            Tag.UpdateDataWrapper(newDataWrapper, GetDefaultDisplayStyle(newDataTypeInfo.DataType));
        }

        private DisplayStyle GetDefaultDisplayStyle(IDataType dataType)
        {
            var defaultDisplayStyle = DisplayStyle.NullStyle;

            if (dataType is BOOL || dataType is SINT || dataType is INT || dataType is DINT || dataType is LINT)
                defaultDisplayStyle = DisplayStyle.Decimal;
            else if (dataType is REAL) defaultDisplayStyle = DisplayStyle.Float;

            return defaultDisplayStyle;
        }

        private void ExecuteChangeDataTypeCommand()
        {
            var controller = ParentCollection?.Controller ?? Controller;
            Contract.Assert(controller != null);

            bool supportsOneDimensionalArray = true;
            bool supportsMultiDimensionalArrays = true;

            if (IsInAOI)
            {
                switch (Usage)
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
            }

            var selectDataTypeDialog = new SelectDataTypeDialog(
                controller, DataType, supportsOneDimensionalArray, supportsMultiDimensionalArrays)
            {
                Owner = Application.Current.MainWindow
            };

            var dialogResult = selectDataTypeDialog.ShowDialog();
            if (dialogResult != null && dialogResult.Value) DataType = selectDataTypeDialog.DataType;
        }

        private void UpdateSelfProperty()
        {
            SelfProperty = null;
            OnPropertyChanged("SelfProperty");
            SelfProperty = this;
            OnPropertyChanged("SelfProperty");
        }

        protected override void OnTagPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnTagPropertyChanged(sender, e);

            if (sender == Tag && !_inTxn)
            {
                if (e.PropertyName.Equals("Description"))
                {
                    CanSetDescription = false;
                    Description = Tag.GetChildDescription(Tag.Description, Tag.DataTypeInfo, Tag.ChildDescription,
                        Tag.GetOperand(Name));
                    CanSetDescription = true;
                }

                if (e.PropertyName.Equals("ExternalAccess")) ExternalAccess = Tag.ExternalAccess;

                if (e.PropertyName.Equals("IsConstant") && ParentItem == null) OnPropertyChanged("IsConstant");

                if (e.PropertyName.Equals("Usage") && ParentItem == null && Usage != Tag.Usage) Usage = Tag.Usage;

                if (e.PropertyName.Equals("DisplayStyle") && ParentItem == null && DisplayStyle != Tag.DisplayStyle)
                    DisplayStyle = Tag.DisplayStyle;

                if (ParentItem == null)
                    if (e.PropertyName.Equals("DataWrapper"))
                    {
                        var parentCollection = ParentCollection as EditTagCollection;

                        bool needRestoreCurrent = false;
                        var defaultView = CollectionViewSource.GetDefaultView(ParentCollection);
                        if (defaultView != null)
                        {
                            needRestoreCurrent = defaultView.CurrentItem == this;
                        }

                        if (parentCollection != null)
                        {
                            var index = parentCollection.IndexOf(this);

                            if (index >= 0)
                            {
                                var newItem = TagToEditTagItem(Tag);

                                parentCollection.RemoveItemsByTag(Tag);

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
    }
}