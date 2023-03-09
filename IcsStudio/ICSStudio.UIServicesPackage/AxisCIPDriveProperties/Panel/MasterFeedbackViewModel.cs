using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Utils;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class MasterFeedbackViewModel : DefaultViewModel
    {
        private IList _feedback1TypeSource;

        public MasterFeedbackViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            CompareProperties = new[]
            {
                "Feedback1Type", "Feedback1Unit",
                "Feedback1CycleResolution", "Feedback1CycleInterpolation",
                "Feedback1StartupMethod", "Feedback1Length", "Feedback1Turns",
                "Feedback1AccelFilterBandwidth", "Feedback1AccelFilterTaps",
                "Feedback1VelocityFilterBandwidth", "Feedback1VelocityFilterTaps"
            };

            ParametersCommand = new RelayCommand(ExecuteParametersCommand);

            UpdateFeedback1TypeSource();
            Feedback1UnitSource = EnumHelper.ToDataSourceOrderByEnumMember<FeedbackUnitType>();
            StartupMethodSource = EnumHelper.ToDataSourceOrderByEnumMember<FeedbackStartupMethodType>();
        }

        public override void Show()
        {
            UpdateFeedback1TypeSource();

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

                if (axisConfiguration == AxisConfigurationType.FeedbackOnly)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public IList Feedback1TypeSource
        {
            get { return _feedback1TypeSource; }
            set { Set(ref _feedback1TypeSource, value); }
        }

        public FeedbackType Feedback1Type
        {
            get { return (FeedbackType) Convert.ToByte(ModifiedCIPAxis.Feedback1Type); }
            set
            {
                var feedback1Type = (FeedbackType) Convert.ToByte(ModifiedCIPAxis.Feedback1Type);
                var feedback1Unit = (FeedbackUnitType) Convert.ToByte(ModifiedCIPAxis.Feedback1Unit);
                if (feedback1Type != value)
                {
                    ModifiedCIPAxis.Feedback1Type = (byte) value;

                    AxisDefaultSetting.LoadDefaultFeedback1Setting(ModifiedCIPAxis, value, feedback1Unit);

                    UIVisibilityAndReadonly();

                    UIRefresh();
                }

                CheckDirty();
                RaisePropertyChanged();

            }
        }

        public IList Feedback1UnitSource { get; }

        public FeedbackUnitType Feedback1Unit
        {
            get { return (FeedbackUnitType) Convert.ToByte(ModifiedCIPAxis.Feedback1Unit); }
            set
            {
                var feedback1Unit = (FeedbackUnitType) Convert.ToByte(ModifiedCIPAxis.Feedback1Unit);
                if (feedback1Unit != value)
                {
                    ModifiedCIPAxis.Feedback1Unit = (byte) value;

                    switch (value)
                    {
                        case FeedbackUnitType.Rev:
                            ModifiedCIPAxis.LoadType = (byte) LoadType.DirectRotary;
                            break;
                        case FeedbackUnitType.Meter:
                            ModifiedCIPAxis.LoadType = (byte) LoadType.DirectLinear;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(value), value, null);
                    }

                    UIVisibilityAndReadonly();

                    UIRefresh();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public Visibility FeedbackSettingVisibility =>
            Feedback1Type == FeedbackType.NotSpecified ? Visibility.Hidden : Visibility.Visible;

        public Visibility LengthVisibility
        {
            get
            {
                if ((Feedback1StartupMethod == FeedbackStartupMethodType.Absolute) &&
                    (Feedback1Unit == FeedbackUnitType.Meter))
                    return Visibility.Visible;

                return Visibility.Hidden;
            }
        }

        public Visibility TurnsVisibility
        {
            get
            {
                if ((Feedback1StartupMethod == FeedbackStartupMethodType.Absolute) &&
                    (Feedback1Unit == FeedbackUnitType.Rev))
                    return Visibility.Visible;

                return Visibility.Hidden;
            }
        }

        public uint Feedback1CycleResolution
        {
            get { return Convert.ToUInt32(ModifiedCIPAxis.Feedback1CycleResolution); }
            set
            {
                if (Convert.ToUInt32(ModifiedCIPAxis.Feedback1CycleResolution) != value)
                {
                    ModifiedCIPAxis.Feedback1CycleResolution = value;

                    UIRefresh();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public uint Feedback1CycleInterpolation
        {
            get { return Convert.ToUInt32(ModifiedCIPAxis.Feedback1CycleInterpolation); }
            set
            {
                if (Convert.ToUInt32(ModifiedCIPAxis.Feedback1CycleInterpolation) != value)
                {
                    ModifiedCIPAxis.Feedback1CycleInterpolation = value;

                    UIRefresh();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public ulong EffectiveResolution => (ulong) Feedback1CycleResolution * Feedback1CycleInterpolation;
        public IList StartupMethodSource { get; }

        public FeedbackStartupMethodType Feedback1StartupMethod
        {
            get
            {
                return
                    (FeedbackStartupMethodType) Convert.ToByte(ModifiedCIPAxis.Feedback1StartupMethod);
            }
            set
            {
                var feedback1StartupMethod =
                    (FeedbackStartupMethodType) Convert.ToByte(ModifiedCIPAxis.Feedback1StartupMethod);
                if (feedback1StartupMethod != value)
                {
                    ModifiedCIPAxis.Feedback1StartupMethod = (byte) value;

                    UIVisibilityAndReadonly();
                    UIRefresh();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float Feedback1Length
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.Feedback1Length); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedCIPAxis.Feedback1Length) - value) > float.Epsilon)
                {
                    ModifiedCIPAxis.Feedback1Length = value;

                    UIRefresh();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public uint Feedback1Turns
        {
            get { return Convert.ToUInt32(ModifiedCIPAxis.Feedback1Turns); }
            set
            {
                if (Convert.ToUInt32(ModifiedCIPAxis.Feedback1Turns) != value)
                {
                    ModifiedCIPAxis.Feedback1Turns = value;

                    UIRefresh();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public RelayCommand ParametersCommand { get; }

        private void ExecuteParametersCommand()
        {
            ParentViewModel.ShowPanel("Parameter List", "Master Feedback");
        }

        #region Private

        private void UpdateFeedback1TypeSource()
        {
            List<FeedbackType> supportFeedbackTypes = null;
            var cipMotionDrive = ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule as CIPMotionDrive;

            if (cipMotionDrive != null)
                supportFeedbackTypes =
                    cipMotionDrive.GetSupportFeedback1Types(ParentViewModel.ModifiedAxisCIPDrive.AxisNumber);

            if (supportFeedbackTypes == null)
                supportFeedbackTypes = new List<FeedbackType>();

            if (!supportFeedbackTypes.Contains(FeedbackType.NotSpecified))
                supportFeedbackTypes.Insert(0, FeedbackType.NotSpecified);

            var oldFeedback1Type = Feedback1Type;

            Feedback1TypeSource = EnumHelper.ToDataSource<FeedbackType>(supportFeedbackTypes);

            if (!supportFeedbackTypes.Contains(oldFeedback1Type))
                ModifiedCIPAxis.Feedback1Type = (byte) supportFeedbackTypes[0];
            else
                ModifiedCIPAxis.Feedback1Type = (byte) oldFeedback1Type;

            RaisePropertyChanged("Feedback1Type");
        }

        private void UIVisibilityAndReadonly()
        {
            RaisePropertyChanged("FeedbackSettingVisibility");
            RaisePropertyChanged("LengthVisibility");
            RaisePropertyChanged("TurnsVisibility");
        }

        private void UIRefresh()
        {
            RaisePropertyChanged("Feedback1Type");
            RaisePropertyChanged("Feedback1Unit");

            RaisePropertyChanged("Feedback1CycleResolution");
            RaisePropertyChanged("Feedback1CycleInterpolation");
            RaisePropertyChanged("EffectiveResolution");
            RaisePropertyChanged("Feedback1StartupMethod");
            RaisePropertyChanged("Feedback1Length");
            RaisePropertyChanged("Feedback1Turns");
        }

        #endregion
    }
}