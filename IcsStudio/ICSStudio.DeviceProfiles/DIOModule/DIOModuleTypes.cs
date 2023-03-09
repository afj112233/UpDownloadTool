using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ICSStudio.DeviceProfiles.Common;
using ICSStudio.DeviceProfiles.DIOModule.Common;

namespace ICSStudio.DeviceProfiles.DIOModule
{
    /// <summary>
    /// PS1734_Discrete.json
    /// </summary>
    public class DIOModuleTypes
    {
        public List<IOModuleType> ModuleTypes { get; set; }
        public List<IOModule> Modules { get; set; }
        public List<IOModuleDefinition> ModuleDefinitions { get; set; }

        public List<IOConnectionConfigDefinition> ConnectionConfigDefinitions { get; set; }

        public List<IOConnectionDefinition> ConnectionDefinitions { get; set; }

        public List<DataTypeDefinition> DataTypeDefinitions { get; set; }

        public List<DataValueDefinition> DataValueDefinitions { get; set; }
        // add other

        public List<StringDefine> StringDefines { get; set; }

        public IOModuleType GetIOModuleType(
            int vendorID, int productType, int productCode)
        {
            foreach (var ioModuleType in ModuleTypes)
            {
                if (ioModuleType.VendorID == vendorID &&
                    ioModuleType.ProductType == productType &&
                    ioModuleType.ProductCode == productCode)
                    return ioModuleType;
            }

            return null;
        }

        public IOModuleDefinition GetModuleDefinition(string moduleDefinitionID)
        {
            foreach (var ioModuleDefinition in ModuleDefinitions)
            {
                if (ioModuleDefinition.ID == moduleDefinitionID)
                    return ioModuleDefinition;
            }

            return null;
        }

        public IOConnectionConfigDefinition GetConnectionConfigDefinitionByID(uint connectionConfigID)
        {
            foreach (var definition in ConnectionConfigDefinitions)
            {
                if (definition.ConfigID == connectionConfigID)
                    return definition;
            }

            return null;
        }

        public IOConnectionDefinition GetConnectionDefinitionByID(string connectionID)
        {
            foreach (var definition in ConnectionDefinitions)
            {
                if (definition.ID == connectionID)
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

        public IOModule GetModule(string moduleID)
        {
            foreach (var module in Modules)
            {
                if (module.ID == moduleID)
                    return module;
            }

            return null;
        }
    }
}
