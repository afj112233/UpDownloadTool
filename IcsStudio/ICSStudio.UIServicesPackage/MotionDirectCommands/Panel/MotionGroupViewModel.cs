using System.Windows.Controls;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.Panel
{
    internal class MotionGroupViewModel : DefaultViewModel
    {
        private ITag _selectedTag;

        protected MotionGroupViewModel(UserControl panel, MotionDirectCommandsViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            AllTags = ParentViewModel.AllMotionGroups;
        }

        public sealed override ITag SelectedTag
        {
            get
            {
                _selectedTag = ParentViewModel.SelectedMotionGroup;
                return _selectedTag;
            }
            protected set
            {
                ParentViewModel.SelectedMotionGroup = value;

                Set(ref _selectedTag, value);
            }
        }

        public override bool CanExecute()
        {
            if (SelectedTag != null && SelectedTag.ParentController.IsOnline)
                return true;

            return false;
        }

        public override IMessageRouterRequest GetQueryCommandRequest()
        {
            Controller controller = SelectedTag.ParentController as Controller;
            if (controller == null)
                return null;

            int instanceId = controller.GetTagId(SelectedTag);

            return MotionDirectCommandHelper.QueryCommandRequest(
                (ushort) CipObjectClassCode.MotionGroup, instanceId, MotionDirectCommand);

        }
    }
}
