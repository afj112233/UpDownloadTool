using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Common;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.DataType
{
    public abstract class DataType : BaseComponent, IDataType
    {
        private string _name1;

        protected DataType()
        {
        }

        public override IController ParentController => null;

        public virtual int BitSize => 0;
        public virtual int ByteSize => 0;
        public virtual int AlignSize => 8;
        public virtual int BaseDataSize => ByteSize;

        public override string Name
        {
            set
            {
                if (_name1 != value)
                {
                    OldName = _name1;
                    _name1 = value;
                    RaisePropertyChanged();
                }
            }
            get { return _name1; }
        }

        public virtual FamilyType FamilyType
        {
            get { return FamilyType.NoFamily; }
        }

        public virtual bool IsStringType => false;
        public virtual bool IsIOType => false;
        public virtual bool IsVendorDefinedType { get; set; }
        public virtual bool IsPredefinedType => false;
        public virtual bool IsUDIDefinedType { get; set; }
        public virtual bool IsSupported => true;
        public virtual bool IsAtomic => false;
        public virtual bool IsInteger => false;
        public virtual bool IsNumber => false;
        public virtual bool IsBool => false;
        public virtual bool IsLINT => false;
        public virtual bool IsReal => false;
        public virtual bool IsStruct => false;
        public virtual bool IsGlobalScopeOnly => false;
        public virtual bool IsAtomicOnly => false;
        public virtual bool IsSequenceInteractionType => false;
        public virtual bool SupportsOneDimensionalArray => false;
        public virtual bool SupportsMultiDimensionalArrays => false;
        public virtual bool IsAxisType => false;
        public virtual bool IsMotionGroupType => false;
        public virtual bool IsCoordinateSystemType => false;
        public virtual bool IsMessageType => false;
        public virtual bool IsAlarmDigitalType => false;
        public virtual bool IsAlarmAnalogType => false;
        public virtual DisplayStyle DefaultDisplayStyle => DisplayStyle.Decimal;

        public virtual DisplayStyle[] GetValidDisplayStyles()
        {
            return new[] {DisplayStyle.Decimal};
        }

        public virtual bool IsValidString(string value, ref uint size)
        {
            return false;
        }

        public virtual IField Create(JToken token)
        {
            return null;
        }

        public virtual IField FixDataField(JToken broken)
        {
            var source = Create(null);
            var correct = source?.ToJToken();
            correct?.FixJToken(broken);
            return Create(correct);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
