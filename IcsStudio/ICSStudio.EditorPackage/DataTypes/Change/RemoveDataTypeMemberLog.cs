namespace ICSStudio.EditorPackage.DataTypes.Change
{
    public class RemoveDataTypeMemberLog : DataTypeChangeLog
    {
        public RemoveDataTypeMemberLog(string memberName, string memberDataType)
        {
            MemberName = memberName;
            MemberDataType = memberDataType;
        }

        public string MemberName { get; }
        public string MemberDataType { get; }
    }

}
