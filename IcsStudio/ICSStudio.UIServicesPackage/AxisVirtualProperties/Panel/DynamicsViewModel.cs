using System;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.UIServicesPackage.View;
using ICSStudio.UIServicesPackage.ViewModel;
using ICSStudio.Utils;

namespace ICSStudio.UIServicesPackage.AxisVirtualProperties.Panel
{
    class DynamicsViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private readonly AxisVirtual _axisVirtual;
        private bool _isDirty;
        private double _speed;
        private double _acceleration;
        private double _deceleration;
        private double _accelerationJerk;
        private double _decelerationJerk;
        private bool _isEnable;
        private readonly ITag _tag;
        private ConversionViewModel _conversionViewModel;
        private const float ConversionConstant = 2.14748e+12f;
        public DynamicsViewModel(Dynamics panel, ConversionViewModel conversionViewModel, ITag tag)
        {
            Control = panel;
            _conversionViewModel = conversionViewModel;
            _tag = tag;
            panel.DataContext = this;
            _axisVirtual = ((SimpleServices.Tags.Tag) tag)?.DataWrapper as AxisVirtual;
            CalCommand = new RelayCommand<string>(CalCommandClick);
            ManualAdjustCommand = new RelayCommand(ManualAdjustCommandClick);
            PositionUnits = Convert.ToString(_axisVirtual.CIPAxis.PositionUnits);
            Messenger.Default.Register<Message>(this, OnReceiveGlobalMessage);
            SetValue();
            
            Controller controller = tag.ParentController as Controller;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged",OnLanguageChanged);
        }

        public object Owner { get; set; }
        public object Control { get; }

        public override void Cleanup()
        {
            Controller controller = _tag.ParentController as Controller;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }

            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
                LanguageManager.GetInstance(), "LanguageChanged", OnLanguageChanged);

            Messenger.Default.Unregister(this);
            base.Cleanup();
        }

        public bool ManualAdjustEnable { set; get; }

        public void SetValue()
        {
            Speed = Convert.ToDouble(_axisVirtual.CIPAxis.MaximumSpeed);

            Acceleration = Convert.ToDouble(_axisVirtual.CIPAxis.MaximumAcceleration);

            AccelerationJerk = Convert.ToDouble(_axisVirtual.CIPAxis.MaximumAccelerationJerk);

            Deceleration = Convert.ToDouble(_axisVirtual.CIPAxis.MaximumDeceleration);

            DecelerationJerk = Convert.ToDouble(_axisVirtual.CIPAxis.MaximumDecelerationJerk);

            ManualAdjustEnable = true;
            
            _isEnable = !(_axisVirtual.Controller.IsOnline && _axisVirtual.AssignedGroup != null);
        }

        public void Compare()
        {
            IsDirty = false;
            if (Math.Abs(Speed - Convert.ToSingle(_axisVirtual.CIPAxis.MaximumSpeed)) > float.Epsilon) IsDirty = true;
            if (Math.Abs(Acceleration - Convert.ToSingle(_axisVirtual.CIPAxis.MaximumAcceleration)) > float.Epsilon) IsDirty = true;
            if (Math.Abs(AccelerationJerk - Convert.ToSingle(_axisVirtual.CIPAxis.MaximumAccelerationJerk)) > float.Epsilon)
                IsDirty = true;
            if (Math.Abs(Deceleration - Convert.ToSingle(_axisVirtual.CIPAxis.MaximumDeceleration)) > float.Epsilon) IsDirty = true;
            if (Math.Abs(DecelerationJerk - Convert.ToSingle(_axisVirtual.CIPAxis.MaximumDecelerationJerk)) > float.Epsilon)
                IsDirty = true;
            PositionUnits = Convert.ToString(_axisVirtual.CIPAxis.PositionUnits);
            RaisePropertyChanged("PositionUnits");
        }

        public void Save()
        {
            if (!CheckValid(Speed, true, "MaximumSpeed") ||
                !CheckValid(Acceleration, false, "MaximumAcceleration") ||
                !CheckValid(Deceleration, false, "MaximumDeceleration") ||
                !CheckValid(AccelerationJerk, false, "MaximumAccelerationJerk") ||
                !CheckValid(DecelerationJerk, false, "MaximumDecelerationJerk"))
                return;

            float maximumSpeed = Convert.ToSingle(_axisVirtual.CIPAxis.MaximumSpeed);
            if (Math.Abs(maximumSpeed - (float)Speed) > float.Epsilon)
            {
                _axisVirtual.CIPAxis.MaximumSpeed = (float)Speed;
                _axisVirtual.NotifyParentPropertyChanged("MaximumSpeed");
            }

            float maximumAcceleration = Convert.ToSingle(_axisVirtual.CIPAxis.MaximumAcceleration);
            if (Math.Abs(maximumAcceleration - (float)Acceleration) > float.Epsilon)
            {
                _axisVirtual.CIPAxis.MaximumAcceleration = (float)Acceleration;
                _axisVirtual.NotifyParentPropertyChanged("MaximumAcceleration");
            }

            float maximumAccelerationJerk = Convert.ToSingle(_axisVirtual.CIPAxis.MaximumAccelerationJerk);
            if (Math.Abs(maximumAccelerationJerk - (float)AccelerationJerk) > float.Epsilon)
            {
                _axisVirtual.CIPAxis.MaximumAccelerationJerk = (float)AccelerationJerk;
                _axisVirtual.NotifyParentPropertyChanged("MaximumAccelerationJerk");
            }

            float maximumDeceleration = Convert.ToSingle(_axisVirtual.CIPAxis.MaximumDeceleration);
            if (Math.Abs(maximumDeceleration - (float)Deceleration) > float.Epsilon)
            {
                _axisVirtual.CIPAxis.MaximumDeceleration = (float)Deceleration;
                _axisVirtual.NotifyParentPropertyChanged("MaximumDeceleration");
            }

            float maximumDecelerationJerk = Convert.ToSingle(_axisVirtual.CIPAxis.MaximumDecelerationJerk);
            if (Math.Abs(maximumDecelerationJerk - (float)DecelerationJerk) > float.Epsilon)
            {
                _axisVirtual.CIPAxis.MaximumDecelerationJerk = (float)DecelerationJerk;
                _axisVirtual.NotifyParentPropertyChanged("MaximumDecelerationJerk");
            }
        }

        private bool CheckValid(double checkValue, bool isSpeed, string name)
        {
            var max = ConversionConstant / _conversionViewModel.ConversionConstant * (isSpeed ? 1 : 1000);
            if (checkValue > max || checkValue < 0)
            {
                MessageBox.Show(
                    $"Failed to changed the properties for axis '{_tag.Name}'\nEnter a {name} between 0 and {max:e5}",
                    "ICS Studio", MessageBoxButton.OK);
                return false;
            }

            return true;
        }

        public RelayCommand<string> CalCommand { set; get; }

        public RelayCommand ManualAdjustCommand { set; get; }

        public void CalCommandClick(string para)
        {
            if (para == "1")
            {
                var dialog = new CalculateJerkDialog(
                        "Maximum Acceleration Jerk",
                        PositionUnits,
                        Speed, Acceleration, AccelerationJerk)
                    {Owner = Application.Current.MainWindow};

                var dialogResult = dialog.ShowDialog();
                if (dialogResult.HasValue && dialogResult.Value)
                    AccelerationJerk = dialog.Jerk;
            }
            else
            {
                var dialog = new CalculateJerkDialog(
                        "Maximum Deceleration Jerk",
                        PositionUnits,
                        Speed, Deceleration, DecelerationJerk)
                    {Owner = Application.Current.MainWindow};

                var dialogResult = dialog.ShowDialog();
                if (dialogResult.HasValue && dialogResult.Value)
                    DecelerationJerk = dialog.Jerk;
            }

        }

        public void ManualAdjustCommandClick()
        {
          
            var viewModel = new ManualAdjustVM(_axisVirtual.ParentTag, PositionUnits);
            TabbedOptionsDialog dialog = new TabbedOptionsDialog(viewModel);
            viewModel.CloseAction = dialog.Close;
            dialog.Width = 520;
            dialog.Owner = Application.Current.MainWindow;
            dialog.ShowDialog();

        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            RaisePropertyChanged("IsEnable");
        }

        public bool IsEnable
        {
            set { Set(ref _isEnable, value); }
            get { return !(_axisVirtual.Controller.IsOnline && _axisVirtual.AssignedGroup != null); }
        }

        public string PositionUnits { set; get; }

        public double Speed
        {
            set
            {
                _speed = value;
                RaisePropertyChanged("Speed");
                Compare();
            }
            get { return _speed; }
        }

        public double Acceleration
        {
            set
            {
                _acceleration = value;
                RaisePropertyChanged("Acceleration");
                Compare();
            }
            get { return _acceleration; }
        }

        public double Deceleration
        {
            set
            {
                _deceleration = value;
                RaisePropertyChanged("Deceleration");
                Compare();
            }
            get { return _deceleration; }
        }

        public double AccelerationJerk
        {
            set
            {
                _accelerationJerk = value;
                AccelJerkText = GetPercentTime(Speed, Acceleration, AccelerationJerk) + LanguageManager.GetInstance().ConvertSpecifier("of Max Accel Time");
                RaisePropertyChanged("AccelerationJerk");
                RaisePropertyChanged("AccelJerkText");
                Compare();
            }
            get { return _accelerationJerk; }
        }

        public double DecelerationJerk
        {
            set
            {
                _decelerationJerk = value;
                DecelJerkText = GetPercentTime(Speed, Deceleration, DecelerationJerk) + LanguageManager.GetInstance().ConvertSpecifier("of Max Decel Time");
                RaisePropertyChanged("DecelJerkText");
                RaisePropertyChanged("DecelerationJerk");
                Compare();
            }
            get { return _decelerationJerk; }
        }

        public string AccelJerkText { set; get; }
        public string DecelJerkText { set; get; }

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
                if (_isDirty == false) ManualAdjustEnable = true;
                else ManualAdjustEnable = false;
                RaisePropertyChanged("ManualAdjustEnable");
            }
        }

        public event EventHandler IsDirtyChanged;

        private string GetPercentTime(
            double speed,
            double accel,
            double jerk)
        {
            if (speed < float.Epsilon || accel < float.Epsilon)
                return "< 1%";

            var percentTime = JerkRateCalculation.CalculatePercentTime(speed, accel, jerk);
            if (percentTime < 1)
                return "< 1%";
            if (percentTime > 100)
                return "> 100%";

            var result = (int) percentTime;
            return $"= {result}%";
        }

        private void OnReceiveGlobalMessage(Message message)
        {
            if (message.Type.Equals("PositionUnits"))
            {
                PositionUnits = message.Value;
                RaisePropertyChanged("PositionUnits");
            }

            if (message.Type.Equals("ManualAdjust"))
            {
                SetValue();
            }
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            AccelJerkText = GetPercentTime(Speed, Acceleration, AccelerationJerk) + LanguageManager.GetInstance().ConvertSpecifier("of Max Accel Time");
            RaisePropertyChanged(nameof(AccelJerkText));
            DecelJerkText = GetPercentTime(Speed, Deceleration, DecelerationJerk) + LanguageManager.GetInstance().ConvertSpecifier("of Max Decel Time");
            RaisePropertyChanged(nameof(DecelJerkText));
        }
    }
}
