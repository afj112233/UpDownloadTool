using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Threading;
using ICSStudio.Components.Controls;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.QuickWatchPackage.View.Models;
using ICSStudio.QuickWatchPackage.View.UI;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.QuickWatchPackage.View
{
    /// <summary>
    /// MonitorControl.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorControl : UserControl
    {
        private DataGridCellInfo _lastSelectedCell;
        public DataTemplate AsteriskTemplate { get; }
        public DataTemplate PencilTemplate { get; }
        private DataTemplate InDataTemplate { get; }
        private DataTemplate OutDataTemplate { get; }
        private DataTemplate InOutDataTemplate { get; }
        public MonitorControl()
        {
            InitializeComponent();
            AsteriskTemplate = (DataTemplate)MainDataGrid.FindResource("AsteriskTemplate");
            PencilTemplate = (DataTemplate)MainDataGrid.FindResource("PencilTemplate");
            InDataTemplate= (DataTemplate)MainDataGrid.FindResource("InDataTemplate");
            OutDataTemplate = (DataTemplate)MainDataGrid.FindResource("OutDataTemplate");
            InOutDataTemplate = (DataTemplate)MainDataGrid.FindResource("InOutDataTemplate");

            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager,EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        private void LanguageChanged(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        public void Refresh()
        {
            var selectedItem = MainDataGrid.SelectedItem;
            if (selectedItem != null)
            {
                MainDataGrid.ScrollIntoView(selectedItem, MainDataGrid.Columns[0]);

                ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    DataGridRow selectedRow =
                        MainDataGrid.ItemContainerGenerator.ContainerFromItem(selectedItem) as DataGridRow;

                    if (selectedRow != null)
                    {
                        var cell = selectedRow.GetCell(0);
                        cell?.Focus();
                    }
                });

            }
        }

        private void MainDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
        {
            if (_lastSelectedCell.Item is MonitorTagItem)
                MainDataGrid.CommitEdit(DataGridEditingUnit.Row, true);

            _lastSelectedCell = MainDataGrid.CurrentCell;
        }

        private void MainDataGrid_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //int haha = 1234;
        }
        
        private void TextBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
                textBox.Focus();
            if (textBox.IsFocused)
            {
                textBox.SelectAll();
            }
        }

        private void MainDataGrid_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.HorizontalChange != 0)
            {
                //TODO(gjc): add code here?
            }
        }
        
        private void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            
        }


        private void DataGrid_OnRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (e.Row.IsNewItem)
                {
                    var item = e.Row.Item as MonitorTagItem;

                    var vm = (DataContext) as MonitorTagsViewModel;
                    if (item != null)
                        if (string.IsNullOrEmpty(item.Name) &&
                            (!vm.NameFilterPopup.IsOpen))
                        {
                            e.Cancel = true;
                            MainDataGrid.CancelEdit();
                            return;
                        }
                    if(((MonitorTagsViewModel)DataContext).NameFilterPopup.IsOpen)
                    {
                        e.Cancel = true;
                        MainDataGrid.BeginEdit();
                        return;
                        //if (!TagNameValidationRule.ValidateName(vm.NameFilterPopup.FilterViewModel.Name,item,vm.ParentModel).IsValid)
                        //{
                        //    e.Cancel = true;
                        //    vm.NameFilterPopup.FilterViewModel.Name = "";
                        //    MainDataGrid.BeginEdit();
                        //    return;
                        //}
                        //else
                        //{
                        //    item.Name = vm.NameFilterPopup.FilterViewModel.Name;
                        //}
                    }
                    MainDataGrid.Dispatcher.BeginInvoke(
                        new Action(() => MainDataGrid.Items.Refresh()),
                        DispatcherPriority.Background);
                }
            }
            else if (e.EditAction == DataGridEditAction.Cancel)
            {
            }
            ((MonitorTagsViewModel)DataContext).ParentModel.Editing = false;
            ((MonitorTagsViewModel)DataContext).ParentModel.RemoveCommand.RaiseCanExecuteChanged();
            e.Row.HeaderTemplate = null;
            e.Row.UpdateLayout();
        }
        
        private void DataGrid_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (((MonitorTagsViewModel) DataContext).ParentModel.SelectedItem.Category != "0")
            {
                if (((MonitorTagItem) e.Row.Item).Tag.ParentCollection.ParentProgram is AoiDefinition)
                {
                    if (((MonitorTagItem) e.Row.Item).Tag.Usage == Usage.InOut)
                    {
                        e.Row.HeaderTemplate = InOutDataTemplate;
                    }
                    if (((MonitorTagItem)e.Row.Item).Tag.Usage == Usage.Input)
                    {
                        e.Row.HeaderTemplate = InDataTemplate;
                    }
                    if (((MonitorTagItem)e.Row.Item).Tag.Usage == Usage.Output)
                    {
                        e.Row.HeaderTemplate = OutDataTemplate;
                    }
                }
            }
            else
            {
                if (e.Row.Item == CollectionView.NewItemPlaceholder) e.Row.HeaderTemplate = AsteriskTemplate;
            }
        }

        private void DataGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            _newRowView = e.Row;
            e.Row.HeaderTemplate = PencilTemplate;
            ((MonitorTagsViewModel)DataContext).ParentModel.Editing = true;
            ((MonitorTagsViewModel)DataContext).ParentModel.RemoveCommand.RaiseCanExecuteChanged();
            e.Row.UpdateLayout();
        }

        private void DataGrid_OnUnloadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.Item == CollectionView.NewItemPlaceholder) e.Row.HeaderTemplate = null;
        }

        private DataGridRow _newRowView=null;
        private void DataGrid_OnInitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            var viewModel = DataContext as MonitorTagsViewModel;
            var tagItem = e.NewItem as MonitorTagItem;
            if (viewModel != null && tagItem != null)
            {
                viewModel.SelectedMonitorTagItem = tagItem;
                tagItem.TagCollectionContainer = viewModel.Scope;
                tagItem.ParentCollection = viewModel.MonitorTagCollection;
            }
        }

        private void DataGrid_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
            }
        }

        private void DataGrid_OnCurrentCellChanged(object sender, EventArgs e)
        {
            // Raise RowEditEnding Event
            if (_lastSelectedCell.Item is MonitorTagItem)
            {
                var row = (DataGridRow)MainDataGrid.ItemContainerGenerator.ContainerFromItem(_lastSelectedCell.Item);

                if (!(row != null && row.IsNewItem))
                    MainDataGrid.CommitEdit(DataGridEditingUnit.Row, true);

            }

            _lastSelectedCell = MainDataGrid.CurrentCell;
        }

        private void AutoCompleteBox_OnTextChanged(object sender, RoutedEventArgs e)
        {
            ((MonitorTagsViewModel) DataContext).NameFilterPopup.FilterViewModel.Name = ((FastAutoCompleteTextBox) sender).Text;
        }
    }
}
