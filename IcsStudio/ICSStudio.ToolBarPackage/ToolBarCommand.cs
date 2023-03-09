//------------------------------------------------------------------------------
// <copyright file="ToolBarCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ICSStudio.EditorPackage;
using ICSStudio.Interfaces.Common;
using ICSStudio.OrganizerPackage.Utilities;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.ControllerOrganizer;
using ICSStudio.UIInterfaces.Editor;
using ICSStudio.UIInterfaces.Error;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.UIInterfaces.Search;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.ToolBarPackage
{
    /// <summary>
    ///     Command handler
    /// </summary>
    internal sealed class ToolBarCommand
    {
        /// <summary>
        ///     Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid ToolBarCommandSet = new Guid("7580301c-20b4-4d2b-9218-4e2cd84909e3");
        /// <summary>
        ///     VS Package that provides this command, not null.
        /// </summary>
        private readonly Package _package;

        private readonly List<string> _searchTextList = new List<string>();

        private string[] _searchTextArray = { };

        private string _currentSearchText = string.Empty;

        public const int MaxListCount = 10;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ToolBarCommand" /> class.
        ///     Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private ToolBarCommand(Package package)
        {
            if (package == null) throw new ArgumentNullException(nameof(package));

            _package = package;

            var commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            if (commandService == null) return;

            var menuCommandID = new CommandID(ToolBarCommandSet, SearchBoxInToolbarCommand);
            var menuItem = new OleMenuCommand(SearchText, menuCommandID);
             menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(ToolBarCommandSet, SearchTextListID);
            menuItem = new OleMenuCommand(DisplaySearchTextList, menuCommandID);
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(ToolBarCommandSet, FindPreviousInToolbarCommand);
            menuItem = new OleMenuCommand(FindPrevious, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(ToolBarCommandSet, FindNextInToolbarCommand);
            menuItem = new OleMenuCommand(FindNext, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(ToolBarCommandSet, FindAllInToolbarCommand);
            menuItem = new OleMenuCommand(FindAll, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(ToolBarCommandSet, ToggleControllerInToolbarCommand);
            menuItem = new OleMenuCommand(ToggleController, menuCommandID);
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(ToolBarCommandSet, ToggleLogicalInToolbarCommand);
            menuItem = new OleMenuCommand(ToggleLogical, menuCommandID);
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(ToolBarCommandSet, VerifyRoutineInToolbarCommand);
            menuItem = new OleMenuCommand(VerifyRoutine, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(ToolBarCommandSet, VerifyControllerInToolbarCommand);
            menuItem = new OleMenuCommand(VerifyController, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(ToolBarCommandSet, BuildControllerInToolbarCommand);
            menuItem = new OleMenuCommand(BuildController, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        ///     Gets the instance of the command.
        /// </summary>
        public static ToolBarCommand Instance { get; private set; }

        /// <summary>
        ///     Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => _package;


        /// <summary>
        ///     This function is the callback used to execute the command when the menu item is clicked.
        ///     See the constructor to see how the menu item is associated with this function using
        ///     OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        /// 

        private void DisplaySearchTextList(object sender, EventArgs e)
        {
            var eventArgs = e as OleMenuCmdEventArgs;
            if (eventArgs == null) return;

            var inValue = eventArgs.InValue as string;
            if (!string.IsNullOrEmpty(inValue))
            {
                Debug.WriteLine("Resources inParam illegal");
                return;
            }

            var outValue = eventArgs.OutValue;

            if (outValue != IntPtr.Zero)
            {
                Marshal.GetNativeVariantForObject(_searchTextArray, outValue);
            }

            else
                Debug.WriteLine("Resources outParam required");
        }

        private void SearchText(object sender, EventArgs e)
        {
            // Note: Marshal works only for dynamic- and dropdown- combos

            var eventArgs = e as OleMenuCmdEventArgs;

            if (eventArgs != null)
            {
                var inValue = eventArgs.InValue as string;
                var outValue = eventArgs.OutValue;

                if (outValue != IntPtr.Zero)
                {
                    // when outValue is non-NULL, the IDE is requesting the current value for the combo
                    Marshal.GetNativeVariantForObject(_currentSearchText, outValue);
                }

                else if (inValue != null)
                {
                    // new value was selected or typed in,see if it is one of _searchTextList
                    if (!_searchTextList.Contains(inValue))
                    {
                        AddSearchTextToList(inValue);
                    }

                    _currentSearchText = inValue;

                    var service = Package.GetGlobalService(typeof(SSearchResultService)) as ISearchResultService;
                    service?.FindSpecifiedText(inValue);
                }
            }
            else
            {
                Debug.WriteLine("EventArgs error!");
            }

        }

        private void AddSearchTextToList(string inValue)
        {

            if (_searchTextList.Count == MaxListCount)
            {
                _searchTextList.RemoveAt(MaxListCount - 1);
            }

            _searchTextList.Insert(0,inValue);

            _searchTextArray = _searchTextList.ToArray();
        }

        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {

            var menuCommand = sender as OleMenuCommand;

            if (menuCommand == null) return;

            menuCommand.Enabled = true;
            menuCommand.Visible = true;

            //Find specified text、Find Previous、Find Next、Find All、
            if (string.IsNullOrEmpty(Controller.GetInstance().ProjectLocaleName))
            {
                menuCommand.Enabled = false;
                menuCommand.Visible = true;
            }

            switch (menuCommand.CommandID.ID)
            {
                //Verify Routine
                case VerifyRoutineInToolbarCommand:
                {
                    //选中树状列表的routine节点
                    var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();
                    var routine = selectedProjectItem?.AssociatedObject as IRoutine;

                    //选中st/rll的routine的区域
                    var stPane = EditorWindowManager.stPane;

                    var rllPane = EditorWindowManager.rllPane;

                    if ((routine != null) || (stPane != null) || (rllPane != null))
                        menuCommand.Enabled = true;

                    else menuCommand.Enabled = false;

                    break;
                }

                //TODO(pyc): 目前只匹配了Verify Routine按钮
            }
        }

        /// <summary>
        ///     Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new ToolBarCommand(package);
        }

        private void FindPrevious(object sender, EventArgs e)
        {
            var service = Package.GetGlobalService(typeof(SSearchResultService)) as ISearchResultService;
            service?.FindPrevious();
        }

        private void FindNext(object sender, EventArgs e)
        {
            var service = Package.GetGlobalService(typeof(SSearchResultService)) as ISearchResultService;
            service?.FindNext();
        }

        private void FindAll(object sender, EventArgs e)
        {
            var service = Package.GetGlobalService(typeof(SSearchResultService)) as ISearchResultService;
            service?.FindAll();
        }

        private void ToggleController(object sender, EventArgs e)
        {
            var service = Package.GetGlobalService(typeof(SControllerOrganizerService)) as IControllerOrganizerService;
            service?.ShowControllerOrganizerToolWindow();
        }

        private void ToggleLogical(object sender, EventArgs e)
        {
            //TODO(pyc):add Toggle Logical快捷按钮
        }

        private void VerifyRoutine(object sender, EventArgs e)
        {
            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();

            var routine = selectedProjectItem?.AssociatedObject as IRoutine;

            var stRoutine = EditorWindowManager.stPane?.Routine;

            var rllRoutine = EditorWindowManager.rllPane?.Routine;

            if ((routine == null) && (stRoutine == null) && (rllRoutine == null)) return;

            if (routine != null) VerifyRoutineManager(routine);

            if (stRoutine != null) VerifyRoutineManager(stRoutine);

            if (rllRoutine != null) VerifyRoutineManager(rllRoutine);

        }

        private void VerifyRoutineManager(IRoutine routine)
        {
            var outputService = Package.GetGlobalService(typeof(SErrorOutputService)) as IErrorOutputService;
            outputService?.Cleanup();

            var createEditorService = Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
            createEditorService?.ParseRoutine(routine, true, true);

            outputService?.Summary();
        }

        private void VerifyController(object sender, EventArgs e)
        {
            IProjectInfoService projectInfoService =
                ServiceProvider.GetService(typeof(SProjectInfoService)) as IProjectInfoService;
            projectInfoService?.VerifyInDialog();
        }

        private void BuildController(object sender, EventArgs e)
        {
            //TODO(pyc):add Build Controller快捷按钮
        }

        /// <summary>
        ///     Command ID.
        /// </summary>

        #region ToolBarCommand
        public const int SearchTextListID = 0x2008;
        public const int SearchBoxInToolbarCommand = 0x2009;
        public const int FindPreviousInToolbarCommand = 0x2010;
        public const int FindNextInToolbarCommand = 0x2011;
        public const int FindAllInToolbarCommand = 0x2012;
        public const int ToggleControllerInToolbarCommand = 0x2013;
        public const int ToggleLogicalInToolbarCommand = 0x2014;
        public const int VerifyRoutineInToolbarCommand = 0x2015;
        public const int VerifyControllerInToolbarCommand = 0x2016;
        public const int BuildControllerInToolbarCommand = 0x2017;
        #endregion
    }
}