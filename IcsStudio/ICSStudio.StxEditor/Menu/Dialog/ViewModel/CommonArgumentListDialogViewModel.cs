using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Instruction;
using ICSStudio.SimpleServices.Tags;

namespace ICSStudio.StxEditor.Menu.Dialog.ViewModel
{
    internal class CommonArgumentListDialogViewModel : ViewModelBase
    {
        private bool? _dialogResult;
        private IXInstruction _instruction;
        public CommonArgumentListDialogViewModel(string instrName, ObservableCollection<Argument> arguments)
        {
            Title = $"{instrName} Instruction - Argument List";
            Arguments = arguments;
            _instruction = Controller.GetInstance().STInstructionCollection.FindInstruction(instrName);
            Debug.Assert(_instruction != null, instrName);
            InsertCommand = new RelayCommand(ExecuteInsertCommand, CanExecuteInsertCommand);
            SaveCommand = new RelayCommand(ExecuteSaveCommand);
            OKCommand = new RelayCommand(ExecuteOKCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            ApplyCommand = new RelayCommand(ExecuteApplyCommand,CanExecuteApplyCommand);
            foreach (var argument in arguments)
            {
                PropertyChangedEventManager.AddHandler(argument, Parameter_PropertyChanged, string.Empty);
            }
            CollectionChangedEventManager.AddHandler(arguments, Parameters_CollectionChanged);
        }

        public override void Cleanup()
        {
            foreach (var argument in Arguments)
            {
                PropertyChangedEventManager.RemoveHandler(argument, Parameter_PropertyChanged, string.Empty);
                argument.Clean();
            }
            CollectionChangedEventManager.RemoveHandler(Arguments, Parameters_CollectionChanged);
        }

        private void Parameters_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldItem in e.OldItems)
                {
                    PropertyChangedEventManager.RemoveHandler((Argument)oldItem, Parameter_PropertyChanged, string.Empty);
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var newItem in e.NewItems)
                {
                    PropertyChangedEventManager.AddHandler((Argument)newItem, Parameter_PropertyChanged, String.Empty);
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
        public Visibility DescriptionVisibility { set; get; } = Visibility.Collapsed;
        public bool? DialogResult
        {
            set { Set(ref _dialogResult, value); }
            get { return _dialogResult; }
        }

        public string Title { get; }

        public RelayCommand InsertCommand { get; }

        private void ExecuteInsertCommand()
        {
            foreach (var argument in Arguments)
            {
                if(!string.IsNullOrEmpty(argument.Param))continue;
                var value = _instruction.DefaultArguments[argument.Parameter];
                argument.Param = value;
            }
        }

        private bool CanExecuteInsertCommand()
        {
            return _instruction.DefaultArguments.Count > 0;
        }

        public RelayCommand SaveCommand { get; }

        private void ExecuteSaveCommand()
        {
            int index = 0;

            foreach (var argument in Arguments)
            {
                var param = argument.Param;
                if (index == 0 && _instruction is AoiDefinition.AOIInstruction)
                {
                    param = "";
                }
                if (_instruction.DefaultArguments.ContainsKey(argument.Parameter))
                {
                    _instruction.DefaultArguments[argument.Parameter] = param;
                }
                else
                {
                    _instruction.DefaultArguments.Add(argument.Parameter, param);
                }

                index++;
            }
            InsertCommand.RaiseCanExecuteChanged();
        }

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

        private void ExecuteApplyCommand()
        {
            RaisePropertyChanged("IsUpdated");
            _canExecuteApplyCommand = false;
            ApplyCommand.RaiseCanExecuteChanged();
        }

        private bool _canExecuteApplyCommand = false;
        private bool CanExecuteApplyCommand()
        {
            return _canExecuteApplyCommand;
        }
        public bool IsUpdated { set; get; }
        public ObservableCollection<Argument> Arguments { get; } = new ObservableCollection<Argument>();
    }


}

