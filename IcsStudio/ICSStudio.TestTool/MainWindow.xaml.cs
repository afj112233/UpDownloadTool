using System.Windows.Controls;

namespace ICSStudio.TestTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void LogBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            LogBox.ScrollToEnd();
        }
    }
}
