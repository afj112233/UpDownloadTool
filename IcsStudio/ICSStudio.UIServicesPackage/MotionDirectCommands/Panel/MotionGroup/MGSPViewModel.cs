using System.Windows.Controls;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.Panel.MotionGroup
{
    internal sealed class MGSPViewModel : MotionGroupViewModel
    {
        public MGSPViewModel(UserControl panel,
            MotionDirectCommandsViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            MotionDirectCommand = MotionDirectCommand.MGSP;
        }
        
        public override IMessageRouterRequest GetExecuteCommandRequest()
        {
            Controller controller = SelectedTag.ParentController as Controller;
            if (controller == null)
                return null;

            int instanceId = controller.GetTagId(SelectedTag);

            return MotionDirectCommandHelper.MGSP(instanceId);
        }
    }
}
