using System;
using System.Threading.Tasks;
using ICSStudio.Cip.Objects;
using NLog;

namespace ICSStudio.SimpleServices.Utilities
{
    public static class CIPControllerExtensions
    {
        internal static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public static async Task<Int32> EnterReadLock(this CIPController cipController)
        {
            Logger.Debug("EnterReadLock");
            return await cipController.ReaderLock();
        }

        public static async Task<Int32> ExitReadLock(this CIPController cipController)
        {
            Logger.Debug("ExitReadLock");
            return await cipController.ReaderUnLock();
        }
    }
}
