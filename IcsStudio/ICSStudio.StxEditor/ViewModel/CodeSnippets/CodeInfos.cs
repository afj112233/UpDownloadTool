using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.AvalonEdit.Document;
using ICSStudio.AvalonEdit.Variable;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Instruction;

namespace ICSStudio.StxEditor.ViewModel.CodeSnippets
{
    internal class SnippetInfo
    {
        private List<VariableInfo> _variableInfos = new List<VariableInfo>();
        private string _codeText;
        private string _originalCodeText;

        public SnippetInfo(string parent)
        {
            Parent = parent;
        }

        public bool IsRoutine { set; get; }

        public int Offset { set; get; }

        public int EndOffset => Offset + CodeText.Length - 1;
        
        public bool IsGrammarError { set; get; }

        public string CodeText
        {
            set
            {
                if (!string.IsNullOrEmpty(value) && value[value.Length - 1] == ':')
                    value = value.Substring(0, value.Length - 1);
                _codeText = value;
                if (string.IsNullOrEmpty(OriginalCodeText))
                    OriginalCodeText = value;
            }
            get { return _codeText; }
        }

        public string OriginalCodeText
        {
            private set { _originalCodeText = value; }
            get
            {
                return _originalCodeText;
            }
        }

        public bool HiddenVariable { set; get; } = false;

        public bool IsCurrent { set; get; } = true;

        public int ErrorStart { set; get; } = -1;

        public string Parent { get; }

        public string DataType { set; get; }
        
        public List<VariableInfo> GetVariableInfos()
        {
            return _variableInfos;
        }

        private bool VariableCompare(IVariableInfo a, IVariableInfo b)
        {
            return a.IsRoutine == b.IsRoutine && a.IsProgram == b.IsProgram && a.IsEnum == b.IsEnum &&
                   a.IsAOI == b.IsAOI && a.IsTask == b.IsTask && a.IsInstr == b.IsInstr && a.IsModule == b.IsModule;
        }
        public object SyncRoot=new object();
        public void AddVariable(VariableInfo info)
        {
            lock (SyncRoot)
            {
                if (info.IsCorrect)
                {
                    if (info.Offset > 0 && info.TextLocation.Column == 0 && info.TextLocation.Line == 0)
                    {
                        Debug.Assert(false);
                    }

                    var variableInfos =_variableInfos.Where(x =>
                        x.Name.Equals(info.Name, StringComparison.OrdinalIgnoreCase) && x.IsDisplay &&
                        VariableCompare(x, info)).ToList();
                    if (variableInfos.Any())
                    {
                        if (info.IsDisplay && !info.IsInstr && !info.IsEnum)
                        {
                            if (variableInfos.FirstOrDefault(p => p.Offset == info.Offset) == null)
                                _variableInfos.Add(info);
                            //info.Dispose();
                        }
                        else
                        {
                            _variableInfos.Add(info);
                        }
                    }
                    else
                    {
                        _variableInfos.Add(info);
                    }
                }
                else
                {
                    if (info.Offset > 0 && info.TextLocation.Column == 0 && info.TextLocation.Line == 0)
                    {
                        Debug.Assert(false);
                    }

                    _variableInfos.Add(info);
                }
            }
        }

        public void SetErrorStart(int pos)
        {
            if (pos == -1 || pos >= CodeText.Length) return;
            if (ErrorStart == -1)
            {
                ErrorStart = pos;
            }
            else
            {
                if (ErrorStart > pos)
                {
                    ErrorStart = pos;
                }
            }
        }

        public List<string> Enums { set; get; } = new List<string>();

        public bool IsEnable { set; get; } = true;
        public bool IsDisplay { set; get; } = true;
        public string ErrorInfo { set; get; }
        public bool IsEnum { set; get; } = false;
        public bool IsProgram { set; get; }
        public bool IsModule { set; get; }
        public bool IsTask { set; get; }
        public bool IsAOI { set; get; }
        public bool IsInstr { set; get; }
        public bool IsNum { set; get; }
        public bool IsUnknown { set; get; }
        public bool IsAstNameNode { set; get; }
        public void Clean()
        {
            _variableInfos.Clear();
            Enums.Clear();
        }

    }
    
    internal class InstrEnumInfo
    {
        private string _instrName;

        public string InstrName
        {
            set
            {
                if (string.IsNullOrEmpty(_instrName))
                    _instrName = value;

            }
            get { return _instrName; }
        }

        public int InstrEnumPosition { set; get; }

        public List<string> Enums { private set; get; }

        public string Interrelated { set; get; }

        public bool GetEnum(IRoutine parent)
        {
            var type = typeof(AllFInstructions);
            var instrType = type.GetFields().FirstOrDefault(i => i.Name == InstrName);
            var instr = (FInstruction)instrType?.GetValue(type);
            Enums = instr?.GetInstrEnumType(InstrEnumPosition, Interrelated)?.Item1;
            return Enums != null;
        }
    }
}
