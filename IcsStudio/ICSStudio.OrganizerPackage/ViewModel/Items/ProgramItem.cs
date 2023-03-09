using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using ICSStudio.Interfaces.Common;
using ICSStudio.UIInterfaces.Project;

namespace ICSStudio.OrganizerPackage.ViewModel.Items
{
    internal class ProgramItem : OrganizerItem
    {
        private readonly IProgram _program;

        public ProgramItem(IProgram program)
        {
            _program = program;

            Name = _program.Name;
            Kind = ProjectItemType.Program;
            Inhibited = _program.Inhibited;
            AssociatedObject = _program;
            IsExpanded = false;

            PropertyChangedEventManager.AddHandler(_program, OnPropertyChanged, "");
        }

        public override void Cleanup()
        {
            base.Cleanup();
            PropertyChangedEventManager.RemoveHandler(_program, OnPropertyChanged, "");
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                Name = _program.Name;
            }

            if (e.PropertyName == "Inhibited")
            {
                Inhibited = _program.Inhibited;
            }

            if (e.PropertyName == "ParentTask")
            {
                var oldItems = Collection as OrganizerItems;
                Contract.Assert(oldItems != null);

                ITask task = _program.ParentTask;
                if (task != null)
                {
                    var taskItem = GetTaskItem(task);

                    oldItems.Remove(this);
                    taskItem.ProjectItems.Add(this);

                }
                else
                {

                    var unscheduledProgramsItem = GetUnscheduledProgramsItem();

                    oldItems.Remove(this);

                    // sort
                    var items = unscheduledProgramsItem.ProjectItems as OrganizerItems;
                    Contract.Assert(items != null);

                    var names = items.Select(x => x.DisplayName).ToList();
                    names.Add(DisplayName);

                    names.Sort((x, y) => string.Compare(x, y, StringComparison.OrdinalIgnoreCase));
                    int index = names.IndexOf(DisplayName);

                    items.Insert(index, this);
                    
                }
            }
        }

        private OrganizerItem GetUnscheduledProgramsItem()
        {
            var items = Collection.Parent.Collection as OrganizerItems;
            Contract.Assert(items != null);

            foreach (var item in items)
            {
                if (item.Kind == ProjectItemType.UnscheduledPrograms)
                    return item;
            }

            return null;
        }

        private OrganizerItem GetTaskItem(ITask task)
        {
            var items = Collection.Parent.Collection as OrganizerItems;
            Contract.Assert(items!=null);

            foreach (var item in items)
            {
                if (item.AssociatedObject == task)
                    return item;
            }

            return null;
        }
        protected override void DisplayNameConvert()
        {
            DisplayName = Name;
        }
    }
}
