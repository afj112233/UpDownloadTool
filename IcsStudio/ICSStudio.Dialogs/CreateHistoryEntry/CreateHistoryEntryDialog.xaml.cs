using System;
using System.Windows;
using ICSStudio.MultiLanguage;

namespace ICSStudio.Dialogs.CreateHistoryEntry
{
    /// <summary>
    /// CreateHistoryEntryDialog.xaml 的交互逻辑
    /// </summary>
    public partial class CreateHistoryEntryDialog 
    {
        public CreateHistoryEntryDialog()
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
