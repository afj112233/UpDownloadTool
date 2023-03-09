using System;
using System.Collections.Generic;
using System.Windows;
using ICSStudio.Gui.Dialogs;
using ICSStudio.GuiTest.Axis;

namespace ICSStudio.GuiTest
{
    class GuiTest : Application
    {
        [STAThread]
        static void Main()
        {

            //TreeViewOptionsDialogTest();
            TabbedOptionsDialogTest();

        }

        // ReSharper disable once UnusedMember.Local
        private static void TreeViewOptionsDialogTest()
        {
            AxisViewModel viewModel = new AxisViewModel();
            TreeViewOptionsDialog dialog = new TreeViewOptionsDialog(viewModel);
            viewModel.CloseAction = dialog.Close;

            dialog.ShowDialog();
        }

        private static void TabbedOptionsDialogTest()
        {
            List<IOptionPanelDescriptor> optionPanels = new List<IOptionPanelDescriptor>
            {
                new DefaultOptionPanelDescriptor("1", "General", "General", new GeneralPanel(), null),
                new DefaultOptionPanelDescriptor("2", "Motor", "Motor Device Specification", new MotorPanel(), null)
            };
            TabbedOptionsDialogViewModel viewModel = new TabbedOptionsDialogViewModel(optionPanels);
            TabbedOptionsDialog dialog = new TabbedOptionsDialog(viewModel);
            viewModel.CloseAction = dialog.Close;

            dialog.ShowDialog();
        }
    }
}
