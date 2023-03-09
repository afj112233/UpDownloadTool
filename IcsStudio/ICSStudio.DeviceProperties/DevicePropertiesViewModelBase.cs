using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Descriptor;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public abstract class DevicePropertiesViewModelBase
        : ViewModelBase, IDevicePropertiesViewModel
    {
        private readonly ModuleDescriptor _descriptor;

        private bool? _dialogResult;
        private string _optionPanelTitle;
        private object _optionPanelContent;

        protected DevicePropertiesViewModelBase(
            IController controller,
            IDeviceModule deviceModule,
            bool isCreating)
        {
            Contract.Assert(controller != null);
            Contract.Assert(deviceModule != null);

            Controller = controller;

            Caption = "Add Caption";

            IsCreating = isCreating;

            OptionPanelNodes = new List<IOptionPanelNode>();

            DeviceModule myDeviceModule = deviceModule as DeviceModule;
            Contract.Assert(myDeviceModule != null);

            _descriptor = new ModuleDescriptor(myDeviceModule);

            PropertyChangedEventManager.AddHandler(myDeviceModule,
                OnModulePropertyChanged, string.Empty);

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", OnLanguageChanged);
        }

        public IController Controller { get; }

        public virtual string Caption { get; }

        public Action CloseAction { get; set; }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        public List<IOptionPanelNode> OptionPanelNodes { get; }

        public string OptionPanelTitle
        {
            get
            {
                var optionPanelTitle = LanguageManager.GetInstance().ConvertSpecifier(_optionPanelTitle);
                if (!string.IsNullOrEmpty(optionPanelTitle)) return optionPanelTitle;
                return _optionPanelTitle;
            }
            set { Set(ref _optionPanelTitle, value); }
        }

        public object OptionPanelContent
        {
            get { return _optionPanelContent; }
            set { Set(ref _optionPanelContent, value); }
        }

        public string Status
        {
            get
            {
                if (IsCreating)
                {
                    return "Creating";
                }

                if (!Controller.IsOnline)
                {
                    return "Offline";
                }

                return _descriptor.Status;
            }
        }

        public bool IsCreating { get; }

        public Visibility ApplyCommandVisibility
        {
            get
            {
                if (IsCreating)
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        public RelayCommand OkCommand { get; protected set; }
        public RelayCommand CancelCommand { get; protected set; }
        public RelayCommand ApplyCommand { get; protected set; }
        public RelayCommand HelpCommand { get; protected set; }

        public abstract int Apply();
        public virtual bool CanApply()
        {
            return true;
        }

        public override void Cleanup()
        {
            base.Cleanup();

            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
                LanguageManager.GetInstance(), "LanguageChanged", OnLanguageChanged);
        }

        private void OnModulePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "EntryStatus")
            {
                RaisePropertyChanged("Status");
            }
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(OptionPanelTitle));
        }
    }
}
