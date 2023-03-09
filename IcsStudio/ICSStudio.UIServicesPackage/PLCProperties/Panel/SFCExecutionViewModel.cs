using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.PLCProperties.Panel
{
    class SFCExecutionViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private bool _isDirty;
        private IController _controller;
        private bool _enable;

        public SFCExecutionViewModel(SFCExecution panel,IController controller)
        {
            Control = panel;
            panel.DataContext = this;
            _controller = controller;
            Enable = !controller.IsOnline;
        }

        public void Refresh()
        {
            Enable = !_controller.IsOnline;
        }


        public bool Enable
        {
            set { Set(ref _enable , value); }
            get { return _enable; }
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
                IsDirtyChanged?.Invoke(this,EventArgs.Empty);
            }
        }

        public event EventHandler IsDirtyChanged;
    }
}
