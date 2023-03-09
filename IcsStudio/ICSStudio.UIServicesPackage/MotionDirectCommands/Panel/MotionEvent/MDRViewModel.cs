using System.Windows.Controls;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.MotionDirectCommands.DataTypes;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.Panel.MotionEvent
{
    internal sealed class MDRViewModel : AxisViewModel
    {
        private MDRParam _mdrParam;

        public MDRViewModel(UserControl panel,
            MotionDirectCommandsViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            MDRParam = new MDRParam();
            MotionDirectCommand = MotionDirectCommand.MDR;
        }

        public MDRParam MDRParam
        {
            get { return _mdrParam; }
            set
            {
                _mdrParam = value;
                RaisePropertyChanged();
            }
        }

        public override IMessageRouterRequest GetExecuteCommandRequest()
        {
            Controller controller = SelectedTag.ParentController as Controller;
            if (controller == null)
                return null;

            int instanceId = controller.GetTagId(SelectedTag);
            return MotionDirectCommandHelper.MDR(instanceId, (uint) MDRParam.InputNumber);
        }
    }
}
