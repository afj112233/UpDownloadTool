using System.Windows;
using ICSStudio.MultiLanguage;

namespace ICSStudio.UIServicesPackage.ImportConfiguration
{
    /// <summary>
    /// ImportDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ImportDialog : Window
    {
        public ImportDialog()
        {
            InitializeComponent();

            LanguageManager.GetInstance().SetLanguage(this);
        }
    }
}
