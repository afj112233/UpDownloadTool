using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.Utils;

namespace ICSStudio.SimpleServices.Instruction.Instructions
{
    using ParamInfo = Tuple<Predicate<ASTExpr>, IDataType, ParameterType>;
    using ParamInfoList = List<Tuple<Predicate<ASTExpr>, IDataType, ParameterType>>;
    
    internal class COPInstruction : RLLSTInstruction
    {
        public COPInstruction(ParamInfoList infos) : base("COP")
        {

        }

        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            var parameters = paramList.nodes;
            var src = (parameters[0] as ASTName).Expr;
            var dest = (parameters[1] as ASTName).Expr;
            gen.GenCopyParameter(src);
            var elementSize = gen.GenCopyParameter(dest);
            parameters[2].Accept(gen);
            gen.masm().CLoadInteger(elementSize);
            gen.masm().IMul();
            gen.masm().CallName("COP", 7);
            gen.masm().Pop();

        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            var parameters = paramList.Accept(checker) as ASTNodeList;
            Debug.Assert(parameters.Count() == 3, parameters.Count().ToString());
            var source = parameters.nodes[0] as ASTName;
            if (source == null)
            {
                throw new TypeCheckerException($"{Name},param 1:Invalid data type.Argument must match parameter data type.");
            }
            else
            {
                if (source.Expr.type is ArrayType||source.Expr.type is BOOL)
                {
                    throw new TypeCheckerException($"{Name},param 1:Invalid data type.Argument must match parameter data type.");
                }
            }

            var dest = parameters.nodes[1] as ASTName;
            if (dest == null)
            {
                throw new TypeCheckerException($"{Name},param 2:Invalid data type.Argument must match parameter data type.");
            }
            else
            {
                if (dest.Expr.type is ArrayType || dest.Expr.type is BOOL)
                {
                    throw new TypeCheckerException($"{Name},param 2:Invalid data type.Argument must match parameter data type.");
                }

                if (dest.type is MESSAGE || dest.type is ALARM_ANALOG || dest.type is ALARM_DIGITAL ||
                    dest.type is AXIS_COMMON || dest.type is AXIS_CONSUMED || dest.type is AXIS_GENERIC ||
                    dest.type is AXIS_GENERIC_DRIVE ||
                    dest.type is AXIS_SERVO || dest.type is AXIS_SERVO_DRIVE || dest.type is COORDINATE_SYSTEM ||
                    dest.type is ENERGY_BASE || dest.type is ENERGY_ELECTRICAL || dest.type is HMIBC ||
                    dest.type is MOTION_GROUP)
                {
                    throw new TypeCheckerException($"{Name},param 2:Invalid data type.Argument must match parameter data type.");
                }
            }
            var len = parameters.nodes[2];
            if (!(len is ASTName || len is ASTInteger))
            {
                throw new TypeCheckerException($"{Name},param 3:Invalid data type.Argument must match parameter data type.");
            }
            else
            {
                if(((len as ASTInteger)?.value??1)==0) throw new TypeCheckerException($"{Name},param 3:Immediate value out of range.");
            }
            return parameters;
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Source", new ExceptType(BOOL.Inst,LREAL.Inst,USINT.Inst,UINT.Inst,UDINT.Inst,ULINT.Inst));
            //need add module
            var param2 = new Tuple<string, IDataType>("Dest", new ExceptType(BOOL.Inst,USINT.Inst,UINT.Inst,UDINT.Inst,LREAL.Inst,AXIS_CONSUMED.Inst,AXIS_VIRTUAL.Inst,AXIS_GENERIC.Inst,AXIS_GENERIC_DRIVE.Inst,AXIS_SERVO.Inst,AXIS_SERVO_DRIVE.Inst,AXIS_CIP_DRIVE.Inst,MESSAGE.Inst,MOTION_GROUP.Inst,COORDINATE_SYSTEM.Inst,ALARM_DIGITAL.Inst,ALARM_ANALOG.Inst,HMIBC.Inst,ULINT.Inst));
            var param3 = new Tuple<string, IDataType>("Length", new ExpectType(SINT.Inst,INT.Inst,DINT.Inst));
            return new List<Tuple<string, IDataType>>() {param1, param2,param3};
        }

    }

    internal class SRTInstruction : FixedInstruction
    {
        public SRTInstruction(ParamInfoList infos) : base("SRT", infos)
        {

        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            var res = paramList.Accept(checker) as ASTNodeList;
            Debug.Assert(res != null);
            var array = paramList.nodes[0] as ASTName;
            var index = paramList.nodes[1] as ASTInteger;
            var name = paramList.nodes[2] as ASTName;
            Debug.Assert(array != null);
            Debug.Assert(index != null);
            Debug.Assert(name != null);
            if (array.loaders.nodes.Count == 2)
            {
                var dims = (array.loaders.nodes[1] as ASTArrayLoader)?.dims;
                if (dims != null)
                    foreach (var dim in dims.nodes)
                    {
                        var integer = dim as ASTInteger;
                        if (integer == null || integer.value != 0)
                            throw new ICSStudioException("SRT : param 1 is not array.");
                    }
            }

            var arrayType = array.Expr.type as ArrayType;
            if (arrayType==null)
                throw new ICSStudioException("SRT : param 1 is not array.");
            int max = arrayType.Dim3 == 0 ? (arrayType.Dim2 == 0 ? 0 : 1) : 2;
            if (index == null)
                throw new ICSStudioException("SRT , param 1 :Invalid kind of operand or argument.");
            if (!(index.value >= 0 && index.value <= max))
                throw new ICSStudioException("SRT ,pram 1:Immediate value out of range.");
            if (!(name.type is CONTROL)) throw new ICSStudioException("SRT ,param 3:Invalid data type.");
            
            return PrepareFixedParameters(res);
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Utils.ThrowNotImplemented(this.Name);
            gen.masm().BiPush(0);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Array", new ExpectType(new ArrayTypeNormal(SINT.Inst),new ArrayTypeNormal(INT.Inst),new ArrayTypeNormal(DINT.Inst),new ArrayTypeNormal(REAL.Inst)));
            var param2=new Tuple<string, IDataType>("Dim. To Vary",new ExpectType(SINT.Inst,INT.Inst,DINT.Inst));
            var param3=new Tuple<string, IDataType>("Control",CONTROL.Inst);
            return new List<Tuple<string, IDataType>>() { param1, param2, param3 };
        }
    }

    internal class RSRTInstruction : FixedInstruction
    {
        public RSRTInstruction(ParamInfoList infos) : base("SRT", infos)
        {

        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            parameters.RemoveRange(parameters.Count - 2, 2);
            return Utils.ParseExprList(parameters);
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Utils.ThrowNotImplemented(this.Name);
            gen.masm().BiPush(0);
        }

    }

    internal class SIZEInstruction : RLLSTInstruction
    {
        public SIZEInstruction(ParamInfoList infos) : base("SIZE")
        {
        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            var res = paramList.Accept(checker) as ASTNodeList;
            Debug.Assert(res != null);
            var array = paramList.nodes[0] as ASTName;
            var index = paramList.nodes[1] as ASTInteger;
            var name = paramList.nodes[2] as ASTName;
            Debug.Assert(array != null);
            Debug.Assert(index != null);
            Debug.Assert(name != null);
            if (array.loaders.nodes.Count == 2)
            {
                var dims = (array.loaders.nodes[1] as ASTArrayLoader)?.dims;
                if (dims != null)
                    foreach (var dim in dims.nodes)
                    {
                        var integer = dim as ASTInteger;
                        if (integer == null || integer.value != 0)
                            throw new ICSStudioException("size : param 1 is not array.");
                    }
            }
            
            if (array.base_dim1==0)
                throw new ICSStudioException("size : param 1 is not array.");
            int max = array.base_dim3 == 0 ? (array.base_dim2 == 0 ? 0 : 1) : 2;
            if(index==null)
                throw new ICSStudioException("size , param 1 :Invalid kind of operand or argument.");
            if(!(index.value >= 0 && index.value <= max))
                throw new ICSStudioException("size ,pram 1:Immediate value out of range.");
            if (!(name.type.IsNumber)) throw new ICSStudioException("size ,param 3:Invalid data type.");
            res.nodes[2] = new ASTNameAddr(name);
            return res;
        }

        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(paramList.Count() == 3, paramList.Count().ToString());

            // var array = paramList.;
            var array = paramList.nodes[0] as ASTName;
            var index = paramList.nodes[1] as ASTInteger;
            var name = paramList.nodes[2] as ASTNameAddr;
            Debug.Assert(array != null);
            Debug.Assert(index != null);
            Debug.Assert(name != null);

            name.Accept(gen);
            if (array.base_dim3 > 0)
            {
                if (index.value == 0)
                {
                    gen.masm().CLoadInteger(array.base_dim3);
                }
                else if (index.value == 1)
                {
                    gen.masm().CLoadInteger(array.base_dim2);
                }
                else if (index.value == 2)
                {
                    gen.masm().CLoadInteger(array.base_dim1);
                }
                else
                {
                    Debug.Assert(false, index.value.ToString());
                }
            }else if (array.base_dim2 > 0)
            {
                if (index.value == 0)
                {
                    gen.masm().CLoadInteger(array.base_dim2);
                }
                else if (index.value == 1)
                {
                    gen.masm().CLoadInteger(array.base_dim1);
                }
            }
            else
            {
                gen.masm().CLoadInteger(array.base_dim1);
            }

            gen.masm().Store(name.ref_type.type, DINT.Inst);

        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Source", new ExceptType(BOOL.Inst,LREAL.Inst,USINT.Inst,UINT.Inst,UDINT.Inst,ULINT.Inst));
            var param2 = new Tuple<string, IDataType>("Dim. To Vary", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param3 = new Tuple<string, IDataType>("Size", new ExpectType(SINT.Inst,INT.Inst,DINT.Inst,REAL.Inst));
            return new List<Tuple<string, IDataType>>() { param1, param2, param3 };
        }
    }

    internal class CPSInstruction : RLLSTInstruction
    {
        public CPSInstruction(ParamInfoList infos) : base("CPS")
        {

        }

        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            var parameters = paramList.nodes;
            var src = (parameters[0] as ASTName).Expr;
            var dest = (parameters[1] as ASTName).Expr;
            gen.GenCopyParameter(src);
            var elementSize = gen.GenCopyParameter(dest);
            parameters[2].Accept(gen);
            gen.masm().CLoadInteger(elementSize);
            gen.masm().IMul();
            gen.masm().CallName("CPS", 7);
            gen.masm().Pop();

        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            var res = paramList.Accept(checker) as ASTNodeList;
            var source = res.nodes[0] as ASTName;
            var dest = res.nodes[1] as ASTName;
            var length = res.nodes[2] as ASTExpr;
            if (source.Expr.type is ArrayType)
            {
                throw new TypeCheckerException($"CPS, Parameter 1:Missing reference to array element.");
            }

            if (source.Expr.type.IsBool)
            {
                throw new TypeCheckerException(
                    $"CPS, Parameter 1:Invalid data type.Argument must match parameter data type.");
            }

            if (dest.Expr.type is ArrayType)
            {
                throw new TypeCheckerException($"CPS, Parameter 2:Missing reference to array element.");
            }

            if (dest.Expr.type.IsBool)
            {
                throw new TypeCheckerException(
                    $"CPS, Parameter 2:Invalid data type.Argument must match parameter data type.");
            }

            if (!length.type.IsInteger)
            {
                throw new TypeCheckerException(
                    $"CPS, Parameter 3:Invalid data type.Argument must match parameter data type.");
            }
            
            if (IsNEG(length))
            {
                throw new TypeCheckerException($"CPS, Parameter 3 :Immediate value out of range.");
            }

            return res;
        }

        private bool IsNEG(ASTNode astNode)
        {
            var astInteger = astNode as ASTInteger;
            if (astInteger != null)
            {
                return astInteger.value < 1;
            }
            
            var astUnaryOp = astNode as ASTUnaryOp;
            if (astUnaryOp != null)
            {
                if (astUnaryOp.op == ASTUnaryOp.Op.NEG) return !IsNEG(astUnaryOp.expr);
                if (astUnaryOp.op == ASTUnaryOp.Op.PLUS) return IsNEG(astUnaryOp.expr);
            }

            return false;
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Source", new ExceptType(BOOL.Inst, LREAL.Inst, USINT.Inst, UINT.Inst, UDINT.Inst, ULINT.Inst));
            //FIXME:add module
            var param2 = new Tuple<string, IDataType>("Dest", new ExceptType(BOOL.Inst, USINT.Inst, UINT.Inst, UDINT.Inst, LREAL.Inst, AXIS_CONSUMED.Inst, AXIS_VIRTUAL.Inst, AXIS_GENERIC.Inst, AXIS_GENERIC_DRIVE.Inst, AXIS_SERVO.Inst, AXIS_SERVO_DRIVE.Inst, AXIS_CIP_DRIVE.Inst, MESSAGE.Inst, MOTION_GROUP.Inst, COORDINATE_SYSTEM.Inst, ALARM_DIGITAL.Inst, ALARM_ANALOG.Inst, HMIBC.Inst, ULINT.Inst));
            var param3 = new Tuple<string, IDataType>("Length", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            return new List<Tuple<string, IDataType>>() { param1, param2, param3 };
        }
    }

    internal class FALInstruction : FixedInstruction
    {
        public FALInstruction(ParamInfoList infos) : base("FAL", infos)
        {

        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            parameters.RemoveRange(1, 2);
            if (parameters.Count == 4)
            {
                parameters[1] = RLLConvert<InstrEnum.Mode>(parameters[1]);
            }
            else
            {
                throw new Exception("FALInstruction:parameter count should be 4");
            }

            return Utils.ParseExprList(parameters);
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Utils.ThrowNotImplemented(this.Name);
            gen.masm().BiPush(0);
        }

    }

    internal class FSCInstruction : FixedInstruction
    {
        public FSCInstruction(ParamInfoList infos) : base("FSC", infos)
        {

        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            parameters.RemoveRange(1, 2);

            if (parameters.Count == 3)
            {
                parameters[1] = RLLConvert<InstrEnum.Mode>(parameters[1]);
            }
            else
            {
                throw new Exception($"FSCInstruction:parameter count should be 3 {parameters.Count}");
            }

            return Utils.ParseExprList(parameters);

        }
    }

    internal class FLLInstruction : FixedInstruction
    {
        public FLLInstruction(ParamInfoList infos) : base("FLL", infos)
        {

        }
    }

    //prescan:If .ER bit is zero during prescan, all the control bits (.DN, .EN, .EU, .EM, .UL, .IN and .FD) will be cleared to zero.
    internal class AVEInstruction : FixedInstruction
    {
        public AVEInstruction(ParamInfoList infos) : base("AVE", infos)
        {

        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            parameters.RemoveRange(parameters.Count - 2, 2);
            return Utils.ParseExprList(parameters);
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[6] as ASTNameAddr;
            //Console.WriteLine($"{addr.expr.type}:{addr.type}:{paramList.Count()}");
            //Debug.Assert(addr != null, (paramList.nodes[3].ToString()));
            Debug.Assert(addr.type is RefType);
            Debug.Assert((addr.type as RefType).type == CONTROL.Inst);
            addr.Accept(gen);
            var en = CONTROL.Inst["EN"];
            var dn = CONTROL.Inst["DN"];
            Utils.GenClearBit(gen.masm(), en);
            Utils.GenClearBit(gen.masm(), dn);
            gen.masm().Pop();
        }
    }

    internal class STDInstruction : FixedInstruction
    {
        public STDInstruction(ParamInfoList infos) : base("STD", infos)
        {

        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            parameters.RemoveRange(parameters.Count - 2, 2);
            return Utils.ParseExprList(parameters);
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[5] as ASTNameAddr;

            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == CONTROL.Inst);
            addr.Accept(gen);
            var en = CONTROL.Inst["EN"];
            var dn = CONTROL.Inst["DN"];
            var er = CONTROL.Inst["ER"];
            Utils.GenClearBit(gen.masm(), en);
            Utils.GenClearBit(gen.masm(), dn);
            Utils.GenClearBit(gen.masm(), er);
            gen.masm().Pop();
        }
    }
}
