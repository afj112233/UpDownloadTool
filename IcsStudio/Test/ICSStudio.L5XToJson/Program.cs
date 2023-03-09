using System;
using System.IO;
using System.Xml;
using AutoMapper;
using ICSStudio.L5XToJson.Objects;
using Newtonsoft.Json;

namespace ICSStudio.L5XToJson
{
    class Program
    {
        static void Main()
        {
            Mapper.Initialize(cfg => { cfg.CreateMap<AB_CIP_Drive_C_2, CIPMotionDriveConfigData>(); });

            string openFile = "L36_0906.L5X";

            string tmpFileName = "L36_0906_0.json";
            string saveFile = "L36_0906_1.json";
            /*
            string openFile = @"C:\Users\chenyuan\Desktop\L36.L5X";
            string saveFile = @"C:\Users\chenyuan\Desktop\sync\json.json";
            string tmpFileName = Path.GetTempFileName();
            */
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(openFile);

                using (var sw = File.CreateText(tmpFileName))
                using (var jw = new JsonTextWriter(sw))
                {
                    jw.Formatting = Newtonsoft.Json.Formatting.Indented;

                    Converter.ToJObject(doc).WriteTo(jw);
                }

                var controller = SimpleServices.Common.Controller.Open(tmpFileName);
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
