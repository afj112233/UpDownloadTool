using ICSStudio.Database.Table;

namespace ICSStudio.UIServicesPackage.SelectModuleType.Common
{
    public class DisplayModuleItem
    {
        public string CatalogNumber { get; set; }
        public string Description { get; set; }
        public string Vendor { get; set; }
        public string Category { get; set; }
        
        // for use easy
        public ProductDetail ProductDetail { get; set; }
    }
}
