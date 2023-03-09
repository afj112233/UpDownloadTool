using ICSStudio.MultiLanguage;
using System;
using System.Windows;

namespace ICSStudio.UIServicesPackage.ManualTune
{
    /// <summary>
    /// ManualTuneDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ManualTuneDialog
    {
        public ManualTuneDialog()
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
