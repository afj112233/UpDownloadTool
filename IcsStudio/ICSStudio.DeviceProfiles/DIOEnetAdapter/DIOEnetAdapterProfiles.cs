using System;
using System.Collections.Generic;
using ICSStudio.DeviceProfiles.Common;
using ICSStudio.DeviceProfiles.DIOEnetAdapter.Common;

namespace ICSStudio.DeviceProfiles.DIOEnetAdapter
{
    [Flags]
    public enum DIOConnectionTypeMask : uint
    {
        ListenOnlyRack = 0x80000,
        None = 0x20000,
        Rack = 0x40000,
        EnhancedRack = 0x4000000
    }

    public class DIOEnetAdapterProfiles
    {
        public int VendorID { get; set; }
        public int ProductType { get; set; }
        public int ProductCode { get; set; }
        public string CatalogNumber { get; set; }
        public List<Description> Descriptions { get; set; }
        public List<MajorWithSeries> MajorRevs { get; set; }

        public List<DrivePort> Ports { get; set; }

        public AOPModuleTypes AOPModuleTypes { get; set; }

        public string GetDescription(int lcid = 1033)
        {
            foreach (var description in Descriptions)
                if (description.LCID == lcid)
                    return description.Text;

            return string.Empty;
        }

        private StringDefine GetStringDefine(string stingsID)
        {
            foreach (var stringDefine in AOPModuleTypes.StringDefines)
                if (stringDefine.ID == stingsID)
                    return stringDefine;

            return null;
        }

        private string GetModuleDefinitionID(int majorRev)
        {
            foreach (var moduleType in AOPModuleTypes.ModuleTypes)
                if (moduleType.MajorRev == majorRev)
                    return moduleType.ModuleDefinitionID;

            return string.Empty;
        }

        public uint GetCommMethodByConfigID(uint configID, int majorRev)
        {
            uint commMethod = 0;

            var moduleDefinitionID = GetModuleDefinitionID(majorRev);
            var moduleDefinition = GetModuleDefinition(moduleDefinitionID);
            if (moduleDefinition != null)
            {
                foreach (var choice in moduleDefinition.Connection.Choices)
                    if (choice.ConfigID == configID)
                    {
                        commMethod = choice.CommMethod;
                        break;
                    }
            }

            return commMethod;
        }

        public string GetConnectionStringByConfigID(uint configID, int majorRev, int lcid = 1033)
        {
            var moduleDefinitionID = GetModuleDefinitionID(majorRev);
            var moduleDefinition = GetModuleDefinition(moduleDefinitionID);
            if (moduleDefinition != null)
            {
                var stingsID = moduleDefinition.StringsID;
                var stringDefine = GetStringDefine(stingsID);

                var commMethodStringID = -1;
                foreach (var choice in moduleDefinition.Connection.Choices)
                    if (choice.ConfigID == configID)
                    {
                        commMethodStringID = choice.StringID;
                        break;
                    }

                if (commMethodStringID > 0 && stringDefine != null)
                    foreach (var item in stringDefine.Strings)
                        if (commMethodStringID == item.ID)
                            foreach (var description in item.Descriptions)
                                if (description.LCID == lcid)
                                    return description.Text;
            }

            return string.Empty;
        }

        private ModuleDefinition GetModuleDefinition(string moduleDefinitionID)
        {
            foreach (var moduleDefinition in AOPModuleTypes.ModuleDefinitions)
                if (moduleDefinition.ID == moduleDefinitionID)
                    return moduleDefinition;

            return null;
        }

        public List<int> GetSupportMajorListByConnectionConfigID(uint connectionConfigID)
        {
            List<int> supportList = new List<int>();

            foreach (var moduleType in AOPModuleTypes.ModuleTypes)
            {
                var moduleDefinition = GetModuleDefinition(moduleType.ModuleDefinitionID);
                if (moduleDefinition != null)
                {
                    foreach (var choice in moduleDefinition.Connection.Choices)
                    {
                        if (choice.ConfigID == connectionConfigID)
                        {
                            supportList.Add(moduleType.MajorRev);
                            break;
                        }

                    }
                }
            }

            return supportList;
        }

        public List<uint> GetConnectionConfigIDListByMajor(int majorRev)
        {
            List<uint> connectionConfigIDList = new List<uint>();

            var moduleDefinitionID = GetModuleDefinitionID(majorRev);
            var moduleDefinition = GetModuleDefinition(moduleDefinitionID);
            if (moduleDefinition != null)
            {
                foreach (var choice in moduleDefinition.Connection.Choices)
                {
                    connectionConfigIDList.Add(choice.ConfigID);
                }
            }

            return connectionConfigIDList;
        }

        public ConnectionConfigDefinition GetConnectionConfigDefinitionByID(uint id)
        {
            foreach (var definition in AOPModuleTypes.ConnectionConfigDefinitions)
            {
                if (definition.ConfigID == id)
                    return definition;
            }

            return null;

        }

        public ConnectionDefinition GetConnectionDefinitionByID(string id)
        {
            foreach (var definition in AOPModuleTypes.ConnectionDefinitions)
            {
                if (definition.ID == id)
                    return definition;
            }

            return null;
        }

        public Tuple<uint, uint> GetDefaultConnectionConfigByMajor(int majorRev)
        {
            var moduleDefinitionID = GetModuleDefinitionID(majorRev);
            var moduleDefinition = GetModuleDefinition(moduleDefinitionID);
            if (moduleDefinition != null)
            {
                foreach (var choice in moduleDefinition.Connection.Choices)
                {
                    if (choice.Default)
                        return new Tuple<uint, uint>(choice.ConfigID, choice.CommMethod);
                }
            }


            return null;
        }

        public ModuleType GetDefaultModuleType()
        {
            foreach (var moduleType in AOPModuleTypes.ModuleTypes)
            {
                if (moduleType.Default)
                    return moduleType;
            }

            return AOPModuleTypes.ModuleTypes[0];
        }

        public string GetSeriesByMajor(int major)
        {
            foreach (var majorRev in MajorRevs)
            {
                if (majorRev.MajorRev == major)
                    return majorRev.Series;
            }

            return string.Empty;
        }

        public DataTypeDefinition GetDataTypeDefinitionByName(string dataType)
        {
            foreach (var definition in AOPModuleTypes.DataTypeDefinitions)
            {
                if (definition.Name == dataType)
                    return definition;
            }

            return null;
        }

        public DrivePort GetDrivePort(string portType)
        {
            foreach (var port in Ports)
            {
                if (port.Type == portType)
                    return port;
            }

            return null;
        }
    }
}