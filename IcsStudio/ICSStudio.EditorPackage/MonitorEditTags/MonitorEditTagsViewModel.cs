using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.VisualStudio.Shell;
using GalaSoft.MvvmLight;
using ICSStudio.Dialogs.Filter.Configure;
using ICSStudio.Dialogs.GlobalSetting;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.EditorPackage.MonitorEditTags.Models;
using UserControl = System.Windows.Controls.UserControl;
using ICSStudio.MultiLanguage;

namespace ICSStudio.EditorPackage.MonitorEditTags
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal sealed class MonitorEditTagsViewModel : ViewModelBase, IEditorPane, ICanApply
    {
        private readonly IController _controller;
        private readonly IDataServer _dataServer;

        private ITagCollectionContainer _scope;
        private ComboBoxTreeCollection _scopeSource;
        private int _selectedIndex;
        private object _propertyGrid;
        private readonly GlobalSetting _globalSetting = GlobalSetting.GetInstance();
        public MonitorEditTagsViewModel(
            IController controller,
            ITagCollectionContainer tagCollectionContainer,
            UserControl userControl)
        {
            var programModule = tagCollectionContainer as ProgramModule;
            if (programModule != null)
            {
                programModule.IsInMonitor = true;
            }

            _scope = tagCollectionContainer;
            _controller = controller;
            _dataServer = _controller.CreateDataServer();

            ScopeSource = new ComboBoxTreeCollection(controller, tagCollectionContainer);

            MonitorTagsViewModel = new MonitorTagsViewModel(controller)
            {
                MonitorTagCollection = { DataServer = _dataServer }
            };
            MonitorTagsViewModel.MonitorTagCollection.Update(_scope);
            MonitorTagsViewModel.PropertyChanged += MonitorTagsViewModelOnPropertyChanged;

            EditTagsViewModel = new EditTagsViewModel(controller)
            {
                EditTagCollection = { DataServer = _dataServer }
            };
            EditTagsViewModel.EditTagCollection.Update(_scope);
            EditTagsViewModel.PropertyChanged += EditTagsViewModelOnPropertyChanged;

            Control = userControl;
            userControl.DataContext = this;

            Controller myController = _controller as Controller;
            if (myController != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    myController, "IsOnlineChanged", OnIsOnlineChanged);
                CollectionChangedEventManager.AddHandler(controller.Programs, OnPropertiesChanged);
                CollectionChangedEventManager.AddHandler(controller.AOIDefinitionCollection, OnPropertiesChanged);
            }

            _dataServer.StartMonitoring(true, false);

            _dataContextSource = new ObservableCollection<AoiDataReference>();
            
            UpdateDataContextSource();
            #region Filter
            
            SelectedFilterType = "All Variables";
            if (_globalSetting.MonitorTagSetting.FilterNameType == 0)
            {
                IsFilterOnName = true;
            }else if (_globalSetting.MonitorTagSetting.FilterNameType == 1)
            {
                IsFilterOnDescription = true;
            }
            else
            {
                IsFilterOnBoth = true;
            }
            #endregion

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
              LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            UpdateCaptionAction?.Invoke(Caption);
            UpdateProperties();
        }

        private void OnPropertiesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ScopeSource = new ComboBoxTreeCollection(_controller, _scope);
        }

        public ObservableCollection<string> FilterType => _globalSetting.MonitorTagSetting.FilterTypeHistory;

        public string SelectedFilterType
        {
            set
            {
                var tmp = _selectedFilterType;
                if(_selectedFilterType == value)return;
                Set(ref _selectedFilterType, value);

                if ("Configure...".Equals(value))
                {
#pragma warning disable VSTHRD001 // 避免旧线程切换 API
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
#pragma warning restore VSTHRD001 // 避免旧线程切换 API
                           new Action(() =>
                           {
                                //Thread.Sleep(TimeSpan.FromSeconds(1));
                                Configure(tmp);
                           }));
                }
                else
                {
                    FilterTag();
                }
            }
            get
            {
                return _selectedFilterType;
            }
        }

        private void Configure(string tmp)
        {
            var dialog = new DefineTagFilterDialog();
            var vm = new DefineTagFilterDialogViewModel(_scope is AoiDefinition, tmp.Split(','));
            dialog.DataContext = vm;
            dialog.Owner = Application.Current.MainWindow;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (dialog.ShowDialog() ?? false)
            {
                var filterInfo = vm.GetFilterInfo();
                if (!string.IsNullOrEmpty(filterInfo) && !FilterType.Contains(filterInfo))
                {
                    if (FilterType.Count >= 12)
                    {
                        FilterType.RemoveAt(11);
                    }
                    FilterType.Insert(2, filterInfo);
                }

                if (string.IsNullOrEmpty(filterInfo))
                {
#pragma warning disable VSTHRD001 // 避免旧线程切换 API
                    Application.Current.Dispatcher.InvokeAsync(() =>
#pragma warning restore VSTHRD001 // 避免旧线程切换 API
                    {
                        SelectedFilterType = "All Variables";
                        FilterTag();
                    }, DispatcherPriority.Background);
                }
                else
                {
#pragma warning disable VSTHRD001 // 避免旧线程切换 API
                    Application.Current.Dispatcher.InvokeAsync(() =>
#pragma warning restore VSTHRD001 // 避免旧线程切换 API
                    {
                        SelectedFilterType = filterInfo;
                        FilterTag();
                    }, DispatcherPriority.Background);

                }
            }
            else
            {
#pragma warning disable VSTHRD001 // 避免旧线程切换 API
                Application.Current.Dispatcher.InvokeAsync(() =>
#pragma warning restore VSTHRD001 // 避免旧线程切换 API
                {
                    SelectedFilterType = tmp;
                }, DispatcherPriority.Background);
            }
        }

        private FilterOnType _filterOnType;
        public void FilterTag()
        {
            MonitorTagsViewModel.MonitorTagCollection.SetFilterInfo(SelectedFilterType, FilterName, _filterOnType);
            EditTagsViewModel.EditTagCollection.SetFilterInfo(SelectedFilterType, FilterName, _filterOnType);
        }

        public string SelectedFilterName { set; get; }

        public ObservableCollection<string> FilterNameHistory => _globalSetting.MonitorTagSetting.FilterNameHistory;

        public bool IsFilterOnName
        {
            set
            {
                Set(ref _isFilterOnName , value);
                if (value)
                {
                    IsFilterOnDescription = false;
                    IsFilterOnBoth = false;
                    _filterOnType = FilterOnType.FilterOnName;
                    FilterTag();
                    _globalSetting.MonitorTagSetting.FilterNameType = 0;
                }
            }
            get { return _isFilterOnName; }
        }

        public bool IsFilterOnDescription
        {
            set
            {
                Set(ref _isFilterOnDescription , value);
                if (value)
                {
                    IsFilterOnName = false;
                    IsFilterOnBoth = false;
                    _filterOnType = FilterOnType.FilterOnDescription;
                    FilterTag();
                    _globalSetting.MonitorTagSetting.FilterNameType = 1;
                }
            }
            get { return _isFilterOnDescription; }
        }

        public bool IsFilterOnBoth
        {
            set
            {
                Set(ref _isFilterOnBoth , value);
                if (value)
                {
                    IsFilterOnName = false;
                    IsFilterOnDescription = false;
                    _filterOnType = FilterOnType.Both;
                    FilterTag();
                    _globalSetting.MonitorTagSetting.FilterNameType = 2;
                }
            }
            get { return _isFilterOnBoth; }
        }

        public string FilterName
        {
            set
            {
                Set(ref _filterName , value);
                FilterTag();
                if (!string.IsNullOrEmpty(value) && MonitorTagsViewModel.MonitorTagCollection.Any())
                {
                    if (!FilterNameHistory.Contains(value))
                    {
                        FilterNameHistory.Add(value);
                    }

                    if (FilterNameHistory.Count > 5)
                    {
                        FilterNameHistory.RemoveAt(0);
                    }

                    SelectedFilterName = value;
                }
            }
            get { return _filterName; }
        }

        public void UpdateProperties()
        {
            try
            {
                var selectItem = GetSelectedItem();
                PropertyGrid = selectItem?.Tag != null ? new PropertiesItem(selectItem, SelectedIndex) : null;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                EditTagsViewModel.UpdateUI();

                PropertiesItem changeProperty = PropertyGrid as PropertiesItem;
                if (changeProperty != null)
                {
                    changeProperty.UpdateReadOnly();
                    PropertyGrid = null;
                    PropertyGrid = changeProperty;
                }

            });
        }

        private void EditTagsViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedEditTagItem")
            {
                UpdateProperties();
            }
        }

        private void MonitorTagsViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedMonitorTagItem" || e.PropertyName == "MonitorTagCollection")
            {
                UpdateProperties();
            }
        }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                int oldSelectedIndex = _selectedIndex;

                Set(ref _selectedIndex, value);

                KeepDataContextSelectedInMonitor();

                RaisePropertyChanged("IsDataContextEnabled");

                UpdateProperties();

                KeepFocusName(oldSelectedIndex, _selectedIndex);
            }
        }

        public ComboBoxTreeCollection ScopeSource
        {
            get
            {
                return _scopeSource;
            }
            set
            {
                Set(ref _scopeSource, value);
            }
        }
        public ITagCollectionContainer Scope
        {
            get { return _scope; }
            set
            {
                var sc = _scope;
                if (value == _scope) return;
                _scope = value;
                if (_scope == null)
                {
#pragma warning disable VSTHRD001 // 避免旧线程切换 API
                    Application.Current.Dispatcher.InvokeAsync(() =>
#pragma warning restore VSTHRD001 // 避免旧线程切换 API
                    {
                        _scope = sc;
                        RaisePropertyChanged();
                    }, DispatcherPriority.Background);
                }
                if (value != null)
                {
                    //Set(ref _scope, value);
                    MonitorTagsViewModel.Update(_scope);
                    EditTagsViewModel.Update(_scope);

                    UpdateCaptionAction?.Invoke(Caption);

                    UpdateDataContextSource();

                    FilterTag();
                }
                else
                {
                    RaisePropertyChanged();
                }

                RaisePropertyChanged("DataContextVisibility");
            }
        }

        public Visibility DataContextVisibility
        {
            get
            {
                if (IsInAOI)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public bool IsDataContextEnabled
        {
            get
            {
                if (_selectedIndex == 0)
                    return true;

                return false;
            }
        }

        private readonly ObservableCollection<AoiDataReference> _dataContextSource;

        public ObservableCollection<AoiDataReference> DataContextSource => _dataContextSource;

        private AoiDataReference _currentDataContext;

        private AoiDataReference _lastDataContext;

        public AoiDataReference CurrentDataContext
        {
            get { return _currentDataContext; }
            set
            {
                if (_currentDataContext != value)
                {
                    Set(ref _currentDataContext, value);

                    MonitorTagsViewModel.UpdateDataContext(_currentDataContext);
                    EditTagsViewModel.EditTagCollection.UpdateDataContext(_currentDataContext);
                }
            }
        }

        public bool IsInAOI => Scope?.Tags?.ParentProgram is AoiDefinition;

        public MonitorTagsViewModel MonitorTagsViewModel { get; }
        public EditTagsViewModel EditTagsViewModel { get; }

        public string Caption
        {
            get
            {
                var controller = Scope as IController;
                if (controller != null)
                    return LanguageManager.GetInstance().ConvertSpecifier("Global Variables")
                           + $" - {controller.Name}";

                var program = Scope as IProgram;
                if (program != null) 
                    return LanguageManager.GetInstance().ConvertSpecifier("Program Parameters and Local Variables")
                           + $" - {program.Name}";

                var aoi = Scope as AoiDefinition;
                if (aoi != null)
                    return LanguageManager.GetInstance().ConvertSpecifier("User Define Function BLOCK Parameters and Local Variables") 
                           + $" - {aoi.Name}";
                return string.Empty;
            }
        }

        public UserControl Control { get; set; }
        public Action CloseAction { get; set; }
        public Action<string> UpdateCaptionAction { get; set; }

        public void SetFocusName(string focusName)
        {
            if (string.IsNullOrEmpty(focusName))
                return;

            if (SelectedIndex == 0)
            {
                var result = MonitorTagsViewModel.SetFocusName(focusName);
                if (!result)
                {
                    FilterName = string.Empty;
                    MonitorTagsViewModel.SetFocusName(focusName);
                }
            }
            else if (SelectedIndex == 1)
            {
                var result = EditTagsViewModel.SetFocusName(focusName);
                if (!result)
                {
                    FilterName = string.Empty;
                    EditTagsViewModel.SetFocusName(focusName);
                }
            }
        }

        public override void Cleanup()
        {
            _dataServer.StopMonitoring(true, true);

            if (MonitorTagsViewModel != null)
            {
                MonitorTagsViewModel.PropertyChanged -= MonitorTagsViewModelOnPropertyChanged;
            }

            if (EditTagsViewModel != null)
            {
                EditTagsViewModel.PropertyChanged -= EditTagsViewModelOnPropertyChanged;
            }

            Controller controller = _controller as Controller;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
                CollectionChangedEventManager.RemoveHandler(controller.Programs, OnPropertiesChanged);
                CollectionChangedEventManager.RemoveHandler(controller.AOIDefinitionCollection, OnPropertiesChanged);
            }

            MonitorTagsViewModel?.Cleanup();
            EditTagsViewModel?.Cleanup();
            base.Cleanup();
            var programModule = _scope as ProgramModule;
            if (programModule != null)
            {
                programModule.IsInMonitor = false;
            }
        }

        public object PropertyGrid
        {
            set { Set(ref _propertyGrid, value); }
            get { return _propertyGrid; }
        }

        public TagItem GetSelectedItem()
        {
            if (SelectedIndex == 0) //Monitor Tags
            {
                return MonitorTagsViewModel.SelectedMonitorTagItem;
            }

            if (SelectedIndex == 1) //Edit Tags
            {
                return EditTagsViewModel.SelectedEditTagItem;
            }

            return null;
        }

        public int Apply()
        {
            // cancel new or edit
            var defaultView =
                CollectionViewSource.GetDefaultView(MonitorTagsViewModel.MonitorTagCollection) as ListCollectionView;
            if (defaultView != null)
            {
                if (defaultView.IsAddingNew)
                    defaultView.CancelNew();

                if (defaultView.IsEditingItem)
                    defaultView.CancelEdit();
            }

            defaultView =
                CollectionViewSource.GetDefaultView(EditTagsViewModel.EditTagCollection) as ListCollectionView;
            if (defaultView != null)
            {
                if (defaultView.IsAddingNew)
                    defaultView.CancelNew();

                if (defaultView.IsEditingItem)
                    defaultView.CancelEdit();
            }

            return 0;
        }

        public bool CanApply()
        {
            return true;
        }

        private void UpdateDataContextSource()
        {
            _dataContextSource.Clear();

            AoiDefinition aoiDefinition = Scope?.Tags?.ParentProgram as AoiDefinition;

            if (aoiDefinition != null)
            {
                _dataContextSource.Add(new AoiDataReference(null, null, null,OnlineEditType.Original, $"{aoiDefinition.Name} <definition>"));

                foreach (var reference in aoiDefinition.References)
                {
                    if (reference?.ParamList == null) continue;
                    AddReference(reference);
                }
            }

            CurrentDataContext = _dataContextSource.Count > 0 ? _dataContextSource[0] : null;

        }
        
        private string _selectedFilterType;
        private string _filterName;
        private bool _isFilterOnName = true;
        private bool _isFilterOnDescription;
        private bool _isFilterOnBoth;

        private void AddReference(AoiDataReference aoiDataReference)
        {
            if (aoiDataReference == null)
                return;

            var referenceItem = _dataContextSource.LastOrDefault(r => r.Routine == aoiDataReference.Routine &&
                                                            r.Line == aoiDataReference.Line &&
                                                            r.Column == aoiDataReference.Column &&
                                                            r.InnerAoiDefinition ==
                                                            aoiDataReference.InnerAoiDefinition &&
                                                            r.InnerDataReference ==
                                                            aoiDataReference.InnerDataReference);

            if (referenceItem != null)
            {
                if (Equals(_currentDataContext, referenceItem))
                {
                    CurrentDataContext = _dataContextSource[0];
                }

                _dataContextSource.Remove(referenceItem);
            }

            var aoiNode = aoiDataReference.ParamList?[0] as ASTName;

            var program = aoiDataReference.Routine.ParentCollection.ParentProgram;
            var aoiTagName = ObtainValue.GetAstName(aoiNode);
            if (aoiDataReference.InnerDataReference != null)
            {
                var referenceName =
                    ObtainValue.GetAstName(aoiDataReference.InnerDataReference.ParamList?[0] as ASTName);
                aoiDataReference.Title = $"{aoiTagName} ({referenceName})";
            }
            else
            {
                if (aoiTagName.StartsWith("\\"))
                {
                    var scope = aoiTagName.Substring(1,
                        aoiTagName.IndexOf(".", StringComparison.OrdinalIgnoreCase) - 1);
                    aoiDataReference.Title = $"{aoiTagName} ({scope})";
                }
                else
                {
                    var tagName = aoiTagName;
                    if (tagName.IndexOf(".", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        tagName = tagName.Substring(0, tagName.IndexOf(".", StringComparison.OrdinalIgnoreCase));
                    }

                    var tag = program.Tags[tagName];
                    var scope = tag?.ParentCollection.ParentProgram.Name ?? "Controller";
                    aoiDataReference.Title = $"{aoiTagName} ({scope})";
                }
            }

            var item = _dataContextSource.FirstOrDefault(r => AoiDataReferenceExtend.CompareIndex(r, aoiDataReference));
            if (item != null)
            {
                _dataContextSource.Insert(_dataContextSource.IndexOf(item), aoiDataReference);
                return;
            }
            else
            {
                item = _dataContextSource.LastOrDefault(r => r.Title.StartsWith(aoiTagName));
                if (item != null)
                {
                    _dataContextSource.Insert(_dataContextSource.IndexOf(item) + 1, aoiDataReference);
                    return;
                }
            }

            _dataContextSource.Add(aoiDataReference);
        }

        private void KeepDataContextSelectedInMonitor()
        {
            if (!IsInAOI)
                return;

            if (_selectedIndex == 1)
            {
                _lastDataContext = _currentDataContext;

                CurrentDataContext = _dataContextSource.Count > 0 ? _dataContextSource[0] : null;
            }
            else if (_selectedIndex == 0)
            {
                if (_dataContextSource.Contains(_lastDataContext))
                {
                    CurrentDataContext = _lastDataContext;
                }
                else
                {
                    CurrentDataContext = _dataContextSource.Count > 0 ? _dataContextSource[0] : null;
                }
            }
        }

        private void KeepFocusName(int oldIndex, int newIndex)
        {
            if (oldIndex != newIndex)
            {
                var focusName = oldIndex == 0
                    ? MonitorTagsViewModel?.SelectedMonitorTagItem?.Name
                    : EditTagsViewModel?.SelectedEditTagItem?.Name;

                SetFocusName(focusName);
            }
        }
    }
}