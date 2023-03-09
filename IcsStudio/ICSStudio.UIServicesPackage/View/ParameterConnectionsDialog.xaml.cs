using ICSStudio.Interfaces.Tags;
using ICSStudio.UIServicesPackage.ViewModel;

namespace ICSStudio.UIServicesPackage.View
{
    /// <summary>
    /// ParameterConnectionsDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ParameterConnectionsDialog
    {
        public ParameterConnectionsDialog(ITag tag)
        {
            InitializeComponent();
            DataContext = new ParameterConnectionsViewModel(tag);
        }
    }
}
