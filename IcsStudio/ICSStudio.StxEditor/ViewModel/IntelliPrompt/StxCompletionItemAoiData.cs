using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Tags;

namespace ICSStudio.StxEditor.ViewModel.IntelliPrompt
{
    internal class StxCompletionItemAoiData : StxCompletionItemData, IFancyCompletionItemData
    {
        public StxCompletionItemAoiData(AoiDefinition aoi) : base(aoi.Name)
        {
            Controller = (Controller)aoi.ParentController;
            Aoi = aoi;
        }

        public Controller Controller { get; }
        public AoiDefinition Aoi { get; }

        public override string Description
        {
            get
            {
                if (Aoi != null)
                    return Aoi.Description;

                return base.Description;
            }
        }

        public override string Name
        {
            get
            {
                if (Aoi != null)
                    return Aoi.Name;

                return base.Name;
            }
        }

        public override ImageSource Image => StxCompletionItemImageSourceProviders.CodeSnippetItemImage;

        object IFancyCompletionItemData.Description
        {
            get
            {
                if (Aoi != null)
                {
                    var param = "";
                    foreach (var parameter in Aoi.GetParameterTags())
                    {
                        if (!(parameter.Name.Equals("EnableIn") || parameter.Name.Equals("EnableOut")))
                        {
                            if (parameter.IsRequired)
                            {
                                param = $"{param},{parameter.Name}";
                            }
                        }
                    }
                    if(param.Length>0)
                        param = param.Substring(1);
                    var tb = new TextBlock { Margin = new Thickness(3) };
                    tb.MaxWidth = StxCompletionItemData.TooltipWidth;
                    tb.Inlines.Add($" Instruction ");
                    tb.Inlines.Add(new Run(Aoi.Name) { Foreground = Brushes.Blue });
                    //tb.Inlines.Add(!string.IsNullOrEmpty(param) ? $" ({Aoi.Name},{param}) " : $" ({Aoi.Name}{param}) ");
                    var header = $" Instruction {Aoi.Name}";
                    int count = 0;
                    var autoLineBreak = MakeLineBreakWithWidth(
                        $" {header}{(!string.IsNullOrEmpty(param) ? $" ({Aoi.Name},{param}) " : $" ({Aoi.Name}{param}) ")}",
                        tb.MaxWidth, tb.FontFamily.ToString(), tb.FontSize, header.Length + 1, -1, -1, ref count);
                    tb.Inlines.Add(autoLineBreak);
                    if (!string.IsNullOrWhiteSpace(Aoi.Description))
                    {
                        //tb.Inlines.Add(new LineBreak());
                        tb.Inlines.Add(Aoi.Description);
                    }

                    return tb;
                }

                return Description;
            }
        }
    }
}
