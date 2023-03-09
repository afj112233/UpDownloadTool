using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIInterfaces.Editor;
using ICSStudio.UIInterfaces.Error;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.UIInterfaces.ControllerOrganizer;

namespace ICSStudio.ErrorOutputPackage.View
{
    public partial class ErrorListControl : IErrorList
    {
        private readonly ErrorListDataModel _dataContext;

        public ErrorListControl(bool isShowTopBar)
        {
            _dataContext = new ErrorListDataModel(isShowTopBar);
            InitializeComponent();
            SearchBox.DataContext = _dataContext;
            dgv.DataContext = _dataContext;
            clearButton.DataContext = _dataContext;
            TopButtonBar.Visibility = isShowTopBar ? Visibility.Visible : Visibility.Collapsed;
            SetTextBoxBindings();
            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged",
                LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        #region IErrorListReporter

        public ObservableCollection<ErrorListDataEntry> DataBindingContext
        {
            get { return _dataContext.ErrorListData; }
            set
            {
                if (value == null)
                {
                    throw new System.ArgumentNullException("Unable to bind to a null reference");
                }

                _dataContext.ErrorListData = value;
            }
        }

        public bool ErrorsVisible
        {
            get { return tglBtnErrors.IsChecked.HasValue && tglBtnErrors.IsChecked.Value; }
            set { tglBtnErrors.IsChecked = value; }
        }

        public bool WarningsVisible
        {
            get { return tglBtnWarnings.IsChecked.HasValue && tglBtnWarnings.IsChecked.Value; }
            set { tglBtnWarnings.IsChecked = value; }
        }

        public bool MessagesVisible
        {
            get { return tglBtnMessages.IsChecked.HasValue && tglBtnMessages.IsChecked.Value; }
            set { tglBtnMessages.IsChecked = value; }
        }

        public void ClearAll()
        {
            _dataContext.ErrorListData = new ObservableCollection<ErrorListDataEntry>();
        }

        //private object _syncRoot = new object();

        public void AddError(string description, OrderType orderType, OnlineEditType onlineEditType, int? line,
            int? offset, object original, int? len = null)
        {
            //lock (_syncRoot)
            {
                _dataContext.AddError(description, orderType, onlineEditType, line, offset, len, original);
                ScrollToEnd();
            }
        }

        public void AddWarning(string description, object original, int? line = null, int? offset = null,
            Destination destination = Destination.None)
        {
            //lock (_syncRoot)
            {
                _dataContext.AddWarning(description, original, line, offset, destination);

                ScrollToEnd();
            }
        }

        private void ScrollToEnd()
        {
            if (Thread.CurrentThread == Application.Current.Dispatcher.Thread)
            {
                if (VisualTreeHelper.GetChildrenCount(dgv) > 0)
                {
                    var border = VisualTreeHelper.GetChild(dgv, 0) as Decorator;
                    if (border != null)
                    {
                        var scroll = border.Child as ScrollViewer;
                        scroll?.ScrollToEnd();
                    }
                }
            }
            else
            {
                Dispatcher.InvokeAsync(() =>
                {
                    if (VisualTreeHelper.GetChildrenCount(dgv) > 0)
                    {
                        var border = VisualTreeHelper.GetChild(dgv, 0) as Decorator;
                        if (border != null)
                        {
                            var scroll = border.Child as ScrollViewer;
                            scroll?.ScrollToEnd();
                        }
                    }
                });
            }
        }

        public void Remove(ErrorListLevel level, object original)
        {
            //lock (_syncRoot)
            _dataContext.Remove(level, original);
        }

        public void Remove(ErrorListLevel level, IRoutine original, OnlineEditType onlineEditType)
        {
            //lock (_syncRoot)
            _dataContext.Remove(level, original, onlineEditType);
        }

        public void RemoveImportError()
        {
            //lock (_syncRoot)
            {
                _dataContext.RemoveImportError();
            }
        }

        public void AddInformation(string description, object original)
        {
            //lock (_syncRoot)
            {
                _dataContext.AddInformation(description, original);
                ScrollToEnd();
            }
        }

        public List<IRoutine> GetErrorRoutines()
        {
            return _dataContext.GetErrorRoutines();
        }

        public void Summary()
        {
            _dataContext.Summary();
        }

        public bool HasError => _dataContext.ErrorListData.Any(e => e.Level == ErrorListLevel.Error);

        #endregion IErrorListReporter

        #region EventHandlers

        private void Errors_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            ToggleButton tgl = (ToggleButton) sender;
            _dataContext.ShowErrors = tgl.IsChecked.HasValue && tgl.IsChecked.Value;
        }

        private void Errors_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            ToggleButton tgl = (ToggleButton) sender;
            _dataContext.ShowErrors = tgl.IsChecked.HasValue && tgl.IsChecked.Value;
        }

        private void Warnings_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            ToggleButton tgl = (ToggleButton) sender;
            _dataContext.ShowWarnings = tgl.IsChecked.HasValue && tgl.IsChecked.Value;
        }

        private void Warnings_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            ToggleButton tgl = (ToggleButton) sender;
            _dataContext.ShowWarnings = tgl.IsChecked.HasValue && tgl.IsChecked.Value;
        }

        private void Informations_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            ToggleButton tgl = (ToggleButton) sender;
            _dataContext.ShowInformations = tgl.IsChecked.HasValue && tgl.IsChecked.Value;
        }

        private void Informations_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            ToggleButton tgl = (ToggleButton) sender;
            _dataContext.ShowInformations = tgl.IsChecked.HasValue && tgl.IsChecked.Value;
        }

        #endregion EventHandlers

        private void SetTextBoxBindings()
        {
            txtErrors.DataContext = _dataContext;
            txtWarnings.DataContext = _dataContext;
            txtMessages.DataContext = _dataContext;
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            var data = (ErrorListDataEntry) ((Hyperlink) sender).DataContext;

            if ((data.Original as IBaseObject)?.IsDeleted ?? false) return;

            var st = data.Original as STRoutine;
            if (st != null)
            {
                if (data.Destination == Destination.None)
                {
                    var createEditorService =
                        Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
                    createEditorService?.CreateSTEditor(st, data.OnlineEditType, data.Line, data.Offset, data.Length);
                }

                else if (data.Destination == Destination.ToControllerOrganizer)
                {
                    var controllerOrganizerService =
                        Package.GetGlobalService(typeof(SControllerOrganizerService)) as IControllerOrganizerService;
                    controllerOrganizerService?.SelectOrganizerItem(st);
                }

                return;
            }

            var rll = data.Original as RLLRoutine;
            if (rll != null)
            {
                var createEditorService =
                    Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
                createEditorService?.CreateRLLEditor(rll, data.Line, data.Offset);

                return;
            }

            var tag = data.Original as Tag;
            if (tag != null)
            {
                if (data.Destination == Destination.None)
                {
                    if (tag.ParentCollection.ParentProgram == null || tag.Usage == Usage.Local)
                    {
                        var createEditorService =
                            Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
                        var program = tag.ParentCollection.ParentProgram;
                        createEditorService?.CreateMonitorEditTags(tag.ParentController, program != null
                            ? (ITagCollectionContainer) program
                            : tag.ParentController, tag.Name);

                        return;
                    }

                    var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
                    var createDialogService =
                        Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;
                    var dialog =
                        createDialogService?.CreateProgramProperties((IProgram) tag.ParentCollection.ParentProgram,
                            true);
                    dialog?.Show(uiShell);

                    return;
                }

                else if (data.Destination == Destination.ToAxisProperty)
                {
                    var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
                    var createDialogService =
                        Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;

                    var axisCIPDrive = tag.DataWrapper as AxisCIPDrive;
                    if (axisCIPDrive != null)
                    {
                        createDialogService?.CreateAxisCIPDriveProperties(tag).Show(uiShell);

                        return;
                    }

                    var axisVirtual = tag.DataWrapper as AxisVirtual;
                    if (axisVirtual != null)
                    {
                        createDialogService?.CreateAxisVirtualProperties(tag).Show(uiShell);

                        return;
                    }
                }
            }

            var module = data.Original as IDeviceModule;
            if (module != null)
            {
                var createEditorService =
                    Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
                createEditorService?.CreateModuleProperties(module);

                return;
            }

            var mes = data.Original as string;
            if (mes != null)
            {
                var importPath = mes;
                var fi = new FileInfo(importPath);
                if (fi.Exists) Process.Start(importPath);

                return;
            }

            var task = data.Original as ITask;
            if(task != null)
            {
                var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
                var createDialogService =
                    Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;

                createDialogService?.CreateTaskProperties(task).Show(uiShell);
                return;
            }
        }
    }
}
