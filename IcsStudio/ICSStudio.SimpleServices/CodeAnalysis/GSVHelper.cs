using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSStudio.SimpleServices.Properties;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace ICSStudio.SimpleServices.CodeAnalysis
{
    // ReSharper disable once InconsistentNaming
    public class GSVHelper
    {
        static Dictionary<string, ParameterInfo> _parameterInfos;

        public static List<string> GetAccessTypes(string name)
        {
            if (_parameterInfos == null)
                InitInfo();

            var key =
                _parameterInfos?.Keys.FirstOrDefault(p => p.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (key == null)
                return null;

            return _parameterInfos[key].AccessTypes;
        }

        static void InitInfo()
        {
            Stream stream = new MemoryStream(Resources.ParameterTable);
            IWorkbook workbook = new XSSFWorkbook(stream);
            ISheet sheet = workbook.GetSheetAt(0);
            _parameterInfos = new Dictionary<string, ParameterInfo>();
            for (int i = 1; i < sheet.LastRowNum; ++i)
            {
                var row = sheet.GetRow(i);
                if (row == null)
                    continue;

                if (row.Cells.Count < 5)
                    continue;

                string key = row.GetCell(0).ToString();
                string types = row.GetCell(3).ToString();
                List<string> typeList = types.Split('/').ToList();
                _parameterInfos[key] = new ParameterInfo { Name = key, AccessTypes = typeList };
            }
        }
    }

    public class ParameterInfo
    {
        public string Name;
        public List<string> AccessTypes;
        //public float MinValue;
        //public float MaxValue;
    }
}
