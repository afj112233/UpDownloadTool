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
    internal class LoadFeedbackViewModel : DefaultViewModel
    {
        private IList _feedback2TypeSource;

        public LoadFeedbackViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            CompareProperties = new[]
            {
                "Feedback2Type", "Feedback2Unit",
                "Feedback2CycleResolution", "Feedback2CycleInterpolation",
                "Feedback2StartupMethod", "Feedback2Length", "Feedback2Turns",
                "Feedback2AccelFilterBandwidth", "Feedback2AccelFilterTaps",
                "Feedback2VelocityFilterBandwidth", "Feedback2VelocityFilterTaps"
            };

            ParametersCommand = new RelayCommand(ExecuteParametersCommand);

            UpdateFeedback2TypeSource();
            Feedback2UnitSource = EnumHelper.ToDataSourceOrderByEnumMember<FeedbackUnitType>();
            StartupMethodSource = EnumHelper.ToDataSourceOrderByEnumMember<FeedbackStartupMethodType>();

        }

        public override void Show()
        {
            UpdateFeedback2TypeSource();

            UIVisibilityAndReadonly();
            UIRefresh();
        }

        public override Visibility Visibility
        {
            get
            {
                var feedbackConfiguration =
                    (FeedbackConfigurationType) Convert.ToByte(ParentViewModel.ModifiedAxisCIPDrive.CIPAxis
                        .FeedbackConfiguration);

                if (feedbackConfiguration == FeedbackConfigurationType.LoadFeedback
                    || feedbackConfiguration == FeedbackConfigurationType.DualFeedback)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public IList Feedback2TypeSource
        {
            get { return _feedback2TypeSource; }
            set { Set(ref _feedback2TypeSource, value); }
        }

        public FeedbackType Feedback2Type
        {
            get { return (FeedbackType) Convert.ToByte(ModifiedCIPAxis.Feedback2Type); }
            set
            {
                var feedback2Type = (FeedbackType) Convert.ToByte(ModifiedCIPAxis.Feedback2Type);
                var feedback2Unit = (FeedbackUnitType) Convert.ToByte(ModifiedCIPAxis.Feedback2Unit);

                if (feedback2Type != value)
                {
                    ModifiedCIPAxis.Feedback2Type = (byte) value;

                    AxisDefaultSetting.LoadDefaultFeedback2Setting(ModifiedCIPAxis, value, feedback2Unit);

                    UIVisibilityAndReadonly();

                    UIRefresh();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public IList Feedback2UnitSource { get; }

        public FeedbackUnitType Feedback2Unit
        {
            get { return (FeedbackUnitType) Convert.ToByte(ModifiedCIPAxis.Feedback2Unit); }
            set
            {
                var feedback2Unit = (FeedbackUnitType) Convert.ToByte(ModifiedCIPAxis.Feedback2Unit);
                if (feedback2Unit != value)
                {
                    ModifiedCIPAxis.Feedback2Unit = (byte) value;

                    UIVisibilityAndReadonly();
                    UIRefresh();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public Visibility FeedbackSettingVisibility =>
            Feedback2Type == FeedbackType.NotSpecified ? Visibility.Hidden : Visibility.Visible;

        public Visibility LengthVisibility
        {
            get
            {
                if (Feedback2StartupMethod == FeedbackStartupMethodType.Absolute &&
                    Feedback2Unit == FeedbackUnitType.Meter)
                    return Visibility.Visible;

                return Visibility.Hidden;
            }
        }

        public Visibility TurnsVisibility
        {
            get
            {
                if (Feedback2StartupMethod == FeedbackStartupMethodType.Absolute &&
                    Feedback2Unit == FeedbackUnitType.Rev)
                    return Visibility.Visible;

                return Visibility.Hidden;
            }
        }

        public uint Feedback2CycleResolution
        {
            get { return Convert.ToUInt32(ModifiedCIPAxis.Feedback2CycleResolution); }
            set
            {
                if (Convert.ToUInt32(ModifiedCIPAxis.Feedback2CycleResolution) != value)
                {
                    ModifiedCIPAxis.Feedback2CycleResolution = value;

                    UIRefresh();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public uint Feedback2CycleInterpolation
        {
            get { return Convert.ToUInt32(ModifiedCIPAxis.Feedback2CycleInterpolation); }
            set
            {
                if (Convert.ToUInt32(ModifiedCIPAxis.Feedback2CycleInterpolation) != value)
                {
                    ModifiedCIPAxis.Feedback2CycleInterpolation = value;

                    UIRefresh();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public ulong EffectiveResolution => (ulong) Feedback2CycleResolution * Feedback2CycleInterpolation;

        public IList StartupMethodSource { get; }

        public FeedbackStartupMethodType Feedback2StartupMethod
        {
            get
            {
                return
                    (FeedbackStartupMethodType) Convert.ToByte(ModifiedCIPAxis.Feedback2StartupMethod);
            }
            set
            {
                var feedback2StartupMethod =
                    (FeedbackStartupMethodType) Convert.ToByte(ModifiedCIPAxis.Feedback2StartupMethod);
                if (feedback2StartupMethod != value)
                {
                    ModifiedCIPAxis.Feedback2StartupMethod = (byte) value;

                    UIVisibilityAndReadonly();
                    UIRefresh();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public float Feedback2Length
        {
            get { return Convert.ToSingle(ModifiedCIPAxis.Feedback2Length); }
            set
            {
                if (Math.Abs(Convert.ToSingle(ModifiedCIPAxis.Feedback2Length) - value) > float.Epsilon)
                {
                    ModifiedCIPAxis.Feedback2Length = value;

                    UIRefresh();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public uint Feedback2Turns
        {
            get { return Convert.ToUInt32(ModifiedCIPAxis.Feedback2Turns); }
            set
            {
                if (Convert.ToUInt32(ModifiedCIPAxis.Feedback2Turns) != value)
                {
                    ModifiedCIPAxis.Feedback2Turns = value;

                    UIRefresh();
                }

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public RelayCommand ParametersCommand { get; }

        private void ExecuteParametersCommand()
        {
            ParentViewModel.ShowPanel("Parameter List", "Load Feedback");
        }

        #region Private

        private void UpdateFeedback2TypeSource()
        {
            List<FeedbackType> supportFeedbackTypes = null;
            var cipMotionDrive = ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule as CIPMotionDrive;

            if (cipMotionDrive != null)
                supportFeedbackTypes =
                    cipMotionDrive.GetSupportFeedback2Types(ParentViewModel.ModifiedAxisCIPDrive.AxisNumber);

            if (supportFeedbackTypes == null)
                supportFeedbackTypes = new List<FeedbackType>();

            if (!supportFeedbackTypes.Contains(FeedbackType.NotSpecified))
                supportFeedbackTypes.Insert(0, FeedbackType.NotSpecified);

            var oldFeedback2Type = Feedback2Type;

            Feedback2TypeSource = EnumHelper.ToDataSource<FeedbackType>(supportFeedbackTypes);

            if (!supportFeedbackTypes.Contains(oldFeedback2Type))
                ModifiedCIPAxis.Feedback2Type = (byte) supportFeedbackTypes[0];
            else
                ModifiedCIPAxis.Feedback2Type = (byte) oldFeedback2Type;

            RaisePropertyChanged("Feedback2Type");
        }

        private void UIVisibilityAndReadonly()
        {
            RaisePropertyChanged("FeedbackSettingVisibility");
            RaisePropertyChanged("LengthVisibility");
            RaisePropertyChanged("TurnsVisibility");
        }

        private void UIRefresh()
        {
            RaisePropertyChanged("Feedback2Type");
            RaisePropertyChanged("Feedback2Unit");

            RaisePropertyChanged("Feedback2CycleResolution");
            RaisePropertyChanged("Feedback2CycleInterpolation");
            RaisePropertyChanged("EffectiveResolution");
            RaisePropertyChanged("Feedback2StartupMethod");
            RaisePropertyChanged("Feedback2Length");
            RaisePropertyChanged("Feedback2Turns");
        }

        #endregion
    }
}