using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ICSStudio.Interfaces.Common;
using Newtonsoft.Json.Linq;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.SourceProtection;
using ICSStudio.Utils;

namespace ICSStudio.SimpleServices.Common
{
    public class RLLRoutine : BaseComponent, IRoutine
    {
        private bool _isAbandoned;

        private bool _isMainRoutine;
        private bool _isFaultRoutine;

        private bool _isEncrypted;

        public RLLRoutine(IController controller)
        {
            ParentController = controller;

            Rungs = new List<RungType>();
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

        public RoutineType Type { get; set; } = RoutineType.RLL;

        private OnlineEditType _currentOnlineEditType;

        public OnlineEditType CurrentOnlineEditType
        {
            set
            {
                _currentOnlineEditType = value;
                RaisePropertyChanged();
            }
            get { return _currentOnlineEditType; }
        }

        public bool IsAoiObject { get; set; }
        public bool ControllerEditsExist { get; set; }
        public bool PendingEditsExist { get; set; }
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

        public List<RungType> Rungs { get; }

        public List<string> CodeText => Rungs.Where(p => p.Type == RungTypeEnum.Normal || p.Type == RungTypeEnum.EditOriginal).Select(rung => rung.Text).ToList();

        public List<string> RungsText => Rungs.Select(rung => rung.Text).ToList();

        public List<RungType> CloneRungs()
        {
            return Rungs.Select(p => p.Clone()).ToList();
        }

        // for Decode
        internal List<KeyValuePair<int, string>> RungComments { get; private set; }

        public EncodedData EncodedData { get; set; }

        public IRoutineCode Routine { get; } = new RoutineCode();
        public bool IsError { get; set; }

        [SuppressMessage("ReSharper", "IdentifierTypo")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public void GenCode(IProgramModule program)
        {
            ASTNode ast = RLLGrammarParser.Parse(CodeText, ParentController as Controller);

            TypeChecker p = new TypeChecker(ParentController as Controller, program as IProgram,
                program as AoiDefinition);
            ast = ast.Accept(p);

            var consts_pool = new ConstPool();

            CodeGenVisitor gen = new CodeGenVisitor(consts_pool);
            ast = ast.Accept(gen);

            var prescan = new PrescanVisitor();
            CodeGenVisitor prescan_gen = new CodeGenVisitor(consts_pool);

            prescan.GenPrescanAST(ast).Accept(prescan_gen);

            var Routine = new Routine();
            Routine.Pool = new List<byte>();
            foreach (var tmp in consts_pool.consts_pool)
            {
                Routine.Pool.AddRange(tmp);
            }

            Routine.Logic = new Function(gen.Codes, gen.SafePoints, gen.LocalsSize);
            Routine.Prescan = new Function(prescan_gen.Codes, prescan_gen.SafePoints, prescan_gen.LocalsSize);

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

            //Rungs
            JArray rungArray = new JArray();
            for (int i = 0; i < Rungs.Count; i++)
            {
                JObject rungObject = new JObject();
                rungObject.Add("Number", i);
                rungObject.Add("Type", RungTypeConvert(Rungs[i].Type));
                rungObject.Add("Text", Rungs[i].Text);
                rungObject.Add("Comment", Rungs[i].Comment);

                rungArray.Add(rungObject);
            }

            routine.Add("Rungs", rungArray);
            //

            var code = (Routine as RoutineCode)?.Code;
            if (useCode && code != null && code.Logic != null && code.Prescan != null && code.Pool != null)
            {
                routine.Add("Logic", code.Logic.ToJson());
                routine.Add("Prescan", code.Prescan.ToJson());
                routine.Add("Pool", Function.EncodeByteArray(code.Pool));
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

        private string RungTypeConvert(RungTypeEnum type)
        {
            switch (type)
            {
                case RungTypeEnum.Normal:
                    return "N";
                case RungTypeEnum.Edit:
                    return "e";
                case RungTypeEnum.Insert:
                    return "i";
                case RungTypeEnum.Delete:
                    return "d";
                case RungTypeEnum.EditOriginal:
                    return "r";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        internal static RungTypeEnum ParserRungType(string value)
        {
            switch (value)
            {
                case "N":
                    return RungTypeEnum.Normal;
                case "e":
                    return RungTypeEnum.Edit;
                case "i":
                    return RungTypeEnum.Insert;
                case "d":
                    return RungTypeEnum.Delete;
                case "r":
                    return RungTypeEnum.EditOriginal;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);

            }
        }

        public bool IsCompiling { get; set; }

        public List<IRoutine> GetJmpRoutines()
        {
            var list = new List<IRoutine>();
            ASTRLLRoutine astRllRoutine =
                RLLGrammarParser.Parse(CodeText, ParentController as Controller);
            foreach (var node in astRllRoutine.list.nodes)
            {
                var rung = node as ASTRLLSequence;
                if (rung != null)
                {
                    GetJsrRoutinesFromBranchLevel(list, ParentCollection, rung);
                }
            }

            return list;
        }

        private void GetJsrRoutinesFromBranchLevel(List<IRoutine> list, IRoutineCollection routines,
            ASTRLLSequence level)
        {
            foreach (var astNode in level.list.nodes)
            {
                var instruction = astNode as ASTRLLInstruction;
                if (instruction != null)
                {

                    if (instruction.instr?.Name == "JSR")
                    {
                        var nameofRoutine2Execute = instruction.param_list[0];
                        var jumpRoutine = routines[nameofRoutine2Execute];
                        if (!list.Contains(jumpRoutine))
                        {
                            list.Add(jumpRoutine);
                        }
                    }

                    continue;
                }

                var branch = astNode as ASTRLLParallel;
                if (branch != null)
                {
                    foreach (var branchNode in branch.list.nodes)
                    {
                        var branchLevel = branchNode as ASTRLLSequence;
                        if (branchLevel != null)
                        {
                            GetJsrRoutinesFromBranchLevel(list, routines, branchLevel);
                        }
                    }
                }
            }
        }

        public void UpdateRungs(List<RungType> newRungs)
        {
            if (newRungs?.Count > 0)
            {
                Rungs.Clear();
                Rungs.AddRange(newRungs);

                RaisePropertyChanged(nameof(CodeText));
            }
        }

        public void UpdateRungs(List<string> codeText)
        {
            Rungs.Clear();

            if (codeText != null)
            {
                foreach (var text in codeText)
                {
                    Rungs.Add(new RungType { Text = text });
                }
            }

            RaisePropertyChanged(nameof(CodeText));
        }

        public void UpdateRungs(List<string> codeText, List<KeyValuePair<int, string>> rungComments)
        {
            Rungs.Clear();

            if (codeText != null)
            {
                foreach (var text in codeText)
                {
                    Rungs.Add(new RungType { Text = text });
                }
            }


            if (rungComments != null)
            {
                foreach (KeyValuePair<int, string> comment in rungComments)
                {
                    if (comment.Key < Rungs.Count)
                    {
                        var rung = Rungs[comment.Key];
                        rung.Comment = comment.Value;
                    }
                }
            }

            RaisePropertyChanged(nameof(CodeText));
        }

        public static RLLRoutine Create(IController controller, JObjectExtensions jsonRoutine)
        {
            var rll = new RLLRoutine(controller) { Name = (string)jsonRoutine["Name"] };
            if (jsonRoutine["Description"] != null)
                rll.Description = (string)jsonRoutine["Description"];

            JArray codeText = (JArray)jsonRoutine["CodeText"];
            JArray rungComments = (JArray)jsonRoutine["RungComments"];
            JArray rungs = (JArray)jsonRoutine["Rungs"];

            rll.RungComments = rungComments?.ToObject<List<KeyValuePair<int, string>>>();

            if (rungs != null)
            {
                List<RungType> rungList = new List<RungType>();
                foreach (var rung in rungs.OfType<JObject>())
                {
                    RungType rungType = new RungType();

                    if (rung.ContainsKey("Type"))
                    {
                        rungType.Type = ParserRungType(rung["Type"]?.ToString());
                    }
                    else
                    {
                        rungType.Type = RungTypeEnum.Normal;
                    }

                    rungType.Text = rung.ContainsKey("Text") ? rung["Text"]?.ToString() : string.Empty;

                    if (rung.ContainsKey("Comment"))
                    {
                        rungType.Comment = rung["Comment"]?.ToString();
                    }

                    rungList.Add(rungType);
                }

                rll.UpdateRungs(rungList);
            }
            else
            {
                rll.UpdateRungs(codeText?.ToObject<List<string>>(),
                    rll.RungComments);
            }


            var Routine = new Routine();
            JObject logic = (JObject)jsonRoutine["Logic"];
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

            JToken pool = jsonRoutine["Pool"]; //(JArray)System.Convert.FromBase64String(.ToString());
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

            rll.Type = RoutineType.RLL;
            (rll.Routine as RoutineCode).UpdateCode(Routine);

            JObject encodedData = (JObject)jsonRoutine["EncodedData"];
            if (encodedData != null)
            {
                rll.EncodedData = encodedData.ToObject<EncodedData>();
                rll.IsEncrypted = true;
            }

            return rll;
        }

    }

    public class RungType
    {
        public RungTypeEnum Type { get; set; }
        public string Text { get; set; }
        public string Comment { get; set; }

        public EditMark Mark { get; set; }

        public RungType Clone()
        {
            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(this.GetType());
            string content = string.Empty;
            using (var writer = new System.IO.StringWriter())
            {
                serializer.Serialize(writer, this);
                content = writer.ToString();
            }

            using (System.IO.StringReader reader = new System.IO.StringReader(content))
                return serializer.Deserialize(reader) as RungType;
        }
    }

    public enum EditMark
    {
        Normal,
        Delete
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum RungTypeEnum
    {
        Normal,
        Edit, //i
        EditOriginal, //r
        //AcceptEdit, //I
        //DeleteAcceptEdit, //dI
        //AcceptEditOriginal, //R
        Delete, //d
        //AcceptDelete, //D
        Insert, //i
        //AcceptInsert, //I
        //AcceptInsertOriginal, //r
        //DeleteAcceptInsert, //dI
        //EditAcceptEdit, //i
        //EditAcceptEditOriginal //r

        //N,
        //I,
        //D,   
        //IR,
        //rR,
        //R,
        //rI,
        //rN,
        //e,
        //er,
        //dN,
        //dD,
        //dI,
        //dIR,
    }

}
