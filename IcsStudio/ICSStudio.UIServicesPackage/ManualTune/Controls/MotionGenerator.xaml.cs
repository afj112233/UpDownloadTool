using System.Windows;
using System.Windows.Controls;
using ICSStudio.UIServicesPackage.ManualTune.ViewModel;

namespace ICSStudio.UIServicesPackage.ManualTune.Controls
{
    /// <summary>
    /// MotionGenerator.xaml 的交互逻辑
    /// </summary>
    public partial class MotionGenerator
    {
        public MotionGenerator()
        {
            InitializeComponent();
        }

        private void CommandsTreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedItem = e.NewValue as TreeViewItem;

            if (selectedItem == null)
                return;

            var selectedCommand = selectedItem.Header.ToString();

            var viewModel = DataContext as MotionGeneratorViewModel;
            viewModel?.ChangeCommand(selectedCommand);
        }
    }
}
