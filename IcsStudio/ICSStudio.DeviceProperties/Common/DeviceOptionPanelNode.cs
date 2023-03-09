using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using GalaSoft.MvvmLight;
using ICSStudio.MultiLanguage;

namespace ICSStudio.DeviceProperties.Common
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal class DeviceOptionPanelNode : ViewModelBase, IOptionPanelNode
    {
        private readonly string _title;
        private bool _isSelected;
        private bool _isExpanded;

        public DeviceOptionPanelNode(
            string id, string title,
            string label = "",
            IOptionPanel optionPanel = null,
            List<IOptionPanelNode> childOptionPanels = null)
        {
            ID = id;
            _title = title;
            Label = label;
            OptionPanel = optionPanel;
            Children = childOptionPanels;

            if (OptionPanel != null)
                OptionPanel.IsDirtyChanged += OptionPanelOnIsDirtyChanged;

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", DeviceOptionPanelNode_LanguageChanged);
        }

        private void OptionPanelOnIsDirtyChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("Title");
        }

        private void DeviceOptionPanelNode_LanguageChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(Title));
            RaisePropertyChanged(nameof(Label));
        }

        public override void Cleanup()
        {
            base.Cleanup();
            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
                LanguageManager.GetInstance(), "LanguageChanged", DeviceOptionPanelNode_LanguageChanged);
        }

        public IOptionPanel OptionPanel { get; }

        public string ID { get; }

        public string Title
        {
            get
            {
                var title = LanguageManager.GetInstance().ConvertSpecifier(_title);
                if (string.IsNullOrEmpty(title)) title = _title;
                if (OptionPanel != null && OptionPanel.IsDirty)
                    return title + "*";

                return title;
            }
        }

        public string Label { get; }

        public object Content => OptionPanel?.Control;
        public List<IOptionPanelNode> Children { get; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                Set(ref _isSelected, value);
                if (value)
                {
                    OptionPanel?.Show();
                }
                else
                {
                    OptionPanel?.Hide();
                }
            }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { Set(ref _isExpanded, value); }
        }

        public bool IsActive { get; set; }

        public Visibility Visibility
        {
            get
            {
                if (OptionPanel != null)
                    return OptionPanel.Visibility;

                return Visibility.Visible;
            }
        }

    }
}
