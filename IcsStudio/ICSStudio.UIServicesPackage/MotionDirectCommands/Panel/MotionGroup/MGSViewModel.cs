using System.Windows.Controls;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.MotionDirectCommands.DataTypes;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.Panel.MotionGroup
{
    internal sealed class MGSViewModel : MotionGroupViewModel
    {
        private MGSParam _mgsParam;

        public MGSViewModel(UserControl panel,
            MotionDirectCommandsViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            MGSParam = new MGSParam();
            MotionDirectCommand = MotionDirectCommand.MGS;
        }

        public MGSParam MGSParam
        {
            get { return _mgsParam; }
            set
            {
                _mgsParam = value;
                RaisePropertyChanged();
            }
        }

        public override IMessageRouterRequest GetExecuteCommandRequest()
        {
            Controller controller = SelectedTag.ParentController as Controller;
            if (controller == null)
                return null;

            int instanceId = controller.GetTagId(SelectedTag);
            return MotionDirectCommandHelper.MGS(instanceId, (uint) MGSParam.StopMode);
        }
    }
}
