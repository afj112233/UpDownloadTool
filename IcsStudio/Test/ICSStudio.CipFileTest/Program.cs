using System;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.CipConnection;

namespace ICSStudio.CipFileTest
{

    class Program
    {
        static void Main()
        {
            var deviceConnection = new DeviceConnection("192.168.1.123");
            int result = deviceConnection.OnLine(true).GetAwaiter().GetResult();

            while (true)
            {
                // eds file upload
                CIPFile fileObject = new CIPFile((ushort) FileInstancesNumber.DeviceEDSFile, deviceConnection);
                var fileName = fileObject.DownloadFile("C:\\Users\\chenyuan\\Desktop\\working\\icsstudio\\EDS.txt").GetAwaiter().GetResult();

                //Console.WriteLine(!string.IsNullOrEmpty(fileName) ? fileName : "upload file failed!");

                System.Threading.Thread.Sleep(10000);


            }

            //Console.ReadKey();
        }
    }
}
