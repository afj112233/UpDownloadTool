using System;
using System.Collections.Generic;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.DeviceProperties.Common;

namespace ICSStudio.DeviceProperties
{
    public class NullDeviceViewModel : ViewModelBase, IDevicePropertiesViewModel
    {
        public NullDeviceViewModel()
        {
            Caption = "TODO: add new device";
            DialogResult = true;

            OptionPanelNodes = null;
            OptionPanelTitle = string.Empty;
            OptionPanelContent = null;

            ApplyCommandVisibility = Visibility.Visible;
            IsCreating = false;
            Status = "Offline";

            OkCommand = new RelayCommand(ExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            ApplyCommand = new RelayCommand(ExecuteApplyCommand, CanExecuteApplyCommand);
            HelpCommand = new RelayCommand(ExecuteHelpCommand);
        }

        private void ExecuteHelpCommand()
        {
        }

        private bool CanExecuteApplyCommand()
        {
            return false;
        }

        private void ExecuteApplyCommand()
        {
        }

        private void ExecuteCancelCommand()
        {
            CloseAction?.Invoke();
            DialogResult = false;
        }

        private void ExecuteOkCommand()
        {
            CloseAction?.Invoke();
            DialogResult = true;
        }

        public string Caption { get; }
        public Action CloseAction { get; set; }
        public bool? DialogResult { get; set; }

        public List<IOptionPanelNode> OptionPanelNodes { get; }
        public string OptionPanelTitle { get; }
        public object OptionPanelContent { get; }
        public string Status { get; }
        public bool IsCreating { get; }
        public Visibility ApplyCommandVisibility { get; }
        public RelayCommand OkCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand ApplyCommand { get; }
        public RelayCommand HelpCommand { get; }

        public int Apply()
        {
            return 0;
        }

        public bool CanApply()
        {
            return false;
        }
    }
}
