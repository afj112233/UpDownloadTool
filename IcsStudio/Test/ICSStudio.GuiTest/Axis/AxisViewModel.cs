using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ICSStudio.Gui.Dialogs;

namespace ICSStudio.GuiTest.Axis
{
    public class AxisViewModel : TreeViewOptionsDialogViewModel
    {
        public AxisViewModel()
        {
            List<IOptionPanelDescriptor> optionPanels = new List<IOptionPanelDescriptor>
            {
                new DefaultOptionPanelDescriptor("1", "General", "General", new GeneralPanel(), null),
                new DefaultOptionPanelDescriptor("2", "Motor", "Motor Device Specification", new MotorPanel(), null)
            };

            OptionPanelNodes = optionPanels.Select(op => new OptionPanelNode(op, this)).ToList();
            OptionPanelNodes[0].IsSelected = true;

            StateVisibility = Visibility.Visible;
            ExpansionVisibility = Visibility.Visible;
            State = "Axis State:";
            ExpansionName = "Manual Tune...";

            Title = "Axis Properties - axis";
        }

        protected override void ExecuteOkCommand()
        {
        }
    }
}
