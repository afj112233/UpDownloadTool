using System.Windows;
using System.Windows.Controls;
using ICSStudio.MultiLanguage;

namespace ICSStudio.UIServicesPackage.ProgramProperties.Panel
{
    /// <summary>
    /// Parameters.xaml 的交互逻辑
    /// </summary>
    public partial class Parameters
    {
        public Parameters()
        {
            InitializeComponent();
        }

        private void Parameters_OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = (ParametersViewModel) DataContext;
            vm.IsDirty = false;
        }
    }

}
