//------------------------------------------------------------------------------
// <copyright file="SearchCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Windows;
using ICSStudio.EditorPackage.STEditor;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIInterfaces.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ICSStudio.Dialogs.GoTo;
using ICSStudio.ErrorOutputPackage;
using ICSStudio.StxEditor.View;
using ICSStudio.Gui.Utils;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.UIServicesPackage.View;
using Type = ICSStudio.UIInterfaces.Editor.Type;
using ICSStudio.MultiLanguage;

namespace ICSStudio.SearchPackage
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class SearchCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>

        #region CommandId

        public const int searchMenu = 0x1000;

        public const int FindCommand = 0x1101;

        public const int ReplaceCommand = 0x1102;
        public const int GoToCommand = 0x1103;
        public const int BrowseLogicCommand = 0x1104;

        public const int CrossReferenceCommand = 0x1201;

        public const int FindNextCommand = 0x1301;
        public const int FindPreviousCommand = 0x1302;

        public const int NextResultCommand = 0x1401;
        public const int PreviousResultCommand = 0x1402;

        public const int MatchingKeywordCommand = 0x1501;
        public const int SearchResultCommandId = 0x1601;
        #endregion

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid SearchCommandPackageCmdSet = new Guid("b7cdc69e-719c-4325-9d59-004d1fe46c10");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package _package;

        private string SearchTextBackup { get; set; } = string.Empty;

        private string SearchText { get; set;} = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private SearchCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this._package = package;

            OleMenuCommandService commandService =
                this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            if (commandService != null)
            {
                AddGroup0Menus(commandService);
                AddGroup1Menus(commandService);
                AddGroup2Menus(commandService);
                AddGroup3Menus(commandService);
                AddGroup4Menus(commandService);
                var menuCommandID = new CommandID(SearchCommandPackageCmdSet, SearchResultCommandId);
                var menuItem = new OleMenuCommand(ShowSearch, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
                menuItem.Enabled = true;
                commandService.AddCommand(menuItem);
            }

            WeakEventManager<Controller, EventArgs>.AddHandler(
                Controller.GetInstance(), "Loaded", DoUpdateUI);
            WeakEventManager<CurrentObject, EventArgs>.AddHandler(
                CurrentObject.GetInstance(), "CurrentObjectChanged", DoUpdateUI);
        }
        private ToolWindowPane _toolWindowPane = null;

        public ToolWindowPane GetPane()
        {
            if (_toolWindowPane == null)
            {
                _toolWindowPane = _package.FindToolWindow(typeof(SearchResultWindow), 0, true);
            }
            return _toolWindowPane;
        }

        private void ShowSearch(object sender, EventArgs e)
        {
            ToolWindowPane window = GetPane();
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());

            var menuitem = sender as MenuCommand;          
            if (menuitem == null) return;
            if (menuitem.Checked)
            {
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Hide());
            }
        }

        private void DoUpdateUI(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                UpdateUI();
            });
        }

        #region group0

        private void AddGroup0Menus(OleMenuCommandService commandService)
        {
            var menuCommandID = new CommandID(SearchCommandPackageCmdSet, FindCommand);
            var menuItem = new OleMenuCommand(Find, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(SearchCommandPackageCmdSet, ReplaceCommand);
            menuItem = new OleMenuCommand(Replace, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(SearchCommandPackageCmdSet, GoToCommand);
            menuItem = new OleMenuCommand(GoTo, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(SearchCommandPackageCmdSet, BrowseLogicCommand);
            menuItem = new OleMenuCommand(BrowseLogic, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        private void BrowseLogic(object sender, EventArgs e)
        {

        }

        private void GoTo(object sender, EventArgs e)
        {
            var stViewModel = (STEditorViewModel) CurrentObject.GetInstance().Current;
            var editor = ((StxEditorView) stViewModel.BottomControl).TextEditor;
            var uiShell = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            var dialog = new GotoDialog();
            var routine = stViewModel.FocusedView.STRoutine;
            var program = routine.ParentCollection.ParentProgram;
            var vm = new GoToDialogViewModel(editor.Document.GetLocation(editor.CaretOffset).Line, editor.LineCount,
                ((StxEditorView) stViewModel.BottomControl).STRoutine, string.Empty, null,
                ((StxEditorView) stViewModel.BottomControl).STRoutine.ParentCollection.ParentProgram);
            dialog.DataContext = vm;
            if ((bool) dialog.ShowDialog(uiShell))
            {
                if (vm.SelectedKind.Equals("Line"))
                {
                    if (vm.LineNumber.Equals("end", StringComparison.OrdinalIgnoreCase))
                    {
                        editor.CaretOffset = editor.Document.Lines[editor.LineCount - 1].Offset;
                    }
                    else
                    {
                        int line = int.Parse(vm.LineNumber) - 1;
                        editor.CaretOffset = editor.Document.Lines[line].Offset;
                        editor.ScrollToLine(line);
                    }
                }

                else if (vm.SelectedKind.Equals("Edit") || vm.SelectedKind.Equals("Monitor"))
                {
                    if (program != null)
                    {
                        ICreateDialogService createDialogService =
                            Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SCreateDialogService)) as
                                ICreateDialogService;
                        var propertiesDialog = createDialogService?.CreateRoutineProperties(routine);
                        propertiesDialog?.Show(uiShell);
                    }
                }

                else if (vm.SelectedKind.Equals("Properties"))
                {
                    if (routine != null)
                    {
                        ICreateDialogService createDialogService =
                            Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SCreateDialogService)) as
                                ICreateDialogService;
                        var dialogProperties =
                            createDialogService?.CreateRoutineProperties(routine);
                        dialogProperties?.Show(uiShell);
                    }
                }

                if (vm.SelectedKind.Equals("Called Routines") || vm.SelectedKind.Equals("Calling Routines"))
                {
                    //TODO(zyl):
                }

                if (vm.SelectedKind.Equals("New"))
                {
                    ICreateDialogService createDialogService =
                        Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SCreateDialogService)) as
                            ICreateDialogService;
                    var dialogTagProperties =
                        createDialogService?.CreateNewTagDialog(DINT.Inst, program.Tags, Usage.Local);
                    dialogTagProperties?.Show(uiShell);
                }

                if (vm.SelectedKind.Equals("Cross Reference"))
                {
                    //TODO(zyl)
                }
            }
        }

        private void Replace(object sender, EventArgs e)
        {
            if (_isOpen) return;
            IVsUIShell vsShell =
                ServiceProvider.GetService(typeof(SVsUIShell)) as IVsUIShell;
            var findInRoutinesDialog = new FindInRoutines(true,false, null,true);
            findInRoutinesDialog.Owner = Application.Current.MainWindow;
            _isOpen = true;
            findInRoutinesDialog.Closed += FindInRoutinesDialog_Closed;
            findInRoutinesDialog.Show(vsShell);
        }

        private bool _isOpen = false;

        private void Find(object sender, EventArgs e)
        {
            FindInRoutines(false);
        }

        internal void FindInRoutines(bool isFromSearchText=true)
        {
            if (_isOpen) return;
            IVsUIShell vsShell =
                ServiceProvider.GetService(typeof(SVsUIShell)) as IVsUIShell;
            var searchText = SearchText;
            if (!isFromSearchText)
            {
                searchText = "";
                if (CurrentObject.GetInstance().Current is STEditorViewModel)
                {
                    searchText = ((STEditorViewModel)CurrentObject.GetInstance().Current).GetSelection();
                }
                //TODO(pyc):add other 
            }
            var findInRoutinesDialog = new FindInRoutines(false,false, searchText, true);
            findInRoutinesDialog.Owner = Application.Current.MainWindow;
            _isOpen = true;
            findInRoutinesDialog.Closed += FindInRoutinesDialog_Closed;
            findInRoutinesDialog.Show(vsShell);
        }

        private void FindInRoutinesDialog_Closed(object sender, EventArgs e)
        {
            ((Window)sender).Closed -= FindInRoutinesDialog_Closed;
            _isOpen = false;
        }

        #endregion

        #region group1

        private void AddGroup1Menus(OleMenuCommandService commandService)
        {
            var menuCommandID = new CommandID(SearchCommandPackageCmdSet, CrossReferenceCommand);
            var menuItem = new OleMenuCommand(CrossReference, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        private void CrossReference(object sender, EventArgs e)
        {
            ICreateEditorService createDialogService =
                Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SCreateEditorService)) as
                    ICreateEditorService;

            createDialogService?.CreateCrossReference(Type.Tag, null, null);
        }

        #endregion

        #region group2


        private void AddGroup2Menus(OleMenuCommandService commandService)
        {
            var menuCommandID = new CommandID(SearchCommandPackageCmdSet, FindNextCommand);
            var menuItem = new OleMenuCommand(FindNext, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(SearchCommandPackageCmdSet, FindPreviousCommand);
            menuItem = new OleMenuCommand(FindPrevious, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        public void FindSpecifiedTextInRoutines(string searchText)
        {
            SearchText = searchText;
            if (searchText != "")
            {
                SearchTextBackup = searchText;
            }
            FindOperationInRoutines(false);
        }

        private void FindNext(object sender, EventArgs e)
        {
            FindOperationInRoutines(false);
        }

        private void FindPrevious(object sender, EventArgs e)
        {
            FindOperationInRoutines(true);
        }

        internal void FindOperationInRoutines(bool isFindPrevious)
        {
            var isShowFindDialog = false;
            if (_isOpen) return;
            if (SearchText == "")
            {
                isShowFindDialog = true;
            }
            IVsUIShell vsShell =
                ServiceProvider.GetService(typeof(SVsUIShell)) as IVsUIShell;
            var findInRoutinesDialog = new FindInRoutines(false, isFindPrevious, SearchTextBackup,isShowFindDialog);
            findInRoutinesDialog.Owner = Application.Current.MainWindow;

            if (isShowFindDialog)
            {
                findInRoutinesDialog.Show(vsShell);
                _isOpen = true;
            }

            findInRoutinesDialog.Closed += FindInRoutinesDialog_Closed;
        }

        #endregion

        #region group3

        private void AddGroup3Menus(OleMenuCommandService commandService)
        {
            var menuCommandID = new CommandID(SearchCommandPackageCmdSet, NextResultCommand);
            var menuItem = new OleMenuCommand(NextResult, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(SearchCommandPackageCmdSet, PreviousResultCommand);
            menuItem = new OleMenuCommand(PreviousResult, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        private void NextResult(object sender, EventArgs e)
        {

        }

        private void PreviousResult(object sender, EventArgs e)
        {

        }

        #endregion

        #region group4

        private void AddGroup4Menus(OleMenuCommandService commandService)
        {
            var menuCommandID = new CommandID(SearchCommandPackageCmdSet, MatchingKeywordCommand);
            var menuItem = new OleMenuCommand(MatchingKeyword, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        private void MatchingKeyword(object sender, EventArgs e)
        {

        }

        #endregion

        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            var menuCommand = sender as OleMenuCommand;

            switch (menuCommand.CommandID.ID)
            {
                case FindCommand:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Find...");
                    break;
                case ReplaceCommand:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Replace...");
                    break;
                case GoToCommand:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Go To...");
                    break;
                case BrowseLogicCommand:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Goto Logic...");
                    break;
                case CrossReferenceCommand:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Cross Reference");
                    break;
                case FindNextCommand:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Find Next");
                    break;
                case FindPreviousCommand:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Find Previous");
                    break;
                case NextResultCommand:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Next Result");
                    break;
                case PreviousResultCommand:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Previous Result");
                    break;
                case MatchingKeywordCommand:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Matching Keyword");
                    break;
            }
            if (menuCommand.CommandID.ID == SearchResultCommandId)
            {
                menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Search Result");
                var window = GetPane();
                var windowFrame = (IVsWindowFrame)window.Frame;
                menuCommand.Checked = windowFrame.IsVisible() == 0;
                menuCommand.Enabled = true;
                menuCommand.Visible = true;
                return;
            }
            if (string.IsNullOrEmpty(Controller.GetInstance().ProjectLocaleName))
            {
                menuCommand.Enabled = false;
                menuCommand.Visible = true;
                return;
            }

            menuCommand.Enabled = true;
            menuCommand.Visible = true;
            if (menuCommand.CommandID.ID == NextResultCommand || menuCommand.CommandID.ID == PreviousResultCommand)
            {
                menuCommand.Enabled = false;
            }

            if (menuCommand.CommandID.ID == ReplaceCommand)
            {
                if (Controller.GetInstance().IsOnline)
                {
                    menuCommand.Enabled = false;
                }
            }
            var current = CurrentObject.GetInstance().Current;
            if (current is STEditorViewModel)
            {
                if (Controller.GetInstance().IsOnline)
                {
                    if (menuCommand.CommandID.ID == ReplaceCommand)
                    {
                        menuCommand.Enabled = false;
                    }
                }
                if (menuCommand.CommandID.ID == MatchingKeywordCommand)
                {
                    menuCommand.Enabled = true;
                    menuCommand.Visible = true;
                }

                if (menuCommand.CommandID.ID == GoToCommand)
                {
                    menuCommand.Enabled = true;
                }
            }
            else
            {
                if (menuCommand.CommandID.ID == MatchingKeywordCommand)
                {
                    menuCommand.Enabled = false;
                    menuCommand.Visible = false;
                }

                if (menuCommand.CommandID.ID == GoToCommand)
                {
                    menuCommand.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static SearchCommand Instance { get; private set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get { return this._package; }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new SearchCommand(package);
        }

        public void UpdateUI()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            IVsUIShell vsShell =
                ServiceProvider.GetService(typeof(SVsUIShell)) as IVsUIShell;

            if (vsShell != null)
            {
                int hr = vsShell.UpdateCommandUI(0);
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
            }
        }
    }

    [Guid("90480D17-9B15-47DE-BF5F-95EFB7CD5542")]
    public class SearchResultWindow : ToolWindowPane
    {
        public SearchResultWindow() : base(null)
        {
            this.Caption = LanguageManager.GetInstance().ConvertSpecifier("SearchResults");
            this.Content = new ErrorWindowControl(false);
            LanguageManager.GetInstance().LanguageChanged += OnLanguageChanged;
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            this.Caption = LanguageManager.GetInstance().ConvertSpecifier("SearchResults");
        }
    }
}
