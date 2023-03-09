namespace ICSStudio.ToolsPackage.SourceProtection
{
    using MultiLanguage;
    /// <summary>
    /// SourceKeyCfgDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SourceKeyCfgDialog
    {
        public SourceKeyCfgDialog()
        {
            InitializeComponent();
            LanguageManager.GetInstance().SetLanguage(this);
        }
    }
}
