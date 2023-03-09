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
using ICSStudio.UIServicesPackage.ProgramProperties.Panel;
using Microsoft.VisualStudio.Shell;
using General = ICSStudio.UIServicesPackage.ProgramProperties.Panel.General;

namespace ICSStudio.UIServicesPackage.ProgramProperties
{
    public class ProgramPropertiesViewModel : TabbedOptionsDialogViewModel, ICanApply
    {
        private readonly IProgram _program;
        private readonly List<IOptionPanelDescriptor> _optionPanelDescriptors;
        private bool _flag = true;
        private readonly ProgramType _type;
        private readonly bool _faultProgram = false;
        private readonly bool _powerProgram = false;


        private bool? _dialogResult;

        public ProgramPropertiesViewModel(IProgram program, bool isShowParameterTab = false)
        {
            if (program == null)
                throw new ArgumentOutOfRangeException();

            _program = program;
            _type = program.Type;

            if (program.ParentController.MajorFaultProgram == program.Name)
            {
                _faultProgram = true;
            }
            else if (program.ParentController.PowerLossProgram == program.Name)
            {
                _powerProgram = true;
            }

            if (program.Type == ProgramType.Phase)
            {
                _optionPanelDescriptors = new List<IOptionPanelDescriptor>
                {
                    new DefaultOptionPanelDescriptor("1","General", "General",
                        new GeneralViewModel<General>(new General(), program), null),
                    new DefaultOptionPanelDescriptor("2","Configuration", "Configuration",
                        new ConfigurationViewModel<EPConfiguration>(new EPConfiguration(), program), null),
                    new DefaultOptionPanelDescriptor("3","Parameters", "Parameters",
                        new ParametersViewModel(new Parameters(), program), null),
                    new DefaultOptionPanelDescriptor("4","Monitor", "Monitor",
                        new MonitorViewModel(new Monitor(), program), null),
                };
            }
            else if (program.Type == ProgramType.Sequence)
            {
                _optionPanelDescriptors = new List<IOptionPanelDescriptor>
                {
                    new DefaultOptionPanelDescriptor("1","General", "General",
                        new GeneralViewModel<ESGeneral>(new ESGeneral(), program), null),
                    new DefaultOptionPanelDescriptor("2","Configuration", "Configuration",
                        new ConfigurationViewModel<ESConfiguration>(new ESConfiguration(), program), null),
                    new DefaultOptionPanelDescriptor("3","Parameters", "Parameters",
                        new ParametersViewModel(new Parameters(), program), null),
                    new DefaultOptionPanelDescriptor("4","Monitor", "Monitor",
                        new MonitorViewModel(new Monitor(), program), null),
                };
            }
            else
            {
                _optionPanelDescriptors = new List<IOptionPanelDescriptor>
                {
                    new DefaultOptionPanelDescriptor("1","General", "General",
                        new GeneralViewModel<General>(new General(), program), null),
                    new DefaultOptionPanelDescriptor("2","Configuration", "Configuration",
                        new ConfigurationViewModel<Configuration>(new Configuration(), program), null),
                    new DefaultOptionPanelDescriptor("3","Parameters", "Parameters",
                        new ParametersViewModel(new Parameters(), program), null),
                    new DefaultOptionPanelDescriptor("4","Monitor", "Monitor",
                        new MonitorViewModel(new Monitor(), program), null),
                };
            }

            if (isShowParameterTab)
                TabbedOptions.SelectedIndex = 2;
            TabbedOptions.AddOptionPanels(_optionPanelDescriptors);
            Title = LanguageManager.GetInstance().ConvertSpecifier("Program Properties") + $" - {program.Name}";

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            Title = LanguageManager.GetInstance().ConvertSpecifier("Program Properties") + $" - {_program.Name}";
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

            if (_type == ProgramType.Sequence)
            {
                if ((_optionPanelDescriptors[0].OptionPanel as GeneralViewModel<ESGeneral>).IsDirty)
                {
                    (_optionPanelDescriptors[0].OptionPanel as GeneralViewModel<ESGeneral>).Save();
                    (_optionPanelDescriptors[0].OptionPanel as GeneralViewModel<ESGeneral>).Compare();
                    Title = LanguageManager.GetInstance().ConvertSpecifier("Program Properties")
                        + $" - {(_optionPanelDescriptors[0].OptionPanel as GeneralViewModel<ESGeneral>).Name}";
                }
            }
            else
            {
                if ((_optionPanelDescriptors[0].OptionPanel as GeneralViewModel<General>).IsDirty)
                {
                    (_optionPanelDescriptors[0].OptionPanel as GeneralViewModel<General>).Save();
                    (_optionPanelDescriptors[0].OptionPanel as GeneralViewModel<General>).Compare();
                    Title = LanguageManager.GetInstance().ConvertSpecifier("Program Properties")
                            + $" - {(_optionPanelDescriptors[0].OptionPanel as GeneralViewModel<General>).Name}";
                }
            }

            if (_type == ProgramType.Phase)
            {
                if ((_optionPanelDescriptors[1].OptionPanel as ConfigurationViewModel<EPConfiguration>).IsDirty)
                {
                    (_optionPanelDescriptors[1].OptionPanel as ConfigurationViewModel<EPConfiguration>).Save();
                    (_optionPanelDescriptors[1].OptionPanel as ConfigurationViewModel<EPConfiguration>).Compare();
                }
            }
            else if (_type == ProgramType.Sequence)
            {
                if ((_optionPanelDescriptors[1].OptionPanel as ConfigurationViewModel<ESConfiguration>).IsDirty)
                {
                    (_optionPanelDescriptors[1].OptionPanel as ConfigurationViewModel<ESConfiguration>).Save();
                    (_optionPanelDescriptors[1].OptionPanel as ConfigurationViewModel<ESConfiguration>).Compare();
                }
            }
            else
            {
                if ((_optionPanelDescriptors[1].OptionPanel as ConfigurationViewModel<Configuration>).IsDirty)
                {
                    (_optionPanelDescriptors[1].OptionPanel as ConfigurationViewModel<Configuration>).Save();
                    (_optionPanelDescriptors[1].OptionPanel as ConfigurationViewModel<Configuration>).Compare();
                }
            }

            if ((_optionPanelDescriptors[2].OptionPanel as ParametersViewModel).IsDirty)
            {
                (_optionPanelDescriptors[2].OptionPanel as ParametersViewModel).Save();
                (_optionPanelDescriptors[2].OptionPanel as ParametersViewModel).IsDirty = false;
            }

            #endregion

            var controller = Controller.GetInstance();
            if (_faultProgram)
            {
                controller.MajorFaultProgram = _program.Name;
            }
            else if (_powerProgram)
            {
                controller.PowerLossProgram = _program.Name;
            }
        }

        private bool Check()
        {
            #region 验证数据

            _flag = true;
            if (_type == ProgramType.Sequence)
            {
                if (!(_optionPanelDescriptors[0].OptionPanel as GeneralViewModel<ESGeneral>).CheckInvalid())
                    _flag = false;

                if (!(_optionPanelDescriptors[1].OptionPanel as ConfigurationViewModel<ESConfiguration>)
                    .CheckMainRoutine())
                {
                    MessageBox.Show(
                        "Failed to modify MainProgram properties.\nSub-Routine cannot be set as main routine of program.",
                        "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Information);
                    _flag = false;
                }
            }
            else if (_type == ProgramType.Phase)
            {
                if (!(_optionPanelDescriptors[1].OptionPanel as ConfigurationViewModel<EPConfiguration>)
                    .CheckMainRoutine())
                {
                    MessageBox.Show(
                        "Failed to modify MainProgram properties.\nSub-Routine cannot be set as main routine of program.",
                        "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Information);
                    _flag = false;
                }
            }
            else
            {
                if (!(_optionPanelDescriptors[0].OptionPanel as GeneralViewModel<General>).CheckInvalid())
                    _flag = false;
                if (!(_optionPanelDescriptors[1].OptionPanel as ConfigurationViewModel<Configuration>)
                    .CheckMainRoutine())
                {

                    MessageBox.Show(
                        "Failed to modify MainProgram properties.\nSub-Routine cannot be set as main routine of program.",
                        "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Information);
                    _flag = false;
                }
            }

            if (!(_optionPanelDescriptors[2].OptionPanel as ParametersViewModel).CheckAllParameters()) _flag = false;



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
                if (CanExecuteApplyCommand() && !_program.IsDeleted)
                {
                    string message = "Apply changes?";
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
