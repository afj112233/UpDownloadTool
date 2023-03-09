using System.ComponentModel;
using ICSStudio.Interfaces.Tags;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.SimpleServices.Common;
using ICSStudio.Interfaces.Common;
using System.Windows;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.Cip.Other;
using System;

namespace ICSStudio.OrganizerPackage.ViewModel.Items
{
    public class MotionGroupItem : OrganizerItem
    {
        private readonly ITag _tag;
        private readonly Controller _controller;
        private string _groupStatus;

        public MotionGroupItem(ITag tag)
        {
            _tag = tag;
            Name = _tag.Name;
            Kind = ProjectItemType.MotionGroup;
            AssociatedObject = _tag;

            PropertyChangedEventManager.AddHandler(_tag, OnPropertyChanged, "");

            _controller = _tag?.ParentController as Controller;
            if (_controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    _controller, "IsOnlineChanged", OnIsOnlineChanged);

                UpdateIsWarning();
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();
            PropertyChangedEventManager.RemoveHandler(_tag, OnPropertyChanged, "");
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
                if (Kind == ProjectItemType.MotionGroup)
                {
                    
                    MotionGroup motionGroup = (_tag as Tag)?.DataWrapper as MotionGroup;
                    if (motionGroup != null)
                    {
                        MotionGroupDescriptor descriptor = new MotionGroupDescriptor();
                        _groupStatus = descriptor.GetGroupStatus(_tag.GetMemberValue("GroupStatus", true));
                    }

                    if (_groupStatus == "Faulted")
                    {
                        IsWarning = true;
                    }
                    else
                    {
                        IsWarning = false;
                    }
                }
            }
            else
            {
                IsWarning = false;
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                Name = _tag.Name;
            }
            //轴组需要有感叹号图标时更新
            //if(e.PropertyName == "GroupStatus")
            //{
            //    UpdateIsWarning();
            //}
        }
        protected override void DisplayNameConvert()
        {
            DisplayName = Name;
        }
    }
}
