namespace ICSStudio.ToolsPackage.SourceProtection
{
    using MultiLanguage;
    /// <summary>
    /// ProtectWarningDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ProtectWarningDialog
    {
        public ProtectWarningDialog()
        {
            InitializeComponent();
            LanguageManager.GetInstance().SetLanguage(this);
        }
    }
}
