using System;
using System.Collections.Generic;
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

    internal class PSCInstruction : FixedInstruction
    {
        public PSCInstruction(ParamInfoList infos) : base("PSC", infos)
        {

        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Utils.ThrowNotImplemented(this.Name);
            gen.masm().BiPush(0);
        }
    }

    internal class PFLInstruction : FixedInstruction
    {
        public PFLInstruction(ParamInfoList infos) : base("PFL", infos)
        {

        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 =
                new Tuple<string, IDataType>("Source", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            return new List<Tuple<string, IDataType>>() {param1};
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Utils.ThrowNotImplemented(this.Name);
            gen.masm().BiPush(0);
        }
    }

    internal class PCMDInstruction : FixedInstruction
    {
        public PCMDInstruction(ParamInfoList infos) : base("PCMD", infos)
        {

        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Phase Name", PHASE.Inst);
            var param2 = new Tuple<string, IDataType>("Command", DINT.Inst);
            var param3 = new Tuple<string, IDataType>("Result", DINT.Inst);
            return new List<Tuple<string, IDataType>>() {param1, param2, param3};
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Utils.ThrowNotImplemented(this.Name);
            gen.masm().BiPush(0);
        }
    }

    internal class PCLFInstruction : FixedInstruction
    {
        public PCLFInstruction(ParamInfoList infos) : base("PCLF", infos)
        {

        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Utils.ThrowNotImplemented(this.Name);
            gen.masm().BiPush(0);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Phase Name", PHASE.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }
    }

    internal class PXRQInstruction : FixedInstruction
    {
        public PXRQInstruction(ParamInfoList infos) : base("PXRQ", infos)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.PXRQ.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.PXRQ.ParseRLLParameters(parameters);
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Utils.ThrowNotImplemented(this.Name);
            gen.masm().BiPush(0);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 =
                new Tuple<string, IDataType>("Phase Instruction", PHASE_INSTRUCTION.Inst);
            var param2 = new Tuple<string, IDataType>("External Request", DINT.Inst);
            var param3 =
                new Tuple<string, IDataType>("Data Value",
                    new ArrayTypeNormal(DINT.Inst));
            return new List<Tuple<string, IDataType>>() {param1, param2, param3};
        }
    }

    internal class PPDInstruction : FixedInstruction
    {
        public PPDInstruction(ParamInfoList infos) : base("PPD", infos)
        {

        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Utils.ThrowNotImplemented(this.Name);
            gen.masm().BiPush(0);
        }
    }

    internal class PRNPInstruction : FixedInstruction
    {
        public PRNPInstruction(ParamInfoList infos) : base("PRNP", infos)
        {

        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Utils.ThrowNotImplemented(this.Name);
            gen.masm().BiPush(0);
        }
    }

    internal class PATTInstruction : FixedInstruction
    {
        public PATTInstruction(ParamInfoList infos) : base("PATT", infos)
        {

        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Utils.ThrowNotImplemented(this.Name);
            gen.masm().BiPush(0);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Phase Name", PHASE.Inst);
            var param2 = new Tuple<string, IDataType>("Result", DINT.Inst);
            return new List<Tuple<string, IDataType>>() {param1, param2};
        }
    }

    internal class PDETInstruction : FixedInstruction
    {
        public PDETInstruction(ParamInfoList infos) : base("PDET", infos)
        {

        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Utils.ThrowNotImplemented(this.Name);
            gen.masm().BiPush(0);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Phase Name", PHASE.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }
    }

    internal class POVRInstruction : FixedInstruction
    {
        public POVRInstruction(ParamInfoList infos) : base("POVR", infos)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.POVR.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.POVR.ParseRLLParameters(parameters);
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Utils.ThrowNotImplemented(this.Name);
            gen.masm().BiPush(0);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Phase Name", PHASE.Inst);
            var param2 = new Tuple<string, IDataType>("Command", DINT.Inst);
            var param3 = new Tuple<string, IDataType>("Result", DINT.Inst);
            return new List<Tuple<string, IDataType>>() {param1, param2, param3};
        }
    }

}
