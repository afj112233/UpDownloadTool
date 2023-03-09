using System;
using System.Windows;
using System.Windows.Input;
using ICSStudio.Dialogs.Filter;
using ICSStudio.MultiLanguage;
using ICSStudio.UIServicesPackage.ViewModel;

namespace ICSStudio.UIServicesPackage.View
{
    /// <summary>
    /// NewTrend.xaml 的交互逻辑
    /// </summary>
    public partial class NewTrend 
    {
        public NewTrend()
        {
            InitializeComponent();
            NameFilterPopup = new NameFilterPopup(null,null,false);
            //< filter:NameFilterPopup x:Name = "NameFilterPopup" />
            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager,EventArgs>.AddHandler(LanguageManager.GetInstance(),"LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        private void NewTrend_OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(NameTextBox);
            if (NameTextBox.IsFocused)
            {
                NameTextBox.SelectAll();
            }
        }

        private void FastAutoCompleteTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                (DataContext as NewTrendViewModel)?.AddCommand.Execute(null);
            }
        }
    }
}
