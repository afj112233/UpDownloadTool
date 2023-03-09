using System.Windows;
using System.Windows.Controls;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.MotionDirectCommands.DataTypes;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.Panel.MotionMove
{
    internal sealed class MAJViewModel : AxisViewModel
    {
        private MAJParam _majParam;

        public MAJViewModel(UserControl panel,
            MotionDirectCommandsViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            MAJParam = new MAJParam();
            MAJParam.OnDynamicPropertyChanged += OnReadOnlyChanged;

            MotionDirectCommand = MotionDirectCommand.MAJ;
        }

        public override IMessageRouterRequest GetExecuteCommandRequest()
        {
            Controller controller = SelectedTag.ParentController as Controller;
            if (controller == null)
                return null;

            int instanceId = controller.GetTagId(SelectedTag);
            return MotionDirectCommandHelper.MAJ(
                instanceId,
                (uint) MAJParam.Direction,
                MAJParam.Speed, (uint) MAJParam.SpeedUnits,
                MAJParam.AccelRate, (uint) MAJParam.AccelUnits,
                MAJParam.DecelRate, (uint) MAJParam.DecelUnits,
                (uint) MAJParam.Profile,
                MAJParam.AccelJerk, MAJParam.DecelJerk,
                (uint) MAJParam.JerkUnits,
                (uint) MAJParam.Merge, (uint) MAJParam.MergeSpeed,
                MAJParam.LockPosition, (uint) MAJParam.LockDirection);
        }

        public MAJParam MAJParam
        {
            get { return _majParam; }
            set
            {
                _majParam = value;
                RaisePropertyChanged();
            }
        }
    }
}
