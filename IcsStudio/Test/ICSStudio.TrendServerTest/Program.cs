using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using Timer = System.Timers.Timer;
using OxyPlot;

namespace ICSStudio.TrendServerTest
{
    class Program
    {
        private static TrendServer _trendServer;
        private static long _totalCount;
        private static long _count;


        static void Main(string[] args)
        {
            Timer timer = new Timer(300);
            timer.Elapsed += OnTimer;

            //
            var controller = Controller.Open(@"C:\Users\zyl\Desktop\arrayTest.json");
            //controller.ProjectCommunicationPath = "192.168.1.211";
            controller.GenCode();
            controller.ConnectAsync("192.168.1.211").GetAwaiter().GetResult();
            controller.Download(ControllerOperationMode.OperationModeNull, false, false).GetAwaiter().GetResult();
            controller.RebuildTagSyncControllerAsync().GetAwaiter().GetResult();
            controller.UpdateState().GetAwaiter();

            while (true)
            {
                if (controller.OperationMode == ControllerOperationMode.OperationModeProgram)
                    break;

                Thread.Sleep(1000);
            }

            controller.ChangeOperationMode(ControllerOperationMode.OperationModeRun).GetAwaiter().GetResult();

            _trendServer = new TrendServer(controller, 20,
                new List<string>()
                {
                    @"\MainProgram.count"
                });
            _trendServer.Start();

            timer.Start();

            Console.ReadKey();

            timer.Stop();

            _trendServer.Stop();
            controller.GoOffline();
        }

        private static void OnTimer(object sender, ElapsedEventArgs e)
        {
            while (!_trendServer.IsEmpty)
            {
                SampleData sampleData;
                if (_trendServer.TryGetTrendData(out sampleData))
                {
                    _totalCount++;
                    if (sampleData.Values[0].Equals(sampleData.Values[1]))
                    {
                        _count++;

                        Console.WriteLine($"{_count}/{_totalCount} = {(double) _count / _totalCount}");
                    }

                    //Console.WriteLine(sampleData.Time.ToString("o") + " " + string.Join(",", sampleData.Values));
                }
            }
        }
    }
}
