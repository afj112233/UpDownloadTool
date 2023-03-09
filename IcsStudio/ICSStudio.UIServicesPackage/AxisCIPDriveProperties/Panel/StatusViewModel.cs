using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ICSStudio.Cip.Objects;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class StatusViewModel : DefaultViewModel
    {
        private readonly DispatcherTimer _uiUpdateTimer;
        public StatusViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            _uiUpdateTimer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(100)}; // 100ms
            _uiUpdateTimer.Tick += UIUpdateTimerOnTick;
            _uiUpdateTimer.Start();
        }

        public override void Cleanup()
        {
            _uiUpdateTimer?.Stop();
            base.Cleanup();
        }

        public override void Show()
        {
            UIVisibilityAndReadonly();
            UIRefresh();
        }

        public string ActualPosition => FloatToString(Convert.ToSingle(OriginalCIPAxis.ActualPosition));
        public string CommandPosition => FloatToString(Convert.ToSingle(OriginalCIPAxis.CommandPosition));
        public string ActualVelocity => FloatToString(Convert.ToSingle(OriginalCIPAxis.ActualVelocity));
        public string CommandVelocity => FloatToString(Convert.ToSingle(OriginalCIPAxis.CommandVelocity));

        public bool CommandEnabled
        {
            get
            {
                var axisConfig = (AxisConfigurationType) Convert.ToByte(ModifiedCIPAxis.AxisConfiguration);
                return axisConfig != AxisConfigurationType.FeedbackOnly;
            }
        }

        public Visibility ActualVisibility
        {
            get
            {
                if (!ParentViewModel.IsOnLine)
                    return Visibility.Hidden;

                return Visibility.Visible;
            }
        }
        public Visibility CommandVisibility
        {
            get
            {
                if (!ParentViewModel.IsOnLine)
                    return Visibility.Hidden;

                var axisConfig = (AxisConfigurationType) Convert.ToByte(ModifiedCIPAxis.AxisConfiguration);
                return axisConfig == AxisConfigurationType.FeedbackOnly ? Visibility.Hidden : Visibility.Visible;
            }
        }

        #region Axis Status

        public bool ACPowerLoss => GetCIPAxisStatus(CIPAxisStatusBitmap.ACPowerLoss);
        public bool DCBusUp => GetCIPAxisStatus(CIPAxisStatusBitmap.DCBusUp);
        public bool DCBusUnload => GetCIPAxisStatus(CIPAxisStatusBitmap.DCBusUnload);
        public bool Shutdown => GetCIPAxisStatus(CIPAxisStatusBitmap.Shutdown);
        public bool PowerStructureEnabled => GetCIPAxisStatus(CIPAxisStatusBitmap.PowerStructureEnabled);
        public bool TrackingCommand => GetCIPAxisStatus(CIPAxisStatusBitmap.TrackingCommand);
        public bool FeedbackIntegrity => GetCIPAxisStatus(CIPAxisStatusBitmap.FeedbackIntegrity);
        public bool PositionLock => GetCIPAxisStatus(CIPAxisStatusBitmap.PositionLock);
        public bool VelocityLock => GetCIPAxisStatus(CIPAxisStatusBitmap.VelocityLock);
        public bool VelocityStandstill => GetCIPAxisStatus(CIPAxisStatusBitmap.VelocityStandstill);
        public bool VelocityThreshold => GetCIPAxisStatus(CIPAxisStatusBitmap.VelocityThreshold);
        public bool VelocityLimit => GetCIPAxisStatus(CIPAxisStatusBitmap.VelocityLimit);
        public bool TorqueThreshold => GetCIPAxisStatus(CIPAxisStatusBitmap.TorqueThreshold);
        public bool TorqueLimit => GetCIPAxisStatus(CIPAxisStatusBitmap.TorqueLimit);
        public bool CurrentLimit => GetCIPAxisStatus(CIPAxisStatusBitmap.CurrentLimit);
        public bool ThermalLimit => GetCIPAxisStatus(CIPAxisStatusBitmap.ThermalLimit);

        private bool GetCIPAxisStatus(CIPAxisStatusBitmap bitmap)
        {
            if (!ParentViewModel.IsOnLine)
                return false;

            var cipAxisStatus =
                (CIPAxisStatusBitmap) Convert.ToUInt32(OriginalCIPAxis.CIPAxisStatus);

            if ((cipAxisStatus & bitmap) != 0)
                return true;

            return false;
        }

        #endregion

        #region Digital I/O

        public bool Enable => GetCIPAxisIOStatus(CIPAxisIOStatusBitmap.EnableInput);
        public bool Home => GetCIPAxisIOStatus(CIPAxisIOStatusBitmap.HomeInput);
        public bool Registration1 => GetCIPAxisIOStatus(CIPAxisIOStatusBitmap.Registration1Input);
        public bool Registration2 => GetCIPAxisIOStatus(CIPAxisIOStatusBitmap.Registration2Input);
        public bool PositiveOvertravel => GetCIPAxisIOStatus(CIPAxisIOStatusBitmap.PositiveOvertravelInput);
        public bool NegativeOvertravel => GetCIPAxisIOStatus(CIPAxisIOStatusBitmap.NegativeOvertravelInput);
        public bool FeedbackThermostat => GetCIPAxisIOStatus(CIPAxisIOStatusBitmap.Feedback1Thermostat);
        public bool MotorThermostat => GetCIPAxisIOStatus(CIPAxisIOStatusBitmap.MotorThermostatInput);
        public bool MechanicalBrake => GetCIPAxisIOStatus(CIPAxisIOStatusBitmap.MechanicalBrakeOutput);

        private bool GetCIPAxisIOStatus(CIPAxisIOStatusBitmap bitmap)
        {
            if (!ParentViewModel.IsOnLine)
                return false;

            var cipAxisIOStatus =
                (CIPAxisIOStatusBitmap) Convert.ToUInt32(OriginalCIPAxis.CIPAxisIOStatus);
            
            if ((cipAxisIOStatus & bitmap) != 0)
                return true;

            return false;
        }

        public bool RegenerativePower => GetCIPAxisIOStatusRA(CIPAxisIOStatusRABitmap.RegenerativePowerInput);
        public bool ContactorEnable => GetCIPAxisIOStatusRA(CIPAxisIOStatusRABitmap.ContactorEnableOutput);
        public bool PreCharge => GetCIPAxisIOStatusRA(CIPAxisIOStatusRABitmap.PreChargeInput);
        public bool BusCapacitor => GetCIPAxisIOStatusRA(CIPAxisIOStatusRABitmap.BusCapacitorModuleInput);
        public bool ShuntThermalSwitch => GetCIPAxisIOStatusRA(CIPAxisIOStatusRABitmap.ShuntThermalSwitchInput);

        private bool GetCIPAxisIOStatusRA(CIPAxisIOStatusRABitmap bitmap)
        {
            if (!ParentViewModel.IsOnLine)
                return false;

            var cipAxisIOStatusRA =
                (CIPAxisIOStatusRABitmap) Convert.ToUInt32(OriginalCIPAxis.CIPAxisIOStatusRA);

            if ((cipAxisIOStatusRA & bitmap) != 0)
                return true;

            return false;
        }

        #endregion

        #region Private

        private string FloatToString(float value)
        {
            return value.ToString(DisplayStyle.Float);
        }

        private void UIUpdateTimerOnTick(object sender, EventArgs e)
        {
            //TODO(gjc): use event handle
            //UIRefresh();
        }

        private void UIRefresh()
        {
            RaisePropertyChanged(nameof(ActualPosition));
            RaisePropertyChanged(nameof(CommandPosition));
            RaisePropertyChanged(nameof(ActualVelocity));
            RaisePropertyChanged(nameof(CommandVelocity));

            RaisePropertyChanged(nameof(ACPowerLoss));
            RaisePropertyChanged(nameof(DCBusUp));
            RaisePropertyChanged(nameof(DCBusUnload));
            RaisePropertyChanged(nameof(Shutdown));
            RaisePropertyChanged(nameof(PowerStructureEnabled));
            RaisePropertyChanged(nameof(TrackingCommand));
            RaisePropertyChanged(nameof(FeedbackIntegrity));
            RaisePropertyChanged(nameof(PositionLock));
            RaisePropertyChanged(nameof(VelocityLock));
            RaisePropertyChanged(nameof(VelocityStandstill));
            RaisePropertyChanged(nameof(VelocityThreshold));
            RaisePropertyChanged(nameof(VelocityLimit));
            RaisePropertyChanged(nameof(TorqueThreshold));
            RaisePropertyChanged(nameof(TorqueLimit));
            RaisePropertyChanged(nameof(CurrentLimit));
            RaisePropertyChanged(nameof(ThermalLimit));

            RaisePropertyChanged(nameof(Enable));
            RaisePropertyChanged(nameof(Home));
            RaisePropertyChanged(nameof(Registration1));
            RaisePropertyChanged(nameof(Registration2));
            RaisePropertyChanged(nameof(PositiveOvertravel));
            RaisePropertyChanged(nameof(NegativeOvertravel));
            RaisePropertyChanged(nameof(FeedbackThermostat));
            RaisePropertyChanged(nameof(MotorThermostat));
            RaisePropertyChanged(nameof(MechanicalBrake));

            RaisePropertyChanged(nameof(RegenerativePower));
            RaisePropertyChanged(nameof(ContactorEnable));
            RaisePropertyChanged(nameof(PreCharge));
            RaisePropertyChanged(nameof(BusCapacitor));
            RaisePropertyChanged(nameof(ShuntThermalSwitch));
        }

        private void UIVisibilityAndReadonly()
        {
            RaisePropertyChanged(nameof(ActualVisibility));
            RaisePropertyChanged(nameof(CommandVisibility));
            RaisePropertyChanged(nameof(CommandEnabled));
        }

        #endregion
    }
}