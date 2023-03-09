using System;
using System.IO;
using System.Linq;
using ICSStudio.FileConverter.L5XToJson;
using ICSStudio.SimpleServices.Common;
using System.Diagnostics;

namespace ICSStudio.ConfigGen
{
    class Program
    {
        // ReSharper disable once UnusedParameter.Local
        static void Main(string[] args)
        {
            string rootPath = @"..\..\data";

            string inputRoot = rootPath + @"\L5XToJsonInputs";
            string outputRoot = rootPath + @"\L5XToJsonChecked";
            string codeRoot = rootPath + @"\JsonGenCode";

            Console.WriteLine($"Clearing {outputRoot}");
            ClearFolder(outputRoot);
            Console.WriteLine($"Clearing {codeRoot}");
            ClearFolder(codeRoot);

            var root = new DirectoryInfo(inputRoot);
            foreach (FileInfo f in root.GetFiles("*.L5X"))
            {
              //  try
                {
                    Console.WriteLine($"Processing {f.Name}");
                    var prefix = f.Name.Substring(0, f.Name.Count() - 4);
                    var inputPath = inputRoot + @"\" + f.Name;
                    var outputPath = outputRoot + @"\" + prefix + ".json";

                    Converter.L5XToJson(inputPath, outputPath);

                    var ctrl = Controller.Open(outputPath);
                    ctrl.GenCode();
                    var codePath = codeRoot + @"\" + prefix + ".json";
                    ctrl.Save(codePath, false);
                }
               // catch (Exception e)
                {
               //     Console.WriteLine(e.Message + " " + f.Name);
                }
            }


        }

        private static void ClearFolder(string path)
        {
            try
            {
                foreach (var d in Directory.GetFileSystemEntries(path))
                {
                    if (File.Exists(d) && !d.EndsWith(".gitkeep"))
                    {
                        var fi = new FileInfo(d);
                        File.Delete(d);
                    }
                    else
                    {
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }


    }
}
