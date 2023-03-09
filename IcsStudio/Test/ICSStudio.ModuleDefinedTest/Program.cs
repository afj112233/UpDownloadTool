using System.IO;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DeviceModule;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICSStudio.ModuleDefinedTest
{
    class Program
    {
        static void Main()
        {
            AdapterTest();
        }

        static void AdapterTest()
        {
            const string enrackFile = @"C:\Users\gjc\Desktop\test\io_connection\enrack1029.json";
            //const string rackFile = @"C:\Users\gjc\Desktop\test\io_connection\rack.json";
            //const string lorackFile = @"C:\Users\gjc\Desktop\test\io_connection\lorack.json";

            string fileName = enrackFile;
            string jsonFile = "adapter_enrack.json";

            var controller = Controller.Open(fileName);

            using (var sw = File.CreateText(jsonFile))
            using (var jw = new JsonTextWriter(sw))
            {
                jw.Formatting = Formatting.Indented;

                foreach (var module in controller.DeviceModules)
                {
                    CommunicationsAdapter adapter = module as CommunicationsAdapter;
                    if (adapter != null)
                    {
                        JArray array = new JArray();

                        var inputDataType = adapter.ExportInputDataType();
                        if (inputDataType != null)
                            array.Add(inputDataType);

                        var outputDataType = adapter.ExportOutputDataType();
                        if (outputDataType != null)
                            array.Add(outputDataType);
                        
                        array.WriteTo(jw);
                    }

                }
            }

        }
    }
}
