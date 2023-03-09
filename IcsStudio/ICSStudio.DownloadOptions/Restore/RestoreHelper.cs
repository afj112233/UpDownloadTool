using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.DownloadOptions.ICSStudio.RestoreTest;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Tags;
using Newtonsoft.Json.Linq;

namespace ICSStudio.DownloadOptions.Restore
{
    public class RestoreHelper
    {
        private readonly Dictionary<string, bool> _udtCheckedCache;
        private readonly Dictionary<string, bool> _aoiCheckedCache;

        private readonly List<string> _supportedTypes = new List<string>()
        {
            "BOOL", "COUNTER", "CONTROL", "DINT", "UDINT",
            "INT", "UINT", "LINT", "ULINT", "PID", "PIDEAUTOTUNE",
            "SINT", "USINT", "REAL", "LREAL", "STRING", "TIMER"
        };

        public RestoreHelper(Controller controller, ProjectInfo projectInfo)
        {
            Controller = controller;
            CipMessager = controller?.CipMessager;
            ProjectInfo = projectInfo;

            _udtCheckedCache = new Dictionary<string, bool>();
            _aoiCheckedCache = new Dictionary<string, bool>();
        }

        public Controller Controller { get; }
        public ICipMessager CipMessager { get; }
        public ProjectInfo ProjectInfo { get; }

        public int Restore()
        {
            if (Controller == null)
                return -1;

            if (!Controller.IsOnline)
                return -1;

            if (CipMessager == null)
                return -1;

            if (ProjectInfo == null)
                return -1;

            // restore tag in controller
            int result = RestoreTagsInController();
            if (result < 0)
                return result;

            // restore programs
            result = RestorePrograms();

            return result < 0 ? result : 0;
        }

        private int RestoreTagsInController()
        {
            if (ProjectInfo?.Tags != null)
            {
                foreach (var controllerTagInfo in ProjectInfo.Tags)
                {
                    //1、判断名字是否相同
                    if (Controller.Tags != null)
                    {
                        Tag tag = Controller.Tags[controllerTagInfo.Name] as Tag;
                        if (tag != null)
                        {
                            SetCIPTagData(controllerTagInfo, tag);
                        }
                    }
                }
            }
            
            return 0;
        }

        private int RestorePrograms()
        {
            if (ProjectInfo.Programs != null)
            {
                foreach (var programInfo in ProjectInfo.Programs)
                {
                    //1、判断 program 名字是否相同
                    if (Controller.Programs[programInfo.Name] != null)
                    {
                        Program program = Controller.Programs[programInfo.Name] as Program;

                        if (programInfo.Tags != null && program != null)
                        {
                            foreach (var programInfoTag in programInfo.Tags)
                            {
                                //2、判断名字是否相同
                                Tag tag = program.Tags[programInfoTag.Name] as Tag;
                                if (tag != null)
                                {
                                    SetCIPTagData(programInfoTag, tag);
                                }
                            }
                        }
                    }
                }
            }

            

            return 0;
        }

        private void SetCIPTagData(TagValueInfo tagInfo, Tag tag)
        {
            //1、判断数据类型是否相同
            if (IsDataTypeEqual(tagInfo, tag))
            {
                //2、都一致 把数据写入 PLC
                CIPTag cipTag = new CIPTag(Controller.GetTagId(tag), CipMessager);

                int length = tagInfo.Data.Length;
                List<Task> tasks = new List<Task>();

                for (int i = 0; i < length;)
                {
                    if (i + 8 <= length)
                    {
                        tasks.Add(cipTag.SetLint(i, BitConverter.ToInt64(tagInfo.Data, i)));
                        i += 8;
                    }
                    else if (i + 4 <= length)
                    {
                        tasks.Add(cipTag.SetDint(i, BitConverter.ToInt32(tagInfo.Data, i)));
                        i += 4;
                    }
                    else
                    {
                        tasks.Add(cipTag.SetSint(i, tagInfo.Data[i]));
                        i += 1;
                    }

                    if (tasks.Count >= 16)
                    {
                        Task.WaitAll(tasks.ToArray());

                        tasks.Clear();
                    }
                }

                if (tasks.Count > 0)
                {
                    Task.WaitAll(tasks.ToArray());

                    tasks.Clear();
                }
                
            }
        }

        private bool IsDataTypeEqual(TagValueInfo tagInfo, Tag tag)
        {
            if (!string.Equals(tagInfo.DataType, tag.DataTypeInfo.ToString(), StringComparison.OrdinalIgnoreCase))
                return false;

            // check datatype size
            if (_supportedTypes.Contains(tag.DataTypeInfo.DataType.Name.ToUpper()))
            {
                return true;
            }

            // UDT
            UserDefinedDataType userDefinedDataType = tag.DataTypeInfo.DataType as UserDefinedDataType;
            if (userDefinedDataType != null)
            {
                return CheckUserDefinedDataType(userDefinedDataType);
            }

            //AOI
            AOIDataType aoiDataType = tag.DataTypeInfo.DataType as AOIDataType;
            if (aoiDataType != null)
            {
                return CheckAOIDataType(aoiDataType);
            }

            return false;
        }

        private bool CheckAOIDataType(AOIDataType aoiDataType)
        {
            string dataTypeName = aoiDataType.Name.ToUpper();
            if (_aoiCheckedCache.ContainsKey(dataTypeName))
                return _aoiCheckedCache[dataTypeName];

            DataTypeInfo dataTypeInfoInPLC = AOIDataTypeToDataTypeInfo(aoiDataType);

            DataTypeInfo dataTypeInfoInProject = ProjectInfo.GetDataTypeInfo(dataTypeName);

            bool isEqual = DataTypeInfoEquals(dataTypeInfoInPLC, dataTypeInfoInProject);

            _aoiCheckedCache.Add(dataTypeName, isEqual);

            return isEqual;
        }

        private bool CheckUserDefinedDataType(UserDefinedDataType userDefinedDataType)
        {
            string dataTypeName = userDefinedDataType.Name.ToUpper();
            if (_udtCheckedCache.ContainsKey(dataTypeName))
                return _udtCheckedCache[dataTypeName];

            JObject udtObject = userDefinedDataType.ConvertToJObject();

            DataTypeInfo dataTypeInfoInPLC = udtObject.ToObject<DataTypeInfo>();

            DataTypeInfo dataTypeInfoInProject = ProjectInfo.GetDataTypeInfo(dataTypeName);

            bool isEqual = DataTypeInfoEquals(dataTypeInfoInPLC, dataTypeInfoInProject);

            _udtCheckedCache.Add(dataTypeName, isEqual);

            return isEqual;
        }

        private DataTypeInfo AOIDataTypeToDataTypeInfo(AOIDataType aoiDataType)
        {
            DataTypeInfo dataTypeInfo = new DataTypeInfo();

            dataTypeInfo.Name = aoiDataType.Name;
            dataTypeInfo.Description = aoiDataType.Description;
            dataTypeInfo.Family = FamilyType.NoFamily;

            List<DataTypeMemberInfo> members = new List<DataTypeMemberInfo>();

            foreach (var member in aoiDataType.TypeMembers)
            {
                DataTypeMemberInfo memberInfo = new DataTypeMemberInfo();

                memberInfo.Name = member.Name;
                memberInfo.Hidden = member.IsHidden;
                memberInfo.DataType = member.DataTypeInfo.DataType.Name;
                memberInfo.Description = member.Description;

                memberInfo.Radix = member.DisplayStyle;
                memberInfo.ExternalAccess = member.ExternalAccess;

                int dim1 = member.DataTypeInfo.Dim1;
                int dim2 = member.DataTypeInfo.Dim2;
                int dim3 = member.DataTypeInfo.Dim3;

                if (dim1 > 0)
                {
                    int totalDimension = Math.Max(dim1, 1) * Math.Max(dim2, 1) * Math.Max(dim3, 1);
                    memberInfo.Dimension = totalDimension;
                }

                members.Add(memberInfo);
            }

            dataTypeInfo.Members = members;
            return dataTypeInfo;
        }

        private bool DataTypeInfoEquals(DataTypeInfo dataTypeInfo0, DataTypeInfo dataTypeInfo1)
        {
            if (dataTypeInfo0 == null || dataTypeInfo1 == null)
            {
                return false;
            }

            if (dataTypeInfo0.Members.Count != dataTypeInfo1.Members.Count)
            {
                return false;
            }

            int count = dataTypeInfo0.Members.Count;

            for (int index = 0; index < count; index++)
            {
                var memberInPLC = dataTypeInfo0.Members[index];
                var memberInProject = dataTypeInfo1.Members[index];

                if (!string.Equals(
                        memberInPLC.DataType, memberInProject.DataType, StringComparison.OrdinalIgnoreCase)
                    || memberInPLC.Dimension != memberInProject.Dimension)
                {
                    return false;
                }

                if (_supportedTypes.Contains(memberInPLC.DataType.ToUpper()))
                    continue;

                if (string.Equals(memberInPLC.DataType, "BIT", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (string.Equals(memberInPLC.DataType, "BITFIELD", StringComparison.OrdinalIgnoreCase))
                    continue;

                // UDT
                UserDefinedDataType memberUdt = Controller.DataTypes[memberInPLC.DataType] as UserDefinedDataType;
                if (memberUdt != null)
                {
                    if (CheckUserDefinedDataType(memberUdt))
                        continue;
                }

                // AOI
                AOIDataType memberAoi = Controller.DataTypes[memberInPLC.DataType] as AOIDataType;
                if (memberAoi != null)
                {
                    if (CheckAOIDataType(memberAoi))
                        continue;
                }

                // not run here
                return false;
            }

            return true;
        }
    }
}
