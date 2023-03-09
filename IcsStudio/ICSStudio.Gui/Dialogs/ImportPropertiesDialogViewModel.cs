using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ICSStudio.Gui.Dialogs
{
    public class ImportPropertiesDialogViewModel : ViewModelBase
    {
        private string _title;

        public ImportPropertiesDialogViewModel()
        {
            TabbedOptions = new TabbedOptions();

            Operation1Command = new RelayCommand(ExecuteOperation1Command, CanExecuteOperation1Command);
            Operation2Command = new RelayCommand(ExecuteOperation2Command, CanExecuteOperation2Command);
            HelpCommand = new RelayCommand(ExecuteHelpCommand);
            CloseCommand = new RelayCommand(ExecuteCloseCommand);
        }

        public ImportPropertiesDialogViewModel(IEnumerable<IOptionPanelDescriptor> optionPanels)
        {
            TabbedOptions = new TabbedOptions();
            TabbedOptions.AddOptionPanels(optionPanels);

            Operation1Command = new RelayCommand(ExecuteOperation1Command,CanExecuteOperation1Command);
            Operation2Command = new RelayCommand(ExecuteOperation2Command,CanExecuteOperation2Command);
            HelpCommand = new RelayCommand(ExecuteHelpCommand);
            CloseCommand = new RelayCommand(ExecuteCloseCommand);
        }

        public string Operation1 { set; get; }
        public string Operation2 { set; get; }
        public string CurrentOperation { set; get; }

        public Action CloseAction { get; set; }

        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }

        public virtual bool IsClosing { set; get; }

        public TabbedOptions TabbedOptions { get; }

        #region Command

        public RelayCommand Operation1Command { get; }
        public RelayCommand Operation2Command { get; }
        public RelayCommand HelpCommand { get; }
        public RelayCommand CloseCommand { get; }

        protected virtual void ExecuteOperation1Command()
        {
            CloseAction?.Invoke();
        }

        protected virtual bool CanExecuteOperation1Command()
        {
            return true;
        }

        protected virtual void ExecuteOperation2Command()
        {
            CloseAction?.Invoke();
        }

        protected virtual bool CanExecuteOperation2Command()
        {
            return true;
        }

        protected virtual void ExecuteHelpCommand()
        {

        }

        private void ExecuteCloseCommand()
        {
            foreach (TabbedOptions.OptionTabPage optionTabPage in TabbedOptions.Items)
            {
                optionTabPage.CleanUp();
            }
            CloseAction?.Invoke();
        }

        #endregion
    }
}
