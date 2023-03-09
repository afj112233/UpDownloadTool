using System.Collections.Generic;
using System.Linq;
using ICSStudio.Database.Table;
using ICSStudio.Utils;

namespace ICSStudio.Database.Database
{
    public class DBHelper : BaseDbHelper
    {
        public DBHelper()
        {
            var dllPath = AssemblyUtils.AssemblyDirectory;
            ConnectionString = $@"Data Source={dllPath}\ModuleProfiles\database.db";
        }

        public string ConnectionString { get; }

        public IEnumerable<ProductDetail> GetAllProducts()
        {
            const string sql = "select * from product_detail";
            return DoQuery<ProductDetail>(sql, ConnectionString);
        }

        public IEnumerable<Category> GetAllCategories()
        {
            const string sql = "select * from category";
            return DoQuery<Category>(sql, ConnectionString);
        }

        public IEnumerable<Description> GetDescriptions(int lcid)
        {
            string sql = $"select * from description where LCID = {lcid}";
            return DoQuery<Description>(sql, ConnectionString);
        }

        // ReSharper disable once InconsistentNaming
        public string GetDescription(int vendorId, int productType, int productCode, int LCID)
        {
            string sql =
                $"select Text from product_desc where VendorID ={vendorId} and ProductType={productType} and ProductCode={productCode} and LCID={LCID}";
            return DoQuery<string>(sql, ConnectionString).SingleOrDefault();
        }

        public string GetVendorName(int vendorId)
        {
            string sql = $"select Name from vendor where ID ={vendorId}";
            return DoQuery<string>(sql, ConnectionString).SingleOrDefault();
        }

        public string GetCatalogNumber(int vendorId, int productType, int productCode)
        {
            string sql =
                $"select CatalogNumber from product where VendorID ={vendorId} and ProductType ={productType} and ProductCode ={productCode}";
            return DoQuery<string>(sql, ConnectionString).SingleOrDefault();
        }
    }
}
