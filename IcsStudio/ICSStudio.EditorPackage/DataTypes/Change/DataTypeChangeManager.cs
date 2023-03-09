using System.Collections.Generic;
using ICSStudio.Interfaces.DataType;

namespace ICSStudio.EditorPackage.DataTypes.Change
{
    internal class DataTypeChangeManager
    {
        private readonly List<DataTypeChangeLog> _logs;

        public DataTypeChangeManager(IDataType dataType)
        {
            DataType = dataType;

            _logs = new List<DataTypeChangeLog>();
        }

        public IDataType DataType { get; }

        public DataTypeChangeLog[] Logs => _logs.ToArray();

        public void AddLog(DataTypeChangeLog log)
        {
            if (log != null)
                _logs.Add(log);
        }

        public void ResetLog()
        {
            _logs.Clear();
        }
    }
}
