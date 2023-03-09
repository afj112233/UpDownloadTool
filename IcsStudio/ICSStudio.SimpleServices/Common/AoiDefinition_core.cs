using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Instruction;
using ICSStudio.SimpleServices.PredefinedType;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.Common
{
    using ParamInfoList = List<Tuple<Predicate<ASTExpr>, IDataType, ParameterType>>;
    using ParamMemberList = List<Tuple<DataTypeMember, ParameterType>>;
    public sealed partial class AoiDefinition
    {
        private sealed class AoiDefinitionCore: IDisposable
        {
            private bool _isTmp;
            public readonly AOIDataType datatype;

            public List<byte> Pool { get; private set; } = new List<byte>();
            public Dictionary<string, Function> _funcs { get; private set; } = new Dictionary<string, Function>();

            //所有的INOUT参数，这个也是整个函数的参数，底下实际调用时需要放到栈上的参数
            public List<Tuple<string, IDataType, int>> _parameters { get; } = new List<Tuple<string, IDataType, int>>();
            private List<ArgInfo> ArgInfos { get; } = new List<ArgInfo>();

            //所有标注了Required的参数，这个主要是用于ST,RLL的调用
            public List<Tuple<string, IDataType, ParameterType>> _arguments { get; } =
                new List<Tuple<string, IDataType, ParameterType>>();

            private int args_size_ = 0;

            public AoiDefinition Host { private get; set; }

            public bool IsParameter(string name)
            {
                return _parameters.FindIndex(p => p.Item1.Equals(name, StringComparison.OrdinalIgnoreCase)) != -1;
            }

            public string Name => _aoiDefinition.Name;

            public string Description => _aoiDefinition.Description;

            public int FindParameterIndex(string name)
            {
                return _parameters.FindIndex(p => p.Item1.Equals(name, StringComparison.OrdinalIgnoreCase));
            }


            public IDataType GetParameterType(int index)
            {
                return _parameters[index].Item2;
            }

            public int GetParameterPos(int index)
            {
                return _parameters[index].Item3;
            }

            public string GetParameterName(int index)
            {
                return _parameters[index].Item1;
            }

            private AoiDefinition _aoiDefinition;
            public AoiDefinitionCore(AoiDefinition aoiDefinition, bool isTmp = false)
            {
                _isTmp = isTmp;
                _aoiDefinition = aoiDefinition;
                this.datatype = new AOIDataType(aoiDefinition);
                datatype.IsTmp = _isTmp;
                Debug.Assert(infos.Count == 0, infos.Count.ToString());

                _arguments.Add(Tuple.Create(aoiDefinition.Name, (IDataType)this.datatype, ParameterType.INOUT));

                infos.Add(Tuple.Create((Predicate<ASTExpr>)CommonInstruction.AllPass, (IDataType)this.datatype,
                    ParameterType.INOUT));
                poses.Add(Tuple.Create((DataTypeMember)null, ParameterType.INOUT));

                args_size_ = 1;
            }

            static ParameterType FromUsage(Usage usage)
            {
                switch (usage)
                {
                    case Usage.Input:
                        return ParameterType.INPUT;
                    case Usage.Output:
                        return ParameterType.OUTPUT;
                    case Usage.InOut:
                        return ParameterType.INOUT;
                    default:
                        Debug.Assert(false, usage.ToString());
                        break;
                }

                return (ParameterType)usage;
            }

            private void PostInit(DataTypeCollection coll, string removeName = null)
            {
                Clear(removeName ?? Name);
                _parameters?.Clear();
                ArgInfos?.Clear();
                instr = null;
                Debug.Assert(infos.Count == 1, infos.Count.ToString());
                Debug.Assert(poses.Count == 1, poses.Count.ToString());

                foreach (ITag param in _aoiDefinition.Tags.Where(t => t.Usage != Usage.Local))
                {
                    var type = param.DataTypeInfo.DataType;
                    if (param.DataTypeInfo.Dim1 > 0)
                    {
                        type = new ArrayType(type, param.DataTypeInfo.Dim1, param.DataTypeInfo.Dim2, param.DataTypeInfo.Dim3);
                    }

                    if (param.IsRequired)
                    {
                        var u = FromUsage(param.Usage);
                        _arguments.Add(Tuple.Create(param.Name, type, u));

                        infos.Add(Tuple.Create((Predicate<ASTExpr>)CommonInstruction.AllPass, type, u));
                        poses.Add(Tuple.Create(this.datatype.TypeMembers[param.Name] as DataTypeMember, u));

                    }

                    if (param.Usage == Usage.InOut)
                    {
                        _parameters.Add(Tuple.Create(param.Name, type, args_size_));
                        if (type.IsBool)
                        {
                            Debug.Assert(type.IsBool && (type == BOOL.SInst || type == BOOL.Inst) && (type as BOOL).RefDataType.ByteSize == 1);
                            args_size_ += 2;
                        }
                        else
                        {
                            args_size_ += 1;
                        }
                    }
                }

                Debug.Assert(instr == null);
                instr = new AOIInstruction(Name, infos, poses, args_size_);
                Debug.Assert(ArgInfos.Count == 0);
                {
                    var parameters = new List<IDataType>();
                    var isRefs = new List<bool>();
                    parameters.Add(this.datatype);
                    isRefs.Add(true);
                    ArgInfos.Add(new ArgInfo { Type = this.datatype, IsRef = true });
                    foreach (var p in _parameters)
                    {
                        parameters.Add(p.Item2);
                        isRefs.Add(true);
                        ArgInfos.Add(new ArgInfo { Type = p.Item2, IsRef = true });

                        if (p.Item2.IsBool)
                        {
                            ArgInfos.Add(new ArgInfo { Type = DINT.Inst, IsRef = false });
                            parameters.Add(DINT.Inst);
                            isRefs.Add(false);
                        }
                    }



                    if (!_isTmp)
                    {
                        RTInstructionCollection.Inst.Add(Name + ".Logic",
                            new RTInstructionInfo(Name + ".Logic", DINT.Inst, parameters, isRefs));
                        RTInstructionCollection.Inst.Add(Name + ".Prescan",
                            new RTInstructionInfo(Name + ".Prescan", DINT.Inst, parameters, isRefs));
                    }
                }
                if (!_isTmp)
                {
                    Controller.GetInstance().STInstructionCollection.AddInstruction(instr);
                    Controller.GetInstance().RLLInstructionCollection.AddInstruction(instr);
                    Controller.GetInstance().FBDInstructionCollection.AddInstruction(instr);
                }
            }

            private void GenCodeFromAST(Controller controller, ConstPool constPool, ASTNode ast)
            {
                TypeChecker p = new TypeChecker(controller, null, this.Host);
                ast = ast.Accept(p);

                CodeGenVisitor gen = new CodeGenVisitor(constPool, args_size_);
                ast.Accept(gen);

                var prescan = new PrescanVisitor();
                CodeGenVisitor prescanGen = new CodeGenVisitor(constPool, args_size_);
                prescan.GenPrescanAST(ast).Accept(prescanGen);



                _funcs["Logic"] = new Function(gen.Codes, gen.SafePoints, gen.LocalsSize, ArgInfos);
                _funcs["Prescan"] = new Function(prescanGen.Codes, prescanGen.SafePoints, prescanGen.LocalsSize, ArgInfos);
            }

            void GenSTCode(Controller controller, STRoutine routine, ConstPool constPool)
            {
                var ast = STASTGenVisitor.Parse(routine.CodeText);

                GenCodeFromAST(controller, constPool, ast);
            }

            void GenRLLCode(Controller controller, RLLRoutine routine, ConstPool constPool)
            {
                ASTNode ast = RLLGrammarParser.Parse(routine.CodeText, controller);

                GenCodeFromAST(controller, constPool, ast);
            }

            public void GenCode(Controller controller)
            {
                var constPool = new ConstPool();
                _funcs = new Dictionary<string, Function>();
                var aoi = (controller.AOIDefinitionCollection as AoiDefinitionCollection)?.Find(Name);
                Debug.Assert(aoi != null, aoi.Name);
                foreach (var routine in aoi.Routines)
                {
                    if (routine.Name != "Logic")
                    {
                        continue;
                    }
                    var type = routine.Type;
                    switch ((RoutineType)type)
                    {
                        case RoutineType.ST:
                            GenSTCode(controller, routine as STRoutine, constPool);
                            break;
                        case RoutineType.RLL:
                            GenRLLCode(controller, routine as RLLRoutine, constPool);
                            break;
                        default:
                            Debug.Assert(false, type.ToString());
                            break;
                    }
                }

                Pool = new List<byte>();
                foreach (var tmp in constPool.consts_pool)
                {
                    Pool.AddRange(tmp);
                }

                var consts = MacroAssembler.ParseConstses(Pool);

                List<bool> isDINT = new List<bool>();
                isDINT.Add(false);
                foreach (var p in _parameters)
                {
                    isDINT.Add(false);

                    if (p.Item2.IsBool)
                    {
                        isDINT.Add(true);
                    }
                }
                foreach (var func in _funcs.Values)
                {
                    {
                        var builder = new BasicBlockBuilder(func, consts, isDINT);
                        builder.BuildBlocks();

                    }
                }
            }

            public void GenNativeCode(Controller controller, OutputStream writer)
            {
                var consts = MacroAssembler.ParseConstses(Pool);
                foreach (var tmp in _funcs)
                {
                    var func = tmp.Value;
                    var name = tmp.Key;

                    var codegen = new CCodeGenerator(null, func, consts, writer);
                    codegen.GenCode(Name + name.ToUpper());

                }
            }

            public void GenSepNativeCode(Controller controller)
            {
                NativeCode = controller.GenerateNativeCodeForAction((OutputStream writer) => GenNativeCode(controller, writer));
            }

            private byte[] NativeCode { get; set; }

            public JObject ConvertToJObject(bool needNativeCode = true)
            {
                var res = _aoiDefinition.GetConfig() as JObject;

                res["Functions"] = ToFunctions(_funcs);
                
                res["Pool"] = Function.EncodeByteArray(Pool);
                if (NativeCode != null && needNativeCode)
                {
                    res.Add("NativeCode", Function.EncodeByteArray(new List<byte>(NativeCode)));
                }
                return Utils.Utils.SortJObject(res);
            }

            private JObject ToFunctions(Dictionary<string, Function> functions)
            {
                JObject functionsObject = new JObject();

                foreach (var keyValuePair in functions)
                {
                    functionsObject.Add(keyValuePair.Key, keyValuePair.Value.ToJson());
                }

                return functionsObject;
            }

            public void Clear(string removeName)
            {
                if (instr == null)
                {
                    return;
                }

                if (!_isTmp)
                {
                    RTInstructionCollection.Inst.Remove(removeName + ".Logic");
                    RTInstructionCollection.Inst.Remove(removeName + ".Prescan");
                    Controller.GetInstance().STInstructionCollection.RemoveInstruction(instr);
                    Controller.GetInstance().RLLInstructionCollection.RemoveInstruction(instr);
                    Controller.GetInstance().FBDInstructionCollection.RemoveInstruction(instr);
                }
            }

            public void Reset()
            {
                _arguments.Clear();
                _arguments.Add(Tuple.Create(Name, (IDataType)this.datatype, ParameterType.INOUT));
                infos.Clear();
                infos.Add(Tuple.Create((Predicate<ASTExpr>)CommonInstruction.AllPass, (IDataType)this.datatype,
                    ParameterType.INOUT));
                poses.Clear();
                poses.Add(Tuple.Create((DataTypeMember)null, ParameterType.INOUT));
                args_size_ = 1;
                PostInit(_aoiDefinition.ParentController.DataTypes as DataTypeCollection, _aoiDefinition.OldName);
            }

            private ParamInfoList infos { get; } = new ParamInfoList();
            private ParamMemberList poses { get; } = new ParamMemberList();

            public AOIInstruction instr { get; private set; }

            private bool _isDisposed = false;
            public void Dispose()
            {
                if(_isDisposed)return;
                datatype?.Dispose();
                RTInstructionCollection.Inst.Remove(Name + ".Logic");
                RTInstructionCollection.Inst.Remove(Name + ".Prescan");
                Controller.GetInstance().STInstructionCollection.RemoveInstruction(instr);
                Controller.GetInstance().RLLInstructionCollection.RemoveInstruction(instr);
                Controller.GetInstance().FBDInstructionCollection.RemoveInstruction(instr);
                _isDisposed = true;
                GC.SuppressFinalize(false);
            }
        }
    }
}
