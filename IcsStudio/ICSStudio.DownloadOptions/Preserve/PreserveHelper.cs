using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.DownloadOptions.ICSStudio.RestoreTest;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Utilities;
using MessagePack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DataTypeInfo = ICSStudio.DownloadOptions.ICSStudio.RestoreTest.DataTypeInfo;

namespace ICSStudio.DownloadOptions.Preserve
{
    public class PreserveHelper
    {
        public PreserveHelper(ICipMessager messager, DataTypeHelper dataTypeHelper)
        {
            CipMessager = messager;
            DataTypeHelper = dataTypeHelper;
        }

        public ICipMessager CipMessager { get; }
        public DataTypeHelper DataTypeHelper { get; }

        public async Task<int> GetDataTypes(ProjectInfo projectInfo)
        {
            List<DataTypeInfo> dataTypeInfos = new List<DataTypeInfo>();

            await GetUDTDataTypes(dataTypeInfos);

            await GetAoiDataTypes(dataTypeInfos);

            if (dataTypeInfos.Count > 0)
            {
                projectInfo.DataTypes = dataTypeInfos.ToArray();
            }

            return 0;
        }

        private async Task GetAoiDataTypes(List<DataTypeInfo> dataTypeInfos)
        {
            List<JObject> aoiObjects = new List<JObject>();

            CIPController cipController = new CIPController(0, CipMessager);

            int num = await cipController.GetNumAOIDefs();

            await cipController.EnterReadLock();

            int currID = 0;
            for (int i = 0; i < num; ++i)
            {
                var tmp = await cipController.FindNextAOIDefs(currID, 1);
                currID = tmp[0];

                var cipAoiDef = new CIPAOIDefStub(currID, cipController.Messager);

                var cfg = await cipAoiDef.GetConfig();

                JObject aoiObject = FromMsgPack(cfg);
                aoiObjects.Add(aoiObject);
            }

            await cipController.ExitReadLock();

            foreach (var aoiObject in aoiObjects)
            {
                DataTypeInfo dataTypeInfo = ToDataTypeInfo(aoiObject);

                dataTypeInfos.Add(dataTypeInfo);
            }
        }

        private DataTypeInfo ToDataTypeInfo(JObject aoiJObject)
        {
            DataTypeInfo dataTypeInfo = new DataTypeInfo();

            dataTypeInfo.Name = aoiJObject["Name"]?.ToString();
            dataTypeInfo.Description = aoiJObject["Description"]?.ToString();
            dataTypeInfo.Family = FamilyType.NoFamily;

            List<DataTypeMemberInfo> members = new List<DataTypeMemberInfo>();

            if (aoiJObject.ContainsKey("Parameters"))
            {
                JArray parameters = aoiJObject["Parameters"] as JArray;
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        Usage usage = (Usage)(byte)parameter["Usage"];
                        if (usage == Usage.InOut)
                            continue;

                        DataTypeMemberInfo memberInfo = new DataTypeMemberInfo();
                        memberInfo.Name = (string)parameter["Name"];
                        memberInfo.DataType = (string)parameter["DataType"];
                        memberInfo.Description = (string)parameter["Description"];

                        memberInfo.Radix = (DisplayStyle)(byte)parameter["Radix"];
                        memberInfo.ExternalAccess = (ExternalAccess)(byte)parameter["ExternalAccess"];

                        string dimensions = (string)parameter["Dimensions"];
                        if (!string.IsNullOrEmpty(dimensions))
                        {
                            var dimensionList = dimensions.Split(' ');
                            int dimension = 1;
                            foreach (var d in dimensionList)
                            {
                                dimension *= int.Parse(d);
                            }

                            memberInfo.Dimension = dimension;
                        }

                        members.Add(memberInfo);
                    }
                }
            }

            if (aoiJObject.ContainsKey("LocalTags"))
            {
                JArray localTags = aoiJObject["LocalTags"] as JArray;
                if (localTags != null)
                {
                    foreach (var localTag in localTags)
                    {
                        DataTypeMemberInfo memberInfo = new DataTypeMemberInfo();
                        memberInfo.Hidden = true;

                        memberInfo.Name = (string)localTag["Name"];
                        memberInfo.DataType = (string)localTag["DataType"];
                        memberInfo.Description = (string)localTag["Description"];

                        memberInfo.Radix = (DisplayStyle)(byte)localTag["Radix"];
                        memberInfo.ExternalAccess = (ExternalAccess)(byte)localTag["ExternalAccess"];

                        string dimensions = (string)localTag["Dimensions"];
                        if (!string.IsNullOrEmpty(dimensions))
                        {
                            var dimensionList = dimensions.Split(' ');
                            int dimension = 1;
                            foreach (var d in dimensionList)
                            {
                                dimension *= int.Parse(d);
                            }

                            memberInfo.Dimension = dimension;
                        }

                        members.Add(memberInfo);
                    }
                }
            }

            dataTypeInfo.Members = members;

            return dataTypeInfo;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private async Task GetUDTDataTypes(List<DataTypeInfo> dataTypeInfos)
        {
            List<JObject> udtObjects = new List<JObject>();

            CIPController cipController = new CIPController(0, CipMessager);

            int num = await cipController.GetNumUDTypes();

            await cipController.EnterReadLock();

            int currID = 0;
            for (int i = 0; i < num; ++i)
            {
                var tmp = await cipController.FindNextUDTypes(currID, 1);

                currID = tmp[0];

                var cipUDType = new CIPUDType(currID, CipMessager);

                var cfg = await cipUDType.GetConfig();

                JObject udtObject = FromMsgPack(cfg);
                udtObjects.Add(udtObject);
            }

            await cipController.ExitReadLock();

            foreach (var udtObject in udtObjects)
            {
                DataTypeInfo dataTypeInfo = udtObject.ToObject<DataTypeInfo>();

                dataTypeInfos.Add(dataTypeInfo);
            }
        }

        public async Task<int> GetAllTags(ProjectInfo projectInfo)
        {
            List<TagValueInfo> controllerTags = new List<TagValueInfo>();

            int result = await GetControllerTags(controllerTags);
            if (result < 0)
                return result;

            projectInfo.Tags = controllerTags.ToArray();

            List<ProgramInfo> programInfos = new List<ProgramInfo>();
            result = await GetProgramInfos(programInfos);
            if (result < 0)
                return result;

            projectInfo.Programs = programInfos.ToArray();

            return 0;
        }

        private async Task<int> GetControllerTags(List<TagValueInfo> tagList)
        {
            CIPController cipController = new CIPController(0, CipMessager);

            int result = await cipController.EnterReadLock();
            if (result != 0)
            {
                return -Math.Abs(result);
            }

            int numTags = await cipController.GetNumTags();

            if (numTags > 0)
            {

                // 1. read ids
                List<int> ids = new List<int>();
                int leftCount = numTags;
                int currID = 0;
                while (leftCount > 0)
                {
                    int readCount = leftCount < 64 ? leftCount : 64;
                    var tmp = await cipController.FindNextTags(currID, readCount);

                    if (tmp != null && tmp.Count > 0)
                    {
                        ids.AddRange(tmp);

                        currID = tmp.Last();
                        leftCount -= tmp.Count;
                    }
                    else
                    {
                        result = -1;
                        goto End;
                    }

                }

                // 2. get tag value info
                for (int i = 0; i < numTags; i++)
                {
                    var cipTag = new CIPTag(ids[i], cipController.Messager);

                    var info = await GetTagValueInfo(cipTag);
                    if (info != null)
                        tagList.Add(info);
                }

                /*
                leftCount = numTags;
                int index = 0;
                while (leftCount > 0)
                {
                    int taskCount = leftCount < 8 ? leftCount : 8;

                    List<Task> tasks = new List<Task>();

                    for (int i = 0; i < taskCount; i++)
                    {
                        var cipTag = new CIPTag(ids[index + i], cipController.Messager);
                        tasks.Add(GetTagValueInfo(cipTag));
                    }

                    Task.WaitAll(tasks.ToArray());

                    foreach (var task in tasks.OfType<Task<TagValueInfo>>())
                    {
                        if (task.Result != null)
                        {
                            tagList.Add(task.Result);
                        }
                    }

                    leftCount -= taskCount;
                    index += taskCount;
                }
                */
            }

            End:

            await cipController.ExitReadLock();

            return result;
        }

        private async Task<TagValueInfo> GetTagValueInfo(CIPTag cipTag)
        {
            TagValueInfo tagValueInfo = new TagValueInfo();
            tagValueInfo.Name = await cipTag.GetName();
            tagValueInfo.DataType = await cipTag.GetTagType();

            //Debug.WriteLine($"{tagValueInfo.Name}:{tagValueInfo.DataType}");

            string typeName;
            int dim1, dim2, dim3;
            int errorCode;

            tagValueInfo.DataType = NormalizeTagType(tagValueInfo.DataType);

            bool isValid =
                DataTypeHelper.ParseDataType(
                    tagValueInfo.DataType,
                    out typeName, out dim1, out dim2, out dim3,
                    out errorCode);

            if (!isValid)
            {
                Debug.WriteLine($"need check here for {tagValueInfo.DataType}.");
                return null;
            }

            if (!DataTypeHelper.IsValidTypeForPreserve(typeName))
            {
                Debug.WriteLine($"not valid type for {tagValueInfo.DataType}.");
                return null;
            }

            long tagSize = DataTypeHelper.GetTagSize(typeName, dim1, dim2, dim3);
            Contract.Assert(tagSize > 0);

            byte[] buffer = new byte[tagSize];
            await GetTagDataAsync(cipTag, 0, buffer);

            tagValueInfo.Data = buffer;

            return tagValueInfo;
        }

        private async Task<int> GetTagDataAsync(CIPTag cipTag, int offset, byte[] buffer)
        {
            const int kPacketSize = 1400;

            int length = buffer.Length;

            int count = length / kPacketSize;

            int index = 0;

            for (int i = 0; i < count; i++)
            {
                var readData = await cipTag.GetData(offset, kPacketSize);
                Array.Copy(readData.ToArray(), 0,
                    buffer, index,
                    kPacketSize);

                index += kPacketSize;
                offset += kPacketSize;
            }

            int leftSize = length % kPacketSize;
            if (leftSize != 0)
            {
                var readData = await cipTag.GetData(offset, leftSize);
                Array.Copy(readData.ToArray(), 0,
                    buffer, index,
                    leftSize);
            }

            return 0;

        }

        private async Task<int> GetProgramInfos(List<ProgramInfo> programList)
        {
            CIPController cipController = new CIPController(0, CipMessager);

            int result = await cipController.EnterReadLock();
            if (result != 0)
            {
                return -Math.Abs(result);
            }

            int programsNum = await cipController.GetNumPrograms();
            var programsIDs = new int[programsNum];

            int programCurrID = 0;
            for (int i = 0; i < programsNum; ++i)
            {
                var tmp = await cipController.FindNextPrograms(programCurrID, 1);
                programCurrID = tmp[0];
                programsIDs[i] = programCurrID;
            }

            for (int i = 0; i < programsNum; ++i)
            {
                CIPProgram cipProgram = new CIPProgram(programsIDs[i], cipController.Messager);

                ProgramInfo programInfo = new ProgramInfo();
                programInfo.Name = await cipProgram.GetName();

                int programTagsNum = await cipProgram.GetNumTags();
                if (programTagsNum > 0)
                {

                    List<TagValueInfo> programTags = new List<TagValueInfo>();

                    // 1. get tag id
                    List<int> ids = new List<int>();
                    int leftCount = programTagsNum;
                    int currID = 0;
                    while (leftCount > 0)
                    {
                        int readCount = leftCount < 64 ? leftCount : 64;
                        var tmp = await cipProgram.FindNextTags(currID, readCount);

                        if (tmp != null && tmp.Count > 0)
                        {
                            ids.AddRange(tmp);

                            currID = tmp.Last();
                            leftCount -= tmp.Count;
                        }
                        else
                        {
                            result = -1;
                            goto End;
                        }

                    }

                    // 2. get tag info
                    for (int j = 0; j < programTagsNum; j++)
                    {
                        var cipTag = new CIPTag(ids[j], cipController.Messager);
                        var info = await GetTagValueInfo(cipTag);
                        if (info != null)
                            programTags.Add(info);
                    }

                    /*
                    leftCount = programTagsNum;
                    int index = 0;
                    while (leftCount > 0)
                    {
                        int taskCount = leftCount < 8 ? leftCount : 8;

                        List<Task> tasks = new List<Task>();

                        for (int j = 0; j < taskCount; j++)
                        {
                            var cipTag = new CIPTag(ids[index + j], cipController.Messager);
                            tasks.Add(GetTagValueInfo(cipTag));
                        }

                        Task.WaitAll(tasks.ToArray());

                        foreach (var task in tasks.OfType<Task<TagValueInfo>>())
                        {
                            if (task.Result != null)
                            {
                                programTags.Add(task.Result);
                            }
                        }

                        leftCount -= taskCount;
                        index += taskCount;
                    }
                    */

                    //
                    if (programTags.Count > 0)
                        programInfo.Tags = programTags.ToArray();

                }

                programList.Add(programInfo);
            }

            End:

            await cipController.ExitReadLock();

            return result;

        }

        private string NormalizeTagType(string tagType) //DINT[0,0,3]
        {
            string type = tagType.Replace("[0,", "[");
            return type.Replace("[0,", "[");
        }

        private static JObject FromMsgPack(List<byte> data)
        {
            for (int i = 0; i < data.Count; ++i)
            {
                data[i] = (byte)(data[i] ^ (byte)(0x5A));
            }

            var obj = (JObject)JsonConvert.DeserializeObject(MessagePackSerializer.ToJson(data.ToArray()));
            return obj;
        }
    }
}
