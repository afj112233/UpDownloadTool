using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class TagViewModel : DefaultViewModel
    {

        public TagViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {

        }

        public bool IsEnabled => !ParentViewModel.Controller.IsOnline;

        public override void Show()
        {
            RaisePropertyChanged(nameof(IsEnabled));

            UIRefresh();
        }

        protected override bool PropertiesChanged()
        {
            bool result = base.PropertiesChanged();
            if (result)
                return true;

            if (!string.Equals(ParentViewModel.AxisTag.Name, ParentViewModel.ModifiedTagName))
                return true;

            if (!string.Equals(ParentViewModel.AxisTag.Description, ParentViewModel.ModifiedDescription))
                return true;

            return false;
        }

        public override int CheckValid()
        {
            int result = 0;
            if (!string.Equals(ParentViewModel.AxisTag.Name, ParentViewModel.ModifiedTagName))
            {
                if (!Checking.CheckTagName(
                        ParentViewModel.ModifiedTagName,
                        ParentViewModel.Controller,
                        Checking.CheckModeType.ChangeTagName))
                {
                    // show page
                    ParentViewModel.ShowPanel("Tag");

                    result = -1;
                }
            }

            return result;
        }

        public string TagName
        {
            get { return ParentViewModel.ModifiedTagName; }
            set
            {
                ParentViewModel.ModifiedTagName = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public string Scope
        {
            get { return Controller.GetInstance().Name; }
            set
            {
                ParentViewModel.Title = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        public string ExternalAccess => ParentViewModel.AxisTag.ExternalAccess.ToString();

        public bool IsTagNameEnabled
        {
            get
            {
                if (ParentViewModel.IsHardRunMode)
                    return false;

                return true;
            }
        }

        public string Description
        {
            get { return ParentViewModel.ModifiedDescription; }
            set
            {
                ParentViewModel.ModifiedDescription = value;

                CheckDirty();
                RaisePropertyChanged();
            }
        }

        private void UIRefresh()
        {
            RaisePropertyChanged(nameof(TagName));
            RaisePropertyChanged(nameof(Description));

            RaisePropertyChanged(nameof(IsTagNameEnabled));
        }
    }
}