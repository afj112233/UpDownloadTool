using System;
using System.Diagnostics;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.UIServicesPackage.ViewModel;

namespace ICSStudio.UIServicesPackage.AxisVirtualProperties.Panel
{
    class UnitsViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private readonly AxisVirtual _axisVirtual;
        private bool _isDirty;
        private string _positionUnits;
        private double _averageVelocityTimebase;
        private Controller controller;

        public object Owner { get; set; }
        public object Control { get; }

        public UnitsViewModel(Units panel, ITag tag)
        {
            Control = panel;
            panel.DataContext = this;
            _axisVirtual = ((SimpleServices.Tags.Tag) tag)?.DataWrapper as AxisVirtual;

            PositionUnits = Convert.ToString(_axisVirtual.CIPAxis.PositionUnits);
            AverageVelocityTimebase = Convert.ToDouble(_axisVirtual.CIPAxis.AverageVelocityTimebase);
            Messenger.Default.Register<Message>(this, m => { m = null; });

            controller = tag.ParentController as Controller;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            RaisePropertyChanged("IsEnable");
        }

        public bool IsEnable
        {
            get { return !_axisVirtual.Controller.IsOnline; }
        }

        public override void Cleanup()
        {
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }
            Messenger.Default.Unregister(this);
            base.Cleanup();
        }

        public void Compare()
        {
            IsDirty = false;
            if (PositionUnits == null || !PositionUnits.Equals(Convert.ToString(_axisVirtual.CIPAxis.PositionUnits)))
                IsDirty = true;
            if (Math.Abs(AverageVelocityTimebase - Convert.ToSingle(_axisVirtual.CIPAxis.AverageVelocityTimebase)) >
                float.Epsilon) IsDirty = true;
        }

        public string PositionUnits
        {
            set
            {
                _positionUnits = value;
                Messenger.Default.Send(new Message() {IsDirty = _isDirty, Type = "PositionUnits", Value = value});
                Compare();
            }
            get { return _positionUnits; }
        }

        public double AverageVelocityTimebase
        {
            set
            {
                _averageVelocityTimebase = value;
                Compare();
            }
            get { return _averageVelocityTimebase; }
        }

        public bool CheckInvalid()
        {

            try
            {
                float min = 0.001f, max = 32;
                if (_axisVirtual.AssignedGroup != null)
                {
                    var mg = ((SimpleServices.Tags.Tag) _axisVirtual.AssignedGroup).DataWrapper as MotionGroup;
                    Debug.Assert(mg != null);
                    var baseUpdatePeriod = mg.CoarseUpdatePeriod / 1000f;
                    min = min * baseUpdatePeriod;
                    max = baseUpdatePeriod;
                } 
                if (AverageVelocityTimebase < min || AverageVelocityTimebase > max)
                {
                    var warningDialog = new WarningDialog("Failed to modify properties",
                            $"Enter a AverageVelocityTimebase between {min} and {max}")
                        {Owner = Application.Current.MainWindow};
                    warningDialog.ShowDialog();
                    return false;
                }
            }
            catch (Exception)
            {
                var warningDialog = new WarningDialog("Failed to modify properties", "String invalid")
                    {Owner = Application.Current.MainWindow};
                warningDialog.ShowDialog();
                return false;
            }

            return true;
        }

        public void Save()
        {
            string positionUnits = Convert.ToString(_axisVirtual.CIPAxis.PositionUnits);
            if (!string.Equals(positionUnits, PositionUnits))
            {
                _axisVirtual.CIPAxis.PositionUnits = PositionUnits;
                _axisVirtual.NotifyParentPropertyChanged("PositionUnits");
            }

            float averageVelocityTimebase = Convert.ToSingle(_axisVirtual.CIPAxis.AverageVelocityTimebase);
            if (Math.Abs(averageVelocityTimebase - (float)AverageVelocityTimebase) > float.Epsilon)
            {
                _axisVirtual.CIPAxis.AverageVelocityTimebase = (float)AverageVelocityTimebase;
                _axisVirtual.NotifyParentPropertyChanged("AverageVelocityTimebase");
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
    }
}