using System;
using System.Windows.Media;
using ICSStudio.AvalonEdit.CodeCompletion;
using ICSStudio.AvalonEdit.Document;
using ICSStudio.AvalonEdit.Editing;

namespace ICSStudio.StxEditor.ViewModel.IntelliPrompt
{
    internal class StxCompletionItem : ICompletionData
    {
        private readonly ICompletionItemData _itemData;
        private readonly IFancyCompletionItemData _fancyItemData;

        public StxCompletionItem(ICompletionItemData itemData)
        {
            _itemData = itemData;
            _fancyItemData = itemData as IFancyCompletionItemData;
        }

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            _itemData.Complete(textArea, completionSegment);
        }

        public ImageSource Image => _itemData.Image;
        public string Text => _itemData.Name;
        public object Content => _itemData.Name;

        public object Description
        {
            get
            {
                if (_fancyItemData != null)
                    return _fancyItemData.Description;

                return _itemData.Description;
            }
        }

        public double Priority => 0;
    }
}
