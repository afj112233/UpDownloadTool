using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Project;

namespace ICSStudio.OrganizerPackage.ViewModel.Items
{
    internal class TaskItem : OrganizerItem
    {
        private readonly ITask _task;

        public TaskItem(ITask task)
        {
            Contract.Assert(task != null);

            _task = task;
            Name = GetDisplayName(_task);
            Kind = ProjectItemType.Task;
            Inhibited = _task.IsInhibited;
            AssociatedObject = _task;

            PropertyChangedEventManager.AddHandler(_task, OnPropertyChanged, "");
        }

        public override void Cleanup()
        {
            base.Cleanup();
            PropertyChangedEventManager.RemoveHandler(_task, OnPropertyChanged, "");
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                Name = GetDisplayName(_task);

                // sort
                var controller = _task.ParentController;
                OrganizerItems items = this.Collection as OrganizerItems;

                if (controller != null && items != null)
                {
                    int oldIndex = items.IndexOf(this);

                    var names = controller.Tasks.Select(x => x.Name).ToList();
                    names.Sort((x, y) =>
                        string.Compare(x, y, StringComparison.OrdinalIgnoreCase));

                    int newIndex = names.IndexOf(_task.Name);

                    if (oldIndex != newIndex && oldIndex >= 0 && newIndex >= 0)
                    {
                        items.Move(oldIndex, newIndex);
                    }

                }
            }

            if (e.PropertyName == "IsInhibited")
            {
                Inhibited = _task.IsInhibited;
            }

            if (e.PropertyName == "Type")
            {
                Name = GetDisplayName(_task);
                UpdateIconKind();
            }

            if (e.PropertyName == "Rate")
            {
                Name = GetDisplayName(_task);
            }
        }

        private string GetDisplayName(ITask task)
        {
            if (task.Type == TaskType.Periodic)
            {
                CTask taskObject = task as CTask;
                if (taskObject != null)
                    return $"{taskObject.Name} ({taskObject.Rate:0.###} ms)";
            }

            return task.Name;
        }
        protected override void DisplayNameConvert()
        {
            DisplayName = Name;
        }
    }
}
