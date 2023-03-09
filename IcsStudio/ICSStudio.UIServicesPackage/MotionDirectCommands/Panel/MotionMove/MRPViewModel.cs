using System.Windows.Controls;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.MotionDirectCommands.DataTypes;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.Panel.MotionMove
{
    internal sealed class MRPViewModel : AxisViewModel
    {
        private MRPParam _mrpParam;

        public MRPViewModel(UserControl panel,
            MotionDirectCommandsViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            MRPParam = new MRPParam();
            MotionDirectCommand = MotionDirectCommand.MRP;
        }

        public MRPParam MRPParam
        {
            get { return _mrpParam; }
            set
            {
                _mrpParam = value;
                RaisePropertyChanged();
            }
        }

        public override IMessageRouterRequest GetExecuteCommandRequest()
        {
            Controller controller = SelectedTag.ParentController as Controller;
            if (controller == null)
                return null;

            int instanceId = controller.GetTagId(SelectedTag);
            return MotionDirectCommandHelper.MRP(
                instanceId,
                MRPParam.Position, (uint) MRPParam.Type, (uint) MRPParam.PositionSelect);
        }
    }
}
