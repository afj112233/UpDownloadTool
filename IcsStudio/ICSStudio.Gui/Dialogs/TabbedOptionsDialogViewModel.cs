using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ICSStudio.Gui.Dialogs
{
    public class TabbedOptionsDialogViewModel : ViewModelBase
    {
        private string _title;

        public TabbedOptionsDialogViewModel()
        {
            TabbedOptions = new TabbedOptions();

            OkCommand = new RelayCommand(ExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            ApplyCommand = new RelayCommand(ExecuteApplyCommand, CanExecuteApplyCommand);
            HelpCommand = new RelayCommand(ExecuteHelpCommand);

            CloseCommand = new RelayCommand(ExecuteCloseCommand);
            ClosingCommand = new RelayCommand<CancelEventArgs>(ExecuteClosingCommand);
            WeakEventManager<TabbedOptions,EventArgs>.AddHandler(TabbedOptions, "OnDirtyChangedHandler",TabbedOptions_OnDirtyChangedHandler);
        }

        public override void Cleanup()
        {
            WeakEventManager<TabbedOptions, EventArgs>.RemoveHandler(TabbedOptions, "OnDirtyChangedHandler", TabbedOptions_OnDirtyChangedHandler);
        }

        private void TabbedOptions_OnDirtyChangedHandler(object sender, EventArgs e)
        {
            ApplyCommand?.RaiseCanExecuteChanged();
        }

        public TabbedOptionsDialogViewModel(IEnumerable<IOptionPanelDescriptor> optionPanels)
        {
            TabbedOptions = new TabbedOptions();
            TabbedOptions.AddOptionPanels(optionPanels);

            OkCommand = new RelayCommand(ExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            ApplyCommand = new RelayCommand(ExecuteApplyCommand, CanExecuteApplyCommand);
            HelpCommand = new RelayCommand(ExecuteHelpCommand);

            CloseCommand = new RelayCommand(ExecuteCloseCommand);
            ClosingCommand = new RelayCommand<CancelEventArgs>(ExecuteClosingCommand);
            WeakEventManager<TabbedOptions, EventArgs>.AddHandler(TabbedOptions, "OnDirtyChangedHandler", TabbedOptions_OnDirtyChangedHandler);
        }

        

        public Action CloseAction { get; set; }

        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }


        public TabbedOptions TabbedOptions { get; }
        public RelayCommand OkCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand ApplyCommand { get; }
        public RelayCommand HelpCommand { get; }

        public RelayCommand<CancelEventArgs> ClosingCommand { get; }
        public RelayCommand CloseCommand { get; }

        #region Command

        protected virtual void ExecuteCancelCommand()
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

        private void ExecuteCloseCommand()
        {
            foreach (TabbedOptions.OptionTabPage optionTabPage in TabbedOptions.Items)
            {
                optionTabPage.CleanUp();
            }
        }

        protected virtual void ExecuteClosingCommand(CancelEventArgs args)
        {
        }

        #endregion

        public virtual void OnClosing()
        {

        }
    }
}
