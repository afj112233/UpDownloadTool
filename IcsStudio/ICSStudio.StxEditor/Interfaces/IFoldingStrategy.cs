using ICSStudio.AvalonEdit.Document;
using ICSStudio.AvalonEdit.Folding;

namespace ICSStudio.StxEditor.Interfaces
{
    interface IFoldingStrategy
    {
        void UpdateFoldings(FoldingManager manager, TextDocument document);
    }
}
