//AUTO GENERATED, DONT EDIT
using ICSStudio.Cip.DataTypes;
using ICSStudio.Cip.EtherNetIP;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ICSStudio.Cip.Objects
{
    public class CIPTagStub : CipBaseObject 
    {
        public CIPTagStub(int instanceId, ICipMessager messager) :
            base(185, instanceId, messager)
        {
        }

        public async Task<List<byte>> GetConfig()
        {
            var size = await GetConfigSize();
            const int CHUNK_SIZE = 512;
            int offset = 0;
            var res = new List<byte>();
            for (int i = 0; i < size/CHUNK_SIZE; ++i) {
                var tmp = await GetConfigData(offset, CHUNK_SIZE);
                res.AddRange(tmp);
                offset += CHUNK_SIZE;
            }
            int left = size % CHUNK_SIZE;
            if (left != 0) {
                var tmp = await GetConfigData(offset, left);
                res.AddRange(tmp);
            }
            return res;
        }
        public async Task<string> GetName()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, InstanceId, 8, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeString(resp.ResponseData);
        }
        public async Task SetName(string param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(Utils.SerializeString(param0));
            var resp = await CallService(16, InstanceId, 8, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task<string> GetTagType()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, InstanceId, 9, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeString(resp.ResponseData);
        }
        public async Task<Int32> GetConfigSize()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, InstanceId, 12, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<List<byte>> GetConfigData(Int32 param0, Int32 param1)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            param.AddRange(BitConverter.GetBytes(param1));
            var resp = await CallService(50, InstanceId, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeListByte(resp.ResponseData);
        }
        public async Task<byte> GetSint(Int32 param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            var resp = await CallService(51, InstanceId, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return resp.ResponseData[0];
        }
        public async Task<Int16> GetInt(Int32 param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            var resp = await CallService(52, InstanceId, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt16(resp.ResponseData, 0);
        }
        public async Task<Int32> GetDint(Int32 param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            var resp = await CallService(53, InstanceId, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<Int64> GetLint(Int32 param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            var resp = await CallService(54, InstanceId, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt64(resp.ResponseData, 0);
        }
        public async Task<float> GetReal(Int32 param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            var resp = await CallService(55, InstanceId, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToSingle(resp.ResponseData, 0);
        }
        public async Task SetSint(Int32 param0, byte param1)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            param.AddRange(BitConverter.GetBytes(param1));
            var resp = await CallService(56, InstanceId, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task SetInt(Int32 param0, Int16 param1)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            param.AddRange(BitConverter.GetBytes(param1));
            var resp = await CallService(57, InstanceId, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task SetDint(Int32 param0, Int32 param1)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            param.AddRange(BitConverter.GetBytes(param1));
            var resp = await CallService(58, InstanceId, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task SetLint(Int32 param0, Int64 param1)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            param.AddRange(BitConverter.GetBytes(param1));
            var resp = await CallService(59, InstanceId, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task SetReal(Int32 param0, float param1)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            param.AddRange(BitConverter.GetBytes(param1));
            var resp = await CallService(60, InstanceId, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task<List<byte>> GetData(Int32 param0, Int32 param1)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            param.AddRange(BitConverter.GetBytes(param1));
            var resp = await CallService(61, InstanceId, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeListByte(resp.ResponseData);
        }
    }
}