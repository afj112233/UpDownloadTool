using System;
using System.Collections.Generic;
using System.Threading;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Online;

namespace ICSStudio.MajorFaultTest
{
    class Program
    {
        static void Main()
        {
            Controller controller = Controller.GetInstance();

            //1. connect
            int result = controller.ConnectAsync("192.168.1.222").GetAwaiter().GetResult();
            if (result == 0)
            {
                var cipMessager = controller.CipMessager;

                MajorFaultHelper helper = new MajorFaultHelper(cipMessager);

                List<MajorFaultInfo> infos = new List<MajorFaultInfo>();
                helper.GetAllMajorFaultInfos(infos).GetAwaiter().GetResult();

                foreach (var majorFaultInfo in infos)
                {
                    Console.WriteLine(majorFaultInfo);
                }

                int count = 0;
                while (count < 10)
                {
                    infos.Clear();

                    helper.GetNextMajorFaultInfos(infos).GetAwaiter().GetResult();

                    foreach (var majorFaultInfo in infos)
                    {
                        Console.WriteLine(majorFaultInfo);
                    }

                    count++;

                    Thread.Sleep(500);
                }

            }

            controller.GoOffline();

            Console.ReadKey();
        }
    }

}
