using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Tags;

namespace ICSStudio.StxEditor.ViewModel.IntelliPrompt
{
    internal class StxCompletionItemTagData : StxCompletionItemData, IFancyCompletionItemData
    {
        public StxCompletionItemTagData(
            Controller controller, Tag tag
        ) : base(tag.Name)
        {
            Controller = controller;
            Tag = tag;
        }

        public Controller Controller { get; }
        public Tag Tag { get; }

        public override string Description
        {
            get
            {
                if (Tag != null)
                    return Tag.Description;

                return base.Description;
            }
        }

        public override string Name
        {
            get
            {
                if (Tag != null)
                    return Tag.Name;

                return base.Name;
            }
        }

        public override ImageSource Image => StxCompletionItemImageSourceProviders.TagItemImage;

        object IFancyCompletionItemData.Description
        {
            get
            {
                if (Tag != null)
                {
                    var tb = new TextBlock {Margin = new Thickness(3)};

                    tb.Inlines.Add(new Run(Tag.DataTypeInfo.ToString()) {Foreground = Brushes.Blue});
                    tb.Inlines.Add(" Tag ");

                    if (!string.IsNullOrWhiteSpace(Tag.Description))
                    {
                        //tb.Inlines.Add(new LineBreak());
                        tb.Inlines.Add(Tag.Description);
                    }

                    return tb;
                }

                return Description;
            }
        }
    }
}