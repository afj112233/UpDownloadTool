using ICSStudio.Interfaces.DataType;

namespace ICSStudio.Interfaces.Tags
{
    public struct DataTypeInfo
    {
        public IDataType DataType { get; set; }

        public int Dim1 { get; set; }

        public int Dim2 { get; set; }

        public int Dim3 { get; set; }

        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return DataType != null ? DataType.Uid : base.GetHashCode();
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DataTypeInfo))
                return false;
            return Equals((DataTypeInfo) obj);
        }

        public bool Equals(DataTypeInfo other)
        {
            return DataType == other.DataType && (int) Dim1 == (int) other.Dim1 &&
                   ((int) Dim2 == (int) other.Dim2 && (int) Dim3 == (int) other.Dim3);
        }

        public static bool operator ==(DataTypeInfo dataTypeInfo1, DataTypeInfo dataTypeInfo2)
        {
            return dataTypeInfo1.Equals(dataTypeInfo2);
        }

        public static bool operator !=(DataTypeInfo dataTypeInfo1, DataTypeInfo dataTypeInfo2)
        {
            return !dataTypeInfo1.Equals(dataTypeInfo2);
        }

        public override string ToString()
        {
            if (DataType == null)
                return string.Empty;

            if (Dim3 > 0)
                return $"{DataType.Name}[{Dim3},{Dim2},{Dim1}]";
            if (Dim2 > 0)
                return $"{DataType.Name}[{Dim2},{Dim1}]";
            if (Dim1 > 0)
                return $"{DataType.Name}[{Dim1}]";

            return DataType.Name;
        }
    }
}
