using System;
using ICSStudio.Interfaces.DataType;
using System.Runtime.Serialization;
using ICSStudio.Cip.Objects;

namespace ICSStudio.SimpleServices.DataWrapper
{
    public enum GeneralFaultType
    {
        [EnumMember(Value = "Non Major Fault")]
        NonMajorFault,
        [EnumMember(Value = "Major Fault")] MajorFault
    }

    public enum GroupType
    {
        [EnumMember(Value = "Warning Enabled")]
        WarningEnabled,
    }

    public class MotionGroup : DataWrapper
    {
        private int _coarseUpdatePeriod;
        private int _alternate1UpdateMultiplier;
        private int _alternate2UpdateMultiplier;
        private bool _autoTagUpdate;
        private GeneralFaultType _generalFaultType;
        private GroupType _groupType;
        private int _phaseShift;

        public static MotionGroup Create(IDataType dataType)
        {
            if (dataType == null || !dataType.IsMotionGroupType)
                return null;

            MotionGroup motionGroup = new MotionGroup(dataType)
            {
                CoarseUpdatePeriod = 2000,
                Alternate1UpdateMultiplier = 1,
                Alternate2UpdateMultiplier = 1,
                AutoTagUpdate = true,
                GroupType = GroupType.WarningEnabled
            };
            
            return motionGroup;
        }

        private MotionGroup(IDataType dataType)
            : base(dataType, 0, 0, 0, null)
        {
        }

        public int CoarseUpdatePeriod
        {
            get { return _coarseUpdatePeriod; }
            set
            {
                if (_coarseUpdatePeriod != value)
                {
                    _coarseUpdatePeriod = value;

                    NotifyParentPropertyChanged();
                }
            }
        }

        public int Alternate1UpdateMultiplier
        {
            get { return _alternate1UpdateMultiplier; }
            set
            {
                if (_alternate1UpdateMultiplier != value)
                {
                    _alternate1UpdateMultiplier = value;
                    NotifyParentPropertyChanged();
                }
            }
        }

        public int Alternate2UpdateMultiplier
        {
            get { return _alternate2UpdateMultiplier; }
            set
            {
                if (_alternate2UpdateMultiplier != value)
                {
                    _alternate2UpdateMultiplier = value;
                    NotifyParentPropertyChanged();
                }
            }
        }

        public bool AutoTagUpdate
        {
            get { return _autoTagUpdate; }
            set
            {
                if (_autoTagUpdate != value)
                {
                    _autoTagUpdate = value;
                    NotifyParentPropertyChanged();
                }
            }
        }

        public GeneralFaultType GeneralFaultType
        {
            get { return _generalFaultType; }
            set
            {
                if (_generalFaultType != value)
                {
                    _generalFaultType = value;
                    NotifyParentPropertyChanged();
                }
            }
        }

        public GroupType GroupType
        {
            get { return _groupType; }
            set
            {
                if (_groupType != value)
                {
                    _groupType = value;
                    NotifyParentPropertyChanged();
                }
            }
        }

        public int PhaseShift
        {
            get { return _phaseShift; }
            set
            {
                if (_phaseShift != value)
                {
                    _phaseShift = value;
                    NotifyParentPropertyChanged();
                }
            }
        }

        public float GetUpdatePeriod(AxisUpdateScheduleType axisUpdateSchedule)
        {
            switch (axisUpdateSchedule)
            {
                case AxisUpdateScheduleType.Base:
                    return (float) CoarseUpdatePeriod / 1000;
                case AxisUpdateScheduleType.Alternate1:
                    return (float) (CoarseUpdatePeriod * Alternate1UpdateMultiplier) / 1000;
                case AxisUpdateScheduleType.Alternate2:
                    return (float) (CoarseUpdatePeriod * Alternate2UpdateMultiplier) / 1000;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axisUpdateSchedule), axisUpdateSchedule, null);
            }

        }
    }
}
