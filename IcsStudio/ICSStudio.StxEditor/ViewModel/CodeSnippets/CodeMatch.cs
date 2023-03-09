using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using ICSStudio.AvalonEdit;
using ICSStudio.AvalonEdit.CodeCompletion;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Instruction;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.StxEditor.Menu;
using ICSStudio.StxEditor.ViewModel.IntelliPrompt;

namespace ICSStudio.StxEditor.ViewModel.CodeSnippets
{
    class CodeMatch
    {
        public static List<IDataTypeMember> Match(string code, IProgramModule program)
        {
            List<IDataTypeMember> matchedCode = new List<IDataTypeMember>();

            if (string.IsNullOrEmpty(code)) return matchedCode;
            int dotPos = code.LastIndexOf(".");
            if (dotPos == -1) return matchedCode;
            string pCode = code.Substring(0, dotPos);
            string mCode = code.Substring(dotPos + 1);
            
            var type = ObtainValue.GetDataTypeInfoIgnoreDims(code, program) as CompositiveType;
            if (type != null)
            {
                foreach (var member in type.TypeMembers)
                {
                    if (CompletionList.IsFuzzyMatch(member.Name, mCode.ToCharArray()))
                    {
                        matchedCode.Add(member);
                    }
                }
            }
            return matchedCode;
        }
    }

    class InstrMatch
    {
        public enum Type
        {
            Tag,
            Routine,
        }

        public static bool Match(TextEditor editor,IRoutine parent,IList<ICompletionData> data,SnippetLexer snippetLexer)
        {
            try
            {
                int offset=0;
                var codes = GetSnippetByPosition.GetParentCode(editor,ref offset ,true);
                if (codes == null) return false;
                var code = string.Join(".", codes);

                if (string.IsNullOrEmpty(code)) return false;
                string name = "";
                if (GetSnippetByPosition.IsInstr(code,ref name))
                {
                    if (string.IsNullOrEmpty(name)) return false;
                    int count = 0;
                    var parameters = MenuSelector.GetInstrParameters(new SnippetInfo(editor.Text)
                        {CodeText = name, Offset = offset},ref count,snippetLexer.GetEndOfCode(offset,editor.Text),(STRoutine)parent);
                    var instr = ((Controller)parent.ParentController).STInstructionCollection.FindInstruction(name);
                    if (parameters.Count > count)
                    {
                        return true;
                    }

                    if (instr.Name.Equals("GSV") || instr.Name.Equals("SSV"))
                    {
                        return false;
                    }

                    if (instr.Name.Equals("RET") || instr.Name.Equals("SBR"))
                    {
                        return true;
                    }

                    if (instr.Name.Equals("JSR"))
                    {
                        foreach (var r in parent.ParentCollection)
                        {
                            var stxCompletionItemRoutineData = new StxCompletionItemRoutineData(r.Name);
                            var m=new StxCompletionItem(stxCompletionItemRoutineData);
                            data.Add(m);
                        }

                        return true;
                    }
                    //TODO(zyl):params list
                    //var @params = instr.GetParameterInfo();
                    //if (!@params.Any()) return true;
                    //int index = Math.Max(parameters.Count - 1, 0);
                    //var param = @params[index];
                
                    return false;
                }

                return false;
            }
            catch (Exception e)
            {
                Debug.Assert(false,e.StackTrace);
                return false;
            }
        }
        
    }
}
