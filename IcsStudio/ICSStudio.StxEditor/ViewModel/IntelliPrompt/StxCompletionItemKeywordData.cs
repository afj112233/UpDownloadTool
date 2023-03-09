using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ICSStudio.StxEditor.ViewModel.IntelliPrompt
{
    internal class StxCompletionItemKeywordData : StxCompletionItemData, IFancyCompletionItemData
    {
        private string _typeName="";
        public StxCompletionItemKeywordData(string keyword,string typeName= "Keyword") : base(keyword)
        {
            Keyword = keyword;
            _typeName = "  "+typeName;
        }

        public string Keyword { get; }

        public override ImageSource Image => StxCompletionItemImageSourceProviders.KeywordItemImage;

        object IFancyCompletionItemData.Description
        {
            get
            {
                var tb = new TextBlock {Margin = new Thickness(3)};

                tb.Inlines.Add(new Run(Keyword) {Foreground = Brushes.Blue});
                tb.Inlines.Add(_typeName);

                return tb;
            }
        }
    }
}
