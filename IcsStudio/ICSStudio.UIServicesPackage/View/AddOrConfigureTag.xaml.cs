using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.UIServicesPackage.ViewModel;

namespace ICSStudio.UIServicesPackage.View
{
    /// <summary>
    /// AddOrConfigureTag.xaml 的交互逻辑
    /// </summary>
    public partial class AddOrConfigureTag : Window
    {
        private AddOrConfigureTagViewModel _vm;
        public AddOrConfigureTag(string trendName, IController controller, List<string> penList)
        {
            InitializeComponent();
            _vm = new AddOrConfigureTagViewModel(this,trendName, controller, penList);

            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager,EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        public List<string> GetAddPenList()
        {
            return _vm.AddPenList;
        }

        public List<string> GetRemovePenList()
        {
            return _vm.RemovePenList;
        }

        private void FastAutoCompleteTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                (DataContext as AddOrConfigureTagViewModel)?.AddCommand.Execute(null);
            }
        }
    }
}
