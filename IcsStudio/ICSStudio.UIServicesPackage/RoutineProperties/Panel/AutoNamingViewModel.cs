using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.UIServicesPackage.RoutineProperties.Panel
{
    class AutoNamingViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private bool _isDirty;

        public AutoNamingViewModel(AutoNaming panel, IRoutine routine)
        {
            Control = panel;
            panel.DataContext = this;
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
                if (_isDirty != value)
                {
                    _isDirty = value;
                    IsDirtyChanged?.Invoke(this, EventArgs.Empty);
                }

                Set(ref _isDirty, value);
            }
        }

        public event EventHandler IsDirtyChanged;
    }
}
