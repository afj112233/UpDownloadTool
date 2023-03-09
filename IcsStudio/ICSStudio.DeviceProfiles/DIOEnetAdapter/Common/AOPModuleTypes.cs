
using System.Collections.Generic;
using ICSStudio.DeviceProfiles.Common;

namespace ICSStudio.DeviceProfiles.DIOEnetAdapter.Common
{
    public class AOPModuleTypes
    {
        public Module Module { get; set; }
        public List<ModuleType> ModuleTypes { get; set; }
        public Bus Bus { get; set; }
        public List<ModuleDefinition> ModuleDefinitions { get; set; }
        public List<ConnectionConfigDefinition> ConnectionConfigDefinitions { get; set; }
        public List<ConnectionDefinition> ConnectionDefinitions { get; set; }
        public List<DataTypeDefinition> DataTypeDefinitions { get; set; }
        public List<DataValueDefinition> DataValueDefinitions { get; set; }
        public List<CIPObject> CIPObjects { get; set; }
        public List<StringDefine> StringDefines { get; set; }
    }
}
