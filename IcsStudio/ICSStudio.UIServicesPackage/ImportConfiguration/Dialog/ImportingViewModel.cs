using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using GalaSoft.MvvmLight;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.ImportConfiguration.Dialog
{
    public class ImportingViewModel:ViewModelBase
    {
        private BackgroundWorker _worker = new BackgroundWorker();
        private double _progress;
        private bool? _dialogResult;
        private double _maximum = 1.0;
        private double _minimum;
        private string _importingTitle;

        public ImportingViewModel()
        {
            _worker.WorkerReportsProgress = true;
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += _worker_DoWork;
            _worker.ProgressChanged += _worker_ProgressChanged;
        }

        public double Maximum
        {
            get { return _maximum; }
            set { Set(ref _maximum, value); }
        }

        public double Minimum
        {
            get { return _minimum; }
            set { Set(ref _minimum,value); }
        }

        public Action WorkAction { get; set; }
        public double Progress
        {
            get { return _progress; }
            set { Set(ref _progress, value); }
        }

        public BackgroundWorker Worker => _worker;

        public string ImportingTitle
        {
            get { return _importingTitle; }
            set { Set(ref _importingTitle,value); }
        }

        public bool? DialogResult
        {
            set { Set(ref _dialogResult , value); }
            get { return _dialogResult; }
        }
        
        private void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                Progress += 1;
            });
        }

        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    ImportingTitle = "Starting . . .";
                });
                WorkAction?.Invoke();
                ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    DialogResult = true;
                });
            }
            catch (Exception exception)
            {
                Debug.Assert(false,exception.StackTrace);
                ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    DialogResult = false;
                });
                return;
            }
        }

        public void Start()
        {
            if(!_worker.IsBusy)
                _worker.RunWorkerAsync();
        }

        public void Stop()
        {
            MessageBox.Show("暂未实现！");
        }

        public void Cancel()
        {
            if(_worker.IsBusy)
                _worker.CancelAsync();
        }
    }
}
