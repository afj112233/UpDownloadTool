using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using ICSStudio.Gui.Dialogs;
using ICSStudio.UIServicesPackage.MotionDirectCommands.Panel;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands
{
    internal partial class MotionDirectCommandsViewModel
    {
        internal sealed class OptionPanelNode : ViewModelBase
        {
            public readonly IOptionPanelDescriptor OptionPanelDescriptor;
            public readonly OptionPanelNode Parent;
            public readonly MotionDirectCommandsViewModel ViewModel;

            private IMotionDirectCommandPanel _optionPanel;
            private List<OptionPanelNode> _children;
            private bool _isActive;
            private bool _isExpanded;
            private bool _isSelected;

            public OptionPanelNode(IOptionPanelDescriptor optionPanel, MotionDirectCommandsViewModel viewModel)
            {
                OptionPanelDescriptor = optionPanel;
                ViewModel = viewModel;

                _isExpanded = true;
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
                        _optionPanel = OptionPanelDescriptor.OptionPanel as IMotionDirectCommandPanel;
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
                    {
                        ViewModel.SelectNode(this);
                        _optionPanel.Show();
                    }
                }
            }

            public override string ToString()
            {
                return Title;
            }
        }
    }
}
