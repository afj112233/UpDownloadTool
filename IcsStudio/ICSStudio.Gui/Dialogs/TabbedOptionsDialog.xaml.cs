using System;
using System.ComponentModel;
using System.Windows;
using ICSStudio.MultiLanguage;

namespace ICSStudio.Gui.Dialogs
{
    /// <summary>
    /// TabbedOptionsDialog.xaml 的交互逻辑
    /// </summary>
    public partial class TabbedOptionsDialog
    {
        public TabbedOptionsDialog(TabbedOptionsDialogViewModel viewModel)
        {
            InitializeComponent();

            Grid.Children.Add(viewModel.TabbedOptions);

            DataContext = viewModel;

            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", ConfigurationViewModel_LanguageChanged);
        }

        private void ConfigurationViewModel_LanguageChanged(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }
        private void TabbedOptionsDialog_OnActivated(object sender, EventArgs e)
        {
            CurrentObject.GetInstance().Current = DataContext;
        }

        private void TabbedOptionsDialog_OnDeactivated(object sender, EventArgs e)
        {
            CurrentObject.GetInstance().Current = CurrentObject.GetInstance().Previous;
        }

        private void TabbedOptionsDialog_OnClosing(object sender, CancelEventArgs e)
        {
            TabbedOptionsDialogViewModel vm = (TabbedOptionsDialogViewModel) DataContext;
            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(LanguageManager.GetInstance(), "LanguageChanged", ConfigurationViewModel_LanguageChanged);
            vm.OnClosing();
        }
    }
}
