using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;


namespace ICSStudio.UIServicesPackage.ViewModel
{
    internal class AutotuneProgressViewModel : ViewModelBase
    {
        private readonly ICipMessager _messager;
        private readonly IMessageRouterRequest _request;
        private readonly CipMotionInstruction _motionInstruction;

        private readonly DispatcherTimer _timer;
        private string _testLog;

        private string _testState;
        private bool? _dialogResult;
        private bool _progressing = true;

        public AutotuneProgressViewModel(ICipMessager messager, IMessageRouterRequest request)
        {
            _messager = messager;
            _request = request;

            OKCommand = new RelayCommand(ExecuteOKCommand, CanOKCommand);
            StopCommand = new RelayCommand(ExecuteStopCommand, CanStopCommand);

            TestState = "Executing";
            TestLog = "Wait for command to complete after axis motion.\r\nCheck for errors if command fails.";
            _motionInstruction = new CipMotionInstruction();

            // update timer, 100ms
            _timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(100)};
            // ReSharper disable once UnusedAnonymousMethodSignature
            _timer.Tick += async delegate(object sender, EventArgs args) { await CycleProgressHandleAsync(); };
            _timer.Start();

        }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        public string TestState
        {
            get { return _testState; }
            set { Set(ref _testState, value); }
        }

        public string TestLog
        {
            get { return _testLog; }
            set { Set(ref _testLog, value); }
        }

        public override void Cleanup()
        {
            _progressing = false;
            _timer?.Stop();

            base.Cleanup();
        }

        public RelayCommand OKCommand { get; }
        public RelayCommand StopCommand { get; }

        private bool CanStopCommand()
        {
            return _progressing;
        }

        private bool CanOKCommand()
        {
            return !_progressing;
        }

        private void ExecuteStopCommand()
        {
            DialogResult = false;
        }

        private void ExecuteOKCommand()
        {
            DialogResult = true;
        }

        private async Task CycleProgressHandleAsync()
        {
            _timer.Stop();

            var response = await _messager.SendUnitData(_request);
            if ((response != null) && (response.GeneralStatus == (byte) CipGeneralStatusCode.Success))
                if (_motionInstruction.Parse(response.ResponseData) == 0)
                    if (_motionInstruction.DN && _motionInstruction.PC)
                    {
                        // success
                        TestState = "Success";
                        TestLog = "Autotune complete.";
                        _progressing = false;
                    }
                    else if (_motionInstruction.ER)
                    {
                        // TODO(gjc): add error handle
                        TestState = "Error";
                        TestLog = "Error!!!";
                        _progressing = false;
                    }

            OKCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();

            if (_progressing)
                _timer.Start();
        }

    }
}
