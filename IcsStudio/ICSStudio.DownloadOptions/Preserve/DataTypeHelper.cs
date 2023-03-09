using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using ICSStudio.DownloadOptions.ICSStudio.RestoreTest;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.PredefinedType;

namespace ICSStudio.DownloadOptions.Preserve
{
    public class DataTypeHelper
    {
        private readonly Dictionary<string, int> _typeDictionary;

        private readonly DataTypeCollection _dataTypeCollection;

        public DataTypeHelper()
        {
            _typeDictionary = new Dictionary<string, int>()
            {
                { "BOOL", 1 },
                { "COUNTER", 16 },
                //{"CONTROL", 16}, //TODO(gjc): need check 暂不支持
                { "DINT", 4 },
                { "UDINT", 4 },
                { "INT", 2 },
                { "UINT", 2 },
                { "LINT", 8 },
                { "ULINT", 8 },
                //{"PID", 184}, //TODO(gjc): need check 暂不支持
                //{"PIDEAUTOTUNE", 88}, //TODO(gjc): need check 暂不支持
                { "SINT", 1 },
                { "USINT", 1 },
                { "REAL", 4 },
                { "LREAL", 8 },
                { "STRING", 88 },
                { "TIMER", 24 }
            };

            _dataTypeCollection = new DataTypeCollection(null);
        }

        public bool ParseDataType(
            string dataType,
            out string dataTypeName,
            out int dim1, out int dim2, out int dim3,
            out int errorCode)
        {
            dataTypeName = string.Empty;
            dim1 = 0;
            dim2 = 0;
            dim3 = 0;
            errorCode = 0;

            if (string.IsNullOrWhiteSpace(dataType))
            {
                errorCode = -1;
                return false;
            }

            if (!Regex.IsMatch(dataType, @"\A[a-z_][a-z0-9_:]*(\[(\d+,){0,2}\d+\])?\Z",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant))
            {
                errorCode = -2;
                return false;
            }

            Match match = Regex.Match(dataType, @"^[a-z0-9_:]+",
                RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
            dataTypeName = match.Value;


            MatchCollection matchCollection =
                Regex.Matches(
                    dataType.Remove(0, dataTypeName.Length),
                    @"(?<=\W+)\d+",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

            //判断是几维数组
            switch (matchCollection.Count)
            {
                case 1:
                    if (int.TryParse(matchCollection[0].Value, out dim1))
                    {
                        if (dim1 == 0)
                        {
                            errorCode = -5;
                            return false;
                        }
                    }
                    else
                    {
                        errorCode = -5;
                        return false;
                    }

                    break;
                case 2:
                    if (int.TryParse(matchCollection[1].Value, out dim1) &&
                        int.TryParse(matchCollection[0].Value, out dim2))
                    {
                        if (dim1 == 0 || dim2 == 0)
                        {
                            errorCode = -5;
                            return false;
                        }
                    }
                    else
                    {
                        errorCode = -5;
                        return false;
                    }

                    break;
                case 3:

                    if (int.TryParse(matchCollection[2].Value, out dim1) &&
                        int.TryParse(matchCollection[1].Value, out dim2) &&
                        int.TryParse(matchCollection[0].Value, out dim3))
                    {
                        if (dim1 == 0 || dim2 == 0 || dim3 == 0)
                        {
                            errorCode = -5;
                            return false;
                        }
                    }
                    else
                    {
                        errorCode = -5;
                        return false;
                    }

                    break;
            }

            return true;
        }

        public long GetTagSize(string typeName, int dim1, int dim2, int dim3)
        {
            //获取type的字节大小
            long typeSize = GetTypeSize(typeName);

            //计算数组个数
            int totalDimension = Math.Max(dim1, 1) * Math.Max(dim2, 1) * Math.Max(dim3, 1);

            if (typeName.Equals("BOOL", StringComparison.OrdinalIgnoreCase) && dim1 > 0)
            {
                return totalDimension / 8;
            }

            //计算总字节大小
            return typeSize * totalDimension;
        }

        public bool IsValidTypeForPreserve(string typeName)
        {
            if (_typeDictionary.ContainsKey(typeName.ToUpper()))
                return true;

            return false;
        }

        private int GetTypeSize(string typeName)
        {
            if (_typeDictionary.ContainsKey(typeName.ToUpper()))
            {
                return _typeDictionary[typeName.ToUpper()];
            }

            return 0;
        }

        public void AddDataTypes(DataTypeInfo[] dataTypes)
        {
            if (dataTypes == null)
                return;

            List<DataTypeInfo> unknownSizeList = new List<DataTypeInfo>(dataTypes);

            do
            {
                List<DataTypeInfo> knowSizeList = new List<DataTypeInfo>();

                foreach (var dataTypeInfo in unknownSizeList)
                {
                    int size = CalculateTypeSize(dataTypeInfo);

                    if (size > 0)
                    {
                        knowSizeList.Add(dataTypeInfo);
                        AddDataType(dataTypeInfo, size);

                        Debug.WriteLine($"{dataTypeInfo.Name}, size:{size}");
                    }
                }

                if (knowSizeList.Count == 0)
                    break;

                foreach (var dataTypeInfo in knowSizeList)
                {
                    unknownSizeList.Remove(dataTypeInfo);
                }

            } while (true);

        }

        private void AddDataType(DataTypeInfo dataTypeInfo, int size)
        {
            if (!_typeDictionary.ContainsKey(dataTypeInfo.Name.ToUpper()))
            {
                _typeDictionary.Add(dataTypeInfo.Name.ToUpper(), size);
            }
        }

        private int CalculateTypeSize(DataTypeInfo dataTypeInfo)
        {
            int offset = 0;

            foreach (var dataTypeMemberInfo in dataTypeInfo.Members)
            {
                if (dataTypeMemberInfo.DataType == "BIT")
                    continue;
                if (dataTypeMemberInfo.DataType == "BITFIELD")
                    continue;

                int dim = dataTypeMemberInfo.Dimension;

                int typeSize;
                int alignSize = 8;
                bool isBool = false;
                var dataType = _dataTypeCollection[dataTypeMemberInfo.DataType];
                if (dataType != null)
                {
                    typeSize = dataType.ByteSize;
                    alignSize = dataType.AlignSize;

                    if (dataType is BOOL)
                        isBool = true;
                }
                else if (_typeDictionary.ContainsKey(dataTypeMemberInfo.DataType.ToUpper()))
                {
                    typeSize = _typeDictionary[dataTypeMemberInfo.DataType.ToUpper()];
                }
                else
                {
                    return -1;
                }

                if (isBool && dim != 0)
                    alignSize = 4;

                offset = AlignUp(offset, alignSize);

                int realSize;
                if (isBool && dim != 0)
                {
                    realSize = dim / 8;
                }
                else
                {
                    realSize = dim == 0 ? typeSize : typeSize * dim;
                }

                offset += realSize;
            }

            return AlignUp(offset, 8);
        }

        private int AlignUp(int value, int align)
        {
            return (value + align - 1) / align * align;
        }
    }
}
