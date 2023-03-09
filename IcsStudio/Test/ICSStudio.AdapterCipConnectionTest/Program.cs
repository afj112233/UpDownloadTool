using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Cip.DataTypes;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.CipConnection;
using Newtonsoft.Json;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace ICSStudio.AdapterCipConnectionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //var conn = new TimeSyncConnection("192.168.1.100");

            //var r = conn.SendSystemTimeMicrosecondsRequest();

            //Console.WriteLine($"response:{conn.ConvertIdentity()}");

            //var conn2=new GetAttributesAllConnection("192.168.1.217");

            //var response2 = conn2.SendAndParseAttributesAllResponse();

            //Console.WriteLine($"conn2.IsOnline:{conn2.IsOnline}");

            //var response = conn2.SendAndParseAttributesAllResponse();

            //Console.ReadKey();

            var conn=new DeviceConnection("192.168.1.81");

            var r = conn.OnLine(false);

            Console.WriteLine($"conn.IsOnline:{r.Result != -1}");

            var timeSync = new CIPTimeSync(1, conn);

            var r1 = timeSync.GetAllTimeSyncInfo();

            Console.WriteLine($"timeSync.GetAllTimeSyncInfo:{r1.Result == 1}");

            var cipIdentity = new CIPIdentity(1, conn);

            var r2 = cipIdentity.GetAttributesAll();

            Console.WriteLine($"cipIdentity.GetAttributesAll:{r2.Result == 1}");

            Console.ReadKey();
        }
    }
}
