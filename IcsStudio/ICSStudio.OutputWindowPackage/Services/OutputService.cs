using System.Threading;
using ICSStudio.UIInterfaces.OutputWindow;
using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IAsyncServiceProvider = Microsoft.VisualStudio.Shell.IAsyncServiceProvider;
using NLog;

namespace ICSStudio.OutputWindowPackage.Services
{
    public class OutputService : SOutputService, IOutputService
    {
        public const string OutputPaneGuidString = "31040D96-B27F-48EE-A44F-4A829A6E948F";
        public const string OutputPaneTitle = "ICSStudio Log";

        internal static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        // ReSharper disable once NotAccessedField.Local
        private readonly IAsyncServiceProvider _asyncServiceProvider;

        private IVsOutputWindowPane _outputWindowPane;

        public OutputService(IAsyncServiceProvider provider)
        {
            _asyncServiceProvider = provider;
        }

        public async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken)
        {
            //await TaskScheduler.Default;
            // do background operations that involve IO or other async methods

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            // query Visual Studio services on main thread unless they are documented as free threaded explicitly.
            // The reason for this is the final cast to service interface (such as IVsShell) may involve COM operations to add/release references.

            //IVsShell vsShell = this._asyncServiceProvider.GetServiceAsync(typeof(SVsShell)) as IVsShell;
            // use Visual Studio services to continue initialization


            IVsUIShell uiShell = await _asyncServiceProvider.GetServiceAsync(typeof(SVsUIShell)) as IVsUIShell;
            if (uiShell != null)
            {
                Guid outputWindowGuid = new Guid("{34e76e81-ee4a-11d0-ae2e-00a0c90fffc3}");
                IVsWindowFrame outputWindowFrame;
                ErrorHandler.ThrowOnFailure(uiShell.FindToolWindow((uint) __VSCREATETOOLWIN.CTW_fForceCreate,
                    ref outputWindowGuid, out outputWindowFrame));

                outputWindowFrame?.Show();
            }

            Guid guid = new Guid(OutputPaneGuidString);

            IVsOutputWindow outWindow =
                await _asyncServiceProvider.GetServiceAsync(typeof(SVsOutputWindow)) as IVsOutputWindow;

            if (outWindow != null)
            {
                ErrorHandler.ThrowOnFailure(outWindow.CreatePane(ref guid, OutputPaneTitle, 1, 0));

                ErrorHandler.ThrowOnFailure(outWindow.GetPane(ref guid, out _outputWindowPane));
            }
        }

        public void OutputString(MessageType messageType, string message)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (_outputWindowPane != null)
                {
                    ErrorHandler.ThrowOnFailure(_outputWindowPane.Activate());
                    ErrorHandler.ThrowOnFailure(_outputWindowPane.OutputStringThreadSafe(message + "\r\n"));
                }
            });
        }
    }
}
