using System.Windows.Controls;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.MotionDirectCommands.DataTypes;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.Panel.MotionMove
{
    internal sealed class MAGViewModel : AxisViewModel
    {
        private MAGParam _magParam;

        private ITag _masterAxis;

        public MAGViewModel(UserControl panel,
            MotionDirectCommandsViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            MAGParam = new MAGParam();
            MAGParam.OnDynamicPropertyChanged += OnReadOnlyChanged;

            MotionDirectCommand = MotionDirectCommand.MAG;
        }

        public MAGParam MAGParam
        {
            get { return _magParam; }
            set
            {
                _magParam = value;
                RaisePropertyChanged();
            }
        }

        public ITag MasterAxis
        {
            get { return _masterAxis; }
            set { Set(ref _masterAxis, value); }
        }
        
        public override void LoadOptions()
        {
            base.LoadOptions();

            MasterAxis = SelectedTag;
        }

        public override IMessageRouterRequest GetExecuteCommandRequest()
        {
            Controller controller = SelectedTag.ParentController as Controller;
            if (controller == null)
                return null;

            int slaveInstanceId = controller.GetTagId(SelectedTag);
            int masterInstanceId = controller.GetTagId(MasterAxis);

            return MotionDirectCommandHelper.MAG(
                slaveInstanceId, masterInstanceId,
                (uint) MAGParam.Direction, MAGParam.Ratio,
                MAGParam.SlaveCounts, MAGParam.MasterCounts,
                MAGParam.AccelRate,
                (uint) MAGParam.MasterReference, (uint) MAGParam.RatioFormat,
                (uint) MAGParam.Clutch, (uint) MAGParam.AccelUnits);
        }
    }
}