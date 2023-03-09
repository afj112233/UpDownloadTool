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

    //prescan:All alarm conditions are acknowledged.
    //All operator requests are cleared
    //All timestamps are cleared   
    internal class ALMDInstruction : FixedInstruction
    {
        public ALMDInstruction(ParamInfoList infos) : base("ALMD", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == ALARM_DIGITAL.Inst);
            addr.Accept(gen);
            var enableIn = ALARM_DIGITAL.Inst["EnableIn"];
            var enableOut = ALARM_DIGITAL.Inst["EnableOut"];
            var inAlarm = ALARM_DIGITAL.Inst["InAlarm"];
            var shelved = ALARM_DIGITAL.Inst["Shelved"];
            var acked = ALARM_DIGITAL.Inst["Acked"];
            Utils.GenSetBit(gen.masm(), enableIn);
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), inAlarm);
            Utils.GenClearBit(gen.masm(), shelved);
            Utils.GenSetBit(gen.masm(), acked);
            Utils.Store(gen, ALARM_DIGITAL.Inst["ProgAckN_1"], paramList.nodes[2] as ASTExpr);
            Utils.Store(gen, ALARM_DIGITAL.Inst["ProgResetN_1"], paramList.nodes[3] as ASTExpr);
            gen.masm().Pop();
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.ST);

            paramList.nodes[0].Accept(gen);
            gen.masm().Dup();

            Utils.Store(gen, ALARM_DIGITAL.Inst["In"], paramList.nodes[1] as ASTExpr);
            Utils.Store(gen, ALARM_DIGITAL.Inst["ProgAck"], paramList.nodes[2] as ASTExpr);
            Utils.Store(gen, ALARM_DIGITAL.Inst["ProgReset"], paramList.nodes[3] as ASTExpr);
            Utils.Store(gen, ALARM_DIGITAL.Inst["ProgDisable"], paramList.nodes[4] as ASTExpr);
            Utils.Store(gen, ALARM_DIGITAL.Inst["ProgEnable"], paramList.nodes[5] as ASTExpr);

            gen.masm().CallName("ALMD", 1);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("ALMD", ALARM_DIGITAL.Inst);
            var param2 = new Tuple<string, IDataType>("In", BOOL.Inst);
            var param3 = new Tuple<string, IDataType>("ProgAck", BOOL.Inst);
            var param4 = new Tuple<string, IDataType>("ProgReset", BOOL.Inst);
            var param5 = new Tuple<string, IDataType>("ProgDisable", BOOL.Inst);
            var param6 = new Tuple<string, IDataType>("ProgEnable", BOOL.Inst);
            return new List<Tuple<string, IDataType>>() {param1, param2, param3, param4, param5, param6};
        }
    }

    // prescan :see in ST
    internal class RALMDInstruction : FixedInstruction
    {
        public RALMDInstruction(ParamInfoList infos) : base("ALMD", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == ALARM_DIGITAL.Inst);
            addr.Accept(gen);
            var enableOut = ALARM_DIGITAL.Inst["EnableOut"];
            var inAlarm = ALARM_DIGITAL.Inst["InAlarm"];
            var shelved = ALARM_DIGITAL.Inst["Shelved"];
            var acked = ALARM_DIGITAL.Inst["Acked"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), inAlarm);
            Utils.GenClearBit(gen.masm(), shelved);
            Utils.GenSetBit(gen.masm(), acked);
            gen.masm().Pop();
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL);
            paramList.nodes[0].Accept(gen);

            Utils.Store(gen, ALARM_DIGITAL.Inst["ProgAck"], paramList.nodes[1] as ASTExpr);
            Utils.Store(gen, ALARM_DIGITAL.Inst["ProgReset"], paramList.nodes[2] as ASTExpr);
            Utils.Store(gen, ALARM_DIGITAL.Inst["ProgDisable"], paramList.nodes[3] as ASTExpr);
            Utils.Store(gen, ALARM_DIGITAL.Inst["ProgEnable"], paramList.nodes[4] as ASTExpr);

            gen.masm().CallName("ALMD", 1);
            gen.masm().Pop();
            gen.masm().Dup();
        }
    }

    //prescan:All operator requests are cleared
    //All timestamps are cleared
    internal class FALMDInstruction : FixedInstruction
    {
        public FALMDInstruction(ParamInfoList infos) : base("ALMD", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == ALARM_DIGITAL.Inst);
            addr.Accept(gen);
            var enableOut = ALARM_DIGITAL.Inst["EnableOut"];
            var inAlarm = ALARM_DIGITAL.Inst["InAlarm"];
            var shelved = ALARM_DIGITAL.Inst["Shelved"];
            var acked = ALARM_DIGITAL.Inst["Acked"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), inAlarm);
            Utils.GenClearBit(gen.masm(), shelved);
            Utils.GenSetBit(gen.masm(), acked);
            gen.masm().Pop();
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.FBD);
            paramList.nodes[0].Accept(gen);
            gen.masm().CallName("ALMD", 1);
            //gen.masm().Pop();

        }
    }
    //prescan:All of the ALMA structure parameters are cleared

    //All alarm conditions are acknowledged.

    //All operator requests are cleared

    //All timestamps are cleared

    //All delivery flags are cleared.
    internal class ALMAInstruction : FixedInstruction
    {
        public ALMAInstruction(ParamInfoList infos) : base("ALMA", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == ALARM_ANALOG.Inst);
            addr.Accept(gen);
            var enableIn = ALARM_ANALOG.Inst["EnableIn"];
            var enableOut = ALARM_ANALOG.Inst["EnableOut"];
            Utils.GenSetBit(gen.masm(),enableIn);
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.Store(gen, ALARM_ANALOG.Inst["InN_1"], paramList.nodes[1] as ASTExpr);
            Utils.Store(gen, ALARM_ANALOG.Inst["ProgAckAllN_1"], paramList.nodes[2] as ASTExpr);
            gen.masm().Pop();
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

            paramList.nodes[0].Accept(gen);
            
            Utils.Store(gen, ALARM_ANALOG.Inst["In"], paramList.nodes[1] as ASTExpr);
            Utils.Store(gen, ALARM_ANALOG.Inst["ProgAckAll"], paramList.nodes[2] as ASTExpr);
            Utils.Store(gen, ALARM_ANALOG.Inst["ProgDisable"], paramList.nodes[3] as ASTExpr);
            Utils.Store(gen, ALARM_ANALOG.Inst["ProgEnable"], paramList.nodes[4] as ASTExpr);

            gen.masm().CallName("ALMA", 1);

            if (context == Context.RLL)
            {
                gen.masm().Bind(label);
                gen.masm().Dup();
            }
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("ALMA", ALARM_ANALOG.Inst);
            var param2 = new Tuple<string, IDataType>("In", DINT.Inst);
            var param3 = new Tuple<string, IDataType>("ProgAckAll", BOOL.Inst);
            var param4 = new Tuple<string, IDataType>("ProgDisable", BOOL.Inst);
            var param5 = new Tuple<string, IDataType>("ProgEnable", BOOL.Inst);
            return new List<Tuple<string, IDataType>>() {param1, param2, param3, param4, param5};
        }
    }

    //prescan:see in ST
    internal class FALMAInstruction : FixedInstruction
    {
        public FALMAInstruction(ParamInfoList infos) : base("ALMA", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == ALARM_ANALOG.Inst);
            addr.Accept(gen);
            var enableOut = ALARM_ANALOG.Inst["EnableOut"];
            Utils.GenClearBit(gen.masm(), enableOut);
            gen.masm().Pop();
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {

            paramList.nodes[0].Accept(gen);
            gen.masm().CallName("ALMA", 1);

        }
    }
}
