using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ICSStudio.MultiLanguage;

namespace ICSStudio.OrganizerPackage.Commands
{
    public sealed class OrganizerCommand
    {
        private readonly Package _package;

        private OrganizerCommand(Package package)
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
                var menuCommandID =
                    new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.controllerOrganizerCommand);
                var menuItem = new OleMenuCommand(ShowControllerOrganizerToolWindow, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.logicalOrganizerCommand);
                menuItem = new OleMenuCommand(ShowLogicalOrganizerToolWindow, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        public static OrganizerCommand Instance { get; private set; }

        private IServiceProvider ServiceProvider => _package;

        public static void Initialize(Package package)
        {
            Instance = new OrganizerCommand(package);
        }

        private void ShowControllerOrganizerToolWindow(object sender, EventArgs eventArgs)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            ShowControllerOrganizer();
        }

        internal void ShowControllerOrganizer()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            ToolWindowPane window = _package.FindToolWindow(typeof(ControllerOrganizer), 0, true);
            if (window?.Frame == null)
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        public void ShowLogicalOrganizerToolWindow(object sender, EventArgs e)
        {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.


            //ToolWindowPane window = _package.FindToolWindow(typeof(LogicalOrganizer), 0, true);
            //if (window?.Frame == null)
            //{
            //    throw new NotSupportedException("Cannot create tool window");
            //}

            //ThreadHelper.ThrowIfNotOnUIThread();
            //IVsWindowFrame windowFrame = (IVsWindowFrame) window.Frame;
            //Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        private void MenuItemOnBeforeQueryStatus(object sender, EventArgs e)
        {
            var menuCommand = sender as OleMenuCommand;
            if (menuCommand != null)
            {
                switch (menuCommand.CommandID.ID)
                {
                    case PackageIds.controllerOrganizerCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Controller Organizer");
                        break;

                    case PackageIds.logicalOrganizerCommand:
                        menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Logical Organizer");
                        break;
                }
            }
        }
    }
}
