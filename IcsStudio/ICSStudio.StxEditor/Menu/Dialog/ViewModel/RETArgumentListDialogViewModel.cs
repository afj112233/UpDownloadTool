using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.StxEditor.Menu.Dialog.ViewModel
{
    internal class RETArgumentListDialogViewModel : ViewModelBase
    {
        private bool? _dialogResult;
        private IProgramModule _program;
        private Hashtable _transformTable;

        public RETArgumentListDialogViewModel(ObservableCollection<Argument> parameters, IProgramModule program,
            string instrName, Hashtable transformTable)
        {
            Parameters = parameters;
            _program = program;
            _transformTable = transformTable;
            Title = $"{instrName} Instruction - Argument List";
            OKCommand = new RelayCommand(ExecuteOKCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            ApplyCommand = new RelayCommand(ExecuteApplyCommand, CanExecuteApplyCommand);
            AddBlank();
            foreach (var parameter in Parameters)
            {
                PropertyChangedEventManager.AddHandler(parameter, Parameter_PropertyChanged, string.Empty);
            }

            CollectionChangedEventManager.AddHandler(parameters, Parameters_CollectionChanged);
        }

        public string Title { get; }

        public override void Cleanup()
        {
            foreach (var parameter in Parameters)
            {
                PropertyChangedEventManager.RemoveHandler(parameter, Parameter_PropertyChanged, String.Empty);
            }
        }

        public void AddBlank()
        {
            var blank = new Argument("", "", null, -1, _program, _transformTable);
            PropertyChangedEventManager.AddHandler(blank, Blank_PropertyChanged, string.Empty);
            Parameters.Add(blank);
        }

        private void Blank_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            AddBlank();
            PropertyChangedEventManager.RemoveHandler((Argument) sender, Blank_PropertyChanged, string.Empty);
        }

        public bool? DialogResult
        {
            set { Set(ref _dialogResult, value); }
            get { return _dialogResult; }
        }

        public ObservableCollection<Argument> Parameters { get; } = new ObservableCollection<Argument>();

        public RelayCommand OKCommand { get; }

        private void ExecuteOKCommand()
        {
            DialogResult = true;
        }

        public RelayCommand CancelCommand { get; }

        private void ExecuteCancelCommand()
        {
            DialogResult = false;
        }

        public RelayCommand ApplyCommand { get; }
        private bool _canExecuteApplyCommand = false;

        private void ExecuteApplyCommand()
        {
            RaisePropertyChanged("IsUpdated");
            _canExecuteApplyCommand = false;
            ApplyCommand.RaiseCanExecuteChanged();
        }

        public bool CanExecuteApplyCommand()
        {
            return _canExecuteApplyCommand;
        }

        public bool IsUpdated { set; get; }
        public RelayCommand HelpCommand { get; }

        private void ExecuteHelpCommand()
        {

        }

        private void Parameters_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldItem in e.OldItems)
                {
                    PropertyChangedEventManager.RemoveHandler((Argument) oldItem, Parameter_PropertyChanged,
                        string.Empty);
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var newItem in e.NewItems)
                {
                    PropertyChangedEventManager.AddHandler((Argument) newItem, Parameter_PropertyChanged, String.Empty);
                }
            }

            _canExecuteApplyCommand = true;
            ApplyCommand.RaiseCanExecuteChanged();
        }

        private void Parameter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _canExecuteApplyCommand = true;
            ApplyCommand.RaiseCanExecuteChanged();
        }

    }
}
