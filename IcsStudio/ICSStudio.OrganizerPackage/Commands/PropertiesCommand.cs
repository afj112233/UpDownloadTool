using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Windows;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.OrganizerPackage.Utilities;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIInterfaces.Project;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.UIInterfaces.Editor;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.OrganizerPackage.Commands
{
    internal sealed class PropertiesCommand
    {
        /// <summary>
        /// menu command, always visible
        /// </summary>
        private readonly Package _package;

        private PropertiesCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            _package = package;

            OleMenuCommandService commandService =
                ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.propertiesCommand);
                var menuItem = new OleMenuCommand(ShowPropertiesWindow, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet,
                    PackageIds.controllerPropertiesCommand);
                menuItem = new OleMenuCommand(ShowControllerPropertiesWindow, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            var selectedProjectItems = ServiceProvider.GetSelectedProjectItems();
            IProjectItem selectedProjectItem = null;
            if (selectedProjectItems.Count > 0)
                selectedProjectItem = selectedProjectItems[0];
            //var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();

            OleMenuCommand menuCommand = sender as OleMenuCommand;

            if (menuCommand != null)
            {
                if (menuCommand.CommandID.ID == PackageIds.propertiesCommand)
                {
                    menuCommand.Text = "English".Equals(LanguageInfo.CurrentLanguage) ? "Properties" : "属性";
                    menuCommand.Enabled = false;

                    if (selectedProjectItem != null)
                    {
                        //TODO(gjc): need edit here
                        if (selectedProjectItem.AssociatedObject == null)
                        {
                            menuCommand.Enabled = false;
                            return;
                        }

                        if (selectedProjectItem.Kind == ProjectItemType.ControllerTags
                            || selectedProjectItem.Kind == ProjectItemType.ProgramTags
                            || selectedProjectItem.Kind == ProjectItemType.FaultHandler
                            || selectedProjectItem.Kind == ProjectItemType.PowerHandler)
                        {
                            menuCommand.Visible = false;
                        }
                        else
                        {
                            menuCommand.Visible = true;
                        }

                        if (selectedProjectItem.Kind == ProjectItemType.ControllerModel
                            || selectedProjectItem.Kind == ProjectItemType.MotionGroup
                            || selectedProjectItem.Kind == ProjectItemType.AxisCIPDrive
                            || selectedProjectItem.Kind == ProjectItemType.AxisVirtual
                            || selectedProjectItem.Kind == ProjectItemType.Task
                            ||  selectedProjectItem.Kind == ProjectItemType.DeviceModule
                            || selectedProjectItem.Kind == ProjectItemType.Routine
                            ||   selectedProjectItem.Kind == ProjectItemType.AddOnDefined
                            || selectedProjectItem.Kind == ProjectItemType.LocalModule)
                            menuCommand.Enabled = true;

                        if (selectedProjectItem.Kind == ProjectItemType.Program
                            || selectedProjectItem.Kind == ProjectItemType.UserDefined
                            || selectedProjectItem.Kind == ProjectItemType.String
                            || selectedProjectItem.Kind == ProjectItemType.AddOnInstruction
                            || selectedProjectItem.Kind == ProjectItemType.Trend)
                        {
                            menuCommand.Enabled = EnableCommand(selectedProjectItems);
                        }

                        if (selectedProjectItem.Kind == ProjectItemType.Predefined)
                        {
                            if ((selectedProjectItem.AssociatedObject as IDataType)?.Name == "BOOL" ||
                                (selectedProjectItem.AssociatedObject as IDataType)?.Name == "DINT" ||
                                (selectedProjectItem.AssociatedObject as IDataType)?.Name == "INT" ||
                                (selectedProjectItem.AssociatedObject as IDataType)?.Name == "LINT" ||
                                (selectedProjectItem.AssociatedObject as IDataType)?.Name == "SINT" ||
                                (selectedProjectItem.AssociatedObject as IDataType)?.Name == "REAL")
                            {
                                menuCommand.Enabled = false;
                            }
                            else
                            {
                                menuCommand.Enabled = true;
                            }
                        }
                    }
                }
                else if (menuCommand.CommandID.ID == PackageIds.controllerPropertiesCommand)
                {
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Controller Properties");
                }
            }
        }

        public bool EnableCommand(List<IProjectItem> selectedProjectItems)
        {
            if (selectedProjectItems == null || selectedProjectItems.Count == 0)
                return false;

            if (selectedProjectItems[0].Kind != ProjectItemType.Program && selectedProjectItems[0].Kind != ProjectItemType.AddOnInstruction && selectedProjectItems[0].Kind != ProjectItemType.UserDefined && selectedProjectItems[0].Kind != ProjectItemType.String && selectedProjectItems[0].Kind != ProjectItemType.Trend)
                return false;

            if (selectedProjectItems.Count > 1)
                return false;

            return true;
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static PropertiesCommand Instance { get; private set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => _package;

        public static void Initialize(Package package)
        {
            Instance = new PropertiesCommand(package);
        }

        public void ShowPropertiesWindow(object sender, EventArgs e)
        {
            //TODO(gjc): add code here
            //1. get selected item
            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();

            ICreateDialogService createDialogService =
                ServiceProvider.GetService(typeof(SCreateDialogService)) as ICreateDialogService;

            var createEditorService =
                ServiceProvider?.GetService(typeof(SCreateEditorService)) as ICreateEditorService;

            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = (IVsUIShell) ServiceProvider.GetService(typeof(SVsUIShell));

            if (selectedProjectItem != null)
            {
                if (selectedProjectItem.AssociatedObject == null) return;
                //2. show properties window
                if (selectedProjectItem.Kind == ProjectItemType.ControllerModel)
                {
                    var window =
                        createDialogService?.CreateControllerProperties(
                            selectedProjectItem.AssociatedObject as IController);
                    window?.Show(uiShell);
                }
                else if (selectedProjectItem.Kind == ProjectItemType.MotionGroup)
                {
                    var window =
                        createDialogService?.CreateMotionGroupProperties(selectedProjectItem.AssociatedObject as ITag);
                    window?.Show(uiShell);
                }
                else if (selectedProjectItem.Kind == ProjectItemType.AxisCIPDrive)
                {
                    var window =
                        createDialogService?.CreateAxisCIPDriveProperties(selectedProjectItem.AssociatedObject as ITag);
                    window?.Show(uiShell);
                }
                else if (selectedProjectItem.Kind == ProjectItemType.DeviceModule)
                {
                    createEditorService?.CreateModuleProperties(selectedProjectItem.AssociatedObject as IDeviceModule);
                }
                else if (selectedProjectItem.Kind == ProjectItemType.Task)
                {
                    var window =
                        createDialogService?.CreateTaskProperties(selectedProjectItem.AssociatedObject as ITask);
                    window?.Show(uiShell);
                }
                else if (selectedProjectItem.Kind == ProjectItemType.Program)
                {
                    var window =
                        createDialogService?.CreateProgramProperties(selectedProjectItem.AssociatedObject as IProgram,false);
                    window?.Show(uiShell);
                }
                else if (selectedProjectItem.Kind == ProjectItemType.AxisVirtual)
                {
                    var window =
                        createDialogService?.CreateAxisVirtualProperties(selectedProjectItem.AssociatedObject as ITag);
                    window?.Show(uiShell);
                }
                else if (selectedProjectItem.Kind == ProjectItemType.Routine)
                {
                    var window =
                        createDialogService?.CreateRoutineProperties(selectedProjectItem.AssociatedObject as IRoutine);
                    window?.Show(uiShell);
                }
                else if (selectedProjectItem.Kind == ProjectItemType.UserDefined)
                {
                    createEditorService?.CreateNewDataType(selectedProjectItem.AssociatedObject as IDataType);
                }
                else if (selectedProjectItem.Kind == ProjectItemType.String)
                {
                    createEditorService?.CreateNewDataType(selectedProjectItem.AssociatedObject as IDataType);
                }
                else if (selectedProjectItem.Kind == ProjectItemType.Predefined)
                {
                    createEditorService?.CreateNewDataType(selectedProjectItem.AssociatedObject as IDataType);
                }
                else if (selectedProjectItem.Kind == ProjectItemType.AddOnDefined ||
                         selectedProjectItem.Kind == ProjectItemType.AddOnInstruction)
                {
                    var window =
                        createDialogService?.AddOnInstructionProperties(
                            selectedProjectItem.AssociatedObject as IAoiDefinition);
                    window?.Show(uiShell);
                }
                else if (selectedProjectItem.Kind == ProjectItemType.LocalModule)
                {
                    var localModel = selectedProjectItem.AssociatedObject as LocalModule;
                    var window =
                        createDialogService?.CreateControllerProperties(localModel?.ParentController);
                    window?.Show(uiShell);
                }
                else if (selectedProjectItem.Kind == ProjectItemType.Trend)
                {
                    var trend = selectedProjectItem.AssociatedObject as ITrend;
                    var window = createDialogService?.CreateRSTrendXProperties(trend, 0);
                    window?.ShowDialog(uiShell);
                }
            }

        }

        public void ShowControllerPropertiesWindow(object sender, EventArgs e)
        {
            var controller = Controller.GetInstance();
            if(controller == null) return;
            if(string.IsNullOrEmpty(controller.Name)) return;

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
    }
}