namespace ICSStudio.ToolsPackage.SourceProtection
{
    using MultiLanguage;
    /// <summary>
    /// SourceKeyFileLocationDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SourceKeyFileLocationDialog
    {
        public SourceKeyFileLocationDialog()
        {
            InitializeComponent();
            LanguageManager.GetInstance().SetLanguage(this);
        }
    }
}
