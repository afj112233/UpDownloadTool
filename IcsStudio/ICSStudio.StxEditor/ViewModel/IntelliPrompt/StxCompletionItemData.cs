using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using ICSStudio.AvalonEdit.Document;
using ICSStudio.AvalonEdit.Editing;

namespace ICSStudio.StxEditor.ViewModel.IntelliPrompt
{
    internal class StxCompletionItemData : ICompletionItemData
    {
        public const double TooltipWidth = 722;
        private string _description = string.Empty;
        private string _name = string.Empty;

        protected StxCompletionItemData()
        {
        }

        public StxCompletionItemData(string name)
        {
            _name = name;
        }

        public StxCompletionItemData(string name, string description)
            : this(name)
        {
            _description = description;
        }

        public virtual string Name
        {
            get { return _name; }
            protected set { _name = value; }
        }

        public virtual string Description
        {
            get { return _description; }
            protected set { _description = value; }
        }

        public virtual ImageSource Image { get; protected set; }
        
        public virtual void Complete(TextArea textArea, ISegment completionSegment)
        {
            textArea.Document.Replace(completionSegment, Name);
        }

        protected string MakeLineBreakWithWidth(string text, double width, string fontFamily, double fontSize,int ignoreLength,int boldStart,int boldLength,ref int lineChangedCount)
        {
            width -= fontSize * 3;
            var measureTextWidth = MeasureTextWidth(text,fontFamily,fontSize,boldStart,boldLength);
            List<string> lines = new List<string>();
            if (measureTextWidth > width)
            {
                while (!string.IsNullOrEmpty(text))
                {
                    if (measureTextWidth < 0)
                    {
                        measureTextWidth = MeasureTextWidth(text, fontFamily, fontSize, boldStart, boldLength);
                    }
                    if (measureTextWidth > width)
                    {
                        var length = (int)Math.Floor(text.Length * (width / measureTextWidth));
                        var line = Adjust(ref length, text, width, fontFamily, fontSize, boldStart, boldLength);
                        if (ignoreLength > 0)
                        {
                            if (ignoreLength > line.Length)
                            {
                                ignoreLength -= line.Length;
                                boldStart -= length;
                                continue;
                            }
                            else
                            {
                                boldStart -= ignoreLength;
                                line = line.Substring(ignoreLength);
                                ignoreLength = 0;
                            }
                        }
                        lines.Add(line);
                        boldStart -= line.Length;
                        if (boldStart > 0)
                            lineChangedCount++;
                        text = text.Substring(length);
                        measureTextWidth = -1;
                    }
                    else
                    {
                        lines.Add(text);
                        break;
                    }
                }
            }
            else
            {
                return text.Substring(ignoreLength);
            }

            return string.Join("\n", lines);
        }

        private string Adjust(ref int offset, string text, double maxWidth, string fontFamily, double fontSize, int boldStart, int boldLength)
        {
            var lineText = text.Substring(0, offset);
            var desiredWidth = MeasureTextWidth(lineText,fontFamily,fontSize, boldStart, boldLength);
            if (desiredWidth > maxWidth)
            {
                while (desiredWidth > maxWidth)
                {
                    offset = offset - 1;
                    if (offset < 0) return lineText;
                    lineText = text.Substring(0, offset);
                    desiredWidth = MeasureTextWidth(lineText, fontFamily, fontSize, boldStart, boldLength);
                }
            }
            else
            {
                while (Math.Abs(desiredWidth - maxWidth) > fontSize)
                {
                    offset = offset + 1;
                    if (offset > text.Length) return lineText;
                    lineText = text.Substring(0, offset);
                    desiredWidth = MeasureTextWidth(lineText, fontFamily, fontSize, boldStart, boldLength);
                }
            }

            return lineText;
        }

        private double MeasureTextWidth(string text,string fontFamily,double fontSize,int boldStart,int boldLength)
        {
            FormattedText formattedText = new FormattedText(
                text,
                System.Globalization.CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface(fontFamily),
                fontSize,
                Brushes.Black
            );
            if (boldStart + boldLength > text.Length)
            {

            }else
            if (boldStart > -1)
                formattedText.SetFontWeight(FontWeights.Bold, boldStart, boldLength);
            return formattedText.WidthIncludingTrailingWhitespace;
        }
    }
}