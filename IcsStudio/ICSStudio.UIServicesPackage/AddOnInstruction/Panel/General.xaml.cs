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

namespace ICSStudio.UIServicesPackage.AddOnInstruction
{
    /// <summary>
    /// General.xaml 的交互逻辑
    /// </summary>
    public partial class General : UserControl
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

        private void General_OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(ValidateNameControl);
            if (ValidateNameControl.IsFocused)
            {
                ValidateNameControl.SelectAll();
            }
        }
    }
}
