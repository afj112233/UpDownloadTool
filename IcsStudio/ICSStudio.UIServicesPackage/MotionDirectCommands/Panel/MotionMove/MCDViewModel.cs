using System.Windows.Controls;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.MotionDirectCommands.DataTypes;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.Panel.MotionMove
{
    internal sealed class MCDViewModel : AxisViewModel
    {
        private MCDParam _mcdParam;

        public MCDViewModel(UserControl panel,
            MotionDirectCommandsViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            MCDParam = new MCDParam();
            MCDParam.OnDynamicPropertyChanged += OnReadOnlyChanged;

            MotionDirectCommand = MotionDirectCommand.MCD;
        }

        public MCDParam MCDParam
        {
            get { return _mcdParam; }
            set
            {
                _mcdParam = value;
                RaisePropertyChanged();
            }
        }
        
        public override IMessageRouterRequest GetExecuteCommandRequest()
        {
            Controller controller = SelectedTag.ParentController as Controller;
            if (controller == null)
                return null;

            int instanceId = controller.GetTagId(SelectedTag);
            return MotionDirectCommandHelper.MCD(
                instanceId,
                (uint) MCDParam.MotionType, (uint) MCDParam.ChangeSpeed, MCDParam.Speed,
                (uint) MCDParam.ChangeAccel, MCDParam.AccelRate,
                (uint) MCDParam.ChangeDecel, MCDParam.DecelRate,
                (uint) MCDParam.ChangeAccelJerk, MCDParam.AccelJerk,
                (uint) MCDParam.ChangeDecelJerk, MCDParam.DecelJerk,
                (uint) MCDParam.SpeedUnits, (uint) MCDParam.AccelUnits,
                (uint) MCDParam.DecelUnits, (uint) MCDParam.JerkUnits);
        }
    }
}
