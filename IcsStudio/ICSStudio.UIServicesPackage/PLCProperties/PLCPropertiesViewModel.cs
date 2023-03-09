using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.UIInterfaces.UI;
using ICSStudio.UIServicesPackage.PLCProperties.Panel;
using Microsoft.VisualStudio.Shell;
using DateTime = ICSStudio.UIServicesPackage.PLCProperties.Panel.DateTime;

namespace ICSStudio.UIServicesPackage.PLCProperties
{
    public class PLCPropertiesViewModel : TabbedOptionsDialogViewModel, ICanApply
    {
        private readonly List<IOptionPanelDescriptor> _optionPanelDescriptors;
        private bool _isCorrect = true;
        private readonly IController _controller;
        private bool? _dialogResult;

        public PLCPropertiesViewModel(IController controller)
        {
            if (controller == null)
                throw new ArgumentOutOfRangeException();
            _controller = controller;

            _optionPanelDescriptors = new List<IOptionPanelDescriptor>()
            {
                new DefaultOptionPanelDescriptor("1", "General", "General",
                    new GeneralViewModel(new General(), controller), null),
                new DefaultOptionPanelDescriptor("2","Major Faults", "Major Faults",
                    new MajorFaultsViewModel(new MajorFaults(), controller), null),
                new DefaultOptionPanelDescriptor("3","Minor Faults", "Minor Faults",
                    new MinorFaultsViewModel(new MinorFaults(), controller), null),
                new DefaultOptionPanelDescriptor("4","Date/Time", "Date/Time",
                    new DateTimeViewModel(new DateTime(), controller), null),
                new DefaultOptionPanelDescriptor("5","Advanced.", "Advanced",
                    new AdvancedViewModel(new Advanced(), controller), null),
                new DefaultOptionPanelDescriptor("6","Port Configuration", "Port Configuration",
                    new PortConfigurationViewModel(new PortConfiguration(), controller), null),
                new DefaultOptionPanelDescriptor("7","Network", "Network",
                    new NetworkViewModel(new Network(), controller), null),
                new DefaultOptionPanelDescriptor("8","Security", "Security",
                    new SecurityViewModel(new Security(), controller), null),
                new DefaultOptionPanelDescriptor("9","Alarm Log", "Alarm Log",
                    new AlarmLogViewModel(new AlarmLog(), controller), null),
                new DefaultOptionPanelDescriptor("10","SFC Execution", "SFC Execution",
                    new SFCExecutionViewModel(new SFCExecution(), controller), null),
                new DefaultOptionPanelDescriptor("11","Project", "Project",
                    new ProjectViewModel(new Project(), controller), null),
                new DefaultOptionPanelDescriptor("12","Nonvolatile Memory", "Nonvolatile Memory",
                    new NonvolatileMemoryViewModel(new NonvolatileMemory(), controller), null),
                new DefaultOptionPanelDescriptor("13","Capacity", "Capacity",
                    new CapacityViewModel(new Capacity(), controller), null),
                new DefaultOptionPanelDescriptor("14","Internet Protocol", "Internet Protocol",
                    new InternetProtocolViewModel(new InternetProtocol(), controller), null)
            };

            TabbedOptions.AddOptionPanels(_optionPanelDescriptors);
            Title = LanguageManager.GetInstance().ConvertSpecifier("Controller Properties") + $" - {controller.Name}";

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                controller as Controller, "IsOnlineChanged", OnIsOnlineChanged);
            WeakEventManager<Controller, EventArgs>.AddHandler(
                controller as Controller, "KeySwitchChanged", OnKeySwitchChanged);
            WeakEventManager<Controller, EventArgs>.AddHandler(
                controller as Controller, "OperationModeChanged", OnOperationModeChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            Title = LanguageManager.GetInstance().ConvertSpecifier("Controller Properties") + $" - {_controller.Name}";
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                foreach (var i in _optionPanelDescriptors)
                {
                    (i.OptionPanel as AlarmLogViewModel)?.Refresh();
                    (i.OptionPanel as AdvancedViewModel)?.Refresh();
                    (i.OptionPanel as GeneralViewModel)?.Refresh();
                    (i.OptionPanel as NetworkViewModel)?.Refresh();
                    (i.OptionPanel as NonvolatileMemoryViewModel)?.Refresh();
                    (i.OptionPanel as PortConfigurationViewModel)?.Refresh();
                    (i.OptionPanel as ProjectViewModel)?.Refresh();
                    (i.OptionPanel as SFCExecutionViewModel)?.Refresh();
                }
            });
        }

        private void OnKeySwitchChanged(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                foreach (var i in _optionPanelDescriptors)
                {
                    (i.OptionPanel as AlarmLogViewModel)?.Refresh();
                    (i.OptionPanel as AdvancedViewModel)?.Refresh();
                    (i.OptionPanel as GeneralViewModel)?.Refresh();
                    (i.OptionPanel as NetworkViewModel)?.Refresh();
                    (i.OptionPanel as NonvolatileMemoryViewModel)?.Refresh();
                    (i.OptionPanel as PortConfigurationViewModel)?.Refresh();
                    (i.OptionPanel as ProjectViewModel)?.Refresh();
                    (i.OptionPanel as SFCExecutionViewModel)?.Refresh();
                }
            });
        }

        private void OnOperationModeChanged(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                foreach (var i in _optionPanelDescriptors)
                {
                    (i.OptionPanel as AlarmLogViewModel)?.Refresh();
                    (i.OptionPanel as AdvancedViewModel)?.Refresh();
                    (i.OptionPanel as GeneralViewModel)?.Refresh();
                    (i.OptionPanel as NetworkViewModel)?.Refresh();
                    (i.OptionPanel as NonvolatileMemoryViewModel)?.Refresh();
                    (i.OptionPanel as PortConfigurationViewModel)?.Refresh();
                    (i.OptionPanel as ProjectViewModel)?.Refresh();
                    (i.OptionPanel as SFCExecutionViewModel)?.Refresh();
                }
            });
        }

        protected override void ExecuteApplyCommand()
        {
            if (!Check()) return;

            DoApply();
        }

        private void DoApply()
        {
            if (_isCorrect)
            {
                if (((GeneralViewModel)_optionPanelDescriptors[0].OptionPanel).IsDirty)
                    ((GeneralViewModel)_optionPanelDescriptors[0].OptionPanel)?.Save();
                if (((DateTimeViewModel)_optionPanelDescriptors[3].OptionPanel).IsDirty)
                    ((DateTimeViewModel)_optionPanelDescriptors[3].OptionPanel)?.Save();
                if (((AdvancedViewModel)_optionPanelDescriptors[4].OptionPanel).IsDirty)
                    ((AdvancedViewModel)_optionPanelDescriptors[4].OptionPanel)?.Save();
                if (((InternetProtocolViewModel)_optionPanelDescriptors[13].OptionPanel).IsDirty)
                    ((InternetProtocolViewModel)_optionPanelDescriptors[13].OptionPanel)?.DoApply();

            }

            // update title
            IStudioUIService uiService =
                Package.GetGlobalService(typeof(SStudioUIService)) as IStudioUIService;

            uiService?.UpdateWindowTitle();
        }

        private bool Check()
        {
            _isCorrect = true;
            if (!((GeneralViewModel) _optionPanelDescriptors[0].OptionPanel).Verify())
            {
                _isCorrect = false;
                return false;
            };
            if (!((AdvancedViewModel)_optionPanelDescriptors[4].OptionPanel).Verify())
            {
                _isCorrect = false;
                return false;
            };
            return _isCorrect;
        }

        protected override void ExecuteCancelCommand()
        {
            _dialogResult = false;
            CloseAction.Invoke();
        }

        protected override void ExecuteOkCommand()
        {
            ExecuteApplyCommand();
            if (_isCorrect)
            {
                _dialogResult = true;
                CloseAction.Invoke();
            }

        }

        protected override void ExecuteClosingCommand(CancelEventArgs args)
        {
            if (!_dialogResult.HasValue)
            {
                if (CanExecuteApplyCommand())
                {
                    string message = LanguageManager.GetInstance().ConvertSpecifier("Apply changes?");
                    string caption = "ICS Studio";

                    var messageBoxResult = MessageBox.Show(message, caption, MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Exclamation);

                    if (messageBoxResult == MessageBoxResult.Cancel)
                    {
                        args.Cancel = true;
                    }
                    else if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        ExecuteApplyCommand();
                        if (!_isCorrect)
                            args.Cancel = true;
                    }
                }
            }
        }

        protected override bool CanExecuteApplyCommand()
        {
            try
            {
                bool result = false;

                foreach (IOptionPanelDescriptor descriptor in _optionPanelDescriptors)
                {
                    if (descriptor != null)
                    {
                        if (descriptor.HasOptionPanel)
                        {
                            var optionPanel = descriptor.OptionPanel;
                            ICanBeDirty dirty = optionPanel as ICanBeDirty;
                            if (dirty != null)
                            {
                                if (dirty.IsDirty)
                                {
                                    result = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (result)
                {
                    // set dirty
                    IProjectInfoService projectInfoService =
                        Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
                    projectInfoService?.SetProjectDirty();
                }

                return result;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public int Apply()
        {
            if (!Check()) return -1;
            DoApply();
            return 0;
        }

        public bool CanApply()
        {
            return CanExecuteApplyCommand();
        }

        public override void Cleanup()
        {
            base.Cleanup();

            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }
    }
}
