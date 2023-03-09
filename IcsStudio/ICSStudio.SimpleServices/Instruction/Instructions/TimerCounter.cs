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

    internal class TONRInstruction : FixedFBDInstruction
    {
        public TONRInstruction(ParamInfoList infos) : base("TONR", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == FBD_TIMER.Inst);
            addr.Accept(gen);
            var enableOut = FBD_TIMER.Inst["EnableOut"];
            var enableIn = FBD_TIMER.Inst["EnableIn"];
            var firstScan = FBD_TIMER.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("TONRTag", FBD_TIMER.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => FBD_TIMER.Inst["EnableIn"];
        protected override string InstrName => "TONR";
    }

    internal class TOFRInstruction : FixedFBDInstruction
    {
        public TOFRInstruction(ParamInfoList infos) : base("TOFR", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == FBD_TIMER.Inst);
            addr.Accept(gen);
            var enableOut = FBD_TIMER.Inst["EnableOut"];
            var enableIn = FBD_TIMER.Inst["EnableIn"];
            var firstScan = FBD_TIMER.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("TOFRTag", FBD_TIMER.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => FBD_TIMER.Inst["EnableIn"];
        protected override string InstrName => "TOFR";
    }

    internal class RTORInstruction : FixedFBDInstruction
    {
        public RTORInstruction(ParamInfoList infos) : base("RTOR", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == FBD_TIMER.Inst);
            addr.Accept(gen);
            var enableOut = FBD_TIMER.Inst["EnableOut"];
            var enableIn = FBD_TIMER.Inst["EnableIn"];
            var firstScan = FBD_TIMER.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("RTORTag", FBD_TIMER.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => FBD_TIMER.Inst["EnableIn"];
        protected override string InstrName => "RTOR";
    }

    internal class CTUDInstruction : FixedFBDInstruction
    {
        public CTUDInstruction(ParamInfoList infos) : base("CTUD", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == FBD_COUNTER.Inst);
            addr.Accept(gen);
            var enableOut = FBD_COUNTER.Inst["EnableOut"];
            var enableIn = FBD_COUNTER.Inst["EnableIn"];
            var firstScan = FBD_COUNTER.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("CTUDTag", FBD_COUNTER.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => FBD_COUNTER.Inst["EnableIn"];
        protected override string InstrName => "CTUD";
    }

    internal class TONInstruction : FixedInstruction
    {
        public TONInstruction(ParamInfoList infos) : base("TON", infos)
        {

        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            parameters.RemoveRange(parameters.Count - 2, 2);
            return Utils.ParseExprList(parameters);
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == TIMER.Inst);
            addr.Accept(gen);
            Utils.GenClearBit(gen.masm(), TIMER.Inst["EN"]);
            Utils.GenClearBit(gen.masm(), TIMER.Inst["TT"]);
            Utils.GenClearBit(gen.masm(), TIMER.Inst["DN"]);
            Utils.GenClearInt(gen.masm(), TIMER.Inst["ACC"]);
            gen.masm().Pop();
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(paramList.Count() == 1);

            gen.masm().Dup();
            paramList.nodes[0].Accept(gen);

            gen.masm().CallName("TON", 2);

            //Utils.S
            /*
            var else_label = new MacroAssembler.Label();
            var exit_label = new MacroAssembler.Label();
            gen.masm().Dup();
            gen.masm().JeqL(else_label);
            
            gen.masm().JmpL(exit_label);
            gen.masm().Bind(else_label);

            gen.masm().Bind(exit_label);
            */
        }
    }

    internal class TOFInstruction : FixedInstruction
    {
        public TOFInstruction(ParamInfoList infos) : base("TOF", infos)
        {

        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            parameters.RemoveRange(parameters.Count - 2, 2);
            return Utils.ParseExprList(parameters);
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == TIMER.Inst);
            addr.Accept(gen);

            Utils.GenClearBit(gen.masm(), TIMER.Inst["EN"]);
            Utils.GenClearBit(gen.masm(), TIMER.Inst["TT"]);
            Utils.GenClearBit(gen.masm(), TIMER.Inst["DN"]);
            Utils.GenCopAtoB(gen.masm(), TIMER.Inst["PRE"], TIMER.Inst["ACC"]);
            gen.masm().Pop();
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(paramList.Count() == 1);

            gen.masm().Dup();
            paramList.nodes[0].Accept(gen);
            gen.masm().CallName("TOF", 2);

            //Utils.S
            /*
            var else_label = new MacroAssembler.Label();
            var exit_label = new MacroAssembler.Label();
            gen.masm().Dup();
            gen.masm().JeqL(else_label);
            
            gen.masm().JmpL(exit_label);
            gen.masm().Bind(else_label);

            gen.masm().Bind(exit_label);
            */
        }
    }

    internal class RTOInstruction : FixedInstruction
    {
        public RTOInstruction(ParamInfoList infos) : base("RTO",infos)
        {

        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            parameters.RemoveRange(parameters.Count - 2, 2);
            return Utils.ParseExprList(parameters);
        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            var res = paramList;
            res.nodes[0]=new ASTNameAddr(res.nodes[0].Accept(checker) as ASTName);
            return res;
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == TIMER.Inst);
            addr.Accept(gen);
            Utils.GenClearBit(gen.masm(), TIMER.Inst["EN"]);
            Utils.GenClearBit(gen.masm(), TIMER.Inst["TT"]);
            gen.masm().Pop();
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            gen.masm().Dup();
            gen.VisitParamList(paramList);
            //paramList.nodes[0].Accept(gen);
            gen.masm().CallName(this.Name, 2);
        }
    }

    internal class CTUInstruction : FixedInstruction
    {
        public CTUInstruction(ParamInfoList infos) : base("CTU", infos)
        {

        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            parameters.RemoveRange(parameters.Count - 2, 2);
            return Utils.ParseExprList(parameters);
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == COUNTER.Inst);
            addr.Accept(gen);
            Utils.GenSetBit(gen.masm(), COUNTER.Inst["CU"]);
            gen.masm().Pop();
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(paramList.Count() == 1, paramList.ToString());
            gen.masm().Dup();
            paramList.nodes[0].Accept(gen);
            gen.masm().CallName("CTU", 2);
        }
    }

    internal class CTDInstruction : FixedInstruction
    {
        public CTDInstruction(ParamInfoList infos) : base("CTD", infos)
        {
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            parameters.RemoveRange(parameters.Count - 2, 2);
            return Utils.ParseExprList(parameters);
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == COUNTER.Inst);
            addr.Accept(gen);
            Utils.GenSetBit(gen.masm(), COUNTER.Inst["CD"]);
            gen.masm().Pop();
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(paramList.Count() == 1, paramList.ToString());
            gen.masm().Dup();
            paramList.nodes[0].Accept(gen);
            gen.masm().CallName("CTD", 2);
        }
    }

    internal class RESInstruction : RLLInstruction
    {
        public RESInstruction() : base("RES")
        {

        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL);

            paramList.Accept(checker);
            Debug.Assert(paramList.Count() == 1, paramList.Count().ToString());

            var res = new ASTNodeList();
            res.AddNode(new ASTNameAddr(paramList.nodes[0] as ASTName));
            return res;
        }

        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL);
            var type = (paramList.nodes[0] as ASTNameAddr).ref_type.type;
            paramList.Accept(gen);
            if (type is TIMER)
            {
                Utils.GenClearInt(gen.masm(), TIMER.Inst["ACC"]);

                Utils.GenClearBit(gen.masm(), TIMER.Inst["EN"]);
                Utils.GenClearBit(gen.masm(), TIMER.Inst["TT"]);
                Utils.GenClearBit(gen.masm(), TIMER.Inst["DN"]);

            }
            else if (type is COUNTER)
            {
                Utils.GenClearInt(gen.masm(), COUNTER.Inst["ACC"]);

                Utils.GenClearBit(gen.masm(), COUNTER.Inst["CU"]);
                Utils.GenClearBit(gen.masm(), COUNTER.Inst["CD"]);
                Utils.GenClearBit(gen.masm(), COUNTER.Inst["DN"]);
                Utils.GenClearBit(gen.masm(), COUNTER.Inst["OV"]);
                Utils.GenClearBit(gen.masm(), COUNTER.Inst["UN"]);
            }
            else if (type is CONTROL)
            {
                Utils.GenClearInt(gen.masm(), CONTROL.Inst["POS"]);

                Utils.GenClearBit(gen.masm(), CONTROL.Inst["EN"]);
                Utils.GenClearBit(gen.masm(), CONTROL.Inst["EU"]);
                Utils.GenClearBit(gen.masm(), CONTROL.Inst["DN"]);
                Utils.GenClearBit(gen.masm(), CONTROL.Inst["EM"]);
                Utils.GenClearBit(gen.masm(), CONTROL.Inst["ER"]);
                Utils.GenClearBit(gen.masm(), CONTROL.Inst["UL"]);
                Utils.GenClearBit(gen.masm(), CONTROL.Inst["IN"]);
                Utils.GenClearBit(gen.masm(), CONTROL.Inst["FD"]);

            }
            else
            {
                Debug.Assert(false);
            }

            gen.masm().Pop();
        }
    }

}
