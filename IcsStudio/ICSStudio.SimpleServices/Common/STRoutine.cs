using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Notification;
using ICSStudio.Interfaces.Tags;
using Newtonsoft.Json.Linq;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.SimpleServices.SourceProtection;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.Utils;

namespace ICSStudio.SimpleServices.Common
{
    //TODO(gjc): refactor BaseRoutine
    public class STRoutine : BaseComponent, IRoutine
    {
        private bool _isUpdateChanged;
        private bool _isAbandoned;

        private bool _isMainRoutine;
        private bool _isFaultRoutine;

        private bool _isEncrypted;

        public STRoutine(IController controller)
        {
            ParentController = controller;
        }

        public IRoutineCollection ParentCollection { get; set; }

        public bool IsMainRoutine
        {
            get { return _isMainRoutine; }
            set
            {
                if (_isMainRoutine != value)
                {
                    _isMainRoutine = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsFaultRoutine
        {
            get { return _isFaultRoutine; }
            set
            {
                if (_isFaultRoutine != value)
                {
                    _isFaultRoutine = value;
                    RaisePropertyChanged();
                }
            }
        }

        public RoutineType Type => RoutineType.ST;
        public bool IsAoiObject { get; set; }
        public bool ControllerEditsExist { get; set; }
        public bool PendingEditsExist => PendingCodeText != null;
        public bool HasEditAccess { get; set; }
        public bool HasEditContentAccess { get; set; }

        public bool IsEncrypted
        {
            get { return _isEncrypted; }
            set
            {
                _isEncrypted = value;
                RaisePropertyChanged();
            }
        }

        public bool IsModified { internal set; get; } = false;

        private ConcurrentQueue<IVariableInfo> VariableInfos { get; set; } = new ConcurrentQueue<IVariableInfo>();
        private ConcurrentQueue<IVariableInfo> PendingVariableInfos { get; set; } = new ConcurrentQueue<IVariableInfo>();
        private ConcurrentQueue<IVariableInfo> TestVariableInfos { get; set; } = new ConcurrentQueue<IVariableInfo>();

        public ConcurrentQueue<IVariableInfo> GetCurrentVariableInfos(OnlineEditType mode)
        { 
            if (mode == OnlineEditType.Original)
                return VariableInfos;
            if (mode == OnlineEditType.Pending)
                return PendingVariableInfos;
            if (mode == OnlineEditType.Test)
                return TestVariableInfos;
            return VariableInfos;
        }

        public ASTStmtMod GetCurrentMod(OnlineEditType mode)
        {
            if (mode == OnlineEditType.Original)
                return OriginalMod;
            if (mode == OnlineEditType.Pending)
                return PendingMod;
            if (mode == OnlineEditType.Test)
                return TestMod;
            return OriginalMod;
        }

        public void SetMode(ASTStmtMod astStmtMod)
        {
            if (CurrentOnlineEditType == OnlineEditType.Original)
                OriginalMod = astStmtMod;
            if (CurrentOnlineEditType == OnlineEditType.Pending)
                PendingMod=astStmtMod;
            if (CurrentOnlineEditType == OnlineEditType.Test)
                TestMod=astStmtMod;
        }

        public ASTStmtMod OriginalMod {private set; get; }
        public ASTStmtMod PendingMod { private set; get; }
        public ASTStmtMod TestMod { private set; get; }
        
        public OnlineEditType CurrentOnlineEditType
        {
            set
            {
                _currentOnlineEditType = value;
                RaisePropertyChanged();
            }
            get { return _currentOnlineEditType; }
        }
        
        public List<ITag> GetAllReferenceTags()
        {
            var tags = new List<ITag>();
            
            AddReferenceTags(tags, VariableInfos);
            AddReferenceTags(tags, PendingVariableInfos);
            AddReferenceTags(tags, TestVariableInfos);

            return tags;
        }


        private void AddReferenceTags(List<ITag> tags,ConcurrentQueue<IVariableInfo> variableInfos)
        {
            foreach (IVariableInfo variableInfo in variableInfos.ToList())
            {
                if (variableInfo.Tag != null && !tags.Contains(variableInfo.Tag))
                {
                    if (variableInfo.Tag.DataTypeInfo.DataType.IsMessageType)
                    {
                        var referenceTags =
                            RoutineExtend.GetMessageReferenceTags(
                                ((Tag)variableInfo.Tag).DataWrapper as MessageDataWrapper);
                        foreach (var referenceTag in referenceTags)
                        {
                            if (!tags.Contains(referenceTag))
                                tags.Add(referenceTag);
                        }
                    }
                    tags.Add(variableInfo.Tag);
                }
            }
        }

        public bool HasTagInOtherProgram(IProgram otherProgram)
        {
            foreach (var variableInfo in VariableInfos)
            {
                if (variableInfo.Tag?.ParentCollection?.ParentProgram == otherProgram)
                {
                    return true;
                }
            }
            foreach (var variableInfo in PendingVariableInfos)
            {
                if (variableInfo.Tag?.ParentCollection?.ParentProgram == otherProgram)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsAbandoned
        {
            get { return _isAbandoned; }
            set
            {
                _isAbandoned = value;
                RaisePropertyChanged();
            }
        }

        public override bool IsDeleted
        {
            get
            {
                if (ParentCollection == null)
                    return true;

                if (ParentCollection.ParentProgram == null)
                    return true;

                if (ParentCollection.ParentProgram.IsDeleted)
                    return true;

                return false;
            }
        }

        public override IController ParentController { get; }

        public uint TrackingGroups { get; set; }
        public bool IsSourceEditable { get; set; }
        public bool IsSourceViewable { get; set; }
        public bool IsSourceCopyable { get; set; }

        public bool IsUpdateChanged
        {
            set
            {
                _isUpdateChanged = value;
                RaisePropertyChanged();
                _isUpdateChanged = false;
            }
            get { return _isUpdateChanged; }
        }

        private List<string> _codeText = new List<string>();
        private OnlineEditType _currentOnlineEditType;

        public void StartPending()
        {
            PendingVariableInfos = new ConcurrentQueue<IVariableInfo>();
            foreach (var variableInfo in VariableInfos)
            {
                PendingVariableInfos.Enqueue((IVariableInfo)variableInfo.Clone());
            }

            PendingMod = OriginalMod;
        }

        public void ApplyTest()
        {
            var tmp = new string[TestCodeText.Count];
            TestCodeText.CopyTo(tmp);
            CodeText = tmp.ToList();
            TestCodeText = null;
            while (!TestVariableInfos.IsEmpty)
            {
                VariableInfos = TestVariableInfos;
                TestVariableInfos=new ConcurrentQueue<IVariableInfo>();
            }

            OriginalMod = TestMod;
            CurrentOnlineEditType = OnlineEditType.Original;
        }

        public void CancelTest()
        {
            TestCodeText = null;

            TestVariableInfos = new ConcurrentQueue<IVariableInfo>();

            CurrentOnlineEditType = OnlineEditType.Original;
        }

        public void CancelPending()
        {
            PendingCodeText = null;
            
            PendingVariableInfos=new ConcurrentQueue<IVariableInfo>();

            CurrentOnlineEditType = OnlineEditType.Original;
        }

        public void ApplyPending()
        {
            CodeText = PendingCodeText;
            PendingCodeText = null;
            IsModified = true;

            VariableInfos = PendingVariableInfos;
            PendingVariableInfos=new ConcurrentQueue<IVariableInfo>();
            OriginalMod = PendingMod;
            CurrentOnlineEditType = OnlineEditType.Original;
        }

        public List<string> CodeText
        {
            get { return _codeText; }
            set
            {
                if (_codeText != value)
                {
                    _codeText = value;
                    RaisePropertyChanged();
                }
            }
        }

        public List<string> TestCodeText { get; set; } = null;
        public List<string> PendingCodeText { get; set; } = null;

        public EncodedData EncodedData { get; set; }

        public IRoutineCode Routine { get; } = new RoutineCode();

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public void GenCode(IProgramModule program)
        {
            GenCode(program,CodeText);
        }

        public void GenCode(IProgramModule program,List<string> code)
        {
            var ast = STASTGenVisitor.Parse(code);

            TypeChecker p = new TypeChecker(ParentController as Controller, program as IProgram,
                program as AoiDefinition);
            ast = ast.Accept(p);

            var consts_pool = new ConstPool();
            CodeGenVisitor gen = new CodeGenVisitor(consts_pool);
            ast.Accept(gen);

            var prescan = new PrescanVisitor();
            CodeGenVisitor prescan_gen = new CodeGenVisitor(consts_pool);
            prescan.GenPrescanAST(ast).Accept(prescan_gen);
            var Routine = new Routine();
            Routine.Pool = new List<byte>();
            foreach (var tmp in consts_pool.consts_pool)
            {
                Routine.Pool.AddRange(tmp);
            }

            Routine.Prescan = new Function(prescan_gen.Codes, prescan_gen.SafePoints, prescan_gen.LocalsSize);
            Routine.Logic = new Function(gen.Codes, gen.SafePoints, gen.LocalsSize);

            var consts = MacroAssembler.ParseConstses(Routine.Pool);

            {
                var builder = new BasicBlockBuilder(Routine.Logic, consts);
                builder.BuildBlocks();
            }

            {
                var builder = new BasicBlockBuilder(Routine.Prescan, consts);
                builder.BuildBlocks();
            }

            (this.Routine as RoutineCode).UpdateCode(Routine);
        }

        public JObject ConvertToJObject(bool useCode)
        {
            var routine = new JObject
            {
                { "Name", Name },
                { "CodeText", JArray.FromObject(CodeText) },
                { "Type", (int)Type },
                { "Description", Description }
            };

            var code = (this.Routine as RoutineCode)?.Code;
            if (useCode && code != null && code.Logic != null && code.Prescan != null && code.Pool != null)
            {
                routine.Add("Logic", code.Logic.ToJson());
                routine.Add("Prescan", code.Prescan.ToJson());
                routine.Add("Pool", Function.EncodeByteArray(code.Pool));
            }

            if (PendingCodeText != null)
            {
                routine.Add("PendingCodeText", JArray.FromObject(PendingCodeText));
            }

            if (TestCodeText != null)
            {
                routine.Add("TestCodeText", JArray.FromObject(TestCodeText));
            }

            // encoded data
            if (IsEncrypted)
            {
                routine.Remove("CodeText");

                Controller controller = ParentController as Controller;
                SourceProtectionManager manager = controller?.SourceProtectionManager;

                EncodedData encodedData = manager?.CreateEncodedData(this);
                if (encodedData != null)
                {
                    EncodedData = encodedData;
                }

                if (EncodedData != null)
                {
                    routine.Add("EncodedData", JObject.FromObject(EncodedData));
                }
            }

            return routine;
        }

        public bool IsCompiling { get; set; }
        public List<IRoutine> GetJmpRoutines()
        {
            var list=new List<IRoutine>();
            var variableInfos = GetCurrentVariableInfos(OnlineEditType.Original);
            if (variableInfos != null)
            {
                var jsrInfos = variableInfos.Where(v => v.IsUseForJSR);
                foreach (var variableInfo in jsrInfos)
                {
                    var routine = ParentCollection[variableInfo.Name];
                    if (routine != null)
                    {
                        list.Add(routine);
                    }
                }
            }
            return list;
        }

        public bool IsError { get; set; }
        
        public static STRoutine Create(IController controller, JObjectExtensions jsonRoutine)
        {
            var st = new STRoutine(controller) { Name = (string)jsonRoutine["Name"] };
            if (jsonRoutine["Description"] != null)
                st.Description = (string)jsonRoutine["Description"];
            JArray codeText = (JArray)jsonRoutine["CodeText"];
            if (codeText != null)
                st.CodeText = codeText.ToObject<List<string>>();

            if (st.CodeText != null && st.CodeText.Count == 0)
                st.CodeText.Add(string.Empty);

            if (jsonRoutine["PendingCodeText"] != null)
            {
                JArray pendingCodeText = (JArray)jsonRoutine["PendingCodeText"];
                st.PendingCodeText = pendingCodeText.ToObject<List<string>>();

                if (st.PendingCodeText != null && st.PendingCodeText.Count == 0)
                    st.PendingCodeText.Add(string.Empty);
            }

            if (jsonRoutine["TestCodeText"] != null)
            {
                JArray testCodeText = (JArray)jsonRoutine["TestCodeText"];
                st.TestCodeText = testCodeText.ToObject<List<string>>();

                if (st.TestCodeText != null && st.TestCodeText.Count == 0)
                    st.TestCodeText.Add(string.Empty);
            }

            JObject logic = (JObject)jsonRoutine["Logic"];
            var Routine = new Routine();
            if (logic != null)
            {
                var jsonLogic = new JObjectExtensions(logic);

                var codes =
                    jsonLogic["Codes"].Type == JTokenType.Array
                        ? jsonLogic["Codes"].ToObject<List<byte>>()
                        : Function.DecodeToByteArray(jsonLogic["Codes"].ToString()).ToList();

                JArray safePoints = (JArray)jsonLogic["SafePoints"];

                Routine.Logic = new Function(codes, safePoints.ToObject<List<int>>(),
                    (int)jsonLogic["LocalsSize"]);
            }

            JObject prescan = (JObject)jsonRoutine["Prescan"];
            if (prescan != null)
            {
                var jsonPrescan = new JObjectExtensions(prescan);

                var codes =
                    jsonPrescan["Codes"].Type == JTokenType.Array
                        ? jsonPrescan["Codes"].ToObject<List<byte>>()
                        : Function.DecodeToByteArray(jsonPrescan["Codes"].ToString()).ToList();

                JArray safePoints = (JArray)jsonPrescan["SafePoints"];
                Routine.Prescan = new Function(codes, safePoints.ToObject<List<int>>(),
                    (int)jsonPrescan["LocalsSize"]);

            }

            JToken pool = jsonRoutine["Pool"];
            if (pool != null)
            {
                if (pool.Type == JTokenType.Array)
                {
                    Routine.Pool = pool.ToObject<List<byte>>();
                }
                else
                {
                    Routine.Pool = new List<byte>(Function.DecodeToByteArray(pool.ToString()));
                }

            }

            JObject encodedData = (JObject)jsonRoutine["EncodedData"];
            (st.Routine as RoutineCode).UpdateCode(Routine);
            if (encodedData != null)
            {
                st.EncodedData = encodedData.ToObject<EncodedData>();
                st.IsEncrypted = true;
            }

            return st;
        }

        protected override void DisposeAction()
        {
            IsCompiling = false;
            while (true)
            {
                IVariableInfo variable;
                var result = VariableInfos.TryDequeue(out variable);
                if(result)
                    variable.Dispose();
                else
                    break;
            }

            while (true)
            {
                IVariableInfo variable;
                var result = PendingVariableInfos.TryDequeue(out variable);
                if (result)
                    variable.Dispose();
                else
                    break;
            }
        }
    }

   
}
