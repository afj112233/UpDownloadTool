using ICSStudio.MultiLanguage;
using System;
using System.Windows;

namespace ICSStudio.Dialogs.NewTag
{
    /// <summary>
    /// NewAoiTagDialog.xaml 的交互逻辑
    /// </summary>
    public partial class NewAoiTagDialog
    {
        public NewAoiTagDialog(NewAoiTagViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }
    }
}
