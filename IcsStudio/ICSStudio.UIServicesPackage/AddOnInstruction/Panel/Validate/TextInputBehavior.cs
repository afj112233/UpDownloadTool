using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ICSStudio.UIServicesPackage.AddOnInstruction.Panel.Validate
{
    internal class TextInputBehavior
    {
        public static readonly DependencyProperty PropertyTypeProperty = DependencyProperty.RegisterAttached(
            "InputCommand", typeof(ICommand), typeof(TextInputBehavior), new PropertyMetadata(InputCommandChanged));

        public static void SetInputCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(PropertyTypeProperty, value);
        }

        public static ICommand GetInputCommand(DependencyObject target)
        {
            return (ICommand) target.GetValue(PropertyTypeProperty);
        }

        static void InputCommandChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = (TextBox) sender;

            if (e.NewValue != null)
                textBox.AddHandler(UIElement.PreviewTextInputEvent, new RoutedEventHandler(InputEventHandler));
            else
                textBox.RemoveHandler(UIElement.PreviewTextInputEvent, new RoutedEventHandler(InputEventHandler));
        }

        static void InputEventHandler(object sender, RoutedEventArgs e)
        {
            if (!(e is TextCompositionEventArgs)) return;

            var command = (ICommand) ((TextBox) sender).GetValue(PropertyTypeProperty);
            if (command.CanExecute(null))
                command.Execute(e);
        }
    }
}
