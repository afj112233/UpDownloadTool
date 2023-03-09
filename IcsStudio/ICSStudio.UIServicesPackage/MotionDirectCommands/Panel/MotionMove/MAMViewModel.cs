using System.Windows.Controls;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.MotionDirectCommands.DataTypes;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.Panel.MotionMove
{
    internal sealed class MAMViewModel : AxisViewModel
    {
        private MAMParam _mamParam;

        public MAMViewModel(UserControl panel,
            MotionDirectCommandsViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            MAMParam = new MAMParam();
            MAMParam.OnDynamicPropertyChanged += OnReadOnlyChanged;

            MotionDirectCommand = MotionDirectCommand.MAM;
        }

        public override IMessageRouterRequest GetExecuteCommandRequest()
        {
            Controller controller = SelectedTag.ParentController as Controller;
            if (controller == null)
                return null;

            int instanceId = controller.GetTagId(SelectedTag);

            return MotionDirectCommandHelper.MAM(
                instanceId,
                (uint) MAMParam.MoveType,
                MAMParam.Position, MAMParam.Speed, (uint) MAMParam.SpeedUnits,
                MAMParam.AccelRate, (uint) MAMParam.AccelUnits,
                MAMParam.DecelRate, (uint) MAMParam.DecelUnits,
                (uint) MAMParam.Profile,
                MAMParam.AccelJerk, MAMParam.DecelJerk, (uint) MAMParam.JerkUnits,
                (uint) MAMParam.Merge, (uint) MAMParam.MergeSpeed,
                MAMParam.LockPosition, (uint) MAMParam.LockDirection,
                MAMParam.EventDistance, MAMParam.CalculatedData);
        }

        public MAMParam MAMParam
        {
            get { return _mamParam; }
            set
            {
                _mamParam = value;
                RaisePropertyChanged();
            }
        }
    }
}
