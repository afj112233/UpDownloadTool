using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.UIServicesPackage.RSTrendXProperties.Panel;
using Microsoft.VisualStudio.Shell;
// ReSharper disable PossibleNullReferenceException

namespace ICSStudio.UIServicesPackage.RSTrendXProperties
{
    public class RSTrendXPropertiesVM : TabbedOptionsDialogViewModel, ICanApply
    {
        private readonly List<IOptionPanelDescriptor> _optionPanelDescriptors;
        private readonly TrendObject _trend;
        private bool _flag = true;
        private readonly int _kind;

        public RSTrendXPropertiesVM(TrendObject trend, int kind)
        {
            if (trend == null)
                throw new ArgumentOutOfRangeException();
            _kind = kind;
            _trend = trend;
            if (kind == 0)
            {
                var penVm= new PensViewModel(new Pens(), trend);
                var generalVm = new GeneralViewModel(new General(), trend);
                penVm.CollectionChanged += generalVm.PenCollectionChanged;
                _optionPanelDescriptors = new List<IOptionPanelDescriptor>()
                {
                    new DefaultOptionPanelDescriptor("1", "Name", "Name", new NameViewModel(new Name(), trend), null),
                    new DefaultOptionPanelDescriptor("2", "General", "General",generalVm
                       , null),
                    new DefaultOptionPanelDescriptor("3", "Display", "Display",
                        new DisplayViewModel(new Display(), trend), null),
                    new DefaultOptionPanelDescriptor("4", "Curve", "Curve",penVm, null) ,
                    new DefaultOptionPanelDescriptor("5", "X-Axis", "X-Axis", new X_AxisViewModel(new X_Axis(), trend),
                        null),
                    new DefaultOptionPanelDescriptor("6", "Y-Axis", "Y-Axis", new Y_AxisViewModel(new Y_Axis(), trend),
                        null),
                    new DefaultOptionPanelDescriptor("7", "Template", "Template", new TemplateViewModel(new Template()),
                        null),
                    new DefaultOptionPanelDescriptor("8", "Sampling", "Sampling",
                        new SamplingViewModel(new Sampling(), trend),
                        null),
                    
                    //目前暂时涉及不到这2个tab页面
                    //new DefaultOptionPanelDescriptor("9", "Start Trigger", "Start Trigger",
                    //    new StartTriggerViewModel(new StartTrigger()), null),
                    //new DefaultOptionPanelDescriptor("10", "Stop Trigger", "Stop Trigger",
                    //    new StopTriggerViewModel(new StopTrigger()), null),
                };
            }
            if (kind == 1)
                _optionPanelDescriptors = new List<IOptionPanelDescriptor>()
                {
                    new DefaultOptionPanelDescriptor("1", "Y-Axis", "Y-Axis", new Y_AxisViewModel(new Y_Axis(), trend),
                        null)
                };
            if (kind == 2)
                _optionPanelDescriptors = new List<IOptionPanelDescriptor>()
                {
                    new DefaultOptionPanelDescriptor("1", "X-Axis", "X-Axis", new X_AxisViewModel(new X_Axis(), trend),
                        null),

                };
            if (kind == 3)
                _optionPanelDescriptors = new List<IOptionPanelDescriptor>()
                {
                    new DefaultOptionPanelDescriptor("1", "Pens", "Pens", new PensViewModel(new Pens(), trend), null),
                };
            TabbedOptions.AddOptionPanels(_optionPanelDescriptors);

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);

            Title = $"ICS Trace "+LanguageManager.GetInstance().ConvertSpecifier("Properties");
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            Title = $"ICS Trace " + LanguageManager.GetInstance().ConvertSpecifier("Properties");
        }

        private readonly double _maxSize = Math.Pow(2, 16);

        protected override void ExecuteApplyCommand()
        {
            if (!Check()) return;
            DoApply(true);
            ApplyCommand.RaiseCanExecuteChanged();
        }

        private void DoApply(bool showTip)
        {
            if (_kind == 0)
            {
                if ((_optionPanelDescriptors[0].OptionPanel as NameViewModel).IsDirty)
                    (_optionPanelDescriptors[0].OptionPanel as NameViewModel)?.Save();
                if ((_optionPanelDescriptors[3].OptionPanel as PensViewModel).IsDirty)
                    (_optionPanelDescriptors[3].OptionPanel as PensViewModel)?.Save();
                if ((_optionPanelDescriptors[1].OptionPanel as GeneralViewModel).IsDirty)
                    (_optionPanelDescriptors[1].OptionPanel as GeneralViewModel)?.Save();
                if ((_optionPanelDescriptors[2].OptionPanel as DisplayViewModel).IsDirty)
                    (_optionPanelDescriptors[2].OptionPanel as DisplayViewModel)?.Save();
                if ((_optionPanelDescriptors[4].OptionPanel as X_AxisViewModel).IsDirty)
                    (_optionPanelDescriptors[4].OptionPanel as X_AxisViewModel)?.Save();
                if ((_optionPanelDescriptors[5].OptionPanel as Y_AxisViewModel).IsDirty)
                    (_optionPanelDescriptors[5].OptionPanel as Y_AxisViewModel)?.Save();
                if ((_optionPanelDescriptors[7].OptionPanel as SamplingViewModel).IsDirty)
                    (_optionPanelDescriptors[7].OptionPanel as SamplingViewModel)?.Save();
                if (_trend.TimeSpan.TotalMilliseconds / _trend.SamplePeriod >= _maxSize && showTip)
                {
                    MessageBox.Show(
                        $"Setting may disrupt trend display.\nEither increase the Sample Period (>={Math.Ceiling(_trend.TimeSpan.TotalMilliseconds / _maxSize)}\nMilliseconds) or decrease the Time Span(<={Math.Floor((_maxSize * _trend.SamplePeriod / (double) 1000 / 60))}\nMinutes).",
                        "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                
            }

            if (_kind == 1)
            {
                if ((_optionPanelDescriptors[0].OptionPanel as Y_AxisViewModel).IsDirty)
                    (_optionPanelDescriptors[0].OptionPanel as Y_AxisViewModel)?.Save();
            }

            if (_kind == 2)
            {
                if ((_optionPanelDescriptors[0].OptionPanel as X_AxisViewModel).IsDirty)
                    (_optionPanelDescriptors[0].OptionPanel as X_AxisViewModel)?.Save();
                if (_trend.TimeSpan.TotalMilliseconds / _trend.SamplePeriod >= _maxSize && showTip)
                {
                    MessageBox.Show(
                        $"Setting may disrupt trend display.\nEither increase the Sample Period (>={Math.Ceiling(_trend.TimeSpan.TotalMilliseconds / _maxSize)}\nMilliseconds) or decrease the Time Span(<={Math.Floor((_maxSize * _trend.SamplePeriod / (double) 1000 / 60))}\nMinutes).",
                        "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }

            if (_kind == 3)
            {
                if ((_optionPanelDescriptors[0].OptionPanel as PensViewModel).IsDirty)
                {
                    (_optionPanelDescriptors[0].OptionPanel as PensViewModel)?.Save();
                }
            }
        }

        private bool Check()
        {
            _flag = true;
            if (_kind == 0)
            {
                if (!(_optionPanelDescriptors[0].OptionPanel as NameViewModel).Verify())
                {
                    _flag = false;
                    return _flag;
                }

                if (!(_optionPanelDescriptors[3].OptionPanel as PensViewModel).Verify())
                {
                    _flag = false;
                    return _flag;
                }

                if (!(_optionPanelDescriptors[5].OptionPanel as Y_AxisViewModel).Verify())
                {
                    _flag = false;
                    return _flag;
                }

                if (!(_optionPanelDescriptors[7].OptionPanel as SamplingViewModel).Verify())
                {
                    _flag = false;
                    return _flag;
                }
            }

            if (_kind == 1)
            {
                if (!(_optionPanelDescriptors[0].OptionPanel as Y_AxisViewModel).Verify())
                {
                    _flag = false;
                    return _flag;
                }
            }

            if (_kind == 3)
            {
                if (!(_optionPanelDescriptors[0].OptionPanel as PensViewModel).Verify())
                {
                    _flag = false;
                    return _flag;
                }
            }

            return _flag;
        }

        protected override void ExecuteOkCommand()
        {
            ExecuteApplyCommand();
            if (_flag)
                CloseAction?.Invoke();
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

        public int Apply()
        {
            if (!Check()) return -1;
            DoApply(false);
            return 0;
        }

        public bool CanApply()
        {
            return CanExecuteApplyCommand();
        }

        public override void OnClosing()
        {
            if (_kind == 0)
            {
                (_optionPanelDescriptors[3].OptionPanel as PensViewModel).IsClosing = true;
                (_optionPanelDescriptors[0].OptionPanel as NameViewModel).IsClosing = true;
                (_optionPanelDescriptors[1].OptionPanel as GeneralViewModel).IsClosing = true;
                (_optionPanelDescriptors[2].OptionPanel as DisplayViewModel).IsClosing = true;
                (_optionPanelDescriptors[4].OptionPanel as X_AxisViewModel).IsClosing = true;
                (_optionPanelDescriptors[5].OptionPanel as Y_AxisViewModel).IsClosing = true;
                //(_optionPanelDescriptors[6].OptionPanel as TemplateViewModel).IsClosing = true;
                (_optionPanelDescriptors[7].OptionPanel as SamplingViewModel).IsClosing = true;
            }
            if (_kind == 1)
                (_optionPanelDescriptors[0].OptionPanel as Y_AxisViewModel).IsClosing = true;
            if (_kind == 2)
                (_optionPanelDescriptors[0].OptionPanel as X_AxisViewModel).IsClosing = true;
            if (_kind == 3)
                (_optionPanelDescriptors[0].OptionPanel as PensViewModel).IsClosing = true;
        }
    }
}
