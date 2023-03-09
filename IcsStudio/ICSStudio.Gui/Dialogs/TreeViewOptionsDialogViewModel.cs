using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;

namespace ICSStudio.Gui.Dialogs
{
    public class TreeViewOptionsDialogViewModel : ViewModelBase
    {
        protected List<IOptionPanel> OptionPanels = new List<IOptionPanel>();
        private OptionPanelNode _activeNode;

        private string _title;
        private string _optionPanelTitle;
        private object _optionPanelContent;
        private string _state;

        //public OptionsDialogViewModel(IEnumerable<IOptionPanelDescriptor> optionPanels)
        //{
        //    OptionPanelNodes = optionPanels.Select(op => new OptionPanelNode(op, this)).ToList();
        //    OptionPanelNodes[0].IsSelected = true;

        //    ApplyCommand = new RelayCommand(ExecuteApplyCommand,CanExecuteApplyCommand);
        //    ExpansionCommand = new RelayCommand(ExecuteExpansionCommand);
        //    HelpCommand = new RelayCommand(ExecuteHelpCommand);
        //    CancelCommand = new RelayCommand(ExecuteCancelCommand);
        //    OkCommand = new RelayCommand(ExecuteOkCommand);
        //}

        public TreeViewOptionsDialogViewModel()
        {
            StateVisibility = Visibility.Collapsed;
            ExpansionVisibility = Visibility.Collapsed;

            ApplyCommand = new RelayCommand(ExecuteApplyCommand, CanExecuteApplyCommand);
            ExpansionCommand = new RelayCommand(ExecuteExpansionCommand);
            HelpCommand = new RelayCommand(ExecuteHelpCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            OkCommand = new RelayCommand(ExecuteOkCommand);
        }


        protected virtual void ExecuteOkCommand()
        {
            CloseAction?.Invoke();
        }

        private void ExecuteCancelCommand()
        {
            CloseAction?.Invoke();
        }

        protected virtual void ExecuteHelpCommand()
        {
        }

        protected virtual void ExecuteExpansionCommand()
        {
        }

        protected virtual bool CanExecuteApplyCommand()
        {
            return false;
        }

        protected virtual void ExecuteApplyCommand()
        {
        }

        public Action CloseAction { get; set; }

        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }

        public string OptionPanelTitle
        {
            get { return _optionPanelTitle; }
            set { Set(ref _optionPanelTitle, value); }
        }

        public List<OptionPanelNode> OptionPanelNodes { get; set; }

        public object OptionPanelContent
        {
            get { return _optionPanelContent; }
            set { Set(ref _optionPanelContent, value); }
        }

        public string State
        {
            get { return _state; }
            set { Set(ref _state, value); }
        }

        public Visibility StateVisibility { get; set; }

        public Visibility ExpansionVisibility { get; set; }
        public string ExpansionName { get; set; }

        public RelayCommand ExpansionCommand { get; }
        public RelayCommand HelpCommand { get; }
        public RelayCommand ApplyCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand OkCommand { get; }

        protected void SelectNode(OptionPanelNode node)
        {
            if (node == _activeNode)
                return;

            if (_activeNode != null)
            {
                _activeNode.IsActive = false;
                _activeNode = node;
            }

            OptionPanelTitle = node.Label;
            OptionPanelContent = node.Content;

            node.IsExpanded = true;
            node.IsActive = true;
        }

        public sealed class OptionPanelNode : ViewModelBase
        {
            public readonly IOptionPanelDescriptor OptionPanelDescriptor;
            public readonly OptionPanelNode Parent;
            public readonly TreeViewOptionsDialogViewModel ViewModel;

            private IOptionPanel _optionPanel;
            private List<OptionPanelNode> _children;
            private bool _isActive;
            private bool _isExpanded;
            private bool _isSelected;

            public OptionPanelNode(IOptionPanelDescriptor optionPanel, TreeViewOptionsDialogViewModel viewModel)
            {
                OptionPanelDescriptor = optionPanel;
                ViewModel = viewModel;
            }

            public OptionPanelNode(IOptionPanelDescriptor optionPanel, OptionPanelNode parent)
            {
                OptionPanelDescriptor = optionPanel;
                Parent = parent;
                ViewModel = parent.ViewModel;
            }

            public string ID => OptionPanelDescriptor.ID;

            public string Title => OptionPanelDescriptor.Title;
            public string Label => OptionPanelDescriptor.Label;

            public object Content
            {
                get
                {
                    if (_optionPanel == null)
                    {
                        _optionPanel = OptionPanelDescriptor.OptionPanel;
                        if (_optionPanel == null)
                            return null;

                        _optionPanel.LoadOptions();
                        ViewModel.OptionPanels.Add(_optionPanel);
                    }

                    return _optionPanel.Control;
                }
            }

            public List<OptionPanelNode> Children
            {
                get
                {
                    return _children ?? (_children = OptionPanelDescriptor.ChildOptionPanelDescriptors
                               .Select(op => new OptionPanelNode(op, this)).ToList());
                }
            }

            public bool IsActive
            {
                get { return _isActive; }
                set { Set(ref _isActive, value); }
            }

            public bool IsExpanded
            {
                get { return _isExpanded; }
                set { Set(ref _isExpanded, value); }
            }

            public bool IsSelected
            {
                get { return _isSelected; }
                set
                {
                    Set(ref _isSelected, value);

                    if (_isSelected)
                        ViewModel.SelectNode(this);
                }
            }

            public override string ToString()
            {
                return Title;
            }
        }
    }
}
