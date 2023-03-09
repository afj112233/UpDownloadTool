using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ICSStudio.Gui.View;

namespace ICSStudio.EditorPackage.MonitorEditTags.UI
{
    public static class DataGridRowExtensions
    {
        public static DataGridCell GetCell(this DataGridRow row, int column)
        {
            DataGridCell cell = null;
            DataGridCellsPresenter presenter =
                VisualTreeHelpers.FindFirstVisualChildOfType<DataGridCellsPresenter>(row);

            if (presenter != null)
                cell = (DataGridCell) presenter.ItemContainerGenerator.ContainerFromIndex(column);

            return cell;
        }
    }
}
