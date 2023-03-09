using System.Collections.Generic;

namespace ICSStudio.Gui.Dialogs
{
    public interface IOptionPanelDescriptor
    {
        string ID { get; }
        string Title { get; }
        string Label { get; }
        string Key { get; }

        string DisplayTitle { get; }
        string DisplayLabel { get; }

        IEnumerable<IOptionPanelDescriptor> ChildOptionPanelDescriptors { get; }
        IOptionPanel OptionPanel { get; }

        bool HasOptionPanel { get; }
    }
}
