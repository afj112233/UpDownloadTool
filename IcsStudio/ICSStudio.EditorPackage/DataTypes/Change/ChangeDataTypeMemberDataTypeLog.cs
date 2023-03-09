namespace ICSStudio.EditorPackage.DataTypes.Change
{
    public class ChangeDataTypeMemberDataTypeLog : DataTypeChangeLog
    {
        public ChangeDataTypeMemberDataTypeLog(string memberName, string oldDataType, string newDataType)
        {
            MemberName = memberName;
            OldDataType = oldDataType;
            NewDataType = newDataType;
        }

        public string MemberName { get; }
        public string OldDataType { get; }
        public string NewDataType { get; }
    }
}
