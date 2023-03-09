using System.Collections.Generic;
using System.Threading.Tasks;
using ICSStudio.Cip.EtherNetIP;

namespace ICSStudio.Cip.Objects
{

    public class CIPController : CIPCtrlStub
    {
        private enum ServiceCode : byte
        {
            TagGet = 0x32,
            TagSet = 0x33,
            LockAccess = 0x34,
            UnlockAccess = 0x35,
            FindNextTagIdList = 0x36,
            GetTagInfo = 0x37,
            FindTaskId = 0x42,
            FindProgramId = 0x43,
        }

        public CIPController(int instanceId, ICipMessager messager) : base(instanceId, messager)
        {
        }

        public async Task TransmitCfg(List<byte> config)
        {
            const int CHUNK_SIZE = 512;

            int offset = 0;
            int size = config.Count;
            for (int i = 0; i < size / CHUNK_SIZE; ++i)
            {
                await TransmitData(config.GetRange(offset, CHUNK_SIZE));
                offset += CHUNK_SIZE;
            }

            int left = size % CHUNK_SIZE;
            if (left != 0)
            {
                await TransmitData(config.GetRange(offset, left));
            }
        }

        public async Task<List<byte>> ReadChangeLog(long index)
        {
            const int chunkSize = 1024;

            int logSize = await GetLogSizeAndPrepare(index);

            List<byte> log = new List<byte>();

            int leftSize = logSize;
            int offset = 0;
            while (leftSize > 0)
            {
                var count = leftSize > chunkSize ? chunkSize : leftSize;

                var bytes = await GetClientData(offset, count);
                log.AddRange(bytes);

                offset += count;
                leftSize -= count;
            }

            return log;

        }

        public async Task<int> WriterLockRetry()
        {
            int retryCount = 0;
            while (true)
            {
                int result = await WriterLock();
                if (result == 0)
                    return 0;

                await Task.Delay(50);

                retryCount++;
                if (retryCount > 20)
                    return -999;
            }
        }

        //void Controller

    }
}
