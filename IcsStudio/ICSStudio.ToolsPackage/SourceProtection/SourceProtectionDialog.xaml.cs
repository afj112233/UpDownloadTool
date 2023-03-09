using System;

namespace ICSStudio.ToolsPackage.SourceProtection
{
    using MultiLanguage;
    /// <summary>
    /// SourceProtectionDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SourceProtectionDialog
    {
        public SourceProtectionDialog()
        {
            InitializeComponent();
            LanguageManager.GetInstance().SetLanguage(this);
        }
    }
}
