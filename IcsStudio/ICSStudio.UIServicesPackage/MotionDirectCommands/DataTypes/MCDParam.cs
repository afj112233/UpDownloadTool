using System.ComponentModel;
using System.Runtime.Serialization;
using ICSStudio.Gui.Converters;
using ICSStudio.Gui.PropertyAttribute;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.DataTypes
{
    [DisplayName("MCD Parameters")]
    [TypeConverter(typeof(PropertiesDeluxeTypeConverter))]
    public class MCDParam : ParamBase
    {
        private BooleanTypes _changeAccel;
        private BooleanTypes _changeAccelJerk;
        private BooleanTypes _changeDecel;
        private BooleanTypes _changeDecelJerk;
        private BooleanTypes _changeSpeed;

        public MCDParam()
        {
            MotionType = MotionTypeTypes.Jog;
            ChangeSpeed = BooleanTypes.No;

            Speed = 0;

            ChangeAccel = BooleanTypes.No;
            AccelRate = 100;

            ChangeDecel = BooleanTypes.No;
            DecelRate = 100;

            ChangeAccelJerk = BooleanTypes.No;
            AccelJerk = 100;

            ChangeDecelJerk = BooleanTypes.No;
            DecelJerk = 100;

            SpeedUnits = SpeedUnitsTypes.UnitsPerSec;
            AccelUnits = AccelUnitsTypes.UnitsPerSec2;
            DecelUnits = DecelUnitsTypes.UnitsPerSec2;
            JerkUnits = JerkUnitsTypes.PercentOfTime;
        }

        [PropertyOrder(0)]
        [DisplayName("Motion Type")]
        [ReadOnly(false)]
        public MotionTypeTypes MotionType { get; set; }

        [PropertyOrder(1)]
        [DisplayName("Change Speed")]
        [ReadOnly(false)]
        public BooleanTypes ChangeSpeed
        {
            get { return _changeSpeed; }
            set
            {
                _changeSpeed = value;
                OnChangeSpeedChange();
            }
        }

        [PropertyOrder(2)]
        [DisplayName("Speed")]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public float Speed { get; set; }

        [PropertyOrder(3)]
        [DisplayName("Change Accel")]
        [ReadOnly(false)]
        public BooleanTypes ChangeAccel
        {
            get { return _changeAccel; }
            set
            {
                _changeAccel = value;
                OnChangeAccelChange();
            }
        }

        [PropertyOrder(4)]
        [DisplayName("Accel Rate")]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public float AccelRate { get; set; }

        [PropertyOrder(5)]
        [DisplayName("Change Decel")]
        [ReadOnly(false)]
        public BooleanTypes ChangeDecel
        {
            get { return _changeDecel; }
            set
            {
                _changeDecel = value;
                OnChangeDecelChange();
            }
        }

        [PropertyOrder(6)]
        [DisplayName("Decel Rate")]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public float DecelRate { get; set; }

        [PropertyOrder(7)]
        [DisplayName("Change Accel Jerk")]
        [ReadOnly(false)]
        public BooleanTypes ChangeAccelJerk
        {
            get { return _changeAccelJerk; }
            set
            {
                _changeAccelJerk = value;
                OnJerkChange();
            }
        }

        [PropertyOrder(8)]
        [DisplayName("Accel Jerk")]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public float AccelJerk { get; set; }

        [PropertyOrder(9)]
        [DisplayName("Change Decel Jerk")]
        [ReadOnly(false)]
        public BooleanTypes ChangeDecelJerk
        {
            get { return _changeDecelJerk; }
            set
            {
                _changeDecelJerk = value;
                OnJerkChange();
            }
        }

        [PropertyOrder(10)]
        [DisplayName("Decel Jerk")]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public float DecelJerk { get; set; }


        [PropertyOrder(11)]
        [DisplayName("Speed Units")]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public SpeedUnitsTypes SpeedUnits { get; set; }

        [PropertyOrder(12)]
        [DisplayName("Accel Units")]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public AccelUnitsTypes AccelUnits { get; set; }


        [PropertyOrder(13)]
        [DisplayName("Decel Units")]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public DecelUnitsTypes DecelUnits { get; set; }


        [PropertyOrder(14)]
        [DisplayName("Jerk Units")]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public JerkUnitsTypes JerkUnits { get; set; }

        private void OnChangeSpeedChange()
        {
            switch (ChangeSpeed)
            {
                case BooleanTypes.No:
                    SetPropertyReadOnly("Speed", true);
                    SetPropertyReadOnly("SpeedUnits", true);
                    break;
                case BooleanTypes.Yes:
                    SetPropertyReadOnly("Speed", false);
                    SetPropertyReadOnly("SpeedUnits", false);
                    break;
            }

            OnDynamicPropertyChanged?.Invoke(this);
        }

        private void OnChangeAccelChange()
        {
            switch (ChangeAccel)
            {
                case BooleanTypes.No:
                    SetPropertyReadOnly("AccelRate", true);
                    SetPropertyReadOnly("AccelUnits", true);
                    break;
                case BooleanTypes.Yes:
                    SetPropertyReadOnly("AccelRate", false);
                    SetPropertyReadOnly("AccelUnits", false);
                    break;
            }

            OnDynamicPropertyChanged?.Invoke(this);
        }

        private void OnChangeDecelChange()
        {
            switch (ChangeDecel)
            {
                case BooleanTypes.No:
                    SetPropertyReadOnly("DecelRate", true);
                    SetPropertyReadOnly("DecelUnits", true);
                    break;
                case BooleanTypes.Yes:
                    SetPropertyReadOnly("DecelRate", false);
                    SetPropertyReadOnly("DecelUnits", false);
                    break;
            }

            OnDynamicPropertyChanged?.Invoke(this);
        }

        private void OnJerkChange()
        {
            SetPropertyReadOnly("AccelJerk", ChangeAccelJerk == BooleanTypes.No);
            SetPropertyReadOnly("DecelJerk", ChangeDecelJerk == BooleanTypes.No);

            if ((ChangeAccelJerk == BooleanTypes.No)
                && (ChangeDecelJerk == BooleanTypes.No))
                SetPropertyReadOnly("JerkUnits", true);
            else
                SetPropertyReadOnly("JerkUnits", false);

            OnDynamicPropertyChanged?.Invoke(this);
        }

        #region enum types

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum MotionTypeTypes : uint
        {
            [EnumMember(Value = "Jog")] Jog = 0,
            [EnumMember(Value = "Move")] Move = 1
        }

        public enum BooleanTypes : uint
        {
            No = 0,
            Yes = 1
        }

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum SpeedUnitsTypes : uint
        {
            [EnumMember(Value = "Units per sec")] UnitsPerSec = 0,
            [EnumMember(Value = "% of Maximum")] PercentOfMaximum = 1,

            [EnumMember(Value = "Units per Master Unit")]
            UnitsPerMasterUnit = 4
        }

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum AccelUnitsTypes : uint
        {
            [EnumMember(Value = "Units per sec2")] UnitsPerSec2 = 0,
            [EnumMember(Value = "% of Maximum")] PercentOfMaximum = 1,

            [EnumMember(Value = "Units per Master Unit2")]
            UnitsPerMasterUnit2 = 4
        }

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum DecelUnitsTypes : uint
        {
            [EnumMember(Value = "Units per sec2")] UnitsPerSec2 = 0,
            [EnumMember(Value = "% of Maximum")] PercentOfMaximum = 1,

            [EnumMember(Value = "Units per Master Unit2")]
            UnitsPerMasterUnit2 = 4
        }

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum JerkUnitsTypes : uint
        {
            [EnumMember(Value = "Units per sec3")] UnitsPerSec3 = 0,
            [EnumMember(Value = "% of Maximum")] PercentOfMaximum = 1,
            [EnumMember(Value = "% of Time")] PercentOfTime = 2,

            [EnumMember(Value = "Units per Master Unit3")]
            UnitsPerMasterUnit3 = 4,

            [EnumMember(Value = "% of Time-Master Driven")]
            PercentOfTimeMasterDriven = 6
        }

        #endregion
    }
}
