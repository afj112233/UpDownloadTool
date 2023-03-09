//AUTO GENERATED, DONT EDIT
using ICSStudio.Cip.DataTypes;
using ICSStudio.Cip.EtherNetIP;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ICSStudio.Cip.Objects
{
    public class CIPProgramStub : CipBaseObject 
    {
        public CIPProgramStub(int instanceId, ICipMessager messager) :
            base(181, instanceId, messager)
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
        public async Task SetDesc(string param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(Utils.SerializeString(param0));
            var resp = await CallService(16, InstanceId, 9, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task<string> GetDesc()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, InstanceId, 9, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeString(resp.ResponseData);
        }
        public async Task SetMainRoutineId(Int32 param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            var resp = await CallService(16, InstanceId, 10, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task<Int32> GetMainRoutineId()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, InstanceId, 10, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task SetFaultRoutineId(Int32 param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            var resp = await CallService(16, InstanceId, 11, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task<Int32> GetFaultRoutineId()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, InstanceId, 11, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task SetInhibit(bool param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            var resp = await CallService(16, InstanceId, 12, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task<bool> GetInhibit()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, InstanceId, 12, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToBoolean(resp.ResponseData, 0);
        }
        public async Task<Int32> GetNumTags()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, InstanceId, 13, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<float> GetMaxScanTime()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, InstanceId, 16, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToSingle(resp.ResponseData, 0);
        }
        public async Task<float> GetLastScanTime()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, InstanceId, 17, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToSingle(resp.ResponseData, 0);
        }
        public async Task<Int32> GetConfigSize()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, InstanceId, 18, param.ToArray());
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
        public async Task<List<Int32>> FindNextTags(Int32 param0, Int32 param1)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            param.AddRange(BitConverter.GetBytes(param1));
            var resp = await CallService(51, InstanceId, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeListInt32(resp.ResponseData);
        }
        public async Task<List<Int32>> FindNextRoutines(Int32 param0, Int32 param1)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            param.AddRange(BitConverter.GetBytes(param1));
            var resp = await CallService(52, InstanceId, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeListInt32(resp.ResponseData);
        }
        public async Task ResetMaxScanTime()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(53, InstanceId, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task<Int32> FindTagId(string param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(Utils.SerializeString(param0));
            var resp = await CallService(54, InstanceId, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task Update(Int32 param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            var resp = await CallService(55, InstanceId, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task<Int32> CreateTag()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(56, InstanceId, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<Int32> FindRoutineId(string param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(Utils.SerializeString(param0));
            var resp = await CallService(57, InstanceId, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
    }
}