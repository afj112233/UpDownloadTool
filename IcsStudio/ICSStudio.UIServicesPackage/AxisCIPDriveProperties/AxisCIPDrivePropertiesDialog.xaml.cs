using System;
using GalaSoft.MvvmLight;
using System.Windows;
using ICSStudio.MultiLanguage;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties
{
    /// <summary>
    /// AxisCIPDrivePropertiesDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AxisCIPDrivePropertiesDialog
    {
        public AxisCIPDrivePropertiesDialog()
        {
            InitializeComponent();
            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        private void OnClosed(object sender, EventArgs e)
        {
            ((ICleanup) DataContext)?.Cleanup();
        }
    }
}
