using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.DeviceProfiles.Common;

namespace ICSStudio.DeviceProfiles.MotionDrive2.Common
{
    public class Identity
    {
        public string CatalogNumber { get; set; }
        public int VendorID { get; set; }
        public int ProductType { get; set; }
        public int ProductCode { get; set; }
        public string ProductName { get; set; }
        public List<Description> Descriptions { get; set; }
        public List<int> MajorRevs { get; set; }
    }
}
