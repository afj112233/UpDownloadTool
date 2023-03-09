using System.Collections.Generic;
using ICSStudio.DeviceProfiles.DIOModule.Common;

namespace ICSStudio.DeviceProfiles.Generic
{
    public class CommMethod
    {
        public string ID { get; set; }
        public string Description { get; set; }
        public IOTag InputTag { get; set; }
        public IOTag OutputTag { get; set; }
        public string DefaultConfigOptionsID { get; set; }

        public List<ConfigOption> ConfigOptions { get; set; }

        public ConfigSize PrimaryConnectionInputSize { get; set; }
        public ConfigSize PrimaryConnectionOutputSize { get; set; }

        public ConfigOption GetConfigOptionByID(string configOptionsID)
        {
            foreach (var configOption in ConfigOptions)
            {
                if (configOption.ID.Equals(configOptionsID))
                    return configOption;
            }

            return null;
        }
    }
}
