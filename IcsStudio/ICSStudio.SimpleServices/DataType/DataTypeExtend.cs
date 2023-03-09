using System.Diagnostics;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.PredefinedType;

namespace ICSStudio.SimpleServices.DataType
{
    public static class DataTypeExtend
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType">original data type</param>
        /// <param name="target"></param>
        /// <param name="isStrict"></param>
        /// <returns></returns>
        public static bool Equal(this IDataType dataType, IDataType target, bool isStrict = false)
        {
            if (dataType.IsBool && target.IsBool) return true;
            if (dataType.FamilyType == FamilyType.StringFamily &&
                target.FamilyType == FamilyType.StringFamily) return true;
            if (dataType == target) return true;
            if (dataType is ArrayTypeDimOne)
            {
                if (target is ArrayTypeDimOne)
                {
                    var type1 = (ArrayTypeDimOne) dataType;
                    var type2 = (ArrayTypeDimOne) target;
                    return type1.type.Equals(type2.type);
                }
                else
                {
                    var arrayType = target as ArrayType;
                    if (arrayType != null &&((ArrayTypeDimOne)dataType).type.Equal(arrayType.Type, isStrict) && arrayType.Dim2 == 0) return true;
                    return false;
                }
            }

            if (dataType is ArrayTypeNormal)
            {
                if (target is ArrayTypeDimOne)
                {
                    return ((ArrayTypeNormal)dataType).Type.Equals(((ArrayTypeDimOne)target).type);
                }
                else if (target is ArrayTypeNormal)
                {
                    return ((ArrayTypeNormal)dataType).Type.Equals(((ArrayTypeNormal)target).Type);
                }
                else
                {
                    var arrayType = target as ArrayType;
                    if (arrayType != null && ((ArrayTypeNormal)dataType).Type.Equal(arrayType.Type, isStrict)) return true;
                    return false;
                }
            }

            if (dataType is ArrayType)
            {
                if (target is ArrayTypeDimOne)
                {
                    return ((ArrayType)dataType).Type.Equals(((ArrayTypeDimOne)target).type);
                }
                else if (target is ArrayTypeNormal)
                {
                    return ((ArrayType)dataType).Type.Equals(((ArrayTypeNormal)target).Type);
                }
                else
                {
                    var arrayType = target as ArrayType;
                    var type = dataType as ArrayType;
                    if (arrayType != null && type.Type.Equals(arrayType.Type) && arrayType.Dim1 <= type.Dim1 &&
                        arrayType.Dim2 <= type.Dim2 && arrayType.Dim3 <= type.Dim3) return true;
                    return false;
                }
            }
            if (!isStrict)
                if (dataType.IsNumber && target.IsNumber)
                    return true;
            if (target is ExpectType)
            {
                foreach (var expectType in ((ExpectType)target).ExpectTypes)
                {
                    if (dataType.Equal(expectType,isStrict)) return true;
                }
            }
            if (target is ExceptType)
            {
                foreach (var exceptType in ((ExceptType)target).ExceptTypes)
                {
                    if (dataType.Equal(exceptType, isStrict)) return false;
                }
            }

            if (dataType is ExpectType)
            {
                foreach (var expectType in ((ExpectType)dataType).ExpectTypes)
                {
                    if (expectType.Equal(target, isStrict)) return true;
                }
            }
            if (dataType is ExceptType)
            {
                foreach (var exceptType in ((ExceptType)dataType).ExceptTypes)
                {
                    if (exceptType.Equal(target, isStrict)) return false;
                }
            }

            if (dataType is AXIS_CIP_DRIVE|| dataType is AXIS_VIRTUAL)
            {
                if (target.GetType().Name == typeof(AXIS_COMMON).Name)
                {
                    return true;
                }
                if (dataType != target) return false;
            }
            else
            {
                if (dataType is AXIS_COMMON && target.IsAxisType || target is AXIS_COMMON && dataType.IsAxisType) return true;
            }
            return GetBaseDataTypeName(dataType,isStrict).Equals(GetBaseDataTypeName(target,isStrict));
        }

        public static bool CheckDataTypeIsUsed(IDataType dataType)
        {
            var controller = Controller.GetInstance();
            if (controller != null)
            {
                foreach (var tag in controller.Tags)
                {
                    if (tag.DataTypeInfo.DataType == dataType)
                    {
                        return true;
                    }

                    if (tag.DataTypeInfo.DataType is CompositiveType)
                    {
                        if (IsMemberContainComponent(tag.DataTypeInfo.DataType as CompositiveType, dataType))
                        {
                            return true;
                        }
                    }
                }

                foreach (var p in controller.Programs)
                {
                    foreach (var t in (p as Program).Tags)
                    {
                        if (t.DataTypeInfo.DataType == dataType)
                        {
                            return true;
                        }

                        if (t.DataTypeInfo.DataType is CompositiveType)
                        {
                            if (IsMemberContainComponent(t.DataTypeInfo.DataType as CompositiveType, dataType))
                            {
                                return true;
                            }
                        }
                    }
                }

                foreach (var d in controller.DataTypes)
                {
                    if (d is CompositiveType && d != dataType&& !(d is AOIDataType))
                    {
                        if (IsMemberContainComponent(d as CompositiveType, dataType))
                        {
                            return true;
                        }
                    }
                }

                foreach (var aoi in controller.AOIDefinitionCollection)
                {
                    foreach (var aoiTag in aoi.Tags)
                    {
                        if (aoiTag.DataTypeInfo.DataType == dataType)
                        {
                            return true;
                        }

                        if (aoiTag.DataTypeInfo.DataType is CompositiveType)
                        {
                            if (IsMemberContainComponent(aoiTag.DataTypeInfo.DataType as CompositiveType, dataType))
                            {
                                return true;
                            }
                        }
                    }
                }

            }

            return false;
        }

        public static string GetBaseDataTypeName(IDataType dataType,bool isStruct)
        {
            if (dataType is UserDefinedDataType|| dataType is AOIDataType) return dataType.Name;
            if (dataType.IsAxisType) return dataType.Name;

            var type = dataType.GetType();
            var baseName = type.Name;
            while (type.BaseType != null)
            {
                if (type.BaseType.Name.Equals("DataType")||type.BaseType.Name.Equals("NativeType"))
                {
                    break;
                }

                if (type.BaseType.Name.Equals("CompositiveType"))
                {
                    if (((CompositiveType) dataType).FamilyType == FamilyType.StringFamily)
                    {
                        if(isStruct&& dataType is UserDefinedDataType)
                            return dataType.Name;
                        return STRING.Inst.Name;
                    }
                    break;
                }

                type = type.BaseType;
                baseName = type.Name;
            }

            return baseName;
        }

        public static bool IsExpected(this ASTExpr expr, IDataType target)
        {
            var refType = target as RefType;
            if (refType != null)
            {
                return refType.type.Equal(target);
            }

            var arrayTypeNormal = target as ArrayTypeNormal;
            if (arrayTypeNormal != null)
            {
                var astName = expr as ASTName;
                if (astName != null)
                {
                    return astName.base_dim1 > 0;
                }

                Debug.Assert(false, "ArrayTypeNormal");
            }

            var arrayTypeDimOne = target as ArrayTypeDimOne;
            if (arrayTypeDimOne != null)
            {
                var astName = expr as ASTName;
                if (astName != null)
                {
                    return astName.base_dim1 > 0 && astName.base_dim2 == 0;
                }

                if (arrayTypeDimOne.AllowNull)
                {
                    var integer2 = expr as ASTInteger;
                    if (integer2 != null)
                    {
                        if (integer2.value == 0) return true;
                        return false;
                    }
                }

                Debug.Assert(false, "ArrayTypeDimOne");
            }

            var integer = expr as ASTInteger;
            if (integer != null)
            {
                if (target.IsBool)
                {
                    return (integer.value == 0 || integer.value == 1);
                }
            }

            var nameNode = expr as ASTName;
            if (nameNode != null)
            {
                return nameNode.Expr.type.Equal(target);
            }

            return expr.type.Equal(target);
        }

        public static bool IsSameType(IDataType currentDataType, IDataType targetDataType)
        {
            var type = currentDataType;
            int count = 0;
            while (!(type.IsPredefinedType||type is AssetDefinedDataType||type is ExceptType||type is ExpectType||type is ZeroType))
            {
                type = (currentDataType as ArrayType)?.Type ?? type;
                type = (type as ArrayTypeDimOne)?.type ?? type;
                type = (type as ArrayTypeNormal)?.Type ?? type;
                type = (type as RefType)?.type ?? type;
                count++;
                if(count < 20)
                {
                    Debug.Assert(true);
                    Controller.GetInstance().Log($"IsSameType() error");
                }
            }

            count = 0;
            var target=  targetDataType;
            while (!(target.IsPredefinedType || target is AssetDefinedDataType || target is ExceptType || target is ExpectType || target is ZeroType))
            {
                target = (currentDataType as ArrayType)?.Type ?? target;
                target = (target as ArrayTypeDimOne)?.type ?? target;
                target = (target as ArrayTypeNormal)?.Type ?? target;
                type = (type as RefType)?.type ?? type; 
                count++;
                if (count < 20)
                {
                    Debug.Assert(true);
                    Controller.GetInstance().Log($"IsSameType() error");
                }
            }

            return type.Equal(target, true);
        }

        public static bool IsMatched(IDataType currentDataType, IDataType targetDataType,bool isStruct=false)
        {
            if (currentDataType == null || targetDataType == null)
            {
                return false;
            }
            var expectDataType = targetDataType as ExpectType;
            var exceptDataType = targetDataType as ExceptType;
            if (expectDataType != null)
            {
                return expectDataType.IsMatched(currentDataType);
            }
            else if (exceptDataType != null)
            {
                return exceptDataType.IsMatched(currentDataType);
            }
            else
            {
                return currentDataType.Equal(targetDataType, isStruct);
            }
        }

        private static bool IsMemberContainComponent(CompositiveType compositiveType, IDataType dataType)
        {
            if (compositiveType == null)
            {
                return false;
            }

            bool flag = false;
            if (compositiveType == dataType)
            {
                return true;
            }

            foreach (var item in compositiveType.TypeMembers)
            {
                if (flag)
                {
                    return true;
                }

                if (item.DataTypeInfo.DataType == dataType)
                {
                    return true;
                }

                if (item.DataTypeInfo.DataType is UserDefinedDataType)
                {
                    flag = IsMemberContainComponent((item.DataTypeInfo.DataType as CompositiveType), dataType);
                }
            }

            if (compositiveType is AOIDataType)
            {
                foreach (var d in (compositiveType as AOIDataType).InOutDataTypes)
                {
                    if (flag) return true;
                    if (d == dataType) return true;
                    if (d is UserDefinedDataType)
                    {
                        flag = IsMemberContainComponent((d as CompositiveType), dataType);
                    }
                }
            }

            return false;
        }

    }
}