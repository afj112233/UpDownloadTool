using System.Collections.Generic;

namespace ICSStudio.DeviceProfiles.DIOModule.Common
{
    public class IOModuleType
    {
        public int VendorID { get; set; }
        public int ProductType { get; set; }
        public int ProductCode { get; set; }

        public List<IOModuleTypeVariant> Variants { get; set; }
    }

    public class IOModuleTypeVariant
    {
        public int MajorRev { get; set; }
        public string ModuleID { get; set; }
        public string ModuleDefinitionID { get; set; }
        public bool Default { get; set; }
    }
}
