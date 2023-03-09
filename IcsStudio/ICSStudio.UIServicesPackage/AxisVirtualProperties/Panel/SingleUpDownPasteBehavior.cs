using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace ICSStudio.UIServicesPackage.AxisVirtualProperties.Panel
{
    public class SingleUpDownPasteBehavior
    {
        public static readonly DependencyProperty PropertyTypeProperty = DependencyProperty.RegisterAttached(
            "PasteCommand", typeof(ICommand), typeof(SingleUpDownPasteBehavior), new PropertyMetadata(default(ICommand),PasteCommandChanged));
        
        public static ICommand GetPasteCommand(DependencyObject target)
        {
            return (ICommand)target.GetValue(PropertyTypeProperty);
        }

        public static void SetPasteCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(PropertyTypeProperty, value);
        }

        static void PasteCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            var newValue = (ICommand)e.NewValue;

            if (newValue != null)
                textBox.AddHandler(DataObject.PastingEvent, new RoutedEventHandler(CommandExecuted), true);
            else
                textBox.RemoveHandler(DataObject.PastingEvent, new RoutedEventHandler(CommandExecuted));

        }

        static void CommandExecuted(object sender, RoutedEventArgs e)
        {
            if (!(e is DataObjectPastingEventArgs)) return;

            var textBox = (TextBox)sender;
            var command = GetPasteCommand(textBox);

            if (command.CanExecute(null))
                command.Execute(e as DataObjectPastingEventArgs);
        }
    }
}
