using System.Collections.Generic;

namespace ICSStudio.Database.Table
{
    public class ProductDetail
    {
        public int ID { get; set; }
        public int VendorID { get; set; }
        public string VendorName { get; set; }
        public int ProductType { get; set; }
        public string ProductTypeName { get; set; }
        public int ProductCode { get; set; }
        public string CatalogNumber { get; set; }
        public string Categories { get; set; }
        public string Ports { get; set; }

        // for ui 
        public int[] CategoryArray { get; set; }
        public int[] PortArray { get; set; }
        public string Description { get; set; }

        public bool Contains(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return true;

            string lowerText = searchText.ToLower();

            // CatalogNumber
            if (CatalogNumber != null && CatalogNumber.ToLower().Contains(lowerText))
                return true;
            // Description
            if (Description != null && Description.ToLower().Contains(lowerText))
                return true;
            // Vendor
            if (VendorName != null && VendorName.ToLower().Contains(lowerText))
                return true;

            return false;
        }

        public bool ContainsCategoryFilters(List<int> filters)
        {
            foreach (var i in CategoryArray)
                if (filters.Contains(i))
                    return true;

            return false;
        }

        public bool ContainsVendorFilters(List<int> filters)
        {
            if (filters.Contains(VendorID))
                return true;

            return false;
        }
    }
}
