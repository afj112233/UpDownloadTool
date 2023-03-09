using System;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.UIServicesPackage.ViewModel;

namespace ICSStudio.UIServicesPackage.AxisVirtualProperties.Panel
{
    class HomingViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private readonly AxisVirtual _axisVirtual;
        private bool _isDirty;
        private double _position;
        private Controller controller;
        private string _mode;
        private string _sequence;

        public object Owner { get; set; }
        public object Control { get; }
        public bool IsEnable
        {
            get { return !controller.IsOnline; }
        }

        public HomingViewModel(Homing panel, ITag tag)
        {
            Control = panel;
            panel.DataContext = this;
            _axisVirtual = ((SimpleServices.Tags.Tag) tag).DataWrapper as AxisVirtual;
            Mode = Enum.GetName(typeof(HomeModeType), Convert.ToByte(_axisVirtual.CIPAxis.HomeMode));
            
            Position = Convert.ToDouble(_axisVirtual.CIPAxis.HomePosition);
            Sequence = Enum.GetName(typeof(HomeSequenceType), Convert.ToByte(_axisVirtual.CIPAxis.HomeSequence));
            PositionUnits = Convert.ToString(_axisVirtual.CIPAxis.PositionUnits);
            Messenger.Default.Register<Message>(this, OnReceiveGlobalMessage);
            controller = tag.ParentController as Controller;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", OnLanguageChanged);
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            RaisePropertyChanged("IsEnable");
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(Mode));
            RaisePropertyChanged(nameof(Sequence));
        }

        public override void Cleanup()
        {
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

        public void Compare()
        {
            IsDirty = false;
            if (Math.Abs(Position - Convert.ToSingle(_axisVirtual.CIPAxis.HomePosition)) > float.Epsilon) IsDirty = true;
            PositionUnits = Convert.ToString(_axisVirtual.CIPAxis.PositionUnits);
            RaisePropertyChanged("PositionUnits");
        }

        public bool CheckInvalid()
        {
            return true;
        }

        public void Save()
        {
            float homePosition = Convert.ToSingle(_axisVirtual.CIPAxis.HomePosition);
            if (Math.Abs(homePosition - (float)Position) > float.Epsilon)
            {
                _axisVirtual.CIPAxis.HomePosition = (float)Position;
                _axisVirtual.NotifyParentPropertyChanged("HomePosition");
            }
        }

        public string PositionUnits { set; get; }

        public string Mode
        {
            set { _mode = value; }
            get
            {
                var mode =  LanguageManager.GetInstance().ConvertSpecifier(_mode);
                return string.IsNullOrEmpty(mode) ? _mode : mode;
            }
        }

        public double Position
        {
            set
            {
                _position = value;
                Compare();
            }
            get { return _position; }
        }

        public string Sequence
        {
            set { _sequence = value; }
            get
            {
                var sequence = LanguageManager.GetInstance().ConvertSpecifier(_sequence);
                return string.IsNullOrEmpty(sequence) ? _sequence : sequence;
            }
        }

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
                RaisePropertyChanged("PositionUnits");
            }
        }
    }
}
