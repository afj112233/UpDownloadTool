using System.Windows.Controls;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.ManualTune.ViewModel;
using ICSStudio.UIServicesPackage.ManualTune.DataTypes;

namespace ICSStudio.UIServicesPackage.ManualTune.Panel.MotionMove
{
    internal sealed class MAMViewModel : AxisViewModel
    {
        private MAMParam _mamParam;

        public MAMViewModel(UserControl panel,
            MotionGeneratorViewModel parentViewModel,MAMParam mamParam)
            : base(panel, parentViewModel)
        {
            MAMParam = mamParam;
            MAMParam.OnDynamicPropertyChanged += OnReadOnlyChanged;

            MotionGeneratorCommand = MotionGeneratorCommand.MAM;
        }

        public override IMessageRouterRequest GetExecuteCommandRequest()
        {
            Controller controller = SelectedTag.ParentController as Controller;
            if (controller == null)
                return null;

            int instanceId = controller.GetTagId(SelectedTag);

            return MotionGeneratorHelper.MAM(
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
