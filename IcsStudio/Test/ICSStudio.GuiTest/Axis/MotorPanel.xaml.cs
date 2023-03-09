using ICSStudio.Gui.Dialogs;

namespace ICSStudio.GuiTest.Axis
{
    /// <summary>
    /// MotorPanel.xaml 的交互逻辑
    /// </summary>
    public partial class MotorPanel : IOptionPanel
    {
        public MotorPanel()
        {
            InitializeComponent();
        }

        public object Owner { get; set; }
        public object Control => this;
        public void LoadOptions()
        {
            
        }

        public bool SaveOptions()
        {
            return true;
        }
    }
}
