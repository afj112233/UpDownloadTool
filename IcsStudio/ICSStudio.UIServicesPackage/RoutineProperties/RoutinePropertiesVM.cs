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
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.RoutineProperties
{
    class RoutinePropertiesVM : TabbedOptionsDialogViewModel, ICanApply
    {
        private readonly IRoutine _routine;
        private readonly List<IOptionPanelDescriptor> _optionPanelDescriptors;
        private bool? _dialogResult;

        public RoutinePropertiesVM(IRoutine routine)
        {
            if (routine == null)
                throw new ArgumentOutOfRangeException();

            _routine = routine;

            _optionPanelDescriptors = new List<IOptionPanelDescriptor>
            {
                new DefaultOptionPanelDescriptor("1","General", "General",
                    new Panel.GeneralViewModel(new Panel.General(), routine), null),

            };
            if (routine.Type == RoutineType.FBD || routine.Type == RoutineType.Sequence)
            {
                _optionPanelDescriptors.Add(new DefaultOptionPanelDescriptor("2",
                    "SheetLayout", "SheetLayout",
                    new Panel.SheetLayoutViewModel(new Panel.SheetLayout(), routine), null));
            }

            if (routine.Type == RoutineType.SFC)
            {
                _optionPanelDescriptors.Add(new DefaultOptionPanelDescriptor("2",
                    "Auto-Naming", "Auto-Naming",
                    new Panel.AutoNamingViewModel(new Panel.AutoNaming(), routine), null));
                _optionPanelDescriptors.Add(new DefaultOptionPanelDescriptor("3",
                    "SheetLayout", "SheetLayout",
                    new Panel.SheetLayoutViewModel(new Panel.SheetLayout(), routine), null));
            }

            TabbedOptions.AddOptionPanels(_optionPanelDescriptors);
            Title = "Routine ";
            Title += LanguageManager.GetInstance().ConvertSpecifier("Properties");
            Title += $" - {routine.Name}";

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            Title = "Routine ";
            Title += LanguageManager.GetInstance().ConvertSpecifier("Properties");
            Title += $" - {_routine.Name}";
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
            if ((_optionPanelDescriptors[0].OptionPanel as Panel.GeneralViewModel).IsDirty)
            {
                if ((_optionPanelDescriptors[0].OptionPanel as Panel.GeneralViewModel).IsValidRoutineName(true))
                {
                    (_optionPanelDescriptors[0].OptionPanel as Panel.GeneralViewModel).Save();
                    (_optionPanelDescriptors[0].OptionPanel as Panel.GeneralViewModel).Compare();
                    Title = "Routine ";
                    Title += LanguageManager.GetInstance().ConvertSpecifier("Properties");
                    Title += $" - {(_optionPanelDescriptors[0].OptionPanel as Panel.GeneralViewModel).Name}";
                }
            }

            var aoi = _routine.ParentCollection.ParentProgram as AoiDefinition;
            aoi?.UpdateChangeHistory();
            //IStudioUIService studioUIService =
            //    Package.GetGlobalService(typeof(SStudioUIService)) as IStudioUIService;
            //studioUIService?.UpdateUI();
        }

        private bool Check()
        {
            var panel = (_optionPanelDescriptors[0].OptionPanel as Panel.GeneralViewModel);
            if (panel.IsDirty)
                return panel.IsValidRoutineName(false);
            return true;
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

            _dialogResult = true;
            CloseAction?.Invoke();
        }

        protected override void ExecuteClosingCommand(CancelEventArgs args)
        {
            if (!_dialogResult.HasValue)
            {
                if (CanExecuteApplyCommand() && !_routine.IsDeleted)
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
                    }
                }
            }
        }

        public int Apply()
        {
            if (!Check()) return -1;
            ExecuteApplyCommand();
            return 0;
        }

        public bool CanApply()
        {
            return CanExecuteApplyCommand();
        }
    }
}
