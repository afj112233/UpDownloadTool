using System.Collections.Generic;
using ICSStudio.Interfaces.Common;
using Newtonsoft.Json.Linq;
using ICSStudio.SimpleServices.Compiler;

namespace ICSStudio.SimpleServices.Common
{

    public class FBDRoutine : BaseComponent, IRoutine
    {
        private bool _isAbandoned;

        private bool _isMainRoutine;
        private bool _isFaultRoutine;

        private bool _isEncrypted;

        public FBDRoutine(IController controller, JObject config)
        {
            this.config = config;
            ParentController = controller;
        }

        public override IController ParentController { get; }
        public uint TrackingGroups { get; }
        public bool IsSourceEditable { get; }
        public bool IsSourceViewable { get; }
        public bool IsSourceCopyable { get; }
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

        public RoutineType Type { get; } = RoutineType.FBD;
        public bool IsAoiObject { get; }
        public bool ControllerEditsExist { get; }
        public bool PendingEditsExist { get; }
        public bool HasEditAccess { get; }
        public bool HasEditContentAccess { get; }

        public bool IsEncrypted
        {
            get { return _isEncrypted; }
            set
            {
                _isEncrypted = value;
                RaisePropertyChanged();
            }
        }

        public IRoutineCode Routine { get; } = new RoutineCode();
        public bool IsError { get; set; }

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


        public JObject config { get; }

        //public Function Logic { get; set; }
        //public Function Prescan { get; set; }
        //public List<byte> Pool { get; set; }

        public void GenCode(IProgramModule program)
        {
            var ast = FBDGrammarParser.Parse(config);
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
            Routine.Prescan = new Function(prescan_gen.Codes, gen.SafePoints, prescan_gen.LocalsSize);

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
            var Routine = (this.Routine as RoutineCode).Code;
            if (Routine.Logic != null && Routine.Prescan != null && Routine.Pool != null && useCode)
            {
                var logic = new JObject
                {
                    {"LocalsSize", Routine.Logic.LocalsSize},
                    {"Codes", Function.EncodeByteArray(Routine.Logic.Codes)},
                    {"SafePoints", JArray.FromObject(Routine.Logic.SafePoints)}

                };

                var prescan = new JObject
                {
                    {"LocalsSize", Routine.Prescan.LocalsSize},
                    {"Codes", Function.EncodeByteArray(Routine.Prescan.Codes)},
                    {"SafePoints", JArray.FromObject(Routine.Prescan.SafePoints)}

                };

                var routine = new JObject
                {
                    {"Name", Name},
                    {"Sheets", JArray.FromObject(this.config["Sheets"])},
                    {"Logic", logic},
                    {"Prescan", prescan},
                    {"Pool", Function.EncodeByteArray(Routine.Pool)},
                    {"Type", (int) Type},
                };

                return routine;
            }
            else
            {
                var routine = new JObject
                {
                    {"Name", Name},
                    {"Sheets", JArray.FromObject(this.config["Sheets"])},
                    {"Type", (int) Type},
                };

                return routine;
            }
        }

        public bool IsCompiling { get; set; }
        public List<IRoutine> GetJmpRoutines()
        {
            return new List<IRoutine>();
        }
    }
}
