//------------------------------------------------------------------------------
// <copyright file="ControllerOverviewCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ICSStudio.MultiLanguage;

namespace ICSStudio.ControllerOverviewPackage
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ControllerOverviewCommand
    {
        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package _package;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerOverviewCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private ControllerOverviewCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            this._package = package;

            OleMenuCommandService commandService =
                this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(PackageGuids.guidControllerOverviewPackageCmdSet,
                    PackageIds.ControllerOverviewCommandId);
                var menuItem = new OleMenuCommand(this.ShowToolWindow, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ControllerOverviewCommand Instance { get; private set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => this._package;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new ControllerOverviewCommand(package);
        }

        /// <summary>
        /// Shows the tool window when the menu item is clicked.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void ShowToolWindow(object sender, EventArgs e)
        {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            ToolWindowPane window = this._package.FindToolWindow(typeof(ControllerOverview), 0, true);
            if (window?.Frame == null)
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame) window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());

            var menuitem = sender as MenuCommand;
            if (menuitem == null) return;
            if (!menuitem.Checked) return;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Hide());
        }

        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            var menuCommand = sender as OleMenuCommand;

            switch (menuCommand.CommandID.ID)
            {
                case PackageIds.ControllerOverviewCommandId:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Controller Overview");
                    var window = this._package.FindToolWindow(typeof(ControllerOverview), 0, true);
                    var windowFrame = (IVsWindowFrame)window.Frame;
                    menuCommand.Checked = windowFrame.IsVisible() == 0;
                    break;
            }
        }
    }
}
