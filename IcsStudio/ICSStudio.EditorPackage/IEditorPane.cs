using System;
using System.Windows.Controls;

namespace ICSStudio.EditorPackage
{
    public interface IEditorPane
    {
        string Caption { get; }
        UserControl Control { get; }
        Action CloseAction { get; set; }
        Action<string> UpdateCaptionAction { get; set; }
    }
}