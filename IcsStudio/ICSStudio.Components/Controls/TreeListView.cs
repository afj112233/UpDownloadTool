using System.Windows;
using System.Windows.Controls;

namespace ICSStudio.Components.Controls
{
    public class TreeListView : TreeView
    {
        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register(nameof(Columns),
            typeof(GridViewColumnCollection), typeof(TreeListView),
            new PropertyMetadata(null));

        public TreeListView()
        {
            SetValue(ColumnsProperty, new GridViewColumnCollection());
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }

        public void SetColumns(GridViewColumnCollection columns)
        {
            SetValue(ColumnsProperty, columns);
        }

        public GridViewColumnCollection Columns =>
            (GridViewColumnCollection) GetValue(ColumnsProperty);
    }
}
