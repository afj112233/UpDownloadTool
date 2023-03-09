using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ICSStudio.StxEditor.ViewModel.IntelliPrompt
{
    internal class StxCompletionItemCodeSnippetData : StxCompletionItemData, IFancyCompletionItemData
    {
        private const string codeSnippetNote = "Note: Tab after inserting snippet shortcut to activate snippet";
        private readonly string _description;
        private readonly string _parameters;
        private readonly string _type;
        public StxCompletionItemCodeSnippetData(string name, string description) : base(name)
        {
            InstrDescription = description;
            _description = string.IsNullOrWhiteSpace(description) ? $"Code snippet for {name}" : description;
        }

        public StxCompletionItemCodeSnippetData(string name, string description, string parameters, string type) : base(name)
        {
            InstrDescription = description;
            _description = string.IsNullOrWhiteSpace(description) ? $"Code snippet for {name}" : description;
            Parameters = parameters;
            _parameters = string.IsNullOrWhiteSpace(parameters) ? $"Function {name}(Source) " : parameters;
            _type = string.IsNullOrWhiteSpace(type) ? "Function" : type;
        }

        public string InstrDescription { get; }

        public string Parameters { get; }

        public override ImageSource Image => StxCompletionItemImageSourceProviders.CodeSnippetItemImage;

        public override string Description => _description;

        public bool IsInstructionOrFunction
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_type) && string.IsNullOrWhiteSpace(_parameters)) return false;
                return true;
            }
        }

        object IFancyCompletionItemData.Description
        {
            get
            {
                var tb = new TextBlock {Margin = new Thickness(3), MaxWidth = TooltipWidth};
                if (string.IsNullOrWhiteSpace(_type) && string.IsNullOrWhiteSpace(_parameters))
                {
                    tb.Inlines.Add(new Run(Name) { Foreground = Brushes.Blue });
                    tb.Inlines.Add(" Code Snippet");

                    tb.Inlines.Add(new LineBreak());
                    tb.Inlines.Add(Description);

                    tb.Inlines.Add(new LineBreak());
                    tb.Inlines.Add(new Run(codeSnippetNote) { FontStyle = FontStyles.Italic });

                }
                else
                {
                    tb.Inlines.Add(_type+" ");
                    tb.Inlines.Add(new Run(Name) { Foreground = Brushes.Blue });
                    tb.Inlines.Add($"({_parameters})");

                    tb.Inlines.Add(new LineBreak());
                    tb.Inlines.Add(Description);
                }
                return tb;
            }
        }

    }
}
