using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ICSStudio.SimpleServices.Online;

namespace ICSStudio.LadderGraphOnlineEditTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string jsonFile = "a0902.json";
            try
            {
                // 1.open
                //Converter.L5XToJson(l5xFile, jsonFile);
                Controller controller = Controller.Open(jsonFile);

                // 2.online
                controller.GenCode();
                controller.ConnectAsync("192.168.1.222").GetAwaiter().GetResult();

                // 3.to program mode
                controller.ChangeOperationMode(ControllerOperationMode.OperationModeProgram).GetAwaiter().GetResult();
                controller.UpdateState().GetAwaiter();
                int timeout = 50;
                while (timeout-- > 0)
                {
                    if (controller.OperationMode == ControllerOperationMode.OperationModeProgram)
                        break;

                    controller.ChangeOperationMode(ControllerOperationMode.OperationModeProgram).GetAwaiter().GetResult();
                    controller.UpdateState().GetAwaiter();

                    Thread.Sleep(100);
                }

                if (timeout < 0)
                {
                    Console.WriteLine("Switch to Program Mode Timeout!");
                    Console.ReadKey();
                }

                controller.Download(ControllerOperationMode.OperationModeNull, false, false).GetAwaiter().GetResult();
                controller.RebuildTagSyncControllerAsync().GetAwaiter().GetResult();
                controller.UpdateState().GetAwaiter();

                // 4.to run mode
                controller.ChangeOperationMode(ControllerOperationMode.OperationModeRun).GetAwaiter().GetResult();
                timeout = 50;
                while (timeout-- > 0)
                {
                    if (controller.OperationMode == ControllerOperationMode.OperationModeRun)
                        break;

                    controller.ChangeOperationMode(ControllerOperationMode.OperationModeRun).GetAwaiter().GetResult();
                    controller.UpdateState().GetAwaiter();

                    Thread.Sleep(100);
                }

                if (timeout < 0)
                {
                    Console.WriteLine("Switch to Run Mode Timeout!");
                    Console.ReadKey();
                }

                // 5.change
                var mainProgram = controller.Programs["MainProgram"] as SimpleServices.Common.Program;
                var routine = mainProgram?.Routines["A"] as RLLRoutine;

                routine?.Rungs.RemoveAt(0);

                OnlineEditHelper onlineEditHelper = new OnlineEditHelper(controller.CipMessager);

                var time0 = onlineEditHelper.GetTimestamp().GetAwaiter().GetResult();
                Console.WriteLine($"time:{time0}");

                onlineEditHelper.ReplaceRoutine(routine).GetAwaiter().GetResult();

                // 4.update
                onlineEditHelper.UpdateProgram(mainProgram).GetAwaiter().GetResult();

                var time1 = onlineEditHelper.GetTimestamp().GetAwaiter().GetResult();
                Console.WriteLine($"time:{time1}");

                // 5.offline
                controller.GoOffline();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                Console.ReadKey();
            }

            Console.ReadKey();
        }
    }
}
