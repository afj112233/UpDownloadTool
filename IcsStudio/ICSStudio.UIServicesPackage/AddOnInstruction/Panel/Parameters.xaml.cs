using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ICSStudio.Gui.View;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.AddOnInstruction.Panel;

namespace ICSStudio.UIServicesPackage.AddOnInstruction
{
    /// <summary>
    /// Parameters.xaml 的交互逻辑
    /// </summary>
    public partial class Parameters : UserControl
    {
        private DataGridCell _lastCell;

        public Parameters()
        {
            InitializeComponent();
            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);

        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        private void DataGrid_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            _lastCell = null;
        }

        private void DataGrid_OnCurrentCellChanged(object sender, EventArgs e)
        {
            if (_lastCell != null)
                _lastCell.IsEditing = false;
        }

        private void MainDataGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            DataGridCellsPresenter presenter =
                VisualTreeHelpers.FindFirstVisualChildOfType<DataGridCellsPresenter>(e.Row);
            DataGridCell cell =
                (DataGridCell) presenter.ItemContainerGenerator.ContainerFromIndex(e.Column.DisplayIndex);
            _lastCell = cell;
        }
        
        private void AutoButton_OnClick(object sender, RoutedEventArgs e)
        {
            var textBox =
                VisualTreeHelpers.FindFirstVisualChildOfType<AutoCompleteBox>(
                    ((Grid) ((Button) sender).CommandParameter));

            var parameter = (ParametersRow) textBox.DataContext;

            bool supportsOneDimensionalArray = true;
            bool supportsMultiDimensionalArrays = true;

            switch (parameter.Usage)
            {
                case Usage.Input:
                case Usage.Output:
                    supportsOneDimensionalArray = false;
                    supportsMultiDimensionalArrays = false;
                    break;

                case Usage.Local:
                    supportsMultiDimensionalArrays = false;
                    break;
            }

            var dialog =
                new Dialogs.SelectDataType.SelectDataTypeDialog(
                    Controller.GetInstance(),
                    parameter.DataType, supportsOneDimensionalArray, supportsMultiDimensionalArrays)
                {
                    Height = 350, Width = 400, Owner = Application.Current.MainWindow
                };

            var result = dialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                //SelectedParameter.Field = null;
                textBox.Text = dialog.DataType;
                textBox.Focus();
            }
        }

        private void MainDataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((ParametersViewModel)DataContext).SelectedItems = ((DataGrid) sender).SelectedItems;
        }



        private void MainDataGrid_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_isCtrlDown)
                e.Handled = true;
        }

        private bool _isCtrlDown;
        private void MainDataGrid_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                _isCtrlDown = true;
        }

        private void MainDataGrid_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                _isCtrlDown = false;
            if (e.Key == Key.Delete)
                e.Handled = true;
        }


        private void UIElement_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            eventArg.RoutedEvent = UIElement.MouseWheelEvent;
            eventArg.Source = sender;
            var scrollViewer = sender as ScrollViewer;
            scrollViewer?.RaiseEvent(eventArg);
        }
    }
}