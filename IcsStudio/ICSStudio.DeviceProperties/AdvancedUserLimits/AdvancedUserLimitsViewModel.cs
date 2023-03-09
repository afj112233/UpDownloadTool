using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.MultiLanguage;

namespace ICSStudio.DeviceProperties.AdvancedUserLimits
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class AdvancedUserLimitsViewModel : ViewModelBase
    {
        private bool? _dialogResult;
        private float _converterThermalOverloadUserLimit;
        private float _busRegulatorThermalOverloadUserLimit;
        private float _busUndervoltageUserLimit;

        public AdvancedUserLimitsViewModel(AdvancedUserLimits advancedUserLimits)
        {
            InputLimits = advancedUserLimits;

            _converterThermalOverloadUserLimit = InputLimits.ConverterThermalOverloadUserLimit;
            _busRegulatorThermalOverloadUserLimit = InputLimits.BusRegulatorThermalOverloadUserLimit;
            _busUndervoltageUserLimit = InputLimits.BusUndervoltageUserLimit;

            OkCommand = new RelayCommand(ExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public AdvancedUserLimits InputLimits { get; }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        public string Title
        {
            get
            {
                var title = LanguageManager.GetInstance().ConvertSpecifier("AdvancedUserLimits");
                if (string.IsNullOrEmpty(title)) title = "Advanced User Limits";
                if (CheckIsDirty())
                    return title + "*";

                return title;
            }
        }

        public float ConverterThermalOverloadUserLimit
        {
            get { return _converterThermalOverloadUserLimit; }
            set
            {
                Set(ref _converterThermalOverloadUserLimit, value);

                RaisePropertyChanged("Title");
            }
        }

        public float BusRegulatorThermalOverloadUserLimit
        {
            get { return _busRegulatorThermalOverloadUserLimit; }
            set
            {
                Set(ref _busRegulatorThermalOverloadUserLimit, value);

                RaisePropertyChanged("Title");
            }
        }

        public float BusUndervoltageUserLimit
        {
            get { return _busUndervoltageUserLimit; }
            set
            {
                Set(ref _busUndervoltageUserLimit, value);

                RaisePropertyChanged("Title");
            }
        }

        public RelayCommand OkCommand { get; }
        public RelayCommand CancelCommand { get; }

        private void ExecuteCancelCommand()
        {
            DialogResult = false;
        }

        private void ExecuteOkCommand()
        {
            DialogResult = true;
        }

        private bool CheckIsDirty()
        {
            if (Math.Abs(_converterThermalOverloadUserLimit - InputLimits.ConverterThermalOverloadUserLimit) >
                float.Epsilon)
                return true;

            if (Math.Abs(_busRegulatorThermalOverloadUserLimit - InputLimits.BusRegulatorThermalOverloadUserLimit) >
                float.Epsilon)
                return true;

            if (Math.Abs(_busUndervoltageUserLimit - InputLimits.BusUndervoltageUserLimit) > float.Epsilon)
                return true;

            return false;
        }

        private void LanguageChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(Title));
        }

        public override void Cleanup()
        {
            base.Cleanup();
            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }
    }
}
