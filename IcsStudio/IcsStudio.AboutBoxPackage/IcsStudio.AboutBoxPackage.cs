using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;
using ICSStudio.MultiLanguage;
using NLog;

namespace ICSStudio.AboutBoxPackage
{
    /// <summary>
    ///     This is the class that implements the package exposed by this assembly.
    ///     The minimum requirement for a class to be considered a valid package for Visual Studio
    ///     is to implement the IVsPackage interface and register itself with the shell.
    ///     This package uses the helper classes defined inside the Managed Package Framework (MPF)
    ///     to do it: it derives from the Package class that provides the implementation of the
    ///     IVsPackage interface and uses the registration attributes defined in the framework to
    ///     register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.GuidAboutBoxPackagePkgString)]
    public sealed class AboutBoxPackage : AsyncPackage
    {
        internal static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Default constructor of the package.
        ///     Inside this method you can place any initialization code that does not require
        ///     any Visual Studio service because at this point the package object is created but
        ///     not sited yet inside Visual Studio environment. The place to do all the other
        ///     initialization is the Initialize method.
        /// </summary>
        public AboutBoxPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", ToString()));
        }

        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation

        #region Package Members

        protected override async Task InitializeAsync(CancellationToken cancellationToken,
            IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);

            Logger.Info($"Version - {Assembly.GetExecutingAssembly().GetName().Version}");

            // Add our command handlers for menu (commands must exist in the .vsct file)
            var mcs = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                // Create the command for the menu item.
                var menuCommandID =
                    new CommandID(GuidList.GuidAboutBoxPackageCmdSet, (int)PkgCmdIDList.CmdidHelpAbout);
                var menuItem = new OleMenuCommand(MenuItemCallback, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                mcs.AddCommand(menuItem);

                menuCommandID = new CommandID(GuidList.GuidAboutBoxPackageCmdSet, PkgCmdIDList.CmdidMultiLan1Command);
                menuItem = new OleMenuCommand(MultiLanCallback, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                mcs.AddCommand(menuItem);

                menuCommandID = new CommandID(GuidList.GuidAboutBoxPackageCmdSet, PkgCmdIDList.CmdidMultiLan2Command);
                menuItem = new OleMenuCommand(MultiLanCallback, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                mcs.AddCommand(menuItem);

                menuCommandID = new CommandID(GuidList.GuidAboutBoxPackageCmdSet, PkgCmdIDList.MultiLanguageMenu);
                menuItem = new OleMenuCommand(Multi, menuCommandID);
                menuItem.BeforeQueryStatus += MenuItemOnBeforeQueryStatus;
                mcs.AddCommand(menuItem);
            }

            // When initialized asynchronously, we *may* be on a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            // Otherwise, remove the switch to the UI thread if you don't need it.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        }

        #endregion

        /// <summary>
        ///     This function is the callback used to execute a command when the a menu item is clicked.
        ///     See the Initialize method to see how the menu item is associated to this function using
        ///     the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            var aboutBox = new AboutBox();
            aboutBox.ShowModal();
        }

        private void MultiLanCallback(object sender, EventArgs e)
        {
            MenuCommand cmd = sender as MenuCommand;
            if (cmd == null)
                return;

            var commandID = cmd.CommandID.ID;
            string selectedLan = "English";

            if (commandID == PkgCmdIDList.CmdidMultiLan2Command)
                selectedLan = "Chinese";

            if (string.CompareOrdinal(LanguageInfo.CurrentLanguage, selectedLan) != 0)
            {
                LanguageInfo.CurrentLanguage = selectedLan;
                LanguageManager.GetInstance().ChangeLanguage();
            }
            //    //初始化多语言资源
            //    //LanguageManager.Init();

            //    //保存当前语言
            //    bool isSuccess = LanguageInfo.SaveCurrentLanguage(selectedLan);
            //    if (!isSuccess)
            //    {
            //        System.Windows.MessageBox.Show(LanguageManager.GetTargetText("多语言设置"), LanguageManager.GetTargetText("多语言设置保存失败"));
            //        return;
            //    }

            //    //更新当前语言
            //    LanguageInfo.CurrentLanguage = selectedLan;
            //    LanguageManager.GetLanguageDetail();//这里需要获取多语言资源

            //    string msg = "I-Con";
            //    //string tmsg = LanguageManager.GetTargetText(msg);

            //    //string str = $"test{msg}";
            //    string tmsg = string.Format(LanguageManager.GetTargetText("test {0}"), msg);

            //    /*            //语言切换
            //    LanguageManager.GetLanguageDetail();
            //    //LanguageManager.LanguageConverter(this);

            //    //翻译页面
            //    //Visual myVisual = ((MainWindow)Application.Current.MainWindow).mainSettingUC;
            //    //LanguageManager.LanguageConverter(myVisual);
            //    */
            //    Debug.WriteLine(tmsg);

            //    var service = GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;
            //    List<Window> wins = service?.GetAllWindows();

            //    foreach (var win in wins)
            //    {
            //        try
            //        {
            //            LanguageManager.LanguageConverter(win);
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLine(ex.Message + ex.StackTrace);
            //        }
            //    }

            //    var service2 = GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
            //    List<UIElement> uis = service2?.GetAllEditors();
            //    foreach (var win in uis)
            //    {
            //        LanguageManager.LanguageConverter(win);
            //    }
        }

        private void Multi(object sender, EventArgs e)
        {

        }

        private void MenuItemOnBeforeQueryStatus(object sender, EventArgs e)
        {
            var menuCommand = sender as OleMenuCommand;
            if (menuCommand == null) return;

            switch (menuCommand.CommandID.ID)
            {
                case (int)PkgCmdIDList.CmdidHelpAbout:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("About");
                    break;
                case PkgCmdIDList.MultiLanguageMenu:
                    menuCommand.Text = LanguageManager.GetInstance().ConvertSpecifier("Language");
                    break;
                case PkgCmdIDList.CmdidMultiLan1Command:
                    menuCommand.Checked = "English".Equals(LanguageInfo.CurrentLanguage);
                    break;
                case PkgCmdIDList.CmdidMultiLan2Command:
                    menuCommand.Checked = !"English".Equals(LanguageInfo.CurrentLanguage);
                    break;
            }
        }
    }
}