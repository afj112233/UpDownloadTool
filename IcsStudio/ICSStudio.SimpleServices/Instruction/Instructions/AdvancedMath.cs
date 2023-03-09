using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.PredefinedType;

namespace ICSStudio.SimpleServices.Instruction.Instructions
{
    using ParamInfo = Tuple<Predicate<ASTExpr>, IDataType, ParameterType>;
    using ParamInfoList = List<Tuple<Predicate<ASTExpr>, IDataType, ParameterType>>;
    internal class LNInstruction : FixedSTInstruction
    {
        public LNInstruction(ParamInfoList infos) : base("LN", infos)
        {

        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            var tmp = paramList.Accept(checker) as ASTNodeList;
            if (context == Context.ST)
            {
                var source = tmp.nodes[0];
                var name = source as ASTName;
                if (name != null)
                {
                    if (!(name.Expr.type is DINT || name.Expr.type is INT || name.Expr.type is SINT ||
                          name.Expr.type is REAL))
                    {
                        throw new TypeCheckerException($"{Name},param 1:Invalid data type.Argument must match parameter data type.");
                    }
                }

            }

            if (context == Context.RLL)
            {

            }

            return PrepareFixedParameters(tmp);
        }

        /*
        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.ST);
            paramList.nodes[0].Accept(gen);
            gen.masm().CallName("LN", 1);
        }
        */
        protected override void Op(CodeGenVisitor gen)
        {
            gen.masm().CallName("LN", 1);
        }

        public override IDataType Type(ASTNodeList paramList) => REAL.Inst;


        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var paramList = new List<Tuple<string, IDataType>>();
            paramList.Add(new Tuple<string, IDataType>("Source", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst)));
            return paramList;
        }
    }
    internal class RLNInstruction : FixedRLLInstruction
    {
        public RLNInstruction(ParamInfoList infos) : base("LN", infos)
        {
        }

        /*
        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL);
            gen.masm().Dup();
            var label = new MacroAssembler.Label();
            gen.masm().JeqL(label);

            paramList.nodes[0].Accept(gen);
            gen.masm().CallName("LN", 1);
            
            var addr = paramList.nodes[1] as ASTNameAddr;
            addr.Accept(gen);
            gen.masm().Store((addr.type as RefType).type, REAL.Inst, 0);

            gen.masm().Bind(label);
            gen.masm().Dup();
        }
        */

        protected override void Op(CodeGenVisitor gen, IDataType tp)
        {
            gen.masm().CallName("LN", 1);
        }
    }
   
    internal class FLNInstruction : FixedFBDInstruction
    {
        public FLNInstruction(ParamInfoList infos) : base("LN", infos)
        {
        }
        /*L
       public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
       {
           Debug.Assert(context == Context.FBD);
           gen.masm().CallName("$LN", 1);
           gen.masm().Pop();
       }
       */

        protected override string InstrName => "FBDLN";
    }
    internal class LOGInstruction : FixedSTInstruction
    {
        public LOGInstruction(ParamInfoList infos) : base("LOG", infos)
        {

        }

        /*
        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.ST);
            paramList.nodes[0].Accept(gen);
            gen.masm().CallName("LOG", 1);
        }
        */

        protected override void Op(CodeGenVisitor gen)
        {
            gen.masm().CallName("LOGF", 1);
        }

        public override IDataType Type(ASTNodeList paramList) => REAL.Inst;

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var paramList = new List<Tuple<string, IDataType>>();
            paramList.Add(new Tuple<string, IDataType>("Source", new ExpectType(SINT.Inst,INT.Inst,DINT.Inst,REAL.Inst)));
            return paramList;
        }
    }
    internal class FLOGInstruction : FixedFBDInstruction
    {
        public FLOGInstruction(ParamInfoList infos) : base("LOG", infos)
        {

        }

        /*
        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.FBD);
            gen.masm().CallName("$LOG", 1);
            gen.masm().Pop();
        }
        */
        protected override string InstrName => "FBDLOG";

    }
    internal class RLOGInstruction : FixedRLLInstruction
    {
        public RLOGInstruction(ParamInfoList infos) : base("LOGF", infos)
        {

        }

        /*
        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL);
            gen.masm().Dup();
            var label = new MacroAssembler.Label();
            gen.masm().JeqL(label);

            paramList.nodes[0].Accept(gen);
            gen.masm().CallName("LOG", 1);

            var addr = paramList.nodes[1] as ASTNameAddr;
            addr.Accept(gen);
            gen.masm().Store((addr.type as RefType).type, REAL.Inst, 0);

            gen.masm().Bind(label);
            gen.masm().Dup();
        }
        */

        protected override void Op(CodeGenVisitor gen, IDataType tp)
        {
            gen.masm().CallName("LOGF", 1);
        }
    }

    internal class XPYInstruction : FixedRLLInstruction
    {
        public XPYInstruction(ParamInfoList infos) : base("XPY", infos)
        {

        }

        /*
        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL);
            gen.masm().Dup();
            var label = new MacroAssembler.Label();
            gen.masm().JeqL(label);

            paramList.nodes[0].Accept(gen);
            paramList.nodes[1].Accept(gen);
            gen.masm().CallName("XPY", 2);

            var addr = paramList.nodes[2] as ASTNameAddr;
            addr.Accept(gen);
            gen.masm().Store((addr.type as RefType).type, REAL.Inst, 0);

            gen.masm().Bind(label);
            gen.masm().Dup();
        }
        */

        protected override void Op(CodeGenVisitor gen, IDataType type)
        {
            gen.masm().CallName("POW", 2);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var paramList = new List<Tuple<string, IDataType>>();
            paramList.Add(new Tuple<string, IDataType>("Source X", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst)));
            paramList.Add(new Tuple<string, IDataType>("Source Y", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst)));
            paramList.Add(new Tuple<string, IDataType>("Dest", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst)));
            return paramList;
        }

    }
    internal class FXPYInstruction : FixedFBDInstruction
    {
        public FXPYInstruction(ParamInfoList infos) : base("XPY", infos)
        {

        }

        protected override IDataTypeMember EnableInMember => FBD_MATH.Inst["EnableIn"];
        protected override string InstrName => "FBDXPY";
    }

}
