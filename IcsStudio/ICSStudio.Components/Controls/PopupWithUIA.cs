using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ICSStudio.Components.Controls
{
    // ReSharper disable once InconsistentNaming
    public class PopupWithUIA : Popup
    {
        static PopupWithUIA()
        {
            AutomationProperties.NameProperty.OverrideMetadata(typeof(PopupWithUIA),
                new FrameworkPropertyMetadata((sender, e) =>
                {
                    PopupWithUIA popupWithUia = (PopupWithUIA) sender;
                    if (popupWithUia == null || !popupWithUia.IsOpen)
                        return;
                    popupWithUia.SetAutomationNameToLogicalChild();
                }));
        }

        public PopupWithUIA()
        {
            Opened += PopupWithUIA_Opened;
        }

        private void PopupWithUIA_Opened(object sender, EventArgs e)
        {
            SetAutomationNameToLogicalChild();
        }

        private void SetAutomationNameToLogicalChild()
        {
            GetVisualParentRoot(Child)?.SetValue(AutomationProperties.NameProperty,
                GetValue(AutomationProperties.NameProperty));
        }

        private Visual GetVisualParentRoot(Visual child)
        {
            Visual visual = child;

            while (visual != null)
            {
                Visual parent = VisualTreeHelper.GetParent(visual) as Visual;
                if (parent == null)
                    break;

                visual = parent;

            }

            return visual;
        }
    }
}
