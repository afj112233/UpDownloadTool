using System.Windows.Controls;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.ManualTune.ViewModel;

namespace ICSStudio.UIServicesPackage.ManualTune.Panel.MotionMove
{
    internal sealed class MAHViewModel : AxisViewModel
    {
        public MAHViewModel(UserControl panel,
            MotionGeneratorViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            MotionGeneratorCommand = MotionGeneratorCommand.MAH;
        }

        public override IMessageRouterRequest GetExecuteCommandRequest()
        {
            Controller controller = SelectedTag.ParentController as Controller;
            if (controller == null)
                return null;

            int instanceId = controller.GetTagId(SelectedTag);

            return MotionGeneratorHelper.MAH(instanceId);
        }
    }
}
