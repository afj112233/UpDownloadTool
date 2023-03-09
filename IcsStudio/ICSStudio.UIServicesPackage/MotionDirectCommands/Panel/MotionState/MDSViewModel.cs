using System.Windows.Controls;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.MotionDirectCommands.DataTypes;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.Panel.MotionState
{
    internal sealed class MDSViewModel : AxisViewModel
    {
        private MDSParam _mdsParam;

        public MDSViewModel(UserControl panel,
            MotionDirectCommandsViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            MDSParam = new MDSParam();
            MotionDirectCommand = MotionDirectCommand.MDS;
        }
        
        public override IMessageRouterRequest GetExecuteCommandRequest()
        {
            Controller controller = SelectedTag.ParentController as Controller;
            if (controller == null)
                return null;

            int instanceId = controller.GetTagId(SelectedTag);

            return MotionDirectCommandHelper.MDS(instanceId, MDSParam.Speed, (uint) MDSParam.SpeedUnits);
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
