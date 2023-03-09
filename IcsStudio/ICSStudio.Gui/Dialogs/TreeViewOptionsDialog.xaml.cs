using System;
using System.Windows;
using ICSStudio.MultiLanguage;

namespace ICSStudio.Gui.Dialogs
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TreeViewOptionsDialog
    {
        public TreeViewOptionsDialog(TreeViewOptionsDialogViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;

            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        private void TreeViewOptionsDialog_OnActivated(object sender, EventArgs e)
        {
            CurrentObject.GetInstance().Current = DataContext;
        }

        private void TreeViewOptionsDialog_OnDeactivated(object sender, EventArgs e)
        {
            CurrentObject.GetInstance().Current = CurrentObject.GetInstance().Previous;
        }
    }
}
