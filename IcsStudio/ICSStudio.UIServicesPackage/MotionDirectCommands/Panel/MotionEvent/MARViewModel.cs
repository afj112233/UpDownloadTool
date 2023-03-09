using System.Windows.Controls;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.MotionDirectCommands.DataTypes;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.Panel.MotionEvent
{
    internal sealed class MARViewModel : AxisViewModel
    {
        private MARParam _marParam;

        public MARViewModel(UserControl panel,
            MotionDirectCommandsViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            MARParam = new MARParam();
            MARParam.OnDynamicPropertyChanged += OnReadOnlyChanged;

            MotionDirectCommand = MotionDirectCommand.MAR;
        }

        public MARParam MARParam
        {
            get { return _marParam; }
            set
            {
                _marParam = value;
                RaisePropertyChanged();
            }
        }
        
        public override IMessageRouterRequest GetExecuteCommandRequest()
        {
            Controller controller = SelectedTag.ParentController as Controller;
            if (controller == null)
                return null;

            int instanceId = controller.GetTagId(SelectedTag);
            return MotionDirectCommandHelper.MAR(
                instanceId,
                (uint) MARParam.InputNumber, (uint) MARParam.TriggerCondition,
                MARParam.MinPosition, MARParam.MaxPosition,
                (uint) MARParam.WindowedRegistration);
        }
    }
}
