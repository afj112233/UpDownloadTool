using System;

namespace ICSStudio.EditorPackage.DataTypes.Change
{
    public abstract class DataTypeChangeLog
    {
        protected DataTypeChangeLog()
        {
            EditTime = DateTime.Now;
        }

        public DateTime EditTime { get; }
    }

}
