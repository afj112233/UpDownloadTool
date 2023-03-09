using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using ICSStudio.MultiLanguage;

namespace ICSStudio.Gui.Dialogs
{
    public class DefaultOptionPanelDescriptor : IOptionPanelDescriptor
    {
        private readonly List<IOptionPanelDescriptor> _childOptionPanelDescriptors;

        public DefaultOptionPanelDescriptor(
            string id, string title, string label,
            IOptionPanel optionPanel,
            List<IOptionPanelDescriptor> childOptionPanelDescriptors)
            : this(id, title, label, title, optionPanel, childOptionPanelDescriptors)
        {

        }

        public DefaultOptionPanelDescriptor(
            string id, string title, string label,
            string key,
            IOptionPanel optionPanel,
            List<IOptionPanelDescriptor> childOptionPanelDescriptors)
        {
            ID = id;
            Title = title;
            Label = label;
            Key = key;
            OptionPanel = optionPanel;
            optionPanel.Owner = this;
            _childOptionPanelDescriptors = childOptionPanelDescriptors;
            LanguageManager.GetInstance().SetLanguage(OptionPanel.Control as UserControl);
        }

        public string ID { get; }
        public string Title { get; }
        public string Label { get; set; }
        public string Key { get; }

        public string DisplayTitle
        {
            get
            {
                if (!string.IsNullOrEmpty(Key))
                    return LanguageManager.Instance.ConvertSpecifier(Key);

                return Title;
            }
        }

        public string DisplayLabel => !string.IsNullOrEmpty(Label)
            ? LanguageManager.Instance.ConvertSpecifier(Label)
            : DisplayTitle;

        public IEnumerable<IOptionPanelDescriptor> ChildOptionPanelDescriptors =>
            _childOptionPanelDescriptors ?? Enumerable.Empty<IOptionPanelDescriptor>();

        public IOptionPanel OptionPanel { get; }

        public bool HasOptionPanel => OptionPanel != null;
    }
}
