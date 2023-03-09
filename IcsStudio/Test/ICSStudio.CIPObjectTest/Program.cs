using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ICSStudio.Cip.Objects;
using ICSStudio.CipConnection;

namespace ICSStudio.CipObjectTest
{
    class Program
    {
        private const string IpAddress = "192.168.1.211";

        static void Main()
        {
            CIPIdentityGetAttributeSingleTest().IgnorCompletion();

            Console.ReadKey();
        }

        private static async Task CIPIdentityGetAttributeSingleTest()
        {
            DeviceConnection conn = new DeviceConnection(IpAddress);
            int result = await conn.OnLine(true);
            if (result != 0)
            {
                Console.WriteLine($"Connect to {IpAddress} failed!");
                return;
            }

            //CIPIdentity identity = new CIPIdentity(1, conn);
            //while (true)
            //{
            //    await CipBaseObject.GetAttributeSingleWithSendRRData("VendorID", identity, conn);
            //    Console.WriteLine($"Vendor ID:{identity.VendorID}");
            //    await Task.Delay(1000);
            //}
            CIPEthernetLinkObject cipEthernet=new CIPEthernetLinkObject(1,conn);
            while (true)
            {
                await cipEthernet.GetCapability();
                await Task.Delay(1000);
            }
        }

        private static async Task CIPIdentityGetAttributeListTest()
        {
            DeviceConnection deviceConnection = new DeviceConnection(IpAddress);

            int result = await deviceConnection.OnLine(true);
            if (result == 0)
            {
                CIPIdentity identity = new CIPIdentity(1, deviceConnection);

                while (true)
                {
                    var temp = await identity.GetAttributeList(new List<ushort> {1, 2, 3, 4, 5, 6, 7});
                    if (temp == 0)
                    {
                        Console.WriteLine($"Vendor ID:{identity.VendorID}");
                        Console.WriteLine($"Vendor ID:{identity.VendorID}");
                        Console.WriteLine($"Product Code:{identity.ProductCode}");
                        Console.WriteLine($"Revision:{identity.Revision.Major}.{identity.Revision.Minor}");
                        Console.WriteLine($"Status:{identity.Status}");
                        Console.WriteLine($"Serial Number:{identity.SerialNumber}");
                        Console.WriteLine($"Product Name:{identity.ProductName}");
                    }
                    else
                    {
                        Console.WriteLine("GetAttributeList failed!");
                        break;
                    }

                    await Task.Delay(500);
                }

            }
            else
            {
                Console.WriteLine($"Connect to {IpAddress} failed!");
            }


        }

        // ReSharper disable once UnusedMember.Local
        //private static async Task CIPTimeSyncTest()
        //{
        //    DeviceConnection deviceConnection = new DeviceConnection(IpAddress);

        //    int result = await deviceConnection.OnLine(true);
        //    if (result == 0)
        //    {
        //        CIPTimeSync timeSync = new CIPTimeSync(1, deviceConnection);

        //        while (true)
        //        {
        //            var temp = await timeSync.GetAttributeList(new List<ushort>
        //            {
        //                1,
        //                2,
        //                3,
        //                4,
        //                5,
        //                6,
        //                7,
        //                8,
        //                9,
        //                10,
        //                11,
        //                12,
        //                13,
        //                14,
        //                15,
        //                16,
        //                17,
        //                18,
        //                19,
        //                20,
        //                21,
        //                22,
        //                23,
        //                24,
        //                25,
        //                26,
        //                27,
        //                28
        //            });
        //            if (temp == 0)
        //            {
        //                Console.WriteLine($"PTPEnable:{timeSync.PTPEnable}");
        //                Console.WriteLine($"IsSynchronized:{timeSync.IsSynchronized}");
        //                Console.WriteLine($"SystemTimeMicroseconds:{timeSync.SystemTimeMicroseconds}");
        //                Console.WriteLine($"SystemTimeNanoseconds:{timeSync.SystemTimeNanoseconds}");
        //                Console.WriteLine($"OffsetFromMaster:{timeSync.OffsetFromMaster}");
        //                Console.WriteLine($"MaxOffsetFromMaster:{timeSync.MaxOffsetFromMaster}");
        //                Console.WriteLine($"MeanPathDelayToMaster:{timeSync.MeanPathDelayToMaster}");


        //            }
        //            else
        //            {
        //                Console.WriteLine("GetAttributeList failed!");
        //                break;
        //            }

        //            await Task.Delay(500);
        //        }

        //    }
        //    else
        //    {
        //        Console.WriteLine($"Connect to {IpAddress} failed!");
        //    }
        //}
    }

    static class AsyncExtension
    {
        public static void IgnorCompletion(this Task task)
        {
        }
    }
}
