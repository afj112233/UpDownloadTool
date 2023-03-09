using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Online;
using ICSStudio.UIServicesPackage.View;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;

namespace ICSStudio.UIServicesPackage.PLCProperties.Panel
{
    class DateTimeViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private readonly Timer _timer;
        private string _dateTime;
        private readonly Timer _resetTimer;
        private bool _isDuring;
        private bool _isDirty;
        private IController _controller;
        private bool _ptpEnable;
        private bool _isEnable;

        public DateTimeViewModel(DateTime panel, IController controller)
        {
            Control = panel;
            panel.DataContext = this;
            _controller = controller;
            ChangedDateCommand = new RelayCommand(ExecuteChangedDateCommand, CanExecuteChangedDateCommand);
            ResetCommand = new RelayCommand(ExecuteResetCommand, CanExecuteChangedDateCommand);
            AdvancedCommand = new RelayCommand(ExecuteAdvancedCommand, CanExecuteChangedDateCommand);
            IsEnable = controller.IsOnline;
            _timer = new Timer(500);
            _timer.Elapsed += _timer_Elapsed;

            _resetTimer = new Timer(15000);
            _resetTimer.Elapsed += Timer_Elapsed;

            DateTime = string.Empty;

            if (controller.IsOnline)
            {
                _timer.Start();
            }


            PTPEnable = ((Controller)controller).TimeSetting.PTPEnable;

            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                controller as Controller, "IsOnlineChanged", OnIsOnlineChanged);

            IsDirty = false;
        }

        public override void Cleanup()
        {
            _timer.Stop();
            _resetTimer.Stop();
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                _controller as Controller, "IsOnlineChanged", OnIsOnlineChanged);

            base.Cleanup();
        }

        public bool IsEnableTimeSyncEnabled => !_controller.IsOnline;

        public bool PTPEnable
        {
            set
            {
                _ptpEnable = value;
                IsDirty = true;
            }
            get { return _ptpEnable; }
        }

        public bool IsEnable
        {
            set { Set(ref _isEnable, value); }
            get { return _isEnable; }
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await TaskScheduler.Default;

                try
                {
                    if (_controller.IsOnline)
                    {
                        Controller controller = _controller as Controller;

                        ICipMessager cipMessager = controller?.CipMessager;

                        if (cipMessager != null)
                        {
                            OnlineEditHelper helper = new OnlineEditHelper(cipMessager);

                            long timestamp = await helper.GetTimestamp();

                            System.DateTime dateTime =
                                new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddTicks(timestamp * 10);

                            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                            var localTime = dateTime.ToLocalTime();

                            DateTime = localTime.ToString("yyyy/MM/dd HH:mm:ss K");
                        }

                    }
                    else
                    {
                        DateTime = string.Empty;
                        _timer.Stop();
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }

            });

        }

        public string DateTime
        {
            set { Set(ref _dateTime, value); }
            get { return _dateTime; }
        }

        public RelayCommand ChangedDateCommand { set; get; }

        private void ExecuteChangedDateCommand()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            var dialog = new DateTimeBrowser(System.DateTime.Now);
            if (dialog.ShowDialog(uiShell) ?? false)
            {
                var dateTime = dialog.GetTime();
                if (dateTime != null)
                {
                    DateTime = ((System.DateTime)dateTime).ToString("yyyy/MM/dd hh:mm:ss");
                }
            }
        }

        private bool CanExecuteChangedDateCommand()
        {
            //return false;
            return _controller.IsOnline;
        }

        public RelayCommand ResetCommand { set; get; }

        private void ExecuteResetCommand()
        {
            if (_isDuring)
            {
                MessageBox.Show(
                    "Failed to set controller time.\nThe Controller's time cannot be set twice within 15-second interval.",
                    "ICS Studio", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk);
            }
            else
            {
                _resetTimer.Start();
                _isDuring = true;
            }
        }

        public RelayCommand AdvancedCommand { set; get; }

        private void ExecuteAdvancedCommand()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            var dialog = new AdvancedTimeSync();
            if (dialog.ShowDialog(uiShell) ?? false)
            {

            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _isDuring = false;
            ((Timer)sender).Stop();
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

        public void Save()
        {
            Controller myController = _controller as Controller;
            if (myController != null)
            {
                //TODO(gjc): need edit later
                if (myController.TimeSetting.PTPEnable != PTPEnable)
                {
                    myController.TimeSetting.PTPEnable = PTPEnable;
                    // ReSharper disable once NotResolvedInText
                    myController.RaisePropertyChanged("TimeSetting.PTPEnable");
                }
            }

            IsDirty = false;
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                ResetCommand.RaiseCanExecuteChanged();
                ChangedDateCommand.RaiseCanExecuteChanged();
                AdvancedCommand.RaiseCanExecuteChanged();

                if (_controller.IsOnline)
                {
                    _timer.Start();
                }
                else
                {
                    DateTime = string.Empty;
                    _timer.Stop();
                }
                IsEnable = _controller.IsOnline;
                RaisePropertyChanged(nameof(IsEnableTimeSyncEnabled));
            });
        }
    }
}
