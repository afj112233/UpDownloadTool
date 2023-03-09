using System;

namespace ICSStudio.UIServicesPackage
{
    using MultiLanguage;
    /// <summary>
    /// ConnectedToDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ConnectedToDialog
    {
        public ConnectedToDialog()
        {
            InitializeComponent();
            LanguageManager.GetInstance().SetLanguage(this);
        }
    }
}
