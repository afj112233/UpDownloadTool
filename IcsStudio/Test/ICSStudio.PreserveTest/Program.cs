using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using ICSStudio.Cip.Objects;
using ICSStudio.DownloadOptions.ICSStudio.RestoreTest;
using ICSStudio.DownloadOptions.Preserve;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Online;
using Formatting = Newtonsoft.Json.Formatting;

namespace ICSStudio.PreserveTest
{
    static class Program
    {
        static void Main()
        {
            Controller controller = Controller.GetInstance();

            //1. connect
            int result = controller.ConnectAsync("192.168.1.222").GetAwaiter().GetResult();

            if (result == 0)
            {
                var cipMessager = controller.CipMessager;

                OnlineEditHelper onlineEditHelper = new OnlineEditHelper(cipMessager);
                var timestamp = onlineEditHelper.GetTimestamp().GetAwaiter().GetResult();

                DateTime dateTime =  new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddTicks(timestamp * 10);

                DataTypeHelper dataTypeHelper = new DataTypeHelper();
                PreserveHelper preserveHelper = new PreserveHelper(cipMessager, dataTypeHelper);

                ProjectInfo projectInfo = new ProjectInfo();

                projectInfo.ProjectName = new CIPController(0, cipMessager).GetName().GetAwaiter().GetResult();
                projectInfo.CommunicationPath = "192.168.1.222";
                projectInfo.User = Environment.MachineName + @"\" + Environment.UserName;
                projectInfo.DownloadTimestamp =
                    DateTime.Now.ToString("ddd MMM dd HH:mm:ss yyyy", DateTimeFormatInfo.InvariantInfo);

                result = preserveHelper.GetDataTypes(projectInfo).GetAwaiter().GetResult();

                dataTypeHelper.AddDataTypes(projectInfo.DataTypes);

                if (result == 0)
                {
                    //2. get tag value
                    int getAllTagsResult = preserveHelper.GetAllTags(projectInfo).GetAwaiter().GetResult();

                    //3. save json file
                    if (getAllTagsResult == 0)
                    {
                        string jsonFile = @"C:\Users\gjc\Desktop\temp\preserve_result.json";
                        using (var sw = File.CreateText(jsonFile))
                        {
                            JsonSerializer serializer = new JsonSerializer
                            {
                                Formatting = Formatting.Indented
                            };
                            serializer.Serialize(sw, projectInfo);
                        }
                    }
                }

            }

            //4. end
            controller.GoOffline();

        }
    }
}
