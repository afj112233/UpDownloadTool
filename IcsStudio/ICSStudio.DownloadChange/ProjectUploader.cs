using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using MessagePack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICSStudio.DownloadChange
{
    public class ProjectUploader
    {
        private readonly CIPController _controller;

        public ProjectUploader(ICipMessager messager)
        {
            Debug.Assert(messager != null);
            Messager = messager;

            _controller = new CIPController(0, messager);

            ProjectInfo = new JObject();
        }

        public ICipMessager Messager { get; }

        public JObject ProjectInfo { get; }

        public async Task UploadLock()
        {
            await _controller.WriterLockRetry();
        }

        public async Task UploadUnlock()
        {
            await _controller.WriterUnLock();
        }

        public async Task UploadControllerProperties()
        {
            string name = await _controller.GetName();
            string majorFaultProgram = await _controller.GetMajorFaultProgram();
            string powerLossProgram = await _controller.GetPowerLossProgram();

            JObject timeSetting = FromMsgPack(await _controller.GetTimeSynchronize());

            //"Name", "MajorFaultProgram", "PowerLossProgram", "TimeSynchronize"
            ProjectInfo.Add("Name", name);
            ProjectInfo.Add("MajorFaultProgram", majorFaultProgram);
            ProjectInfo.Add("PowerLossProgram", powerLossProgram);
            ProjectInfo.Add("TimeSynchronize", timeSetting);
        }

        public async Task UploadControllerTags()
        {
            JArray tags = new JArray();

            int num = await _controller.GetNumTags();

            if (num > 0)
            {
                List<int> ids = new List<int>();
                int leftCount = num;
                int currID = 0;
                while (leftCount > 0)
                {
                    int readCount = leftCount < 64 ? leftCount : 64;
                    var tmp = await _controller.FindNextTags(currID, readCount);

                    if (tmp != null && tmp.Count > 0)
                    {
                        ids.AddRange(tmp);

                        currID = tmp.Last();
                        leftCount -= tmp.Count;
                    }
                    else
                    {
                        throw new NotImplementedException("Not run here!");
                    }

                }


                for (int i = 0; i < num; ++i)
                {
                    var t = new CIPTag(ids[i], Messager);
                    var cfg = await t.GetConfig();

                    tags.Add(FromMsgPack(cfg));
                }
            }

            ProjectInfo.Add("Tags", tags);
        }

        public async Task UploadDataTypes()
        {
            JArray dataTypes = new JArray();

            int num = await _controller.GetNumUDTypes();

            var ids = new int[num];

            int id = 0;
            for (int i = 0; i < num; ++i)
            {
                var tmp = await _controller.FindNextUDTypes(id, 1);
                id = tmp[0];
                ids[i] = id;
            }

            for (int i = 0; i < num; ++i)
            {
                var t = new CIPUDType(ids[i], Messager);
                var cfg = await t.GetConfig();
                dataTypes.Add(FromMsgPack(cfg));
            }

            ProjectInfo.Add("DataTypes", dataTypes);
        }

        public async Task UploadAOIDefinitions()
        {
            JArray aoiDes = new JArray();

            int num = await _controller.GetNumAOIDefs();

            var ids = new int[num];

            int id = 0;
            for (int i = 0; i < num; ++i)
            {
                var tmp = await _controller.FindNextAOIDefs(id, 1);
                id = tmp[0];
                ids[i] = id;
            }

            for (int i = 0; i < num; ++i)
            {
                var t = new CIPAOIDefStub(ids[i], Messager);
                var cfg = await t.GetConfig();
                aoiDes.Add(FromMsgPack(cfg));
            }

            ProjectInfo.Add("AddOnInstructionDefinitions", aoiDes);
        }

        public async Task UploadTasks()
        {
            JArray tasks = new JArray();

            int num = await _controller.GetNumTasks();

            var ids = new int[num];

            int id = 0;
            for (int i = 0; i < num; ++i)
            {
                var tmp = await _controller.FindNextTasks(id, 1);
                id = tmp[0];
                ids[i] = id;
            }

            for (int i = 0; i < num; ++i)
            {
                var t = new CIPTask(ids[i], Messager);
                var cfg = await t.GetConfig();
                tasks.Add(FromMsgPack(cfg));
            }

            ProjectInfo.Add("Tasks", tasks);
        }

        public async Task UploadPrograms()
        {
            JArray programs = new JArray();

            int num = await _controller.GetNumPrograms();

            var ids = new int[num];

            int id = 0;
            for (int i = 0; i < num; ++i)
            {
                var tmp = await _controller.FindNextPrograms(id, 1);
                id = tmp[0];
                ids[i] = id;
            }

            for (int i = 0; i < num; ++i)
            {
                var t = new CIPProgram(ids[i], Messager);
                var cfg = await t.GetConfig();
                programs.Add(FromMsgPack(cfg));
            }

            ProjectInfo.Add("Programs", programs);
        }

        public async Task UploadModules()
        {
            JArray modules = new JArray();

            int num = await _controller.GetNumModules();

            var ids = new int[num];

            int id = 0;
            for (int i = 0; i < num; ++i)
            {
                var tmp = await _controller.FindNextModules(id, 1);
                id = tmp[0];
                ids[i] = id;
            }

            for (int i = 0; i < num; ++i)
            {
                var t = new CIPModule(ids[i], Messager);
                var cfg = await t.GetConfig();
                modules.Add(FromMsgPack(cfg));

            }

            ProjectInfo.Add("Modules", modules);
        }

        private static JObject FromMsgPack(List<byte> data)
        {
            for (int i = 0; i < data.Count; ++i)
            {
                data[i] = (byte)(data[i] ^ 0x5A);
            }

            var obj = (JObject)JsonConvert.DeserializeObject(MessagePackSerializer.ToJson(data.ToArray()));
            return obj;
        }
    }
}
