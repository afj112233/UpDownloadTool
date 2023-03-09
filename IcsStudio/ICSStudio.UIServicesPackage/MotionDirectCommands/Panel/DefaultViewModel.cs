using System.Collections.ObjectModel;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Interfaces.Tags;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.Panel
{
    internal class DefaultViewModel : ViewModelBase, IMotionDirectCommandPanel
    {
        protected readonly MotionDirectCommandsViewModel ParentViewModel;

        private ObservableCollection<ITag> _allTags;

        public DefaultViewModel(UserControl panel,
            MotionDirectCommandsViewModel parentViewModel)
        {
            Control = panel;
            panel.DataContext = this;

            ParentViewModel = parentViewModel;
            MotionDirectCommand = MotionDirectCommand.NullCommand;
        }

        public object Owner { get; set; }
        public object Control { get; }

        public virtual void LoadOptions()
        {
        }

        public virtual bool SaveOptions()
        {
            return true;
        }

        public virtual void Show()
        {
            // ReSharper disable ExplicitCallerInfoArgument
            RaisePropertyChanged("SelectedTag");
            // ReSharper restore ExplicitCallerInfoArgument
        }

        public virtual bool CanExecute()
        {
            return false;
        }

        public MotionDirectCommand MotionDirectCommand { get; protected set; }

        public virtual IMessageRouterRequest GetQueryCommandRequest()
        {
            return null;
        }

        public virtual IMessageRouterRequest GetExecuteCommandRequest()
        {
            return null;
        }

        public RelayCommand<ITag> AxisPropertiesCommand => ParentViewModel.AxisPropertiesCommand;
        public RelayCommand<ITag> MotionGroupPropertiesCommand => ParentViewModel.MotionGroupPropertiesCommand;

        // ReSharper disable once MemberCanBeProtected.Global
        public ObservableCollection<ITag> AllTags
        {
            get { return _allTags; }
            set { Set(ref _allTags, value); }
        }

        public virtual ITag SelectedTag { get; protected set; }
    }
}
