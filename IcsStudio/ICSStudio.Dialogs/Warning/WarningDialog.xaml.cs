using System;
using System.Windows;
using ICSStudio.MultiLanguage;

namespace ICSStudio.Dialogs.Warning
{
    /// <summary>
    /// WarningDialog.xaml 的交互逻辑
    /// </summary>
    public partial class WarningDialog
    {
        public WarningDialog(string message, string reason, string errorCode = "")
        {
            InitializeComponent();

            var warningViewModel = new WarningViewModel(message, reason, errorCode);
            DataContext = warningViewModel;

            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }
    }
}
