using System.ComponentModel;
using System.Runtime.Serialization;
using ICSStudio.Gui.Converters;
using ICSStudio.Gui.PropertyAttribute;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.DataTypes
{
    [DisplayName("MAG Parameters")]
    [TypeConverter(typeof(PropertiesDeluxeTypeConverter))]
    public class MAGParam : ParamBase
    {
        private ClutchTypes _clutch;

        private RatioFormatTypes _ratioFormat;

        public MAGParam()
        {
            Direction = DirectionTypes.Same;
            Ratio = 1;
            SlaveCounts = 1;
            MasterCounts = 1;
            MasterReference = MasterReferenceTypes.Actual;
            RatioFormat = RatioFormatTypes.Real;
            Clutch = ClutchTypes.Disabled;
            AccelRate = 100;
            AccelUnits = AccelUnitsTypes.UnitsPerSec2;
        }

        [PropertyOrder(0)] [ReadOnly(false)] public DirectionTypes Direction { get; set; }

        [PropertyOrder(1)]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public float Ratio { get; set; }

        [PropertyOrder(2)]
        [DisplayName("Slave Counts")]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public int SlaveCounts { get; set; }

        [PropertyOrder(3)]
        [DisplayName("Master Counts")]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public int MasterCounts { get; set; }

        [PropertyOrder(4)]
        [DisplayName("Master Reference")]
        [ReadOnly(false)]
        public MasterReferenceTypes MasterReference { get; set; }

        [PropertyOrder(5)]
        [DisplayName("Ratio Format")]
        [ReadOnly(false)]
        public RatioFormatTypes RatioFormat
        {
            get { return _ratioFormat; }
            set
            {
                _ratioFormat = value;
                OnRatioFormatChange();
            }
        }

        [PropertyOrder(6)]
        [DisplayName("Clutch")]
        [ReadOnly(false)]
        public ClutchTypes Clutch
        {
            get { return _clutch; }
            set
            {
                _clutch = value;
                OnClutchChange();
            }
        }

        [PropertyOrder(7)]
        [DisplayName("Accel Rate")]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public float AccelRate { get; set; }

        [PropertyOrder(8)]
        [DisplayName("Accel Units")]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public AccelUnitsTypes AccelUnits { get; set; }

        private void OnRatioFormatChange()
        {
            switch (RatioFormat)
            {
                case RatioFormatTypes.Real:
                    SetPropertyReadOnly("Ratio", false);
                    SetPropertyReadOnly("SlaveCounts", true);
                    SetPropertyReadOnly("MasterCounts", true);
                    break;
                case RatioFormatTypes.FractionSlaveMasterCounts:
                    SetPropertyReadOnly("Ratio", true);
                    SetPropertyReadOnly("SlaveCounts", false);
                    SetPropertyReadOnly("MasterCounts", false);
                    break;
            }

            OnDynamicPropertyChanged?.Invoke(this);
        }

        private void OnClutchChange()
        {
            switch (Clutch)
            {
                case ClutchTypes.Disabled:
                    SetPropertyReadOnly("AccelRate", true);
                    SetPropertyReadOnly("AccelUnits", true);
                    break;
                case ClutchTypes.Enabled:
                    SetPropertyReadOnly("AccelRate", false);
                    SetPropertyReadOnly("AccelUnits", false);
                    break;
            }

            OnDynamicPropertyChanged?.Invoke(this);
        }

        #region enum types

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum DirectionTypes : uint
        {
            [EnumMember(Value = "Same")] Same = 0,
            [EnumMember(Value = "Opposite")] Opposite = 1,
            [EnumMember(Value = "Reverse")] Reverse = 2,
            [EnumMember(Value = "Unchanged")] Unchanged = 3
        }

        public enum MasterReferenceTypes : uint
        {
            Actual = 0,
            Command = 1
        }

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum RatioFormatTypes : uint
        {
            [EnumMember(Value = "Real")] Real = 0,

            [EnumMember(Value = "Fraction slave master counts")]
            FractionSlaveMasterCounts = 1
        }

        public enum ClutchTypes : uint
        {
            Enabled = 0,
            Disabled = 1,
        }

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum AccelUnitsTypes : uint
        {
            [EnumMember(Value = "Units per sec2")] UnitsPerSec2 = 0,
            [EnumMember(Value = "% of Maximum")] PercentOfMaximum = 1
        }

        #endregion
    }
}
