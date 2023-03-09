using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.UIInterfaces.Dialog;

namespace ICSStudio.DeviceProperties
{
    public interface IDevicePropertiesViewModel : INotifyPropertyChanged, ICanApply
    {
        string Caption { get; }
        Action CloseAction { get; set; }
        bool? DialogResult { get; set; }

        List<IOptionPanelNode> OptionPanelNodes { get; }
        string OptionPanelTitle { get; }
        object OptionPanelContent { get; }

        string Status { get; }
        bool IsCreating { get; }
        Visibility ApplyCommandVisibility { get; }

        RelayCommand OkCommand { get; }
        RelayCommand CancelCommand { get; }
        RelayCommand ApplyCommand { get; }
        RelayCommand HelpCommand { get; }
    }
}
