using System.ComponentModel;
using System.Runtime.Serialization;
using ICSStudio.Gui.Converters;
using ICSStudio.Gui.PropertyAttribute;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.DataTypes
{
    [DisplayName("MAM Parameters")]
    [TypeConverter(typeof(PropertiesDeluxeTypeConverter))]
    public class MAMParam : ParamBase
    {
        private MergeTypes _merge;
        private MergeSpeedTypes _mergeSpeed;
        private ProfileTypes _profile;

        public MAMParam()
        {
            MoveType = MoveTypeTypes.Absolute;
            Position = 0;
            Speed = 0;
            SpeedUnits = SpeedUnitsTypes.UnitsPerSec;

            AccelRate = 100;
            AccelUnits = AccelUnitsTypes.UnitsPerSec2;

            DecelRate = 100;
            DecelUnits = DecelUnitsTypes.UnitsPerSec2;

            Profile = ProfileTypes.Trapezoidal;

            AccelJerk = 100;
            DecelJerk = 100;

            JerkUnits = JerkUnitsTypes.PercentOfMaximum;
            //JerkUnits = JerkUnitsTypes.PercentOfTime;

            Merge = MergeTypes.Disabled;
            MergeSpeed = MergeSpeedTypes.Programmed;

            LockPosition = 0;
            LockDirection = LockDirectionTypes.None;

            EventDistance = 0;
            CalculatedData = 0;
        }

        [PropertyOrder(0)]
        [DisplayName("Move Type")]
        [ReadOnly(false)]
        public MoveTypeTypes MoveType { get; set; }

        [PropertyOrder(1)]
        [DisplayName("Position")]
        [ReadOnly(false)]
        public float Position { get; set; }

        [PropertyOrder(2)]
        [DisplayName("Speed")]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public float Speed { get; set; }

        [PropertyOrder(3)]
        [DisplayName("Speed Units")]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public SpeedUnitsTypes SpeedUnits { get; set; }

        [PropertyOrder(4)]
        [DisplayName("Accel Rate")]
        [ReadOnly(false)]
        public float AccelRate { get; set; }

        [PropertyOrder(5)]
        [DisplayName("Accel Units")]
        [ReadOnly(false)]
        public AccelUnitsTypes AccelUnits { get; set; }

        [PropertyOrder(6)]
        [DisplayName("Decel Rate")]
        [ReadOnly(false)]
        public float DecelRate { get; set; }

        [PropertyOrder(7)]
        [DisplayName("Decel Units")]
        [ReadOnly(false)]
        public DecelUnitsTypes DecelUnits { get; set; }

        [PropertyOrder(8)]
        [DisplayName("Profile")]
        [ReadOnly(false)]
        public ProfileTypes Profile
        {
            get { return _profile; }
            set
            {
                _profile = value;
                OnProfileChanged();
            }
        }

        [PropertyOrder(9)]
        [DisplayName("Accel Jerk")]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public float AccelJerk { get; set; }

        [PropertyOrder(10)]
        [DisplayName("Decel Jerk")]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public float DecelJerk { get; set; }

        [PropertyOrder(11)]
        [DisplayName("Jerk Units")]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public JerkUnitsTypes JerkUnits { get; set; }

        [PropertyOrder(12)]
        [DisplayName("Merge")]
        [ReadOnly(false)]
        public MergeTypes Merge
        {
            get { return _merge; }
            set
            {
                _merge = value;
                OnMergeChanged();
            }
        }

        [PropertyOrder(13)]
        [DisplayName("Merge Speed")]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public MergeSpeedTypes MergeSpeed
        {
            get { return _mergeSpeed; }
            set
            {
                _mergeSpeed = value;
                OnMergeSpeedChanged();
            }
        }



        [PropertyOrder(14)]
        [DisplayName("Lock Position")]
        [ReadOnly(true)]
        public float LockPosition { get; set; }

        [PropertyOrder(15)]
        [DisplayName("Lock Direction")]
        [ReadOnly(true)]
        public LockDirectionTypes LockDirection { get; set; }


        // TODO(gjc):need more edit
        [PropertyOrder(16)]
        [DisplayName("Event Distance")]
        [ReadOnly(true)]
        public float EventDistance { get; set; }

        [PropertyOrder(17)]
        [DisplayName("Calculated Data")]
        [ReadOnly(true)]
        public float CalculatedData { get; set; }

        private void OnMergeChanged()
        {
            switch (Merge)
            {
                case MergeTypes.Disabled:
                    SetPropertyReadOnly("MergeSpeed", true);
                    break;
                case MergeTypes.Enabled:
                    SetPropertyReadOnly("MergeSpeed", false);
                    break;
            }

            if (Merge == MergeTypes.Enabled && MergeSpeed == MergeSpeedTypes.Current)
            {
                SetPropertyReadOnly("Speed", true);
                SetPropertyReadOnly("SpeedUnits", true);
            }
            else
            {
                SetPropertyReadOnly("Speed", false);
                SetPropertyReadOnly("SpeedUnits", false);
            }

            OnDynamicPropertyChanged?.Invoke(this);
        }

        private void OnMergeSpeedChanged()
        {
            if (Merge == MergeTypes.Enabled && MergeSpeed == MergeSpeedTypes.Current)
            {
                SetPropertyReadOnly("Speed", true);
                SetPropertyReadOnly("SpeedUnits", true);
            }
            else
            {
                SetPropertyReadOnly("Speed", false);
                SetPropertyReadOnly("SpeedUnits", false);
            }

            OnDynamicPropertyChanged?.Invoke(this);
        }

        private void OnProfileChanged()
        {
            switch (Profile)
            {
                case ProfileTypes.Trapezoidal:
                    SetPropertyReadOnly("AccelJerk", true);
                    SetPropertyReadOnly("DecelJerk", true);
                    SetPropertyReadOnly("JerkUnits", true);
                    break;
                case ProfileTypes.SCurve:
                    SetPropertyReadOnly("AccelJerk", false);
                    SetPropertyReadOnly("DecelJerk", false);
                    SetPropertyReadOnly("JerkUnits", false);
                    break;
            }

            OnDynamicPropertyChanged?.Invoke(this);
        }

        #region enum types

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum MoveTypeTypes : uint
        {
            [EnumMember(Value = "Absolute")] Absolute = 0,
            [EnumMember(Value = "Incremental")] Incremental = 1,

            [EnumMember(Value = "Rotary Shortest Path")]
            RotaryShortestPath = 2,

            [EnumMember(Value = "Rotary Positive")]
            RotaryPositive = 3,

            [EnumMember(Value = "Rotary Negative")]
            RotaryNegative = 4,

            [EnumMember(Value = "Absolute Master Offset")]
            AbsoluteMasterOffset = 5,

            [EnumMember(Value = "Incremental Master Offset")]
            IncrementalMasterOffset = 6
        }

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum SpeedUnitsTypes : uint
        {
            [EnumMember(Value = "Units per sec")]
            UnitsPerSec = 0,
            [EnumMember(Value = "% of Maximum")]
            PercentOfMaximum = 1,

            [EnumMember(Value = "Seconds")]
            Seconds = 3,

            [EnumMember(Value = "Units per MasterUnit")]
            UnitsPerMasterUnit = 4,
            [EnumMember(Value = "Master Units")]
            MasterUnits = 7
        }

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum AccelUnitsTypes : uint
        {
            [EnumMember(Value = "Units per sec2")]
            UnitsPerSec2 = 0,
            [EnumMember(Value = "% of Maximum")]
            PercentOfMaximum = 1,

            [EnumMember(Value = "Seconds")]
            Seconds = 3,

            [EnumMember(Value = "Units per MasterUnit2")]
            UnitsPerMasterUnit2 = 4,
            [EnumMember(Value = "Master Units")]
            MasterUnits = 7
        }

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum DecelUnitsTypes : uint
        {
            [EnumMember(Value = "Units per sec2")]
            UnitsPerSec2 = 0,
            [EnumMember(Value = "% of Maximum")]
            PercentOfMaximum = 1,

            [EnumMember(Value = "Seconds")]
            Seconds = 3,

            [EnumMember(Value = "Units per MasterUnit2")]
            UnitsPerMasterUnit2 = 4,
            [EnumMember(Value = "Master Units")]
            MasterUnits = 7
        }

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum ProfileTypes : uint
        {
            [EnumMember(Value = "Trapezoidal")]
            Trapezoidal = 0,
            [EnumMember(Value = "S-Curve")]
            SCurve = 1
        }

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum JerkUnitsTypes : uint
        {
            [EnumMember(Value = "Units per sec3")]
            UnitsPerSec3 = 0,
            [EnumMember(Value = "% of Maximum")]
            PercentOfMaximum = 1,
            [EnumMember(Value = "% of Time")]
            PercentOfTime = 2,
            [EnumMember(Value = "Seconds")]
            Seconds = 3,

            [EnumMember(Value = "Units per MasterUnit3")]
            UnitsPerMasterUnit3 = 4,

            [EnumMember(Value = "% of Time-Master Driven")]
            PercentOfTimeMasterDriven = 6,
            [EnumMember(Value = "Master Units")]
            MasterUnits = 7
        }

        public enum MergeTypes : uint
        {
            Disabled = 0,
            Enabled = 1
        }

        public enum MergeSpeedTypes : uint
        {
            Programmed = 0,
            Current = 1
        }

        public enum LockDirectionTypes : uint
        {
            [EnumMember(Value = "None")]
            None = 0,

            [EnumMember(Value = "Immediate Forward Only")]
            ImmediateForwardOnly = 1,

            [EnumMember(Value = "Immediate Reverse Only")]
            ImmediateReverseOnly = 2,

            [EnumMember(Value = "Position Forward Only")]
            PositionForwardOnly = 3,

            [EnumMember(Value = "Position Reverse Only")]
            PositionReverseOnly = 4
        }

        #endregion
    }
}
