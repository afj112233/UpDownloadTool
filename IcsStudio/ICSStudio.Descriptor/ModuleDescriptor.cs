using System.Collections.Generic;
using System.IO;
using ICSStudio.SimpleServices.DeviceModule;
using Newtonsoft.Json;

namespace ICSStudio.Descriptor
{
    public class ModuleDescriptor
    {
        private static readonly Dictionary<ushort, string> ModuleStatusCodes;
        private static readonly Dictionary<ushort, string> ModuleFaultCodes;

        private readonly DeviceModule _deviceModule;

        static ModuleDescriptor()
        {
            ModuleStatusCodes = new Dictionary<ushort, string>
            {
                {0x0000, "Standby"},
                {0x1000, "Faulted"},
                {0x2000, "Validating"},
                {0x3000, "Connecting"},
                {0x4000, "Running"},
                {0x5000, "Shutting down"},
                {0x6000, "Inhibited"},
                {0x7000, "Waiting"},
                {0x9000, "Firmware Updating"},
                {0xA000, "Configuring"}
            };

            //
            ModuleFaultCodes = new Dictionary<ushort, string>();

            Stream s =
                typeof(ModuleDescriptor).Assembly.GetManifestResourceStream(
                    "ICSStudio.Descriptor.Files.ModuleFaultCode.json");
            if (s != null)
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    JsonSerializer serializer = new JsonSerializer();

                    serializer.Converters.Add(new DictionaryJsonConverter());

                    ModuleFaultCodes =
                        (Dictionary<ushort, string>) serializer.Deserialize(sr, typeof(Dictionary<ushort, string>));
                }
            }
        }

        public ModuleDescriptor(DeviceModule deviceModule)
        {
            _deviceModule = deviceModule;
        }

        public string Status
        {
            get
            {
                if (_deviceModule != null)
                {
                    var entryStatus = _deviceModule.EntryStatus;
                    var statusCode = (ushort) (entryStatus & 0xF000);

                    if (ModuleStatusCodes.ContainsKey(statusCode))
                        return ModuleStatusCodes[statusCode];

                }

                return "Wrong Status";
            }
        }

        public string ModuleFault
        {
            get
            {
                if (_deviceModule != null)
                {
                    var faultCode = (ushort) _deviceModule.FaultCode;

                    if (ModuleFaultCodes.ContainsKey(faultCode))
                    {
                        return $"(Code 16#{faultCode:x4}) {ModuleFaultCodes[faultCode]}";
                    }

                    return string.Empty;
                }

                return "Wrong Fault";
            }
        }
    }
}
