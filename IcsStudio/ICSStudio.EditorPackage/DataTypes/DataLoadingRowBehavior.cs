using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ICSStudio.EditorPackage.DataTypes
{
    internal class DataLoadingRowBehavior
    {
        public static readonly DependencyProperty PropertyTypeProperty = DependencyProperty.RegisterAttached(
            "Command", typeof(ICommand), typeof(DataLoadingRowBehavior), new PropertyMetadata(default(ICommand),LoadRowPropertyChanged));

        public static void SetCommand(DependencyObject target,ICommand command)
        {
            target.SetValue(PropertyTypeProperty, command);
        }

        public static ICommand GetCommand(DependencyObject target)
        {
            return (ICommand) target.GetValue(PropertyTypeProperty);
        }

        static void LoadRowPropertyChanged(DependencyObject sender,DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = (DataGrid)sender;
            var newValue = (ICommand)e.NewValue;

            if (newValue != null)
                dataGrid.LoadingRow += DataGrid_LoadingRow; 
            else
                dataGrid.LoadingRow -= DataGrid_LoadingRow;
        }

        private static void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var dataGrid = (DataGrid) sender;
            var command = GetCommand(dataGrid);

            if(command.CanExecute(null))
                command.Execute(e);
        }
    }
}
