using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using System.Windows;
using System;

namespace ICSStudio.Dialogs.ConfigDialogs
{
    /// <summary>
    /// MessageConfigurationDialog.xaml 的交互逻辑
    /// </summary>
    public partial class MessageConfigurationDialog
    {
        public MessageConfigurationDialog(ITag tag,string title)
        {
            InitializeComponent();

            DataContext = new MessageConfigurationViewModel(tag,title);
            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }
        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }
    }
}
