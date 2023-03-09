using System.Windows;
using System.Windows.Controls;

namespace ICSStudio.Components.Controls
{
    public class TreeListViewItem : TreeViewItem
    {
        private int _level = -1;

        public int Level
        {
            get
            {
                if (_level == -1)
                {
                    TreeListViewItem item =
                        ItemsControlFromItemContainer(this) as TreeListViewItem;

                    if (item != null)
                    {
                        _level = item.Level + 1;
                    }
                    else
                    {
                        _level = 0;
                    }
                }

                return _level;
            }
        }

        protected override DependencyObject GetContainerForItemOverride() => new TreeListViewItem();

        protected override bool IsItemItsOwnContainerOverride(object item) => item is TreeListViewItem;
    }
}
