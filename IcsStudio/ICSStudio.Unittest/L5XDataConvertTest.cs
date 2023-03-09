using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.FileConverter.L5XToJson;
using ICSStudio.SimpleServices.Common;
using Xunit;

namespace Unittest
{
    public class L5XDataTest
    {
        [Fact]
        public void K5XDataConvertTest()
        {
            string rootPath = @"..\..\data";

            string inputRoot = rootPath + @"\AllPredefinedDataType";
            string outputRoot = rootPath + @"\AllPredefinedDataType";
            var root = new DirectoryInfo(inputRoot);
            foreach (FileInfo f in root.GetFiles("*.L5X"))
            {
                Console.WriteLine($"Processing {f.Name}");
                var inputPath = inputRoot + @"\" + f.Name;
                var outputPath = outputRoot + @"\test.json";
                Converter.L5XToJson(inputPath, outputPath);
                Controller.Open(outputPath);
                Console.WriteLine($"Conversion is OK");
            }
        }
    }
}
