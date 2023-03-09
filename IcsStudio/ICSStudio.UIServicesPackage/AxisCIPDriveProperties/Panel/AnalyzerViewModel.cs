using System;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.Cip.Objects;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    internal class AnalyzerViewModel : DefaultViewModel
    {
        public AnalyzerViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
        }

        public override void Show()
        {
            
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

                if (ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule == null)
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }
    }
}