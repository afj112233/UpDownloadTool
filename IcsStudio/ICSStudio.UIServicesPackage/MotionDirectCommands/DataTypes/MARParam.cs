using System.ComponentModel;
using System.Runtime.Serialization;
using ICSStudio.Gui.Converters;
using ICSStudio.Gui.PropertyAttribute;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.DataTypes
{
    [DisplayName("MAR Parameters")]
    [TypeConverter(typeof(PropertiesDeluxeTypeConverter))]
    public class MARParam : ParamBase
    {
        private WindowedRegistrationTypes _windowedRegistration;

        public MARParam()
        {
            TriggerCondition = TriggerConditionTypes.PositiveEdge;
            WindowedRegistration = WindowedRegistrationTypes.Disabled;

            MinPosition = 0;
            MaxPosition = 0;

            InputNumber = InputNumberTypes.Registration1Position;
        }


        [DisplayName("Trigger Condition")]
        [PropertyOrder(0)]
        [ReadOnly(false)]
        public TriggerConditionTypes TriggerCondition { get; set; }

        [DisplayName("Windowed Registration")]
        [PropertyOrder(1)]
        [ReadOnly(false)]
        public WindowedRegistrationTypes WindowedRegistration
        {
            get { return _windowedRegistration; }
            set
            {
                _windowedRegistration = value;
                OnWindowedRegistrationChange();
            }
        }

        [DisplayName("Min. Position")]
        [PropertyOrder(2)]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public float MinPosition { get; set; }

        [DisplayName("Max. Position")]
        [PropertyOrder(3)]
        [ReadOnly(false)]
        [PropertyAttributesProvider("DynamicPropertyAttributesProvider")]
        public float MaxPosition { get; set; }

        [DisplayName("Input Number")]
        [PropertyOrder(4)]
        [ReadOnly(false)]
        public InputNumberTypes InputNumber { get; set; }

        private void OnWindowedRegistrationChange()
        {
            switch (WindowedRegistration)
            {
                case WindowedRegistrationTypes.Disabled:
                    SetPropertyReadOnly("MinPosition", true);
                    SetPropertyReadOnly("MaxPosition", true);
                    break;
                case WindowedRegistrationTypes.Enabled:
                    SetPropertyReadOnly("MinPosition", false);
                    SetPropertyReadOnly("MaxPosition", false);
                    break;
            }

            OnDynamicPropertyChanged?.Invoke(this);
        }

        #region enum types

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum TriggerConditionTypes : uint
        {
            [EnumMember(Value = "Positive_Edge")] PositiveEdge = 0,
            [EnumMember(Value = "Negative_Edge")] NegativeEdge = 1
        }

        public enum WindowedRegistrationTypes : uint
        {
            Disabled = 0,
            Enabled = 1
        }

        [TypeConverter(typeof(EnumMemberValueConverter))]
        public enum InputNumberTypes : uint
        {
            [EnumMember(Value = "1")] Registration1Position = 1,
            [EnumMember(Value = "2")] Registration2Position = 2
        }

        #endregion
    }
}
