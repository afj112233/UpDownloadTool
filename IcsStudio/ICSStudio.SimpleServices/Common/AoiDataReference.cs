using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Annotations;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.Utils.TagExpression;

namespace ICSStudio.SimpleServices.Common
{
    public class AoiDataReference : INotifyPropertyChanged,IDisposable
    {
        private string _title;
        private ASTInstr _astInstr;

        public AoiDataReference(AoiDefinition aoi, IRoutine routine, ASTInstr astInstr,OnlineEditType onlineEditType,
            string title = "")
        {
            OnlineEditType = onlineEditType;
            _astInstr = astInstr;
            ReferenceAoi = aoi;
            _title = title;
            Routine = routine;
            ParamList = astInstr?.param_list.nodes.ToList();
            Offset = astInstr?.ContextStart ?? -1;
            if (ParamList != null)
            {
                foreach (var node in ParamList)
                {
                    if (node is ASTName)
                    {
                        var tagName = ObtainValue.GetAstName((ASTName) node);
                        TransformList.Add(tagName);
                        continue;
                    }

                    if (node is ASTInteger)
                    {
                        TransformList.Add(((ASTInteger) node).value.ToString());
                        continue;
                    }

                    if (node is ASTFloat)
                    {
                        TransformList.Add(((ASTFloat) node).value.ToString());
                        continue;
                    }

                    if (node is ASTBinLogicOp)
                    {
                        continue;
                    }
                    Debug.Assert(false);
                }

                CreateTransformTable();
            }
        }

        public OnlineEditType OnlineEditType { get; }
        public ASTInstr AoiContext => _astInstr;

        public AoiDefinition ReferenceAoi { get; }

        public Hashtable TransformTable { private set; get; }

        public override string ToString()
        {
            return _title;
        }

        public string GetContext()
        {
            var st = Routine as STRoutine;
            if (st != null)
            {
                var code = string.Join("\n", st.CodeText);
                return code.Substring(_astInstr.ContextStart, _astInstr.ContextEnd - _astInstr.ContextStart + 1);
            }

            return string.Empty;
        }

        private void CreateTransformTable()
        {
            TransformTable = new Hashtable();
            foreach (var tag in ReferenceAoi.Tags)
            {
                if (tag.Usage == Usage.InOut)
                {
                    TransformTable[tag.Name.ToUpper()] = TransformList[ReferenceAoi.GetIndexOfAoiParameters(tag) + 1];
                }
                else
                {
                    TransformTable[tag.Name.ToUpper()] = $"{TransformList[0]}.{tag.Name}";
                }
            }
        }

        public IProgramModule GetReferenceProgram()
        {
            IProgramModule program = Routine?.ParentCollection.ParentProgram;
            if (InnerDataReference != null)
            {
                program = InnerDataReference.GetReferenceProgram();
            }

            return program;
        }

        public string GetTransformName(string tagName)
        {
            var splitName = tagName.Split('.');
            string dim = "";
            var specific = SplitDim(splitName[0].ToUpper(), ref dim);
            var transformName = (string) TransformTable[specific];
            splitName[0] = transformName + dim;
            if (InnerDataReference != null)
            {
                transformName = InnerDataReference.GetTransformName(string.Join(".", splitName));
                return transformName;
            }

            return string.Join(".", splitName);
        }

        private string SplitDim(string name,ref string dim)
        {
            try
            {
                var tmp = name;
                var index = tmp.IndexOf("[");
                if (index>-1)
                {
                    dim = tmp.Substring(index);
                    return tmp.Substring(0, index);
                }
                else
                {
                    return name;
                }
            }
            catch (Exception e)
            {
                Debug.Assert(false,e.StackTrace);
                return name;
            }

        }

        public IRoutine GetTargetRoutine()
        {
            var inner = InnerDataReference;
            while (inner != null)
            {
                if (inner.InnerDataReference == null)
                    return inner.Routine;
                inner = inner.InnerDataReference;
            }

            return Routine;
        }

        public List<string> TransformList { get; } = new List<string>();

        public string Title
        {
            set
            {
                _title = value;
                OnPropertyChanged();
            }
            get { return _title; }
        }

        public IRoutine Routine { get; }

        public List<ASTNode> ParamList { get; }

        public int Offset { get; }

        public int Line { set; get; }

        public int Column { set; get; }

        public AoiDataReference InnerDataReference { set; get; }

        public AoiDefinition InnerAoiDefinition { set; get; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isDisposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
        {
            if (_isDisposed) return;
            _isDisposed = true;
            if (isDisposing)
            {
                ParamList.Clear();
                InnerDataReference = null;
                InnerAoiDefinition = null;
                _title = null;
                TransformList.Clear();
                TransformTable = null;
                _astInstr = null;
            }
        }
    }

    public static class AoiDataReferenceExtend
    {
        public static bool CompareIndex(AoiDataReference a, AoiDataReference b)
        {
            if (a?.ParamList == null) return false;
            if (b?.ParamList == null) return false;
            var aTagName = ObtainValue.GetAstName(a.ParamList?[0] as ASTName);
            var bTagName = ObtainValue.GetAstName(b.ParamList?[0] as ASTName);
            if (string.Equals(aTagName, bTagName, StringComparison.OrdinalIgnoreCase))
            {
                if (aTagName.EndsWith("]") && bTagName.EndsWith("]"))
                {
                    int aDim1 = 0, aDim2 = 0, aDim3 = 0;
                    int bDim1 = 0, bDim2 = 0, bDim3 = 0;
                    var index1 = aTagName.LastIndexOf("[");
                    var dimsA = aTagName.Substring(index1, aTagName.Length - index1);
                    index1 = bTagName.LastIndexOf("[");
                    var dimsB = bTagName.Substring(index1, bTagName.Length - index1);
                    var res1 = ObtainValue.ParseDims(dimsA, ref aDim1, ref aDim2, ref aDim3);
                    var res2 = ObtainValue.ParseDims(dimsB, ref bDim1, ref bDim2, ref bDim3);
                    if (res1 && res2)
                    {
                        var offset1 = Math.Max(1, aDim1) * Math.Max(1, aDim2) * Math.Max(1, aDim3);
                        var offset2 = Math.Max(1, bDim1) * Math.Max(1, bDim2) * Math.Max(1, bDim3);
                        return offset1 > offset2;
                    }
                }
                return CompareIndex(a.InnerDataReference, b.InnerDataReference);
            }

            return String.CompareOrdinal(aTagName, bTagName) > 0;
        }

        public static Hashtable GetFinalTransformTable(this AoiDataReference dataContext)
        {
            Hashtable finalHashtable = new Hashtable(dataContext.TransformTable);

            AoiDataReference nextDataContext = dataContext.InnerDataReference;

            TagExpressionParser parser = new TagExpressionParser();

            List<string> keyList = new List<string>();
            foreach (var hashtableKey in finalHashtable.Keys)
            {
                keyList.Add((string)hashtableKey);
            }

            while (nextDataContext != null)
            {
                foreach (var key in keyList)
                {
                    var operand = (string)finalHashtable[key];

                    try
                    {
                        var tagExpression = parser.Parser(operand);
                        var simpleTagExpression = parser.GetSimpleTagExpression(tagExpression);
                        if (simpleTagExpression != null)
                        {
                            string replaceOperand =
                                (string)nextDataContext.TransformTable[simpleTagExpression.TagName.ToUpper()];
                            simpleTagExpression.TagName = replaceOperand;

                            finalHashtable[key] = tagExpression.ToString();
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }

                }

                nextDataContext = nextDataContext.InnerDataReference;
            }

            return finalHashtable;
        }
    }
}
