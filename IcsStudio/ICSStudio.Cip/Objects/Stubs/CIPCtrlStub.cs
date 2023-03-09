//AUTO GENERATED, DONT EDIT
using ICSStudio.Cip.DataTypes;
using ICSStudio.Cip.EtherNetIP;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ICSStudio.Cip.Objects
{
    public class CIPCtrlStub : CipBaseObject 
    {
        public CIPCtrlStub(int instanceId, ICipMessager messager) :
            base(179, instanceId, messager)
        {
        }

        public async Task<List<byte>> GetTag(string param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(Utils.SerializeString(param0));
            var resp = await CallService(50, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeListByte(resp.ResponseData);
        }
        public async Task SetTag(string param0, List<byte> param1)
        {
            List<byte> param = new List<byte>();
            param.AddRange(Utils.SerializeString(param0));
            param.AddRange(Utils.SerializeListByte(param1));
            var resp = await CallService(51, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task<Int32> ReaderLock()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(52, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<Int32> ReaderUnLock()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(53, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<Int32> WriterLock()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(54, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<Int32> WriterUnLock()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(55, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<List<Int32>> FindNextUDTypes(Int32 param0, Int32 param1)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            param.AddRange(BitConverter.GetBytes(param1));
            var resp = await CallService(56, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeListInt32(resp.ResponseData);
        }
        public async Task<List<Int32>> FindNextTags(Int32 param0, Int32 param1)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            param.AddRange(BitConverter.GetBytes(param1));
            var resp = await CallService(57, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeListInt32(resp.ResponseData);
        }
        public async Task<List<Int32>> FindNextPrograms(Int32 param0, Int32 param1)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            param.AddRange(BitConverter.GetBytes(param1));
            var resp = await CallService(58, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeListInt32(resp.ResponseData);
        }
        public async Task<List<Int32>> FindNextTasks(Int32 param0, Int32 param1)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            param.AddRange(BitConverter.GetBytes(param1));
            var resp = await CallService(59, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeListInt32(resp.ResponseData);
        }
        public async Task<List<Int32>> FindNextModules(Int32 param0, Int32 param1)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            param.AddRange(BitConverter.GetBytes(param1));
            var resp = await CallService(60, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeListInt32(resp.ResponseData);
        }
        public async Task<Int32> FindTaskId(string param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(Utils.SerializeString(param0));
            var resp = await CallService(61, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<Int32> FindProgramId(string param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(Utils.SerializeString(param0));
            var resp = await CallService(62, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task Clear()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(63, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task Store()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(64, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task ResetTranSientData()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(65, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task TransmitData(List<byte> param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(Utils.SerializeListByte(param0));
            var resp = await CallService(66, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task<Int32> CreateTag()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(67, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<Int32> CreateProgram()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(68, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<Int32> CreateModule()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(69, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<Int32> CreateTask()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(70, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<Int32> CreateType()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(71, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task FinalizeTypeCreation()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(72, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task Setup()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(73, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task<Int32> FindTagId(string param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(Utils.SerializeString(param0));
            var resp = await CallService(74, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<List<Int32>> FindNextAOIDefs(Int32 param0, Int32 param1)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            param.AddRange(BitConverter.GetBytes(param1));
            var resp = await CallService(75, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeListInt32(resp.ResponseData);
        }
        public async Task<Int32> CreateAOI()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(76, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<Int32> SetTimeSynchronize()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(77, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<Int32> CreateMDType()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(78, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<List<Int32>> FindNextTrends(Int32 param0, Int32 param1)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            param.AddRange(BitConverter.GetBytes(param1));
            var resp = await CallService(79, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeListInt32(resp.ResponseData);
        }
        public async Task<Int32> CreateTrend()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(80, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task SetNativeCode()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(81, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task Test()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(82, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task SetLogId(Int64 param0, Int64 param1)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            param.AddRange(BitConverter.GetBytes(param1));
            var resp = await CallService(83, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task<List<byte>> GetClientData(Int32 param0, Int32 param1)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            param.AddRange(BitConverter.GetBytes(param1));
            var resp = await CallService(84, 0, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeListByte(resp.ResponseData);
        }
        public async Task SetName(string param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(Utils.SerializeString(param0));
            var resp = await CallService(16, 0, 8, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task SetMajorFaultProgram(string param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(Utils.SerializeString(param0));
            var resp = await CallService(16, 0, 9, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task SetPowerLossProgram(string param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(Utils.SerializeString(param0));
            var resp = await CallService(16, 0, 10, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task SetState(Int32 param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            var resp = await CallService(16, 0, 11, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task ClearFaults()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(16, 0, 12, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task SetTimestamp(Int64 param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            var resp = await CallService(16, 0, 13, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task SaveConfig()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(16, 0, 14, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
        }
        public async Task<Int32> GetNumUDTypes()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, 0, 8, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<Int32> GetNumTags()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, 0, 9, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<Int32> GetNumPrograms()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, 0, 10, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<Int32> GetNumAOIDefs()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, 0, 11, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<Int32> GetHasProject()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, 0, 12, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<string> GetName()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, 0, 13, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeString(resp.ResponseData);
        }
        public async Task<string> GetMajorFaultProgram()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, 0, 14, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeString(resp.ResponseData);
        }
        public async Task<string> GetPowerLossProgram()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, 0, 15, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeString(resp.ResponseData);
        }
        public async Task<List<byte>> GetTimeSynchronize()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, 0, 16, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeListByte(resp.ResponseData);
        }
        public async Task<Int32> GetNumModules()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, 0, 17, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<Int32> GetNumTasks()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, 0, 18, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<Int64> GetTimestamp()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, 0, 19, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt64(resp.ResponseData, 0);
        }
        public async Task<CtrlStatus> GetStatus()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, 0, 20, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeCtrlStatus(resp.ResponseData);
        }
        public async Task<Int32> GetNumTrends()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, 0, 21, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<Int32> GetMinMajorFaultId()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, 0, 22, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<List<byte>> GetMajorFaultInfo(Int32 param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            var resp = await CallService(14, 0, 23, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeListByte(resp.ResponseData);
        }
        public async Task<Int64> GetCurrentLogId()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, 0, 24, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt64(resp.ResponseData, 0);
        }
        public async Task<Int32> GetLogSizeAndPrepare(Int64 param0)
        {
            List<byte> param = new List<byte>();
            param.AddRange(BitConverter.GetBytes(param0));
            var resp = await CallService(14, 0, 25, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return BitConverter.ToInt32(resp.ResponseData, 0);
        }
        public async Task<LogIdPair> GetCurrentLogIdPair()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, 0, 26, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeLogIdPair(resp.ResponseData);
        }
        public async Task<List<byte>> GetInfo()
        {
            List<byte> param = new List<byte>();
            var resp = await CallService(14, 0, 27, param.ToArray());
            if (resp.GeneralStatus != (byte)CipGeneralStatusCode.Success)
            {
                throw new CIPGeneralStatusCodeException((CipGeneralStatusCode)resp.GeneralStatus, resp.ToString());
            }
            return Utils.DeSerializeListByte(resp.ResponseData);
        }
    }
}