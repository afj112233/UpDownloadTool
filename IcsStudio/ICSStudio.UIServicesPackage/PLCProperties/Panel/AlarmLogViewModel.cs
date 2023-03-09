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
    class AlarmLogViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private bool _isDirty;
        private readonly IController _controller;

        public AlarmLogViewModel(AlarmLog panel,IController controller)
        {
            Control = panel;
            _controller = controller;
            panel.DataContext = this;
        }

        public bool IsOnline {
            get
            {
                if (!_controller.IsOnline)
                    return false;
                if (_controller.KeySwitchPosition == ControllerKeySwitch.RunKeySwitch)
                    return false;
                if (_controller.OperationMode == ControllerOperationMode.OperationModeRun)
                    return false;
                return _controller.IsOnline;
            }
        }

        public Visibility IsVisibly {
            get
            {
                if (!_controller.IsOnline)
                    return Visibility.Hidden;
                if (_controller.KeySwitchPosition == ControllerKeySwitch.RunKeySwitch)
                    return Visibility.Hidden;
                if (_controller.OperationMode == ControllerOperationMode.OperationModeRun)
                    return Visibility.Hidden;
                return Visibility.Visible;
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

        public void Refresh()
        {
            RaisePropertyChanged(nameof(IsOnline));
            RaisePropertyChanged(nameof(IsVisibly));
        }
    }
}
