using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICSStudio.MultiLanguage;

namespace ICSStudio.UIServicesPackage.View
{
    /// <summary>
    /// ChangeControllerDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ChangeControllerDialog : Window
    {
        public ChangeControllerDialog()
        {
            InitializeComponent();
            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(TypeComboBox);
        }
        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }
    }
}
