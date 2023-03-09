using System;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Online;

namespace ICSStudio.DateTimeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Controller controller = Controller.GetInstance();

            //1. connect
            int result = controller.ConnectAsync("192.168.1.223").GetAwaiter().GetResult();

            if (result == 0)
            {
                var cipMessager = controller.CipMessager;

                OnlineEditHelper onlineEditHelper = new OnlineEditHelper(cipMessager);

                var timestamp = onlineEditHelper.GetTimestamp().GetAwaiter().GetResult();
                Console.WriteLine($"begin timestamp: {timestamp}");

                DateTime dateTime =
                    new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddTicks(timestamp * 10);

                Console.WriteLine($"{dateTime:yyyy/MM/dd HH:mm:ss}");


                DateTime nowUtc = DateTime.Now.ToUniversalTime();
                timestamp = (nowUtc.Ticks- (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks) / 10;
                Console.WriteLine($"update timestamp: {timestamp}");
                Console.WriteLine($"{nowUtc:yyyy/MM/dd HH:mm:ss}");

                onlineEditHelper.SetTimestamp(timestamp).GetAwaiter();

                //
                timestamp = onlineEditHelper.GetTimestamp().GetAwaiter().GetResult();

                dateTime =
                    new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddTicks(timestamp * 10);

                Console.WriteLine($"end timestamp: {timestamp}");

                Console.WriteLine($"{dateTime:yyyy/MM/dd HH:mm:ss}");
            }

            controller.GoOffline();

            Console.ReadKey();
        }
    }
}
