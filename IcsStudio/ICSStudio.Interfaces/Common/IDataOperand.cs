using System;
using System.ComponentModel;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.Utils.TagExpression;

namespace ICSStudio.Interfaces.Common
{
    public interface IDataOperand : INotifyPropertyChanged
    {
        IDataServer DataServer { get; }

        ITagCollection TagCollection { get; }

        DataTypeInfo DataTypeInfo { get; }

        ITag Tag { get; }

        IField Field { get; }

        bool IsMonitoring { get; }

        bool BoolValue { get; }

        int Int32Value { get; }

        short Int16Value { get; }

        sbyte Int8Value { get; }

        uint UInt32Value { get; }

        ushort UInt16Value { get; }

        byte UInt8Value { get; }

        double DoubleValue { get; }

        string FormattedValueString { get; }

        object OriginalValue { get; }

        bool IsValueValid { get; }

        bool IsOperandValid { get; }

        bool IsAtomic { get; }

        bool IsString { get; }

        bool IsForced { get; }

        bool BoolForcedValue { get; }

        bool IsBool { get; }

        bool IsConstant { get; }

        Usage Usage { get; }

        event EventHandler DisplayStyleChanged;

        TagExpressionBase GetTagExpression();

        byte[] GetRawValue();

        void SetOperandString(string operand);

        void SetOperandString(string operand, ITagCollection tagCollection);

        void SetOperandString(
            string operand,
            ITagCollection tagCollection,
            bool allowPrivateMemberReferences);

        void SetOperandString(
            string operand,
            ITagCollection tagCollection,
            bool allowPrivateMemberReferences,
            ITagDataContext dataContext);

        void SetValue(string tagValue);

        void SetValue(string tagValue, bool allowPrivateMemberReferences);

        void ForceUpdate();

        void StartMonitoring(bool monitorValue, bool monitorAttribute);

        void StopMonitoring(bool stopMonitorValue, bool stopMonitorAttribute);

        bool CheckClientChangeValueAccess(ITagDataContext dataContext);

        //event EventHandler<DataOperandAttributeChangedEventArgs> OperandAttributeChanged;
    }

    public class DataOperandAttributeChangedEventArgs : EventArgs
    {
        protected DataOperandAttributeChangedEventArgs()
        {
            ChangeFlags = 0L;
        }

        public DataOperandAttributeChangedEventArgs(long changeFlags)
        {
            ChangeFlags = changeFlags;
        }

        public long ChangeFlags { get; }
    }
}
