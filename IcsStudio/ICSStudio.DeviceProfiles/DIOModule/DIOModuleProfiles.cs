using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using ICSStudio.DeviceProfiles.Common;
using ICSStudio.DeviceProfiles.DIOModule.Common;

namespace ICSStudio.DeviceProfiles.DIOModule
{
    public class DIOModuleProfiles
    {
        public int VendorID { get; set; }
        public int ProductType { get; set; }
        public int ProductCode { get; set; }
        public string CatalogNumber { get; set; }
        public List<Description> Descriptions { get; set; }
        public List<MajorWithSeries> MajorRevs { get; set; }

        public List<DrivePort> Ports { get; set; }

        public string PSFile { get; set; }

        public DIOModuleTypes DIOModuleTypes { get; set; }

        public IOModuleTypeVariant GetDefaultModuleType()
        {
            Contract.Assert(DIOModuleTypes != null);

            IOModuleType ioModuleType = DIOModuleTypes.GetIOModuleType(VendorID, ProductType, ProductCode);

            if (ioModuleType != null)
            {
                foreach (var typeVariant in ioModuleType.Variants)
                {
                    if (typeVariant.Default)
                        return typeVariant;
                }
            }

            return null;
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

        public Tuple<uint, uint?, string> GetConnectionConfig(
            string moduleDefinitionID, string connectionID)
        {
            Contract.Assert(DIOModuleTypes != null);

            var moduleDefinition = DIOModuleTypes.GetModuleDefinition(moduleDefinitionID);
            if (moduleDefinition != null)
            {
                foreach (var ioConnectionChoice in moduleDefinition.Connection.Choices)
                {
                    if (ioConnectionChoice.ID == connectionID)
                        return new Tuple<uint, uint?, string>(ioConnectionChoice.ConfigID,
                            ioConnectionChoice.CommMethod, ioConnectionChoice.DataFormat);
                }
            }

            return null;
        }

        public string GetDescription(int lcid = 1033)
        {
            foreach (var description in Descriptions)
                if (description.LCID == lcid)
                    return description.Text;

            return string.Empty;
        }

        public string GetConnectionStringByConfigID(uint connectionConfigID, int major, int lcid = 1033)
        {
            var moduleDefinition = GetModuleDefinition(major);
            if (moduleDefinition != null)
            {
                var stringDefine = GetStringDefine(moduleDefinition.StringsID);

                var commMethodStringID = -1;
                foreach (var choice in moduleDefinition.Connection.Choices)
                    if (choice.ConfigID == connectionConfigID)
                    {
                        commMethodStringID = choice.StringID;
                        break;
                    }

                return stringDefine.GetString(commMethodStringID, lcid);

            }

            return string.Empty;
        }

        public string GetDataFormatStringByConfigID(uint connectionConfigID, int major, int lcid = 1033)
        {
            var moduleDefinition = GetModuleDefinition(major);
            if (moduleDefinition != null)
            {
                var stringDefine = GetStringDefine(moduleDefinition.StringsID);

                string dataFormat = string.Empty;
                foreach (var choice in moduleDefinition.Connection.Choices)
                    if (choice.ConfigID == connectionConfigID)
                    {
                        dataFormat = choice.DataFormat;
                        break;
                    }

                int dataFormatStringID = 0;
                if (moduleDefinition.DataFormat?.Choices != null)
                {
                    foreach (var choice in moduleDefinition.DataFormat.Choices)
                    {
                        if (choice.ID == dataFormat)
                        {
                            dataFormatStringID = choice.StringID;
                        }
                    }
                }

                return stringDefine.GetString(dataFormatStringID, lcid);

            }

            return string.Empty;
        }

        public IOModule GetModule(int major)
        {
            IOModuleType ioModuleType = DIOModuleTypes.GetIOModuleType(VendorID, ProductType, ProductCode);

            if (ioModuleType != null)
            {
                foreach (var typeVariant in ioModuleType.Variants)
                {
                    if (typeVariant.MajorRev == major)
                    {
                        return DIOModuleTypes.GetModule(typeVariant.ModuleID);
                    }
                }
            }

            return null;
        }

        private IOModuleDefinition GetModuleDefinition(int major)
        {
            IOModuleType ioModuleType = DIOModuleTypes.GetIOModuleType(VendorID, ProductType, ProductCode);

            if (ioModuleType != null)
            {
                foreach (var typeVariant in ioModuleType.Variants)
                {
                    if (typeVariant.MajorRev == major)
                    {
                        return DIOModuleTypes.GetModuleDefinition(typeVariant.ModuleDefinitionID);
                    }
                }
            }

            return null;
        }

        private StringDefine GetStringDefine(string stingsID)
        {
            foreach (var stringDefine in DIOModuleTypes.StringDefines)
                if (stringDefine.ID == stingsID)
                    return stringDefine;

            return null;
        }

        public List<int> GetSupportMajorListByConnectionConfigID(uint connectionConfigID)
        {
            var ioModuleType = DIOModuleTypes.GetIOModuleType(VendorID, ProductType, ProductCode);
            if (ioModuleType != null)
            {
                List<int> supportList = new List<int>();

                foreach (var ioModuleTypeVariant in ioModuleType.Variants)
                {
                    var moduleDefinition =
                        DIOModuleTypes.GetModuleDefinition(ioModuleTypeVariant.ModuleDefinitionID);
                    if (moduleDefinition != null)
                    {
                        foreach (var choice in moduleDefinition.Connection.Choices)
                        {
                            if (choice.ConfigID == connectionConfigID)
                            {
                                supportList.Add(ioModuleTypeVariant.MajorRev);
                                break;
                            }
                        }
                    }
                }

                return supportList;
            }

            return null;
        }

        public List<uint> GetConnectionConfigIDListByMajor(int major)
        {
            var moduleDefinition = GetModuleDefinition(major);
            if (moduleDefinition != null)
            {
                List<uint> configIDList = new List<uint>();
                foreach (var choice in moduleDefinition.Connection.Choices)
                {
                    configIDList.Add(choice.ConfigID);
                }

                return configIDList;
            }

            return null;
        }

        public uint? GetCommMethodByConfigID(uint connectionConfigID, int major)
        {
            var moduleDefinition = GetModuleDefinition(major);
            if (moduleDefinition != null)
            {
                foreach (var choice in moduleDefinition.Connection.Choices)
                {
                    if (choice.ConfigID == connectionConfigID)
                        return choice.CommMethod;

                }
            }

            return null;
        }
    }
}
