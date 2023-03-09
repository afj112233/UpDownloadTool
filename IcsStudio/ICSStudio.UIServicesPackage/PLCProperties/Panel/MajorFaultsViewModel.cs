using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Online;
using ICSStudio.UIInterfaces.Command;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;

namespace ICSStudio.UIServicesPackage.PLCProperties.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    class MajorFaultsViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private bool _isDirty;
        private readonly IController _controller;

        private string _status;
        private string _faults;

        private readonly Timer _getMajorFaultTimer;
        private MajorFaultHelper _majorFaultHelper;
        private readonly ConcurrentQueue<MajorFaultInfo> _majorFaultInfos;

        public MajorFaultsViewModel(MajorFaults panel, IController controller)
        {
            Control = panel;
            panel.DataContext = this;

            _controller = controller;

            _status = LanguageManager.GetInstance().ConvertSpecifier("No major faults since last cleared.");
            _faults = string.Empty;

            ClearCommand = new RelayCommand(ExecuteClearCommand, CanExecuteClearCommand);

            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                controller as Controller, "IsOnlineChanged", OnIsOnlineChanged);
            WeakEventManager<Controller, EventArgs>.AddHandler(
                controller as Controller, "OperationModeChanged", OnOperationModeChanged);

            _majorFaultInfos = new ConcurrentQueue<MajorFaultInfo>();

            _getMajorFaultTimer = new Timer(500);
            _getMajorFaultTimer.Elapsed += GetMajorFaultHandle;
            _getMajorFaultTimer.AutoReset = false;

            if (_controller.IsOnline)
            {
                Controller myController = _controller as Controller;

                _majorFaultHelper = new MajorFaultHelper(myController?.CipMessager);

                StartGetMajorFaultTimer();
            }

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(),
                "LanguageChanged", LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(Status));
        }


        public override void Cleanup()
        {
            _getMajorFaultTimer.Stop();
            _getMajorFaultTimer.Elapsed -= GetMajorFaultHandle;

            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);

            base.Cleanup();
        }

        public RelayCommand ClearCommand { get; }

        private void ExecuteClearCommand()
        {
            var commandService =
                Package.GetGlobalService(typeof(SCommandService)) as ICommandService;

            commandService?.ClearFaults(_controller);
        }

        private bool CanExecuteClearCommand()
        {
            if (_controller != null && _controller.IsOnline)
            {
                if (_controller.OperationMode == ControllerOperationMode.OperationModeFaulted)
                    return true;
            }

            return false;
        }

        public string Status
        {
            get
            {
                if (_controller == null)
                    return string.Empty;

                if (!_controller.IsOnline)
                    return LanguageManager.GetInstance().ConvertSpecifier("Offline.");

                return _status;
            }
        }



        public string Faults
        {
            get
            {
                if (_controller == null)
                    return string.Empty;

                if (!_controller.IsOnline)
                    return string.Empty;

                return _faults;
            }
        }

        public object Owner { get; set; }
        public object Control { get; }

        public void LoadOptions()
        {

        }

        public bool SaveOptions()
        {
            return true;
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                _isDirty = value;
                IsDirtyChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler IsDirtyChanged;

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (_controller.IsOnline)
                {
                    Controller myController = _controller as Controller;

                    _majorFaultHelper = new MajorFaultHelper(myController?.CipMessager);

                    StartGetMajorFaultTimer();
                }
                else
                {
                    while (!_majorFaultInfos.IsEmpty)
                    {
                        MajorFaultInfo info = null;
                        _majorFaultInfos.TryDequeue(out info);
                    }
                }

                Refresh();
            });
        }

        [SuppressMessage("ReSharper", "NotAccessedVariable")]
        private void OnOperationModeChanged(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (_controller != null && _controller.IsOnline)
                {
                    if (_controller.OperationMode != ControllerOperationMode.OperationModeFaulted)
                    {
                        await _majorFaultHelper.Reset();

                        while (!_majorFaultInfos.IsEmpty)
                        {
                            MajorFaultInfo info;
                            _majorFaultInfos.TryDequeue(out info);
                        }

                        _status = LanguageManager.GetInstance().ConvertSpecifier("No major faults since last cleared.");
                        _faults = string.Empty;
                    }
                }

                Refresh();
            });
        }

        private void Refresh()
        {
            ClearCommand.RaiseCanExecuteChanged();

            RaisePropertyChanged("Status");
            RaisePropertyChanged("Faults");
        }

        private void StartGetMajorFaultTimer()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                try
                {
                    await TaskScheduler.Default;

                    List<MajorFaultInfo> infos = new List<MajorFaultInfo>();
                    int result = await _majorFaultHelper.GetAllMajorFaultInfos(infos);


                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    if (result == 0)
                        AddMajorFaultInfos(infos);

                    Refresh();

                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
                finally
                {
                    if (_controller.IsOnline)
                    {
                        _getMajorFaultTimer.Start();
                    }
                }

            });
        }

        private void GetMajorFaultHandle(object sender, ElapsedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                try
                {
                    await TaskScheduler.Default;

                    List<MajorFaultInfo> infos = new List<MajorFaultInfo>();
                    int result = await _majorFaultHelper.GetNextMajorFaultInfos(infos);

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    if (result == 0)
                        AddMajorFaultInfos(infos);

                    Refresh();

                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
                finally
                {
                    if (_controller.IsOnline)
                    {
                        _getMajorFaultTimer.Start();
                    }
                }

            });
        }

        private void AddMajorFaultInfos(List<MajorFaultInfo> infos)
        {
            if (infos != null && infos.Count > 0)
            {
                foreach (var info in infos)
                {
                    _majorFaultInfos.Enqueue(info);
                }

                var faultInfos = _majorFaultInfos.ToArray();

                _status = $"{faultInfos.Length} major fault since last cleared.";

                StringBuilder builder = new StringBuilder();

                foreach (var info in faultInfos)
                {
                    builder.AppendLine(info.ToString());
                }

                _faults = builder.ToString();
            }
        }
    }
}
