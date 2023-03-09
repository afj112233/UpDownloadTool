using System.Collections.Generic;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;

namespace ICSStudio.Interfaces.DataType
{
    public interface IDataTypeCollection : IBaseComponentCollection<IDataType>
    {
        IEnumerable<int> GetAllReferencerDataTypeUids(IDataType dataType);

        IEnumerable<IDataType> GetFamilyDataTypes(FamilyType family);

        bool ParseDataType(
            string dataType,
            out string dataTypeName,
            out int dim1, out int dim2, out int dim3,
            out int errorCode);

        DataTypeInfo ParseDataTypeInfo(string dataType);

        IDataType FindUserDataType(string name,bool isTmp);
    }
}
