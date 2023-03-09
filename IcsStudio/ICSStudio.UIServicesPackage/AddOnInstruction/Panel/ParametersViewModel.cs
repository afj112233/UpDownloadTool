using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Gui.View;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Annotations;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIServicesPackage.AddOnInstruction.Panel.Validate;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json.Linq;

namespace ICSStudio.UIServicesPackage.AddOnInstruction.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class ParametersViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private readonly IController _controller;
        private bool _isDirty;
        private ParametersRow _selectedParametersRow;
        private bool _canMoveDownCommand;
        private bool _canMoveUpCommand;
        private bool _onlineEnable;
        private readonly AoiDefinition _aoiDefinition;
        private readonly AddOnInstructionVM _vm;
        public ParametersViewModel(Parameters panel, IAoiDefinition aoiDefinition, AddOnInstructionVM vm)
        {
            CopyCommand = new RelayCommand(ExecuteCopyCommand, CanExecuteCopyCommand);
            CutCommand = new RelayCommand(ExecuteCutCommand, CanExecuteCutCommand);
            PasteCommand = new RelayCommand(ExecutePasteCommand, CanExecutePasteCommand);
            DeleteCommand = new RelayCommand(ExecuteDeleteCommand, CanExecuteDeleteCommand);
            _vm = vm;
            Control = panel;
            panel.DataContext = this;
            ParametersRows = new ParametersRow(aoiDefinition, null, aoiDefinition.ParentController.IsOnline, null)
                {IsBase = true};
            _aoiDefinition = (AoiDefinition) aoiDefinition;
            MoveUpCommand = new RelayCommand(ExecuteMoveUpCommand, CanMoveUpCommand);
            MoveDownCommand = new RelayCommand(ExecuteMoveDownCommand, CanMoveDownCommand);
            DataGridRowHeaderCommand = new RelayCommand<DataGridRowHeader>(ExecuteDataGridRowHeaderCommand);
            KeyUpCommand = new RelayCommand<KeyEventArgs>(ExecuteKeyUpCommand);
            List<Usage> temp = new List<Usage>();
            temp.Add(Usage.Input);
            temp.Add(Usage.Output);
            temp.Add(Usage.InOut);
            UsageList = temp.Select(x => { return new {DisplayName = x.ToString(), Value = x}; }).ToList();
            List<ExternalAccess> eaList = new List<ExternalAccess>();
            eaList.Add(ExternalAccess.ReadWrite);
            eaList.Add(ExternalAccess.ReadOnly);
            eaList.Add(ExternalAccess.None);
            ExternalAccessList = eaList.Select(x => { return new {DisplayName = x.ToString(), Value = x}; }).ToList();
            _controller = _aoiDefinition.ParentController;
            AutoComplete = new List<string>();
            foreach (var item in _controller.DataTypes)
            {
                if (item.Name.StartsWith("BOOL:", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (item.Name.Contains("$"))
                    continue;
                AutoComplete.Add(item.Name);
            }

            AutoComplete.Sort((x, y) => string.Compare(x, y, StringComparison.OrdinalIgnoreCase));
            SetDataGrid();
            AddListener();
            SetControlEnable();

            if (_controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    (Controller) _controller, "IsOnlineChanged", OnIsOnlineChanged);
            }

            PropertyChangedEventManager.AddHandler(ParametersRows, OnPropertyChanged, string.Empty);
            WeakEventManager<ObservableCollection<BaseTagRow>, NotifyCollectionChangedEventArgs>.AddHandler(
                ParametersRows.Child, "CollectionChanged", Child_CollectionChanged);

            MenuOpeningCommand=new RelayCommand<ContextMenuEventArgs>(ExecuteMenuOpeningCommand);
        }
        public RelayCommand CopyCommand { get; }

        private void ExecuteCopyCommand()
        {
            if (SelectedItems?.Count > 1)
            {
                var list = new List<BaseTagRow>();
                foreach (BaseTagRow selectedItem in SelectedItems)
                {
                    list.Add(selectedItem);
                }
                AoiContextMenu.ClipboardOperation.Copy(list, false, OnPropertyChanged);
            }
            else
                AoiContextMenu.ClipboardOperation.Copy(SelectedParameter, false, OnPropertyChanged);
        }

        private bool CanExecuteCopyCommand()
        {
            if (_aoiDefinition.IsSealed || _aoiDefinition.ParentController.IsOnline) return false;
            return true;
        }

        public RelayCommand CutCommand { get; }

        private void ExecuteCutCommand()
        {
            if (SelectedItems?.Count > 1)
            {
                var list = new List<BaseTagRow>();
                foreach (BaseTagRow selectedItem in SelectedItems)
                {
                    list.Add(selectedItem);
                }
                AoiContextMenu.ClipboardOperation.Copy(list, true, OnPropertyChanged);
            }
            else
                AoiContextMenu.ClipboardOperation.Copy(SelectedParameter, true, OnPropertyChanged);
        }

        private bool CanExecuteCutCommand()
        {
            if (_aoiDefinition.IsSealed || _aoiDefinition.ParentController.IsOnline) return false;
            if (SelectedItems?.Count > 1)
            {
                foreach (BaseTagRow selectedItem in SelectedItems)
                {
                    if (selectedItem.IsBlank()) return false;
                }

                return true;
            }

            if (SelectedParameter.IsBlank()) return false;

            return true;
        }

        public RelayCommand PasteCommand { get; }

        private void ExecutePasteCommand()
        {
            AoiContextMenu.ClipboardOperation.Paste(SelectedParameter, OnPropertyChanged);
        }

        private bool CanExecutePasteCommand()
        {
            if (SelectedItems?.Count > 1) return false;
            return SelectedParameter.IsBlank() && AoiContextMenu.Paste.CanPaste();
        }

        public RelayCommand DeleteCommand { get; }

        private void ExecuteDeleteCommand()
        {
            if (SelectedItems?.Count > 1)
            {
                AoiContextMenu.ClipboardOperation.Delete(SelectedItems, OnPropertyChanged);
            }
            else
            {
                AoiContextMenu.ClipboardOperation.Delete(SelectedParameter, OnPropertyChanged);
            }
        }

        private bool CanExecuteDeleteCommand()
        {
            if (_controller.IsOnline || _aoiDefinition.IsSealed) return false;
            if (SelectedItems?.Count > 1)
            {
                if (_aoiDefinition.ParentController.IsOnline || _aoiDefinition.IsSealed) return false;
                return true;
            }
            else
            {
                if (SelectedParameter.IsBlank() || SelectedParameter.ParentAddOnInstruction.IsSealed ||
                    !(SelectedParameter.IsMember) || _aoiDefinition.ParentController.IsOnline) return false;
                return true;
            }
        }
        #region Context menu
        public RelayCommand<ContextMenuEventArgs> MenuOpeningCommand { get; }

        public void ExecuteMenuOpeningCommand(ContextMenuEventArgs e)
        {
            var dataGrid = (DataGrid) e.Source;
            dataGrid.ContextMenu = null;
            if (SelectedItems?.Count > 1)
            {
                var list = new BaseTagRow[SelectedItems.Count];
                SelectedItems.CopyTo(list, 0);
                dataGrid.ContextMenu = AoiContextMenu.GetMultiContextMenu(list.ToList(), OnPropertyChanged);
            }
            else
            {
                if (SelectedParameter != null)
                {
                    dataGrid.ContextMenu = string.IsNullOrEmpty(SelectedParameter?.Name) ? AoiContextMenu.GetEmptyContextMenu(SelectedParameter, OnPropertyChanged) : AoiContextMenu.GetParameterContextMenu(SelectedParameter,this, OnPropertyChanged);
                }
            }

            if (dataGrid.ContextMenu != null)
            {
                dataGrid.ContextMenu.IsOpen = true;
            }
        }
        #endregion

        public IList SelectedItems { set; get; }

        //private bool _isMemberChanged;

        private void Child_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    if (((BaseTagRow) item).Left.Left > 0)
                    {
                        return;
                    }
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    var tagItem = (BaseTagRow) item;
                    if (tagItem.Left.Left > 0)
                    {
                        return;
                    }
                    _delList.Add(tagItem.Tag);
                }
            }
            IsDirty = true;
        }

        private List<ITag> _delList=new List<ITag>();

        public override void Cleanup()
        {
            PropertyChangedEventManager.RemoveHandler(ParametersRows, OnPropertyChanged, string.Empty);
            WeakEventManager<ObservableCollection<BaseTagRow>, NotifyCollectionChangedEventArgs>.RemoveHandler(
                ParametersRows.Child, "CollectionChanged", Child_CollectionChanged);
            RemoveListener();

            if (_controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                    (Controller) _controller, "IsOnlineChanged", OnIsOnlineChanged);
            }

            foreach (var row in ParametersRows.Child)
            {
                row?.RemoveListener();
            }
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                SetControlEnable();

                IsDirty = false;
            });
        }

        public void SetControlEnable()
        {
            if (_aoiDefinition.ParentController.IsOnline || _aoiDefinition.IsSealed)
            {
                OnlineEnable = false;
            }
            else
            {
                OnlineEnable = true;
            }

            for (int i = 0; i < ParametersRows.Child.Count; i++)
            {
                if (i == 0 || i == 1) continue;
                var row = (ParametersRow) ParametersRows.Child[i];
                if (row.Usage == Usage.Input || row.Usage == Usage.Output|| row.Usage == null)
                {
                    row.IsOnline = !OnlineEnable;
                    //row.ReqEnabled = OnlineEnable;
                    //row.VisEnabled = OnlineEnable;
                    //row.ConstantEnable = row.ConstantEnable;
                }
            }
        }

        public List<Tuple<string, bool, ITag>> GetParamInfos()
        {
            var list=new List<Tuple<string,bool,ITag>>();
            foreach (var item in ParametersRows.Child)
            {
                if (item.IsMember)
                {
                    if (string.IsNullOrEmpty(item.Name)) continue;
                    if (item.Req)
                    {
                        list.Add(new Tuple<string, bool, ITag>(item.Name, item.IsDirty, item.Tag));
                    }
                    else
                    {
                        if (item.Tag.IsRequired)
                        {
                            list.Add(new Tuple<string, bool, ITag>("", true, item.Tag));
                        }
                    }
                }
            }

            var delParamList = _delList.Where(p => p.IsRequired);
            foreach (var tag in delParamList)
            {
                if(string.IsNullOrEmpty(tag.Name))continue;
                list.Add(new Tuple<string, bool, ITag>("",false,tag));
                IsSequenceChanged = true;
            }
            return list;
        }
        
        public bool IsReadOnly => !OnlineEnable;

        public bool OnlineEnable
        {
            set
            {
                Set(ref _onlineEnable, value);
                RaisePropertyChanged("IsReadOnly");
            }
            get { return _onlineEnable; }
        }

        public void AddListener()
        {
            CollectionChangedEventManager.AddHandler(_aoiDefinition.Tags, Tags_CollectionChanged);
            foreach (var tag in _aoiDefinition.Tags)
            {
                PropertyChangedEventManager.AddHandler(tag, Tag_PropertyChanged, "Usage");
            }
        }

        public void RemoveListener()
        {
            CollectionChangedEventManager.RemoveHandler(_aoiDefinition.Tags, Tags_CollectionChanged);
            foreach (var tag in _aoiDefinition.Tags)
            {
                PropertyChangedEventManager.RemoveHandler(tag, Tag_PropertyChanged, "Usage");
            }
        }

        public string ErrorMessage { set; get; }

        public string ErrorReason { set; get; }

        public bool HasReport { set; get; }

        public bool IsClosed { set; get; }

        public void SetDataGrid()
        {
            foreach (var parameter in _aoiDefinition.GetParameterTags())
            {
                CreateItem(parameter);
            }

            BlankRowFactory();
        }

        public bool IsSequenceChanged { private set; get; } = false;

        public void Compare()
        {
            var parametersTag = _aoiDefinition.GetParameterTags();
            IsDirty = ParametersRows.Child.Count - 1 != parametersTag.Count;
            int i = 0;
            foreach (var item in parametersTag)
            {
                var p = item as Tag;
                if (i == 0 || i == 1)
                {
                    i++;
                    continue;
                }

                if (!item.Name.Equals(ParametersRows.Child[i].Name, StringComparison.OrdinalIgnoreCase))
                {
                    IsDirty = true;
                    return;
                }

                if (item.Usage != ParametersRows.Child[i].Usage)
                {
                    IsDirty = true;
                    return;
                }

                if (!item.DataTypeInfo.DataType.Name.Equals(ParametersRows.Child[i].DataType,
                    StringComparison.OrdinalIgnoreCase))
                {
                    IsDirty = true;
                    return;
                }

                if (p != null)
                {
                    var defaultValue = (p.DataWrapper.Data?.ToJToken() as JValue)?.Value?.ToString();
                    if (defaultValue != ParametersRows.Child[i].Default)
                    {
                        IsDirty = true;
                        return;
                    }
                }

                if (item.DisplayStyle != ParametersRows.Child[i].Style)
                {
                    IsDirty = true;
                    return;
                }

                if (p != null && p.IsRequired != ParametersRows.Child[i].Req)
                {
                    IsDirty = true;
                    return;
                }

                if (p != null && p.IsVisible != ParametersRows.Child[i].Vis)
                {
                    IsDirty = true;
                    return;
                }

                if (item.Description != ParametersRows.Child[i].Description)
                {
                    IsDirty = true;
                    return;
                }

                if (item.ExternalAccess != ParametersRows.Child[i].ExternalAccess)
                {
                    IsDirty = true;
                    return;
                }

                if (item.IsConstant != ParametersRows.Child[i].Constant)
                {
                    IsDirty = true;
                    return;
                }

                i++;
            }

        }
        
        public void Save()
        {
            var tagCollection = (TagCollection)_aoiDefinition.Tags;
            RemoveListener();
            foreach (var tag in _delList)
            {
                tagCollection.DeleteTag(tag, true, true, false);
            }

            if (_delList.Any())
                _vm.NeedReset = true;
            foreach (var item in ParametersRows.Child)
            {
                if (item.IsMember)
                {
                    if (string.IsNullOrEmpty(item.Name)) continue;
                    if (item.IsStructChanged)
                        _vm.NeedReset = true;
                    item.RemoveListener();
                    var parameter = (Tag)item.Tag;
                    Debug.Assert(parameter != null, item.Name);
                    parameter.Name = item.Name;
                    parameter.Usage = item.Usage ?? Usage.Input;

                    DataTypeInfo dataTypeInfo = _controller.DataTypes.ParseDataTypeInfo(item.DataType);

                    var dataWrapper = new DataWrapper(dataTypeInfo, null);
                    var value = ValueConverter.ConvertValue(
                        FormatOp.RemoveFormat(item.Default, item.Style != DisplayStyle.Ascii),
                        item.Style ?? DisplayStyle.NullStyle,
                        DisplayStyle.Decimal, dataTypeInfo.DataType.BitSize,
                        ValueConverter.SelectIntType(dataTypeInfo.DataType));
                    if (item.Usage != Usage.InOut)
                    {
                        JToken defaultValue = null;
                        if (item.Style == DisplayStyle.Exponential || item.Style == DisplayStyle.Float)
                        {
                            if (value.Equals("Infinity"))
                                defaultValue = new JValue(float.PositiveInfinity);
                            else if (value.Equals("-Infinity"))
                                defaultValue = new JValue(float.NegativeInfinity);
                            else
                                defaultValue = new JValue(float.Parse(value));
                        }
                        else
                        {
                            defaultValue = new JValue(int.Parse(value));
                        }

                        if (item.DataType.Equals("BOOL", StringComparison.OrdinalIgnoreCase))
                        {
                            dataWrapper.Data = new BoolField(defaultValue);
                        }
                        else if (item.DataType.Equals("SINT", StringComparison.OrdinalIgnoreCase))
                        {
                            dataWrapper.Data = new Int8Field(defaultValue);
                        }
                        else if (item.DataType.Equals("INT", StringComparison.OrdinalIgnoreCase))
                        {
                            dataWrapper.Data = new Int16Field(defaultValue);
                        }
                        else if (item.DataType.Equals("DINT", StringComparison.OrdinalIgnoreCase))
                        {
                            dataWrapper.Data = new Int32Field(defaultValue);
                        }
                        else if (item.DataType.Equals("LINT", StringComparison.OrdinalIgnoreCase))
                        {
                            dataWrapper.Data = new Int32Field(defaultValue);
                        }
                        else if (item.DataType.Equals("REAL", StringComparison.OrdinalIgnoreCase))
                        {
                            dataWrapper.Data = new RealField(defaultValue);
                        }

                    }

                    if (item.Usage == Usage.InOut && item.DataType.IndexOf('[') > 0)
                    {
                        var dataType = item.DataType.Substring(0, item.DataType.IndexOf('['));
                        var dim = int.Parse(item.DataType.Replace(dataType, "").Replace("[", "").Replace("]", ""));
                        parameter.UpdateDataWrapper(new DataWrapper(_controller.DataTypes[dataType], dim, 0, 0, item.Field?.ToJToken()),
                            item.Style ?? DisplayStyle.NullStyle);
                    }
                    else
                    {
                        parameter.UpdateDataWrapper(dataWrapper, item.Style ?? DisplayStyle.Decimal);
                    }
                    parameter.IsRequired = item.Req;
                    parameter.IsVisible = item.Vis;
                    parameter.Description = item.Description;
                    parameter.ExternalAccess = item.ExternalAccess ?? ExternalAccess.ReadWrite;
                    parameter.IsConstant = item.Constant;
                    parameter.ChildDescription = item.ChildDescription;
                    foreach (var child in item.Child) child.IsDirty = false;
                    item.IsDirty = false;
                    item.IsStructChanged = false;
                    item.AddListener();
                }
            }

            //tagCollection.Clear();
            foreach (var item in ParametersRows.Child)
            {
                if (item.IsMember)
                {
                    if (string.IsNullOrEmpty(item.Name)) continue;
                    tagCollection.AddTag(item.Tag,false,false);
                }
            }
            IsSequenceChanged = false;
            _delList.Clear();
            AddListener();
            IsDirty = false;
        }

        public List<ITag> GetOrderTags()
        {
            var list=new List<ITag>();
            foreach (var item in ParametersRows.Child)
            {
                if (item.IsMember)
                {
                    if (string.IsNullOrEmpty(item.Name)) continue;
                    list.Add(item.Tag);
                }
            }
            return list;
        }
        
        public List<string> AutoComplete { get; }

        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Req"
                || e.PropertyName == "Vis"
                || e.PropertyName == "VisEnabled"
                || e.PropertyName == "ReqEnabled"
                || e.PropertyName == "ConstantEnable"
                || e.PropertyName == "UsageEnabled"
                || e.PropertyName == "IsDirty"
                || e.PropertyName == "RowHeaderVisibility") return;

            if (e.PropertyName == "Default")
            {
                _vm.IsEnable = true;
            }
            if (e.PropertyName == "Dirty")
            {
                IsDirty = true;
            }
            else
            {
                var parametersRow = sender as ParametersRow;
                int index = ParametersRows.Child.IndexOf(parametersRow);
                PropertyChangedEventManager.RemoveHandler(parametersRow, OnPropertyChanged, string.Empty);
                //parametersRow.PropertyChanged -= OnPropertyChanged;
                parametersRow.SetDefaultProperties();
                PropertyChangedEventManager.AddHandler(parametersRow, OnPropertyChanged, string.Empty);
                //parametersRow.PropertyChanged += OnPropertyChanged;

                if (index == ParametersRows.Child.Count - 1)
                {
                    BlankRowFactory();
                }

                if (e.PropertyName == "Name")
                {
                    ChangedChildName(parametersRow);
                }

                if (e.PropertyName == "DataType")
                {
                    parametersRow.CanSetDescription = false;
                    parametersRow.Description = parametersRow.GetChildDescription(parametersRow.Name);
                    parametersRow.ResetChildDescription();
                    parametersRow.CanSetDescription = true;
                }

                //Compare();
                RaisePropertyChanged("ParametersRows");
            }

        }

        public void BlankRowFactory(int index=-1)
        {
            var list = ParametersRows.Child.ToList();
            var parameter = new ParametersRow(_aoiDefinition,
                new Tag(_aoiDefinition.Tags as TagCollection), ParametersRows.IsOnline,
                ParametersRows);
            parameter.ChildDescription = new JArray();
            parameter.IsMember = true;
            PropertyChangedEventManager.AddHandler(parameter, ParametersRows.OnChildPropertyChanged, string.Empty);
            PropertyChangedEventManager.AddHandler(parameter, OnPropertyChanged, string.Empty);
            parameter.DescriptionEnable = !_aoiDefinition.ParentController.IsOnline;

            if(index>-1)
                ParametersRows.Child.Insert(index,parameter);
            else
                ParametersRows.Child.Add(parameter);
            CheckMoveCommand(SelectedParameter);
            RaisePropertyChanged("ParametersRows");

            foreach (var item in list)
            {
                var parametersRow = item as ParametersRow;
                if (parametersRow != null)
                    parametersRow.ActivePropertyChangedEventHandler(nameof(parametersRow.RowHeaderVisibility));
            }
        }

        public ParametersRow SelectedParameter
        {
            set
            {
                if (_selectedParametersRow != value && value != null)
                {
                    CheckMoveCommand(value);
                }

                Set(ref _selectedParametersRow, value);
            }
            get { return _selectedParametersRow; }
        }

        public void CheckMoveCommand(ParametersRow selectedRow)
        {
            _canMoveDownCommand = false;
            _canMoveUpCommand = false;
            List<ParametersRow> childList = new List<ParametersRow>();
            foreach (ParametersRow item in ParametersRows.Child)
            {
                if (item.IsMember) childList.Add(item);
            }

            if (selectedRow != null && !string.IsNullOrEmpty(selectedRow.Name) &&
                selectedRow.Name != "EnableIn" && selectedRow.Name != "EnableOut" &&
                !selectedRow.Name.Contains(".") && !selectedRow.Name.Contains("["))
            {
                int index = childList.IndexOf(selectedRow);
                if (index == 2 && childList.Count > 4)
                {
                    _canMoveDownCommand = true;
                }
                else if (index == childList.Count - 2 && childList.Count > 4)
                {
                    _canMoveUpCommand = true;
                }
                else if (index < childList.Count - 2 && index > 2)
                {
                    _canMoveDownCommand = true;
                    _canMoveUpCommand = true;
                }
            }

            MoveUpCommand.RaiseCanExecuteChanged();
            MoveDownCommand.RaiseCanExecuteChanged();
        }

        #region Command

        public RelayCommand<DataGridRowHeader> DataGridRowHeaderCommand { set; get; }

        public void ExecuteDataGridRowHeaderCommand(DataGridRowHeader sender)
        {
            var row = sender.DataContext as ParametersRow;
            if (row != null)
            {
                var parametersViewModel =
                    VisualTreeHelpers.FindVisualParentOfType<UserControl>(sender).DataContext as ParametersViewModel;
                parametersViewModel?.CheckMoveCommand(row);
            }
        }

        public bool CanMoveUpCommand()
        {
            return _canMoveUpCommand;
        }

        public RelayCommand MoveUpCommand { set; get; }

        private void ExecuteMoveUpCommand()
        {
            int index = ParametersRows.Child.IndexOf(SelectedParameter);
            int fIndex = index - 1;
            while (fIndex > 1 && ParametersRows.Child[fIndex] != null)
            {
                if (ParametersRows.Child[fIndex].IsMember)
                {
                    if (ParametersRows.Child[fIndex].Tag.IsRequired)
                    {
                        IsSequenceChanged = true;
                    }
                    int length = GetChildLength(SelectedParameter);
                    index = index + length;
                    for (int i = length; i >= 0; i--)
                    {
                        ParametersRows.Child.Move(index, fIndex);
                    }

                    fIndex = 0;
                }
                else
                {
                    fIndex--;
                }
            }

            if (SelectedParameter.Tag.IsRequired)
            {
                IsSequenceChanged = true;
            }
            _vm.NeedReset = true;
            CheckMoveCommand(SelectedParameter);
            Compare();
        }

        public RelayCommand MoveDownCommand { set; get; }

        public bool CanMoveDownCommand()
        {
            return _canMoveDownCommand;
        }

        private void ExecuteMoveDownCommand()
        {
            int index = ParametersRows.Child.IndexOf(SelectedParameter);
            int bIndex = index + 1;
            int length = 0;
            while (bIndex < ParametersRows.Child.Count && ParametersRows.Child[bIndex] != null)
            {
                if (ParametersRows.Child[bIndex].IsMember)
                {
                    if (ParametersRows.Child[bIndex].Tag.IsRequired)
                    {
                        IsSequenceChanged = true;
                    }
                    bIndex = bIndex + GetChildLength((ParametersRow) ParametersRows.Child[bIndex]);
                    for (int i = length; i >= 0; i--)
                    {
                        ParametersRows.Child.Move(index, bIndex);
                    }

                    bIndex = ParametersRows.Child.Count;
                }
                else
                {
                    length++;
                    bIndex++;
                }
            }

            if (SelectedParameter.Tag.IsRequired)
                IsSequenceChanged = true;
            _vm.NeedReset = true;
            CheckMoveCommand(SelectedParameter);
            Compare();
        }

        public RelayCommand<KeyEventArgs> KeyUpCommand { get; }

        private void ExecuteKeyUpCommand(KeyEventArgs e)
        {
            if (SelectedParameter.Name != null && SelectedParameter.Name != "EnableIn" &&
                SelectedParameter.Name != "EnableOut")
            {
                if (e.Key == Key.Delete)
                {
                    if (SelectedParameter.IsMember)
                    {
                        ParametersRows.ClearBaseChild(SelectedParameter);
                        ParametersRows.Child.Remove(SelectedParameter);
                        Compare();
                    }
                }
            }
        }

        #endregion

        public int GetChildLength(ParametersRow parametersRow)
        {
            int length = 0;
            int point = ParametersRows.Child.IndexOf(parametersRow) + 1;
            while (point < ParametersRows.Child.Count && ParametersRows.Child[point] != null)
            {
                if (ParametersRows.Child[point].IsMember)
                {
                    point = ParametersRows.Child.Count;
                }
                else
                {
                    length++;
                    point++;
                }
            }

            return length;
        }

        public IList UsageList { set; get; }
        public IList ExternalAccessList { set; get; }

        public IController Controller => _controller;
        public object Owner { get; set; }
        public object Control { get; }

        public void LoadOptions()
        {

        }

        public ParametersRow ParametersRows { set; get; }

        public bool SaveOptions()
        {
            return true;
        }

        public bool IsDirty
        {
            set
            {
                ThreadHelper.JoinableTaskFactory.Run(async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    if (_isDirty != value)
                    {
                        Set(ref _isDirty, value);
                        IsDirtyChanged?.Invoke(this, new EventArgs());
                    }
                });
            }
            get { return _isDirty; }
        }
        
        public event EventHandler IsDirtyChanged;

        private void ChangedChildName(ParametersRow parametersRow)
        {
            PropertyChangedEventManager.RemoveHandler(parametersRow, OnPropertyChanged, string.Empty);
            //parametersRow.PropertyChanged -= OnPropertyChanged;
            foreach (var item in parametersRow.Child)
            {
                if (!string.IsNullOrEmpty(parametersRow.OldName) && !string.IsNullOrEmpty(item.Name))
                    item.Name = item.Name.Replace($"{parametersRow.OldName}.", $"{parametersRow.Name}.")
                        .Replace($"{parametersRow.OldName}[", $"{parametersRow.Name}[");
                if (item.Child.Count > 0)
                    ChangedChildName((ParametersRow) item);
            }

            PropertyChangedEventManager.AddHandler(parametersRow, OnPropertyChanged, string.Empty);
            //parametersRow.PropertyChanged += OnPropertyChanged;
        }

        private void Tag_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Usage")
            {
                var item = ParametersRows.Child.FirstOrDefault(c => c.Tag == sender);
                if (item != null)
                {
                    if (((ITag) sender).Usage == Usage.Local)
                    {
                        ParametersRows.Child.Remove(item);
                    }
                }
                else
                {
                    if (((ITag) sender).Usage != Usage.Local)
                    {
                        ParametersRows.Child.RemoveAt(ParametersRows.Child.Count - 1);
                        CreateItem((ITag) sender);
                        BlankRowFactory();
                    }
                }
            }
        }

        private void Tags_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                ParametersRows.Child.RemoveAt(ParametersRows.Child.Count - 1);
                foreach (ITag tag in e.NewItems)
                {
                    PropertyChangedEventManager.AddHandler(tag, Tag_PropertyChanged, "Usage");
                    if ((tag.Usage != Usage.Local))
                        CreateItem(tag);
                }

                BlankRowFactory();
            }

            if (e.OldItems != null)
                foreach (ITag tag in e.OldItems)
                {
                    var item = ParametersRows.Child.FirstOrDefault(c => c.Tag == tag);
                    if (item != null)
                    {
                        ParametersRows.Child.Remove(item);
                        item.RemoveListener();
                    }
                }
        }

        private void CreateItem(ITag parameter)
        {
            var p = parameter as Tag;
            ParametersRow parametersRow =
                new ParametersRow(_aoiDefinition, p, ParametersRows.IsOnline, ParametersRows);
            parametersRow.ChildDescription = p?.ChildDescription ?? new JArray();
            parametersRow.Name = parameter.Name;
            parametersRow.Description = parameter.Description ?? parameter.DataTypeInfo.DataType.Description;
            string dataType = "";
            dataType = parameter.DataTypeInfo.ToString();

            parametersRow.Style =
                parameter.DataTypeInfo.DataType.IsAtomic ? parameter.DisplayStyle : (DisplayStyle?) null;
            parametersRow.DataType = dataType;
            parametersRow.Usage = parameter.Usage;
            parametersRow.Req = p.IsRequired;
            parametersRow.Vis = p.IsVisible;
            parametersRow.Constant = parameter.IsConstant;
            if (parameter.Usage != Usage.InOut)
            {
                parametersRow.ExternalAccess = parameter.ExternalAccess;
                parametersRow.Default = FormatOp.ConvertField(p.DataWrapper.Data, parameter.DisplayStyle);
                parametersRow.Field = p.DataWrapper.Data;
            }
            else
            {
                parametersRow.ExternalAccess = null;
                parametersRow.Default = "";
            }

            if (parameter.Name == "EnableIn" || parameter.Name == "EnableOut")
            {
                parametersRow.NameEnabled = false;
                parametersRow.UsageEnabled = false;
                parametersRow.DataTypeEnabled = false;
                parametersRow.DefaultEnabled = false;
                parametersRow.StyleEnabled = false;
                parametersRow.ReqEnabled = false;
                parametersRow.VisEnabled = false;
                parametersRow.ExternalAccessEnabled = false;
                parametersRow.ConstantEnable = false;
            }

            if (parameter.Usage == Usage.InOut)
            {
                parametersRow.DefaultEnabled = false;
                parametersRow.ReqEnabled = false;
                parametersRow.VisEnabled = false;
                parametersRow.ExternalAccessEnabled = false;
            }

            parametersRow.IsMember = true;
            parametersRow.IsDirty = false;
            parametersRow.IsStructChanged = false;
            PropertyChangedEventManager.AddHandler(parametersRow, OnPropertyChanged, string.Empty);
            ParametersRows.Child.Add(parametersRow);
            PropertyChangedEventManager.AddHandler(parametersRow, ParametersRows.OnChildPropertyChanged, string.Empty);
        }

    }

    public sealed class ParametersRow : BaseTagRow
    {
        private Usage? _usage;
        private string _aliasFor;
        private string _default;
        private bool _reqEnabled = true;
        private bool _constantEnable = false;
        private bool _externalAccessEnabled = true;
        private IDataType _dataTypeL;
        private bool _isOnline;
        private bool _usageEnabled = true;

        public ParametersRow(IAoiDefinition aoiDefinition, ITag tag, bool isOnline, BaseTagRow parent) : base(
            aoiDefinition, tag, parent)
        {
            IsOnline = isOnline;
        }

        public Visibility RowHeaderVisibility => IsBlank() ? Visibility.Visible : Visibility.Collapsed;

        public override bool IsOnline
        {
            get { return _isOnline; }
            set
            {
                NoActionChange = true;
                _isOnline = value;
                OnPropertyChanged("ReqEnabled");
                OnPropertyChanged("VisEnabled");
                OnPropertyChanged("ConstantEnable");
                OnPropertyChanged("UsageEnabled");
                NoActionChange = false;
            }
        }

        public bool IsShowMessage { get; set; } = true;

        public override Usage? Usage
        {
            set
            {
                if (_usage != value)
                {
                    _usage = value;
                    ExternalAccess = ExternalAccess ?? Interfaces.DataType.ExternalAccess.ReadWrite;
                    if (_usage == Interfaces.Tags.Usage.InOut)
                    {
                        Req = true;
                        DefaultEnabled = false;
                        ReqEnabled = false;
                        ConstantEnable = true;
                        ExternalAccessEnabled = false;
                        ExternalAccess = null;
                        Default = "";
                        OnPropertyChanged();
                        OnPropertyChanged("DisplayUsage");
                        OnPropertyChanged("Default");
                    }
                    else
                    {
                        Constant = false;
                        if (!(DataType.Equals("BOOL", StringComparison.OrdinalIgnoreCase) ||
                              DataType.Equals("SINT", StringComparison.OrdinalIgnoreCase) ||
                              DataType.Equals("INT", StringComparison.OrdinalIgnoreCase) ||
                              DataType.Equals("DINT", StringComparison.OrdinalIgnoreCase) ||
                              DataType.Equals("LINT", StringComparison.OrdinalIgnoreCase) ||
                              DataType.Equals("REAL", StringComparison.OrdinalIgnoreCase)))
                        {
                            if (IsShowMessage)
                                MessageBox.Show("Tag can only be created as InOut parameter in Add-On Instruction",
                                    "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                            _usage = Interfaces.Tags.Usage.InOut;
                            OnPropertyChanged();
                        }

                        if (DataType == null || (DataType.Equals("BOOL", StringComparison.OrdinalIgnoreCase) ||
                                                 DataType.Equals("SINT", StringComparison.OrdinalIgnoreCase) ||
                                                 DataType.Equals("INT", StringComparison.OrdinalIgnoreCase) ||
                                                 DataType.Equals("DINT", StringComparison.OrdinalIgnoreCase) ||
                                                 DataType.Equals("LINT", StringComparison.OrdinalIgnoreCase) ||
                                                 DataType.Equals("REAL", StringComparison.OrdinalIgnoreCase)))
                        {
                            if (Default == "")
                            {
                                Default = "0";
                            }

                            ReqEnabled = true;
                            DefaultEnabled = true;
                            ConstantEnable = false;
                            ExternalAccessEnabled = true;
                            OnPropertyChanged();
                            OnPropertyChanged("DisplayUsage");
                            OnPropertyChanged("Default");
                        }
                    }
                }
            }
            get { return _usage; }
        }

        public bool ReqEnabled
        {
            set
            {
                _reqEnabled = value;
                OnPropertyChanged();
            }
            get
            {
                if (IsOnline) return false;
                return _reqEnabled;
            }
        }

        public bool UsageEnabled
        {
            set
            {
                _usageEnabled = value; 
                OnPropertyChanged();
            }
            get
            {
                if (IsOnline) return false;
                return _usageEnabled;
            }
        }

        public Visibility UsageVisibility { set; get; } = Visibility.Visible;

        public Visibility ReqVisibility { set; get; } = Visibility.Visible;

        public Visibility VisVisibility { set; get; } = Visibility.Visible;

        public Visibility ConstantVisibility { set; get; } = Visibility.Visible;

        public Visibility ExternalAccessVisibility { set; get; } = Visibility.Visible;

        public string DisplayUsage => _usage?.ToString();

        private void UpdateData(string oldOne, string newOne)
        {
            if (Usage == Interfaces.Tags.Usage.InOut || string.IsNullOrEmpty(oldOne) ||
                string.IsNullOrEmpty(newOne)) return;
            var oldDataTypeInfo = _controller.DataTypes.ParseDataTypeInfo(oldOne);
            var value = ValueConverter.ConvertValue(
                FormatOp.RemoveFormat(Default, Style != DisplayStyle.Ascii),
                Style ?? DisplayStyle.Decimal,
                DisplayStyle.Decimal, oldDataTypeInfo.DataType.BitSize,
                ValueConverter.SelectIntType(oldDataTypeInfo.DataType));
            JToken defaultValue = null;
            if (Usage != Interfaces.Tags.Usage.InOut)
            {
                if (Style == DisplayStyle.Exponential || Style == DisplayStyle.Float)
                {
                    if (value.Equals("Infinity"))
                        defaultValue = new JValue(float.PositiveInfinity);
                    else if (value.Equals("-Infinity"))
                        defaultValue = new JValue(float.NegativeInfinity);
                    else
                        defaultValue = new JValue(float.Parse(value));
                }
                else
                {
                    defaultValue = new JValue(int.Parse(value));
                }
            }

            var newDataTypeInfo = _controller.DataTypes.ParseDataTypeInfo(newOne);
            var oldDataWrapper = new DataWrapper(oldDataTypeInfo.DataType, oldDataTypeInfo.Dim1, oldDataTypeInfo.Dim2,
                oldDataTypeInfo.Dim3, defaultValue);
            var newDataWrapper = new DataWrapper(newDataTypeInfo.DataType, newDataTypeInfo.Dim1, newDataTypeInfo.Dim2,
                newDataTypeInfo.Dim3, null);
            DataHelper.Copy(newDataWrapper, oldDataWrapper);
            Field = newDataWrapper.Data;
        }

        public override string DataType
        {
            set
            {
                if (_dataType != value)
                {
                    if (string.IsNullOrEmpty(value)) return;
                    if (!IsInitial)
                    {
                        UpdateData(_dataType, value);
                        _dataType = value;
                        if (_dataType.IndexOf("[") < 0)
                        {
                            //Size = Controller.DataTypes[value].BitSize;
                            DataTypeL = Controller.DataTypes[value];
                        }
                        else
                        {
                            //Size = 0;
                            DataTypeL = Controller.DataTypes[value.Substring(0, value.IndexOf("["))];
                            ButtonVisibility = Visibility.Collapsed;
                        }

                        if (!DataTypeL.IsAtomic) UsageEnabled = false;
                        //SetChild(this);
                        IsInitial = true;
                    }
                    else
                    {
                        Regex regex2 =
                            new Regex(DataTypeValidate.ParameterDataTypeRegex);
                        if (!regex2.IsMatch(value))
                        {
                            _dataType = value;
                            OnPropertyChanged();
                            return;
                        }

                        UsageEnabled = true;
                        //Field = null;
                        string dataType = "";
                        Regex regex = new Regex(@"^bool\[[0-9]+\]", RegexOptions.IgnoreCase);

                        if (value.IndexOf("[") > 0)
                        {
                            dataType = value.Substring(0, value.IndexOf("["));
                            ButtonVisibility = Visibility.Collapsed;
                        }
                        else
                        {
                            dataType = value;
                        }

                        if (regex.IsMatch(value))
                        {
                            int dim = int.Parse(value.Substring(value.IndexOf("[") + 1).Replace("]", ""));
                            dim = (dim + 32 - 1) / 32 * 32;
                            value = $"{dataType}[{dim}]";
                        }

                        if (Controller.DataTypes[dataType] == null)
                        {
                            _dataType = value;
                            UsageEnabled = false;
                            OnPropertyChanged();
                            return;
                        }

                        if (Usage == Interfaces.Tags.Usage.InOut)
                        {
                            _dataType = value;
                            _default = "";
                            //UsageEnabled = false;
                            ExpanderVis = Visibility.Visible;
                            ExpanderCloseVis = Visibility.Collapsed;
                            if (Controller.DataTypes[dataType] == null) return;
                            if (_dataType.IndexOf("[") < 0)
                            {
                                // Size = Controller.DataTypes[value].BitSize;
                                if (!Flag)
                                    DataTypeL = Controller.DataTypes[value];
                            }
                            else
                            {
                                // Size = 0;
                                if (!Flag)
                                    DataTypeL = Controller.DataTypes[value.Substring(0, value.IndexOf("["))];
                            }

                            //SetChild(this);

                            if (_dataType.Equals("BOOL", StringComparison.OrdinalIgnoreCase) ||
                                _dataType.Equals("REAL", StringComparison.OrdinalIgnoreCase))
                            {
                                ExpanderVis = Visibility.Hidden;
                            }

                            OnPropertyChanged();
                            OnPropertyChanged("Default");
                            return;
                        }

                        OldDefaultValue = Default;
                        if (value?.IndexOf("[") > 0)
                        {
                            UpdateData(_dataType, value);
                            _dataType = value;
                            //UsageEnabled = false;
                            if (_dataType?.IndexOf("[") < 0)
                            {
                                if (!Flag)
                                    DataTypeL = Controller.DataTypes[value];
                            }
                            else
                            {
                                if (!Flag)
                                    DataTypeL = Controller.DataTypes[value.Substring(0, value.IndexOf("["))];
                            }

                            _default = "";
                            //SetChild(this);
                            OnPropertyChanged();
                            OnPropertyChanged("Default");
                            return;
                        }

                        UpdateData(_dataType, value);
                        _default = Field.ToJToken() is JValue ? (Field.ToJToken() as JValue)?.Value.ToString() : "0";

                        ChildDescription = new JArray();
                        _dataType = value;
                        ExpanderVis = Visibility.Visible;
                        ExpanderCloseVis = Visibility.Collapsed;
                        //SetChild(this);
                        if (_dataType.IndexOf("[") < 0)
                        {
                            if (!Flag)
                                DataTypeL = Controller.DataTypes[value];
                        }
                        else
                        {
                            if (!Flag)
                                DataTypeL = Controller.DataTypes[value.Substring(0, value.IndexOf("["))];
                        }
                    }

                    ExpanderVis = Visibility.Visible;
                    ExpanderCloseVis = Visibility.Collapsed;
                    if (_dataType.Equals("BOOL", StringComparison.OrdinalIgnoreCase) ||
                        _dataType.Equals("REAL", StringComparison.OrdinalIgnoreCase))
                    {
                        ExpanderVis = Visibility.Hidden;
                    }

                    OnPropertyChanged();
                    NoActionChange = true;
                    OnPropertyChanged("Default");
                }
                else
                {
                    OnPropertyChanged();
                    OnPropertyChanged("Default");
                }
            }
            get { return _dataType; }
        }

        public string AliasFor
        {
            set
            {
                if (_aliasFor != value)
                {
                    _aliasFor = value;
                    OnPropertyChanged();
                }
            }
            get { return _aliasFor; }
        }

        public override string Default
        {
            set
            {
                if (_default != value)
                {
                    value = FormatOp.RemoveFormat(value, Style != DisplayStyle.Ascii);
                    _default = value;
                    int flag = 0;
                    if (DataType == null) SetDefaultProperties();
                    if (string.IsNullOrEmpty(_default)) flag = 1;
                    if (flag < 1)
                    {
                        if (Usage == Interfaces.Tags.Usage.InOut) return;
                        if (!string.IsNullOrEmpty(Name))
                        {
                            if (DataType.Equals("SINT", StringComparison.OrdinalIgnoreCase))
                            {
                                ValueConverter.CheckValueOverflow(Style, value, ref flag, 127);
                            }
                            else if (DataType.Equals("INT", StringComparison.OrdinalIgnoreCase))
                            {
                                ValueConverter.CheckValueOverflow(Style, value, ref flag, Int16.MaxValue);
                            }
                            else if (DataType.Equals("DINT", StringComparison.OrdinalIgnoreCase))
                            {
                                ValueConverter.CheckValueOverflow(Style, value, ref flag, Int32.MaxValue);
                            }
                            else if (DataType.Equals("LINT", StringComparison.OrdinalIgnoreCase))
                            {
                                ValueConverter.CheckValueOverflow(Style, value, ref flag, Int64.MaxValue);
                            }
                            else if (DataType.Equals("BOOL", StringComparison.OrdinalIgnoreCase))
                            {
                                if (_default != "0" && _default != "1") flag = 3;
                            }
                        }


                        if (Style == DisplayStyle.Float)
                        {
                            if (value.Length > 9)
                                _default = (float.Parse(value)).ToString("E");
                            else
                            {
                                if (FormatOp.IsPositiveInfinity(value))
                                    _default = "1.$";
                                else if (FormatOp.IsNegativeInfinity(value))
                                    _default = "-1.$";
                                else
                                    _default = (float.Parse(value)).ToString("F");
                            }
                        }
                        else if (Style == DisplayStyle.Exponential)
                        {
                            if (FormatOp.IsPositiveInfinity(value))
                                _default = "1.#INF0000e+000";
                            else if (FormatOp.IsNegativeInfinity(value))
                                _default = "-1.#INF0000e+000";
                            else
                                _default = (float.Parse(value)).ToString("E");
                        }

                        if (flag == 0)
                        {
                            if (!string.IsNullOrEmpty(_default))
                            {
                                DefaultEnabled = true;
                            }

                            OnPropertyChanged();
                        }
                    }
                }
            }
            get
            {
                if (Usage == Interfaces.Tags.Usage.InOut) return "";
                if (string.IsNullOrEmpty(_default) || Style == null || DataTypeL == null) return _default;
                if (NoFormat) return _default;
                return FormatOp.FormatValue(_default, (DisplayStyle) Style, DataTypeL);
            }
        }

        public string ExternalAccessDisplay => ExternalAccess?.ToString();

        public bool ExternalAccessEnabled
        {
            set
            {
                _externalAccessEnabled = value;
                OnPropertyChanged();
            }
            get { return _externalAccessEnabled; }
        }

        public bool ConstantEnable
        {
            set
            {
                _constantEnable = value;
                OnPropertyChanged();
            }
            get
            {
                if (IsOnline) return false;
                return _constantEnable;
            }
        }

        public override void SetDefaultProperties()
        {
            if (string.IsNullOrEmpty(DataType)) DataType = "DINT";
            if (Usage == null) Usage = Interfaces.Tags.Usage.Input;
            if (string.IsNullOrEmpty(_default))
            {
                if (Usage == Interfaces.Tags.Usage.InOut)
                    Default = "";
                else
                    Default = "0";
            }

            if (ExternalAccess == null)
            {
                if (Usage != Interfaces.Tags.Usage.InOut)
                    ExternalAccess = Interfaces.DataType.ExternalAccess.None;
            }
        }

        public DataTypeInfo GetDataTypeInfo(string oDataType)
        {
            int dim1 = 0, dim2 = 0, dim3 = 0;
            string dataTypeString = oDataType;
            if (dataTypeString.IndexOf("[") > 0)
            {
                dataTypeString = dataTypeString.Substring(0, dataTypeString.IndexOf("["));
                string dimArray = oDataType.Replace(dataTypeString, "").Replace("[", "").Replace("]", "");
                if (dimArray.Split(',').Length == 1)
                {
                    dim1 = int.Parse(dimArray);
                }
                else if (dimArray.Split(',').Length == 2)
                {
                    dim1 = int.Parse(dimArray.Split(',')[1]);
                    dim2 = int.Parse(dimArray.Split(',')[0]);
                }
                else if (dimArray.Split(',').Length == 3)
                {
                    dim1 = int.Parse(dimArray.Split(',')[2]);
                    dim2 = int.Parse(dimArray.Split(',')[1]);
                    dim3 = int.Parse(dimArray.Split(',')[0]);
                }
                else
                {
                    dataTypeString = "";
                    dim1 = 0;
                    dim2 = 0;
                    dim3 = 0;
                }
            }

            DataTypeInfo dataTypeInfo = new DataTypeInfo();
            dataTypeInfo.DataType = Controller.DataTypes[dataTypeString];
            dataTypeInfo.Dim1 = dim1;
            dataTypeInfo.Dim2 = dim2;
            dataTypeInfo.Dim3 = dim3;
            return dataTypeInfo;
        }

        [NotifyPropertyChangedInvocator]
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName?.IndexOf("enable", StringComparison.OrdinalIgnoreCase) < 0 
                && !nameof(ExpanderCloseVis).Equals(propertyName) && !nameof(ExpanderVis).Equals(propertyName) && !nameof(IsDirty).Equals(propertyName))
            {
                IsDirty = true;

                if (propertyName != "Default"&& propertyName != "Description")
                {
                    IsStructChanged = true;
                }
            }

            if (!NoActionChange)
            {
                if (propertyName == "Default" && DataType != null && Usage != Interfaces.Tags.Usage.InOut)
                {
                    if (!IsBChanged)
                    {
                        CollectionChangedEventManager.RemoveHandler(B, OnCollectionChanged);
                        //B.CollectionChanged -= OnCollectionChanged;
                        SetB();
                        CollectionChangedEventManager.AddHandler(B, OnCollectionChanged);
                        //B.CollectionChanged += OnCollectionChanged;
                    }

                    if (!IsChildChanged)
                    {
                        ChildOffMonitor();
                        ResetChildDefault();
                        ChildOnMonitor();
                    }
                }

                if (propertyName == "Description")
                {
                    ChangeDescription();
                }

                if (propertyName == "ExternalAccess")
                {
                    foreach (var row in Child)
                    {
                        row.ExternalAccess = ExternalAccess;
                    }
                }
            }

            ActivePropertyChangedEventHandler(propertyName);
        }

        protected override void SetChild(BaseTagRow baseTagRow)
        {
            var parametersRow = baseTagRow as ParametersRow;
            if (parametersRow?.DataType == null ||
                parametersRow.DataType.Equals("bool", StringComparison.OrdinalIgnoreCase)) return;
            parametersRow.Child.Clear();
            int dim = 0;

            string dataTypeString = parametersRow.DataType;
            DataTypeInfo dataTypeInfo = GetDataTypeInfo(dataTypeString);
            if (dataTypeInfo.DataType == null) return;
            if (dataTypeString.IndexOf("[") > 0)
            {
                dataTypeString = dataTypeString.Substring(0, dataTypeString.IndexOf("["));
                if (dataTypeString.Equals("BOOL", StringComparison.OrdinalIgnoreCase) && parametersRow.DataType
                        .Replace(dataTypeString, "").Replace("[", "").Replace("]", "").Split(',').Length > 1) return;
                dim = dataTypeInfo.Dim1;
            }

            IDataType dataType = Controller.DataTypes[dataTypeString];
            if (dataType is CompositiveType)
            {
                parametersRow.ExpanderCloseVis = Visibility.Collapsed;
                parametersRow.ExpanderVis = Visibility.Visible;
                if (dim > 0)
                {
                    SetArrayChild(dataTypeInfo, parametersRow);
                }
                else
                    foreach (var itemMember in (dataType as CompositiveType).TypeMembers)
                    {
                        if(itemMember.IsHidden)continue;
                        if (dataType is AOIDataType)
                        {
                            if (!(dataType as AOIDataType).IsMemberShowInOtherAoi(itemMember.Name)) continue;
                        }

                        ParametersRow child = new ParametersRow(ParentAddOnInstruction, null, IsOnline, this)
                        {
                            Left = new Thickness(parametersRow.Left.Left + 10, 0, 0, 0)
                        };
                        child.ChildDescription = ChildDescription;
                        child.Name = $"{parametersRow.Name}.{itemMember.Name}";
                        child.DataType = itemMember.DataTypeInfo.Dim1 > 0
                            ? $"{FormatOp.ConvertMemberName(itemMember.DataTypeInfo.DataType.Name)}[{itemMember.DataTypeInfo.Dim1}]"
                            : FormatOp.ConvertMemberName(itemMember.DataTypeInfo.DataType.Name);
                        child.Default = "";
                        child.CanSetDescription = false;
                        child.Description = GetChildDescription(child.Name);
                        child.CanSetDescription = true;
                        child.Style = itemMember.DisplayStyle;
                        child.ExternalAccess = parametersRow.ExternalAccess;
                        child.NameEnabled = false;
                        child.UsageEnabled = false;
                        child.DataTypeEnabled = false;
                        child.DefaultEnabled = false;
                        child.StyleEnabled = false;
                        child.ReqEnabled = false;
                        child.VisEnabled = false;
                        child.ExternalAccessEnabled = false;
                        child.ConstantEnable = false;
                        child.UsageVisibility = Visibility.Hidden;
                        child.ReqVisibility = Visibility.Hidden;
                        child.VisVisibility = Visibility.Hidden;
                        child.ExternalAccessVisibility = Visibility.Hidden;
                        child.ConstantVisibility = Visibility.Hidden;
                        child.IsDirty = parametersRow.IsDirty;
                        parametersRow.Child.Add(child);
                        PropertyChangedEventManager.AddHandler(child, parametersRow.OnChildPropertyChanged,
                            string.Empty);
                        //SetChild(child);
                    }
            }
            else
            {
                if (dataType is BOOL | dataType is REAL)
                {
                    if (dim > 0)
                    {
                        parametersRow.ExpanderCloseVis = Visibility.Collapsed;
                        parametersRow.ExpanderVis = Visibility.Visible;
                        SetArrayChild(dataTypeInfo, parametersRow);
                        return;
                    }
                    else
                        return;
                }

                parametersRow.ExpanderCloseVis = Visibility.Collapsed;
                parametersRow.ExpanderVis = Visibility.Visible;
                if (dataTypeInfo.Dim1 > 0)
                {
                    SetArrayChild(dataTypeInfo, parametersRow);
                }
                else
                {
                    for (int i = 0; i < dataType.BitSize; i++)
                    {
                        ParametersRow child = new ParametersRow(ParentAddOnInstruction, null, IsOnline, this)
                            {Left = new Thickness(parametersRow.Left.Left + 10, 0, 0, 0)};
                        child.ChildDescription = parametersRow.ChildDescription;
                        child.Name = $"{parametersRow.Name}.{i}";
                        child.DataType = "BOOL";
                        if (parametersRow.Field == null)
                        {
                            child.Default = "";
                            child.DefaultEnabled = false;
                        }
                        else
                        {
                            child.Default = parametersRow.Field.GetBitValue(i) ? "1" : "0";
                        }

                        child.Style = DisplayStyle.Decimal;
                        child.ExternalAccess = parametersRow.ExternalAccess;
                        child.CanSetDescription = false;
                        child.Description = GetChildDescription(child.Name);
                        child.CanSetDescription = true;
                        child.NameEnabled = false;
                        child.UsageEnabled = false;
                        child.DataTypeEnabled = false;
                        child.StyleEnabled = false;
                        child.ReqEnabled = false;
                        child.VisEnabled = false;
                        child.ExternalAccessEnabled = false;
                        child.ConstantEnable = false;
                        child.UsageVisibility = Visibility.Hidden;
                        child.ReqVisibility = Visibility.Hidden;
                        child.VisVisibility = Visibility.Hidden;
                        child.ConstantVisibility = Visibility.Hidden;
                        child.IsDirty = parametersRow.IsDirty;
                        PropertyChangedEventManager.AddHandler(child, parametersRow.OnChildPropertyChanged,
                            string.Empty);
                        //child.PropertyChanged += parametersRow.OnChildPropertyChanged;
                        parametersRow.Child.Add(child);
                    }
                }

            }
        }

        public override IDataType DataTypeL
        {
            protected set
            {
                Flag = false;
                if (_dataTypeL != null)
                    PropertyChangedEventManager.RemoveHandler(_dataTypeL, DataTypePropertyChanged, string.Empty);
                _dataTypeL = value;

                if (_dataTypeL != null)
                    PropertyChangedEventManager.AddHandler(_dataTypeL, DataTypePropertyChanged, string.Empty);
                IsDataTypeInitialize = true;
                ResetStyle();
                IsDataTypeInitialize = false;
            }
            get { return _dataTypeL; }
        }

        private void SetArrayChild(DataTypeInfo dataTypeInfo, ParametersRow parametersRow)
        {
            var list = new List<BaseTagRow>();
            var index = parametersRow.Child.Count;
            if (dataTypeInfo.Dim3 > 0)
            {
                for (int i = 0; i < dataTypeInfo.Dim3; i++)
                {
                    for (int j = 0; j < dataTypeInfo.Dim2; j++)
                    {
                        for (int k = 0; k < dataTypeInfo.Dim1; k++)
                        {
                            ParametersRow child = CreateArrayChild($"{Name}[{i},{j},{k}]", dataTypeInfo.DataType.Name,
                                parametersRow);
                            list.Add(child);
                        }
                    }
                }
            }
            else if (dataTypeInfo.Dim2 > 0)
            {
                for (int j = 0; j < dataTypeInfo.Dim2; j++)
                {
                    for (int k = 0; k < dataTypeInfo.Dim1; k++)
                    {
                        ParametersRow child = CreateArrayChild($"{Name}[{j},{k}]", dataTypeInfo.DataType.Name,
                            parametersRow);
                        list.Add(child);
                    }
                }
            }
            else if (dataTypeInfo.Dim1 > 0)
            {
                for (int k = 0; k < dataTypeInfo.Dim1; k++)
                {
                    ParametersRow child = CreateArrayChild($"{Name}[{k}]", dataTypeInfo.DataType.Name, parametersRow);
                    list.Add(child);
                }
            }

            parametersRow.Child.AddRange(index, list);
        }

        private ParametersRow CreateArrayChild(string name, string dateType, ParametersRow parametersRow)
        {
            ParametersRow child = new ParametersRow(ParentAddOnInstruction, null, IsOnline, this);
            IsInCreate = true;
            NoActionChange = true;
            child.CanSetDescription = false;
            child.ChildDescription = ChildDescription;
            child.Name = name;
            child.DataType = dateType;
            child.Default = "";

            child.Style = parametersRow.Style;
            child.ExternalAccess = null;
            child.NameEnabled = false;
            child.UsageEnabled = false;
            child.DataTypeEnabled = false;
            child.DefaultEnabled = false;
            child.StyleEnabled = false;
            child.ReqEnabled = false;
            child.VisEnabled = false;
            child.ExternalAccessEnabled = false;
            child.ConstantEnable = false;
            child.UsageVisibility = Visibility.Hidden;
            child.ReqVisibility = Visibility.Hidden;
            child.VisVisibility = Visibility.Hidden;
            child.ExternalAccessVisibility = Visibility.Hidden;
            child.ConstantVisibility = Visibility.Hidden;
            child.Left = new Thickness(parametersRow.Left.Left + 10, 0, 0, 0);
            child.PropertyChanged += parametersRow.OnChildPropertyChanged;
            child.CanSetDescription = true;
            child.IsDirty = parametersRow.IsDirty;
            NoActionChange = false;
            IsInCreate = false;
            return child;
        }

        private void DataTypePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ExpanderCloseVis == Visibility.Visible)
            {
                ExpanderVis = Visibility.Visible;
                ExpanderCloseVis = Visibility.Collapsed;
            }

            Child.Clear();

            if (e.PropertyName == "Name")
            {
                Flag = true;
                DataType = _dataTypeL.Name;
            }
        }

    }
}