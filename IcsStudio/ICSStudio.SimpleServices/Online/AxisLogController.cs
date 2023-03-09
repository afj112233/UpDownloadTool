using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using ICSStudio.Cip.DataTypes;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using NLog;

namespace ICSStudio.SimpleServices.Online
{
    public class AxisLogController
    {
        internal static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<ITag, ConcurrentQueue<AxisExceptionLogItem>> _axisLogs;

        public AxisLogController(Controller controller)
        {
            _axisLogs = new ConcurrentDictionary<ITag, ConcurrentQueue<AxisExceptionLogItem>>();

            Controller = controller;

            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                Controller, "IsOnlineChanged", OnIsOnlineChanged);
        }

        ~AxisLogController()
        {
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                Controller, "IsOnlineChanged", OnIsOnlineChanged);
        }

        public Controller Controller { get; }

        public async Task GetLogsAsync(ITag axisTag)
        {
            if (Controller != null && Controller.IsOnline)
            {
                ConcurrentQueue<AxisExceptionLogItem> queue
                    = _axisLogs.GetOrAdd(axisTag, new ConcurrentQueue<AxisExceptionLogItem>());

                if (queue == null)
                {
                    _axisLogs.TryRemove(axisTag, out queue);
                    return;
                }

                int instanceId = Controller.GetTagId(axisTag);
                ICipMessager messager = Controller.CipMessager;

                List<AxisExceptionLogItem> logs = new List<AxisExceptionLogItem>();

                if (messager != null)
                {
                    // request
                    var request = new MessageRouterRequest
                    {
                        Service = (byte)CipAxisServiceCode.GetLogs,
                        RequestPath = new PaddedEPath((ushort)CipObjectClassCode.Axis, instanceId),
                        RequestData = null
                    };

                    // send
                    var response = await messager.SendUnitData(request);
                    if ((response != null) && (response.GeneralStatus == (byte)CipGeneralStatusCode.Success))
                    {
                        var responseData = response.ResponseData;

                        byte count = responseData[0];
                        int offset = 1;

                        for (int i = 0; i < count; i++)
                        {
                            AxisExceptionLogItem item = new AxisExceptionLogItem
                            {
                                Type = BitConverter.ToInt16(responseData, offset),
                                Code = responseData[offset + 2],
                                SubCode = BitConverter.ToInt16(responseData, offset + 3),
                                StopAction = responseData[offset + 5],
                                Timestamp = BitConverter.ToInt64(responseData, offset + 6),
                                StateChange = responseData[offset + 14],
                            };

                            logs.Add(item);

                            offset += 15;
                        }
                    }
                }

                if (logs.Count > 0)
                {
                    foreach (var item in logs)
                    {
                        queue.Enqueue(item);
                    }
                }

            }
            
        }

        public async Task ClearLogAsync(ITag axisTag)
        {
            if (Controller != null && Controller.IsOnline)
            {
                ICipMessager messager = Controller.CipMessager;
                int instanceId = Controller.GetTagId(axisTag);

                // request
                var request = new MessageRouterRequest
                {
                    Service = (byte)CipAxisServiceCode.ClearLogs,
                    RequestPath = new PaddedEPath((ushort)CipObjectClassCode.Axis, instanceId),
                    RequestData = null
                };

                // response
                var response = await messager.SendUnitData(request);
                if (response != null && (response.GeneralStatus == (byte)CipGeneralStatusCode.Success))
                {
                }

                ConcurrentQueue<AxisExceptionLogItem> queue;
                _axisLogs.TryRemove(axisTag, out queue);
            }

        }

        public AxisExceptionLogItem[] GetSnapshot(ITag axisTag)
        {
            if (Controller != null && Controller.IsOnline)
            {
                ConcurrentQueue<AxisExceptionLogItem> queue
                    = _axisLogs.GetOrAdd(axisTag, new ConcurrentQueue<AxisExceptionLogItem>());

                return queue?.ToArray();
            }

            return null;
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            Task.Run(() =>
            {
                try
                {
                    _axisLogs.Clear();
                }
                catch (Exception exception)
                {
                    Logger.Error(exception);
                }
            });
        }
    }
}
