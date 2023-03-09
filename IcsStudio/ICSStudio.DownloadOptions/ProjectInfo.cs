using System;
using System.Collections.Generic;
using ICSStudio.Interfaces.DataType;

namespace ICSStudio.DownloadOptions
{
    namespace ICSStudio.RestoreTest
    {
        public class ProjectInfo
        {
            public string ProjectName { get; set; }
            public string CommunicationPath { get; set; }
            public string User { get; set; }
            public string DownloadTimestamp { get; set; }

            public DataTypeInfo[] DataTypes { get; set; }

            public TagValueInfo[] Tags { get; set; }

            public ProgramInfo[] Programs { get; set; }

            public DataTypeInfo GetDataTypeInfo(string typeName)
            {
                foreach (var dataTypeInfo in DataTypes)
                {
                    if (string.Equals(typeName, dataTypeInfo.Name, StringComparison.OrdinalIgnoreCase))
                        return dataTypeInfo;
                }

                return null;
            }
        }

        public class ProgramInfo
        {
            public string Name { get; set; }
            public TagValueInfo[] Tags { get; set; }
        }

        public class TagValueInfo
        {
            public string DataType { get; set; }
            public string Name { get; set; }
            public byte[] Data { get; set; }
        }

        public class DataTypeInfo
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public FamilyType Family { get; set; }
            public List<DataTypeMemberInfo> Members { get; set; }
        }

        public class DataTypeMemberInfo
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string EngineeringUnit { get; set; }
            public ExternalAccess ExternalAccess { get; set; }
            public string DataType { get; set; }
            public int Dimension { get; set; }
            public DisplayStyle Radix { get; set; }
            public string DisplayName { get; set; }
            public bool Hidden { get; set; }

            public int? BitNumber { get; set; }
            public string Target { get; set; }
        }
    }

}
