using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using ICSStudio.FileConverter.JsonToL5X.Model;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using Newtonsoft.Json.Linq;

namespace ICSStudio.FileConverter.JsonToL5X
{
    public static partial class Converter
    {
        private static DataTypeType ToDataTypeType(UserDefinedDataType dataType)
        {
            DataTypeType dataTypeType = new DataTypeType();

            Contract.Assert(dataType != null);

            dataTypeType.Name = dataType.Name;
            dataTypeType.Family =
                dataType.FamilyType == FamilyType.StringFamily
                    ? DTFamilyTypeEnum.StringFamily
                    : DTFamilyTypeEnum.NoFamily;
            dataTypeType.Class = DTClassTypeEnum.User;

            //Description
            dataTypeType.Description = ToDescription(dataType.Description);

            //Members
            dataTypeType.Members = ToDataTypeMembers(dataType);

            return dataTypeType;
        }

        private static DataTypeMemberType[] ToDataTypeMembers(UserDefinedDataType dataType)
        {
            JObject dataTypeJObject = dataType.ConvertToJObject();

            if (dataTypeJObject.ContainsKey("Members"))
            {
                var definitionList = dataTypeJObject["Members"]?.ToObject<List<DataTypeMemberDefinition>>();
                if (definitionList != null && definitionList.Count > 0)
                {
                    List<DataTypeMemberType> memberTypes = new List<DataTypeMemberType>();

                    foreach (var definition in definitionList)
                    {
                        DataTypeMemberType memberType = new DataTypeMemberType();

                        Contract.Assert(definition != null);

                        memberType.Name = definition.Name;
                        memberType.DataType = definition.DataType;
                        memberType.Dimension = ushort.Parse(definition.Dimension);
                        memberType.Radix = ToRadix(definition.Radix);
                        memberType.Hidden = definition.Hidden ? BoolEnum.@true : BoolEnum.@false;
                        memberType.ExternalAccess = ToExternalAccess(definition.ExternalAccess);

                        if (!string.IsNullOrEmpty(definition.Target))
                        {
                            memberType.Target = definition.Target;
                            memberType.BitNumber = (ushort) definition.BitNumber;
                        }

                        memberType.Description = ToDescription(definition.Description);

                        memberTypes.Add(memberType);
                    }

                    if (memberTypes.Count > 0)
                        return memberTypes.ToArray();

                }
            }

            return null;

        }

        private static ExternalAccessEnum ToExternalAccess(ExternalAccess externalAccess)
        {
            switch (externalAccess)
            {
                case ExternalAccess.ReadWrite:
                    return ExternalAccessEnum.ReadWrite;

                case ExternalAccess.ReadOnly:
                    return ExternalAccessEnum.ReadOnly;

                case ExternalAccess.None:
                    return ExternalAccessEnum.None;

                default:
                    throw new ArgumentOutOfRangeException(nameof(externalAccess), externalAccess, null);
            }

        }
    }
}
