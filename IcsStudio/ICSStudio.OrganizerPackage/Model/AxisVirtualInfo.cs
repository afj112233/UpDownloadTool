using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Tags;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.OrganizerPackage.Model
{
    internal class AxisVirtualInfo : BaseSimpleInfo
    {
        private readonly Tag _tag;

        public AxisVirtualInfo(Tag tag,
            ObservableCollection<SimpleInfo> infoSource) : base(infoSource)
        {
            _tag = tag;

            CreateInfoItems();

            PropertyChangedEventManager.AddHandler(_tag,
                OnTagPropertyChanged, string.Empty);

            AxisVirtual axisVirtual = _tag?.DataWrapper as AxisVirtual;
            if (axisVirtual?.AssignedGroup != null)
            {
                PropertyChangedEventManager.AddHandler(axisVirtual.AssignedGroup, OnMotionGroupPropertyChanged,
                    string.Empty);
            }

        }

        private void OnMotionGroupPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (e.PropertyName == "CoarseUpdatePeriod")
                {
                    string updatePeriod = string.Empty;

                    AxisVirtual axisVirtual = _tag?.DataWrapper as AxisVirtual;
                    MotionGroup motionGroup = ((Tag)axisVirtual?.AssignedGroup)?.DataWrapper as MotionGroup;
                    if (motionGroup != null)
                    {
                        var axisUpdateSchedule =
                            (AxisUpdateScheduleType)Convert.ToByte(axisVirtual.CIPAxis.AxisUpdateSchedule);
                        updatePeriod =
                            $"{motionGroup.GetUpdatePeriod(axisUpdateSchedule):F1}ms";
                    }

                    SetSimpleInfo("Update Period", updatePeriod);
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

                if (e.PropertyName == "AssignedGroup" || e.PropertyName == "AxisUpdateSchedule")
                {
                    string updatePeriod = string.Empty;

                    AxisVirtual axisVirtual = _tag?.DataWrapper as AxisVirtual;
                    MotionGroup motionGroup = ((Tag)axisVirtual?.AssignedGroup)?.DataWrapper as MotionGroup;
                    if (motionGroup != null)
                    {
                        var axisUpdateSchedule =
                            (AxisUpdateScheduleType)Convert.ToByte(axisVirtual.CIPAxis.AxisUpdateSchedule);
                        updatePeriod =
                            $"{motionGroup.GetUpdatePeriod(axisUpdateSchedule):F1}ms";
                    }

                    SetSimpleInfo("Update Period", updatePeriod);

                    if (axisVirtual?.AssignedGroup != null)
                    {
                        PropertyChangedEventManager.AddHandler(axisVirtual.AssignedGroup, OnMotionGroupPropertyChanged,
                            string.Empty);
                    }
                }
            });
        }

        private void CreateInfoItems()
        {
            AxisVirtual axisVirtual = _tag?.DataWrapper as AxisVirtual;

            if (InfoSource != null && axisVirtual != null)
            {
                //TODO(gjc): need add here for Axis State
                AxisDescriptor axisDescriptor = new AxisDescriptor(axisVirtual.CIPAxis);

                string updatePeriod = string.Empty;
                MotionGroup motionGroup = ((Tag)axisVirtual.AssignedGroup)?.DataWrapper as MotionGroup;
                if (motionGroup != null)
                {
                    var axisUpdateSchedule =
                        (AxisUpdateScheduleType)Convert.ToByte(axisVirtual.CIPAxis.AxisUpdateSchedule);
                    updatePeriod =
                        $"{motionGroup.GetUpdatePeriod(axisUpdateSchedule):F1}ms";
                }


                string axisState = string.Empty;

                if (axisVirtual.Controller.IsOnline)
                {
                    if (axisVirtual.AssignedGroup == null)
                    {
                        axisState = "Not Grouped";
                    }
                    else
                    {
                        //TODO(tlm):虚轴还没有状态通道，暂时显示效果为ready，待后续完善
                        axisState = axisDescriptor.AxisState;
                    }
                }

                InfoSource.Add(new SimpleInfo { Name = "Type", Value = "AXIS_VIRTUAL" });
                InfoSource.Add(new SimpleInfo { Name = "Description", Value = _tag.Description });
                InfoSource.Add(new SimpleInfo { Name = "Axis State", Value = axisState });
                InfoSource.Add(new SimpleInfo { Name = "Update Period", Value = updatePeriod });

            }
        }
    }
}
