using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ICSStudio.Dialogs.BrowseString.RichTextBoxExtend
{
    internal static class RichTextBoxExtend
    {
        public static string GetText(this RichTextBox richTextBox)
        {
            var content = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            var str = content.Text.Replace("\n", "").Replace("\r", "").Replace("\t","$T");
            return str;
        }
    }
}
