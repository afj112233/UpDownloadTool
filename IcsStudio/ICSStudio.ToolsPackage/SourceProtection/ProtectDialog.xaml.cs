namespace ICSStudio.ToolsPackage.SourceProtection
{
    using MultiLanguage;
    /// <summary>
    /// ProtectDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ProtectDialog
    {
        public ProtectDialog()
        {
            InitializeComponent();
            LanguageManager.GetInstance().SetLanguage(this);
        }
    }
}
