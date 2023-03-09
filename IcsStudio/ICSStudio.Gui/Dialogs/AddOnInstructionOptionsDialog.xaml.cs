using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICSStudio.MultiLanguage;
using Application = System.Windows.Application;

namespace ICSStudio.Gui.Dialogs
{
    /// <summary>
    /// AddOnInstructionOptionsDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AddOnInstructionOptionsDialog 
    {
        public AddOnInstructionOptionsDialog(AddOnInstructionOptionsDialogViewModel viewModel)
        {
            InitializeComponent();
            Grid.Children.Add(viewModel.TabbedOptions);
            DataContext = viewModel;
            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager,EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        private void AddOnInstructionOptionsDialog_OnClosing(object sender, CancelEventArgs e)
        {
            var vm = DataContext as AddOnInstructionOptionsDialogViewModel;
            if(vm!=null)
            {
                vm.IsClosing = true;
                e.Cancel = !vm.IsClosing;
            }
        }

        private void AddOnInstructionOptionsDialog_OnActivated(object sender, EventArgs e)
        {
            CurrentObject.GetInstance().Current = DataContext;
        }

        private void AddOnInstructionOptionsDialog_OnDeactivated(object sender, EventArgs e)
        {
            CurrentObject.GetInstance().Current = CurrentObject.GetInstance().Previous;
        }
    }
}
