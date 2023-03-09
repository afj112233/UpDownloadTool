using System.Collections.ObjectModel;
using ICSStudio.UIInterfaces.Project;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.OrganizerPackage.ViewModel
{
    public class OrganizerItems : ObservableCollection<OrganizerItem>, IProjectItems
    {
        public OrganizerItems(IProjectItem parent)
        {
            Parent = parent;
        }

        public OrganizerItems(OrganizerItems items)
        {
            Parent = items.Parent;

        }

        public IProjectItem Parent { get; }

        public void Add(IProjectItem item)
        {
            OrganizerItem organizerItem = item as OrganizerItem;
            if (organizerItem != null)
            {
                base.Add(organizerItem);
                organizerItem.Collection = this;
            }
        }

        public void Insert(int index, IProjectItem item)
        {
            OrganizerItem organizerItem = item as OrganizerItem;
            if (organizerItem != null)
            {
                ThreadHelper.JoinableTaskFactory.Run(async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    base.Insert(index, organizerItem);
                    organizerItem.Collection = this;
                });
            }
        }

        public void Remove(IProjectItem item)
        {
            OrganizerItem organizerItem = item as OrganizerItem;
            if (organizerItem != null)
            {
                base.Remove(organizerItem);
                organizerItem.Collection = null;
            }
        }
    }
}
