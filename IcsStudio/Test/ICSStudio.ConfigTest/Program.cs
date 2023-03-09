using ICSStudio.FileConverter.L5XToJson;
using ICSStudio.SimpleServices.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSStudio.ConfigTest
{
    class Program
    {
        static void Main(string[] args)
        {

            TestL5XToJson();
            TestCodeGen();
        }


        private static string rootPath = @"..\..\data";

        static JObject ReadJsonFile(string path)
        {
            using (StreamReader file = File.OpenText(path))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject o = (JObject)JToken.ReadFrom(reader);
                    return o;
                }
            }
        }

        public static void TestL5XToJson()
        {
            string inputRoot = rootPath + @"\L5XToJsonInputs";
            string checkRoot = rootPath + @"\L5XToJsonChecked";
            var root = new DirectoryInfo(inputRoot);

            foreach (FileInfo f in root.GetFiles("*.L5X"))
            {
                Console.WriteLine($"Verifying {f.Name}");
                var prefix = f.Name.Substring(0, f.Name.Count() - 4);
                var inputPath = inputRoot + @"\" + f.Name;
                var checkPath = checkRoot + @"\" + prefix + ".json";

                string tmpFileName = Path.GetTempFileName();

                Converter.L5XToJson(inputPath, tmpFileName);

                Debug.Assert(JToken.DeepEquals(ReadJsonFile(checkPath),ReadJsonFile(tmpFileName)));
                File.Delete(tmpFileName);
            }

        }

        public static void TestCodeGen()
        {
            var inputRoot = rootPath + @"\L5XToJsonChecked";
            var checkRoot = rootPath + @"\JsonGenCode";
            var root = new DirectoryInfo(inputRoot);

            foreach (var f in root.GetFiles("*.json"))
            {
                Console.WriteLine($"Verifying {f.Name}");

                var prefix = f.Name.Substring(0, f.Name.Count() - 5);
                var inputPath = inputRoot + @"\" + f.Name;
                var checkPath = checkRoot + @"\" + prefix + ".json";

                var tmpFileName = Path.GetTempFileName();

                var ctrl = Controller.Open(inputPath);
                ctrl.GenCode();
                ctrl.Save(tmpFileName);

                Debug.Assert(JToken.DeepEquals(ReadJsonFile(checkPath),ReadJsonFile(tmpFileName)));
                File.Delete(tmpFileName);

            }
        }
    }
}
