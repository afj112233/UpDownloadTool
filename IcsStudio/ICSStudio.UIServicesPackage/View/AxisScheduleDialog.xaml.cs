using ICSStudio.MultiLanguage;

namespace ICSStudio.UIServicesPackage.View
{
    /// <summary>
    /// AxisScheduleDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AxisScheduleDialog
    {
        public AxisScheduleDialog()
        {
            InitializeComponent();

            LanguageManager.GetInstance().SetLanguage(this);
        }
    }
}
