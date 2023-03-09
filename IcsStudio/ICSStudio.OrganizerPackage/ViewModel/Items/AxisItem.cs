using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.SimpleServices.Common;
using System.Windows;
using ICSStudio.Interfaces.Common;
using ICSStudio.Cip.Other;

namespace ICSStudio.OrganizerPackage.ViewModel.Items
{
    internal class AxisItem : OrganizerItem
    {
        private readonly Tag _axis;
        private readonly Controller _controller;
        private string _axisState;
        private string _axisFault;
        private readonly AxisDescriptor _axisDescriptor;

        public AxisItem(ITag tag)
        {
            _axis = tag as Tag;
            Contract.Assert(_axis != null);
            Name = _axis.Name;
            AssociatedObject = _axis;

            AxisCIPDrive axisCIPDrive = _axis?.DataWrapper as AxisCIPDrive;
            AxisVirtual axisVirtual = _axis?.DataWrapper as AxisVirtual;
            if (axisCIPDrive != null)
            {
                Kind = ProjectItemType.AxisCIPDrive;
                _axisDescriptor = new AxisDescriptor(axisCIPDrive.CIPAxis);
            }
            else if (axisVirtual != null)
            {
                Kind = ProjectItemType.AxisVirtual;
                _axisDescriptor = new AxisDescriptor(axisVirtual.CIPAxis);
            }
            else
            {
                throw new NotImplementedException("Check here!");
            }

            PropertyChangedEventManager.AddHandler(_axis, OnTagPropertyChanged, "");

            _controller = _axis?.ParentController as Controller;
            if (_controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    _controller, "IsOnlineChanged", OnIsOnlineChanged);

                UpdateIsWarning();
            }
        }

        public override void Cleanup()
        {
            PropertyChangedEventManager.RemoveHandler(_axis, OnTagPropertyChanged, "");
            if (_controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                    _controller, "IsOnlineChanged", OnIsOnlineChanged);
            }
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            UpdateIsWarning();
        }

        private void UpdateIsWarning()
        {
            if (_controller.IsOnline)
            {
                _axisState = _axisDescriptor.CIPAxisState;

                IsWarning = _axisState == "Faulted";
            }
            else
            {
                IsWarning = false;
            }
        }

        private void UpdateIsFault()
        {
            if (_controller.IsOnline)
            {
                _axisFault = _axisDescriptor.AxisFault;

                IsWarning = _axisFault == "PhysicalAxisFault";
            }
            else
            {
                IsWarning = false;
            }
        }

        private void OnTagPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // not in the tree
            if (Collection == null)
                return;

            if (e.PropertyName == "Name")
            {
                Name = _axis.Name;

                var parentItems = Collection as OrganizerItems;
                Contract.Assert(parentItems != null);

                int oldIndex = parentItems.IndexOf(this);

                int newIndex = 0;
                foreach (var item in parentItems)
                {
                    if (item.Kind == ProjectItemType.CoordinateSystem)
                        break;

                    int result = string.Compare(
                        item.Name,
                        Name,
                        StringComparison.OrdinalIgnoreCase);

                    if (result == 0)
                    {
                        newIndex--;
                    }

                    if (result > 0)
                        break;

                    newIndex++;
                }

                if (oldIndex != newIndex)
                    parentItems.Move(oldIndex, newIndex);

            }

            if (e.PropertyName == "AssignedGroup")
            {
                ITag assignedGroup = null;

                AxisCIPDrive axisCIPDrive = _axis?.DataWrapper as AxisCIPDrive;
                AxisVirtual axisVirtual = _axis?.DataWrapper as AxisVirtual;

                if (axisCIPDrive != null)
                {
                    assignedGroup = axisCIPDrive.AssignedGroup;
                }
                else if (axisVirtual != null)
                {
                    assignedGroup = axisVirtual.AssignedGroup;
                }

                var oldParentItems = Collection as OrganizerItems;
                Contract.Assert(oldParentItems != null);

                var parentItem =
                    assignedGroup != null ? GetMotionGroupItem(assignedGroup) : GetUngroupedAxesItem();

                Contract.Assert(parentItem != null);
                Contract.Assert(oldParentItems.Parent != parentItem);

                oldParentItems.Remove(this);
                InsertAxisItem(parentItem, this);
            }

            if(e.PropertyName == "CIPAxisState")
            {
                UpdateIsWarning();
            }

            if (e.PropertyName == "AxisFault")
            {
                UpdateIsFault();
            }
        }

        private OrganizerItem GetUngroupedAxesItem()
        {
            var items = Collection?.Parent?.Collection as OrganizerItems;
            Contract.Assert(items != null);

            foreach (var item in items)
            {
                if (item.Kind == ProjectItemType.UngroupedAxes)
                    return item;
            }

            return null;
        }

        private OrganizerItem GetMotionGroupItem(ITag tag)
        {
            var items = Collection?.Parent?.Collection as OrganizerItems;
            Contract.Assert(items != null);

            foreach (var item in items)
            {
                if (item.Kind == ProjectItemType.MotionGroup && item.AssociatedObject == tag)
                    return item;
            }

            return null;
        }

        private void InsertAxisItem(OrganizerItem parentItem, AxisItem axisItem)
        {
            OrganizerItems items = parentItem.ProjectItems as OrganizerItems;
            Contract.Assert(items != null);

            int index = 0;
            foreach (var item in items)
            {
                if (item.Kind == ProjectItemType.CoordinateSystem)
                    break;

                if (string.Compare(
                    item.Name,
                    axisItem.Name,
                    StringComparison.OrdinalIgnoreCase) > 0)
                    break;

                index++;
            }

            items.Insert(index, axisItem);
        }

        protected override void DisplayNameConvert()
        {
            DisplayName = Name;
        }
    }
}
