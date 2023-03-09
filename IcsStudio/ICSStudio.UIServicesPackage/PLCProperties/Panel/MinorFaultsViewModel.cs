using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.PLCProperties.Panel
{
    class MinorFaultsViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private bool _isDirty;
        private IController _controller;
        private string _status;

        public MinorFaultsViewModel(MinorFaults panel,IController controller)
        {
            Control = panel;
            panel.DataContext = this;
            _controller = controller;
            Status = controller.IsOnline ? "No major faults since last cleared." : "Offline.";
            ClearCommand=new RelayCommand(ExecuteClearCommand,CanExecuteClearCommand);
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                controller as Controller, "IsOnlineChanged", OnIsOnlineChanged);

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(Status));
        }

        public override void Cleanup()
        {
            base.Cleanup();

            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public RelayCommand ClearCommand { get; }

        private void ExecuteClearCommand()
        {

        }

        private bool CanExecuteClearCommand()
        {
            return false;
        }

        public string Status
        {
            set { Set(ref _status , value); }
            get { return LanguageManager.GetInstance().ConvertSpecifier(_status); }
        }

        public string Faults { set; get; }
        public bool CheckBoxEnable { set; get; }
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

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                Status = _controller.IsOnline ? "No major faults since last cleared." : "Offline.";
                CheckBoxEnable = !_controller.IsOnline;
            });
        }
    }
}
