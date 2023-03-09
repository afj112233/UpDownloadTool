using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ICSStudio.Cip.EtherNetIP
{
    public enum EncapsulationCommandType : ushort
    {
        NOP = 0x0000, // may be sent only using TCP

        ListServices = 0x0004, // may be sent using either UDP or TCP
        ListIdentity = 0x0063, // may be sent using either UDP or TCP
        ListInterfaces = 0x0064, // optional,may be sent using either UDP or TCP

        RegisterSession = 0x0065, // may be sent only using TCP
        UnRegisterSession = 0x0066, // may be sent only using TCP

        SendRRData = 0x006F, // may be sent only using TCP
        SendUnitData = 0x0070 // may be sent only using TCP
    }

    // vol 2,Table 2-6.3 Item ID Numbers
    public enum CommonPacketFormatItemType : ushort
    {
        NullAddressItem = 0x0000,
        ConnectedAddressItem = 0x00A1,
        ConnectedDataItem = 0x00B1,
        UnconnectedDataItem = 0x00B2

        // TODO(gjc): add more
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct EncapsulationHeader
    {
        public ushort command;
        public ushort length;
        public uint session_handle;
        public uint status;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] public byte[] sender_context;

        public uint options;

        public byte[] GetBytes()
        {
            byte[] result = new byte[24];

            Array.Copy(BitConverter.GetBytes(command), 0, result, 0, 2);
            Array.Copy(BitConverter.GetBytes(length), 0, result, 2, 2);
            Array.Copy(BitConverter.GetBytes(session_handle), 0, result, 4, 4);
            Array.Copy(BitConverter.GetBytes(status), 0, result, 8, 4);
            Array.Copy(sender_context, 0, result, 12, 8);
            Array.Copy(BitConverter.GetBytes(options), 0, result, 20, 4);

            return result;
        }

        public void ParseBytes(byte[] buffer)
        {
            if (buffer.Length >= 24)
            {
                command = BitConverter.ToUInt16(buffer, 0);
                length = BitConverter.ToUInt16(buffer, 2);
                session_handle = BitConverter.ToUInt32(buffer, 4);
                status = BitConverter.ToUInt32(buffer, 8);

                sender_context = new byte[8];
                Array.Copy(buffer, 12, sender_context, 0, 8);

                options = BitConverter.ToUInt32(buffer, 20);

            }
        }

        public string GetSenderContext()
        {
            string context = string.Empty;
            if (sender_context != null)
            {
                foreach (byte b in sender_context)
                {
                    context += b.ToString("X2");
                }
            }
            return context;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CIPUnconnectedMessageDataHeader
    {
        public uint interface_handle;
        public ushort time_out;

        public ushort item_count;

        public ushort address_type_id;
        public ushort address_length;

        public ushort data_type_id;
        public ushort data_length;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CIPConnectedMessageDataHeader
    {
        public uint interface_handle;
        public ushort time_out;

        public ushort item_count;

        public ushort address_type_id;
        public ushort address_length;
        public uint address_data;

        public ushort data_type_id;
        public ushort data_length;
        public ushort sequence_count;
    }


    public class EncapsulationPacket
    {
        public EncapsulationPacket(EncapsulationHeader header, IntPtr data)
        {
            PacketHeader = header;
            if (PacketHeader.length == 0)
                PacketData = null;
            else
            {
                PacketData = new byte[PacketHeader.length];

                Marshal.Copy(data, PacketData, 0, PacketHeader.length);
            }

            // time
            PacketTime = DateTime.MinValue;
        }

        public EncapsulationPacket(EncapsulationHeader header, byte[] data)
        {
            PacketHeader = header;
            if (PacketHeader.length == 0)
                PacketData = null;
            else
            {
                PacketData = new byte[PacketHeader.length];

                Array.Copy(data, PacketData, PacketHeader.length);
            }

            // time
            PacketTime = DateTime.MinValue;
        }

        public byte[] GetBytes()
        {
            byte[] headerBuffer = PacketHeader.GetBytes();
            byte[] result = new byte[headerBuffer.Length + PacketHeader.length];

            Array.Copy(headerBuffer, result, headerBuffer.Length);

            if (PacketData != null)
            {
                Array.Copy(PacketData, 0, result, headerBuffer.Length, PacketHeader.length);
            }

            return result;
        }

        public DateTime PacketTime { get; set; }

        public EncapsulationHeader PacketHeader { get; set; }
        public byte[] PacketData { get; set; }

        public bool TryMatch(EncapsulationPacket packet)
        {
            if (packet == null)
                return false;


            if (PacketHeader.command != packet.PacketHeader.command)
                return false;

            switch (PacketHeader.command)
            {
                case (ushort) EncapsulationCommandType.NOP:
                    break;
                case (ushort) EncapsulationCommandType.ListIdentity:
                    return true;
                case (ushort) EncapsulationCommandType.ListInterfaces:
                    return CompareSenderContext(packet);
                case (ushort) EncapsulationCommandType.RegisterSession:
                    return CompareSenderContext(packet);
                case (ushort) EncapsulationCommandType.UnRegisterSession:
                    break;
                case (ushort) EncapsulationCommandType.ListServices:
                    return CompareSenderContext(packet);
                case (ushort) EncapsulationCommandType.SendRRData:
                    return CompareSenderContext(packet); // TODO(gjc): add session handle check
                case (ushort) EncapsulationCommandType.SendUnitData:
                    return CompareSequenceCount(packet); // TODO(gjc): add session handle check, how to check connection id?  
            }


            return false;
        }

        private bool CompareSequenceCount(EncapsulationPacket packet)
        {
            try
            {
                var sequenceCount0 = BitConverter.ToUInt16(PacketData, 20);
                var sequenceCount1 = BitConverter.ToUInt16(packet.PacketData, 20);

                if (sequenceCount0 == sequenceCount1)
                    return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }

            return false;
        }

        private bool CompareSenderContext(EncapsulationPacket packet)
        {
            for (var i = 0; i < 8; i++)
                if (PacketHeader.sender_context[i] != packet.PacketHeader.sender_context[i])
                    return false;

            return true;
        }
    }
}