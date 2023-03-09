using System.Windows.Controls;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.MotionDirectCommands.DataTypes;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.Panel.MotionState
{
    internal sealed class MDOViewModel : AxisViewModel
    {
        private MDOParam _mdoParam;

        public MDOViewModel(UserControl panel,
            MotionDirectCommandsViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            MDOParam = new MDOParam();
            MotionDirectCommand = MotionDirectCommand.MDO;
        }

        public override IMessageRouterRequest GetExecuteCommandRequest()
        {
            Controller controller = SelectedTag.ParentController as Controller;
            if (controller == null)
                return null;

            int instanceId = controller.GetTagId(SelectedTag);
            return MotionDirectCommandHelper.MDO(instanceId, MDOParam.DriveOutput, (uint) MDOParam.DriveUnits);
        }

        public MDOParam MDOParam
        {
            get { return _mdoParam; }
            set
            {
                _mdoParam = value;
                RaisePropertyChanged();
            }
        }
    }
}
