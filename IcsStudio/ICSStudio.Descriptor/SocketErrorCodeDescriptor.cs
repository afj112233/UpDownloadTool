using System.Collections.Generic;

namespace ICSStudio.Descriptor
{
    public class SocketErrorCodeDescriptor
    {
        private static readonly Dictionary<ushort, string> SocketErrorCode;

        static SocketErrorCodeDescriptor()
        {
            SocketErrorCode = new Dictionary<ushort, string>
            {
                {0x0001, "Connection failure"},
                {0x0002, "Resource unavailable"},
                {0x0003, "Invalid value in object specific data parameter of service request"},
                {0x0004, "IOI syntax error"},
                {0x0005, "Class or instance not supported"},
                {0x0006, "Insufficient packet space. Request too large"},
                {0x0007, "Messaging connection lost"},
                {0x0008, "Unsupported service requested"},
                {0x0009, "Parameter error in module configuration"},
                {0x000a, "Attribute in Get_Attributes_list or Set_Attributes_list has non-zero status"},
                {0x000b, "Object already in requested mode or state"},
                {0x000c, "Cannot perform requested service in current mode or state"},
                {0x000d, "Object already exists"},
                {0x000e, "Attribute value isn't allowed to be set"},
                {0x000f, "Access permission denied for requested service"},
                {0x0010, "Mode or State of module doesn't allow object to perform requested service"},
                {0x0011, "Reply data too large"},
                {0x0012, "Requested service specifies operation that is going to fragment primitive data value"},
                {0x0013, "Configuration data size too short"},
                {0x0014, "Undefined or unsupported attribute"},
                {0x0015, "Configuration data size too large"},
                {0x0016, "Object does not exist"},
                {0x0017, "Fragmentation sequence is not active"},
                {0x0018, "Attribute data does not exist"},
                {0x0019, "Failed to set attribute data"},
                {0x001a, "Bridging failure. Requested packet too large for network"},
                {0x001b, "Bridging failure. Response packet too large for network"},
                {0x001c, "Missing attribute for requested service"},
                {0x001d, "Invalid attribute value list returned"},
                {0x001e, "Embedded service resulted in error"},
                {0x001f, "Error processing connection related service"},
                {0x0020, "Invalid Parameter"},
                {0x0021, "Write-once value or medium already written"},
                {0x0022, "Invalid reply received"},

                {0x0025, "Key Segment in IOI string doesn't match destination module"},
                {0x0026, "IOI string is either too large or too small"},
                {0x0027, "Unable to set attribute at this time"},
                {0x0028, "DeviceNet - Invalid member ID"},
                {0x0029, "DeviceNet - Member not settable"},
                {0x002a, "Services not supported. or Attribute not supported/settable"},

                {0x00d0, "Map instance undefined; or Block Transfer timeout"},
                {0x00d1, "Module not in run state; or Block Transfer checksum error"},
                {0x00d2, "Scanner requested either read or write, but Block Transfer module responded with opposite"},
                {0x00d3, "Requested length is different than response length"},

                {0x00f0, "Failed to reset RIO channels. There are still outstanding Block Transfer requests"},
                {0x00f1, "Broadcasting not supported for the selected message type"},
                {0x00f2, "Broadcasting not supported for current system protocol or data size invalid"},
                {0x00f3, "Queues for remote Block Transfers are full"},

                {0x00f5, "No communication channels are configured for requested rack or slot"},
                {0x00f6, "No communication channels are configured for remote I/O"},
                {0x00f7, "Block Transfer timed out before completion"},
                {0x00f8, "Error in Block Transfer protocol. Unsolicited Block Transfer"},
                {0x00f9, "Block Transfer data was lost due to invalid communication channel."},
                {0x00fa, "BT module requested different length than BT instruction."},
                {0x00fb, "Message port not supported; or Block Transfer check sum error"},
                {0x00fc, "Message data type not supported; or Block Transfer data transfer error"},
                {0x00fd, "Message uninitialized; or Block Transfer data block too large for module"},
                {0x00fe, "Message timeout"},
                {0x00ff, "General Error"},

                {0x0100, "Connection in use"},

            };


        }

        public static string GetErrorText(ushort errorCode)
        {
            if (SocketErrorCode.ContainsKey(errorCode))
                return SocketErrorCode[errorCode];

            return "An unknown error occurred";
        }
    }
}
