using System;
using System.Diagnostics;
using System.IO;
using ICSStudio.DownloadOptions.ICSStudio.RestoreTest;
using ICSStudio.DownloadOptions.Restore;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using Newtonsoft.Json;

namespace ICSStudio.RestoreTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string commPath = "192.168.1.222";
            string projectFile = @"C:\Users\gjc\Desktop\temp\aoi_test.json";
            string projectInfoFile = @"C:\Users\gjc\Desktop\temp\preserve_result.json";

            //1. open controller
            Controller controller = Controller.Open(projectFile);

            //2. download
            if (Download(controller, commPath) == 0)
            {
                //3. create ProjectInfo
                ProjectInfo projectInfo
                    = GetProjectInfoByJsonFile(projectInfoFile);

                //4. restore
                RestoreHelper restoreHelper = new RestoreHelper(controller, projectInfo);
                int result = restoreHelper.Restore();
                if (result < 0)
                {
                    Debug.WriteLine("Restore failed!");
                }

                controller.GoOffline();
            }
        }

        static int Download(Controller controller, string commPath)
        {
            try
            {
                controller.GenCode();
                controller.ConnectAsync(commPath).GetAwaiter().GetResult();
                controller.Download(ControllerOperationMode.OperationModeNull, false, false).GetAwaiter().GetResult();
                controller.RebuildTagSyncControllerAsync().GetAwaiter().GetResult();
                controller.UpdateState().GetAwaiter();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }

            return 0;
        }

        static ProjectInfo GetProjectInfoByJsonFile(string filePath)
        {
            StreamReader fileStream = File.OpenText(filePath);
            JsonSerializer serializer = new JsonSerializer();
            return (ProjectInfo) (serializer.Deserialize(fileStream, typeof(ProjectInfo)));
        }
    }
}
