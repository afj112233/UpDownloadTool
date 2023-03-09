using System.Windows;
using System.Windows.Input;
using ICSStudio.MultiLanguage;

namespace ICSStudio.UIServicesPackage.ProgramProperties.Panel
{
    /// <summary>
    /// General.xaml 的交互逻辑
    /// </summary>
    public partial class General
    {
        public General()
        {
            InitializeComponent();
        }

        private void General_OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(NameBox);
            if (NameBox.IsFocused)
            {
                NameBox.SelectAll();
            }
        }
    }
}
