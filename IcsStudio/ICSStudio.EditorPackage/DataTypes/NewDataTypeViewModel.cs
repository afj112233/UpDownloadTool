using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Dialogs.WaitDialog;
using ICSStudio.Dialogs.Warning;
using ICSStudio.EditorPackage.DataTypes.Change;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Gui.View;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Annotations;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIInterfaces.GlobalClipboard;
using ICSStudio.UIInterfaces.Project;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json.Linq;
using DataTypeCollection = ICSStudio.SimpleServices.DataType.DataTypeCollection;
using DataTypeMember = ICSStudio.SimpleServices.DataType.DataTypeMember;
using Type = System.Type;
using UserDefinedDataType = ICSStudio.SimpleServices.DataType.UserDefinedDataType;

namespace ICSStudio.EditorPackage.DataTypes
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public partial class NewDataTypeViewModel : ViewModelBase, IEditorPane, ICanApply, IGlobalClipboard
    {
        private int _maxNumberOfChar;
        private bool _isDirty;
        private bool _isExist;
        private bool _nameValid;
        private bool _isReadonly;
        private bool _isDescriptionReadonly;
        private bool _isDataTypeEmpty;
        private bool _isHideInherited;
        private string _name;
        private string _description;
        private string _engineeringUnit;
        private object _property;
        private readonly IDataType _dataType;
        private readonly Controller _controller;
        private CompositiveType _compositiveType;
        private DataGridRowProperty _selectedDataGridRow;
        private DataGridRowProperty _selectedListViewItem;

        public NewDataTypeViewModel(UserControl userControl, IDataType dataType)
        {
            _dataType = dataType;
            Control = userControl;
            userControl.DataContext = this;
            _compositiveType = dataType as CompositiveType;

            _controller = Controller.GetInstance();

            _dataTypeChangeManager = new DataTypeChangeManager(_dataType);
            _tagValueDic = new Dictionary<ITag, object>();

            ComponentProperty = new ComponentProperty();
            DataGrid = new ObservableCollection<DataGridRowProperty>();
            
            ApplyCommand = new RelayCommand(ExecuteApplyCommand, CanApplyCommand);
            ApplyCommand.RaiseCanExecuteChanged();
            FocusCommand = new RelayCommand(ExecuteFocusCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            OkCommand = new RelayCommand(ExecuteOkCommand, CanOkCommand);
            PropertiesCommand = new RelayCommand(ExecutePropertiesCommand);
            MemberFocusCommand = new RelayCommand(ExecuteMemberFocusCommand);
            KeyUpCommand = new RelayCommand<KeyEventArgs>(ExecuteKeyUpCommand);
            ListViewFocusCommand = new RelayCommand(ExecuteListViewFocusCommand);
            PropertiesFocusCommand = new RelayCommand(ExecutePropertiesFocusCommand);
            HyperlinkCommand = new RelayCommand<DataGridRow>(ExecuteHyperlinkCommand);
            ExpandedCommand = new RelayCommand<RoutedEventArgs>(ExecuteExpandedCommand);
            Hyperlink3Command = new RelayCommand<ListViewItem>(ExecuteHyperlink3Command);
            Hyperlink2Command = new RelayCommand<ListViewItem>(ExecuteHyperlink2Command);
            CollapsedCommand = new RelayCommand<RoutedEventArgs>(ExecuteCollapsedCommand);
            NameClickCommand = new RelayCommand<RoutedEventArgs>(ExecuteNameClickCommand);
            LoadRowCommand = new RelayCommand<DataGridRowEventArgs>(ExecuteLoadRowCommand);
            RowLostFocusCommand = new RelayCommand<DataGridRow>(ExecuteRowLostFocusCommand);
            NameLostFocusCommand = new RelayCommand<RoutedEventArgs>(ExecuteNameLostFocusCommand);
            PropertyValueCommand = new RelayCommand<RoutedEventArgs>(ExecutePropertyValueCommand);
            PropertyValue2Command = new RelayCommand<RoutedEventArgs>(ExecutePropertyValue2Command);

            CutCommand = new RelayCommand(ExecuteCutCommand, CanExecuteCutCommand);
            CopyCommand = new RelayCommand(ExecuteCopyCommand, CanExecuteCopyCommand);
            PasteCommand = new RelayCommand(ExecutePasteCommand, CanExecutePasteCommand);
            DeleteMemberCommand = new RelayCommand(ExecuteDeleteMemberCommand, CanExecuteDeleteMemberCommand);
            InsertMemberCommand = new RelayCommand(ExecuteInsertMemberCommand, CanExecuteInsertMemberCommand);

            IsDescriptionReadOnly = false;
            if (dataType.Name != "")
            {
                IsReadOnly = dataType.FamilyType == FamilyType.StringFamily;
                CaptionType = "DataType";
                this.Caption = LanguageManager.GetInstance().ConvertSpecifier(CaptionType) +$":{dataType.Name}";
                _isExist = true;
            }
            else
            {
                if (dataType.IsStringType)
                {
                    IsReadOnly = true;
                    if (_compositiveType != null)
                    {
                        var comTypeMembers = _compositiveType.TypeMembers as TypeMemberComponentCollection;

                        if (comTypeMembers?.Count == 0)
                        {
                            comTypeMembers.AddDataTypeMember(new DataTypeMember
                            { Name = "LEN", DataType = DINT.Inst, DisplayStyle = DisplayStyle.Decimal });
                            comTypeMembers.AddDataTypeMember(new DataTypeMember
                            { Name = "DATA", DataType = SINT.Inst, DisplayStyle = DisplayStyle.Ascii });
                        }
                    }

                    CaptionType = "String";
                    CaptionNewType = "String";
                   
                }
                else
                {
                    IsHideInherited = true;
                    CaptionType = "DataType";
                    CaptionNewType = "UDT";
                }
                this.Caption = LanguageManager.GetInstance().ConvertSpecifier(CaptionType) + ": "
                    + LanguageManager.GetInstance().ConvertSpecifier("New") + " " + CaptionNewType;
            }

            MaxCharVisibility = dataType.IsStringType ? Visibility.Visible : Visibility.Collapsed;
            MemberProperty = new MemberProperty();

            SetAutoComplete();

            IsHideInherited = true;
            if (_compositiveType != null)
            {
                Name = _compositiveType.Name;
                Description = _compositiveType.Description;
                EngineeringUnit = _compositiveType.EngineeringUnit;
                SetDataGrid();
            }
            else
            {
                Component = new CompositiveType();
                if (IsAddNew())
                {
                    DataGrid.Add(BlankDataGridFactor());
                }
            }

            CollectionChangedEventManager.AddHandler(DataGrid, OnCollectionChanged);
            CollectionChangedEventManager.AddHandler(_compositiveType.TypeMembers, OnMemberCollectionChanged);
            ComponentProperty.Name = _compositiveType.Name;
            ComponentProperty.Description = _compositiveType.Description;
            ComponentProperty.EngineeringUnit = _compositiveType.EngineeringUnit;
            PropertyChangedEventManager.AddHandler(ComponentProperty, OnComponentPropertyChanged, "");

            ExternalAccessList = Enum.GetValues(typeof(ExternalAccess)).Cast<ExternalAccess>().Select(value =>
            {
                var e = Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()),
                    typeof(EnumMemberAttribute)) as EnumMemberAttribute;
                if (value == ExternalAccess.NullExternalAccess || value == ExternalAccess.Undefined) return null;
                if (e != null)
                    return new
                    {
                        DisplayName = e.Value,
                        Value = value
                    };

                return new
                {
                    DisplayName = value.ToString(),
                    Value = value
                };
            })
                .OrderBy(item => item?.Value)
                .ToList();
            ExternalAccessList.RemoveAt(0);
            ExternalAccessList.RemoveAt(0);
            Property = ComponentProperty;
            IsDescriptionEnabled = !_controller?.IsOnline??true;
            if (dataType.IsPredefinedType)
            {
                IsReadOnly = true;
                IsDescriptionReadOnly = true;
                IsDescriptionEnabled = false;
            }

            WeakEventManager<LanguageManager,EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);

            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                _controller, "IsOnlineChanged", OnIsOnlineChanged);
            IsDirty = false;
            _isStructChanged = false;
            SetControlEnable();
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            if (_dataType.Name != "")
            {
                this.Caption = LanguageManager.GetInstance().ConvertSpecifier(CaptionType) + $": {_dataType.Name}";
            }
            else
            {
                this.Caption = LanguageManager.GetInstance().ConvertSpecifier(CaptionType) + ": "
                    + LanguageManager.GetInstance().ConvertSpecifier("New") + " " + CaptionNewType;
            }

            //ChangeAttributeLanguage();
            RaisePropertyChanged(nameof(Size));
            RaisePropertyChanged(nameof(Caption));
        }

        private void ChangeAttributeLanguage()
        {
            var property = new ComponentProperty();
            var type = typeof(DisplayNameAttribute);
            foreach (var propertyInfo in property.GetType().GetProperties())
            {
                var attributes = propertyInfo.GetCustomAttributes(true);
                var display = attributes.FirstOrDefault(p => p is DisplayNameAttribute);
                if(display == null) continue;
                var displayNameAttribute = display as DisplayNameAttribute;
                var fieldInfo = type.GetField("_displayName", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance);
                if (fieldInfo == null) continue;
                var displayValue = fieldInfo.GetValue(displayNameAttribute).ToString();
                fieldInfo.SetValue(displayNameAttribute,LanguageManager.GetInstance().ConvertSpecifier(displayValue));
            }
        }

        private bool _isStructChanged;

        public RelayCommand CutCommand { get; }

        private bool CanExecuteCutCommand()
        {
            //if (SelectedDataGridRow == null || string.IsNullOrEmpty(SelectedDataGridRow.Name)) return false;
            //return true;
            //暂时观察无论在线离线情况下，无论是string还是predefined中都不可cut，等遇到有可用cut时再加限定条件
            return false;
        }

        private void ExecuteCutCommand()
        {
            if (!IsEnabled) return;
            ExecuteCopyCommand();
            DataGrid.Remove(SelectedDataGridRow);
        }

        public RelayCommand CopyCommand { get; }

        private bool CanExecuteCopyCommand()
        {
            if (SelectedDataGridRow == null || string.IsNullOrEmpty(SelectedDataGridRow.Name)) return false;
            return true;
        }

        private void ExecuteCopyCommand()
        {
            var str =
                $"{SelectedDataGridRow.Name} {SelectedDataGridRow.DisplayName} {SelectedDataGridRow.DisplayStyle.ToString()} {SelectedDataGridRow.ExternalAccess.ToString()}";
            Clipboard.SetData("ics.dataType", str);
        }

        public RelayCommand PasteCommand { get; }

        private bool CanExecutePasteCommand()
        {
            if (SelectedDataGridRow == null) return false;
            var str = Clipboard.GetData("ics.dataType") as string;
            if (string.IsNullOrEmpty(str))
                return false;

            return true;
        }

        private void ExecutePasteCommand()
        {
            if (!IsEnabled) return;
            var str = Clipboard.GetData("ics.dataType") as string;
            if (!string.IsNullOrEmpty(str))
            {
                var param = str.Split(' ');

                SelectedDataGridRow.Name = param[0];
                SelectedDataGridRow.DisplayName = param[1];
                SelectedDataGridRow.DisplayStyle = (DisplayStyle)Enum.Parse(typeof(DisplayStyle), param[2]);
                SelectedDataGridRow.ExternalAccess = (ExternalAccess)Enum.Parse(typeof(ExternalAccess), param[3]);
            }
        }

        public RelayCommand DeleteMemberCommand { get; }

        private void ExecuteDeleteMemberCommand()
        {
            DataGrid.Remove(SelectedDataGridRow);
            _isStructChanged = true;
            IsDirty = true;
        }

        private bool CanExecuteDeleteMemberCommand()
        {
            if (_dataType.FamilyType == FamilyType.StringFamily) return false;
            if (SelectedDataGridRow == null) return false;
            var index = DataGrid.IndexOf(SelectedDataGridRow);
            return index != DataGrid.Count - 1;
        }

        public static RoutedUICommand InsertUICommand { get; } = new RoutedUICommand("insert", "insert", typeof(NewDataType));

        public RelayCommand InsertMemberCommand { get; }

        private void ExecuteInsertMemberCommand()
        {
            var blank = new DataGridRowProperty(_controller, null) { DataType = new CompositiveType(), ExternalAccess = ExternalAccess.ReadWrite };
            PropertyChangedEventManager.AddHandler(blank, OnPropertyChanged, string.Empty);
            var index = DataGrid.IndexOf(SelectedDataGridRow);
            DataGrid.Insert(index, blank);
            _isStructChanged = true;
            IsDirty = true;
        }

        private bool CanExecuteInsertMemberCommand()
        {
            if (_dataType.FamilyType == FamilyType.StringFamily) return false;
            if (SelectedDataGridRow == null) return false;
            return true;
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                SetControlEnable();
                RaisePropertyChanged(nameof(IsOnlineEnabled));
            });
        }

        private void SetControlEnable()
        {
            if (_dataType.IsPredefinedType)
            {
                IsEnabled = false;
            }
            if (_controller.IsOnline)
            {
                if (!_dataType.IsPredefinedType)
                {
                    if (DataTypeExtend.CheckDataTypeIsUsed(_dataType))
                    {
                        IsEnabled = false;
                        IsPropertyEnabled = false;
                        IsReadOnly = true;
                    }
                    else
                    {
                        IsEnabled = true;
                        IsPropertyEnabled = true;
                        IsReadOnly = false;
                    }
                }
            }
            else
            {
                if (!_dataType.IsPredefinedType)
                {
                    IsEnabled = true;
                    IsPropertyEnabled = true;
                    IsReadOnly = _dataType.FamilyType == FamilyType.StringFamily;
                }
            }
        }

        public override void Cleanup()
        {
            CollectionChangedEventManager.RemoveHandler(DataGrid, OnCollectionChanged);
            CollectionChangedEventManager.RemoveHandler(_compositiveType.TypeMembers, OnMemberCollectionChanged);
            PropertyChangedEventManager.RemoveHandler(ComponentProperty, OnComponentPropertyChanged, "");

            PropertyChangedEventManager.RemoveHandler(_compositiveType, OnByteSizePropertyChanged, "");
            foreach (var member in _compositiveType.TypeMembers)
            {
                PropertyChangedEventManager.RemoveHandler(member, OnByteSizePropertyChanged, "");
            }

            foreach (var item in DataGrid)
            {
                PropertyChangedEventManager.RemoveHandler(item.DataType, OnDataTypePropertyChanged,
                    string.Empty);
                PropertyChangedEventManager.RemoveHandler(item, OnPropertyChanged, string.Empty);
            }

            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                _controller, "IsOnlineChanged", OnIsOnlineChanged);
        }
        
        public void SetDataGrid()
        {
            for (int i = DataGrid.Count-1; i >=0; i--)
            {
                var item = DataGrid[i];
                PropertyChangedEventManager.RemoveHandler(item.DataType, OnDataTypePropertyChanged,
                    string.Empty);
                PropertyChangedEventManager.RemoveHandler(item, OnPropertyChanged, string.Empty);
                DataGrid.Remove(item);
            }

            PropertyChangedEventManager.RemoveHandler(_compositiveType, OnByteSizePropertyChanged, "");
            bool flag = true;
            if (_compositiveType.TypeMembers != null && _compositiveType.TypeMembers.Count > 0)
            {

                foreach (var item in _compositiveType.TypeMembers)
                {
                    if (item.IsHidden)
                        continue;

                    if (item.DataTypeInfo.DataType == null)
                    {
                        flag = false;
                    }

                    var member = item as DataTypeMember;
                    string name = member?.DisplayName;
                    if (string.IsNullOrEmpty(name))
                    {
                        continue;
                    }

                    if (name?.IndexOf('[') > 0)
                        name = name.Substring(0, name.IndexOf('['));
                    if (item.DataTypeInfo.DataType != null && name != item.DataTypeInfo.DataType.Name) flag = false;
                    if (_dataType.IsStringType)
                    {
                        SetStringMember(item, name);
                    }
                    else
                    {
                        DataGridRowProperty dataGridRowProperty = new DataGridRowProperty(_controller, member)
                        {
                            Name = item.Name,
                            Description = item.Description,
                            DataType = item.DataTypeInfo.DataType,
                            EngineeringUnit = member.EngineeringUnit,
                            ExternalAccess = item.ExternalAccess,
                            Dim1 = item.DataTypeInfo.Dim1,
                            Dim2 = item.DataTypeInfo.Dim2,
                            Dim3 = item.DataTypeInfo.Dim3,
                            DisplayStyle = item.DisplayStyle,
                            DisplayName = member.DisplayName,
                            OldName = item.Name,
                        };
                        dataGridRowProperty.IsDirty = false;
                        PropertyChangedEventManager.AddHandler(dataGridRowProperty.DataType, OnDataTypePropertyChanged,
                            string.Empty);
                        PropertyChangedEventManager.AddHandler(dataGridRowProperty, OnPropertyChanged, string.Empty);
                        DataGrid.Add(dataGridRowProperty);
                    }
                }

                if (!_dataType.IsPredefinedType && !_dataType.IsStringType)
                {
                    IsHideInherited = false;
                    DataGrid.Add(BlankDataGridFactor());
                }
                else if (_dataType.IsStringType)
                {
                    //Aos has two base member: "LEN" and "DATA", when something monitor the member collection and add the "LEN",
                    //it will produce an error due to the lack of "DATA"
                    if (_compositiveType.TypeMembers["DATA"] != null)
                        MaxNumberOfChar = _compositiveType.TypeMembers["DATA"].DataTypeInfo.Dim1;
                }

                PropertyChangedEventManager.AddHandler(_compositiveType, OnByteSizePropertyChanged, "");
                //_compositiveType.PropertyChanged += OnByteSizePropertyChanged;
            }
            else
            {
                if (_dataType.IsStringType)
                {
                    BlankLenDataGridRow = new DataGridRowProperty(_controller, null)
                    {
                        DataType = DINT.Inst,
                        ExternalAccess = ExternalAccess.ReadWrite,
                        Name = "LEN",
                        DisplayStyle = DisplayStyle.Decimal
                    };
                    DataGrid.Add(BlankLenDataGridRow);

                    BlankDataGridRow = new DataGridRowProperty(_controller, null)
                    {
                        DataType = SINT.Inst,
                        ExternalAccess = ExternalAccess.ReadWrite,
                        Name = "DATA",
                        DisplayStyle = DisplayStyle.Ascii
                    };
                    DataGrid.Add(BlankDataGridRow);

                }
                else
                    DataGrid.Add(BlankDataGridFactor());
            }

            if (flag)
            {
                string temp = _compositiveType.ByteSize == 0 ? "??" : _compositiveType.ByteSize.ToString();
                Size = $"{temp} bytes";
                ComponentProperty.DataTypeSize = $"{temp} bytes";
            }
            else
            {
                Size = "?? bytes";
                ComponentProperty.DataTypeSize = "?? bytes";
            }

            RaisePropertyChanged("DataGrid");
            RaisePropertyChanged("Size");
        }

        public void SetAutoComplete()
        {
            AutoComplete.Clear();
            if (_controller != null)
                foreach (var item in _controller.DataTypes)
                {
                    if (item.Name.StartsWith("BOOL:", StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (item.Name.Contains("$"))
                        continue;
                    AutoComplete.Add(item.Name);
                }

            AutoComplete.Sort((x, y) => string.Compare(x, y, StringComparison.OrdinalIgnoreCase));
            RaisePropertyChanged(nameof(AutoComplete));
        }

        public void OnComponentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Name != ComponentProperty.Name)
                Name = ComponentProperty.Name;
            if (Description != ComponentProperty.Description)
                Description = ComponentProperty.Description;
            if (EngineeringUnit != ComponentProperty?.EngineeringUnit)
                EngineeringUnit = ComponentProperty?.EngineeringUnit;
            if (MaxNumberOfChar != ComponentProperty.MaxNumberOfChar)
                MaxNumberOfChar = ComponentProperty.MaxNumberOfChar;
        }

        public void OnMemberPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (SelectedDataGridRow.DisplayName != MemberProperty.Name)
                SelectedDataGridRow.Name = MemberProperty.Name;
            string name;
            if (MemberProperty.DataType.IndexOf('[') > 0)
            {
                name = MemberProperty.DataType.Substring(0, MemberProperty.DataType.IndexOf('['));
            }
            else
            {
                name = MemberProperty.DataType;
            }

            if (SelectedDataGridRow.DisplayName != name)
                SelectedDataGridRow.DisplayName = name;
            if (SelectedDataGridRow.EngineeringUnit != MemberProperty.EngineeringUnit)
                SelectedDataGridRow.EngineeringUnit = MemberProperty.EngineeringUnit;
            if (SelectedDataGridRow.Description != MemberProperty.Description)
                SelectedDataGridRow.Description = MemberProperty.Description;
            if (SelectedDataGridRow.ExternalAccess != MemberProperty.ExternalAccess)
                SelectedDataGridRow.ExternalAccess = MemberProperty.ExternalAccess;
            if (SelectedDataGridRow.DisplayStyle != MemberProperty.DisplayStyle)
                SelectedDataGridRow.DisplayStyle = MemberProperty.DisplayStyle;
            RaisePropertyChanged("Property");
            RaisePropertyChanged("SelectedDataGridRow");
        }

        public void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                IsDirty = true;
                Size = "??";
                ComponentProperty.DataTypeSize = "??";
                RaisePropertyChanged("Size");
                RaisePropertyChanged("Property");
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (DataGridRowProperty item in e.OldItems)
                {
                    PropertyChangedEventManager.RemoveHandler(item.DataType, OnDataTypePropertyChanged, string.Empty);
                    PropertyChangedEventManager.RemoveHandler(item, OnPropertyChanged, string.Empty);
                }
            }

            _isStructChanged = true;
        }

        public void OnByteSizePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_compositiveType.TypeMembers.Contains(sender))
                SetDataGrid();
        }

        public void OnMemberCollectionChanged(object sender,
            NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                SetDataGrid();
            }
        }

        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsSelected")|| e.PropertyName.Equals("IsNameInvalid")|| e.PropertyName.Equals("IsDisplayNameInvalid")) return;

            if (e.PropertyName == "DataType")
            {
                PropertyChangedEventManager.AddHandler(_selectedDataGridRow.DataType, OnDataTypePropertyChanged,
                    string.Empty);
            }

            //_selectedDataGridRow.DataType.PropertyChanged += OnDataTypePropertyChanged;
            var item = (DataGridRowProperty)sender;
            int index = DataGrid.IndexOf(item);
            MemberProperty = new MemberProperty();

            if (index == DataGrid.Count - 1 && !string.IsNullOrEmpty(item.Name) &&
                !_dataType.IsStringType)
            {
                DataGrid.Add(BlankDataGridFactor());
                RaisePropertyChanged("DataGrid");
            }

            if (e.PropertyName == "DisplayName")
            {
                _isStructChanged = true;
                Size ="??";
                ComponentProperty.DataTypeSize = "??";
                RaisePropertyChanged("Size");
                RaisePropertyChanged("ComponentProperty");
                SetStyle(true);
                IsHideInherited = CheckIsHideInherited();
            }

            MemberProperty.Name = _selectedDataGridRow?.Name;
            MemberProperty.DataTypeValue = _selectedDataGridRow?.DataType;
            MemberProperty.DataType = _selectedDataGridRow?.DisplayName;
            MemberProperty.EngineeringUnit = _selectedDataGridRow?.EngineeringUnit;
            MemberProperty.Description = _selectedDataGridRow?.Description;
            MemberProperty.ExternalAccess = _selectedDataGridRow?.ExternalAccess ?? ExternalAccess.ReadWrite;
            if (_selectedDataGridRow != null) MemberProperty.DisplayStyle = _selectedDataGridRow.DisplayStyle;
            PropertyChangedEventManager.AddHandler(MemberProperty, OnMemberPropertyChanged, string.Empty);
            Property = MemberProperty;
            RaisePropertyChanged("MemberProperty");
            RaisePropertyChanged("Property");
            RaisePropertyChanged("IsHideInherited");
            IsDirty = true;

            //log
            PropertyChangedExtendedEventArgs<string> args = e as PropertyChangedExtendedEventArgs<string>;
            if (e.PropertyName == "Name" && args != null)
            {
                ChangeDataTypeMemberNameLog log = 
                    new ChangeDataTypeMemberNameLog(args.OldValue, args.NewValue);
                _dataTypeChangeManager.AddLog(log);
            }

            if (e.PropertyName == "DisplayName" && args != null)
            {
                ChangeDataTypeMemberDataTypeLog log =
                    new ChangeDataTypeMemberDataTypeLog(item.Name, args.OldValue, args.NewValue);
                _dataTypeChangeManager.AddLog(log);

            }

        }

        //public void Compare()
        //{

        //    if (_dataType.IsStringType)
        //    {
        //        if (_compositiveType.Name != Name)
        //        {
        //            IsDirty = true;
        //            return;
        //        }

        //        if (_compositiveType.Description != Description)
        //        {
        //            IsDirty = true;
        //            return;
        //        }

        //        int maxChar = _compositiveType.TypeMembers["DATA"]?.DataTypeInfo.Dim1 ?? 0;
        //        if (maxChar != MaxNumberOfChar)
        //        {
        //            IsDirty = true;
        //            return;
        //        }
        //    }
        //    else
        //    {
        //        if (_compositiveType.Name != Name)
        //        {
        //            IsDirty = true;
        //            return;
        //        }

        //        if (_compositiveType.Description != Description)
        //        {
        //            IsDirty = true;
        //            return;
        //        }

        //        if (_compositiveType.EngineeringUnit != EngineeringUnit)
        //        {
        //            IsDirty = true;
        //            return;
        //        }

        //        if ((DataGrid.Count - 1) != _compositiveType.TypeMembers.Count)
        //        {
        //            IsDirty = true;
        //            return;
        //        }

        //        foreach (var item in DataGrid)
        //        {
        //            if (item.Name == null)
        //            {
        //                continue;
        //            }

        //            if (_compositiveType.TypeMembers[item.Name] != null)
        //            {
        //                DataTypeMember member = _compositiveType.TypeMembers[item.Name] as DataTypeMember;

        //                if (item.Name != member?.Name || item.Description != member?.Description ||
        //                    item.DisplayName != member?.DisplayName || item.ExternalAccess != member?.ExternalAccess ||
        //                    item.EngineeringUnit != member.EngineeringUnit || item.DisplayStyle != member.DisplayStyle)
        //                {
        //                    IsDirty = true;
        //                    return;
        //                }
        //            }
        //            else
        //            {
        //                IsDirty = true;
        //                return;
        //            }
        //        }
        //    }

        //    IsDirty = false;
        //}

        public List<string> AutoComplete { get; } = new List<string>();

        public CompositiveType Component
        {
            set { _compositiveType = value; }
            get { return _compositiveType; }
        }

        public void SetStyle(bool isChanged)
        {
            if (string.IsNullOrEmpty(_selectedDataGridRow?.DisplayName)) return;

            //
            string typeName;
            int dim0, dim1, dim2;
            int errorCode;

            var isValid = _controller.DataTypes.ParseDataType(
                _selectedDataGridRow?.DisplayName,
                out typeName,
                out dim0,
                out dim1,
                out dim2, out errorCode);

            IDataType dataType = null;
            if (isValid)
                dataType = _controller.DataTypes[typeName];
            //

            if (dataType != null)
            {
                string name = dataType.Name;
                _selectedDataGridRow.DataType = dataType;
                List<DisplayStyle> styleTypes = new List<DisplayStyle>();
                if (name.Equals("REAL", StringComparison.OrdinalIgnoreCase))
                {
                    styleTypes.Add(DisplayStyle.Float);
                    styleTypes.Add(DisplayStyle.Exponential);
                    if (!styleTypes.Contains(SelectedDataGridRow.DisplayStyle))
                        SelectedDataGridRow.DisplayStyle = DisplayStyle.Float;
                }
                else if (name.Equals("BOOL", StringComparison.OrdinalIgnoreCase))
                {
                    styleTypes.Add(DisplayStyle.Binary);
                    styleTypes.Add(DisplayStyle.Octal);
                    styleTypes.Add(DisplayStyle.Decimal);
                    styleTypes.Add(DisplayStyle.Hex);
                    if (!styleTypes.Contains(SelectedDataGridRow.DisplayStyle))
                        SelectedDataGridRow.DisplayStyle = DisplayStyle.Decimal;
                }
                else if (name.Equals("DINT", StringComparison.OrdinalIgnoreCase) ||
                         name.Equals("SINT", StringComparison.OrdinalIgnoreCase) ||
                         name.Equals("INT", StringComparison.OrdinalIgnoreCase))
                {
                    styleTypes.Add(DisplayStyle.Binary);
                    styleTypes.Add(DisplayStyle.Octal);
                    styleTypes.Add(DisplayStyle.Decimal);
                    styleTypes.Add(DisplayStyle.Hex);
                    styleTypes.Add(DisplayStyle.Ascii);
                    if (!styleTypes.Contains(SelectedDataGridRow.DisplayStyle))
                        SelectedDataGridRow.DisplayStyle = DisplayStyle.Decimal;
                }
                else if (name.Equals("LINT", StringComparison.OrdinalIgnoreCase))
                {
                    styleTypes.Add(DisplayStyle.Binary);
                    styleTypes.Add(DisplayStyle.Octal);
                    styleTypes.Add(DisplayStyle.Decimal);
                    styleTypes.Add(DisplayStyle.Hex);
                    styleTypes.Add(DisplayStyle.Ascii);
                    styleTypes.Add(DisplayStyle.DateTime);
                    styleTypes.Add(DisplayStyle.DateTimeNS);
                    if (!styleTypes.Contains(SelectedDataGridRow.DisplayStyle))
                        SelectedDataGridRow.DisplayStyle = DisplayStyle.Decimal;
                }
                else
                {
                    StyleTypes = EnumHelper.ToDataSource<DisplayStyle>();
                }

                if (styleTypes.Count > 0)
                    StyleTypes = styleTypes.Select(x =>
                    {
                        string displayName = x.ToString();
                        if (x == DisplayStyle.DateTimeNS)
                        {
                            displayName = "DataTime(ns)";
                        }

                        return new { DisplayName = displayName, Value = x };
                    }).ToList();
                RaisePropertyChanged("StyleTypes");
            }
        }

        public ObservableCollection<DataGridRowProperty> DataGrid { set; get; }

        public RelayCommand Command { set; get; }

        public IList StyleTypes { set; get; }

        public string Size
        {
            get
            {
                return LanguageManager.GetInstance().ConvertSpecifier("DataTypeSize") + ": "+ _size;
            }
            set
            {
                _size = value;
                RaisePropertyChanged();
            }
        }

        public string Caption
        {
            set
            {
                _caption = value;
                UpdateCaptionAction?.Invoke(_caption);
            }
            get { return _caption; }
        }

        public string CaptionType { set; get; }
        public string CaptionNewType { set; get; }

        public UserControl Control { get; }
        public Action CloseAction { get; set; }
        public Action<string> UpdateCaptionAction { get; set; }

        public bool IsEnabled
        {
            set { Set(ref _isEnabled, value); }
            get { return _isEnabled; }
        }

        public bool IsOnlineEnabled => !_controller.IsOnline;

        public bool IsDescriptionEnabled
        {
            set { Set(ref _isDescriptionEnabled, value); }
            get { return _isDescriptionEnabled; }
        }

        public bool IsPropertyEnabled
        {
            set { Set(ref _isPropertyEnabled, value); }
            get { return _isPropertyEnabled; }
        }

        public bool IsReadOnly
        {
            set
            {
                Set(ref _isReadonly, value);
                IsPropertyEnabled = !value;
                RaisePropertyChanged("IsPropertyEnabled");
            }
            get { return _isReadonly; }
        }

        public bool IsDescriptionReadOnly
        {
            set { Set(ref _isDescriptionReadonly, value); }
            get { return _isDescriptionReadonly; }
        }

        public IList ExternalAccessList { set; get; }
        public DataGridRowProperty BlankLenDataGridRow;

        public DataGridRowProperty BlankDataGridRow;

        //private bool _enabled = true;
        private bool _isPropertyEnabled;
        private bool _isEnabled;
        private bool _isDescriptionEnabled;
        private string _caption;
        public Visibility MaxCharVisibility { set; get; }

        public bool IsHideInherited
        {
            set
            {
                _isHideInherited = value;
                RaisePropertyChanged();
            }
            get { return _isHideInherited; }
        }

        public object Property
        {
            set
            {
                Set(ref _property, value);
                if (value is MemberProperty)
                {
                    if (value == ListViewMemberProperty)
                    {
                        IsPropertyEnabled = false;
                    }
                    else
                    {
                        IsPropertyEnabled = !IsReadOnly;
                    }
                }
                else if (value is ComponentProperty)
                {
                    IsPropertyEnabled = IsEnabled;
                }

                IsHideInherited = CheckIsHideInherited();
                RaisePropertyChanged("IsPropertyEnabled");
            }
            get { return _property; }
        }

        public string Name
        {
            set
            {
                Set(ref _name, value);
                IsDirty = true;
                ComponentProperty.Name = value;
            }
            get { return _name; }
        }

        public string Description
        {
            set
            {
                Set(ref _description, value);
                IsDirty = true;
                ComponentProperty.Description = value;
            }
            get { return _description ?? ""; }
        }

        public string EngineeringUnit
        {
            set
            {
                Set(ref _engineeringUnit);
                IsDirty = true;
                ComponentProperty.EngineeringUnit = value;
            }
            get { return _engineeringUnit ?? ""; }
        }

        public int MaxNumberOfChar
        {
            set
            {
                Set(ref _maxNumberOfChar, value);
                IsDirty = true;
                ComponentProperty.MaxNumberOfChar = value;
            }
            get { return _maxNumberOfChar; }
        }

        public bool IsDirty
        {
            set
            {
                if (_isDirty != value)
                {
                    Set(ref _isDirty, value);
                    if (value)
                    {
                        if (!Caption.EndsWith("*"))
                        {
                            Caption = $"{Caption}*";
                        }
                    }
                    else
                    {
                        if (Caption.EndsWith("*"))
                        {
                            Caption = Caption.Substring(0, Caption.Length - 1);
                        }
                    }

                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
            get { return _isDirty; }
        }

        public bool IsAddNew()
        {
            if (DataGrid.Count == 0) return true;
            if (string.IsNullOrEmpty(DataGrid[DataGrid.Count - 1].Name)) return false;
            return true;
        }

        public DataGridRowProperty BlankDataGridFactor()
        {
            DataGridRowProperty blankDataGrid = new DataGridRowProperty(_controller, null)
            { DataType = new CompositiveType(), ExternalAccess = ExternalAccess.ReadWrite };
            PropertyChangedEventManager.AddHandler(blankDataGrid, OnPropertyChanged, string.Empty);
            return blankDataGrid;
        }

        public void OnDataTypePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var preDirty = IsDirty;
            SetDataGrid();
            SetAutoComplete();
            IsDirty = preDirty;
        }

        public ComponentProperty ComponentProperty { set; get; }
        public MemberProperty MemberProperty { set; get; }
        public MemberProperty ListViewMemberProperty { set; get; }

        public DataGridRowProperty SelectedDataGridRow
        {
            set
            {
                if (value == null) return;
                IsPropertyEnabled = !IsReadOnly;
                RaisePropertyChanged("IsPropertyEnabled");
                if (_selectedDataGridRow != value)
                {
                    _selectedDataGridRow = value;
                    MemberProperty = new MemberProperty
                    {
                        Name = _selectedDataGridRow?.Name,
                        DataTypeValue = _selectedDataGridRow?.DataType,
                        DataType = _selectedDataGridRow?.DisplayName
                    };
                    SetStyle(false);
                    MemberProperty.DisplayStyle = _selectedDataGridRow.DisplayStyle;
                    MemberProperty.EngineeringUnit = _selectedDataGridRow?.EngineeringUnit;
                    MemberProperty.Description = _selectedDataGridRow?.Description;
                    MemberProperty.ExternalAccess = _selectedDataGridRow?.ExternalAccess ?? ExternalAccess.ReadWrite;
                    PropertyChangedEventManager.AddHandler(MemberProperty, OnMemberPropertyChanged, string.Empty);
                    //MemberProperty.PropertyChanged += OnMemberPropertyChanged;
                    MemberProperty = value.Name == null ? null : MemberProperty;
                    Property = MemberProperty;
                    IsHideInherited = CheckIsHideInherited();
                    RaisePropertyChanged();
                    RaisePropertyChanged("Property");
                }
            }
            get { return _selectedDataGridRow; }
        }

        public DataGridRowProperty SelectedListViewItem
        {
            set
            {
                IsPropertyEnabled = false;
                RaisePropertyChanged("IsPropertyEnabled");
                if (value == null) return;
                if (_selectedListViewItem != value)
                {
                    _selectedListViewItem = value;
                    ListViewMemberProperty = new MemberProperty
                    {
                        Name = _selectedListViewItem?.Name,
                        DataTypeValue = _selectedListViewItem?.DataType,
                        DataType = _selectedListViewItem?.DisplayName
                    };
                    SetStyle(false);
                    ListViewMemberProperty.DisplayStyle = _selectedListViewItem.DisplayStyle;
                    ListViewMemberProperty.EngineeringUnit = _selectedDataGridRow?.EngineeringUnit;
                    ListViewMemberProperty.Description = _selectedListViewItem?.Description;
                    ListViewMemberProperty.ExternalAccess =
                        _selectedListViewItem?.ExternalAccess ?? ExternalAccess.ReadWrite;
                    IsHideInherited = !CheckIsBaseDataType(_selectedListViewItem?.DataType?.Name);
                    Property = ListViewMemberProperty;
                    RaisePropertyChanged("Property");
                }
            }
            get { return _selectedListViewItem; }
        }

        public RelayCommand<RoutedEventArgs> NameClickCommand { set; get; }

        private void ExecuteNameClickCommand(RoutedEventArgs e)
        {
            DependencyObject dataGridCell =
                VisualTreeHelpers.FindVisualParentOfType<DataGridCell>(e.OriginalSource as DependencyObject);
            DependencyObject dataGridRow =
                VisualTreeHelpers.FindVisualParentOfType<DataGridRow>(e.OriginalSource as DependencyObject);
            DependencyObject dataGrid2 =
                VisualTreeHelpers.FindVisualParentOfType<DataGrid>(e.OriginalSource as DependencyObject);
            int count = ((DataGrid)dataGrid2).Items.Count;

            DataGridRowProperty dataGrid =
                (DataGridRowProperty)((System.Windows.Controls.Primitives.ButtonBase)e.OriginalSource)
                .CommandParameter;

            var dialog = new Dialogs.SelectDataType.SelectDataTypeDialog(
                dataGrid.Controller, dataGrid.DisplayName, true, false)
            {
                Height = 350,
                Width = 400,
                Owner = Application.Current.MainWindow
            };

            var result = dialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                if (count - 1 == (dataGridRow as DataGridRow).GetIndex() &&
                    string.IsNullOrEmpty(((DataGridRowProperty)(dataGridRow as DataGridRow).DataContext).Name))
                {
                    (dataGridCell as DataGridCell).IsEditing = false;
                }
                else
                {
                    dataGrid.DisplayName = dialog.DataType;
                    (dataGridCell as DataGridCell).IsEditing = false;
                }
            }
        }

        public RelayCommand<RoutedEventArgs> NameLostFocusCommand { set; get; }

        private void ExecuteNameLostFocusCommand(RoutedEventArgs e)
        {
            DependencyObject dataGridCell =
                VisualTreeHelpers.FindVisualParentOfType<DataGridCell>(e.OriginalSource as DependencyObject);
            if (dataGridCell != null)
                ((DataGridCell)dataGridCell).IsEditing = false;
        }

        public RelayCommand OkCommand { set; get; }

        private void ExecuteOkCommand()
        {
            if (IsDirty)
            {
                ExecuteApplyCommand();
            }
            else
            {
                CloseAction?.Invoke();
            }

            if (!_isDataTypeEmpty && _nameValid)
                CloseAction?.Invoke();
        }

        private bool CanOkCommand { get; } = true;

        public RelayCommand ApplyCommand { get; }

        private void ExecuteApplyCommand()
        {
            _flagDataTypeSize = true;
            if (!CheckMember(_flagDataTypeSize)) return;
            var dialog = new Wait(DoApply, true)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();

            foreach (var item in DataGrid)
            {
                item.IsNameInvalid = false;
                item.IsDisplayNameInvalid = false;
            }

            IsDirty = false;
            _isStructChanged = false;
        }

        private bool _flagDataTypeSize;
        private string _size;

        private void DoApply()
        {
            PropertyChangedEventManager.RemoveHandler(ComponentProperty, OnComponentPropertyChanged, "");
            PropertyChangedEventManager.RemoveHandler(_compositiveType, OnByteSizePropertyChanged, "");
            CollectionChangedEventManager.RemoveHandler(_compositiveType.TypeMembers, OnMemberCollectionChanged);
            JArray jObjectMembers = new JArray();
            JObject info = new JObject
            {
                ["Name"] = Name,
                ["Description"] = Description,
                ["Family"] = _dataType.IsStringType
                    ? (byte)FamilyType.StringFamily
                    : (byte)FamilyType.NoFamily,
                ["EngineeringUnit"] = EngineeringUnit,
                ["Members"] = jObjectMembers
            };
            if (_compositiveType.FamilyType == FamilyType.StringFamily && MaxNumberOfChar != _compositiveType.TypeMembers["DATA"].DataTypeInfo.Dim1)
            {
                _isStructChanged = true;
            }

            //
            PreApply();

            //
            int boolCount = 0;
            string target = "";
            if (_isStructChanged)
            {
                ApplyDirtyMember();
                foreach (var item in _compositiveType.TypeMembers)
                {
                    PropertyChangedEventManager.RemoveHandler(item, OnByteSizePropertyChanged, "");
                }

                foreach (var item in DataGrid)
                {
                    item.IsDirty = false;
                    if (string.IsNullOrEmpty(item.DisplayName)) continue;
                    if (item.OldName != null && !item.Name.Equals(item.OldName))
                    {
                        (_compositiveType as UserDefinedDataType)?.MemberChangedList.Add(
                            new Tuple<string, string>(item.OldName, item.Name));
                        item.OldName = item.Name;
                    }

                    if (item.DataType is BOOL)
                    {
                        item.Dim1 = ((item.Dim1 + 32 - 1) / 32 * 32);
                        if (item.Dim1 != 0 && item.Dim1 % 32 == 0)
                        {
                            jObjectMembers.Add(DataGridRowPropertyToDataTypeMember(item));
                            boolCount = 0;
                            continue;
                        }

                        if (boolCount % 8 == 0)
                        {
                            jObjectMembers.Add(SINTMemberFactory(ref target, jObjectMembers.Count, item.Description));

                        }

                        jObjectMembers.Add(BITMemberFactory(item, target, boolCount % 8));
                        boolCount++;
                        continue;
                    }
                    else
                    {
                        boolCount = 0;
                    }

                    jObjectMembers.Add(DataGridRowPropertyToDataTypeMember(item));
                }

                var udt = _compositiveType as UserDefinedDataType;
                if (udt != null)
                {
                    udt.ResetMembers(info,_controller.DataTypes);
                    int index = 0;
                    foreach (var member in udt.TypeMembers)
                    {
                        var item = DataGrid[index++];
                        item.DataTypeMember = (DataTypeMember)member;
                    }
                }
            }
            else
            {
                _dataType.Name = Name;
                _dataType.Description = Description;
                ApplyDirtyMember();
                ((UserDefinedDataType)_dataType).ResetInfo();
                ((UserDefinedDataType)_dataType).RaisePropertyChanged("ByteSize");
            }

            //
            PostApply();

            if (!_isExist)
            {
                (_controller.DataTypes as DataTypeCollection)?.AddDataType(_compositiveType);
                _isExist = true;
            }

            if (_flagDataTypeSize)
            {
                Size = $"{_compositiveType.ByteSize} bytes";

                ComponentProperty.DataTypeSize = $"{_compositiveType.ByteSize}bytes";

            }
            else
            {
                Size = "?? bytes";
                ComponentProperty.DataTypeSize = "?? bytes";
            }

            if (_isStructChanged)
                foreach (var item in _compositiveType.TypeMembers)
                {
                    PropertyChangedEventManager.AddHandler(item, OnByteSizePropertyChanged, "");
                }
            CodeSynchronization.GetInstance().Update();
            (_compositiveType as UserDefinedDataType)?.MemberChangedList.Clear();
            PropertyChangedEventManager.AddHandler(ComponentProperty, OnComponentPropertyChanged, "");
            PropertyChangedEventManager.AddHandler(_compositiveType, OnByteSizePropertyChanged, "");
            CollectionChangedEventManager.AddHandler(_compositiveType.TypeMembers, OnMemberCollectionChanged);
            IsDirty = false;
            CaptionType = "DataType";
            Caption = LanguageManager.GetInstance().ConvertSpecifier(CaptionType) +$":{_compositiveType.Name}";
            UpdateCaptionAction?.Invoke(Caption);
            RaisePropertyChanged("Size");
            RaisePropertyChanged("ComponentProperty");

            WeakEventManager<LanguageManager,EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged",LanguageChangedHandler);
        }
        
        private void ApplyDirtyMember()
        {
            foreach (var item in DataGrid)
            {
                if (string.IsNullOrEmpty(item.DisplayName) || !item.IsDirty) continue;
                Debug.Assert(item.DataTypeMember != null);
                var member = item.DataTypeMember;
                PropertyChangedEventManager.RemoveHandler(member, OnByteSizePropertyChanged, "");
                member.Name = item.Name;
                member.DataType = item.DataTypeInfo.DataType;
                member.Dim1 = item.Dim1;
                member.Dim2 = item.Dim2;
                member.Dim3 = item.Dim3;
                member.Description = item.Description;
                member.ExternalAccess = item.ExternalAccess;
                item.IsDirty = false;
                PropertyChangedEventManager.AddHandler(member, OnByteSizePropertyChanged, "");
            }
        }

        private bool CanApplyCommand()
        {
            if (_isDirty)
            {
                // set dirty
                IProjectInfoService projectInfoService =
                    Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
                projectInfoService?.SetProjectDirty();
            }

            return _isDirty;
        }

        public RelayCommand CancelCommand { set; get; }

        private void ExecuteCancelCommand()
        {
            CloseAction?.Invoke();
        }

        public RelayCommand<DataGridRow> HyperlinkCommand { set; get; }

        private void ExecuteHyperlinkCommand(DataGridRow sender)
        {
            DataGridRowProperty member =
                (sender).DataContext as DataGridRowProperty;
            member.LinkList.Clear();
            member.SecondLinkList.Clear();
            var detailListView = VisualTreeHelpers.GetChildObject<ListView>(sender as DependencyObject,
                "detailList");
            detailListView.ItemsSource = member.DetailList;
        }

        public RelayCommand<ListViewItem> Hyperlink2Command { set; get; }

        private void ExecuteHyperlink2Command(ListViewItem sender)
        {
            //1+2
            var listViewItem = sender;
            var listView = VisualTreeHelpers.FindVisualParentOfType<ListView>(listViewItem as DependencyObject);
            DataGridRowProperty member = (listViewItem as ListViewItem).DataContext as DataGridRowProperty;
            List<DataGridRowProperty> listMembers = new List<DataGridRowProperty>();
            foreach (var item in (member.DataType as CompositiveType).TypeMembers)
            {
                if (member.DataType is AOIDataType)
                {
                    if (!(member.DataType as AOIDataType).IsMemberShowInOtherAoi(item.Name)) continue;
                }

                DataGridRowProperty dataGridRowProperty =
                    new DataGridRowProperty(member.Controller, (DataTypeMember)item)
                    {
                        Name = item.Name,
                        Description = item.Description,
                        DataType = item.DataTypeInfo.DataType,
                        EngineeringUnit = (item as DataTypeMember).EngineeringUnit,
                        ExternalAccess = item.ExternalAccess,
                        DisplayStyle = item.DisplayStyle,
                        DisplayName = (item as DataTypeMember).DisplayName
                    };
                listMembers.Add(dataGridRowProperty);
            }

            var grid = VisualTreeHelpers.FindVisualParentOfType<Grid>(listView);
            ListView detailListView = VisualTreeHelpers.GetChildObject<ListView>(grid, "detailList");

            detailListView.SelectedItem = (listViewItem as ListViewItem).DataContext;
            detailListView.ItemsSource = listMembers;

            SelectedDataGridRow.SecondLinkList.Clear();
            SelectedDataGridRow.SecondLinkList.Add(SelectedListViewItem);
            int index = SelectedDataGridRow.LinkList.IndexOf(SelectedListViewItem);
            int count = SelectedDataGridRow.LinkList.Count;
            for (int i = index; i < count; i++)
            {
                SelectedDataGridRow.LinkList.RemoveAt(index);
            }

            RaisePropertyChanged("SelectedDataGridRow");
        }

        public RelayCommand<ListViewItem> Hyperlink3Command { set; get; }

        private void ExecuteHyperlink3Command(ListViewItem sender)
        {
            //1+1
            var listViewItem = sender;
            var listView = VisualTreeHelpers.FindVisualParentOfType<ListView>(listViewItem as DependencyObject);
            DataGridRowProperty member = (listViewItem as ListViewItem).DataContext as DataGridRowProperty;
            List<DataGridRowProperty> listMembers = new List<DataGridRowProperty>();
            foreach (var item in (member.DataType as CompositiveType).TypeMembers)
            {
                if (member.DataType is AOIDataType)
                {
                    if (!(member.DataType as AOIDataType).IsMemberShowInOtherAoi(item.Name)) continue;
                }

                DataGridRowProperty dataGridRowProperty =
                    new DataGridRowProperty(member.Controller, (DataTypeMember)item)
                    {
                        Name = item.Name,
                        Description = item.Description,
                        DataType = item.DataTypeInfo.DataType,
                        EngineeringUnit = (item as DataTypeMember).EngineeringUnit,
                        ExternalAccess = item.ExternalAccess,
                        DisplayStyle = item.DisplayStyle,
                        DisplayName = (item as DataTypeMember).DisplayName
                    };
                listMembers.Add(dataGridRowProperty);
            }

            var grid = VisualTreeHelpers.FindVisualParentOfType<Grid>(listView);
            ListView detailListView = VisualTreeHelpers.GetChildObject<ListView>(grid, "detailList");

            detailListView.SelectedItem = (listViewItem as ListViewItem).DataContext;
            detailListView.ItemsSource = listMembers;

            if (SelectedDataGridRow.SecondLinkList.Count == 0)
            {
                SelectedDataGridRow.SecondLinkList.Add(SelectedListViewItem);
            }
            else
            {
                SelectedDataGridRow.LinkList.Add(SelectedDataGridRow.SecondLinkList[0]);
                SelectedDataGridRow.SecondLinkList.RemoveAt(0);
                SelectedDataGridRow.SecondLinkList.Add(SelectedListViewItem);
            }

            RaisePropertyChanged("SelectedDataGridRow");
        }

        public RelayCommand FocusCommand { set; get; }

        private void ExecuteFocusCommand()
        {
            Property = ComponentProperty;
            RaisePropertyChanged("Property");
        }

        public RelayCommand MemberFocusCommand { set; get; }

        private void ExecuteMemberFocusCommand()
        {
            Property = MemberProperty;
            RaisePropertyChanged("Property");
        }

        public RelayCommand ListViewFocusCommand { set; get; }

        private void ExecuteListViewFocusCommand()
        {
            Property = ListViewMemberProperty;
            RaisePropertyChanged("Property");

        }

        public RelayCommand PropertiesFocusCommand { set; get; }

        private void ExecutePropertiesFocusCommand()
        {
            if (SelectedDataGridRow == Property)
            {
                IsHideInherited =
                    !CheckIsBaseDataType(SelectedDataGridRow.DataType?.Name);
            }
            else
            {
                IsHideInherited =
                    !CheckIsBaseDataType(SelectedListViewItem.DataType?.Name);
            }

        }

        public RelayCommand PropertiesCommand { set; get; }

        private void ExecutePropertiesCommand()
        {
            var dialog = new Dialogs.SelectDataType.SelectDataTypeDialog(
                _controller, MemberProperty.DataType, true, false)
            {
                Height = 350,
                Width = 400,
                Owner = Application.Current.MainWindow
            };

            var result = dialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                MemberProperty.DataType = dialog.DataType;
            }
        }

        public RelayCommand<RoutedEventArgs> PropertyValueCommand { set; get; }

        private void ExecutePropertyValueCommand(RoutedEventArgs e)
        {
            GetValue(e.Source, "Description");
        }

        public RelayCommand<RoutedEventArgs> PropertyValue2Command { set; get; }

        private void ExecutePropertyValue2Command(RoutedEventArgs e)
        {
            GetValue(e.Source, "Engineering");
        }

        public RelayCommand<RoutedEventArgs> ExpandedCommand { set; get; }

        private void ExecuteExpandedCommand(RoutedEventArgs e)
        {
            DependencyObject row = VisualTreeHelpers.FindVisualParentOfType<DataGridRow>(e.Source as Expander);
            ((DataGridRow)row).DetailsVisibility = Visibility.Visible;
        }

        public RelayCommand<RoutedEventArgs> CollapsedCommand { set; get; }

        private void ExecuteCollapsedCommand(RoutedEventArgs e)
        {
            DependencyObject row = VisualTreeHelpers.FindVisualParentOfType<DataGridRow>(e.Source as Expander);
            ((DataGridRow)row).DetailsVisibility = Visibility.Collapsed;
        }

        public RelayCommand<DataGridRowEventArgs> LoadRowCommand { set; get; }

        private void ExecuteLoadRowCommand(DataGridRowEventArgs e)
        {
            DataGridRowProperty member = e.Row.DataContext as DataGridRowProperty;
            if (member != null && member.ExpanderVisibility != Visibility.Hidden)
            {
                e.Row.DetailsVisibility = Visibility.Collapsed;
            }
            else
            {
                e.Row.DetailsVisibility = Visibility.Visible;
            }
        }

        public RelayCommand<DataGridRow> RowLostFocusCommand { set; get; }

        private void ExecuteRowLostFocusCommand(DataGridRow sender)
        {
            ((DataGridRow)sender).IsSelected = false;
        }

        public RelayCommand<KeyEventArgs> KeyUpCommand { set; get; }

        private void ExecuteKeyUpCommand(KeyEventArgs e)
        {
            if (!IsEnabled) return;
            if (_dataType.FamilyType == FamilyType.StringFamily) return;
            var dataGrid = (DataGrid)e.Source;
            var selectedRow = (DataGridRowProperty)dataGrid.SelectedItem;
            if (selectedRow?.Name != null)
            {
                if (e.Key == Key.Delete)
                {
                    DataGrid.Remove(selectedRow);
                    IsDirty = true;
                }
            }
        }

        private void GetValue(object sender, string kind)
        {
            Window window = Window.GetWindow((Button)sender);
            Point point = ((Button)sender).TransformToAncestor(window).Transform(new Point(0, 0));
            string value;
            if (kind == "Description")
            {
                value = (((Button)sender).CommandParameter as ComponentProperty) == null
                    ? (((Button)sender).CommandParameter as MemberProperty).Description
                    : (((Button)sender).CommandParameter as ComponentProperty).Description;
            }
            else
            {
                value = (((Button)sender).CommandParameter as ComponentProperty) == null
                    ? (((Button)sender).CommandParameter as MemberProperty).EngineeringUnit
                    : (((Button)sender).CommandParameter as ComponentProperty).EngineeringUnit;
            }

            var viewmodel = new InputDialogViewModel(value);
            var dialog = new InputDialog(point.X - 300, point.Y + 20)
            {
                DataContext = viewmodel,
                Owner = Application.Current.MainWindow
            };
            if (dialog.ShowDialog().Value)
            {
                if (kind == "Description")
                {
                    if ((((Button)sender).CommandParameter as ComponentProperty) == null)
                    {
                        ((MemberProperty)((Button)sender).CommandParameter).Description = viewmodel.Description;
                    }
                    else
                    {
                        ((ComponentProperty)((Button)sender).CommandParameter).Description = viewmodel.Description;
                    }

                }
                else
                {
                    if ((((Button)sender).CommandParameter as ComponentProperty) == null)
                    {
                        ((MemberProperty)((Button)sender).CommandParameter).EngineeringUnit = viewmodel.Description;
                    }
                    else
                    {
                        ((ComponentProperty)((Button)sender).CommandParameter).EngineeringUnit =
                            viewmodel.Description;
                    }
                }

            }

        }

        public bool CheckIsBaseDataType(string name)
        {
            if (name == "SINT" || name?.IndexOf("BOOL") > -1 ||
                name == "DINT" || name == "REAL" ||
                name == "LINT" || name == "INT")
            {
                return true;
            }

            return false;
        }

        public bool CheckIsHideInherited()
        {
            if (_dataType.IsStringType) return false;
            if (Property == null || Property is ComponentProperty) return true;
            else
            {
                MemberProperty memberProperty = Property as MemberProperty;
                if (memberProperty?.DataType != null)
                {
                    IDataType dataType = _controller.DataTypes[memberProperty.DataType];
                    if (!(dataType is CompositiveType))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private bool IsValidName(string name)
        {
            string warningMessage = "";
            string warningReason = string.Empty;
            bool isValid = true;

            if (string.IsNullOrEmpty(name))
            {
                isValid = false;
                warningReason = "Error in name: Name is invalid.";
            }

            if (isValid)
            {
                if (name.Length > 40 || name.EndsWith("_") || name.IndexOf("__") > -1)
                {
                    isValid = false;
                    warningReason = "Error in name: Name is invalid.";
                }
            }


            if (isValid)
            {
                Regex regex = new Regex(@"\b[a-zA-Z_][a-zA-Z0-9_]*");
                if (!regex.IsMatch(name) || name.IndexOf("__") > -1 || name.EndsWith("_"))
                {
                    isValid = false;
                    warningReason = "Error in name: Name is invalid.";
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
                        warningReason = "Error in name: Name is invalid.";
                    }
                }
            }

            if (isValid)
            {
                foreach (var item in _controller.DataTypes)
                {
                    if (item != Component && item.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        isValid = false;
                        warningReason = "Error in name: Already exists.";
                    }
                }
            }

            if (!isValid)
            {
                var warningDialog = new WarningDialog(warningMessage, warningReason)
                { Owner = Application.Current.MainWindow };
                warningDialog.ShowDialog();
            }

            return isValid;
        }

        private bool IsValidMemberName(DataGridRowProperty component)
        {
            string warningMessage = "";
            string warningReason = string.Empty;
            bool isValid = true;

            if (string.IsNullOrEmpty(component.Name))
            {
                isValid = false;
                warningReason = "Error in member name: Name is invalid.";
            }

            if (isValid)
            {
                if (component.Name.Length > 40 || component.Name.EndsWith("_") ||
                    component.Name.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    isValid = false;
                    warningReason = "Error in member name: Name is invalid.";
                }
            }

            if (isValid)
            {
                Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*(:[a-zA-Z0-9_]+)?$");
                //Regex regex = new Regex(@"^[a-zA-Z_](_[a-zA-Z0-9])?[a-zA-Z0-9]*$");
                if (!regex.IsMatch(component.Name))
                {
                    isValid = false;
                    warningReason = "Error in member name: Name is invalid.";
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
                    if (keyWord.Equals(component.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        isValid = false;
                        warningReason = "Error in member name: Name is invalid.";
                    }
                }
            }

            if (isValid)
            {
                foreach (var item in DataGrid)
                {
                    if (item != component &&
                        string.Equals(item.Name, component.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        isValid = false;
                    }
                }

                if (!isValid)
                {
                    warningReason = "Error in member name: This member name was used on more than one member.";
                }
            }

            if (!isValid)
            {
                component.IsSelected = true;
                component.IsNameInvalid = true;

                _nameValid = false;
                var warningDialog = new WarningDialog(warningMessage, warningReason)
                { Owner = Application.Current.MainWindow };
                warningDialog.ShowDialog();
            }

            return isValid;
        }

        private bool IsValidDataTypeName(string name)
        {
            string warningMessage = "";
            string warningReason = string.Empty;
            bool isValid = true;

            if (string.IsNullOrEmpty(name))
            {
                isValid = false;
                warningReason = "Error in member data type:data type is invalid.";
            }

            if (isValid)
            {
                if (name.Length > 40 || name.EndsWith("_") ||
                    name.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    isValid = false;
                    warningReason = "Error in member data type:data type is invalid.";
                }
            }

            if (isValid)
            {
                Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*(:[a-zA-Z0-9_]+)?$");
                if (!regex.IsMatch(name))
                {
                    isValid = false;
                    warningReason = "Error in member data type:data type is invalid.";
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
                        warningReason = "Error in member data type:data type is invalid.";
                    }
                }
            }

            if (isValid)
            {
                if (name.Equals("ALARM_DIGITAL", StringComparison.OrdinalIgnoreCase) ||
                    name.Equals("ALARM_ANALOG", StringComparison.OrdinalIgnoreCase) ||
                    name.Equals("AXIS_CIP_DRIVE", StringComparison.OrdinalIgnoreCase) ||
                    name.Equals("AXIS_CONSUMED", StringComparison.OrdinalIgnoreCase) ||
                    name.Equals("AXIS_GENERIC", StringComparison.OrdinalIgnoreCase) ||
                    name.Equals("AXIS_GENERIC_DRIVE", StringComparison.OrdinalIgnoreCase) ||
                    name.Equals("AXIS_SERVO", StringComparison.OrdinalIgnoreCase) ||
                    name.Equals("AXIS_SERVO_DRIVE", StringComparison.OrdinalIgnoreCase) ||
                    name.Equals("AXIS_VIRTUAL", StringComparison.OrdinalIgnoreCase) ||
                    name.Equals("COORDINATE_SYSTEM", StringComparison.OrdinalIgnoreCase) ||
                    name.Equals("MESSAGE", StringComparison.OrdinalIgnoreCase))
                {
                    isValid = false;
                    warningReason =
                        "Error in member data type:A member of a User-Defined data type cannot be of this type.";
                }
            }

            if (isValid)
            {
                if (_controller.DataTypes[name] == null)
                {
                    isValid = false;
                    warningReason = "Error in member data type:data type is invalid.";
                }
            }

            //TODO(clx): 暂时不支持ULINT/USINT/UINT/UDINT/LREAL/LINT
            if (isValid)
            {
                string[] unsupportedTypes =
                {
                    LREAL.Inst.Name,
                    ULINT.Inst.Name,
                    UDINT.Inst.Name,
                    UINT.Inst.Name,
                    USINT.Inst.Name,
                    LINT.Inst.Name
                };
                if (unsupportedTypes.Contains(name, StringComparer.OrdinalIgnoreCase))
                {
                    isValid = false;
                    warningReason = $"The {name} data type is not supported by this controller type.";
                }
            }

            if (isValid)
            {
                //TODO(ZYL):Check Module defined data type
            }

            if (!isValid)
            {
                _isDataTypeEmpty = true;
                var warningDialog = new WarningDialog(warningMessage, warningReason)
                { Owner = Application.Current.MainWindow };
                warningDialog.ShowDialog();
            }

            return isValid;
        }

        private bool IsMemberContainComponent(CompositiveType compositiveType)
        {
            if (compositiveType == null)
            {
                return true;
            }

            bool flag = true;
            if (compositiveType.Name == _compositiveType.Name)
            {
                return false;
            }

            foreach (var item in compositiveType.TypeMembers)
            {

                if ((item as DataTypeMember).DisplayName == _compositiveType.Name)
                {
                    return false;
                }

                if ((item.DataTypeInfo.DataType as UserDefinedDataType) != null)
                {
                    flag = IsMemberContainComponent((item.DataTypeInfo.DataType as CompositiveType));
                }

                if (!flag)
                {

                    return false;
                }
            }

            return true;
        }

        private bool IsMaxNumberOfCharValid()
        {
            if (MaxNumberOfChar >= 1 && MaxNumberOfChar <= 65535)
                return true;
            else
            {
                var warningDialog =
                    new WarningDialog("", "Enter a value between 1 and 65535 for Maximum Characters")
                    { Owner = Application.Current.MainWindow };
                _nameValid = false;
                warningDialog.ShowDialog();
                return false;
            }

        }

        private JObject DataGridRowPropertyToDataTypeMember(DataGridRowProperty dataGridRowProperty)
        {
            if (dataGridRowProperty != null)
            {
                int dim1 = 0;
                string name = dataGridRowProperty.DisplayName;
                if (dataGridRowProperty.DisplayName.IndexOf(']') > 0)
                {
                    dim1 = int.Parse(dataGridRowProperty.DisplayName
                        .Substring(dataGridRowProperty.DisplayName.IndexOf('['),
                            dataGridRowProperty.DisplayName.Length - dataGridRowProperty.DisplayName.IndexOf('['))
                        .Replace("[", "").Replace("]", ""));
                    name = dataGridRowProperty.DisplayName.Substring(0, dataGridRowProperty.DisplayName.IndexOf('['));
                }

                if (dataGridRowProperty.Dim1 == 00 && dim1 != 0)
                {
                    dataGridRowProperty.Dim1 = dim1;
                }

                JObject member = new JObject
                {
                    ["Name"] = dataGridRowProperty.Name,
                    ["Description"] = dataGridRowProperty.Description,
                    ["EngineeringUnit"] = dataGridRowProperty.EngineeringUnit,
                    ["ExternalAccess"] = (byte)dataGridRowProperty.ExternalAccess,
                    ["DataType"] = dataGridRowProperty.DataType?.Name ?? "",
                    ["Dimension"] = dataGridRowProperty.Dim1,
                    ["Radix"] = (byte)dataGridRowProperty.DisplayStyle,
                    ["DisplayName"] = name == dataGridRowProperty.DataType?.Name ? "" : name,
                    ["Hidden"] = false
                };
                return member;
            }

            return null;
        }

        private JObject SINTMemberFactory(ref string target, int count, string description)
        {
            var name = Name;
            if (name.Length > 10)
            {
                name = name.Substring(0, 10);
            }
            target = $"ZZZZZZZZZZ{name}{count}";
            JObject member = new JObject
            {
                ["Name"] = target,
                ["Description"] = description,
                ["EngineeringUnit"] = "",
                ["ExternalAccess"] = (byte)ExternalAccess.ReadWrite,
                ["DataType"] = "SINT",
                ["Dimension"] = 0,
                ["Radix"] = (byte)DisplayStyle.Decimal,
                ["DisplayName"] = target,
                ["Hidden"] = true
            };
            return member;
        }

        private JObject BITMemberFactory(DataGridRowProperty dataGridRowProperty, string target, int bitNumber)
        {
            JObject member = new JObject
            {
                ["Name"] = dataGridRowProperty.Name,
                ["Description"] = dataGridRowProperty.Description,
                ["EngineeringUnit"] = dataGridRowProperty.EngineeringUnit,
                ["ExternalAccess"] = (byte)dataGridRowProperty.ExternalAccess,
                ["DataType"] = "BIT",
                ["Dimension"] = 0,
                ["Radix"] = (byte)dataGridRowProperty.DisplayStyle,
                ["DisplayName"] = "BOOL",
                ["Hidden"] = false,
                ["BitNumber"] = bitNumber,
                ["Target"] = target
            };
            return member;
        }

        //private void CycleUpdateTimerHandle(object state, EventArgs e)
        //{
        //    if (_dataType.IsPredefinedType) return;
        //    Compare();
        //    if (_controller.IsOnline)
        //    {
        //        if (DataTypeExtend.CheckDataTypeIsUsed(_dataType))
        //        {
        //            IsEnabled = false;
        //            IsPropertyEnabled = false;
        //            IsReadOnly = true;
        //        }
        //        else
        //        {
        //            IsEnabled = true;
        //            IsPropertyEnabled = true;
        //            IsReadOnly = false;
        //        }
        //    }
        //}

        private void SetEmptyDataType(CompositiveType pointCompositiveType)
        {
            foreach (var dataType in _controller.DataTypes)
            {
                if ((dataType as UserDefinedDataType) == null ||
                    (dataType as CompositiveType) == pointCompositiveType ||
                    dataType.IsPredefinedType)
                {
                    continue;
                }

                SetValue(dataType as UserDefinedDataType, pointCompositiveType);
            }
        }

        private bool SetValue(CompositiveType compositiveType, CompositiveType pointCompositiveType)
        {
            if (compositiveType == null) return false;
            bool flag = false;
            foreach (var member in compositiveType.TypeMembers)
            {
                if ((member.DataTypeInfo.DataType as CompositiveType) != null)
                {
                    if ((member.DataTypeInfo.DataType as CompositiveType) == pointCompositiveType)
                    {
                        flag = true;
                    }
                }
                else
                {
                    if ((member as DataTypeMember).DisplayName == pointCompositiveType.Name)
                    {
                        (member as DataTypeMember).DataType = pointCompositiveType;

                        flag = true;
                    }
                }
            }

            if (flag)
            {
                RedoInfo(compositiveType as UserDefinedDataType);
                SetEmptyDataType(compositiveType);
            }

            return flag;
        }

        private void ResetDisplayName()
        {
            foreach (var item in _controller.DataTypes)
            {
                if (item is UserDefinedDataType)
                {
                    SetDisplayNameValue(item as CompositiveType);
                }
            }
        }

        private void SetDisplayNameValue(CompositiveType compositiveType)
        {
            if (compositiveType == null) return;
            foreach (var item in compositiveType.TypeMembers)
            {
                if (item.DataTypeInfo.DataType == _compositiveType)
                {
                    (item as DataTypeMember).DisplayName = _compositiveType.Name;
                    continue;
                }

                if (item.DataTypeInfo.DataType is UserDefinedDataType)
                {
                    SetDisplayNameValue(item.DataTypeInfo.DataType as UserDefinedDataType);
                }
            }
        }

        private void RedoInfo(UserDefinedDataType userDefinedDataType)
        {

            JArray jObjectMembers = new JArray();
            JObject info = new JObject
            {
                ["Name"] = userDefinedDataType.Name,
                ["Description"] = userDefinedDataType.Description,
                ["Family"] = (byte)userDefinedDataType.FamilyType,
                ["Members"] = jObjectMembers
            };

            foreach (var item in userDefinedDataType.TypeMembers)
            {
                var dataTypeMember = item as DataTypeMember;
                if (dataTypeMember?.DisplayName == null) continue;
                JObject member = new JObject
                {
                    ["Name"] = item.Name,
                    ["Description"] = item.Description,
                    ["EngineeringUnit"] = dataTypeMember.EngineeringUnit,
                    ["ExternalAccess"] = (byte)item.ExternalAccess,
                    ["DataType"] = item.DataTypeInfo.DataType?.Name,
                    ["Dimension"] = item.DataTypeInfo.Dim1,
                    ["Radix"] = (byte)item.DisplayStyle,
                    ["DisplayName"] = dataTypeMember.DisplayName,
                    ["Hidden"] = false
                };
                jObjectMembers.Add(member);
            }

            userDefinedDataType.ResetMembers(info, _controller.DataTypes);
        }
        
        private bool CheckMember(bool flagDataTypeSize)
        {
            if (!IsValidName(Name))
            {
                _nameValid = false;
                return false;
            }
            else
            {
                _nameValid = true;
            }


            int validCount = 0;
            if (_dataType.IsStringType)
            {
                if (BlankDataGridRow.Dim1 != MaxNumberOfChar)
                {
                    BlankDataGridRow.Dim1 = MaxNumberOfChar;
                    _isStructChanged = true;
                }
                if (!IsMaxNumberOfCharValid()) return false;
            }

            for (int i = DataGrid.Count - 1; i >= 0; i--)
            {
                var item = DataGrid[i];
                if (string.IsNullOrEmpty(item.DisplayName))
                {
                    if (!string.IsNullOrEmpty(item.Name) || i < DataGrid.Count - 1)
                    {
                        item.IsDisplayNameInvalid = true;
                        item.IsSelected = true;

                        string message = "Error in member data type:Data type is invalid.";
                        var warningDialog = new WarningDialog("", message)
                        { Owner = Application.Current.MainWindow };
                        warningDialog.ShowDialog();
                        _nameValid = false;
                        return false;
                    }

                    continue;
                }

                string name = item.DisplayName;
                int dim1 = 0;
                if (name.IndexOf(']') > 0)
                {
                    string strDim1 = name
                        .Substring(name.IndexOf('['), name.Length - name.IndexOf('['))
                        .Replace("[", "").Replace("]", "");
                    Regex regex = new Regex("^[0-9]+$");
                    if (!regex.IsMatch(strDim1))
                    {
                        item.IsDisplayNameInvalid = true;
                        item.IsSelected = true;

                        string message = "Error in member data type:The size of the array on the data type is invalid.";
                        var warningDialog = new WarningDialog("", message)
                        { Owner = Application.Current.MainWindow };
                        warningDialog.ShowDialog();
                        _nameValid = false;
                        return false;
                    }

                    dim1 = int.Parse(strDim1);
                    name = item.DisplayName.Substring(0, item.DisplayName.IndexOf('['));
                }

                item.Dim1 = item.Dim1 == 0 ? dim1 : item.Dim1;
                if (name == Name)
                {
                    item.IsDisplayNameInvalid = true;
                    item.IsSelected = true;

                    string message =
                        $"Error in member data type '{name}':Tag or user-defined data type is directly or indirectly referencing itself.";
                    var warningDialog = new WarningDialog("", message)
                    { Owner = Application.Current.MainWindow };
                    warningDialog.ShowDialog();
                    _nameValid = false;
                    return false;
                }

                if (_controller.DataTypes[name] == null)
                {
                    item.IsDisplayNameInvalid = true;
                    item.IsSelected = true;

                    _isDataTypeEmpty = true;
                    string message = $"Error in member data type '{name}':DataType is not exist.";
                    var warningDialog = new WarningDialog("", message)
                    { Owner = Application.Current.MainWindow };
                    warningDialog.ShowDialog();
                    _nameValid = false;
                    return false;
                }

                item.DataType = _controller.DataTypes[name];
                if (item.DataType is MESSAGE || item.DataType is AXIS_GENERIC || item.DataType is AXIS_GENERIC_DRIVE ||
                    item.DataType is AXIS_SERVO || item.DataType is AXIS_SERVO_DRIVE ||
                    item.DataType is COORDINATE_SYSTEM || item.DataType is ENERGY_BASE || item
                        .DataType is ENERGY_ELECTRICAL || item.DataType is HMIBC || item.DataType is MOTION_GROUP ||
                    item.DataType is AXIS_CIP_DRIVE || item.DataType is AXIS_VIRTUAL || item.DataType is AXIS_CONSUMED)
                {
                    item.IsDisplayNameInvalid = true;
                    item.IsSelected = true;

                    string message =
                        $"Error in member data type:A member of a User-Defined data type cannot be of this type\n(data type:{name}).";
                    var warningDialog = new WarningDialog("", message)
                    { Owner = Application.Current.MainWindow };
                    warningDialog.ShowDialog();
                    _nameValid = false;
                    return false;
                }

                if (IsValidMemberName(item) && IsValidDataTypeName(name))
                {
                    if (!IsMemberContainComponent(
                        (item.DataType as CompositiveType)))
                    {
                        item.IsDisplayNameInvalid = true;
                        item.IsSelected = true;

                        string message =
                            $"Error in member data type '{name}':Tag or user-defined data type is directly or indirectly referencing itself.";
                        var warningDialog = new WarningDialog("", message)
                        { Owner = Application.Current.MainWindow };
                        warningDialog.ShowDialog();
                        _nameValid = false;
                        return false;
                    }

                    if (_controller.DataTypes[name] == null)
                    {
                        flagDataTypeSize = false;
                    }

                    validCount++;
                }
                else
                {
                    _nameValid = false;
                    return false;
                }
            }

            if (validCount == 0)
            {
                _isDataTypeEmpty = true;
                var warningDialog =
                    new WarningDialog("", "Unable to modify data type:At least one member required.")
                    { Owner = Application.Current.MainWindow };
                warningDialog.ShowDialog();
                return false;
            }

            _isDataTypeEmpty = false;
            return true;
        }

        private void SetStringMember(IDataTypeMember item, string name)
        {
            var member = item as DataTypeMember;
            if (name == "DINT")
            {
                BlankLenDataGridRow = new DataGridRowProperty(_controller, member)
                {
                    Name = item.Name,
                    OldName = item.Name,
                    Description = item.Description,
                    DataType = item.DataTypeInfo.DataType,
                    EngineeringUnit = member.EngineeringUnit,
                    ExternalAccess = item.ExternalAccess,
                    Dim1 = item.DataTypeInfo.Dim1,
                    Dim2 = item.DataTypeInfo.Dim2,
                    Dim3 = item.DataTypeInfo.Dim3,
                    DisplayStyle = item.DisplayStyle,
                    DisplayName = member.DisplayName,
                    IsDirty = false
                };
                PropertyChangedEventManager.AddHandler(BlankLenDataGridRow.DataType, OnDataTypePropertyChanged, "");
                DataGrid.Add(BlankLenDataGridRow);
            }
            else
            {
                BlankDataGridRow = new DataGridRowProperty(_controller, member)
                {
                    Name = item.Name,
                    OldName = item.Name,
                    Description = item.Description,
                    DataType = item.DataTypeInfo.DataType,
                    EngineeringUnit = member.EngineeringUnit,
                    ExternalAccess = item.ExternalAccess,
                    Dim1 = item.DataTypeInfo.Dim1,
                    Dim2 = item.DataTypeInfo.Dim2,
                    Dim3 = item.DataTypeInfo.Dim3,
                    DisplayStyle = item.DisplayStyle,
                    DisplayName = member.DisplayName,
                    IsDirty = false
                };
                PropertyChangedEventManager.AddHandler(BlankDataGridRow.DataType, OnDataTypePropertyChanged, "");
                DataGrid.Add(BlankDataGridRow);
            }
        }

        public int Apply()
        {
            _flagDataTypeSize = true;
            if (!CheckMember(_flagDataTypeSize))
            {
                return -1;
            }

            DoApply();
            return 0;
        }

        public bool CanApply()
        {
            return IsDirty;
        }

        public bool CanPasted()
        {
            var textBox = CurrentObject.GetInstance().CurrentControl as TextBox;
            if (textBox != null)
            {
                return ApplicationCommands.Paste.CanExecute(null, textBox);
            }

            return false;
        }

        public bool CanCut()
        {
            var textBox = CurrentObject.GetInstance().CurrentControl as TextBox;
            if (textBox != null)
            {
                return ApplicationCommands.Cut.CanExecute(null, textBox);
            }

            return false;
        }

        public bool CanCopy()
        {
            var textBox = CurrentObject.GetInstance().CurrentControl as TextBox;
            if (textBox != null)
            {
                return ApplicationCommands.Copy.CanExecute(null, textBox);
            }

            return false;
        }

        public void DoPaste()
        {
            var textBox = CurrentObject.GetInstance().CurrentControl as TextBox;
            if (textBox != null)
            {
                ApplicationCommands.Paste.Execute(null, textBox);
            }
        }

        public void DoCut()
        {
            var textBox = CurrentObject.GetInstance().CurrentControl as TextBox;
            if (textBox != null)
            {
                ApplicationCommands.Cut.Execute(null, textBox);
            }
        }

        public void DoCopy()
        {
            var textBox = CurrentObject.GetInstance().CurrentControl as TextBox;
            if (textBox != null)
            {
                ApplicationCommands.Copy.Execute(null, textBox);
            }
        }
    }

    public class DataGridRowProperty : INotifyPropertyChanged
    {
        private string _name;
        private string _description = string.Empty;
        private IDataType _dataType;
        private Visibility _expanderVisibility;
        private List<DataGridRowProperty> _detailList;
        private string _displayName;
        private string _engineeringUnit = string.Empty;
        private int _dim1;
        private DisplayStyle _displayStyle;
        private string _rowHeader = string.Empty;
        private bool _isDirty;
        private bool _isSelected;
        private bool _isNameInvalid;
        private bool _isDisplayNameInvalid;
        private Visibility _brushVisibility;

        public DataGridRowProperty(IController controller, DataTypeMember dataTypeMember)
        {
            DataTypeMember = dataTypeMember ?? new DataTypeMember();
            Controller = controller;
            LinkList = new ObservableCollection<DataGridRowProperty>();
            SecondLinkList = new ObservableCollection<DataGridRowProperty>();
        }

        public DataTypeMember DataTypeMember { get; internal set; }

        public IController Controller { set; get; }

        public string RowHeader
        {
            get { return _rowHeader; }
            set
            {
                if (value == _rowHeader) return;
                _rowHeader = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected) return;
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public bool IsNameInvalid
        {
            get { return _isNameInvalid; }
            set
            {
                if (value == _isNameInvalid) return;
                _isNameInvalid = value;
                OnPropertyChanged();
            }
        }

        public bool IsDisplayNameInvalid
        {
            get { return _isDisplayNameInvalid; }
            set
            {
                if (value == _isDisplayNameInvalid) return;
                _isDisplayNameInvalid = value;
                OnPropertyChanged();
            }
        }


        public string Name
        {
            get { return _name; }
            set
            {
                BrushVisibility = Visibility.Hidden;
                if (_name != value)
                {
                    string oldName = _name;
                    _name = value;

                    OnPropertyChanged(nameof(Name), oldName, value);
                    IsDirty = true;
                    IsNameInvalid = false;
                }
            }
        }

        public bool IsDirty
        {
            set
            {
                RowHeader = value ? "*" : string.Empty;
                _isDirty = value;
            }
            get { return _isDirty; }
        }

        public string OldName { set; get; }

        public string Description
        {
            get { return _description ?? ""; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged();
                    IsDirty = true;
                }
            }
        }

        public DataTypeInfo DataTypeInfo => new DataTypeInfo()
        {
            DataType = DataType,
            Dim1 = Dim1,
            Dim2 = Dim2,
            Dim3 = Dim3
        };

        public DisplayStyle DisplayStyle
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

        public ExternalAccess ExternalAccess { get; set; }
        public bool IsHidden { get; set; }

        public IDataType DataType
        {
            get { return _dataType; }
            set
            {
                if (_dataType != value)
                {
                    _dataType = value;
                    OnPropertyChanged();
                    OnPropertyChanged("DetailList");
                    OnPropertyChanged("ExpanderVisibility");
                }
            }
        }

        public int Dim1
        {
            get { return _dim1; }
            set
            {
                if (_dim1 != value)
                {
                    _dim1 = value;
                    OnPropertyChanged("DisplayName");
                }

            }
        }

        public int Dim2 { get; set; }
        public int Dim3 { get; set; }
        public int FieldIndex { get; set; }

        public Visibility BrushVisibility
        {
            set
            {
                if (value == _brushVisibility) return;
                _brushVisibility = value;
                OnPropertyChanged();
            }
            get { return _brushVisibility; }
        }

        public string EngineeringUnit
        {
            get { return _engineeringUnit ?? ""; }
            set
            {
                if (_engineeringUnit != value)
                {
                    _engineeringUnit = value;
                    OnPropertyChanged();
                    IsDirty = true;
                }
            }
        }

        public string DisplayName
        {
            set
            {
                // old display name
                string oldDisplayName = _displayName;
                if (Dim1 > 0)
                    oldDisplayName = $"{_displayName}[{Dim1}]";

                //
                IsDisplayNameInvalid = false;
                string name = value;
                if (name.IndexOf(']') > 0)
                {
                    string dim = name
                        .Substring(name.IndexOf('['), name.Length - name.IndexOf('['))
                        .Replace("[", "").Replace("]", "");
                    Regex regex = new Regex(@"^[0-9]*$");
                    if (!regex.IsMatch(dim))
                    {
                        string message = "Error in member data type:The size of the array on the data type is invalid";
                        var warningDialog = new WarningDialog("",
                                message)
                            { Owner = Application.Current.MainWindow };
                        warningDialog.ShowDialog();
                        dim = "0";
                    }

                    if (!string.IsNullOrEmpty(dim))
                    {
                        var dim1 = int.Parse(dim);
                        Dim1 = dim1;
                        name = name.Substring(0, name.IndexOf('['));
                    }

                }
                else
                {
                    Dim1 = 0;
                }

                IsDirty = true;


                _displayName = name;

                // new display name
                string newDisplayName = _displayName;
                if (Dim1 > 0)
                    newDisplayName = $"{_displayName}[{Dim1}]";

                OnPropertyChanged(nameof(DisplayName), oldDisplayName, newDisplayName);

                OnPropertyChanged("DetailList");
                OnPropertyChanged("ExpanderVisibility");
            }
            get
            {
                if (string.IsNullOrEmpty(_displayName))
                {
                    if (Dim1 > 0)
                        return $"{DataType?.Name}[{Dim1}]";
                    else
                        return DataType?.Name ?? "";
                }

                if (Dim1 > 0)
                {
                    if (_displayName.IndexOf('[') > 0)
                    {
                        return $"{_displayName.Substring(0, _displayName.IndexOf('['))}[{Dim1}]";
                    }
                    else
                    {
                        return $"{_displayName}[{Dim1}]";
                    }
                }
                else
                    return _displayName ?? "";
            }
        }

        //private bool VerifyDataType(string dataType)
        //{
        //    string dataTypeName = "";
        //    int dim1 = 0, dim2 = 0, dim3 = 0, result = 0;
        //    var flag = Controller.DataTypes.ParseDataType(dataType, out dataTypeName, out dim1, out dim2, out dim3,
        //        out result);
        //    if (flag)
        //    {
        //        var d = Controller.DataTypes[dataTypeName];
        //        if (d is ALARM_ANALOG || d is ALARM_DIGITAL) return false;
        //        if (d is AXIS_COMMON || d is AXIS_CONSUMED || d is AXIS_GENERIC || d is AXIS_GENERIC_DRIVE ||
        //            d is AXIS_SERVO || d is AXIS_SERVO_DRIVE) return false;
        //        if (d is COORDINATE_SYSTEM) return false;
        //        if (d is ENERGY_BASE || d is ENERGY_ELECTRICAL) return false;
        //        if (d is HMIBC) return false;
        //        if (d is MOTION_GROUP) return false;
        //        if (d is MESSAGE) return false;
        //        return true;
        //    }

        //    return false;
        //}

        public IList DetailList
        {
            get
            {
                string displayName = DisplayName;
                if (string.IsNullOrEmpty(displayName)) return null;
                _detailList = new List<DataGridRowProperty>();
                if (displayName.IndexOf('[') > 0)
                    displayName = displayName.Substring(0, displayName.IndexOf('['));
                IDataType dataType = Controller.DataTypes[displayName];
                if ((dataType as CompositiveType) != null)
                {
                    foreach (var item in (dataType as CompositiveType).TypeMembers)
                    {
                        if (dataType is AOIDataType)
                        {
                            if (!(dataType as AOIDataType).IsMemberShowInOtherAoi(item.Name)) continue;
                        }

                        var member = item as DataTypeMember;
                        DataGridRowProperty dataGridRowProperty = new DataGridRowProperty(Controller, null)
                        {
                            Name = item.Name,
                            Description = item.Description,
                            DataType = item.DataTypeInfo.DataType,
                            EngineeringUnit = member.EngineeringUnit,
                            ExternalAccess = item.ExternalAccess,
                            DisplayStyle = item.DisplayStyle,
                            DisplayName = member.DisplayName,
                        };
                        _detailList.Add(dataGridRowProperty);
                    }

                }

                return _detailList;
            }
        }

        public Visibility ExpanderVisibility
        {
            get
            {
                if (DetailList != null && DetailList.Count > 0)
                {
                    _expanderVisibility = Visibility.Visible;
                }
                else
                {
                    _expanderVisibility = Visibility.Collapsed;
                }

                return _expanderVisibility;
            }
        }

        public ObservableCollection<DataGridRowProperty> LinkList { set; get; }

        public ObservableCollection<DataGridRowProperty> SecondLinkList { set; get; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void OnPropertyChanged<T>(string propertyName, T oldValue, T newValue)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            propertyChanged?.Invoke(this,
                new PropertyChangedExtendedEventArgs<T>(propertyName, oldValue, newValue));
        }
    }

    

    public class ComponentProperty : MaxNumberOfCharProperty
    {
        private string _name;
        private string _description;
        private string _engineeringUnit;
        private string _dataTypeSize;

        [CategoryAttribute("常规"), DescriptionAttribute("指定名称")]
        [DisplayName("Name")]
        public string Name
        {
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
            get { return _name; }
        }

        [CategoryAttribute("常规"), DescriptionAttribute("指定最多512个字符的文档")]
        [DisplayName("Description")]
        public string Description
        {
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
            get { return _description; }
        }

        [DisplayName("Data Type Size")]
        [CategoryAttribute("常规"), DescriptionAttribute("指示Data type 结构的大小")]
        public string DataTypeSize
        {
            set
            {
                if (_dataTypeSize != value)
                {
                    _dataTypeSize = value;
                    OnPropertyChanged();
                }
            }
            get { return _dataTypeSize; }
        }

        [CategoryAttribute("数据"),
         DescriptionAttribute("Specifies the unit of measure for the value(i.e. inches,psi,etc)")]
        [DisplayName("Engineering Unit")]
        public string EngineeringUnit
        {
            set
            {
                if (_engineeringUnit != value)
                {
                    _engineeringUnit = value;
                    OnPropertyChanged();
                }
            }
            get { return _engineeringUnit; }
        }

        [Browsable(false)]
        public bool IsEnabled { set; get; } = false;
    }

    public class MaxNumberOfCharProperty : INotifyPropertyChanged
    {
        private int _maxNumberOfCharProperty;

        [DisplayName("Max Number Of Char"), Description("指定字符串中的最大字符数"), Category("常规")]
        public int MaxNumberOfChar
        {
            set
            {
                if (_maxNumberOfCharProperty != value)
                {
                    _maxNumberOfCharProperty = value;
                    OnPropertyChanged();
                }
            }
            get { return _maxNumberOfCharProperty; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MemberProperty : MemberDisplayNameProperty
    {
        private string _name;
        private string _description;
        private string _engineeringUnit;
        private ExternalAccess _externalAccess;

        private string _dataType;

        [CategoryAttribute("常规"), DescriptionAttribute("Specifies the name")]

        public string Name
        {
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
            get { return _name; }
        }

        [CategoryAttribute("常规"), DescriptionAttribute("Specifies up to 512 characters of documentation")]
        public string Description
        {
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
            get { return _description; }
        }

        [Category("常规"), Description("indicates the size of the data type structure")]
        [DisplayName("Data Type")]

        public string DataType
        {
            set
            {
                if (_dataType != value)
                {
                    _dataType = value;
                    OnPropertyChanged();
                }
            }
            get { return _dataType; }
        }

        [Browsable(false)]
        public IDataType DataTypeValue { set; get; }
        [Browsable(false)]
        public int Dim1 { set; get; }
        [Browsable(false)]
        public int Dim2 { set; get; }
        [Browsable(false)]
        public int Dim3 { set; get; }

        [CategoryAttribute("数据"),
         DescriptionAttribute("Specifies the unit of measure for the value(i.e. inches,psi,etc)")]
        [DisplayName("Engineering Unit")]
        public string EngineeringUnit
        {
            set
            {
                if (_engineeringUnit != value)
                {
                    _engineeringUnit = value;
                    OnPropertyChanged();
                }
            }
            get { return _engineeringUnit; }
        }

        [DisplayName("External Access")]
        [CategoryAttribute("常规"),
         DescriptionAttribute(
             "Specifies how the member can be viewed and changed from applications connected to the controller")]
        public ExternalAccess ExternalAccess
        {
            set
            {
                if (_externalAccess != value)
                {
                    _externalAccess = value;
                    OnPropertyChanged();
                }
            }
            get { return _externalAccess; }
        }
    }

    public class MemberDisplayNameProperty : INotifyPropertyChanged
    {

        private DisplayStyle _displayStyle;

        [DisplayName("DisplayStyle"), Description("Specifies the display style for the value of the member"),
         Category("常规")]
        public DisplayStyle DisplayStyle
        {
            set
            {
                if (_displayStyle != value)
                {
                    _displayStyle = value;
                    OnPropertyChanged();
                }
            }
            get { return _displayStyle; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

