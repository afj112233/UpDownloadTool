using System.ComponentModel;
using System.Runtime.Serialization;
using ICSStudio.Gui.Converters;
using ICSStudio.Gui.PropertyAttribute;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ICSStudio.UIServicesPackage.ManualTune.DataTypes
{
    [DisplayName("MAS Parameters")]
    [TypeConverter(typeof(PropertiesDeluxeTypeConverter))]
    public class MASParam : ParamBase
    {
        private BooleanTypes _changeDecel;

        private BooleanTypes _changeDecelJerk;

        private StopTypes _stopType;

        public MASParam()
        {
            StopType = StopTypes.All;
            ChangeDecel = BooleanTypes.No;
            DecelRate = 100;
            DecelUnits = DecelUnitsTypes.UnitsPerSec2;
            ChangeDecelJerk = BooleanTypes.Yes;
            DecelJerk = 100;
            JerkUnits = JerkUnitsTypes.PercentOfMaximum;
            //JerkUnits = JerkUnitsTypes.PercentOfTime;
        }


        [DisplayName("Stop Type")]
        [PropertyOrder(0)]
        [ReadOnly(false)]
        public StopTypes StopType
        {
            get { return _stopType; }
            set
            {
                _stopType = value;
                UpdateReadOnlyProperty();
            }
        }

        [DisplayName("Change Decel")]
        [PropertyOrder(1)]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public BooleanTypes ChangeDecel
        {
            get { return _changeDecel; }
            set
            {
                _changeDecel = value;
                UpdateReadOnlyProperty();
            }
        }


        [DisplayName("Decel Rate")]
        [PropertyOrder(2)]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public float DecelRate { get; set; }

        [DisplayName("Decel Units")]
        [PropertyOrder(3)]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public DecelUnitsTypes DecelUnits { get; set; }

        [DisplayName("Change Decel Jerk")]
        [PropertyOrder(4)]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public BooleanTypes ChangeDecelJerk
        {
            get { return _changeDecelJerk; }
            set
            {
                _changeDecelJerk = value;
                UpdateReadOnlyProperty();
            }
        }


        [DisplayName("Decel Jerk")]
        [PropertyOrder(5)]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public float DecelJerk { get; set; }

        [DisplayName("Jerk Units")]
        [PropertyOrder(6)]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public JerkUnitsTypes JerkUnits { get; set; }

        private void UpdateReadOnlyProperty()
        {
            var isChangeDecelReadOnly = false;
            var isChangeDecelJerkReadOnly = false;

            switch (StopType)
            {
                case StopTypes.All:
                    isChangeDecelJerkReadOnly = true;
                    break;

                case StopTypes.Gear:
                    isChangeDecelJerkReadOnly = true;
                    break;

                //case StopTypes.Tune:
                //    isChangeDecelJerkReadOnly = true;
                //    break;

                //case StopTypes.Test:
                //    isChangeDecelJerkReadOnly = true;
                //    break;

                case StopTypes.TimeCam:
                    isChangeDecelJerkReadOnly = true;
                    break;

                case StopTypes.PositionCam:
                    isChangeDecelJerkReadOnly = true;
                    break;

                //case StopTypes.DirectControl:
                //    isChangeDecelReadOnly = true;
                //    isChangeDecelJerkReadOnly = true;
                //    break;
            }

            SetPropertyReadOnly("ChangeDecel", isChangeDecelReadOnly);
            if (!isChangeDecelReadOnly && (ChangeDecel == BooleanTypes.Yes))
            {
                SetPropertyReadOnly("DecelRate", false);
                SetPropertyReadOnly("DecelUnits", false);
            }
            else
            {
                SetPropertyReadOnly("DecelRate", true);
                SetPropertyReadOnly("DecelUnits", true);
            }

            SetPropertyReadOnly("ChangeDecelJerk", isChangeDecelJerkReadOnly);
            if (!isChangeDecelJerkReadOnly && (ChangeDecelJerk == BooleanTypes.Yes))
            {
                SetPropertyReadOnly("DecelJerk", false);
                SetPropertyReadOnly("JerkUnits", false);
            }
            else
            {
                SetPropertyReadOnly("DecelJerk", true);
                SetPropertyReadOnly("JerkUnits", true);
            }

            OnDynamicPropertyChanged?.Invoke(this);
        }

        #region enum types

        public enum BooleanTypes : uint
        {
            No = 0,
            Yes = 1
        }

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum DecelUnitsTypes : uint
        {
            [EnumMember(Value = "Units per sec2")] UnitsPerSec2 = 0,
            [EnumMember(Value = "% of Maximum")] PercentOfMaximum = 1
        }


        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum JerkUnitsTypes : uint
        {
            [EnumMember(Value = "Units per sec3")] UnitsPerSec3 = 0,
            [EnumMember(Value = "% of Maximum")] PercentOfMaximum = 1,
            //[EnumMember(Value = "% of Time")] PercentOfTime = 2
        }

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum StopTypes : uint
        {
            All = 0,
            Jog = 1,
            Move = 2,
            Gear = 3,
            Home = 4,

            //Tune = 5,
            //Test = 6,
            [EnumMember(Value = "Time Cam")] TimeCam = 7,
            [EnumMember(Value = "Position Cam")] PositionCam = 8,

            //[EnumMember(Value = "Master Offset Move")]
            //MasterOffsetMove = 9,
            //[EnumMember(Value = "Direct Control")] DirectControl = 10
        }

        #endregion
    }
}
