using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.Cip.Objects;
using ICSStudio.SimpleServices.DataWrapper;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class PolarityViewModel : DefaultViewModel
    {
        public PolarityViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            CompareProperties = new[]
            {
                "MotionPolarity", "MotorPolarity", "Feedback1Polarity", "Feedback2Polarity"
            };

            //TODO(gjc): need edit later
            PeriodicRefreshProperties = new[]
            {
                "MotionPolarity"
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
                    (AxisConfigurationType)Convert.ToByte(ParentViewModel.ModifiedAxisCIPDrive.CIPAxis
                        .AxisConfiguration);

                if (axisConfiguration == AxisConfigurationType.ConverterOnly)
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        public PolarityType MotionPolarity
        {
            get { return (PolarityType)Convert.ToByte(ModifiedCIPAxis.MotionPolarity); }
            set
            {
                ModifiedCIPAxis.MotionPolarity = (byte)value;

                ParentViewModel.AddEditPropertyName(nameof(MotionPolarity));

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public Visibility MotorPolarityVisibility
        {
            get
            {
                var motorType = (MotorType)Convert.ToByte(ModifiedCIPAxis.MotorType);

                if (motorType == MotorType.NotSpecified)
                    return Visibility.Hidden;

                return Visibility.Visible;
            }
        }

        public PolarityType MotorPolarity
        {
            get { return (PolarityType)Convert.ToByte(ModifiedCIPAxis.MotorPolarity); }
            set
            {
                ModifiedCIPAxis.MotorPolarity = (byte)value;

                ParentViewModel.AddEditPropertyName(nameof(MotorPolarity));

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public Visibility Feedback1PolarityVisibility
        {
            get
            {
                if (ParentViewModel.ModifiedAxisCIPDrive.SupportAttribute("Feedback1Polarity"))
                {
                    var feedback1Type = (FeedbackType)Convert.ToByte(ModifiedCIPAxis.Feedback1Type);
                    if (feedback1Type != FeedbackType.NotSpecified)
                        return Visibility.Visible;
                }

                return Visibility.Hidden;
            }
        }

        public PolarityType Feedback1Polarity
        {
            get { return (PolarityType)Convert.ToByte(ModifiedCIPAxis.Feedback1Polarity); }
            set
            {
                ModifiedCIPAxis.Feedback1Polarity = (byte)value;

                ParentViewModel.AddEditPropertyName(nameof(Feedback1Polarity));

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public Visibility Feedback2PolarityVisibility
        {
            get
            {
                if (ParentViewModel.ModifiedAxisCIPDrive.SupportAttribute("Feedback2Polarity"))
                {
                    var feedback2Type = (FeedbackType)Convert.ToByte(ModifiedCIPAxis.Feedback2Type);
                    var feedbackConfiguration =
                        (FeedbackConfigurationType)Convert.ToByte(ModifiedCIPAxis.FeedbackConfiguration);

                    if (feedback2Type != FeedbackType.NotSpecified &&
                        (feedbackConfiguration == FeedbackConfigurationType.LoadFeedback ||
                         feedbackConfiguration == FeedbackConfigurationType.DualFeedback))
                        return Visibility.Visible;
                }

                return Visibility.Hidden;
            }
        }

        public PolarityType Feedback2Polarity
        {
            get { return (PolarityType)Convert.ToByte(ModifiedCIPAxis.Feedback2Polarity); }
            set
            {
                ModifiedCIPAxis.Feedback2Polarity = (byte)value;

                ParentViewModel.AddEditPropertyName(nameof(Feedback2Polarity));

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public bool IsPolarityEnabled
        {
            get
            {
                if (ParentViewModel.IsPowerStructureEnabled)
                    return false;

                if (ParentViewModel.IsHardRunMode)
                    return false;

                return true;
            }
        }

        private void UIVisibilityAndReadonly()
        {
            RaisePropertyChanged("MotorPolarityVisibility");
            RaisePropertyChanged("Feedback1PolarityVisibility");
            RaisePropertyChanged("Feedback2PolarityVisibility");

            RaisePropertyChanged(nameof(IsPolarityEnabled));
        }

        private void UIRefresh()
        {
            RaisePropertyChanged("MotionPolarity");
            RaisePropertyChanged("MotorPolarity");
            RaisePropertyChanged("Feedback1Polarity");
            RaisePropertyChanged("Feedback2Polarity");
        }
    }
}