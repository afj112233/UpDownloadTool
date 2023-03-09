using System;
using System.Threading;
using System.Threading.Tasks;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.CipControllerTest
{
    class Program
    {
        private const string IpAddress = "192.168.1.211";

        static void Main()
        {
            try
            {
                for (int i = 0; i < 30; i++)
                {
                    Console.WriteLine($"Index {i}");

                    //ControllerDownloadTest().GetAwaiter().GetResult();

                    ControllerUploadTest().GetAwaiter().GetResult();

                    Console.WriteLine("");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                Console.WriteLine("Test failed!");
            }

            
            //ControllerUploadTest().GetAwaiter().GetResult();
            //Controller.GetInstance().GenCode();
            //Controller.GetInstance().Save(@"C:\Users\chenyuan\Desktop\sync\json2.json");
            Console.WriteLine("Done!");
            Console.ReadKey();
            //while (true)
            //{
            //    Thread.Sleep(100000000);
            //}
        }

        private static async Task ControllerUploadTest()
        {
            var controller = Controller.GetInstance();

            await controller.ConnectAsync(IpAddress);
            await controller.Upload(false);
            controller.GoOffline();
            Thread.Sleep(1000);
        }

        private static async Task ControllerDownloadTest()
        {
            var ctrl = Controller.Open(@"C:\Users\11584\Desktop\ccc.json");

            ctrl.GenCode();

            await ctrl.ConnectAsync(IpAddress);
            
            while (ctrl.OperationMode != ControllerOperationMode.OperationModeProgram)
            {
                await ctrl.ChangeOperationMode(ControllerOperationMode.OperationModeProgram);
                await ctrl.UpdateState();
                await Task.Delay(100);
            }

            await ctrl.Download(Interfaces.Common.ControllerOperationMode.OperationModeNull, false, false);

            await ctrl.RebuildTagSyncControllerAsync();
            await ctrl.UpdateState();

            await ctrl.ChangeOperationMode(ControllerOperationMode.OperationModeRun);

            ctrl.GoOffline();
        }

    }

}
