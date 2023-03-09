using System.Windows.Controls;
using ICSStudio.Gui.Dialogs;

namespace ICSStudio.GuiTest.Axis
{
    /// <summary>
    /// GeneralPanel.xaml 的交互逻辑
    /// </summary>
    public partial class GeneralPanel : IOptionPanel
    {
        public GeneralPanel()
        {
            InitializeComponent();
        }

        public object Owner { get; set; }

        public object Control => this;

        public void LoadOptions()
        {
            //TODO(gjc): add code here
        }

        public bool SaveOptions()
        {
            //TODO(gjc): add code here
            return true;
        }
    }
}
