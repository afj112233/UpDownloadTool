using System.Windows;
using ICSStudio.MultiLanguage;
using ICSStudio.UIServicesPackage.ViewModel;

namespace ICSStudio.UIServicesPackage.View
{
    /// <summary>
    /// CalculateJerkDialog.xaml 的交互逻辑
    /// </summary>
    public partial class CalculateJerkDialog
    {
        private readonly CalculateJerkViewModel _viewModel;

        public CalculateJerkDialog(string title,
            string positionUnits,
            double speed, double accel, double jerk,
            int width = 425)
        {
            InitializeComponent();

            Width = width;

            _viewModel = new CalculateJerkViewModel(title, positionUnits, speed, accel, jerk);
            DataContext = _viewModel;

            LanguageManager.GetInstance().SetLanguage(this);
        }

        public double Jerk => _viewModel.Jerk;

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
