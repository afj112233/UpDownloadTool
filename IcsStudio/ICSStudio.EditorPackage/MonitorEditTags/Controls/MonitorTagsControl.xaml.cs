using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using ICSStudio.EditorPackage.MonitorEditTags.Commands;
using ICSStudio.EditorPackage.MonitorEditTags.Models;
using ICSStudio.EditorPackage.MonitorEditTags.UI;
using ICSStudio.Gui.View;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Tags;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.EditorPackage.MonitorEditTags.Controls
{
    /// <summary>
    ///     MonitorTagsControl.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorTagsControl
    {
        private DataGridCellInfo _lastSelectedCell;

        private readonly DispatcherTimer _uiUpdateTimer;

        private bool _sortFlag;

        public MonitorTagsControl()
        {
            InitializeComponent();

            _uiUpdateTimer = new DispatcherTimer(DispatcherPriority.SystemIdle)
            {
                Interval = TimeSpan.FromMilliseconds(1000)
            };

            _uiUpdateTimer.Tick += OnTick;

            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        private void OnTick(object sender, EventArgs e)
        {
            try
            {
                MonitorTagsViewModel viewModel = DataContext as MonitorTagsViewModel;

                if (viewModel?.Controller == null)
                    return;

                if (!viewModel.Controller.IsOnline)
                    return;

                Controller controller = viewModel.Controller as Controller;
                if (controller == null)
                    return;

                TagSyncController tagSyncController
                    = controller.Lookup(typeof(TagSyncController)) as TagSyncController;
                if (tagSyncController == null)
                    return;

                if (viewModel.IsInAOI && viewModel.DataContext.ReferenceAoi != null)
                {
                    var dataContext = viewModel.DataContext;
                    var program = dataContext.GetReferenceProgram();
                    var transformTable = dataContext.GetFinalTransformTable();

                    foreach (var item in MainDataGrid.ItemContainerGenerator.Items.OfType<MonitorTagItem>())
                    {
                        if (!(item.ItemType == TagItemType.Array || item.ItemType == TagItemType.Struct))
                        {
                            var row = MainDataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                            if (row != null && row.IsVisible)
                            {
                                var dataServer = item.DataServer;
                                var transformName = (string)transformTable[item.Tag.Name.ToUpper()];

                                var targetName = transformName + item.Name.Remove(0, item.Tag.Name.Length);

                                var dataOperand = dataServer.CreateDataOperand(program, targetName);
                                if (!dataOperand.IsOperandValid)
                                    dataOperand = dataServer.CreateDataOperand(dataServer.ParentController, targetName);

                                if (dataOperand.IsOperandValid)
                                    tagSyncController.Update(dataOperand.Tag, targetName);
                            }
                        }
                    }

                    return;
                }

                foreach (var item in MainDataGrid.ItemContainerGenerator.Items.OfType<MonitorTagItem>())
                {
                    if (!(item.ItemType == TagItemType.Array || item.ItemType == TagItemType.Struct))
                    {
                        var row = MainDataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                        if (row != null && row.IsVisible)
                        {
                            tagSyncController.Update(item.Tag, item.Name);
                            //Debug.WriteLine($"update {item.Tag.Name}.");
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
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

        private void MainDataGrid_OnRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            //int hehe = 12341;
        }

        //private void TextBox_OnLoaded(object sender, RoutedEventArgs e)
        //{
        //    TextBox textBox = sender as TextBox;
        //    if (textBox != null)
        //        textBox.Focus();
        //    if (textBox.IsFocused)
        //    {
        //        textBox.SelectAll();
        //    }
        //}

        private void MainDataGrid_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.HorizontalChange != 0)
            {
                //TODO(gjc): add code here?
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _uiUpdateTimer.Stop();
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!_uiUpdateTimer.IsEnabled && IsVisible)
                _uiUpdateTimer.Start();

            if (IsVisible)
            {
                Refresh();
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _uiUpdateTimer.Start();
        }

        private void MainDataGrid_OnSorting(object sender, DataGridSortingEventArgs e)
        {
            if (_sortFlag)
            {
                _sortFlag = false;
            }
            else
            {
                _sortFlag = true;
                e.Handled = true;
            }
        }

        private void RowMenu_OnOpened(object sender, RoutedEventArgs e)
        {
            EditCommands.ConfigCommand.RaiseCanExecuteChanged();
        }

        private void MainDataGrid_OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            DataGridRow currentRow = GetCurrentRow();
            if (currentRow == null || currentRow.IsSelected || MainDataGrid.SelectedItems.Count != 0)
                return;

            MainDataGrid.SelectedItems.Add(currentRow.Item);
        }

        private DataGridRow GetCurrentRow()
        {
            DataGridCellInfo currentCell = MainDataGrid.CurrentCell;
            return MainDataGrid.ItemContainerGenerator.ContainerFromItem(currentCell.Item) as DataGridRow;
        }

        private void MainDataGrid_OnTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Keyboard.FocusedElement is TextBox)
                return;

            //ThreadHelper.JoinableTaskFactory.RunAsync(VsTaskRunContext.UIThreadNormalPriority,
            //    async delegate
            //    {
            //        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            //        SendInputToTextBox(e.Text);
            //    });

#pragma warning disable VSTHRD001 // 避免旧线程切换 API
            this.MainDataGrid.Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
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

        private void MainDataGrid_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.Z
                || e.Key >= Key.NumPad0 && e.Key <= Key.Divide
                || e.Key >= Key.Oem1 && e.Key <= Key.Oem102)
            {
                DataGridCell dataGridCell = e.KeyboardDevice.FocusedElement as DataGridCell;
                if (dataGridCell != null && !dataGridCell.IsEditing && !dataGridCell.IsReadOnly)
                {
                    MainDataGrid.BeginEdit();
                }
            }
            else if (e.Key == Key.Return)
            {

            }
            else if (e.Key == Key.Next || e.Key == Key.Prior)
            {

            }
            else if (e.Key == Key.Home)
            {

            }
            else if (e.Key == Key.Escape)
            {

            }
        }

        private void ValueControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.Focus();
                textBox.SelectAll();
            }

        }

        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            (sender as TextBox)?.SelectAll();
        }

        #region For Expand

        private int _expandedItemIndex;
        private double _rowHeight;

        private void ToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;
            if (toggleButton != null && toggleButton.IsChecked == true)
            {
                var gridScrollViewer = VisualTreeHelpers.FindFirstVisualChildOfType<ScrollViewer>(MainDataGrid);
                if (gridScrollViewer == null)
                {
                    return;
                }

                if (_expandedItemIndex >= 0 && _expandedItemIndex < MainDataGrid.Items.Count)
                {
                    var item = MainDataGrid.Items[_expandedItemIndex] as MonitorTagItem;
                    if (item?.Children != null)
                    {
                        {
                            int childrenCount = item.Children.Count;

                            int maxCount = (int)(gridScrollViewer.ActualHeight / _rowHeight);

                            if (_expandedItemIndex + childrenCount > maxCount)
                            {
                                gridScrollViewer.ScrollToVerticalOffset(_expandedItemIndex);
                            }

                        }

                    }
                }

            }
        }

        private void ToggleButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;
            if (toggleButton != null && toggleButton.IsChecked == false)
            {
                var dataGridRow = VisualTreeHelpers.FindVisualParentOfType<DataGridRow>(toggleButton);
                if (dataGridRow != null)
                {
                    _expandedItemIndex = MainDataGrid.ItemContainerGenerator.IndexFromContainer(dataGridRow);
                    _rowHeight = dataGridRow.ActualHeight;
                }
                else
                {
                    _expandedItemIndex = -1;
                }
            }
        }

        #endregion
    }
}