using ICSStudio.MultiLanguage;
using System;
using System.Windows;
using System.Windows.Input;

namespace ICSStudio.UIServicesPackage.TagProperties.Panel
{
    /// <summary>
    /// General.xaml 的交互逻辑
    /// </summary>
    public partial class General
    {
        public General()
        {
            InitializeComponent();
            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(NameBox);
            if (NameBox.IsFocused)
            {
                NameBox.SelectAll();
            }
        }
    }
}
