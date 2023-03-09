using ICSStudio.MultiLanguage;
using System;
using System.Windows;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands
{
    /// <summary>
    /// MotionDirectCommandsDialog.xaml 的交互逻辑
    /// </summary>
    public partial class MotionDirectCommandsDialog
    {
        public MotionDirectCommandsDialog()
        {
            InitializeComponent();
            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }
    }
}
