using System;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Correlate;

namespace ICSStudio.CorrelateTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var controller = Controller.Open(@"E:\2020\icsstudio\build\Debug\json\trend_display.json");
            controller.ProjectCommunicationPath = "192.168.1.211";
            controller.GenCode();
            controller.Download(ControllerOperationMode.OperationModeNull, false, false).GetAwaiter().GetResult();

            ConnectedController connectedController = new ConnectedController(controller.CipMessager);

            connectedController.Upload().GetAwaiter().GetResult();

            ControllerCompare compare = new ControllerCompare();

            var result
                = compare.Compare(controller, connectedController);

            Console.WriteLine(result);
        }
    }
}
