using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.QuickWatchPackage.View.Models
{
    public class ComboBoxTreeCollection : ObservableCollection<ComboBoxTreeItem>
    {
        public ComboBoxTreeCollection(IController controller
            , ITagCollectionContainer tagCollectionContainer)
        {
            var controllerItem = new ComboBoxTreeItem(controller.Name, controller) {ParentCollection = this};
            var programsItem = new ComboBoxTreeItem("Programs") {ParentCollection = this};
            var equipmentPhasesItem = new ComboBoxTreeItem("Equipment Phases") {ParentCollection = this};
            var equipmentSequencesItem = new ComboBoxTreeItem("Equipment Sequences") {ParentCollection = this};
            var addOnInstructionsItem = new ComboBoxTreeItem("UD Function Block") {ParentCollection = this};

            Add(controllerItem);
            Add(programsItem);
            Add(equipmentPhasesItem);
            Add(equipmentSequencesItem);
            Add(addOnInstructionsItem);

            foreach (var program in controller.Programs)
            {
                var item = new ComboBoxTreeItem(program.Name, program);
                programsItem.AddChildren(item);
            }

            foreach (var program in controller.Programs)
                if (program == tagCollectionContainer)
                {
                    programsItem.IsExpanded = true;
                    break;
                }

            //
            foreach (var aoiDefinition in controller.AOIDefinitionCollection)
            {
                var item = new ComboBoxTreeItem(aoiDefinition.Name, aoiDefinition);
                addOnInstructionsItem.AddChildren(item);
            }

            foreach (var aoiDefinition in controller.AOIDefinitionCollection)
            {
                if (aoiDefinition == tagCollectionContainer)
                {
                    addOnInstructionsItem.IsExpanded = true;
                    break;
                }
            }
        }

        public void InsertItems(int index, List<ComboBoxTreeItem> items)
        {
            if (index > Count || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (index == Count)
                AddItems(items);
            else
                for (var i = items.Count - 1; i >= 0; i--)
                {
                    items[i].ParentCollection = this;
                    Insert(index, items[i]);
                }
        }

        public void RemoveItems(List<ComboBoxTreeItem> items)
        {
            foreach (var item in items)
            {
                if (item.HasChildren)
                    RemoveItems(item.Children);

                Remove(item);

                item.ParentCollection = null;
                item.IsExpanded = false;
            }
        }

        private void AddItems(List<ComboBoxTreeItem> items)
        {
            foreach (var item in items)
            {
                item.ParentCollection = this;
                Add(item);
            }
        }
    }
}