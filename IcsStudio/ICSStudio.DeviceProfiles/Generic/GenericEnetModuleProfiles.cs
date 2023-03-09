using System.Collections.Generic;
using ICSStudio.DeviceProfiles.Common;

namespace ICSStudio.DeviceProfiles.Generic
{
    public class GenericEnetModuleProfiles
    {
        public int VendorID { get; set; }
        public int ProductType { get; set; }
        public int ProductCode { get; set; }
        public string CatalogNumber { get; set; }
        public List<Description> Descriptions { get; set; }
        public List<int> MajorRevs { get; set; }
        public List<DrivePort> Ports { get; set; }

        public List<EnetModuleType> ModuleProperties { get; set; }

        public CommMethod GetCommMethodByID(string commMethodID)
        {
            EnetModuleType moduleType = ModuleProperties[0];

            foreach (var commMethod in moduleType.CommMethods)
            {
                if (commMethod.ID.Equals(commMethodID))
                    return commMethod;
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
    }
}
