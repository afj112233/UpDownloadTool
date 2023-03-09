using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Ports;
using ICSStudio.XmlToJson.Converter;

namespace ICSStudio.XmlToJson
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    class Program
    {
        static void Main()
        {
            //PS1734Test();
            //PS1769_L1YEmbeddedTest();
            //ICONPS1734DiscreteTest();
            ICONPS1734AnalogTest();

            //MotionDriveTest();
            //PSKinetix5000DriveTest();
            //PSICMDriveTest();
            //CIPDriveTest();


            //string inputFolder = @"E:\2020\icsstudio\ICSStudio\Test\ICSStudio.XmlToJson\Motion Drive\RA CIP Motion Kinetix";
            //string outputFolder = @"E:\temp";
            //string searchPattern = "2198-*.xml";

            //MotionDriveTest(inputFolder,outputFolder,searchPattern);
        }



        private static void CIPDriveTest()
        {
            string folder = @"E:\2020\icsstudio\ICSStudio\Test\ICSStudio.XmlToJson\Motion Drive\RA CIP Motion Kinetix\";
            string xmlFile = "2198-H003-ERS.xml";
            string outFolder = @"E:\temp\";
            string jsonFile = "2198-H003-ERS.json";

            CIPDriveConverter.XmlToJson(
                folder + xmlFile, outFolder + jsonFile);
        }

        private static void PSICMDriveTest()
        {
            string folder = @"E:\2020\icsstudio\ICSStudio\Test\ICSStudio.XmlToJson\Motion Drive\I-CON\";
            string xmlFile = "PSICM-D5.xml";
            string outFolder = @"E:\temp\";
            string jsonFile = "PSICM-D5.json";

            PSDriveConverter.XmlToJson(
                folder + xmlFile, outFolder + jsonFile);
        }

        private static void PSKinetix5000DriveTest()
        {
            string folder = @"E:\2020\icsstudio\ICSStudio\Test\ICSStudio.XmlToJson\Motion Drive\RA CIP Motion Kinetix\";
            string xmlFile = "PSKinetix5500Drive.xml";
            string outFolder = @"E:\temp\";
            string jsonFile = "PSKinetix5500Drive.json";

            PSDriveConverter.XmlToJson(
                folder + xmlFile, outFolder + jsonFile);
        }

        private static void MotionDriveTest()
        {
            string inputFolder = @"E:\2020\icsstudio\ICSStudio\Test\ICSStudio.XmlToJson\Motion Drive\I-CON";
            string outputFolder = @"E:\temp";
            var allXmlFiles = Directory.GetFiles(inputFolder, "ICM-D*.xml", SearchOption.AllDirectories);

            foreach (var xmlFile in allXmlFiles)
            {
                string saveFile = outputFolder + "\\" + Path.GetFileNameWithoutExtension(xmlFile) + ".json";

                CIPDriveConverter.XmlToJson(xmlFile, saveFile);
                //var motionDriveProfiles = MotionDriveConverter.DeserializeFromXmlFile(xmlFile);
                //motionDriveProfiles.SerializesJsonFile(saveFile);

                // post handle
                string[] searchWords =
                {
                    "Disable Coast", "Current Decel Disable", "Ramped Decel Disable",
                    "Current Decel Hold", "Ramped Decel Hold"
                };
                string[] replaceWords =
                {
                    "Disable & Coast", "Current Decel & Disable", "Ramped Decel & Disable",
                    "Current Decel & Hold", "Ramped Decel & Hold"
                };

                ReplaceWords(saveFile, searchWords, replaceWords);
            }
        }

        private static void MotionDriveTest(string inputFolder, string outputFolder, string searchPattern)
        {
            var allXmlFiles = Directory.GetFiles(inputFolder, searchPattern, SearchOption.AllDirectories);

            foreach (var xmlFile in allXmlFiles)
            {
                string saveFile = outputFolder + "\\" + Path.GetFileNameWithoutExtension(xmlFile) + ".json";

                CIPDriveConverter.XmlToJson(xmlFile, saveFile);

                // post handle
                string[] searchWords =
                {
                    "Disable Coast", "Current Decel Disable", "Ramped Decel Disable",
                    "Current Decel Hold", "Ramped Decel Hold"
                };
                string[] replaceWords =
                {
                    "Disable & Coast", "Current Decel & Disable", "Ramped Decel & Disable",
                    "Current Decel & Hold", "Ramped Decel & Hold"
                };

                ReplaceWords(saveFile, searchWords, replaceWords);
            }
        }

        private static void PS1734Test()
        {
            string folder = "DIO Discrete//";
            string xmlFile = "PS1734_Discrete.xml";
            string jsonFile = "PS1734_Discrete.json";

            PS1734DiscreteConverter.XmlToJson(
                folder + xmlFile, folder + jsonFile);
        }

        private static void PS1769_L1YEmbeddedTest()
        {
            string folder = "DIO Discrete//";
            string xmlFile = "PS1769_L1YEmbedded.xml";
            string jsonFile = "PS1769_L1YEmbedded.json";

            PS1734DiscreteConverter.XmlToJson(
                folder + xmlFile, folder + jsonFile);
        }

        static void ICONPS1734DiscreteTest()
        {
            string folder = "DIO Discrete//";
            string xmlFile = "ICON_PS1734_Discrete.xml";
            string jsonFile = "ICON_PS1734_Discrete.json";

            PS1734DiscreteConverter.XmlToJson(
                folder + xmlFile, folder + jsonFile);
        }

        static void ICONPS1734AnalogTest()
        {
            string folder = "DIO Analog//";
            string xmlFile = "ICON_PS1734_Analog.xml";
            string jsonFile = "ICON_PS1734_Analog.json";

            PS1734DiscreteConverter.XmlToJson(
                folder + xmlFile, folder + jsonFile);
        }

        public static void ReplaceWords(string fileName, string[] searchWords, string[] replaceWords)
        {
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite))
            {
                var sr = new StreamReader(fs);
                var content = sr.ReadToEnd();

                // replace
                for (var i = 0; i < searchWords.Length; i++)
                    content = content.Replace(searchWords[i], replaceWords[i]);

                fs.Seek(0, SeekOrigin.Begin);
                var sw = new StreamWriter(fs);

                sw.Write(content);
                sw.Flush();

                fs.Close();
            }
        }
    }
}
