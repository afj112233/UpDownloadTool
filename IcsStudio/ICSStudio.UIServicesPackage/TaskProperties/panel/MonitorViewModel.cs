using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.TaskProperties.panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    class MonitorViewModel : ViewModelBase, IOptionPanel
    {
        private readonly ITask _task;
        private readonly DispatcherTimer _timer;

        public MonitorViewModel(Monitor monitor, ITask task)
        {
            Control = monitor;
            monitor.DataContext = this;

            _task = task;

            ResetCommand = new RelayCommand(ExecuteResetCommand, CanExecuteResetCommand);

            Controller controller = task.ParentController as Controller;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(250)
            };
            _timer.Tick += OnTick;

            if (IsOnline)
                _timer.Start();

        }

        public bool IsOnline => _task.ParentController.IsOnline;

        private void OnTick(object sender, EventArgs e)
        {
            RaisePropertyChanged("MaxScanTime");
            RaisePropertyChanged("LastScanTime");
            RaisePropertyChanged("MaxIntervalTime");
            RaisePropertyChanged("MinIntervalTime");
            RaisePropertyChanged("OverlapCount");
        }

        public string MaxScanTime => IsOnline ? $"{_task.MaxScanTime / 1000f:0.000}" : string.Empty;

        public string LastScanTime => IsOnline ? $"{_task.LastScanTime / 1000f:0.000}" : string.Empty;

        public string MaxIntervalTime => IsOnline ? $"{_task.MaxIntervalTime / 1000f:0.000}" : string.Empty;

        public string MinIntervalTime => IsOnline ? $"{_task.MinIntervalTime / 1000f:0.000}" : string.Empty;

        public string OverlapCount => IsOnline ? $"{_task.OverlapCount}" : string.Empty;

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                RaisePropertyChanged("IsOnline");
                RaisePropertyChanged("MaxScanTime");
                RaisePropertyChanged("LastScanTime");
                RaisePropertyChanged("MaxIntervalTime");
                RaisePropertyChanged("MinIntervalTime");
                RaisePropertyChanged("OverlapCount");

                ResetCommand.RaiseCanExecuteChanged();

                if (IsOnline)
                    _timer.Start();

                else
                    _timer.Stop();

            });
        }

        public override void Cleanup()
        {
            _timer.Stop();
            base.Cleanup();
        }

        public RelayCommand ResetCommand { get; }

        private void ExecuteResetCommand()
        {
            try
            {
                Controller controller = _task.ParentController as Controller;
                controller?.ResetMaxScanTime(_task).GetAwaiter();
            }
            catch (Exception)
            {
                // ignor
            }
        }

        private bool CanExecuteResetCommand()
        {
            return IsOnline;
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
    }
}
