using System.Windows.Controls;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.ManualTune.ViewModel;
using ICSStudio.UIServicesPackage.ManualTune.DataTypes;

namespace ICSStudio.UIServicesPackage.ManualTune.Panel.MotionState
{
    internal sealed class MDSViewModel : AxisViewModel
    {
        private MDSParam _mdsParam;

        public MDSViewModel(UserControl panel,
            MotionGeneratorViewModel parentViewModel,MDSParam mdsParam)
            : base(panel, parentViewModel)
        {
            MDSParam = mdsParam;
            MotionGeneratorCommand = MotionGeneratorCommand.MDS;
        }
        
        public override IMessageRouterRequest GetExecuteCommandRequest()
        {
            Controller controller = SelectedTag.ParentController as Controller;
            if (controller == null)
                return null;

            int instanceId = controller.GetTagId(SelectedTag);

            return MotionGeneratorHelper.MDS(instanceId, MDSParam.Speed, (uint) MDSParam.SpeedUnits);
        }

        public MDSParam MDSParam
        {
            get { return _mdsParam; }
            set
            {
                _mdsParam = value;
                RaisePropertyChanged();
            }
        }

    }
}
