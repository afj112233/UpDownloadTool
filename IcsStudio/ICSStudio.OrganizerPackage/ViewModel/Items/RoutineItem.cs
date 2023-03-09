using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using ICSStudio.UIInterfaces.Project;

namespace ICSStudio.OrganizerPackage.ViewModel.Items
{
    internal class RoutineItem : OrganizerItem
    {
        private readonly IRoutine _routine;

        public RoutineItem(IRoutine routine)
        {
            _routine = routine;

            Name = _routine.Name;
            Kind = ProjectItemType.Routine;
            AssociatedObject = _routine;

            IsMainRoutine = _routine.IsMainRoutine;
            IsFaultRoutine = _routine.IsFaultRoutine;

            PropertyChangedEventManager.AddHandler(_routine, OnPropertyChanged, "");
        }
        
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                Name = _routine.Name;

                SortByRoutineName();
            }

            if (e.PropertyName == "IsMainRoutine" || e.PropertyName == "IsFaultRoutine")
            {
                IsMainRoutine = _routine.IsMainRoutine;
                IsFaultRoutine = _routine.IsFaultRoutine;

                SortByRoutineName();
            }

        }

        public override void Cleanup()
        {
            PropertyChangedEventManager.RemoveHandler(_routine, OnPropertyChanged, "");
        }

        private void SortByRoutineName()
        {
            IProgram program = _routine.ParentCollection.ParentProgram as IProgram;
            IAoiDefinition aoiDefinition = _routine.ParentCollection.ParentProgram as IAoiDefinition;

            if (program != null)
            {
                SortInProgram();
            }
            else if (aoiDefinition != null)
            {
                SortInAoi();
            }

        }

        private void SortInProgram()
        {
            OrganizerItems items = this.Collection as OrganizerItems;
            Contract.Assert(items != null);

            int oldIndex = items.IndexOf(this);

            List<string> itemNames = items.Select(x =>
            {
                if (x.Kind == ProjectItemType.ProgramTags)
                    return "0";

                if (x.Kind == ProjectItemType.Routine)
                {
                    IRoutine routine = x.AssociatedObject as IRoutine;
                    if (routine != null)
                    {
                        if (routine.IsMainRoutine)
                            return "0" + x.DisplayName;
                        if (routine.IsFaultRoutine)
                            return "1" + x.DisplayName;
                    }
                }

                return x.DisplayName;
            }).ToList();


            string itemName = itemNames[oldIndex];

            itemNames.Sort((x, y) => string.Compare(x, y, StringComparison.OrdinalIgnoreCase));

            int newIndex = itemNames.IndexOf(itemName);

            if (oldIndex != newIndex)
                items.Move(oldIndex, newIndex);
        }

        private void SortInAoi()
        {
            throw new NotImplementedException();
        }
        protected override void DisplayNameConvert()
        {
            DisplayName = Name;
        }
    }
}

