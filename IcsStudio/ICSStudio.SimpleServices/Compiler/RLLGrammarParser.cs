using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Antlr4.Runtime.Misc;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using Antlr4.Runtime;
using System;
using System.Runtime.Remoting.Messaging;
using ICSStudio.SimpleServices.Instruction;
using Xunit;

namespace ICSStudio.SimpleServices.Compiler
{

    public class RLLRungParser
    {
        private int offset_;
        private string text_;
        private Controller ctrl;
        public RLLRungParser(Controller ctrl)
        {
            this.ctrl = ctrl;
        }
        static bool IsBracketMatch(string text)
        {
            return true;
        }

        char Curr()
        {
            return text_[offset_];
        }

        void SkipWhitespace()
        {
            while (offset_ < text_.Length && Curr() == ' ')
            {
                offset_ += 1;
            }
        }
        void Advance(int step)
        {
            Debug.Assert(step >= 0);
            offset_ += step;
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
            while (true)
            {
                parallel.Add(ParseSequence());
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
            char[] pattern = { '[', ',' };

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

        private static int FindMatchedParenthese(string text, int offset)
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
                    continue;
                }

            }

            Debug.Assert(false, "");
            return -1;

        }

        ASTRLLInstruction ParseInstruction()
        {
            SkipWhitespace();
            var end = text_.IndexOf('(', offset_);
            var name = text_.Substring(offset_, end - offset_).Trim();
            Advance(end - offset_);
            Consume('(');
            end = FindMatchedParenthese(text_, offset_);
            //end = text_.IndexOf(')', offset_);

            var paramList = ParseParameters(text_.Substring(offset_, end - offset_)).ToArray();
            //var param_list = text_.Substring(offset_, end - offset_).Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < paramList.Length; ++i)
            {
                paramList[i] = paramList[i].Trim();
            }
            Advance(end - offset_);
            Consume(')');

            IXInstruction instr = null;
            {
                //ctrl.AoiDefinitionCollection.
                //ctrl.AoiDefinitions
                //First Find from AOI, then RLLInstruction
                //ctrl.AoiDefinitionCollection
                var tmp = (ctrl.AOIDefinitionCollection as AoiDefinitionCollection)?.Find(name);
                if (tmp != null)
                {
                    instr = tmp.instr;
                }
                else
                {
                    instr = ctrl.RLLInstructionCollection.FindInstruction(name);

                }
            }

            return new ASTRLLInstruction(name, instr, paramList.ToList());
        }

        public ASTRLLSequence ParseRung(string text)
        {
            offset_ = 0;
            text_ = text;
            return ParseSequence();
        }

    }

    public class RLLGrammarParser
    {
        public ASTRLLRoutine ParseRoutine(List<string> codes, Controller ctrl)
        {
            ASTRLLRoutine routine = new ASTRLLRoutine();
            RLLRungParser parser = new RLLRungParser(ctrl);
            foreach (var code in codes)
            {
                routine.Add(parser.ParseRung(code));
            }
            return routine;
        }

        public static ASTRLLRoutine Parse(List<string> codes, Controller ctrl)
        {
            RLLGrammarParser parser = new RLLGrammarParser();
            return parser.ParseRoutine(codes, ctrl);
        }
    }
}