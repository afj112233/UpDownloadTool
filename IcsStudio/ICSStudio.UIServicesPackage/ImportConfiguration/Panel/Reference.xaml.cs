using System.Windows.Controls;
using ICSStudio.MultiLanguage;

namespace ICSStudio.UIServicesPackage.ImportConfiguration.Panel
{
    /// <summary>
    /// Reference.xaml 的交互逻辑
    /// </summary>
    public partial class Reference : UserControl
    {
        public Reference()
        {
            InitializeComponent();

            LanguageManager.GetInstance().SetLanguage(this);
        }
    }
}
