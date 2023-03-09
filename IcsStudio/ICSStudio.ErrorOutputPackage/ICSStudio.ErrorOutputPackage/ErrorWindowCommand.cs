//------------------------------------------------------------------------------
// <copyright file="ErrorWindowCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ICSStudio.MultiLanguage;

namespace ICSStudio.ErrorOutputPackage
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ErrorWindowCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        public const int ErrorWindowCommandId = 0x0101;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("3b04cc28-161c-495e-bf17-7cc61de6c80b");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package _package;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorWindowCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private ErrorWindowCommand(Package package)
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
                var menuCommandID = new CommandID(CommandSet, ErrorWindowCommandId);
                var menuItem = new OleMenuCommand(this.ShowToolWindow, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ErrorWindowCommand Instance { get; private set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => _package;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new ErrorWindowCommand(package);
        }

        private ToolWindowPane _toolWindowPane;

        public ToolWindowPane GetPane()
        {
            return _toolWindowPane ?? (_toolWindowPane = _package.FindToolWindow(typeof(ErrorWindow), 0, true));
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
            ToolWindowPane window = GetPane();
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
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
                case ErrorWindowCommandId:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Errors");
                    var window = GetPane();
                    var windowFrame = (IVsWindowFrame)window.Frame;
                    menuCommand.Checked = windowFrame.IsVisible() == 0;
                    break;
            }
        }
    }
}
