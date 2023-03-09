using System;
using System.Collections.Generic;
using System.Linq;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Compiler;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.ServiceModel.Dispatcher;
using ICSStudio.Interfaces.Instruction;
using ICSStudio.SimpleServices.Annotations;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.Instruction.Instructions;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.Utils;
using Xunit;
using Type = System.Type;

namespace ICSStudio.SimpleServices.Instruction
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;
    using ParamInfo = Tuple<Predicate<ASTExpr>, IDataType, ParameterType>;
    using ParamInfoList = List<Tuple<Predicate<ASTExpr>, IDataType, ParameterType>>;

    public enum Context
    {
        ST,
        RLL,
        FBD,
    };

    public enum ParameterType
    {
        INPUT,
        OUTPUT,
        INOUT,
    }

    //SetPredictor,
    //EnumPredictor
    //ArithPredictor,

    //set enum type
    //(predictor, type, input | output | inout);
    /*
    public interface IInstruction
    
    {
        //基础类型（SINT, INT, DINT, REAL, BOOL）

        //总的匹配规则是
        //对于输入，分为算数类型和布尔类型，分别匹配.这个直接得到表达式的值
        //对于输出类型，如果类型一致，则直接加载对应的地址
        //如果是不同类型的算术类型，从栈上分配一个SLOT，使用栈上的SLOT的地址，输出完再转换
        //如果是输入输出类型，要求类型必须兼容，有继承关系的可以按照父子类型来处理，无继承关系的必须一致

        //如果是数组类型的话，则当成输入输出类型来处理，同时传必要的偏移和数组本身的长度

        //对于某些可选参数的。对应数值是0
        //
        ASTNode Match(ASTNodeList paramList, Context context = Context.ST);
        ASTNode Prescan(ASTNodeList paramList, Context context = Context.ST);
        bool TypeCheck(ASTNodeList paramList, Context context = Context.ST);
    }

    */

    public abstract class IXInstruction
    {
        public virtual ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return parameters;
        }

        public virtual ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return Utils.ParseExprList(parameters);
        }

        public virtual ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            return paramList.Accept(checker) as ASTNodeList;
        }

        public abstract void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST);

        public virtual void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {

        }

        public virtual IDataType Type(ASTNodeList paramList)
        {
            return DINT.Inst;
        }

        public virtual List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var paramList = new List<Tuple<string, IDataType>>();
            return paramList;
        }

        public Dictionary<string,string> DefaultArguments { get; }=new Dictionary<string, string>();
        
        public string Name { get; internal set; }

        protected  string RLLConvert<T>(string parameter)
        {
            var name = parameter;
            Debug.Assert(!string.IsNullOrEmpty(name));
            int immediate;
            var res = int.TryParse(name, out immediate);
            if (res)
            {
                if (Enum.IsDefined(typeof(T), immediate))
                {
                    return parameter;
                }
                throw new InstructionException("error enum.");
            }
            object result = null;
            var enums = Enum.GetValues(typeof(T));
            foreach (T @enum in enums)
            {
                var attribute =
                    Attribute.GetCustomAttribute(@enum.GetType().GetField(@enum.ToString()),
                        typeof(EnumMemberAttribute)) as EnumMemberAttribute;
                if (attribute != null && attribute.Value.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    result = @enum;
                    break;
                }

            }

            if (result != null)
            {
                var value = Enum.Parse(typeof(T), result.ToString());
                return ((int)value).ToString();
            }
            else
            {
                throw new InstructionException("error enum.");
            }

            //return parameter;
        }
    }

    public class FixedInstruction : IXInstruction
    {
        protected ParamInfoList Infos { get; }

        public FixedInstruction(string name, ParamInfoList infos)
        {
            this.Name = name;
            this.Infos = infos;
        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            var tmp = paramList.Accept(checker) as ASTNodeList;
            MatchParam(tmp);
            return PrepareFixedParameters(tmp);
        }
        
        protected virtual void MatchParam(ASTNodeList paramList)
        {
            var infos = GetParameterInfo();
            if (infos.Count == 0)
            {
                infos = Infos.Select(info => { return new Tuple<string, IDataType>("", info.Item2); }).ToList();
            }
            if(paramList.Count()< infos.Count)throw new InstructionException("More arguments are expected for instruction.");
            if(paramList.Count()> infos.Count)throw new InstructionException("Too many arguments found for instruction.");
            for (int i = 0; i < infos.Count; i++)
            {
                var reference = infos[i];
                var astName = paramList.nodes[i] as ASTName;
                if (astName != null)
                {
                    if(!DataTypeExtend.IsMatched(astName.Expr.type,reference.Item2,this is AoiDefinition.AOIInstruction||"mcsv".Equals(Name,StringComparison.OrdinalIgnoreCase)))
                    {
                        if (DataTypeExtend.IsSameType(astName.Expr.type, reference.Item2))
                        {
                            if (astName.Expr.type is ArrayType)
                            {
                                throw new InstructionException(
                                    $"{Name},param {i + 1}:Missing reference to array element.");
                            }
                            else
                            {
                                if (reference.Item2 is ArrayTypeDimOne || reference.Item2 is ArrayTypeNormal ||
                                    reference.Item2 is ArrayType)
                                {
                                    if (astName.base_dim1 > 0)
                                    {
                                        if ("mapc".Equals(Name, StringComparison.OrdinalIgnoreCase) ||
                                            "matc".Equals(Name, StringComparison.OrdinalIgnoreCase) ||
                                            "mcsv".Equals(Name, StringComparison.OrdinalIgnoreCase) ||
                                            "mccp".Equals(Name, StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (astName.Expr.type is CAM_PROFILE)
                                            {
                                                bool isCorrect = astName.ArrayNodeList == null ||
                                                                 (astName.ArrayNodeList.nodes[0] as ASTInteger)
                                                                 ?.value == 0;
                                                if (!isCorrect)
                                                {
                                                    throw new InstructionException(
                                                        $"{Name},param {i + 1}:Reference must be zeroth element of an array,ex. tag[0].");
                                                }
                                            }
                                        }
                                        continue;
                                    }
                                }
                                throw new InstructionException(
                                    $"{Name},param {i + 1}:Not array element.");
                            }
                        }
                        throw new InstructionException(
                            $"{Name},param {i + 1}:Invalid data type.Argument must match parameter data type.");
                    }
                    continue;
                }

                var astInteger = paramList.nodes[i] as ASTInteger;
                if (astInteger != null)
                {
                    if (reference.Item2.IsBool)
                    {
                        if (astInteger.value == 0 || astInteger.value == 1)
                        {
                            continue;
                        }
                        else
                        {
                            throw new InstructionException(
                                $"{Name},param {i + 1}:Invalid data type.Argument must match parameter data type.");
                        }
                    }

                    if (reference.Item2 is ZeroType)
                    {
                        if (astInteger.value == 0)
                        {
                            continue;
                        }
                    }

                    if ((reference.Item2 as ExpectType)?.ExpectTypes.Any(inf => inf is ZeroType) ?? false)
                    {
                        if (astInteger?.value == 0)
                        {
                            continue;
                        }
                    }
                    
                    var arrayTypeDimOne = reference.Item2 as ArrayTypeDimOne;
                    if(arrayTypeDimOne!=null)
                    {
                        if ((arrayTypeDimOne?.AllowNull ?? false) && astInteger.value == 0)
                        {
                            continue;
                        }
                        else
                        {
                            throw new InstructionException(
                                $"{Name},param {i + 1}:Invalid data type.Argument must match parameter data type.");
                        }
                    }
                    if(!DataTypeExtend.IsMatched(reference.Item2,new ExpectType(GetIntegerExpectedType(astInteger.value,true))))
                        throw new InstructionException($"{Name},param {i + 1}:Invalid data type.Argument must match parameter data type.");
                    continue;
                }

                var astReal = paramList.nodes[i] as ASTFloat;
                if (astReal != null)
                {
                    if (!DataTypeExtend.IsMatched(reference.Item2, REAL.Inst))
                        throw new InstructionException($"{Name},param {i + 1}:Invalid data type.Argument must match parameter data type.");
                    continue;
                }

            }
        }

        private IDataType[] GetIntegerExpectedType(BigInteger v,bool needReal)
        {
            var expectedType =new List<IDataType>();
            if(needReal)
                expectedType.Add(REAL.Inst);
            if (v >= byte.MinValue && v <= byte.MaxValue)
            {
                expectedType.Add(SINT.Inst);
                expectedType.Add(USINT.Inst);
                expectedType.Add(INT.Inst);
                expectedType.Add(UINT.Inst);
                expectedType.Add(DINT.Inst);
                expectedType.Add(UDINT.Inst);
                return expectedType.ToArray();
            }
            if (v>byte.MaxValue&&v<=sbyte.MaxValue)
            {
                expectedType.Add(USINT.Inst);
                expectedType.Add(INT.Inst);
                expectedType.Add(UINT.Inst);
                expectedType.Add(DINT.Inst);
                expectedType.Add(UDINT.Inst);
                return expectedType.ToArray();
            }

            if (v >= short.MinValue && v <= short.MaxValue)
            {
                expectedType.Add(INT.Inst);
                expectedType.Add(UINT.Inst);
                expectedType.Add(DINT.Inst);
                expectedType.Add(UDINT.Inst);
                return expectedType.ToArray();
            }
            if (v > short.MaxValue && v <= ushort.MaxValue)
            {
                expectedType.Add(UINT.Inst);
                expectedType.Add(DINT.Inst);
                expectedType.Add(UDINT.Inst);
                return expectedType.ToArray();
            }

            if (v >= int.MinValue && v <= int.MaxValue)
            {
                expectedType.Add(DINT.Inst);
                expectedType.Add(UDINT.Inst);
                return expectedType.ToArray();
            }

            if (v > int.MaxValue && v <= uint.MaxValue)
            {
                expectedType.Add(UDINT.Inst);
                return expectedType.ToArray();
            }
            return expectedType.ToArray();
        }

        public IDataType GetParamDataType(int index)
        {
            if (index < 0 || index >= Infos.Count) return null;
            var info = Infos[index];
            return info.Item2;
        }

        public int ParametersCount => Infos.Count;

        /*
        protected static void AcceptParamList(CodeGenVisitor gen, ASTNodeList paramList)
        {
            for (int i = paramList.Count() - 1; i >= 0; --i)
            {
                paramList.nodes[i].Accept(gen);
            }
        }
        */

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            //AcceptParamList(gen, paramList);
            gen.VisitParamList(paramList);
            gen.masm().CallName(this.Name, paramList.Count());
        }


        protected static ASTExpr ConvertInput(ASTExpr ex, IDataType target)
        {
            var expr = ex as ASTName;
            return expr != null
                ? new ASTTypeConv(
                    new ASTNameValue(expr) {ContextStart = expr?.ContextStart ?? 0, ContextEnd = expr?.ContextEnd ?? 0},
                    target)
                : new ASTTypeConv(ex, target);
        }

        protected static ASTNodeList ConvertInout(ASTExpr ex, IDataType target)
        {
            var res = new ASTNodeList();

            if (target is ArrayTypeNormal)
            {
                if (!(ex is ASTName)) throw new InstructionException();
                var expr = ex as ASTName;
                res.AddNode(new ASTNameAddr(expr));
                res.AddNode(new ASTInteger(expr.base_dim1));
                res.AddNode(new ASTInteger(expr.base_dim2));
                res.AddNode(new ASTInteger(expr.base_dim3));
            }
            else if (target is ArrayTypeDimOne)
            {
                //FIXME check dimension
                if (!(ex is ASTName || ex is ASTInteger)) throw new InstructionException();
                //Debug.Assert(ex is ASTName || ex is ASTInteger);

                var expr = ex as ASTName;
                if (expr != null)
                {
                    var name = expr;
                    res.AddNode(new ASTNameAddr(name));
                    res.AddNode(new ASTInteger(0));
                    res.AddNode(new ASTInteger(name.base_dim1));
                }
                else
                {
                    // Debug.Assert(((ASTInteger)ex).value == 0,((ASTInteger)ex).value.ToString());
                    if (((ASTInteger) ex).value != 0) throw new InstructionException();
                    res.AddNode(new ASTNullAddr(target));
                    res.AddNode(new ASTInteger(0));
                    res.AddNode(new ASTInteger(0));
                }
            }
            else if (target is BOOL)
            {
                var name = ex as ASTName;
                //Debug.Assert(name != null);
                if (name == null) throw new InstructionException();
                res.AddNode(new ASTNameAddr(name));

            }
            else if (target is PID && ex is ASTInteger && ((ASTInteger) ex).value == 0)
            {
                res.AddNode(new ASTNullAddr(target));
            }
            else
            {
                var name = ex as ASTName;
                // Debug.Assert(name != null, ex.ToString());
                if (name == null) throw new InstructionException();
                res.AddNode(new ASTNameAddr(name));
            }

            return res;
        }

        protected static ASTNode ConvertOutput(ASTExpr ex, IDataType target)
        {
            //Debug.Assert(false);
            Debug.Assert(ex is ASTName);
            Debug.Assert(ex.type.IsInteger || ex.type.IsReal || ex.type.IsBool);
            return new ASTNameAddr(ex as ASTName)
                {ContextStart = ex?.ContextStart ?? 0, ContextEnd = ex?.ContextEnd ?? 0};
        }

        protected ASTNodeList PrepareFixedParameters(ASTNodeList param_list)
        {
            Debug.Assert(Infos != null);
            var res = new ASTNodeList();
            var infos = GetParameterInfo();
            if (infos.Count == 0)
            {
                infos = Infos.Select(info => { return new Tuple<string, IDataType>("", info.Item2); }).ToList();
            }
            if (infos.Count != param_list.Count())
            {
                if (infos.Count > param_list.Count())
                {
                    throw new InstructionException("More arguments are expected for instruction.");
                }
                else
                {
                    throw new InstructionException("Too many arguments found for instruction.");
                }
            }
            for (var i = 0; i < infos.Count; ++i)
            {
                var node = param_list.nodes.ElementAt(i) as ASTExpr;
                Debug.Assert(node != null, param_list.nodes.ElementAt(i).ToString());
                var info = Infos.ElementAt(i);
                var info2 = infos.ElementAt(i);
                if (!node.IsExpected(info2.Item2))
                {
                    if ((info2.Item2 as ExpectType)?.ExpectTypes.Any(inf=>inf is ZeroType)??false)
                    {
                        var integer = node as ASTInteger;
                        if (integer?.value != 0)
                        {
                            throw new InstructionException($"Parameter {i + 1}:Invalid data type.Argument must match parameter data type..");
                        }
                    }

                    else if (info2.Item2 is ZeroType)
                    {
                        var integer = node as ASTInteger;
                        if (integer?.value != 0)
                        {
                            throw new InstructionException(
                                $"Parameter {i + 1}:Invalid data type.Argument must match parameter data type..");
                        }
                    }
                    else
                    {
                        if (DataTypeExtend.IsSameType(node.type, info2.Item2))
                        {
                            if (node.type is ArrayType)
                            {
                                throw new InstructionException(
                                    $"{Name},param {i + 1}:Missing reference to array element.");
                            }
                            else
                            {
                                throw new InstructionException(
                                    $"{Name},param {i + 1}:Not array element.");
                            }
                        }
                        throw new InstructionException(
                            $"Parameter {i + 1}:Invalid data type.Argument must match parameter data type..");
                    }
                }
                var type = info.Item3;
                if (type == ParameterType.INPUT)
                {
                    res.AddNode(ConvertInput(node, info.Item2));
                }
                else if (type == ParameterType.INOUT)
                {  
                    res.AddNodes(ConvertInout(node, info.Item2));
                }
                else
                {
                    res.AddNode(ConvertOutput(node, info.Item2));
                }
            }

            return res;
        }

        protected static IDataType CommonType(IDataType left, IDataType right)
        {
            if (left.IsBool && right.IsBool)
            {
                return BOOL.Inst;
            }

            if (left.IsInteger && right.IsInteger)
            {
                return DINT.Inst;
            }

            if (left.IsNumber && right.IsNumber)
            {
                return REAL.Inst;
            }

            if (left is AXIS_COMMON && right is AXIS_COMMON)
            {
                return AXIS_COMMON.Inst;
            }

            throw new CommonTypeNotFoundException($"{left.ToString()}, {right.ToString()}");
        }


    }

    public class RLLInstruction : IXInstruction
    {
        public RLLInstruction(string name)
        {
            Name = name;
        }

        public sealed override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL, Name);
            var label = new MacroAssembler.Label();
            gen.masm().Dup();
            gen.masm().JeqL(label);
            LogicExec(gen, paramList, context);
            gen.masm().Bind(label);
            gen.masm().Dup();
        }

        public virtual void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            throw new NotImplementedException();
        }
    }

    public class RLLSTInstruction : IXInstruction
    {
        public RLLSTInstruction(string name)
        {
            Name = name;
        }

        public sealed override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST) 
        {
            Debug.Assert(context == Context.ST || context == Context.RLL);
            var label = new MacroAssembler.Label();
            if (context == Context.RLL)
            {
                gen.masm().Dup();
                gen.masm().JeqL(label);
            }

            LogicExec(gen, paramList, context);

            if (context == Context.RLL)
            {
                gen.masm().Bind(label);
                gen.masm().Dup();
            }
            else
            {
                gen.masm().BiPush(0);
            }

        }

        public virtual void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            throw new NotImplementedException();
        }
    }

    public class FixedRLLSTInstruction : FixedInstruction
    {
        public FixedRLLSTInstruction(string name, ParamInfoList infos) : base(name, infos)
        {
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.ST || context == Context.RLL);
            var label = new MacroAssembler.Label();
            if (context == Context.RLL)
            {
                gen.masm().Dup();
                gen.masm().JeqL(label);
            }

            LogicExec(gen, paramList, context);

            if (context == Context.RLL)
            {
                gen.masm().Bind(label);
                gen.masm().Dup();
            }
            else
            {
                gen.masm().BiPush(0);
            }

        }

        public virtual void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            throw new NotImplementedException();
        }
    }
    /*
    public abstract class FixedRLLBinOpInstruction : FixedInstruction
    {
        public FixedRLLBinOpInstruction(string name, ParamInfoList infos) : base(name, infos)
        {

        }

        public sealed override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL, context.ToString());
            Debug.Assert(paramList.Count() == 3, paramList.Count().ToString());
            Debug.Assert((paramList.nodes[0] as ASTExpr).type == (paramList.nodes[1] as ASTExpr).type, paramList.ToString());
            gen.VisitParamList(paramList);
            var tp = (paramList.nodes[0] as ASTExpr).type;

            Op(gen, tp);

            gen.masm().Store((paramList.nodes[2] as ASTNameAddr).ref_type.type, tp, 0);
            gen.masm().Dup();
        }

        public abstract void Op(CodeGenVisitor gen, IDataType tp);

    }
    */

    /*
    public abstract class FixedRLLUnOpInstruction : FixedInstruction
    {
        public FixedRLLUnOpInstruction(string name, ParamInfoList infos) : base(name, infos)
        {

        }

        public sealed override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL, context.ToString());
            Debug.Assert(paramList.Count() == 2, paramList.Count().ToString());
            gen.VisitParamList(paramList);
            var tp = (paramList.nodes[0] as ASTExpr).type;

            Op(gen, tp);

            gen.masm().Store((paramList.nodes[1] as ASTNameAddr).ref_type.type, tp, 0);
            gen.masm().Dup();
        }

        public abstract void Op(CodeGenVisitor gen, IDataType tp);
    }
    */

    public abstract class FixedRLLTrueInstruction : FixedInstruction
    {
        public FixedRLLTrueInstruction(string name, ParamInfoList infos) : base(name, infos)
        {

        }

        public sealed override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL, context.ToString());
            Debug.Assert(paramList.Count() >= 1, paramList.Count().ToString());

            var label = new MacroAssembler.Label();
            gen.masm().Dup();
            gen.masm().Dup();
            gen.masm().JeqL(label);

            Op(gen, paramList);

            gen.masm().Bind(label);
        }

        protected abstract void Op(CodeGenVisitor gen, ASTNodeList paramList);
    }


    public abstract class FixedRLLInstruction : FixedInstruction
    {
        public FixedRLLInstruction(string name, ParamInfoList infos) : base(name, infos)
        {

        }

        public sealed override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL, context.ToString());
            Debug.Assert(paramList.Count() >= 2, paramList.Count().ToString());

            var label = new MacroAssembler.Label();
            gen.masm().Dup();
            gen.masm().Dup();
            gen.masm().JeqL(label);

            //gen.VisitParamList(paramList);
            paramList.nodes[paramList.Count() - 1].Accept(gen);
            for (int i = 0; i < paramList.Count() - 1; ++i)
                paramList.nodes[i].Accept(gen);
            var tp = (paramList.nodes[0] as ASTExpr).type;

            Op(gen, tp);

            gen.masm().Store((paramList.nodes[paramList.Count() - 1] as ASTNameAddr).ref_type.type, ReturnType(tp));

            gen.masm().Bind(label);
        }

        protected abstract void Op(CodeGenVisitor gen, IDataType tp);

        protected virtual IDataType ReturnType(IDataType firstType)
        {
            return firstType;
        }
    }

    public abstract class FixedRLLOutInstruction : FixedInstruction
    {
        public FixedRLLOutInstruction(string name, ParamInfoList infos) : base(name, infos)
        {

        }

        public sealed override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL, context.ToString());
            Debug.Assert(paramList.Count() >= 1, paramList.Count().ToString());

            var label = new MacroAssembler.Label();
            gen.masm().Dup();
            gen.masm().Dup();
            gen.masm().JeqL(label);

            Op(gen, paramList);
            gen.masm().Swap();
            gen.masm().Pop();

            gen.masm().Bind(label);
        }

        protected abstract void Op(CodeGenVisitor gen, ASTNodeList paramList);
    }

    public abstract class FixedSTInstruction : FixedInstruction
    {
        public FixedSTInstruction(string name, ParamInfoList infos) : base(name, infos)
        {

        }

        public sealed override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.ST, context.ToString());
            gen.VisitParamList(paramList);
            Op(gen);
        }

        protected abstract void Op(CodeGenVisitor gen);

    }

    public class CommonTypeNotFoundException : ICSStudioException
    {
        public CommonTypeNotFoundException(string str) : base(str)
        {
        }
    }

    public class FixedTokenInstruction : FixedInstruction
    {
        public FixedTokenInstruction(string name, ParamInfoList infos) : base(name, infos)
        {
        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {

          //  Console.WriteLine(Name);
            var tmp = PrepareFixedParameters(paramList.Accept(checker) as ASTNodeList);
            MatchParam(paramList);
            var res = new ASTNodeList();
            if (context == Context.ST)
            {
                res.AddNode(new ASTInteger(65));
            } else if (context == Context.RLL)
            {
                res.AddNode(new ASTDup(DINT.Inst));
            }

            res.AddNodes(tmp);
           // Console.WriteLine($"Ddone {Name}");
            res.OriginalNodeList = paramList;
            return res;
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {

            gen.VisitParamList(paramList);
            gen.masm().CallName(this.Name, paramList.Count());
        }

    }

    public class Fixed2PosTokenInstruction : FixedTokenInstruction
    {
        public Fixed2PosTokenInstruction(string name, ParamInfoList infos) : base(name, infos)
        {
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var type = MOTION_INSTRUCTION.Inst;
            var instr = paramList.nodes[2] as ASTNameAddr;

            Debug.Assert(CheckInstrParamType.CheckMotionParam(instr));

            paramList.nodes[2].Accept(gen);

            Utils.GenClearBit(gen.masm(), type["EN"]);
            Utils.GenClearBit(gen.masm(), type["DN"]);
            Utils.GenClearBit(gen.masm(), type["ER"]);
            Utils.GenClearBit(gen.masm(), type["IP"]);
            gen.masm().Pop();
            gen.masm().BiPush(0);
        }
    }

    public class Fixed2B3PosTokenInstruction : FixedTokenInstruction
    {
        public Fixed2B3PosTokenInstruction(string name, ParamInfoList infos) : base(name, infos)
        {
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var type = MOTION_INSTRUCTION.Inst;
            var instr = paramList.nodes[2] as ASTNameAddr;

            Debug.Assert(CheckInstrParamType.CheckMotionParam(instr));

            paramList.nodes[2].Accept(gen);

            Utils.GenClearBit(gen.masm(), type["EN"]);
            Utils.GenClearBit(gen.masm(), type["DN"]);
            Utils.GenClearBit(gen.masm(), type["ER"]);
            gen.masm().Pop();
            gen.masm().BiPush(0);
        }


    }
    /*

    public class MCLMInstruction : FixedTokenInstruction
    {
        public MCLMInstruction(ParamInfoList infos) : base("MCLM", infos)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() == 22)
            {
                parameters.nodes[5] = STConvert<InstrEnum.MCLMSpeedunits>(parameters.nodes[5]);
                parameters.nodes[7] = STConvert<InstrEnum.MCLMAccelUnits>(parameters.nodes[7]);
                parameters.nodes[9] = STConvert<InstrEnum.MCLMAccelUnits>(parameters.nodes[9]);
                parameters.nodes[10] = STConvert<InstrEnum.Profile>(parameters.nodes[10]);
                parameters.nodes[13] = STConvert<InstrEnum.MCLMJerkUnits>(parameters.nodes[13]);
                parameters.nodes[15] = STConvert<InstrEnum.MCLMMerge>(parameters.nodes[15]);
                parameters.nodes[16] = STConvert<InstrEnum.MCLMSpeed>(parameters.nodes[16]);
                parameters.nodes[19] = STConvert<InstrEnum.MCLMLockDirection>(parameters.nodes[19]);
            }
            else
            {
                throw new Exception("MCLMFInstruction:parameter count should be 22");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() == 22)
            {
                parameters[5] = RLLConvert<InstrEnum.MCLMSpeedunits>(parameters[5]);
                parameters[7] = RLLConvert<InstrEnum.MCLMAccelUnits>(parameters[7]);
                parameters[9] = RLLConvert<InstrEnum.MCLMAccelUnits>(parameters[9]);
                parameters[10] = RLLConvert<InstrEnum.Profile>(parameters[10]);
                parameters[13] = RLLConvert<InstrEnum.MCLMJerkUnits>(parameters[13]);
                parameters[15] = RLLConvert<InstrEnum.MCLMMerge>(parameters[15]);
                parameters[16] = RLLConvert<InstrEnum.MCLMSpeed>(parameters[16]);
                parameters[19] = RLLConvert<InstrEnum.MCLMLockDirection>(parameters[19]);
            }
            else
            {
                throw new Exception("MCLMFInstruction:parameter count should be 22");
            }
            return Utils.ParseExprList(parameters);
        }
    }
    */

    public class UnImplementedInstruction : FixedInstruction
    {
        public UnImplementedInstruction() : base(null, new ParamInfoList { })
        {
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            gen.masm().BiPush(0);
        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            return paramList.Accept(checker) as ASTNodeList;
        }

    }

    internal class FakeInstruction : IXInstruction
    {
        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            /*
            if (parameters.Count() != 4)
            {
                throw new Exception($"GSVInstruction:parameters count should be 1:{parameters.Count()}");
            }
            */

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            /*
            if (parameters.Count() != 4)
            {
                throw new Exception($"EventInstruction:parameters count should be 1:{parameters.Count()}");
            }
            */

            return Utils.ParseExprList(parameters);
        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            return paramList;
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            gen.masm().BiPush(0);
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            gen.masm().BiPush(0);

        }
    }

    public class Mapping
    {
        public TempSlot slot;
        public ASTName name;
    }

    /*
    public abstract class IFixedInstruction : IInstruction
    {
        protected string name { get; set; }
        protected ParamInfoList infos { get; } = new ParamInfoList();

        protected IFixedInstruction()
        {
        }

        protected IFixedInstruction(string name, ParamInfoList infos)
        {
            this.name = name;
            this.infos = infos;
        }

        public virtual ASTNode Match(ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(false);
            return null;
        }

        public virtual ASTNode Prescan(ASTNodeList paramList, Context context = Context.ST)
        {
            //Debug.Assert(false);
            return new ASTNop();
        }

        public virtual bool TypeCheck(ASTNodeList paramList, Context context = Context.ST)
        {
            return false;
        }

        protected static ASTExpr ConvertInput(ASTExpr ex, IDataType target)
        {
            var expr = ex as ASTName;
            return expr != null ? new ASTTypeConv(new ASTNameValue(expr), target) : new ASTTypeConv(ex, target);
        }

        protected static ASTNodeList ConvertInout(ASTExpr ex, IDataType target)
        {
            var res = new ASTNodeList();
            if (target is ArrayTypeDimOne)
            {
                //FIXME check dimension
                if (!(ex is ASTName || ex is ASTInteger)) throw new InstructionException();
                //Debug.Assert(ex is ASTName || ex is ASTInteger);

                var expr = ex as ASTName;
                if (expr != null)
                {
                    var name = expr;
                    res.AddNode(new ASTNameAddr(name));
                    res.AddNode(new ASTInteger(0));
                    res.AddNode(new ASTInteger(name.dim1));
                }
                else
                {
                    //Debug.Assert(((ASTInteger)ex).value == 0);
                    if (((ASTInteger) ex).value != 0) throw new InstructionException();
                    res.AddNode(new ASTNullAddr(target));
                    res.AddNode(new ASTInteger(0));
                    res.AddNode(new ASTInteger(0));
                }
            }
            else if (target is BOOL)
            {
                if (!(ex is ASTName)) throw new InstructionException();
                var name = ex as ASTName;
                //Debug.Assert(name != null);

            }
            else
            {
                if (!(ex is ASTName)) throw new InstructionException();
                var name = ex as ASTName;
                //Debug.Assert(name != null, ex.ToString());
                res.AddNode(new ASTNameAddr(name));
            }

            return res;
        }

        //转换后的参数列表,因为类型不同需要的分配操作,分配操作对应的映射
        protected static Tuple<ASTNodeList, ASTNodeList, Mapping> ConvertOutput(ASTExpr expr, IDataType target)
        {
            var res = new ASTNodeList();
            var converter = new ASTNodeList();
            var name = expr as ASTName;
            Debug.Assert(expr.type.IsAtomic);
            Debug.Assert(name != null, expr.ToString());
            Debug.Assert(name.type == target, $"{expr.ToString()}:{target.ToString()}");

            Mapping mapping = null;
            if (name.type != target)
            {
                var slot = new TempSlot(target);
                converter.AddNode(new ASTTempAddr(slot));
                //res.AddNode(new ASTStore(new ASTTempAddr(slot), new ASTTypeConv(new ASTNameValue(name), target)));
                mapping = new Mapping {slot = slot, name = name};
            }
            else
            {
                res.AddNode(new ASTNameAddr(name));
            }

            return Tuple.Create(res, converter, mapping);
        }

        protected List<Mapping> PrepareFixedParameters(ASTNodeList param_list, ASTNodeList conv_param_list,
            ASTNodeList res)
        {
            var mappings = new List<Mapping>();

            for (var i = 0; i < infos.Count; ++i)
            {
                var node = param_list.nodes.ElementAt(i) as ASTExpr;
                Debug.Assert(node != null, param_list.nodes.ElementAt(i).ToString());
                var info = infos.ElementAt(i);
                var type = info.Item3;
                if (type == ParameterType.INPUT)
                {
                    conv_param_list.AddNode(ConvertInput(node, info.Item2));
                }
                else if (type == ParameterType.INOUT)
                {
                    conv_param_list.AddNodes(ConvertInout(node, info.Item2));
                }
                else
                {
                    var tuple = ConvertOutput(node, info.Item2);
                    conv_param_list.AddNodes(tuple.Item1);
                    res.AddNodes(tuple.Item2);
                    if (tuple.Item3 != null)
                    {
                        mappings.Add(tuple.Item3);
                    }
                }

            }

            return mappings;
        }

        protected static void ProcessOutput(List<Mapping> mappings, ASTNodeList res)
        {
            foreach (var mapping in mappings)
            {
                res.AddNode(new ASTStore(new ASTNameAddr(mapping.name),
                    new ASTTypeConv(new ASTTempValue(mapping.slot), mapping.name.type)));
            }
        }

        protected void ProcessCall(string rt_name, ASTNodeList param_list, ASTNodeList conv_param_list, ASTNodeList res)
        {

            var mappings = PrepareFixedParameters(param_list, conv_param_list, res);

            res.AddNode(new ASTRTCall(rt_name, conv_param_list));

            ProcessOutput(mappings, res);
        }
    }

        */
    public abstract class FixedFBDInstruction : FixedInstruction
    {
        public FixedFBDInstruction(string name, ParamInfoList infos) : base(name, infos)
        {
        }

        public sealed override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            //Debug.Assert(context == Context.FBD || context == Context.ST, context.ToString());

            Debug.Assert(paramList.Count() == 1, paramList.ToString());

            gen.VisitParamList(paramList);

            if (context == Context.ST)
            {
                Debug.Assert(EnableInMember != null);
                Utils.GenSetBit(gen.masm(), EnableInMember);
            }

            gen.masm().CallName(InstrName, 1);
        }

        protected virtual IDataTypeMember EnableInMember { get; }

        protected abstract string InstrName { get; }

    }
    /*
    public class FixedTokenInstruction : IFixedInstruction
    {
        public FixedTokenInstruction(string name, ParamInfoList infos) : base(name, infos)
        {
        }

        private ASTNode GenToken(ASTNodeList list, Context context)
        {
            if (context == Context.ST)
            {
                return new ASTInteger(65);
            }
            else if (context == Context.RLL)
            {
                var slot = new TempSlot(DINT.Inst);
                list.AddNode(new ASTStore(new ASTTempAddr(slot), new ASTDup(DINT.Inst), false));
                return new ASTTempValue(slot);
            }
            else
            {
                Debug.Assert(false, context.ToString());
                return null;
            }
        }

        private ASTNode PostProcess(Context context)
        {
            return new ASTNop(); 
        }

        public override ASTNode Match(ASTNodeList param_list, Context context = Context.ST)
        {
            //FIXME add type check for param_list;
            Debug.Assert(infos.Count == param_list.nodes.Count, $"{infos.Count}:{param_list.nodes.Count}");

            var res = new ASTNodeList();
            var conv_param_list = new ASTNodeList();
            ASTNode token = GenToken(res, context);
            conv_param_list.AddNode(token);
            var mappings = PrepareFixedParameters(param_list, conv_param_list, res);
            res.AddNode(new ASTRTCall(this.name, conv_param_list));
            ProcessOutput(mappings, res);
            res.AddNode(PostProcess(context));

            return res;
        }
    }
    */

    /*
    public class FixedFBDInstruction : IFixedInstruction
    {
        public FixedFBDInstruction(string name, ParamInfoList infos) : base(name, infos)
        {
        }

        public override ASTNode Match(ASTNodeList paramList, Context context = Context.ST)
        {
            if (context == Context.ST)
            {
                return new ASTRTCall(name, paramList);
            }
            else
            {
                Debug.Assert(false, context.ToString());
            }
            return null;
        }
    }
    */

    /*
        //XIC XIO
        public class FixedCustomInstruction : IFixedInstruction
        {
            public FixedCustomInstruction(string name, ParamInfoList infos, ASTCodeGen.CodeGenDelegate logic = null, ASTCodeGen.CodeGenDelegate prescan = null) : base(name, infos)
            {
                this._logic = logic;
                this._logic = prescan;
            }

            public override ASTNode Match(ASTNodeList param_list, Context context = Context.ST)
            {
                if (_logic == null)
                    return new ASTNop();

                var res = new ASTNodeList();
                var conv_param_list = new ASTNodeList();
                var mappings = PrepareFixedParameters(param_list, conv_param_list, res);

                res.AddNode(new ASTCodeGen(_logic));

                ProcessOutput(mappings, res);
                return res;
            }

            private readonly ASTCodeGen.CodeGenDelegate _logic;
            //private readonly ASTCodeGen.CodeGenDelegate _prescan;   
        }
    */

    /*
    public class FixedCustomInstruction : IFixedInstruction
    {
        public FixedCustomInstruction(string name, ParamInfoList infos) : base(name, infos)
        {

        }

    }
    */

    /*
    public class AFIFixedInstruction : FixedCustomInstruction
    {
        public AFIFixedInstruction(ParamInfoList infos) : base("AFI", infos)
        {
        }

        public override ASTNode Match(ASTNodeList param_list, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL);
            return new ASTInteger(0);
        }
    }
    */


    /*
    public class OTUFixedInstruction : FixedCustomInstruction
    {
        public OTUFixedInstruction(ParamInfoList infos) : base("OTU", infos)
        {
        }

        public override ASTNode Match(ASTNodeList param_list, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL, context.ToString());
            Debug.Assert(param_list.nodes.Count == 1, param_list.nodes.Count.ToString());
            var tag = param_list.nodes.ElementAt(0) as ASTName;
            Debug.Assert(tag?.type is BOOL);

            var res = new ASTNodeList();
            res.AddNode(new ASTDup(BOOL.Inst));
            res.AddNode(ASTNode.CreateLIRIf(new ASTDup(BOOL.Inst), new ASTStore(new ASTNameAddr(tag), new ASTInteger(0)), new ASTNop()));
            return res;
        }
    }
    */

    /*
    public class EQUFixedInstruction : FixedCustomInstruction
    {
        public EQUFixedInstruction(ParamInfoList infos) : base("EQU", infos)
        {
        }

        public override ASTNode Match(ASTNodeList param_list, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL, context.ToString());
            Debug.Assert(param_list.nodes.Count == 2, param_list.nodes.Count.ToString());

            var res = new ASTNodeList();

            var left = param_list.nodes[0] as ASTExpr;
            var right = param_list.nodes[1] as ASTExpr;
            Debug.Assert(left != null && right != null);

            var cmp = new ASTBinRelOp(ASTBinOp.Op.EQ, left, right);
            cmp.type = BOOL.Inst;
            cmp.op_type = DINT.Inst;

            var true_branch = ASTNode.CreateLIRIf(cmp, new ASTInteger(1), new ASTInteger(0));
            res.AddNode(ASTNode.CreateLIRIf(new ASTDup(BOOL.Inst), true_branch, new ASTInteger(0)));
            return res;
        }
    }
    */

    /*
    public class VariableInstruction : IInstruction
    {
        public ASTNode Match(ASTNodeList param_list, Context context = Context.ST)
        {
            return null;
        }

        public ASTNode Prescan(ASTNodeList param_list, Context context = Context.ST)
        {
            return new ASTNop();
        }

        public bool TypeCheck(ASTNodeList param_list, Context context = Context.ST)
        {
            return false;
        }

    }
    */

    //public class SWPBInstruction : FixedTokenInstruction
    //{
    //    public SWPBInstruction(ParamInfoList infos) : base("SWPB", infos)
    //    {
    //    }

    //    public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
    //    {
    //        if (parameters.Count() == 3)
    //        {
    //            parameters.nodes[1] = STConvert<InstrEnum.OrderMode>(parameters.nodes[1]);
    //        }
    //        else
    //        {
    //            throw new Exception("SWPBFInstruction:parameter count should be 3");
    //        }

    //        return parameters;
    //    }

    //    public override ASTNodeList ParseRLLParameters(List<string> parameters)
    //    {
    //        if (parameters.Count() == 3)
    //        {
    //            parameters[1] = RLLConvert<InstrEnum.MasterReference>(parameters[1]);
    //        }
    //        else
    //        {
    //            throw new Exception("SWPBFInstruction:parameter count should be 3");
    //        }
    //        return Utils.ParseExprList(parameters);
    //    }

    //}

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "Typo")]
    // ReSharper disable All
    internal class CommonInstruction
    {
        public static bool AllPass(ASTExpr expr)
        {
            return true;
        }
        /*
        public static readonly FixedTokenInstruction MSO = new FixedTokenInstruction("MSO", new ParamInfoList
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
        });
        public static readonly FixedTokenInstruction MSF = new FixedTokenInstruction("MSF", new ParamInfoList
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
        });
        public static readonly FixedTokenInstruction MASD = new FixedTokenInstruction("MASD", new ParamInfoList
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
        });
        public static readonly FixedTokenInstruction MASR = new FixedTokenInstruction("MASR", new ParamInfoList
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
        });
        public static readonly FixedTokenInstruction MAFR = new FixedTokenInstruction("MAFR", new ParamInfoList
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
        });
        public static readonly FixedTokenInstruction MAS = new FixedTokenInstruction("MAS", new ParamInfoList
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),

        });

        public static readonly FixedTokenInstruction MAM = new FixedTokenInstruction("MAM", new ParamInfoList
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(DINT.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(REAL.Inst), ParameterType.INOUT),
        });

        public static readonly FixedTokenInstruction MAJ = new FixedTokenInstruction("MAJ", new ParamInfoList
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
        });

        public static readonly FixedTokenInstruction MATC = new FixedTokenInstruction("MATC", new ParamInfoList
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(CAM_PROFILE.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
        });

        public static readonly FixedFBDInstruction OSRI = new FixedFBDInstruction("OSRI", new ParamInfoList
        {
            new ParamInfo(AllPass, FBD_ONESHOT.Inst, ParameterType.INOUT),
        });

        public static readonly FixedCustomInstruction XIC = new FixedCustomInstruction("XIC", new ParamInfoList
        {
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INOUT)
        }, InstructionBuiltins.XICLogic);

        public static readonly XIOFixedInstruction XIO = new XIOFixedInstruction(new ParamInfoList
        {
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INOUT)
        });
        public static readonly AFIFixedInstruction AFI = new AFIFixedInstruction(new ParamInfoList
        {
        });
        public static readonly OTEFixedInstruction OTE = new OTEFixedInstruction(new ParamInfoList
        {
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INOUT)
        });
        
        public static readonly FixedCustomInstruction OTU = new OTUFixedInstruction(new ParamInfoList
        {
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INOUT)
        });

        
        public static readonly FixedCustomInstruction OTU = new FixedCustomInstruction("OTU", new ParamInfoList
        {
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INOUT)
        }, InstructionBuiltins.OTULogic);
         
        public static readonly OTLFixedInstruction OTL = new OTLFixedInstruction(new ParamInfoList
        {
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction EQU = new FixedInstruction("EQU", new ParamInfoList
        {
            //USELESS
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INOUT)
        });
        */

        public static readonly IXInstruction ALM = new ALMInstruction(new ParamInfoList
        {
            new ParamInfo(AllPass, ALARM.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction SCL = new SCLInstruction(new ParamInfoList
        {
            new ParamInfo(AllPass, SCALE.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction PID = new PIDInstruction(new ParamInfoList
        {
            new ParamInfo(AllPass, PredefinedType.PID.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, PredefinedType.PID.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction PIDE = new PIDEInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, PID_ENHANCED.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction FPIDE = new FPIDEInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, PID_ENHANCED.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, PIDE_AUTOTUNE.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction IMC = new IMCInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, PredefinedType.IMC.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction CC = new CCInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, PredefinedType.CC.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction MMC = new MMCInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, PredefinedType.MMC.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction RMPS = new RMPSInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, PredefinedType.RAMP_SOAK.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(REAL.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(REAL.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(REAL.Inst), ParameterType.INOUT)
        });

        public static readonly IXInstruction POSP = new POSPInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, POSITION_PROP.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction SRTP = new SRTPInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, SPLIT_RANGE.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction LDLG = new LDLGInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, LEAD_LAG.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction FGEN = new FGENInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FUNCTION_GENERATOR.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction TOT = new TOTInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, TOTALIZER.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction DEDT = new DEDTInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DEADTIME.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(REAL.Inst), ParameterType.INOUT),
        });

        public static readonly IXInstruction D2SD = new D2SDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DISCRETE_2STATE.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction D3SD = new D3SDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DISCRETE_3STATE.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction PMUL = new PMULInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, PULSE_MULTIPLIER.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction SCRV = new SCRVInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, SCALE.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction PI = new PIInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, PROP_INT.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction INTG = new INTGInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, INTEGRATOR.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction SOC = new SOCInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, SEC_ORDER_CONTROLLER.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction UPDN = new UPDNInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, UP_DOWN_ACCUM.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction HPF = new HPFInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FILTER_HIGH_PASS.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction LPF = new LPFInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FILTER_LOW_PASS.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction NTCH = new NTCHInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FILTER_NOTCH.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction LDL2 = new LDL2Instruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, LEAD_LAG_SEC_ORDER.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction DERV = new DERVInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DERIVATIVE.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction SEL = new SELInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, SELECT.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction ESEL = new ESELInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, SELECT.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction SSUM = new SSUMInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, SELECTED_SUMMER.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction SNEG = new SNEGInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, SELECTABLE_NEGATE.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction MUX = new MUXInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, MULTIPLEXER.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction HLL = new HLLInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, HL_LIMIT.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction RLIM = new RLIMInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, RATE_LIMITER.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction MAVE = new MAVEInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, MOVING_AVERAGE.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(REAL.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(REAL.Inst), ParameterType.INOUT),
        });

        public static readonly IXInstruction MSTD = new MSTDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, MOVING_STD_DEV.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(REAL.Inst), ParameterType.INOUT),
        });

        public static readonly IXInstruction MINC = new MINCInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, MINIMUM_CAPTURE.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction MAXC = new MAXCInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, MAXIMUM_CAPTURE.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction ALMD = new ALMDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, ALARM_DIGITAL.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction RALMD = new RALMDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, ALARM_DIGITAL.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
        });

        public static readonly IXInstruction FALMD = new FALMDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, ALARM_DIGITAL.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction ALMA = new ALMAInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, ALARM_DIGITAL.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction FALMA = new FALMAInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, ALARM_DIGITAL.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction OSRI = new OSRIInstruction(new ParamInfoList
        {
            new ParamInfo(AllPass, FBD_ONESHOT.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction OSFI = new OSFIInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_ONESHOT.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction TONR = new TONRInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_TIMER.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction TOFR = new TOFRInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_TIMER.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction RTOR = new RTORInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_TIMER.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction CTUD = new CTUDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_COUNTER.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction FLIM = new FLIMInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_LIMIT.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction LIM = new LIMInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction FMEQ = new FMEQInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_MASK_EQUAL.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction RMEQ = new MEQInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction SQRT = new SQRTInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction FSQRT = new FSQRTInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_MATH_ADVANCED.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction ABS = new ABSInstruction();

        public static readonly IXInstruction FABS = new FABSInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_MATH_ADVANCED.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction RABS = new RABSInstruction();

        public static readonly IXInstruction MVMT = new MVMTInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_MASKED_MOVE.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction SWPB = new SWPBInstruction();

        public static readonly IXInstruction BTDT = new BTDTInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_BIT_FIELD_DISTRIBUTE.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction DFF = new DFFInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FLIP_FLOP_D.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction JKFF = new JKFFInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FLIP_FLOP_JK.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction SETD = new SETDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DOMINANT_SET.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction RESD = new RESDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DOMINANT_RESET.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction HMIBC = new HMIBCInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, PredefinedType.HMIBC.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction RHMIBC = new R_HMIBCInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, PredefinedType.HMIBC.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction SIN = new SINInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction RSIN = new RSINInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction FSIN = new FSINInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_MATH_ADVANCED.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction COS = new COSInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction RCOS = new RCOSInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction FCOS = new FCOSInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_MATH_ADVANCED.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction TAN = new TANInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction RTAN = new RTANInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction FTAN = new FTANInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_MATH_ADVANCED.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction ASIN = new ASINInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction RASIN = new RASINInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction FASIN = new FASINInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_MATH_ADVANCED.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction ACOS = new ACOSInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction RACOS = new RACOSInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction FACOS = new FACOSInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_MATH_ADVANCED.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction ATAN = new ATANInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction RATAN = new RATANInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction FATAN = new FATANInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_MATH_ADVANCED.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction LN = new LNInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction RLN = new RLNInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction FLN = new FLNInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_MATH_ADVANCED.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction LOG = new LOGInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction RLOG = new RLOGInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction FLOG = new FLOGInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_MATH_ADVANCED.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction DEG = new DEGInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction RDEG = new RDEGInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction FDEG = new FDEGInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_MATH_ADVANCED.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction RAD = new RADInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction RRAD = new RRADInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction FRAD = new FRADInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_MATH_ADVANCED.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction TRUNC = new TRUNCInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction RTRN = new TRNInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction FTRN = new FTRNInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_TRUNCATE.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction COP = new COPInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction SRT = new SRTInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, new ArrayTypeNormal(DINT.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, CONTROL.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction RSRT = new RSRTInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, new ArrayTypeNormal(DINT.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, CONTROL.Inst, ParameterType.INOUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT)
        });

        public static readonly IXInstruction SIZE = new SIZEInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, new ArrayTypeDimOne(DINT.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction CPS = new CPSInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction PSC = new PSCInstruction(new ParamInfoList { });

        public static readonly IXInstruction PFL = new PFLInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction PCMD = new PCMDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, PHASE.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction PCLF = new PCLFInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, PHASE.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction PXRQ = new PXRQInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, PHASE_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(DINT.Inst), ParameterType.INOUT),
        });

        public static readonly IXInstruction PPD = new PPDInstruction(new ParamInfoList { });
        public static readonly IXInstruction PRNP = new PRNPInstruction(new ParamInfoList { });

        public static readonly IXInstruction PATT = new PATTInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, PHASE.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction PDET = new PDETInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, PHASE.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction POVR = new POVRInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, PHASE.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction SATT = new SATTInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, SEQUENCE.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction SDET = new SDETInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, SEQUENCE.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction SCMD = new SCMDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, SEQUENCE.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction SCLF = new SCLFInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, SEQUENCE.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction SOVR = new SOVRInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, SEQUENCE.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction SASI = new SASIInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, SEQUENCE.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, STRING.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction JSR = new JSRInstruction(); //

        public static readonly IXInstruction RET = new RETInstruction();

        public static readonly IXInstruction SBR = new SBRInstruction(new ParamInfoList { });
        public static readonly IXInstruction TND = new TNDInstruction(new ParamInfoList { });
        public static readonly IXInstruction UID = new UIDInstruction(new ParamInfoList { });
        public static readonly IXInstruction UIE = new UIEInstruction(new ParamInfoList { });
        public static readonly IXInstruction SFR = new SFRInstruction(new ParamInfoList { });
        public static readonly IXInstruction SFP = new SFPInstruction(new ParamInfoList { });

        public static readonly IXInstruction EOT = new EOTInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction EVENT = new EventInstruction();

        public static readonly IXInstruction MSG = new MSGInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, MESSAGE.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction GSV = new GSVInstruction(); /*("GSV", new ParamInfoList()
        {
            new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
            new ParamInfo(AllPass,STRING.Inst,ParameterType.INPUT),
            new ParamInfo(AllPass,STRING.Inst,ParameterType.INPUT),
            new ParamInfo(AllPass,DINT.Inst,ParameterType.INOUT)
        });*/

        public static readonly IXInstruction SSV = new SSVInstruction(); /* FixedInstruction("SSV", new ParamInfoList()
            {
                new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
                new ParamInfo(AllPass,STRING.Inst,ParameterType.INPUT),
                new ParamInfo(AllPass,STRING.Inst,ParameterType.INPUT),
                new ParamInfo(AllPass,DINT.Inst,ParameterType.INOUT)
            });
            */

        public static readonly IXInstruction IOT = new IOTInstruction(new ParamInfoList
            {new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT)});

        public static readonly IXInstruction MSO = new MSOInstruction(new ParamInfoList
        {
            new ParamInfo(AllPass, AXIS_CIP_DRIVE.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction MSF = new MSFInstruction(new ParamInfoList
        {
            new ParamInfo(AllPass, AXIS_CIP_DRIVE.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction MASD = new MASDInstruction(new ParamInfoList
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction MASR = new MASRInstruction(new ParamInfoList
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction MDO = new MDOInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, AXIS_SERVO.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction MDF = new MDFInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, AXIS_SERVO.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction MDS = new MDSInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction MAFR = new MAFRInstruction(new ParamInfoList
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction MAS = new MASInstruction(new ParamInfoList
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),

        });

        public static readonly IXInstruction MAH = new MAHInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction MAJ = new MAJInstruction(new ParamInfoList
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction MAM = new MAMInstruction(new ParamInfoList
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(REAL.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(REAL.Inst), ParameterType.INOUT),
        });

        public static readonly IXInstruction MAG = new MAGInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, UDINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, UDINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, UDINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction MCD = new MCDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction MRP = new MRPInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction MCCP = new MCCPInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(CAM.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(CAM_PROFILE.Inst), ParameterType.INOUT),
        });

        public static readonly IXInstruction MCSV = new MCSVInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(CAM_PROFILE.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction MAPC = new MAPCInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, INT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(CAM_PROFILE.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, INT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, INT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, INT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, INT.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction MATC = new MATCInstruction(new ParamInfoList
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(CAM_PROFILE.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction MDAC = new MDACInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, INT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, INT.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction MGS = new MGSInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, MOTION_GROUP.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction MGSD = new MGSDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, MOTION_GROUP.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction MGSR = new MGSRInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, MOTION_GROUP.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction MGSP = new MGSPInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, MOTION_GROUP.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction MAW = new MAWInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction MDW = new MDWInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction MAR = new MARInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, INT.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction MDR = new MDRInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, INT.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction MAOC = new MAOCInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(OUTPUT_CAM.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(OUTPUT_COMPENSATION.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction MDOC = new MDOCInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, AXIS_CIP_DRIVE.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, INT.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction MAAT = new MAATInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction MRAT = new MRATInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction MAHD = new MAHDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction MRHD = new MRHDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction MCS = new MCSInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, COORDINATE_SYSTEM.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction MCLM = new MCLMInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, COORDINATE_SYSTEM.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(REAL.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(REAL.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(REAL.Inst), ParameterType.INOUT)
        });

        public static readonly IXInstruction MCCM = new MCCMInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, COORDINATE_SYSTEM.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(REAL.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(REAL.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(REAL.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(REAL.Inst), ParameterType.INOUT),
        });

        public static readonly IXInstruction MCCD = new MCCDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, COORDINATE_SYSTEM.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction MCT = new MCTInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, COORDINATE_SYSTEM.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, COORDINATE_SYSTEM.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(REAL.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(REAL.Inst), ParameterType.INOUT)
        });

        public static readonly IXInstruction MCTP = new MCTPInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, COORDINATE_SYSTEM.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, COORDINATE_SYSTEM.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(REAL.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(REAL.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(REAL.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(REAL.Inst), ParameterType.INOUT),
        });

        public static readonly IXInstruction MCSD = new MCSDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, COORDINATE_SYSTEM.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction MCSR = new MCSRInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, COORDINATE_SYSTEM.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction MDCC = new MDCCInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, COORDINATE_SYSTEM.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, AXIS_COMMON.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, MOTION_INSTRUCTION.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, INT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction FIND = new FINDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, STRING.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, STRING.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction INSERT = new INSERTInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, STRING.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, STRING.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, STRING.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction CONCAT = new CONCATInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, STRING.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, STRING.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, STRING.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction RCONCAT = new RCONCATInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, STRING.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, STRING.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, STRING.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction MID = new MIDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, STRING.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, STRING.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction DELETE = new DELETEInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, STRING.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, STRING.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction DTOS = new DTOSInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, STRING.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction STOD = new STODInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, STRING.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction RTOS = new RTOSInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, STRING.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction STOR = new STORInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, STRING.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction UPPER = new UPPERInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, STRING.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, STRING.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction LOWER = new LOWERInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, STRING.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, STRING.Inst, ParameterType.INOUT)
        });

        //RLL specific Instructions
        public static readonly IXInstruction XIC = new XICInstruction(new ParamInfoList
        {
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction XIO = new XIOInstruction(new ParamInfoList
        {
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction AFI = new AFIInstruction(new ParamInfoList
        {
        });

        public static readonly IXInstruction OTE = new OTEInstruction(new ParamInfoList
        {
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction OTU = new OTUInstruction(new ParamInfoList
        {
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction OTL = new OTLInstruction(new ParamInfoList
        {
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction FEQU = new FEQUInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_COMPARE.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction EQU = new EQUInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction FNEQ = new FNEQInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_COMPARE.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction RNEQ = new NEQInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction FLES = new FLESInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_COMPARE.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction RLES = new LESInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction FGRT = new FGRTInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_COMPARE.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction RGRT = new GRTInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction FLEQ = new FLEQInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_COMPARE.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction RLEQ = new LEQInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction FGEQ = new FGEQInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_COMPARE.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction RGEQ = new GEQInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction FADD = new FADDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_MATH.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction RADD = new ADDInstruction();

        public static readonly IXInstruction FSUB = new FSUBInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_MATH.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction RSUB = new SUBInstruction();

        public static readonly IXInstruction FMUL = new FMULInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_MATH.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction RMUL = new MULInstruction();

        public static readonly IXInstruction FDIV = new FDIVInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_MATH.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction RDIV = new DIVInstruction();

        public static readonly IXInstruction FMOD = new FMODInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_MATH.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction RMOD = new MODInstruction();

        public static readonly IXInstruction FSQR = new FSQRInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_MATH_ADVANCED.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction RSQR = new RSQRTInstruction();

        public static readonly IXInstruction FNEG = new FNEGInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_MATH_ADVANCED.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction RNEG = new NEGInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction FAND = new FANDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_LOGICAL.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction RAND = new ANDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction FOR = new F_ORInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_LOGICAL.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction ROR = new ORInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction FXOR = new FXORInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_LOGICAL.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction RXOR = new XORInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction FNOT = new FNOTInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_CONVERT.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction RNOT = new NOTInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction BAND = new BANDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_BOOLEAN_AND.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction BOR = new BORInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_BOOLEAN_OR.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction BXOR = new BXORInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_BOOLEAN_XOR.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction BNOT = new BNOTInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_BOOLEAN_NOT.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction FXPY = new FXPYInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_MATH.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction RXPY = new XPYInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, LREAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, LREAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, LREAL.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction FTOD = new FTODInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_CONVERT.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction RTOD = new TODInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction FFRD = new FFRDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, FBD_CONVERT.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction RFRD = new FRDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction RTON = new TONInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, TIMER.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction RTOF = new TOFInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, TIMER.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction RRTO = new RTOInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, TIMER.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction RCTU = new CTUInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, COUNTER.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction RCTD = new CTDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, COUNTER.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT)
        });

        //FIXME:unknown type
        public static readonly IXInstruction RRES = new RESInstruction();

        //FIXME:unknown type
        public static readonly IXInstruction RCMP = new CMPInstruction();

        public static readonly IXInstruction RCPT = new CPTInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction RMOV = new MOVInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction RMVM = new MVMInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction RCLR = new CLRInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT)
        });

        public static readonly IXInstruction RBTD = new BTDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction RFAL = new FALInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, CONTROL.Inst, ParameterType.INOUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT)
        });

        //FIXME:unknown type
        public static readonly IXInstruction RFSC = new FSCInstruction(new ParamInfoList());

        public static readonly IXInstruction RFLL = new FLLInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT)
        });

        public static readonly IXInstruction RAVE = new AVEInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, new ArrayTypeNormal(DINT.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, CONTROL.Inst, ParameterType.INOUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
        });

        public static readonly IXInstruction RSTD = new STDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, new ArrayTypeDimOne(DINT.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, REAL.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, CONTROL.Inst, ParameterType.INOUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
        });

        public static readonly IXInstruction RBSL = new BSLInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, new ArrayTypeNormal(DINT.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, CONTROL.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
        });

        public static readonly IXInstruction RBSR = new BSRInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, new ArrayTypeNormal(DINT.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, CONTROL.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INPUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
        });

        //FIXME:unknown type
        public static readonly IXInstruction RFFL = new FFLInstruction(new ParamInfoList());

        //FIXME:unknown type
        public static readonly IXInstruction RFFU = new FFUInstruction(new ParamInfoList());

        //FIXME:unknown type
        public static readonly IXInstruction RLFL = new LFLInstruction(new ParamInfoList());

        //FIXME:unknown type
        public static readonly IXInstruction RLFU = new LFUInstruction(new ParamInfoList());

        public static readonly IXInstruction RSQI = new SQIInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, new ArrayTypeDimOne(DINT.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, CONTROL.Inst, ParameterType.INOUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
        });

        public static readonly IXInstruction RSQO = new SQOInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, new ArrayTypeNormal(DINT.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, CONTROL.Inst, ParameterType.INOUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
        });

        public static readonly IXInstruction RSQL = new SQLInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, new ArrayTypeNormal(DINT.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, CONTROL.Inst, ParameterType.INOUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
        });

        //FIXME:unknown type
        public static readonly IXInstruction RJMP = new JMPInstruction(new ParamInfoList { });

        //FIXME:unknown type
        public static readonly IXInstruction RLBL = new LBLInstruction(new ParamInfoList { });

        //FIXME:unknown type
        public static readonly IXInstruction RJXR = new FakeInstruction();

        public static readonly IXInstruction RMCR = new MCRInstruction(new ParamInfoList());

        //FIXME:unknown type
        public static readonly IXInstruction RSFR = new SFRInstruction(new ParamInfoList());

        //FIXME:unknown type
        public static readonly IXInstruction RSFP = new SFPInstruction(new ParamInfoList());

        public static readonly IXInstruction RNOP = new NOPInstruction(new ParamInfoList());

        //FIXME:unknown type
        public static readonly IXInstruction RFOR = new FORInstruction(new ParamInfoList());

        public static readonly IXInstruction RBRK = new BRKInstruction(new ParamInfoList());

        public static readonly IXInstruction RFBC = new FBCInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, new ArrayTypeDimOne(DINT.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(DINT.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(DINT.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, CONTROL.Inst, ParameterType.INOUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
            new ParamInfo(AllPass, CONTROL.Inst, ParameterType.INOUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
        });

        public static readonly IXInstruction RDDT = new DDTInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, new ArrayTypeDimOne(DINT.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(DINT.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, new ArrayTypeDimOne(DINT.Inst), ParameterType.INOUT),
            new ParamInfo(AllPass, CONTROL.Inst, ParameterType.INOUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
            new ParamInfo(AllPass, CONTROL.Inst, ParameterType.INOUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
        });

        public static readonly IXInstruction RDTR = new DTRInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
            new ParamInfo(AllPass, DINT.Inst, ParameterType.INPUT),
        });

        public static readonly IXInstruction ONS = new ONSInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction OSR = new OSRInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction OSF = new OSFInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INOUT),
            new ParamInfo(AllPass, BOOL.Inst, ParameterType.INOUT),
        });

        public static readonly IXInstruction TON = new TONInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, TIMER.Inst, ParameterType.INOUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
        });

        public static readonly IXInstruction TOF = new TOFInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, TIMER.Inst, ParameterType.INOUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
        });

        public static readonly IXInstruction RTO = new RTOInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, TIMER.Inst, ParameterType.INOUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
        });

        public static readonly IXInstruction CTU = new CTUInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, COUNTER.Inst, ParameterType.INOUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
        });

        public static readonly IXInstruction CTD = new CTDInstruction(new ParamInfoList()
        {
            new ParamInfo(AllPass, COUNTER.Inst, ParameterType.INOUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
            //new ParamInfo(AllPass,DINT.Inst,ParameterType.INPUT),
        });

        public static readonly IXInstruction RES = new RESInstruction();
    }
}
