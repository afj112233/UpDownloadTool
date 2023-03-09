using System.Collections.Generic;
using ICSStudio.DeviceProfiles.Common;

namespace ICSStudio.DeviceProfiles.MotionDrive2.Common
{
    public class PowerStructure
    {
        public int ID { get; set; }
        public bool Integrated { get; set; }
        public string DutySelect { get; set; }
        public string Voltage { get; set; }
        
        public List<Description> CatalogNumber { get; set; }
        public List<Description> Description { get; set; }

        public BusConfiguration BusConfiguration { get; set; }

        public string GetCatalogNumber(int lcid = 1033)
        {
            if (CatalogNumber != null)
            {
                foreach (var descriptionItem in CatalogNumber)
                {
                    if (descriptionItem.LCID == lcid)
                        return descriptionItem.Text;
                }
            }

            return string.Empty;
        }

        public string GetDescription(int lcid = 1033)
        {
            if (Description != null)
            {
                foreach (var descriptionItem in Description)
                {
                    if (descriptionItem.LCID == lcid)
                        return descriptionItem.Text;
                }
            }

            return string.Empty;
        }
    }

    public class BusConfiguration
    {
        public List<BusID> BusCompatibilityID { get; set; }
        public int RelativeRating { get; set; }
        public int MaxBusMasters { get; set; }
        public int MaxBusFollowers { get; set; }
        public int MaxInGroup { get; set; }
    }

    public class BusID
    {
        public int ID { get; set; }
    }
}
