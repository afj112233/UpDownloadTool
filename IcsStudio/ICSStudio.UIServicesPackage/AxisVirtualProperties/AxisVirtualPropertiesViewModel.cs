using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.UIServicesPackage.AxisVirtualProperties.Panel;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.AxisVirtualProperties
{
    public class AxisVirtualPropertiesViewModel : TabbedOptionsDialogViewModel, ICanApply
    {
        private readonly List<IOptionPanelDescriptor> _optionPanelDescriptors;
        private readonly ITag _axisVirtual;
        private bool _flag = true;
        private bool? _dialogResult;

        public AxisVirtualPropertiesViewModel(ITag axisVirtual)
        {
            if (axisVirtual == null)
                throw new ArgumentOutOfRangeException();
            var vm = new ConversionViewModel(new Conversion(), axisVirtual);
            _optionPanelDescriptors = new List<IOptionPanelDescriptor>
            {
                new DefaultOptionPanelDescriptor("1", "General", "General",
                    new GeneralViewModel(new General(), axisVirtual), null),
                new DefaultOptionPanelDescriptor("2", "Motion Planner", "Motion Planner",
                    new MotionPlannerViewModel(new MotionPlanner(), axisVirtual), null),
                new DefaultOptionPanelDescriptor("3", "Units", "Units",
                    new UnitsViewModel(new Units(), axisVirtual), null),
                new DefaultOptionPanelDescriptor("4", "Conversion", "Conversion",
                    vm, null),
                new DefaultOptionPanelDescriptor("5", "Homing", "Homing",
                    new HomingViewModel(new Homing(), axisVirtual), null),
                new DefaultOptionPanelDescriptor("6", "Dynamics", "Dynamics",
                    new DynamicsViewModel(new Dynamics(),vm, axisVirtual), null),
                new DefaultOptionPanelDescriptor("7", "Variable", "Variable",
                    new TagViewModel(new Tag(), axisVirtual), null),
            };
            _axisVirtual = axisVirtual;
            TabbedOptions.AddOptionPanels(_optionPanelDescriptors);
            Title = LanguageManager.GetInstance().ConvertSpecifier("Axis Virtual Properties") + $" - {axisVirtual.Name}";
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
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

        private bool Check()
        {
            #region 验证数据

            _flag = true;
            if (!((MotionPlannerViewModel)_optionPanelDescriptors[1].OptionPanel).CheckInvalid()) _flag = false;
            if (_flag&&!((UnitsViewModel)_optionPanelDescriptors[2].OptionPanel).CheckInvalid()) _flag = false;
            if (_flag && !((ConversionViewModel)_optionPanelDescriptors[3].OptionPanel).CheckInvalid()) _flag = false;
            AxisVirtualParameters.PositioningMode pm = ((ConversionViewModel)_optionPanelDescriptors[3].OptionPanel)
                .Selected;
            double cc = ((ConversionViewModel)_optionPanelDescriptors[3].OptionPanel).ConversionConstant;
            int pu = ((ConversionViewModel)_optionPanelDescriptors[3].OptionPanel).PositionUnwind;
            float position = (float)((HomingViewModel)_optionPanelDescriptors[4].OptionPanel).Position;
            if (pm == AxisVirtualParameters.PositioningMode.Linear)
            {
                if (position < -(2147480000 / cc) || position > (2147480000 / cc))
                {
                    var warningDialog = new WarningDialog(
                            $"Failed to change the properties for axis '{_axisVirtual.Name}'",
                            "Enter a HomePosition between " + -(2147480000 / cc) + " and " + (2147480000 / cc))
                        {Owner = Application.Current.MainWindow};
                    warningDialog.ShowDialog();
                    _flag = false;
                }
            }
            else
            {
                if (position < 0 || position > (pu / cc))
                {
                    var warningDialog = new WarningDialog(
                            $"Failed to change the properties for axis '{_axisVirtual.Name}'",
                            "Enter a HomePosition between 0 and " + (pu / cc))
                        {Owner = Application.Current.MainWindow};
                    warningDialog.ShowDialog();
                    _flag = false;
                }
            }

            if (_flag&&!((TagViewModel)_optionPanelDescriptors[6].OptionPanel).CheckInvalid()) _flag = false;
            return _flag;

            #endregion
        }

        private void DoApply()
        {
            #region 保存数据

            if ((_optionPanelDescriptors[0].OptionPanel as GeneralViewModel).IsDirty)
            {
                (_optionPanelDescriptors[0].OptionPanel as GeneralViewModel).Save();
                (_optionPanelDescriptors[0].OptionPanel as GeneralViewModel).Compare();
            }

            if ((_optionPanelDescriptors[1].OptionPanel as MotionPlannerViewModel).IsDirty)
            {
                (_optionPanelDescriptors[1].OptionPanel as MotionPlannerViewModel).Save();
                (_optionPanelDescriptors[1].OptionPanel as MotionPlannerViewModel).Compare();
            }

            if ((_optionPanelDescriptors[2].OptionPanel as UnitsViewModel).IsDirty)
            {
                (_optionPanelDescriptors[2].OptionPanel as UnitsViewModel).Save();
                (_optionPanelDescriptors[2].OptionPanel as UnitsViewModel).Compare();
            }

            if ((_optionPanelDescriptors[3].OptionPanel as ConversionViewModel).IsDirty)
            {
                (_optionPanelDescriptors[3].OptionPanel as ConversionViewModel).Save();
                (_optionPanelDescriptors[3].OptionPanel as ConversionViewModel).Compare();
            }

            if ((_optionPanelDescriptors[4].OptionPanel as HomingViewModel).IsDirty)
            {
                (_optionPanelDescriptors[4].OptionPanel as HomingViewModel).Save();
                (_optionPanelDescriptors[4].OptionPanel as HomingViewModel).Compare();
            }

            if ((_optionPanelDescriptors[5].OptionPanel as DynamicsViewModel).IsDirty)
            {
                (_optionPanelDescriptors[5].OptionPanel as DynamicsViewModel).Save();
                (_optionPanelDescriptors[5].OptionPanel as DynamicsViewModel).Compare();
            }

            if ((_optionPanelDescriptors[6].OptionPanel as TagViewModel).IsDirty)
            {
                (_optionPanelDescriptors[6].OptionPanel as TagViewModel).Save();
                (_optionPanelDescriptors[6].OptionPanel as TagViewModel).Compare();
                Title = $"Axis Virtual Properties - {(_optionPanelDescriptors[6].OptionPanel as TagViewModel).Name}";
            }


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
                var tag = _axisVirtual as SimpleServices.Tags.Tag;
                if (tag != null && tag.IsDeleted)
                    return;

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
                        if (!_flag)
                            args.Cancel = true;
                    }
                }
            }
        }

        private void CycleUpdateTimerHandle(object state, EventArgs e)
        {
            ApplyCommand.RaiseCanExecuteChanged();
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

        private void LanguageChanged(object sender, EventArgs e)
        {
            Title = LanguageManager.GetInstance().ConvertSpecifier("Axis Virtual Properties") + $" - {_axisVirtual.Name}";
        }
    }
}
