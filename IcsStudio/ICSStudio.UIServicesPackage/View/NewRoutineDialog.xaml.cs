using System;
using System.Windows;
using System.Windows.Input;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.UIServicesPackage.ViewModel;

namespace ICSStudio.UIServicesPackage.View
{
    /// <summary>
    /// NewRoutineDialog.xaml 的交互逻辑
    /// </summary>
    public partial class NewRoutineDialog 
    {
        public NewRoutineDialog(IProgramModule program,string name = default(string))
        {
            InitializeComponent();
            DataContext = new NewRoutineDialogViewModel(program,name);
            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(NameValidateNameControl);
            if (NameValidateNameControl.IsFocused)
            {
                NameValidateNameControl.SelectAll();
            }
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }
    }
}
