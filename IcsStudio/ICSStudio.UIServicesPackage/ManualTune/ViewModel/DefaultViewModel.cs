using System.Collections.ObjectModel;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Interfaces.Tags;
using ICSStudio.UIServicesPackage.ManualTune.Panel;

namespace ICSStudio.UIServicesPackage.ManualTune.ViewModel
{
    internal class DefaultViewModel : ViewModelBase,IMotionGeneratorPanel
    {
        protected readonly MotionGeneratorViewModel ParentViewModel;

        private ObservableCollection<ITag> _allTags;

        public DefaultViewModel(UserControl panel,
            MotionGeneratorViewModel parentViewModel)
        {
            Control = panel;
            panel.DataContext = this;

            ParentViewModel = parentViewModel;
            MotionGeneratorCommand = MotionGeneratorCommand.NullCommand;
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

        public MotionGeneratorCommand MotionGeneratorCommand { get; protected set; }

        public virtual IMessageRouterRequest GetQueryCommandRequest()
        {
            return null;
        }

        public virtual IMessageRouterRequest GetExecuteCommandRequest()
        {
            return null;
        }

        public RelayCommand<ITag> AxisPropertiesCommand => ParentViewModel.AxisPropertiesCommand;
        // ReSharper disable once MemberCanBeProtected.Global
        public ObservableCollection<ITag> AllTags
        {
            get { return _allTags; }
            set { Set(ref _allTags, value); }
        }

        public virtual ITag SelectedTag { get; protected set; }
    }
}
