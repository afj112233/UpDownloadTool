using System;
using System.Linq;
using System.Threading.Tasks;
using ICSStudio.SimpleServices.Common;
using ICSStudio.FileConverter.L5XToJson;
using System.IO;
using System.Threading;

namespace ICSStudio.InstructionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ControllerDownloadTest().GetAwaiter().GetResult();
            Console.ReadKey();
        }

        private static async Task ControllerUploadTest()
        {
            await Controller.GetInstance().Upload(false);
        }

        private static async Task ControllerDownloadTest()
        {


            string rootPath = @"..\..\data";

            string inputRoot = rootPath + @"\L5XToJsonInputs";
            string outputRoot = rootPath + @"\L5XToJsonChecked";
            string codeRoot = rootPath + @"\JsonGenCode";

            /*
            Console.WriteLine($"Clearing {outputRoot}");
            ClearFolder(outputRoot);
            Console.WriteLine($"Clearing {codeRoot}");
            ClearFolder(codeRoot);
            */
            var root = new DirectoryInfo(inputRoot);

            while (true)
            {
                foreach (FileInfo f in root.GetFiles("*.L5X"))
                {
                    string tmpFileName = Path.GetTempFileName();
                    try
                    {

                        Console.WriteLine($"Processing {f.Name}");
                        var prefix = f.Name.Substring(0, f.Name.Count() - 4);
                        var inputPath = inputRoot + @"\" + f.Name;
                        var outputPath = outputRoot + @"\" + prefix + ".json";

                        Converter.L5XToJson(inputPath, tmpFileName);

                        var ctrl = Controller.Open(tmpFileName);
                        ctrl.GenCode();

                        await Controller.GetInstance().ConnectAsync("192.168.1.123");
                        await Controller.GetInstance()
                            .Download(Interfaces.Common.ControllerOperationMode.OperationModeNull, false, false);
                        /*
                        var codePath = codeRoot + @"\" + prefix + ".json";
                        ctrl.Save(codePath);
                        */
                        Thread.Sleep(4000);
                    }
                    finally
                    {
                        File.Delete(tmpFileName);
                    }
                }
            }

            /*
            var ctrl = Controller.Open(@"C:\Users\chenyuan\Desktop\sync\json.json");
            ctrl.GenCode();
            await Controller.GetInstance().Download(Interfaces.Common.ControllerOperationMode.OperationModeNull, false, false);
            */
        }
    }
}
