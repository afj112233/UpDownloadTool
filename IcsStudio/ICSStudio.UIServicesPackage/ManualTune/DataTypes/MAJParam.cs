using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows;
using ICSStudio.Gui.Converters;
using ICSStudio.Gui.PropertyAttribute;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ICSStudio.UIServicesPackage.ManualTune.DataTypes
{
    [DisplayName("MAJ Parameters")]
    [TypeConverter(typeof(PropertiesDeluxeTypeConverter))]
    public class MAJParam : ParamBase
    {
        private MergeTypes _merge;
        private MergeSpeedTypes _mergeSpeed;
        private ProfileTypes _profile;

        public MAJParam()
        {
            Direction = DirectionTypes.Forward;
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

            UpdateReadOnly();
        }

        [PropertyOrder(0)] [ReadOnly(false)] public DirectionTypes Direction { get; set; }


        [PropertyOrder(1)]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public float Speed { get; set; }

        [PropertyOrder(2)]
        [DisplayName("Speed Units")]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public SpeedUnitsTypes SpeedUnits { get; set; }


        [PropertyOrder(3)]
        [DisplayName("Accel Rate")]
        [ReadOnly(false)]
        public float AccelRate { get; set; }

        [PropertyOrder(4)]
        [DisplayName("Accel Units")]
        [ReadOnly(false)]
        public AccelUnitsTypes AccelUnits { get; set; }

        [PropertyOrder(5)]
        [DisplayName("Decel Rate")]
        [ReadOnly(false)]
        public float DecelRate { get; set; }


        [PropertyOrder(6)]
        [DisplayName("Decel Units")]
        [ReadOnly(false)]
        public DecelUnitsTypes DecelUnits { get; set; }

        [PropertyOrder(7)]
        [DisplayName("Profile")]
        [ReadOnly(false)]
        public ProfileTypes Profile
        {
            get { return _profile; }
            set
            {
                if (_profile != value)
                {
                    _profile = value;
                    OnProfileChange();
                }

            }
        }

        [PropertyOrder(8)]
        [DisplayName("Accel Jerk")]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public float AccelJerk { get; set; }

        [PropertyOrder(9)]
        [DisplayName("Decel Jerk")]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public float DecelJerk { get; set; }


        [PropertyOrder(9)]
        [DisplayName("Jerk Units")]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public JerkUnitsTypes JerkUnits { get; set; }

        [PropertyOrder(10)]
        [ReadOnly(false)]
        public MergeTypes Merge
        {
            get { return _merge; }
            set
            {
                if (_merge != value)
                {
                    _merge = value;
                    OnMergeChange();
                }

            }
        }

        [PropertyOrder(11)]
        [DisplayName("Merge Speed")]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public MergeSpeedTypes MergeSpeed
        {
            get { return _mergeSpeed; }
            set
            {
                if (_mergeSpeed != value)
                {
                    _mergeSpeed = value;
                    OnMergeSpeedChange();
                }
            }
        }


        [PropertyOrder(12)]
        [DisplayName("Lock Position")]
        [ReadOnly(true)]
        public float LockPosition { get; set; }

        [PropertyOrder(13)]
        [DisplayName("Lock Direction")]
        [ReadOnly(true)]
        public LockDirectionTypes LockDirection { get; set; }

        private void OnProfileChange()
        {
            SetPropertyReadOnly("AccelJerk", IsAccelJerkReadOnly());
            SetPropertyReadOnly("DecelJerk", IsDecelJerkReadOnly());
            SetPropertyReadOnly("JerkUnits", IsJerkUnitsReadOnly());

            OnDynamicPropertyChanged?.Invoke(this);
        }

        private void OnMergeChange()
        {
            SetPropertyReadOnly("MergeSpeed", IsMergeSpeedReadOnly());

            SetPropertyReadOnly("Speed", IsSpeedReadOnly());
            SetPropertyReadOnly("SpeedUnits", IsSpeedUnitsReadOnly());

            OnDynamicPropertyChanged?.Invoke(this);
        }

        private void OnMergeSpeedChange()
        {
            SetPropertyReadOnly("Speed", IsSpeedReadOnly());
            SetPropertyReadOnly("SpeedUnits", IsSpeedUnitsReadOnly());
            
            OnDynamicPropertyChanged?.Invoke(this);
        }

        #region enum types

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum DirectionTypes : uint
        {
            [EnumMember(Value = "Forward")] Forward = 0,
            [EnumMember(Value = "Reverse")] Reverse = 1
        }

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum SpeedUnitsTypes : uint
        {
            [EnumMember(Value = "Units per sec")] UnitsPerSec = 0,
            [EnumMember(Value = "% of Maximum")] PercentOfMaximum = 1,

            //[EnumMember(Value = "Units per Master Unit")]
            //UnitsPerMasterUnit = 4
        }


        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum AccelUnitsTypes : uint
        {
            [EnumMember(Value = "Units per sec2")] UnitsPerSec2 = 0,
            [EnumMember(Value = "% of Maximum")] PercentOfMaximum = 1,

            //[EnumMember(Value = "Units per Master Unit2")]
            //UnitsPerMasterUnit2 = 4
        }

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum DecelUnitsTypes : uint
        {
            [EnumMember(Value = "Units per sec2")] UnitsPerSec2 = 0,
            [EnumMember(Value = "% of Maximum")] PercentOfMaximum = 1,

            //[EnumMember(Value = "Units per Master Unit2")]
            //UnitsPerMasterUnit2 = 4
        }

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum ProfileTypes : uint
        {
            [EnumMember(Value = "Trapezoidal")] Trapezoidal = 0,
            [EnumMember(Value = "S-Curve")] SCurve = 1
        }

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum JerkUnitsTypes : uint
        {
            [EnumMember(Value = "Units per sec3")] UnitsPerSec3 = 0,
            [EnumMember(Value = "% of Maximum")] PercentOfMaximum = 1,
            //[EnumMember(Value = "% of Time")] PercentOfTime = 2,

            //[EnumMember(Value = "Units per Master Unit3")]
            //UnitsPerMasterUnit3 = 4,

            //[EnumMember(Value = "% of Time-Master Driven")]
            //PercentOfTimeMasterDriven = 6
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

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum LockDirectionTypes : uint
        {
            [EnumMember(Value = "None")] None = 0,

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

        #region Private

        private void UpdateReadOnly()
        {
            SetPropertyReadOnly("Speed", IsSpeedReadOnly());
            SetPropertyReadOnly("SpeedUnits", IsSpeedUnitsReadOnly());

            SetPropertyReadOnly("AccelJerk", IsAccelJerkReadOnly());
            SetPropertyReadOnly("DecelJerk", IsDecelJerkReadOnly());
            SetPropertyReadOnly("JerkUnits", IsJerkUnitsReadOnly());

            SetPropertyReadOnly("MergeSpeed", IsMergeSpeedReadOnly());
        }

        private bool IsMergeSpeedReadOnly()
        {
            if (_merge == MergeTypes.Disabled)
                return true;

            return false;
        }

        private bool IsJerkUnitsReadOnly()
        {
            if (_profile == ProfileTypes.Trapezoidal)
                return true;

            return false;
        }

        private bool IsDecelJerkReadOnly()
        {
            if (_profile == ProfileTypes.Trapezoidal)
                return true;

            return false;
        }

        private bool IsAccelJerkReadOnly()
        {
            if (_profile == ProfileTypes.Trapezoidal)
                return true;

            return false;
        }

        private bool IsSpeedUnitsReadOnly()
        {
            if (_merge == MergeTypes.Enabled && _mergeSpeed == MergeSpeedTypes.Current)
                return true;

            return false;
        }

        private bool IsSpeedReadOnly()
        {
            if (_merge == MergeTypes.Enabled && _mergeSpeed == MergeSpeedTypes.Current)
                return true;

            return false;
        }

        #endregion
    }
}
