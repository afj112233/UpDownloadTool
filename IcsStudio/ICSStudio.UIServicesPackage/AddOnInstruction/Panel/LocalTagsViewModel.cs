using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Dialogs.BrowseString;
using ICSStudio.Dialogs.BrowseString.RichTextBoxExtend;
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
using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json.Linq;

namespace ICSStudio.UIServicesPackage.AddOnInstruction.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class LocalTagsViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private readonly IController _controller;
        private bool _isDirty;
        private LocalTagRow _selectLocalTagRow;
        private bool _onlineEnable;
        private readonly AoiDefinition _aoiDefinition;
        private readonly AddOnInstructionVM _vm;

        public LocalTagsViewModel(LocalTags panel, IAoiDefinition aoiDefinition, AddOnInstructionVM vm)
        {
            CopyCommand = new RelayCommand(ExecuteCopyCommand, CanExecuteCopyCommand);
            CutCommand = new RelayCommand(ExecuteCutCommand, CanExecuteCutCommand);
            PasteCommand = new RelayCommand(ExecutePasteCommand, CanExecutePasteCommand);
            DeleteCommand = new RelayCommand(ExecuteDeleteCommand, CanExecuteDeleteCommand);
            _vm = vm;
            Control = panel;
            panel.DataContext = this;
            LocalTagRows = new LocalTagRow(aoiDefinition, null, null) { IsBase = true };
            List<ExternalAccess> eaList = new List<ExternalAccess>();
            eaList.Add(ExternalAccess.ReadWrite);
            eaList.Add(ExternalAccess.ReadOnly);
            eaList.Add(ExternalAccess.None);
            ExternalAccessList = eaList.Select(x => { return new { DisplayName = x.ToString(), Value = x }; }).ToList();
            _aoiDefinition = (AoiDefinition)aoiDefinition;
            Contract.Assert(_aoiDefinition != null);

            DataGrid2RightMouseCommand = new RelayCommand<MouseButtonEventArgs>(ExecuteDataGrid2RightMouseCommand);
            KeyUpCommand = new RelayCommand<KeyEventArgs>(ExecuteKeyUpCommand);
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

            Controller controller = _aoiDefinition.ParentController as Controller;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }

            PropertyChangedEventManager.AddHandler(LocalTagRows, OnPropertyChanged, string.Empty);
            WeakEventManager<ObservableCollection<BaseTagRow>, NotifyCollectionChangedEventArgs>.AddHandler(
                LocalTagRows.Child, "CollectionChanged", Child_CollectionChanged);
            MenuOpeningCommand = new RelayCommand<ContextMenuEventArgs>(ExecuteMenuOpeningCommand);
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
                AoiContextMenu.ClipboardOperation.Copy(SelectedLocalTagRow, false, OnPropertyChanged);
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
                AoiContextMenu.ClipboardOperation.Copy(SelectedLocalTagRow, true, OnPropertyChanged);
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

            if (SelectedLocalTagRow.IsBlank()) return false;

            return true;
        }

        public RelayCommand PasteCommand { get; }

        private void ExecutePasteCommand()
        {
            AoiContextMenu.ClipboardOperation.Paste(SelectedLocalTagRow, OnPropertyChanged);
        }

        private bool CanExecutePasteCommand()
        {
            if (SelectedItems?.Count > 1) return false;
            return SelectedLocalTagRow.IsBlank() && AoiContextMenu.Paste.CanPaste();
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
                AoiContextMenu.ClipboardOperation.Delete(SelectedLocalTagRow, OnPropertyChanged);
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
                if (SelectedLocalTagRow.IsBlank() || SelectedLocalTagRow.ParentAddOnInstruction.IsSealed ||
                    !(SelectedLocalTagRow.IsMember) || _aoiDefinition.ParentController.IsOnline) return false;
                return true;
            }
        }

        public RelayCommand<ContextMenuEventArgs> MenuOpeningCommand { get; }
        public IList SelectedItems { set; get; }

        public void ExecuteMenuOpeningCommand(ContextMenuEventArgs e)
        {
            var dataGrid = (DataGrid)e.Source;
            dataGrid.ContextMenu = null;
            if (SelectedItems?.Count > 1)
            {
                var list = new BaseTagRow[SelectedItems.Count];
                SelectedItems.CopyTo(list, 0);
                dataGrid.ContextMenu = AoiContextMenu.GetMultiContextMenu(list.ToList(), OnPropertyChanged);
            }
            else
            {
                if (SelectedLocalTagRow != null)
                {
                    dataGrid.ContextMenu = string.IsNullOrEmpty(SelectedLocalTagRow.Tag.Name)
                        ? AoiContextMenu.GetEmptyContextMenu(SelectedLocalTagRow, OnPropertyChanged)
                        : AoiContextMenu.GetLocalContextMenu(SelectedLocalTagRow, OnPropertyChanged);
                }
            }

            if (dataGrid.ContextMenu != null)
            {
                dataGrid.ContextMenu.IsOpen = true;
            }
        }

        public IList ExternalAccessList { get; }

        private void Child_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    if (((BaseTagRow)item).Left.Left > 0)
                    {
                        return;
                    }
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    var tagItem = (BaseTagRow)item;
                    if (tagItem.Left.Left > 0)
                    {
                        return;
                    }

                    _delList.Add(tagItem.Tag);
                }
            }

            IsDirty = true;
        }

        private List<ITag> _delList = new List<ITag>();

        public override void Cleanup()
        {
            PropertyChangedEventManager.RemoveHandler(LocalTagRows, OnPropertyChanged, string.Empty);

            WeakEventManager<ObservableCollection<BaseTagRow>, NotifyCollectionChangedEventArgs>.RemoveHandler(
                LocalTagRows.Child, "CollectionChanged", Child_CollectionChanged);
            RemoveListener();
            Controller controller = _aoiDefinition.ParentController as Controller;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }

            foreach (var row in LocalTagRows.Child)
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
            if (_controller.IsOnline || _aoiDefinition.IsSealed)
            {
                OnlineEnable = false;
            }
            else
            {
                OnlineEnable = true;
            }
        }

        public bool OnlineEnable
        {
            set
            {
                Set(ref _onlineEnable, value);
                RaisePropertyChanged("IsReadOnly");
            }
            get { return _onlineEnable; }
        }

        public bool IsReadOnly => !OnlineEnable;

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

        public bool IsCloseCancel { set; get; }

        public void SetDataGrid()
        {
            foreach (var tag in _aoiDefinition.GetLocalTags())
            {
                CreateItem(tag);
            }

            BlankRowFactory();
        }

        public void Compare()
        {
            IsDirty = false;
            var localTags = _aoiDefinition.GetLocalTags();
            if (LocalTagRows.Child.Count - 1 != localTags.Count) IsDirty = true;
            int i = 0;
            foreach (var item in localTags)
            {
                if (!item.Name.Equals(LocalTagRows.Child[i].Name, StringComparison.OrdinalIgnoreCase))
                {
                    IsDirty = true;
                    return;
                }

                if (!item.DataTypeInfo.DataType.Name.Equals(LocalTagRows.Child[i].DataType,
                        StringComparison.OrdinalIgnoreCase))
                {
                    IsDirty = true;
                    return;
                }

                var defaultValue = (((item as Tag).DataWrapper.Data).ToJToken() as JValue).Value.ToString();
                if (defaultValue != LocalTagRows.Child[i].Default)
                {
                    IsDirty = true;
                    return;
                }

                if (item.DisplayStyle != LocalTagRows.Child[i].Style)
                {
                    IsDirty = true;
                    return;
                }

                if (item.Description != LocalTagRows.Child[i].Description)
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
            {
                _vm.NeedReset = true;
            }

            foreach (LocalTagRow item in LocalTagRows.Child)
            {
                if (item.IsMember)
                {
                    if (!item.IsDirty) continue;
                    if (item.IsStructChanged)
                        _vm.NeedReset = true;
                    item.RemoveListener();
                    if (item.IsMember)
                    {
                        if (string.IsNullOrEmpty(item.Name)) continue;

                        item.SetTag();
                    }
                    foreach (var child in item.Child) child.IsDirty = false;
                    item.IsDirty = false;
                    item.IsStructChanged = false;
                    tagCollection.AddTag(item.Tag, false, false);
                    item.AddListener();
                }
            }

            AddListener();
            IsDirty = false;
        }

        public List<ITag> GetOrderTags()
        {
            var list = new List<ITag>();
            foreach (var item in LocalTagRows.Child)
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
                var localTagRow = sender as LocalTagRow;
                PropertyChangedEventManager.RemoveHandler(localTagRow, OnPropertyChanged, String.Empty);
                localTagRow.SetDefaultProperties();
                PropertyChangedEventManager.AddHandler(localTagRow, OnPropertyChanged, String.Empty);
                int index = LocalTagRows.Child.IndexOf(localTagRow);
                if (index == LocalTagRows.Child.Count - 1)
                {
                    BlankRowFactory();
                }

                if (e.PropertyName == "Name")
                {
                    ChangedChildName(localTagRow);
                }

                if (e.PropertyName == "DataType")
                {
                    localTagRow.CanSetDescription = false;
                    localTagRow.Description = localTagRow.GetChildDescription(localTagRow.Name);
                    localTagRow.ResetChildDescription();
                    localTagRow.CanSetDescription = true;
                }

                //Compare();
                RaisePropertyChanged("LocalTagRows");
            }
        }

        public void BlankRowFactory()
        {
            var localTag = new LocalTagRow(_aoiDefinition,
                new Tag(_aoiDefinition.Tags as TagCollection), LocalTagRows);
            //localTag.ChildDescription = new JArray();
            localTag.IsMember = true;
            localTag.DescriptionEnable = !_aoiDefinition.ParentController.IsOnline;
            PropertyChangedEventManager.AddHandler(localTag, LocalTagRows.OnChildPropertyChanged, String.Empty);
            PropertyChangedEventManager.AddHandler(localTag, OnPropertyChanged, String.Empty);
            LocalTagRows.Child.Add(localTag);
            RaisePropertyChanged("LocalTagRows");
        }

        public LocalTagRow SelectedLocalTagRow
        {
            set { Set(ref _selectLocalTagRow, value); }
            get { return _selectLocalTagRow; }
        }

        public RelayCommand<MouseButtonEventArgs> DataGrid2RightMouseCommand { set; get; }
        private ContextMenu cm2;

        private void ExecuteDataGrid2RightMouseCommand(MouseButtonEventArgs e)
        {
            ((DataGrid)e.Source).CommitEdit();
            DependencyObject obj = (DependencyObject)e.OriginalSource;
            obj = VisualTreeHelpers.FindVisualParentOfType<DataGridColumnHeader>(obj);
            if (obj == null || (!((DataGridColumnHeader)obj).Content.Equals("Name"))) return;
            DataGridColumnHeader header = obj as DataGridColumnHeader;
            if (header.Content == null) return;
            if (header.ContextMenu != null) return;
            if (cm2 == null) cm2 = new ContextMenu();
            MenuItem mi;
            mi = new MenuItem() { Header = "Sort Column", IsEnabled = true };
            mi.CommandParameter = header;
            WeakEventManager<MenuItem, RoutedEventArgs>.AddHandler(mi, "Click", SortClick);
            //mi.Click += SortClick;
            cm2.Items.Insert(0, mi);

            MenuItem mi2;
            mi2 = new MenuItem() { Header = "Include Tag Members In Sorting", IsEnabled = true };
            mi2.CommandParameter = header;
            WeakEventManager<MenuItem, RoutedEventArgs>.AddHandler(mi2, "Click", Mi2_Click);
            //mi2.Click += Mi2_Click;
            cm2.Items.Insert(1, mi2);

            header.ContextMenu = cm2;
        }

        private bool _descending, _includeMember;

        private void Mi2_Click(object sender, RoutedEventArgs e)
        {
            _includeMember = !_includeMember;
            RaisePropertyChanged("LocalTagRows");
            ((MenuItem)sender).IsChecked = _includeMember;
        }

        private void SortClick(object sender, RoutedEventArgs e)
        {
            _descending = !_descending;
            var selectedItem = SelectedLocalTagRow;
            var defaultView = CollectionViewSource.GetDefaultView(LocalTagRows.Child) as ListCollectionView;
            if (defaultView != null)
            {
                if (defaultView.IsAddingNew || defaultView.IsEditingItem)
                    return;

                using (defaultView.DeferRefresh())
                {
                    defaultView.CustomSort = new TagRowCompare(_descending, _includeMember);
                }
            }

            SelectedLocalTagRow = selectedItem;
        }

        public RelayCommand<KeyEventArgs> KeyUpCommand { get; }

        private void ExecuteKeyUpCommand(KeyEventArgs e)
        {
            if (SelectedLocalTagRow.Name != null)
            {
                if (e.Key == Key.Delete)
                {
                    if (SelectedLocalTagRow.IsMember)
                    {
                        LocalTagRows.ClearBaseChild(SelectedLocalTagRow);
                        LocalTagRows.Child.Remove(SelectedLocalTagRow);
                        Compare();
                    }
                }
            }
        }

        public IController Controller => _controller;
        public object Owner { get; set; }
        public object Control { get; }

        public void LoadOptions()
        {

        }

        public LocalTagRow LocalTagRows { set; get; }

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

        private void ChangedChildName(LocalTagRow localTagRow)
        {
            PropertyChangedEventManager.RemoveHandler(localTagRow, OnPropertyChanged, string.Empty);
            //localTagRow.PropertyChanged -= OnPropertyChanged;
            foreach (LocalTagRow item in localTagRow.Child)
            {
                if (!string.IsNullOrEmpty(localTagRow.OldName) && !string.IsNullOrEmpty(item.Name))
                    item.Name = item.Name.Replace($"{localTagRow.OldName}.", $"{localTagRow.Name}.")
                        .Replace($"{localTagRow.OldName}[", $"{localTagRow.Name}[");
                if (item.Child.Count > 0)
                    ChangedChildName(item);
            }

            PropertyChangedEventManager.AddHandler(localTagRow, OnPropertyChanged, string.Empty);
            //localTagRow.PropertyChanged += OnPropertyChanged;
        }

        private void Tag_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Usage")
            {
                var item = LocalTagRows.Child.FirstOrDefault(c => c.Tag == sender);
                if (item != null)
                {
                    if (((ITag)sender).Usage != Usage.Local)
                    {
                        LocalTagRows.Child.Remove(item);
                    }
                }
                else
                {
                    if (((ITag)sender).Usage == Usage.Local)
                    {
                        LocalTagRows.Child.RemoveAt(LocalTagRows.Child.Count - 1);
                        CreateItem((ITag)sender);
                        BlankRowFactory();
                    }
                }
            }
        }

        private void Tags_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                LocalTagRows.Child.RemoveAt(LocalTagRows.Child.Count - 1);
                foreach (ITag tag in e.NewItems)
                {
                    PropertyChangedEventManager.AddHandler(tag, Tag_PropertyChanged, "Usage");
                    if ((tag.Usage == Usage.Local))
                        CreateItem(tag);
                }

                BlankRowFactory();
            }

            if (e.OldItems != null)
                foreach (ITag tag in e.OldItems)
                {
                    var item = LocalTagRows.Child.FirstOrDefault(c => c.Tag == tag);
                    if (item != null)
                    {
                        LocalTagRows.Child.Remove(item);
                        item.RemoveListener();
                    }
                }
        }

        private void CreateItem(ITag tag)
        {
            var tag2 = tag as Tag;

            string dataType = "";
            dataType = tag.DataTypeInfo.ToString();
            LocalTagRow localTagRow = new LocalTagRow(_aoiDefinition, tag, LocalTagRows);
            localTagRow.ExternalAccess = tag.ExternalAccess;
            localTagRow.Field = (tag as Tag)?.DataWrapper?.Data.DeepCopy();
            localTagRow.Name = tag.Name;
            if ("XConnOK".Equals(localTagRow.Name))
            {

            }
            localTagRow.ChildDescription = tag2?.ChildDescription ?? new JArray();
            //localTagRow.Field = tag2.DataWrapper?.Data;
            if (!(tag.DataTypeInfo.DataType is CompositiveType))
            {
                localTagRow.Style = tag.DisplayStyle;
            }
            else
            {
                localTagRow.StyleEnabled = false;
            }

            localTagRow.DataType = dataType;

            string value = "";
            if (tag.DataTypeInfo.DataType != null)
            {
                value = tag.DataTypeInfo.DataType is CompositiveType
                    ? ""
                    : tag.DataTypeInfo.Dim1 > 0
                        ? ""
                        : FormatOp.ConvertField(tag2?.DataWrapper.Data, tag2.DisplayStyle);
            }

            localTagRow.Description = tag.Description ?? tag.DataTypeInfo.DataType?.Description;
            if (tag.DataTypeInfo.DataType?.FamilyType != FamilyType.StringFamily)
                localTagRow.Default = value;

            LocalTagRows.Child.Add(localTagRow);

            if (localTagRow.DataType.Equals("BOOL", StringComparison.OrdinalIgnoreCase) ||
                localTagRow.DataType.Equals("SINT", StringComparison.OrdinalIgnoreCase) ||
                localTagRow.DataType.Equals("INT", StringComparison.OrdinalIgnoreCase) ||
                localTagRow.DataType.Equals("DINT", StringComparison.OrdinalIgnoreCase) ||
                localTagRow.DataType.Equals("LINT", StringComparison.OrdinalIgnoreCase) ||
                localTagRow.DataType.Equals("REAL", StringComparison.OrdinalIgnoreCase))
            {
                string binary = localTagRow.Default == null
                    ? ""
                    : ValueConverter.ConvertGenericAnyToBinary(localTagRow.Default, localTagRow.Style,
                        Controller.DataTypes[localTagRow.DataType].BitSize);
                binary = FormatOp.RemoveFormat(binary, false);
                for (int i = 0; i < binary.Length; i++)
                {
                    try
                    {
                        localTagRow.B[binary.Length - i - 1] = int.Parse(binary.Substring(i, 1));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }

            localTagRow.ExternalAccess = tag.ExternalAccess;
            localTagRow.IsMember = true;
            localTagRow.IsDirty = false;
            PropertyChangedEventManager.AddHandler(localTagRow, OnPropertyChanged, string.Empty);
            PropertyChangedEventManager.AddHandler(localTagRow, LocalTagRows.OnChildPropertyChanged, string.Empty);
        }
    }

    public sealed class LocalTagRow : BaseTagRow
    {
        private string _default;

        public LocalTagRow(IAoiDefinition aoiDefinition, ITag tag, BaseTagRow parent) : base(aoiDefinition,
            tag, parent)
        {
            BrowseStringCommand = new RelayCommand(ExecuteBrowseStringCommand);
            Usage = Interfaces.Tags.Usage.Local;
        }

        public RelayCommand BrowseStringCommand { set; get; }

        private void ExecuteBrowseStringCommand()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            var message = new Message(null, Default, Name);
            PropertyChangedEventManager.AddHandler(message, Message_PropertyChanged, "Text");
            //var vm = new BrowseStringViewModel(DataTypeL, message);
            var dialog = new BrowseStringDialog(DataTypeL, message);
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
                Default = ((Message)sender).Text;
            }
        }

        private void UpdateData(string oldOne, string newOne)
        {
            var oldDataTypeInfo = _controller.DataTypes.ParseDataTypeInfo(oldOne);
            var newDataTypeInfo = _controller.DataTypes.ParseDataTypeInfo(newOne);
            var oldDataWrapper = new DataWrapper(oldDataTypeInfo.DataType, oldDataTypeInfo.Dim1, oldDataTypeInfo.Dim2,
                oldDataTypeInfo.Dim3, Field.ToJToken());
            var newDataWrapper = new DataWrapper(newDataTypeInfo.DataType, newDataTypeInfo.Dim1, newDataTypeInfo.Dim2,
                newDataTypeInfo.Dim3, null);
            DataHelper.Copy(newDataWrapper, oldDataWrapper);
            Field = newDataWrapper.Data;
        }

        public void SetTag()
        {
            if (!IsDirty) return;
            string dataType = "";
            int dim = 0;
            if (DataType.IndexOf('[') > 0)
            {
                dataType = DataType.Substring(0, DataType.IndexOf('['));
                dim = int.Parse(DataType.Replace(dataType, "").Replace("[", "").Replace("]", ""));
                if (dataType.Equals("BOOL", StringComparison.OrdinalIgnoreCase))
                {
                    dim = (dim + 32 - 1) / 32 * 32;
                    if (dim == 0)
                        DataType = dataType;
                    else
                        DataType = $"{dataType}[{dim}]";
                }
            }
            else
            {
                dataType = DataType;
            }

            var tag = (Tag)TmpTag.Tag;
            Debug.Assert(tag != null);
            tag.Name = Name;

            tag.UpdateDataWrapper(new DataWrapper(_controller.DataTypes[dataType], dim, 0, 0, Field?.ToJToken()),
                Style ?? DisplayStyle.NullStyle);
            tag.Description = Description;
            tag.ChildDescription = ChildDescription;
            tag.Usage = Interfaces.Tags.Usage.Local;
            tag.ExternalAccess = ExternalAccess ?? Interfaces.DataType.ExternalAccess.None;
        }

        public LocalTagRow ParentTagRow { set; get; }

        public override string DataType
        {
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (_dataType != value)
                {
                    DefaultEnabled = true;
                    ButtonVisibility = Visibility.Visible;
                    if (IsInCreate)
                    {
                        _dataType = value;

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

                        if (_dataType == null)
                        {
                            ExpanderVis = Visibility.Visible;
                            ExpanderCloseVis = Visibility.Collapsed;
                            return;
                        }

                        goto Skin;
                    }

                    if (!IsInitial)
                    {
                        _dataType = value;
                        ExpanderVis = Visibility.Visible;
                        ExpanderCloseVis = Visibility.Collapsed;
                        var dataTypeInfo = Controller.DataTypes.ParseDataTypeInfo(value);
                        DataTypeL = dataTypeInfo.DataType;
                        if (Field == null)
                        {
                            Field = (new DataWrapper(dataTypeInfo.DataType, dataTypeInfo.Dim1, dataTypeInfo.Dim2,
                                dataTypeInfo.Dim3, null)).Data;
                        }

                        IsInitial = true;
                    }
                    else
                    {
                        Regex regex2 = new Regex(DataTypeValidate.LocalDataTypeRegex);
                        if (!regex2.IsMatch(value))
                        {
                            _dataType = value;
                            OnPropertyChanged();
                            return;
                        }

                        string dataType = "";
                        if (value.IndexOf("[") > 0)
                        {
                            dataType = value.Substring(0, value.IndexOf("["));
                        }
                        else
                        {
                            dataType = value;

                        }

                        if (dataType.Equals("BOOL", StringComparison.OrdinalIgnoreCase) && value?.IndexOf("[") > 0)
                        {
                            var dim = value.Replace(dataType, "").Replace("[", "").Replace("]", "");
                            if (int.Parse(dim) > 0)
                            {
                                int boolDim = (int.Parse(dim) + 32 - 1) / 32 * 32;
                                value = $"{dataType}[{boolDim}]";
                            }
                        }

                        if (Controller.DataTypes[dataType] == null)
                        {
                            _dataType = value;
                            OnPropertyChanged();
                            return;
                        }

                        ChildDescription = new JArray();

                        if (string.IsNullOrEmpty(_dataType)) _dataType = "DINT";
                        OldDefaultValue = Default;
                        UpdateData(_dataType, value);

                        _dataType = value;

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

                        if (_dataType == null)
                        {
                            ExpanderVis = Visibility.Visible;
                            ExpanderCloseVis = Visibility.Collapsed;
                            return;
                        }
                    }

                    //Regex regex = new Regex(@"^(bool|sint|int|dint|real|LINT)(\[[0-9]+\])?", RegexOptions.IgnoreCase);
                    Skin:
                    if ((DataTypeL?.IsNumber ?? false) || (DataTypeL?.IsBool ?? false))
                    {
                        //_default = Data is JValue ? (Data as JValue)?.Value.ToString() : "0";
                        var data = Field?.ToJToken();
                        _default = data is JValue ? (data as JValue)?.Value.ToString() : "0";
                        //ResetStyle();
                        StyleEnabled = true;
                        if (_dataType.IndexOf('[') > 0)
                        {
                            _default = "";
                            DefaultEnabled = false;
                            ButtonVisibility = Visibility.Collapsed;
                            OnPropertyChanged("DotVisibility");
                            OnPropertyChanged("DotVisibility2");
                            OnPropertyChanged("ButtonVisibility");
                            OnPropertyChanged("StyleDisplay");
                            NoActionChange = true;
                            OnPropertyChanged("Default");
                        }
                    }
                    else
                    {
                        Style = null;
                        if (DataTypeL.FamilyType != FamilyType.StringFamily)
                            Default = "";
                        DefaultEnabled = false;
                        StyleEnabled = false;
                        ButtonVisibility = Visibility.Collapsed;
                        OnPropertyChanged("DotVisibility");
                        OnPropertyChanged("DotVisibility2");
                        OnPropertyChanged("ButtonVisibility");
                        OnPropertyChanged("StyleDisplay");
                        NoActionChange = true;
                        OnPropertyChanged("Default");
                    }

                    ExpanderVis = Visibility.Visible;
                    ExpanderCloseVis = Visibility.Collapsed;
                    //SetChild(this);
                    if (_dataType.Equals("BOOL", StringComparison.OrdinalIgnoreCase) ||
                        _dataType.Equals("REAL", StringComparison.OrdinalIgnoreCase))
                    {
                        ExpanderVis = Visibility.Hidden;
                    }
                }

                OnPropertyChanged();
                OnPropertyChanged("DotVisibility");
                OnPropertyChanged("DotVisibility2");
                OnPropertyChanged("Default");
            }
            get { return _dataType; }
        }

        public bool IsUpdateDefault { set; get; } = true;

        public override string Default
        {
            set
            {
                if (_default != value)
                {
                    if (IsInCreate || NoActionChange)
                    {
                        _default = value;
                        OnPropertyChanged();
                        return;
                    }

                    value = FormatOp.RemoveFormat(value, Style != DisplayStyle.Ascii);
                    _default = value;
                    if (_dataType != null && _dataType.IndexOf('[') > 0)
                    {
                        OnPropertyChanged();
                        return;
                    }

                    if (_dataType == null || _dataType.Equals("BOOL", StringComparison.OrdinalIgnoreCase) ||
                        _dataType.Equals("SINT", StringComparison.OrdinalIgnoreCase) ||
                        _dataType.Equals("INT", StringComparison.OrdinalIgnoreCase) ||
                        _dataType.Equals("DINT", StringComparison.OrdinalIgnoreCase) ||
                        _dataType.Equals("LINT", StringComparison.OrdinalIgnoreCase) ||
                        _dataType.Equals("REAL", StringComparison.OrdinalIgnoreCase))
                    {
                        int flag = 0;
                        if (DataType == null) SetDefaultProperties();
                        if (string.IsNullOrEmpty(_default)) flag = 1;
                        if (flag < 1)
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


                            if (Style == DisplayStyle.Float)
                            {
                                if (value.Length > 9)
                                    _default = (float.Parse(value)).ToString("E");
                                else
                                {
                                    if (FormatOp.IsPositiveInfinity(value))
                                    {
                                        _default = "1.$";
                                        SetField();
                                    }
                                    else if (FormatOp.IsNegativeInfinity(value))
                                    {
                                        _default = "-1.$";
                                        SetField();
                                    }
                                    else
                                    {
                                        _default = (float.Parse(value)).ToString("F");
                                        SetField();
                                    }
                                }
                            }
                            else if (Style == DisplayStyle.Exponential)
                            {
                                if (FormatOp.IsPositiveInfinity(value))
                                {
                                    _default = "1.#INF0000e+000";
                                    SetField();
                                }
                                else if (FormatOp.IsNegativeInfinity(value))
                                {
                                    _default = "-1.#INF0000e+000";
                                    SetField();
                                }
                                else
                                {
                                    _default = (float.Parse(value)).ToString("E");
                                    SetField();
                                }
                            }
                            else
                            {
                                if (flag == 0)
                                {
                                    _default = value;
                                    SetField();
                                }
                            }

                            OnPropertyChanged();
                        }
                    }
                    else if (DataTypeL.FamilyType == FamilyType.StringFamily)
                    {
                        _default = $"'{value}'";
                        OnPropertyChanged();
                    }
                    else
                    {
                        _default = "";
                        OnPropertyChanged();
                    }

                }
            }
            get
            {
                if (_default == null || Style == null || DataTypeL == null)
                {
                    return _default;
                }

                try
                {
                    if (NoFormat) return _default;
                    return FormatOp.FormatValue(_default, (DisplayStyle)Style, DataTypeL);
                }
                catch (Exception)
                {
                    return "";
                }

            }
        }

        private void SetField()
        {
            SetTagValue(_default);
        }

        private void SetTagValue(string textValue)
        {
            textValue = FormatOp.ConvertValue(textValue, Style ?? DisplayStyle.Decimal, DataTypeL);

            if (Field == null) return;
            //textValue = FormatOp.FormatValue(textValue, DisplayStyle, DataType);

            var boolArray = Field as BoolArrayField;
            if (boolArray != null)
            {
                boolArray.Set(FieldIndex, int.Parse(textValue) == 1);
                return;
            }

            var boolField = Field as BoolField;
            if (boolField != null)
            {
                boolField.value = ValueConverter.ToByte(textValue, Style ?? DisplayStyle.Decimal);
                return;
            }

            Int8Field int8Field = Field as Int8Field;
            if (int8Field != null)
            {
                if (IsBitSel)
                    int8Field.SetBitValue(FieldIndex, textValue != "0");
                else
                    int8Field.value = ValueConverter.ToSByte(textValue, Style ?? DisplayStyle.Decimal);
                return;
            }

            Int16Field int16Field = Field as Int16Field;
            if (int16Field != null)
            {
                if (IsBitSel)
                    int16Field.SetBitValue(FieldIndex, textValue != "0");
                else
                    int16Field.value = ValueConverter.ToShort(textValue, Style ?? DisplayStyle.Decimal);
                return;
            }

            Int32Field int32Field = Field as Int32Field;
            if (int32Field != null)
            {
                if (IsBitSel)
                    int32Field.SetBitValue(FieldIndex, textValue != "0");
                else
                    int32Field.value = ValueConverter.ToInt(textValue, Style ?? DisplayStyle.Decimal);
                return;
            }

            Int64Field int64Field = Field as Int64Field;
            if (int64Field != null)
            {
                if (IsBitSel)
                    int64Field.SetBitValue(FieldIndex, textValue != "0");
                else
                    int64Field.value = ValueConverter.ToLong(textValue, Style ?? DisplayStyle.Decimal);
                return;
            }

            RealField realField = Field as RealField;
            if (realField != null)
            {
                realField.value = ValueConverter.ToFloat(textValue);
                return;
            }

            LRealField lRealField = Field as LRealField;
            if (lRealField != null)
            {
                lRealField.value = ValueConverter.ToLong(textValue, Style ?? DisplayStyle.Decimal);
                return;
            }

            Debug.Assert(false, "error field.");
        }

        public Visibility DotVisibility
        {
            get
            {
                if (string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(DataType))
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    if (DataTypeL?.FamilyType == FamilyType.StringFamily) return Visibility.Collapsed;
                    if (DefaultEnabled) return Visibility.Collapsed;
                    else return Visibility.Visible;
                }
            }
        }

        public Visibility DotVisibility2
        {
            get
            {
                if (DotVisibility == Visibility.Visible) return Visibility.Collapsed;
                else return Visibility.Visible;
            }
        }

        public override void SetDefaultProperties()
        {
            if (string.IsNullOrEmpty(DataType)) DataType = "DINT";
            if (_dataType.Equals("BOOL", StringComparison.OrdinalIgnoreCase) ||
                _dataType.Equals("SINT", StringComparison.OrdinalIgnoreCase) ||
                _dataType.Equals("INT", StringComparison.OrdinalIgnoreCase) ||
                _dataType.Equals("DINT", StringComparison.OrdinalIgnoreCase) ||
                _dataType.Equals("LINT", StringComparison.OrdinalIgnoreCase) ||
                _dataType.Equals("REAL", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(_default)) Default = "0";
            }

            if (ExternalAccess == null) ExternalAccess = Interfaces.DataType.ExternalAccess.None;
        }

        [NotifyPropertyChangedInvocator]
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName?.IndexOf("enable", StringComparison.OrdinalIgnoreCase) < 0
                && !nameof(ExpanderCloseVis).Equals(propertyName) && !nameof(ExpanderVis).Equals(propertyName) && !nameof(IsDirty).Equals(propertyName))
            {
                IsDirty = true;

                if (propertyName != "Default" && propertyName != "Description")
                {
                    IsStructChanged = true;
                }
            }

            if (IsInCreate || NoActionChange)
            {
                ActivePropertyChangedEventHandler(propertyName);
                return;
            }

            if (!NoActionChange && !IsDataTypeInitialize)
            {
                if (propertyName == "Default" && DataType != null)
                {
                    if (IsUpdateDefault || IsBitSel)
                    {
                        if (ParentTagRow != null)
                        {
                            ParentTagRow.B[BitOffset] = int.Parse(Default);
                            ParentTagRow.ReSetDefaultValue();
                        }
                    }

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

                if (propertyName == "ExternalAccess")
                {
                    Parallel.ForEach(Child, item => { item.ExternalAccess = ExternalAccess; });
                }

                if (propertyName == "Description")
                {
                    ChangeDescription();
                }
            }

            ActivePropertyChangedEventHandler(propertyName);
        }

        protected override void SetChild(BaseTagRow baseTagRow)
        {
            var localTagRow = baseTagRow as LocalTagRow;
            if (localTagRow.DataType == null) return;
            localTagRow.Child.Clear();
            int dim = 0;
            string dataTypeString = localTagRow.DataType;
            if (localTagRow.Field == null)
            {
                var info = Controller.DataTypes.ParseDataTypeInfo(dataTypeString);
                localTagRow.Field = (new DataWrapper(info.DataType, info.Dim1, info.Dim2, info.Dim3, null)).Data;
            }

            if (dataTypeString.IndexOf("[", StringComparison.Ordinal) > 0)
            {
                dataTypeString = dataTypeString.Substring(0, dataTypeString.IndexOf("["));
                if (localTagRow.DataType.Replace(dataTypeString, "").Replace("[", "").Replace("]", "").Split(',')
                        .Length > 1) return;
                dim = int.Parse(localTagRow.DataType.Replace(dataTypeString, "").Replace("[", "").Replace("]", ""));
            }

            //IDataType dataType = Controller.DataTypes[dataTypeString];
            IDataType dataType = localTagRow.DataTypeL;

            IsInCreate = true;
            NoActionChange = true;
            if (dim > 0)
            {
                localTagRow.ExpanderCloseVis = Visibility.Collapsed;
                localTagRow.ExpanderVis = Visibility.Visible;
                var list = new List<BaseTagRow>();
                for (int i = 0; i < dim; i++)
                {
                    LocalTagRow child = new LocalTagRow(ParentAddOnInstruction, localTagRow.Tag, localTagRow)
                    {
                        IsInCreate = true,
                        ChildDescription = ChildDescription, Name = $"{localTagRow.Name}[{i}]",
                        Left = new Thickness(localTagRow.Left.Left + 10, 0, 0, 0)
                    };
                    child.NoActionChange = true;
                    child.ExternalAccess = localTagRow.ExternalAccess;

                    child.SortMemberIndex = localTagRow.SortMemberIndex;
                    child.SortDimIndex = i;
                    child.Style = localTagRow.Style;

                    child.DefaultEnabled = !(dataType is CompositiveType);
                    child.NameEnabled = false;
                    child.DataTypeEnabled = false;
                    child.StyleEnabled = false;
                    child.CanSetDescription = false;
                    if (baseTagRow.DataTypeL is CompositiveType)
                    {
                        if (localTagRow.Field is IArrayField)
                        {
                            var boolArray = localTagRow.Field as BoolArrayField;
                            if (boolArray != null)
                            {
                                child.FieldIndex = i;
                                child.IsBitSel = true;
                                child.Field = boolArray;
                            }

                            var array = localTagRow.Field as ArrayField;
                            if (array != null)
                            {
                                child.Field = array.fields[i].Item1;
                            }
                        }

                        child.DataType = dataTypeString;
                    }
                    else
                    {
                        child.DataType = dataTypeString;
                        if (localTagRow.Field is IArrayField)
                        {
                            var boolArray = localTagRow.Field as BoolArrayField;
                            if (boolArray != null)
                            {
                                child.FieldIndex = i;
                                child.IsBitSel = true;
                                child.Field = boolArray;
                            }

                            var arrayField = localTagRow.Field as ArrayField;
                            if (arrayField != null)
                            {
                                child.Field = arrayField.fields[i].Item1;
                            }

                            string value;
                            if (boolArray != null)
                            {
                                value = boolArray.Get(i) ? "1" : "0";
                            }
                            else
                            {
                                value = GetIntegerValue(child.Field);
                            }

                            value = ValueConverter.ConvertValue(value, DisplayStyle.Decimal,
                                (DisplayStyle)localTagRow.Style, child.DataTypeL.BitSize,
                                ValueConverter.SelectIntType(child.DataTypeL));
                            child.Default = value;
                            //child.Default = FormatOp.FormatValue(value, (DisplayStyle)localTagRow.Style, dataType);
                        }
                        else
                        {
                            var value = "0";
                            value = ValueConverter.ConvertValue(value, DisplayStyle.Decimal,
                                (DisplayStyle)localTagRow.Style, child.DataTypeL.BitSize,
                                ValueConverter.SelectIntType(child.DataTypeL));
                            child.Default = value;
                            //child.Default = FormatOp.FormatValue(value, (DisplayStyle)localTagRow.Style, dataType);
                        }
                    }

                    if (dataType is BOOL) child.ButtonVisibility = Visibility.Collapsed;

                    child.CanSetDescription = true;
                    child.NoActionChange = false;
                    child.IsInCreate = false;

                    //localTagRow.Child.Add(child);
                    list.Add(child);
                    PropertyChangedEventManager.AddHandler(child, localTagRow.OnChildPropertyChanged, string.Empty);
                    //child.PropertyChanged += localTagRow.OnChildPropertyChanged;
                }

                localTagRow.Child.AddRange(localTagRow.Child.Count, list);
            }
            else if (dataType is CompositiveType)
            {
                localTagRow.ExpanderCloseVis = Visibility.Collapsed;
                localTagRow.ExpanderVis = Visibility.Visible;
                LocalTagRow parentTag = null;
                foreach (DataTypeMember itemMember in (dataType as CompositiveType).TypeMembers)
                {
                    if (itemMember.IsHidden) continue;
                    var index = itemMember.FieldIndex;
                    if (dataType is AOIDataType)
                    {
                        if (!(dataType as AOIDataType).IsMemberShowInOtherAoi(itemMember.Name)) continue;
                    }

                    LocalTagRow child = new LocalTagRow(ParentAddOnInstruction, localTagRow.Tag, localTagRow)
                    {
                        IsInCreate = true,
                        ChildDescription = ChildDescription, Name = $"{localTagRow.Name}.{itemMember.Name}",
                        IsUpdateDefault = false,
                        Left = new Thickness(localTagRow.Left.Left + 10, 0, 0, 0)
                    };
                    child.ExternalAccess = localTagRow.ExternalAccess;
                    child.SortMemberIndex = itemMember.FieldIndex;
                    child.Style = itemMember.DisplayStyle;
                    var array = localTagRow.Field.ToJToken() as JArray;
                    if (array != null && array.Count > 0)
                    {
                        if (itemMember.DataTypeInfo.DataType is BOOL && itemMember.DataTypeInfo.Dim1 > 0)
                        {
                            var data = array[index];
                            var jValue = data as JValue;
                            Debug.Assert(jValue != null);
                            child.Field = localTagRow.Field;
                            child.FieldIndex = itemMember.BitOffset;
                            child.IsBitSel = true;
                            child.DataType = itemMember.DataTypeInfo.Dim1 > 0
                                ? $"{FormatOp.ConvertMemberName(itemMember.DataTypeInfo.DataType.Name)}[{itemMember.DataTypeInfo.Dim1}]"
                                : FormatOp.ConvertMemberName(itemMember.DataTypeInfo.DataType.Name);
                        }
                        else if (localTagRow.Field?.ToJToken() != null &&
                                 array.Count >= index)
                        {
                            child.Field = (localTagRow.Field as ICompositeField).fields[index].Item1;
                            child.DataType = itemMember.DataTypeInfo.Dim1 > 0
                                ? $"{FormatOp.ConvertMemberName(itemMember.DataTypeInfo.DataType.Name)}[{itemMember.DataTypeInfo.Dim1}]"
                                : FormatOp.ConvertMemberName(itemMember.DataTypeInfo.DataType.Name);
                            if (itemMember.DataTypeInfo.DataType is BOOL)
                            {
                                child.Default = child.Field.GetBitValue(itemMember.BitOffset) ? "1" : "0";
                                child.IsBitSel = true;
                            }
                            else
                            {
                                if (itemMember.DataType.FamilyType != FamilyType.StringFamily)
                                    child.Default = array[index] is JValue
                                        ? array[index]?.ToString()
                                        : "";
                            }

                        }
                        else
                        {
                            child.DataType = itemMember.DataTypeInfo.Dim1 > 0
                                ? $"{FormatOp.ConvertMemberName(itemMember.DataTypeInfo.DataType.Name)}[{itemMember.DataTypeInfo.Dim1}]"
                                : FormatOp.ConvertMemberName(itemMember.DataTypeInfo.DataType.Name);
                            if (itemMember.DataType.FamilyType != FamilyType.StringFamily)
                                child.Default = (Style == DisplayStyle.Ascii ? "\0" : "0");
                        }
                    }
                    else
                    {
                        child.DataType = itemMember.DataTypeInfo.Dim1 > 0
                            ? $"{FormatOp.ConvertMemberName(itemMember.DataTypeInfo.DataType.Name)}[{itemMember.DataTypeInfo.Dim1}]"
                            : FormatOp.ConvertMemberName(itemMember.DataTypeInfo.DataType.Name);
                        child._default =
                            string.IsNullOrEmpty((localTagRow.Field.ToJToken() as JValue)?.Value?.ToString())
                                ? (child.Style == DisplayStyle.Ascii ? "\0" : "0")
                                : ((JValue)localTagRow.Field.ToJToken())?.Value?.ToString();
                    }

                    if (itemMember.DataType.IsInteger)
                        parentTag = child;
                    if (parentTag != null && itemMember.BitOffset != 0)
                    {
                        child.IsBitSel = true;
                        child.ParentTagRow = parentTag;
                        child.BitOffset = itemMember.BitOffset;
                    }

                    child.CanSetDescription = false;
                    child.Description = GetChildDescription(child.Name);
                    child.CanSetDescription = true;
                    child.NameEnabled = false;
                    child.DataTypeEnabled = false;
                    child.DefaultEnabled = !(itemMember.DataTypeInfo.DataType is CompositiveType) &&
                                           itemMember.DataTypeInfo.Dim1 == 0 && itemMember.DataTypeInfo.Dim2 == 0 &&
                                           itemMember.DataTypeInfo.Dim3 == 0;
                    child.StyleEnabled = false;
                    child.IsInCreate = false;
                    localTagRow.Child.Add(child);
                    PropertyChangedEventManager.AddHandler(child, localTagRow.OnChildPropertyChanged, String.Empty);
                    //child.PropertyChanged += localTagRow.OnChildPropertyChanged;
                }

            }
            else
            {
                if (dataType is BOOL | dataType is REAL)
                {
                    IsInCreate = false;
                    NoActionChange = false;
                    return;
                }

                if (dim > 0)
                {
                    localTagRow.ExpanderCloseVis = Visibility.Collapsed;
                    localTagRow.ExpanderVis = Visibility.Visible;
                }
                else
                {
                    localTagRow.ExpanderCloseVis = Visibility.Collapsed;
                    localTagRow.ExpanderVis = Visibility.Visible;
                }

                for (int i = 0; i < dataType.BitSize; i++)
                {
                    LocalTagRow child = new LocalTagRow(ParentAddOnInstruction, localTagRow.Tag, localTagRow)
                    {
                        IsInCreate = true,
                        ChildDescription = ChildDescription,
                        Name = $"{localTagRow.Name}.{i}",
                        Style = DisplayStyle.Decimal,
                        DataType = "BOOL",
                        Default = localTagRow.B[i].ToString(),
                        NameEnabled = false,
                        IsUpdateDefault = false,
                        IsBitSel = true,
                        Field = localTagRow.Field,
                        FieldIndex = i,
                        Left = new Thickness(localTagRow.Left.Left + 10, 0, 0, 0)
                    };
                    child.ExternalAccess = localTagRow.ExternalAccess;
                    child.SortBitOffset = i;
                    child.CanSetDescription = false;
                    child.Description = GetChildDescription(child.Name);
                    child.CanSetDescription = true;
                    child.StyleEnabled = false;
                    child.ButtonVisibility = Visibility.Collapsed;
                    child.IsInCreate = false;
                    PropertyChangedEventManager.AddHandler(child, localTagRow.OnChildPropertyChanged, string.Empty);
                    //child.PropertyChanged += localTagRow.OnChildPropertyChanged;
                    localTagRow.Child.Add(child);
                }
            }

            IsInCreate = false;
            NoActionChange = false;
        }
    }
}