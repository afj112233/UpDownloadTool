using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace ICSStudio.Dialogs.WaitDialog
{
    class WaitViewModel : ViewModelBase
    {
        private bool? _dialogResult;
        private readonly Action _action;
        private readonly bool _isCloseAfter;
        public WaitViewModel(Action action,bool isCloseAfter)
        {
            _isCloseAfter = isCloseAfter;
            _action = action;
        }
        
        public bool? DialogResult
        {
            set { Set(ref _dialogResult, value); }
            get { return _dialogResult; }
        }

        public void GoOnAction()
        {
            if (_action == null) return;
            try
            {
                _action();
            }
            catch (Exception e)
            {
                Controller.GetInstance().Log($"WaitViewModel.GoOnAction(){e.StackTrace}");
            }
            finally
            {
                if (_isCloseAfter)
                    DialogResult = true;
            }
        }
        
    }
}
