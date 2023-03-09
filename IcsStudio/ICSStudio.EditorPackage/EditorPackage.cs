//------------------------------------------------------------------------------
// <copyright file="EditorPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;
using ICSStudio.AvalonEdit.Document;
using ICSStudio.Dialogs.WaitDialog;
using ICSStudio.EditorPackage.DataTypes;
using ICSStudio.EditorPackage.FBDEditor;
using ICSStudio.EditorPackage.FlowChartEditor;
using ICSStudio.EditorPackage.ModuleProperties;
using ICSStudio.EditorPackage.MonitorEditTags;
using ICSStudio.EditorPackage.Reference;
using ICSStudio.EditorPackage.RLLEditor;
using ICSStudio.EditorPackage.STEditor;
using ICSStudio.EditorPackage.Trend;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.CodeAnalysis;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.Online;
using ICSStudio.SimpleServices.SourceProtection;
using ICSStudio.StxEditor.ViewModel.CodeSnippets;
using ICSStudio.StxEditor.ViewModel.Highlighting;
using ICSStudio.UIInterfaces.Editor;
using ICSStudio.UIInterfaces.Error;
using ICSStudio.UIInterfaces.Project;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;
using Type = System.Type;

namespace ICSStudio.EditorPackage
{
    /// <summary>
    ///     This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The minimum requirement for a class to be considered a valid package for Visual Studio
    ///         is to implement the IVsPackage interface and register itself with the shell.
    ///         This package uses the helper classes defined inside the Managed Package Framework (MPF)
    ///         to do it: it derives from the Package class that provides the implementation of the
    ///         IVsPackage interface and uses the registration attributes defined in the framework to
    ///         register itself and its components with the shell. These attributes tell the pkgdef creation
    ///         utility what data to put into .pkgdef file.
    ///     </para>
    ///     <para>
    ///         To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...
    ///         &gt; in .vsixmanifest file.
    ///     </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideService(typeof(SCreateEditorService))]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [Guid(PackageGuidString)]
    // ReSharper disable InvalidXmlDocComment
    public sealed class EditorPackage : Package, ICreateEditorService, SCreateEditorService
    {
        /// <summary>
        ///     EditorPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "0e9f1242-68fd-479d-9723-206491f2b751";

        private readonly EditorWindowManager _windowManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EditorPackage" /> class.
        /// </summary>
        public EditorPackage()
        {

            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
            IServiceContainer serviceContainer = this;
            ServiceCreatorCallback creationCallback = CreateService;
            serviceContainer.AddService(typeof(SCreateEditorService),
                creationCallback, true);

            _windowManager = new EditorWindowManager();

        }

        public void CloseAllToolWindows(bool closeProject = true)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                _windowManager.CloseAllEditorWindowPanes(closeProject);
                Ladder.Graph.Styles.BoxInstructionStyleRenderer.ClearBindings();
                RLLEditorViewModel.ClearOperands();
            });
        }

        public int ApplyAllToolWindows()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return _windowManager.ApplyAllToolWindows();
        }

        public void CloseWindow(int id)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _windowManager.CloseEditorPane(id);
        }

        public object GetWindow(int id)
        {
            return _windowManager.GetEditorWindowPane(id);
        }

        public void CreateRLLEditor(IRoutine routine, int? rungIndex = null, int? offset = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            // check permission
            var projectInfoService =
                GetService(typeof(SProjectInfoService)) as IProjectInfoService;
            var controller = projectInfoService?.Controller as Controller;
            if (controller == null)
                return;

            var permission = controller.SourceProtectionManager.GetPermission(routine);
            if (permission == SourcePermission.Use)
                return;

            var aoiDefinition = routine.ParentCollection?.ParentProgram as IAoiDefinition;
            if (aoiDefinition != null)
            {
                permission = controller.SourceProtectionManager.GetPermission(aoiDefinition);
                if (permission == SourcePermission.Use)
                    return;
            }

            var editorWindowPane = _windowManager.GetEditorWindowPane(routine.Uid);

            try
            {
                if (editorWindowPane == null)
                {
                    var rllEditorControl = new RLLEditorControl();
                    var rllEditorViewModel = new RLLEditorViewModel(routine, rllEditorControl);
                    editorWindowPane = _windowManager.CreateEditorWindowPane(routine.Uid, rllEditorViewModel);
                    rllEditorControl.ViewModel = rllEditorViewModel;
                }

                var vm = (RLLEditorViewModel)editorWindowPane.EditorPane;
                if (rungIndex >= 0 && rungIndex < vm.Graph.Rungs.Count)
                {
                    var focusedRung = vm.Graph.Rungs[rungIndex.Value];
                    if (offset < 0 || offset == null)
                    {
                        vm.Graph.FocusedItem = focusedRung;
                        offset = -1;
                    }

                    vm.LocateElement(rungIndex.Value, offset.Value);
                }

                ErrorHandler.ThrowOnFailure(editorWindowPane.Frame.Show());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }

        public void CloseMonitorEditTags(
            IController controller, ITagCollectionContainer tagCollectionContainer,
            string focusName = "", bool isEditMonitorShow = false)
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                // close
                _windowManager.CloseMonitorWindowPanes(tagCollectionContainer);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }

        public void CreateSTEditor(IRoutine routine, OnlineEditType onlineEditType, int? line = null,
            int? offset = null, int? len = null,
            AoiDataReference aoiDataReference = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            // check permission
            var projectInfoService =
                GetService(typeof(SProjectInfoService)) as IProjectInfoService;
            var controller = projectInfoService?.Controller as Controller;
            if (controller == null)
                return;

            var permission = controller.SourceProtectionManager.GetPermission(routine);
            if (permission == SourcePermission.Use)
                return;

            var aoiDefinition = routine.ParentCollection.ParentProgram as IAoiDefinition;
            if (aoiDefinition != null)
            {
                permission = controller.SourceProtectionManager.GetPermission(aoiDefinition);
                if (permission == SourcePermission.Use)
                    return;
            }

            var editorWindowPane = _windowManager.GetEditorWindowPane(routine.Uid);
            Action action = () =>
            {
                Dispatcher.CurrentDispatcher.VerifyAccess();
                try
                {
                    // open
                    var stEditorControl = new STEditorControl();
                    bool isOpening = true;
                    if (editorWindowPane == null)
                    {
                        var stEditorViewModel = new STEditorViewModel(routine, stEditorControl);
                        editorWindowPane = _windowManager.CreateEditorWindowPane(routine.Uid, stEditorViewModel);
                        stEditorControl.Loaded += StEditorControl_Loaded;
                    }
                    else
                    {
                        isOpening = false;
                    }

                    var vm = ((STEditorViewModel)editorWindowPane.EditorPane);
                    vm.Options.IsOnlyTextMarker = isOpening;
                    if (onlineEditType == OnlineEditType.Original)
                    {
                        var st = (STRoutine)routine;
                        if (st.PendingCodeText == null && st.TestCodeText == null)
                            vm.Options.HideAll = true;
                        else
                            vm.Options.ShowOriginal = true;
                    }

                    if (onlineEditType == OnlineEditType.Pending)
                        vm.Options.ShowPending = true;
                    if (onlineEditType == OnlineEditType.Test)
                        vm.Options.ShowTest = true;
                    ErrorHandler.ThrowOnFailure(editorWindowPane.Frame.Show());
                    vm.Options.IsOnlyTextMarker = false;
                    if (line != null && offset != null || aoiDataReference != null)
                    {
                        if (stEditorControl == vm.Control)
                        {
#pragma warning disable VSTHRD001 // 避免旧线程切换 API
                            stEditorControl.Dispatcher.Invoke(delegate
#pragma warning restore VSTHRD001 // 避免旧线程切换 API
                            {
                                var viewModel = editorWindowPane.EditorPane as STEditorViewModel;
                                if (line != null && offset != null)
                                {
                                    viewModel?.SetCaret((int)line, (int)offset, len ?? 0);
                                }

                                if (aoiDataReference != null)
                                {
                                    viewModel?.SetReference(aoiDataReference);
                                }
                            }, DispatcherPriority.SystemIdle);
                        }
                        else
                        {
                            var viewModel = editorWindowPane.EditorPane as STEditorViewModel;
                            if (line != null && offset != null)
                            {
                                viewModel?.SetCaret((int)line, (int)offset, len ?? 0);
                            }

                            if (aoiDataReference != null)
                            {
                                viewModel?.SetReference(aoiDataReference);
                            }
                        }
                    }

                    vm.IsInitial = false;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            };
            if (editorWindowPane == null)
            {
                _waitDialog = new Wait(action)
                {
                    Owner = Application.Current.MainWindow,
                    Object = routine
                };
                _waitDialog.ShowDialog();
            }
            else
            {
                action();
            }
        }

        private void StEditorControl_Loaded(object sender, RoutedEventArgs e)
        {
            var control = (STEditorControl)sender;
#pragma warning disable VSTHRD001 // 避免旧线程切换 API
            control.Dispatcher.InvokeAsync(() =>
#pragma warning restore VSTHRD001 // 避免旧线程切换 API
            {
                var vm = (STEditorViewModel)control.DataContext;
                //ui卡住可能导致绑定失效
                if (!(vm.Options.IsTopLoaded && vm.Options.IsBottomLoaded))
                {
                    vm.UpdateControl();
                }

                _waitDialog?.Close();

            }, DispatcherPriority.Background);
        }

        private Wait _waitDialog;

        public void CreateSFCEditor(IRoutine routine)
        {
            try
            {
                var sfcEditorControl = new FlowChartEditorControl();
                var sfcEditorViewModel = new FlowChartEditorViewModel(routine, sfcEditorControl);

                var editorWindowPane = _windowManager.CreateEditorWindowPane(routine.Uid, sfcEditorViewModel);

                ThreadHelper.ThrowIfNotOnUIThread();
                ErrorHandler.ThrowOnFailure(editorWindowPane.Frame.Show());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }

        public void CreateFBDEditor(IRoutine routine)
        {
            try
            {
                var fbdEditorControl = new FBDEditorControl();
                var fbdEditorViewModel = new FBDEditorViewModel(routine, fbdEditorControl);

                var editorWindowPane = _windowManager.CreateEditorWindowPane(routine.Uid, fbdEditorViewModel);

                ThreadHelper.ThrowIfNotOnUIThread();
                ErrorHandler.ThrowOnFailure(editorWindowPane.Frame.Show());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }

        public void CreateModuleProperties(IDeviceModule deviceModule)
        {
            try
            {
                var modulePropertiesViewModel = new ModulePropertiesViewModel(deviceModule);

                var editorWindowPane =
                    _windowManager.CreateEditorWindowPane(deviceModule.Uid, modulePropertiesViewModel);

                ThreadHelper.ThrowIfNotOnUIThread();
                ErrorHandler.ThrowOnFailure(editorWindowPane.Frame.Show());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }

        public List<IDeviceModule> GetDeviceModulesInOpen()
        {
            var moduleProperties = _windowManager.GetAllModuleProperties();
            List<IDeviceModule> result = new List<IDeviceModule>();

            if (moduleProperties != null)
            {
                foreach (var windowPane in moduleProperties)
                {
                    var viewModel = windowPane.EditorPane as ModulePropertiesViewModel;
                    if (viewModel != null)
                        result.Add(viewModel.DeviceModule);
                }
            }

            return result;
        }

        public void CreateMonitorEditTags(
            IController controller, ITagCollectionContainer tagCollectionContainer,
            string focusName = "", bool isEditMonitorShow = false)
        {
            try
            {
                MonitorEditTagsViewModel monitorEditorViewModel;

                EditorWindowPane editorWindowPane =
                    _windowManager.CreateEditorWindowPane(tagCollectionContainer.GetHashCode(), null);
                if (editorWindowPane == null)
                {
                    monitorEditorViewModel =
                        new MonitorEditTagsViewModel(controller,
                            tagCollectionContainer,
                            new MonitorEditTagsControl());

                    editorWindowPane =
                        _windowManager.CreateEditorWindowPane(tagCollectionContainer.GetHashCode(),
                            monitorEditorViewModel);
                }

                ThreadHelper.ThrowIfNotOnUIThread();
                ErrorHandler.ThrowOnFailure(editorWindowPane.Frame.Show());
                // reset
                monitorEditorViewModel = editorWindowPane.EditorPane as MonitorEditTagsViewModel;

                if (monitorEditorViewModel != null)
                {
                    monitorEditorViewModel.Scope = tagCollectionContainer;
                    monitorEditorViewModel.SelectedIndex = isEditMonitorShow ? 1 : 0;
                    monitorEditorViewModel.SetFocusName(focusName);

                    MonitorEditTagsControl monitorEditTagsControl =
                        monitorEditorViewModel.Control as MonitorEditTagsControl;
                    monitorEditTagsControl?.Refresh();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }

        public void CreateAoiMonitorEditTags(
            IController controller, ITagCollectionContainer tagCollectionContainer, AoiDataReference aoiDataReference,
            string focusName = "", bool isEditMonitorShow = false)
        {
            try
            {
                MonitorEditTagsViewModel monitorEditorViewModel;

                EditorWindowPane editorWindowPane =
                    _windowManager.CreateEditorWindowPane(tagCollectionContainer.GetHashCode(), null);


                if (editorWindowPane == null)
                {
                    monitorEditorViewModel =
                        new MonitorEditTagsViewModel(controller,
                            tagCollectionContainer,
                            new MonitorEditTagsControl());

                    editorWindowPane =
                        _windowManager.CreateEditorWindowPane(tagCollectionContainer.GetHashCode(),
                            monitorEditorViewModel);
                    //monitorEditorViewModel.CurrentDataContext = aoiDataReference;
                }

                ThreadHelper.ThrowIfNotOnUIThread();
                ErrorHandler.ThrowOnFailure(editorWindowPane.Frame.Show());

                // reset
                monitorEditorViewModel = editorWindowPane.EditorPane as MonitorEditTagsViewModel;

                if (monitorEditorViewModel != null)
                {
                    monitorEditorViewModel.Scope = tagCollectionContainer;
                    monitorEditorViewModel.SelectedIndex = isEditMonitorShow ? 1 : 0;
                    monitorEditorViewModel.SetFocusName(focusName);

                    MonitorEditTagsControl monitorEditTagsControl =
                        monitorEditorViewModel.Control as MonitorEditTagsControl;
                    monitorEditTagsControl?.Refresh();
                }

                if (monitorEditorViewModel != null) monitorEditorViewModel.CurrentDataContext = aoiDataReference;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }

        public void CreateNewDataType(IDataType dataType)
        {
            try
            {
                var viewModel = new NewDataTypeViewModel(new NewDataType(), dataType);

                var editorWindowPane =
                    _windowManager.CreateEditorWindowPane(dataType?.Uid ?? Guid.NewGuid().GetHashCode(), viewModel);

                ThreadHelper.ThrowIfNotOnUIThread();
                ErrorHandler.ThrowOnFailure(editorWindowPane.Frame.Show());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }

        public void CloseSTEditor(IRoutine routine)
        {
            _windowManager.CloseEditorPane(routine.Uid);
            ThreadHelper.ThrowIfNotOnUIThread();
        }

        private EditorWindowPane pane;
        public void CreateCrossReference(ICSStudio.UIInterfaces.Editor.Type filterType, IProgramModule program,
            string name)
        {
            pane = _windowManager.GetCrossReferencePane();
            if (pane == null)
            {
                var vm = new CrossReferenceViewModel(filterType, program, name);
                pane = _windowManager.CreateEditorWindowPane(0, vm);
            }

            pane.Caption = LanguageManager.GetInstance().ConvertSpecifier("Cross Reference");
            var crossReferenceViewModel = pane.EditorPane as CrossReferenceViewModel;
            if (filterType == UIInterfaces.Editor.Type.AOI)
            {
                crossReferenceViewModel?.UpdateAoiInstructionSearch(name);
            }
            else if (filterType == UIInterfaces.Editor.Type.Routine)
            {
                crossReferenceViewModel?.UpdateRoutineSearch(name, program.Name);
            }
            else
            {
                crossReferenceViewModel?.UpdateTagSearch(name,
                    program == null ? Controller.GetInstance().Name : program.Name);
            }

            ThreadHelper.ThrowIfNotOnUIThread();
            ErrorHandler.ThrowOnFailure(pane.Frame.Show());
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);

        }

        public void CreateCrossReference(IRoutine routine, string name)
        {
            pane = _windowManager.GetCrossReferencePane();
            var program = routine.ParentCollection.ParentProgram;
            var filterType = UIInterfaces.Editor.Type.Label;
            if (pane == null)
            {
                var vm = new CrossReferenceViewModel(filterType, program, name);
                pane = _windowManager.CreateEditorWindowPane(0, vm);
            }

            pane.Caption = LanguageManager.GetInstance().ConvertSpecifier("Cross Reference");
            var crossReferenceViewModel = pane.EditorPane as CrossReferenceViewModel;

            crossReferenceViewModel?.UpdateLabelSearch(name, program.Name, routine.Name);

            ThreadHelper.ThrowIfNotOnUIThread();
            ErrorHandler.ThrowOnFailure(pane.Frame.Show());
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            pane.Caption = LanguageManager.GetInstance().ConvertSpecifier("Cross Reference");
        }

        public void CreateTrend(ITrend trend, bool isRunImmediately = false)
        {
            var control = new Trend.Trend();
            var vm = new TrendViewModel(control, trend, isRunImmediately);
            var pane = _windowManager.CreateEditorWindowPane(trend.Uid, vm);
            control.Frame = pane.Frame;
            ThreadHelper.ThrowIfNotOnUIThread();
            ErrorHandler.ThrowOnFailure(pane.Frame.Show());
        }

        public object GetActiveEditorWindow()
        {
            return _windowManager.GetActiveEditorWindow();
        }

        public void ParseRoutine(IRoutine routine, bool needAddAoiDataReference, bool canAddError = false,
            bool isForce = false, bool isOnlyParseEdit = false)
        {
            //TODO(zyl):add other routine
            //if(!(routine is STRoutine))return;
            if (OnlineEditHelper.CompilingPrograms.Contains(routine.ParentCollection.ParentProgram) && !isForce)
            {
                return;
            }

            if (routine is STRoutine)
            {
                if (canAddError)
                {
                    _outputService?.AddMessages(
                        $"Verifying routine '{routine.Name}' of {(routine.ParentCollection.ParentProgram is AoiDefinition ? "add-on instruction" : "program")} '{routine.ParentCollection.ParentProgram.Name}'",
                        routine);
                }

                var existPane = _windowManager.GetEditorWindowPane(routine.Uid);
                if (existPane != null)
                {
                    var viewmodel = ((STEditorViewModel)existPane.EditorPane);
                    if ((routine as STRoutine)?.IsUpdateChanged ?? false)
                    {
                        viewmodel.Document.Reset();
                    }

                    if (!viewmodel.Document.NeedParse && !routine.IsError)
                    {
                        if (!routine.IsError)
                        {
                            var errorOutput =
                                Package.GetGlobalService(typeof(SErrorOutputService)) as IErrorOutputService;
                            errorOutput?.RemoveError(routine, ((STRoutine)routine).CurrentOnlineEditType);
                        }

                        return;
                    }

                    viewmodel.Document.SnippetLexer.CanAddError = canAddError;
                    viewmodel.Document.IsNeedBackground = !canAddError;
                    if (isForce)
                    {
                        viewmodel.Document.SnippetLexer.ParserWholeCode(viewmodel.Document.GetCurrentCode(), false,
                            true);
                    }
                    else
                    {
                        viewmodel.Document.UpdateLexer(false);
                    }

                    viewmodel.Document.SnippetLexer.CanAddError = false;
                    viewmodel.Document.IsNeedBackground = true;
                    viewmodel.Document.NeedParse = false;
                    ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        viewmodel.SetView();
                    });
                }
                else
                {
                    if (!Controller.GetInstance().IsLoading && (isOnlyParseEdit || !routine.IsError)) return;
                    var st = routine as STRoutine;
                    {
                        string code = string.Join("\n", st.CodeText);

                        var document = new TextDocument(code);

                        TextMarkerService textMarkerService = new TextMarkerService(document, st);
                        textMarkerService.OnlyAddError = true;
                        var lexer = new SnippetLexer(textMarkerService, st, needAddAoiDataReference);
                        lexer.CanAddError = canAddError;
                        st.CurrentOnlineEditType = OnlineEditType.Original;
                        lexer.ParserWholeCode(code, false);
                        if (st.PendingCodeText != null)
                        {
                            st.CurrentOnlineEditType = OnlineEditType.Pending;
                            code = string.Join("\n", st.PendingCodeText);
                            document.Text = code;
                            lexer.ParserWholeCode(code, false);
                        }

                        if (st.TestCodeText != null)
                        {
                            st.CurrentOnlineEditType = OnlineEditType.Test;
                            code = string.Join("\n", st.TestCodeText);
                            document.Text = code;
                            lexer.ParserWholeCode(code, false);
                        }

                        st.CurrentOnlineEditType = OnlineEditType.Original;
                        textMarkerService.Clear();
                    }
                }
            }

            if (routine is RLLRoutine)
            {
                var ld = routine as RLLRoutine;
                try
                {
                    routine.IsError = false;
                    //var codeText = ld.CodeText;
                    //_outputService?.RemoveMessage(ld);
                    //_outputService?.RemoveError(ld, ld.CurrentOnlineEditType);
                    _outputService?.AddMessages(
                        $"Verifying routine '{ld.Name}' of program '{ld.ParentCollection.ParentProgram.Name}'", ld);

                    RLLGrammarAnalysis grammarAnalysis = new RLLGrammarAnalysis();
                    //grammarAnalysis.Analysis(string.Join("    ", codeText), (Controller)ld.ParentController, ld);
                    grammarAnalysis.Analysis(ld);

                    foreach (var error in grammarAnalysis.Errors)
                    {
                        if (error.Level == Level.Error)
                            _outputService?.AddErrors(error.Info, OrderType.None, OnlineEditType.Original,
                                error.RungIndex, error.Offset, ld);
                        if (error.Level == Level.Warning)
                            _outputService?.AddWarnings(error.Info, ld, error.RungIndex, error.Offset);
                    }
                }
                catch (ErrorInfo error)
                {
                    Debug.WriteLine(error.Message + error.StackTrace);
                    if (error.Level == Level.Error)
                        _outputService?.AddErrors(error.Info, OrderType.None, OnlineEditType.Original, error.RungIndex,
                            error.Offset, ld);
                    if (error.Level == Level.Warning)
                        _outputService?.AddWarnings(error.Info, ld);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message + e.StackTrace);
                    _outputService?.AddErrors("Unknown Error.", OrderType.None, OnlineEditType.Original, 0, null, ld);
                }
            }

            //TODO(zyl):add other routine
        }

        readonly IErrorOutputService _outputService =
            Package.GetGlobalService(typeof(SErrorOutputService)) as IErrorOutputService;

        public void ParseErrorRoutine()
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += (sender, e) =>
            {
                var routines = _windowManager.GetErrorRoutinePane();
                var currentPane = CurrentObject.GetInstance().Current as STEditorViewModel;
                var currentRoutine = currentPane?.Routine;
                if (currentRoutine == null) return;
                ParseRoutine(currentRoutine, true);
                foreach (var routine in routines)
                {
                    if (routine == currentRoutine) continue;
                    var st = routine as STRoutine;
                    if (st != null)
                    {
                        ParseRoutine(st, true);
                    }
                }
            };
            backgroundWorker.RunWorkerAsync();
        }

        public void UpdateAllRoutineOnlineStatus()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var panes = _windowManager.GetAllRoutinePane();
            foreach (var editorWindowPane in panes)
            {
                var stPane = editorWindowPane.EditorPane as STEditorViewModel;
                if (stPane != null)
                {
                    stPane.Options.IsRaiseCommandStatus = true;
                    if (OnlineEditHelper.CompilingPrograms.Contains(stPane.Routine.ParentCollection.ParentProgram))
                    {
                        stPane.Options.CanEditorInput = stPane.Routine.CurrentOnlineEditType == OnlineEditType.Pending;
                    }
                    else
                    {
                        if (Controller.GetInstance().IsOnline)
                        {
                            stPane.Options.CanEditorInput =
                                stPane.Routine.CurrentOnlineEditType == OnlineEditType.Pending;
                        }
                        else
                        {
                            stPane.Options.CanEditorInput = !Controller.GetInstance().IsOnline;
                        }
                    }
                }
            }

            IVsUIShell vsShell =
                Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;

            if (vsShell != null)
            {
                int hr = vsShell.UpdateCommandUI(0);
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
            }
        }

        public List<ITag> ParseRoutineTag(IRoutine routine)
        {
            var tags = new List<ITag>();

            var st = routine as STRoutine;
            if (st != null)
            {
                ParseRoutine(st, false);
                return st.GetAllReferenceTags();
            }

            RLLRoutine rll = routine as RLLRoutine;
            if (rll != null)
            {
                try
                {
                    ASTNode astRllRoutine =
                        RLLGrammarParser.Parse(rll.CodeText, rll.ParentController as Controller);

                    RLLTagReferenceHelper helper = new RLLTagReferenceHelper(rll);
                    astRllRoutine.Accept(helper);

                    return helper.Tags;
                }
                catch (Exception)
                {
                    //ignore
                }

            }

            return tags;
        }

        public bool CheckRoutineInRun(IRoutine routine)
        {
            var stRoutine = routine as STRoutine;
            if (stRoutine != null)
                return RoutineExtend.CheckRoutineInRun(stRoutine);
            return true;
        }

        public bool IsThereATrendRunning()
        {
            foreach (var item in _windowManager.GetAllTrendPane())
            {
                var trendPane = item.EditorPane as TrendViewModel;
                if (trendPane != null)
                    if (trendPane.Trend.IsScrolling)
                        return true;
            }

            return false;
        }

        public List<UIElement> GetAllEditors()
        {
            return _windowManager.GetAllEditors();
        }

        public void SetStOnlineEditMode(IRoutine routine, OnlineEditType type)
        {
            var st = routine as STRoutine;
            if (st != null)
            {
                var editorWindowPane = _windowManager.GetEditorWindowPane(routine.Uid);
                if (editorWindowPane != null)
                {
                    var vm = editorWindowPane.EditorPane as STEditorViewModel;
                    if (vm != null)
                    {
                        if (type == OnlineEditType.Original)
                            vm.Options.ShowOriginal = true;
                        if (type == OnlineEditType.Pending)
                            vm.Options.ShowOriginal = true;
                        if (type == OnlineEditType.Test)
                            vm.Options.ShowTest = true;
                    }
                }
            }
        }

        #region Package Members



        /// <summary>
        ///     Initialization of the package; this method is called right after the package is sited, so this is the place
        ///     where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        //protected override void Initialize()
        //{
        //    base.Initialize();
        //}

        #endregion

        private object CreateService(IServiceContainer container, Type serviceType)
        {
            if (container != this) return null;

            if (typeof(SCreateEditorService) == serviceType) return this;

            return null;
        }

    }
    // ReSharper restore InvalidXmlDocComment
}