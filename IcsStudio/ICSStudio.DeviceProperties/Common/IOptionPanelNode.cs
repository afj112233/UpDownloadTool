using System.Collections.Generic;
using System.Windows;

namespace ICSStudio.DeviceProperties.Common
{
    public interface IOptionPanelNode
    {
        string ID { get; }
        string Title { get; }
        string Label { get; }
        object Content { get; }
        List<IOptionPanelNode> Children { get; }

        bool IsSelected { get; set; }
        bool IsExpanded { get; set; }
        bool IsActive { get; set; }
        Visibility Visibility { get; }
    }
}
