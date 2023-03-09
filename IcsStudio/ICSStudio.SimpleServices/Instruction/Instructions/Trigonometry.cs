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
    internal class SINInstruction : FixedSTInstruction
    {
        public SINInstruction(ParamInfoList infos) : base("SIN", infos)
        {

        }

        protected override void Op(CodeGenVisitor gen)
        {
            gen.masm().CallName("SINF", 1);
        }

        public override IDataType Type(ASTNodeList paramList) => REAL.Inst;

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var paramList = new List<Tuple<string, IDataType>>();
            var expectedType = new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst);
            paramList.Add(new Tuple<string, IDataType>("Source", expectedType));
            return paramList;
        }
    }
    internal class RSINInstruction : FixedRLLInstruction
    {
        public RSINInstruction(ParamInfoList infos) : base("SIN", infos)
        {

        }

        protected override void Op(CodeGenVisitor gen, IDataType type)
        {
            gen.masm().CallName("SINF", 1);
        }
    }

    internal class FSINInstruction : FixedFBDInstruction
    {
        public FSINInstruction(ParamInfoList infos) : base("SIN", infos)
        {

        }

        protected override string InstrName => "FBDSIN";
    }
    internal class COSInstruction : FixedSTInstruction
    {
        public COSInstruction(ParamInfoList infos) : base("COS", infos)
        {

        }

        protected override void Op(CodeGenVisitor gen)
        {
            gen.masm().CallName("COSF", 1);
        }

        public override IDataType Type(ASTNodeList paramList) => REAL.Inst;

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var paramList = new List<Tuple<string, IDataType>>();
            var expectedType=new ExpectType(SINT.Inst,INT.Inst,DINT.Inst,REAL.Inst);
            paramList.Add(new Tuple<string, IDataType>("Source", expectedType));
            return paramList;
        }
    }
    internal class RCOSInstruction : FixedRLLInstruction
    {
        public RCOSInstruction(ParamInfoList infos) : base("COS", infos)
        {

        }

        protected override void Op(CodeGenVisitor gen, IDataType tp)
        {
            gen.masm().CallName("COSF", 1);
        }

    }

    internal class FCOSInstruction : FixedFBDInstruction
    {
        public FCOSInstruction(ParamInfoList infos) : base("COS", infos)
        {

        }

        protected override string InstrName => "FBDCOS";
    }

    internal class TANInstruction : FixedSTInstruction
    {
        public TANInstruction(ParamInfoList infos) : base("TAN", infos)
        {

        }

        protected override void Op(CodeGenVisitor gen)
        {
            gen.masm().CallName("TANF", 1);
        }

        public override IDataType Type(ASTNodeList paramList) => REAL.Inst;

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var paramList = new List<Tuple<string, IDataType>>();
            var expectedType = new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst);
            paramList.Add(new Tuple<string, IDataType>("Source", expectedType));
            return paramList;
        }
    }
    internal class RTANInstruction : FixedRLLInstruction
    {
        public RTANInstruction(ParamInfoList infos) : base("TAN", infos)
        {

        }

        protected override void Op(CodeGenVisitor gen, IDataType type)
        {
            gen.masm().CallName("TANF", 1);
        }
    }

    internal class FTANInstruction : FixedFBDInstruction
    {
        public FTANInstruction(ParamInfoList infos) : base("TAN", infos)
        {

        }

        protected override string InstrName => "FBDTAN";

    }
    internal class ASINInstruction : FixedSTInstruction
    {
        public ASINInstruction(ParamInfoList infos) : base("ASIN", infos)
        {

        }

        protected override void Op(CodeGenVisitor gen)
        {
            gen.masm().CallName("ASINF", 1);
        }

        public override IDataType Type(ASTNodeList paramList) => REAL.Inst;

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var paramList = new List<Tuple<string, IDataType>>();
            var expectedType = new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst);
            paramList.Add(new Tuple<string, IDataType>("Source", expectedType));
            return paramList;
        }
    }
    internal class RASINInstruction : FixedRLLInstruction
    {
        public RASINInstruction(ParamInfoList infos) : base("ASN", infos)
        {

        }

        protected sealed override void Op(CodeGenVisitor gen, IDataType tp)
        {
            gen.masm().CallName("ASINF", 1);
        }
    }
    internal class FASINInstruction : FixedFBDInstruction
    {
        public FASINInstruction(ParamInfoList infos) : base("ASIN", infos)
        {

        }

        protected override string InstrName => "FBDASIN";
    }

    internal class ACOSInstruction : FixedSTInstruction
    {
        public ACOSInstruction(ParamInfoList infos) : base("ACOS", infos)
        {

        }

        protected override void Op(CodeGenVisitor gen)
        {
            gen.masm().CallName("ACOSF", 1);
        }


        public override IDataType Type(ASTNodeList paramList) => REAL.Inst;

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var paramList = new List<Tuple<string, IDataType>>();
            var expectedType = new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst);
            paramList.Add(new Tuple<string, IDataType>("Source", expectedType));
            return paramList;
        }
    }
    internal class RACOSInstruction : FixedRLLInstruction
    {
        public RACOSInstruction(ParamInfoList infos) : base("ACS", infos)
        {

        }

        /*
        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL, context.ToString());
            Debug.Assert(paramList.Count() == 2, paramList.ToString());
            paramList.nodes[0].Accept(gen);
            gen.masm().CallName("ACOSF", 1);

            var dst_tp = (paramList.nodes[1] as ASTNameAddr).ref_type.type;
            paramList.nodes[1].Accept(gen);
            gen.masm().Store(dst_tp, REAL.Inst, 0);
            gen.masm().Dup();
        }
        */

        protected override void Op(CodeGenVisitor gen, IDataType tp)
        {
            gen.masm().CallName("ACOSF", 1);
        }
    }


    internal class FACOSInstruction : FixedFBDInstruction
    {
        public FACOSInstruction(ParamInfoList infos) : base("ACOS", infos)
        {

        }

        protected override string InstrName => "FBDACOS";
    }
    internal class ATANInstruction : FixedSTInstruction
    {
        public ATANInstruction(ParamInfoList infos) : base("ATAN", infos)
        {

        }

        protected override void Op(CodeGenVisitor gen)
        {
            gen.masm().CallName("ATANF", 1);
        }

        public override IDataType Type(ASTNodeList paramList) => REAL.Inst;

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var paramList = new List<Tuple<string, IDataType>>();
            var expectedType = new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst);
            paramList.Add(new Tuple<string, IDataType>("Source", expectedType));
            return paramList;
        }
    }
    internal class RATANInstruction : FixedRLLInstruction
    {
        public RATANInstruction(ParamInfoList infos) : base("ATN", infos)
        {

        }

        protected override void Op(CodeGenVisitor gen, IDataType tp)
        {
            gen.masm().CallName("ATANF", 1);
        }

    }
    internal class FATANInstruction : FixedFBDInstruction
    {
        public FATANInstruction(ParamInfoList infos) : base("ATAN", infos)
        {

        }

        protected override string InstrName => "FBDATAN";

    }
}
