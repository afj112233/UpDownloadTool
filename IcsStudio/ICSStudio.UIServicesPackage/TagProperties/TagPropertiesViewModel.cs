using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.UIServicesPackage.TagProperties.Panel;
using ICSStudio.UIServicesPackage.View;
using Microsoft.VisualStudio.Shell;
using ICSStudio.MultiLanguage;

namespace ICSStudio.UIServicesPackage.TagProperties
{
    public class TagPropertiesViewModel : TabbedOptionsDialogViewModel
    {
        private readonly ITag _tag;
        private readonly List<IOptionPanelDescriptor> _optionPanelDescriptors;

        private readonly bool _isAoi;

        //TODO(gjc): need remove later
        private bool _isDirty = false;
        private AoiGeneralViewModel _aoiGeneralViewModel;
        private GeneralViewModel _generalViewModel;

        private bool? _dialogResult;

        public TagPropertiesViewModel(ITag tag)
        {
            _tag = tag;
            _isAoi = tag.ParentCollection.ParentProgram is AoiDefinition;

            _aoiGeneralViewModel = new AoiGeneralViewModel(new AoiGeneral(), tag);
            _generalViewModel = new GeneralViewModel(new General(), tag, tag.ParentCollection.ParentProgram);

            _optionPanelDescriptors = new List<IOptionPanelDescriptor> {GetDefaultOption()};

            TabbedOptions.AddOptionPanels(_optionPanelDescriptors);

            UpdateTitle();

            Controller controller = _tag.ParentController as Controller;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            UpdateTitle();
        }

        private DefaultOptionPanelDescriptor GetDefaultOption()
        {
            if (_isAoi)
            {
                _aoiGeneralViewModel.PropertyChanged += ViewModelChanged;
                return new DefaultOptionPanelDescriptor("1", "General", "General", _aoiGeneralViewModel, null);
            }

            _generalViewModel.PropertyChanged += ViewModelChanged;
            return new DefaultOptionPanelDescriptor("1", "General", "General", _generalViewModel, null);
        }

        private void ViewModelChanged(object sender, EventArgs e)
        {
            _isDirty = true;
            ApplyCommand.RaiseCanExecuteChanged();
        }

        protected override bool CanExecuteApplyCommand()
        {
            var result = _isAoi ? _isDirty : _generalViewModel.IsDirty;

            if (result)
            {
                // set dirty
                IProjectInfoService projectInfoService =
                    Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
                projectInfoService?.SetProjectDirty();
            }

            return result;
        }

        public void OpenParameterConnections()
        {
            ParameterConnectionsDialog parameterConnections = new ParameterConnectionsDialog(_tag)
            {
                Owner = Application.Current.MainWindow
            };
            parameterConnections.ShowDialog();
        }

        protected override void ExecuteApplyCommand()
        {
            DoApply();
            if (_generalViewModel.IsOpenParameterConnections)
            {
                OpenParameterConnections();
            }

            _isDirty = false;
            ApplyCommand.RaiseCanExecuteChanged();
        }

        private void DoApply()
        {
            if (_isAoi)
            {
                foreach (var t in _optionPanelDescriptors)
                {
                    if (((AoiGeneralViewModel) t.OptionPanel).Save())
                    {
                        //var projectInfoService =
                        //    Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
                        //projectInfoService?.Verify(_tag);
                    }
                    else
                    {
                        MessageBox.Show(((AoiGeneralViewModel) t.OptionPanel).ErrorMessage,
                            "ICSStudio", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        break;
                    }
                }
            }
            else
            {
                foreach (var t in _optionPanelDescriptors)
                {
                    if (((GeneralViewModel) t.OptionPanel).SaveOptions())
                    {
                        //var projectInfoService =
                        //    Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
                        //projectInfoService?.Verify(_tag);
                    }
                    else
                    {
                        MessageBox.Show(((GeneralViewModel) t.OptionPanel).ErrorMessage,
                            "ICSStudio", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        break;
                    }
                }
            }

            UpdateTitle();
        }

        protected override void ExecuteCancelCommand()
        {
            _dialogResult = false;
            CloseAction.Invoke();
        }

        protected override void ExecuteOkCommand()
        {
            if (CanExecuteApplyCommand())
            {
                ExecuteApplyCommand();
            }

            if (_generalViewModel.IsOpenParameterConnections)
            {
                OpenParameterConnections();
            }

            _dialogResult = true;
            CloseAction?.Invoke();
        }

        protected override void ExecuteClosingCommand(CancelEventArgs args)
        {
            if (!_dialogResult.HasValue)
            {
                Tag tag = _tag as Tag;
                if (tag != null && tag.IsDeleted)
                    return;

                if (CanExecuteApplyCommand())
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
                    }
                }
            }
        }
        //LanguageManager.GetInstance().ConvertSpecifier("New Parameter or Tag");
        private void UpdateTitle()
        {
            Title = _tag.ParentCollection.IsControllerScoped
                ? LanguageManager.GetInstance().ConvertSpecifier("Tag Properties -") + $" {_tag.Name}"
                : LanguageManager.GetInstance().ConvertSpecifier("Parameter/Local Tag Properties -") + $" {_tag.Name}";
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var generalViewModel = _optionPanelDescriptors[0].OptionPanel as GeneralViewModel;
                generalViewModel?.UpdateUI();

            });
        }
    }
}
