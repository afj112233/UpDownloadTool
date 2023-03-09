using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;

namespace ICSStudio.StxEditor.Menu
{
    /// <summary>
    /// FilterView.xaml 的交互逻辑
    /// </summary>
    public partial class FilterView : UserControl
    {
        public FilterView()
        {
            InitializeComponent();
            ServerDataTemplate = (DataTemplate) MainDataGrid.FindResource("ServerDataTemplate");
        }

        public void ScrollItemToView()
        {
            var selected = MainDataGrid.SelectedItem;
            if (selected != null)
                MainDataGrid.ScrollIntoView(MainDataGrid.SelectedItem);
        }
        
        private DataTemplate ServerDataTemplate { get; }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                if (((Item) e.AddedItems[0]).Name == "Hide Configuration..." ||
                    ((Item) e.AddedItems[0]).Name == "Configure...")
                {
                    int index = ((ComboBox) sender).Items.IndexOf(e.RemovedItems[0]);
                    ((ComboBox) sender).SelectedIndex = index;
                }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ((Button) sender).ContextMenu.IsOpen = true;
        }

        private void EventSetter_OnHandler(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }

        private void DataGrid_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            var row = e.Row;
            if (row.DataContext is TagItem && ((TagItem) row.DataContext).IsControllerTag)
                row.HeaderTemplate = ServerDataTemplate;
            else
                row.HeaderTemplate = null;
        }

        private void UIElement_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool isNumber = e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9;
            bool isLetter = e.Key >= Key.A && e.Key <= Key.Z ||
                            (e.Key >= Key.A && e.Key <= Key.Z && e.KeyboardDevice.Modifiers == ModifierKeys.Shift);
            bool isCtrlA = e.Key == Key.A && e.KeyboardDevice.Modifiers == ModifierKeys.Control;
            bool isCtrlV = e.Key == Key.V && e.KeyboardDevice.Modifiers == ModifierKeys.Control;
            bool isBack = e.Key == Key.Back;
            bool isLeftOrRight = e.Key == Key.Left || e.Key == Key.Right;
            bool isUpOrDown = e.Key == Key.Up || e.Key == Key.Down;

            if (isLetter || isNumber || isCtrlA || isCtrlV || isBack || isLeftOrRight || isUpOrDown)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void MainDataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGrid = (DataGrid) sender;
            if (e.AddedItems.Count > 0)
                dataGrid.ScrollIntoView(e.AddedItems[0]);
        }
    }
}
