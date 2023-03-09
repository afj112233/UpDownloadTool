using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using ICSStudio.DeviceProperties;
using ICSStudio.DeviceProperties.Adapters;
using ICSStudio.DeviceProperties.AnalogIOs;
using ICSStudio.DeviceProperties.ServoDrives;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.DeviceProperties.DiscreteIOs;
using ICSStudio.DeviceProperties.Generic;
using ICSStudio.MultiLanguage;
using ICSStudio.UIInterfaces.Dialog;

namespace ICSStudio.EditorPackage.ModuleProperties
{
    public class ModulePropertiesViewModel
        : ViewModelBase, IEditorPane, ICanApply
    {
        private readonly IDevicePropertiesViewModel _viewModel;

        public ModulePropertiesViewModel(IDeviceModule deviceModule)
        {
            DeviceModule = deviceModule;

            Control = new DevicePropertiesControl();

            if (deviceModule is CIPMotionDrive)
            {
                _viewModel =
                    new ServoDrivesViewModel(deviceModule.ParentController, deviceModule);
            }
            else if (deviceModule is CommunicationsAdapter)
            {
                _viewModel = new DIOEnetAdapterViewModel(deviceModule.ParentController, deviceModule);
            }
            else if (deviceModule is DiscreteIO)
            {
                _viewModel = new DIODiscreteViewModel(deviceModule.ParentController, deviceModule);
            }
            else if (deviceModule is AnalogIO)
            {
                _viewModel = new DIOAnalogViewModel(deviceModule.ParentController, deviceModule);
            }
            else if (deviceModule is GeneralEthernet)
            {
                _viewModel = new GenericEnetViewModel(deviceModule.ParentController, deviceModule);
            }
            else
            {
                //TODO(gjc): add code here
                _viewModel = new NullDeviceViewModel();
            }

            Control.DataContext = _viewModel;
            _viewModel.PropertyChanged += ViewModelOnPropertyChanged;

            LanguageManager.GetInstance().SetLanguage(Control);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public IDeviceModule DeviceModule { get; }

        public string Caption
        {
            get
            {
                if (_viewModel != null)
                    return _viewModel.Caption;

                return "Add other Device Properties";
            }
        }

        public UserControl Control { get; }

        public Action CloseAction
        {
            get { return _viewModel?.CloseAction; }
            set
            {
                if (_viewModel != null)
                    _viewModel.CloseAction = value;
            }
        }

        public Action<string> UpdateCaptionAction { get; set; }

        public override void Cleanup()
        {
            var cleanup = _viewModel as ICleanup;
            cleanup?.Cleanup();

            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);

            base.Cleanup();
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Caption") UpdateCaptionAction?.Invoke(Caption);
        }

        public int Apply()
        {
            ICanApply canApply = _viewModel as ICanApply;
            if (canApply != null)
            {
                return canApply.Apply();
            }

            return 0;
        }

        public bool CanApply()
        {
            return true;
        }

        private void LanguageChanged(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(Control);
        }
    }
}