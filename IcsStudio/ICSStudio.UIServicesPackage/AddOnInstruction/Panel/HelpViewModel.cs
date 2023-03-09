using System;
using System.Diagnostics.Contracts;
using System.Windows;
using GalaSoft.MvvmLight;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.AddOnInstruction.panel
{
    class HelpViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private bool _isAllEnabled = true;
        private bool _isDirty;
        private string _description;
        private readonly AoiDefinition _aoiDefinition;

        public HelpViewModel(Help panel, IAoiDefinition aoiDefinition)
        {

            Control = panel;
            panel.DataContext = this;
            _aoiDefinition = (AoiDefinition) aoiDefinition;
            Contract.Assert(_aoiDefinition != null);

            Description = _aoiDefinition.ExtendedDescription;
            IsAllEnabled = !_aoiDefinition.IsSealed;
            IsDirty = false;

            Controller controller = _aoiDefinition.ParentController as Controller;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }
        }

        public override void Cleanup()
        {
            Controller controller = _aoiDefinition.ParentController as Controller;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                IsAllEnabled = !_aoiDefinition.IsSealed;
            });
        }

        public string Description
        {
            set
            {
                Set(ref _description, value);
                IsDirty = true;
            }
            get { return _description; }
        }

        public void Save()
        {
            _aoiDefinition.ExtendedDescription = Description;
            IsDirty = false;
        }

        public bool IsAllEnabled
        {
            set { Set(ref _isAllEnabled, value); }
            get { return _isAllEnabled; }
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
            set
            {
                Set(ref _isDirty, value);
                IsDirtyChanged?.Invoke(this, new EventArgs());
            }
            get { return _isDirty; }
        }

        public event EventHandler IsDirtyChanged;
    }
}
