#define Ignore_Version_Check

using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Online;
using ICSStudio.UIInterfaces.Version;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.Services
{
    public class VersionService : IVersionService, SVersionService
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly Package _package;

        public VersionService(Package package)
        {
            _package = package;
        }

        public string GetCoreVersion()
        {
            Version coreVersion = Assembly.GetExecutingAssembly().GetName().Version;
            return coreVersion.ToString();
        }

        public VersionCheckResult CheckControllerVersion(IController controller)
        {

#if Ignore_Version_Check
            return new VersionCheckResult();
#else
            return ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                var result = await CheckControllerVersionAsync(controller);
                return result;
            });
#endif
        }

        // ReSharper disable once UnusedMember.Local
        private async Task<VersionCheckResult> CheckControllerVersionAsync(IController controller)
        {
            VersionCheckResult result = new VersionCheckResult();

            try
            {
                Controller myController = controller as Controller;
                if (myController == null)
                {
                    result.Kind = -1;
                    result.Message = "Controller is NULL!";
                    return result;
                }

                VersionHelper versionHelper = new VersionHelper(myController.CipMessager);

                var info = await versionHelper.GetVersionInfo();
                if (info == null)
                {
                    result.Kind = -2;
                    result.Message = "Get controller version info failed!";
                    return result;
                }

                // get ics version
                Version coreVersion = Assembly.GetExecutingAssembly().GetName().Version;

                if (coreVersion.Minor != info.Version)
                {
                    result.Kind = -3;

                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("The versions don't match.");
                    builder.AppendLine("ics version:");
                    builder.AppendLine($"{coreVersion}");
                    builder.AppendLine("controller version:");
                    builder.AppendLine(info.ToString());

                    result.Message = builder.ToString();
                    return result;
                }

            }
            catch (Exception)
            {
                result.Kind = -100;
                result.Message = "Catch exception!";
                return result;
            }

            return result;
        }

    }
}
