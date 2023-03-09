namespace ICSStudio.EditorPackage.DataTypes.Change
{
    public class AddDataTypeMemberLog : DataTypeChangeLog
    {
        public AddDataTypeMemberLog(string memberName, string memberDataType)
        {
            MemberName = memberName;
            MemberDataType = memberDataType;
        }

        public string MemberName { get; }
        public string MemberDataType { get; }
    }
}
