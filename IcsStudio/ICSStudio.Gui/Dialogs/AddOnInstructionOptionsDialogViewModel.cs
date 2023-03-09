using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ICSStudio.Gui.Dialogs
{
    public class AddOnInstructionOptionsDialogViewModel : ViewModelBase
    {
        private string _title;
        private string _dataSize;
        private bool _isEnable;
        private bool _isChecked;

        public AddOnInstructionOptionsDialogViewModel()
        {
            TabbedOptions = new TabbedOptions();

            OkCommand = new RelayCommand(ExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            ApplyCommand = new RelayCommand(ExecuteApplyCommand, CanExecuteApplyCommand);
            HelpCommand = new RelayCommand(ExecuteHelpCommand);
            LogicCommand=new RelayCommand(ExecuteLogicCommand);
            CloseCommand = new RelayCommand(ExecuteCloseCommand);
            WeakEventManager<TabbedOptions, EventArgs>.AddHandler(TabbedOptions, "OnDirtyChangedHandler", TabbedOptions_OnDirtyChangedHandler);
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set { Set(ref _isChecked , value); }
        }

        public bool IsEnable
        {
            get { return _isEnable; }
            set
            {
                Set(ref _isEnable , value);
                if (!value) IsChecked = false;
            }     
        }

        public AddOnInstructionOptionsDialogViewModel(IEnumerable<IOptionPanelDescriptor> optionPanels)
        {
            TabbedOptions = new TabbedOptions();
            TabbedOptions.AddOptionPanels(optionPanels);
            
            OkCommand = new RelayCommand(ExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            ApplyCommand = new RelayCommand(ExecuteApplyCommand, CanExecuteApplyCommand);
            HelpCommand = new RelayCommand(ExecuteHelpCommand);
            LogicCommand = new RelayCommand(ExecuteLogicCommand);
            CloseCommand = new RelayCommand(ExecuteCloseCommand);
            WeakEventManager<TabbedOptions, EventArgs>.AddHandler(TabbedOptions, "OnDirtyChangedHandler", TabbedOptions_OnDirtyChangedHandler);
        }
        public override void Cleanup()
        {

            WeakEventManager<TabbedOptions, EventArgs>.RemoveHandler(TabbedOptions, "OnDirtyChangedHandler", TabbedOptions_OnDirtyChangedHandler);
        }
        private void TabbedOptions_OnDirtyChangedHandler(object sender, EventArgs e)
        {
            ApplyCommand?.RaiseCanExecuteChanged();
        }

        public Action CloseAction { get; set; }

        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }

        public virtual string DataSize
        {
            set { Set(ref _dataSize, value); }
            get { return _dataSize; }
        }

        public virtual bool IsClosing { set; get; }
        
        public TabbedOptions TabbedOptions { get; }
        public RelayCommand OkCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand ApplyCommand { get; }
        public RelayCommand HelpCommand { get; }
        public RelayCommand LogicCommand { get; }
        public RelayCommand CloseCommand { get; }
        #region Command
        private void ExecuteCancelCommand()
        {
            CloseAction?.Invoke();
        }

        protected virtual void ExecuteOkCommand()
        {
            CloseAction?.Invoke();
        }

        protected virtual void ExecuteApplyCommand()
        {
        }

        protected virtual bool CanExecuteApplyCommand()
        {

            return false;
        }

        protected virtual void ExecuteHelpCommand()
        {

        }

        protected virtual void ExecuteLogicCommand()
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
