using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ICSStudio.Gui.View;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.AddOnInstruction.Panel;

namespace ICSStudio.UIServicesPackage.AddOnInstruction
{
    /// <summary>
    /// LocalTags.xaml 的交互逻辑
    /// </summary>
    public partial class LocalTags : UserControl
    {
        private DataGridCell _lastCell;

        public LocalTags()
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

            var parameter = (LocalTagRow) textBox.DataContext;

            var dialog =
                new Dialogs.SelectDataType.SelectDataTypeDialog(
                    Controller.GetInstance(),
                    parameter.DataType, true, false)
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
            ((LocalTagsViewModel)DataContext).SelectedItems = ((DataGrid)sender).SelectedItems;
        }
        
        private void MainDataGrid_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                e.Handled = true;
        }
    }
}
