using System.Windows;
using System.Windows.Controls;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.ManualTune.ViewModel;
using ICSStudio.UIServicesPackage.ManualTune.DataTypes;

namespace ICSStudio.UIServicesPackage.ManualTune.Panel.MotionMove
{
    internal sealed class MAJViewModel : AxisViewModel
    {
        private MAJParam _majParam;

        public MAJViewModel(UserControl panel,
            MotionGeneratorViewModel parentViewModel,MAJParam majParam)
            : base(panel, parentViewModel)
        {
            MAJParam = majParam;
            MAJParam.OnDynamicPropertyChanged += OnReadOnlyChanged;

            MotionGeneratorCommand = MotionGeneratorCommand.MAJ;
        }

        public override IMessageRouterRequest GetExecuteCommandRequest()
        {
            Controller controller = SelectedTag.ParentController as Controller;
            if (controller == null)
                return null;

            int instanceId = controller.GetTagId(SelectedTag);
            return MotionGeneratorHelper.MAJ(
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
