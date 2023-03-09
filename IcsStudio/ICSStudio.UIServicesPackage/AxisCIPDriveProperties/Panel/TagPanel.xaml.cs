using System;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.MultiLanguage;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    /// <summary>
    ///     TagPanel.xaml 的交互逻辑
    /// </summary>
    public partial class TagPanel : UserControl
    {
        public TagPanel()
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