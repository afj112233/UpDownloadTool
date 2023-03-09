using ICSStudio.SimpleServices.Common;

namespace ICSStudio.DataServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var controller = Controller.Open(@"F:\test\trend\trend_test.json");

            var dataServer = controller.CreateDataServer();

            dataServer.StartMonitoring(true, true);

            var dataOperand = dataServer.CreateDataOperand(controller,"inTask1.[8]");
        }
    }
}
