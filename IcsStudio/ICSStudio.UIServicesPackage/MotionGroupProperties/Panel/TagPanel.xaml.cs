using System;
using System.Windows;
using ICSStudio.MultiLanguage;

namespace ICSStudio.UIServicesPackage.MotionGroupProperties.Panel
{
    /// <summary>
    /// TagPanel.xaml 的交互逻辑
    /// </summary>
    public partial class TagPanel
    {
        public TagPanel()
        {
            InitializeComponent();

            LanguageManager.GetInstance().SetLanguage(this);

            WeakEventManager<LanguageManager,EventArgs>.AddHandler(LanguageManager.GetInstance(),"LanguageChanged", LanguageChangedHandler);
            
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }
    }
}
