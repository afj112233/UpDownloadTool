using System;
using System.ComponentModel.Design;
using System.Windows;
using ICSStudio.OrganizerPackage.Utilities;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.UIInterfaces.UI;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.OrganizerPackage.Commands
{
    internal class ToolbarCommand
    {
        private readonly Package _package;

        private ToolbarCommand(Package package)
        {
            _package = package;
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            OleMenuCommandService commandService =
                ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.newInToolbarCommand);
                var menuItem = new OleMenuCommand(NewProject, menuCommandID);
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.openInToolbarCommand);
                menuItem = new OleMenuCommand(OpenProject, menuCommandID);
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.saveInToolbarCommand);
                menuItem = new OleMenuCommand(SaveProject, menuCommandID);
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.printInToolbarCommand);
                menuItem = new OleMenuCommand(PrintProject, menuCommandID);
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.cutInToolbarCommand);
                menuItem = new OleMenuCommand(Cut, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.copyInToolbarCommand);
                menuItem = new OleMenuCommand(Copy, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(PackageGuids.organizerPackageCmdSet, PackageIds.pasteInToolbarCommand);
                menuItem = new OleMenuCommand(Paste, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
                commandService.AddCommand(menuItem);

            }
        }

        public static ToolbarCommand Instance { get; private set; }

        public static void Initialize(Package package)
        {
            Instance = new ToolbarCommand(package);
        }

        private void NewProject(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            FileCommand.Instance.NewProject();
        }

        private void OpenProject(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            FileCommand.Instance.OpenProject();
        }

        private void SaveProject(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            FileCommand.Instance.SaveFile(false);
        }

        private void PrintProject(object sender, EventArgs e)
        {
            //TODO(zyl):print
        }

        private void Cut(object sender, EventArgs e)
        {
            ClipboardCommand.Instance.Cut();
        }

        private void Copy(object sender, EventArgs e)
        {
            ClipboardCommand.Instance.Copy();
        }

        private void Paste(object sender, EventArgs e)
        {
            ClipboardCommand.Instance.Paste();
        }

        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand menuCommand = (OleMenuCommand) sender;
            var selectedProjectItem = ServiceProvider.GetSelectedProjectItem();
            switch (menuCommand.CommandID.ID)
            {
                case PackageIds.cutInToolbarCommand:
                    menuCommand.Enabled = ClipboardCommand.Instance.CanCut(selectedProjectItem);
                    break;
                case PackageIds.copyInToolbarCommand:
                    menuCommand.Enabled = ClipboardCommand.Instance.CanCopy(selectedProjectItem);
                    break;
                case PackageIds.pasteInToolbarCommand:
                    menuCommand.Enabled = ClipboardCommand.Instance.CanPaste(selectedProjectItem);
                    break;
            }
        }

        private IServiceProvider ServiceProvider => _package;
    }
}
