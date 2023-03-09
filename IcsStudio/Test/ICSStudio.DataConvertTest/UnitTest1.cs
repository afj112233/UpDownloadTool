using System;
using System.IO;
using System.Linq;
using ICSStudio.FileConverter.L5XToJson;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ICSStudio.DataConvertTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void DataConvertTest()
        {
            string rootPath = @"..\..\data";

            string inputRoot = rootPath + @"\AllPredefinedDataType";
            string outputRoot = rootPath + @"\AllPredefinedDataType";
            var root = new DirectoryInfo(inputRoot);
            foreach (FileInfo f in root.GetFiles("*.L5X"))
            {
                Console.WriteLine($"Processing {f.Name}");
                var prefix = f.Name.Substring(0, f.Name.Count() - 4);
                var inputPath = inputRoot + @"\" + f.Name;
                var outputPath = outputRoot + @"\" + prefix + ".json";
                Converter.L5XToJson(inputPath, outputPath);
                Controller.Open(outputPath);
                Console.WriteLine($"Conversion is OK");
            }
        }
    }
}
