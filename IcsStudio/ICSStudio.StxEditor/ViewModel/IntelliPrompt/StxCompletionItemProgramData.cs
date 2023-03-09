using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ICSStudio.SimpleServices.Tags;

namespace ICSStudio.StxEditor.ViewModel.IntelliPrompt
{
    sealed class StxCompletionItemProgramData : StxCompletionItemData, IFancyCompletionItemData
    {
        public StxCompletionItemProgramData(string name):base(name)
        {
            Name = name;
        }

        //public object Description { get; }

        public override ImageSource Image => StxCompletionItemImageSourceProviders.ProgramImage;

        object IFancyCompletionItemData.Description
        {
            get
            {
                var tb = new TextBlock { Margin = new Thickness(3) };

                tb.Inlines.Add(new Run(Name) { Foreground = Brushes.Blue });
                tb.Inlines.Add(" Program");
                
                return tb;
            }
        }
    }
}
