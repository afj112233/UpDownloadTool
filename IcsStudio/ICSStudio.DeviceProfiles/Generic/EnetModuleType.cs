using System.Collections.Generic;
using ICSStudio.DeviceProfiles.Common;

namespace ICSStudio.DeviceProfiles.Generic
{
    public class EnetModuleType
    {
        public string ID { get; set; }
        public string CatalogNumber { get; set; }

        public int DefaultUpdateRate { get; set; }
        public int MinimumUpdateRate { get; set; }
        public int MaximumUpdateRate { get; set; }

        public string DefaultCommMethod { get; set; }

        //CommMethods
        public List<CommMethod> CommMethods { get; set; }

        //Configs
        public List<Config> Configs { get; set; }

        //DataTypeDefinitions
        public List<DataTypeDefinition> DataTypeDefinitions { get; set; }

        public Config GetConfigByID(string configID)
        {
            foreach (var config in Configs)
            {
                if (config.ID.Equals(configID))
                    return config;
            }

            return null;
        }

        public DataTypeDefinition GetDataTypeDefinition(string dataType)
        {
            foreach (var dataTypeDefinition in DataTypeDefinitions)
            {
                if (dataTypeDefinition.Name.Equals(dataType))
                    return dataTypeDefinition;
            }

            return null;
        }
    }
}
