using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace ICSStudio.Dialogs.WaitDialog
{
    /// <summary>
    /// Wait.xaml 的交互逻辑
    /// </summary>
    public partial class Wait
    {
        private readonly WaitViewModel _vm;

        public Wait(Action action, bool isCloseAfter = true)
        {
            InitializeComponent();
            _vm = new WaitViewModel(action, isCloseAfter);
            DataContext = _vm;

            KeyDown += Wait_KeyDown;
        }

        private void Wait_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4)
                e.Handled = true;
        }

        public object Object { set; get; }

        private void Wait_OnContentRendered(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                _vm.GoOnAction();
            });
        }
    }
}
