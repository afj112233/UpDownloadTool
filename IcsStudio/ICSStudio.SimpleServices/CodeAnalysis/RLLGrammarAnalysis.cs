using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Instruction;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.Utils.TagExpression;

namespace ICSStudio.SimpleServices.CodeAnalysis
{
    public class RLLGrammarAnalysis
    {
        private string Code { get; set; }

        public void Analysis(string code, Controller ctrl, RLLRoutine routine)
        {
            Code = code.Trim();
            if (!string.IsNullOrEmpty(Code))
            {
                if (!Code.EndsWith(";"))
                    Code += ";";
                Parse(code.Split(new[] { "   " }, StringSplitOptions.RemoveEmptyEntries).ToList(), ctrl, routine);
            }
        }

        public void Analysis(RLLRoutine routine)
        {
            Parse(routine.RungsText, (Controller)routine.ParentController, routine);
        }

        public ASTRLLRoutine Parse(List<string> codes, Controller ctrl, RLLRoutine routine)
        {
            return ParseRoutine(codes, ctrl, routine);
        }

        private int _curRungIndex;
        public ASTRLLRoutine ParseRoutine(List<string> codes, Controller ctrl, RLLRoutine rllRoutine)
        {
            ASTRLLRoutine routine = new ASTRLLRoutine();
            RLLRungParser parser = new RLLRungParser(ctrl);
            foreach (var code in codes)
            {
                var rung = parser.ParseRung(codes, code, _curRungIndex, rllRoutine);
                routine.Add(rung);
                if (code.Trim() == ";")
                {
                    Errors.Add(new ErrorInfo { RungIndex = _curRungIndex, Info = $"Error: Rung {_curRungIndex}: Empty rung.", Level = Level.Error, Offset = -1 });
                    _curRungIndex++;

                    continue;
                }

                if (parser.Error != null)
                    Errors.AddRange(parser.Error);

                var checkOutputResult = CheckOutput(rung);
                if (!checkOutputResult)
                    Errors.Add(new ErrorInfo { RungIndex = _curRungIndex, Info = $"Error: Rung {_curRungIndex}: Missing output instruction.", Level = Level.Error, Offset = -1 });

                _curRungIndex++;
            }
            return routine;
        }

        public void SetStartRungIndex(int index)
        {
            if (index > 0)
                _curRungIndex = index;
        }

        public List<ErrorInfo> Errors = new List<ErrorInfo>();

        private readonly IList<string> _inputInstructionList = new List<string>
            {"XIC", "XIO", "ONS", "CMP", "EQU", "GRT", "LES", "LEQ", "LIM", "MEQ", "NEQ"};
        private bool CheckOutput(ASTRLLSequence sequence)
        {
            var last = sequence.list.nodes.LastOrDefault();
            if (last == null)
                return false;

            var instruction = last as ASTRLLInstruction;
            if (instruction != null)
            {
                return _inputInstructionList.FirstOrDefault(p =>
                    p.Equals(instruction.name, StringComparison.OrdinalIgnoreCase)) == null;
            }

            var branch = last as ASTRLLParallel;//ASTRLLSequence;
            if (branch != null)
            {
                foreach (var node in branch.list.nodes)
                {
                    var level = node as ASTRLLSequence;

                    if (!CheckOutput(level))
                        return false;
                }

                return true;
            }

            return false;
        }

        public static bool IsNumber(string strVal)
        {
            double doubleVal;
            bool isNumber = double.TryParse(strVal, out doubleVal);

            if (isNumber)
                return true;

            Regex regex = new Regex(@"^2#[01_]+$");
            if (regex.IsMatch(strVal))
                return true;

            regex = new Regex(@"^8#[0-7_]+$");
            if (regex.IsMatch(strVal))
                return true;

            regex = new Regex(@"^16#[0-9A-Fa-f_]+$");
            if (regex.IsMatch(strVal))
                return true;

            return false;
        }
    }

    public class RLLRungParser
    {
        private int _offset;
        private string _text;
        private readonly Controller _ctrl;
        private RLLRoutine _routine;
        private ITagCollection _collection;
        private List<string> _codes;
        public List<ErrorInfo> Error = new List<ErrorInfo>();
        private int _rungIndex;
        private int _outputOffset;

        public RLLRungParser(Controller ctrl)
        {
            _ctrl = ctrl;
        }

        char Curr()
        {
            return _text[_offset];
        }

        void SkipWhitespace()
        {
            while (_offset < _text.Length && Curr() == ' ')
            {
                _offset += 1;
            }
        }
        void Advance(int step)
        {
            Debug.Assert(step >= 0);
            _offset += step;
        }

        void Consume(char ch)
        {
            Debug.Assert(Curr() == ch);
            Advance(1);
        }

        ASTRLLParallel ParseParallel()
        {
            SkipWhitespace();
            ASTRLLParallel parallel = new ASTRLLParallel();
            _outputOffset++;
            while (true)
            {
                var branchLevelOffset = _outputOffset++;
                var sequence = ParseSequence();
                if (sequence.list.Count() == 0)
                {
                    Error.Add(new ErrorInfo { Info = $"Warning: Rung {_rungIndex}: Shorted branch detected.", Level = Level.Warning, RungIndex = _rungIndex, Offset = branchLevelOffset });
                }
                parallel.Add(sequence);
                var ch = Curr();
                if (ch == ']')
                {
                    break;
                }
                Consume(',');
            }

            return parallel;
        }

        ASTRLLSequence ParseSequence()
        {
            ASTRLLSequence seq = new ASTRLLSequence();
            while (true)
            {
                SkipWhitespace();
                var ch = Curr();
                if (ch == ';' || ch == ']' || ch == ',')
                {
                    break;
                }

                Debug.Assert(ch != '(');
                if (ch == '[')
                {
                    Consume('[');
                    seq.Add(ParseParallel());
                    Consume(']');
                }
                else
                {
                    seq.Add(ParseInstruction());
                }

                _outputOffset++;
            }

            return seq;
        }

        private static int GetOffset(string str, int offset)
        {
            char[] pattern = { '[', ',' };
            offset = str.IndexOfAny(pattern, offset);
            if (offset == -1)
            {
                return -1;
            }

            if (str[offset] == ',')
            {
                return offset;
            }

            offset = str.IndexOf(']', offset + 1);
            if (offset == -1)
            {
                return -1;
            }

            while (true)
            {
                offset = str.IndexOfAny(pattern, offset + 1);
                if (offset == -1)
                {
                    return -1;
                }

                if (str[offset] == ',') return offset;

                offset = str.IndexOf(']', offset + 1);
                if (offset == -1)
                {
                    return -1;
                }
            }
        }

        private static List<string> ParseParameters(string str)
        {
            var res = new List<string>();
            int offset = 0;
            while (true)
            {
                int index = GetOffset(str, offset);
                if (index == -1)
                {
                    var param = str.Substring(offset).Trim();
                    if (param.Length != 0)
                        res.Add(param);
                    break;
                }

                Debug.Assert(str[index] == ',');
                if (str[index] == ',')
                {
                    var param = str.Substring(offset, index - offset).Trim();
                    //if (param.Length != 0)
                    if (index >= offset)
                    {
                        res.Add(param);
                    }
                    offset = index + 1;
                }
            }

            return res;
        }

        private static int FindMatchedParenthesis(string text, int offset)
        {
            int nest = 0;
            for (int i = offset; i < text.Length; ++i)
            {
                Debug.Assert(nest >= 0, nest.ToString());
                if (text[i] == ')' && nest == 0)
                {
                    return i;
                }

                if (text[i] == '(')
                {
                    nest++;
                    continue;
                }

                if (text[i] == ')')
                {
                    nest--;
                }

            }

            Debug.Assert(false, "");
            return -1;

        }

        ASTRLLInstruction ParseInstruction()
        {
            SkipWhitespace();
            var end = _text.IndexOf('(', _offset);
            var name = _text.Substring(_offset, end - _offset).Trim();

            if (!IsLegalMnemonic(name))
                Error.Add(new ErrorInfo
                {
                    Info = "Error: Unknown instruction. Instruction is not known or not defined.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });

            Advance(end - _offset);
            Consume('(');
            end = FindMatchedParenthesis(_text, _offset);

            if (end < 0)
                throw new ErrorInfo { Info = "Error: Unbalanced parenthesis or brackets.", RungIndex = _rungIndex, Level = Level.Error };

            var paramList = ParseParameters(_text.Substring(_offset, end - _offset)).ToArray();

            for (int i = 0; i < paramList.Length; ++i)
            {
                paramList[i] = paramList[i].Trim();
            }

            //Check Parameter Counts
            paramList = CheckParameters(name, paramList);

            Advance(end - _offset);
            Consume(')');

            IXInstruction instr;
            {
                //ctrl.AoiDefinitionCollection.
                //ctrl.AoiDefinitions
                //First Find from AOI, then RLLInstruction
                //ctrl.AoiDefinitionCollection
                var tmp = (_ctrl.AOIDefinitionCollection as AoiDefinitionCollection)?.Find(name);
                instr = tmp != null ? tmp.instr : _ctrl.RLLInstructionCollection.FindInstruction(name);
            }

            return new ASTRLLInstruction(name, instr, paramList.ToList());
        }

        private string[] CheckParameters(string mnemonic, string[] paramList)
        {
            var definition = InstructionDefinition.GetDefinition(mnemonic);
            bool isAOI = false;
            List<ITag> requiredAOIParas = null;

            if (definition == null)
            {
                var aoiDefinition = _ctrl.AOIDefinitionCollection.FirstOrDefault(p =>
                    p.Name.Equals(mnemonic, StringComparison.OrdinalIgnoreCase));
                if (aoiDefinition == null)
                {
                    Error.Add(new ErrorInfo
                    {
                        Info = "Error: Unknown instruction. Instruction is not known or not defined.",
                        Level = Level.Error,
                        RungIndex = _rungIndex,
                        Offset = _outputOffset
                    });
                    return paramList;
                }

                definition = GetRLLInstructionDefinition(aoiDefinition);
                requiredAOIParas = aoiDefinition.Tags.Where(p => p.IsRequired).ToList();
                isAOI = true;
            }

            var paras = definition.Parameters.Where(p =>
                !p.Type.Contains("Target") || (p.Type == "ReadWrite_DataTarget" && p.DataType == "SINT")).ToList();//p => p.Type != "Read_DataTarget" && p.Type != "Write_DataTarget").ToList();
            if (mnemonic.Equals("TON", StringComparison.OrdinalIgnoreCase) || mnemonic.Equals("TOF", StringComparison.OrdinalIgnoreCase) || mnemonic.Equals("RTO", StringComparison.OrdinalIgnoreCase) || mnemonic.Equals("CTU", StringComparison.OrdinalIgnoreCase) || mnemonic.Equals("CTD", StringComparison.OrdinalIgnoreCase))
                paras = paras.Where(p => p.Type != "Write_none").ToList();

            if (mnemonic.Equals("JSR", StringComparison.OrdinalIgnoreCase))
            {
                var list = paramList.ToList();
                CheckJsr(list);
                return list.ToArray();
            }

            if (mnemonic.Equals("LBL", StringComparison.OrdinalIgnoreCase))
            {
                CheckLbl(paramList);
                return paramList;
            }

            if (mnemonic.Equals("JMP", StringComparison.OrdinalIgnoreCase))
            {
                CheckJmp(paramList);
                return paramList;
            }

            if (mnemonic.Equals("RET", StringComparison.OrdinalIgnoreCase))
            {
                CheckRet(paramList);
                return paramList;
            }

            int paraCount = paras.Count;

            if (paraCount > paramList.Length)
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Warning: {mnemonic}: Not enough arguments found. Adding additional ones.",
                    Level = Level.Warning,
                    Offset = _outputOffset
                });
                var list = new List<string>();
                list.AddRange(paramList);
                IEnumerable<string> lacks = Enumerable.Repeat("?", paraCount - paramList.Length);
                list.AddRange(lacks);
                return list.ToArray();
            }

            if (paraCount < paramList.Length)
            {
                Error.Add(new ErrorInfo
                {
                    Info = "Error: Invalid number of arguments for instruction.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
                return paramList;
            }

            if (!HasArgument(paramList))
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, {mnemonic}: Instruction has no arguments specified.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
                return paramList;
            }

            if (mnemonic.Equals("GSV", StringComparison.OrdinalIgnoreCase))
            {
                CheckGsv(paramList);
                return paramList;
            }

            if (mnemonic.Equals("SSV", StringComparison.OrdinalIgnoreCase))
            {
                CheckSsv(paramList);
                return paramList;
            }

            if (isAOI && requiredAOIParas != null)
            {
                Debug.Assert(requiredAOIParas.Count + 1 == paramList.Length);
            }

            for (int i = 0; i < paras.Count() && i < paramList.Length; ++i)
            {
                var para = paras[i];
                if (para.Type == "ReadWrite_DataTarget")
                    continue;
                var actualPara = paramList[i];

                if (mnemonic.Equals("COP", StringComparison.OrdinalIgnoreCase) && (i == 0 || i == 1))
                {
                    double val;
                    if (double.TryParse(actualPara, out val))
                    {
                        Error.Add(new ErrorInfo
                        {
                            Info = $"Error: Rung {_rungIndex}, COP, Operand {i}: Invalid kind of operand or argument i.e. tag, literal, or expression.",
                            Level = Level.Error,
                            RungIndex = _rungIndex,
                            Offset = _outputOffset
                        });
                    }
                    else
                    {
                        var info = GetDataOperandInfo(actualPara, _collection);
                        if (info == null)
                        {
                            Error.Add(new ErrorInfo
                            {
                                Info = $"Error: Rung {_rungIndex}, COP, Operand {i}: Referenced tag is undefined.",
                                Level = Level.Error,
                                RungIndex = _rungIndex,
                                Offset = _outputOffset
                            });
                        }
                    }
                    continue;
                }

                ITag tag = null;
                if (isAOI && i > 0)
                {
                    tag = requiredAOIParas[i - 1];
                }

                var checkResult = CheckTypes(para, actualPara, tag);
                //if (mnemonic == "FAL")
                //    checkResult = CheckTypeResult.Passed;

                int enumVal = -1;
                //if (mnemonic == "GSV" || mnemonic == "SSV")
                //{
                //    if (i == 1)
                //        checkResult = CheckTypeResult.Passed;
                //    else if (i == 3)
                //        checkResult = CheckGSVDest(actualPara, paramList[2]);
                //    else if (checkResult != CheckTypeResult.Passed)
                //        enumVal = Instruction.Utils.ParseEnum(mnemonic, i, actualPara, paramList[0]);
                //}
                //else 
                if (checkResult != CheckTypeResult.Passed)
                    enumVal = Instruction.Utils.ParseEnum(mnemonic, i, actualPara);

                if (mnemonic.Equals("MAM", StringComparison.OrdinalIgnoreCase) && checkResult != CheckTypeResult.Passed)
                {
                    if (i == 19 || i == 18)
                    {
                        if (actualPara == "0")
                            checkResult = CheckTypeResult.Passed;
                    }
                }

                if (enumVal >= 0 && (para.DataType == "SINT" || para.DataType == "DINT"))
                    checkResult = CheckTypeResult.Passed;

                if (checkResult == CheckTypeResult.Passed && mnemonic.Equals("MOV", StringComparison.OrdinalIgnoreCase))
                {
                    checkResult = CheckMov(paramList, i);
                }

                if (checkResult == CheckTypeResult.Passed && mnemonic.Equals("SIZE", StringComparison.OrdinalIgnoreCase) && i == 1)
                {
                    checkResult = CheckSize(actualPara, i);
                }

                if (!isAOI)
                    mnemonic = mnemonic.ToUpper();

                if (checkResult == CheckTypeResult.TagUndefined)
                {
                    Error.Add(new ErrorInfo
                    {
                        Info = $"Error: Rung {_rungIndex}, {mnemonic}, Operand {i}: Referenced tag is undefined.",
                        Level = Level.Error,
                        RungIndex = _rungIndex,
                        Offset = _outputOffset
                    });
                }
                if (checkResult == CheckTypeResult.Overflow)
                {
                    Error.Add(new ErrorInfo
                    {
                        Info = $"Error: Rung {_rungIndex}, {mnemonic}, Operand {i}: Signed value overflow.",
                        Level = Level.Error,
                        RungIndex = _rungIndex,
                        Offset = _outputOffset
                    });
                }
                if (checkResult == CheckTypeResult.UnmatchedTagType)
                {
                    Error.Add(new ErrorInfo
                    {
                        Info = $"Error: Rung {_rungIndex}, {mnemonic}, Operand {i}: Invalid data type. Argument must match parameter data type.",
                        Level = Level.Error,
                        RungIndex = _rungIndex,
                        Offset = _outputOffset
                    });
                }
                if (checkResult == CheckTypeResult.UnmatchedValueType)
                {
                    Error.Add(new ErrorInfo
                    {
                        Info = $"Error: Rung {_rungIndex}, {mnemonic}, Operand {i}: Invalid kind of operand or argument i.e. tag, literal, or expression.",
                        Level = Level.Error,
                        RungIndex = _rungIndex,
                        Offset = _outputOffset
                    });
                }
                if (checkResult == CheckTypeResult.Unknown)
                {
                    Error.Add(new ErrorInfo
                    {
                        Info = $"Error: Rung {_rungIndex}, {mnemonic}, Operand {i}: Unknown Error.",
                        Level = Level.Error,
                        RungIndex = _rungIndex,
                        Offset = _outputOffset
                    });
                }
                if (checkResult == CheckTypeResult.UnknownRoutine)
                {
                    Error.Add(new ErrorInfo
                    {
                        Info = $"Error: Rung {_rungIndex}, {mnemonic}, Operand {i}: Invalid reference to unknown routine.",
                        Level = Level.Error,
                        RungIndex = _rungIndex,
                        Offset = _outputOffset
                    });
                }
                if (checkResult == CheckTypeResult.MissingArgument)
                {
                    Error.Add(new ErrorInfo
                    {
                        Info = $"Error: Rung {_rungIndex}, {mnemonic}, Operand {i}: Missing operand or argument.",
                        Level = Level.Error,
                        RungIndex = _rungIndex,
                        Offset = _outputOffset
                    });
                }
                if (checkResult == CheckTypeResult.InvalidBitSpecifier)
                {
                    Error.Add(new ErrorInfo
                    {
                        Info = $"Error: Rung {_rungIndex}, {mnemonic}, Operand {i}: Invalid bit specifier.",
                        Level = Level.Error,
                        RungIndex = _rungIndex,
                        Offset = _outputOffset
                    });
                }
                if (checkResult == CheckTypeResult.InvalidExpression)
                {
                    Error.Add(new ErrorInfo
                    {
                        Info = $"Error: Rung {_rungIndex}, {mnemonic}, Operand {i}: Tag name invalid.",
                        Level = Level.Error,
                        RungIndex = _rungIndex,
                        Offset = _outputOffset
                    });
                }
                if (checkResult == CheckTypeResult.NotArray)
                {
                    Error.Add(new ErrorInfo
                    {
                        Info = $"Error: Rung {_rungIndex}, {mnemonic}, Operand {i}: Not array element.",
                        Level = Level.Error,
                        RungIndex = _rungIndex,
                        Offset = _outputOffset
                    });
                }

                if (checkResult == CheckTypeResult.MovMixString)
                {
                    Error.Add(new ErrorInfo
                    {
                        Info = $"Error: Rung {_rungIndex}, MOV: MOV instructions and assignments cannot mix Strings with other data types.",
                        Level = Level.Error,
                        RungIndex = _rungIndex,
                        Offset = _outputOffset
                    });
                }

                if (checkResult == CheckTypeResult.InvalidExpressionOrTag)
                {
                    Error.Add(new ErrorInfo
                    {
                        Info = $"Error: Rung {_rungIndex}, {mnemonic}, Operand {i}: Invalid expression or tag.",
                        Level = Level.Error,
                        RungIndex = _rungIndex,
                        Offset = _outputOffset
                    });
                }

                if (checkResult == CheckTypeResult.InvalidArraySubscript)
                {
                    Error.Add(new ErrorInfo
                    {
                        Info = $"Error: Rung {_rungIndex}, {mnemonic}, Operand {i}: Invalid array subscript specifier.",
                        Level = Level.Error,
                        RungIndex = _rungIndex,
                        Offset = _outputOffset
                    });
                }
            }
            return paramList;
        }

        private bool HasArgument(string[] paramList)
        {
            if (paramList.Length == 0)
                return true;

            return paramList.ToList().Exists(p => !p.Contains("?"));
        }

        private CheckTypeResult CheckSize(string actualPara, int index)
        {
            CheckTypeResult checkResult = CheckTypeResult.Unknown;

            if (actualPara.Contains("?"))
                return CheckTypeResult.MissingArgument;

            var isNumber = RLLGrammarAnalysis.IsNumber(actualPara);

            //if (index == 0)
            //{
            //    if (isNumber)
            //    {
            //        checkResult = CheckTypeResult.NotArray;
            //    }
            //    else
            //    {
            //        var info = GetDataOperandInfo(actualPara, _collection);

            //        if (info == null && !IsLegalTagName(actualPara))
            //        {
            //            return CheckTypeResult.TagUndefined;
            //        }

            //        if (info != null && info.DataTypeInfo.Dim1 > 0)
            //        {
            //            checkResult = CheckTypeResult.Passed;
            //        }
            //    }
            //}

            if (index == 1)
            {
                if (!isNumber)
                    checkResult = CheckTypeResult.UnmatchedValueType;
                else
                {
                    int dim;
                    bool isFormatted = false;
                    int fromBase = -1;
                    bool canConvert = false;
                    if (actualPara.StartsWith("2#"))
                    {
                        isFormatted = true;
                        fromBase = 2;
                        actualPara = actualPara.Substring(2);
                        canConvert = CanConvert<sbyte>(actualPara, 2);
                    }
                    if (actualPara.StartsWith("8#"))
                    {
                        isFormatted = true;
                        fromBase = 8;
                        actualPara = actualPara.Substring(2);
                        canConvert = CanConvert<sbyte>(actualPara, 8);
                    }
                    if (actualPara.StartsWith("16#"))
                    {
                        isFormatted = true;
                        fromBase = 16;
                        actualPara = actualPara.Substring(3);
                        canConvert = CanConvert<sbyte>(actualPara, 16);
                    }

                    if (isFormatted)
                    {
                        if (canConvert)
                        {
                            dim = Convert.ToInt32(actualPara, fromBase);
                            if (0 <= dim && dim < 3)
                                return CheckTypeResult.Passed;

                            return CheckTypeResult.ImmediateValueOutOfRange;
                        }

                        return CheckTypeResult.InvalidExpressionOrTag;
                    }

                    bool result = int.TryParse(actualPara, out dim);
                    if (result)
                    {
                        if (0 <= dim && dim < 3)
                            return CheckTypeResult.Passed;

                        return CheckTypeResult.ImmediateValueOutOfRange;
                    }

                    return CheckTypeResult.UnmatchedTagType;
                }
            }

            if (index == 2)
            {
                if (isNumber)
                    checkResult = CheckTypeResult.UnmatchedValueType;
                else
                {
                    var info = GetDataOperandInfo(actualPara, _collection);
                    if (info == null)
                        checkResult = CheckTypeResult.TagUndefined;
                    else
                    {
                        var acceptTypes = new List<string> { "SINT", "INT", "DINT", "REAL" };
                        if (acceptTypes.Contains(info.DataTypeInfo.ToString()))
                            checkResult = CheckTypeResult.Passed;
                        else
                        {
                            checkResult = CheckTypeResult.UnmatchedTagType;
                        }

                    }
                }
            }

            return checkResult;
        }

        private CheckTypeResult CheckMov(string[] paramList, int index)
        {
            CheckTypeResult checkResult = CheckTypeResult.Passed;

            var actualPara = paramList[index];
            var isNumber = RLLGrammarAnalysis.IsNumber(actualPara);
            var isString = actualPara.Contains("'");

            if (index == 0)
            {
                var para1 = paramList[1];
                var info0 = GetDataOperandInfo(actualPara, _collection);
                var info1 = GetDataOperandInfo(para1, _collection);
                if (info1 != null)
                {
                    if (info0 != null)
                    {
                        if (info1.DataTypeInfo.ToString().Equals("string", StringComparison.OrdinalIgnoreCase))
                        {
                            if (isNumber || !info0.DataTypeInfo.ToString()
                                    .Equals("string", StringComparison.OrdinalIgnoreCase))
                                return CheckTypeResult.MovMixString;
                        }

                        if (!isNumber && info0.DataTypeInfo.ToString().Equals("string", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!info1.DataTypeInfo.ToString()
                                    .Equals("string", StringComparison.OrdinalIgnoreCase))
                                return CheckTypeResult.MovMixString;
                        }
                    }
                    else
                    {
                        if (isString)
                        {
                            if (info1.DataTypeInfo.ToString().Equals("string", StringComparison.OrdinalIgnoreCase))
                                return CheckTypeResult.Passed;

                            return CheckTypeResult.MovMixString;
                        }
                    }
                }
            }

            return checkResult;
        }

        private void CheckGsv(string[] paramList)
        {
            var para = paramList[0];
            var enumVal = Instruction.Utils.ParseEnum("GSV", 0, para, paramList[0]);
            if (enumVal < 0)
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, GSV, Operand 0: Class name not valid..",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
                return;
            }

            var para2 = paramList[2];
            enumVal = Instruction.Utils.ParseEnum("GSV", 2, para2, para);
            if (enumVal < 0)
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, GSV, Operand 2: Invalid reference to unknown attribute.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
                return;
            }

            var para3 = paramList[3];
            var checkResult = CheckGsvDest(para3, paramList[2]);
            if (checkResult == CheckTypeResult.UnmatchedTagType)
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, GSV, Operand 3: Invalid data type. Argument must match parameter data type.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
            }
            if (checkResult == CheckTypeResult.TagUndefined)
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, GSV, Operand 3: Referenced tag is undefined.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
            }
            if (checkResult == CheckTypeResult.MissingArgument)
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, GSV, Operand 3: Missing operand or argument.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
            }
            if (checkResult == CheckTypeResult.InvalidExpression)
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, GSV, Operand 3: Invalid expression or tag.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
            }
            if (checkResult == CheckTypeResult.TagUndefined)
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, GSV, Operand 3: Referenced tag is undefined.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
            }
            if (checkResult == CheckTypeResult.Unknown)
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, GSV, Operand 3: Unknown Error.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
            }
        }

        private void CheckSsv(string[] paramList)
        {
            var para = paramList[0];
            var enumVal = Instruction.Utils.ParseEnum("SSV", 0, para, paramList[0]);
            if (enumVal < 0)
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, SSV, Operand 0: Class name not valid..",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
                return;
            }

            var para2 = paramList[2];
            enumVal = Instruction.Utils.ParseEnum("SSV", 2, para2, para);
            if (enumVal < 0)
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, SSV, Operand 2: Invalid reference to unknown attribute.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
                return;
            }

            var para3 = paramList[3];
            var checkResult = CheckGsvDest(para3, paramList[2]);
            if (checkResult == CheckTypeResult.UnmatchedTagType)
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, SSV, Operand 3: Invalid data type. Argument must match parameter data type.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
            }
            if (checkResult == CheckTypeResult.TagUndefined)
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, SSV, Operand 3: Referenced tag is undefined.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
            }
            if (checkResult == CheckTypeResult.MissingArgument)
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, SSV, Operand 3: Missing operand or argument.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
            }
            if (checkResult == CheckTypeResult.InvalidExpression)
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, SSV, Operand 3: Invalid expression or tag.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
            }
            if (checkResult == CheckTypeResult.TagUndefined)
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, SSV, Operand 3: Referenced tag is undefined.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
            }
            if (checkResult == CheckTypeResult.Unknown)
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, SSV, Operand 3: Unknown Error.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
            }
        }

        private CheckTypeResult CheckGsvDest(string tag, string atrribute)
        {
            List<string> accessTypes = GSVHelper.GetAccessTypes(atrribute);
            if (accessTypes == null)
                return CheckTypeResult.UnmatchedTagType;

            var info = GetDataOperandInfo(tag, _collection);
            if (info != null)
            {
                string tagDataType = info.DataTypeInfo.ToString();

                if (accessTypes.Contains(tagDataType))
                    return CheckTypeResult.Passed;

                return CheckTypeResult.UnmatchedTagType;
            }

            double doubleVal;
            bool isNumber = double.TryParse(tag, out doubleVal);
            if (!isNumber)
            {
                if (!IsLegalTagName(tag))
                {
                    if (tag.Contains("?"))
                        return CheckTypeResult.MissingArgument;
                    return CheckTypeResult.InvalidExpression;
                }
                return CheckTypeResult.TagUndefined;
            }

            return CheckTypeResult.Unknown;
        }

        private void CheckJmp(string[] paraList)
        {
            if (paraList.Length == 0)
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, JMP: Instruction has no arguments specified.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
                return;
            }

            if (paraList.Length > 1)
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, JMP: Invalid number of arguments for instruction.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
                return;
            }

            var labelName = paraList[0];
            if (!IsLegalTagName(labelName))
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, JMP: JMP/LBL instruction to label that does not exist.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
                return;
            }

            if (CountLabel(labelName) < 1)
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, JMP instruction has no target label ({labelName})",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
            }
        }

        private void CheckLbl(string[] paraList)
        {
            if (paraList.Length == 0)
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, LBL: Instruction has no arguments specified.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
                return;
            }

            if (paraList.Length > 1)
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, LBL: Invalid number of arguments for instruction.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
                return;
            }

            var labelName = paraList[0];
            if (!IsLegalTagName(labelName))
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, LBL: JMP/LBL instruction to label that does not exist.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
                return;
            }

            //查重
            if (CountLabel(labelName) > 1)
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, LBL: Duplicate label definition.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
            }
        }

        private int CountLabel(string labelName)
        {
            int count = 0;
            string expression = $"LBL({labelName})";
            foreach (var code in _codes)
            {
                int codeLength = code.Length;
                int remainLength = code.Replace(expression, "").Length;
                int replacedLength = codeLength - remainLength;
                count += replacedLength / expression.Length;
            }

            return count;
        }

        private void CheckRet(string[] paraList)
        {
            for (int i = 0; i < paraList.Length; ++i)
            {
                string para = paraList[i];

                var info = GetDataOperandInfo(para, _collection);
                if (info != null)
                {
                    continue;
                }

                double val;
                if (double.TryParse(para, out val))
                {
                    continue;
                }

                if (para.StartsWith("'") && para.EndsWith("'"))
                {
                    Error.Add(new ErrorInfo
                    {
                        Info = $"Error: Rung {_rungIndex}, RET, Invalid data type. Argument must match parameter data type.",
                        Level = Level.Error,
                        RungIndex = _rungIndex,
                        Offset = _outputOffset
                    });
                    continue;
                }

                if (!IsLegalTagName(para))
                {
                    Error.Add(new ErrorInfo
                    {
                        Info = $"Error: Rung {_rungIndex}, RET, Operand {i}: Invalid expression or tag.",
                        Level = Level.Error,
                        RungIndex = _rungIndex,
                        Offset = _outputOffset
                    });
                    continue;
                }

                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, RET, Operand {i}: Referenced tag is undefined.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });

            }
        }

        private void CheckJsr(List<string> paraList)
        {
            if (paraList.Count == 0)
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, JSR: Operand 0: Invalid reference to unknown routine.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
                return;
            }

            string routineName = paraList[0];
            var jsrRoutine = _routine.ParentCollection[routineName];
            if (jsrRoutine == null)
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, JSR: Operand 0: Invalid reference to unknown routine.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
                return;
            }

            if (jsrRoutine.Name.Equals(_routine.ParentCollection.ParentProgram.MainRoutineName,
                    StringComparison.OrdinalIgnoreCase))
            {
                Error.Add(new ErrorInfo
                {
                    Info = $"Error: Rung {_rungIndex}, JSR: Jumping to Main Routine not allowed.",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });
            }

            if (paraList.Count == 1)
            {
                //只有Routine名的话，按InputPara为0来处理
                paraList.Add("0");
                return;
            }

            int inputNum;
            bool checkInputNum = int.TryParse(paraList[1], out inputNum);
            if (!checkInputNum)
                Error.Add(new ErrorInfo
                {
                    Info = "Error: Missing numeric value indicating number of input parameters",
                    Level = Level.Error,
                    RungIndex = _rungIndex,
                    Offset = _outputOffset
                });

            //参数比inputNum少，则将inputNum更新为实际数目
            if (inputNum > paraList.Count - 2)
            {
                inputNum = paraList.Count - 2;
                paraList[1] = inputNum.ToString();
            }

            //检验输入参数
            for (int i = 2; i < paraList.Count; ++i)//inputNum
            {
                string para = paraList[i];
                double val;
                if (double.TryParse(para, out val))
                {
                    Error.Add(new ErrorInfo
                    {
                        Info = $"Error: Rung {_rungIndex}, JSR, Operand {i}: Invalid kind of operand or argument i.e. tag, literal, or expression.",
                        Level = Level.Error,
                        RungIndex = _rungIndex,
                        Offset = _outputOffset
                    });
                    continue;
                }

                var info = GetDataOperandInfo(para, _collection);
                if (info == null)
                {
                    Error.Add(new ErrorInfo
                    {
                        Info = $"Error: Rung {_rungIndex}, JSR, Operand {i}: Referenced tag is undefined.",
                        Level = Level.Error,
                        RungIndex = _rungIndex,
                        Offset = _outputOffset
                    });
                }
            }

            //参数>=inputNum，则继续解析outputNum
            //int outputNum = paraList.Length - inputNum - 2;
            //for (int i = inputNum + 2; i < outputNum; ++i)
            //{
            //    string para = paraList[i];
            //    double val;
            //    if (double.TryParse(para, out val))
            //    {
            //        Error.Add(new ErrorInfo { Info = $"Error: Rung {_rungIndex}, JSR, Operand {i}: Invalid kind of operand or argument i.e. tag, literal, or expression.", Level = Level.Error, RungIndex = _rungIndex });
            //        continue;
            //    }

            //    var info = GetDataOperandInfo(para, _collection);
            //    if (info == null)
            //    {
            //        Error.Add(new ErrorInfo { Info = $"Error: Rung {_rungIndex}, JSR, Operand {i}: Referenced tag is undefined.", Level = Level.Error, RungIndex = _rungIndex });
            //    }

            //}
        }

        private bool IsLegalMnemonic(string mnemonic)
        {
            if (_ctrl.RLLInstructionCollection.FindInstruction(mnemonic) != null)
                return true;

            if (((AoiDefinitionCollection)_ctrl.AOIDefinitionCollection)?.Find(mnemonic) != null)
                return true;

            return false;
        }

        private bool IsLegalTagName(string name)
        {
            TagExpressionParser parser = new TagExpressionParser();
            var tagExpression = parser.Parser(name);
            if (tagExpression == null)
                return false;
            SimpleTagExpression simpleTagExpression = parser.GetSimpleTagExpression(tagExpression);
            name = simpleTagExpression.TagName;
            if (string.IsNullOrEmpty(name))
                return false;

            if (name.EndsWith("_") || name.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
                return false;

            Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_:.]*$");
            if (!regex.IsMatch(name))
                return false;

            string[] keyWords =
            {
                "goto",
                "repeat", "until", "or", "end_repeat",
                "return", "exit",
                "if", "then", "elsif", "else", "end_if",
                "case", "of", "end_case",
                "for", "to", "by", "do", "end_for",
                "while", "end_while",
                "not", "mod", "and", "xor", "or"
            };

            if (keyWords.ToList().Exists(p => p.Equals(name, StringComparison.OrdinalIgnoreCase)))
                return false;

            return true;
        }

        public static InstructionDefinition GetRLLInstructionDefinition(IAoiDefinition aoiDefinition)
        {
            if (aoiDefinition == null)
                return null;

            InstructionDefinition definition = new InstructionDefinition();

            definition.Mnemonic = aoiDefinition.Name;
            definition.Description = aoiDefinition.Description;
            definition.Type = "Box";

            // Parameters
            var parameters = new List<InstructionParameter>();
            var bitlegs = new List<string>();

            // aoi type
            InstructionParameter parameter = new InstructionParameter
            {
                Label = aoiDefinition.Name,
                Type = "Read_none",
                DataType = aoiDefinition.Name,
                Visible = true,
                HasConfig = true,
                Formats = new List<FormatType> { FormatType.Tag }
            };
            parameters.Add(parameter);

            // parameter and bitLegs
            foreach (var tag in aoiDefinition.Tags)
            {
                if (tag.Name == "EnableIn" || tag.Name == "EnableOut")
                    continue;

                if (tag.Usage == Usage.Local)
                    continue;

                var dataType = tag.DataTypeInfo.DataType;
                var formats = new List<FormatType>();
                if (dataType is UserDefinedDataType)
                    formats.Add(FormatType.Tag);
                else if (dataType.IsNumber || dataType.IsStringType || dataType.IsBool)
                {
                    formats.Add(FormatType.Immediate);
                    formats.Add(FormatType.Tag);
                }
                else if (dataType.IsPredefinedType)
                    formats.Add(FormatType.Tag);

                if (tag.Usage == Usage.Output
                    && tag.IsVisible
                    && !tag.IsRequired
                    && dataType.IsBool)
                {
                    bitlegs.Add(tag.Name);
                    continue;
                }

                if (tag.Usage == Usage.Input || tag.Usage == Usage.Output)
                {
                    if (tag.IsRequired && dataType.IsAtomic)
                    {
                        // two lines
                        parameter = new InstructionParameter
                        {
                            Label = tag.Name,
                            Type = "Read_DataSource",
                            DataType = tag.DataTypeInfo.ToString(),
                            Visible = true,
                            HasConfig = false,
                            Formats = formats
                        };
                        parameter.AcceptTypes = new List<string> { parameter.DataType };
                        parameters.Add(parameter);

                        parameter = new InstructionParameter
                        {
                            Label = string.Empty,
                            Type = "Read_DataTarget",
                            DataType = string.Empty,
                            Visible = true,
                            HasConfig = false,
                            Formats = formats
                        };
                        parameters.Add(parameter);

                        continue;
                    }

                    if (tag.IsVisible && !tag.IsRequired)
                    {
                        parameter = new InstructionParameter
                        {
                            Label = tag.Name,
                            Type = "Read_DataTarget",
                            DataType = tag.DataTypeInfo.ToString(),
                            Visible = true,
                            HasConfig = false,
                            Formats = formats
                        };
                        parameter.AcceptTypes = new List<string> { parameter.DataType };
                        parameters.Add(parameter);
                    }

                    continue;
                }

                // default , InOut
                parameter = new InstructionParameter
                {
                    Label = tag.Name,
                    Type = "Read_none",
                    DataType = tag.DataTypeInfo.ToString(),
                    Visible = tag.IsVisible,
                    HasConfig = false,
                    Formats = formats
                };
                parameter.AcceptTypes = new List<string> { parameter.DataType };
                parameters.Add(parameter);

            }

            definition.Parameters = parameters;
            definition.BitLegs = bitlegs;

            return definition;
        }

        private DataOperandInfo GetDataOperandInfo(string operand, ITagCollection collection)
        {
            // get tag and expression
            TagExpressionParser parser = new TagExpressionParser();
            var tagExpression = parser.Parser(operand);
            if (tagExpression == null)
                return null;

            SimpleTagExpression simpleTagExpression = parser.GetSimpleTagExpression(tagExpression);

            ITagCollection tagCollection = collection.FirstOrDefault(p => p.Name.Equals(simpleTagExpression.TagName, StringComparison.OrdinalIgnoreCase)) == null ? _ctrl.Tags : collection;

            var tag = tagCollection[simpleTagExpression.TagName] as Tag;

            if (tag == null)
                return null;

            // data type and value
            DataTypeInfo dataTypeInfo = tag.DataTypeInfo;
            IField field = tag.DataWrapper.Data;
            bool isDynamicField = false;
            DisplayStyle displayStyle = tag.DisplayStyle;
            bool isBit = false;
            int bitOffset = -1;

            TagExpressionBase expression = simpleTagExpression;
            while (expression.Next != null)
            {
                expression = expression.Next;

                int bitMemberNumber = -1;
                var bitMemberNumberAccessExpression = expression as BitMemberNumberAccessExpression;
                var bitMemberExpressionAccessExpression = expression as BitMemberExpressionAccessExpression;

                if (bitMemberNumberAccessExpression != null)
                {
                    bitMemberNumber = bitMemberNumberAccessExpression.Number;
                }
                else if (bitMemberExpressionAccessExpression != null)
                {
                    //TODO(gjc): need check here
                    if (bitMemberExpressionAccessExpression.Number.HasValue)
                        bitMemberNumber = bitMemberExpressionAccessExpression.Number.Value;
                    else if (bitMemberExpressionAccessExpression.ExpressionNumber != null)
                        bitMemberNumber = 0;
                }

                if (bitMemberNumber >= 0)
                {
                    if (!dataTypeInfo.DataType.IsInteger || dataTypeInfo.Dim1 != 0)
                        return null;

                    isBit = true;
                    bitOffset = bitMemberNumber;
                    displayStyle = DisplayStyle.Decimal;

                    break;
                }

                //
                var memberAccessExpression = expression as MemberAccessExpression;
                if (memberAccessExpression != null)
                {
                    var compositeField = field as ICompositeField;
                    var compositiveType = dataTypeInfo.DataType as CompositiveType;
                    if (compositeField != null && compositiveType != null && dataTypeInfo.Dim1 == 0)
                    {
                        var dataTypeMember = compositiveType.TypeMembers[memberAccessExpression.Name] as DataTypeMember;
                        if (dataTypeMember == null)
                            return null;

                        //if (dataTypeMember.IsHidden)
                        //    return null;

                        field = compositeField.fields[dataTypeMember.FieldIndex].Item1;
                        dataTypeInfo = dataTypeMember.DataTypeInfo;
                        displayStyle = dataTypeMember.DisplayStyle;

                        if (dataTypeMember.IsBit && dataTypeInfo.Dim1 == 0)
                        {
                            isBit = true;
                            bitOffset = dataTypeMember.BitOffset;
                            displayStyle = DisplayStyle.Decimal;
                        }


                    }
                    else
                    {
                        return null;
                    }

                }

                //
                var elementAccessExpression = expression as ElementAccessExpression;
                if (elementAccessExpression != null
                    && elementAccessExpression.Indexes != null
                    && elementAccessExpression.Indexes.Count > 0)
                {
                    int index;

                    switch (elementAccessExpression.Indexes.Count)
                    {
                        case 1:
                            if (dataTypeInfo.Dim1 > 0 && dataTypeInfo.Dim2 == 0 && dataTypeInfo.Dim3 == 0)
                            {
                                index = elementAccessExpression.Indexes[0];
                                break;
                            }
                            else
                                return null;
                        case 2:
                            if (dataTypeInfo.Dim1 > 0 && dataTypeInfo.Dim2 > 0 && dataTypeInfo.Dim3 == 0)
                            {
                                index = elementAccessExpression.Indexes[0] * dataTypeInfo.Dim1 +
                                        elementAccessExpression.Indexes[1];
                                break;
                            }
                            else
                                return null;
                        case 3:
                            if (dataTypeInfo.Dim1 > 0 && dataTypeInfo.Dim2 > 0 && dataTypeInfo.Dim3 > 0)
                            {
                                index = elementAccessExpression.Indexes[0] * dataTypeInfo.Dim2 * dataTypeInfo.Dim1 +
                                        elementAccessExpression.Indexes[1] * dataTypeInfo.Dim1 +
                                        elementAccessExpression.Indexes[2];
                                break;
                            }
                            else
                                return null;
                        default:
                            throw new NotImplementedException();
                    }

                    ArrayField arrayField = field as ArrayField;
                    BoolArrayField boolArrayField = field as BoolArrayField;

                    if (arrayField == null && boolArrayField == null)
                        return null;

                    if (arrayField != null)
                    {
                        if (index < 0 || index >= arrayField.Size())
                            return null;

                        dataTypeInfo = new DataTypeInfo { DataType = dataTypeInfo.DataType };
                        field = arrayField.fields[index].Item1;
                    }

                    if (boolArrayField != null)
                    {
                        if (index < 0 || index >= boolArrayField.BitCount)
                            return null;

                        dataTypeInfo = new DataTypeInfo { DataType = dataTypeInfo.DataType };
                        field = boolArrayField;
                        isBit = true;
                        bitOffset = index;
                    }
                }

                if (elementAccessExpression != null
                    && elementAccessExpression.ExpressionIndexes != null
                    && elementAccessExpression.ExpressionIndexes.Count > 0)
                {
                    // ignore expression valid

                    ArrayField arrayField = field as ArrayField;
                    BoolArrayField boolArrayField = field as BoolArrayField;

                    if (arrayField == null && boolArrayField == null)
                        return null;

                    int index = 0;
                    if (arrayField != null)
                    {
                        dataTypeInfo = new DataTypeInfo { DataType = dataTypeInfo.DataType };
                        field = arrayField.fields[index].Item1;
                    }

                    if (boolArrayField != null)
                    {
                        dataTypeInfo = new DataTypeInfo { DataType = dataTypeInfo.DataType };
                        field = boolArrayField;
                        isBit = true;
                        bitOffset = index;
                    }

                    isDynamicField = true;
                }

            }

            return new DataOperandInfo
            {
                Expression = tagExpression,
                Tag = tag,
                DisplayStyle = displayStyle,
                Field = field,
                IsDynamicField = isDynamicField,
                DataTypeInfo = dataTypeInfo,
                IsBit = isBit,
                BitOffset = bitOffset
            };

        }

        private bool IsOutOfRange(IField field, int offset)
        {
            Int8Field int8Field = field as Int8Field;
            if (int8Field != null)
            {
                if (offset > 7)
                    return true;
            }

            Int16Field int16Field = field as Int16Field;
            if (int16Field != null)
            {
                if (offset > 15)
                    return true;
            }

            Int32Field int32Field = field as Int32Field;
            if (int32Field != null)
            {
                if (offset > 31)
                    return true;
            }

            Int64Field int64Field = field as Int64Field;
            if (int64Field != null)
            {
                if (offset > 63)
                    return true;
            }

            BoolArrayField boolArrayField = field as BoolArrayField;
            if (boolArrayField != null)
            {
                if (offset >= boolArrayField.BitCount)
                    return true;
            }

            BoolField boolField = field as BoolField;
            if (boolField != null)
            {
                if (offset > 0)
                    return true;
            }

            return false;
        }

        List<string> _numberTypes = new List<string> { "SINT", "DINT", "INT", "REAL" };

        private CheckTypeResult CheckTypes(InstructionParameter para, string actualPara, ITag aoiPara)
        {
            CheckTypeResult checkPass = CheckTypeResult.Unknown;
            if (actualPara.Contains("?"))
                return CheckTypeResult.MissingArgument;

            var formats = para.Formats;
            var dataType = para.DataType;
            var acceptTypes = para.AcceptTypes;
            var isStringConstant = actualPara.StartsWith("'") && actualPara.EndsWith("'");
            bool isNumber = RLLGrammarAnalysis.IsNumber(actualPara);

            #region Immediate

            if (formats?.Contains(FormatType.Immediate) == true)
            {
                if (dataType.Equals("string", StringComparison.OrdinalIgnoreCase) && isStringConstant)
                    return CheckTypeResult.Passed;

                if (isNumber)
                    checkPass = CheckType(dataType, actualPara);

                if (checkPass == CheckTypeResult.Passed)
                    return checkPass;

                bool passed = false;
                foreach (var type in acceptTypes)
                {
                    if (type.Equals("string", StringComparison.OrdinalIgnoreCase) && isStringConstant)
                    {
                        passed = true;
                        break;
                    }

                    if (isNumber)
                    {
                        var result = CheckType(type, actualPara);
                        if (result == CheckTypeResult.Passed)
                        {
                            return result;
                        }
                    }
                }

                if (passed)
                    return CheckTypeResult.Passed;

                //考虑AOI的特例
                if (aoiPara != null && _numberTypes.Contains(para.DataType))
                {
                    if (isNumber && aoiPara.Usage == Usage.Input)
                        return CheckTypeResult.Passed;
                }

                //只有Immediate时，可以直接判断出来不为Immediate的情况，提示错误
                bool isImmediateOnly = formats.Count == 1 && formats[0] == FormatType.Immediate;

                //如果接收类型只有Immediate或者Operand是常量，说明Operand类型不匹配
                if (isImmediateOnly || isNumber || isStringConstant)
                {
                    if (aoiPara?.Usage == Usage.InOut)
                        return CheckTypeResult.UnmatchedValueType;

                    return CheckTypeResult.UnmatchedTagType;
                }
            }

            #endregion

            #region Routine

            if (formats?.Contains(FormatType.Routine) == true)
            {
                if (para.DataType.Equals("routine", StringComparison.OrdinalIgnoreCase))
                {
                    if (_collection.ParentProgram.Routines[actualPara] != null)
                        return CheckTypeResult.Passed;
                }

                if (formats.Count == 1 && formats[0] == FormatType.Routine)
                    return CheckTypeResult.UnknownRoutine;
            }

            #endregion

            #region Tag

            if (formats?.Contains(FormatType.Tag) == true)
            {
                var info = GetDataOperandInfo(actualPara, _collection);

                //TODO(Ender):完善索引为表达式的解析。对于数组成员形式的，不管索引合法性，直接放过
                if (info == null)
                {
                    if (actualPara.Contains("["))
                    {
                        var simpleLen = actualPara.IndexOf("[");
                        var simpleTag = actualPara.Substring(0, simpleLen);
                        info = GetDataOperandInfo($"{simpleTag}[0]", _collection);
                    }
                }

                if (info != null)
                {
                    if (para.DataType.Equals("STRING") || para.AcceptTypes?.Contains("STRING") == true)
                    {
                        if (info.DataTypeInfo.DataType.IsStringType)
                            return CheckTypeResult.Passed;

                        if (para.AcceptTypes.Count == 1 && para.AcceptTypes[0].Equals("STRING", StringComparison.OrdinalIgnoreCase) && !info.DataTypeInfo.ToString().Equals("STRING", StringComparison.OrdinalIgnoreCase))
                            return CheckTypeResult.UnmatchedTagType;
                    }

                    string tagDataType = info.DataTypeInfo.ToString();
                    if (para.DataType == tagDataType)
                        return CheckTypeResult.Passed;

                    if (para.AcceptTypes?.Contains(tagDataType) == true)
                    {
                        return CheckTypeResult.Passed;
                    }

                    if (aoiPara != null)
                    {
                        if (_numberTypes.Contains(para.DataType) && aoiPara.Usage == Usage.Input)
                        {
                            if (_numberTypes.Contains(tagDataType))
                                return CheckTypeResult.Passed;
                        }
                        //InOut需严格匹配

                        var basicType = aoiPara.DataTypeInfo.DataType.ToString();
                        if (aoiPara.Usage == Usage.InOut && aoiPara.DataTypeInfo.Dim1 > 0 && basicType == info.DataTypeInfo.DataType.ToString() &&
                            aoiPara.DataTypeInfo.Dim1 <= info.DataTypeInfo.Dim1)
                            return CheckTypeResult.Passed;
                    }

                    if (para.DataType == "BOOL" && info.IsBit)
                    {
                        if (IsOutOfRange(info.Field, info.BitOffset))
                            return CheckTypeResult.InvalidBitSpecifier;
                        return CheckTypeResult.Passed;
                    }

                    if (para.Formats?.Contains(FormatType.TagArray) == true)
                    {
                        if (info.DataTypeInfo.Dim1 > 0 &&
                            para.AcceptTypes?.Contains(info.DataTypeInfo.DataType.Name) == true)
                            return CheckTypeResult.Passed;

                        if (info.Tag.DataTypeInfo.Dim1 > 0 &&
                            para.AcceptTypes?.Contains(info.DataTypeInfo.DataType.Name) == true)
                            return CheckTypeResult.Passed;
                    }

                    return CheckTypeResult.UnmatchedTagType;
                }

                if (formats.Count == 1 && formats[0] == FormatType.Tag)
                {
                    if (isStringConstant && !para.AcceptTypes.Contains("STRING"))
                        return CheckTypeResult.UnmatchedTagType;

                    if (isNumber || isStringConstant)
                        return CheckTypeResult.UnmatchedValueType;

                    //TODO: 校验Tag名的合法性
                    if (!IsLegalTagName(actualPara))
                    {
                        if (actualPara.Contains("?"))
                            return CheckTypeResult.MissingArgument;

                        return CheckTypeResult.TagUndefined;
                    }

                    if (actualPara.Equals("s:fs", StringComparison.OrdinalIgnoreCase))
                    {
                        if (para.DataType == "BOOL" || para.AcceptTypes?.Contains("BOOL") == true)
                            return CheckTypeResult.Passed;

                        return CheckType(para.Type, actualPara);
                    }

                    return CheckTypeResult.TagUndefined;
                }

                if (formats?.Contains(FormatType.Expression) != true && formats?.Contains(FormatType.TagArray) != true)
                    return CheckTypeResult.TagUndefined;
            }

            #endregion

            #region Tag Array

            if (formats?.Contains(FormatType.TagArray) == true)
            {
                if (isNumber)
                {
                    checkPass = CheckTypeResult.NotArray;
                }
                else if (isStringConstant)
                {
                    checkPass = CheckTypeResult.UnmatchedTagType;
                }
                else
                {
                    var info = GetDataOperandInfo(actualPara, _collection);

                    if (info == null)
                    {
                        if (!IsLegalTagName(actualPara))
                            return CheckTypeResult.InvalidExpressionOrTag;

                        return CheckTypeResult.TagUndefined;
                    }

                    //数组的dim1为一维的索引
                    if (info.DataTypeInfo.Dim1 > 0)
                    {
                        var type = info.DataTypeInfo.DataType.ToString();
                        if (dataType != type && acceptTypes?.Contains(type) == false && _ctrl.DataTypes.FirstOrDefault(p => p is UserDefinedDataType && p.Name.Equals(type, StringComparison.OrdinalIgnoreCase)) == null)
                            return CheckTypeResult.UnmatchedTagType;

                        return CheckTypeResult.Passed;
                    }

                    //tagArray[0]的情况
                    if (info.DataTypeInfo.Dim1 == 0)
                    {
                        var type = info.DataTypeInfo.ToString();
                        if (dataType != type && acceptTypes?.Contains(type) == false)
                            return CheckTypeResult.UnmatchedTagType;

                        var expr = info.Expression as ElementAccessExpression;
                        if (expr != null)
                        {
                            var index = expr.Indexes[0];
                            if (index != 0)
                                return CheckTypeResult.InvalidArraySubscript;

                            return CheckTypeResult.Passed;
                        }

                        return CheckTypeResult.NotArray;
                    }
                }
            }

            #endregion

            #region Expression

            if (formats?.Contains(FormatType.Expression) == true)
            {
                var expr = Instruction.Utils.ParseExpr(actualPara);
                if (string.IsNullOrEmpty(expr.Error))
                {
                    //TODO(Ender):Invalid Expression情况的处理，如int+string
                    return CheckTypeResult.Passed;
                }

                return CheckTypeResult.InvalidExpressionOrTag;
            }

            #endregion

            return checkPass;
        }

        //private CheckTypeResult CheckTypes(InstructionParameter para, string actualPara)
        //{
        //    if (actualPara.Contains("?"))
        //        return CheckTypeResult.MissingArgument;

        //    var isStringConstant = actualPara.StartsWith("'") && actualPara.EndsWith("'");
        //    var info = GetDataOperandInfo(actualPara, _collection);

        //    if (para.DataType.Equals("STRING") || para.AcceptTypes?.Contains("STRING") == true)
        //    {
        //        if (isStringConstant)
        //        {
        //            if (para.Formats?.Contains(FormatType.Immediate) == true)
        //                return CheckTypeResult.Passed;
        //        }

        //        if (info != null)
        //        {
        //            if (info.DataTypeInfo.DataType.IsStringType)
        //                return CheckTypeResult.Passed;

        //            if (para.AcceptTypes.Count == 1 && para.AcceptTypes[0].Equals("STRING", StringComparison.OrdinalIgnoreCase) && !info.DataTypeInfo.ToString().Equals("STRING", StringComparison.OrdinalIgnoreCase))
        //                return CheckTypeResult.UnmatchedTagType;
        //        }
        //    }


        //    //if (para.DataType.Equals("STRING"))
        //    //{
        //    //    if (info != null)
        //    //    {
        //    //        if (info.DataTypeInfo.DataType.IsStringType)
        //    //            return CheckTypeResult.Passed;

        //    //        return CheckTypeResult.UnmatchedTagType;
        //    //    }

        //    //    //TODO:检验val和关联的其他参数是否匹配
        //    //    return CheckTypeResult.Passed;
        //    //}

        //    if (info != null)
        //    {
        //        string tagDataType = info.DataTypeInfo.ToString();
        //        if (para.DataType == tagDataType)
        //            return CheckTypeResult.Passed;

        //        if (para.AcceptTypes?.Contains(tagDataType) == true)
        //        {
        //            return CheckTypeResult.Passed;
        //        }

        //        if (para.DataType == "BOOL" && info.IsBit)
        //        {
        //            if (IsOutOfRange(info.Field, info.BitOffset))
        //                return CheckTypeResult.InvalidBitSpecifier;
        //            return CheckTypeResult.Passed;
        //        }

        //        if (para.Formats?.Contains(FormatType.TagArray) == true)
        //        {
        //            if (info.DataTypeInfo.Dim1 > 0 &&
        //                para.AcceptTypes?.Contains(info.DataTypeInfo.DataType.Name) == true)
        //                return CheckTypeResult.Passed;

        //            if (info.Tag.DataTypeInfo.Dim1 > 0 &&
        //                para.AcceptTypes?.Contains(info.DataTypeInfo.DataType.Name) == true)
        //                return CheckTypeResult.Passed;
        //        }

        //        return CheckTypeResult.UnmatchedTagType;
        //    }

        //    //var enumVal = Instruction.Utils.ParseEnum(actualPara);
        //    //if (enumVal >= 0 && (para.DataType == "SINT" || para.DataType == "DINT"))
        //    //    return CheckTypeResult.Passed;

        //    // Tag名不为数字
        //    //double doubleVal;
        //    bool isNumber = RLLGrammarAnalysis.IsNumber(actualPara);//double.TryParse(actualPara, out doubleVal);

        //    if (!isNumber && !isStringConstant)
        //    {
        //        if (para.DataType == "routine")
        //        {
        //            if (_collection.ParentProgram.Routines[actualPara] != null)
        //                return CheckTypeResult.Passed;

        //            return CheckTypeResult.UnknownRoutine;
        //        }

        //        //TODO: 校验Tag名的合法性
        //        if (!IsLegalTagName(actualPara))
        //        {
        //            if (actualPara.Contains("?"))
        //                return CheckTypeResult.MissingArgument;

        //            return CheckTypeResult.TagUndefined;
        //        }

        //        if (actualPara.Equals("s:fs", StringComparison.OrdinalIgnoreCase))
        //        {
        //            if (para.DataType == "BOOL" || para.AcceptTypes?.Contains("BOOL") == true)
        //                return CheckTypeResult.Passed;

        //            return CheckType(para.Type, actualPara);
        //        }

        //        return CheckTypeResult.TagUndefined;
        //    }

        //    //Check Format
        //    var formats = para.Formats;
        //    if (formats != null)
        //    {
        //        if (formats.Count == 1)
        //        {
        //            if (formats[0] == FormatType.Tag)
        //                return CheckTypeResult.UnmatchedValueType;
        //        }
        //    }

        //    CheckTypeResult checkPass = CheckTypeResult.Unknown;
        //    if (para.AcceptTypes == null)
        //    {
        //        checkPass = CheckType(para.DataType, actualPara);
        //    }

        //    if (checkPass != CheckTypeResult.Passed && para.AcceptTypes != null)
        //        foreach (string type in para.AcceptTypes)
        //        {
        //            checkPass = CheckType(type, actualPara);

        //            //有验证通过的类型，则不需要继续验证
        //            if (checkPass == CheckTypeResult.Passed)
        //                break;
        //        }
        //    return checkPass;
        //}

        private CheckTypeResult CheckType(string type, string actualPara)
        {
            CheckTypeResult checkPass;
            if (type == "SINT")
            {
                checkPass = CheckPass<byte>(actualPara);
            }
            else if (type == "INT")
            {
                checkPass = CheckPass<short>(actualPara);
            }
            else if (type == "DINT")
            {
                checkPass = CheckPass<int>(actualPara);
            }
            else if (type == "LINT")
            {
                checkPass = CheckPass<long>(actualPara);
            }
            else if (type == "REAL")
            {
                checkPass = CheckPass<float>(actualPara);
            }
            else if (type == "LREAL")
            {
                checkPass = CheckPass<double>(actualPara);
            }
            else if (type == "USINT")
            {
                checkPass = CheckPass<sbyte>(actualPara);
            }
            else if (type == "UINT")
            {
                checkPass = CheckPass<ushort>(actualPara);
            }
            else if (type == "UDINT")
            {
                checkPass = CheckPass<uint>(actualPara);
            }
            else if (type == "ULINT")
            {
                checkPass = CheckPass<ulong>(actualPara);
            }
            else if (type == "BOOL")
            {
                checkPass = CheckBool(actualPara);
            }
            else
            {
                checkPass = CheckTypeResult.UnmatchedValueType;
            }

            return checkPass;
        }

        private CheckTypeResult CheckBool(string val)
        {
            if (val == "0" || val == "1" || val.Equals("s:fs", StringComparison.OrdinalIgnoreCase))
                return CheckTypeResult.Passed;
            return CheckTypeResult.UnmatchedValueType;
        }

        private CheckTypeResult CheckPass<T>(string val)
        {
            try
            {
                if (val.StartsWith("2#"))
                {
                    var binStr = val.Substring(2);
                    var rslt = CanConvert<T>(binStr, 2);
                    if (rslt)
                        return CheckTypeResult.Passed;
                }
                if (val.StartsWith("8#"))
                {
                    var binStr = val.Substring(2);
                    var rslt = CanConvert<T>(binStr, 8);
                    if (rslt)
                        return CheckTypeResult.Passed;
                }
                if (val.StartsWith("16#"))
                {
                    var binStr = val.Substring(3);
                    var rslt = CanConvert<T>(binStr, 16);
                    if (rslt)
                        return CheckTypeResult.Passed;
                }     // ReSharper disable once UnusedVariable
                var result = (T)Convert.ChangeType(val, typeof(T));
                return CheckTypeResult.Passed;
            }
            catch (OverflowException)
            {
                return CheckTypeResult.Overflow;
            }
            catch (FormatException)
            {
                return CheckTypeResult.UnmatchedValueType;
            }
            catch (Exception)
            {
                // ignored
            }

            return CheckTypeResult.Unknown;
        }

        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
        private bool CanConvert<T>(string strVal, int fromBase)
        {
            if (default(T) is byte)
            {
                Convert.ToByte(strVal, fromBase);
                return true;
            }
            if (default(T) is short)
            {
                Convert.ToInt16(strVal, fromBase);
                return true;
            }
            if (default(T) is int)
            {
                Convert.ToInt32(strVal, fromBase);
                return true;
            }
            if (default(T) is long)
            {
                Convert.ToInt64(strVal, fromBase);
                return true;
            }
            if (default(T) is sbyte)
            {
                Convert.ToSByte(strVal, fromBase);
                return true;
            }
            if (default(T) is ushort)
            {
                Convert.ToUInt16(strVal, fromBase);
                return true;
            }
            if (default(T) is uint)
            {
                Convert.ToUInt32(strVal, fromBase);
                return true;
            }
            if (default(T) is ulong)
            {
                Convert.ToUInt64(strVal, fromBase);
                return true;
            }

            throw new NotSupportedException();
        }

        enum CheckTypeResult
        {
            Passed,
            TagUndefined,
            Overflow,
            InvalidBitSpecifier,
            UnmatchedValueType,
            UnmatchedTagType,
            InvalidExpression,
            InvalidExpressionOrTag,
            UnknownRoutine,
            MissingArgument,
            NotArray,
            ImmediateValueOutOfRange,
            MovMixString,
            InvalidArraySubscript,
            Unknown
        }

        public ASTRLLSequence ParseRung(List<string> codes, string text, int rungIndex, RLLRoutine routine)
        {
            _codes = codes;
            _routine = routine;
            _collection = routine.ParentCollection.ParentProgram.Tags;
            _offset = 0;
            _text = text;
            _rungIndex = rungIndex;
            _outputOffset = 0;
            return ParseSequence();
        }
    }

    public class ErrorInfo : Exception
    {
        public int RungIndex { get; set; }
        public string Info { get; set; }
        public Level Level { get; set; }
        public int Offset { get; set; }
    }

    public enum Level
    {
        Message,
        Warning,
        Error
    }
}
