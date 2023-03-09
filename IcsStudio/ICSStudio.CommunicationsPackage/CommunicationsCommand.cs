//------------------------------------------------------------------------------
// <copyright file="CommunicationsCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows;
using ICSStudio.CommunicationsPackage.View;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Command;
using ICSStudio.UIInterfaces.Project;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NLog;
using Application = System.Windows.Application;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Online;
using ICSStudio.UIInterfaces.Dialog;

namespace ICSStudio.CommunicationsPackage
{
    /// <summary>
    ///     Command handler
    /// </summary>
    internal sealed class CommunicationsCommand
    {
        internal static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     VS Package that provides this command, not null.
        /// </summary>
        private readonly Package _package;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommunicationsCommand" /> class.
        ///     Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private CommunicationsCommand(Package package)
        {
            if (package == null) throw new ArgumentNullException(nameof(package));

            _package = package;

            var commandService =
                ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            if (commandService != null)
            {
                AddGroup0Menus(commandService);
                AddGroup1Menus(commandService);
                AddGroup2Menus(commandService);
                AddGroup3Menus(commandService);
                AddGroup4Menus(commandService);
            }

            var controller = Controller.GetInstance();
            WeakEventManager<Controller, EventArgs>.AddHandler(
                controller, "KeySwitchChanged", OnKeySwitchChanged);
            WeakEventManager<Controller, EventArgs>.AddHandler(
                controller, "OperationModeChanged", OnOperationModeChanged);
        }

        private void OnOperationModeChanged(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                UpdateUI();
            });
        }

        private void OnKeySwitchChanged(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                UpdateUI();
            });
        }

        /// <summary>
        ///     Gets the instance of the command.
        /// </summary>
        public static CommunicationsCommand Instance { get; private set; }

        /// <summary>
        ///     Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => _package;

        /// <summary>
        ///     Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new CommunicationsCommand(package);
        }

        private void MenuItemOnBeforeQueryStatus(object sender, EventArgs e)
        {
            var menuCommand = sender as OleMenuCommand;
            var projectInfoService =
                ServiceProvider?.GetService(typeof(SProjectInfoService)) as IProjectInfoService;
            var controller = projectInfoService?.Controller;

            if (menuCommand != null)
                switch (menuCommand.CommandID.ID)
                {
                    case PackageIds.whoActiveCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Net Path");
                        break;
                    case PackageIds.selectRecentPathCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Select Recent Path...");
                        break;
                    case PackageIds.goOnlineCommand:
                    {
                        if (controller != null && controller.IsOnline)
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Logout");
                        else
                            menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Login");
                    }
                        break;
                    case PackageIds.uploadCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Upload");
                        if (controller != null && controller.IsOnline)
                            menuCommand.Enabled = false;
                        else
                            menuCommand.Enabled = true;
                        break;

                    case PackageIds.downloadCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Download");
                        if (controller == null)
                            menuCommand.Enabled = false;
                        else
                        {
                            menuCommand.Enabled = !controller.IsOnline;
                        }

                        break;


                    case PackageIds.programModeCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Program Mode");
                        if (controller != null && controller.IsOnline)
                        {
                            if (controller.KeySwitchPosition == ControllerKeySwitch.RemoteKeySwitch
                                && (controller.OperationMode == ControllerOperationMode.OperationModeRun ||
                                    controller.OperationMode == ControllerOperationMode.OperationModeDebug))
                            {
                                menuCommand.Enabled = true;
                                break;
                            }
                        }

                        menuCommand.Enabled = false;
                        break;

                    case PackageIds.runModeCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Run Mode");
                        if (controller != null && controller.IsOnline)
                        {
                            if (controller.KeySwitchPosition == ControllerKeySwitch.RemoteKeySwitch
                                && (controller.OperationMode == ControllerOperationMode.OperationModeProgram ||
                                    controller.OperationMode == ControllerOperationMode.OperationModeDebug))
                            {
                                menuCommand.Enabled = true;
                                break;
                            }
                        }

                        menuCommand.Enabled = false;
                        break;
                    case PackageIds.testModeCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Test Mode");
                        if (controller != null && controller.IsOnline)
                        {
                            if (controller.KeySwitchPosition == ControllerKeySwitch.RemoteKeySwitch
                                && (controller.OperationMode == ControllerOperationMode.OperationModeProgram ||
                                    controller.OperationMode == ControllerOperationMode.OperationModeRun))
                            {
                                //Test Mode模式目前还没有开发，暂时禁用Communications菜单下的Test Mode
                                menuCommand.Enabled = false;
                                break;
                            }
                        }

                        menuCommand.Enabled = false;
                        break;

                    case PackageIds.lockControllerCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Lock Controller");
                        menuCommand.Enabled = false;
                        break;

                    case PackageIds.saveToControllerCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Save To Controller");
                        if (controller != null && controller.IsOnline)
                        {
                            if (OnlineEditHelper.CompilingPrograms.Any())
                            {
                                menuCommand.Enabled = false;
                            }
                            else
                            {
                                menuCommand.Enabled = true;
                            }
                        }
                        else
                        {
                            menuCommand.Enabled = false;
                        }

                        return;


                    case PackageIds.clearFaultsCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Clear Faults");
                        if (controller != null && controller.IsOnline)
                        {
                            if (controller.OperationMode == ControllerOperationMode.OperationModeFaulted)
                            {
                                menuCommand.Enabled = true;
                                break;
                            }
                        }

                        menuCommand.Enabled = false;
                        break;
                    case PackageIds.gotoFaultsCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Go To Faults");
                        menuCommand.Enabled = false;
                        break;
                    case PackageIds.controllerPropertiesCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Controller Properties");
                        break;
                }
        }

        #region Group0

        private void AddGroup0Menus(OleMenuCommandService commandService)
        {
            var menuCommandID = new CommandID(PackageGuids.communicationsCommandPackageCmdSet,
                PackageIds.whoActiveCommand);
            var menuItem = new OleMenuCommand(WhoActive, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.communicationsCommandPackageCmdSet,
                PackageIds.selectRecentPathCommand);
            menuItem = new OleMenuCommand(SelectRecentPath, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.communicationsCommandPackageCmdSet,
                PackageIds.selectSoftwareCommand);
            menuItem = new OleMenuCommand(SelectSoftware, menuCommandID);
            commandService.AddCommand(menuItem);

        }

        private void SelectSoftware(object sender, EventArgs e)
        {
            //TODO(gjc): add code here
        }

        private void SelectRecentPath(object sender, EventArgs e)
        {
            //TODO(gjc): add code here
        }

        private void WhoActive(object sender, EventArgs e)
        {
            var dialog = new WhoActiveDialog
            {
                Owner = Application.Current.MainWindow
            };

            var result = dialog.ShowDialog();

            if (result.HasValue)
            {
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    if (!mainWindow.IsActive)
                        mainWindow.Activate();
                }
            }
        }

        #endregion

        #region Group1

        private void AddGroup1Menus(OleMenuCommandService commandService)
        {
            var menuCommandID = new CommandID(PackageGuids.communicationsCommandPackageCmdSet,
                PackageIds.goOnlineCommand);
            var menuItem = new OleMenuCommand(GoOnline, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.communicationsCommandPackageCmdSet,
                PackageIds.uploadCommand);
            menuItem = new OleMenuCommand(Upload, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.communicationsCommandPackageCmdSet,
                PackageIds.downloadCommand);
            menuItem = new OleMenuCommand(Download, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        private void GoOnline(object sender, EventArgs e)
        {
            var projectInfoService =
                ServiceProvider?.GetService(typeof(SProjectInfoService)) as IProjectInfoService;
            var controller = projectInfoService?.Controller;
            var currentProject = projectInfoService?.CurrentProject;

            if (controller != null && currentProject != null && !string.IsNullOrEmpty(currentProject.RecentCommPath))
            {
                var commandService =
                    ServiceProvider.GetService(typeof(SCommandService)) as ICommandService;

                commandService?.GoOnlineOrOffline(controller, currentProject.RecentCommPath);
            }
            else
            {
                var dialog = new WhoActiveDialog
                {
                    Owner = Application.Current.MainWindow
                };

                var result = dialog.ShowDialog();

                if (result.HasValue)
                {
                    var mainWindow = Application.Current.MainWindow;
                    if (mainWindow != null)
                    {
                        if (!mainWindow.IsActive)
                            mainWindow.Activate();
                    }
                }
            }

        }

        private void Download(object sender, EventArgs e)
        {
            var controller = Controller.GetInstance();

            var commandService =
                ServiceProvider.GetService(typeof(SCommandService)) as ICommandService;

            var projectInfoService =
                ServiceProvider?.GetService(typeof(SProjectInfoService)) as IProjectInfoService;
            var currentProject = projectInfoService?.CurrentProject;

            string commPath = string.Empty;
            if (currentProject != null)
                commPath = currentProject.RecentCommPath;

            commandService?.Download(controller, commPath);
        }

        private void Upload(object sender, EventArgs e)
        {
            var controller = Controller.GetInstance();

            var commandService =
                ServiceProvider.GetService(typeof(SCommandService)) as ICommandService;

            var projectInfoService =
                ServiceProvider?.GetService(typeof(SProjectInfoService)) as IProjectInfoService;
            var currentProject = projectInfoService?.CurrentProject;

            string commPath = string.Empty;
            if (currentProject != null)
                commPath = currentProject.RecentCommPath;

            commandService?.Upload(controller, commPath);
        }

        #endregion

        #region Group2

        private void AddGroup2Menus(OleMenuCommandService commandService)
        {
            var menuCommandID = new CommandID(PackageGuids.communicationsCommandPackageCmdSet,
                PackageIds.programModeCommand);
            var menuItem = new OleMenuCommand(ProgramMode, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.communicationsCommandPackageCmdSet,
                PackageIds.runModeCommand);
            menuItem = new OleMenuCommand(RunMode, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.communicationsCommandPackageCmdSet,
                PackageIds.testModeCommand);
            menuItem = new OleMenuCommand(TestMode, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        private void TestMode(object sender, EventArgs e)
        {
            var controller = Controller.GetInstance();

            var commandService =
                ServiceProvider.GetService(typeof(SCommandService)) as ICommandService;

            commandService?.ChangeOperationMode(controller, ControllerOperationMode.OperationModeDebug);
        }

        private void RunMode(object sender, EventArgs e)
        {
            var controller = Controller.GetInstance();

            var commandService =
                ServiceProvider.GetService(typeof(SCommandService)) as ICommandService;

            commandService?.ChangeOperationMode(controller, ControllerOperationMode.OperationModeRun);
        }

        private void ProgramMode(object sender, EventArgs e)
        {
            var controller = Controller.GetInstance();

            var commandService =
                ServiceProvider.GetService(typeof(SCommandService)) as ICommandService;

            commandService?.ChangeOperationMode(controller, ControllerOperationMode.OperationModeProgram);
        }

        #endregion

        #region Group3

        private void AddGroup3Menus(OleMenuCommandService commandService)
        {
            var menuCommandID = new CommandID(PackageGuids.communicationsCommandPackageCmdSet,
                PackageIds.lockControllerCommand);
            var menuItem = new OleMenuCommand(LockController, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.communicationsCommandPackageCmdSet,
                PackageIds.saveToControllerCommand);
            menuItem = new OleMenuCommand(SaveToController, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);

        }

        private void LockController(object sender, EventArgs e)
        {
            //TODO(gjc): add code here
        }

        private void SaveToController(object sender, EventArgs e)
        {
            var controller = Controller.GetInstance();

            var commandService =
                ServiceProvider.GetService(typeof(SCommandService)) as ICommandService;

            commandService?.SaveToController(controller);
        }

        #endregion

        #region Group4

        private void AddGroup4Menus(OleMenuCommandService commandService)
        {
            var menuCommandID = new CommandID(PackageGuids.communicationsCommandPackageCmdSet,
                PackageIds.clearFaultsCommand);
            var menuItem = new OleMenuCommand(ClearFaults, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.communicationsCommandPackageCmdSet,
                PackageIds.gotoFaultsCommand);
            menuItem = new OleMenuCommand(GotoFaults, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);

            menuCommandID = new CommandID(PackageGuids.communicationsCommandPackageCmdSet,
                PackageIds.controllerPropertiesCommand);
            menuItem = new OleMenuCommand(ShowControllerPropertiesWindow, menuCommandID);
            menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        private void GotoFaults(object sender, EventArgs e)
        {
            //TODO(gjc): add code here
        }

        private void ClearFaults(object sender, EventArgs e)
        {
            var controller = Controller.GetInstance();

            var commandService =
                ServiceProvider.GetService(typeof(SCommandService)) as ICommandService;

            commandService?.ClearFaults(controller);
        }

        public void ShowControllerPropertiesWindow(object sender, EventArgs e)
        {
            var controller = Controller.GetInstance();
            if (controller == null) return;
            if (string.IsNullOrEmpty(controller.Name)) return;

            ThreadHelper.ThrowIfNotOnUIThread();


            ICreateDialogService createDialogService =
                ServiceProvider.GetService(typeof(SCreateDialogService)) as ICreateDialogService;

            var window =
                createDialogService?.CreateControllerProperties(controller);
            var uiShell = (IVsUIShell)ServiceProvider.GetService(typeof(SVsUIShell));
            window?.Show(uiShell);
            //TODO(gjc): add code here
            //1. get controller
            //2. show properties window
        }

        #endregion

        private void UpdateUI()
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
}