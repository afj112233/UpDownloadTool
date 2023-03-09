using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.UIServicesPackage.TaskProperties.panel;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.TaskProperties
{
    public class TaskPropertiesViewModel : TabbedOptionsDialogViewModel, ICanApply
    {
        private readonly ITask _task;
        private readonly List<IOptionPanelDescriptor> _optionPanelDescriptors;
        private bool _flag = true;
        private bool? _dialogResult;

        public TaskPropertiesViewModel(ITask task)
        {
            if (task == null)
                throw new ArgumentOutOfRangeException();

            _task = task;

            _optionPanelDescriptors = new List<IOptionPanelDescriptor>
            {
                new DefaultOptionPanelDescriptor("1", "General", "General",
                    new GeneralViewModel(new General(), task), null),
                new DefaultOptionPanelDescriptor("2", "Configuration", "Configuration",
                    new ConfigurationViewModel(new Configuration(), task), null),
                new DefaultOptionPanelDescriptor("3", "Program Schedule", "Program Schedule",
                    new ProgramScheduleViewModel(new ProgramSchedule(), task), null),
                new DefaultOptionPanelDescriptor("4", "Monitor", "Monitor",
                    new MonitorViewModel(new Monitor(), task), null),

            };
            TabbedOptions.AddOptionPanels(_optionPanelDescriptors);
            Title = LanguageManager.GetInstance().ConvertSpecifier("Task Properties") + $" - {task.Name}";

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            Title = LanguageManager.GetInstance().ConvertSpecifier("Task Properties") + $" - {_task.Name}";
        }

        protected override bool CanExecuteApplyCommand()
        {
            try
            {
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
                                    // set dirty
                                    IProjectInfoService projectInfoService =
                                        Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
                                    projectInfoService?.SetProjectDirty();

                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return false;
        }

        protected override void ExecuteApplyCommand()
        {
            if (!Check()) return;
            DoApply();
        }

        private void DoApply()
        {
            #region 保存数据

            if ((_optionPanelDescriptors[0].OptionPanel as GeneralViewModel).IsDirty)
            {
                (_optionPanelDescriptors[0].OptionPanel as GeneralViewModel).Save();
                (_optionPanelDescriptors[0].OptionPanel as GeneralViewModel).Compare();
                Title = LanguageManager.GetInstance().ConvertSpecifier("Task Properties")
                    + $" - {(_optionPanelDescriptors[0].OptionPanel as GeneralViewModel).Name}";
            }

            if ((_optionPanelDescriptors[1].OptionPanel as ConfigurationViewModel).IsDirty)
            {
                (_optionPanelDescriptors[1].OptionPanel as ConfigurationViewModel).Save();
                (_optionPanelDescriptors[1].OptionPanel as ConfigurationViewModel).Compare();
            }

            if ((_optionPanelDescriptors[2].OptionPanel as ProgramScheduleViewModel).IsDirty)
            {
                (_optionPanelDescriptors[2].OptionPanel as ProgramScheduleViewModel).Save();
                (_optionPanelDescriptors[2].OptionPanel as ProgramScheduleViewModel).SetList();
                (_optionPanelDescriptors[2].OptionPanel as ProgramScheduleViewModel).Compare();
            }

            #endregion
        }

        private bool Check()
        {
            #region 验证数据

            _flag = true;
            if (!(_optionPanelDescriptors[0].OptionPanel as GeneralViewModel).CheckInvalid()) _flag = false;
            if (!(_optionPanelDescriptors[1].OptionPanel as ConfigurationViewModel).CheckInvalid()) _flag = false;
            if (!(_optionPanelDescriptors[2].OptionPanel as ProgramScheduleViewModel).CheckInvalid()) _flag = false;

            return _flag;

            #endregion
        }

        protected override void ExecuteCancelCommand()
        {
            _dialogResult = false;
            CloseAction.Invoke();
        }

        protected override void ExecuteOkCommand()
        {
            if (CanExecuteApplyCommand())
                ExecuteApplyCommand();

            if (_flag)
            {
                _dialogResult = true;
                CloseAction?.Invoke();
            }
        }

        protected override void ExecuteClosingCommand(CancelEventArgs args)
        {
            if (!_dialogResult.HasValue)
            {
                if (CanExecuteApplyCommand() && _task.ParentCollection != null)
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
                        if (!_flag)
                            args.Cancel = true;
                    }
                }
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
