using System.Windows;
using System.Windows.Controls;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.Panel
{
    internal class AxisViewModel : DefaultViewModel
    {
        private ITag _selectedTag;

        protected AxisViewModel(UserControl panel, MotionDirectCommandsViewModel parentViewModel) : base(panel,
            parentViewModel)
        {
            AllTags = ParentViewModel.AllAxises;
            SelectedTag = ParentViewModel.SelectedAxis;
        }

        public sealed override ITag SelectedTag
        {
            get
            {
                _selectedTag = ParentViewModel.SelectedAxis;
                return _selectedTag;
            }
            protected set
            {
                ParentViewModel.SelectedAxis = value;

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
                (ushort) CipObjectClassCode.Axis, instanceId, MotionDirectCommand);

        }

        private bool _isPropertyGridReadOnly;

        public bool IsPropertyGridReadOnly
        {
            get { return _isPropertyGridReadOnly; }
            set { Set(ref _isPropertyGridReadOnly, value); }
        }

        protected void OnReadOnlyChanged(object obj)
        {
            // refresh
            IsPropertyGridReadOnly = true;
            IsPropertyGridReadOnly = false;
        }
    }
}
