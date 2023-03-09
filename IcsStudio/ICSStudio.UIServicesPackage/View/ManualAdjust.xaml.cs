using System.Windows.Controls;
using ICSStudio.MultiLanguage;
using Xceed.Wpf.Toolkit;

namespace ICSStudio.UIServicesPackage.View
{
    /// <summary>
    /// ManualAdjust.xaml 的交互逻辑
    /// </summary>
    public partial class ManualAdjust :UserControl
    {
        public ManualAdjust()
        {
            InitializeComponent();

            LanguageManager.GetInstance().SetLanguage(this);
        }

        private void S1_OnSpinned(object sender, SpinEventArgs e)
        {

            ((SingleUpDown) sender).Increment = ((SingleUpDown) sender).Value * 0.01f;
        }
    }
}
