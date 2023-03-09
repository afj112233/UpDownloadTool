using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.SimpleServices.Utilities;
using MessagePack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.Online
{
    public class MajorFaultHelper
    {
        private int _nextId;

        private readonly SemaphoreSlim _asyncLock;

        public MajorFaultHelper(ICipMessager messager)
        {
            Messager = messager;

            _asyncLock = new SemaphoreSlim(1);
        }

        public ICipMessager Messager { get; }

        public async Task<int> GetAllMajorFaultInfos(List<MajorFaultInfo> infos)
        {
            return await GetMajorFaultInfos(infos, 0);
        }

        public async Task<int> GetNextMajorFaultInfos(List<MajorFaultInfo> infos)
        {
            return await GetMajorFaultInfos(infos, _nextId);
        }

        private async Task<int> GetMajorFaultInfos(List<MajorFaultInfo> infos, int startId)
        {
            if (infos == null)
                return -1;

            await _asyncLock.WaitAsync();

            try
            {
                CIPController cipController = new CIPController(0, Messager);

                await cipController.EnterReadLock();

                int minMajorFaultId = await cipController.GetMinMajorFaultId();

                int nextId = minMajorFaultId;

                if (startId > minMajorFaultId)
                    nextId = startId;

                while (true)
                {
                    List<byte> buffer = await cipController.GetMajorFaultInfo(nextId);

                    JArray jArray = FromMsgPackToArray(buffer);
                    if (jArray.Count == 0)
                        break;

                    nextId += jArray.Count;

                    var list = jArray.ToObject<List<MajorFaultInfo>>();
                    if (list != null && list.Count > 0)
                        infos.AddRange(list);
                }

                await cipController.ExitReadLock();

                _nextId = nextId;

                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
            finally
            {
                _asyncLock.Release();
            }
        }

        public async Task Reset()
        {
            await _asyncLock.WaitAsync();

            _nextId = 0;

            _asyncLock.Release();
        }

        private static JArray FromMsgPackToArray(List<byte> data)
        {
            for (int i = 0; i < data.Count; ++i)
            {
                data[i] = (byte)(data[i] ^ 0x5A);
            }

            var obj = JsonConvert.DeserializeObject(MessagePackSerializer.ToJson(data.ToArray())) as JArray;

            return obj;
        }
    }
}
