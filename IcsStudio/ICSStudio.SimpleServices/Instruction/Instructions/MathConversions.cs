using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.PredefinedType;

namespace ICSStudio.SimpleServices.Instruction.Instructions
{
    using ParamInfo = Tuple<Predicate<ASTExpr>, IDataType, ParameterType>;
    using ParamInfoList = List<Tuple<Predicate<ASTExpr>, IDataType, ParameterType>>;
    internal class DEGInstruction : FixedSTInstruction
    {
        public DEGInstruction(ParamInfoList infos) : base("DEG", infos)
        {

        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            var res = paramList.Accept(checker) as ASTNodeList;
            var source = paramList.nodes[0] as ASTExpr;
            if (source == null)
            {
                throw new TypeCheckerException($"{Name},param 1:Invalid kind of operand or argument.");
            }
            if (source is ASTName)
            {
                var node = source as ASTName;
                if (node.type.IsInteger || node.type.IsReal)
                {
                    if (node.Expr.type is ArrayType) throw new TypeCheckerException($"{Name},param 1:Missing reference to array element.");
                }
                else
                {
                    throw new TypeCheckerException($"{Name},param 1:Invalid data type.Argument must match parameter data type.");
                }
            }
            else
            {
                if (!(source.type.IsInteger || source.type.IsReal))
                    throw new TypeCheckerException($"{Name},param 1:Invalid data type.Argument must match parameter data type.");
            }
            return PrepareFixedParameters(res);
        }

        /*
        public sealed override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.ST, context.ToString());
            Debug.Assert(paramList.Count() == 1, paramList.ToString());
            Debug.Assert((paramList.nodes[0] as ASTExpr).type is REAL, paramList.nodes[0].ToString());
            paramList.Accept(gen);
            gen.masm().CLoadFloat(57.29578f);
            gen.masm().FMul();
        }
        */

        protected override void Op(CodeGenVisitor gen)
        {
            gen.masm().CLoadFloat(57.29578f);
            gen.masm().FMul();
        }

        public override IDataType Type(ASTNodeList paramList) => REAL.Inst;


        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var paramList = new List<Tuple<string, IDataType>>();
            paramList.Add(new Tuple<string, IDataType>("Source", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst)));
            return paramList;
        }
    }
    internal class RDEGInstruction : FixedRLLInstruction
    {
        public RDEGInstruction(ParamInfoList infos) : base("DEG", infos)
        {

        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            var res = paramList.Accept(checker) as ASTNodeList;
            var source = paramList.nodes[0] as ASTName;
            if (source == null)
            {
                if (paramList.nodes[0] is ASTInteger||paramList.nodes[0] is ASTFloat)
                {

                }
                else
                {
                    throw new TypeCheckerException($"{Name},param 1:Invalid kind of operand or argument.");
                }
            }
            else
            {
                if (source.type.IsInteger || source.type.IsReal)
                {
                    if (source.Expr.type is ArrayType) throw new TypeCheckerException($"{Name},param 1:Missing reference to array element.");
                }
                else
                {
                    throw new TypeCheckerException($"{Name},param 1:Invalid data type.Argument must match parameter data type.");
                }
            }
           

            var dest = paramList.nodes[1] as ASTName;
            if (dest == null)
            {
                throw new TypeCheckerException($"{Name},param 2:Invalid kind of operand or argument.");
            }

            if (dest.type.IsInteger || dest.type.IsReal)
            {
                if (dest.Expr.type is ArrayType) throw new TypeCheckerException($"{Name},param 2:Missing reference to array element.");
            }
            else
            {
                throw new TypeCheckerException($"{Name},param 2:Invalid data type.Argument must match parameter data type.");
            }
            return PrepareFixedParameters(res);
        }

        protected override void Op(CodeGenVisitor gen, IDataType tp)
        {
            gen.masm().CLoadFloat(57.29578f);
            gen.masm().FMul();
        }

    }
    internal class FDEGInstruction : FixedFBDInstruction
    {
        public FDEGInstruction(ParamInfoList infos) : base("DEG", infos)
        {

        }

        protected override string InstrName => "FBDDEG";

    }
    internal class RADInstruction : FixedSTInstruction
    {
        public RADInstruction(ParamInfoList infos) : base("RAD", infos)
        {

        }

        protected override void Op(CodeGenVisitor gen)
        {
            gen.masm().CallName("RADF", 1);
        }

        public override IDataType Type(ASTNodeList paramList) => REAL.Inst;
        
        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var paramList = new List<Tuple<string, IDataType>>();
            paramList.Add(new Tuple<string, IDataType>("Source", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst)));
            return paramList;
        }
    }
    internal class RRADInstruction : FixedRLLInstruction
    {
        public RRADInstruction(ParamInfoList infos) : base("RAD", infos)
        {

        }

        protected override void Op(CodeGenVisitor gen, IDataType type)
        {
            gen.masm().CallName("RADF", 1);
        }
    }
    internal class FRADInstruction : FixedFBDInstruction
    {
        public FRADInstruction(ParamInfoList infos) : base("RAD", infos)
        {

        }

        protected override string InstrName => "FBDRAD";
    }
    //prescan:The rung is set to false.
    internal class TRUNCInstruction : FixedSTInstruction
    {
        public TRUNCInstruction(ParamInfoList infos) : base("TRUNC", infos)
        {

        }
        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
        }

        protected override void Op(CodeGenVisitor gen)
        {
            gen.masm().CallName("TRUNC", 1);
        }


        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var paramList = new List<Tuple<string, IDataType>>();
            paramList.Add(new Tuple<string, IDataType>("Source", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst)));
            return paramList;
        }
    }
    internal class TODInstruction : FixedRLLInstruction
    {
        public TODInstruction(ParamInfoList infos) : base("TOD", infos)
        {

        }

        protected override void Op(CodeGenVisitor gen, IDataType type)
        {
            gen.masm().CallName("TOD", 1);
        }
    }
    internal class FTODInstruction : FixedFBDInstruction
    {
        public FTODInstruction(ParamInfoList infos) : base("TOD", infos)
        {

        }

        protected override string InstrName => "FBDTOD";
    }
    internal class FRDInstruction : FixedRLLInstruction
    {
        public FRDInstruction(ParamInfoList infos) : base("FRD", infos)
        {

        }
        protected override void Op(CodeGenVisitor gen, IDataType tp)
        {
            gen.masm().CallName("FRD", 1);
        }
    }
    internal class FFRDInstruction : FixedFBDInstruction
    {
        public FFRDInstruction(ParamInfoList infos) : base("FRD", infos)
        {

        }

        protected override string InstrName => "FBDFRD";
    }
    //prescan:The rung is set to false.
    internal class TRNInstruction : FixedRLLInstruction
    {
        public TRNInstruction(ParamInfoList infos) : base("TRN", infos)
        {

        }

        protected override void Op(CodeGenVisitor gen, IDataType type)
        {
            gen.masm().CallName("TRUNC", 1);
        }

        protected override IDataType ReturnType(IDataType firstDataTYpe)
        {
            return DINT.Inst;
        }
    }

    //prescan:The rung is set to false.
    internal class FTRNInstruction : FixedFBDInstruction
    {
        public FTRNInstruction(ParamInfoList infos) : base("TRN", infos)
        {

        }

        protected override string InstrName => "FBDTRUNC";
    }

}
