using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Instruction;

namespace ICSStudio.StxEditor.ViewModel.IntelliPrompt
{
    internal class StxCompletionItemTrack: StxCompletionItemData, IFancyCompletionItemData
    {
        private IXInstruction _instr;
        private int _index;
        private StxCompletionItemData _instrCodeSnippetData;
        private List<Tuple<string, IDataType>> _parameters;
        public StxCompletionItemTrack(IXInstruction instruction, StxCompletionItemData instrCodeSnippetData, int index)
        {
            _instr = instruction;
            _instrCodeSnippetData = instrCodeSnippetData;
            _parameters = _instr.GetParameterInfo();
            _index = index;
        }

        private string[] GetParameters()
        {
            var instr = _instrCodeSnippetData as StxCompletionItemCodeSnippetData;
            if (instr != null)
            {
                return instr.Parameters.Split(',');
            }

            return _instr.GetParameterInfo().Select(p => p.Item1).ToArray();
        }

        object IFancyCompletionItemData.Description
        {
            get
            {
                var tb = new TextBlock() { Margin = new Thickness(3), MaxWidth = TooltipWidth };
                tb.Inlines.Add("Instruction ");
                tb.Inlines.Add(new Run(_instr.Name) { Foreground = Brushes.Blue });
                tb.Inlines.Add("(");
                var parameters = GetParameters();
                var header = $"Instruction {_instr.Name}(";
                string parametersStr = "",targetParam="";
                int offset = 0;
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (i == _index)
                    {
                        offset = parametersStr.Length;
                        targetParam = $"{parameters[i]}{(i == parameters.Length - 1 ? "" : ",")}";
                        parametersStr = $"{parametersStr}{targetParam}";
                        //tb.Inlines.Add(new Bold(new Run($"{parameters[i]}{(i == parameters.Length - 1 ? "" : ",")}")));
                    }
                    else
                    {
                        parametersStr = $"{parametersStr}{parameters[i]}{(i == parameters.Length - 1 ? "" : ",")}";
                        //tb.Inlines.Add($"{parameters[i]}{(i == parameters.Length - 1 ? "" : ",")}");
                    }
                }

                parametersStr += ")";
                int count = 0;
                var adjustStr=MakeLineBreakWithWidth($"{header}{parametersStr}", TooltipWidth, tb.FontFamily.ToString(), tb.FontSize,
                    header.Length,offset+header.Length,targetParam.Length,ref count);
                offset += count;
                tb.Inlines.Add(adjustStr.Substring(0,offset));
                tb.Inlines.Add(new Bold(new Run(adjustStr.Substring(offset,targetParam.Length))));
                tb.Inlines.Add(adjustStr.Substring(offset+targetParam.Length));
                
                tb.Inlines.Add(new LineBreak());
                tb.Inlines.Add(new Run((_instrCodeSnippetData as StxCompletionItemCodeSnippetData)?.InstrDescription ??
                                       (_instrCodeSnippetData as StxCompletionItemAoiData)?.Description));
                if (_index < _parameters.Count)
                {
                    tb.Inlines.Add(new LineBreak());
                    tb.Inlines.Add(new LineBreak());
                    var parameter = _parameters[_index];
                    tb.Inlines.Add(new Bold(new Run($"{parameter.Item1}:") { FontStyle = FontStyles.Italic }));
                    tb.Inlines.Add(new Run($"Enter operand of type {parameter.Item2}")
                        { FontStyle = FontStyles.Italic });
                }
                return tb;
            }
        }
    }
}
