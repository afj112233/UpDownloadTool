using ICSStudio.Cip.Objects;
using ICSStudio.CipConnection;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Tags;
using MessagePack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.Interfaces.Tags;

namespace ICSStudio.SimpleServices.Common
{
    public partial class Controller
    {
        #region Is online

        private bool _isOnline;
        private readonly object _isOnlineLock = new object();

        public bool IsOnline
        {
            get
            {
                lock (_isOnlineLock)
                {
                    return _isOnline;
                }

            }
            set
            {
                bool oldValue;

                lock (_isOnlineLock)
                {
                    if (_isOnline == value)
                        return;

                    oldValue = _isOnline;
                    _isOnline = value;
                }

                var handler = IsOnlineChanged;
                handler?.Invoke(this, new IsOnlineChangedEventArgs
                {
                    OldValue = oldValue,
                    NewValue = value
                });
            }
        }

        public bool IsConnected
        {
            get
            {
                var messager = CipMessager;
                return messager != null && messager.ConnectionStatus == ConnectionStatus.ForwardOpen;
            }
        }

        /// <summary>
        /// be careful in ui
        /// </summary>
        public event IsOnlineChangedEventHandler IsOnlineChanged;

        #endregion

        #region Connect

        public async Task<int> ConnectAsync(string commPath)
        {
            if (IsConnected)
                return 0;

            string ipAddress = commPath;

            GoOffline();

            CipMessager = new DeviceConnection(ipAddress)
            {
                SendTimeout = 1000, ReceiveTimeout = 0
            };

            int result = await _connection.OnLine(true);

            if (result != 0)
            {
                Log($"Connect to {ipAddress} failed!");
            }

            return result;
        }

        #endregion

        #region Upload

        public async Task Upload(bool performMerge)
        {
            IsLoading = true;

            Clear();

            if (CheckCipMessager(_connection) < 0)
            {
                IsLoading = false;
                return;
            }

            Stopwatch stopwatch = Stopwatch.StartNew();

            CIPController controller = new CIPController(0, _connection);

            await controller.WriterLockRetry();

            Name = await controller.GetName();
            MajorFaultProgram = await controller.GetMajorFaultProgram();
            PowerLossProgram = await controller.GetPowerLossProgram();
            AddTimeSetting(FromMsgpack(await controller.GetTimeSynchronize()));

            await UploadDataTypes(controller);
            await UploadAOIDefs(controller);

            FinalizeTypeCreation();
            //FinalizeAoi();
            await UploadModules(controller);
            await UploadTags(controller);
            await UploadPrograms(controller);
            await UploadTasks(controller);
            await UploadTrends(controller);
            PostLoadJson();
            
            await _transactionManager.PostUploadSetupAsync(CipMessager);

            await controller.WriterUnLock();

            double elapsedTime = stopwatch.ElapsedMilliseconds;

            Logger.Info($"Upload {Name} elapsed time: {elapsedTime} ms");

            IsLoading = false;
        }

        public async Task UploadDataTypes(CIPController ctrl)
        {
            int num = await ctrl.GetNumUDTypes();
            Console.WriteLine($"num udtypes {await ctrl.GetNumUDTypes()}");
            var ids = new int[num];

            int curr_id = 0;
            for (int i = 0; i < num; ++i)
            {
                var tmp = await ctrl.FindNextUDTypes(curr_id, 1);
                curr_id = tmp[0];
                ids[i] = curr_id;
            }

            for (int i = 0; i < num; ++i)
            {
                var t = new CIPUDType(ids[i], ctrl.Messager);
                var cfg = await t.GetConfig();
                AddDataType(FromMsgpack(cfg));
            }
        }

        public async Task UploadAOIDefs(CIPController ctrl)
        {
            int num = await ctrl.GetNumAOIDefs();
            Console.WriteLine($"num aoidefs {num}");
            var ids = new int[num];

            int curr_id = 0;
            for (int i = 0; i < num; ++i)
            {
                var tmp = await ctrl.FindNextAOIDefs(curr_id, 1);
                curr_id = tmp[0];
                ids[i] = curr_id;
            }

            for (int i = 0; i < num; ++i)
            {
                var t = new CIPAOIDefStub(ids[i], ctrl.Messager);
                var cfg = await t.GetConfig();
                AddAOIDefinition(FromMsgpack(cfg));
            }
        }

        private static JObject FromMsgpack(List<byte> data)
        {
            for (int i = 0; i < data.Count; ++i)
            {
                data[i] = (byte)(data[i] ^ (byte)(0x5A));
            }

            var obj = (JObject)JsonConvert.DeserializeObject(MessagePackSerializer.ToJson(data.ToArray()));
            return obj;
        }

        private static List<byte> ToMsgpack(JObject obj)
        {
            var res = MessagePackSerializer.FromJson(JsonConvert.SerializeObject(obj)).ToList();
            for (int i = 0; i < res.Count; ++i)
            {
                res[i] = (byte)(res[i] ^ (byte)0x5A);
            }

            return res;
        }

        public async Task UploadModules(CIPController ctrl)
        {
            int num = await ctrl.GetNumModules();
            Console.WriteLine($"num modules {num}");
            var ids = new int[num];

            int curr_id = 0;
            for (int i = 0; i < num; ++i)
            {
                var tmp = await ctrl.FindNextModules(curr_id, 1);
                curr_id = tmp[0];
                ids[i] = curr_id;
            }

            for (int i = 0; i < num; ++i)
            {
                try
                {
                    var t = new CIPModule(ids[i], ctrl.Messager);
                    var cfg = await t.GetConfig();
                    AddDeviceModule(FromMsgpack(cfg));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public async Task UploadPrograms(CIPController ctrl)
        {
            int num = await ctrl.GetNumPrograms();
            Console.WriteLine($"num programs {num}");
            var ids = new int[num];

            int curr_id = 0;
            for (int i = 0; i < num; ++i)
            {
                var tmp = await ctrl.FindNextPrograms(curr_id, 1);
                curr_id = tmp[0];
                ids[i] = curr_id;
            }

            for (int i = 0; i < num; ++i)
            {
                try
                {
                    var t = new CIPProgram(ids[i], ctrl.Messager);
                    var cfg = await t.GetConfig();
                    AddProgram(FromMsgpack(cfg));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public async Task UploadTags(CIPController ctrl)
        {
            int num = await ctrl.GetNumTags();
            Console.WriteLine($"num tags {num}");
            var ids = new int[num];

            int curr_id = 0;
            int index = 0;
            for (int i = 0; i < (num + 63) / 64; ++i)
            {
                var tmp = await ctrl.FindNextTags(curr_id, 64);
                foreach (var id in tmp)
                {
                    ids[index] = id;
                    index += 1;
                    curr_id = id;
                }
            }

            //Debug.Assert(index == num);
            if (index != num)
            {
                Logger.Fatal($"upload tag fatal {index} {num}");
            }

            for (int i = 0; i < num; ++i)
            {
                var t = new CIPTag(ids[i], ctrl.Messager);
                var cfg = await t.GetConfig();
                AddTag(FromMsgpack(cfg));
            }
        }

        public async Task UploadTasks(CIPController ctrl)
        {
            int num = await ctrl.GetNumTasks();
            Console.WriteLine($"num tasks {num}");
            var ids = new int[num];

            int curr_id = 0;
            for (int i = 0; i < num; ++i)
            {
                var tmp = await ctrl.FindNextTasks(curr_id, 1);
                curr_id = tmp[0];
                ids[i] = curr_id;
            }

            for (int i = 0; i < num; ++i)
            {
                try
                {
                    var t = new CIPTask(ids[i], ctrl.Messager);
                    var cfg = await t.GetConfig();
                    AddTask(FromMsgpack(cfg));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public async Task UploadTrends(CIPController ctrl)
        {
            int num = await ctrl.GetNumTrends();
            Console.WriteLine($"num trend {num}");
            var ids = new int[num];

            int curr_id = 0;
            for (int i = 0; i < num; ++i)
            {
                var tmp = await ctrl.FindNextTrends(curr_id, 1);
                curr_id = tmp[0];
                ids[i] = curr_id;
            }

            for (int i = 0; i < num; ++i)
            {
                try
                {
                    var t = new CIPTrendStub(ids[i], ctrl.Messager);
                    var cfg = await t.GetConfig();
                    AddTrend(FromMsgpack(cfg));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        //public async Task DownloadNativeCode(CIPController ctrl)
        //{

        //}

        #endregion

        #region Download

        public async Task Download(ControllerOperationMode modeAfterDownload, bool enableForces, bool applyPortChanges)
        {
            if (CheckCipMessager(_connection) < 0)
                return;

            Stopwatch stopwatch = Stopwatch.StartNew();

            CIPController controller = new CIPController(0, _connection);

            await controller.WriterLockRetry();
            await controller.Clear();

            await controller.SetName(Name);
            if (MajorFaultProgram != null)
            {
                await controller.SetMajorFaultProgram(MajorFaultProgram);
            }

            if (PowerLossProgram != null)
            {
                await controller.SetPowerLossProgram(PowerLossProgram);
            }

            await controller.ResetTranSientData();

            foreach (var tp in _dataTypeCollection.OfType<ModuleDefinedDataType>())
            {
                await controller.TransmitCfg(ToMsgpack(tp.ConvertToJObject()));
                await controller.CreateMDType();
            }

            foreach (var userDefinedDataType in _dataTypeCollection.OfType<UserDefinedDataType>())
            {
                await controller.TransmitCfg(ToMsgpack(userDefinedDataType.ConvertToJObject()));
                await controller.CreateType();
            }

            foreach (var def in _aoiDefinitionCollection)
            {
                await controller.TransmitCfg(ToMsgpack(def.ConvertToJObject()));
                await controller.CreateAOI();
            }

            await controller.FinalizeTypeCreation();


            foreach (var d in DeviceModules)
            {
                var deviceModule = d as DeviceModule.DeviceModule;
                Debug.Assert(deviceModule != null);
                await controller.TransmitCfg(ToMsgpack(deviceModule.ConvertToJObject()));
                await controller.CreateModule();
            }

            foreach (var t in Tags)
            {
                var tag = t as Tag;
                Debug.Assert(tag != null);

                if (tag != null && !tag.IsModuleTag)
                {
                    await controller.TransmitCfg(ToMsgpack(tag.ConvertToJObject()));
                    await controller.CreateTag();
                }
            }

            foreach (var p in Programs)
            {
                var program = p as Program;
                Debug.Assert(program != null);
                await controller.TransmitCfg(ToMsgpack(program.ConvertToJObject(true)));
                await controller.CreateProgram();
            }

            foreach (var t in Tasks)
            {
                var task = t as CTask;
                Debug.Assert(task != null);
                await controller.TransmitCfg(ToMsgpack(task.ConvertToJObject()));
                await controller.CreateTask();
            }

            foreach (var trend in Trends)
            {
                await controller.TransmitCfg(ToMsgpack(trend.ToJson()));
                await controller.CreateTrend();
            }

            await controller.TransmitCfg(ToMsgpack(TimeSetting.ConvertToJObject()));
            await controller.SetTimeSynchronize();

            await controller.TransmitCfg(NativeCode.ToList());
            await controller.SetNativeCode();
            
            await controller.Setup();

            //
            await _transactionManager.SetupAsync(CipMessager);
            //

            await controller.WriterUnLock();

            double elapsedTime = stopwatch.ElapsedMilliseconds;

            Logger.Info($"Download {Name} elapsed time: {elapsedTime} ms");
        }

        private int CheckCipMessager(ICipMessager connection)
        {
            if (connection == null)
                return -1;

            if (connection.ConnectionStatus != ConnectionStatus.ForwardOpen)
                return -2;

            return 0;
        }

        #endregion

        #region Value sync

        public async Task<int> RebuildTagSyncControllerAsync()
        {
            int result = await _tagSyncController.RebuildAsync();

            if (result == 0)
            {
                _tagSyncController.PreOnlineChanged();

                IsOnline = true;
            }

            _tagSyncController.StartGetAllTagsTimer();

            // change

#if DEBUG
            ChangeReceiveTimeout(0);
#else
            ChangeReceiveTimeout(8000);
#endif


            return result;
        }

        private void ChangeReceiveTimeout(int receiveTimeout)
        {
            try
            {
                DeviceConnection deviceConnection = CipMessager as DeviceConnection;
                if (deviceConnection != null)
                {
                    deviceConnection.ReceiveTimeout = receiveTimeout;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public int GetTagId(ITag tag)
        {
            return _tagSyncController.GetTagId(tag);
        }

        public async Task<int> SetTagValueToPLC(ITag tag, string specifier, string value)
        {
            return await _tagSyncController.SetValue(tag, specifier, value);
        }

        public async Task<Tuple<int, string>> GetTagValueFromPLC(ITag tag, string specifier)
        {
            return await _tagSyncController.GetValue(tag, specifier);
        }

#endregion

#region Other

        private void OnDisconnected(object sender, EventArgs e)
        {
            IsOnline = false;
        }

        private void OnConnected(object sender, EventArgs e)
        {
            IsOnline = false;
        }

#endregion

        public async Task<int> SetInhibited(IProgram program, bool inhibited)
        {
            if (!IsOnline)
                return -1;

            int programId = _tagSyncController.GetProgramId(program);
            CIPProgram cipProgram = new CIPProgram(programId, CipMessager);

            await cipProgram.SetInhibit(inhibited);

            return 0;
        }

        public async Task<int> SetInhibited(ITask task, bool inhibited)
        {
            if (!IsOnline)
                return -1;

            int taskId = _tagSyncController.GetTaskId(task);
            CIPTask cipTask = new CIPTask(taskId, CipMessager);

            await cipTask.SetInhibit(inhibited);

            return 0;
        }

        public async Task<int> ResetMaxScanTime(IProgram program)
        {
            if (!IsOnline)
                return -1;

            int programId = _tagSyncController.GetProgramId(program);
            CIPProgram cipProgram = new CIPProgram(programId, CipMessager);
            await cipProgram.ResetMaxScanTime();

            return 0;
        }

        public async Task<int> ResetMaxScanTime(ITask task)
        {
            if (!IsOnline)
                return -1;

            int taskId = _tagSyncController.GetTaskId(task);
            CIPTask cipTask = new CIPTask(taskId, CipMessager);
            await cipTask.Reset();

            return 0;
        }

        public async Task<int> SetPeriod(ITask task, float period)
        {
            if (!IsOnline)
                return -1;

            int taskId = _tagSyncController.GetTaskId(task);
            CIPTask cipTask = new CIPTask(taskId, CipMessager);

            await cipTask.SetPeriod(period);

            return 0;
        }

        public async Task<ICipMessager> GetMessager(IDeviceModule deviceModule)
        {
            return await _moduleConnectionManager.GetMessager(deviceModule);
        }

        public async Task RemoveMessagerByDeviceModule(IDeviceModule deviceModule)
        {
            await _moduleConnectionManager.RemoveMessager(deviceModule);
        }
    }
}
