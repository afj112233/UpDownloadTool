using System;
using System.Windows;
using System.Windows.Input;
using ICSStudio.MultiLanguage;

namespace ICSStudio.UIServicesPackage.MotionGroupProperties.Panel
{
    /// <summary>
    /// AttributePanel.xaml 的交互逻辑
    /// </summary>
    public partial class AttributePanel
    {
        public AttributePanel()
        {
            InitializeComponent();

            LanguageManager.GetInstance().SetLanguage(this);

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);

        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        private void AttributePanel_OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(SingleUpDown);
        }
    }
}
