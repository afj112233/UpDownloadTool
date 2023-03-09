using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Utilities;
using MessagePack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace ICSStudio.SimpleServices.Online
{
    public class OnlineEditHelper
    {
        internal static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public OnlineEditHelper(ICipMessager messager)
        {
            Messager = messager;
        }
        
        public static ConcurrentBag<IProgram> CompilingPrograms { set; get; }=new ConcurrentBag<IProgram>();

        public ICipMessager Messager { get; }

        public async Task<int> ReplaceRoutine(IRoutine routine,bool isNeedGenCode=true)
        {
            if (!routine.ParentController.IsOnline) return -1;

            if (isNeedGenCode)
                routine.GenCode(routine.ParentCollection.ParentProgram);

            CIPRoutine cipRoutine = await GetCIPRoutine(routine);
            if (cipRoutine == null)
                return -1;

            CIPController cipController = new CIPController(0, Messager);

            int result = await cipController.WriterLockRetry();
            if (result < 0)
                return result;
            
            await cipController.TransmitCfg(ToMsgPack(routine.ConvertToJObject(true)));
            await cipRoutine.Replace();

            await cipController.WriterUnLock();
            
            //TODO(gjc): need edit later
            var st = routine as STRoutine;
            if (st != null)
                st.IsModified = false;
            //

            return 0;
        }

        public async Task<int> UpdateProgram(Program program)
        {
            if (!program.ParentController.IsOnline) return -1;
            program.GenSepNativeCode((Controller)program.ParentController);

            CIPController cipController = new CIPController(0, Messager);

            int programId = await cipController.FindProgramId(program.Name);

            CIPProgram cipProgram = new CIPProgram(programId, Messager);

            JObject programUpdateInfo = program.CreateProgramUpdateInfo();

            int taskId = await cipController.FindTaskId(program.ParentTask?.Name);

            int result = await cipController.WriterLockRetry();
            if (result < 0)
                return result;

            await cipController.TransmitCfg(ToMsgPack(programUpdateInfo));
            await cipProgram.Update(taskId);

            await cipController.WriterUnLock();

            return 0;
        }

        public async Task<int> CreateTagInController(JObject tagObject)
        {
            CIPController cipController = new CIPController(0, Messager);

            int result = await cipController.WriterLockRetry();
            if (result < 0)
                return result;

            await cipController.TransmitCfg(ToMsgPack(tagObject));
            await cipController.CreateTag();

            await cipController.WriterUnLock();

            return 0;
        }

        public async Task<int> CreateTagInProgram(JObject tagObject, IProgram program)
        {
            CIPController cipController = new CIPController(0, Messager);
            CIPProgram cipProgram = await GetCIPProgram(program);

            int result = await cipController.WriterLockRetry();
            if (result < 0)
                return result;

            await cipController.TransmitCfg(ToMsgPack(tagObject));
            await cipProgram.CreateTag();

            await cipController.WriterUnLock();

            return 0;
        }

        public async Task<int> ChangeTagName(int tagId, string tagName)
        {
            //TODO(gjc): need edit later
            CIPController cipController = new CIPController(0, Messager);
            CIPTag cipTag = new CIPTag(tagId, Messager);

            int result = await cipController.WriterLockRetry();
            if (result < 0)
                return result;

            await cipTag.SetName(tagName);

            await cipController.WriterUnLock();

            return 0;
        }

        public async Task<long> GetTimestamp()
        {
            CIPController cipController = new CIPController(0, Messager);

            return await cipController.GetTimestamp();
        }

        public async Task SetTimestamp(long timestamp)
        {
            CIPController cipController = new CIPController(0, Messager);

            await cipController.SetTimestamp(timestamp);
        }

        public async Task<int> SaveToController()
        {
            try
            {
                CIPController cipController = new CIPController(0, Messager);

                int result = await cipController.WriterLockRetry();
                if (result < 0)
                    return result;

                await cipController.SaveConfig();

                await cipController.WriterUnLock();

                return 0;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return -1;
            }
        }

        private async Task<CIPRoutine> GetCIPRoutine(IRoutine routine)
        {
            try
            {
                CIPController cipController = new CIPController(0, Messager);

                int programId = await cipController.FindProgramId(routine.ParentCollection.ParentProgram.Name);
                CIPProgram cipProgram = new CIPProgram(programId, Messager);

                await cipController.EnterReadLock();

                var routineId = await cipProgram.FindRoutineId(routine.Name);

                await cipController.ExitReadLock();

                return new CIPRoutine((ushort)routineId, Messager);
            }
            catch (Exception e)
            {
                Logger.Error(e, $"GetCIPRoutine:{routine.Name} failed!");

                return null;
            }

        }

        private async Task<CIPProgram> GetCIPProgram(IProgram program)
        {
            CIPController cipController = new CIPController(0, Messager);

            int programId = await cipController.FindProgramId(program.Name);
            CIPProgram cipProgram = new CIPProgram(programId, Messager);

            return cipProgram;
        }

        internal static List<byte> ToMsgPack(JObject obj)
        {
            var res = MessagePackSerializer.FromJson(JsonConvert.SerializeObject(obj)).ToList();
            for (int i = 0; i < res.Count; ++i)
            {
                res[i] = (byte)(res[i] ^ 0x5A);
            }

            return res;
        }
    }
}
