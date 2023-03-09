using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using ICSStudio.AvalonEdit;
using ICSStudio.AvalonEdit.Document;
using ICSStudio.AvalonEdit.Variable;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.Tags;

namespace ICSStudio.StxEditor.ViewModel.CodeSnippets
{
    internal static class GetSnippetByPosition
    {
        //private static bool _isErrorMatch = false;
        //private static string _functionMatchStr = "(^| |\t|\n|;){0}(( )|\n|\r\n|\t|$)";
        //private static string _endFunctionMatchStr = "(^| |\t|\n|;){0}(;|( )|\n|\r\n|\t|$)";

        public static string[] Keyword =
        {
            "if", "then", "elsif", "else", "end_if", "case", "of", "end_case", "for", "to", "by", "do", "end_for",
            "while", "exit", "do", "end_while", "repeat", "until", "end_repeat"
        };

        public static string[] MathStatusFlag = {"S:FS", "S:N", "S:Z", "S:V", "S:C", "S:MINOR"};

        public static bool IsMathStatusFlag(string code)
        {
            if (string.IsNullOrEmpty(code)) return false;
            foreach (var s in MathStatusFlag)
            {
                if (s.Equals(code.Trim(), StringComparison.OrdinalIgnoreCase)) return true;
            }

            return false;
        }

        public static string GetIdOrInstrRegex()
        {
            //string idOrEnums = @"((\\|\%)?\b([A-Za-z_][:A-Za-z0-9_\.\[\]\(\)]*)(?!=))";
            //var instr = $@"({string.Join("|", GetInstructions())})[ ]*\(({idOrEnums})?\)?";
            //return $"({instr}|{idOrEnums})(?!=)";
            var instrs = string.Join("|", GetInstructions());
            var regex = $"(?<=\b({instrs})[ ]*)";
            return @"((((" + regex +
                   @")|((\\|\%)?\b([A-Za-z_][:A-Za-z0-9_\.\,\[\]]*))(\([_A-Za-z0-9 \,\(\)_\-\.\%\[\]\:\\]*\))?))|(\\|\%)?\b(?<!#)[A-Za-z_]+[:_A-Za-z0-9\.]*\b(\[[\[\]\-\,A-Za-z0-9_\\\.]+(\,[\[\]\,\(\)A-Za-z0-9_\\]+){0,}\])?(\.(((\\)?[A-Za-z_]+[_A-Za-z0-9]*(\[[\[\]\,\(\)A-Za-z0-9_\\]+(\,(\\)?[\[\]\,\(\)A-Za-z0-9_\\]+){0,}\])?)|[0-9]+))*)";
        }

        public static bool IsInstr(string name, ref string instrName)
        {
            if (name.IndexOf("(") > 0)
            {
                instrName = name.Substring(0, name.IndexOf("("));
                return GetInstructions().Contains(instrName, StringComparer.OrdinalIgnoreCase);
            }

            return GetInstructions().Contains(name, StringComparer.OrdinalIgnoreCase);
        }

        public static DocumentLine GetPosLine(TextEditor editor)
        {
            int pos = editor.SelectionStart;
            foreach (var line in editor.Document.Lines)
            {
                if (line.Offset <= pos && line.EndOffset >= pos)
                    return line;
            }

            Debug.Assert(false);
            return null;
        }

        public static string[] GetParentCode(TextEditor editor, ref int offset, bool isGetWholeCode = false)
        {
            List<string> endSign = new List<string>() {" ", "\n", "\t", "=", "+", "*", "-", "&", ";", "\r","," };
            bool flag = false;
            string code = "";
            string text = editor.Text;
            offset = editor.SelectionStart - 1;
            if (isGetWholeCode)
            {
                offset += 1;
            }

            for (int i = offset + 1; i < text.Length; i++)
            {
                if (text[i] == ' ') continue;
                if (text[i] == ')') flag = true;
                else break;
            }

            if (flag) endSign.Add("(");
            int rBrackets = 0;
            if (!isGetWholeCode && text[offset] != '.') return null;
            offset--;
            while (offset > -1)
            {
                string letter = text.Substring(offset, 1);
                if (endSign.Contains(letter))
                {
                    if (rBrackets <= 0)
                        break;
                    code = letter + code;
                }
                else
                {
                    if (letter == "[")
                    {
                        if (rBrackets == 0)
                        {
                            break;
                        }
                        else
                        {
                            rBrackets--;
                        }
                    }

                    if (letter == "]") rBrackets++;
                    code = letter + code;
                }

                offset--;
            }

            offset++;
            int p = code.IndexOf("[");
            while (p > -1)
            {
                int p2 = code.IndexOf("]");
                if (p2 < p) return null;
                code.Remove(p, p2 - p + 1);
                p = code.IndexOf("[", p2 + 1);
            }

            return code.Split('.');
        }

        public static bool TryGetInstr(string code, ref int offset,ref string instr)
        {
            code = code.Substring(0, offset--);
            code = RoutineCodeTextExtension.ConvertCommentToWhiteBlank(code,null);
            bool isStartGetParam = false;
            bool isStartInstrName = false;
            int parenthesesCount = 0;
            for (; offset >= 0; offset--)
            {
                var c = code[offset];
                if (char.IsLetter(c) || char.IsWhiteSpace(c) || ',' == c || char.IsNumber(c) || '[' == c || ']' == c || '\r' == c || '\n' == c)
                {
                    if (isStartInstrName)
                    {
                        if (!char.IsLetter(c))
                        {
                            offset++;
                            break;
                        }
                    }
                    isStartGetParam = true;
                    instr = $"{c}{instr}";
                    continue;
                }

                if (';' == c)
                {
                    offset++;
                    break;
                }
                if (')' == c)
                {
                    if (isStartGetParam)
                    {
                        parenthesesCount++;
                        instr = $"{c}{instr}";
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }

                if ('(' == c)
                {
                    if (parenthesesCount == 0)
                    {
                        isStartInstrName = true;
                        instr = $"{c}{instr}";
                        continue;
                    }
                    else
                    {
                        parenthesesCount--;
                        instr = $"{c}{instr}";
                        continue;
                    }
                }
            }

            offset = Math.Max(0, offset);
            return isStartInstrName;
        }

        public static SnippetInfo GetOtherInfo(DocumentLine currentLine, int offset, string codeSnippet)
        {
            if (offset >= codeSnippet.Length) return null;
            string fontStr = "";
            string backStr = "";
            int startOffset = -1;
            Regex regex = new Regex(@"[\%#_A-Za-z0-9\\\.:]");
            bool flag = false;
            bool flag2 = false;
            bool flag3 = true;
            bool isNeg = false;
            if (currentLine.Offset < offset)
                for (int i = offset - 1; i >= currentLine.Offset; i--)
                {

                    if (flag3 && (codeSnippet[i] == '+' || codeSnippet[i] == '-'))
                    {
                        if (codeSnippet[i] == '-') isNeg = true;
                        flag2 = true;
                        continue;
                    }

                    if (flag2)
                    {
                        if (codeSnippet[i] == 'e' || codeSnippet[i] == 'E')
                        {
                            startOffset -= 2;
                            fontStr = (isNeg ? "e-" : "e+") + fontStr;
                            flag2 = false;
                            continue;
                        }

                        break;
                    }

                    if (codeSnippet[i] == '-')
                    {
                        flag = true;
                        startOffset = i;
                    }
                    else if (flag)
                    {
                        if (codeSnippet[i] == '\t' || codeSnippet[i] == ' ')
                        {
                            continue;
                        }

                        if (regex.IsMatch(codeSnippet[i].ToString()))
                        {
                            startOffset++;
                            flag = false;
                        }

                        break;
                    }
                    else if (regex.IsMatch(codeSnippet[i].ToString()))
                    {
                        fontStr = $"{codeSnippet[i]}" + fontStr;
                        if (codeSnippet[i] == '.') flag3 = false;
                        startOffset = i;
                    }
                    else
                    {
                        startOffset = i + 1;
                        break;
                    }

                    if (i - 1 == currentLine.Offset) startOffset = currentLine.Offset;
                }

            bool isScientificNotation = false;
            var endOffset = offset;
            for (; endOffset < currentLine.EndOffset; endOffset++)
            {
                if (!isScientificNotation && (codeSnippet[endOffset] == 'e' || codeSnippet[endOffset] == 'E'))
                {
                    isScientificNotation = true;
                    continue;
                }

                if (isScientificNotation)
                {
                    if (codeSnippet[endOffset] == '+' || codeSnippet[endOffset] == '-')
                    {
                        if (codeSnippet[endOffset] == '-')
                            backStr += "e-";
                        else
                            backStr += "e+";
                        isScientificNotation = false;
                        continue;
                    }
                    else
                    {
                        backStr += codeSnippet[endOffset - 1];
                        isScientificNotation = false;
                    }
                }

                if (regex.IsMatch(codeSnippet[endOffset].ToString()))
                {
                    if (startOffset == -1) startOffset = offset;
                    backStr = backStr + $"{codeSnippet[endOffset]}";
                }
                else
                {
                    break;
                }
            }
            
            if (isScientificNotation && endOffset == currentLine.EndOffset)
            {
                backStr += codeSnippet[endOffset - 1];
            }

            string joinStr = (flag ? "-" : "") + fontStr + backStr;
            int distance = endOffset - startOffset;
            foreach (var snippetInfo in GetAllSnippetLocation(joinStr, codeSnippet))
            {
                if (snippetInfo.Offset <= distance && snippetInfo.EndOffset >= distance)
                    return ReCheckCoverStr(offset,
                        new SnippetInfo(codeSnippet) {CodeText = snippetInfo.CodeText, Offset = startOffset});
            }

            return ReCheckCoverStr(offset, new SnippetInfo(codeSnippet) {CodeText = joinStr, Offset = startOffset});
        }

        public static SnippetInfo ReCheckCoverStr(int mousePos, SnippetInfo snippetInfo)
        {
            if (snippetInfo.CodeText.IndexOf(",") > -1 && snippetInfo.CodeText.IndexOf("(") == -1 &&
                snippetInfo.CodeText.IndexOf("[") == -1)
            {
                var offset = mousePos;
                offset = Math.Abs(snippetInfo.Offset - offset);
                offset = offset >= snippetInfo.CodeText.Length ? snippetInfo.CodeText.Length - 1 : offset;
                string fornt = "", back = "";
                int newOffset = 0;
                for (int i = offset; i > -1; i--)
                {
                    if (snippetInfo.CodeText[i] == ',')
                    {
                        fornt = snippetInfo.CodeText.Substring(i + 1, offset - i);
                        newOffset = i + 1;
                        break;
                    }

                    if (i == 0)
                    {
                        fornt = snippetInfo.CodeText.Substring(0, offset + 1);
                        newOffset = 0;
                    }
                }

                for (int i = offset + 1; i < snippetInfo.CodeText.Length; i++)
                {
                    if (snippetInfo.CodeText[i] == ',')
                    {
                        back = snippetInfo.CodeText.Substring(offset + 1, i - offset - 1);
                        break;
                    }

                    if (i == snippetInfo.EndOffset)
                    {
                        back = snippetInfo.CodeText.Substring(offset + 1, i - offset);
                        offset = 0;
                    }
                }

                snippetInfo.CodeText = fornt + back;
                snippetInfo.Offset = snippetInfo.Offset + newOffset;
            }

            if (snippetInfo.CodeText.EndsWith(":"))
                snippetInfo.CodeText = snippetInfo.CodeText.Substring(0, snippetInfo.CodeText.Length - 1);
            return snippetInfo;
        }

        public static bool IsInComment(string code, int position)
        {
            code = RoutineCodeTextExtension.RemoveRegion(code);
            Regex regex = new Regex(@"/\*|\(\*|//");
            int p2 = 0;
            foreach (Match match in regex.Matches(code))
            {
                if (match.Index < p2) continue;
                if (match.Index > position) return false;
                if (match.Value == "//")
                {
                    code = RoutineCodeTextExtension.RemoveComment2(code, match.Index, ref p2);
                    if (p2 >= position && match.Index <= position) return true;
                    if (match.Index > position) return false;
                }
                else
                {
                    code = RoutineCodeTextExtension.RemoveComment1(code, match.Index, match.Value, ref p2);
                    if (p2 >= position && match.Index <= position) return true;
                    if (match.Index > position) return false;
                }
            }

            return false;
        }

        public static string GetFontCode(TextEditor textEditor,ref int lengthToKeyword)
        {
            var line = GetPosLine(textEditor);
            if (line.Length <= 0) return string.Empty;
            string snippet = textEditor.Text.Substring(line.Offset, line.Length);
            int offset = textEditor.SelectionStart - line.Offset;
            Regex regex = new Regex(GetIdOrInstrRegex(), RegexOptions.IgnoreCase);
            var matches = regex.Matches(snippet);
            for (int i = matches.Count - 1; i >= 0; i--)
            {
                var match = matches[i];
                if (match.Index + match.Length-1 <= offset)
                {
                    lengthToKeyword = offset - match.Index ;
                    return matches[i].Value;
                }
            }

            return string.Empty;
        }

        public static string GetParserCode(string code)
        {
            return code.Replace("\n", " ").Replace("\r", " ").Replace("\t", " ");
        }

        public static bool IsInRegionComment(string code, int pos)
        {
            Regex regex = new Regex(@"\b(2#[01]+|8#[0-7]+|[0-9]+|16#[0-9A-Fa-f]+)");
            var matched = regex.Match(code);
            if (matched.Index <= pos && matched.Index + matched.Length >= pos) return false;

            regex = new Regex("#(region|endregion).*",RegexOptions.IgnoreCase);
            var matches = regex.Matches(code);
            foreach (Match match in matches)
            {
                if (match.Index > pos) return false;
                if (match.Index <= pos && match.Index + match.Length >= pos) return true;
            }

            return false;
        }

        public static bool IsEnterInstrEnum(InstrEnumInfo instrEnumInfo, TextEditor editor, STRoutine parent)
        {
            int offset = editor.SelectionStart - 1;
            string codeText = editor.Text;
            string instrName = "";
            int pos = -1;
            bool getName = false, startGet = false, isInstr = false;
            for (int i = offset; i > -1; i--)
            {
                if (codeText[i] == ',')
                {
                    pos++;
                    continue;
                }

                if (codeText[i] == ';')
                {
                    return false;
                }

                if (codeText[i] == '(')
                {
                    getName = true;
                    isInstr = true;
                    continue;
                }

                if (getName)
                {
                    if (char.IsLetter(codeText[i]))
                    {
                        instrName = codeText[i] + instrName;
                        startGet = true;
                    }
                    else
                    {
                        if (startGet) break;
                    }
                }
            }

            if (isInstr)
            {
                instrEnumInfo.InstrName = instrName;
                instrEnumInfo.InstrEnumPosition = pos + 1;
                if (!instrEnumInfo.GetEnum(parent)) return false;
            }

            return isInstr;
        }
        
        public static SnippetInfo GetVariableInfo(TextEditor textEditor, TextLocation logicalPosition)
        {
            var offset = textEditor.Document.GetOffset(logicalPosition);
            var variableInfos = textEditor.TextArea.TextView.VariableInfos?.ToList();
            var variableInfo =
                variableInfos?.FirstOrDefault(v => v.IsHit(offset));
            var coverStr = new SnippetInfo(textEditor.Document.Text);
            if (variableInfo == null)
            {
                coverStr = GetSnippetByPosition.GetOtherInfo(textEditor.Document.Lines[logicalPosition.Line - 1],
                    offset,
                    textEditor.Document.Text);
            }
            else
            {
                if(((VariableInfo)variableInfo).IsCorrect)
                {
                    coverStr.CodeText = variableInfo.Name;
                    coverStr.Offset = variableInfo.Offset;
                    coverStr.IsRoutine = variableInfo.IsRoutine;
                    coverStr.IsEnum = variableInfo.IsEnum;
                    coverStr.IsProgram = variableInfo.IsProgram;
                    coverStr.IsModule = variableInfo.IsModule;
                    coverStr.IsTask = variableInfo.IsTask;
                    coverStr.IsAOI = variableInfo.IsAOI;
                    coverStr.IsInstr = variableInfo.IsInstr;
                    coverStr.IsNum = variableInfo.IsNum;
                    coverStr.IsUnknown = variableInfo.IsUnknown;
                    coverStr.AddVariable((VariableInfo) variableInfo);
                }
                else
                {
                    var astName = ((VariableInfo) variableInfo).AstNode as ASTName;
                    if (astName != null)
                    {
                        coverStr= GetExact(astName, offset, textEditor.Document.Text);
                        coverStr.AddVariable((VariableInfo)variableInfo);
                        return coverStr;
                    }
                }
            }

            return coverStr;
        }

        private static SnippetInfo GetExact(ASTName astName,int offset,string parent)
        {
            foreach (var astNode in astName.id_list.nodes)
            {
                if(astNode.ContextStart<=offset&&astNode.ContextEnd>=offset)
                {
                    var astNameItem = astNode as ASTNameItem;
                    if (astNameItem != null)
                    {
                        if (astNameItem.arr_list != null)
                        {
                            foreach (var node in astNameItem.arr_list.nodes)
                            {
                                var subAstName = node as ASTName;
                                if(subAstName!=null)
                                if (subAstName.ContextStart <= offset && subAstName.ContextEnd >= offset)
                                {
                                    return GetExact(subAstName, offset, parent);
                                }
                            }
                        }

                        return new SnippetInfo(parent) {CodeText = astNameItem.id,Offset = astNameItem.ContextStart,IsCurrent = false,IsAstNameNode = true};
                    }
                }
            }
            return new SnippetInfo(parent) { CodeText = ObtainValue.GetAstName(astName), Offset = astName.ContextStart,IsCurrent = false, IsAstNameNode = true };
        }

        private static IList<SnippetInfo> GetAllSnippetLocation(string codeSnippet, string parent)
        {
            var oldCodeSnippet = codeSnippet;
            List<SnippetInfo> locationList = new List<SnippetInfo>();
            Regex regex =
                new Regex(GetIdOrInstrRegex(), RegexOptions.IgnoreCase);
            var matchCollection = regex.Matches(codeSnippet);
            while (matchCollection.Count > 0)
            {
                SnippetInfo snippetInfo = new SnippetInfo(parent);
                var match = matchCollection[matchCollection.Count - 1];
                snippetInfo.Offset = match.Index;
                snippetInfo.CodeText = oldCodeSnippet.Substring(match.Index, match.Length);
                locationList.Add(snippetInfo);
                codeSnippet = codeSnippet.Remove(snippetInfo.Offset, snippetInfo.CodeText.Length)
                    .Insert(snippetInfo.Offset, GetRepeatWord(snippetInfo.CodeText.Length, "0"));
                matchCollection = regex.Matches(codeSnippet);
            }

            return locationList;
        }

        private static string GetRepeatWord(int length, string wold)
        {
            string repeatWord = "";
            while (length > 0)
            {
                repeatWord = repeatWord + wold;
                length--;
            }

            return repeatWord;
        }

        private static List<string> _instrList = null;

        private static List<string> GetInstructions()
        {
            //var dataList = new List<string>();
            if (_instrList != null)
            {
                return _instrList;
            }

            _instrList = new List<string>();
            _instrList.Add("ABS");
            _instrList.Add("ACOS");
            _instrList.Add("ALM");
            _instrList.Add("ALMA");
            _instrList.Add("ALMD");
            _instrList.Add("ASIN");
            _instrList.Add("ATAN");
            _instrList.Add("BTDT");

            _instrList.Add("CC");
            _instrList.Add("CONCAT");
            _instrList.Add("COP");
            _instrList.Add("COS");
            _instrList.Add("CPS");
            _instrList.Add("CTUD");

            _instrList.Add("D2SD");
            _instrList.Add("D3SD");
            _instrList.Add("DEDT");
            _instrList.Add("DEG");
            _instrList.Add("DELETE");
            _instrList.Add("DERV");
            _instrList.Add("DFF");
            _instrList.Add("DTOS");

            _instrList.Add("EOT");
            _instrList.Add("ESEL");
            _instrList.Add("EVENT");

            _instrList.Add("FGEN");
            _instrList.Add("FIND");

            _instrList.Add("HLL");
            _instrList.Add("HMIBC");
            _instrList.Add("HPF");

            _instrList.Add("IMC");
            _instrList.Add("INSERT");
            _instrList.Add("INTG");
            _instrList.Add("IOT");

            _instrList.Add("JKFF");
            _instrList.Add("JSR");

            _instrList.Add("LDL2");
            _instrList.Add("LDLG");
            _instrList.Add("LN");
            _instrList.Add("LOG");
            _instrList.Add("LOWER");
            _instrList.Add("LPF");

            _instrList.Add("MAAT");
            _instrList.Add("MAFR");
            _instrList.Add("MAG");
            _instrList.Add("MAH");
            _instrList.Add("MAHD");
            _instrList.Add("MAJ");
            _instrList.Add("MAM");
            _instrList.Add("MAOC");
            _instrList.Add("MAPC");
            _instrList.Add("MAR");
            _instrList.Add("MAS");
            _instrList.Add("MASD");
            _instrList.Add("MASR");
            _instrList.Add("MATC");
            _instrList.Add("MAVE");
            _instrList.Add("MAW");
            _instrList.Add("MAXC");
            _instrList.Add("MCCD");
            _instrList.Add("MCCM");
            _instrList.Add("MCCP");
            _instrList.Add("MCD");
            _instrList.Add("MCLM");
            _instrList.Add("MCPM");
            _instrList.Add("MCS");
            _instrList.Add("MCSD");
            _instrList.Add("MCSR");
            _instrList.Add("MCSV");
            _instrList.Add("MCT");
            _instrList.Add("MCTO");
            _instrList.Add("MCTP");
            _instrList.Add("MCTPO");
            _instrList.Add("MDAC");
            _instrList.Add("MDCC");
            _instrList.Add("MDF");
            _instrList.Add("MDO");
            _instrList.Add("MDOC");
            _instrList.Add("MDR");
            _instrList.Add("MDS");
            _instrList.Add("MDW");
            _instrList.Add("MGS");
            _instrList.Add("MGSD");
            _instrList.Add("MGSP");
            _instrList.Add("MGSR");
            _instrList.Add("MID");
            _instrList.Add("MINC");
            _instrList.Add("MMC");
            _instrList.Add("MRAT");
            _instrList.Add("MRHD");
            _instrList.Add("MRP");
            _instrList.Add("MSF");
            _instrList.Add("MSG");
            _instrList.Add("MSO");
            _instrList.Add("MSTD");
            _instrList.Add("MVMT");

            _instrList.Add("NTCH");

            _instrList.Add("OSFI");
            _instrList.Add("OSRI");

            _instrList.Add("PATT");
            _instrList.Add("PCLF");
            _instrList.Add("PCMD");
            _instrList.Add("PDET");
            _instrList.Add("PFL");
            _instrList.Add("PI");
            _instrList.Add("PID");
            _instrList.Add("PIDE");
            _instrList.Add("PMUL");
            _instrList.Add("POSP");
            _instrList.Add("POVR");
            _instrList.Add("PPD");
            _instrList.Add("PRNP");
            _instrList.Add("PXRQ");
            _instrList.Add("PSC");

            _instrList.Add("RAD");
            _instrList.Add("RESD");
            _instrList.Add("RET");
            _instrList.Add("RLIM");
            _instrList.Add("RMPS");
            _instrList.Add("RTOR");
            _instrList.Add("RTOS");

            _instrList.Add("SASI");
            _instrList.Add("SATT");
            _instrList.Add("SBR");
            _instrList.Add("SCL");
            _instrList.Add("SCLF");
            _instrList.Add("SCMD");
            _instrList.Add("SCRV");
            _instrList.Add("SDET");
            _instrList.Add("SETD");
            _instrList.Add("SFP");
            _instrList.Add("SFR");
            _instrList.Add("SIN");
            _instrList.Add("SIZE");
            _instrList.Add("SNEG");
            _instrList.Add("SOC");
            _instrList.Add("SOVR");
            _instrList.Add("SQRT");
            _instrList.Add("SRTP");
            _instrList.Add("SRT");
            _instrList.Add("SSUM");
            _instrList.Add("SSV");
            _instrList.Add("STOD");
            _instrList.Add("STOR");
            _instrList.Add("SWPB");
            _instrList.Add("TONR");
            _instrList.Add("TOFR");
            _instrList.Add("UPPER");
            _instrList.Add("LOWER");
            _instrList.Add("UPDN");
            _instrList.Add("GSV");
            _instrList.Add("TRUNC");
            _instrList.Add("TND");
            _instrList.Add("UID");
            _instrList.Add("UIE");
            _instrList.Add("TAN");
            foreach (var aoi in Controller.GetInstance().AOIDefinitionCollection)
            {
                _instrList.Add(aoi.Name);
            }

            return _instrList;
        }

    }
}