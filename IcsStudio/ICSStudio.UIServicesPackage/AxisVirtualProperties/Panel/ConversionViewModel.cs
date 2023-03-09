using System;
using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.UIServicesPackage.ViewModel;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.AxisVirtualProperties.Panel
{
    class ConversionViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private readonly Controller _controller;
        private readonly AxisVirtual _axisVirtual;

        private bool _isDirty;
        private AxisVirtualParameters.PositioningMode _selected;
        private double _conversionConstant;
        private int _positionUnwind;
        private string _positionUnits;

        public object Owner { get; set; }
        public object Control { get; }

        public bool IsEnable => !_controller.IsOnline;

        public ConversionViewModel(Conversion panel, ITag tag)
        {
            Control = panel;
            panel.DataContext = this;

            _axisVirtual = ((SimpleServices.Tags.Tag)tag)?.DataWrapper as AxisVirtual;
            Debug.Assert(_axisVirtual != null);

            PositioningModeList = EnumHelper.ToDataSource<AxisVirtualParameters.PositioningMode>();
            Selected = (AxisVirtualParameters.PositioningMode)_axisVirtual.RotaryAxis;
            IntervalBoxInputCommand = new RelayCommand<TextCompositionEventArgs>(ExecuteIntervalBoxInputCommand);
            IntervalBoxKeyDownCommand = new RelayCommand<KeyEventArgs>(ExecuteIntervalBoxKeyDownCommand);
            PastingCommand = new RelayCommand<DataObjectPastingEventArgs>(ExecutePastingCommand);
            ConversionConstant = Convert.ToDouble(_axisVirtual.CIPAxis.ConversionConstant.ToSingle(null).ToString("R"));
            PositionUnwind = Convert.ToInt32(_axisVirtual.CIPAxis.PositionUnwind);
            PositionUnits = Convert.ToString(_axisVirtual.CIPAxis.PositionUnits);

            Messenger.Default.Register<Message>(this, OnReceiveGlobalMessage);

            _controller = tag?.ParentController as Controller;
            if (_controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    _controller, "IsOnlineChanged", OnIsOnlineChanged);
            }

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", OnLanguageChanged);
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(
                async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    RaisePropertyChanged(nameof(IsEnable));
                    RaisePropertyChanged(nameof(PositionUnwindIsEnable));

                });

        }

        public override void Cleanup()
        {
            if (_controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                    _controller, "IsOnlineChanged", OnIsOnlineChanged);
            }

            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
                LanguageManager.GetInstance(), "LanguageChanged", OnLanguageChanged);

            Messenger.Default.Unregister(this);
            base.Cleanup();
        }

        public void Compare()
        {
            IsDirty = false;

            if (Selected != (AxisVirtualParameters.PositioningMode)_axisVirtual.RotaryAxis)
                IsDirty = true;

            if (Math.Abs((float)ConversionConstant - Convert.ToSingle(_axisVirtual.CIPAxis.ConversionConstant)) >
                float.Epsilon)
                IsDirty = true;

            if (PositionUnwind != Convert.ToInt32(_axisVirtual.CIPAxis.PositionUnwind))
                IsDirty = true;

            PositionUnits = Convert.ToString(_axisVirtual.CIPAxis.PositionUnits);
        }

        public bool CheckInvalid()
        {
            try
            {

                if (ConversionConstant < 1e-12 || ConversionConstant > 1e+12)
                {
                    var warningDialog = new WarningDialog("Failed to modify properties",
                            "Enter a ConversionConstant between 1e-12 and 1e+12")
                        { Owner = Application.Current.MainWindow };
                    warningDialog.ShowDialog();
                    return false;
                }

                if (PositionUnwind < 1 || PositionUnwind > 1000000000)
                {
                    var warningDialog = new WarningDialog("Failed to modify properties",
                            "Enter a PositionUnwind between 0 and 1000000000")
                        { Owner = Application.Current.MainWindow };
                    warningDialog.ShowDialog();
                    return false;
                }
            }
            catch (Exception)
            {
                var warningDialog = new WarningDialog("Failed to modify properties", "String invalid")
                    { Owner = Application.Current.MainWindow };
                warningDialog.ShowDialog();
                return false;
            }

            return true;
        }

        public void Save()
        {
            byte rotaryAxis = Convert.ToByte(_axisVirtual.RotaryAxis);
            if (rotaryAxis != (byte)Selected)
            {
                _axisVirtual.RotaryAxis = (byte)Selected;
                _axisVirtual.NotifyParentPropertyChanged("RotaryAxis");
            }

            float conversionConstant = Convert.ToSingle(_axisVirtual.CIPAxis.ConversionConstant);
            if (Math.Abs(conversionConstant - (float)ConversionConstant) > float.Epsilon)
            {
                _axisVirtual.CIPAxis.ConversionConstant = (float)ConversionConstant;
                _axisVirtual.NotifyParentPropertyChanged("ConversionConstant");
            }

            uint positionUnwind = Convert.ToUInt32(_axisVirtual.CIPAxis.PositionUnwind);
            if (positionUnwind != (uint)PositionUnwind)
            {
                _axisVirtual.CIPAxis.PositionUnwind = (uint)PositionUnwind;
                _axisVirtual.NotifyParentPropertyChanged("PositionUnwind");
            }
        }

        public string PositionUnits
        {
            set { Set(ref _positionUnits , value); }
            get { return LanguageManager.GetInstance().ConvertSpecifier("Feedback Counts") + "/1.0 " + _positionUnits; }
        }

        public IList PositioningModeList { set; get; }

        public AxisVirtualParameters.PositioningMode Selected
        {
            set
            {
                _selected = value;

                Compare();

                RaisePropertyChanged(nameof(IsEnable));
                RaisePropertyChanged(nameof(PositionUnwindIsEnable));
            }
            get { return _selected; }
        }

        public double ConversionConstant
        {
            set
            {
                _conversionConstant = value;
                Compare();
            }
            get { return _conversionConstant; }
        }

        public int PositionUnwind
        {
            set
            {
                _positionUnwind = value;
                Compare();
            }
            get { return _positionUnwind; }
        }

        public bool PositionUnwindIsEnable
        {
            get
            {
                if (_controller.IsOnline)
                    return false;

                return _selected == AxisVirtualParameters.PositioningMode.Rotary;
            }
        }

        #region Command

        public RelayCommand<KeyEventArgs> IntervalBoxKeyDownCommand { set; get; }

        private void ExecuteIntervalBoxKeyDownCommand(KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        public RelayCommand<TextCompositionEventArgs> IntervalBoxInputCommand { set; get; }

        private void ExecuteIntervalBoxInputCommand(TextCompositionEventArgs e)
        {
            e.Handled = !IsNumber(e.Text);
        }

        private bool IsNumber(string @string)
        {
            if (string.IsNullOrEmpty(@string))
                return false;
            foreach (char c in @string)
            {
                if (!char.IsDigit(c))
                    return false;
            }

            return true;
        }

        public RelayCommand<DataObjectPastingEventArgs> PastingCommand { set; get; }

        private void ExecutePastingCommand(DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsNumber(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        #endregion

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
                if (_isDirty != value)
                {
                    _isDirty = value;
                    IsDirtyChanged?.Invoke(this, EventArgs.Empty);
                }

                Set(ref _isDirty, value);
            }
        }

        public event EventHandler IsDirtyChanged;

        private void OnReceiveGlobalMessage(Message message)
        {
            if (message.Type.Equals("PositionUnits"))
            {
                PositionUnits = message.Value;

                RaisePropertyChanged(nameof(PositionUnits));
            }
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(PositionUnits));
        }
    }
}
