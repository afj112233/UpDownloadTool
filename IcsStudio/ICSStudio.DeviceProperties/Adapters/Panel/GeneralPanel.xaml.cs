using System.Text.RegularExpressions;
using System.Windows.Input;

namespace ICSStudio.DeviceProperties.Adapters.Panel
{
    /// <summary>
    /// GeneralPanel.xaml 的交互逻辑
    /// </summary>
    public partial class GeneralPanel
    {
        public GeneralPanel()
        {
            InitializeComponent();
        }

        public void Limitnumber(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
        }
    }
}
