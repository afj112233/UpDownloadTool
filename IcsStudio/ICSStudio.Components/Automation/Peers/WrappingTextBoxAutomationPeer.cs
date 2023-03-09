using System.Windows.Automation.Peers;
using System.Windows.Controls;
using ICSStudio.Components.Controls;

namespace ICSStudio.Components.Automation.Peers
{
    public class WrappingTextBoxAutomationPeer : TextBoxAutomationPeer
    {
        public string Value => (this.Owner as WrappingTextBox)?.WrapText;

        public WrappingTextBoxAutomationPeer(TextBox textBox)
            : base(textBox)
        {
        }
    }
}
