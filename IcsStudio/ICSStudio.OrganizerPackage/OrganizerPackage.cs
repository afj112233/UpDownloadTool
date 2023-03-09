//------------------------------------------------------------------------------
// <copyright file="ControllerOrganizerPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.IO;
using EnvDTE;
using ICSStudio.Dialogs.GlobalSetting;
using ICSStudio.Gui.Controls;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.OrganizerPackage.Commands;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ICSStudio.MultiLanguage;
using ICSStudio.OrganizerPackage.View;
using ICSStudio.OrganizerPackage.ViewModel;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.ControllerOrganizer;
using Type = System.Type;

namespace ICSStudio.OrganizerPackage
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
#pragma warning disable VSSDK004 
    [ProvideAutoLoad(UIContextGuids.SolutionExists)]
#pragma warning restore VSSDK004 
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(ControllerOrganizer), Style = VsDockStyle.Tabbed, DockedWidth = 300,
        Window = "DocumentWell", Orientation = ToolWindowOrientation.Left)]
    [ProvideToolWindowVisibility(typeof(ControllerOrganizer), VSConstants.UICONTEXT.NoSolution_string)]
    [ProvideToolWindow(typeof(LogicalOrganizer))]
    [Guid(PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class OrganizerPackage : Package, SControllerOrganizerService, IControllerOrganizerService
    {
        /// <summary>
        /// ControllerOrganizerPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "f92062fc-89aa-4e53-9e85-ca55b8d4a201";

        public OrganizerPackage()
        {
            IServiceContainer serviceContainer = this;
            ServiceCreatorCallback creationCallback = CreateService;
            serviceContainer.AddService(typeof(SControllerOrganizerService),
                creationCallback, true);
        }

        private object CreateService(IServiceContainer container, Type serviceType)
        {
            if (container != this) return null;

            if (typeof(SControllerOrganizerService) == serviceType) return this;

            return null;
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            FileCommand.Initialize(this);
            OrganizerCommand.Initialize(this);
            PropertiesCommand.Initialize(this);
            ContextMenuCommand.Initialize(this);
            ClipboardCommand.Initialize(this);
            ToolbarCommand.Initialize(this);
            base.Initialize();
            
            var window = Application.Current.MainWindow;
            if (window != null)
            {
                window.Loaded += Window_Loaded;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            IVsShell shell = GetService(typeof(SVsShell)) as IVsShell;
            if (shell != null)
            {
                IVsPackage package;

                //Loading QuickWatchPackagePackage、QuickWatchPackage
                Guid packageToBeLoadedGuid = new Guid("7916416d-a7a9-4290-9f75-8c36cffe4474");
                shell.LoadPackage(ref packageToBeLoadedGuid, out package);
                Guid toolGuid = new Guid("346d2c50-4fb6-414c-b76f-f24d07a6d991");
                package.CreateTool(ref toolGuid);

                //Loading ErrorWindowPackage、ErrorWindow
                packageToBeLoadedGuid = new Guid("0a6faef5-9905-45a6-ba94-9a09f56eb33b");
                shell.LoadPackage(ref packageToBeLoadedGuid, out package);
                toolGuid = new Guid("89b6b15f-56a8-49fb-9d16-d636524b3449");
                package.CreateTool(ref toolGuid);

                //Loading ControllerOverviewPackage、ControllerOverview
                packageToBeLoadedGuid = new Guid("57cf9bea-3da0-4c24-b7a0-94a747e6abfa");
                shell.LoadPackage(ref packageToBeLoadedGuid, out package);
                toolGuid = new Guid("6eb49a1e-25b9-4e94-adac-5f8adcc57526");
                package.CreateTool(ref toolGuid);

                //Loading AboutBoxPackage
                packageToBeLoadedGuid = new Guid("babd0d66-291a-4434-b14b-926af678588f");
                shell.LoadPackage(ref packageToBeLoadedGuid, out package);

                //Loading ToolBoxPackage、ToolBox
                packageToBeLoadedGuid = new Guid("03ccbf46-786d-4ee5-b0da-e05269be6df9");
                shell.LoadPackage(ref packageToBeLoadedGuid, out package);
                toolGuid = new Guid("a6937b6b-1d17-495b-a533-de3a1c444658");
                package.CreateTool(ref toolGuid);


            }
            GlobalSetting.GetInstance().ReadConfig();
            InvalidateDefaultHotKeys();
            //HideSystemMenu();
            HideSystemToolbar();
            LanguageManager.GetInstance().LanguageChanged += SetMenuLanguage;
            LanguageManager.GetInstance().ChangeLanguage();
            if (shell != null)
            {
                IVsPackage package;

                //Loading QuickWatchPackagePackage、QuickWatchPackage
                Guid packageToBeLoadedGuid = new Guid("10e25e04-f971-4778-8978-4682a4fb3d02");
                shell.LoadPackage(ref packageToBeLoadedGuid, out package);
            }
        }

        private void InvalidateDefaultHotKeys()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
#if !DEBUG
            try
            {
                if (File.Exists(@"Extensions\Application\ResetHotkey"))
                {
                    using (File.OpenWrite(@"Extensions\Application\ResetHotkey"))
                    {
                        var globalSetting = GlobalSetting.GetInstance();
                        if (globalSetting.ShortcutSetting.IsSetting) return;
                        DTE dte = GetService(typeof(DTE)) as DTE;
                        if (dte == null) return;
                        var page = dte.Properties["Environment", "International"];
                        var isChinese = false;
                        foreach (Property property in page)
                        {
                            isChinese = (uint)property.Value == 2052;
                            break;
                        }
                        var enumerator = dte.Commands.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            var command = (Command)enumerator.Current;
                            if (command == null || string.IsNullOrEmpty(command.Name))
                            {
                                continue;
                            }

                            if (globalSetting.ShortcutSetting.ICSStudioCommandList.ContainsKey(command.Name))
                            {
                                var bind = globalSetting.ShortcutSetting.ICSStudioCommandList[command.Name];
                                command.Bindings = command.Bindings = new object[] { $"{(isChinese ? "全局" : "Global")}{bind}" }; ;
                                continue;
                            }
                            command.Bindings = new object[0];
                        }

                    }
                    File.Delete(@"Extensions\Application\ResetHotkey");
                }
            }
            catch (Exception)
            {
                //ignore
            }
#endif
#if DEBUG
            var globalSetting = GlobalSetting.GetInstance();
            if (globalSetting.ShortcutSetting.IsSetting) return;
            DTE dte = GetService(typeof(DTE)) as DTE;
            if (dte == null) return;
            var page = dte.Properties["Environment", "International"];
            var isChinese = false;
            foreach (Property property in page)
            {
                isChinese = (uint)property.Value == 2052;
                break;
            }
            var enumerator = dte.Commands.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var command = (Command)enumerator.Current;
                if (command == null || string.IsNullOrEmpty(command.Name))
                {
                    continue;
                }

                if (globalSetting.ShortcutSetting.ICSStudioCommandList.ContainsKey(command.Name))
                {
                    var bind = globalSetting.ShortcutSetting.ICSStudioCommandList[command.Name];
                    command.Bindings = command.Bindings = new object[] { $"{(isChinese ? "全局" : "Global")}{bind}" }; ;
                    continue;
                }
                command.Bindings = new object[0];
            }

            globalSetting.ShortcutSetting.IsSetting = true;
            //globalSetting.SaveConfig();
#endif
        }
        private void SetMenuLanguage(object sender, EventArgs e)
        {
            var window = Application.Current.MainWindow;
            var vsMenuItems = window.FindDescendants<VsMenuItem>().ToList();
            if (vsMenuItems?.Count() > 0)
            {
                foreach (var menuItem in vsMenuItems)
                {
                    if (menuItem.Visibility != Visibility.Collapsed)
                    {
                        var menuItems = new List<string>() { "MenuCommunications", "MenuSearch", "MenuFile", "MenuView", "MenuHelp", "MenuEdit", "MenuTools","MenuOperation" };
                        var menuHeaders = new List<List<string>>
                        { new List<string>(){ "Communications","通信" , "Communications(_C)","通信(_C)"},
                            new List<string>(){ "Search" ,"搜索", "Search(_S)", "搜索(_S)" }  ,
                            new List<string>(){"文件","File(_F)","_File","File","文件(_F)","_文件","File(F)"},
                            new List<string>(){"View","查看","View(_V)","查看(_V)"},
                            new List<string>(){"帮助","Help","Help(_H)","_Help","帮助(_H)","_帮助"},
                            new List<string>(){"Edit","编辑","Edit(_E)","编辑(_E)"},
                            new List<string>(){ "Tools" ,"工具", "Tools(_T)" ,"工具(_T)"},
                            new List<string>(){"Operation","操作", "Operation(_O)", "操作(_O)" }
                        };
                        for (int i = 0; i < 8; i++)
                        {
                            if (menuHeaders[i].Contains((string) menuItem.Header))
                                menuItem.Header = LanguageManager.GetInstance().ConvertSpecifier(menuItems[i]);
                        }
                    }
                }
            }
        }

        private void HideSystemMenu()
        {
            var window = Application.Current.MainWindow;
            var vsMenuItems = window.FindDescendants<VsMenuItem>().ToList();
            var hideMenuIndexList = new List<string>() { "窗口(_W)", "工具(_T)", "调试(_D)", "视图(_V)", "编辑(_E)", "_Edit","_View","_Debug","_Tools","_Window"};
            var exist=new List<string>();
            //int index = 0;
            if (vsMenuItems?.Count() > 0)
            {
                foreach (var menuItem in vsMenuItems)
                {
                    var title = (string) menuItem.Header;
                    if (hideMenuIndexList.Contains(title))
                    {
                        menuItem.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        if (exist.Contains(title))
                        {
                            menuItem.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            exist.Add(title);
                        }
                    }
                    //index++;
                }
            }
        }

        private void HideSystemToolbar()
        {
            var window = Application.Current.MainWindow;
            var vsToolBars = window.FindDescendants<VsToolBar>();
            int index = 0;
            foreach (var vsToolBar in vsToolBars)
            {
                if (index!=2)
                {
                    vsToolBar.Visibility = Visibility.Collapsed;
                }
                index ++;
            }
        }

        #endregion


        private ToolWindowPane GetToolWindowPane()
        {
            return FindToolWindow(typeof(ControllerOrganizer), 0, true);
        }

        public void RungsExport(RLLRoutine rllRoutine, int startIndex, int endIndex)
        {
            ContextMenuCommand.Instance.RungsExport(rllRoutine,startIndex,endIndex);
        }

        public void SelectOrganizerItem(IBaseObject baseObject)
        {
            if(baseObject == null)  return;
            var view = GetToolWindowPane()?.Content as ControllerOrganizerView;
            if(view == null) return;
            var vm = view.DataContext as ControllerOrganizerVM;
            if (vm == null) return;

            var routine = baseObject as IRoutine;
            if (routine != null)
            {
                
                var routineOrganizerItem = vm.GetRoutineItem(routine);
                var routineTreeViewItem = TreeViewExtensions.GetTreeViewItem(view.ControllerOrganizerTreeView, routineOrganizerItem);
                TreeViewExtensions.MakeSingleSelection(view.ControllerOrganizerTreeView, routineTreeViewItem);

                return;
            }

            var program = baseObject as IProgram;
            if (program != null)
            {
                var programOrganizerItem = vm.GetProgramItem(program);
                var programTreeViewItem =
                    TreeViewExtensions.GetTreeViewItem(view.ControllerOrganizerTreeView, programOrganizerItem);
                TreeViewExtensions.MakeSingleSelection(view.ControllerOrganizerTreeView,programTreeViewItem);

                return;
            }

            var task = baseObject as ITask;
            if (task != null)
            {
                var taskOrganizerItem = vm.GetTaskItem(task);
                var taskTreeViewItem =
                    TreeViewExtensions.GetTreeViewItem(view.ControllerOrganizerTreeView, taskOrganizerItem);
                TreeViewExtensions.MakeSingleSelection(view.ControllerOrganizerTreeView, taskTreeViewItem);

                return;
            }

            var aoi = baseObject as IAoiDefinition;
            if (aoi != null)
            {
                var aoiOrganizerItem = vm.GetAddOnInstructionItem(aoi);
                var aoiTreeViewItem =
                    TreeViewExtensions.GetTreeViewItem(view.ControllerOrganizerTreeView, aoiOrganizerItem);
                TreeViewExtensions.MakeSingleSelection(view.ControllerOrganizerTreeView, aoiTreeViewItem);

                return;
            }

            //var axis = baseObject as ITag;
            //if (axis != null)
            //{
            //    var axisOrganizerItem = vm.GetAxisItem(axis);
            //    var axisTreeViewItem =
            //        TreeViewExtensions.GetTreeViewItem(view.ControllerOrganizerTreeView, axisOrganizerItem);
            //    TreeViewExtensions.MakeSingleSelection(view.ControllerOrganizerTreeView, axisTreeViewItem);

            //    return;
            //}


            //TODO(XWQ): other component, such as task,aoi,module and so on ...
        }

        public void ShowControllerOrganizerToolWindow()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            OrganizerCommand.Instance.ShowControllerOrganizer();
        }
    }
}
