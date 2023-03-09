using System.Collections.Generic;
using System.Linq;
using ICSStudio.DownloadChange;

namespace ICSStudio.UIServicesPackage.DownloadCompareTool.ViewModel.Item
{
    internal class TasksItem:CompareItem
    {
        private readonly List<IDiffItem> _tasks;

        public TasksItem(List<IDiffItem> tasks)
        {
            _tasks = tasks;

            Title = "Tasks";

            BuildChildren();
        }

        private void BuildChildren()
        {
            Children = new List<CompareItem>();

            if (_tasks != null && _tasks.Count > 0)
            {
                foreach (var task in _tasks.OrderBy(x => x.ChangeType))
                {
                    if (task.ChangeType == ItemChangeType.Unchanged)
                        continue;

                    var taskName = task.ChangeType == ItemChangeType.Added
                        ? task.NewValue["Name"]?.ToString()
                        : task.OldValue["Name"]?.ToString();

                    var taskItem = new CompareItem { Title = taskName, DiffItem = task };

                    Children.Add(taskItem);

                    ItemType = CompareItemType.Modified;
                }
            }
        }
    }
}
