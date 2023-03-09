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

    //prescan:The rung-condition-out is set to false.ST no action token
    internal class SATTInstruction : FixedInstruction
    {
        public SATTInstruction(ParamInfoList infos) : base("SATT", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {

        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Utils.ThrowNotImplemented(this.Name);
            gen.masm().BiPush(0);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Sequence Name", SEQUENCE.Inst);
            var param2 = new Tuple<string, IDataType>("Result", DINT.Inst);
            return new List<Tuple<string, IDataType>>() {param1, param2};
        }
    }

    //prescan:The rung-condition-out is set to false.ST no action token
    internal class SDETInstruction : FixedInstruction
    {
        public SDETInstruction(ParamInfoList infos) : base("SDET", infos)
        {

        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Utils.ThrowNotImplemented(this.Name);
            gen.masm().BiPush(0);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Sequence Name", SEQUENCE.Inst);
            return new List<Tuple<string, IDataType>>() {param1,};
        }
    }

    //prescan:The rung-condition-out is set to false.ST no action token
    internal class SCMDInstruction : FixedInstruction
    {
        public SCMDInstruction(ParamInfoList infos) : base("SCMD", infos)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.SCMD.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.SCMD.ParseRLLParameters(parameters);
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Utils.ThrowNotImplemented(this.Name);
            gen.masm().BiPush(0);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Sequence Name", SEQUENCE.Inst);
            var param2 = new Tuple<string, IDataType>("Command", DINT.Inst);
            var param3 = new Tuple<string, IDataType>("Result", DINT.Inst);
            return new List<Tuple<string, IDataType>>() {param1, param2, param3};
        }
    }

    //prescan:The rung-condition-out is set to false.ST no action token
    internal class SCLFInstruction : FixedInstruction
    {
        public SCLFInstruction(ParamInfoList infos) : base("SCLF", infos)
        {

        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Utils.ThrowNotImplemented(this.Name);
            gen.masm().BiPush(0);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Sequence Name", SEQUENCE.Inst);
            var param2 = new Tuple<string, IDataType>("Result", DINT.Inst);
            return new List<Tuple<string, IDataType>>() {param1, param2};
        }
    }

    //prescan:The rung-condition-out is set to false.ST no action token
    internal class SOVRInstruction : FixedInstruction
    {
        public SOVRInstruction(ParamInfoList infos) : base("SOVR", infos)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.SOVR.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.SOVR.ParseRLLParameters(parameters);
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Utils.ThrowNotImplemented(this.Name);
            gen.masm().BiPush(0);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Sequence Name", SEQUENCE.Inst);
            var param2 = new Tuple<string, IDataType>("Command", DINT.Inst);
            var param3 = new Tuple<string, IDataType>("Result", DINT.Inst);
            return new List<Tuple<string, IDataType>>() {param1, param2, param3};
        }
    }

    //prescan:The rung-condition-out is set to false.ST no action token
    internal class SASIInstruction : FixedInstruction
    {
        public SASIInstruction(ParamInfoList infos) : base("SASI", infos)
        {

        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Utils.ThrowNotImplemented(this.Name);
            gen.masm().BiPush(0);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Sequence Name", SEQUENCE.Inst);
            var param2 = new Tuple<string, IDataType>("Sequence Id", STRING.Inst);
            var param3 = new Tuple<string, IDataType>("Result", DINT.Inst);
            return new List<Tuple<string, IDataType>>() {param1, param2, param3};
        }
    }

}