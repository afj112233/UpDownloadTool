using System;
using ICSStudio.FileConverter.L5XToJson;
using ICSStudio.SimpleServices.Common;
using System.IO;

namespace ICSStudio.L5XToJsonTest
{
    class Program
    {
        static void Main()
        {/*
            string openFile = "L36_0906.L5X";

            string tmpFileName = "L36_0906_0.json";
            string saveFile = "L36_0906_1.json";
            */
            string openFile = @"C:\Users\zyl\Desktop\question\aa.L5X";
            string saveFile = @"C:\Users\zyl\Desktop\NewSTMAM.json";
            string tmpFileName = Path.GetTempFileName();
            
            try
            {
                Converter.L5XToJson(openFile,tmpFileName);

                var controller = Controller.Open(tmpFileName);
                controller.GenCode();
                controller.Save(saveFile);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
