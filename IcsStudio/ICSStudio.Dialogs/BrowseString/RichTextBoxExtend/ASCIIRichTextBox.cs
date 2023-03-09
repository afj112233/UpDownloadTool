using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ICSStudio.Dialogs.BrowseString.RichTextBoxExtend
{
    internal class ASCIIRichTextBox : RichTextBox
    {
        public ASCIIRichTextBox()
        {
            
        }

        public static readonly DependencyProperty PropertyTypeProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(ASCIIRichTextBox), new PropertyMetadata(default(string), TextValueChanged));
        
        public string Text
        {
            get { return (string)GetValue(PropertyTypeProperty); }
            set { SetValue(PropertyTypeProperty, value); }
        }

        static void TextValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = (ASCIIRichTextBox)sender;
            textBox.Document.Blocks.Clear();
            var str = (string)e.NewValue;
            textBox.AppendText(str);
            foreach (var documentBlock in textBox.Document.Blocks)
            {
                documentBlock.TextAlignment = TextAlignment.Left;
            }
        }
    }
}
