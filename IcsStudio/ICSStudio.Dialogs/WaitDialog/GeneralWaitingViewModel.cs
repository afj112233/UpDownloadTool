using System;
using System.Diagnostics;
using System.Threading;
using GalaSoft.MvvmLight;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.Dialogs.WaitDialog
{
    public class GeneralWaitingViewModel : ObservableObject
    {
        private string _waitingTip = "please wait...";
        private double _maximum = 100;
        private double _minimum;
        private double _progress;
        private bool _isIndeterminate;
        private bool? _dialogResult;

        public GeneralWaitingViewModel(bool isIndeterminate)
        {
            _isIndeterminate = isIndeterminate;
        }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        public bool IsIndeterminate
        {
            get { return _isIndeterminate; }
            set { Set(ref _isIndeterminate, value); }
        }

        public string WaitingTip
        {
            get { return _waitingTip; }
            set { Set(ref _waitingTip, value); }
        }

        public double Maximum
        {
            get { return _maximum; }
            set { Set(ref _maximum, value); }
        }

        public double Minimum
        {
            get { return _minimum; }
            set { Set(ref _minimum, value); }
        }

        public double Progress
        {
            get { return _progress; }
            set { Set(ref _progress, value); }
        }

        public Action WorkAction { get; set; }

        public void ReportProgress(double progress = 1)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                Progress += progress;
            });
        }

        public void Start()
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    WorkAction?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
                finally
                {
                    Close();
                }
            });
        }

        public void Close()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                DialogResult = true;
            });
        }

        public void UpdateWaitingTip(string tip)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                WaitingTip = tip;
            });
        }

        public void ResetProgress(double maximum, double minimum = 0, double start = 0)
        {
            Minimum = minimum;
            Maximum = maximum;
            Progress = start;
        }
    }
}