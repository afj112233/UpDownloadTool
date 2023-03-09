using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using MessagePack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace ICSStudio.SimpleServices.Online
{
    public class VersionHelper
    {
        internal static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public VersionHelper(ICipMessager messager)
        {
            Messager = messager;
        }

        public ICipMessager Messager { get; }

        public async Task<ControllerVersionInfo> GetVersionInfo()
        {
            try
            {
                CIPController cipController = new CIPController(0, Messager);

                var buffer = await cipController.GetInfo();
                JObject info = FromMsgPack(buffer);

                var versionInfo = info.ToObject<ControllerVersionInfo>();

                return versionInfo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static JObject FromMsgPack(List<byte> data)
        {
            for (int i = 0; i < data.Count; ++i)
            {
                data[i] = (byte)(data[i] ^ 0x5A);
            }

            var obj = (JObject)JsonConvert.DeserializeObject(MessagePackSerializer.ToJson(data.ToArray()));
            return obj;
        }
    }

    public class ControllerVersionInfo
    {
        public int Version { get; set; }
        public string BuildNo { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"Version: {Version}");
            builder.AppendLine($"BuildNo: {BuildNo?.Trim()}");

            return builder.ToString();
        }
    }
}
