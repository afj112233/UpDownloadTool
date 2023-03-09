using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using ICSStudio.Gui.Utils;
using ICSStudio.MultiLanguage;

namespace ICSStudio.Gui.Dialogs
{
    public class TabbedOptions : TabControl
    {
        public void AddOptionPanels(IEnumerable<IOptionPanelDescriptor> optionPanels)
        {
            if (optionPanels == null)
                throw new ArgumentNullException(nameof(optionPanels));

            foreach (IOptionPanelDescriptor descriptor in optionPanels)
            {
                if (descriptor != null)
                {
                    if (descriptor.HasOptionPanel)
                    {
                        Items.Add(new OptionTabPage(this, descriptor));
                    }

                    AddOptionPanels(descriptor.ChildOptionPanelDescriptors);
                }
            }
        }

        public event EventHandler OnDirtyChangedHandler;

        private void OnIsDirtyChanged(object sender, EventArgs e)
        {
            // throw new NotImplementedException();
            OnDirtyChangedHandler?.Invoke(this,new EventArgs());
        }

        public sealed class OptionTabPage : TabItem
        {
            // ReSharper disable once NotAccessedField.Local
            private readonly TabbedOptions _options;
            private readonly IOptionPanelDescriptor _descriptor;
            private TextBlock _placeholder;

            // ReSharper disable once InconsistentNaming
            internal IOptionPanel _optionPanel;


            public OptionTabPage(TabbedOptions options, IOptionPanelDescriptor descriptor)
            {
                _options = options;
                _descriptor = descriptor;
                Header = LanguageManager.GetInstance().ConvertSpecifier(_descriptor.Key);
                _placeholder = new TextBlock {Text = "loading..."};
                _placeholder.IsVisibleChanged += Placeholder_IsVisibleChanged;
                Content = _placeholder;
                LanguageManager.GetInstance().LanguageChanged += OptionTabPage_LanguageChanged;
            }

            private void OptionTabPage_LanguageChanged(object sender, EventArgs e)
            {
                if (Header.ToString().Contains("*"))
                {
                    Header = LanguageManager.GetInstance().ConvertSpecifier(_descriptor.Key) + "*";
                }
                else
                {
                    Header = LanguageManager.GetInstance().ConvertSpecifier(_descriptor.Key);
                }
            }

            public void CleanUp()
            {
                var defaultOptionPanelDescriptor = _descriptor as DefaultOptionPanelDescriptor;
                var vm = defaultOptionPanelDescriptor?.OptionPanel as ViewModelBase;
                LanguageManager.GetInstance().LanguageChanged -= OptionTabPage_LanguageChanged;
                vm?.Cleanup();
            }

            private void Placeholder_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(LoadPadContentIfRequired));
            }

            private void LoadPadContentIfRequired()
            {
                if (_placeholder != null && _placeholder.IsVisible)
                {
                    _placeholder = null;
                    _optionPanel = _descriptor.OptionPanel;
                    if (_optionPanel != null)
                    {
                        _optionPanel.LoadOptions();

                        ICanBeDirty dirty = _optionPanel as ICanBeDirty;
                        if (dirty != null)
                        {
                            dirty.IsDirtyChanged += _options.OnIsDirtyChanged;
                            dirty.IsDirtyChanged += OnIsDirtyChanged;
                        }


                        Content = _optionPanel.Control;
                    }
                }
            }

            private void OnIsDirtyChanged(object sender, EventArgs e)
            {
                ICanBeDirty dirty = _optionPanel as ICanBeDirty;
                if (dirty != null)
                {
                    if (dirty.IsDirty)
                    {
                        Header = LanguageManager.GetInstance().ConvertSpecifier(_descriptor.Key) + "*";
                    }
                    else
                    {
                        Header = LanguageManager.GetInstance().ConvertSpecifier(_descriptor.Key);
                    }
                }
            }
        }
    }
}
