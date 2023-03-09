using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ICSStudio.UIServicesPackage.RoutineProperties.Panel
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

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (NameBox1.IsVisible)
            {
                if (NameBox1.IsEnabled)
                {
                    Keyboard.Focus(NameBox1);
                    if (NameBox1.IsFocused)
                    {
                        NameBox1.SelectAll();
                    }
                }
                else
                {
                    NameBox1.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    Keyboard.Focus(Description);
                    if (Description.IsFocused)
                    {
                        Description.SelectAll();
                    }
                }
            }
            else if (NameBox2.IsVisible)
            {
                if (NameBox2.IsEnabled)
                {
                    Keyboard.Focus(NameBox1);
                    if (NameBox2.IsFocused)
                    {
                        NameBox2.SelectAll();
                    }
                }
                else
                {
                    NameBox2.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    Keyboard.Focus(Description);
                    if (Description.IsFocused)
                    {
                        Description.SelectAll();
                    }
                }
            }
        }
    }
}
