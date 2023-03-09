using System.Windows.Controls;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.MotionDirectCommands.DataTypes;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.Panel.MotionEvent
{
    internal sealed class MAWViewModel : AxisViewModel
    {
        private MAWParam _mawParam;

        public MAWViewModel(UserControl panel,
            MotionDirectCommandsViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            MAWParam = new MAWParam();
            MotionDirectCommand = MotionDirectCommand.MAW;
        }

        public MAWParam MAWParam
        {
            get { return _mawParam; }
            set
            {
                _mawParam = value;
                RaisePropertyChanged();
            }
        }

        public override IMessageRouterRequest GetExecuteCommandRequest()
        {
            Controller controller = SelectedTag.ParentController as Controller;
            if (controller == null)
                return null;

            int instanceId = controller.GetTagId(SelectedTag);
            return MotionDirectCommandHelper.MAW(instanceId, MAWParam.Position, (uint) MAWParam.TriggerCondition);
        }
    }
}
