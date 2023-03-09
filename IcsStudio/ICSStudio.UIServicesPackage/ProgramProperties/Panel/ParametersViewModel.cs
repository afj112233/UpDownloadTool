using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Dialogs.Filter;
using ICSStudio.Dialogs.SelectTag;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Gui.View;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Notification;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json.Linq;
using Type = ICSStudio.UIInterfaces.Editor.Type;

namespace ICSStudio.UIServicesPackage.ProgramProperties.Panel
{
    public enum Operation
    {
        Create,
        Edit,
        Delete
    }

    partial class ParametersViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private bool _isDirty;

        private readonly IProgram _program;
        private ParameterRow _selectedRow;
        private readonly FilterViewModel _filterViewModel;

        public ParametersViewModel(Parameters panel, IProgram program)
        {
            Control = panel;
            panel.DataContext = this;
            _program = program;
            _filterViewModel = new FilterViewModel(program.ParentController, program, true, true, "");
            if (program.Type == ProgramType.Sequence || program.Type == ProgramType.Phase)
            {
                SequencingVisibility = Visibility.Visible;
            }
            else
            {
                SequencingVisibility = Visibility.Collapsed;
            }

            HyperlinkCommand = new RelayCommand(ExecuteHyperlinkCommand);
            Hyperlink2Command = new RelayCommand<ParameterRow>(ExecuteHyperlink2Command);
            HomeHyperlinkCommand = new RelayCommand(ExecuteHomeHyperlinkCommand);
            DataTypeCommand = new RelayCommand(ExecuteDataTypeCommand);
            LoadDataGridCommand = new RelayCommand<DataGrid>(ExecuteLoadDataGridCommand);
            DataGridRightMouseCommand = new RelayCommand<MouseButtonEventArgs>(ExecuteDataGridRightMouseCommand);
            TextBoxLostFocusCommand = new RelayCommand<RoutedEventArgs>(ExecuteTextBoxLostFocusCommand);
            DataGrid2KeyUpCommand = new RelayCommand<KeyEventArgs>(ExecuteDataGrid2KeyUpCommand);
            SelectTagCommand = new RelayCommand(ExecuteSelectTagCommand);
            DeleteParameterCommand = new RelayCommand(ExecuteDeleteParameterCommand);
            DeleteConnectionCommand = new RelayCommand(ExecuteDeleteConnectionCommand);
            AutoComplete = new List<string>();
            foreach (var item in program.ParentController.DataTypes)
            {
                if (item.Name.StartsWith("BOOL:", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (item.Name.Contains("$"))
                    continue;
                AutoComplete.Add(item.Name);
            }

            AutoComplete.Sort((x, y) => string.Compare(x, y, StringComparison.OrdinalIgnoreCase));
            List<Usage> usages = new List<Usage>() { Usage.Input, Usage.Output, Usage.InOut, Usage.SharedData };
            Usages = usages.Select(x =>
            {
                var attribute =
                    Attribute.GetCustomAttribute(x.GetType().GetField(x.ToString()), typeof(EnumMemberAttribute)) as
                        EnumMemberAttribute;
                string displayName = attribute?.Value;
                if (displayName == "SharedData")
                    displayName = "Public";
                return new { Value = x, DisplayName = displayName };
            }).ToList();
            List<ExternalAccess> externalAccesses = new List<ExternalAccess>()
                { ExternalAccess.ReadWrite, ExternalAccess.ReadOnly, ExternalAccess.None };
            ExternalAccesses = externalAccesses.Select(x => { return new { Value = x, DisplayName = x.ToString() }; })
                .ToList();
            SetRows(program.Tags);
            if (!ParameterRows.Any())
                BlankFactor();
            AddListen();
            IsDirty = false;

            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                Controller.GetInstance(), "IsOnlineChanged", OnIsOnlineChanged);
        }

        private void AddListen()
        {
            CollectionChangedEventManager.AddHandler(ParameterRows, ParameterRows_CollectionChanged);
            ConnectionRows.CollectionChanged += ConnectionRows_CollectionChanged;
            CollectionChangedEventManager.AddHandler(_program.ParentController.ParameterConnections,
                ParameterConnections_CollectionChanged);
            MonitorUsage();
        }

        private void RemoveListen()
        {
            CollectionChangedEventManager.RemoveHandler(ParameterRows, ParameterRows_CollectionChanged);
            ConnectionRows.CollectionChanged -= ConnectionRows_CollectionChanged;
            CollectionChangedEventManager.RemoveHandler(_program.ParentController.ParameterConnections,
                ParameterConnections_CollectionChanged);
            OffMonitorUsage();
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                RaisePropertyChanged(nameof(IsConnectionsEnabled));
                IsReadOnly = Controller.GetInstance().IsOnline;
                RaisePropertyChanged(nameof(IsReadOnly));
            });
        }

        private bool _isUpdateDirty = true;

        private void ParameterConnections_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _isUpdateDirty = false;
            //添加或删除对应conn
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ParameterConnection item in e.NewItems)
                {
                    if (item.SourcePath.StartsWith($"\\{_program.Name}"))
                    {
                        if (Enumerable.Any(TempParameterRows))
                        {
                            var parameterRow = TempParameterRows.FirstOrDefault(t =>
                                item.SourcePath.StartsWith(t.GetFullName(), StringComparison.OrdinalIgnoreCase));
                            var child = parameterRow?.GetChild(item.SourcePath);
                            child?.AddConnection(item);
                        }
                        else
                        {
                            var parameterRow = ParameterRows.FirstOrDefault(t =>
                                item.SourcePath.StartsWith(t.GetFullName(), StringComparison.OrdinalIgnoreCase));
                            parameterRow?.AddConnection(item);
                        }

                        if (SelectedRow != null && SelectedRow.GetFullName()
                                .Equals(item.SourcePath, StringComparison.OrdinalIgnoreCase))
                            SetConnectionRows(SelectedRow);
                        break;
                    }

                    if (item.DestinationPath.StartsWith($"\\{_program.Name}"))
                    {
                        if (Enumerable.Any(TempParameterRows))
                        {
                            var parameterRow = TempParameterRows.FirstOrDefault(t =>
                                item.DestinationPath.StartsWith(t.GetFullName(), StringComparison.OrdinalIgnoreCase));
                            parameterRow?.AddConnection(item);
                        }
                        else
                        {
                            var parameterRow = ParameterRows.FirstOrDefault(t =>
                                item.DestinationPath.StartsWith(t.GetFullName(), StringComparison.OrdinalIgnoreCase));
                            parameterRow?.AddConnection(item);
                        }

                        if (SelectedRow != null && SelectedRow.GetFullName()
                                .Equals(item.DestinationPath, StringComparison.OrdinalIgnoreCase))
                            SetConnectionRows(SelectedRow);
                        break;
                    }
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ParameterConnection item in e.OldItems)
                {
                    if (item.SourcePath.StartsWith($"\\{_program.Name}"))
                    {
                        if (Enumerable.Any(TempParameterRows))
                        {
                            var parameterRow = TempParameterRows.FirstOrDefault(t =>
                                item.SourcePath.StartsWith(t.GetFullName(), StringComparison.OrdinalIgnoreCase));
                            parameterRow?.DeleteConnection(item);
                        }
                        else
                        {
                            var parameterRow = ParameterRows.FirstOrDefault(t =>
                                item.SourcePath.StartsWith(t.GetFullName(), StringComparison.OrdinalIgnoreCase));
                            parameterRow?.DeleteConnection(item);
                        }

                        if (SelectedRow != null && SelectedRow.GetFullName()
                                .Equals(item.SourcePath, StringComparison.OrdinalIgnoreCase))
                            SetConnectionRows(SelectedRow);
                        break;
                    }

                    if (item.DestinationPath.StartsWith($"\\{_program.Name}"))
                    {
                        if (Enumerable.Any(TempParameterRows))
                        {
                            var parameterRow = TempParameterRows.FirstOrDefault(t =>
                                item.DestinationPath.StartsWith(t.GetFullName(), StringComparison.OrdinalIgnoreCase));
                            parameterRow?.DeleteConnection(item);
                        }
                        else
                        {
                            var parameterRow = ParameterRows.FirstOrDefault(t =>
                                item.DestinationPath.StartsWith(t.GetFullName(), StringComparison.OrdinalIgnoreCase));
                            parameterRow?.DeleteConnection(item);
                        }

                        if (SelectedRow != null && SelectedRow.GetFullName()
                                .Equals(item.DestinationPath, StringComparison.OrdinalIgnoreCase))
                            SetConnectionRows(SelectedRow);
                        break;
                    }
                }
            }

            _isUpdateDirty = true;
        }

        private void MonitorUsage()
        {
            _program.Tags.CollectionChanged += Tags_CollectionChanged;
            foreach (ITag tag in _program.Tags)
            {
                tag.PropertyChanged += Tag_PropertyChanged;
            }
        }

        private void Tag_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _isUpdateDirty = false;
            var tag = (ITag)sender;
            ParameterRow parameterRow = null;
            if (TempParameterRows.Any())
            {
                parameterRow = TempParameterRows.FirstOrDefault(p => p.Tag == tag);
            }
            else
            {
                parameterRow = ParameterRows.FirstOrDefault(p => p.Tag == tag);
            }

            if (e.PropertyName == "Usage")
            {

                if (tag.Usage == Usage.Local)
                {

                    if (SelectedRow == parameterRow)
                    {
                        SelectedRow = null;
                    }

                    ParameterRows.Remove(parameterRow);
                }
                else
                {
                    if (parameterRow != null)
                    {
                        var extendedEventArgs = e as PropertyChangedExtendedEventArgs<Usage>;
                        if (extendedEventArgs != null && extendedEventArgs.OldValue == parameterRow.Usage)
                        {
                            parameterRow.Usage = extendedEventArgs.NewValue;
                        }

                        parameterRow.CheckConnection();
                    }
                    else
                    {
                        TagToParameterRow(tag, true);
                    }
                }
            }

            if (parameterRow == null) return;
            if (e.PropertyName == "Name")
            {
                var extendedEventArgs = e as PropertyChangedExtendedEventArgs<string>;
                if (extendedEventArgs.OldValue.Equals(parameterRow.Name, StringComparison.OrdinalIgnoreCase))
                {
                    parameterRow.Name = extendedEventArgs.NewValue;
                }
            }

            if (e.PropertyName == "Description")
            {
                parameterRow?.UpdateDescription();
            }

            if (e.PropertyName == "DataWrapper")
            {
                var extendedEventArgs = e as PropertyChangedExtendedEventArgs<DataWrapper>;
                if (extendedEventArgs?.OldValue.DataTypeInfo.ToString().Equals(parameterRow?.DataType) ?? false)
                {
                    parameterRow.DataType = extendedEventArgs.NewValue.DataTypeInfo.ToString();
                    ExecuteHomeHyperlinkCommand();
                }
            }

            if (e.PropertyName == "IsConstant")
            {
                var extendedEventArgs = e as PropertyChangedExtendedEventArgs<bool>;
                if (!(parameterRow.IsConstant ^ extendedEventArgs.OldValue))
                {
                    parameterRow.IsConstant = extendedEventArgs.NewValue;
                }
            }

            _isUpdateDirty = true;
        }

        private void Tags_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _isUpdateDirty = false;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ITag tag in e.NewItems)
                {
                    if (tag.Usage != Usage.Local)
                    {
                        TagToParameterRow(tag, true);

                    }
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ITag tag in e.OldItems)
                {
                    if (tag.Usage != Usage.Local)
                    {
                        var parameterRow = ParameterRows.FirstOrDefault(p => p.Tag == tag);
                        RemoveParameter(parameterRow);
                    }
                }
            }

            _isUpdateDirty = true;
        }

        private void RemoveParameter(ParameterRow parameterRow)
        {
            if (SelectedRow == parameterRow)
            {
                SelectedRow = null;
            }

            foreach (var connectionInfo in parameterRow.ConnectionRows.ToList())
            {
                parameterRow.DeleteConnection(connectionInfo, true);
            }

            parameterRow.Tag.PropertyChanged -= Tag_PropertyChanged;
            if (Enumerable.Any(TempParameterRows))
            {
                TempParameterRows.Remove(parameterRow);
            }
            else
            {
                ParameterRows.Remove(parameterRow);
            }

            parameterRow.Clean();
            Scope.CheckParameterConnection();
        }

        private void OffMonitorUsage()
        {
            _program.Tags.CollectionChanged -= Tags_CollectionChanged;
            foreach (ITag tag in _program.Tags)
            {
                tag.PropertyChanged -= Tag_PropertyChanged;
            }
        }

        private void ConnectionRows_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                //var info = (ConnectionInfo) e.NewItems[0];
                //if (string.IsNullOrEmpty(info.Name)) return;
                //if (info.Parent.Usage == Usage.Input || info.Parent.Usage == Usage.InOut)
                //{

                //}
                //else
                //{
                //    ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
                //    {
                //        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                //        // You're now on the UI thread.
                //        var blank = new ConnectionInfo(Scope,info.Parent);
                //        ConnectionRows.Add(blank);
                //    });
                //}
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var info = (ConnectionInfo)e.OldItems[0];
                if (Enumerable.Any(ConnectionRows))
                {

                }
                else
                {
                    ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        // You're now on the UI thread.
                        var blank = new ConnectionInfo(Scope, info.Parent);
                        ConnectionRows.Add(blank);
                    });
                }
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                Controller.GetInstance(), "IsOnlineChanged", OnIsOnlineChanged);

            foreach (var parameterRow in Scope.GetParameterRows())
            {
                parameterRow.Clean();
            }

            RemoveListen();
        }

        private readonly List<ParameterRow> _delList = new List<ParameterRow>();

        private void ParameterRows_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {

            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ParameterRow item in e.OldItems)
                {
                    if (!item.IsNew && !item.IsMember)
                        _delList.Add(item);
                    item.PropertyChanged -= OnPropertyChanged;
                }
            }
        }

        public void Save()
        {
            RemoveListen();
            Controller.GetInstance().CanVerify = false;
            var collection = (TagCollection)_program.Tags;
            collection.IsSaveTagNameChange = true;
            if (TempParameterRows.Count > 0)
            {
                AddParameterToTags(TempParameterRows);
            }
            else
            {
                AddParameterToTags(ParameterRows);
            }

            collection.ApplyTagNameChanged();
            Controller.GetInstance().CanVerify = true;
            IsDirty = false;
            AddListen();
            Notifications.Publish(new MessageData() { Type = MessageData.MessageType.Verify });
        }

        public void AddParameterToTags(IList rows)
        {
            var collection = _program.Tags as TagCollection;
            var controller = _program.ParentController;
            foreach (var parameter in _delList)
            {
                collection?.DeleteTag(parameter.Tag, true, true, false);
                controller.ParameterConnections.RemoveConnection(parameter.Tag);
                parameter.RemoveListen();
            }

            _delList.Clear();

            foreach (ParameterRow row in rows)
            {
                row.RemoveListen();
            }

            for (int i = rows.Count - 1; i >= 0; i--)
            {
                var r = (ParameterRow)rows[i];
                if (r.IsMember) continue;
                if (r.Name == null && r.DataType == null) continue;

                r.ApplyChanges();

                if (r.IsNew)
                {
                    (collection)?.AddTag(r.Tag, false, false);
                    r.IsNew = false;
                }

            }

            foreach (ParameterRow r in rows)
            {
                if (r.IsMember) continue;
                if (r.Name == null && r.DataType == null) continue;
                foreach (var deleteParameterConnection in r.DeleteParameterConnections)
                {
                    controller.ParameterConnections.Remove(deleteParameterConnection);
                }

                foreach (var connectionInfo in r.ParameterConnectionList)
                {
                    if (connectionInfo.ParameterConnection != null)
                    {
                        if (connectionInfo.IsDirty)
                        {
                            connectionInfo.ParameterConnection.ApplyChanged(connectionInfo.Parent.GetFullName(),
                                connectionInfo.Parent.Usage, connectionInfo.Name, connectionInfo.Usage);
                            connectionInfo.AdviseConnectionParameter();
                        }
                    }
                    else
                    {
                        var connection = controller.ParameterConnections.CreateConnection(
                            connectionInfo.Parent.GetFullName(), connectionInfo.Parent.Usage, connectionInfo.Name,
                            connectionInfo.Usage);
                        connectionInfo.ParameterConnection = (ParameterConnection)connection;
                        if (connectionInfo.ReferenceConnectionInfo != null)
                            connectionInfo.ReferenceConnectionInfo.ParameterConnection =
                                connectionInfo.ParameterConnection;
                    }
                }

                //r.SetParameterConnectionList();
                r.CheckConnection();
            }

            foreach (ParameterRow row in rows)
            {
                row.AddListen();
            }
        }

        public bool CheckAllParameters()
        {
            IList pList;
            if (TempParameterRows.Count > 0)
            {
                pList = TempParameterRows;
            }
            else
            {
                pList = ParameterRows;
            }

            foreach (ParameterRow r in pList)
            {
                if (r.Name == null && r.DataType == null) continue;
                string error = r.Name == "" ? LanguageManager.GetInstance().ConvertSpecifier("Invalid name.") : CheckParameter(r);
                if (error != null)
                {
                    r.ErrorVisibility = Visibility.Visible;
                    MessageBox.Show(error, "ICS Studio");
                    return false;
                }
            }

            return true;
        }

        public string CheckParameter(ParameterRow r)
        {
            try
            {
                Regex regex = new Regex(@"^([A-Za-z]([0-9]+)?)+((_[a-zA-Z0-9]+)*|[a-zA-Z0-9]*)$");
                if (r.Name != null && !regex.IsMatch(r.Name))
                {
                    r.ErrorVisibility = Visibility.Visible;
                    //return $"Invalid name \"{r.Name}\".";
                    return LanguageManager.GetInstance().ConvertSpecifier("Invalid name.");
                }

                if (TempParameterRows.Count > 0)
                {
                    bool flag = false;
                    foreach (var row in TempParameterRows)
                    {
                        if (row.Name == null) continue;
                        if (row.Name.Equals(r.Name) && row != r)
                        {
                            row.ErrorVisibility = Visibility.Visible;
                            r.ErrorVisibility = Visibility.Visible;
                            flag = true;
                        }
                    }

                    if (flag) return LanguageManager.GetInstance().ConvertSpecifier("A Tag by the same name already exists in this collection.");
                }
                else
                {
                    bool flag = false;
                    foreach (var row in ParameterRows)
                    {
                        if (row.Name == null) continue;
                        if (row.Name.Equals(r.Name) && row != r)
                        {
                            row.ErrorVisibility = Visibility.Visible;
                            r.ErrorVisibility = Visibility.Visible;
                            flag = true;
                        }
                    }

                    if (flag) return LanguageManager.GetInstance().ConvertSpecifier("A Tag by the same name already exists in this collection.");
                }

                if (string.IsNullOrEmpty(r.DataType))
                {
                    r.ErrorVisibility = Visibility.Visible;
                    //return $"The Tag \"{r.Name}\" invalid data type.";
                    return LanguageManager.GetInstance().ConvertSpecifier("Invalid data type.");
                }

                DataTypeInfo dataTypeInfo = Controller.GetInstance().DataTypes.ParseDataTypeInfo(r.DataType);
                if (dataTypeInfo.DataType == null)
                {
                    r.ErrorVisibility = Visibility.Visible;
                    //return $"The Tag \"{r.Name}\" invalid data type.";
                    return LanguageManager.GetInstance().ConvertSpecifier("Invalid data type.");
                }

                if (dataTypeInfo.DataType.IsMessageType && (r.Usage != Usage.InOut))
                {
                    r.ErrorVisibility = Visibility.Visible;
                    return
                        LanguageManager.GetInstance().ConvertSpecifier("Tag can only be created at the Controller scope or as Local Tag" +
                                                                       " / Inout Parameter at Program scope.");
                }

                r.ErrorVisibility = r.ConnectionRows.Any(c => c.ErrorVisibility == Visibility.Visible)
                    ? Visibility.Visible
                    : Visibility.Collapsed;

                return null;
            }
            catch (Exception e)
            {
                Debug.Assert(false, e.Message);
                return null;
            }
        }

        private List<ITag> _originalList = new List<ITag>();

        public void SetRows(ITagCollection collection)
        {
            ParameterRows.Clear();
            foreach (var t in collection)
            {
                TagToParameterRow(t, false);
            }

            foreach (var parameterRow in ParameterRows.ToList())
            {
                parameterRow?.SetParameterConnectionList();
                parameterRow?.VerityConnections();
            }
        }

        private void TagToParameterRow(ITag t, bool needSetConn)
        {
            if (!t.IsProgramParameter) return;
            ParameterRow row = new ParameterRow(Scope, t, null, null, (JArray)((Tag)t).ChildDescription.DeepClone(),
                false);
            _originalList.Add(t);
            if (t.DataTypeInfo.Dim1 > 0 || !(t.DataTypeInfo.DataType is BOOL || t.DataTypeInfo.DataType is REAL))
            {
                row.ExpanderVisibility = Visibility.Visible;
            }
            else
            {
                row.ExpanderVisibility = Visibility.Collapsed;
            }

            row.PenVisibility = Visibility.Collapsed;
            row.Name = t.Name;
            row.Usage = t.Usage;
            row.DataType = t.DataTypeInfo.ToString();
            row.Sequencing = t.IsSequencing;
            row.BaseTag = t.BaseTag?.Name;
            row.ExternalAccess = t.ExternalAccess;
            row.IsConstant = t.IsConstant;
            if (needSetConn)
                row.SetParameterConnectionList();
            row.PropertyChanged += OnPropertyChanged;
            if (Enumerable.Any(ParameterRows) && string.IsNullOrEmpty(ParameterRows.Last().Name))
            {
                ParameterRows.Insert(ParameterRows.Count - 1, row);
            }
            else
            {
                ParameterRows.Add(row);
            }
        }

        public ConnectionInfo SelectedConnection { set; get; }

        public bool IsConnectionsEnabled => !Controller.GetInstance().IsOnline;

        #region Command

        public RelayCommand HyperlinkCommand { set; get; }

        public void ExecuteHyperlinkCommand()
        {
            if (SecondLinkList.Count > 0)
            {
                LinkList.Add(SecondLinkList[0]);
                SecondLinkList.RemoveAt(0);
                SecondLinkList.Add(SelectedRow);
            }
            else
            {
                SecondLinkList.Add(SelectedRow);
            }

            ExpandChild(SelectedRow);
        }

        public RelayCommand<DataGrid> LoadDataGridCommand { set; get; }

        private void ExecuteLoadDataGridCommand(DataGrid dg)
        {
            if (dg.Name.Contains("2"))
            {
                _toggleMenuItem2 = new MenuItem() { Header = "Toggle Column" };
                foreach (var col in dg.Columns)
                {
                    if (col.Header == null) continue;
                    if ((string)col.Header == "Name")
                        _toggleMenuItem2.Items.Add(new MenuItem()
                        { Header = col.Header, IsChecked = true, IsEnabled = false, IsCheckable = true });
                    else if ("Sequencing".Equals((string)col.Header))
                    {
                        _toggleMenuItem2.Items.Add(new MenuItem()
                        { Header = col.Header, IsChecked = false, IsEnabled = false, IsCheckable = false });
                    }
                    else
                    {
                        MenuItem mi = new MenuItem()
                        { Header = col.Header, IsChecked = true, IsCheckable = true };
                        mi.Checked += MenuItemChecked;
                        mi.Unchecked += MenuItemUnchecked;
                        _toggleMenuItem2.Items.Add(mi);
                    }
                }

                _toggleMenuItem2.CommandParameter = dg;
            }
            else
            {
                _toggleMenuItem1 = new MenuItem() { Header = "Toggle Column" };
                foreach (var col in dg.Columns)
                {
                    if (col.Header == null) continue;
                    if ((string)col.Header == "Name")
                        _toggleMenuItem1.Items.Add(new MenuItem()
                        { Header = col.Header, IsChecked = true, IsEnabled = false, IsCheckable = true });
                    else
                    {
                        MenuItem mi = new MenuItem()
                        { Header = col.Header, IsChecked = true, IsCheckable = true };
                        mi.Checked += MenuItemChecked;
                        mi.Unchecked += MenuItemUnchecked;
                        _toggleMenuItem1.Items.Add(mi);
                    }
                }

                _toggleMenuItem1.CommandParameter = dg;
            }
        }

        private MenuItem _toggleMenuItem1 = null;
        private MenuItem _toggleMenuItem2 = null;
        private ContextMenu _cm = null;
        public RelayCommand<MouseButtonEventArgs> DataGridRightMouseCommand { set; get; }

        private void ExecuteDataGridRightMouseCommand(MouseButtonEventArgs e)
        {
            DependencyObject dependencyObject = (DependencyObject)e.OriginalSource;
            if (_cm == null)
                _cm = new ContextMenu();
            else
                _cm.Items.Clear();
            var headerCol = VisualTreeHelpers.FindVisualParentOfType<DataGridColumnHeader>(dependencyObject);
            if (headerCol != null)
            {
                DataGridColumnHeader header = headerCol;
                if (header?.Content == null) return;
                MenuItem mi;
                mi = new MenuItem() { Header = "Sort Column" };
                mi.CommandParameter = header;
                mi.Click += SortClick;
                _cm.Items.Add(mi);

                MenuItem mi2;
                if (header.Content.ToString() == "Name")
                    mi2 = new MenuItem() { Header = "Hide Column", IsEnabled = false };
                else
                    mi2 = new MenuItem() { Header = "Hide Column" };
                mi2.CommandParameter = header;
                mi2.Click += HeaderClick;
                _cm.Items.Add(mi2);
                var dataGrid = VisualTreeHelpers.FindVisualParentOfType<DataGrid>(headerCol);
                if (dataGrid.Name.Contains("2"))
                {
                    _cm.Items.Add(_toggleMenuItem2);
                }
                else
                {
                    _cm.Items.Add(_toggleMenuItem1);
                }

                header.ContextMenu = _cm;
                return;
            }

            var headRow = VisualTreeHelpers.FindVisualParentOfType<DataGridRowHeader>(dependencyObject);
            if (headRow != null)
            {
                DataGridRowHeader header = headRow;
                var row = headRow.DataContext as ConnectionInfo;
                if (row != null)
                {
                    Debug.Assert(row != null);
                    if (!string.IsNullOrEmpty(row.DataType) || !string.IsNullOrEmpty(row.Name))
                    {
                        var gotoItem = new MenuItem()
                        {
                            Header = $"Go to Cross Reference for \"{row?.Name}\"",
                            IsEnabled = row.ErrorVisibility != Visibility.Visible
                        };
                        gotoItem.CommandParameter = row;
                        gotoItem.Click += GotoItem_Click;
                        ;
                        _cm.Items.Add(gotoItem);
                    }

                    MenuItem mi2;
                    mi2 = new MenuItem() { Header = "Delete", IsEnabled = row.IsCreate };
                    mi2.CommandParameter = row;
                    mi2.Click += Delete_Click;
                    ;
                    _cm.Items.Add(mi2);
                    header.ContextMenu = _cm;
                    return;
                }

                var row2 = headRow.DataContext as ParameterRow;
                if (row2 != null)
                {
                    Debug.Assert(row2 != null);
                    if (!string.IsNullOrEmpty(row2.DataType))
                    {
                        var gotoItem = new MenuItem()
                        {
                            Header = $"Go to Cross Reference for \"{row2?.Name}\"",
                            IsEnabled = row2.ErrorVisibility != Visibility.Visible
                        };
                        gotoItem.CommandParameter = row2;
                        gotoItem.Click += GotoItem_Click;
                        ;
                        _cm.Items.Add(gotoItem);
                    }

                    MenuItem mi2;
                    mi2 = new MenuItem()
                    { Header = "Delete", IsEnabled = !(string.IsNullOrEmpty(row2.Name) || row2.IsMember) };
                    mi2.CommandParameter = row2;
                    mi2.Click += Delete_Click;
                    _cm.Items.Add(mi2);
                    header.ContextMenu = _cm;
                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var connectionInfo = ((MenuItem)sender).CommandParameter as ConnectionInfo;
            if (connectionInfo != null)
            {
                connectionInfo.Parent.DeleteConnection(connectionInfo, true);
            }

            var parameterRow = ((MenuItem)sender).CommandParameter as ParameterRow;
            if (parameterRow != null)
            {
                RemoveParameter(parameterRow);
            }
        }

        private void GotoItem_Click(object sender, RoutedEventArgs e)
        {
            var name = (((MenuItem)sender).CommandParameter as ParameterRow)?.Name ??
                       ((((MenuItem)sender).CommandParameter as ConnectionInfo))?.Name;
            var createEditorService = Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
            createEditorService?.CreateCrossReference(Type.Tag, null, name);
        }


        public RelayCommand<RoutedEventArgs> TextBoxLostFocusCommand { set; get; }

        private void ExecuteTextBoxLostFocusCommand(RoutedEventArgs e)
        {
            DependencyObject dataGridCell =
                VisualTreeHelpers.FindVisualParentOfType<DataGridCell>(e.OriginalSource as DependencyObject);
            (dataGridCell as DataGridCell).IsEditing = false;
        }

        public RelayCommand<KeyEventArgs> DataGrid2KeyUpCommand { set; get; }

        private void ExecuteDataGrid2KeyUpCommand(KeyEventArgs e)
        {
            var dataGrid = e.Source as DataGrid;
            var selectedRow = dataGrid?.SelectedItem as ParameterRow;
            if (selectedRow?.Name != null && selectedRow.DataType != null)
            {
                if (e.Key == Key.Delete)
                {
                    RemoveParameter(selectedRow);
                    IsDirty = true;
                }
            }
        }

        public RelayCommand SelectTagCommand { get; }

        private void ExecuteSelectTagCommand()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            _filterViewModel.Name = SelectedConnection.Name;
            var unexpectedUsage = GetUnexpectedUsage();
            if (!string.IsNullOrEmpty(unexpectedUsage))
                _filterViewModel.SetUsage(GetUnexpectedUsage());
            var dialog = new SelectTagDialog(_filterViewModel);

            if (dialog.ShowDialog(uiShell) ?? false)
            {
                SelectedConnection.Name = dialog.Selection;
            }

            dialog.Clean();
            dialog.DataContext = null;
            _filterViewModel.ResetSelectedShowItem();
        }

        private string GetUnexpectedUsage()
        {
            var unexpectedUsages = Usage.Local.ToString();
            if (SelectedConnection.Parent.Usage == Usage.SharedData)
            {
                unexpectedUsages = $"{unexpectedUsages},{SelectedConnection.Parent.DisplayUsage},<controller>";
            }
            else
            {
                unexpectedUsages = $"{unexpectedUsages},{SelectedConnection.Parent.DisplayUsage}";
            }

            return unexpectedUsages;
        }

        public RelayCommand DeleteParameterCommand { get; }

        private void ExecuteDeleteParameterCommand()
        {
            if (SelectedRow != null && !string.IsNullOrEmpty(SelectedRow.DisplayUsage))
            {
                RemoveParameter(SelectedRow);
            }

            IsDirty = true;
        }

        public RelayCommand DeleteConnectionCommand { get; }

        private void ExecuteDeleteConnectionCommand()
        {
            if (SelectedConnection != null && (!string.IsNullOrEmpty(SelectedConnection.UsageDisplay) ||
                                               !string.IsNullOrEmpty(SelectedConnection.Name)))
            {
                SelectedRow.DeleteConnection(SelectedConnection, true);
                //ConnectionRows.Remove(SelectedConnection);
            }

            IsDirty = true;
        }

        public FilterViewModel FilterViewModel => _filterViewModel;

        private void HeaderClick(object sender, RoutedEventArgs e)
        {
            DataGridColumnHeader header = ((MenuItem)sender).CommandParameter as DataGridColumnHeader;
            if (header == null) return;
            ContextMenu obj = ((MenuItem)sender).Parent as ContextMenu;
            foreach (var item in ((MenuItem)obj.Items[2]).Items)
            {
                MenuItem mi = item as MenuItem;
                if (mi.Header.ToString() == header.Content.ToString())
                {
                    mi.IsChecked = !mi.IsChecked;
                    header.Visibility = mi.IsChecked ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        private void SortClick(object sender, RoutedEventArgs e)
        {
            DataGridColumnHeader header = ((MenuItem)sender).CommandParameter as DataGridColumnHeader;
            var dataGrid = VisualTreeHelpers.FindVisualParentOfType<DataGrid>(header);
            if (dataGrid == null) return;
            DataGridSort(header?.Name, dataGrid);
        }

        private void MenuItemChecked(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            DataGrid dg =
                ((MenuItem)(mi.Parent)).CommandParameter as DataGrid;

            foreach (var col in dg.Columns)
            {
                if (col.Header == null) continue;
                if (mi.Header.ToString() == col.Header.ToString())
                {
                    col.Visibility = Visibility.Visible;
                }
            }
        }

        private void MenuItemUnchecked(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            DataGrid dg =
                ((MenuItem)(mi.Parent)).CommandParameter as DataGrid;
            foreach (var col in dg.Columns)
            {
                if (col.Header == null) continue;
                if (mi.Header.ToString() == col.Header.ToString())
                {
                    col.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void DataGridSort(string columnName, DataGrid dataGrid)
        {
            var defaultView = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource) as ListCollectionView;
            if (defaultView != null)
            {
                if (defaultView.IsAddingNew)
                    defaultView.CommitNew();
                if (defaultView.IsEditingItem)
                    defaultView.CommitEdit();
                using (defaultView.DeferRefresh())
                {
                    if (dataGrid.Name.Contains("2"))
                    {
                        var compare = defaultView.CustomSort as ParameterCompare;
                        var type = GetColumnType(columnName);
                        if (compare != null && compare.ColumnType == type)
                        {
                            defaultView.CustomSort = new ParameterCompare(!compare.Descending, type);
                        }
                        else
                        {
                            defaultView.CustomSort = new ParameterCompare(false, type);
                        }
                    }
                    else
                    {
                        var compare = defaultView.CustomSort as ConnectionInfoCompare;
                        var type = GetColumnType(columnName);
                        if (compare != null && compare.ColumnType == type)
                        {
                            defaultView.CustomSort = new ConnectionInfoCompare(!compare.Descending, type);
                        }
                        else
                        {
                            defaultView.CustomSort = new ConnectionInfoCompare(false, type);
                        }
                    }
                }

            }
        }

        private ColumnType GetColumnType(string columnName)
        {
            if (columnName == "Name") return ColumnType.Name;
            if (columnName == "Usage") return ColumnType.Usage;
            if (columnName == "Data Type") return ColumnType.DataType;
            if (columnName == "Alias For") return ColumnType.AliasFor;
            if (columnName == "Base Tag") return ColumnType.BaseTag;
            if (columnName == "Description") return ColumnType.Description;
            if (columnName == "External Access") return ColumnType.ExternalAccess;
            if (columnName == "Constant") return ColumnType.Constant;
            if (columnName == "Connections") return ColumnType.Connections;
            return ColumnType.Name;
        }

        //private bool _descending1;
        //private bool _descending2;

        public RelayCommand<ParameterRow> Hyperlink2Command { set; get; }

        public void ExecuteHyperlink2Command(ParameterRow parameterRow)
        {
            int index = LinkList.IndexOf(parameterRow);
            SecondLinkList.RemoveAt(0);
            SecondLinkList.Add(parameterRow);
            for (int i = index; i < LinkList.Count; i++)
            {
                LinkList.RemoveAt(i);
            }

            ExpandChild(parameterRow);
        }

        public RelayCommand HomeHyperlinkCommand { set; get; }

        public void ExecuteHomeHyperlinkCommand()
        {
            if (TempParameterRows.Count < 2) return;
            IsReadOnly = false;
            SecondLinkList.Clear();
            LinkList.Clear();
            ParameterRows.Clear();
            foreach (var temp in TempParameterRows)
            {
                ParameterRows.Add(temp);
            }

            TempParameterRows.Clear();
            RaisePropertyChanged("IsReadOnly");
        }

        #endregion

        public void ExpandChild(ParameterRow parameterRow)
        {
            if (TempParameterRows.Count == 0)
                SaveToTemp();
            IsReadOnly = true;
            ParameterRows.Clear();
            if (!parameterRow.Children.Any())
                parameterRow.GetChildren();
            ParameterRows.AddRange(0, parameterRow.Children);
            RaisePropertyChanged("IsReadOnly");
        }

        public void SaveToTemp()
        {
            TempParameterRows.Clear();
            foreach (var row in ParameterRows)
            {
                TempParameterRows.Add(row);
            }
        }

        public bool IsReadOnly { set; get; } = Controller.GetInstance().IsOnline;

        public ObservableCollection<ConnectionInfo> ConnectionRows { set; get; } =
            new ObservableCollection<ConnectionInfo>();

        public ObservableCollection<ParameterRow> LinkList { set; get; } = new ObservableCollection<ParameterRow>();

        public ObservableCollection<ParameterRow> SecondLinkList { set; get; } =
            new ObservableCollection<ParameterRow>();

        public ParameterCollection ParameterRows => Scope.ParameterRows;

        public List<ParameterRow> TempParameterRows => Scope.TempParameterRows;

        public TempScope Scope { get; } = new TempScope();

        public ParameterRow SelectedRow
        {
            set
            {
                if (_selectedRow != null)
                    _selectedRow.PropertyChanged -= Value_PropertyChanged;
                Set(ref _selectedRow, value);
                if (_selectedRow != null)
                    _selectedRow.PropertyChanged += Value_PropertyChanged;
                SetConnectionRows(_selectedRow);
            }
            get { return _selectedRow; }
        }

        private void Value_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UpdateConnectionRows")
            {
                SetConnectionRows((ParameterRow)sender);
            }
        }

        private void SetConnectionRows(ParameterRow row)
        {
            ConnectionRows.CollectionChanged -= ConnectionRows_CollectionChanged;
            ConnectionRows.Clear();
            if (row != null)
            {
                if (Enumerable.Any(row.ConnectionRows))
                {
                    foreach (var connectionInfo in row.ConnectionRows)
                    {
                        //if (connectionInfo.ErrorVisibility == Visibility.Visible && string.IsNullOrEmpty(connectionInfo.DataType))
                        //    connectionInfo.GetInfo();
                        ConnectionRows.Add(connectionInfo);
                    }
                }
            }

            ConnectionRows.CollectionChanged += ConnectionRows_CollectionChanged;
        }

        public List<string> AutoComplete { get; }
        public Visibility SequencingVisibility { set; get; }
        public object Owner { get; set; }
        public object Control { get; }
        public IList Usages { get; set; }
        public IList ExternalAccesses { get; set; }
        public RelayCommand DataTypeCommand { set; get; }

        public void BlankFactor()
        {
            var newTag = new Tag((TagCollection)_program.Tags);
            newTag.Usage = Usage.Input;
            ParameterRow blankRow = new ParameterRow(Scope, newTag, null, null, new JArray(), false, true);
            blankRow.PropertyChanged += OnPropertyChanged;
            ParameterRows.Add(blankRow);
        }

        //private bool _isVerity = false;
        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UpdateConnectionRows")
            {
                return;
            }

            //if (e.PropertyName == "UpdateParameterStatus")
            //{
            //    if(!_isVerity)
            //    {
            //        _isVerity = true;
            //        foreach (var parameterRow in Scope.GetParameterRows())
            //        {
            //            parameterRow.VerityAllConnection();
            //        }

            //        _isVerity = false;
            //    }
            //    return;
            //}
            IsDirty = true;
            var parameter = (ParameterRow)sender;
            int index = ParameterRows.IndexOf(parameter);
            if (index == ParameterRows.Count - 1 && !string.IsNullOrEmpty(((ParameterRow)sender).Name))
            {
                BlankFactor();
            }

            if (e.PropertyName == "IsCreate")
            {
                if (!Enumerable.Any(parameter.ConnectionRows))
                {
                    var r = new ConnectionInfo(Scope, parameter);
                    parameter.ConnectionRows.Add(r);
                }

                ConnectionRows.Add(parameter.ConnectionRows[0]);
            }
        }

        public void ExecuteDataTypeCommand()
        {
            var dialog =
                new Dialogs.SelectDataType.SelectDataTypeDialog(
                    _program.ParentController,
                    SelectedRow.DataType,
                    true, true)
                {
                    Height = 350,
                    Width = 400,
                    Owner = Application.Current.MainWindow
                };

            var result = dialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                SelectedRow.DataType = dialog.DataType;
                RaisePropertyChanged("SelectedRow");
            }
        }

        public void LoadOptions()
        {

        }

        public bool SaveOptions()
        {
            return true;
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (!_isUpdateDirty) return;
                if (_isDirty != value)
                {
                    _isDirty = value;
                    IsDirtyChanged?.Invoke(this, EventArgs.Empty);
                }

                Set(ref _isDirty, value);
            }
        }

        public event EventHandler IsDirtyChanged;
    }
}
