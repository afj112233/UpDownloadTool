using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;

namespace ICSStudio.UIServicesPackage.RSTrendXProperties.Panel
{
    class StopTriggerViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private bool _isDirty;

        public StopTriggerViewModel(StopTrigger panel)
        {
            panel.DataContext = this;
            Control = panel;
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
