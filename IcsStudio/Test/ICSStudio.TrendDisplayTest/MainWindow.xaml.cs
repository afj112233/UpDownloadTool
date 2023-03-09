using System;

namespace ICSStudio.TrendDisplayTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private MainViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainViewModel();
            DataContext = _viewModel;
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            _viewModel.Cleanup();
        }
    }
}
