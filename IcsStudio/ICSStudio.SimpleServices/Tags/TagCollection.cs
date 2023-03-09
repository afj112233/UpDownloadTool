using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.SimpleServices.Tags
{
    public class TagCollection : BaseCommon,ITagCollection
    {
        private readonly List<ITag> _tags;
        private readonly Controller _parentController;
        private readonly ProgramModule _parentProgram;
        private bool _isSaveTagNameChange = false;

        public TagCollection(IController controller)
        {
            _tags = new List<ITag>();

            _parentController = controller as Controller;
        }

        public TagCollection(IProgramModule program)
        {
            _tags = new List<ITag>();
            
            _parentProgram = program as ProgramModule;
            PropertyChangedEventManager.AddHandler(program, ParentProgram_PropertyChanged, "Name");
        }

        private void ParentProgram_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //TODO(gjc): add here???
            if (Controller.GetInstance().IsLoading) return;
            var program = sender as Program;
            if (program == null) return;
            var regex = new Regex($@"\\{program.OldName}(?=(\W|$))");
            if (e.PropertyName == "Name")
            {
                UpdateProgramNameTrend(program.OldName, program.Name);
                foreach (var parentControllerProgram in ParentController.Programs)
                {
                    foreach (var routine in parentControllerProgram.Routines)
                    {
                        var st = routine as STRoutine;
                        bool isChanged = false;
                        if (st != null)
                        {
                            {
                                var codeList = st.CodeText;
                                if (UpdateProgramNameInCode(ref codeList, regex, program.Name))
                                {
                                    st.CodeText = codeList;
                                    isChanged = true;
                                }
                            }

                            if (st.PendingCodeText != null)
                            {
                                var codeList = st.PendingCodeText;
                                if (UpdateProgramNameInCode(ref codeList, regex, program.Name))
                                {
                                    st.PendingCodeText = codeList;
                                    isChanged = true;
                                }
                            }

                            if (st.TestCodeText != null)
                            {
                                var codeList = st.TestCodeText;
                                if (UpdateProgramNameInCode(ref codeList, regex, program.Name))
                                {
                                    st.TestCodeText = codeList;
                                    isChanged = true;
                                }
                            }

                            if (isChanged)
                                st.IsUpdateChanged = true;
                        }

                        //TODO(zyl):add other
                    }
                }

            }
        }

        public IEnumerator<ITag> GetEnumerator() => _tags.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        protected override void DisposeAction()
        {
            if (_parentProgram != null)
            {
                PropertyChangedEventManager.RemoveHandler(_parentProgram, ParentProgram_PropertyChanged, "Name");
            }

            var removeTags = _tags.ToList();

            _tags.Clear();

            foreach (var tag in removeTags)
            {
                PropertyChangedEventManager.RemoveHandler(tag, TagChanged, "");
                tag.Dispose();
            }
        }

        public override IController ParentController => _parentController ?? _parentProgram?.ParentController;
        
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Count => _tags.Count;

        public ITag this[int uid]
        {
            get { throw new NotImplementedException(); }
        }

        public ITag this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                    return null;

                foreach (var tag in _tags)
                {
                    if (tag.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                        return tag;
                }

                return null;
            }
        }

        public ITag TryGetChildByUid(int uid)
        {
            throw new NotImplementedException();
        }

        public ITag TryGetChildByName(string name)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<ComponentCoreInfo> GetComponentCoreInfoList()
        {
            throw new NotImplementedException();
        }

        public ComponentCoreInfo GetComponentCoreInfo(int uid)
        {
            throw new NotImplementedException();
        }

        public bool IsControllerScoped => _parentProgram == null;

        public IProgramModule ParentProgram => _parentProgram;
#pragma warning disable 67
        public event EventHandler<TagModifiedEventArgs> OnTagModified;
#pragma warning restore
        public ITag GetDeviceTag(string name, bool deep)
        {
            throw new NotImplementedException();
        }

        public ITag TryGetDeviceTag(string name, bool deep)
        {
            throw new NotImplementedException();
        }

        public TagInfo GetTagInfo(string tagName, bool bAllowPrivateMemberReferences = false)
        {
            throw new NotImplementedException();
        }

        public void DeleteTag(ITag tag, bool allowDeleteIfReferenced, bool allowDeleteIfReadOnly, bool resetAoi)
        {
            if (tag == null) return;
            if (_tags.Contains(tag))
            {
                if (tag.IsAlias && !allowDeleteIfReferenced)
                    return;

                if (tag.IsReadOnly && !allowDeleteIfReadOnly)
                    return;

                if (!tag.IsAlias && tag.DataTypeInfo.DataType.IsMotionGroupType)
                {
                    RemoveMotionGroup(tag);
                }
                else if (!tag.IsAlias && tag.DataTypeInfo.DataType.IsAxisType)
                {
                    RemoveAxis(tag);
                }
                else
                    _tags.Remove(tag);

                if (resetAoi)
                {
                    var aoi = ParentProgram as AoiDefinition;
                    aoi?.datatype.Reset();
                }

                //if (needVerify)
                //    Notifications.Publish(new MessageData() {Object = tag, Type = MessageData.MessageType.DelTag});
                //if (!_isDisposed)
                {
                    PropertyChangedEventManager.RemoveHandler(tag, TagChanged, "");
                    CollectionChanged?.Invoke(this,
                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, tag));
                }
            }
        }

        public IEnumerable<ITag> TrackedTags { get; set; }

        public void AddTag(ITag tag, bool resetAoi, bool isTmpAoi)
        {
            if (tag == null) return;

            if (isTmpAoi || tag.ParentCollection == this)
            {
                if (!_tags.Any(t => t.Name.Equals(tag.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    _tags.Add(tag);

                    //TODO(gjc): add here???
                    if (resetAoi)
                    {
                        var aoi = ParentProgram as AoiDefinition;
                        aoi?.datatype?.Reset();
                    }

                    //TODO(gjc): add here???
                    //if (!_isDisposed)
                    {
                        PropertyChangedEventManager.AddHandler(tag, TagChanged, "");
                        CollectionChanged?.Invoke(this,
                            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, tag));
                    }
                }

            }
        }

        private readonly object _syncRoot = new object();

        public void AddTags(List<ITag> tags, ITagCollection collection, bool resetAoi, bool isTmpAoi)
        {
            if (!tags.Any()) return;
            if (isTmpAoi || collection == this)
            {
                //object syncRoot=new object();
                //foreach (var tag in tags)
                Parallel.ForEach(tags.ToList(), tag =>
                {
                    if (tag == null)
                    {
                        Debug.Assert(false);
                        return;
                    }

                    bool sameName = false;

                    lock (_syncRoot)
                    {
                        foreach (var item in _tags)
                        {
                            if (item.Name.Equals(tag.Name, StringComparison.OrdinalIgnoreCase))
                            {
                                sameName = true;
                            }
                        }
                    }

                    if (!sameName)
                    {
                        lock (_syncRoot)
                        {
                            _tags.Add(tag);
                        }

                        //TODO(gjc): add here???
                        PropertyChangedEventManager.AddHandler(tag, TagChanged, "");
                    }
                    else
                    {
                        lock (_syncRoot)
                        {
                            tags.Remove(tag);
                        }
                    }
                });

                if (resetAoi)
                {
                    var aoi = ParentProgram as AoiDefinition;
                    aoi?.datatype?.Reset();
                }

                //if (needNotification&&!Controller.GetInstance().IsLoading)
                //{
                //    Notifications.Publish(new MessageData() { Type = MessageData.MessageType.Verify });
                //}
                //if (!_isDisposed)
                CollectionChanged?.Invoke(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, tags));
            }
        }

        public void Clear()
        {
            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, _tags));

            var removeTags = _tags.ToList();

            _tags.Clear();

            foreach (var tag in removeTags)
            {
                PropertyChangedEventManager.RemoveHandler(tag, TagChanged, "");
                tag.Dispose();
            }
        }

        public void Order(List<ITag> correctOrder)
        {
            Debug.Assert(_tags.Count == correctOrder.Count);
            _tags.Clear();
            _tags.AddRange(correctOrder);
        }

        public bool IsNeedVerifyRoutine { set; get; } = true;

        private void TagChanged(object sender, PropertyChangedEventArgs e)
        {
            var aoi = ParentProgram as AoiDefinition;
            var tag = (ITag)sender;
            var pendingCompileRoutine = PendingCompileRoutine.GetInstance();
            if (e.PropertyName == "Usage" || e.PropertyName == "DataWrapper" || e.PropertyName == "IsConstant")
            {
                if (aoi != null)
                {
                    foreach (var routine in aoi.Routines)
                    {
                        var st = routine as STRoutine;
                        if (st != null)
                        {
                            if (st.GetAllReferenceTags().All(t => t != tag)) continue;
                            pendingCompileRoutine.Add(st, false);
                        }
                    }
                }
                else
                {
                    foreach (var program in ParentController.Programs)
                    {
                        foreach (var routine in program.Routines)
                        {
                            var st = routine as STRoutine;
                            if (st != null)
                            {
                                if (st.GetAllReferenceTags().All(t => t != tag)) continue;
                                pendingCompileRoutine.Add(st, false);
                            }
                        }
                    }
                }

                if (IsNeedVerifyRoutine)
                    pendingCompileRoutine.CompilePendingRoutines();
                return;
            }

            if (e.PropertyName != "Name") return;

            if (string.IsNullOrEmpty(tag.OldName)) return;

            var codeSynchronization = CodeSynchronization.GetInstance();

            if (aoi != null)
            {
                aoi.datatype.MemberChangedList.Add(new Tuple<string, string>(tag.OldName, tag.Name));
                foreach (var routine in aoi.Routines)
                {
                    var st = routine as STRoutine;
                    if (st != null)
                    {
                        CodeSynchronization.WaitCompiler(st);
                        string programName = _parentController == null ? _parentProgram.Name : "";
                        bool isChanged = false;
                        UpdateCode(st, tag, programName, ref isChanged);
                        //if (isChanged)
                        //{
                        //    st.IsError = true;
                        //    st.IsUpdateChanged = true;
                        //    //if (IsNeedVerifyRoutine)
                        //    //    Notifications.Publish(new MessageData()
                        //    //        {Object = routine, Type = MessageData.MessageType.Verify});
                        //    pendingCompileRoutine.Add(st,false);
                        //}
                    }

                    //TODO(ZYL):add other
                }
            }
            else
            {
                UpdateTagNameTrend(tag.OldName, tag.Name);
                UpdateMessageReference(tag.OldName, tag.Name);
                foreach (var program in ParentController.Programs)
                {
                    if (ParentProgram == null &&
                        (program.Tags.FirstOrDefault(t =>
                            t.Name.Equals(tag.OldName, StringComparison.OrdinalIgnoreCase)) != null)) continue;
                    foreach (var routine in program.Routines)
                    {
                        var st = routine as STRoutine;
                        if (st != null)
                        {
                            CodeSynchronization.WaitCompiler(st);
                            string programName = _parentController == null ? _parentProgram.Name : "";
                            bool isChanged = false;
                            UpdateCode(st, tag, programName, ref isChanged);
                            //if (isChanged)
                            //{
                            //    st.IsError = true;
                            //    st.IsUpdateChanged = true;
                            //    //if (IsNeedVerifyRoutine)
                            //    //    Notifications.Publish(new MessageData()
                            //    //        {Object = routine, Type = MessageData.MessageType.Verify});
                            //    pendingCompileRoutine.Add(st, false);
                            //}
                        }

                        //TODO(ZYL):add other
                    }
                }
            }

            if (IsNeedVerifyRoutine)
                codeSynchronization.Update();
        }

        public bool IsSaveTagNameChange
        {
            set
            {
                _isSaveTagNameChange = value;
                if (_isSaveTagNameChange)
                    TagNameChanged.Clear();
            }
            get { return _isSaveTagNameChange; }
        }

        public Dictionary<STRoutine, Dictionary<OnlineEditType, List<Tuple<string, string, int>>>>
            TagNameChanged { get; } =
            new Dictionary<STRoutine, Dictionary<OnlineEditType, List<Tuple<string, string, int>>>>();

        public void ApplyTagNameChanged()
        {
            if (!IsSaveTagNameChange) return;
            //var changedRoutines=new List<STRoutine>();
            foreach (var key in TagNameChanged.Keys)
            {
                var changes = TagNameChanged[key];
                foreach (var changesKey in changes.Keys)
                {
                    var list = changes[changesKey];
                    var code = "";
                    if (changesKey == OnlineEditType.Original)
                    {
                        code = string.Join("\n", key.CodeText);
                    }
                    else if (changesKey == OnlineEditType.Pending)
                    {
                        code = string.Join("\n", key.PendingCodeText);
                    }
                    else if (changesKey == OnlineEditType.Test)
                    {
                        code = string.Join("\n", key.TestCodeText);
                    }

                    foreach (var tuple in list.OrderByDescending(l => l.Item3))
                    {
                        code = code.Remove(tuple.Item3, tuple.Item1.Length);
                        code = code.Insert(tuple.Item3, tuple.Item2);
                    }

                    if (changesKey == OnlineEditType.Original)
                    {
                        key.CodeText = code.Split('\n').ToList();
                    }
                    else if (changesKey == OnlineEditType.Pending)
                    {
                        key.PendingCodeText = code.Split('\n').ToList();
                    }
                    else if (changesKey == OnlineEditType.Test)
                    {
                        key.TestCodeText = code.Split('\n').ToList();
                    }
                }

                key.IsUpdateChanged = true;
            }

            TagNameChanged.Clear();
            IsSaveTagNameChange = false;
        }

        private void SaveTagNameChanged(STRoutine routine, OnlineEditType onlineEditType, string oldName,
            string newName, List<int> offset)
        {
            Dictionary<OnlineEditType, List<Tuple<string, string, int>>> dic = null;
            if (TagNameChanged.ContainsKey(routine))
            {
                dic = TagNameChanged[routine];
            }
            else
            {
                dic = new Dictionary<OnlineEditType, List<Tuple<string, string, int>>>();
                TagNameChanged[routine] = dic;
            }

            List<Tuple<string, string, int>> changes = null;
            if (dic.ContainsKey(onlineEditType))
            {
                changes = dic[onlineEditType];
            }
            else
            {
                changes = new List<Tuple<string, string, int>>();
                dic[onlineEditType] = changes;
            }

            foreach (var i in offset)
            {
                changes.Add(new Tuple<string, string, int>(oldName, newName, i));
            }
        }

        private void UpdateCode(STRoutine st, ITag tag, string programName, ref bool isChanged)
        {
            var codeSynchronization = CodeSynchronization.GetInstance();
            var offsetList = new List<int>();
            {
                var variableInfos = st.GetCurrentVariableInfos(OnlineEditType.Original).ToList()
                    .Where(v => v.Tag == tag);
                foreach (var v in variableInfos)
                {
                    var offset = v.GetOffsetsWithoutProgram();
                    if (!offsetList.Contains(offset))
                        offsetList.Add(offset);
                }

                if (IsSaveTagNameChange)
                {
                    SaveTagNameChanged(st, OnlineEditType.Original, tag.OldName, tag.Name, offsetList);
                }
                else
                {
                    //st.CodeText = UpdateTagNameInCode(st.CodeText, tag.OldName, tag.Name, programName,
                    //    ref isChanged,
                    //    offsetList);
                    codeSynchronization.Add(st,
                        new UpdateCodeParameter(tag.OldName, tag.Name, OnlineEditType.Original, offsetList));
                }
            }
            if (st.PendingEditsExist)
            {
                var variableInfos = st.GetCurrentVariableInfos(OnlineEditType.Pending).ToList()
                    .Where(v => v.Tag == tag);
                offsetList.Clear();
                foreach (var v in variableInfos)
                {
                    var offset = v.GetOffsetsWithoutProgram();
                    if (!offsetList.Contains(offset))
                        offsetList.Add(offset);
                }

                if (IsSaveTagNameChange)
                {
                    SaveTagNameChanged(st, OnlineEditType.Pending, tag.OldName, tag.Name, offsetList);
                }
                else
                {
                    //st.PendingCodeText = UpdateTagNameInCode(st.PendingCodeText, tag.OldName, tag.Name,
                    //    programName,
                    //    ref isChanged, offsetList);
                    codeSynchronization.Add(st,
                        new UpdateCodeParameter(tag.OldName, tag.Name, OnlineEditType.Pending, offsetList));
                }
            }

            if (st.TestCodeText != null)
            {
                var variableInfos = st.GetCurrentVariableInfos(OnlineEditType.Test).ToList().Where(v => v.Tag == tag);
                offsetList.Clear();
                foreach (var v in variableInfos)
                {
                    var offset = v.GetOffsetsWithoutProgram();
                    if (!offsetList.Contains(offset))
                        offsetList.Add(offset);
                }

                if (IsSaveTagNameChange)
                {
                    SaveTagNameChanged(st, OnlineEditType.Test, tag.OldName, tag.Name, offsetList);
                }
                else
                {
                    //st.TestCodeText = UpdateTagNameInCode(st.TestCodeText, tag.OldName, tag.Name, programName,
                    //    ref isChanged, offsetList);
                    codeSynchronization.Add(st,
                        new UpdateCodeParameter(tag.OldName, tag.Name, OnlineEditType.Test, offsetList));
                }
            }
        }

        private void UpdateTagNameTrend(string oldName, string newName)
        {
            var program = ParentProgram as Program;
            if (program != null)
            {
                oldName = $@"\{program.Name}.{oldName}";
                newName = $@"\{program.Name}.{newName}";
            }

            foreach (TrendObject trend in Controller.GetInstance().Trends)
            {
                if (trend.AxisPenName?.StartsWith(oldName, StringComparison.OrdinalIgnoreCase) ?? false)
                {
                    trend.AxisPenName = trend.AxisPenName.Remove(0, oldName.Length).Insert(0, newName);
                }

                foreach (PenObject pen in trend.Pens)
                {
                    if (pen.Name.StartsWith(oldName, StringComparison.OrdinalIgnoreCase))
                        pen.Name = pen.Name.Remove(0, oldName.Length).Insert(0, newName);
                }
            }
        }

        private void UpdateProgramNameTrend(string oldName, string newName)
        {
            var program = ParentProgram as Program;
            if (program != null)
            {
                oldName = $@"\{program.Name}";
                newName = $@"\{program.Name}";
            }

            foreach (TrendObject trend in Controller.GetInstance().Trends)
            {
                if (IsInTag(oldName, trend.AxisPenName))
                {
                    trend.AxisPenName = trend.AxisPenName.Remove(0, oldName.Length).Insert(0, newName);
                }

                foreach (PenObject pen in trend.Pens)
                {
                    if (IsInTag(oldName, pen.Name))
                        pen.Name = pen.Name.Remove(0, oldName.Length).Insert(0, newName);
                }
            }
        }

        private bool IsInTag(string oldName, string specifier)
        {
            if (string.IsNullOrEmpty(specifier)) return false;

            if (specifier.StartsWith(oldName))
            {
                if (specifier.Equals(oldName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                var nextChar = specifier[oldName.Length];
                if (nextChar == '.' || nextChar == '[') return true;
            }

            return false;
        }

        private void UpdateMessageReference(string oldName, string newName)
        {
            if (ParentProgram != null) return;
            foreach (var tag in _tags.Where(t => ((Tag)t).DataWrapper is MessageDataWrapper))
            {
                var message = (MessageDataWrapper)((Tag)tag).DataWrapper;
                var source = message.SourceElement;
                if (IsInTag(oldName, source))
                    message.SourceElement = source.Remove(0, oldName.Length).Insert(0, newName);
                var destination = message.DestinationElement;
                if (IsInTag(oldName, destination))
                    message.DestinationElement = destination.Remove(0, oldName.Length).Insert(0, newName);
            }
        }

        private bool UpdateProgramNameInCode(
            ref List<string> codeList, Regex regex, string programName)
        {
            if (codeList == null) return false;
            var code = string.Join("\n", codeList);
            var cleanCode = RoutineCodeTextExtension.ConvertCommentToWhiteBlank(code, null);
            if (!string.IsNullOrEmpty(cleanCode))
            {
                var matches = regex.Matches(cleanCode);
                if (matches.Count > 0)
                {
                    for (int i = matches.Count - 1; i >= 0; i--)
                    {
                        code = code.Remove(matches[i].Index, matches[i].Value.Length);
                        code = code.Insert(matches[i].Index, $"\\{programName}");
                    }

                    codeList = code.Split('\n').ToList();
                    return true;
                }
            }

            return false;
        }

        private void RemoveMotionGroup(ITag motionGroup)
        {
            foreach (Tag tag in _tags.OfType<Tag>())
            {
                var axisVirtual = tag.DataWrapper as AxisVirtual;
                if (axisVirtual != null && axisVirtual.AssignedGroup == motionGroup)
                {
                    axisVirtual.AssignedGroup = null;
                }

                var axisCIPDrive = tag.DataWrapper as AxisCIPDrive;
                if (axisCIPDrive != null && axisCIPDrive.AssignedGroup == motionGroup)
                {
                    axisCIPDrive.AssignedGroup = null;
                }

                //TODO(gjc): add other here
            }

            _tags.Remove(motionGroup);
        }

        private void RemoveAxis(ITag axis)
        {
            Tag tag = axis as Tag;
            if (tag != null)
            {
                var axisCIPDrive = tag.DataWrapper as AxisCIPDrive;
                var cipMotionDrive = axisCIPDrive?.AssociatedModule as CIPMotionDrive;
                if (cipMotionDrive != null)
                {
                    cipMotionDrive.RemoveAxis(axis, axisCIPDrive.AxisNumber);
                    axisCIPDrive.UpdateAxisChannel(null, 0);
                }
            }

            _tags.Remove(axis);
        }
    }
}
