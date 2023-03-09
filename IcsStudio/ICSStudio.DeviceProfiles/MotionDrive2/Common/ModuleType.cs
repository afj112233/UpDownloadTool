using System.Collections.Generic;

namespace ICSStudio.DeviceProfiles.MotionDrive2.Common
{
    public class ModuleType
    {
        public int VendorID { get; set; }
        public int ProductType { get; set; }
        public int ProductCode { get; set; }

        public List<ModuleTypeVariant> Variants { get; set; }
    }

    public class ModuleTypeVariant
    {
        public int MajorRev { get; set; }
        public string ModuleID { get; set; }
        public string ModuleDefinitionID { get; set; }
        public bool Default { get; set; }
    }
}
