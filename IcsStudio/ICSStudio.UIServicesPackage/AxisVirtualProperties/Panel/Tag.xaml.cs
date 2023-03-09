using System.Windows;
using System.Windows.Input;

namespace ICSStudio.UIServicesPackage.AxisVirtualProperties.Panel
{
    /// <summary>
    /// Tag.xaml 的交互逻辑
    /// </summary>
    public partial class Tag
    {
        public Tag()
        {
            InitializeComponent();
        }

        private void Tag_OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(NameBox);
            if (NameBox.IsFocused)
            {
                NameBox.SelectAll();
            }
        }
    }
}
