using System;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.EditLogs
{
    internal interface IEditLog
    {
        DateTime EditTime { get; }
        bool CanOnlineUpdate { get; }

        int DoOnlineUpdate();

        JObject ConvertToJObject();
    }
}
