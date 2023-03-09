using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.UIServicesPackage.View;
using ICSStudio.UIServicesPackage.ViewModel;
using Microsoft.VisualStudio.Shell.Interop;

namespace ICSStudio.UIServicesPackage.PLCProperties.Panel
{
    class SecurityViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private string _configure;
        private bool _isDirty;

        public SecurityViewModel(Security panel, IController controller)
        {
            Control = panel;
            panel.DataContext = this;
            Command = new RelayCommand(ExecuteCommand);
        }

        public string Configure
        {
            set { Set(ref _configure, value); }
            get { return _configure; }
        }

        public string AuditValue { set; get; }

        public RelayCommand Command { set; get; }

        public void ExecuteCommand()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            var config = FormatOp.RemoveFormat(Configure);
            try
            {
                Convert.ToInt64(config, 16);
                config = Configure;
            }
            catch (Exception)
            {
                config = "";
            }

            var uiShell = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            var dialog = new ConfigureDetect();
            var vm = new ConfigureDetectViewModel(config);
            dialog.DataContext = vm;
            if (dialog.ShowDialog(uiShell) ?? false)
            {
                Configure = vm.GetConfigure();
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
    }
}
