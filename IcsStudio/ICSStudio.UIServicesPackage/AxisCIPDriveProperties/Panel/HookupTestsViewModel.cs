using System;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.Cip.Objects;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    internal class HookupTestsViewModel : DefaultViewModel
    {
        public HookupTestsViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
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

                if (ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule == null)
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        public Visibility MotorAndFeedbackVisibility { get; set; }
        public Visibility MotorFeedbackVisibility { get; set; }
        public Visibility MasterFeedbackVisibility { get; set; }
        public Visibility MarkerVisibility { get; set; }
    }
}