//------------------------------------------------------------------------------
// <copyright file="QuickWatchPackageCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using ICSStudio.MultiLanguage;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ICSStudio.QuickWatchPackage
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class QuickWatchPackageCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        public const int QuickWatchPackageCommandId = 0x0201;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("9d8d5e73-bbe8-45a8-8949-9673476a7f53");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickWatchPackageCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private QuickWatchPackageCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService =
                this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, QuickWatchPackageCommandId);
                var menuItem = new OleMenuCommand(this.ShowToolWindow, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static QuickWatchPackageCommand Instance { get; private set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get { return this.package; }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new QuickWatchPackageCommand(package);
        }

        /// <summary>
        /// Shows the tool _window when the menu item is clicked.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.<  private ToolWindowPane _window;
        private void ShowToolWindow(object sender, EventArgs e)
        {
            // Get the instance number 0 of this tool _window. This _window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool _window does not exists it will be created.
            var window = this.package.FindToolWindow(typeof(QuickWatchPackage), 0, true);
            _toolWindowPane = window;
            if (window?.Frame == null)
            {
                throw new NotSupportedException("Cannot create tool _window");
            }
            IVsWindowFrame windowFrame = (IVsWindowFrame) window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());

            var menuitem = sender as MenuCommand;
            if (menuitem == null) return;
            if (!menuitem.Checked) return;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Hide());
        }

        private ToolWindowPane _toolWindowPane = null;

        public ToolWindowPane GetPane()
        {
            if (_toolWindowPane == null)
                _toolWindowPane = this.package.FindToolWindow(typeof(QuickWatchPackage), 0, true);

            return _toolWindowPane;
        }

        public void Show(object sender, EventArgs e)
        {
            ShowToolWindow(sender, e);
        }
        private void MenuItemOnBeforeQueryStatus(object sender, EventArgs e)
        {
            var menuCommand = sender as OleMenuCommand;
            if (menuCommand == null) return;
            switch (menuCommand.CommandID.ID)
            {
                case QuickWatchPackageCommandId:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Quick Monitor");
                    var window = GetPane();
                    var windowFrame = (IVsWindowFrame)window.Frame;
                    menuCommand.Checked = windowFrame.IsVisible() == 0;
                    break;
            }
        }

    }
}
