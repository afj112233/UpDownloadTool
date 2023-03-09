using System;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIServicesPackage.View;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    class ManualAdjustViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private readonly AxisVirtual _axisVirtual;
        private float _speed;
        private float _accel;
        private float _decel;
        private float _accelJerk;
        private float _decelJerk;
        private readonly DispatcherTimer _timer;
        private bool _isDirty;

        public ManualAdjustViewModel(ManualAdjust panel, ITag tag, string positionUnits)
        {
            Control = panel;
            panel.DataContext = this;

            _axisVirtual = ((Tag) tag)?.DataWrapper as AxisVirtual;
            Title = "Manual Adjust - " + tag?.Name;
            PositionUnits = positionUnits;

            _speed = Convert.ToSingle(_axisVirtual.CIPAxis.MaximumSpeed);
            Speed = Convert.ToSingle(_axisVirtual.CIPAxis.MaximumSpeed);
            _accel = Convert.ToSingle(_axisVirtual.CIPAxis.MaximumAcceleration);
            Accel = Convert.ToSingle(_axisVirtual.CIPAxis.MaximumAcceleration);

            _decel = Convert.ToSingle(_axisVirtual.CIPAxis.MaximumDeceleration);
            Decel = Convert.ToSingle(_axisVirtual.CIPAxis.MaximumDeceleration);

            _accelJerk = Convert.ToSingle(_axisVirtual.CIPAxis.MaximumAccelerationJerk);
            AccelJerk = Convert.ToSingle(_axisVirtual.CIPAxis.MaximumAccelerationJerk);

            _decelJerk = Convert.ToSingle(_axisVirtual.CIPAxis.MaximumDecelerationJerk);
            DecelJerk = Convert.ToSingle(_axisVirtual.CIPAxis.MaximumDecelerationJerk);

            ResetCommand = new RelayCommand(Reset, CanResetCommand);

            _timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(400)};

            _timer.Tick += CycleUpdateTimerHandle;
            _timer.Start();
            Messenger.Default.Register<Message>(this, m => m = null);
        }

        public override void Cleanup()
        {
            _timer?.Stop();
            Messenger.Default.Unregister(this);
            base.Cleanup();
        }

        public void Save()
        {
            _axisVirtual.CIPAxis.MaximumSpeed = Speed;
            _axisVirtual.CIPAxis.MaximumAcceleration = Accel;
            _axisVirtual.CIPAxis.MaximumDeceleration = Decel;
            _axisVirtual.CIPAxis.MaximumAccelerationJerk = AccelJerk;
            _axisVirtual.CIPAxis.MaximumDecelerationJerk = DecelJerk;

            Messenger.Default.Send(new Message() {Type = "ManualAdjust"});
        }

        public RelayCommand ResetCommand { get; }

        public bool CanResetCommand()
        {
            if (Math.Abs(Speed - Convert.ToSingle(_axisVirtual.CIPAxis.MaximumSpeed)) > float.Epsilon ||
                Math.Abs(Accel - Convert.ToSingle(_axisVirtual.CIPAxis.MaximumAcceleration)) > float.Epsilon ||
                Math.Abs(Decel - Convert.ToSingle(_axisVirtual.CIPAxis.MaximumDeceleration)) > float.Epsilon ||
                Math.Abs(AccelJerk - Convert.ToSingle(_axisVirtual.CIPAxis.MaximumAccelerationJerk)) > float.Epsilon ||
                Math.Abs(DecelJerk - Convert.ToSingle(_axisVirtual.CIPAxis.MaximumDecelerationJerk)) > float.Epsilon)
            {
                IsDirty = true;
                return true;
            }

            IsDirty = false;
            return false;
        }

        public void Reset()
        {
            Speed = _speed;
            Accel = _accel;
            AccelJerk = _accelJerk;
            Decel = _decel;
            DecelJerk = +DecelJerk;

            Save();

            RaisePropertyChanged("Speed");
            RaisePropertyChanged("Accel");
            RaisePropertyChanged("AccelJerk");
            RaisePropertyChanged("Decel");
            RaisePropertyChanged("DecelJerk");
        }

        public bool IsEnable { set; get; }
        public string PositionUnits { set; get; }
        public float Speed { set; get; }
        public float Accel { set; get; }
        public float Decel { set; get; }
        public float AccelJerk { set; get; }
        public float DecelJerk { set; get; }
        public string Title { get; }
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
                if (_isDirty != value)
                {
                    _isDirty = value;
                    IsDirtyChanged?.Invoke(this, EventArgs.Empty);
                }

                Set(ref _isDirty, value);
            }
        }

        public event EventHandler IsDirtyChanged;

        private void CycleUpdateTimerHandle(object state, EventArgs e)
        {
            ResetCommand.RaiseCanExecuteChanged();
        }

    }
}
