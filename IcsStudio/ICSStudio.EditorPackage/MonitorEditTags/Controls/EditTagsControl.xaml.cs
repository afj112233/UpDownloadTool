using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using ICSStudio.EditorPackage.MonitorEditTags.Commands;
using ICSStudio.EditorPackage.MonitorEditTags.Models;
using ICSStudio.EditorPackage.MonitorEditTags.UI;
using ICSStudio.Gui.View;
using ICSStudio.MultiLanguage;
using Microsoft.VisualStudio.Shell;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace ICSStudio.EditorPackage.MonitorEditTags.Controls
{
    /// <summary>
    ///     EditTagsControl.xaml 的交互逻辑
    /// </summary>
    public partial class EditTagsControl
    {
        private DataGridCellInfo _lastSelectedCell;
        //private bool _isSetCellSelected;

        public EditTagsControl()
        {
            InitializeComponent();
            AsteriskTemplate = (DataTemplate)MainDataGrid.FindResource("AsteriskTemplate");
            PencilTemplate = (DataTemplate)MainDataGrid.FindResource("PencilTemplate");
            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        public DataTemplate AsteriskTemplate { get; }
        public DataTemplate PencilTemplate { get; }

        private void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
        }


        private void DataGrid_OnRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (e.Row.IsNewItem)
                {
                    var item = e.Row.Item as EditTagItem;
                    if (item != null)
                        if (string.IsNullOrEmpty(item.Name))
                        {
                            e.Cancel = true;
                            MainDataGrid.CancelEdit();
                            return;
                        }
                }
            }
            else if (e.EditAction == DataGridEditAction.Cancel)
            {
            }

            //e.Row.KeyDown -= MainDataGridOnKeyDown;
            e.Row.HeaderTemplate = null;
            e.Row.UpdateLayout();
        }

        private void DataGrid_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.Item == CollectionView.NewItemPlaceholder) e.Row.HeaderTemplate = AsteriskTemplate;
        }

        private void DataGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Row.HeaderTemplate = PencilTemplate;
            e.Row.UpdateLayout();
        }

        private void DataGrid_OnUnloadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.Item == CollectionView.NewItemPlaceholder) e.Row.HeaderTemplate = null;
        }

        private void DataGrid_OnInitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            var viewModel = DataContext as EditTagsViewModel;
            var tagItem = e.NewItem as EditTagItem;
            if (viewModel != null && tagItem != null)
            {
                tagItem.Controller = viewModel.Controller;
                tagItem.Scope = viewModel.Scope;
                tagItem.ParentCollection = viewModel.EditTagCollection;
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
            if (_lastSelectedCell.Item is EditTagItem)
            {
                var row = (DataGridRow)MainDataGrid.ItemContainerGenerator.ContainerFromItem(_lastSelectedCell.Item);

                if (!(row != null && row.IsNewItem))
                    MainDataGrid.CommitEdit(DataGridEditingUnit.Row, true);

            }

            _lastSelectedCell = MainDataGrid.CurrentCell;
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

                    var cell = selectedRow?.GetCell(0);
                    if (cell != null)
                    {
                        cell.Focus();
                        Keyboard.Focus(cell);
                    }

                });

            }
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
            {
                Refresh();
            }
        }

        private void MainDataGrid_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            DependencyObject source = (DependencyObject)e.OriginalSource;
            DataGridColumnHeader header = VisualTreeHelpers.FindVisualParentOfType<DataGridColumnHeader>(source);

            if (header != null)
            {
                bool result = MainDataGrid.CommitEdit();

                if (!result)
                {
                    e.Handled = true;
                }
            }

        }

        private void RowMenu_OnOpened(object sender, RoutedEventArgs e)
        {
            EditCommands.ConfigCommand.RaiseCanExecuteChanged();
        }

        private void MainDataGrid_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Delete)
                {
                    DataGridRow currentRow = GetCurrentRow();
                    if (currentRow != null && currentRow.IsNewItem)
                    {
                        MainDataGrid.CancelEdit();
                    }
                }
                else if (e.KeyboardDevice.Modifiers == 0 && (e.Key >= Key.D0 && e.Key <= Key.Z
                                                             || e.Key >= Key.NumPad0 && e.Key <= Key.Divide
                                                             || e.Key >= Key.Oem1 && e.Key <= Key.Oem102))
                {
                    DataGridCell dataGridCell = e.KeyboardDevice.FocusedElement as DataGridCell;
                    if (dataGridCell != null && !dataGridCell.IsEditing && !dataGridCell.IsReadOnly)
                    {
                        MainDataGrid.BeginEdit();
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }

        }

        private DataGridRow GetCurrentRow()
        {
            DataGridCellInfo currentCell = MainDataGrid.CurrentCell;
            return MainDataGrid.ItemContainerGenerator.ContainerFromItem(currentCell.Item) as DataGridRow;
        }

        private void MainDataGrid_OnTextInput(object sender, TextCompositionEventArgs e)
        {
            //int haha = 1234;

            if (Keyboard.FocusedElement is TextBox)
                return;

#pragma warning disable VSTHRD001 // 避免旧线程切换 API
            MainDataGrid.Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
#pragma warning restore VSTHRD001 // 避免旧线程切换 API
                (Action)(() => SendInputToTextBox(e.Text)));

            e.Handled = true;
        }

        private void SendInputToTextBox(string text)
        {
            TextBox focusedElement = Keyboard.FocusedElement as TextBox;

            if (focusedElement == null)
                return;

            TextCompositionEventArgs compositionEventArgs = new TextCompositionEventArgs(
                InputManager.Current.PrimaryKeyboardDevice,
                new TextComposition(InputManager.Current, focusedElement, text));
            compositionEventArgs.RoutedEvent = TextCompositionManager.PreviewTextInputEvent;

            focusedElement.RaiseEvent(compositionEventArgs);

            if (compositionEventArgs.Handled)
                return;

            compositionEventArgs.RoutedEvent = TextCompositionManager.TextInputEvent;
            focusedElement.RaiseEvent(compositionEventArgs);
        }

        private void ValueControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.Focus();
                textBox.SelectAll();
            }

            AutoCompleteBox autoCompleteBox = sender as AutoCompleteBox;
            if (autoCompleteBox != null)
            {
                autoCompleteBox.Focus();
            }
        }

        private void Validation_OnError(object sender, ValidationErrorEventArgs e)
        {
            var textBox = sender as TextBox ?? new TextBox();
            var validation = Validation.GetErrors(textBox);
            if (validation.Count > 0)
            {
                var error = validation[0].ErrorContent.ToString();
                MessageBox.Show("Failed to create a variable\n" + error, "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }
    }
}