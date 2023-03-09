using ICSStudio.CommunicationsPackage.ViewModel;
using ICSStudio.Gui.Utils;
using ICSStudio.MultiLanguage;

namespace ICSStudio.CommunicationsPackage.View
{
    /// <summary>
    /// SelectRecentPathDialog.xaml 的交互逻辑
    /// </summary>
    public partial class WhoActiveDialog
    {
        public WhoActiveDialog()
        {
            InitializeComponent();

            DataContext = new WhoActiveViewModel();

            SourceInitialized += (x, y) => { this.HideMinimizeButtons(); };
            LanguageManager.GetInstance().SetLanguage(this);
        }
    }
}
