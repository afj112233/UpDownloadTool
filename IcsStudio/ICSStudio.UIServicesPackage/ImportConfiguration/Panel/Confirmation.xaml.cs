using System.Windows.Controls;
using ICSStudio.MultiLanguage;

namespace ICSStudio.UIServicesPackage.ImportConfiguration.Panel
{
    /// <summary>
    /// Confirmation.xaml 的交互逻辑
    /// </summary>
    public partial class Confirmation : UserControl
    {
        public Confirmation()
        {
            InitializeComponent();
            LanguageManager.GetInstance().SetLanguage(this);
        }
    }
}
