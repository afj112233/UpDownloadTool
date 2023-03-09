//AUTO GENERATED, DONT EDIT
using ICSStudio.Cip.DataTypes;
using ICSStudio.Cip.EtherNetIP;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ICSStudio.Cip.Objects
{
    public class CIPModuleStub : CipBaseObject 
    {
        public CIPModuleStub(int instanceId, ICipMessager messager) :
            base(183, instanceId, messager)
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
        public async Task<Int32> FindModuleId(string param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(Utils.SerializeString(param0));
            var resp = await CallService(50, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<Int32> GetConfigSize()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, InstanceId, 8, param.ToArray());
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
        public async Task<List<byte>> GetCustomAttribute(Int32 param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            var resp = await CallService(51, InstanceId, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeListByte(resp.ResponseData);
        }
        public async Task SetCustomAttribute(Int32 param0, List<byte> param1)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            param.AddRange(Utils.SerializeListByte(param1));
            var resp = await CallService(52, InstanceId, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task<Int16> GetEntryStatus()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, InstanceId, 9, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt16(resp.ResponseData, 0);
        }
        public async Task<Int16> GetFaultCode()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, InstanceId, 10, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt16(resp.ResponseData, 0);
        }
        public async Task<Int32> GetFaultInfo()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, InstanceId, 11, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<Int16> GetFirmwareSupervisorStatus()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, InstanceId, 12, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt16(resp.ResponseData, 0);
        }
        public async Task<Int16> GetForceStatus()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, InstanceId, 13, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt16(resp.ResponseData, 0);
        }
        public async Task<Int32> GetInstance()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, InstanceId, 14, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<Int16> GetLEDStatus()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, InstanceId, 15, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt16(resp.ResponseData, 0);
        }
        public async Task<Int16> GetMode()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, InstanceId, 16, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt16(resp.ResponseData, 0);
        }
        public async Task<string> GetPath()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, InstanceId, 17, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeString(resp.ResponseData);
        }
    }
}