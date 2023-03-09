using System;
using System.Collections.Generic;

namespace ICSStudio.Cip.DataTypes
{
    public class PaddedEPath
    {
        public PaddedEPath(ushort classId, int instanceId)
        {
            ClassId = classId;
            InstanceId = instanceId;
            AttributeId = new ushort?();
        }

        public PaddedEPath(ushort classId, int instanceId, ushort attributeId)
        {
            ClassId = classId;
            InstanceId = instanceId;
            AttributeId = attributeId;
        }

        public ushort ClassId { get; set; }
        public int InstanceId { get; set; }
        public ushort? AttributeId { get; set; }

        public byte[] ToByteArray()
        {
            var byteList = new List<byte>(16) { 0 };

            // class id
            if (ClassId <= byte.MaxValue)
            {
                byteList.Add(0x20);
                byteList.Add((byte)ClassId);
            }
            else
            {
                byteList.Add(0x21);
                byteList.Add(0x00);
                byteList.Add((byte)(ClassId & 0xFF));
                byteList.Add((byte)(ClassId >> 8));
            }

            // instance id
            if (InstanceId <= byte.MaxValue)
            {
                byteList.Add(0x24);
                byteList.Add((byte)InstanceId);
            }
            else if(InstanceId <= ushort.MaxValue)
            {
                byteList.Add(0x25);
                byteList.Add(0x00);
                byteList.Add((byte)(InstanceId & 0xFF));
                byteList.Add((byte)(InstanceId >> 8));
            }
            else
            {
                byteList.Add(0x26);
                byteList.Add(0x00);
                byteList.AddRange(BitConverter.GetBytes(InstanceId));
            }

            // attribute id
            if (AttributeId.HasValue)
                if (AttributeId.Value <= byte.MaxValue)
                {
                    byteList.Add(0x30);
                    byteList.Add((byte)AttributeId.Value);
                }
                else
                {
                    byteList.Add(0x31);
                    byteList.Add(0x00);
                    byteList.Add((byte)(AttributeId.Value & 0xFF));
                    byteList.Add((byte)(AttributeId.Value >> 8));
                }

            byteList[0] = (byte)((byteList.Count - 1) / 2);

            return byteList.ToArray();
        }
    }
}