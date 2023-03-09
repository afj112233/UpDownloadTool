using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.UIServicesPackage.View;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    class ManualAdjustVM : TabbedOptionsDialogViewModel
    {
        private readonly List<IOptionPanelDescriptor> _optionPanelDescriptors;
        private readonly DispatcherTimer _timer;
        private readonly ITag _tag;

        public ManualAdjustVM(ITag tag, string positionUnits)
        {
            if (tag == null)
                throw new ArgumentOutOfRangeException();
            _tag = tag;
            _optionPanelDescriptors = new List<IOptionPanelDescriptor>
            {
                new DefaultOptionPanelDescriptor("1", "Dynamics", "Dynamics",
                    new ManualAdjustViewModel(new ManualAdjust(), tag, positionUnits), null),
            };

            TabbedOptions.AddOptionPanels(_optionPanelDescriptors);
            Title = LanguageManager.GetInstance().ConvertSpecifier("Manual Adjust") + $" - {tag.Name}";
            _timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(400)};

            _timer.Tick += CycleUpdateTimerHandle;
            _timer.Start();

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public override void Cleanup()
        {
            _timer?.Stop();
            base.Cleanup();

            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
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
            (_optionPanelDescriptors[0].OptionPanel as ManualAdjustViewModel).Save();

            //IStudioUIService studioUIService =
            //    Package.GetGlobalService(typeof(SStudioUIService)) as IStudioUIService;
            //studioUIService?.UpdateUI();
        }

        protected override void ExecuteOkCommand()
        {
            if (CanExecuteApplyCommand())
                ExecuteApplyCommand();
            CloseAction?.Invoke();
        }

        private void CycleUpdateTimerHandle(object state, EventArgs e)
        {
            ApplyCommand.RaiseCanExecuteChanged();
        }

        private void LanguageChanged(object sender, EventArgs e)
        {
            Title = LanguageManager.GetInstance().ConvertSpecifier("Manual Adjust") + $" - {_tag.Name}";
        }
    }
}
