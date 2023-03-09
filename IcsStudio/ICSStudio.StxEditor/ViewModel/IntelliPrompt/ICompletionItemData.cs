using System.Windows.Media;
using ICSStudio.AvalonEdit.Document;
using ICSStudio.AvalonEdit.Editing;

namespace ICSStudio.StxEditor.ViewModel.IntelliPrompt
{
    interface ICompletionItemData
    {
        string Name { get; }
        string Description { get; }
        ImageSource Image { get; }
        void Complete(TextArea textArea, ISegment completionSegment);
    }

    interface IFancyCompletionItemData : ICompletionItemData
    {
        new object Description { get; }
    }
}
