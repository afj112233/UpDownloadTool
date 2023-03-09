using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using ICSStudio.Cip.Objects;
using ICSStudio.SimpleServices;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Utilities;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;

namespace ICSStudio.UIServicesPackage.ViewModel
{

    public class ComparingViewModel : ViewModelBase
    {
        //private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly Controller _controller;
        private bool? _dialogResult;

        public ComparingViewModel(Controller controller)
        {
            _controller = controller;

            //_correlateResult = CorrelateResult.Unknown;
        }

        public bool? DialogResult
        {
            set { Set(ref _dialogResult, value); }
            get { return _dialogResult; }
        }

        //private CorrelateResult _correlateResult;
        //public CorrelateResult CorrelateResult => _correlateResult;

        //public void Correlate()
        //{
        //    ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
        //    {
        //        await TaskScheduler.Default;

        //        int result = 0;
        //        _correlateResult = CorrelateResult.Unknown;

        //        try
        //        {
        //            CIPController cipController = new CIPController(0, _controller.CipMessager);

        //            var res = await cipController.GetHasProject();
        //            if (res < 1)
        //            {
        //                result = (int)CorrelateResult.NoProject;
        //                _correlateResult = CorrelateResult.NoProject;

        //                Logger.Error($"Correlate failed: no project!");
        //            }

        //            if (result == 0)
        //            {
        //                var transactionManager =
        //                    _controller.Lookup(typeof(SimpleServices.TransactionManager)) as
        //                        SimpleServices.TransactionManager;

        //                if (transactionManager == null)
        //                {
        //                    result = (int)CorrelateResult.NoSupported;
        //                    _correlateResult = CorrelateResult.NoSupported;
        //                }
        //                else
        //                {
        //                    result = await cipController.WriterLockRetry();

        //                    if (result == 0)
        //                    {
        //                        result = await transactionManager.Correlate(_controller.CipMessager);
        //                        await cipController.WriterUnLock();

        //                        if (result != 0)
        //                        {
        //                            _correlateResult = CorrelateResult.FailedToCorrelate;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        _correlateResult = CorrelateResult.FailedToGetLock;
        //                        Logger.Error($"Correlate failed: get writer lock failed!");
        //                    }

        //                }
        //            }

        //            if (result == 0)
        //            {
        //                //TODO(gjc): add code here
        //            }

        //        }
        //        catch (Exception)
        //        {
        //            //ignore
        //            result = (int)CorrelateResult.ExceptionToCorrelate;
        //            _correlateResult = CorrelateResult.ExceptionToCorrelate;
        //        }

        //        if (result == 0)
        //        {
        //            _correlateResult = CorrelateResult.Success;
        //        }
        //        else
        //        {
        //            Logger.Error($"Correlate failed: {_correlateResult}");
        //        }

        //        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        //        DialogResult = result == 0;

        //    });
        //}

        private CompareResult _compareResult;
        public CompareResult CompareResult => _compareResult;

        public void Compare()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await TaskScheduler.Default;

                int result;
                _compareResult = CompareResult.Unknown;

                //
                try
                {
                    var transactionManager =
                        _controller.Lookup(typeof(TransactionManager)) as TransactionManager;

                    if (transactionManager == null)
                    {
                        result = (int)CompareResult.NotSupported;
                        _compareResult = CompareResult.NotSupported;
                    }
                    else
                    {
                        var cipController = new CIPController(0, _controller.CipMessager);

                        result = await cipController.EnterReadLock();

                        if (result == 0)
                        {
                            _compareResult = await transactionManager.Compare(_controller.CipMessager);
                            result = (int)_compareResult;

                            await cipController.ExitReadLock();
                        }
                        else
                        {
                            _compareResult = CompareResult.FailedToGetLock;
                        }
                        
                    }
                }
                catch (Exception)
                {
                    //ignore
                    result = (int)CompareResult.Exception;
                    _compareResult = CompareResult.Exception;
                }

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                DialogResult = result == 0;

            });

        }
    }
}
