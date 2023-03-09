using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    [SuppressMessage("ReSharper", "ArrangeAccessorOwnerBody")]
    public class ConnectingViewModel : ViewModelBase
    {
        private readonly Controller _controller;
        private readonly string _commPath;

        private bool? _dialogResult;

        public ConnectingViewModel(Controller controller, string commPath)
        {
            _controller = controller;
            _commPath = commPath;

            CancelCommand = new RelayCommand(ExecuteCancelCommand);
        }

        public bool IsUserCancel { get; private set; }

        private void ExecuteCancelCommand()
        {
            IsUserCancel = true;
            _controller.GoOffline();
        }

        public bool? DialogResult
        {
            set { Set(ref _dialogResult, value); }
            get { return _dialogResult; }
        }

        public RelayCommand CancelCommand { get; }

        public void Connect()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await TaskScheduler.Default;

                int result = int.MinValue;
                try
                {
                    result = await _controller.ConnectAsync(_commPath);
                }
                catch (Exception)
                {
                    //ignore
                }

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                DialogResult = result == 0;

            });
        }
    }
}
