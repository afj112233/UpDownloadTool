using System;
using System.Windows;
using ICSStudio.MultiLanguage;
using ICSStudio.UIServicesPackage.ViewModel;

namespace ICSStudio.UIServicesPackage.View
{
    /// <summary>
    /// NewTaskDialog.xaml 的交互逻辑
    /// </summary>
    public partial class NewTaskDialog
    {
        public NewTaskDialog()
        {
            InitializeComponent();

            var newTaskViewModel = new NewTaskViewModel();
            DataContext = newTaskViewModel;

            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }
    }
}
