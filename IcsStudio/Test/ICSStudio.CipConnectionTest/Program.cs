using System;
using System.Threading.Tasks;
using ICSStudio.CipConnection;

namespace ICSStudio.CipConnectionTest
{
    class Program
    {
        static void Main()
        {
            var deviceConnection = new DeviceConnection("192.168.1.118");
            deviceConnection.OnLine(true).IgnorCompletion();

            Console.ReadKey();

            deviceConnection.OffLine();


        }
    }

    static class AsyncExtension
    {
        public static void IgnorCompletion(this Task task)
        {
        }
    }
}
