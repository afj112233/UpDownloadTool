// AUTO GENERATED DONT EDIT
using System;
using System.Collections.Generic;

namespace ICSStudio.Cip.Objects
{
    internal partial class Utils
    {

        public static byte[] SerializeCtrlStatus(CtrlStatus obj) {
            List<byte> res = new List<byte>();

            res.AddRange(BitConverter.GetBytes(obj.Mode));
            res.AddRange(BitConverter.GetBytes(obj.State));
            return res.ToArray();
        }

        public static CtrlStatus DeSerializeCtrlStatus(byte[] data) {
            var res = new CtrlStatus();
            int offset = 0;
            res.Mode = Utils.ToInt8(data, offset);
            offset += 1;
            res.State = Utils.ToInt8(data, offset);
            offset += 1;
            return res;
        }

        public static byte[] SerializeLogIdPair(LogIdPair obj) {
            List<byte> res = new List<byte>();

            res.AddRange(BitConverter.GetBytes(obj.LogId));
            res.AddRange(BitConverter.GetBytes(obj.Hash));
            return res.ToArray();
        }

        public static LogIdPair DeSerializeLogIdPair(byte[] data) {
            var res = new LogIdPair();
            int offset = 0;
            res.LogId = Utils.ToInt64(data, offset);
            offset += 8;
            res.Hash = Utils.ToInt64(data, offset);
            offset += 8;
            return res;
        }

    }

}