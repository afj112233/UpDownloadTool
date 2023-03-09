using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ICSStudio.Gui.Utils;
using ICSStudio.MultiLanguage;
using System.Windows;

namespace ICSStudio.Dialogs.ConfigDialogs
{
    /// <summary>
    /// CamEditorDialog.xaml 的交互逻辑
    /// </summary>
    public partial class CamEditorDialog
    {
        public CamEditorDialog(CamEditorViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            Width = MinWidth;
            Height = MinHeight;

            SourceInitialized += (x, y) => { this.HideMinimizeButtons(); };
            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        private void DataGrid_PreviewCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            var viewModel = DataContext as CamEditorViewModel;
            if (viewModel == null)
                return;

            if (e.Command == DataGrid.DeleteCommand)
            {
                // SelectedItems -> Tuple list
                List<Tuple<int, CamPoint>> deleteList = new List<Tuple<int, CamPoint>>();
                foreach (var point in PointDataGrid.SelectedItems)
                {
                    var camPoint = point as CamPoint;
                    if (camPoint != null)
                    {
                        Tuple<int, CamPoint> tuple = new Tuple<int, CamPoint>(viewModel.CamPoints.IndexOf(camPoint), camPoint);
                        deleteList.Add(tuple);
                    }
                }

                // tuple list sort
                deleteList.Sort((x, y) => x.Item1 - y.Item1);

                //Do Delete
                viewModel.DeletePoint(deleteList);

                viewModel.UpdateView();
            }
        }

        private void DataGrid_OnRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            var viewModel = DataContext as CamEditorViewModel;
            if (viewModel == null)
                return;

            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (e.Row.IsNewItem)
                {
                    var item = e.Row.Item as CamPoint;
                    if (item != null)
                    {
                        Collection<CamPoint> camPoints = viewModel.CamPoints;

                        Contract.Assert(camPoints.IndexOf(item) == camPoints.Count - 1);

                        var addCommand = new AddCommand(viewModel, camPoints.Count - 1, item);
                        viewModel.CommandManager.AddCommand(addCommand);

                        viewModel.UpdateUndoRedoCanExecute();

                        if (!PointDataGrid.IsKeyboardFocusWithin)
                        {
                            PointDataGrid.Dispatcher.BeginInvoke(
                                new Action(() => PointDataGrid.Items.Refresh()),
                                DispatcherPriority.Background);
                        }
                    }
                }
            }
        }

        private void PointDataGrid_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (e.Row.IsEditing)
                {
                    var textBox = e.EditingElement as TextBox;
                    if (textBox != null)
                    {
                        string value = textBox.Text;
                        if (value == "")
                        {
                            textBox.Text = "0";
                        }
                    }
                }
            }
        }

        private DataGridCellInfo _lastSelectedCell;

        private void PointDataGrid_OnCurrentCellChanged(object sender, EventArgs e)
        {
            if (_lastSelectedCell.Item is CamPoint)
                PointDataGrid.CommitEdit(DataGridEditingUnit.Row, true);

            _lastSelectedCell = PointDataGrid.CurrentCell;
        }
    }
}
