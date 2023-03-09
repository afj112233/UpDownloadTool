using System;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.MultiLanguage;

namespace ICSStudio.UIServicesPackage.MotionGroupProperties.Panel
{
    /// <summary>
    /// AxisAssignmentPanel.xaml 的交互逻辑
    /// </summary>
    public partial class AxisAssignmentPanel 
    {
        public AxisAssignmentPanel()
        {
            InitializeComponent();

            LanguageManager.GetInstance().SetLanguage(this);

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);

        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }
    }
}
