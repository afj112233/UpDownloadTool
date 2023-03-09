using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.Interfaces.Notification;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Utilities;
using ICSStudio.Utils;
using ICSStudio.Utils.TagExpression;
using Newtonsoft.Json.Linq;
using NLog;
using Timer = System.Timers.Timer;

namespace ICSStudio.SimpleServices.Tags
{
    internal enum TagSyncResults
    {
        ControllerOffline = -1,
        ProgramCountMismatch = -2,
        ProgramNameMismatch = -3,
        AddProgramIDFailed = -4,
        TagCountMismatch = -5,
        TagNameMismatch = -6,
        AddTagIDFailed = -7,
        GetTagIDFailed = -8,
        SetTagValueFailed = -9,
        TaskCountMismatch = -10,
        TaskNameMismatch = -11,
        AddTaskIDFailed = -12,
        ModuleCountMismatch = -13,
        AddModuleIDFailed = -14,

        Failure = -127,
        Success = 0
    }

    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Local")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
    public class TagSyncController
    {
        internal static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<IDeviceModule, int> _deviceModuleIds;
        private readonly ConcurrentDictionary<ITask, int> _taskIds;
        private readonly ConcurrentDictionary<IProgram, int> _programIds;

        private readonly ConcurrentDictionary<ITag, int> _tagIds;

        private readonly Timer _getAllTagsTimer;
        private readonly Stopwatch _stopwatch;

        private readonly Timer _getOtherTimer;
        private readonly Stopwatch _getOtherStopwatch;

        private readonly ConcurrentDictionary<ITag, JToken> _tagValueCache;

        private readonly ConcurrentDictionary<ITag, ConcurrentDictionary<string, DateTime>> _updateTags;
        private readonly ConcurrentDictionary<ITag, ConcurrentDictionary<string, DateTime>> _updateAxisTags;

        public TagSyncController(Controller controller)
        {
            Controller = controller;

            _deviceModuleIds = new ConcurrentDictionary<IDeviceModule, int>();
            _taskIds = new ConcurrentDictionary<ITask, int>();
            _programIds = new ConcurrentDictionary<IProgram, int>();
            _tagIds = new ConcurrentDictionary<ITag, int>();

            _tagValueCache = new ConcurrentDictionary<ITag, JToken>();

            _updateTags = new ConcurrentDictionary<ITag, ConcurrentDictionary<string, DateTime>>();
            _updateAxisTags = new ConcurrentDictionary<ITag, ConcurrentDictionary<string, DateTime>>();

            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                Controller, "IsOnlineChanged", OnIsOnlineChanged);

            _stopwatch = new Stopwatch();
            _getOtherStopwatch = new Stopwatch();

            _getAllTagsTimer = new Timer(100);
            _getAllTagsTimer.Elapsed += GetAllTagsHandle;
            _getAllTagsTimer.AutoReset = false;

            _getOtherTimer = new Timer(1000);
            _getOtherTimer.Elapsed += GetOtherHandle;
            _getOtherTimer.AutoReset = false;

        }
        


        ~TagSyncController()
        {
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                Controller, "IsOnlineChanged", OnIsOnlineChanged);
        }

        public Controller Controller { get; }

        public async Task<int> RebuildAsync()
        {
            _updateTags.Clear();

            CIPController cipController = new CIPController(0, Controller.CipMessager);
            int result = await cipController.WriterLockRetry();
            if (result != 0)
            {
                Logger.Error($"Writer lock failed!:{result}");
                return result;
            }

            result = await RebuildTaskIds();
            if (result < 0)
            {
                Logger.Error($"Rebuild task ids failed!:{result}");
                return result;
            }

            result = await RebuildProgramIds();
            if (result < 0)
            {
                Logger.Error($"Rebuild program ids failed!:{result}");
                return result;
            }

            result = await RebuildTagIds();
            if (result < 0)
            {
                Logger.Error($"Rebuild tag ids failed!:{result}");
                return result;
            }

            result = await RebuildDeviceModuleIds();
            if (result < 0)
            {
                Logger.Error($"Rebuild device module ids failed!:{result}");
                return result;
            }

            result = await cipController.WriterUnLock();
            if (result != 0)
            {
                Logger.Error($"Writer unlock failed!:{result}");
                return result;
            }

            //AddMotionTagToUpdate();

            return (int)TagSyncResults.Success;
        }


        internal void StartGetAllTagsTimer()
        {
            _getAllTagsTimer.Start();
            _getOtherTimer.Start();
        }
        
        public int GetTagId(ITag tag)
        {
            if (Controller.IsOnline)
            {
                int tagId;
                if (_tagIds.TryGetValue(tag, out tagId))
                    return tagId;
            }

            //TODO(gjc): need check later for PaddedEPath
            return -1;
        }

        public int GetProgramId(IProgram program)
        {
            if (Controller.IsOnline)
            {
                int programId;
                if (_programIds.TryGetValue(program, out programId))
                    return programId;
            }

            return -1;
        }

        public int GetTaskId(ITask task)
        {
            if (Controller.IsOnline)
            {
                int taskId;
                if (_taskIds.TryGetValue(task, out taskId))
                    return taskId;
            }

            return -1;
        }

        public int GetModuleId(IDeviceModule module)
        {
            if (Controller.IsOnline)
            {
                int moduleId;
                if (_deviceModuleIds.TryGetValue(module, out moduleId))
                    return moduleId;
            }

            return -1;
        }

        public async Task<int> SetValue(ITag tag, string specifier, string value)
        {
            if (!Controller.IsOnline)
                return (int)TagSyncResults.ControllerOffline;

            Logger.Trace($"SetValue {specifier}:{value}");

            var info = GetTagPropertyInfoByName(tag, specifier);
            if (info != null)
            {
                int result = await SetTagPropertyAsync(info, value);

                if (result == (int)TagSyncResults.Success)
                {
                    // read back field
                    int tagId;
                    if (!_tagIds.TryGetValue(info.Tag, out tagId))
                        return (int)TagSyncResults.GetTagIDFailed;

                    CIPTag cipTag = new CIPTag(tagId, Controller.CipMessager);

                    result = await GetFieldValueAsync(info.Field, cipTag, info.Offset);

                    Notifications.SendNotificationData(new TagNotificationData()
                    {
                        Tag = tag,
                        Type = TagNotificationData.NotificationType.Value,
                        Field = info.Field
                    });

                }

                return result;
            }

            Logger.Trace($"Not found {specifier}");
            return (int)TagSyncResults.Failure;
        }

        public async Task<Tuple<int, string>> GetValue(ITag tag, string specifier)
        {
            if (!Controller.IsOnline)
                return new Tuple<int, string>((int)TagSyncResults.ControllerOffline, string.Empty);

            var info = GetTagPropertyInfoByName(tag, specifier);
            if (info != null)
            {
                var result = await GetTagPropertyAsync(info);
                return result;
            }

            Logger.Trace($"Not found {specifier}");

            return new Tuple<int, string>((int)TagSyncResults.Failure, string.Empty);
        }

        private async Task<int> RebuildTagIds()
        {
            _tagIds.Clear();

            // controller
            CIPController cipController = new CIPController(0, Controller.CipMessager);

            foreach (var tag in Controller.Tags.OfType<Tag>())
            {
                try
                {
                    int tagId = await cipController.FindTagId(tag.Name);
                    if (!_tagIds.TryAdd(tag, tagId))
                        return (int)TagSyncResults.AddTagIDFailed;

                    Logger.Trace($"{tag.Name}:{tagId}");
                }
                catch (ICSStudioException)
                {
                    //ignore
                    Logger.Trace($"Get tagId Failed: {tag.Name}");
                }
            }


            // program
            foreach (var program in Controller.Programs)
            {
                var programId = _programIds[program];
                CIPProgram cipProgram = new CIPProgram(programId, Controller.CipMessager);

                foreach (var tag in program.Tags.OfType<Tag>())
                {
                    try
                    {
                        int tagId = await cipProgram.FindTagId(tag.Name);
                        if (!_tagIds.TryAdd(tag, tagId))
                            return (int)TagSyncResults.AddTagIDFailed;

                        Logger.Trace($"{tag.Name}:{tagId}");
                    }
                    catch (Exception)
                    {
                        //ignore
                        Logger.Trace($"Get tagId Failed: {program.Name}.{tag.Name}");
                    }
                }

            }

            return (int)TagSyncResults.Success;
        }

        private async Task<int> RebuildDeviceModuleIds()
        {
            _deviceModuleIds.Clear();

            CIPController cipController = new CIPController(0, Controller.CipMessager);
            int numModules = await cipController.GetNumModules();

            if (Controller.DeviceModules.Count != numModules)
            {
                return (int)TagSyncResults.ModuleCountMismatch;
            }

            CIPModule cipModule = new CIPModule(0, Controller.CipMessager);
            foreach (var module in Controller.DeviceModules)
            {
                int id = await cipModule.FindModuleId(module.Name);

                if (!_deviceModuleIds.TryAdd(module, id))
                    return (int)TagSyncResults.AddModuleIDFailed;

                Logger.Trace($"{module.Name}:{id}");
            }

            return (int)TagSyncResults.Success;
        }

        private async Task<int> RebuildTaskIds()
        {
            _taskIds.Clear();

            CIPController cipController = new CIPController(0, Controller.CipMessager);
            int numTasks = await cipController.GetNumTasks();

            if (Controller.Tasks.Count != numTasks)
            {
                return (int)TagSyncResults.TaskCountMismatch;
            }

            var ids = new int[numTasks];
            int currID = 0;
            for (int i = 0; i < numTasks; i++)
            {
                var temp = await cipController.FindNextTasks(currID, 1);
                currID = temp[0];
                ids[i] = currID;
            }

            for (int i = 0; i < numTasks; i++)
            {
                var cipTask = new CIPTask(ids[i], cipController.Messager);
                var name = await cipTask.GetName();

                var task = Controller.Tasks[name];
                if (task == null)
                    return (int)TagSyncResults.TaskNameMismatch;

                if (!_taskIds.TryAdd(task, ids[i]))
                    return (int)TagSyncResults.AddTaskIDFailed;

            }

            return (int)TagSyncResults.Success;
        }

        private async Task<int> RebuildProgramIds()
        {
            _programIds.Clear();

            CIPController cipController = new CIPController(0, Controller.CipMessager);
            int numPrograms = await cipController.GetNumPrograms();

            if (Controller.Programs.Count != numPrograms)
            {
                return (int)TagSyncResults.ProgramCountMismatch;
            }

            var ids = new int[numPrograms];

            int currID = 0;
            for (int i = 0; i < numPrograms; ++i)
            {
                var tmp = await cipController.FindNextPrograms(currID, 1);
                currID = tmp[0];
                ids[i] = currID;
            }

            for (int i = 0; i < numPrograms; ++i)
            {
                var t = new CIPProgram(ids[i], cipController.Messager);
                var name = await t.GetName();

                var program = Controller.Programs[name];
                if (program == null)
                    return (int)TagSyncResults.ProgramNameMismatch;


                if (!_programIds.TryAdd(program, ids[i]))
                    return (int)TagSyncResults.AddProgramIDFailed;

            }

            return (int)TagSyncResults.Success;
        }


        //[Obsolete]
        //private async Task<int> GetTagValueAsync(ITag tag, int tagId)
        //{
        //    CIPTag cipTag = new CIPTag(tagId, Controller.CipMessager);
        //    var name = await cipTag.GetName();

        //    if (!string.Equals(tag.Name, name))
        //        return (int) TagSyncResults.TagNameMismatch;

        //    Tag tagObject = tag as Tag;
        //    if (tagObject != null)
        //    {
        //        var field = tagObject.DataWrapper.Data;
        //        int result = await GetFieldValueAsync(field, cipTag, 0);
        //        if (result < 0)
        //            return result;

        //        //for AxisCIPDrive
        //        AxisCIPDrive axisCIPDrive = tagObject.DataWrapper as AxisCIPDrive;
        //        axisCIPDrive?.TagMembersToCIPAxis(tag);
        //    }

        //    return (int) TagSyncResults.Success;
        //}

        private async Task<int> GetTagValueAsync2(ITag tag, int tagId)
        {
            CIPTag cipTag = new CIPTag(tagId, Controller.CipMessager);
            var name = await cipTag.GetName();

            if (!string.Equals(tag.Name, name))
                return (int)TagSyncResults.TagNameMismatch;

            Tag tagObject = tag as Tag;
            if (tagObject != null)
            {
                IField field = tagObject.DataWrapper.Data;

                int byteSize = GetTagSize(tag);

                // 1. load all data to memory
                byte[] buffer = new byte[byteSize];

                int result = await GetTagDataAsync(cipTag, 0, buffer);
                if (result < 0)
                    return result;

                // 2. update fields
                field.Update(buffer, 0);

                //for AxisCIPDrive
                if (Controller.IsOnline)
                {
                    AxisCIPDrive axisCIPDrive = tagObject.DataWrapper as AxisCIPDrive;
                    axisCIPDrive?.TagMembersToCIPAxis(tag);
                }

            }

            return (int)TagSyncResults.Success;
        }

        private int GetTagSize(ITag tag)
        {
            Tag tagObject = tag as Tag;
            if (tagObject == null)
                return 0;

            IField field = tagObject.DataWrapper.Data;

            int byteSize;

            ArrayField arrayField = field as ArrayField;
            BoolArrayField boolArrayField = field as BoolArrayField;

            if (boolArrayField != null)
            {
                byteSize = boolArrayField.BitCount / 8;
            }
            else if (arrayField != null)
            {
                byteSize = tag.DataTypeInfo.DataType.ByteSize * arrayField.Size();
            }
            else
            {
                byteSize = tag.DataTypeInfo.DataType.ByteSize;
            }

            return byteSize;
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

            return (int)TagSyncResults.Success;

        }

        private async Task<int> GetFieldValueAsync(IField field, CIPTag cipTag, int offset)
        {
            try
            {
                BoolField boolField = field as BoolField;
                if (boolField != null)
                {
                    byte value = await cipTag.GetSint(offset);
                    boolField.value = value;
                    return (int)TagSyncResults.Success;
                }

                Int8Field int8Field = field as Int8Field;
                if (int8Field != null)
                {
                    sbyte value = (sbyte)await cipTag.GetSint(offset);
                    int8Field.value = value;
                    return (int)TagSyncResults.Success;
                }

                Int16Field int16Field = field as Int16Field;
                if (int16Field != null)
                {
                    short value = await cipTag.GetInt(offset);
                    int16Field.value = value;
                    return (int)TagSyncResults.Success;
                }

                Int32Field int32Field = field as Int32Field;
                if (int32Field != null)
                {
                    int value = await cipTag.GetDint(offset);
                    int32Field.value = value;
                    return (int)TagSyncResults.Success;
                }

                Int64Field int64Field = field as Int64Field;
                if (int64Field != null)
                {
                    long value = await cipTag.GetLint(offset);
                    int64Field.value = value;
                    return (int)TagSyncResults.Success;
                }

                RealField realField = field as RealField;
                if (realField != null)
                {
                    float value = await cipTag.GetReal(offset);
                    realField.value = value;
                    return (int)TagSyncResults.Success;
                }

                LRealField lRealField = field as LRealField;
                if (lRealField != null)
                {
                    long value = await cipTag.GetLint(offset);
                    lRealField.value = BitConverter.ToDouble(BitConverter.GetBytes(value), 0);
                    return (int)TagSyncResults.Success;
                }

                BoolArrayField boolArrayField = field as BoolArrayField;
                if (boolArrayField != null)
                {
                    int bitCount = boolArrayField.BitCount;
                    int intCount = bitCount / 32;
                    Contract.Assert(bitCount % 32 == 0);

                    for (int i = 0; i < intCount; i++)
                    {
                        int value = await cipTag.GetDint(offset + i * 4);
                        boolArrayField.Add(i * 32, value);
                    }

                    return (int)TagSyncResults.Success;
                }

                ArrayField arrayField = field as ArrayField;
                if (arrayField != null)
                {
                    foreach (var tuple in arrayField.fields)
                    {
                        int result = await GetFieldValueAsync(tuple.Item1, cipTag, offset + tuple.Item2);
                        if (result < 0)
                            return result;
                    }

                    return (int)TagSyncResults.Success;
                }

                ICompositeField compositeField = field as ICompositeField;
                if (compositeField != null)
                {
                    foreach (var tuple in compositeField.fields)
                    {
                        int result = await GetFieldValueAsync(tuple.Item1, cipTag, offset + tuple.Item2);
                        if (result < 0)
                        {
                            Logger.Error($"get field value failed, tagid:{cipTag.InstanceId}, offset:{tuple.Item2}");
                            return result;
                        }

                    }

                    return (int)TagSyncResults.Success;
                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return (int)TagSyncResults.Failure;
            }

            return (int)TagSyncResults.Success;
        }

        private async Task<int> GetFieldValueAsync2(TagPropertyInfo info, CIPTag cipTag)
        {
            try
            {
                IField field = info.Field;
                int offset = info.Offset;

                BoolField boolField = field as BoolField;
                if (boolField != null)
                {
                    byte value = await cipTag.GetSint(offset);
                    boolField.value = value;
                    return (int)TagSyncResults.Success;
                }

                Int8Field int8Field = field as Int8Field;
                if (int8Field != null)
                {
                    sbyte value = (sbyte)await cipTag.GetSint(offset);
                    int8Field.value = value;
                    return (int)TagSyncResults.Success;
                }

                Int16Field int16Field = field as Int16Field;
                if (int16Field != null)
                {
                    short value = await cipTag.GetInt(offset);
                    int16Field.value = value;
                    return (int)TagSyncResults.Success;
                }

                Int32Field int32Field = field as Int32Field;
                if (int32Field != null)
                {
                    int value = await cipTag.GetDint(offset);
                    int32Field.value = value;
                    return (int)TagSyncResults.Success;
                }

                Int64Field int64Field = field as Int64Field;
                if (int64Field != null)
                {
                    long value = await cipTag.GetLint(offset);
                    int64Field.value = value;
                    return (int)TagSyncResults.Success;
                }

                RealField realField = field as RealField;
                if (realField != null)
                {
                    float value = await cipTag.GetReal(offset);
                    realField.value = value;
                    return (int)TagSyncResults.Success;
                }

                LRealField lRealField = field as LRealField;
                if (lRealField != null)
                {
                    long value = await cipTag.GetLint(offset);
                    lRealField.value = BitConverter.ToDouble(BitConverter.GetBytes(value), 0);
                    return (int)TagSyncResults.Success;
                }

                ArrayField arrayField = field as ArrayField;
                BoolArrayField boolArrayField = field as BoolArrayField;

                int byteSize = 0;
                if (boolArrayField != null)
                {
                    byteSize = boolArrayField.BitCount / 8;
                }
                else if (arrayField != null)
                {
                    byteSize = info.DataType.ByteSize * arrayField.Size();
                }
                else
                {
                    byteSize = info.DataType.ByteSize;
                }

                byte[] buffer = new byte[byteSize];

                int result = await GetTagDataAsync(cipTag, offset, buffer);
                if (result < 0)
                    return result;

                field.Update(buffer, 0);

                //BoolArrayField boolArrayField = field as BoolArrayField;
                //if (boolArrayField != null)
                //{
                //    int bitCount = boolArrayField.BitCount;
                //    int intCount = bitCount / 32;
                //    Contract.Assert(bitCount % 32 == 0);

                //    for (int i = 0; i < intCount; i++)
                //    {
                //        int value = await cipTag.GetDint(offset + i * 4);
                //        boolArrayField.Add(i * 32, value);
                //    }

                //    return (int)TagSyncResults.Success;
                //}

                //ArrayField arrayField = field as ArrayField;
                //if (arrayField != null)
                //{
                //    foreach (var tuple in arrayField.fields)
                //    {
                //        int result = await GetFieldValueAsync(tuple.Item1, cipTag, offset + tuple.Item2);
                //        if (result < 0)
                //            return result;
                //    }

                //    return (int)TagSyncResults.Success;
                //}

                //ICompositeField compositeField = field as ICompositeField;
                //if (compositeField != null)
                //{
                //    foreach (var tuple in compositeField.fields)
                //    {
                //        int result = await GetFieldValueAsync(tuple.Item1, cipTag, offset + tuple.Item2);
                //        if (result < 0)
                //        {
                //            Logger.Error($"get field value failed, tagid:{cipTag.InstanceId}, offset:{tuple.Item2}");
                //            return result;
                //        }

                //    }

                //    return (int)TagSyncResults.Success;
                //}

            }
            catch (Exception e)
            {
                Logger.Error(e);
                return (int)TagSyncResults.Failure;
            }

            return (int)TagSyncResults.Success;
        }

        private async Task<Tuple<int, string>> GetTagPropertyAsync(TagPropertyInfo info)
        {
            if (!Controller.IsOnline)
                return new Tuple<int, string>((int)TagSyncResults.ControllerOffline, string.Empty);

            int tagId;
            if (!_tagIds.TryGetValue(info.Tag, out tagId))
                return new Tuple<int, string>((int)TagSyncResults.GetTagIDFailed, string.Empty);

            CIPTag cipTag = new CIPTag(tagId, Controller.CipMessager);

            if (info.DataType.IsBool)
            {
                BoolField boolField = info.Field as BoolField;
                if (boolField != null)
                {
                    Contract.Assert(info.SubIndex == 0);

                    byte value = await cipTag.GetSint(info.Offset);

                    return new Tuple<int, string>((int)TagSyncResults.Success, value.ToString());
                }

                Int8Field int8Field = info.Field as Int8Field;
                if (int8Field != null)
                {
                    byte value = await cipTag.GetSint(info.Offset);
                    bool boolValue = BitValue.Get(value, info.SubIndex);
                    return new Tuple<int, string>((int)TagSyncResults.Success, boolValue ? "1" : "0");
                }

                Int16Field int16Field = info.Field as Int16Field;
                if (int16Field != null)
                {
                    short value = await cipTag.GetInt(info.Offset);
                    bool boolValue = BitValue.Get(value, info.SubIndex);
                    return new Tuple<int, string>((int)TagSyncResults.Success, boolValue ? "1" : "0");
                }

                Int32Field int32Field = info.Field as Int32Field;
                if (int32Field != null)
                {
                    int value = await cipTag.GetDint(info.Offset);
                    bool boolValue = BitValue.Get(value, info.SubIndex);
                    return new Tuple<int, string>((int)TagSyncResults.Success, boolValue ? "1" : "0");
                }

                Int64Field int64Field = info.Field as Int64Field;
                if (int64Field != null)
                {
                    long value = await cipTag.GetLint(info.Offset);
                    bool boolValue = BitValue.Get(value, info.SubIndex);
                    return new Tuple<int, string>((int)TagSyncResults.Success, boolValue ? "1" : "0");
                }

                BoolArrayField boolArrayField = info.Field as BoolArrayField;
                if (boolArrayField != null)
                {
                    Contract.Assert(info.SubIndex < boolArrayField.BitCount);
                    int intIndex = info.SubIndex / 32;
                    int value = await cipTag.GetDint(info.Offset + 4 * intIndex);
                    bool boolValue = BitValue.Get(value, info.SubIndex - intIndex * 32);
                    return new Tuple<int, string>((int)TagSyncResults.Success, boolValue ? "1" : "0");
                }

                throw new NotImplementedException();
            }

            if (info.DataType.IsNumber)
            {
                Contract.Assert(info.SubIndex == 0);

                RealField realField = info.Field as RealField;
                if (realField != null)
                {
                    float value = await cipTag.GetReal(info.Offset);
                    return new Tuple<int, string>((int)TagSyncResults.Success,
                        value.ToString("g9", CultureInfo.InvariantCulture));
                }

                Int8Field int8Field = info.Field as Int8Field;
                if (int8Field != null)
                {
                    sbyte value = (sbyte)await cipTag.GetSint(info.Offset);
                    return new Tuple<int, string>((int)TagSyncResults.Success,
                        value.ToString());
                }

                Int16Field int16Field = info.Field as Int16Field;
                if (int16Field != null)
                {
                    short value = await cipTag.GetInt(info.Offset);
                    return new Tuple<int, string>((int)TagSyncResults.Success,
                        value.ToString());
                }

                Int32Field int32Field = info.Field as Int32Field;
                if (int32Field != null)
                {
                    int value = await cipTag.GetDint(info.Offset);
                    return new Tuple<int, string>((int)TagSyncResults.Success,
                        value.ToString());
                }

                Int64Field int64Field = info.Field as Int64Field;
                if (int64Field != null)
                {
                    long value = await cipTag.GetLint(info.Offset);
                    return new Tuple<int, string>((int)TagSyncResults.Success,
                        value.ToString());
                }

                LRealField lRealField = info.Field as LRealField;
                if (lRealField != null)
                {
                    long value = await cipTag.GetLint(info.Offset);
                    double doubleValue = BitConverter.ToDouble(BitConverter.GetBytes(value), 0);
                    return new Tuple<int, string>((int)TagSyncResults.Success,
                        doubleValue.ToString("g17", CultureInfo.InvariantCulture));
                }

                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        private async Task<int> SetTagPropertyAsync(TagPropertyInfo info, string value)
        {
            if (!Controller.IsOnline)
                return (int)TagSyncResults.ControllerOffline;

            int tagId;
            if (!_tagIds.TryGetValue(info.Tag, out tagId))
                return (int)TagSyncResults.GetTagIDFailed;

            CIPTag cipTag = new CIPTag(tagId, Controller.CipMessager);

            if (info.DataType.IsStringType)
            {
                var bytes = ValueConverter.ToBytes(value);

                ICompositeField stringField = info.Field as ICompositeField;
                Contract.Assert(stringField != null);
                Contract.Assert(stringField.fields.Count == 2);

                var lengthField = (Int32Field)stringField.fields[0].Item1;
                var arrayField = (ArrayField)stringField.fields[1].Item1;
                Contract.Assert(lengthField != null);
                Contract.Assert(arrayField != null);

                int maxCount = arrayField.Size();
                Contract.Assert(bytes.Count <= maxCount);


                // length
                await cipTag.SetDint(info.Offset + stringField.fields[0].Item2, bytes.Count);

                // array
                int i;
                int offset = info.Offset + stringField.fields[1].Item2;
                for (i = 0; i < bytes.Count; i++)
                {
                    await cipTag.SetSint(offset + arrayField.fields[i].Item2, bytes[i]);
                }

                for (; i < maxCount; i++)
                {
                    await cipTag.SetSint(offset + arrayField.fields[i].Item2, 0);
                }

                return (int)TagSyncResults.Success;
            }

            if (info.DataType.IsBool)
            {
                bool boolValue = ValueConverter.ToBoolean(value);

                BoolField boolField = info.Field as BoolField;
                if (boolField != null)
                {
                    Contract.Assert(info.SubIndex == 0);

                    await cipTag.SetSint(info.Offset, (byte)(boolValue ? 1 : 0));

                    return (int)TagSyncResults.Success;
                }

                Int8Field int8Field = info.Field as Int8Field;
                if (int8Field != null)
                {
                    sbyte newValue = BitValue.Set(int8Field.value, info.SubIndex, boolValue);
                    await cipTag.SetSint(info.Offset, (byte)newValue);
                    return (int)TagSyncResults.Success;
                }

                Int16Field int16Field = info.Field as Int16Field;
                if (int16Field != null)
                {
                    short newValue = BitValue.Set(int16Field.value, info.SubIndex, boolValue);
                    await cipTag.SetInt(info.Offset, newValue);
                    return (int)TagSyncResults.Success;
                }

                Int32Field int32Field = info.Field as Int32Field;
                if (int32Field != null)
                {
                    int newValue = BitValue.Set(int32Field.value, info.SubIndex, boolValue);
                    await cipTag.SetDint(info.Offset, newValue);
                    return (int)TagSyncResults.Success;
                }

                Int64Field int64Field = info.Field as Int64Field;
                if (int64Field != null)
                {
                    long newValue = BitValue.Set(int64Field.value, info.SubIndex, boolValue);
                    await cipTag.SetLint(info.Offset, newValue);
                    return (int)TagSyncResults.Success;
                }

                BoolArrayField boolArrayField = info.Field as BoolArrayField;
                if (boolArrayField != null)
                {
                    Contract.Assert(info.SubIndex < boolArrayField.BitCount);
                    int intIndex = info.SubIndex / 32;
                    int newValue = boolArrayField.GetInt(intIndex);
                    newValue = BitValue.Set(newValue, info.SubIndex - intIndex * 32, boolValue);
                    await cipTag.SetDint(info.Offset + 4 * intIndex, newValue);
                    return (int)TagSyncResults.Success;
                }

                throw new NotImplementedException();
            }

            if (info.DataType.IsNumber)
            {
                Contract.Assert(info.SubIndex == 0);

                RealField realField = info.Field as RealField;
                if (realField != null)
                {
                    float newValue = ValueConverter.ToFloat(value);
                    await cipTag.SetReal(info.Offset, newValue);
                    return (int)TagSyncResults.Success;
                }

                Int8Field int8Field = info.Field as Int8Field;
                if (int8Field != null)
                {
                    sbyte newValue = sbyte.Parse(value);
                    await cipTag.SetSint(info.Offset, (byte)newValue);
                    return (int)TagSyncResults.Success;
                }

                Int16Field int16Field = info.Field as Int16Field;
                if (int16Field != null)
                {
                    short newValue = short.Parse(value);
                    await cipTag.SetInt(info.Offset, newValue);
                    return (int)TagSyncResults.Success;
                }

                Int32Field int32Field = info.Field as Int32Field;
                if (int32Field != null)
                {
                    int newValue = int.Parse(value);
                    await cipTag.SetDint(info.Offset, newValue);
                    return (int)TagSyncResults.Success;
                }

                Int64Field int64Field = info.Field as Int64Field;
                if (int64Field != null)
                {
                    long newValue = long.Parse(value);
                    await cipTag.SetLint(info.Offset, newValue);
                    return (int)TagSyncResults.Success;
                }

                LRealField lRealField = info.Field as LRealField;
                if (lRealField != null)
                {
                    double newValue = double.Parse(value);
                    long longValue = BitConverter.ToInt64(BitConverter.GetBytes(newValue), 0);
                    await cipTag.SetLint(info.Offset, longValue);
                    return (int)TagSyncResults.Success;
                }

                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        private TagPropertyInfo GetTagPropertyInfoByName(ITag tag, string name)
        {
            Tag tagObject = tag as Tag;
            if (tagObject == null)
                return null;

            TagExpressionParser parser = new TagExpressionParser();
            var tagExpression = parser.Parser(name);
            if (tagExpression == null)
                return null;

            SimpleTagExpression simpleTagExpression = parser.GetSimpleTagExpression(tagExpression);
            if (!string.Equals(tag.Name, simpleTagExpression.TagName, StringComparison.OrdinalIgnoreCase))
                return null;

            int offset = 0;
            DataTypeInfo dataTypeInfo = tagObject.DataTypeInfo;
            IField field = tagObject.DataWrapper.Data;
            bool isBit = false;
            int bitOffset = -1;

            TagExpressionBase expression = simpleTagExpression;

            while (expression.Next != null)
            {
                expression = expression.Next;

                int bitMemberNumber = -1;
                var bitMemberNumberAccessExpression = expression as BitMemberNumberAccessExpression;
                var bitMemberExpressionAccessExpression = expression as BitMemberExpressionAccessExpression;

                if (bitMemberNumberAccessExpression != null)
                {
                    bitMemberNumber = bitMemberNumberAccessExpression.Number;
                }
                else if (bitMemberExpressionAccessExpression != null)
                {
                    if (bitMemberExpressionAccessExpression.Number.HasValue)
                        bitMemberNumber = bitMemberExpressionAccessExpression.Number.Value;
                    else
                        return null;
                }

                if (bitMemberNumber >= 0)
                {
                    if (!dataTypeInfo.DataType.IsInteger || dataTypeInfo.Dim1 != 0)
                        return null;

                    isBit = true;
                    bitOffset = bitMemberNumber;

                    IDataType bitType;
                    switch (dataTypeInfo.DataType.BitSize)
                    {
                        case 8:
                            bitType = BOOL.SInst;
                            break;
                        case 16:
                            bitType = BOOL.IInst;
                            break;
                        case 32:
                            bitType = BOOL.DInst;
                            break;
                        case 64:
                            bitType = BOOL.LInst;
                            break;
                        default:
                            throw new NotSupportedException();
                    }

                    dataTypeInfo = new DataTypeInfo() { DataType = bitType };

                    break;
                }

                //
                var memberAccessExpression = expression as MemberAccessExpression;
                if (memberAccessExpression != null)
                {
                    var compositeField = field as ICompositeField;
                    var compositiveType = dataTypeInfo.DataType as CompositiveType;
                    if (compositeField != null && compositiveType != null && dataTypeInfo.Dim1 == 0)
                    {
                        var dataTypeMember = compositiveType.TypeMembers[memberAccessExpression.Name] as DataTypeMember;
                        if (dataTypeMember == null)
                            return null;

                        //if (dataTypeMember.IsHidden)
                        //    return null;

                        field = compositeField.fields[dataTypeMember.FieldIndex].Item1;
                        offset += compositeField.fields[dataTypeMember.FieldIndex].Item2;
                        dataTypeInfo = dataTypeMember.DataTypeInfo;
                        if (dataTypeMember.IsBit && dataTypeInfo.Dim1 == 0)
                        {
                            isBit = true;
                            bitOffset = dataTypeMember.BitOffset;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }


                var elementAccessExpression = expression as ElementAccessExpression;
                if (elementAccessExpression != null)
                {
                    Contract.Assert(elementAccessExpression.Indexes != null);

                    int index;

                    switch (elementAccessExpression.Indexes.Count)
                    {
                        case 1:
                            if (dataTypeInfo.Dim1 > 0 && dataTypeInfo.Dim2 == 0 && dataTypeInfo.Dim3 == 0)
                            {
                                index = elementAccessExpression.Indexes[0];
                                break;
                            }
                            else
                                return null;
                        case 2:
                            if (dataTypeInfo.Dim1 > 0 && dataTypeInfo.Dim2 > 0 && dataTypeInfo.Dim3 == 0)
                            {
                                index = elementAccessExpression.Indexes[0] * dataTypeInfo.Dim1 +
                                        elementAccessExpression.Indexes[1];
                                break;
                            }
                            else
                                return null;
                        case 3:
                            if (dataTypeInfo.Dim1 > 0 && dataTypeInfo.Dim2 > 0 && dataTypeInfo.Dim3 > 0)
                            {
                                index = elementAccessExpression.Indexes[0] * dataTypeInfo.Dim2 * dataTypeInfo.Dim1 +
                                        elementAccessExpression.Indexes[1] * dataTypeInfo.Dim1 +
                                        elementAccessExpression.Indexes[2];
                                break;
                            }
                            else
                                return null;
                        default:
                            throw new NotImplementedException();
                    }

                    ArrayField arrayField = field as ArrayField;
                    BoolArrayField boolArrayField = field as BoolArrayField;

                    if (arrayField == null && boolArrayField == null)
                        return null;

                    if (arrayField != null)
                    {
                        dataTypeInfo = new DataTypeInfo { DataType = dataTypeInfo.DataType };
                        field = arrayField.fields[index].Item1;
                        offset += arrayField.fields[index].Item2;
                    }

                    if (boolArrayField != null)
                    {
                        dataTypeInfo = new DataTypeInfo { DataType = dataTypeInfo.DataType };
                        field = boolArrayField;
                        isBit = true;
                        bitOffset = index;
                    }

                }
            }

            if (dataTypeInfo.DataType.IsStringType ||
                dataTypeInfo.DataType.IsBool ||
                dataTypeInfo.DataType.IsNumber)
            {
                TagPropertyInfo info = new TagPropertyInfo(name, tag)
                {
                    DataType = dataTypeInfo.DataType,
                    Field = field,
                    Offset = offset,
                    SubIndex = isBit ? bitOffset : 0
                };

                return info;
            }

            return null;
        }

        //private List<TagPropertyInfo> BuildTagPropertyName(
        //    Tag tag, string name, IDataType dataType,
        //    IField field, int offset)
        //{
        //    List<TagPropertyInfo> infos = new List<TagPropertyInfo>();

        //    if (dataType.IsBool || dataType.IsReal)
        //    {
        //        TagPropertyInfo info = new TagPropertyInfo(name, tag)
        //        {
        //            DataType = dataType, Field = field, Offset = offset
        //        };
        //        infos.Add(info);
        //    }
        //    else if (dataType.IsInteger)
        //    {
        //        TagPropertyInfo info = new TagPropertyInfo(name, tag)
        //        {
        //            DataType = dataType, Field = field, Offset = offset
        //        };
        //        infos.Add(info);

        //        // add bit
        //        IDataType bitType;
        //        switch (dataType.BitSize)
        //        {
        //            case 8:
        //                bitType = BOOL.SInst;
        //                break;
        //            case 16:
        //                bitType = BOOL.IInst;
        //                break;
        //            case 32:
        //                bitType = BOOL.DInst;
        //                break;
        //            case 64:
        //                bitType = BOOL.LInst;
        //                break;
        //            default:
        //                throw new NotSupportedException();
        //        }

        //        for (var i = 0; i < dataType.BitSize; i++)
        //        {
        //            info = new TagPropertyInfo($"{name}.{i}", tag)
        //            {
        //                DataType = bitType, Field = field, Offset = offset, SubIndex = i
        //            };
        //            infos.Add(info);
        //        }
        //    }
        //    else
        //    {
        //        // struct, UserDefinedDataType, ModuleDefinedDataType
        //        var compositeField = (ICompositeField) field;
        //        Contract.Assert(compositeField != null);

        //        // for string and udt-string
        //        if (dataType.IsStringType)
        //        {
        //            TagPropertyInfo info = new TagPropertyInfo(name, tag)
        //            {
        //                DataType = dataType, Field = field, Offset = offset
        //            };
        //            infos.Add(info);
        //        }

        //        foreach (var member in ((CompositiveType) dataType).TypeMembers)
        //        {
        //            if (member.IsHidden)
        //                continue;

        //            string memberName = $"{name}.{member.Name}";
        //            var dataTypeInfo = member.DataTypeInfo;
        //            var dataTypeMember = member as DataTypeMember;
        //            Contract.Assert(dataTypeMember != null);

        //            var tuple = compositeField.fields[dataTypeMember.FieldIndex];

        //            if (dataTypeInfo.Dim1 == 0)
        //            {
        //                var result =
        //                    BuildTagPropertyName(tag, memberName, dataTypeInfo.DataType, tuple.Item1,
        //                        offset + tuple.Item2);

        //                if (dataTypeMember.IsBit)
        //                {
        //                    Contract.Assert(result.Count == 1);
        //                    result[0].SubIndex = dataTypeMember.BitOffset;
        //                }


        //                infos.AddRange(result);
        //            }
        //            else
        //            {
        //                // array

        //                var isBoolArray = dataTypeInfo.DataType.IsBool;
        //                if (isBoolArray)
        //                    Contract.Assert(tuple.Item1 is BoolArrayField);
        //                else
        //                    Contract.Assert(tuple.Item1 is ArrayField);

        //                var dim3 = Math.Max(dataTypeInfo.Dim3, 1);
        //                var dim2 = Math.Max(dataTypeInfo.Dim2, 1);

        //                var index = 0;
        //                for (var z = 0; z < dim3; z++)
        //                for (var y = 0; y < dim2; y++)
        //                for (var x = 0; x < dataTypeInfo.Dim1; x++)
        //                {
        //                    string itemName;
        //                    if (dataTypeInfo.Dim3 > 0)
        //                        itemName = $"{memberName}[{z},{y},{x}]";
        //                    else if (dataTypeInfo.Dim2 > 0)
        //                        itemName = $"{memberName}[{y},{x}]";
        //                    else
        //                        itemName = $"{memberName}[{x}]";

        //                    if (isBoolArray)
        //                    {
        //                        TagPropertyInfo info = new TagPropertyInfo(itemName, tag)
        //                        {
        //                            DataType = dataTypeInfo.DataType,
        //                            Field = tuple.Item1,
        //                            Offset = offset + tuple.Item2,
        //                            SubIndex = index
        //                        };
        //                        infos.Add(info);
        //                    }
        //                    else
        //                    {
        //                        var arrayField = (ArrayField) tuple.Item1;

        //                        var result = BuildTagPropertyName(tag, itemName, dataTypeInfo.DataType,
        //                            arrayField.fields[index].Item1,
        //                            offset + tuple.Item2 + arrayField.fields[index].Item2);

        //                        infos.AddRange(result);
        //                    }

        //                    index++;
        //                }
        //            }

        //        }

        //    }

        //    return infos;
        //}

        private async void GetAllTagsHandle(object sender, ElapsedEventArgs e)
        {
            //var startTime = DateTime.Now;

            _stopwatch.Restart();

            try
            {
                if (Controller.IsOnline)
                {
                    {
                        //Stopwatch stopwatch = Stopwatch.StartNew();

                        if (Controller.IsOnline)
                            await PullTagsAsync();


                        if (Controller.IsOnline)
                            Notifications.Publish(new MessageData() { Type = MessageData.MessageType.PullFinished });

                        //Logger.Trace($"GetAllTags {stopwatch.ElapsedMilliseconds} ms");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("In Get All Tags " + ex);
            }
            finally
            {
                if (Controller.IsOnline)
                {
                    double elapsedTime = _stopwatch.ElapsedMilliseconds;

                    double interval = 300 - elapsedTime;

                    if (interval < 50)
                        interval = 50;

                    _getAllTagsTimer.Interval = interval;

                    _getAllTagsTimer.Start();
                }

            }
        }

        private async void GetOtherHandle(object sender, ElapsedEventArgs e)
        {
            _getOtherStopwatch.Restart();

            try
            {
                if (Controller.IsOnline)
                {
                    {
                        if (Controller.IsOnline)
                            await PullAxisPropertiesAsync();

                        if (Controller.IsOnline)
                            await PullAllPrograms();

                        if (Controller.IsOnline)
                            await PullAllTasks();

                        if (Controller.IsOnline)
                            await PullAllModules();


                        //Logger.Trace($"GetOther {stopwatch.ElapsedMilliseconds} ms");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("In Get Other " + ex);
            }
            finally
            {
                if (Controller.IsOnline)
                {
                    double elapsedTime = _getOtherStopwatch.ElapsedMilliseconds;

                    double interval = 1000 - elapsedTime;

                    if (interval < 50)
                        interval = 50;

                    _getOtherTimer.Interval = interval;

                    _getOtherTimer.Start();
                }

            }
        }

        private async Task<int> PullAxisPropertiesAsync()
        {
            var tags = _updateAxisTags.Keys.ToList();

            DateTime now = DateTime.Now;
            now = now.AddSeconds(-2);

            foreach (var tag in tags)
            {
                ConcurrentDictionary<string, DateTime> tagLiveTime;
                if (_updateAxisTags.TryGetValue(tag, out tagLiveTime))
                {
                    var propertyNames = tagLiveTime.Keys.ToList();
                    List<string> updatePropertyNames = new List<string>();

                    var axisCIPDrive = (tag as Tag)?.DataWrapper as AxisCIPDrive;
                    CIPAxis originalAxis = axisCIPDrive?.CIPAxis;
                    CIPAxis updateAxis = originalAxis?.Clone() as CIPAxis;

                    if (updateAxis == null)
                    {
                        _updateAxisTags.TryRemove(tag, out tagLiveTime);
                        continue;
                    }

                    updateAxis.InstanceId = GetTagId(tag);
                    updateAxis.Messager = Controller.CipMessager;

                    foreach (var propertyName in propertyNames)
                    {
                        DateTime dataTime;
                        if (tagLiveTime.TryGetValue(propertyName, out dataTime))
                        {
                            if (dataTime < now)
                            {
                                tagLiveTime.TryRemove(propertyName, out dataTime);
                            }
                            else
                            {
                                updatePropertyNames.Add(propertyName);

                                await updateAxis.GetAttributeSingle(propertyName);

                            }
                        }
                    }

                    if (updatePropertyNames.Count > 0)
                    {
                        var differentAttributeList =
                            CipAttributeHelper.GetDifferentAttributeList(
                                originalAxis,
                                updateAxis, updatePropertyNames);

                        if (differentAttributeList.Count > 0)
                        {
                            originalAxis.Apply(updateAxis, differentAttributeList);

                            axisCIPDrive.NotifyParentPropertyChanged(
                                CipAttributeHelper.AttributeIdsToNames(typeof(CIPAxis), differentAttributeList)
                                    .ToArray());

                            // log
                            foreach (string attributeName in CipAttributeHelper.AttributeIdsToNames(typeof(CIPAxis),
                                         differentAttributeList))
                            {
                                Logger.Trace(
                                    $"PLC Update, {tag.Name}.{attributeName}, value:{updateAxis.GetAttributeValueString(attributeName)}");
                            }

                        }
                    }

                    if (tagLiveTime.IsEmpty)
                    {
                        _updateAxisTags.TryRemove(tag, out tagLiveTime);
                        //Logger.Trace($"remove tag for update:{tag.Name}");
                    }
                }
            }

            return (int)TagSyncResults.Success;

        }

        private async Task<int> PullAllModules()
        {
            foreach (KeyValuePair<IDeviceModule, int> keyValuePair in _deviceModuleIds)
            {
                int result = await GetModuleInfoAsync(keyValuePair.Key, keyValuePair.Value);
                if (result < 0)
                    return result;
            }

            return (int)TagSyncResults.Success;
        }

        private async Task<int> GetModuleInfoAsync(IDeviceModule module, int moduleId)
        {
            CIPModule cipModule = new CIPModule(moduleId, Controller.CipMessager);

            DeviceModule.DeviceModule deviceModule = module as DeviceModule.DeviceModule;

            if (deviceModule != null)
            {
                short entryStatus = await cipModule.GetEntryStatus();
                short faultCode = await cipModule.GetFaultCode();
                int faultInfo = await cipModule.GetFaultInfo();

                deviceModule.EntryStatus = entryStatus;
                deviceModule.FaultCode = faultCode;
                deviceModule.FaultInfo = faultInfo;
            }

            return (int)TagSyncResults.Success;
        }

        private async Task<int> PullAllTasks()
        {
            foreach (KeyValuePair<ITask, int> keyValuePair in _taskIds)
            {
                int result = await GetTaskInfoAsync(keyValuePair.Key, keyValuePair.Value);
                if (result < 0)
                    return result;
            }

            return (int)TagSyncResults.Success;
        }

        private async Task<int> GetTaskInfoAsync(ITask task, int taskId)
        {
            CIPTask cipTask = new CIPTask(taskId, Controller.CipMessager);
            var name = await cipTask.GetName();

            if (!string.Equals(task.Name, name))
                return (int)TagSyncResults.TaskNameMismatch;

            CTask taskObject = task as CTask;
            if (taskObject != null)
            {
                float maxScanTime = await cipTask.GetMaxScanTime();
                float lastScanTime = await cipTask.GetLastScanTime();
                float maxIntervalTime = await cipTask.GetMaxIntervalTime();
                float minIntervalTime = await cipTask.GetMinIntervalTime();
                int overlapCount = await cipTask.GetOverlapCount();
                bool isInhibited = await cipTask.GetInhibit();

                taskObject.MaxScanTime = (long)(maxScanTime * 1000);
                taskObject.LastScanTime = (long)(lastScanTime * 1000);
                taskObject.MaxIntervalTime = (long)(maxIntervalTime * 1000);
                taskObject.MinIntervalTime = (long)(minIntervalTime * 1000);
                taskObject.OverlapCount = overlapCount;
                taskObject.IsInhibited = isInhibited;
            }

            return (int)TagSyncResults.Success;
        }

        private async Task<int> PullAllPrograms()
        {
            foreach (KeyValuePair<IProgram, int> keyValuePair in _programIds)
            {
                int result = await GetProgramInfoAsync(keyValuePair.Key, keyValuePair.Value);
                if (result < 0)
                    return result;
            }

            return (int)TagSyncResults.Success;
        }

        private async Task<int> GetProgramInfoAsync(IProgram program, int programId)
        {
            CIPProgram cipProgram = new CIPProgram(programId, Controller.CipMessager);
            var name = await cipProgram.GetName();

            if (!string.Equals(program.Name, name))
                return (int)TagSyncResults.ProgramNameMismatch;

            Program programObject = program as Program;
            if (programObject != null)
            {
                float maxScanTime = await cipProgram.GetMaxScanTime();
                float lastScanTime = await cipProgram.GetLastScanTime();
                bool inhibited = await cipProgram.GetInhibit();

                programObject.MaxScanTime = (long)maxScanTime;
                programObject.LastScanTime = (long)lastScanTime;
                programObject.Inhibited = inhibited;
            }

            return (int)TagSyncResults.Success;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        internal async Task<int> PullAllTagsAsync()
        {
            foreach (KeyValuePair<ITag, int> keyValuePair in _tagIds)
            {
                try
                {
                    var result = await GetTagValueAsync2(keyValuePair.Key, keyValuePair.Value);
                    if (result < 0)
                        return result;
                }
                catch (Exception e)
                {
                    Logger.Trace($"Get Tag Value failed: {keyValuePair.Key.Name}, message:{e.Message}");
                    return (int)TagSyncResults.Failure;
                }
            }

            return (int)TagSyncResults.Success;
        }

        private async Task<int> PullTagsAsync()
        {
            var tags = _updateTags.Keys.ToList();

            //Logger.Trace($"{DateTime.Now:hh:mm:ss.fff} Pull tags count: {tags.Count}");

            foreach (var tag in tags)
            {
                //Logger.Trace($"Pull tag:{tag.Name}.");
                await PullTagAsync(tag);
            }

            // update list
            DateTime now = DateTime.Now;
            now = now.AddSeconds(-2);

            foreach (var tag in tags)
            {
                ConcurrentDictionary<string, DateTime> tagLiveTime;
                if (_updateTags.TryGetValue(tag, out tagLiveTime))
                {
                    var members = tagLiveTime.Keys.ToList();
                    foreach (var member in members)
                    {
                        DateTime dataTime;
                        if (tagLiveTime.TryGetValue(member, out dataTime))
                        {
                            if (dataTime < now)
                            {
                                tagLiveTime.TryRemove(member, out dataTime);
                            }
                        }
                    }

                    if (tagLiveTime.IsEmpty)
                    {
                        _updateTags.TryRemove(tag, out tagLiveTime);
                        //Logger.Trace($"remove tag for update:{tag.Name}");
                    }
                }
            }

            return (int)TagSyncResults.Success;
        }

        private async Task<int> PullTagAsync(ITag tag)
        {
            int tagId;
            if (_tagIds.TryGetValue(tag, out tagId))
            {
                try
                {

                    int result = 0;
                    int tagSize = GetTagSize(tag);

                    if (tagSize > 1400)
                    {
                        ConcurrentDictionary<string, DateTime> tagLiveTime;
                        if (_updateTags.TryGetValue(tag, out tagLiveTime))
                        {
                            var members = tagLiveTime.Keys.ToList();

                            if (!members.Contains(tag.Name))
                            {
                                if (tagSize > 1400 * members.Count)
                                {
                                    foreach (var member in members)
                                    {
                                        var info = GetTagPropertyInfoByName(tag, member);
                                        if (info != null)
                                        {
                                            CIPTag cipTag = new CIPTag(tagId, Controller.CipMessager);

                                            var tempResult = await GetFieldValueAsync2(info, cipTag);
                                            if (result > tempResult)
                                                result = tempResult;
                                        }
                                    }

                                    if (result < 0)
                                    {
                                        Logger.Trace($"Get Tag member failed: {tag.Name}");
                                        return result;
                                    }
                                }
                            }
                        }
                    }


                    result = await GetTagValueAsync2(tag, tagId);
                    if (result < 0)
                    {
                        Logger.Trace($"Get Tag Value failed: {tag.Name}");
                        return result;
                    }

                    return 0;
                }
                catch (Exception e)
                {
                    Logger.Trace($"Get Tag Value failed: {tag.Name}, message:{e.Message}");
                    return (int)TagSyncResults.Failure;
                }
            }

            Logger.Trace($"Not found tagid: {tag.Name}");
            return (int)TagSyncResults.Failure;
        }

        private void AddMotionTagToUpdate()
        {
            foreach (var tag in Controller.Tags)
            {
                if (tag.DataTypeInfo.DataType.IsAxisType ||
                    tag.DataTypeInfo.DataType.IsMotionGroupType)
                {
                    ConcurrentDictionary<string, DateTime> tagLiveTime = new ConcurrentDictionary<string, DateTime>();
                    tagLiveTime.TryAdd("", DateTime.MaxValue);

                    _updateTags.TryAdd(tag, tagLiveTime);

                }
            }
        }

        internal void PreOnlineChanged()
        {
            try
            {
                // reset
                ResetTasks();
                ResetPrograms();

                // save 
                SaveTagsValue();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            Task.Run(() =>
            {
                try
                {
                    if (e.NewValue)
                        return;

                    // restore
                    RestoreTagsValue();

                    Notifications.Publish(new MessageData() { Type = MessageData.MessageType.Restored });
                }
                catch (Exception exception)
                {
                    Logger.Error(exception);
                }
            });
        }

        private void ResetPrograms()
        {
            foreach (var program in Controller.Programs.OfType<Program>())
            {
                program.MaxScanTime = 0;
                program.LastScanTime = 0;
            }
        }

        private void ResetTasks()
        {
            foreach (CTask task in Controller.Tasks.OfType<CTask>())
            {
                task.MaxScanTime = 0;
                task.LastScanTime = 0;
                task.MaxIntervalTime = 0;
                task.MinIntervalTime = 0;
                task.OverlapCount = 0;
            }
        }

        internal void RestoreTagsValue()
        {
            foreach (var tag in Controller.Tags)
            {
                Tag tagObject = tag as Tag;
                if (tagObject != null)
                {
                    JToken value;
                    if (_tagValueCache.TryGetValue(tagObject, out value))
                    {
                        tagObject.DataWrapper.Data.Update(value);
                    }
                }
            }

            foreach (var program in Controller.Programs)
            {
                foreach (var tag in program.Tags)
                {
                    Tag tagObject = tag as Tag;
                    if (tagObject != null)
                    {
                        JToken value;
                        if (_tagValueCache.TryGetValue(tagObject, out value))
                        {
                            tagObject.DataWrapper.Data.Update(value);
                        }
                    }
                }
            }

        }

        internal void SaveTagsValue()
        {
            _tagValueCache.Clear();

            foreach (var tag in Controller.Tags)
            {
                Tag tagObject = tag as Tag;
                if (tagObject != null)
                {
                    _tagValueCache.TryAdd(tag, tagObject.DataWrapper.Data.ToJToken());
                }
            }

            foreach (var program in Controller.Programs)
            {
                foreach (var tag in program.Tags)
                {
                    Tag tagObject = tag as Tag;
                    if (tagObject != null)
                    {
                        _tagValueCache.TryAdd(tag, tagObject.DataWrapper.Data.ToJToken());
                    }
                }
            }

        }

        private class TagPropertyInfo
        {
            public TagPropertyInfo(string name, ITag tag)
            {
                Name = name;
                Tag = tag;
            }

            public string Name { get; }
            public int Offset { get; set; } // for field
            public IDataType DataType { get; set; }
            public IField Field { get; set; }
            public int SubIndex { get; set; }
            public ITag Tag { get; }
        }

        public async Task<int> AddTagInController(Tag tag)
        {
            if (Controller.IsOnline)
            {
                if (!_tagIds.ContainsKey(tag))
                {
                    CIPController cipController = new CIPController(0, Controller.CipMessager);


                    int tagId = await cipController.FindTagId(tag.Name);
                    if (!_tagIds.TryAdd(tag, tagId))
                        return (int)TagSyncResults.AddTagIDFailed;

                    Logger.Trace($"User add {tag.Name}:{tagId} in controller");

                    return (int)TagSyncResults.Success;

                }
            }

            return (int)TagSyncResults.ControllerOffline;
        }

        public async Task<int> AddTagInProgram(Tag tag, IProgram program)
        {
            if (Controller.IsOnline)
            {
                if (!_tagIds.ContainsKey(tag))
                {
                    CIPController cipController = new CIPController(0, Controller.CipMessager);

                    await cipController.EnterReadLock();

                    int programId = await cipController.FindProgramId(program.Name);

                    CIPProgram cipProgram = new CIPProgram(programId, Controller.CipMessager);

                    int tagId = await cipProgram.FindTagId(tag.Name);

                    await cipController.ExitReadLock();

                    if (!_tagIds.TryAdd(tag, tagId))
                        return (int)TagSyncResults.AddTagIDFailed;

                    Logger.Trace($"add {tag.Name}:{tagId} in program:{program.Name}");

                    return (int)TagSyncResults.Success;

                }
            }

            return (int)TagSyncResults.ControllerOffline;

        }

        public void Update(ITag tag, string member)
        {
            Update(_updateTags, tag, member);
        }

        public void UpdateAxisProperties(ITag tag, string propertyName)
        {
            Update(_updateAxisTags, tag, propertyName);
        }

        private void Update(
            ConcurrentDictionary<ITag, ConcurrentDictionary<string, DateTime>> updateDictionary,
            ITag tag, string name)
        {
            if (!updateDictionary.ContainsKey(tag))
            {
                updateDictionary.TryAdd(tag, new ConcurrentDictionary<string, DateTime>());
            }

            ConcurrentDictionary<string, DateTime> tagLiveTime;

            if (updateDictionary.TryGetValue(tag, out tagLiveTime))
            {
                tagLiveTime[name] = DateTime.Now;
            }
            else
            {
                Logger.Trace($"not found {tag.Name}.");
            }
        }
    }
}
