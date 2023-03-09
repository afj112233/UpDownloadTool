using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ICSStudio.EditorPackage.Reference.Extend
{
    internal class ListViewItemDoubleClickBehavior
    {
        public static readonly DependencyProperty PropertyTypeProperty = DependencyProperty.RegisterAttached(
            "DoubleClickCommand", typeof(ICommand), typeof(ListViewItemDoubleClickBehavior), new PropertyMetadata(PropertyChanged));

        public static void SetDoubleClickCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(PropertyTypeProperty,value);
        }

        public static ICommand GetDoubleClickCommand(DependencyObject target)
        {
            return (ICommand) target.GetValue(PropertyTypeProperty);
        }

        static void PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var listViewItem = (ListViewItem) obj;
            if (e.NewValue != null)
                listViewItem.AddHandler(Control.MouseDoubleClickEvent, new RoutedEventHandler(DoubleClickEvent), true);
            else
                listViewItem.RemoveHandler(Control.MouseDoubleClickEvent, new RoutedEventHandler(DoubleClickEvent));
        }

        static void DoubleClickEvent(object sender, RoutedEventArgs e)
        {
            if(!(e is MouseButtonEventArgs ))return;
            var command = (ICommand)((ListViewItem) sender).GetValue(PropertyTypeProperty);
            if (command.CanExecute(null))
            {
                command.Execute(e);
            }
        }
    }
}
