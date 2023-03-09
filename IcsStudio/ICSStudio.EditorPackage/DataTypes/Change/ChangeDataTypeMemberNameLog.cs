namespace ICSStudio.EditorPackage.DataTypes.Change
{
    public class ChangeDataTypeMemberNameLog : DataTypeChangeLog
    {
        public ChangeDataTypeMemberNameLog(string oldName, string newName)
        {
            OldName = oldName;
            NewName = newName;
        }

        public string OldName { get; }
        public string NewName { get; }
    }
}
