using ICSStudio.Gui.Utils;
using ICSStudio.MultiLanguage;
using System;
using System.Windows;

namespace ICSStudio.UIServicesPackage.View
{
    /// <summary>
    /// DownloadDialog.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadDialog
    {
        public DownloadDialog()
        {
            InitializeComponent();

            SourceInitialized += (x, y) => { this.HideMinimizeAndMaximizeButtons(); };
            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }
    }
}
