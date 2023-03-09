using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.Cip.Objects;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class ModelViewModel : DefaultViewModel
    {
        public ModelViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            CompareProperties = new[]
            {
                "PMMotorTorqueConstant", "PMMotorRotaryVoltageConstant",
                "PMMotorResistance", "PMMotorInductance", "PMMotorFluxSaturation",
                "PMMotorForceConstant", "PMMotorLinearVoltageConstant",
                "InductionMotorFluxCurrent", "InductionMotorRatedSlipSpeed",
                "InductionMotorStatorLeakageReactance",
                "InductionMotorRotorLeakageReactance",
                "InductionMotorStatorResistance"
            };
        }

        public override void Show()
        {
            UIVisibilityAndReadonly();
            UIRefresh();
        }

        public override Visibility Visibility
        {
            get
            {
                var axisConfiguration =
                    (AxisConfigurationType) Convert.ToByte(ParentViewModel.ModifiedAxisCIPDrive.CIPAxis
                        .AxisConfiguration);

                if (axisConfiguration == AxisConfigurationType.ConverterOnly
                    || axisConfiguration == AxisConfigurationType.FeedbackOnly)
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        public Visibility RotaryPMModelVisibility
        {
            get
            {
                var motorType = (MotorType) Convert.ToByte(ModifiedCIPAxis.MotorType);
                if (motorType == MotorType.RotaryPermanentMagnet)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public Visibility LinearPMModelVisibility
        {
            get
            {
                var motorType = (MotorType) Convert.ToByte(ModifiedCIPAxis.MotorType);
                if (motorType == MotorType.LinearPermanentMagnet)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public Visibility RotaryInductionModelVisibility
        {
            get
            {
                var motorType = (MotorType) Convert.ToByte(ModifiedCIPAxis.MotorType);
                if (motorType == MotorType.RotaryInduction)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public bool MotorParameterEditEnabled
        {
            get
            {
                var motorDataSource = (MotorDataSourceType) Convert.ToByte(ModifiedCIPAxis.MotorDataSource);
                var motorCatalogNumber = ModifiedCIPAxis.MotorCatalogNumber.GetString();

                if (motorDataSource == MotorDataSourceType.Database &&
                    !string.Equals(motorCatalogNumber, "<none>"))
                    return false;

                if (ParentViewModel.IsHardRunMode)
                    return false;

                return true;
            }
        }

        public float PMMotorTorqueConstant
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.PMMotorTorqueConstant); }
            set
            {
                ModifiedCIPAxis.PMMotorTorqueConstant = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float PMMotorRotaryVoltageConstant
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.PMMotorRotaryVoltageConstant); }
            set
            {
                ModifiedCIPAxis.PMMotorRotaryVoltageConstant = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float PMMotorResistance
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.PMMotorResistance); }
            set
            {
                ModifiedCIPAxis.PMMotorResistance = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float PMMotorInductance
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.PMMotorInductance); }
            set
            {
                ModifiedCIPAxis.PMMotorInductance = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float PMMotorFluxSaturation0
        {
            get { return ModifiedCIPAxis.PMMotorFluxSaturation.GetValue(0); }
            set
            {
                ModifiedCIPAxis.PMMotorFluxSaturation.SetValue(0, value);


                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float PMMotorFluxSaturation1
        {
            get { return ModifiedCIPAxis.PMMotorFluxSaturation.GetValue(1); }
            set
            {
                ModifiedCIPAxis.PMMotorFluxSaturation.SetValue(1, value);

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float PMMotorFluxSaturation2
        {
            get { return ModifiedCIPAxis.PMMotorFluxSaturation.GetValue(2); }
            set
            {
                ModifiedCIPAxis.PMMotorFluxSaturation.SetValue(2, value);

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float PMMotorFluxSaturation3
        {
            get { return ModifiedCIPAxis.PMMotorFluxSaturation.GetValue(3); }
            set
            {
                ModifiedCIPAxis.PMMotorFluxSaturation.SetValue(3, value);

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float PMMotorFluxSaturation4
        {
            get { return ModifiedCIPAxis.PMMotorFluxSaturation.GetValue(4); }
            set
            {
                ModifiedCIPAxis.PMMotorFluxSaturation.SetValue(4, value);

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float PMMotorFluxSaturation5
        {
            get { return ModifiedCIPAxis.PMMotorFluxSaturation.GetValue(5); }
            set
            {
                ModifiedCIPAxis.PMMotorFluxSaturation.SetValue(5, value);

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float PMMotorFluxSaturation6
        {
            get { return ModifiedCIPAxis.PMMotorFluxSaturation.GetValue(6); }
            set
            {
                ModifiedCIPAxis.PMMotorFluxSaturation.SetValue(6, value);

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float PMMotorFluxSaturation7
        {
            get { return ModifiedCIPAxis.PMMotorFluxSaturation.GetValue(7); }
            set
            {
                ModifiedCIPAxis.PMMotorFluxSaturation.SetValue(7, value);

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        //
        public float PMMotorForceConstant
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.PMMotorForceConstant); }
            set
            {
                ModifiedCIPAxis.PMMotorForceConstant = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float PMMotorLinearVoltageConstant
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.PMMotorLinearVoltageConstant); }
            set
            {
                ModifiedCIPAxis.PMMotorLinearVoltageConstant = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        //
        public float InductionMotorFluxCurrent
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.InductionMotorFluxCurrent); }
            set
            {
                ModifiedCIPAxis.InductionMotorFluxCurrent = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float InductionMotorRatedSlipSpeed
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.InductionMotorRatedSlipSpeed); }
            set
            {
                ModifiedCIPAxis.InductionMotorRatedSlipSpeed = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float InductionMotorStatorLeakageReactance
        {
            get
            {
                return
                    Convert.ToSingle(ModifiedCIPAxis.InductionMotorStatorLeakageReactance);
            }
            set
            {
                ModifiedCIPAxis.InductionMotorStatorLeakageReactance = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float InductionMotorRotorLeakageReactance
        {
            get
            {
                return
                    Convert.ToSingle(ModifiedCIPAxis.InductionMotorRotorLeakageReactance);
            }
            set
            {
                ModifiedCIPAxis.InductionMotorRotorLeakageReactance = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float InductionMotorStatorResistance
        {
            get
            {
                return
                    Convert.ToSingle(ModifiedCIPAxis.InductionMotorStatorResistance);
            }
            set
            {
                ModifiedCIPAxis.InductionMotorStatorResistance = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        #region Private

        // ReSharper disable once UnusedMember.Local
        private void UIVisibilityAndReadonly()
        {
            RaisePropertyChanged("RotaryPMModelVisibility");
            RaisePropertyChanged("LinearPMModelVisibility");
            RaisePropertyChanged("RotaryInductionModelVisibility");

            RaisePropertyChanged("MotorParameterEditEnabled");
        }

        private void UIRefresh()
        {
            RaisePropertyChanged("PMMotorTorqueConstant");
            RaisePropertyChanged("PMMotorRotaryVoltageConstant");
            RaisePropertyChanged("PMMotorResistance");
            RaisePropertyChanged("PMMotorInductance");
            RaisePropertyChanged("PMMotorFluxSaturation0");
            RaisePropertyChanged("PMMotorFluxSaturation1");
            RaisePropertyChanged("PMMotorFluxSaturation2");
            RaisePropertyChanged("PMMotorFluxSaturation3");
            RaisePropertyChanged("PMMotorFluxSaturation4");
            RaisePropertyChanged("PMMotorFluxSaturation5");
            RaisePropertyChanged("PMMotorFluxSaturation6");
            RaisePropertyChanged("PMMotorFluxSaturation7");
            RaisePropertyChanged("MotorParameterEditEnabled");

            RaisePropertyChanged(nameof(InductionMotorFluxCurrent));
            RaisePropertyChanged(nameof(InductionMotorRatedSlipSpeed));
            RaisePropertyChanged(nameof(InductionMotorStatorLeakageReactance));
            RaisePropertyChanged(nameof(InductionMotorRotorLeakageReactance));
            RaisePropertyChanged(nameof(InductionMotorStatorResistance));
        }

        #endregion
    }
}