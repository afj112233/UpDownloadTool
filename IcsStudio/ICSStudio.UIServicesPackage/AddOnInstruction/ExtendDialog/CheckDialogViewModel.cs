using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.Tags;

namespace ICSStudio.UIServicesPackage.AddOnInstruction.ExtendDialog
{
    public class CheckDialogViewModel : ViewModelBase
    {
        private ReferenceItem _selectedItem;
        private bool _isOnlyShowChangedParams;
        private bool? _dialogResult;

        public CheckDialogViewModel(AoiDefinition aoiDefinition, List<Tuple<string, bool, ITag>> paramInfos)
        {
            OkCommand = new RelayCommand(ExecuteOkCommand);
            NoCommand = new RelayCommand(ExecuteNoCommand);
            Content1 = string.Format(
                LanguageManager.GetInstance().ConvertSpecifier("CheckDialogHint1"),
                aoiDefinition.Name
                );
            Content2 = string.Format(
                LanguageManager.GetInstance().ConvertSpecifier("CheckDialogHint2"),
                aoiDefinition.Name
                );

            foreach (var reference in aoiDefinition.References)
            {
                var item = new ReferenceItem(reference, paramInfos);
                Items.Add(item);
            }
            SelectedItem = Items[0];
        }

        public List<ReferenceItem> Items { get; } = new List<ReferenceItem>();

        public ReferenceItem SelectedItem
        {
            set
            {
                Set(ref _selectedItem, value);
                SelectedArgumentItems.Clear();
                foreach (var item in _selectedItem.Items)
                {
                    if (IsOnlyShowChangedParams)
                    {
                        if (item.IsDirty)
                            SelectedArgumentItems.Add(item);
                    }
                    else
                    {
                        SelectedArgumentItems.Add(item);
                    }
                }
            }
            get { return _selectedItem; }
        }

        public bool IsOnlyShowChangedParams
        {
            set
            {
                _isOnlyShowChangedParams = value;
                SelectedItem = SelectedItem;
            }
            get { return _isOnlyShowChangedParams; }
        }

        public ObservableCollection<ArgumentItem> SelectedArgumentItems { get; } =
            new ObservableCollection<ArgumentItem>();

        public string Content1 { get; }
        public string Content2 { get; }

        public RelayCommand OkCommand { get; }

        private void ExecuteOkCommand()
        {
            DialogResult = true;
        }

        public RelayCommand NoCommand { get; }

        private void ExecuteNoCommand()
        {
            DialogResult = false;
        }

        public bool? DialogResult
        {
            set { Set(ref _dialogResult, value); }
            get { return _dialogResult; }
        }

        public bool IsOpenCrossReference { set; get; }
    }

    public class ReferenceItem
    {
        private readonly AoiDataReference _aoiDataReference;
        private List<Tuple<string, bool, ITag>> _paramInfos;
        public ReferenceItem(AoiDataReference reference, List<Tuple<string, bool, ITag>> paramInfos)
        {
            _paramInfos = paramInfos;
            _aoiDataReference = reference;
            Container = reference.Routine.ParentCollection.ParentProgram.Name;
            Routine = reference.Routine.Name;
            Location = $"Line {reference.Line}";
            var paramList = reference.ParamList.ToList();
            paramList.RemoveAt(0);
            var tmp = paramList.ToList();
            //var delList = new List<Tuple<string, bool, ITag>>();
            foreach (var paramInfo in paramInfos)
            {
                var index = GetArgumentIndex(reference.ReferenceAoi, paramInfo.Item3.Name);
                if (string.IsNullOrEmpty(paramInfo.Item1))
                {
                    var p = paramList[index];
                    var item2 = new ArgumentItem() { Parameter = "<Unknown>", IsDirty = false, Argument = GetNodeName(p), Start = p.ContextStart, End = p.ContextEnd };
                    item2.Context = $"(*unknown:{item2.Argument}*)";
                    Items.Add(item2);
                    tmp.Remove(p);
                    continue;
                }

                var item = new ArgumentItem();
                if (index == -1)
                {
                    item.Argument = "";
                    item.Context = ",";
                }
                else
                {
                    var p = paramList[index];
                    item.Argument = GetNodeName(p);
                    item.Start = p.ContextStart;
                    item.End = p.ContextEnd;
                    tmp.Remove(p);
                    item.Context = item.Argument;
                }

                item.Parameter = paramInfo.Item1;
                item.IsDirty = paramInfo.Item2;
                Items.Add(item);
            }
            Debug.Assert(tmp.Count == 0);
            //foreach (var node in tmp)
            //{
            //    var item = new ArgumentItem() { Parameter = "<Unknown>", IsDirty = false, Argument = GetNodeName(node),Start = node.ContextStart,End = node.ContextEnd};
            //    item.Context = $"(*unknown:{item.Argument}*)";
            //    Items.Add(item);
            //}
        }

        public void UpdateInstrParam()
        {
            try
            {
                //TODO(zyl):编辑器中的代码可能还没编译，会造成意想不到的问题
                var st = _aoiDataReference.Routine as STRoutine;
                if (st != null)
                {
                    var aoiContext = _aoiDataReference.OnlineEditType == OnlineEditType.Original
                        ? string.Join("\n", st.CodeText)
                        : string.Join("\n", st.PendingCodeText);
                    var paramCount = _aoiDataReference.ParamList.Count;
                    Queue<int> points = new Queue<int>();
                    for (int i = paramCount - 1; i >= 1; i--)
                    {
                        var astNode = _aoiDataReference.ParamList[i];
                        var start = astNode.ContextStart;
                        var end = astNode.ContextEnd;
                        if (IsParamDel(i))
                        {
                            start = FindComma(aoiContext, start);
                        }
                        //aoiContext = aoiContext.Remove(start, end - start + 1);
                        //aoiContext = aoiContext.Insert(start, "".PadLeft(end-start+1));
                        points.Enqueue(start);
                        points.Enqueue(end);
                    }
                    var paramList = new List<string>();
                    var index = 0;
                    foreach (var p in Items.Select(item => item.Context).ToList())
                    {
                        if (index == 0)
                        {
                            paramList.Add(p);
                            index++;
                        }
                        else
                        {
                            if (p == ",")
                            {
                                var item = paramList[index - 1];
                                paramList[index - 1] = $"{item}{p}";
                            }
                            else
                            {
                                paramList.Add(p);
                                index++;
                            }
                        }
                    }

                    var lastEnd = -1;
                    var lastStart = _aoiDataReference.ParamList.Last().ContextEnd;
                    for (int i = paramList.Count - 1; i >= 0; i--)
                    {
                        var p = paramList[i];
                        if (points.Any())
                        {
                            lastStart = points.Dequeue();
                            lastEnd = points.Dequeue();
                        }

                        if (lastEnd == -1)
                        {
                            aoiContext = aoiContext.Insert(lastStart + 1, p);
                        }
                        else
                        {
                            aoiContext = aoiContext.Remove(lastStart, lastEnd - lastStart + 1);
                            aoiContext = aoiContext.Insert(lastStart, p);
                        }
                    }

                    if (_aoiDataReference.OnlineEditType == OnlineEditType.Original)
                        st.CodeText = aoiContext.Split('\n').ToList();
                    else
                        st.PendingCodeText = aoiContext.Split('\n').ToList();
                }
            }
            catch (Exception e)
            {
                Controller.GetInstance().Log($"UpdateInstrParam():{e}");
            }
            //aoiContext = string.Format(aoiContext, args: paramList.ToArray());
        }

        private int FindComma(string context, int end)
        {
            while (end > 0)
            {
                var c = context[end];
                if (c.Equals(',')) return end;
                end--;
            }

            return -1;
        }

        private bool IsParamDel(int paramIndex)
        {
            var param = _aoiDataReference.ReferenceAoi.instr.GetParameterInfo()[paramIndex];
            var paramInfo = _paramInfos.FirstOrDefault(p => p.Item3.Name.Equals(param.Item1));
            return string.IsNullOrEmpty(paramInfo?.Item1);
        }

        private int GetArgumentIndex(AoiDefinition aoi, string tag)
        {
            var paramList = aoi.instr.GetParameterInfo();
            paramList.RemoveAt(0);
            for (int i = 0; i < paramList.Count; i++)
            {
                var p = paramList[i];
                if (p.Item1.Equals(tag, StringComparison.OrdinalIgnoreCase)) return i;
            }

            return -1;
        }

        private string GetNodeName(ASTNode node)
        {
            var astName = node as ASTName;
            if (astName != null)
            {
                return ObtainValue.GetAstName(astName);
            }

            var integer = node as ASTInteger;
            if (integer != null)
            {
                return ((int)integer.value).ToString();
            }

            var floatNode = node as ASTFloat;
            if (floatNode != null)
            {
                return (floatNode.value).ToString();
            }
            Debug.Assert(false);
            return "";
        }
        public string Container { set; get; }

        public string Routine { set; get; }

        public AoiDataReference AoiDataReference => _aoiDataReference;

        public string Location { set; get; }

        public int Start => _aoiDataReference.AoiContext.ContextStart;

        public List<ArgumentItem> Items { get; } = new List<ArgumentItem>();
    }

    public class ArgumentItem
    {
        public string Parameter { set; get; }

        public string Argument { set; get; }

        public string Sign => IsDirty ? "*" : "";

        public string Context { set; get; }

        public bool IsDirty { set; get; }

        public int Start { set; get; } = -1;
        public int End { set; get; } = -1;
    }
}
