namespace ICSStudio.EditorPackage.DataTypes.Change
{
    public class ChangeDataTypeNameLog : DataTypeChangeLog
    {
        public ChangeDataTypeNameLog(string oldName, string newName)
        {
            OldName = oldName;
            NewName = newName;
        }

        public string OldName { get; }
        public string NewName { get; }
    }
}
