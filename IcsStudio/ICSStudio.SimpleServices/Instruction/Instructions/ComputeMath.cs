using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.PredefinedType;

namespace ICSStudio.SimpleServices.Instruction.Instructions
{
    using System.Diagnostics;
    using ParamInfo = Tuple<Predicate<ASTExpr>, IDataType, ParameterType>;
    using ParamInfoList = List<Tuple<Predicate<ASTExpr>, IDataType, ParameterType>>;

    internal class SQRTInstruction : FixedSTInstruction
    {
        public SQRTInstruction(ParamInfoList infos) : base("SQRT", infos)
        {

        }

        protected override void Op(CodeGenVisitor gen)
        {
            gen.masm().CallName("SQRTF", 1);
        }

        public override IDataType Type(ASTNodeList paramList) => REAL.Inst;
        
        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var paramList = new List<Tuple<string, IDataType>>();
            paramList.Add(new Tuple<string, IDataType>("Source", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst)));
            return paramList;
        }
    }

    internal class FSQRTInstruction : FixedFBDInstruction
    {
        public FSQRTInstruction(ParamInfoList infos) : base("SQRT", infos)
        {

        }

        protected override string InstrName => "FBDSQRT";
    }

    internal class RSQRTInstruction : RLLInstruction
    {
        public RSQRTInstruction() : base("SQRT")
        {

        }
        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            paramList.nodes[1].Accept(gen);
            paramList.nodes[0].Accept(gen);
            var tp = (paramList.nodes[0] as ASTExpr).type;
            MathUtils.SQRT(gen, tp);
            gen.masm().Store((paramList.nodes[1] as ASTNameAddr).ref_type.type, (paramList.nodes[0] as ASTExpr).type);

        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(paramList.Count() == 2, paramList.Count().ToString());
            var tmp = paramList.Accept(checker) as ASTNodeList;
            var res = new ASTNodeList();
            res.AddNode(tmp.nodes[0]);
            res.AddNode(new ASTNameAddr(tmp.nodes[1] as ASTName));
            return res;
        }
    }

    internal class MathUtils
    {
        public static void ABS(CodeGenVisitor gen, IDataType tp)
        {
            if (tp.IsInteger && !(tp is LINT))
            {
                gen.masm().CallName("IABS", 1);
            }
            else if (tp is LINT)
            {
                gen.masm().CallName("LABS", 1);
            }
            else if (tp is REAL)
            {
                gen.masm().CallName("FABS", 1);
            }
            else if (tp is LREAL)
            {
                gen.masm().CallName("DABS", 1);
            }
            else
            {
                Debug.Assert(false, $"{tp}");
            }
        }

        public static void SQRT(CodeGenVisitor gen, IDataType tp)
        {

            if (tp.IsInteger)
            {
                gen.masm().CallName("SQRTI", 1);
            } else if (tp is REAL)
            {
                gen.masm().CallName("SQRTF", 1);

            }
            else
            {
                Debug.Assert(false, $"{tp}");

            }
        }

        public static IDataType GetType(IDataType type)
        {
            if (type is LINT)
            {
                return LINT.Inst;
            }
            else if (type is LREAL)
            {
                return LREAL.Inst;
            }
            else if (type.IsInteger)
            {
                return DINT.Inst;
            }
            else
            {
                Debug.Assert(type is REAL);
                return REAL.Inst;
            }
        }
    }

    internal class ABSInstruction : IXInstruction
    {
        public ABSInstruction()
        {
            Name = "ABS";
        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            return paramList.Accept(checker) as ASTNodeList;
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(paramList.Count() == 1);
            var tp = (paramList.nodes[0] as ASTExpr).type;
            paramList.nodes[0].Accept(gen);
            MathUtils.ABS(gen, tp);

        }

        public override IDataType Type(ASTNodeList paramList)
        {
            if (paramList.Count() != 1)
                return null;
            var param = paramList.nodes[0] as ASTExpr;
            Debug.Assert(param != null);

            return MathUtils.GetType(param.type);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var paramList = new List<Tuple<string, IDataType>>();
            var expectedType=new ExpectType(SINT.Inst,USINT.Inst,INT.Inst,UINT.Inst,DINT.Inst,UDINT.Inst,LINT.Inst,ULINT.Inst,REAL.Inst,LREAL.Inst);
            paramList.Add(new Tuple<string, IDataType>("Source",expectedType));
            return paramList;
        }
    }

    internal class FABSInstruction : FixedFBDInstruction
    {
        public FABSInstruction(ParamInfoList infos) : base("ABS", infos)
        {

        }

        protected override string InstrName => "FBDABS";

    }

    internal class RABSInstruction : RLLInstruction
    {
        public RABSInstruction() : base("ABS")
        {
            Name = "ABS";
        }

        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            paramList.nodes[1].Accept(gen);
            paramList.nodes[0].Accept(gen);
            var tp = (paramList.nodes[0] as ASTExpr).type;
            MathUtils.ABS(gen, tp);
            gen.masm().Store((paramList.nodes[1] as ASTNameAddr).ref_type.type, (paramList.nodes[0] as ASTExpr).type);

        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(paramList.Count() == 2, paramList.Count().ToString());
            var tmp = paramList.Accept(checker) as ASTNodeList;
            var res = new ASTNodeList();
            res.AddNode(tmp.nodes[0]);
            res.AddNode(new ASTNameAddr(tmp.nodes[1] as ASTName));
            return res;
        }

    }

    internal class CPTInstruction : RLLInstruction
    {
        public CPTInstruction(ParamInfoList infos) : base("CPT")
        {

        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(paramList.Count() == 2, paramList.Count().ToString());
            var res = new ASTNodeList();
            var tmp = paramList.Accept(checker) as ASTNodeList;
            res.AddNode(new ASTNameAddr(tmp.nodes[0] as ASTName));
            res.AddNode(tmp.nodes[1]);
            return res;
        }

        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            paramList.nodes[0].Accept(gen);
            paramList.nodes[1].Accept(gen);
            gen.masm().Store((paramList.nodes[0] as ASTNameAddr).ref_type.type, (paramList.nodes[1] as ASTExpr).type);

        }
    }

    internal class BinOPRLLInstruction : RLLInstruction
    {
        public BinOPRLLInstruction(string name) : base(name)
        {

        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            var res = paramList.Accept(checker) as ASTNodeList;
            var lhs = res.nodes[0] as ASTExpr;
            var rhs = res.nodes[1] as ASTExpr;
            var tp = CodeGenVisitor.CommonType(lhs.type, rhs.type);
            res.nodes[0] = new ASTTypeConv(lhs, tp);
            res.nodes[1] = new ASTTypeConv(rhs, tp);
            res.nodes[2] = new ASTNameAddr(res.nodes[2] as ASTName);
            return res;
        }

        public sealed override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            var lhs = paramList.nodes[0] as ASTExpr;
            var rhs = paramList.nodes[1] as ASTExpr;
            var dest = paramList.nodes[2] as ASTNameAddr;
            Debug.Assert(dest != null);
            dest.Accept(gen);
            lhs.Accept(gen);
            rhs.Accept(gen);
            Op(gen, lhs.type);
            Debug.Assert(lhs.type == rhs.type, $"{lhs.type}:{rhs.type}");
            gen.masm().Store(dest.ref_type.type, lhs.type);

        }

        protected virtual void Op(CodeGenVisitor gen, IDataType type)
        {
            throw new NotImplementedException();
        }
    }

    internal class ADDInstruction : BinOPRLLInstruction
    {
        public ADDInstruction() : base("ADD")
        {

        }

        protected override void Op(CodeGenVisitor gen, IDataType type)
        {
            gen.masm().Add(CodeGenVisitor.GetPrimitiveType(type));
        }

        /*
        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL, context.ToString());
            Debug.Assert(paramList.Count() == 3, paramList.Count().ToString());
            Debug.Assert((paramList.nodes[0] as ASTExpr).type == (paramList.nodes[1] as ASTExpr).type, paramList.ToString());
            gen.VisitParamList(paramList);
            var tp = (paramList.nodes[0] as ASTExpr).type;
            gen.masm().Add(tp);
            gen.masm().Store((paramList.nodes[2] as ASTNameAddr).ref_type.type, tp, 0);
            gen.masm().Dup();
        }
        */
    }

    internal class FADDInstruction : FixedFBDInstruction
    {
        public FADDInstruction(ParamInfoList infos) : base("ADD", infos)
        {

        }

        protected override string InstrName => "FBDADD";


    }

    internal class SUBInstruction : BinOPRLLInstruction
    {
        public SUBInstruction() : base("SUB")
        {

        }

        protected override void Op(CodeGenVisitor gen, IDataType type)
        {
            gen.masm().Sub(CodeGenVisitor.GetPrimitiveType(type));
        }
    }

    internal class FSUBInstruction : FixedFBDInstruction
    {
        public FSUBInstruction(ParamInfoList infos) : base("SUB", infos)
        {

        }

        protected override string InstrName => "FBDSUB";
    }

    internal class MULInstruction : BinOPRLLInstruction
    {
        public MULInstruction() : base("MUL")
        {

        }
        /*

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Utils.ThrowNotImplemented(this.Name);
            gen.masm().BiPush(0);
        }
        */

        protected override void Op(CodeGenVisitor gen, IDataType type)
        {
            gen.masm().Mul(CodeGenVisitor.GetPrimitiveType(type));
        }
    }

    internal class FMULInstruction : FixedFBDInstruction
    {
        public FMULInstruction(ParamInfoList infos) : base("MUL", infos)
        {

        }

        protected override string InstrName => "FBDMUL";
    }

    internal class DIVInstruction : BinOPRLLInstruction
    {
        public DIVInstruction() : base("DIV")
        {

        }

        protected override void Op(CodeGenVisitor gen, IDataType type)
        {
            var tp = CodeGenVisitor.GetPrimitiveType(type);
            gen.masm().Div(tp);
        }
    }

    internal class FDIVInstruction : FixedFBDInstruction
    {
        public FDIVInstruction(ParamInfoList infos) : base("DIV", infos)
        {

        }

        protected override string InstrName => "FBDDIV";
    }

    internal class MODInstruction : BinOPRLLInstruction
    {
        public MODInstruction() : base("MOD")
        {

        }

        /*

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Utils.ThrowNotImplemented(this.Name);
            gen.masm().BiPush(0);
        }
        */
        protected override void Op(CodeGenVisitor gen, IDataType type)
        {
            var tp = CodeGenVisitor.GetPrimitiveType(type);
            gen.masm().Mod(tp);
        }


    }

    internal class FMODInstruction : FixedFBDInstruction
    {
        public FMODInstruction(ParamInfoList infos) : base("MOD", infos)
        {

        }

        protected override string InstrName => "FBDMOD";
    }

    internal class SQRInstruction : RLLInstruction
    {
        public SQRInstruction() : base("SQR")
        {

        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(paramList.Count() == 2, paramList.Count().ToString());
            var res = new ASTNodeList();
            var tmp = paramList.Accept(checker) as ASTNodeList;
            res.AddNode(tmp.nodes[0]);
            res.AddNode(new ASTNameAddr(tmp.nodes[1] as ASTName));
            return res;
        }

        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            paramList.nodes[1].Accept(gen);
            paramList.nodes[0].Accept(gen);
            gen.masm().Store((paramList.nodes[1] as ASTNameAddr).ref_type.type, (paramList.nodes[0] as ASTExpr).type);

        }
    }

    internal class FSQRInstruction : FixedFBDInstruction
    {
        public FSQRInstruction(ParamInfoList infos) : base("SQR", infos)
        {

        }

        protected override string InstrName => "FBDSQRT";
    }

    internal class NEGInstruction : FixedRLLInstruction
    {
        public NEGInstruction(ParamInfoList infos) : base("NEG", infos)
        {

        }
        /*
        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Utils.ThrowNotImplemented(this.Name);
            gen.masm().BiPush(0);
        }
        */

        protected override void Op(CodeGenVisitor gen, IDataType type)
        {
            gen.masm().Neg(CodeGenVisitor.GetPrimitiveType(type));
        }
    }

    internal class FNEGInstruction : FixedFBDInstruction
    {
        public FNEGInstruction(ParamInfoList infos) : base("NEG", infos)
        {

        }

        protected override string InstrName => "FBDNEG";
    }

}
