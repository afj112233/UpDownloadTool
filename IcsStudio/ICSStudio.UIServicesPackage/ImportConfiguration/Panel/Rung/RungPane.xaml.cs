using System.Windows.Controls;
using ICSStudio.MultiLanguage;

namespace ICSStudio.UIServicesPackage.ImportConfiguration.Panel.Rung
{
    /// <summary>
    /// RungPane.xaml 的交互逻辑
    /// </summary>
    public partial class RungPane : UserControl
    {
        public RungPane()
        {
            InitializeComponent();
            LanguageManager.GetInstance().SetLanguage(this);
        }
    }
}
