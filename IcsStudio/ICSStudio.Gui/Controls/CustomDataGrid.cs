using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace ICSStudio.Gui.Controls
{
    public class CustomDataGrid : DataGrid
    {
        public CustomDataGrid()
        {
            SelectionChanged += CustomDataGrid_SelectionChanged;
        }

        private void CustomDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedItemsList = SelectedItems;
        }

        public static readonly DependencyProperty SelectedItemsListProperty = DependencyProperty.Register(
            "SelectedItemsList", typeof(IList), typeof(CustomDataGrid), new PropertyMetadata(default(IList)));

        public IList SelectedItemsList
        {
            get { return (IList) GetValue(SelectedItemsListProperty); }
            set { SetValue(SelectedItemsListProperty, value); }
        }
    }
}
