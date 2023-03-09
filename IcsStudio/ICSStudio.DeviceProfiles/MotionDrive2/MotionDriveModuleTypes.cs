using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ICSStudio.DeviceProfiles.Common;
using ICSStudio.DeviceProfiles.MotionDrive2.Common;

namespace ICSStudio.DeviceProfiles.MotionDrive2
{
    public class MotionDriveModuleTypes
    {
        // ModuleTypes
        public List<ModuleType> ModuleTypes { get; set; }

        // Modules
        public List<Module> Modules { get; set; }

        // ModuleDefinitions
        public List<ModuleDefinition> ModuleDefinitions { get; set; }

        // ConnectionConfigDefinitions
        public List<ModuleConnectionConfigDefinition> ConnectionConfigDefinitions { get; set; }

        // ConnectionDefinitions
        public List<ModuleConnectionDefinition> ConnectionDefinitions { get; set; }

        // DataTypeDefinitions
        public List<DataTypeDefinition> DataTypeDefinitions { get; set; }

        // DataValueDefinitions
        public List<DataValueDefinition> DataValueDefinitions { get; set; }

        // EnumDefines
        public List<EnumDefine> EnumDefines { get; set; }

        // StringDefines
        public List<StringDefine> StringDefines { get; set; }


        public ModuleType GetModuleType(
            int vendorID, int productType, int productCode)
        {
            foreach (var moduleType in ModuleTypes)
            {
                if (moduleType.VendorID == vendorID &&
                    moduleType.ProductType == productType &&
                    moduleType.ProductCode == productCode)
                    return moduleType;
            }

            return null;
        }

        public ModuleDefinition GetModuleDefinition(string moduleDefinitionID)
        {
            foreach (var moduleDefinition in ModuleDefinitions)
            {
                if (moduleDefinition.ID == moduleDefinitionID)
                    return moduleDefinition;
            }

            return null;
        }

        public ModuleConnectionConfigDefinition GetConnectionConfigDefinitionByID(uint connectionConfigID)
        {
            foreach (var definition in ConnectionConfigDefinitions)
            {
                if (definition.ConfigID == connectionConfigID)
                    return definition;
            }

            return null;
        }

        public List<byte> GetDataValueByID(string valueID)
        {
            foreach (var definition in DataValueDefinitions)
            {
                if (definition.ID == valueID)
                {
                    return definition.Data.Split(',').Select(x => byte.Parse(x, NumberStyles.AllowHexSpecifier))
                        .ToList();
                }
            }

            return null;
        }

        public DataTypeDefinition GetDataTypeDefinitionByName(string dataType)
        {
            foreach (var definition in DataTypeDefinitions)
            {
                if (definition.Name == dataType)
                    return definition;
            }

            return null;
        }

        public List<int> GetEnumValueList(string enumName)
        {
            List<int> resultList = new List<int>();

            foreach (var define in EnumDefines)
            {
                if (define.ID.Equals(enumName))
                {
                    foreach (var enumValue in define.Values)
                    {
                        if (!resultList.Contains(enumValue.Value))
                            resultList.Add(enumValue.Value);
                    }
                }
            }

            return resultList;
        }
    }
}
