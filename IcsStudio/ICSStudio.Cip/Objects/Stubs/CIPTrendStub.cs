//AUTO GENERATED, DONT EDIT
using ICSStudio.Cip.DataTypes;
using ICSStudio.Cip.EtherNetIP;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ICSStudio.Cip.Objects
{
    public class CIPTrendStub : CipBaseObject 
    {
        public CIPTrendStub(int instanceId, ICipMessager messager) :
            base(187, instanceId, messager)
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
        public async Task<Int32> FindPenId(string param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(Utils.SerializeString(param0));
            var resp = await CallService(51, InstanceId, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<List<byte>> GetPenData(Int32 param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            var resp = await CallService(52, InstanceId, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeListByte(resp.ResponseData);
        }
    }
}