using System;
using System.Collections;
using System.Windows;
using GalaSoft.MvvmLight;
using ICSStudio.Cip.Objects;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataWrapper;

namespace ICSStudio.UIServicesPackage.AxisVirtualProperties.Panel
{
    class MotionPlannerViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private bool _isDirty;
        private int _outputCamExecutionTargets;
        private readonly AxisVirtual _axisVirtual;
        private bool _check1, _check2;
        private double _masterPositionFilterBandwidth;
        private ProgrammedStopModeType _selected;
        private Controller controller;
        private uint _bit;
        public object Owner { get; set; }
        public object Control { get; }

        public bool IsEnable
        {
            get { return !controller.IsOnline; }
        }
        public bool MasterPositionFilterBandwidthIsEnable
        {
            get { return Check2 && !controller.IsOnline; }
        }

        public MotionPlannerViewModel(MotionPlanner panel, ITag tag)
        {
            Control = panel;
            panel.DataContext = this;
            _axisVirtual = ((SimpleServices.Tags.Tag) tag)?.DataWrapper as AxisVirtual;
            SetValue();
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
            RaisePropertyChanged("MasterPositionFilterBandwidthIsEnable");
        }

        public override void Cleanup()
        {
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }
        }

        public bool CheckInvalid()
        {
            try
            {
                if (OutputCamExecutionTargets < 0 || OutputCamExecutionTargets > 8)
                {
                    var warningDialog = new WarningDialog("Failed to modify properties",
                            "Enter a OutputCamExecutionTargets between 0 and 8")
                        {Owner = Application.Current.MainWindow};
                    warningDialog.ShowDialog();
                    return false;
                }

                if (MasterPositionFilterBandwidth < 0 || MasterPositionFilterBandwidth > 1000)
                {
                    var warningDialog = new WarningDialog("Failed to modify properties",
                            "Enter a MasterPositionFilterBandwidth between 0 and 1000")
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
            uint outputCamExecutionTargets = Convert.ToUInt32(_axisVirtual.CIPAxis.OutputCamExecutionTargets);
            if (outputCamExecutionTargets != (uint)OutputCamExecutionTargets)
            {
                _axisVirtual.CIPAxis.OutputCamExecutionTargets = (uint)OutputCamExecutionTargets;
                _axisVirtual.NotifyParentPropertyChanged("OutputCamExecutionTargets");
            }

            byte programmedStopMode = Convert.ToByte(_axisVirtual.CIPAxis.ProgrammedStopMode);
            if (programmedStopMode != (byte)Selected)
            {
                _axisVirtual.CIPAxis.ProgrammedStopMode = (byte)Selected;
                _axisVirtual.NotifyParentPropertyChanged("ProgrammedStopMode");
            }

            uint masterInputConfigurationBits = Convert.ToUInt32(_axisVirtual.CIPAxis.MasterInputConfigurationBits);
            if (masterInputConfigurationBits != _bit)
            {
                _axisVirtual.CIPAxis.MasterInputConfigurationBits = _bit;
                _axisVirtual.NotifyParentPropertyChanged("MasterInputConfigurationBits");
            }

            float masterPositionFilterBandwidth = Convert.ToSingle(_axisVirtual.CIPAxis.MasterPositionFilterBandwidth);
            if (Math.Abs(masterPositionFilterBandwidth - (float)MasterPositionFilterBandwidth) > float.Epsilon)
            {
                _axisVirtual.CIPAxis.MasterPositionFilterBandwidth = (float)MasterPositionFilterBandwidth;
                _axisVirtual.NotifyParentPropertyChanged("MasterPositionFilterBandwidth");
            }
        }

        public void SetValue()
        {
            OutputCamExecutionTargets = Convert.ToInt32(_axisVirtual.CIPAxis.OutputCamExecutionTargets);
            ProgramStopActionList = EnumHelper.ToDataSource<ProgrammedStopModeType>();
            Selected = (ProgrammedStopModeType) Convert.ToByte(_axisVirtual.CIPAxis.ProgrammedStopMode);
            _bit = Convert.ToUInt32(_axisVirtual.CIPAxis.MasterInputConfigurationBits);
            MasterPositionFilterBandwidth = Convert.ToDouble(_axisVirtual.CIPAxis.MasterPositionFilterBandwidth);
        }

        public void Compare()
        {
            IsDirty = false;
            if (OutputCamExecutionTargets != Convert.ToInt32(_axisVirtual.CIPAxis.OutputCamExecutionTargets))
                IsDirty = true;
            if (Selected != (ProgrammedStopModeType) Convert.ToByte(_axisVirtual.CIPAxis.ProgrammedStopMode))
                IsDirty = true;
            if (Convert.ToByte(_axisVirtual.CIPAxis.MasterInputConfigurationBits) != _bit) IsDirty = true;
            if (Math.Abs(MasterPositionFilterBandwidth -
                         Convert.ToSingle(_axisVirtual.CIPAxis.MasterPositionFilterBandwidth)) > float.Epsilon)
                IsDirty = true;

        }

        public double MasterPositionFilterBandwidth
        {
            set
            {
                Set(ref _masterPositionFilterBandwidth, value);
                Compare();
            }
            get { return _masterPositionFilterBandwidth; }
        }

        public bool Check1
        {
            set
            {

                FlagsEnumHelper.SelectFlag(AxisVirtualParameters.MasterInputConfigurationTypes.MasterDelayCompensation,
                    value, ref _bit);
                Set(ref _check1, value);
                Compare();
            }
            get
            {
                _check1 = FlagsEnumHelper.ContainFlag(_bit,
                    AxisVirtualParameters.MasterInputConfigurationTypes.MasterDelayCompensation);
                return _check1;
            }
        }

        public bool Check2
        {
            set
            {
                FlagsEnumHelper.SelectFlag(AxisVirtualParameters.MasterInputConfigurationTypes.MasterPositionFilter,
                    value, ref _bit);
                Set(ref _check2, value);
                //if (_check2) IsEnable = true;
                //else IsEnable = false;
                RaisePropertyChanged("MasterPositionFilterBandwidthIsEnable");
                Compare();
            }
            get
            {
                _check2 = FlagsEnumHelper.ContainFlag(_bit,
                    AxisVirtualParameters.MasterInputConfigurationTypes.MasterPositionFilter);
                return _check2;
            }
        }

        public int OutputCamExecutionTargets
        {
            set
            {
                Set(ref _outputCamExecutionTargets, value);
                Compare();
            }
            get { return _outputCamExecutionTargets; }
        }

        public IList ProgramStopActionList { set; get; }

        public ProgrammedStopModeType Selected
        {
            set
            {
                _selected = value;
                Compare();
            }
            get { return _selected; }
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
