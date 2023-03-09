using System.Windows.Controls;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.ManualTune.ViewModel;
using ICSStudio.UIServicesPackage.ManualTune.DataTypes;

namespace ICSStudio.UIServicesPackage.ManualTune.Panel.MotionMove
{
    internal sealed class MASViewModel : AxisViewModel
    {
        private MASParam _masParam;

        public MASViewModel(UserControl panel,
            MotionGeneratorViewModel parentViewModel,MASParam masParam)
            : base(panel, parentViewModel)
        {
            MASParam = masParam;
            MASParam.OnDynamicPropertyChanged += OnReadOnlyChanged;

            MotionGeneratorCommand = MotionGeneratorCommand.MAS;
        }

        public MASParam MASParam
        {
            get { return _masParam; }
            set
            {
                _masParam = value;
                RaisePropertyChanged();
            }
        }
        
        public override IMessageRouterRequest GetExecuteCommandRequest()
        {
            Controller controller = SelectedTag.ParentController as Controller;
            if (controller == null)
                return null;

            int instanceId = controller.GetTagId(SelectedTag);

            var stopType = (uint) MASParam.StopType;
            var decelRate = MASParam.DecelRate;
            var decelJerk = MASParam.DecelJerk;
            var jerkUnits = (uint) MASParam.JerkUnits;
            var decelUnits = (uint) MASParam.DecelUnits;
            var changeDecel = (uint) MASParam.ChangeDecel;
            var changeDecelJerk = (uint) MASParam.ChangeDecelJerk;

            // add other handle
            if (!((MASParam.StopType == MASParam.StopTypes.Jog)
                  || (MASParam.StopType == MASParam.StopTypes.Move)))
                changeDecelJerk = 0;

            return MotionGeneratorHelper.MAS(
                instanceId,
                stopType,
                decelRate, decelJerk,
                jerkUnits, decelUnits,
                changeDecel, changeDecelJerk);
        }
    }
}
