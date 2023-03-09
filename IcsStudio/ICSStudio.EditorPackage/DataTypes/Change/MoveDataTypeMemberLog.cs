namespace ICSStudio.EditorPackage.DataTypes.Change
{
    public class MoveDataTypeMemberLog : DataTypeChangeLog
    {
        public MoveDataTypeMemberLog(string memberName, string memberDataType, int oldIndex, int newIndex)
        {
            MemberName = memberName;
            MemberDataType = memberDataType;
            OldIndex = oldIndex;
            NewIndex = newIndex;
        }

        public string MemberName { get; }
        public string MemberDataType { get; }
        public int OldIndex { get; }
        public int NewIndex { get; }
    }
}
