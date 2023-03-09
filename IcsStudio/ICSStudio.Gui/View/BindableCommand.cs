using System;
using System.Windows;
using System.Windows.Input;

namespace ICSStudio.Gui.View
{
    public class BindableCommand : Freezable, ICommand
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command),
            typeof(ICommand), typeof(BindableCommand), new PropertyMetadata(OnCommandPropertyChanged));

        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (Command != null)
                return Command.CanExecute(parameter);
            return false;
        }

        public void Execute(object parameter)
        {
            Command?.Execute(parameter);
        }

        protected override Freezable CreateInstanceCore()
        {
            return new BindableCommand();
        }

        private static void OnCommandPropertyChanged(
            DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            var bindableCommand = (BindableCommand) sender;
            var oldValue = e.OldValue as ICommand;
            if (oldValue != null)
                oldValue.CanExecuteChanged -= bindableCommand.CanExecuteChanged;
            var newValue = e.NewValue as ICommand;
            if (newValue == null)
                return;
            newValue.CanExecuteChanged += bindableCommand.CanExecuteChanged;
        }
    }
}