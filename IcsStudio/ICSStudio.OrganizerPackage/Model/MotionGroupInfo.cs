using System.Collections.ObjectModel;
using System.ComponentModel;
using ICSStudio.Cip.Other;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Notification;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.SimpleServices.Tags;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.OrganizerPackage.Model
{
    internal class MotionGroupInfo : BaseSimpleInfo, IConsumer
    {
        private readonly Tag _tag;

        public MotionGroupInfo(Tag tag, ObservableCollection<SimpleInfo> infoSource) :
            base(infoSource)
        {
            _tag = tag;

            CreateInfoItems();

            PropertyChangedEventManager.AddHandler(_tag,
                OnTagPropertyChanged, string.Empty);

            Notifications.ConnectConsumer(this);

        }

        ~MotionGroupInfo()
        {
            Notifications.DisconnectConsumer(this);
        }

        private void OnTagPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (e.PropertyName == "Description")
                {
                    SetSimpleInfo("Description", _tag.Description);
                }

                if (e.PropertyName == "CoarseUpdatePeriod")
                {
                    MotionGroup motionGroup = _tag?.DataWrapper as MotionGroup;
                    if (motionGroup != null)
                    {
                        SetSimpleInfo("Coarse Update Period",
                            $"{motionGroup.CoarseUpdatePeriod / 1000f:f1}ms");
                    }
                }
            });
        }

        private void CreateInfoItems()
        {
            IController controller = _tag?.ParentController;
            MotionGroup motionGroup = _tag?.DataWrapper as MotionGroup;

            if (InfoSource != null && controller != null && motionGroup != null)
            {
                MotionGroupDescriptor descriptor = new MotionGroupDescriptor();

                string groupStatus = string.Empty;
                string groupFault = string.Empty;
                string axisFault = string.Empty;

                if (controller.IsOnline)
                {
                    groupStatus = descriptor.GetGroupStatus(_tag.GetMemberValue("GroupStatus", true));
                    groupFault = descriptor.GetGroupFault(_tag.GetMemberValue("GroupFault", true));
                    axisFault = descriptor.GetAxisFault(_tag.GetMemberValue("AxisFault", true));
                }

                InfoSource.Add(new SimpleInfo { Name = "Type", Value = "MOTION_GROUP Periodic" });
                InfoSource.Add(new SimpleInfo { Name = "Description", Value = _tag.Description });
                InfoSource.Add(new SimpleInfo
                {
                    Name = "Coarse Update Period",
                    Value = $"{motionGroup.CoarseUpdatePeriod / 1000f:f1}ms"
                });
                InfoSource.Add(new SimpleInfo { Name = "Timing Model", Value = "One Cycle" });
                InfoSource.Add(new SimpleInfo
                {
                    Name = "Group Status", Value = groupStatus
                });
                InfoSource.Add(new SimpleInfo
                {
                    Name = "Group Fault", Value = groupFault

                });
                InfoSource.Add(new SimpleInfo
                {
                    Name = "Axis Fault", Value = axisFault
                });
            }
        }

        public void Consume(MessageData message)
        {
            if (message.Type == MessageData.MessageType.PullFinished
                || message.Type == MessageData.MessageType.Restored)
            {
                ThreadHelper.JoinableTaskFactory.RunAsync(
                    async delegate
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        IController controller = _tag?.ParentController;
                        MotionGroupDescriptor descriptor = new MotionGroupDescriptor();

                        if (controller != null)
                        {
                            string groupStatus = string.Empty;
                            string groupFault = string.Empty;
                            string axisFault = string.Empty;

                            if (controller.IsOnline)
                            {
                                groupStatus = descriptor.GetGroupStatus(_tag.GetMemberValue("GroupStatus", true));
                                groupFault = descriptor.GetGroupFault(_tag.GetMemberValue("GroupFault", true));
                                axisFault = descriptor.GetAxisFault(_tag.GetMemberValue("AxisFault", true));

                                // update
                                TagSyncController tagSyncController
                                    = (controller as Controller)?.Lookup(typeof(TagSyncController)) as
                                    TagSyncController;
                                tagSyncController?.Update(_tag, _tag.Name);
                            }

                            SetSimpleInfo("Group Status", groupStatus);
                            SetSimpleInfo("Group Fault", groupFault);
                            SetSimpleInfo("Axis Fault", axisFault);
                        }


                    });
            }
        }
    }
}
