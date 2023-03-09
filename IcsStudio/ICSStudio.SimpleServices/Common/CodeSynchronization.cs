using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Notification;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Notification;

namespace ICSStudio.SimpleServices.Common
{
    public class CodeSynchronization
    {
        // ReSharper disable once InconsistentNaming
        private static readonly CodeSynchronization _codeSynchronization = new CodeSynchronization();

        private CodeSynchronization()
        {

        }

        public static CodeSynchronization GetInstance()
        {
            return _codeSynchronization;
        }

        private Dictionary<IRoutine, List<UpdateCodeParameter>> _pendingCodeUpdateDictionary =
            new Dictionary<IRoutine, List<UpdateCodeParameter>>();

        private Dictionary<IRoutine, List<UpdateCodeParameter>> _waitCodeUpdateDictionary =
            new Dictionary<IRoutine, List<UpdateCodeParameter>>();

        private readonly object _syncRoot = new object();

        public void Add(IRoutine routine, UpdateCodeParameter updateCodeParameter)
        {
            lock (_syncRoot)
            {
                if (_isUpdate == 1)
                {
                    if (_waitCodeUpdateDictionary.ContainsKey(routine))
                    {
                        var list = _waitCodeUpdateDictionary[routine];
                        if (updateCodeParameter.OnlineEditType == null)
                        {
                            list.Add(updateCodeParameter);
                        }
                        else
                        {
                            if (updateCodeParameter.Regex == null)
                            {
                                list.Add(updateCodeParameter);
                            }
                            else
                            {
                                var parameter = list.FirstOrDefault(l => l.Regex.Equals(updateCodeParameter.Regex));
                                if (parameter == null)
                                    list.Add(updateCodeParameter);
                                else
                                {
                                    list.Remove(parameter);
                                    list.Add(updateCodeParameter);
                                }
                            }
                        }
                    }
                    else
                    {
                        var list = new List<UpdateCodeParameter>() { updateCodeParameter };
                        _waitCodeUpdateDictionary.Add(routine, list);
                    }
                }
                else
                {
                    if (_pendingCodeUpdateDictionary.ContainsKey(routine))
                    {
                        var list = _pendingCodeUpdateDictionary[routine];
                        if (updateCodeParameter.OnlineEditType == null)
                        {
                            list.Add(updateCodeParameter);
                        }
                        else
                        {
                            if (updateCodeParameter.Regex == null)
                            {
                                list.Add(updateCodeParameter);
                            }
                            else
                            {
                                if (!list.Any(l => l.Regex.Equals(updateCodeParameter.Regex)))
                                    list.Add(updateCodeParameter);
                            }
                        }
                    }
                    else
                    {
                        var list = new List<UpdateCodeParameter>() { updateCodeParameter };
                        _pendingCodeUpdateDictionary.Add(routine, list);
                    }
                }
            }
        }

        public void Remove(IRoutine routine)
        {
            _pendingCodeUpdateDictionary.Remove(routine);
        }

        private int _isUpdate = 0;

        public void Update()
        {
            if (Interlocked.Exchange(ref _isUpdate, 1) == 0)
            {
                try
                {
                    var pendingRaiseRoutines = new ConcurrentQueue<STRoutine>();
                    Parallel.ForEach(_pendingCodeUpdateDictionary, pending =>
                    {
                        var st = (STRoutine)pending.Key;
                        bool isChanged = false;
                        if (pending.Value[0].OnlineEditType != null)
                        {
                            UpdateTagNameInCode(st, pending.Value, ref isChanged);
                        }
                        else
                        {
                            foreach (var parameter in pending.Value)
                            {
                                WaitCompiler(st);
                                var regex = new Regex(parameter.Regex);
                                st.CodeText = UpdateTagNameInCode(st.CodeText, regex, parameter.UnChangedName,
                                    parameter.ChangedName, parameter.ProgramName,
                                    ref isChanged, null, parameter.TagName);

                                if (st.PendingCodeText != null)
                                {
                                    st.PendingCodeText = UpdateTagNameInCode(st.PendingCodeText, regex,
                                        parameter.UnChangedName,
                                        parameter.ChangedName, parameter.ProgramName,
                                        ref isChanged, null, parameter.TagName);
                                }
                            }
                        }

                        if (isChanged && !pendingRaiseRoutines.Contains(st))
                        {
                            pendingRaiseRoutines.Enqueue(st);
                        }
                    });

                    foreach (var routine in pendingRaiseRoutines)
                    {
                        routine.IsError = true;
                        routine.IsUpdateChanged = true;
                        Notifications.Publish(new MessageData()
                            { Object = routine, Type = MessageData.MessageType.Verify, IsForce = true });
                    }
                }
                catch (Exception e)
                {
                    Debug.Assert(false, e.StackTrace);
                }
                finally
                {
                    _pendingCodeUpdateDictionary.Clear();
                    bool isNeedUpdate = false;
                    if (_waitCodeUpdateDictionary.Count > 0)
                    {
                        _pendingCodeUpdateDictionary = _waitCodeUpdateDictionary;
                        _waitCodeUpdateDictionary = new Dictionary<IRoutine, List<UpdateCodeParameter>>();
                        isNeedUpdate = true;
                    }

                    Interlocked.Exchange(ref _isUpdate, 0);
                    if (isNeedUpdate)
                        Update();
                }
            }

        }

        public static void WaitCompiler(IRoutine routine)
        {
            if (routine == null)
            {
                return;
            }

            var sw = new Stopwatch();
            sw.Start();
            while (routine.IsCompiling)
            {
                Thread.Sleep(500);
            }

            sw.Stop();
        }

        public static void WaitReferenceRoutineCompile(ITag tag)
        {
            var programModule = tag.ParentCollection.ParentProgram;
            var referenceRoutines = new List<STRoutine>();
            if (programModule is AoiDefinition)
            {
                foreach (var routine in programModule.Routines.OfType<STRoutine>())
                {
                    if (routine.IsCompiling && routine.GetAllReferenceTags().Contains(tag))
                        referenceRoutines.Add(routine);
                }
            }
            else
            {
                foreach (var program in Controller.GetInstance().Programs)
                {
                    foreach (var routine in program.Routines.OfType<STRoutine>())
                    {
                        if (routine.IsCompiling && routine.GetAllReferenceTags().Contains(tag))
                            referenceRoutines.Add(routine);
                    }
                }
            }

            while (true)
            {
                if (referenceRoutines.Any(r => r.IsCompiling))
                {
                    Thread.Sleep(100);
                    continue;
                }
                else
                {
                    break;
                }
            }
        }

        #region Update ST Code

        public void UpdateModuleName(DeviceModule.DeviceModule module)
        {
            Parallel.ForEach(Controller.GetInstance().Programs, program =>
            {
                foreach (var routine in program.Routines.OfType<STRoutine>())
                {
                    WaitCompiler(routine);
                    var variableInfos = routine.GetCurrentVariableInfos(OnlineEditType.Original).Where(v =>
                        v.IsModule && v.Module == module).Select(v=>v.Offset).ToList();
                    if (variableInfos.Any())
                    {
                        var updateCodeParameter = new UpdateCodeParameter(module.OldName, module.Name,
                            OnlineEditType.Original, variableInfos);
                        Add(routine, updateCodeParameter);
                    }

                    variableInfos = routine.GetCurrentVariableInfos(OnlineEditType.Pending).Where(v =>
                        v.IsModule && v.Module == module).Select(v => v.Offset).ToList();
                    if (variableInfos.Any())
                    {
                        var updateCodeParameter = new UpdateCodeParameter(module.OldName, module.Name,
                            OnlineEditType.Pending, variableInfos);
                        Add(routine, updateCodeParameter);
                    }
                }
            });
        }

        private List<string> UpdateTagNameInCode(List<string> codeList, Regex regex, string unchangedName,
            string changedName, string programName,
            ref bool isChanged, IVariableInfo variableInfo, string name)
        {
            if (codeList == null) return null;
            var code = string.Join("\n", codeList);
            var cleanCode = RoutineCodeTextExtension.ConvertCommentToWhiteBlank(code, null);
            if (!string.IsNullOrEmpty(cleanCode))
            {
                var matches = regex.Matches(cleanCode);
                if (matches.Count > 0)
                {
                    isChanged = true;
                    for (int i = matches.Count - 1; i >= 0; i--)
                    {
                        var matched = matches[i];
                        if (variableInfo != null && !variableInfo.IsInCode(matched.Index)) continue;
                        var matchedName = matched.Value;
                        if (string.IsNullOrEmpty(matchedName))
                        {
                            return codeList;
                        }

                        var memberRegex = new Regex(@"\." + unchangedName + @"(\[.*?\])?(?![\s\w\(])");
                        var match = memberRegex.Matches(matchedName);
                        if (match.Count > 0)
                        {
                            var lastMatch = match[match.Count - 1];
                            var index = lastMatch.Index + 1 + matched.Index;
                            code = code.Remove(index, unchangedName.Length);
                            code = code.Insert(index, changedName);
                        }
                    }

                    return code.Split('\n').ToList();
                }
            }

            return codeList;
        }

        private void UpdateTagNameInCode(STRoutine routine, List<UpdateCodeParameter> updateCodeParameters,
            ref bool isChanged)
        {
            var dic = new Dictionary<OnlineEditType, List<Tuple<string, string, int>>>();
            foreach (var updateCodeParameter in updateCodeParameters)
            {
                if (updateCodeParameter.OnlineEditType == null) continue;
                if (dic.ContainsKey((OnlineEditType)updateCodeParameter.OnlineEditType))
                {
                    var list = dic[(OnlineEditType)updateCodeParameter.OnlineEditType];
                    foreach (var i in updateCodeParameter.OffsetList)
                    {
                        list.Add(new Tuple<string, string, int>(updateCodeParameter.UnChangedName,
                            updateCodeParameter.ChangedName, i));
                    }
                }
                else
                {
                    var list = new List<Tuple<string, string, int>>();
                    dic[(OnlineEditType)updateCodeParameter.OnlineEditType] = list;
                    foreach (var i in updateCodeParameter.OffsetList)
                    {
                        list.Add(new Tuple<string, string, int>(updateCodeParameter.UnChangedName,
                            updateCodeParameter.ChangedName, i));
                    }
                }
            }

            foreach (var item in dic)
            {
                if (item.Key == OnlineEditType.Original)
                {
                    var codeText = string.Join("\n", routine.CodeText);
                    bool isModified = false;
                    codeText = UpdateTagNameInCode(item.Value, codeText, ref isModified);
                    if (isModified)
                    {
                        isChanged = true;
                        routine.CodeText = codeText.Split('\n').ToList();
                    }
                }

                if (item.Key == OnlineEditType.Pending)
                {
                    var codeText = string.Join("\n", routine.PendingCodeText);
                    bool isModified = false;
                    codeText = UpdateTagNameInCode(item.Value, codeText, ref isModified);
                    if (isModified)
                    {
                        isChanged = true;
                        routine.PendingCodeText = codeText.Split('\n').ToList();
                    }
                }

                if (item.Key == OnlineEditType.Test)
                {
                    var codeText = string.Join("\n", routine.TestCodeText);
                    bool isModified = false;
                    codeText = UpdateTagNameInCode(item.Value, codeText, ref isModified);
                    if (isModified)
                    {
                        isChanged = true;
                        routine.TestCodeText = codeText.Split('\n').ToList();
                    }
                }
            }
        }

        private string UpdateTagNameInCode(List<Tuple<string, string, int>> items, string code, ref bool isChanged)
        {
            foreach (var tuple in items.OrderByDescending(n => n.Item3))
            {
                code = code.Remove(tuple.Item3, tuple.Item1.Length);
                code = code.Insert(tuple.Item3, tuple.Item2);
                isChanged = true;
            }

            return code;
        }

        #endregion


    }

    public class UpdateCodeParameter
    {
        public UpdateCodeParameter(string programName, string unChangedName, string changedName, string regex,
            string tagName)
        {
            ProgramName = programName;
            if (unChangedName.EndsWith("(\\[.*?\\])?"))
                unChangedName = unChangedName.Replace("(\\[.*?\\])?", "");
            UnChangedName = unChangedName;
            if (changedName.EndsWith("(\\[.*?\\])?"))
                changedName = changedName.Replace("(\\[.*?\\])?", "");
            ChangedName = changedName;
            Regex = regex;
            TagName = tagName;
        }

        public UpdateCodeParameter(string unChangedName, string changedName, OnlineEditType onlineEdit,
            List<int> offsetList)
        {
            UnChangedName = unChangedName;
            ChangedName = changedName;
            OnlineEditType = onlineEdit;
            OffsetList = offsetList;
        }

        public OnlineEditType? OnlineEditType { get; }

        public List<int> OffsetList { get; }

        public string TagName { get; }

        public string ProgramName { get; }

        public string UnChangedName { get; }

        public string ChangedName { get; }

        public string Regex { get; }
    }
}
