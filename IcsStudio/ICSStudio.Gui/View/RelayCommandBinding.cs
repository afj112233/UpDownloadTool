using System.ComponentModel;
using System.Windows.Input;

namespace ICSStudio.Gui.View
{
    public sealed class RelayCommandBinding : CommandBinding
    {
        public RelayCommandBinding()
        {
            CanExecute += OnHandleCanExecute;
            Executed += OnHandleExecuted;
        }

        public ICommand RelayCommand { get; set; }

        public object CommandParameter { get; set; }

        public void OnHandleCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var parameter1 = CommandParameter;
            if (CommandParameter == null && e != null)
                parameter1 = e.Parameter;
            var flag = RelayCommand.CanExecute(parameter1);
            if (e == null)
                return;
            var parameter2 = e.Parameter as CancelEventArgs;
            if (parameter2 != null)
                parameter2.Cancel = true;
            e.CanExecute = flag;
            e.Handled = true;
        }

        private void OnHandleExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var parameter1 = CommandParameter;
            if (CommandParameter == null && e != null)
                parameter1 = e.Parameter;
            RelayCommand.Execute(parameter1);
            if (e == null)
                return;
            var parameter2 = e.Parameter as CancelEventArgs;
            if (parameter2 != null)
                parameter2.Cancel = true;
            e.Handled = true;
        }
    }
}