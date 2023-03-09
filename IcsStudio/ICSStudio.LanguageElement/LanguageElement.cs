//------------------------------------------------------------------------------
// <copyright file="LanguageElement.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.Dialogs.GlobalSetting;
using ICSStudio.LanguageElement.View;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using ICSStudio.MultiLanguage;


namespace ICSStudio.LanguageElement
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class LanguageElement
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("a2a38551-588b-44b9-8cd5-e7df674fe242");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package _package;

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageElement"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private LanguageElement(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            this._package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new OleMenuCommand(this.MenuItemCallback, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
            Show();
        }
        

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static LanguageElement Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this._package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new LanguageElement(package);
        }

        private ToolWindowPane _toolWindowPane;
        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            var menu = (OleMenuCommand) sender;
            menu.Checked = !menu.Checked;

            if (_icsVsToolbar!=null)
            {
                _icsVsToolbar.Visibility = _icsVsToolbar.Visibility == Visibility.Visible
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
            else
            {
                Show();
            }

            GlobalSetting.GetInstance().ProgramInstructionVisibilitySetting = _icsVsToolbar.Visibility;

            GlobalSetting.GetInstance().SaveConfig();
        }

        private ICSVsToolbar _icsVsToolbar;
        private void Show()
        {
            var window = Application.Current.MainWindow;

            if(window==null)return;

            //var vsMenuItems = window.FindDescendants<VsMenuItem>();
            var vsToolBarHost = window.FindDescendants<VsToolBarHostControl>().FirstOrDefault();

            var topBorder = vsToolBarHost.FindDescendants<Border>().FirstOrDefault(b => b.Name == "TopDockBorder");
            var topBarTrays = topBorder.FindDescendants<VsToolBarTray>().FirstOrDefault();
            //var vsToolBarTrays = vsToolBarHost.FindDescendants<VsToolBarTray>().FirstOrDefault();
            _icsVsToolbar = new ICSVsToolbar()
            {
                Width = GlobalSetting.GetInstance().ProgramInstructionWidthSetting,
                Height = Properties.Settings.Default.StLanguageHeight,
            };

            _icsVsToolbar.Items.Add(new View.View() { Width = Properties.Settings.Default.StLanguageWidth });

            //设置Program Instruction初始时候是否可见
            _icsVsToolbar.Loaded += ProgramInstructionLoad;

            //customToolbar.Items.Add(new TextBlock() {Text = "dddddddd"});
            topBarTrays?.ToolBars.Add(_icsVsToolbar);
            //var vsToolBars = vsToolBarHost.FindDescendants<VsToolBar>();
            window.InvalidateMeasure();
        }

        private void ProgramInstructionLoad(object sender, RoutedEventArgs e)
        {
            _icsVsToolbar.Visibility = GlobalSetting.GetInstance().ProgramInstructionVisibilitySetting;
        }

        public ToolWindowPane GetPane()
        {
            if (_toolWindowPane == null)
                _toolWindowPane = this._package.FindToolWindow(typeof(LanguageElementPane), 0, true);

            return _toolWindowPane;
        }

        private void MenuItemOnBeforeQueryStatus(object sender, EventArgs e)
        {
            var menuCommand = sender as OleMenuCommand;
            if (menuCommand == null) return;
            switch (menuCommand.CommandID.ID)
            {
                case CommandId:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Program Instruction");
                    menuCommand.Checked = _icsVsToolbar?.Visibility==Visibility.Visible;
                    break;
            }
        }
    }

    [Guid("C5916DCC-8EF8-4C4E-B4E4-3B973EE43884")]
    public class LanguageElementPane : ToolWindowPane
    {
        public LanguageElementPane() : base(null)
        {
            Caption = "Program Instruction";
            Content = new View.View();
        }
    }
}
