using System.Collections.ObjectModel;

namespace ICSStudio.ToolBoxPackage
{
    public class ToolBoxItems : ObservableCollection<ToolBoxItem>
    {
        public ToolBoxItems(ToolBoxItem parent)
        {
            Parent = parent;
        }

        public ToolBoxItems(ToolBoxItems items)
        {
            Parent = items.Parent;
        }

        public ToolBoxItem Parent { get; }

        public new void Add(ToolBoxItem item)
        {
            if (item != null)
            {
                base.Add(item);
                item.Parent = this;
            }
        }

        public new void Remove(ToolBoxItem item)
        {
            if (item != null)
            {
                base.Remove(item);
                item.Parent = null;
            }
        }
    }
}
