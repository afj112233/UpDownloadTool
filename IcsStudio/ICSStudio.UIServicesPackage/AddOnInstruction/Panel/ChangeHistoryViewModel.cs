using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.AddOnInstruction.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    class ChangeHistoryViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private bool _isDirty;
        private bool _canClearCommand;
        private readonly AoiDefinition _aoiDefinition;

        public ChangeHistoryViewModel(ChangeHistory panel, IAoiDefinition aoiDefinition)
        {
            Control = panel;
            panel.DataContext = this;
            _canClearCommand = true;
            _aoiDefinition = (AoiDefinition) aoiDefinition;
            ClearCommand = new RelayCommand(ExecuteClearCommand, CanClearCommand);
            CreatedDate = _aoiDefinition.CreatedDate.ToString(CultureInfo.CurrentCulture);
            CreatedBy = _aoiDefinition.CreatedBy;
            EditedBy = _aoiDefinition.EditedBy;
            EditedDate = _aoiDefinition.EditedDate.ToString(CultureInfo.CurrentCulture);
            SetControlEnable();
            _aoiDefinition.PropertyChanged += OnPropertyChanged;

            Controller controller = _aoiDefinition.ParentController as Controller;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }
        }

        public override void Cleanup()
        {
            _aoiDefinition.PropertyChanged -= OnPropertyChanged;
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

                SetControlEnable();
            });
        }

        public void SetControlEnable()
        {

            if (_aoiDefinition.ParentController.IsOnline || _aoiDefinition.IsSealed)
            {
                _canClearCommand = false;
            }
            else
            {
                _canClearCommand = true;
            }

            ClearCommand.RaiseCanExecuteChanged();
        }

        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "EditedDate")
            {
                EditedDate = _aoiDefinition.EditedDate.ToString(CultureInfo.CurrentCulture);
                RaisePropertyChanged("EditedDate");
            }

            if (e.PropertyName == "EditedBy")
            {
                EditedBy = _aoiDefinition.EditedBy;
                RaisePropertyChanged("EditedBy");
            }
        }

        public void Save()
        {
            _aoiDefinition.CreatedBy = CreatedBy;
            _aoiDefinition.EditedBy = EditedBy;
            //_aoiDefinition.EditedDate = DateTime.Now;
            IsDirty = false;
        }

        public string CreatedBy { set; get; }
        public string CreatedDate { set; get; }
        public string EditedBy { set; get; }
        public string EditedDate { set; get; }

        public RelayCommand ClearCommand { set; get; }

        public bool CanClearCommand()
        {
            return _canClearCommand;
        }

        public void ExecuteClearCommand()
        {
            CreatedBy = "Not Available";
            EditedBy = "Not Available";
            RaisePropertyChanged("CreatedBy");
            RaisePropertyChanged("EditedBy");
            IsDirty = true;
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
