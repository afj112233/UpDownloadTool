using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Notification;
using ICSStudio.OrganizerPackage.ViewModel;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.SimpleServices.Tags;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.OrganizerPackage.Model
{
    internal class AxisCIPDriveInfo : BaseSimpleInfo, IConsumer
    {
        private readonly Tag _tag;

        private readonly ObservableCollection<OrganizerItemInfo> _treeViewInfo;

        public AxisCIPDriveInfo(
            Tag tag,
            ObservableCollection<SimpleInfo> infoSource,
            ObservableCollection<OrganizerItemInfo> treeViewInfo) :
            base(infoSource)
        {
            _tag = tag;
            _treeViewInfo = treeViewInfo;

            CreateInfoItems();

            CreateTreeViewInfoItems();

            PropertyChangedEventManager.AddHandler(_tag,
                OnTagPropertyChanged, string.Empty);

            AxisCIPDrive axisCIPDrive = _tag?.DataWrapper as AxisCIPDrive;
            if (axisCIPDrive?.AssignedGroup != null)
            {
                PropertyChangedEventManager.AddHandler(axisCIPDrive.AssignedGroup, OnMotionGroupPropertyChanged,
                    string.Empty);
            }

            if (axisCIPDrive?.AssociatedModule != null)
            {
                PropertyChangedEventManager.AddHandler(axisCIPDrive.AssociatedModule, OnModulePropertyChanged,
                    string.Empty);
            }

            Notifications.ConnectConsumer(this);

        }

        ~AxisCIPDriveInfo()
        {
            Notifications.DisconnectConsumer(this);
        }

        private void OnModulePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (e.PropertyName == "DisplayText")
                {
                    AxisCIPDrive axisCIPDrive = _tag?.DataWrapper as AxisCIPDrive;
                    CIPMotionDrive motionDrive = axisCIPDrive?.AssociatedModule as CIPMotionDrive;
                    if (motionDrive != null && _treeViewInfo.Count >= 2)
                    {
                        var infoItem = _treeViewInfo[0];
                        infoItem.DisplayName = motionDrive.DisplayText;
                    }
                }
            });
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

                if (e.PropertyName == "MotorCatalogNumber")
                {
                    AxisCIPDrive axisCIPDrive = _tag?.DataWrapper as AxisCIPDrive;
                    if (axisCIPDrive != null)
                    {
                        AxisDescriptor axisDescriptor = new AxisDescriptor(axisCIPDrive.CIPAxis);
                        SetSimpleInfo("Motor Catalog", axisDescriptor.MotorCatalog);
                    }
                }

                if (e.PropertyName == "AssignedGroup" || e.PropertyName == "AxisUpdateSchedule")
                {
                    AxisCIPDrive axisCIPDrive = _tag?.DataWrapper as AxisCIPDrive;

                    MotionGroup motionGroup = ((Tag)axisCIPDrive?.AssignedGroup)?.DataWrapper as MotionGroup;

                    string updatePeriod = string.Empty;
                    if (motionGroup != null)
                    {
                        var axisUpdateSchedule =
                            (AxisUpdateScheduleType)Convert.ToByte(axisCIPDrive.CIPAxis.AxisUpdateSchedule);
                        updatePeriod =
                            $"{motionGroup.GetUpdatePeriod(axisUpdateSchedule):F1}ms";
                    }

                    SetSimpleInfo("Update Period", updatePeriod);

                    if (axisCIPDrive?.AssignedGroup != null)
                    {
                        PropertyChangedEventManager.AddHandler(axisCIPDrive.AssignedGroup, OnMotionGroupPropertyChanged,
                            string.Empty);
                    }
                }

                if (e.PropertyName == "AxisNumber")
                {
                    AxisCIPDrive axisCIPDrive = _tag?.DataWrapper as AxisCIPDrive;
                    CIPMotionDrive motionDrive = axisCIPDrive?.AssociatedModule as CIPMotionDrive;

                    if (axisCIPDrive != null && motionDrive != null && _treeViewInfo.Count >= 2)
                    {
                        var itemInfo = _treeViewInfo[1];
                        itemInfo.DisplayName =
                            $"Axis {axisCIPDrive.AxisNumber} - {motionDrive.GetFirstPort(PortType.Ethernet).Address}";
                    }

                }

                if (e.PropertyName == "AssociatedModule")
                {
                    CreateTreeViewInfoItems();

                    AxisCIPDrive axisCIPDrive = _tag?.DataWrapper as AxisCIPDrive;
                    if (axisCIPDrive?.AssociatedModule != null)
                    {
                        PropertyChangedEventManager.AddHandler(axisCIPDrive.AssociatedModule, OnModulePropertyChanged,
                            string.Empty);
                    }
                }
            });
        }

        private void OnMotionGroupPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (e.PropertyName == "CoarseUpdatePeriod")
                {
                    AxisCIPDrive axisCIPDrive = _tag?.DataWrapper as AxisCIPDrive;

                    MotionGroup motionGroup = ((Tag)axisCIPDrive?.AssignedGroup)?.DataWrapper as MotionGroup;

                    string updatePeriod = string.Empty;
                    if (motionGroup != null)
                    {
                        var axisUpdateSchedule =
                            (AxisUpdateScheduleType)Convert.ToByte(axisCIPDrive.CIPAxis.AxisUpdateSchedule);
                        updatePeriod =
                            $"{motionGroup.GetUpdatePeriod(axisUpdateSchedule):F1}ms";
                    }

                    SetSimpleInfo("Update Period", updatePeriod);
                }
            });
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
                        AxisCIPDrive axisCIPDrive = _tag?.DataWrapper as AxisCIPDrive;

                        if (controller != null && axisCIPDrive != null)
                        {
                            AxisDescriptor axisDescriptor = new AxisDescriptor(axisCIPDrive.CIPAxis);

                            //MotionGroup motionGroup = ((Tag)axisCIPDrive.AssignedGroup)?.DataWrapper as MotionGroup;
                            //MotionGroupDescriptor descriptor = new MotionGroupDescriptor();

                            string axisState = string.Empty;
                            string axisFault = string.Empty;
                            string moduleFaults = string.Empty;
                            string groupFault = string.Empty;
                            string motionFault = string.Empty;
                            string initializationFault = string.Empty;
                            string aprFault = string.Empty;
                            string attributeError = string.Empty;
                            string guardFault = string.Empty;
                            string startInhibited = string.Empty;

                            if (controller.IsOnline)
                            {
                                if (axisCIPDrive.AssignedGroup != null)
                                {
                                    axisState = axisDescriptor.CIPAxisState;
                                    axisFault = axisDescriptor.AxisFault;
                                    moduleFaults = axisDescriptor.ModuleFaults;
                                    //groupFault = descriptor.GetGroupFault(motionGroup.ParentTag.GetMemberValue("GroupFault", true));
                                    groupFault = axisDescriptor.GroupFault;
                                    motionFault = axisDescriptor.MotionFault;
                                    initializationFault = axisDescriptor.InitializationFault;
                                    aprFault = axisDescriptor.APRFault;
                                    attributeError = axisDescriptor.AttributeError;
                                    guardFault = axisDescriptor.GuardFault;
                                    startInhibited = axisDescriptor.StartInhibited;

                                    // update
                                    TagSyncController tagSyncController
                                        = (controller as Controller)?.Lookup(typeof(TagSyncController)) as
                                        TagSyncController;
                                    tagSyncController?.Update(_tag, _tag.Name);
                                }
                                else
                                {
                                    axisState = "Not Grouped";
                                }
                            }

                            SetSimpleInfo("Axis State", axisState);
                            SetSimpleInfo("Axis Fault", axisFault);
                            SetSimpleInfo("Module Faults", moduleFaults);
                            SetSimpleInfo("Group Fault", groupFault);
                            SetSimpleInfo("Motion Fault", motionFault);
                            SetSimpleInfo("Initialization Fault", initializationFault);
                            SetSimpleInfo("APR Fault", aprFault);
                            SetSimpleInfo("Attribute Error", attributeError);
                            SetSimpleInfo("Guard Fault", guardFault);
                            SetSimpleInfo("Start Inhibited", startInhibited);
                        }

                    });
            }
        }

        private void CreateTreeViewInfoItems()
        {
            _treeViewInfo.Clear();

            AxisCIPDrive axisCIPDrive = _tag?.DataWrapper as AxisCIPDrive;
            CIPMotionDrive motionDrive = axisCIPDrive?.AssociatedModule as CIPMotionDrive;

            if (motionDrive != null)
            {
                _treeViewInfo.Add(new OrganizerItemInfo()
                {
                    DisplayName = motionDrive.DisplayText,
                    IconVisibility = Visibility.Collapsed
                });

                _treeViewInfo.Add(new OrganizerItemInfo()
                {
                    DisplayName =
                        $"Axis {axisCIPDrive.AxisNumber} - {motionDrive.GetFirstPort(PortType.Ethernet).Address}",
                    IconVisibility = Visibility.Visible,
                    IconKind = "TagPlus",
                    IconForeground = "#FFFFD700",
                    Space = "    "
                });
            }
            else
            {
                _treeViewInfo.Add(new OrganizerItemInfo()
                    { DisplayName = "<none>", IconVisibility = Visibility.Collapsed });
            }

        }

        private void CreateInfoItems()
        {
            IController controller = _tag?.ParentController;
            AxisCIPDrive axisCIPDrive = _tag?.DataWrapper as AxisCIPDrive;

            if (InfoSource != null && controller != null && axisCIPDrive != null)
            {
                MotionGroup motionGroup = ((Tag)axisCIPDrive.AssignedGroup)?.DataWrapper as MotionGroup;
                string updatePeriod = string.Empty;
                if (motionGroup != null)
                {
                    var axisUpdateSchedule =
                        (AxisUpdateScheduleType)Convert.ToByte(axisCIPDrive.CIPAxis.AxisUpdateSchedule);
                    updatePeriod =
                        $"{motionGroup.GetUpdatePeriod(axisUpdateSchedule):F1}ms";
                }

                AxisDescriptor axisDescriptor = new AxisDescriptor(axisCIPDrive.CIPAxis);

                string axisState = string.Empty;
                string axisFault = string.Empty;
                string moduleFaults = string.Empty;
                string groupFault = string.Empty;
                string motionFault = string.Empty;
                string initializationFault = string.Empty;
                string aprFault = string.Empty;
                string attributeError = string.Empty;
                string guardFault = string.Empty;
                string startInhibited = string.Empty;
                string motorCatalog = axisDescriptor.MotorCatalog;

                if (controller.IsOnline)
                {
                    if (axisCIPDrive.AssignedGroup != null)
                    {
                        //MotionGroupDescriptor descriptor = new MotionGroupDescriptor();
                        axisState = axisDescriptor.CIPAxisState;
                        axisFault = axisDescriptor.AxisFault;
                        moduleFaults = axisDescriptor.ModuleFaults;
                        //groupFault = descriptor.GetGroupFault(motionGroup.ParentTag.GetMemberValue("GroupFault", true));
                        groupFault = axisDescriptor.GroupFault;
                        motionFault = axisDescriptor.MotionFault;
                        initializationFault = axisDescriptor.InitializationFault;
                        aprFault = axisDescriptor.APRFault;
                        attributeError = axisDescriptor.AttributeError;
                        guardFault = axisDescriptor.GuardFault;
                        startInhibited = axisDescriptor.StartInhibited;
                    }
                    else
                    {
                        axisState = "Not Grouped";
                    }
                }

                InfoSource.Add(new SimpleInfo { Name = "Type", Value = "AXIS_CIP_DRIVE" });
                InfoSource.Add(new SimpleInfo { Name = "Description", Value = _tag.Description });
                InfoSource.Add(new SimpleInfo { Name = "Axis State", Value = axisState });
                InfoSource.Add(new SimpleInfo { Name = "Update Period", Value = updatePeriod });
                InfoSource.Add(new SimpleInfo { Name = "Axis Fault", Value = axisFault });
                InfoSource.Add(new SimpleInfo { Name = "Module Faults", Value = moduleFaults });
                InfoSource.Add(new SimpleInfo { Name = "Group Fault", Value = groupFault });
                InfoSource.Add(new SimpleInfo { Name = "Motion Fault", Value = motionFault });
                InfoSource.Add(new SimpleInfo { Name = "Initialization Fault", Value = initializationFault });
                InfoSource.Add(new SimpleInfo { Name = "APR Fault", Value = aprFault });
                InfoSource.Add(new SimpleInfo { Name = "Attribute Error", Value = attributeError });
                InfoSource.Add(new SimpleInfo { Name = "Guard Fault", Value = guardFault });
                InfoSource.Add(new SimpleInfo { Name = "Start Inhibited", Value = startInhibited });
                InfoSource.Add(new SimpleInfo { Name = "Motor Catalog", Value = motorCatalog });

            }
        }
    }
}
